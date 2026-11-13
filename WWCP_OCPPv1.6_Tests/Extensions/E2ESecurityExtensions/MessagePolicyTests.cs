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

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// What a signer of a request may have an OCPP 1.6 charge point do: the
    /// table of section 5.3 of the white paper "Signed OCPP Messages and
    /// Signer Roles", in the words of OCPP 1.6 (phase 3 of its chapter 7).
    /// </summary>
    /// <remarks>
    /// Each test signs a request as a central system's adapter does, takes what
    /// goes on the wire, parses it as the charge point does and verifies it as
    /// the charge point's adapter does. Nothing is sent over a network. The
    /// roles are the ones section 5.3 names, with the permissions it gives them.
    /// </remarks>
    [TestFixture]
    public class MessagePolicyTests
    {

        #region Data

        private static readonly NetworkingNode_Id  cp01              = NetworkingNode_Id.Parse("cp01");
        private static readonly SourceRouting      toTheChargePoint  = SourceRouting.To(cp01);

        private static          Int32              requestIds        = 160000;

        private static Request_Id NextRequestId
            => Request_Id.Parse(Interlocked.Increment(ref requestIds).ToString());

        private static readonly String[] ofSupport    = [ "state:read", "state:run", "reset:run", "unlock:run" ];
        private static readonly String[] ofTheCPO     = [ "*:read", "node:run", "reset:run", "reset-immediate:run", "state:run", "unlock:run",
                                                          "firmware:edit", "session:run", "availability:edit", "connections:edit",
                                                          "dns:edit", "nts:edit" ];
        private static readonly String[] ofInstaller  = [ .. ofTheCPO, "power:edit", "calibration:edit", "meters:edit" ];

        private static IEnumerable<String> PermissionsOf(String Role)

            => Role switch {
                   "systemadmin"  => [ "*:read", "*:edit", "*:run" ],
                   "support"      => ofSupport,
                   "cpo"          => ofTheCPO,
                   "installer"    => ofInstaller,
                   "authorization" => [ "authorization:read", "authorization:edit" ],
                   _              => []
               };

        #endregion

        #region (private) AChargePoint(...) / Sent(...) / Accepts(...)

        /// <summary>
        /// The signature policy of a charge point that knows the given keys by their roles.
        /// </summary>
        private static SignaturePolicy AChargePoint(MessagePolicyModes                 Mode,
                                                    IEnumerable<(KeyPair, String)>?    Signers                    = null,
                                                    Func<Boolean>?                     IsMetrologicallyRelevant   = null)
        {

            var signers      = (Signers ?? []).ToArray();
            var chargePoint  = new SignaturePolicy().BindTo(cp01, Version.WebSocketSubProtocolId);

            chargePoint.Mode               = Mode;
            chargePoint.IncomingDirection  = "CSMS->CS";
            chargePoint.SignerRolesOf      = signature => signers.Where (signer => signature.KeyId.SequenceEqual(signer.Item1.PublicKeyBytes)).
                                                                  Select(signer => signer.Item2);
            chargePoint.PermissionsOfRole  = PermissionsOf;

            chargePoint.AddActionPermissions(ChargePointActionPermissions.Default(IsMetrologicallyRelevant));

            foreach (var action in ChargePointActionPermissions.ActionsDecidedPerKey)
                chargePoint.ActionsDecidedPerEntry.Add(action);

            return chargePoint;

        }

        /// <summary>
        /// Sign the request as a central system does, and hand back what the charge
        /// point parses and what was on the wire.
        /// </summary>
        private static (TRequest Received, JObject OnTheWire) Sent<TRequest>(TRequest                 Request,
                                                                             Func<TRequest, JObject>  ToJSON,
                                                                             Func<JObject, TRequest>  Parse,
                                                                             params KeyPair[]         SignedBy)
            where TRequest : IRequest
        {

            if (SignedBy.Length > 0)
                Assert.That(new SignaturePolicy().
                                BindTo(NetworkingNode_Id.CSMS, Version.WebSocketSubProtocolId).
                                SignRequestMessage(Request, ToJSON(Request), out var errorResponse, [.. SignedBy.Select(keyPair => keyPair.ToSignInfo1())]),
                            Is.True,
                            errorResponse);

            var onTheWire = ToJSON(Request);

            return (Parse(onTheWire), onTheWire);

        }

        private static Boolean Accepts<TRequest>(SignaturePolicy          ChargePoint,
                                                 (TRequest, JObject)      Sent,
                                                 Func<TRequest, JObject>  ToJSON,
                                                 out String?              ErrorResponse)
            where TRequest : IRequest

            => ChargePoint.VerifyRequestMessage(Sent.Item1, ToJSON(Sent.Item1), out ErrorResponse, Sent.Item2);


        private static (ResetRequest, JObject) AReset(ResetType Type, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new ResetRequest(toTheChargePoint, Type, RequestId: requestId),
                        request => request.ToJSON(),
                        json    => ResetRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (TriggerMessageRequest, JObject) ATrigger(MessageTrigger Message, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new TriggerMessageRequest(toTheChargePoint, Message, RequestId: requestId),
                        request => request.ToJSON(),
                        json    => TriggerMessageRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (UnlockConnectorRequest, JObject) AnUnlock(params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new UnlockConnectorRequest(toTheChargePoint, Connector_Id.Parse(1), RequestId: requestId),
                        request => request.ToJSON(),
                        json    => UnlockConnectorRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (UpdateFirmwareRequest, JObject) AFirmwareUpdate(params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new UpdateFirmwareRequest(toTheChargePoint, URL.Parse("https://example.org/fw0001.bin"), Timestamp.Now, RequestId: requestId),
                        request => request.ToJSON(),
                        json    => UpdateFirmwareRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (SetChargingProfileRequest, JObject) AChargingProfile(ChargingProfilePurposes Purpose, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new SetChargingProfileRequest(
                            toTheChargePoint,
                            Connector_Id.Parse(Purpose == ChargingProfilePurposes.ChargePointMaxProfile ? "0" : "1"),
                            new ChargingProfile(
                                ChargingProfile_Id.Parse("1"),
                                1,
                                Purpose,
                                ChargingProfileKinds.Absolute,
                                new ChargingSchedule(
                                    ChargingRateUnits.Watts,
                                    [
                                        new ChargingSchedulePeriod(
                                            TimeSpan.Zero,
                                            ChargingRateValue.ParseWatts(11000)
                                        )
                                    ]
                                )
                            ),
                            RequestId: requestId
                        ),
                        request => request.ToJSON(),
                        json    => SetChargingProfileRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (ClearChargingProfileRequest, JObject) AClearing(ChargingProfilePurposes? Purpose, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new ClearChargingProfileRequest(
                            toTheChargePoint,
                            ChargingProfileId:        Purpose.HasValue ? null : ChargingProfile_Id.Parse("1"),
                            ChargingProfilePurpose:   Purpose,
                            RequestId:                requestId
                        ),
                        request => request.ToJSON(),
                        json    => ClearChargingProfileRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static (ChangeConfigurationRequest, JObject) AChange(String Key, params KeyPair[] SignedBy)
        {
            var requestId = NextRequestId;
            return Sent(new ChangeConfigurationRequest(toTheChargePoint, Key, "1", RequestId: requestId),
                        request => request.ToJSON(),
                        json    => ChangeConfigurationRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                        SignedBy);
        }

        private static Boolean Accepts(SignaturePolicy ChargePoint, (ResetRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (TriggerMessageRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (UnlockConnectorRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (UpdateFirmwareRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (SetChargingProfileRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (ClearChargingProfileRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        private static Boolean Accepts(SignaturePolicy ChargePoint, (ChangeConfigurationRequest, JObject) Sent, out String? ErrorResponse)
            => Accepts(ChargePoint, Sent, request => request.ToJSON(), out ErrorResponse);

        /// <summary>
        /// Whether the charge point would change, or show, the key for whoever signed the request.
        /// </summary>
        private static Boolean MayTouch(SignaturePolicy ChargePoint, (ChangeConfigurationRequest, JObject) Sent, String Operation, out String? Reason)
        {

            var key = Sent.Item1.Key;

            Assert.That(Accepts(ChargePoint, Sent, out var errorResponse), Is.True, $"A request decided key by key was refused as a whole: {errorResponse}");

            return ChargePoint.DecideVariable(Sent.Item1, ChargePointActionPermissions.ComponentOfKey(key), key, null, null, Operation, out Reason);

        }

        #endregion


        #region WhileThePolicyIsOffNothingAsksWhoSigned()

        /// <summary>
        /// The default: a charge point behaves as it did before there was a table.
        /// </summary>
        [Test]
        public void WhileThePolicyIsOffNothingAsksWhoSigned()
        {

            var chargePoint = AChargePoint(MessagePolicyModes.Off);

            Assert.Multiple(() => {
                Assert.That(Accepts(chargePoint, AReset(ResetType.Hard),                  out var reset),   Is.True, reset);
                Assert.That(MayTouch(chargePoint, AChange("dns.servers"), "edit",         out var change),  Is.True, change);
            });

        }

        #endregion

        #region ASoftResetIsTheRestartOnceIdleAndAHardOneTheRestartAtOnce()

        /// <summary>
        /// The support restarts a charge point softly, and not hard.
        /// </summary>
        [Test]
        public void ASoftResetIsTheRestartOnceIdleAndAHardOneTheRestartAtOnce()
        {

            var support      = ECCKeyPair.GenerateKeys()!;
            var cpo          = ECCKeyPair.GenerateKeys()!;
            var chargePoint  = AChargePoint(MessagePolicyModes.Enforce, [ (support, "support"), (cpo, "cpo") ]);

            Assert.Multiple(() => {

                Assert.That(Accepts(chargePoint, AReset(ResetType.Soft, support), out var soft),      Is.True,  soft);
                Assert.That(Accepts(chargePoint, AReset(ResetType.Hard, support), out var hard),      Is.False, "The support restarted a charge point at once!");
                Assert.That(hard, Does.Contain("reset-immediate:run"));
                Assert.That(Accepts(chargePoint, AReset(ResetType.Hard, cpo),     out var byTheCPO),  Is.True,  byTheCPO);

                Assert.That(Accepts(chargePoint, AReset(ResetType.Soft),          out var unsigned),  Is.False, "Nobody signed, and the charge point restarted!");
                Assert.That(unsigned, Does.Contain("reset:run"));

            });

        }

        #endregion

        #region TheSupportMayAskForTheStatusAndReleaseAConnectorAndNothingMore()

        /// <summary>
        /// What the telephone support does all day, and what it does not.
        /// </summary>
        [Test]
        public void TheSupportMayAskForTheStatusAndReleaseAConnectorAndNothingMore()
        {

            var support      = ECCKeyPair.GenerateKeys()!;
            var chargePoint  = AChargePoint(MessagePolicyModes.Enforce, [ (support, "support") ]);

            Assert.Multiple(() => {

                Assert.That(Accepts(chargePoint, ATrigger(MessageTrigger.StatusNotification, support), out var status),    Is.True,  status);
                Assert.That(Accepts(chargePoint, ATrigger(MessageTrigger.BootNotification,   support), out var boot),      Is.False, "The support had a BootNotification sent!");
                Assert.That(boot, Does.Contain("node:run"));

                Assert.That(Accepts(chargePoint, AnUnlock(support),                                    out var unlock),    Is.True,  unlock);
                Assert.That(Accepts(chargePoint, AFirmwareUpdate(support),                             out var firmware),  Is.False, "The support updated the firmware!");
                Assert.That(firmware, Does.Contain("firmware:edit"));

            });

        }

        #endregion

        #region ReservationsTheLocalListTheCacheAndDataTransfersHaveTheirRights()

        /// <summary>
        /// Reservations need "session:run"; the version of the local list
        /// "authorization:read"; sending the list and clearing the cache
        /// "authorization:edit"; a DataTransfer "node:run" (Achim, 2026-11-13).
        /// </summary>
        [Test]
        public void ReservationsTheLocalListTheCacheAndDataTransfersHaveTheirRights()
        {

            var cpo            = ECCKeyPair.GenerateKeys()!;
            var support        = ECCKeyPair.GenerateKeys()!;
            var authorization  = ECCKeyPair.GenerateKeys()!;
            var chargePoint    = AChargePoint(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (support, "support"), (authorization, "authorization") ]);

            (ReserveNowRequest, JObject) AReservation(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new ReserveNowRequest(toTheChargePoint, Connector_Id.Parse(1), Reservation_Id.NewRandom, Timestamp.Now + TimeSpan.FromMinutes(15), IdToken.NewRandom(), RequestId: requestId),
                            request => request.ToJSON(),
                            json => ReserveNowRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            (CancelReservationRequest, JObject) ACancellation(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new CancelReservationRequest(toTheChargePoint, Reservation_Id.NewRandom, RequestId: requestId),
                            request => request.ToJSON(),
                            json => CancelReservationRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            (GetLocalListVersionRequest, JObject) AListVersion(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new GetLocalListVersionRequest(toTheChargePoint, RequestId: requestId),
                            request => request.ToJSON(),
                            json => GetLocalListVersionRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            (SendLocalListRequest, JObject) AList(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new SendLocalListRequest(toTheChargePoint, 1, UpdateTypes.Full, RequestId: requestId),
                            request => request.ToJSON(),
                            json => SendLocalListRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            (ClearCacheRequest, JObject) AClearedCache(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new ClearCacheRequest(toTheChargePoint, RequestId: requestId),
                            request => request.ToJSON(),
                            json => ClearCacheRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            (DataTransferRequest, JObject) ADataTransfer(KeyPair SignedBy)
            {
                var requestId = NextRequestId;
                return Sent(new DataTransferRequest(toTheChargePoint, Vendor_Id.Parse("ACME Inc."), RequestId: requestId),
                            request => request.ToJSON(),
                            json => DataTransferRequest.Parse(json, requestId, toTheChargePoint, NetworkPath.Empty),
                            SignedBy);
            }

            Assert.Multiple(() => {

                // The operator holds and lets go outlets, reads everything and has messages sent.
                Assert.That(Accepts(chargePoint, AReservation (cpo),           request => request.ToJSON(), out var e1),    Is.True,   e1);
                Assert.That(Accepts(chargePoint, ACancellation(cpo),           request => request.ToJSON(), out var e2),    Is.True,   e2);
                Assert.That(Accepts(chargePoint, AListVersion (cpo),           request => request.ToJSON(), out var e3),    Is.True,   e3);
                Assert.That(Accepts(chargePoint, ADataTransfer(cpo),           request => request.ToJSON(), out var e4),    Is.True,   e4);
                Assert.That(Accepts(chargePoint, AList        (cpo),           request => request.ToJSON(), out var e5),    Is.False,  "Reading everything sent a local list!");
                Assert.That(e5,                                                                    Does.Contain("authorization:edit"));
                Assert.That(Accepts(chargePoint, AClearedCache(cpo),           request => request.ToJSON(), out var e6),    Is.False,  "Reading everything cleared the cache!");
                Assert.That(e6,                                                                    Does.Contain("authorization:edit"));

                Assert.That(Accepts(chargePoint, AListVersion (authorization), request => request.ToJSON(), out var e7),    Is.True,   e7);
                Assert.That(Accepts(chargePoint, AList        (authorization), request => request.ToJSON(), out var e8),    Is.True,   e8);
                Assert.That(Accepts(chargePoint, AClearedCache(authorization), request => request.ToJSON(), out var e9),    Is.True,   e9);

                // A known signer without the right is no longer enough.
                Assert.That(Accepts(chargePoint, AReservation (support),       request => request.ToJSON(), out var e10),   Is.False,  "The support held an outlet!");
                Assert.That(e10,                                                                   Does.Contain("session:run"));
                Assert.That(Accepts(chargePoint, ACancellation(support),       request => request.ToJSON(), out var e11),   Is.False,  "The support let go of a reservation!");
                Assert.That(e11,                                                                   Does.Contain("session:run"));
                Assert.That(Accepts(chargePoint, AListVersion (support),       request => request.ToJSON(), out var e12),   Is.False,  "The support read the version of the local list!");
                Assert.That(e12,                                                                   Does.Contain("authorization:read"));
                Assert.That(Accepts(chargePoint, ADataTransfer(support),       request => request.ToJSON(), out var e13),   Is.False,  "The support sent a DataTransfer!");
                Assert.That(e13,                                                                   Does.Contain("node:run"));

            });

        }

        #endregion

        #region AChargingProfileIsJudgedByItsPurpose()

        /// <summary>
        /// The operator manages the load of sessions, the installer says what the
        /// charge point may draw; clearing needs what setting needs.
        /// </summary>
        [Test]
        public void AChargingProfileIsJudgedByItsPurpose()
        {

            var cpo          = ECCKeyPair.GenerateKeys()!;
            var installer    = ECCKeyPair.GenerateKeys()!;
            var chargePoint  = AChargePoint(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (installer, "installer") ]);

            Assert.Multiple(() => {

                Assert.That(Accepts(chargePoint, AChargingProfile(ChargingProfilePurposes.TxDefaultProfile,      cpo),       out var ofASession),   Is.True,  ofASession);
                Assert.That(Accepts(chargePoint, AChargingProfile(ChargingProfilePurposes.ChargePointMaxProfile, cpo),       out var maximum),      Is.False, "The operator changed what the charge point may draw!");
                Assert.That(maximum, Does.Contain("power:edit"));
                Assert.That(Accepts(chargePoint, AChargingProfile(ChargingProfilePurposes.ChargePointMaxProfile, installer), out var byInstaller),  Is.True,  byInstaller);

                Assert.That(Accepts(chargePoint, AClearing(ChargingProfilePurposes.TxProfile,                    cpo),       out var cleared),      Is.True,  cleared);
                Assert.That(Accepts(chargePoint, AClearing(ChargingProfilePurposes.ChargePointMaxProfile,        cpo),       out var clearedMax),   Is.False, "The operator cleared what the charge point may draw!");
                Assert.That(Accepts(chargePoint, AClearing(null,                                                 cpo),       out var byId),         Is.False, "The operator cleared a profile whose purpose nobody knows!");
                Assert.That(Accepts(chargePoint, AClearing(null,                                                 installer), out var byIdOfThem),   Is.True,  byIdOfThem);

            });

        }

        #endregion

        #region AFirmwareUpdateOfAMetrologicallyRelevantChargePointNeedsTheCalibrationRight()

        /// <summary>
        /// The operator updates the firmware, but not that of a charge point under a
        /// calibration certificate: that is the installer's.
        /// </summary>
        [Test]
        public void AFirmwareUpdateOfAMetrologicallyRelevantChargePointNeedsTheCalibrationRight()
        {

            var cpo        = ECCKeyPair.GenerateKeys()!;
            var installer  = ECCKeyPair.GenerateKeys()!;
            var relevant   = false;
            var chargePoint  = AChargePoint(MessagePolicyModes.Enforce, [ (cpo, "cpo"), (installer, "installer") ], () => relevant);

            Assert.That(Accepts(chargePoint, AFirmwareUpdate(cpo), out var before), Is.True, before);

            relevant = true;

            Assert.Multiple(() => {
                Assert.That(Accepts(chargePoint, AFirmwareUpdate(cpo),       out var after),        Is.False, "The operator updated the firmware of a metrologically relevant charge point!");
                Assert.That(after, Does.Contain("calibration:edit"));
                Assert.That(Accepts(chargePoint, AFirmwareUpdate(installer), out var byInstaller),  Is.True,  byInstaller);
            });

        }

        #endregion

        #region AConfigurationKeyIsJudgedByWhatItBelongsTo()

        /// <summary>
        /// ChangeConfiguration and GetConfiguration are decided key by key: the keys
        /// of the DNS client and of the network time belong to "dns" and "nts", every
        /// other key to "configuration".
        /// </summary>
        [Test]
        public void AConfigurationKeyIsJudgedByWhatItBelongsTo()
        {

            var support      = ECCKeyPair.GenerateKeys()!;
            var cpo          = ECCKeyPair.GenerateKeys()!;
            var admin        = ECCKeyPair.GenerateKeys()!;
            var chargePoint  = AChargePoint(MessagePolicyModes.Enforce, [ (support, "support"), (cpo, "cpo"), (admin, "systemadmin") ]);

            Assert.Multiple(() => {

                Assert.That(ChargePointActionPermissions.ComponentOfKey("dns.rootPins.3"),     Is.EqualTo("DNSClientController"));
                Assert.That(ChargePointActionPermissions.ComponentOfKey("NTSKE.legal.aead"),   Is.EqualTo("NTSClientController"));
                Assert.That(ChargePointActionPermissions.ComponentOfKey("HeartbeatInterval"),  Is.EqualTo("ConfigurationKeys"));

                Assert.That(MayTouch(chargePoint, AChange("dns.servers",       cpo),     "edit", out var dns),        Is.True,  dns);
                Assert.That(MayTouch(chargePoint, AChange("ntske.legal.aead",  cpo),     "edit", out var nts),        Is.True,  nts);
                Assert.That(MayTouch(chargePoint, AChange("HeartbeatInterval", cpo),     "edit", out var heartbeat),  Is.False, "The operator changed a key of the configuration!");
                Assert.That(heartbeat, Does.Contain("configuration:edit"));
                Assert.That(MayTouch(chargePoint, AChange("HeartbeatInterval", cpo),     "read", out var read),       Is.True,  read);
                Assert.That(MayTouch(chargePoint, AChange("HeartbeatInterval", admin),   "edit", out var byAdmin),    Is.True,  byAdmin);

                Assert.That(MayTouch(chargePoint, AChange("dns.servers",       support), "read", out var bySupport),  Is.False, "The support read how the charge point is set up!");
                Assert.That(MayTouch(chargePoint, AChange("dns.servers"),                "edit", out var unsigned),   Is.False, "Nobody signed, and a key was changed!");

            });

        }

        #endregion

        #region TheTableNamesEveryActionOnce()

        /// <summary>
        /// Whatever a request says, one row of its action fits it; and the two
        /// actions decided key by key are not in the table at all.
        /// </summary>
        [Test]
        public void TheTableNamesEveryActionOnce()
        {

            var table = ChargePointActionPermissions.Default().ToArray();

            Assert.Multiple(() => {

                // Each action has exactly one row without a condition: what it needs when no other row fits.
                foreach (var action in table.Select(row => row.Action).Distinct())
                    Assert.That(table.Count(row => row.Action == action && row.Where.Count == 0), Is.EqualTo(1), action);

                Assert.That(table.Select(row => row.Action), Does.Not.Contain("ChangeConfiguration").And.Not.Contain("GetConfiguration"));

            });

        }

        #endregion

    }

}
