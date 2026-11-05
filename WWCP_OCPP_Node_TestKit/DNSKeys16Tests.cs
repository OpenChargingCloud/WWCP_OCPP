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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.OCPP.Node;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The configuration keys of OCPP 1.6 for the name servers (Secure DNS,
    /// section 3.2), as the node's OCPP 1.6 node answers them: read from the
    /// node, and changed as OCPP 2.1 changes them.
    /// </summary>
    /// <remarks>
    /// The node is the one of DNSDeviceModelWritingTests: a name server over
    /// TLS held to a root, and one over UDP at an IPv6 address, built and never
    /// started.
    /// </remarks>
        public abstract class DNSKeys16Tests : ABridgeTests
    {

        #region Data

        private const String  ARoot  = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  APin   = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private String            directory  = "";
        private IBridgedNode?  station;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithTwoNameServers()
        {

            directory  = TestNodes.TemporaryDirectory("dns-keys16");

            var configuration = TestNodes.Offline;

            configuration["dns"] = JObject.Parse($$"""
                                       { "servers": [ { "address": "192.0.2.53",   "transport": "TLS", "rootFingerprint": "{{ARoot}}" },
                                                      { "address": "2001:db8::53", "transport": "UDP" } ],
                                         "maxRetries": 2 }
                                       """);

            station    = Nodes.New(directory, configuration);

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


        private OCPP16.CP.IConfigurationKeySource Keys
            => station!.OCPPv1_6Node.ConfigurationKeySources.OfType<DNSDeviceModel>().Single();

        private String? Value(String Key)
            => Keys.Get(Key)?.Value;

        private Task<OCPP16.ConfigurationStatus?> Change(String Key, String Value)
            => Keys.ChangeAsync(Key, Value, "CS-16");


        #region TheKeysAreReadFromTheNode()

        /// <summary>
        /// The keys of the paper the node can answer, with one pair of pin keys
        /// per server; a list is per server in the order of the file, and a key
        /// is found whatever its case and named as the paper writes it.
        /// </summary>
        [Test]
        public void TheKeysAreReadFromTheNode()
        {

            var keys = Keys.Keys.ToArray();

            Assert.Multiple(() => {

                Assert.That(keys,  Is.SupersetOf(new[] { "dns.enabled", "dns.servers", "dns.queryTimeouts", "dns.rootCAs", "dns.encryptedOnly", "dns.rootPins.1", "dns.certificatePins.2" }));
                Assert.That(keys,  Has.None.EqualTo("dns.rootPins.3"));

                Assert.That(Value("dns.servers"),           Is.EqualTo("192.0.2.53,2001:db8::53"));
                Assert.That(Value("dns.transports"),        Is.EqualTo("TLS,UDP"));
                Assert.That(Value("dns.ports"),             Is.EqualTo("853,53"));
                Assert.That(Value("dns.queryTimeouts"),     Is.EqualTo(","));
                Assert.That(Value("dns.onPinMismatch"),     Is.EqualTo("Refuse,Refuse"));
                Assert.That(Value("dns.rootPins.1"),        Is.EqualTo(ARoot));
                Assert.That(Value("dns.rootPins.2"),        Is.EqualTo(""));
                Assert.That(Value("dns.maxRetries"),        Is.EqualTo("2"));
                Assert.That(Value("dns.recursionDesired"),  Is.EqualTo("Set"));
                Assert.That(Value("dns.encryptedOnly"),     Is.EqualTo("false"));

                Assert.That(Keys.Get("DNS.MAXRETRIES")?.Key,  Is.EqualTo("dns.maxRetries"));
                Assert.That(Keys.Get("DNS.ROOTPINS.1")?.Key,  Is.EqualTo("dns.rootPins.1"));
                Assert.That(Keys.Get("dns.rootPins.3"),       Is.Null,  "a server the station does not have");
                Assert.That(Keys.Get("ntp.legal.minServers"), Is.Null,  "the time servers' key");

            });

        }

        #endregion

        #region AKeyIsChangedAsOCPP21ChangesItsVariable()

        /// <summary>
        /// A key is changed as OCPP 2.1 changes the variable it stands for: in
        /// effect, in the file, named with the central system - which is not
        /// told back of it - and refused where 2.1 refuses it.
        /// </summary>
        [Test]
        public async Task AKeyIsChangedAsOCPP21ChangesItsVariable()
        {

            Assert.That(station!.TryDial("ws://127.0.0.1:9/cp", "1.6", out var error),
                        Is.True,
                        error);

            var before     = station.Node.Log.LastId;

            var three      = await Change("dns.maxRetries",        "3");
            var tooMany    = await Change("dns.maxRetries",        "300");
            var unset      = await Change("dns.recursionDesired",  "Unset");

            Assert.Multiple(() => {

                Assert.That((three, tooMany, unset),          Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Rejected, OCPP16.ConfigurationStatus.Accepted)));

                Assert.That(Value("dns.maxRetries"),          Is.EqualTo("3"));
                Assert.That(Value("dns.recursionDesired"),    Is.EqualTo("Unset"));
                Assert.That(OnDisk()?["maxRetries"]?.Value<Int32>(),  Is.EqualTo(3));

                Assert.That(station.Node.Log.Recent(100, before, null).Select(entry => entry.Message),
                            Has.Some.Contains("(by ocpp:CS-16)"));

                Assert.That(station.OCPPv1_6Node.OfflineQueue.Pending.Where(message => message.Kind == "SecurityEventNotification"),
                            Is.Empty,
                            "the central system's own change told back");

            });

        }

        #endregion

        #region OnlyEncryptedServersAreAskedAsOCPP21AsksThem()

        /// <summary>
        /// "dns.encryptedOnly" is changed as DNSClientController.EncryptedOnly:
        /// the server over UDP stays and is not asked, and one more server -
        /// over UDP, as the last one, where "dns.transports" says no more - is
        /// Rejected, and none is added.
        /// </summary>
        [Test]
        public async Task OnlyEncryptedServersAreAskedAsOCPP21AsksThem()
        {

            var on       = await Change("dns.encryptedOnly", "true");
            var asked    = station!.Node.DNSClient.DNSServers.Select(server => server.ToString()).ToArray();
            var another  = await Change("dns.servers", "192.0.2.53,2001:db8::53,192.0.2.54");

            Assert.Multiple(() => {
                Assert.That(on,                          Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("dns.encryptedOnly"),  Is.EqualTo("true"));
                Assert.That(asked,                       Is.EqualTo(new[] { "tls://192.0.2.53:853" }));
                Assert.That(Value("dns.servers"),        Is.EqualTo("192.0.2.53,2001:db8::53"), "the server over UDP was taken away, or another one added");
                Assert.That(another,                     Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(OnDisk()?["encryptedOnly"]?.Value<Boolean>(),  Is.True);
            });

        }

        #endregion

        #region AListIsReadAsThePaperReadsIt()

        /// <summary>
        /// A list shorter than the servers holds its last value for the rest, an
        /// empty entry is the one before it, a longer list is refused, and a
        /// list one server of which cannot take its entry changes no server.
        /// </summary>
        [Test]
        public async Task AListIsReadAsThePaperReadsIt()
        {

            var pinsOnUDP  = await Change("dns.transports", "UDP,TCP");
            var afterPins  = Value("dns.transports");

            var one        = await Change("dns.transports", "TLS");
            var afterOne   = Value("dns.transports");

            var two        = await Change("dns.transports", "TLS,TCP");
            var afterTwo   = Value("dns.transports");

            var gap        = await Change("dns.transports", "TLS,");
            var afterGap   = Value("dns.transports");

            var longer     = await Change("dns.transports", "TLS,TLS,TLS");

            var group      = await Change("dns.rootCAs",    "cpo");

            Assert.Multiple(() => {

                Assert.That(pinsOnUDP,   Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(afterPins,   Is.EqualTo("TLS,UDP"),  "the second server was changed although the first could not be");

                Assert.That((one, two, gap, longer),  Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted,
                                                                  OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Rejected)));
                Assert.That(afterOne,    Is.EqualTo("TLS,TLS"));
                Assert.That(afterTwo,    Is.EqualTo("TLS,TCP"));
                Assert.That(afterGap,    Is.EqualTo("TLS,TLS"));
                Assert.That(Value("dns.transports"),  Is.EqualTo("TLS,TLS"),  "a refused list changed something");

                Assert.That(group,                    Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("dns.rootCAs"),     Is.EqualTo("cpo,cpo"));

            });

        }

        #endregion

        #region APortOrATimeoutLeftOutIsTheServersOwn()

        /// <summary>
        /// An empty or missing entry of "dns.ports" is the port of that server's
        /// transport, and of "dns.queryTimeouts" the timeout of the client -
        /// not the entry before it, as in every other list.
        /// </summary>
        [Test]
        public async Task APortOrATimeoutLeftOutIsTheServersOwn()
        {

            var first      = await Change("dns.ports",          "8853");
            var afterFirst = Value("dns.ports");

            var second     = await Change("dns.ports",          ",5353");
            var afterBoth  = Value("dns.ports");

            var timeout    = await Change("dns.queryTimeouts",  "2");

            Assert.Multiple(() => {
                Assert.That((first, second, timeout),     Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted)));
                Assert.That(afterFirst,                   Is.EqualTo("8853,53"));
                Assert.That(afterBoth,                    Is.EqualTo("853,5353"));
                Assert.That(Value("dns.queryTimeouts"),   Is.EqualTo("2,"));
            });

        }

        #endregion

        #region TheServersAreChangedAddedAndTakenAwayByTheirPlaces()

        /// <summary>
        /// The list of servers gives a server another address at its place -
        /// which keeps its id and what it is held to - adds a server at the end
        /// with the next free id and the transport of the last but none of its
        /// pins, and takes away the ones beyond its end; it cannot take all of
        /// them away. What OCPP 2.1 reads of them is the same servers by their
        /// ids.
        /// </summary>
        [Test]
        public async Task TheServersAreChangedAddedAndTakenAwayByTheirPlaces()
        {

            Assert.That(await Change("dns.transports", "TLS"),  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));

            var grown       = await Change("dns.servers", "192.0.2.54,2001:db8::53,192.0.2.55");
            var servers     = Value("dns.servers");
            var transports  = Value("dns.transports");
            var pinOfFirst  = Value("dns.rootPins.1");
            var pinOfNew    = Value("dns.rootPins.3");
            var grownIds    = DNSDeviceModel.IdsIn(station!.Node.DNSConfigurationJSON());

            var shrunk      = await Change("dns.servers", "192.0.2.54");
            var none        = await Change("dns.servers", "");

            Assert.Multiple(() => {

                Assert.That(grown,                     Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(servers,                   Is.EqualTo("192.0.2.54,2001:db8::53,192.0.2.55"));
                Assert.That(transports,                Is.EqualTo("TLS,TLS,TLS"),  "the new server took the last one's transport");
                Assert.That(pinOfFirst,                Is.EqualTo(ARoot),          "the server given another address kept its root pin");
                Assert.That(pinOfNew,                  Is.EqualTo(""),             "the new server took another one's pins");
                Assert.That(grownIds,                  Is.EqualTo(new[] { "dns-1", "dns-2", "dns-3" }));

                Assert.That(shrunk,                    Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(none,                      Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(Value("dns.servers"),      Is.EqualTo("192.0.2.54"));
                Assert.That(DNSDeviceModel.IdsIn(station.Node.DNSConfigurationJSON()),  Is.EqualTo(new[] { "dns-1" }));

            });

        }

        #endregion

        #region ANewServerTakesAnIdNoOtherHas()

        /// <summary>
        /// A server added at the end takes the next id no other server has: where
        /// a CSMS named the second one "dns-3", the new third one is "dns-4".
        /// </summary>
        [Test]
        public async Task ANewServerTakesAnIdNoOtherHas()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse("""
                                                                         { "servers": [ { "id": "dns-1", "address": "192.0.2.53",   "transport": "TCP" },
                                                                                        { "id": "dns-3", "address": "2001:db8::53", "transport": "UDP" } ] }
                                                                         """),
                                                           out var error),
                        Is.True, error);

            var grown = await Change("dns.servers", "192.0.2.53,2001:db8::53,192.0.2.55");

            Assert.Multiple(() => {
                Assert.That(grown,                                                  Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(DNSDeviceModel.IdsIn(station.Node.DNSConfigurationJSON()),  Is.EqualTo(new[] { "dns-1", "dns-3", "dns-4" }));
                Assert.That(Value("dns.servers"),                                   Is.EqualTo("192.0.2.53,2001:db8::53,192.0.2.55"));
            });

        }

        #endregion

        #region TheNthServersPinsAreItsOwn()

        /// <summary>
        /// The pins of the n-th server are that server's: set there and nowhere
        /// else, refused where the server is asked in clear text, and a server
        /// beyond the list has no key to set.
        /// </summary>
        [Test]
        public async Task TheNthServersPinsAreItsOwn()
        {

            var set     = await Change("dns.certificatePins.1", APin);
            var onUDP   = await Change("dns.certificatePins.2", APin);
            var beyond  = await Change("dns.certificatePins.3", APin);

            Assert.Multiple(() => {
                Assert.That(set,                              Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("dns.certificatePins.1"),   Is.EqualTo(APin));
                Assert.That(Value("dns.rootPins.1"),          Is.EqualTo(ARoot));
                Assert.That(onUDP,                            Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(Value("dns.certificatePins.2"),   Is.EqualTo(""));
                Assert.That(beyond,                           Is.Null,  "a key the source does not have is not its to answer");
            });

        }

        #endregion

        #region SwitchedOffWhileTheCentralSystemIsDialledByNameIsRejected()

        /// <summary>
        /// Name resolution cannot be switched off while a central system is
        /// dialled by its name, in 1.6 as in 2.1 (T19).
        /// </summary>
        [Test]
        public async Task SwitchedOffWhileTheCentralSystemIsDialledByNameIsRejected()
        {

            Assert.That(station!.TryDial("ws://csms.example.invalid/cp", "1.6", out var error),
                        Is.True,
                        error);

            var off = await Change("dns.enabled", "false");

            Assert.Multiple(() => {
                Assert.That(off,                     Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(Value("dns.enabled"),    Is.EqualTo("true"));
            });

        }

        #endregion


        #region (private) OnDisk()

        private JObject? OnDisk()

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path))["dns"] as JObject;

        #endregion

    }

}
