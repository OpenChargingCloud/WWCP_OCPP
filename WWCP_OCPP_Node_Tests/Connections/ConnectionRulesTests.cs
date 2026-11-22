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
using org.GraphDefined.Vanaheimr.Illias;
using URL = org.GraphDefined.Vanaheimr.Hermod.HTTP.URL;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// What a node writes down as a connection: one rule for every kind of
    /// node - any kind of node at the other end, a back end spoken to as
    /// that, a node named spoken to by its name, a charging station only by
    /// its name - and rules narrowed for a node, as for one that connects
    /// downwards alone.
    /// </summary>
    /// <remarks>
    /// The first half is the rule every node has: what a node wrote down
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
        private static ConnectionRules ConnectingDownwards

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


        #region One rule for every node

        [Test]
        public void ANodeThatSaysNothingOffersEveryKindOfNode()
        {

            var json = store.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["connectionTypes"]!.Select(type => type.Value<String>()),
                            Is.EqualTo(new[] { "CSMS", "CSMSBackup", "LocalController", "Gateway", "ChargingStation", "EnergyMeter" }));
                Assert.That(json["ocppVersions"]!.Select(version => version.Value<String>()),
                            Is.EqualTo(new[] { "OCPP2.1", "OCPP1.6" }));
                Assert.That(json["rules"]!.Value<Boolean>("inbound"),                Is.True);
                Assert.That(json["rules"]!.Value<Boolean>("ssh"),                    Is.True);
                Assert.That(json["rules"]!.Value<Boolean>("namesTheNodeItLeadsTo"),  Is.True);
            });

        }

        [Test]
        public void ItsConnectionsAreRoutedUnderANameOfTheirOwn()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("The CSMS"), URL.Parse("wss://csms.example.org/cs001"), "CSMS", true, null, null,
                                               out var id, out var error),
                        Is.True, error);

            var entry = store.Connections.Single();

            Assert.Multiple(() => {
                Assert.That(entry.RemoteNodeId,                              Is.Null);
                Assert.That(entry.ToJSON()[ConnectionEntry.RemoteNodeIdKey], Is.Null);
                Assert.That(WWCPNode.NextHopOf(entry),               Is.EqualTo(NetworkingNode_Id.Parse($"connection-{id}")));
            });

        }

        [Test]
        public void AConnectionMayNameTheNodeAtItsOtherEnd()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("The CSMS"), URL.Parse("wss://csms.example.org/cs001"), "CSMS", true, null, null,
                                               out _, out var error,
                                               RemoteNodeId: "csms-01"),
                        Is.True, error);

            Assert.That(WWCPNode.NextHopOf(store.Connections.Single()), Is.EqualTo(NetworkingNode_Id.Parse("csms-01")));

        }

        [Test]
        public void AChargingStationItConnectsToHasToBeNamed()
        {

            Assert.That(store.TryAddConnection(I18NString.Create("A station"), URL.Parse("wss://station.example.org/cs001"), "ChargingStation", true, null, null,
                                               out _, out var error),
                        Is.False);

            Assert.That(error, Does.Contain("which charging station is at the other end"));

            Assert.That(store.TryAddConnection(I18NString.Create("A station"), URL.Parse("wss://station.example.org/cs001"), "ChargingStation", true, null, null,
                                               out _, out error,
                                               RemoteNodeId: "cs007"),
                        Is.True, error);

            Assert.That(WWCPNode.NextHopOf(store.Connections.Single()), Is.EqualTo(NetworkingNode_Id.Parse("cs007")));

        }

        #endregion

        #region A node narrowed to connecting downwards

        [Test]
        public void ItOffersWhatIsBelowIt()
        {

            store.Rules = ConnectingDownwards;

            var json = store.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["connectionTypes"]!.Select(type => type.Value<String>()),
                            Is.EqualTo(new[] { "ChargingStation", "LocalController", "Gateway" }));
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

            store.Rules = ConnectingDownwards;

            Assert.That(store.TryAddConnection(I18NString.Create("Station 7"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                               out var id, out var error,
                                               RemoteNodeId: "  cs007 "),
                        Is.True, error);

            var entry = store.Connections.Single();

            Assert.Multiple(() => {
                Assert.That(entry.RemoteNodeId,                                                Is.EqualTo("cs007"));
                Assert.That(entry.ToJSON().Value<String>(ConnectionEntry.RemoteNodeIdKey),     Is.EqualTo("cs007"));
                Assert.That(WWCPNode.NextHopOf(entry),                                 Is.EqualTo(NetworkingNode_Id.Parse("cs007")));
            });

            // And read back as it was written.
            var again = new ConnectionStore(store.Path) { Rules = ConnectingDownwards };

            Assert.That(again.Connections.Single(one => one.Id == id).RemoteNodeId, Is.EqualTo("cs007"));

        }

        [Test]
        public void AConnectionThatNamesNoNodeIsNotWrittenDown()
        {

            store.Rules = ConnectingDownwards;

            Assert.That(store.TryAddConnection(I18NString.Create("Station 7"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                               out _, out var error),
                        Is.False);

            Assert.That(error, Does.Contain("which charging station is at the other end"));

        }

        [TestCase("CSMS")]
        [TestCase("CentralSystem")]
        [TestCase("connection-1")]
        [TestCase("two words")]
        public void ANameAnOCPPNodeKeepsForItselfIsNoNodeBelow(String Name)
        {

            store.Rules = ConnectingDownwards;

            Assert.That(store.TryAddConnection(I18NString.Create("Station 7"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                               out _, out var error,
                                               RemoteNodeId: Name),
                        Is.False);

            Assert.That(error, Is.Not.Empty);

        }

        [Test]
        public void ItConnectsToNoBackEndNoSSHAndNoOCPP16AndIsNotConnected()
        {

            store.Rules = ConnectingDownwards;

            Assert.Multiple(() => {

                Assert.That(store.TryAddConnection(I18NString.Create("A CSMS"), URL.Parse("wss://csms.example.org/csms-01"), "CSMS", true, null, null,
                                                   out _, out var notBelow, RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(notBelow, Does.Contain("CSMS"));

                Assert.That(store.TryAddConnection(I18NString.Create("Over SSH"), URL.Parse("ssh://csms-01@station7.example.org"), "ChargingStation", true, null, null,
                                                   out _, out var overSSH, RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(overSSH, Does.Contain("ssh://"));

                Assert.That(store.TryAddConnection(I18NString.Create("An old one"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                                   out _, out var old, OCPPVersion: "OCPP1.6", RemoteNodeId: "cs007"),
                            Is.False);
                Assert.That(old, Does.Contain("OCPP 1.6"));

                Assert.That(store.TryAddConnection(I18NString.Create("Inbound"), URL.Parse("wss://csms.example.org:9443/cs007"), "ChargingStation", true, null, null,
                                                   out _, out var inbound, Inbound: true),
                            Is.False);
                Assert.That(inbound, Does.Contain("is one it makes"));

            });

            Assert.That(store.Connections, Is.Empty);

        }

        [Test]
        public void AChangeKeepsTheNodeUnlessItSaysAnother()
        {

            store.Rules = ConnectingDownwards;

            Assert.That(store.TryAddConnection(I18NString.Create("Station 7"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                               out var id, out var error,
                                               RemoteNodeId: "cs007"),
                        Is.True, error);

            var revision = store.ConnectionRevisionOf(id!);

            // Nothing said of it: kept.
            Assert.That(store.TryUpdateConnection(id, I18NString.Create("Station seven"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                                  out error),
                        Is.True, error);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs007"));
            Assert.That(store.ConnectionRevisionOf(id!),               Is.EqualTo(revision));

            // Another one: connected to again, under the other name.
            Assert.That(store.TryUpdateConnection(id, I18NString.Create("Station seven"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                                  out error,
                                                  RemoteNodeId: "cs008"),
                        Is.True, error);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs008"));
            Assert.That(store.ConnectionRevisionOf(id!),               Is.GreaterThan(revision));

            // None at all: not for a node that routes by it.
            Assert.That(store.TryUpdateConnection(id, I18NString.Create("Station seven"), URL.Parse("wss://station7.example.org:9443/csms-01"), "ChargingStation", true, null, null,
                                                  out error,
                                                  RemoteNodeId: ""),
                        Is.False);

            Assert.That(store.Connections.Single().RemoteNodeId, Is.EqualTo("cs008"));

        }

        #endregion

        #region AGatewayIsABackEnd_AndANodeBelow()

        /// <summary>
        /// A gateway is what a charging station, a local controller or another
        /// gateway connects to as its back end, and what a CSMS connects to downwards:
        /// "Gateway" is written down by a node that connects to its back end and by
        /// one that connects downwards, named after the node it leads to, and
        /// read back as itself - not as the charging station a connection that
        /// says nothing is there.
        /// </summary>
        [Test]
        public void AGatewayIsABackEnd_AndANodeBelow()
        {

            Assert.That(ConnectionEntry.TryParseType("Gateway", out var type), Is.True);
            Assert.That(type,                                                  Is.EqualTo(ConnectionType.Gateway));

            Assert.That(store.TryAddConnection(I18NString.Create("The gateway above"), URL.Parse("wss://gateway.example.org:2354/cs001"), "Gateway", true, null, null,
                                               out var up, out var error),
                        Is.True, error);

            Assert.That(store.Connections.Single(connection => connection.Id == up).ConnectionType, Is.EqualTo(ConnectionType.Gateway));

            store.Rules = ConnectingDownwards;

            Assert.That(store.TryAddConnection(I18NString.Create("A gateway below"), URL.Parse("wss://gateway7.example.org:9443/csms-01"), "Gateway", true, null, null,
                                               out var down, out error,
                                               RemoteNodeId: "gw007"),
                        Is.True, error);

            var below = store.Connections.Single(connection => connection.Id == down);

            Assert.Multiple(() => {
                Assert.That(below.ConnectionType,  Is.EqualTo(ConnectionType.Gateway));
                Assert.That(below.RemoteNodeId,    Is.EqualTo("gw007"));
            });

        }

        #endregion

    }

}
