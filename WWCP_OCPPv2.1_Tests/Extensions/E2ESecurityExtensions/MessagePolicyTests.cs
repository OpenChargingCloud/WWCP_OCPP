/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// The policy table: who must have signed a request before its handler runs
    /// (white paper "Signed OCPP Messages and Signer Roles", chapter 5, and
    /// phase 3 of its chapter 7).
    /// </summary>
    /// <remarks>
    /// Each test signs a request as a CSMS's adapter does, takes what goes on
    /// the wire, parses it as the station does and verifies it as the station's
    /// adapter does. Nothing is sent over a network. The roles are the ones
    /// section 5.3 names, with the permissions it gives them.
    /// </remarks>
    [TestFixture]
    public class MessagePolicyTests
    {

        #region Data

        private static readonly NetworkingNode_Id  cs01           = NetworkingNode_Id.Parse("cs01");
        private static readonly SourceRouting      toTheStation   = SourceRouting.To(cs01);
        private static          Int32              requestIds     = 100000;

        private static Request_Id NextRequestId
            => Request_Id.Parse(Interlocked.Increment(ref requestIds).ToString());

        private static readonly String[] ofSupport    = [ "state:read", "state:run", "reset:run", "unlock:run" ];

        private static readonly String[] ofTheCPO     = [ "*:read", "node:run", "reset:run", "reset-immediate:run", "state:run", "unlock:run",
                                                          "firmware:edit", "session:run", "availability:edit", "connections:edit" ];

        private static readonly String[] ofInstaller  = [ .. ofTheCPO, "power:edit", "calibration:edit", "meters:edit" ];

        private static IEnumerable<String> PermissionsOf(String Role)

            => Role switch {
                   "systemadmin"  => [ "*:read", "*:edit", "*:run" ],
                   "support"      => ofSupport,
                   "cpo"          => ofTheCPO,
                   "installer"    => ofInstaller,
                   "dns-admin"    => [ "dns:read", "dns:edit" ],
                   _              => []
               };

        #endregion

        #region (private) AStation(...) / Sent(...) / Accepts(...)

        /// <summary>
        /// The signature policy of a charging station that knows the given keys by their roles.
        /// </summary>
        private static SignaturePolicy AStation(MessagePolicyModes                 Mode,
                                                IEnumerable<(KeyPair, String)>?    Signers                    = null,
                                                Func<Boolean>?                     IsMetrologicallyRelevant   = null)
        {

            var signers  = (Signers ?? []).ToArray();
            var station  = new SignaturePolicy().BindTo(cs01, Version.WebSocketSubProtocolId);

            station.Mode               = Mode;
            station.IncomingDirection  = "CSMS->CS";
            station.SignerRolesOf      = signature => signers.Where (signer => signature.KeyId.SequenceEqual(signer.Item1.PublicKeyBytes)).
                                                              Select(signer => signer.Item2);
            station.PermissionsOfRole  = PermissionsOf;

            station.AddActionPermissions(ChargingStationActionPermissions.Default(IsMetrologicallyRelevant));

            return station;

        }

        /// <summary>
        /// Sign the request as a CSMS does, and hand back what the station parses
        /// and what was on the wire.
        /// </summary>
        private static (TRequest Received, JObject OnTheWire) Sent<TRequest>(TRequest                         Request,
                                                                             Func<TRequest, Boolean, JObject>  ToJSON,
                                                                             Func<JObject, TRequest>           Parse,
                                                                             params KeyPair[]                  SignedBy)
            where TRequest : IRequest
        {

            if (SignedBy.Length > 0)
                Assert.That(new SignaturePolicy().
                                BindTo(NetworkingNode_Id.CSMS, Version.WebSocketSubProtocolId).
                                SignRequestMessage(Request, ToJSON(Request, true), out var errorResponse, [.. SignedBy.Select(keyPair => keyPair.ToSignInfo1())]),
                            Is.True,
                            errorResponse);

            var onTheWire = ToJSON(Request, false);

            return (Parse(onTheWire), onTheWire);

        }

        private static (ResetRequest, JObject) AReset(ResetType Type, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new ResetRequest(toTheStation, Type, RequestId: requestId),
                        (request, withContext) => request.ToJSON(withContext),
                        json => ResetRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        private static (TriggerMessageRequest, JObject) ATrigger(MessageTrigger Message, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new TriggerMessageRequest(toTheStation, Message, RequestId: requestId),
                        (request, withContext) => request.ToJSON(withContext),
                        json => TriggerMessageRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        private static (UnlockConnectorRequest, JObject) AnUnlock(params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new UnlockConnectorRequest(toTheStation, EVSE_Id.Parse(1), Connector_Id.Parse(1), RequestId: requestId),
                        (request, withContext) => request.ToJSON(withContext),
                        json => UnlockConnectorRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        private static (UpdateFirmwareRequest, JObject) AFirmwareUpdate(params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new UpdateFirmwareRequest(
                            toTheStation,
                            new Firmware(
                                FirmwareURL:         URL.Parse("https://example.org/fw0001.bin"),
                                RetrieveTimestamp:   Timestamp.Now
                            ),
                            1,
                            RequestId: requestId
                        ),
                        (request, withContext) => request.ToJSON(withContext),
                        json => UpdateFirmwareRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        private static (SetChargingProfileRequest, JObject) AChargingProfile(ChargingProfilePurpose Purpose, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new SetChargingProfileRequest(
                            toTheStation,
                            EVSE_Id.Parse(Purpose == ChargingProfilePurpose.TxDefaultProfile ? (UInt16) 1 : (UInt16) 0),
                            new ChargingProfile(
                                Id:                       ChargingProfile_Id.NewRandom,
                                StackLevel:               1,
                                ChargingProfilePurpose:   Purpose,
                                ChargingProfileKind:      ChargingProfileKinds.Absolute,
                                ChargingSchedules:        [
                                                              new ChargingSchedule(
                                                                  Id:                        ChargingSchedule_Id.NewRandom(),
                                                                  ChargingRateUnit:          ChargingRateUnits.Watts,
                                                                  ChargingSchedulePeriods:   [
                                                                                                 new ChargingSchedulePeriod(
                                                                                                     StartPeriod:   TimeSpan.Zero,
                                                                                                     Limit:         ChargingRateValue.ParseWatts(11000)
                                                                                                 )
                                                                                             ],
                                                                  StartSchedule:             Timestamp.Now
                                                              )
                                                          ]
                            ),
                            RequestId: requestId
                        ),
                        (request, withContext) => request.ToJSON(withContext),
                        json => SetChargingProfileRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        private static (ClearChargingProfileRequest, JObject) AClearing(ChargingProfilePurpose? Purpose, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new ClearChargingProfileRequest(
                            toTheStation,
                            ChargingProfileId:         Purpose.HasValue ? null : ChargingProfile_Id.NewRandom,
                            ChargingProfileCriteria:   Purpose.HasValue ? new ClearChargingProfile(ChargingProfilePurpose: Purpose) : null,
                            RequestId:                 requestId
                        ),
                        (request, withContext) => request.ToJSON(withContext),
                        json => ClearChargingProfileRequest.Parse(json, requestId, toTheStation, NetworkPath.Empty),
                        SignedBy);
        }

        /// <summary>
        /// Whether the station's adapter would hand the request to its handler.
        /// </summary>
        private static Boolean Accepts<TRequest>(SignaturePolicy               Station,
                                                 (TRequest, JObject)           Sent,
                                                 Func<TRequest, JObject>       WithContext,
                                                 out String?                   ErrorResponse)
            where TRequest : IRequest

            => Station.VerifyRequestMessage(Sent.Item1, WithContext(Sent.Item1), out ErrorResponse, Sent.Item2);

        private static Boolean Accepts(SignaturePolicy Station, (ResetRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy Station, (TriggerMessageRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy Station, (UnlockConnectorRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy Station, (UpdateFirmwareRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy Station, (SetChargingProfileRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy Station, (ClearChargingProfileRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(Station, Sent, request => request.ToJSON(true), out ErrorResponse);

        #endregion


        #region The modes

        #region WhileThePolicyIsOffNothingAsksWhoSigned()

        /// <summary>
        /// "Off" is the default, and a station in it behaves as before it had a
        /// policy table: an unsigned request of a standard CSMS passes.
        /// </summary>
        [Test]
        public void WhileThePolicyIsOffNothingAsksWhoSigned()
        {

            var station = AStation(MessagePolicyModes.Off);

            Assert.Multiple(() => {
                Assert.That(new SignaturePolicy().Mode, Is.EqualTo(MessagePolicyModes.Off));
                Assert.That(Accepts(station, AReset(ResetType.Immediate), out var errorResponse), Is.True, errorResponse);
            });

        }

        #endregion

        #region InEnforceAnUnsignedRequestOfTheCSMSIsRefused()

        /// <summary>
        /// What the station's table says a Reset needs, nobody holds who has not signed.
        /// </summary>
        [Test]
        public void InEnforceAnUnsignedRequestOfTheCSMSIsRefused()
        {

            var station = AStation(MessagePolicyModes.Enforce);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle), out var errorResponse), Is.False, "An unsigned Reset got past the policy!");
                Assert.That(errorResponse, Does.Contain("no valid signature").And.Contain("reset:run"));
            });

        }

        #endregion

        #region InObserveAViolationIsReportedAndTheRequestGoesOn()

        [Test]
        public void InObserveAViolationIsReportedAndTheRequestGoesOn()
        {

            var station     = AStation(MessagePolicyModes.Observe);
            var violations  = new List<PolicyViolation>();

            station.OnPolicyViolation += violations.Add;

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle), out var errorResponse), Is.True, errorResponse);
                Assert.That(violations,                  Has.Count.EqualTo(1));
                Assert.That(violations[0].Action,        Is.EqualTo("Reset"));
                Assert.That(violations[0].Refused,       Is.False);
            });

        }

        #endregion

        #region InEnforceABrokenSignatureIsRefusedAndInObserveItCountsAsNoSigner()

        /// <summary>
        /// The table wants to know who signed, so what is signed is verified, also where no
        /// verification rule asks for signatures. A payload changed on the way makes the
        /// signature invalid: refused in "Enforce", and in "Observe" its key is no signer.
        /// </summary>
        [Test]
        public void InEnforceABrokenSignatureIsRefusedAndInObserveItCountsAsNoSigner()
        {

            var admin = ECCKeyPair.GenerateKeys()!;

            (ResetRequest, JObject) Tampered()
            {
                var (signed, onTheWire)  = AReset(ResetType.OnIdle, admin);
                onTheWire["type"]        = "Immediate";
                return (ResetRequest.Parse(onTheWire, signed.RequestId, toTheStation, NetworkPath.Empty), onTheWire);
            }

            var enforcing   = AStation(MessagePolicyModes.Enforce, [ (admin, "systemadmin") ]);
            var observing   = AStation(MessagePolicyModes.Observe, [ (admin, "systemadmin") ]);
            var violations  = new List<PolicyViolation>();

            observing.OnPolicyViolation += violations.Add;

            Assert.Multiple(() => {
                Assert.That(Accepts(enforcing, Tampered(), out _),                 Is.False, "A request changed after it was signed got past the policy!");
                Assert.That(Accepts(observing, Tampered(), out var errorResponse), Is.True,  errorResponse);
                Assert.That(violations, Has.Count.EqualTo(1), "The changed request was executed without a report!");
            });

        }

        #endregion

        #endregion

        #region The station's table: what each action needs

        #region TheSupportMayRestartWhenIdleButNotAtOnce()

        /// <summary>
        /// Q12: restarting once idle and restarting at once are two rights, and the
        /// telephone support holds the first only.
        /// </summary>
        [Test]
        public void TheSupportMayRestartWhenIdleButNotAtOnce()
        {

            var support  = ECCKeyPair.GenerateKeys()!;
            var cpo      = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (support, "support"), (cpo, "cpo") ]);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle,             support), out var whenIdle),  Is.True,  whenIdle);
                Assert.That(Accepts(station, AReset(ResetType.Immediate,          support), out var atOnce),    Is.False, "The support restarted a station at once!");
                Assert.That(atOnce, Does.Contain("reset-immediate:run"));
                Assert.That(Accepts(station, AReset(ResetType.ImmediateAndResume, support), out _),             Is.False, "The support restarted a station at once!");
                Assert.That(Accepts(station, AReset(ResetType.Immediate,          cpo),     out var byTheCPO),  Is.True,  byTheCPO);
            });

        }

        #endregion

        #region TheSupportMayAskForTheEVSEStatusAndNoOtherMessage()

        /// <summary>
        /// TriggerMessage is checked by the message it asks for.
        /// </summary>
        [Test]
        public void TheSupportMayAskForTheEVSEStatusAndNoOtherMessage()
        {

            var support  = ECCKeyPair.GenerateKeys()!;
            var cpo      = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (support, "support"), (cpo, "cpo") ]);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, ATrigger(MessageTrigger.StatusNotification,             support), out var status),  Is.True,  status);
                Assert.That(Accepts(station, ATrigger(MessageTrigger.BootNotification,               support), out var boot),    Is.False, "The support triggered a BootNotification!");
                Assert.That(boot, Does.Contain("node:run"));
                Assert.That(Accepts(station, ATrigger(MessageTrigger.BootNotification,               cpo),     out var byCPO),   Is.True,  byCPO);
                Assert.That(Accepts(station, ATrigger(MessageTrigger.SignChargingStationCertificate, cpo),     out var newCert), Is.False, "The operator had the station ask for a new certificate!");
                Assert.That(newCert, Does.Contain("certificates:edit"));
            });

        }

        #endregion

        #region TheSupportMayReleaseAConnector()

        [Test]
        public void TheSupportMayReleaseAConnector()
        {

            var support  = ECCKeyPair.GenerateKeys()!;
            var stranger = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (support, "support") ]);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AnUnlock(support),  out var errorResponse), Is.True, errorResponse);
                Assert.That(Accepts(station, AnUnlock(stranger), out var byAStranger),   Is.False, "A key that holds no role released a connector!");
                Assert.That(byAStranger, Does.Contain("unlock:run"));
            });

        }

        #endregion

        #region AChargingProfileIsJudgedByItsPurpose()

        /// <summary>
        /// The operator manages the load of sessions, the installer says what the station
        /// may draw, and external constraints do not come from a CSMS at all.
        /// </summary>
        [Test]
        public void AChargingProfileIsJudgedByItsPurpose()
        {

            var cpo        = ECCKeyPair.GenerateKeys()!;
            var installer  = ECCKeyPair.GenerateKeys()!;
            var admin      = ECCKeyPair.GenerateKeys()!;
            var station    = AStation(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (installer, "installer"), (admin, "systemadmin") ]);

            Assert.Multiple(() => {

                Assert.That(Accepts(station, AChargingProfile(ChargingProfilePurpose.TxDefaultProfile,                   cpo),       out var ofASession),  Is.True,  ofASession);
                Assert.That(Accepts(station, AChargingProfile(ChargingProfilePurpose.ChargingStationMaxProfile,          cpo),       out var stationMax),  Is.False, "The operator changed what the station may draw!");
                Assert.That(stationMax, Does.Contain("power:edit"));
                Assert.That(Accepts(station, AChargingProfile(ChargingProfilePurpose.ChargingStationMaxProfile,          installer), out var byInstaller), Is.True,  byInstaller);

                Assert.That(Accepts(station, AChargingProfile(ChargingProfilePurpose.ChargingStationExternalConstraints, admin),     out var external),    Is.False, "A CSMS set external constraints!");
                Assert.That(external, Does.Contain("External constraints"));

            });

        }

        #endregion

        #region ClearingIsJudgedByThePurposeItNames()

        /// <summary>
        /// Clearing needs what setting needs, but for the two purposes a CSMS does not
        /// name among its criteria: external constraints and local generation (OCPP 2.1,
        /// K10.FR.06). A profile of local generation the CSMS has set goes by its
        /// identification, which needs both rights.
        /// </summary>
        [Test]
        public void ClearingIsJudgedByThePurposeItNames()
        {

            var cpo        = ECCKeyPair.GenerateKeys()!;
            var installer  = ECCKeyPair.GenerateKeys()!;
            var admin      = ECCKeyPair.GenerateKeys()!;
            var station    = AStation(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (installer, "installer"), (admin, "systemadmin") ]);

            Assert.Multiple(() => {

                Assert.That(Accepts(station, AClearing(ChargingProfilePurpose.TxDefaultProfile,                   cpo),       out var ofASession),  Is.True,  ofASession);
                Assert.That(Accepts(station, AClearing(ChargingProfilePurpose.ChargingStationMaxProfile,          cpo),       out var stationMax),  Is.False, "The operator cleared what the station may draw!");
                Assert.That(stationMax, Does.Contain("power:edit"));
                Assert.That(Accepts(station, AClearing(ChargingProfilePurpose.ChargingStationMaxProfile,          installer), out var byInstaller), Is.True,  byInstaller);

                Assert.That(Accepts(station, AClearing(ChargingProfilePurpose.ChargingStationExternalConstraints, admin),     out var external),    Is.False, "A CSMS cleared external constraints!");
                Assert.That(external, Does.Contain("External constraints"));
                Assert.That(Accepts(station, AClearing(ChargingProfilePurpose.LocalGeneration,                    admin),     out var local),       Is.False, "A CSMS cleared local generation by its purpose!");
                Assert.That(local,    Does.Contain("Local generation"));

                // Setting it stays the installer's, and so does clearing it by its identification.
                Assert.That(Accepts(station, AChargingProfile(ChargingProfilePurpose.LocalGeneration,             installer), out var set),         Is.True,  set);
                Assert.That(Accepts(station, AClearing(null,                                                      installer), out var byId),        Is.True,  byId);
                Assert.That(Accepts(station, AClearing(null,                                                      cpo),       out var byIdOfCPO),   Is.False, "The operator cleared a profile whose purpose nobody knows!");

            });

        }

        #endregion

        #region AFirmwareUpdateOfAMetrologicallyRelevantStationNeedsTheCalibrationRight()

        /// <summary>
        /// Whether the station is metrologically relevant the station says itself, not the
        /// message: the operator's firmware rollout then needs the installer's right too.
        /// </summary>
        [Test]
        public void AFirmwareUpdateOfAMetrologicallyRelevantStationNeedsTheCalibrationRight()
        {

            var cpo        = ECCKeyPair.GenerateKeys()!;
            var installer  = ECCKeyPair.GenerateKeys()!;
            var relevant   = false;
            var station    = AStation(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (installer, "installer") ], () => relevant);

            Assert.That(Accepts(station, AFirmwareUpdate(cpo), out var plain), Is.True, plain);

            relevant = true;

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AFirmwareUpdate(cpo),       out var refused),     Is.False, "The operator updated the firmware of a metrologically relevant station!");
                Assert.That(refused, Does.Contain("calibration:edit"));
                Assert.That(Accepts(station, AFirmwareUpdate(installer), out var byInstaller), Is.True,  byInstaller);
            });

        }

        #endregion

        #endregion

        #region The policy table: who must have signed

        #region AnEntryThatWantsTwoKeysIsNotMetByOneKeyTwice()

        /// <summary>
        /// A quorum counts distinct keys: the same key signing twice is one signer.
        /// </summary>
        [Test]
        public void AnEntryThatWantsTwoKeysIsNotMetByOneKeyTwice()
        {

            var first    = ECCKeyPair.GenerateKeys()!;
            var second   = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (first, "release-manager"), (second, "release-manager") ]).
                               AddPolicyEntry(new MessagePolicyEntry(
                                                  "firmware",
                                                  "UpdateFirmware",
                                                  SignerRequirement.AnyOf([ "release-manager" ], 2),
                                                  Direction:    "CSMS->CS",
                                                  Versions:     [ "ocpp2.0.1", "ocpp2.1" ],
                                                  Description:  "Firmware updates need two release managers"
                                              ));

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AFirmwareUpdate(first),         out var byOne),    Is.False, "One release manager updated the firmware!");
                Assert.That(byOne, Does.Contain("'firmware'").And.Contain("1 of the 2"));
                Assert.That(Accepts(station, AFirmwareUpdate(first, first),  out _),            Is.False, "One key signing twice counted as two release managers!");
                Assert.That(Accepts(station, AFirmwareUpdate(first, second), out var byTwo),    Is.True,  byTwo);
            });

        }

        #endregion

        #region AnEntryOverridesTheStationsTable()

        /// <summary>
        /// The table a station ships says a Reset when idle needs "reset:run"; an entry of
        /// the policy table can say that it needs nobody's signature.
        /// </summary>
        [Test]
        public void AnEntryOverridesTheStationsTable()
        {

            var station = AStation(MessagePolicyModes.Enforce).
                              AddPolicyEntry(new MessagePolicyEntry("reset-when-idle",
                                                                    "Reset",
                                                                    SignerRequirement.Unsigned,
                                                                    Where: [ FieldCondition.Equal("type", "OnIdle") ]));

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle),    out var whenIdle), Is.True,  whenIdle);
                Assert.That(Accepts(station, AReset(ResetType.Immediate), out _),            Is.False, "The entry for OnIdle let an immediate Reset through!");
            });

        }

        #endregion

        #region TheMoreSpecificEntryWins()

        /// <summary>
        /// An action beats "*", and an entry with a condition beats one without.
        /// </summary>
        [Test]
        public void TheMoreSpecificEntryWins()
        {

            var officer  = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (officer, "security-officer") ]).
                               AddPolicyEntries([
                                   new MessagePolicyEntry("everything", "*",     SignerRequirement.Unsigned),
                                   new MessagePolicyEntry("reset",      "Reset", SignerRequirement.AnySigner),
                                   new MessagePolicyEntry("reset-now",  "Reset", SignerRequirement.AllOf([ "security-officer", "release-manager" ]),
                                                          Where: [ FieldCondition.In("type", "Immediate", "ImmediateAndResume") ])
                               ]);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AnUnlock(),                            out var byTheDefault), Is.True,  byTheDefault);
                Assert.That(Accepts(station, AReset(ResetType.OnIdle),              out var unsigned),     Is.False, "\"*\" beat the entry for Reset!");
                Assert.That(unsigned, Does.Contain("'reset'"));
                Assert.That(Accepts(station, AReset(ResetType.OnIdle,    officer),  out var signed),       Is.True,  signed);
                Assert.That(Accepts(station, AReset(ResetType.Immediate, officer),  out var atOnce),       Is.False, "The entry without a condition beat the one with it!");
                Assert.That(atOnce, Does.Contain("'reset-now'").And.Contain("'release-manager'"));
            });

        }

        #endregion

        #region TwoEntriesAMessageMatchesAlikeAreRefusedWhenWritten()

        [Test]
        public void TwoEntriesAMessageMatchesAlikeAreRefusedWhenWritten()
        {

            var station = AStation(MessagePolicyModes.Enforce).
                              AddPolicyEntry(new MessagePolicyEntry("reset", "Reset", SignerRequirement.AnySigner, Versions: [ "ocpp2.1" ]));

            Assert.Multiple(() => {

                Assert.That(() => station.AddPolicyEntry(new MessagePolicyEntry("reset-again", "Reset", SignerRequirement.Unsigned)),
                            Throws.ArgumentException.With.Message.Contains("'reset'"));

                Assert.That(() => station.AddPolicyEntry(new MessagePolicyEntry("reset", "Reset", SignerRequirement.Unsigned, Where: [ FieldCondition.Equal("type", "OnIdle") ])),
                            Throws.ArgumentException.With.Message.Contains("already"));

                // Another version, or a condition of its own, tells them apart.
                Assert.That(() => station.AddPolicyEntry(new MessagePolicyEntry("reset-2.0.1", "Reset", SignerRequirement.Unsigned, Versions: [ "ocpp2.0.1" ])),
                            Throws.Nothing);

                Assert.That(station.TryAddPolicyEntries(
                                JArray.Parse("""
                                             [ { "id": "a", "match": { "action": "GetLog" }, "require": "AnySigner" },
                                               { "id": "b", "match": { "action": "GetLog" }, "require": "Unsigned"  } ]
                                             """),
                                out var errorResponse),
                            Is.False);

                Assert.That(errorResponse,                                              Does.Contain("'a'"));
                Assert.That(station.PolicyEntries.Select(entry => entry.Id),            Is.EquivalentTo(new[] { "reset", "reset-2.0.1" }),
                            "A table that was refused left an entry behind!");

            });

        }

        #endregion

        #region AnEntryThatOnlyRecordsLetsTheRequestThrough()

        /// <summary>
        /// "Record" is for a migration: the request is executed, and the violation reported.
        /// </summary>
        [Test]
        public void AnEntryThatOnlyRecordsLetsTheRequestThrough()
        {

            var violations  = new List<PolicyViolation>();
            var station     = AStation(MessagePolicyModes.Enforce).
                                  AddPolicyEntry(new MessagePolicyEntry("reset", "Reset", SignerRequirement.AnySigner, PolicyViolationActions.Record));

            station.OnPolicyViolation += violations.Add;

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle), out var errorResponse), Is.True, errorResponse);
                Assert.That(violations.Single().Entry?.Id,   Is.EqualTo("reset"));
                Assert.That(violations.Single().Refused,     Is.False);
            });

        }

        #endregion

        #region AnEntryForTheOtherDirectionOrVersionMatchesNothing()

        [Test]
        public void AnEntryForTheOtherDirectionOrVersionMatchesNothing()
        {

            var cpo      = ECCKeyPair.GenerateKeys()!;
            var station  = AStation(MessagePolicyModes.Enforce, [ (cpo, "cpo") ]).
                               AddPolicyEntries([
                                   new MessagePolicyEntry("from-the-station", "Reset", SignerRequirement.Unsigned,  Direction: "CS->CSMS"),
                                   new MessagePolicyEntry("in-1.6",           "Reset", SignerRequirement.Unsigned,  Direction: "CSMS->CS", Versions: [ "ocpp1.6" ])
                               ]);

            Assert.Multiple(() => {
                Assert.That(Accepts(station, AReset(ResetType.OnIdle),      out _),             Is.False, "An entry for another direction or version let a Reset through!");
                Assert.That(Accepts(station, AReset(ResetType.OnIdle, cpo), out var byTheCPO),  Is.True,  byTheCPO);
            });

        }

        #endregion

        #region AnEntryIsWrittenAndReadAsJSON()

        [Test]
        public void AnEntryIsWrittenAndReadAsJSON()
        {

            var json = JObject.Parse("""
                                     {
                                         "id":            "firmware",
                                         "match":         { "action": "UpdateFirmware", "direction": "CSMS->CS", "versions": ["ocpp2.0.1", "ocpp2.1"] },
                                         "require":       { "anyOf": ["release-manager"], "count": 2 },
                                         "onViolation":   "Refuse",
                                         "description":   "Firmware updates need two release managers"
                                     }
                                     """);

            Assert.That(MessagePolicyEntry.TryParse(json, out var entry, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(entry!.Id,                  Is.EqualTo("firmware"));
                Assert.That(entry.Action,               Is.EqualTo("UpdateFirmware"));
                Assert.That(entry.Direction,            Is.EqualTo("CSMS->CS"));
                Assert.That(entry.Versions,             Is.EqualTo(new[] { "ocpp2.0.1", "ocpp2.1" }));
                Assert.That(entry.Require.Kind,         Is.EqualTo(SignerRequirement.Kinds.AnyOf));
                Assert.That(entry.Require.Count,        Is.EqualTo(2));
                Assert.That(entry.OnViolation,          Is.EqualTo(PolicyViolationActions.Refuse));
                Assert.That(JToken.DeepEquals(entry.ToJSON(), json), Is.True, entry.ToJSON().ToString());

                var where = JObject.Parse("""{ "id": "x", "match": { "action": "Reset", "where": { "type": { "in": ["Immediate"] }, "evseId": { "equals": 1 } } }, "require": "AnySigner" }""");
                Assert.That(MessagePolicyEntry.TryParse(where, out var withConditions, out var whereError), Is.True, whereError);
                Assert.That(withConditions!.Where.Select(condition => condition.ToString()), Is.EqualTo(new[] { "type in [Immediate]", "evseId = 1" }));

            });

        }

        #endregion

        #region AnEntryThatIsNotOneIsRefused(JSON, Why)

        [TestCase("""{ "match": { "action": "Reset" }, "require": "AnySigner" }""",                                                  "\"id\"")]
        [TestCase("""{ "id": "x", "require": "AnySigner" }""",                                                                        "\"match\"")]
        [TestCase("""{ "id": "x", "match": { }, "require": "AnySigner" }""",                                                          "\"action\"")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" } }""",                                                                "requirement")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": "Anybody" }""",                                          "Anybody")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": { "anyOf": [] } }""",                                    "no role")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": { "anyOf": ["a"], "count": 0 } }""",                     "\"count\"")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": { "anyOf": ["a"], "allOf": ["b"] } }""",                 "not both")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": { "allOf": ["a"], "count": 1 } }""",                     "\"count\"")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset", "direction": "sideways" }, "require": "AnySigner" }""",               "sideways")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset", "where": { "type": { "matches": ".*" } } }, "require": "AnySigner" }""", "matches")]
        [TestCase("""{ "id": "x", "match": { "action": "Reset" }, "require": "AnySigner", "onViolation": "Ignore" }""",               "\"onViolation\"")]
        public void AnEntryThatIsNotOneIsRefused(String JSON, String Why)
        {

            Assert.Multiple(() => {
                Assert.That(MessagePolicyEntry.TryParse(JObject.Parse(JSON), out _, out var errorResponse), Is.False, JSON);
                Assert.That(errorResponse, Does.Contain(Why));
            });

        }

        #endregion

        #endregion

    }

}
