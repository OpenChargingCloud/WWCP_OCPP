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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.NN
{

    /// <summary>
    /// A NotifyNetworkTopology message sent by the local controller has to arrive at the CSMS.
    /// </summary>
    /// <remarks>
    /// THIS TEST IS EXPECTED TO FAIL until the gap below is closed. It is not a broken test,
    /// and weakening the assertion would only hide what it is here to record.
    ///
    /// Measured, hop by hop: the local controller has a route to the CSMS, hands the message
    /// to its outgoing adapter, and the adapter reports SentMessageResults.Success over the
    /// socket pair whose far end is the CSMS server's own port. At the CSMS, not a single
    /// receive event fires - not OnJSONSendMessageReceived, not OnJSONRequestMessageReceived,
    /// not the typed OnNotifyNetworkTopologyMessageReceived. The charging station does not see
    /// it either. The message is written to the right socket and then vanishes.
    ///
    /// Both halves exist: OCPPAdapter.SendJSONSendMessage sends it, and the incoming adapter
    /// parses OCPP_JSONSendMessage and raises OnJSONSendMessageReceived. So this is not a
    /// missing implementation but something between the socket and the parse chain, and
    /// nobody has pinned down what yet. The next step is to capture the raw frame as the
    /// CSMS receives it.
    ///
    /// NN_Messages_Tests.NotifyNetworkTopology_Test1 and _Test2 fail for this same reason,
    /// but only report "The Network Topology Notification did not reach the CSMS!" without
    /// saying how far it got. This test does.
    /// </remarks>
    [TestFixture]
    public class NN_TopologyNotificationDelivery : AChargingStationWithNetworkingNodeTests
    {

        #region ATopologyNotificationReachesTheCSMS()

        [Test]
        public async Task ATopologyNotificationReachesTheCSMS()
        {

            Assert.Multiple(() => {
                Assert.That(localController1,        Is.Not.Null);
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
            });

            var lcSent       = new ConcurrentList<String>();
            var lcJsonOut    = new ConcurrentList<String>();
            var csmsJsonIn   = new ConcurrentList<String>();
            var csmsTypedIn  = new ConcurrentList<String>();

            localController1!.OCPP.OUT.OnNotifyNetworkTopologyMessageSent     += (t, s, c, m, r, ct) => { lcSent.     TryAdd(r.ToString());                     return Task.CompletedTask; };
            localController1. OCPP.OUT.OnJSONSendMessageSent                  += (t, s, c, m, r, ct) => { lcJsonOut.  TryAdd($"dest={m.Destination} sent={r}"); return Task.CompletedTask; };
            testCSMS1!.       OCPP.IN. OnJSONSendMessageReceived              += (t, s, c, m, ct)    => { csmsJsonIn. TryAdd($"dest={m.Destination}");          return Task.CompletedTask; };
            testCSMS1.        OCPP.IN. OnNotifyNetworkTopologyMessageReceived += (t, s, c, m, ct)    => { csmsTypedIn.TryAdd("received");                       return Task.CompletedTask; };

            var response = await localController1.OCPP.OUT.NotifyNetworkTopology(
                                     new NotifyNetworkTopologyMessage(
                                         SourceRouting.CSMS,
                                         new NetworkTopologyInformation(
                                             RoutingNode:  localController1.Id,
                                             Routes:       [
                                                               new NetworkRoutingInformation(
                                                                   DestinationId:   localController1.Id,
                                                                   Priority:        23
                                                               )
                                                           ],
                                             Priority:     5
                                         )
                                     )
                                 );

            // A send message is fire and forget, so give it a moment to cross the wire.
            await Task.Delay(500);

            Assert.Multiple(() => {

                // The sending half - this part works today.
                Assert.That(localController1.Routing.LookupNetworkingNode(NetworkingNode_Id.CSMS, out _),
                                                 Is.True,                             "The local controller has no route to the CSMS!");
                Assert.That(lcSent.Count,        Is.EqualTo(1),                        "The NotifyNetworkTopology message did not leave the local controller!");
                Assert.That(lcJsonOut.Count,     Is.EqualTo(1),                        "The message was never handed to the outgoing JSON adapter!");
                Assert.That(lcJsonOut.First(),   Does.Contain("sent=Success"),         "The message left the local controller unsent!");
                Assert.That(response.Result,     Is.EqualTo(SentMessageResults.Success));

                // The receiving half - this is what is broken.
                Assert.That(csmsJsonIn.Count,    Is.EqualTo(1),                        "The message was written to the CSMS socket, but its incoming adapter never surfaced it!");
                Assert.That(csmsTypedIn.Count,   Is.EqualTo(1),                        "The CSMS never received the NotifyNetworkTopology message!");

            });

        }

        #endregion

    }

}
