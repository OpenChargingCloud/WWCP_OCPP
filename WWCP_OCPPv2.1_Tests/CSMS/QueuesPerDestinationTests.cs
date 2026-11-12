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

using System.Diagnostics;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A charging station connected to its CSMS and to a successor keeps what it
    /// has to tell each of them in a queue of its own.
    /// </summary>
    [TestFixture]
    public class QueuesPerDestinationTests : ACSMSTests
    {

        #region Data

        private static readonly NetworkingNode_Id  First   = NetworkingNode_Id.Parse("connection-first");
        private static readonly NetworkingNode_Id  Second  = NetworkingNode_Id.Parse("connection-second");

        private TestChargingStationNode?  chargingStation;

        #endregion

        #region StopTheChargingStation()

        [TearDown]
        public async Task StopTheChargingStation()
        {

            if (chargingStation is not null)
                await chargingStation.Stop();

            chargingStation = null;

        }

        #endregion


        #region (private) AStationConnectedToBoth(Id)

        /// <summary>
        /// Connected to the first back end, which is its CSMS, and to the second,
        /// each under a name of its own.
        /// </summary>
        protected async Task<TestChargingStationNode> AStationConnectedToBoth(String Id)
        {

            chargingStation = new TestChargingStationNode(
                                  Id:                     NetworkingNode_Id.Parse(Id),
                                  VendorName:             "GraphDefined OEM #1",
                                  Model:                  "VCP.1",
                                  DisableSendHeartbeats:  true,
                                  HTTPAPI_Disabled:       true,
                                  WebAPI_Disabled:        true,
                                  DNSClient:              testCSMS1!.DNSClient
                              );

            foreach (var (csms, server, name, csmsToo) in new[] { (testCSMS1!, testBackendWebSockets1!, First,  true),
                                                                 (testCSMS2!, testBackendWebSockets2!, Second, false) })
            {

                csms.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

                var upgrade = await chargingStation.ConnectOCPPWebSocketClient(
                                        NextHopNetworkingNodeId:   name,
                                        RoutingNetworkingNodeIds:  csmsToo ? [ NetworkingNode_Id.CSMS ] : null,
                                        RemoteURL:                 URL.Parse($"http://127.0.0.1:{server.IPPort}/{chargingStation.Id}"),
                                        HTTPAuthentication:        HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd"),
                                        SecWebSocketProtocols:     OCPPSubprotocols.Offer([ Version.WebSocketSubProtocolId ], Encoding),
                                        DisableWebSocketPings:     true
                                    );

                Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            }

            return chargingStation;

        }

        #endregion

        #region (private static) Told(Received, Type)

        /// <summary>
        /// Whether a security event of the given type was received, within ten seconds.
        /// </summary>
        private static async Task<Boolean> Told(List<LogJSONRequest> Received, String Type)
        {

            var watch = Stopwatch.StartNew();

            while (watch.Elapsed < TimeSpan.FromSeconds(10))
            {

                if (WasTold(Received, Type))
                    return true;

                await Task.Delay(20);

            }

            return WasTold(Received, Type);

        }

        private static Boolean WasTold(List<LogJSONRequest> Received, String Type)

            => Received.ToArray().Any(request => request.JSONMessage[2]?.Value<String>()       == "SecurityEventNotification" &&
                                                 request.JSONMessage[3]?["type"]?.Value<String>() == Type);

        #endregion


        #region WhatIsForTheSecondGoesToTheSecond()

        [Test]
        public async Task WhatIsForTheSecondGoesToTheSecond()
        {

            var station = await AStationConnectedToBoth("GD-QD-001");

            await station.QueueSecurityEventAsync(SecurityEventType.Parse("ForTheSecond"), Destination: Second);
            await station.QueueSecurityEventAsync(SecurityEventType.Parse("ForTheCSMS"));

            await Assert.MultipleAsync(async () => {

                Assert.That(await Told(csms2WebSocketJSONRequestsReceived!, "ForTheSecond"),  Is.True,  "the second back end was not told what was for it");
                Assert.That(await Told(csms1WebSocketJSONRequestsReceived!, "ForTheCSMS"),    Is.True,  "the CSMS was not told what was for it");

                Assert.That(WasTold(csms1WebSocketJSONRequestsReceived!, "ForTheSecond"),         Is.False, "the CSMS was told what was for the second back end");
                Assert.That(WasTold(csms2WebSocketJSONRequestsReceived!, "ForTheCSMS"),           Is.False, "the second back end was told what was for the CSMS");

                Assert.That(station.OfflineQueue.Pending,                                     Is.Empty);
                Assert.That(station.OfflineQueueFor(Second).Pending,                          Is.Empty);

            });

        }

        #endregion

        #region ASecondThatIsHeldHoldsUpNothingElse()

        /// <summary>
        /// The second back end has not accepted the station: what is for it waits,
        /// and what is for the CSMS goes.
        /// </summary>
        [Test]
        public async Task ASecondThatIsHeldHoldsUpNothingElse()
        {

            var station = await AStationConnectedToBoth("GD-QD-002");

            station.HoldOfflineQueueFor(Second, true);

            await station.QueueSecurityEventAsync(SecurityEventType.Parse("ForTheSecond"), Destination: Second);
            await station.QueueSecurityEventAsync(SecurityEventType.Parse("ForTheCSMS"));

            Assert.That(await Told(csms1WebSocketJSONRequestsReceived!, "ForTheCSMS"), Is.True,
                        "what was for the CSMS waited for the second back end");

            Assert.Multiple(() => {
                Assert.That(WasTold(csms2WebSocketJSONRequestsReceived!, "ForTheSecond"),  Is.False, "the held back end was told");
                Assert.That(station.OfflineQueueFor(Second).Pending,                   Has.Count.EqualTo(1));
            });

            station.HoldOfflineQueueFor(Second, false);

            await station.FlushOfflineQueueAsync(Second);

            Assert.That(await Told(csms2WebSocketJSONRequestsReceived!, "ForTheSecond"), Is.True,
                        "what was for the second back end did not go once it was let go");

        }

        #endregion

    }

}
