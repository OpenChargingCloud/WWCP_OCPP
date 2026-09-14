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
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.NN
{

    /// <summary>
    /// A NotifyNetworkTopology message sent by the local controller has to arrive at the CSMS.
    /// </summary>
    /// <remarks>
    /// This test used to fail, and it recorded the wrong reason. It said the message was
    /// "written to the right socket and then vanishes", and pointed at something between the
    /// socket and the parse chain. The socket was never the problem.
    ///
    /// Captured at the CSMS, the frame this test used to send was:
    ///
    ///     [6,"CSMS",[],"7A31vv...","NotifyNetworkTopology",{...}]
    ///                  ^^
    ///
    /// An empty network path. OCPP_JSONSendMessage.TryParse rejects that - "The network path
    /// must not be empty!" - so the frame matched none of the five message kinds, fell out of
    /// the chain, and was dropped. Nothing said so: the incoming adapter reported the request
    /// and response parse errors but never the send one, so the only trace was a complaint
    /// about a JSON *request* that this frame never was. That reporting gap is fixed too.
    ///
    /// The path was empty because the test built NotifyNetworkTopologyMessage by hand and
    /// the constructor defaults it. INetworkingNode.NotifyNetworkTopology() fills in
    /// NetworkPath.From(NetworkingNode.Id) - going through the low-level OCPP.OUT entry point
    /// skips that. This test now uses the extension method, like any other caller would.
    ///
    /// The second half of the old failure was simply a race: a send message is fire and
    /// forget, and the assertions ran before it could cross the wire.
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
            var csmsTypedIn  = new ConcurrentList<NotifyNetworkTopologyMessage>();

            localController1!.OCPP.OUT.OnNotifyNetworkTopologyMessageSent     += (t, s, c, m, r, ct) => { lcSent.     TryAdd(r.ToString());                     return Task.CompletedTask; };
            localController1. OCPP.OUT.OnJSONSendMessageSent                  += (t, s, c, m, r, ct) => { lcJsonOut.  TryAdd($"dest={m.Destination} path={m.NetworkPath} sent={r}"); return Task.CompletedTask; };
            testCSMS1!.       OCPP.IN. OnJSONSendMessageReceived              += (t, s, c, m, ct)    => { csmsJsonIn. TryAdd($"dest={m.Destination}");          return Task.CompletedTask; };
            testCSMS1.        OCPP.IN. OnNotifyNetworkTopologyMessageReceived += (t, s, c, m, ct)    => { csmsTypedIn.TryAdd(m);                                return Task.CompletedTask; };

            var response = await localController1.NotifyNetworkTopology(
                                     Destination:                  SourceRouting.CSMS,
                                     NetworkTopologyInformation:   new NetworkTopologyInformation(
                                                                       RoutingNode:  localController1.Id,
                                                                       Routes:       [
                                                                                         new NetworkRoutingInformation(
                                                                                             DestinationId:   localController1.Id,
                                                                                             Priority:        23
                                                                                         )
                                                                                     ],
                                                                       Priority:     5
                                                                   )
                                 );

            // A send message is fire and forget, so give it a moment to cross the wire.
            await Task.Delay(500);

            Assert.Multiple(() => {

                // The sending half
                Assert.That(localController1.Routing.LookupNetworkingNode(NetworkingNode_Id.CSMS, out _),
                                                 Is.True,                             "The local controller has no route to the CSMS!");
                Assert.That(lcSent.Count,        Is.EqualTo(1),                        "The NotifyNetworkTopology message did not leave the local controller!");
                Assert.That(lcJsonOut.Count,     Is.EqualTo(1),                        "The message was never handed to the outgoing JSON adapter!");
                Assert.That(lcJsonOut.First(),   Does.Contain("sent=Success"),         "The message left the local controller unsent!");
                Assert.That(response.Result,     Is.EqualTo(SentMessageResults.Success));

                // The network path has to carry the sender: a send message with an empty one is
                // rejected by the receiver's parser and dropped without ever becoming a message.
                Assert.That(lcJsonOut.First(),   Does.Contain($"path=[ {localController1.Id} ]"),
                                                                                       "The message went out without recording its sender!");

                // The receiving half
                Assert.That(csmsJsonIn.Count,    Is.EqualTo(1),                        "The message reached the CSMS socket, but its incoming adapter never surfaced it!");
                Assert.That(csmsTypedIn.Count,   Is.EqualTo(1),                        "The CSMS never received the NotifyNetworkTopology message!");

                Assert.That(csmsTypedIn.First().DestinationId,              Is.EqualTo(NetworkingNode_Id.CSMS));
                Assert.That(csmsTypedIn.First().NetworkPath.Length,         Is.EqualTo(1));
                Assert.That(csmsTypedIn.First().NetworkPath.Source,         Is.EqualTo(localController1.Id));
                Assert.That(csmsTypedIn.First().NetworkTopologyInformation, Is.Not.Null);

            });

        }

        #endregion

    }

}
