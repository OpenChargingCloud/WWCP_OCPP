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

using System.Security.Cryptography;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.KeyPins
{

    /// <summary>
    /// A local controller answers its own Device Model - EVQICtrlr[TLSServer] of the white paper "Key
    /// Pins and TLS 1.3 Raw Public Keys for Local Controllers" (sections 5.5 and 7.4) - for the requests
    /// addressed to itself, and forwards the same requests addressed to a station behind it, untouched
    /// and unanswered by its own.
    /// </summary>
    /// <remarks>
    /// The CSMS, the local controller "GD-NN001" in overlay networking, and the station "GD-CP001"
    /// behind it, as the forwarding tests have them. The controller's keys come from a fake source.
    /// </remarks>
    [TestFixture]
    public class LocalControllerOwnDeviceModelTests : AChargingStationWithNetworkingNodeTests
    {

        #region Data

        private static readonly String pinA = APin();
        private static readonly String pinB = APin();
        private static readonly String pinC = APin();

        private static readonly DateTimeOffset created = DateTimeOffset.Parse("2026-11-01T08:00:00Z");

        #endregion

        #region (class) ATLSServer / AReadOnlyTLSServer

        /// <summary>
        /// The keys of a Local Controller's listener, as LocalController keeps them: a current key, an
        /// announced next one, and the switch from the one to the other - remembering each request that
        /// asked for a switch, and refusing all of them where told to.
        /// </summary>
        private class AReadOnlyTLSServer : IEVQICtrlrSource
        {

            public String       Current   { get; set; } = pinA;
            public String?      Next      { get; set; } = pinB;

            public IEnumerable<String> TLSServerVariables
                => [ "KeyPins", "NextKeyPin", "KeyCreated", "RawPublicKeys", "ActiveKeyPin" ];

            public String? TLSServerValue(String Variable)

                => Variable switch {
                       "KeyPins"        => Next is not null ? $"{Current},{Next}" : Current,
                       "NextKeyPin"     => Next ?? "",
                       "KeyCreated"     => created.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                       "RawPublicKeys"  => "Off",
                       "ActiveKeyPin"   => Current,
                       _                => null
                   };

        }

        private sealed class ATLSServer : AReadOnlyTLSServer, ITLSServerKeySwitch
        {

            public List<(String Pin, SetVariablesRequest? Request)>  Switches  { get; } = [];

            public StatusInfo?  Refusal  { get; set; }

            public Task<StatusInfo?> SwitchToNextKeyAsync(KeyPin                NextKeyPin,
                                                          SetVariablesRequest?  Request,
                                                          CancellationToken     CancellationToken)
            {

                Switches.Add((NextKeyPin.ToHex(), Request));

                if (Refusal is not null)
                    return Task.FromResult<StatusInfo?>(Refusal);

                if (NextKeyPin.ToHex() == Next)
                {
                    Current = Next;
                    Next    = null;
                }

                return Task.FromResult<StatusInfo?>(null);

            }

        }

        #endregion

        #region SetUp / TearDown

        /// <summary>
        /// The controller's EVQICtrlr, from a fake source a test may replace.
        /// </summary>
        private ATLSServer? tlsServer;

        [SetUp]
        public void GiveTheControllerItsKeys()
        {

            Assert.That(localController1, Is.Not.Null);

            tlsServer = new ATLSServer();

            localController1!.RemoveComponent(nameof(EVQICtrlr), EVQICtrlr.TLSServer);
            Assert.That(localController1.AddComponent(new EVQICtrlr(tlsServer)), Is.True);

            // Charging station 1 is reachable via the local controller.
            testCSMS1!.Routing.AddOrUpdateStaticRouting(chargingStation1!.Id, localController1.Id);

        }

        [TearDown]
        public void TakeTheControllersKeysAway()
        {
            localController1?.RemoveComponent(nameof(EVQICtrlr), EVQICtrlr.TLSServer);
        }

        #endregion

        #region (private static) APin() / Entry(Variable, Value)

        private static String APin()
        {
            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            return KeyPin.FromKey(key).ToHex();
        }

        private static SetVariableData Entry(String Variable, String Value, String? Instance = EVQICtrlr.TLSServer)

            => new (
                   Component:        new Component(Name: nameof(EVQICtrlr), Instance: Instance),
                   Variable:         new Variable (Name: Variable),
                   AttributeValue:   Value
               );

        private static GetVariableData Asked(String Variable, String? Instance = EVQICtrlr.TLSServer)

            => new (
                   Component:   new Component(Name: nameof(EVQICtrlr), Instance: Instance),
                   Variable:    new Variable (Name: Variable)
               );

        #endregion


        #region TryCheck_ActiveKeyPinIsTheOnlyWritableOne_Test()

        /// <summary>
        /// ActiveKeyPin takes one key pin, with or without colons, and nothing else; every other variable
        /// of EVQICtrlr[TLSServer] is read-only, and one it does not have is not found. Writable it is only
        /// where the source switches keys.
        /// </summary>
        [Test]
        public void TryCheck_ActiveKeyPinIsTheOnlyWritableOne_Test()
        {

            static String? ReasonOf(Boolean Fits, StatusInfo? Refusal)
                => Fits ? null : Refusal?.ReasonCode;

            static MutabilityTypes? MutabilityOf(ComponentConfig Component, String Variable)
                => Component.VariableConfigs.Single(variable => variable.Name == Variable).Attributes?.Mutability;

            var switching = new EVQICtrlr(new ATLSServer());
            var showing   = new EVQICtrlr(new AReadOnlyTLSServer());

            Assert.Multiple(() => {

                Assert.That(ReasonOf(EVQICtrlr.TryCheck("ActiveKeyPin",   pinB,                                  out var refusal), refusal), Is.Null);
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("activekeypin",   KeyPin.Parse(pinB).ToHexWithColons(),  out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("ActiveKeyPin",   "",                                    out refusal),     refusal), Is.EqualTo("InvalidValue"), "no pin");
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("ActiveKeyPin",   $"{pinA},{pinB}",                      out refusal),     refusal), Is.EqualTo("InvalidValue"), "two pins");
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("ActiveKeyPin",   pinB[..62],                            out refusal),     refusal), Is.EqualTo("InvalidValue"), "62 digits");

                Assert.That(ReasonOf(EVQICtrlr.TryCheck("KeyPins",        pinB,                                  out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("NextKeyPin",     pinB,                                  out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("KeyCreated",     "2026-11-12T00:00:00Z",                out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("RawPublicKeys",  "On",                                  out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(EVQICtrlr.TryCheck("KeyPin",         pinB,                                  out refusal),     refusal), Is.EqualTo("NotFound"));

                Assert.That(MutabilityOf(switching, "ActiveKeyPin"),    Is.EqualTo(MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(switching, "KeyPins"),         Is.EqualTo(MutabilityTypes.ReadOnly));
                Assert.That(switching.SetVariablesSource,               Is.Not.Null);

                Assert.That(MutabilityOf(showing,   "ActiveKeyPin"),    Is.EqualTo(MutabilityTypes.ReadOnly), "a source that switches nothing");
                Assert.That(showing.SetVariablesSource,                 Is.Null);

                Assert.That(switching.Instance,                         Is.EqualTo("TLSServer"));
                Assert.That(switching.VariableConfigs.Single(variable => variable.Name == "RawPublicKeys").Characteristics?.ValuesList,
                                                                        Is.EqualTo(new[] { "Off", "On" }));
                Assert.That(switching.VariableConfigs.Single(variable => variable.Name == "KeyCreated").Characteristics?.DataType,
                                                                        Is.EqualTo(DataTypes.DateTime));

            });

        }

        #endregion


        #region GetVariables_ToTheController_IsAnsweredByItsOwnDeviceModel_Test()

        /// <summary>
        /// GetVariables addressed to the local controller is answered from its own EVQICtrlr, read from the
        /// source at that moment, and goes no further: neither FORWARD nor the station behind it sees it.
        /// </summary>
        [Test]
        public async Task GetVariables_ToTheController_IsAnsweredByItsOwnDeviceModel_Test()
        {

            var forwarded  = 0;
            var atStation  = 0;

            localController1!.OCPP.FORWARD.OnGetVariablesRequestReceived += (timestamp, sender, connection, request, ct) => { Interlocked.Increment(ref forwarded); return Task.CompletedTask; };
            chargingStation1!.OCPP.IN.     OnGetVariablesRequestReceived += (timestamp, sender, connection, request, ct) => { Interlocked.Increment(ref atStation); return Task.CompletedTask; };

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(localController1.Id),
                                     VariableData:   [
                                                         Asked("KeyPins"),
                                                         Asked("NextKeyPin"),
                                                         Asked("KeyCreated"),
                                                         Asked("RawPublicKeys"),
                                                         Asked("ActiveKeyPin"),
                                                         Asked("KeyPin", "TLSClient")
                                                     ]
                                 );

            Assert.Multiple(() => {

                Assert.That(forwarded,                                                           Is.EqualTo(0), "FORWARD saw a request for the controller itself!");
                Assert.That(atStation,                                                           Is.EqualTo(0), "The station got a request for the controller!");

                Assert.That(response.Result.ResultCode,                                         Is.EqualTo(OCPP.ResultCode.OK));
                Assert.That(response.Results.Take(5).Select(result => result.AttributeStatus),  Is.All.EqualTo(GetVariableStatus.Accepted));
                Assert.That(response.Results.Take(5).Select(result => result.AttributeValue),   Is.EqualTo(new[] { $"{pinA},{pinB}", pinB, "2026-11-01T08:00:00Z", "Off", pinA }));
                Assert.That(response.Results.LastOrDefault()?.AttributeStatus,                   Is.EqualTo(GetVariableStatus.UnknownComponent), "EVQICtrlr[TLSClient] is a station's");

            });

        }

        #endregion

        #region SetVariables_ActiveKeyPin_SwitchesToTheNextKeyOnly_Test()

        /// <summary>
        /// Writing the announced next key's pin into ActiveKeyPin switches to it, with the request handed
        /// to the switch; writing it again - now the current key's - is accepted and switches nothing;
        /// any other pin, no pin, and the read-only variables are refused with the reason.
        /// </summary>
        [Test]
        public async Task SetVariables_ActiveKeyPin_SwitchesToTheNextKeyOnly_Test()
        {

            var toTheNext   = await SetVariables(localController1!.Id, Entry("ActiveKeyPin", KeyPin.Parse(pinB).ToHexWithColons()));
            var again       = await SetVariables(localController1. Id, Entry("ActiveKeyPin", pinB));
            var toAnother   = await SetVariables(localController1. Id, Entry("ActiveKeyPin", pinC));
            var toTheOld    = await SetVariables(localController1. Id, Entry("ActiveKeyPin", pinA));
            var toNothing   = await SetVariables(localController1. Id, Entry("ActiveKeyPin", "next"));
            var readOnly    = await SetVariables(localController1. Id, Entry("KeyPins",      pinC));

            Assert.Multiple(() => {

                Assert.That(toTheNext.AttributeStatus,                      Is.EqualTo(SetVariableStatus.Accepted), toTheNext.AttributeStatusInfo?.AdditionalInfo);
                Assert.That(again.    AttributeStatus,                      Is.EqualTo(SetVariableStatus.Accepted), again.    AttributeStatusInfo?.AdditionalInfo);

                Assert.That(tlsServer!.Switches.Select(@switch => @switch.Pin),
                                                                            Is.EqualTo(new[] { pinB }), "one switch, to the next key, and none for the pin already current");
                Assert.That(tlsServer.Switches[0].Request?.NetworkPath.Source,
                                                                            Is.EqualTo(testCSMS1!.Id), "the switch is handed the request with its sender");
                Assert.That(tlsServer.Current,                              Is.EqualTo(pinB));

                Assert.That(toAnother.AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(toAnother.AttributeStatusInfo?.ReasonCode,      Is.EqualTo("InvalidValue"));
                Assert.That(toAnother.AttributeStatusInfo?.AdditionalInfo ?? "",
                                                                            Does.Contain("neither the key pin of the current key"));

                Assert.That(toTheOld. AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected), "the key switched away from is no way back");

                Assert.That(toNothing.AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(toNothing.AttributeStatusInfo?.ReasonCode,      Is.EqualTo("InvalidValue"));

                Assert.That(readOnly. AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(readOnly. AttributeStatusInfo?.ReasonCode,      Is.EqualTo("ReadOnly"));

                Assert.That(tlsServer.Switches.Count,                       Is.EqualTo(1));
                Assert.That(tlsServer.Current,                              Is.EqualTo(pinB));

            });

        }

        #endregion

        #region SetVariables_ASwitchTheSourceRefuses_IsRejectedWithItsReason_Test()

        /// <summary>
        /// Where the source refuses the switch - the right "certificates:edit" missing, say - the write is
        /// rejected with the source's reason, and the key stays.
        /// </summary>
        [Test]
        public async Task SetVariables_ASwitchTheSourceRefuses_IsRejectedWithItsReason_Test()
        {

            tlsServer!.Refusal = new StatusInfo("NotAuthorized", "Switching the server key needs certificates:edit.");

            var result = await SetVariables(localController1!.Id, Entry("ActiveKeyPin", pinB));

            Assert.Multiple(() => {
                Assert.That(result.AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(result.AttributeStatusInfo?.ReasonCode,      Is.EqualTo("NotAuthorized"));
                Assert.That(result.AttributeStatusInfo?.AdditionalInfo ?? "",
                                                                         Does.Contain("certificates:edit"));
                Assert.That(tlsServer.Current,                           Is.EqualTo(pinA));
            });

        }

        #endregion

        #region GetReport_ToTheController_ReportsItsOwnComponent_Test()

        /// <summary>
        /// GetReport and GetBaseReport addressed to the local controller are accepted, and the report
        /// follows from the controller itself: EVQICtrlr[TLSServer] with its values and characteristics; a
        /// configuration inventory lists only what a CSMS may write.
        /// </summary>
        [Test]
        public async Task GetReport_ToTheController_ReportsItsOwnComponent_Test()
        {

            var reportParts  = NotifyReportsOf(4901);
            var report       = await testCSMS1!.GetReport(
                                         Destination:          SourceRouting.To(localController1!.Id),
                                         GetReportRequestId:   4901,
                                         ComponentCriteria:    [],
                                         ComponentVariables:   [ new ComponentVariable(Component: new Component(Name: nameof(EVQICtrlr))) ]
                                     );
            var reportData   = await CompleteReport(reportParts);

            var configParts  = NotifyReportsOf(4902);
            var config       = await testCSMS1!.GetBaseReport(
                                         Destination:              SourceRouting.To(localController1.Id),
                                         GetBaseReportRequestId:   4902,
                                         ReportBase:               ReportBase.ConfigurationInventory
                                     );
            var configData   = await CompleteReport(configParts);

            var activeKeyPin = reportData.Single(data => data.Variable.Name == "ActiveKeyPin");

            Assert.Multiple(() => {

                Assert.That(report.Status,                                                     Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(reportParts.Select(part => part.NetworkPath.Source).Distinct(),    Is.EqualTo(new[] { localController1.Id }), "the report comes from the controller");

                Assert.That(reportData.Select(data => data.Component.Name).Distinct(),         Is.EqualTo(new[] { nameof(EVQICtrlr) }));
                Assert.That(reportData.Select(data => data.Variable.Name),                     Is.EquivalentTo(new[] { "KeyPins", "NextKeyPin", "KeyCreated", "RawPublicKeys", "ActiveKeyPin" }));
                Assert.That(activeKeyPin.VariableAttributes.Single().Value,                    Is.EqualTo(pinA));
                Assert.That(activeKeyPin.VariableAttributes.Single().Mutability,               Is.EqualTo(MutabilityTypes.ReadWrite));

                Assert.That(config.Status,                                                     Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(configData.Where(data => data.Component.Name == nameof(EVQICtrlr)).Select(data => data.Variable.Name),
                                                                                               Is.EqualTo(new[] { "ActiveKeyPin" }));

            });

        }

        #endregion


        #region SetVariables_ForAStationBehindTheController_IsForwardedUntouched_Test()

        /// <summary>
        /// The same SetVariables and GetVariables addressed to the station behind the local controller are
        /// forwarded, and answered by the station's own Device Model, which has no EVQICtrlr: the
        /// controller's own component neither answers nor changes, and its keys are not switched.
        /// </summary>
        [Test]
        public async Task SetVariables_ForAStationBehindTheController_IsForwardedUntouched_Test()
        {

            var forwarded  = 0;
            var atStation  = 0;

            localController1!.OCPP.FORWARD.OnSetVariablesRequestReceived += (timestamp, sender, connection, request, ct) => { Interlocked.Increment(ref forwarded); return Task.CompletedTask; };
            chargingStation1!.OCPP.IN.     OnSetVariablesRequestReceived += (timestamp, sender, connection, request, ct) => { Interlocked.Increment(ref atStation); return Task.CompletedTask; };

            var set  = await SetVariables(chargingStation1.Id, Entry("ActiveKeyPin", pinB));

            var get  = await testCSMS1!.GetVariables(
                                 Destination:    SourceRouting.To(chargingStation1.Id),
                                 VariableData:   [ Asked("KeyPins") ]
                             );

            Assert.Multiple(() => {

                Assert.That(set.AttributeStatus,                     Is.EqualTo(SetVariableStatus.UnknownComponent), "the station's answer, not the controller's");
                Assert.That(get.Results.Single().AttributeStatus,    Is.EqualTo(GetVariableStatus.UnknownComponent), "the station's answer, not the controller's");

                Assert.That(forwarded,                               Is.EqualTo(1));
                Assert.That(atStation,                               Is.EqualTo(1));

                Assert.That(tlsServer!.Switches,                     Is.Empty, "the controller's key was switched by a request for a station!");
                Assert.That(tlsServer.Current,                       Is.EqualTo(pinA));

            });

        }

        #endregion


        #region (private) SetVariables(Destination, Entry)

        private async Task<SetVariableResult> SetVariables(NetworkingNode_Id  Destination,
                                                           SetVariableData    Entry)
        {

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(Destination),
                                     VariableData:   [ Entry ]
                                 );

            Assert.That(response.Result.ResultCode, Is.EqualTo(OCPP.ResultCode.OK), response.Result.Description);

            return response.SetVariableResults.Single();

        }

        #endregion

        #region (private) NotifyReportsOf(RequestId) / CompleteReport(Parts)

        /// <summary>
        /// Collect the NotifyReport requests the CSMS receives for the given report.
        /// </summary>
        private ConcurrentList<NotifyReportRequest> NotifyReportsOf(Int32 RequestId)
        {

            var parts = new ConcurrentList<NotifyReportRequest>();

            testCSMS1!.OCPP.IN.OnNotifyReportRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {

                if (notifyReportRequest.NotifyReportRequestId == RequestId)
                    parts.TryAdd(notifyReportRequest);

                return Task.CompletedTask;

            };

            return parts;

        }

        /// <summary>
        /// Wait for the last part of a report, and return all of its report data.
        /// </summary>
        private static async Task<List<ReportData>> CompleteReport(ConcurrentList<NotifyReportRequest> Parts)
        {

            var deadline = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);

            while (DateTimeOffset.UtcNow < deadline &&
                   !(Parts.Any(part => part.ToBeContinued != true) &&
                     Parts.Count == Parts.Max(part => part.SequenceNumber) + 1))
            {
                await Task.Delay(20);
            }

            Assert.That(Parts.Any(part => part.ToBeContinued != true), Is.True, "The last part of the report never came!");

            return [.. Parts.OrderBy(part => part.SequenceNumber).SelectMany(part => part.ReportData)];

        }

        #endregion

    }

}
