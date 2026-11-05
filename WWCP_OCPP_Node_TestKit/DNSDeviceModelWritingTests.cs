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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.OCPP.Node;

using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What a CSMS changes of the node's name resolution through
    /// SetVariables, against what the node then holds.
    /// </summary>
    /// <remarks>
    /// The first tests go through the OCPP 2.1 node handed to the bridge
    /// into the node, built and not started: nothing is asked of a name
    /// server, and the node has no CSMS connection whose name a change could
    /// cut off. The
    /// pre-flight, which needs one, is tried against a node of words: a
    /// section, a change that is taken or not, and names that resolve or not.
    /// </remarks>
        public abstract class DNSDeviceModelWritingTests : ABridgeTests
    {

        #region Data

        private const String  ARoot            = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  AnotherRoot      = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        private String            directory  = "";
        private IBridgedNode?  station;
        private DNSDeviceModel    model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithTwoNameServers()
        {

            directory  = TestNodes.TemporaryDirectory("dns-device-model-writing");

            var configuration = TestNodes.Offline;

            configuration["dns"] = new JObject(
                                       new JProperty("servers", new JArray(
                                           new JObject(
                                               new JProperty("address",          "192.0.2.53"),
                                               new JProperty("transport",        "TLS"),
                                               new JProperty("rootFingerprint",  ARoot)
                                           ),
                                           new JObject(
                                               new JProperty("address",          "2001:db8::53"),
                                               new JProperty("transport",        "UDP")
                                           )
                                       )),
                                       new JProperty("maxRetries", 2)
                                   );

            station    = Nodes.New(directory, configuration);
            model      = new DNSDeviceModel(station.Node.DNSConfigurationJSON);

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            TestNodes.Remove(directory);

        }

        #endregion


        #region TheClientIsSetAsTheFileWouldSayIt()

        /// <summary>
        /// Settings of DNSClientController go into the node, and are what the
        /// next read says: "Unset" recursion as the node's false.
        /// </summary>
        [Test]
        public async Task TheClientIsSetAsTheFileWouldSayIt()
        {

            var results = await Set(null,
                                    (null, "DNSSECOK",         "true"),
                                    (null, "MaxRetries",       "3"),
                                    (null, "RecursionDesired", "Unset"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                     Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.ClientValue("DNSSECOK"),           Is.EqualTo("true"));
                Assert.That(model.ClientValue("MaxRetries"),         Is.EqualTo("3"));
                Assert.That(model.ClientValue("RecursionDesired"),   Is.EqualTo("Unset"));

                Assert.That(model.ServerValue("dns-1", "Address"),         Is.EqualTo("192.0.2.53"), "the servers are kept");
                Assert.That(model.ServerValue("dns-1", "RootPins"),        Is.EqualTo(ARoot));

            });

        }

        #endregion

        #region ANewServerComesWithItsAddress()

        /// <summary>
        /// A server named anew in Servers is one change with its entries, with
        /// ACID: a server over TLS held to a root, from the start, by the id
        /// the CSMS gave it - written into the file with the others' ids.
        /// </summary>
        [Test]
        public async Task ANewServerComesWithItsAddress()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    ("office", "Transport", "TLS"),
                                    ("office", "RootPins",  AnotherRoot),
                                    ("office", "Address",   "203.0.113.53"),
                                    (null,     "Servers",   "dns-1,dns-2,office"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                     Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.ServerValue("office", "Address"),         Is.EqualTo("203.0.113.53"));
                Assert.That(model.ServerValue("office", "Transport"),       Is.EqualTo("TLS"));
                Assert.That(model.ServerValue("office", "Port"),            Is.EqualTo("853"));
                Assert.That(model.ServerValue("office", "RootPins"),        Is.EqualTo(AnotherRoot));

                Assert.That(model.ClientValue("Servers"),                   Is.EqualTo("dns-1,dns-2,office"));
                Assert.That((OnDisk()["dns"]?["servers"] as JArray)?.Select(server => server.Value<String>("id")),
                                                                            Is.EqualTo(new[] { "dns-1", "dns-2", "office" }), "every id is in the file now");

            });

        }

        #endregion

        #region ANewServerHasNothingToSetWithoutAnAddress()

        /// <summary>
        /// A server named anew in Servers without an Address in the same
        /// request is refused, with the Servers entry that names it; a server
        /// that no Servers names is no component at all.
        /// </summary>
        [Test]
        public async Task ANewServerHasNothingToSetWithoutAnAddress()
        {

            var results  = await Set(OCPP21.DataConsistencyModel.ACID,
                                     (null,     "Servers", "dns-1,dns-2,office"),
                                     ("office", "Port",    "5353"));

            var unnamed  = await Set(null, ("office", "Port", "5353"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InvalidValue"));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("'DNSServerController[office]' is new: set its Address"));
                Assert.That(StatusesOf(unnamed),                                       Is.EqualTo(new[] { OCPP21.SetVariableStatus.UnknownComponent }));
                Assert.That(model.ClientValue("Servers"),                              Is.EqualTo("dns-1,dns-2"));
            });

        }

        #endregion

        #region ANewServerNeedsACIDOrBASE()

        /// <summary>
        /// Entry by entry, a new server cannot be made: the Servers entry that
        /// names it has no Address to make it with, and its own entries have no
        /// server to be set on. Both say to use ACID or BASE, as for a new time
        /// server, and nothing changes.
        /// </summary>
        [Test]
        public async Task ANewServerNeedsACIDOrBASE()
        {

            var results = await Set(OCPP21.DataConsistencyModel.IndependentRequests,
                                    (null,     "Servers", "dns-1,dns-2,office"),
                                    ("office", "Address", "203.0.113.53"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InvalidValue"));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.AdditionalInfo),
                                                                                       Is.All.Contains("with the DataConsistencyModel ACID or BASE"));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,            Does.StartWith("'DNSServerController[office].Address' is not there yet"));
                Assert.That(model.ClientValue("Servers"),                              Is.EqualTo("dns-1,dns-2"));
            });

        }

        #endregion

        #region AServerKeepsItsIdWhenTheOneBeforeItIsTakenAway()

        /// <summary>
        /// The first server taken away and the second changed in the same
        /// request, independently: the second is "dns-2" before and after, and
        /// nothing moves up into a place the first left - which slots did, a
        /// later entry of slot 1 meaning the server of slot 2.
        /// </summary>
        [Test]
        public async Task AServerKeepsItsIdWhenTheOneBeforeItIsTakenAway()
        {

            var results = await Set(null,
                                    (null,    "Servers", "dns-2"),
                                    ("dns-2", "Port",    "5353"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                           Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(model.ServerValue("dns-2", "Address"),         Is.EqualTo("2001:db8::53"));
                Assert.That(model.ServerValue("dns-2", "Port"),            Is.EqualTo("5353"));
                Assert.That(model.ServerValue("dns-1", "Address"),         Is.Null, "taken away");
                Assert.That(model.ClientValue("Servers"),                  Is.EqualTo("dns-2"));

                Assert.That((OnDisk()["dns"]?["servers"] as JArray)?.Select(server => server.Value<String>("id")),
                                                                           Is.EqualTo(new[] { "dns-2" }), "and so at the next start");

            });

        }

        #endregion

        #region ThePortOfTheOldTransportGoesWithIt()

        /// <summary>
        /// A server switched to TLS without a port is asked on 853, not on the
        /// 53 it had as the default of UDP; a port chosen by hand stays.
        /// </summary>
        [Test]
        public async Task ThePortOfTheOldTransportGoesWithIt()
        {

            var toTLS     = await Set(null, ("dns-2", "Transport", "TLS"));
            var portNow   = model.ServerValue("dns-2", "Port");

            var byHand    = await Set(null, ("dns-2", "Port",      "5353"));
            var toTCP     = await Set(null, ("dns-2", "Transport", "TCP"));

            Assert.Multiple(() => {

                Assert.That(StatusesOf([.. toTLS, .. byHand, .. toTCP]), Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(portNow,                                 Is.EqualTo("853"));
                Assert.That(model.ServerValue("dns-2", "Port"),            Is.EqualTo("5353"));
                Assert.That(model.ServerValue("dns-2", "Transport"),       Is.EqualTo("TCP"));

            });

        }

        #endregion

        #region NoServerAtAllIsRefused()

        /// <summary>
        /// Servers naming none is refused, and the servers are kept (SecureDNS
        /// T14); so is an Address emptied, which takes no server away: that is
        /// for Servers to do.
        /// </summary>
        [Test]
        public async Task NoServerAtAllIsRefused()
        {

            var none     = await Set(null, (null, "Servers", ""));

            var emptied  = await Set(OCPP21.DataConsistencyModel.ACID,
                                     ("dns-1", "Address", ""),
                                     ("dns-2", "Port",    "5353"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(none),                                          Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected }));
                Assert.That(none[0].AttributeStatusInfo?.ReasonCode,                   Is.EqualTo("InvalidValue"));
                Assert.That(none[0].AttributeStatusInfo?.AdditionalInfo,               Does.Contain("names no name server"));
                Assert.That(StatusesOf(emptied),                                       Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(emptied[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("leaving its id out of DNSClientController.Servers"));
                Assert.That(model.ServerValue("dns-1", "Address"),                     Is.EqualTo("192.0.2.53"));
                Assert.That(model.ServerValue("dns-2", "Address"),                     Is.EqualTo("2001:db8::53"));
                Assert.That(model.ServerValue("dns-2", "Port"),                        Is.EqualTo("53"));
            });

        }

        #endregion

        #region ServersSaysWhichThereAreAndInWhichOrder()

        /// <summary>
        /// Servers is checked as a list of ids before anything is tried, and
        /// read back as it was set: in its order, which is the order in the file.
        /// </summary>
        [Test]
        public async Task ServersSaysWhichThereAreAndInWhichOrder()
        {

            var refused   = await Set(null,
                                      (null, "Servers", "dns-1,Office"));

            var twice     = await Set(null,
                                      (null, "Servers", "dns-1,dns-1"));

            var tooMany   = await Set(null,
                                      (null, "Servers", String.Join(",", Enumerable.Range(1, 17).Select(n => $"dns-{n}"))));

            var reordered = await Set(null,
                                      (null, "Servers", "dns-2,dns-1"));

            Assert.Multiple(() => {

                Assert.That(refused[0].AttributeStatusInfo?.AdditionalInfo,   Does.Contain("'Office' cannot be the id of a name server"));
                Assert.That(twice  [0].AttributeStatusInfo?.AdditionalInfo,   Does.Contain("names 'dns-1' more than once"));
                Assert.That(tooMany[0].AttributeStatusInfo?.ReasonCode,       Is.EqualTo("ValueOutOfRange"));

                Assert.That(StatusesOf(reordered),                            Is.EqualTo(new[] { OCPP21.SetVariableStatus.Accepted }));
                Assert.That(model.ClientValue("Servers"),                     Is.EqualTo("dns-2,dns-1"));
                Assert.That((OnDisk()["dns"]?["servers"] as JArray)?.Select(server => server.Value<String>("address")),
                                                                              Is.EqualTo(new[] { "2001:db8::53", "192.0.2.53" }));
                Assert.That(model.ServerValue("dns-1", "RootPins"),           Is.EqualTo(ARoot), "a server keeps what it is held to wherever it stands");

            });

        }

        #endregion

        #region AServerTakenAwayCannotBeSetAsWell()

        /// <summary>
        /// A server Servers leaves out cannot be set in the same change: with
        /// ACID, both are refused, and nothing changes.
        /// </summary>
        [Test]
        public async Task AServerTakenAwayCannotBeSetAsWell()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null,    "Servers", "dns-1"),
                                    ("dns-2", "Port",    "5353"),
                                    (null,    "MaxRetries", "4"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                              Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,   Does.Contain("'DNSServerController[dns-2]' is taken away by DNSClientController.Servers in the same change"));
                Assert.That(model.ClientValue("Servers"),                     Is.EqualTo("dns-1,dns-2"));
                Assert.That(model.ClientValue("MaxRetries"),                  Is.EqualTo("2"));
            });

        }

        #endregion

        #region ARefusalOfTheNodeRollsTheRestBack()

        /// <summary>
        /// With ACID, a pin for a server over UDP is refused by the node, in
        /// the words of the Device Model; the other entry is rolled back, and
        /// nothing changed.
        /// </summary>
        [Test]
        public async Task ARefusalOfTheNodeRollsTheRestBack()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null, "MaxRetries",      "4"),
                                    ("dns-2",  "CertificatePins", AnotherRoot));

            Assert.Multiple(() => {

                Assert.That(StatusesOf(results),                     Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rolledback, OCPP21.SetVariableStatus.Rejected }));
                Assert.That(results[1].AttributeStatusInfo?.ReasonCode,      Is.EqualTo("InvalidValue"));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,  Does.Contain("'DNSServerController[dns-2]'"));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,  Does.Not.Contain("dns.servers"));

                Assert.That(model.ClientValue("MaxRetries"),         Is.EqualTo("2"));
                Assert.That(model.ServerValue("dns-2", "CertificatePins"), Is.EqualTo(""));

            });

        }

        #endregion

        #region AChangeTheCSMSHasNotSeenIsNotMade()

        /// <summary>
        /// Compare and set: an entry whose old value is not the value now is
        /// refused, and a CSMS overwrites nothing it has not seen.
        /// </summary>
        [Test]
        public async Task AChangeTheCSMSHasNotSeenIsNotMade()
        {

            var node     = station!.OCPPv2_1Node;

            var request  = new OCPP21.CSMS.SetVariablesRequest(
                               Destination:   SourceRouting.To(node.Id),
                               VariableData:  [
                                                  new OCPP21.SetVariableData(
                                                      Component:          new OCPP21.Component(Name: DNSDeviceModel.ServerComponent, Instance: "dns-2"),
                                                      Variable:           new OCPP21.Variable (Name: "Port"),
                                                      AttributeValue:     "5353",
                                                      OldAttributeValue:  "5300"
                                                  )
                                              ]
                           );

            var results  = await OCPP21.DeviceModel.SetVariablesAsync(request, (name, evse) => node.GetComponentConfigs(name, evse), default, node.NewInstanceOf);

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("ValueChanged"));
                Assert.That(model.ServerValue("dns-2", "Port"),                              Is.EqualTo("53"));
            });

        }

        #endregion

        #region SwitchedOffWhileTheCSMSIsDialledByNameIsRefused()

        /// <summary>
        /// Name resolution is not switched off while a CSMS connection is
        /// dialled by its name (SecureDNS T19).
        /// </summary>
        [Test]
        public async Task SwitchedOffWhileTheCSMSIsDialledByNameIsRefused()
        {

            Assert.That(station!.TryDial("ws://csms.example.test/cs001", "2.1", out var error),
                        Is.True, error);

            var results = await Set(null, (null, "Enabled", "false"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("CSMSUnreachable"));
                Assert.That(results.Single().AttributeStatusInfo?.AdditionalInfo,      Does.Contain("csms.example.test"));
                Assert.That(model.ClientValue("Enabled"),                              Is.EqualTo("true"));
            });

        }

        #endregion


        #region OnlyEncryptedServersAreAskedWhenTheCSMSSaysSo()

        /// <summary>
        /// EncryptedOnly is set as the file would say it: the server over UDP
        /// stays, and is no longer asked; set back, it is asked again.
        /// </summary>
        [Test]
        public async Task OnlyEncryptedServersAreAskedWhenTheCSMSSaysSo()
        {

            var on     = await Set(null, (null, "EncryptedOnly", "true"));
            var asked  = station!.Node.DNSClient.DNSServers.Select(server => server.ToString()).ToArray();
            var read   = model.ClientValue("EncryptedOnly");
            var saved  = OnDisk()["dns"]?["encryptedOnly"]?.Value<Boolean>();

            var off    = await Set(null, (null, "EncryptedOnly", "false"));

            Assert.Multiple(() => {
                Assert.That(on.Single().AttributeStatus,         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(asked,                               Is.EqualTo(new[] { "tls://192.0.2.53:853" }));
                Assert.That(read,                                Is.EqualTo("true"));
                Assert.That(saved,                               Is.True);
                Assert.That(model.ServerIds,                     Is.EqualTo(new[] { "dns-1", "dns-2" }), "the server over UDP was taken away");
                Assert.That(off.Single().AttributeStatus,        Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(station.Node.DNSClient.DNSServers.Count(),  Is.EqualTo(2), "switched off, the server over UDP is not asked again");
            });

        }

        #endregion

        #region ANewServerInClearTextIsRefusedForWhatItIsWhileTheCSMSIsDialledByName()

        /// <summary>
        /// While only encrypted servers are asked, a new one over UDP is
        /// refused as the node refuses it, naming it - and so it is while a
        /// CSMS is dialled by its name, where the pre-flight says so before it
        /// would ask anything, and not that the CSMS would be cut off.
        /// </summary>
        [Test]
        public async Task ANewServerInClearTextIsRefusedForWhatItIsWhileTheCSMSIsDialledByName()
        {

            // Quick to give up on the servers nobody answers on: the pre-flight
            // asks the configuration as it is as well.
            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse("""{ "encryptedOnly": true, "queryTimeoutSeconds": 0.2, "maxRetries": 0 }"""), out var error),
                        Is.True,
                        error);

            Assert.That(station.TryDial("ws://csms.example.test/cs001", "2.1", out error),
                        Is.True, error);

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null,     "Servers",    "dns-1,dns-2,office"),
                                    ("office", "Address",    "203.0.113.53"),
                                    ("office", "Transport",  "UDP"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                   Is.All.Not.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(results[1].AttributeStatus,                            Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results[1].AttributeStatusInfo?.ReasonCode,            Is.EqualTo("InvalidValue"));
                Assert.That(results[1].AttributeStatusInfo?.AdditionalInfo,        Does.StartWith("'DNSServerController[office]' would be asked over UDP, in clear text"));
                Assert.That(model.ServerIds,                                       Is.EqualTo(new[] { "dns-1", "dns-2" }));
            });

        }

        #endregion


        #region (class) ANodeOfWords

        /// <summary>
        /// What the source needs of a node, written down: a "dns" section with
        /// two servers, changes that are taken or refused as told, and names
        /// that resolve now and with a change as told.
        /// </summary>
        private sealed class ANodeOfWords
        {

            public JObject              Section          { get; set; } = new (
                                                                             new JProperty("enabled", true),
                                                                             new JProperty("servers", new JArray(
                                                                                 new JObject(new JProperty("id", "dns-1"), new JProperty("address", "192.0.2.53"),   new JProperty("port", 853), new JProperty("transport", "TLS")),
                                                                                 new JObject(new JProperty("id", "dns-2"), new JProperty("address", "2001:db8::53"), new JProperty("port", 53),  new JProperty("transport", "UDP"))
                                                                             )),
                                                                             new JProperty("limits", new JObject(new JProperty("maxServers", 16)))
                                                                         );

            public List<JObject>        Changes          { get; } = [];

            public List<String?>        Origins          { get; } = [];

            /// <summary>
            /// What happens elsewhere while a change is resolved with: a save
            /// on the page, say.
            /// </summary>
            public Action?              Meanwhile        { get; set; }

            public Boolean              NotSaved         { get; set; }

            public Boolean              ResolvesNow      { get; set; } = true;

            public Boolean              ResolvesThen     { get; set; } = true;

            public String[]             Names            { get; set; } = [ "csms.example.test" ];

            /// <summary>
            /// The names of the NTS-KE servers of the legal time.
            /// </summary>
            public String[]             LegalNames       { get; set; } = [];

            /// <summary>
            /// Names that would not resolve with the change, whatever
            /// ResolvesThen says of the others.
            /// </summary>
            public HashSet<String>      LostThen         { get; } = new (StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// What the source wrote into the log.
            /// </summary>
            public List<String>         Logged           { get; } = [];

            public Boolean TryUpdate(Func<JObject, JObject?>           Change,
                                     [NotNullWhen(false)] out String?  Error,
                                     out Boolean                       NotSaved,
                                     String?                           Origin = null)
            {

                NotSaved = this.NotSaved;
                Error    = NotSaved ? "The file is read-only." : null;

                if (NotSaved)
                    return false;

                if (Change(Section) is JObject change)
                {
                    Changes.Add(change);
                    Origins.Add(Origin);
                }

                return true;

            }

            public Task<DNSPreflight> ResolveWith(JObject              Change,
                                                  IEnumerable<String>  Names,
                                                  TimeSpan             Within,
                                                  CancellationToken    CancellationToken)

                => Task.Run(() => {

                       if (Change.Count > 0)
                           Meanwhile?.Invoke();

                       // Switched off, a node resolves nothing, as its dry run says.
                       var switchedOff = Change["enabled"]?.Type == JTokenType.Boolean && !Change.Value<Boolean>("enabled");

                       return new DNSPreflight(
                                       Names.ToDictionary(name => name,
                                                          name => Change.Count == 0
                                                                      ? ResolvesNow
                                                                      : ResolvesThen && !switchedOff && !LostThen.Contains(name)),
                                       Change.Count == 0 || ResolvesThen && LostThen.Count == 0
                                           ? []
                                           : [ new ServerJudgement(
                                                   Service:                 "dns",
                                                   Server:                  "tls://203.0.113.53:853",
                                                   At:                      DateTimeOffset.UtcNow,
                                                   Accepted:                false,
                                                   Outcome:                 "untrusted",
                                                   CertificateFingerprint:  AnotherRoot,
                                                   RootFingerprint:         null,
                                                   AnchoredBy:              null,
                                                   HeldTo:                  null,
                                                   Learned:                 TrustOnFirstUse.None,
                                                   Previously:              null,
                                                   FirstSeen:               false,
                                                   Steps:                   []
                                               ) ]
                                   );

                   });

            public DNSDeviceModel Source()

                => new (() => Section, TryUpdate, ResolveWith, () => Names, Logged.Add, () => LegalNames);

        }

        #endregion

        #region AChangeThatCutsTheCSMSOffIsRefused()

        /// <summary>
        /// A name of a CSMS connection that resolves now and would not resolve
        /// with the change makes all of it CSMSUnreachable, saying the name and
        /// what each server came to; nothing is put into the node (SecureDNS T20).
        /// </summary>
        [Test]
        public async Task AChangeThatCutsTheCSMSOffIsRefused()
        {

            var node    = new ANodeOfWords { ResolvesThen = false };

            var results = await SetIn(node.Source(), null,
                                      ("dns-1", "Address",   "203.0.113.53"),
                                      ("dns-1", "RootPins",  AnotherRoot));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("CSMSUnreachable"));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("'csms.example.test' would not resolve"));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("tls://203.0.113.53:853 untrusted"));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region ANameThatDoesNotResolveNowBlocksNothing()

        /// <summary>
        /// A name that does not resolve now does not block a change, so that a
        /// CSMS can repair a broken configuration.
        /// </summary>
        [Test]
        public async Task ANameThatDoesNotResolveNowBlocksNothing()
        {

            var node    = new ANodeOfWords { ResolvesNow = false, ResolvesThen = false };

            var results = await SetIn(node.Source(), null, ("dns-2", "Address", "198.51.100.53"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(node.Changes.Single()["servers"]?[1]?["address"]?.Value<String>(), Is.EqualTo("198.51.100.53"));
            });

        }

        #endregion

        #region AChangeThatLeavesTheLegalTimeNoNTSKEServerIsRefused()

        /// <summary>
        /// A change after which none of the NTS-KE servers of the legal time
        /// would resolve, while they do now, is NTSKEUnreachable as a whole,
        /// saying the names and what each name server came to; nothing is put
        /// into the node (SecureDNS-DeviceModel-Concept.md B.13 point 6).
        /// </summary>
        [Test]
        public async Task AChangeThatLeavesTheLegalTimeNoNTSKEServerIsRefused()
        {

            var node    = new ANodeOfWords { Names = [], LegalNames = [ "nts1.example.test", "nts2.example.test" ], ResolvesThen = false };

            var results = await SetIn(node.Source(), OCPP21.DataConsistencyModel.ACID,
                                      ("dns-1", "Address",   "203.0.113.53"),
                                      ("dns-1", "RootPins",  AnotherRoot));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo(DNSDeviceModel.NTSKEUnreachable));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("'nts1.example.test', 'nts2.example.test' would not resolve").
                                                                                       And.Contains("no NTS-KE server left").
                                                                                       And.Contains("tls://203.0.113.53:853 untrusted"));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region AChangeThatKeepsTheLegalTimeAnNTSKEServerIsMadeAndSaysWhatItLoses()

        /// <summary>
        /// A change after which one NTS-KE server of the legal time would no
        /// longer resolve, and another still would, is put into effect: every
        /// entry is accepted with NTSKEUnreachable in its status info, naming
        /// the one lost and the one kept, and the log says it as well.
        /// </summary>
        [Test]
        public async Task AChangeThatKeepsTheLegalTimeAnNTSKEServerIsMadeAndSaysWhatItLoses()
        {

            var node    = new ANodeOfWords { Names = [], LegalNames = [ "nts1.example.test", "nts2.example.test" ] };

            node.LostThen.Add("nts1.example.test");

            var results = await SetIn(node.Source(), OCPP21.DataConsistencyModel.ACID,
                                      ("dns-2", "Address",  "198.51.100.53"),
                                      (null,    "UseCache", "false"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo(DNSDeviceModel.NTSKEUnreachable));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("'nts1.example.test' of the legal time would not resolve any more, 'nts2.example.test' still would"));
                Assert.That(node.Changes.Single()["servers"]?[1]?["address"]?.Value<String>(), Is.EqualTo("198.51.100.53"));
                Assert.That(node.Logged.Any(line => line.Contains("the legal time loses NTS-KE servers") && line.Contains("'nts1.example.test'")),
                            Is.True, String.Join(Environment.NewLine, node.Logged));
            });

        }

        #endregion

        #region AnNTSKEServerOfTheLegalTimeThatDoesNotResolveNowBlocksNothing()

        /// <summary>
        /// An NTS-KE server of the legal time that does not resolve now does
        /// not block a change, and is not said to be lost by it, so that a
        /// CSMS can repair a broken configuration.
        /// </summary>
        [Test]
        public async Task AnNTSKEServerOfTheLegalTimeThatDoesNotResolveNowBlocksNothing()
        {

            var node    = new ANodeOfWords { Names = [], LegalNames = [ "nts1.example.test" ], ResolvesNow = false, ResolvesThen = false };

            var results = await SetIn(node.Source(), null, ("dns-2", "Address", "198.51.100.53"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(results.Single().AttributeStatusInfo,                      Is.Null);
                Assert.That(node.Changes,                                              Has.Count.EqualTo(1));
            });

        }

        #endregion

        #region NameResolutionIsNotSwitchedOffUnderTheLegalTime()

        /// <summary>
        /// Switched off, a node resolves nothing: while an NTS-KE server of the
        /// legal time resolves, that is NTSKEUnreachable, even with no CSMS
        /// dialled by its name.
        /// </summary>
        [Test]
        public async Task NameResolutionIsNotSwitchedOffUnderTheLegalTime()
        {

            var node    = new ANodeOfWords { Names = [], LegalNames = [ "nts1.example.test" ] };

            var results = await SetIn(node.Source(), null, (null, "Enabled", "false"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo(DNSDeviceModel.NTSKEUnreachable));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region ACSMSCutOffIsSaidBeforeTheLegalTime()

        /// <summary>
        /// A change that would cut off the CSMS and the legal time alike is
        /// CSMSUnreachable: the CSMS is the one it must not lose at all. A
        /// name of both is the CSMS's.
        /// </summary>
        [Test]
        public async Task ACSMSCutOffIsSaidBeforeTheLegalTime()
        {

            var node    = new ANodeOfWords { LegalNames = [ "csms.example.test", "nts1.example.test" ], ResolvesThen = false };

            var results = await SetIn(node.Source(), null, ("dns-1", "Address", "203.0.113.53"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("CSMSUnreachable"));
                Assert.That(results.Single().AttributeStatusInfo?.AdditionalInfo,     Does.Contain("'csms.example.test' would not resolve").
                                                                                       And.Not.Contains("nts1"));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region WhatCannotBeSavedIsAnInternalError()

        /// <summary>
        /// A change the node cannot save is refused as an internal error, and
        /// nothing of it was put into effect (SecureDNS T17).
        /// </summary>
        [Test]
        public async Task WhatCannotBeSavedIsAnInternalError()
        {

            var node    = new ANodeOfWords { NotSaved = true };

            var results = await SetIn(node.Source(), OCPP21.DataConsistencyModel.ACID,
                                      (null, "UseCache",  "false"),
                                      ("dns-2",  "Port",      "5353"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.All.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InternalError"));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region TheWholeListIsSentWithWhatItWasShownHeldTo()

        /// <summary>
        /// A change of one server sends the whole list, in its order, each
        /// server with its id and what the request found it held to - so that a
        /// pin learned on first use in the meantime is not lost - and a mismatch
        /// only where a pin can bring one about.
        /// </summary>
        [Test]
        public async Task TheWholeListIsSentWithWhatItWasShownHeldTo()
        {

            var node = new ANodeOfWords();

            ((JObject) node.Section["servers"]![0]!)["rootFingerprint"] = ARoot;
            ((JObject) node.Section["servers"]![0]!)["onMismatch"]      = "record";

            var results = await SetIn(node.Source(), null, ("dns-2", "Port", "5353"));

            var sent    = node.Changes.Single()["servers"] as JArray;

            Assert.Multiple(() => {

                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(sent?.Count,                                               Is.EqualTo(2));
                Assert.That(sent?.Select(server => server.Value<String>("id")),        Is.EqualTo(new[] { "dns-1", "dns-2" }));

                Assert.That(sent?[0]?["address"]?.Value<String>(),                     Is.EqualTo("192.0.2.53"));
                Assert.That(sent?[0]?["rootFingerprints"]?.Values<String>(),           Is.EqualTo(new[] { ARoot }));
                Assert.That(sent?[0]?["onMismatch"]?.Value<String>(),                  Is.EqualTo("record"));
                Assert.That(sent?[0]?[ServerPins.AsShownKey]?["rootFingerprints"]?.Values<String>(), Is.EqualTo(new[] { ARoot }));

                Assert.That(sent?[1]?["address"]?.Value<String>(),                     Is.EqualTo("2001:db8::53"));
                Assert.That(sent?[1]?["port"]?.Value<Int32>(),                         Is.EqualTo(5353));
                Assert.That(sent?[1]?["onMismatch"],                                   Is.Null, "a server held to nothing has no mismatch to come to");
                Assert.That(sent?[1]?[ServerPins.AsShownKey],                          Is.InstanceOf<JObject>().And.Empty);

            });

        }

        #endregion


        #region AChangeNamesTheCSMSThatMadeIt()

        /// <summary>
        /// The node writes a change of a CSMS down as the CSMS's, and tells it
        /// so: a bridge telling the CSMS of local changes leaves its own out.
        /// </summary>
        [Test]
        public async Task AChangeNamesTheCSMSThatMadeIt()
        {

            var told    = new List<(IReadOnlyList<String> Changes, String? Origin)>();

            station!.Node.OnDNSConfigurationChanged += (changes, origin) => {
                lock (told)
                    told.Add((changes, origin));
            };

            var before  = station.Node.Log.LastId;
            var results = await Set(null, (null, "MaxRetries", "3"));

            Assert.Multiple(() => {

                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(told.Select   (change => change.Origin),                   Is.EqualTo(new[] { "ocpp:CSMS" }));
                Assert.That(told.SelectMany(change => change.Changes),                 Is.EqualTo(new[] { "max retries = 3" }));
                Assert.That(told.All(change => ChangeOrigin.IsOCPP(change.Origin)),    Is.True);

                Assert.That(station.Node.Log.Recent(100, before, null).Select(entry => entry.Message),
                            Has.Some.EqualTo("DNS configuration changed: max retries = 3 (by ocpp:CSMS)."));

            });

        }

        #endregion

        #region AServerAddedElsewhereMeanwhileIsNotWrittenAway()

        /// <summary>
        /// A server added on the page while a change of the CSMS was resolved
        /// with: the whole list the CSMS's change would send back is not the
        /// one the node holds, so the change is refused as ValueChanged, and
        /// the page's server is kept.
        /// </summary>
        [Test]
        public async Task AServerAddedElsewhereMeanwhileIsNotWrittenAway()
        {

            var node = new ANodeOfWords();

            node.Meanwhile = () => ((JArray) node.Section["servers"]!).Add(new JObject(new JProperty("id", "dns-3"), new JProperty("address", "198.51.100.53"), new JProperty("port", 53), new JProperty("transport", "UDP")));

            var results = await SetIn(node.Source(), null, ("dns-2", "Port", "5353"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("ValueChanged"));
                Assert.That(node.Changes,                                              Is.Empty,  "nothing was sent that would write the page's server away");
                Assert.That((node.Section["servers"] as JArray)?.Count,                Is.EqualTo(3));
            });

        }

        #endregion

        #region IdsChangedElsewhereMeanwhileAreAChangeElsewhere()

        /// <summary>
        /// The same servers given other ids on the page while a change of the
        /// CSMS was resolved with are other servers to it: the change is refused
        /// as ValueChanged, rather than writing the ids it read back.
        /// </summary>
        [Test]
        public async Task IdsChangedElsewhereMeanwhileAreAChangeElsewhere()
        {

            var node = new ANodeOfWords();

            node.Meanwhile = () => node.Section["servers"]![1]!["id"] = "office";

            var results = await SetIn(node.Source(), null, ("dns-1", "Port", "5353"));

            Assert.Multiple(() => {
                Assert.That(results.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("ValueChanged"));
                Assert.That(node.Changes,                                              Is.Empty);
            });

        }

        #endregion

        #region WithBASEANewServerIsOneWithServers()

        /// <summary>
        /// With BASE, a server the request adds is set as one with
        /// DNSClientController, whose Servers adds it, and each server the
        /// node has by itself.
        /// </summary>
        [Test]
        public async Task WithBASEANewServerIsOneWithServers()
        {

            var results = await Set(OCPP21.DataConsistencyModel.BASE,
                                    (null,     "Servers",   "dns-1,dns-2,office"),
                                    ("office", "Address",   "203.0.113.53"),
                                    ("office", "Transport", "TCP"),
                                    ("dns-2",  "Port",      "0"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                     Is.EqualTo(new[] { OCPP21.SetVariableStatus.Accepted, OCPP21.SetVariableStatus.Accepted,
                                                                                         OCPP21.SetVariableStatus.Accepted, OCPP21.SetVariableStatus.Rejected }));
                Assert.That(model.ServerValue("office", "Transport"),  Is.EqualTo("TCP"));
                Assert.That(model.ClientValue("Servers"),            Is.EqualTo("dns-1,dns-2,office"));
            });

        }

        #endregion

        #region TheSameServersWrittenOtherwiseAreNoChangeElsewhere()

        /// <summary>
        /// The node writes a server as it reads it - an IPv6 address in its
        /// short form, a port written out. The second change of a request
        /// whose first sent an address written otherwise finds the servers
        /// the first one sent, not a change made elsewhere.
        /// </summary>
        [Test]
        public async Task TheSameServersWrittenOtherwiseAreNoChangeElsewhere()
        {

            var results = await Set(null,
                                    ("dns-2", "Address", "2001:DB8:0:0:0:0:0:5353"),
                                    ("dns-1", "Port",    "5300"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                     Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.ServerValue("dns-2", "Address"),         Is.EqualTo("2001:db8::5353"));
                Assert.That(model.ServerValue("dns-2", "Port"),            Is.EqualTo("53"));
                Assert.That(model.ServerValue("dns-1", "Port"),            Is.EqualTo("5300"));
            });

        }

        #endregion

        #region AGroupOfRootsStaysWithItsServer()

        /// <summary>
        /// A server held to a group of roots is still held to it after a change
        /// of another server - and after a change of its own port, which makes
        /// it another server to the node, held to what the change sends.
        /// </summary>
        [Test]
        public async Task AGroupOfRootsStaysWithItsServer()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(new JObject(
                            new JProperty("servers", new JArray(
                                new JObject(
                                    new JProperty("address",    "192.0.2.53"),
                                    new JProperty("transport",  "TLS"),
                                    new JProperty("rootGroup",  "cpo")
                                ),
                                new JObject(
                                    new JProperty("address",    "2001:db8::53"),
                                    new JProperty("transport",  "UDP")
                                )
                            ))
                        ), out var error), Is.True, error);

            var elsewhere = await Set(null, ("dns-2", "Port", "5353"));
            var groupNow  = station.Node.DNSConfigurationJSON()["servers"]?[0]?["rootGroup"]?.Value<String>();

            var moved     = await Set(null, ("dns-1", "Port", "8853"));
            var server    = station.Node.DNSConfigurationJSON()["servers"]?[0];

            Assert.Multiple(() => {

                Assert.That(StatusesOf([.. elsewhere, .. moved]),    Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));

                Assert.That(groupNow,                                Is.EqualTo("cpo"), "after a change of another server");
                Assert.That(server?["port"]?.Value<Int32>(),         Is.EqualTo(8853));
                Assert.That(server?["rootGroup"]?.Value<String>(),   Is.EqualTo("cpo"), "after a change of its own port");
                Assert.That(server?["onMismatch"],                   Is.Null,           "a group has no mismatch to come to");

            });

        }

        #endregion

        #region RootCAsIsTheGroupOfRootsAServerIsHeldTo()

        /// <summary>
        /// RootCAs is the group of roots a server is held to: written into its
        /// entry beside its pins, read back as it was written, taken away by an
        /// empty value - and refused, before the node is asked, where the store
        /// could not mark a group of that name.
        /// </summary>
        [Test]
        public async Task RootCAsIsTheGroupOfRootsAServerIsHeldTo()
        {

            var set      = await Set(null, ("dns-1", "RootCAs", "CPO"));
            var written  = station!.Node.DNSConfigurationJSON()["servers"]?[0];
            var read     = model.ServerValue("dns-1", "RootCAs");

            var wrong    = await Set(null, ("dns-1", "RootCAs", "the CPO"));
            var emptied  = await Set(null, ("dns-1", "RootCAs", ""));

            Assert.Multiple(() => {

                Assert.That(set.Single().AttributeStatus,                         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(written?["rootGroup"]?.Value<String>(),               Is.EqualTo("CPO"));
                Assert.That(written?["rootFingerprint"]?.Value<String>(),         Is.EqualTo(ARoot), "the pin beside it");
                Assert.That(read,                                                 Is.EqualTo("CPO"));

                Assert.That(wrong.Single().AttributeStatus,                       Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(wrong.Single().AttributeStatusInfo?.AdditionalInfo,   Does.Contain("cannot name a group of roots here"));

                Assert.That(emptied.Single().AttributeStatus,                     Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(station.Node.DNSConfigurationJSON()["servers"]?[0]?["rootGroup"],  Is.Null);
                Assert.That(model.ServerValue("dns-1", "RootCAs"),                      Is.EqualTo(""));

            });

        }

        #endregion


        #region TheServersGoBackToThoseTheStationWasStartedWithAndComeBack()

        /// <summary>
        /// ServerSource System gives the name servers back to the ones the
        /// node was started with - held to nothing, out of the file, told as
        /// the CSMS's change - and Manual writes those into the file again.
        /// Each read says where the servers are from.
        /// </summary>
        [Test]
        public async Task TheServersGoBackToThoseTheStationWasStartedWithAndComeBack()
        {

            var told     = new List<(IReadOnlyList<String> Changes, String? Origin)>();

            station!.Node.OnDNSConfigurationChanged += (changes, origin) => {
                lock (told)
                    told.Add((changes, origin));
            };

            var before   = model.ClientValue("ServerSource");

            var system   = await Set(null, (null, "ServerSource", "System"));
            var given    = (Source: model.ClientValue("ServerSource"), Transport: model.ServerValue("dns-1", "Transport"), RootPins: model.ServerValue("dns-1", "RootPins"),
                            InTheFile: OnDisk()["dns"]?["servers"]);

            var manual   = await Set(null, (null, "ServerSource", "Manual"));
            var written  = OnDisk()["dns"]?["servers"] as JArray;

            Assert.Multiple(() => {

                Assert.That(before,                                  Is.EqualTo("Manual"));

                Assert.That(system.Single().AttributeStatus,         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(given.Source,                            Is.EqualTo("System"));
                Assert.That(given.Transport,                         Is.EqualTo("UDP"), "the node's own");
                Assert.That(given.RootPins,                          Is.EqualTo(""),    "held to nothing");
                Assert.That(given.InTheFile,                         Is.Null);

                Assert.That(manual.Single().AttributeStatus,         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.ClientValue("ServerSource"),       Is.EqualTo("Manual"));
                Assert.That(written?.Select(server => server.Value<String>("address")),  Is.EqualTo(new[] { "192.0.2.53", "2001:db8::53" }));
                Assert.That(written?[0]?.Value<String>("transport"),                     Is.EqualTo("UDP"), "the one it was started with, not the file's before");
                Assert.That(written?[0]?["rootFingerprint"],                             Is.Null);

                Assert.That(told.Select(change => change.Origin),    Is.EqualTo(new[] { "ocpp:CSMS", "ocpp:CSMS" }));
                Assert.That(told[0].Changes,                         Has.Some.EndsWith(", the ones it was started with"));

            });

        }

        #endregion

        #region ServersGivenBackAreNotSetInTheSameRequest()

        /// <summary>
        /// ServerSource System and a server as one change cannot both be meant:
        /// with ACID the source is refused, and the server rolled back with it.
        /// </summary>
        [Test]
        public async Task ServersGivenBackAreNotSetInTheSameRequest()
        {

            var results = await Set(OCPP21.DataConsistencyModel.ACID,
                                    (null, "ServerSource", "System"),
                                    ("dns-2",  "Port",         "5353"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                                       Is.EqualTo(new[] { OCPP21.SetVariableStatus.Rejected, OCPP21.SetVariableStatus.Rolledback }));
                Assert.That(results[0].AttributeStatusInfo?.ReasonCode,                Is.EqualTo("InvalidValue"));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,            Does.Contain("cannot be one change with an entry of DNSServerController"));
                Assert.That(model.ClientValue("ServerSource"),                         Is.EqualTo("Manual"));
                Assert.That(model.ServerValue("dns-2", "Port"),                              Is.EqualTo("53"));
            });

        }

        #endregion

        #region AServerChangedAfterTheServersAreGivenBackIsOneOfThem()

        /// <summary>
        /// Entry by entry, "dns-1" changed after ServerSource System is the
        /// first server the node was started with: over UDP and held to
        /// nothing, not the file's server over TLS held to a root that was
        /// "dns-1" before - and the change writes them into the file again.
        /// </summary>
        [Test]
        public async Task AServerChangedAfterTheServersAreGivenBackIsOneOfThem()
        {

            var results = await Set(OCPP21.DataConsistencyModel.IndependentRequests,
                                    (null, "ServerSource", "System"),
                                    ("dns-1",  "Port",         "5353"));

            Assert.Multiple(() => {
                Assert.That(StatusesOf(results),                     Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(model.ServerValue("dns-1", "Address"),         Is.EqualTo("192.0.2.53"));
                Assert.That(model.ServerValue("dns-1", "Transport"),       Is.EqualTo("UDP"));
                Assert.That(model.ServerValue("dns-1", "Port"),            Is.EqualTo("5353"));
                Assert.That(model.ServerValue("dns-1", "RootPins"),        Is.EqualTo(""));
                Assert.That(model.ClientValue("ServerSource"),       Is.EqualTo("Manual"));
            });

        }

        #endregion

        #region ServersGivenBackAreTriedAgainstTheNamesOfTheCSMSFirst()

        /// <summary>
        /// Servers given back to those the node was started with must still
        /// resolve the CSMS: where they would not, System is CSMSUnreachable and
        /// nothing is put into the node; where they would, "servers": null is.
        /// </summary>
        [Test]
        public async Task ServersGivenBackAreTriedAgainstTheNamesOfTheCSMSFirst()
        {

            var node      = new ANodeOfWords { ResolvesThen = false };

            node.Section["serversFrom"] = "file";

            var refused   = await SetIn(node.Source(), null, (null, "ServerSource", "System"));
            var nothing   = node.Changes.Count;

            node.ResolvesThen = true;

            var accepted  = await SetIn(node.Source(), null, (null, "ServerSource", "System"));

            Assert.Multiple(() => {
                Assert.That(refused.Single().AttributeStatus,                          Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(refused.Single().AttributeStatusInfo?.ReasonCode,          Is.EqualTo("CSMSUnreachable"));
                Assert.That(nothing,                                                   Is.Zero);
                Assert.That(accepted.Single().AttributeStatus,                         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(node.Changes.Single()["servers"]?.Type,                    Is.EqualTo(JTokenType.Null));
            });

        }

        #endregion


        #region (private) OnDisk()

        /// <summary>
        /// The node's file as it is now.
        /// </summary>
        private JObject OnDisk()

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path));

        #endregion

        #region (private) Set(Model, params Entries)

        /// <summary>
        /// SetVariables of the DNS controllers as the node's OCPP 2.1 node
        /// answers it: (null, variable, value) for the client, (id, ...) for a
        /// server.
        /// </summary>
        private async Task<OCPP21.SetVariableResult[]> Set(OCPP21.DataConsistencyModel?                             Model,
                                                           params (String? Server, String Variable, String Value)[]  Entries)
        {

            var node    = station!.OCPPv2_1Node;

            var request = new OCPP21.CSMS.SetVariablesRequest(
                              Destination:           SourceRouting.To(node.Id),
                              VariableData:          [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                                     Component:       new OCPP21.Component(Name:     entry.Server is null ? DNSDeviceModel.ClientComponent : DNSDeviceModel.ServerComponent,
                                                                                                                           Instance: entry.Server),
                                                                                     Variable:        new OCPP21.Variable(Name: entry.Variable),
                                                                                     AttributeValue:  entry.Value
                                                                                 ))],
                              DataConsistencyModel:  Model
                          );

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(request, (name, evse) => node.GetComponentConfigs(name, evse), default, node.NewInstanceOf)];

        }

        #endregion

        #region (private static) SetIn(Source, Model, params Entries)

        /// <summary>
        /// The given entries handed to the given source, as the Device Model
        /// hands them: found in controllers of that source.
        /// </summary>
        private static async Task<OCPP21.SetVariableResult[]> SetIn(DNSDeviceModel                                          Source,
                                                                    OCPP21.DataConsistencyModel?                            Model,
                                                                    params (String? Server, String Variable, String Value)[]  Entries)
        {

            var client   = new OCPP21.DNSClientController(Source);
            var servers  = OCPP21.DNSServerController.AllOf(Source).ToArray();

            var entries  = Entries.Select(entry => {

                               OCPP21.ComponentConfig component = entry.Server is null
                                                                      ? client
                                                                      : servers.FirstOrDefault(server => String.Equals(server.Instance, entry.Server, StringComparison.OrdinalIgnoreCase)) ??
                                                                        OCPP21.DNSServerController.NewInstance(Source, entry.Server);

                               return new OCPP21.SetVariableEntry(
                                          new OCPP21.SetVariableData(
                                              Component:       new OCPP21.Component(Name: component.Name, Instance: component.Instance),
                                              Variable:        new OCPP21.Variable(Name: entry.Variable),
                                              AttributeValue:  entry.Value
                                          ),
                                          component,
                                          component.VariableConfigs.Single(variable => String.Equals(variable.Name, entry.Variable, StringComparison.OrdinalIgnoreCase))
                                      );

                           }).ToArray();

            return [.. await Source.SetVariablesAsync(entries, Model ?? OCPP21.DataConsistencyModel.IndependentRequests, CancellationToken.None)];

        }

        #endregion

        #region (private static) StatusesOf(Results)

        private static OCPP21.SetVariableStatus[] StatusesOf(IEnumerable<OCPP21.SetVariableResult> Results)

            => [.. Results.Select(result => result.AttributeStatus)];

        #endregion

    }

}
