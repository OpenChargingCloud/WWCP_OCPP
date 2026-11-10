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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A charging station connected to two back ends at once answers each of them
    /// over the connection its request came over.
    /// </summary>
    /// <remarks>
    /// Both connections lead to "CSMS", and a node has one route by that name: to
    /// whichever connection came up last. An answer used to be looked up by that
    /// name, so the answer to the first back end's request went to the second one,
    /// which knew nothing of it, and the first waited out its timeout. And an
    /// error to a request the station had no processor for was addressed to nobody
    /// at all, and dropped before it was sent.
    /// </remarks>
    [TestFixture]
    public class AnswersFollowTheirConnectionTests : ACSMSTests
    {

        #region Data

        private TestChargingStationNode? chargingStation;

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
        /// A charging station connected to the first back end and then to the
        /// second one, both as its CSMS.
        /// </summary>
        private async Task<TestChargingStationNode> AStationConnectedToBoth(String Id)
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

            foreach (var (csms, server) in new[] { (testCSMS1!, testBackendWebSockets1!),
                                                   (testCSMS2!, testBackendWebSockets2!) })
            {

                csms.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

                var upgrade = await chargingStation.ConnectOCPPWebSocketClient(
                                        NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                        RemoteURL:                URL.Parse($"http://127.0.0.1:{server.IPPort}/{chargingStation.Id}"),
                                        HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd"),
                                        DisableWebSocketPings:    true
                                    );

                Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols),
                            $"The charging station could not connect to '{csms.Id}'.");

            }

            return chargingStation;

        }

        #endregion

        #region (private static) Ask(CSMS, ChargingStation, Action)

        private static Task<SendRequestState> Ask(TestCSMSNode       CSMS,
                                                  NetworkingNode_Id  ChargingStation,
                                                  String             Action)

            => CSMS.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:   Timestamp.Now,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkingMode:     NetworkingMode.Standard,
                       Destination:        SourceRouting.To(ChargingStation),
                       NetworkPath:        NetworkPath.From(CSMS.Id),
                       RequestId:          Request_Id.NewRandom(),
                       Action:             Action,
                       Payload:            new JObject(),
                       RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                   )
               );

        #endregion


        #region TheBackEndThatConnectedFirstIsAnswered()

        /// <summary>
        /// The first back end asks, the second one connected last: the answer
        /// goes to the first, and the second receives nothing.
        /// </summary>
        [Test]
        public async Task TheBackEndThatConnectedFirstIsAnswered()
        {

            var station  = await AStationConnectedToBoth("GD-MH-001");
            var state    = await Ask(testCSMS1!, station.Id, "GetLocalListVersion");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,                         Is.Not.Null,
                            $"The back end that asked was not answered: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                Assert.That(csms2WebSocketJSONResponsesReceived,        Is.Empty,
                            "The back end that connected last was handed the answer to another one's request.");

            });

        }

        #endregion

        #region TheBackEndThatConnectedLastIsAnswered()

        /// <summary>
        /// The other way round, which worked before as well, and has to go on
        /// working.
        /// </summary>
        [Test]
        public async Task TheBackEndThatConnectedLastIsAnswered()
        {

            var station  = await AStationConnectedToBoth("GD-MH-002");
            var state    = await Ask(testCSMS2!, station.Id, "GetLocalListVersion");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,                         Is.Not.Null,
                            $"The back end that asked was not answered: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                Assert.That(csms1WebSocketJSONResponsesReceived,        Is.Empty,
                            "The back end that connected first was handed the answer to another one's request.");

            });

        }

        #endregion

        #region AnUnknownActionIsRefusedToTheBackEndThatSentIt()

        /// <summary>
        /// A request the charging station has no processor for is refused, and the
        /// refusal reaches the back end that sent it, rather than its timeout.
        /// </summary>
        [Test]
        public async Task AnUnknownActionIsRefusedToTheBackEndThatSentIt()
        {

            var station  = await AStationConnectedToBoth("GD-MH-003");
            var state    = await Ask(testCSMS1!, station.Id, "NoSuchAction");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,                         Is.Null);

                Assert.That(state.JSONRequestErrorMessage?.ErrorCode,   Is.EqualTo(ResultCode.ProtocolError),
                            "The refusal of an unknown action did not reach the back end that sent it.");

                Assert.That(csms2WebSocketJSONRequestErrorsReceived,    Is.Empty,
                            "The refusal went to the back end that connected last.");

            });

        }

        #endregion

    }

}
