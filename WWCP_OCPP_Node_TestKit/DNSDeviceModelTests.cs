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

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.OCPP.Node;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// What a CSMS reads of the node's name resolution in the Device Model,
    /// against what the node's file and the node itself say.
    /// </summary>
    /// <remarks>
    /// Read from a node that was built and not started, so that nothing is
    /// ever asked of a name server: the values come from the configuration
    /// file, through the same JSON the web page reads, and the verdicts of
    /// handshakes from the node's own record of one.
    /// </remarks>
        public abstract class DNSDeviceModelTests : ABridgeTests
    {

        #region Data

        /// <summary>
        /// A root fingerprint as somebody might type it, in capitals and with
        /// colons, which the node keeps in small letters and without them.
        /// </summary>
        private const String  ARootAsTyped     = "9D:2E:4F:6A:8B:0C:1D:3E:5F:7A:9B:1C:3D:5E:7F:9A:0B:2C:4D:6E:8F:0A:1B:3C:5D:7E:9F:1A:2B:4C:6D:8E";
        private const String  ARoot            = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  ACertificate     = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";
        private const String  AnotherOne      = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        private String            directory  = "";
        private IBridgedNode?  station;
        private DNSDeviceModel    model      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithTwoNameServers()
        {

            directory  = TestNodes.TemporaryDirectory("dns-device-model");

            var configuration = TestNodes.Offline;

            configuration["dns"] = new JObject(
                                       new JProperty("servers", new JArray(
                                           new JObject(
                                               new JProperty("address",              "192.0.2.53"),
                                               new JProperty("transport",            "TLS"),
                                               new JProperty("queryTimeoutSeconds",  2.5),
                                               new JProperty("rootFingerprint",      ARootAsTyped),
                                               new JProperty("onMismatch",           "record"),
                                               new JProperty("trustOnFirstUse",      "certificate")
                                           ),
                                           new JObject(
                                               new JProperty("address",              "2001:0db8:0000:0000:0000:0000:0000:0053"),
                                               new JProperty("transport",            "UDP")
                                           )
                                       )),
                                       new JProperty("queryTimeoutSeconds",  3),
                                       new JProperty("maxRetries",           2),
                                       new JProperty("dnssecOK",             true),
                                       new JProperty("useCache",             false)
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


        #region TheClientReadsAsTheFileSaysIt()

        /// <summary>
        /// DNSClientController says what the file says, where it says
        /// something, and what the client does otherwise.
        /// </summary>
        [Test]
        public void TheClientReadsAsTheFileSaysIt()
        {

            Assert.Multiple(() => {

                Assert.That(model.ClientValue("Enabled"),           Is.EqualTo("true"));
                Assert.That(model.ClientValue("QueryTimeout"),      Is.EqualTo("3"));
                Assert.That(model.ClientValue("MaxRetries"),        Is.EqualTo("2"));
                Assert.That(model.ClientValue("DNSSECOK"),          Is.EqualTo("true"));
                Assert.That(model.ClientValue("UseCache"),          Is.EqualTo("false"));

                // What the file does not mention: the client's own.
                Assert.That(model.ClientValue("RecursionDesired"),  Is.EqualTo("Set"));
                Assert.That(model.ClientValue("FollowCNAMEs"),      Is.EqualTo("true"));
                Assert.That(model.ClientValue("ClientSubnet"),      Is.EqualTo(""),   "no subnet is sent");
                Assert.That(model.ClientValue("ServerSource"),      Is.EqualTo("Manual"), "the file's servers");
                Assert.That(model.ClientValue("EncryptedOnly"),     Is.EqualTo("false"),  "every server asked");

                Assert.That(model.ClientValue("MaxServers"),        Is.EqualTo("16"));
                Assert.That(model.ClientValue("Servers"),           Is.EqualTo("dns-1,dns-2"), "named by their places, as the file names none");
                Assert.That(model.ServerIds,                        Is.EqualTo(new[] { "dns-1", "dns-2" }));

                Assert.That(Int32.Parse(model.ClientValue("UDPPayloadSize")!), Is.GreaterThan(0));
                Assert.That(Int32.Parse(model.ClientValue("MaxCNAMEFollows")!), Is.GreaterThan(0));

            });

        }

        #endregion

        #region EachServerIsAnInstanceByItsId()

        /// <summary>
        /// Each server of the file is an instance by its id - "dns-n" for the
        /// n-th, as the file gives none - with what it is held to, its port
        /// filled in for its transport and its address written short.
        /// </summary>
        [Test]
        public void EachServerIsAnInstanceByItsId()
        {

            Assert.Multiple(() => {

                Assert.That(model.ServerValue("dns-1", "Address"),          Is.EqualTo("192.0.2.53"));
                Assert.That(model.ServerValue("dns-1", "Transport"),        Is.EqualTo("TLS"));
                Assert.That(model.ServerValue("dns-1", "Port"),             Is.EqualTo("853"));
                Assert.That(model.ServerValue("dns-1", "QueryTimeout"),     Is.EqualTo("2.5"));
                Assert.That(model.ServerValue("dns-1", "RootPins"),         Is.EqualTo(ARoot), "in the form the node keeps");
                Assert.That(model.ServerValue("dns-1", "CertificatePins"),  Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "OnPinMismatch"),    Is.EqualTo("Record"));
                Assert.That(model.ServerValue("dns-1", "TrustOnFirstUse"),  Is.EqualTo("Certificate"));

                Assert.That(model.ServerValue("dns-2", "Address"),          Is.EqualTo("2001:db8::53"));
                Assert.That(model.ServerValue("dns-2", "Transport"),        Is.EqualTo("UDP"));
                Assert.That(model.ServerValue("dns-2", "Port"),             Is.EqualTo("53"));
                Assert.That(model.ServerValue("dns-2", "QueryTimeout"),     Is.EqualTo(""), "the client's");
                Assert.That(model.ServerValue("dns-2", "OnPinMismatch"),    Is.EqualTo("Refuse"));
                Assert.That(model.ServerValue("dns-2", "TrustOnFirstUse"),  Is.EqualTo("None"));

                // No handshake yet.
                Assert.That(model.ServerValue("dns-1", "Problem"),            Is.EqualTo("false"));
                Assert.That(model.ServerValue("dns-1", "CertificateStatus"),  Is.EqualTo(""));
                Assert.That(model.ServerValue("dns-1", "KnownSince"),         Is.EqualTo(""));

            });

        }

        #endregion

        #region WhatIsNotThereIsNotThere()

        /// <summary>
        /// A server the node does not have - the slots "1" and "16" of
        /// before among them - and a variable the node cannot answer are not
        /// there at all; ids are read in any case, as OCPP reads instances.
        /// </summary>
        [Test]
        public void WhatIsNotThereIsNotThere()
        {

            Assert.Multiple(() => {

                foreach (var variable in DNSDeviceModel.ServerVariables)
                {
                    Assert.That(model.ServerValue("dns-3", variable), Is.Null, variable);
                    Assert.That(model.ServerValue("1",     variable), Is.Null, variable);
                    Assert.That(model.ServerValue("16",    variable), Is.Null, variable);
                }

                Assert.That(model.ServerValue("DNS-2", "Transport"), Is.EqualTo("UDP"), "in any case");

                foreach (var notYet in new[] { "AuthenticationName", "Enabled", "Priority" })
                    Assert.That(model.ServerValue("dns-1", notYet), Is.Null, notYet);

                Assert.That(model.ClientValue("NoSuchThing"), Is.Null);

            });

        }

        #endregion

        #region NamesAreReadInAnyCase()

        /// <summary>
        /// OCPP compares the names of variables without regard to case.
        /// </summary>
        [Test]
        public void NamesAreReadInAnyCase()
        {

            Assert.Multiple(() => {
                Assert.That(model.ClientValue("dnssecok"),         Is.EqualTo("true"));
                Assert.That(model.ServerValue("dns-1", "TRANSPORT"),     Is.EqualTo("TLS"));
            });

        }

        #endregion

        #region AChangeElsewhereIsReadAtTheNextAsk()

        /// <summary>
        /// Nothing is kept: what the page changes is what the next GetVariables
        /// reads, the three ways recursion can be set included, and a server
        /// the page takes away is gone - here the second, as a page sending its
        /// first back without an id leaves it "dns-1".
        /// </summary>
        [Test]
        public void AChangeElsewhereIsReadAtTheNextAsk()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(new JObject(new JProperty("recursionDesired", false)),              out var error), Is.True, error);
            Assert.That(model.ClientValue("RecursionDesired"), Is.EqualTo("Unset"));

            Assert.That(station.Node.TryUpdateDNSConfiguration(new JObject(new JProperty("recursionDesired", JValue.CreateNull())), out error),     Is.True, error);
            Assert.That(model.ClientValue("RecursionDesired"), Is.EqualTo("Default"));

            Assert.That(station.Node.TryUpdateDNSConfiguration(new JObject(
                                                              new JProperty("servers", new JArray(
                                                                  new JObject(
                                                                      new JProperty("address",                  "2001:db8::53"),
                                                                      new JProperty("transport",                "TLS"),
                                                                      new JProperty("certificateFingerprints",  new JArray(ACertificate, AnotherOne))
                                                                  )
                                                              ))
                                                          ),
                                                          out error),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(model.ServerValue("dns-1", "Address"),          Is.EqualTo("2001:db8::53"));
                Assert.That(model.ServerValue("dns-1", "CertificatePins"),  Is.EqualTo($"{ACertificate},{AnotherOne}"));
                Assert.That(model.ServerValue("dns-2", "Address"),          Is.Null);
                Assert.That(model.ClientValue("Servers"),                   Is.EqualTo("dns-1"));
            });

        }

        #endregion

        #region SwitchedOffTheServersStillRead()

        /// <summary>
        /// Name resolution switched off holds no servers in its client, yet the
        /// instances keep the servers the file has, which are what comes back
        /// when it is switched on again.
        /// </summary>
        [Test]
        public void SwitchedOffTheServersStillRead()
        {

            Assert.That(station!.Node.TryUpdateDNSConfiguration(new JObject(new JProperty("enabled", false)), out var error), Is.True, error);

            Assert.Multiple(() => {
                Assert.That(model.ClientValue("Enabled"),     Is.EqualTo("false"));
                Assert.That(model.ServerValue("dns-1", "Address"),  Is.EqualTo("192.0.2.53"));
                Assert.That(model.ServerValue("dns-2", "Address"),  Is.EqualTo("2001:db8::53"));
            });

        }

        #endregion

        #region TheOCPPNodeReadsTheStationsNameResolution()

        /// <summary>
        /// The node's OCPP 2.1 node answers GetVariables for the DNS
        /// controllers from the node: a change made elsewhere is what
        /// it reads next, a server added or taken away on the page comes or goes
        /// as an instance, and a node rebuilt for changed EVSEs has them again.
        /// </summary>
        [Test]
        public void TheOCPPNodeReadsTheStationsNameResolution()
        {

            static String? ValueIn(protocols.OCPPv2_1.IDeviceModelNode  Node,
                                   String                                         Component,
                                   String?                                        Instance,
                                   String                                         Variable)

                => protocols.OCPPv2_1.DeviceModel.GetVariables(
                       [
                           new protocols.OCPPv2_1.GetVariableData(
                               Component:   new protocols.OCPPv2_1.Component(Component, Instance),
                               Variable:    new protocols.OCPPv2_1.Variable(Variable)
                           )
                       ],
                       (name, evse) => Node.GetComponentConfigs(name, evse)
                   ).Single().AttributeValue;

            var before = station!.OCPPv2_1Node;

            Assert.Multiple(() => {
                Assert.That(ValueIn(before, "dnsclientcontroller", null, "DNSSECOK"),     Is.EqualTo("true"));
                Assert.That(ValueIn(before, "DNSServerController", "dns-2",  "Address"),  Is.EqualTo("2001:db8::53"));
                Assert.That(ValueIn(before, "DNSServerController", "dns-3",  "Address"),  Is.Null, "no such server");
            });

            Assert.That(station.Node.TryUpdateDNSConfiguration(new JObject(new JProperty("dnssecOK", false)), out var error), Is.True, error);

            Assert.That(ValueIn(before, "DNSClientController", null, "DNSSECOK"), Is.EqualTo("false"), "read again, not kept");

            // The page takes the first away and adds one, sending back the
            // ids it was shown: the second keeps its own, and the new one is
            // the next free.
            Assert.That(station.Node.TryUpdateDNSConfiguration(JObject.Parse("""
                                                                        { "servers": [ { "id": "dns-2", "address": "2001:db8::53" },
                                                                                       { "address": "198.51.100.53" } ] }
                                                                        """),
                                                          out error),
                        Is.True, error);

            Assert.Multiple(() => {
                Assert.That(before.AllComponentConfigs.Where (component => component.Name == nameof(protocols.OCPPv2_1.DNSServerController)).
                                                       Select(component => component.Instance),  Is.EquivalentTo(new[] { "dns-2", "dns-3" }));
                Assert.That(ValueIn(before, "DNSServerController", "dns-3",  "Address"),         Is.EqualTo("198.51.100.53"));
                Assert.That(ValueIn(before, "DNSServerController", "dns-1",  "Address"),         Is.Null, "taken away");
                Assert.That(ValueIn(before, "DNSClientController", null,     "Servers"),         Is.EqualTo("dns-2,dns-3"));
            });

            // OCPP nodes built anew, as a charging station builds them when
            // its EVSEs changed.
            station.NewOCPPNodes();

            var after = station.OCPPv2_1Node;

            Assert.Multiple(() => {
                Assert.That(after,                                                     Is.Not.SameAs(before), "the node was rebuilt");
                Assert.That(after.AllComponentConfigs.Where (component => component.Name == nameof(protocols.OCPPv2_1.DNSServerController)).
                                                      Select(component => component.Instance),  Is.EquivalentTo(new[] { "dns-2", "dns-3" }));
                Assert.That(ValueIn(after, "DNSServerController", "dns-3",  "Address"),         Is.EqualTo("198.51.100.53"));
            });

        }

        #endregion

        #region TheLastHandshakeReadsAsAVerdict()

        /// <summary>
        /// What the node made of a server's certificate last, as the node writes
        /// it down: a refusal is a problem, the outcome becomes an entry of the
        /// valuesList, and since when a server is known is written as OCPP
        /// writes a time.
        /// </summary>
        [Test]
        public void TheLastHandshakeReadsAsAVerdict()
        {

            var section  = station!.Node.DNSConfigurationJSON();
            var server   = (JObject) section["servers"]![0]!;

            server["judgement"]  = new ServerJudgement(
                                       Service:                 "dns",
                                       Server:                  "tls://192.0.2.53:853",
                                       At:                      DateTimeOffset.UtcNow,
                                       Accepted:                false,
                                       Outcome:                 "pinMismatch",
                                       CertificateFingerprint:  ACertificate,
                                       RootFingerprint:         AnotherOne,
                                       AnchoredBy:              null,
                                       HeldTo:                  null,
                                       Learned:                 TrustOnFirstUse.None,
                                       Previously:              null,
                                       FirstSeen:               false,
                                       Steps:                   []
                                   ).ToJSON();

            server["known"]      = new JObject(
                                       new JProperty("certificate",  ACertificate),
                                       new JProperty("root",         AnotherOne),
                                       new JProperty("since",        new DateTimeOffset(2026, 10, 29, 14, 3, 7, TimeSpan.FromHours(1)).AddTicks(1234567).ToString("o"))
                                   );

            Assert.Multiple(() => {

                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "Problem"),                 Is.EqualTo("true"));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "CertificateStatus"),       Is.EqualTo("PinMismatch"));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "CertificateFingerprint"),  Is.EqualTo(ACertificate));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "RootFingerprint"),         Is.EqualTo(AnotherOne));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "KnownSince"),              Is.EqualTo("2026-10-29T13:03:07.123Z"));

                // A server believed is no problem.
                server["judgement"]!["accepted"] = true;
                server["judgement"]!["outcome"]  = "accepted";

                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "Problem"),                 Is.EqualTo("false"));
                Assert.That(DNSDeviceModel.ServerValueIn(section, "dns-1", "CertificateStatus"),       Is.EqualTo("Accepted"));

            });

        }

        #endregion

    }

}
