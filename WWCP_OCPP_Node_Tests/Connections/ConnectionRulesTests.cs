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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// What a kind of node writes down as a connection: a node that dials
    /// its back end as it always did, and a node that dials downwards - a
    /// CSMS dialling a charging station or a local controller - by rules of
    /// its own.
    /// </summary>
    /// <remarks>
    /// The first half is the one that must not move: a charging station and
    /// a local controller say nothing of any rules, and what they wrote down
    /// before is written down, read and routed as before.
    /// </remarks>
    [TestFixture]
    public class ConnectionRulesTests
    {

        #region Data

        private String           directory  = "";
        private ConnectionStore  store      = default!;

        /// <summary>
        /// What a CSMS says of its connections.
        /// </summary>
        private static ConnectionRules DiallingDownwards

            => new () {
                   Types                  = ConnectionRules.NodesBelow,
                   Inbound                = false,
                   SSH                    = false,
                   OCPP1_6                = false,
                   NamesTheNodeItLeadsTo  = true
               };

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory  = TestNodes.TemporaryDirectory("connectionRules");
            store      = new ConnectionStore(Path.Combine(directory, "conn"));
        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            TestNodes.Remove(directory);
        }

        #endregion


        #region A node that dials its back end: as it always was

        [Test]
        public void ANodeThatSaysNothingOffersItsBackEnds()
        {

            var json = store.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["connectionTypes"]!.Select(type => type.Value<String>()),
                            Is.EqualTo(new[] { "CSMS", "CSMSBackup", "LocalController", "Gateway" }));
                Assert.That(json["ocppVersions"]!.Select(version => version.Value<String>()),
                            Is.EqualTo(new[] { "OCPP2.1", "OCPP1.6" }));
                Assert.That(json["rules"]!.Value<Boolean>("inbound"),                Is.True);
                Assert.That(json["rules"]!.Value<Boolean>("ssh"),                    Is.True);
                Assert.That(json["rules"]!.Value<Boolean>("namesTheNodeItLeadsTo"),  Is.False);
            });

        }

        [Test]
        public void ItsConnectionsAreRoutedUnderANameOfTheirOwn()
        {

            Assert.That(store.TryAddConnection("The CSMS", "wss://csms.example.org/cs001", "CSMS", true, null, null,
                                               out var id, out var error),
                        Is.True, error);

            var entry = store.Connections.Single();

            Assert.Multiple(() => {
                Assert.That(entry.RemoteNodeId,                              Is.Null);
                Assert.That(entry.ToJSON()[ConnectionEntry.RemoteNodeIdKey], Is.Null);
                Assert.That(OCPPDiallingNode.NextHopOf(entry),               Is.EqualTo(NetworkingNode_Id.Parse($"connection-{id}")));
            });

        }

        [Test]
        public void ItsConnectionsNameNoNodeAtTheirOtherEnd()
        {

            Assert.That(store.TryAddConnection("The CSMS", "wss://csms.example.org/cs001", "CSMS", true, null, null,
                                               out _, out var error,
                                               RemoteNodeId: "csms-01"),
                        Is.False);

            Assert.That(error, Does.Contain("names no node at its other end"));

        }

        [Test]
        public void AChargingStationIsNothingItDials()
        {

            Assert.That(store.TryAddConnection("A station", "wss://station.example.org/cs001", "ChargingStation", true, null, null,
                                               out _, out var error),
                        Is.False);

            Assert.That(error, Does.Contain("ChargingStation"));

        }

        #endregion

        #region A node that dials downwards

        [Test]
        public void ItOffersWhatIsBelowIt()
        {

            store.Rules = DiallingDownwards;

            var json = store.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["connectionTypes"]!.Select(type => type.Value<String>()),
                            Is.EqualTo(new[] { "ChargingStation", "LocalController" }));
                Assert.That(json["ocppVersions"]!.Select(version => version.Value<String>()),
                            Is.EqualTo(new[] { "OCPP2.1" }));
                Assert.That(json["rules"]!.Value<Boolean>("inbound"),                Is.False);
                Assert.That(json["rules"]!.Value<Boolean>("ssh"),                    Is.False);
                Assert.That(json["rules"]!.Value<Boolean>("namesTheNodeItLeadsTo"),  Is.True);
            });

        }

        [Test]
        public void AConnectionIsRoutedUnderTheNameOfTheNodeItLeadsTo()
        {

            store.Rules = DiallingDownwards;

            Assert.That(store.TryAddConnection("Station 7", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                               out var id, out var error,
                                               RemoteNodeId: "  cs007 "),
                        Is.True, error);

            var entry = store.Connections.Single();

            Assert.Multiple(() => {
                Assert.That(entry.RemoteNodeId,                                                Is.EqualTo("cs007"));
                Assert.That(entry.ToJSON().Value<String>(ConnectionEntry.RemoteNodeIdKey),     Is.EqualTo("cs007"));
                Assert.That(OCPPDiallingNode.NextHopOf(entry),                                 Is.EqualTo(NetworkingNode_Id.Parse("cs007")));
            });

            // And read back as it was written.
            var again = new ConnectionStore(store.Path) { Rules = DiallingDownwards };

            Assert.That(again.Connections.Single(one => one.Id == id).RemoteNodeId, Is.EqualTo("cs007"));

        }

        [Test]
        public void AConnectionThatNamesNoNodeIsNotWrittenDown()
        {

            store.Rules = DiallingDownwards;

            Assert.That(store.TryAddConnection("Station 7", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                               out _, out var error),
                        Is.False);

            Assert.That(error, Does.Contain("which node is at the other end"));

        }

        [TestCase("CSMS")]
        [TestCase("CentralSystem")]
        [TestCase("connection-1")]
        [TestCase("two words")]
        public void ANameAnOCPPNodeKeepsForItselfIsNoNodeBelow(String Name)
        {

            store.Rules = DiallingDownwards;

            Assert.That(store.TryAddConnection("Station 7", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                               out _, out var error,
                                               RemoteNodeId: Name),
                        Is.False);

            Assert.That(error, Is.Not.Empty);

        }

        [Test]
        public void ItDialsNoBackEndNoSSHAndNoOCPP16AndIsNotDialled()
        {

            store.Rules = DiallingDownwards;

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection("A CSMS", "wss://csms.example.org/csms-01", "CSMS", true, null, null,
                                                   out _, out var notBelow, RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(notBelow, Does.Contain("CSMS"));

                Assert.That(store.TryAddConnection("Over SSH", "ssh://csms-01@station7.example.org", "ChargingStation", true, null, null,
                                                   out _, out var overSSH, RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(overSSH, Does.Contain("ssh://"));

                Assert.That(store.TryAddConnection("An old one", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                                   out _, out var old, OCPPVersion: "OCPP1.6", RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(old, Does.Contain("OCPP 1.6"));

                Assert.That(store.TryAddConnection("Dialled", "wss://csms.example.org:9443/cs007", "ChargingStation", true, null, null,
                                                   out _, out var dialled, Inbound: true),
                            Is.False);
                Assert.That(dialled, Does.Contain("is one it dials"));

            });

            Assert.That(store.Connections, Is.Empty);

        }

        [Test]
        public void AChangeKeepsTheNodeUnlessItSaysAnother()
        {

            store.Rules = DiallingDownwards;

            Assert.That(store.TryAddConnection("Station 7", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                               out var id, out var error,
                                               RemoteNodeId: "cs007"),
                        Is.True, error);

            var revision = store.DialRevisionOf(id!);

            // Nothing said of it: kept.
            Assert.That(store.TryUpdateConnection(id, "Station seven", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                                  out error),
                        Is.True, error);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs007"));
            Assert.That(store.DialRevisionOf(id!),               Is.EqualTo(revision));

            // Another one: dialled again, under the other name.
            Assert.That(store.TryUpdateConnection(id, "Station seven", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                                  out error,
                                                  RemoteNodeId: "cs008"),
                        Is.True, error);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs008"));
            Assert.That(store.DialRevisionOf(id!),               Is.GreaterThan(revision));

            // None at all: not for a node that routes by it.
            Assert.That(store.TryUpdateConnection(id, "Station seven", "wss://station7.example.org:9443/csms-01", "ChargingStation", true, null, null,
                                                  out error,
                                                  RemoteNodeId: ""),
                        Is.False);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs008"));

        }

        #endregion

        #region AGatewayIsABackEnd_NotYetANodeBelow()

        /// <summary>
        /// A gateway is what a charging station, a local controller or another
        /// gateway dials as its back end: "Gateway" is written down by a node
        /// that dials its back end, and read back as itself. Not yet by a node
        /// that dials downwards: what a gateway below a CSMS is spoken to as is
        /// not decided, and a type a CSMS would speak to as a charging station
        /// is refused rather than mishandled.
        /// </summary>
        [Test]
        public void AGatewayIsABackEnd_NotYetANodeBelow()
        {

            Assert.That(ConnectionEntry.TryParseType("Gateway", out var type), Is.True);
            Assert.That(type,                                                  Is.EqualTo(ConnectionType.Gateway));

            Assert.That(store.TryAddConnection("The gateway above", "wss://gateway.example.org:2354/cs001", "Gateway", true, null, null,
                                               out var up, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(connection => connection.Id == up).ConnectionType, Is.EqualTo(ConnectionType.Gateway));

            store.Rules = DiallingDownwards;

            Assert.That(store.TryAddConnection("A gateway below", "wss://gateway7.example.org:9443/csms-01", "Gateway", true, null, null,
                                               out _, out error,
                                               RemoteNodeId: "gw007"),
                        Is.False);

            Assert.That(error, Does.Contain("is not what a connection of this"));

        }

        #endregion

    }

}
