/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The DNS controllers of the Secure DNS white paper, as a CSMS reads them:
    /// values from their source at every read, one slot per name server, the
    /// unused ones included - and writable where the paper allows it and the
    /// source takes a change, all writes of a request at once.
    /// </summary>
    [TestFixture]
    public class DNSControllerTests : AChargingStationTests
    {

        #region (class) ADNSSource

        /// <summary>
        /// A source holding its values in dictionaries a test changes, with two
        /// name servers in sixteen slots, and without EncryptedOnly and
        /// ServerSource, as a station whose node does not have them yet.
        /// </summary>
        private sealed class ADNSSource : IDNSControllerSource
        {

            public Dictionary<String, String>                      Client   { get; } = new (StringComparer.OrdinalIgnoreCase) {
                ["Enabled"]           = "true",
                ["QueryTimeout"]      = "2.5",
                ["MaxRetries"]        = "1",
                ["RecursionDesired"]  = "Set",
                ["UseCache"]          = "true",
                ["DNSSECOK"]          = "false",
                ["FollowCNAMEs"]      = "true",
                ["MaxCNAMEFollows"]   = "8",
                ["MaxServers"]        = "16",
                ["UDPPayloadSize"]    = "1232",
                ["ClientSubnet"]      = ""
            };

            public List<Dictionary<String, String>>                Servers  { get; } = [
                new (StringComparer.OrdinalIgnoreCase) { ["Address"] = "192.0.2.53",   ["Port"] = "853", ["Transport"] = "TLS", ["RootPins"] = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e", ["CertificateStatus"] = "Accepted" },
                new (StringComparer.OrdinalIgnoreCase) { ["Address"] = "2001:db8::53", ["Port"] = "53",  ["Transport"] = "UDP" }
            ];

            public IEnumerable<String>  ClientVariables
                => Client.Keys;

            public IEnumerable<String>  ServerVariables
                => [ "Address", "Port", "Transport", "QueryTimeout", "CertificatePins", "RootPins", "OnPinMismatch", "TrustOnFirstUse",
                     "Problem", "CertificateStatus", "CertificateFingerprint", "RootFingerprint", "KnownSince" ];

            public UInt16               Slots
                => 16;

            public String? ClientValue(String Variable)

                => Client.TryGetValue(Variable, out var value)
                       ? value
                       : null;

            public String? ServerValue(UInt16 Slot, String Variable)

                => Slot < 1 || Slot > Slots || !ServerVariables.Contains(Variable, StringComparer.OrdinalIgnoreCase)
                       ? null
                       : Slot <= Servers.Count
                             ? Servers[Slot - 1].GetValueOrDefault(Variable, "")
                             : "";

        }

        #endregion


        #region GetVariables_ReadsTheSourceAtEveryRead_Test()

        /// <summary>
        /// GetVariables answers what the source says at that moment, for the
        /// client, for a slot with a server and for an unused one.
        /// </summary>
        [Test]
        public async Task GetVariables_ReadsTheSourceAtEveryRead_Test()
        {

            var (chargingStation, source) = AStationWithDNS();

            var first   = await GetVariables(chargingStation,
                                             (nameof(DNSClientController), null, "QueryTimeout"),
                                             (nameof(DNSServerController), "1",  "Address"),
                                             (nameof(DNSServerController), "1",  "RootPins"),
                                             (nameof(DNSServerController), "2",  "Transport"),
                                             (nameof(DNSServerController), "3",  "Address"));

            source.Client["QueryTimeout"]   = "3";
            source.Servers[0]["Address"]    = "198.51.100.53";

            var second  = await GetVariables(chargingStation,
                                             (nameof(DNSClientController), null, "QueryTimeout"),
                                             (nameof(DNSServerController), "1",  "Address"));

            Assert.Multiple(() => {

                Assert.That(first.Select(result => result.AttributeStatus),   Is.All.EqualTo(GetVariableStatus.Accepted));
                Assert.That(first.Select(result => result.AttributeValue),    Is.EqualTo(new[] { "2.5", "192.0.2.53", source.Servers[0]["RootPins"], "UDP", "" }));

                Assert.That(second.Select(result => result.AttributeValue),   Is.EqualTo(new[] { "3", "198.51.100.53" }));

            });

        }

        #endregion

        #region GetVariables_WhatIsNotThereIsUnknown_Test()

        /// <summary>
        /// A slot beyond the last is an unknown component, and a variable of the
        /// paper that the source cannot answer is an unknown variable, not an
        /// empty value.
        /// </summary>
        [Test]
        public async Task GetVariables_WhatIsNotThereIsUnknown_Test()
        {

            var (chargingStation, _) = AStationWithDNS();

            var results = await GetVariables(chargingStation,
                                             (nameof(DNSServerController), "17", "Address"),
                                             (nameof(DNSClientController), null, "EncryptedOnly"),
                                             (nameof(DNSServerController), "1",  "AuthenticationName"));

            Assert.That(results.Select(result => result.AttributeStatus),
                        Is.EqualTo(new[] { GetVariableStatus.UnknownComponent, GetVariableStatus.UnknownVariable, GetVariableStatus.UnknownVariable }));

        }

        #endregion

        #region SetVariables_IsReadOnlyForNow_Test()

        /// <summary>
        /// A source that takes no changes leaves every variable read-only: a
        /// CSMS is told so, and nothing reaches the source.
        /// </summary>
        [Test]
        public async Task SetVariables_IsReadOnlyForNow_Test()
        {

            var (chargingStation, source) = AStationWithDNS();

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(chargingStation.Id),
                                     VariableData:   [
                                                         new SetVariableData(
                                                             Component:        new Component(Name: nameof(DNSServerController), Instance: "1"),
                                                             Variable:         new Variable (Name: "Address"),
                                                             AttributeValue:   "203.0.113.53"
                                                         ),
                                                         new SetVariableData(
                                                             Component:        new Component(Name: nameof(DNSClientController)),
                                                             Variable:         new Variable (Name: "Enabled"),
                                                             AttributeValue:   "false"
                                                         )
                                                     ]
                                 );

            Assert.Multiple(() => {
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatus),              Is.All.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("ReadOnly"));
                Assert.That(source.Servers[0]["Address"],                                                       Is.EqualTo("192.0.2.53"));
                Assert.That(source.Client["Enabled"],                                                           Is.EqualTo("true"));
            });

        }

        #endregion

        #region GetBaseReport_ListsEverySlotWithItsCharacteristics_Test()

        /// <summary>
        /// A full inventory lists the client and all sixteen slots, the unused
        /// ones with empty values, each variable with the characteristics of the
        /// paper; a configuration inventory lists none of them while they are
        /// read-only.
        /// </summary>
        [Test]
        public async Task GetBaseReport_ListsEverySlotWithItsCharacteristics_Test()
        {

            var (chargingStation, _) = AStationWithDNS();

            var fullParts     = NotifyReportsOf(4801);
            var full          = await testCSMS1!.GetBaseReport(
                                          Destination:              SourceRouting.To(chargingStation.Id),
                                          GetBaseReportRequestId:   4801,
                                          ReportBase:               ReportBase.FullInventory
                                      );
            var fullReport    = await CompleteReport(fullParts);

            var configParts   = NotifyReportsOf(4802);
            var config        = await testCSMS1!.GetBaseReport(
                                          Destination:              SourceRouting.To(chargingStation.Id),
                                          GetBaseReportRequestId:   4802,
                                          ReportBase:               ReportBase.ConfigurationInventory
                                      );
            var configReport  = await CompleteReport(configParts);

            var servers       = fullReport.Where(reportData => reportData.Component.Name == nameof(DNSServerController)).ToList();
            var queryTimeout  = fullReport.Single(reportData => reportData.Component.Name == nameof(DNSClientController) &&
                                                                reportData.Variable. Name == "QueryTimeout");
            var transport     = servers.   First (reportData => reportData.Variable. Name == "Transport");

            Assert.Multiple(() => {

                Assert.That(full.Status,                                                                   Is.EqualTo(GenericDeviceModelStatus.Accepted));

                Assert.That(fullReport.Count(reportData => reportData.Component.Name == nameof(DNSClientController)), Is.EqualTo(11));
                Assert.That(servers.Select(reportData => reportData.Component.Instance).Distinct(),        Is.EquivalentTo(Enumerable.Range(1, 16).Select(slot => slot.ToString())));
                Assert.That(servers.Count,                                                                 Is.EqualTo(16 * 13));

                Assert.That(servers.Where (reportData => reportData.Component.Instance == "16").
                                    Select(reportData => reportData.VariableAttributes.Single().Value),    Is.All.EqualTo(""));

                Assert.That(queryTimeout.VariableAttributes.Single().Value,                                Is.EqualTo("2.5"));
                Assert.That(queryTimeout.VariableAttributes.Single().Mutability,                           Is.EqualTo(MutabilityTypes.ReadOnly));
                Assert.That(queryTimeout.VariableCharacteristics?.DataType,                                Is.EqualTo(DataTypes.Decimal));
                Assert.That(queryTimeout.VariableCharacteristics?.MinLimit,                                Is.EqualTo(0.1m));
                Assert.That(queryTimeout.VariableCharacteristics?.MaxLimit,                                Is.EqualTo(120m));

                Assert.That(transport.VariableCharacteristics?.DataType,                                   Is.EqualTo(DataTypes.OptionList));
                Assert.That(transport.VariableCharacteristics?.ValuesList,                                 Is.EqualTo(new[] { "UDP", "TCP", "TLS", "HTTPS", "HTTPS_GET" }));

                Assert.That(configReport.Any(reportData => reportData.Component.Name.StartsWith("DNS")),   Is.False);

            });

        }

        #endregion

        #region VariableAttribute_KeepsAnEmptyValue_Test()

        /// <summary>
        /// A reported empty value is read as an empty value, not as a missing
        /// one, which OCPP 2.1 allows only for a write-only variable; and a value
        /// is read as it was sent, spaces included.
        /// </summary>
        [Test]
        public void VariableAttribute_KeepsAnEmptyValue_Test()
        {

            Assert.Multiple(() => {

                Assert.That(VariableAttribute.TryParse(Newtonsoft.Json.Linq.JObject.Parse("""{ "value": "",       "mutability": "ReadOnly"  }"""), out var empty,     out var error),  Is.True, error);
                Assert.That(empty?.Value,     Is.EqualTo(""));

                Assert.That(VariableAttribute.TryParse(Newtonsoft.Json.Linq.JObject.Parse("""{ "value": " a b ",  "mutability": "ReadOnly"  }"""), out var spaced,    out error),      Is.True, error);
                Assert.That(spaced?.Value,    Is.EqualTo(" a b "));

                Assert.That(VariableAttribute.TryParse(Newtonsoft.Json.Linq.JObject.Parse("""{                    "mutability": "WriteOnly" }"""), out var writeOnly, out error),      Is.True, error);
                Assert.That(writeOnly?.Value, Is.Null);

                // And the way out: an empty value is written, a missing one is not.
                Assert.That(new VariableAttribute(Value: "",   Mutability: MutabilityTypes.ReadOnly). ToJSON().Value<String>("value"),   Is.EqualTo(""));
                Assert.That(new VariableAttribute(Value: null, Mutability: MutabilityTypes.WriteOnly).ToJSON().ContainsKey("value"),        Is.False);

            });

        }

        #endregion

        #region Constructor_RefusesASlotThatIsNotThere_Test()

        [Test]
        public void Constructor_RefusesASlotThatIsNotThere_Test()
        {

            var source = new ADNSSource();

            Assert.Multiple(() => {
                Assert.That(() => new DNSServerController(source,  0), Throws.InstanceOf<ArgumentOutOfRangeException>());
                Assert.That(() => new DNSServerController(source, 17), Throws.InstanceOf<ArgumentOutOfRangeException>());
                Assert.That(DNSServerController.AllOf(source).Select(controller => controller.Instance),
                            Is.EqualTo(Enumerable.Range(1, 16).Select(slot => slot.ToString())));
            });

        }

        #endregion

        #region AWritingSourceMakesWritableWhatThePaperAllows_Test()

        /// <summary>
        /// A source that takes changes makes the variables it writes read-write -
        /// those the paper lets a CSMS write, and no others - and takes the
        /// writes of its controllers. A source that takes none leaves all of
        /// them read-only.
        /// </summary>
        [Test]
        public void AWritingSourceMakesWritableWhatThePaperAllows_Test()
        {

            var writer          = new ADNSWriter(new ADNSSource());
            var client          = new DNSClientController(writer);
            var server          = new DNSServerController(writer, 1);
            var readOnlyClient  = new DNSClientController(new ADNSSource());

            MutabilityTypes? MutabilityOf(ComponentConfig Component, String Variable)
                => Component.VariableConfigs.Single(variable => variable.Name == Variable).Attributes?.Mutability;

            Assert.Multiple(() => {

                Assert.That(MutabilityOf(client, "Enabled"),                                 Is.EqualTo(MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(client, "MaxServers"),                              Is.EqualTo(MutabilityTypes.ReadOnly),  "the paper says read-only");
                Assert.That(MutabilityOf(client, "DNSSECOK"),                                Is.EqualTo(MutabilityTypes.ReadOnly),  "the source does not write it");
                Assert.That(MutabilityOf(server, "Address"),                                 Is.EqualTo(MutabilityTypes.ReadWrite));
                Assert.That(MutabilityOf(server, "Problem"),                                 Is.EqualTo(MutabilityTypes.ReadOnly),  "the paper says read-only");

                Assert.That(client.SetVariablesSource,                                       Is.SameAs(writer));
                Assert.That(server.SetVariablesSource,                                       Is.SameAs(writer));

                Assert.That(readOnlyClient.VariableConfigs.Select(variable => variable.Attributes?.Mutability),
                                                                                             Is.All.EqualTo(MutabilityTypes.ReadOnly));
                Assert.That(readOnlyClient.SetVariablesSource,                               Is.Null);

            });

        }

        #endregion

        #region TryCheck_SaysWhatThePaperAllows_Test()

        /// <summary>
        /// A value is checked against the type, the limits and the values the
        /// paper gives its variable, and refused as a SetVariables result says it.
        /// </summary>
        [Test]
        public void TryCheck_SaysWhatThePaperAllows_Test()
        {

            String? ReasonOf(Boolean Fits, StatusInfo? Refusal)
                => Fits ? null : Refusal?.ReasonCode;

            var seventeen = String.Join(",", Enumerable.Range(0, 17).Select(index => new String((Char) ('a' + index % 6), 64)));

            Assert.Multiple(() => {

                Assert.That(ReasonOf(DNSClientController.TryCheck("Enabled",          "TRUE",  out var refusal), refusal), Is.Null);
                Assert.That(ReasonOf(DNSClientController.TryCheck("Enabled",          "yes",   out refusal),     refusal), Is.EqualTo("InvalidValue"));
                Assert.That(ReasonOf(DNSClientController.TryCheck("QueryTimeout",     "0.05",  out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));
                Assert.That(ReasonOf(DNSClientController.TryCheck("QueryTimeout",     "",      out refusal),     refusal), Is.EqualTo("InvalidValue"));
                Assert.That(ReasonOf(DNSClientController.TryCheck("MaxRetries",       "256",   out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));
                Assert.That(ReasonOf(DNSClientController.TryCheck("RecursionDesired", "unset", out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(DNSClientController.TryCheck("NoSuchVariable",   "1",     out refusal),     refusal), Is.EqualTo("NotFound"));

                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "Port",          "0",     out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "Port",          "",      out refusal),     refusal), Is.Null, "the port of the transport");
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "QueryTimeout",  "",      out refusal),     refusal), Is.Null, "the client's");
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "Address",       "",      out refusal),     refusal), Is.Null, "an unused slot");
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "Transport",     "tls",   out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "Transport",     "QUIC",  out refusal),     refusal), Is.EqualTo("InvalidValue"));
                Assert.That(ReasonOf(DNSServerController.TryCheck(2, "RootPins",      seventeen, out refusal),   refusal), Is.EqualTo("ValueOutOfRange"));

            });

            DNSServerController.TryCheck(2, "Port", "70000", out var named);

            Assert.That(named?.AdditionalInfo, Does.Contain("DNSServerController[2].Port"));

        }

        #endregion

        #region SetVariables_ReachesAWritingSourceAtOnce_Test()

        /// <summary>
        /// The writes of a request to both controllers reach a writing source in
        /// one call, in request order; a variable it does not write stays
        /// read-only.
        /// </summary>
        [Test]
        public async Task SetVariables_ReachesAWritingSourceAtOnce_Test()
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var writer = new ADNSWriter(new ADNSSource());

            Assert.That(chargingStation1!.AddComponent(new DNSClientController(writer)), Is.True);

            foreach (var server in DNSServerController.AllOf(writer))
                Assert.That(chargingStation1.AddComponent(server), Is.True);

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(chargingStation1.Id),
                                     VariableData:   [
                                                         new SetVariableData(Component: new Component(Name: nameof(DNSServerController), Instance: "3"), Variable: new Variable(Name: "Address"), AttributeValue: "203.0.113.53"),
                                                         new SetVariableData(Component: new Component(Name: nameof(DNSServerController), Instance: "3"), Variable: new Variable(Name: "Port"), AttributeValue: "54"),
                                                         new SetVariableData(Component: new Component(Name: nameof(DNSClientController)), Variable: new Variable(Name: "Enabled"), AttributeValue: "false")
                                                     ]
                                 );

            Assert.Multiple(() => {

                Assert.That(writer.Calls,                                                    Is.EqualTo(new[] { "DNSServerController(3).Address=203.0.113.53|DNSClientController.Enabled=false" }));
                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatus),
                                                                                             Is.EqualTo(new[] { SetVariableStatus.Accepted, SetVariableStatus.Rejected, SetVariableStatus.Accepted }));
                Assert.That(response.SetVariableResults.ElementAt(1).AttributeStatusInfo?.ReasonCode,
                                                                                             Is.EqualTo("ReadOnly"));

            });

        }

        #endregion

        #region (class) ADNSWriter

        /// <summary>
        /// A source that reads from another and takes writes of Enabled,
        /// MaxServers (which the paper does not let a CSMS write), Address and
        /// Problem (neither that), remembering each call.
        /// </summary>
        private sealed class ADNSWriter(ADNSSource Values) : IDNSControllerSource, ISetVariablesSource
        {

            public List<String>         Calls                    { get; } = [];

            public IEnumerable<String>  ClientVariables          => Values.ClientVariables;
            public IEnumerable<String>  ServerVariables          => Values.ServerVariables;
            public IEnumerable<String>  WritableClientVariables  => [ "Enabled", "MaxServers" ];
            public IEnumerable<String>  WritableServerVariables  => [ "Address", "Problem" ];
            public UInt16               Slots                    => Values.Slots;

            public String? ClientValue(String Variable)
                => Values.ClientValue(Variable);

            public String? ServerValue(UInt16 Slot, String Variable)
                => Values.ServerValue(Slot, Variable);

            public Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                            DataConsistencyModel             DataConsistencyModel,
                                                                            CancellationToken                CancellationToken)
            {

                Calls.Add(String.Join("|", Entries.Select(entry => $"{entry.Data.Component.Name}{(entry.Data.Component.Instance is not null ? $"({entry.Data.Component.Instance})" : "")}.{entry.Data.Variable.Name}={entry.Data.AttributeValue}")));

                return Task.FromResult<IReadOnlyList<SetVariableResult>>(
                           [.. Entries.Select(entry => new SetVariableResult(AttributeStatus: SetVariableStatus.Accepted, Component: entry.Data.Component, Variable: entry.Data.Variable, AttributeType: entry.Data.AttributeType))]
                       );

            }

        }

        #endregion


        #region (private) AStationWithDNS()

        /// <summary>
        /// The test's charging station with the DNS controllers of a source of
        /// its own.
        /// </summary>
        private (TestChargingStationNode ChargingStation, ADNSSource Source) AStationWithDNS()
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var source = new ADNSSource();

            Assert.That(chargingStation1!.AddComponent(new DNSClientController(source)), Is.True);

            foreach (var server in DNSServerController.AllOf(source))
                Assert.That(chargingStation1.AddComponent(server), Is.True, server.ToString());

            return (chargingStation1, source);

        }

        #endregion

        #region (private) GetVariables(ChargingStation, params Asked)

        private async Task<IReadOnlyList<GetVariableResult>> GetVariables(TestChargingStationNode                                          ChargingStation,
                                                                          params (String Component, String? Instance, String Variable)[]  Asked)
        {

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(ChargingStation.Id),
                                     VariableData:   [.. Asked.Select(asked => new GetVariableData(
                                                                                   Component:   new Component(Name: asked.Component, Instance: asked.Instance),
                                                                                   Variable:    new Variable (Name: asked.Variable)
                                                                               ))]
                                 );

            return [.. response.Results];

        }

        #endregion

        #region (private) NotifyReportsOf(RequestId)

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

        #endregion

        #region (private static) CompleteReport(Parts)

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
