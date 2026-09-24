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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A charging station can be asked something the moment it has connected.
    /// </summary>
    /// <remarks>
    /// The CSMS's WebSocket server used to answer the upgrade with 101 Switching
    /// Protocols first and register the charging station afterwards, and the
    /// charging station in turn wrote down who the CSMS is only once its connect
    /// had returned. A request in between found the charging station unknown to
    /// the CSMS, or found a charging station that could not tell whom to answer.
    /// Both register before that moment now: the server before its 101, the
    /// client before it reads its first frame.
    ///
    /// The first test holds the CSMS for a moment after it has sent the 101,
    /// which is where the old order left the charging station unknown. The
    /// second sends a request in the moment the charging station is registered
    /// and its 101 not yet sent: it has to follow the 101 rather than go out
    /// ahead of it, and the charging station has to be able to answer it.
    /// </remarks>
    [TestFixture]
    public class ReachableTheMomentItHasConnectedTests : ACSMSTests
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


        #region (private) NewChargingStation  (Id)

        private TestChargingStationNode NewChargingStation(String Id)

            => chargingStation = new TestChargingStationNode(
                                     Id:                     NetworkingNode_Id.Parse(Id),
                                     VendorName:             "GraphDefined OEM #1",
                                     Model:                  "VCP.1",
                                     DisableSendHeartbeats:  true,
                                     HTTPAPI_Disabled:       true,
                                     WebAPI_Disabled:        true,
                                     DNSClient:              testCSMS1!.DNSClient
                                 );

        #endregion

        #region (private) Connect             (ChargingStation)

        private Task<HTTPResponse> Connect(TestChargingStationNode ChargingStation)
        {

            testCSMS1!.AddOrUpdateHTTPBasicAuth(ChargingStation.Id, "1234abcd");

            return ChargingStation.ConnectOCPPWebSocketClient(
                       NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                       RemoteURL:                URL.Parse($"http://127.0.0.1:{testBackendWebSockets1!.IPPort}/{ChargingStation.Id}"),
                       HTTPAuthentication:       HTTPBasicAuthentication.Create(ChargingStation.Id.ToString(), "1234abcd"),
                       DisableWebSocketPings:    true
                   );

        }

        #endregion

        #region (private) GetLocalListVersion (ChargingStation)

        private Task<SendRequestState> GetLocalListVersion(TestChargingStationNode ChargingStation)

            => testCSMS1!.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:   Timestamp.Now,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkingMode:     NetworkingMode.Standard,
                       Destination:        SourceRouting.To(ChargingStation.Id),
                       NetworkPath:        NetworkPath.From(testCSMS1.Id),
                       RequestId:          Request_Id.NewRandom(),
                       Action:             "GetLocalListVersion",
                       Payload:            new JObject(),
                       RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                   )
               );

        #endregion


        #region AChargingStationCanBeAskedTheMomentItHasConnected()

        /// <summary>
        /// The CSMS held for a moment after its 101: the charging station has its
        /// answer and is asked at once, while the CSMS has not moved on yet.
        /// </summary>
        [Test]
        public async Task AChargingStationCanBeAskedTheMomentItHasConnected()
        {

            // After the 101 has gone out, and for longer than the charging
            // station needs to read it.
            HTTPResponseLogDelegate hold = (timestamp, server, request, response, ct) => Task.Delay(1500, ct);

            testBackendWebSockets1!.OnHTTPResponse += hold;

            try
            {

                var station  = NewChargingStation("GD-RC-001");
                var upgrade  = await Connect(station);
                var state    = await GetLocalListVersion(station);

                Assert.Multiple(() => {

                    Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

                    Assert.That(state.JSONResponse,      Is.Not.Null,
                                $"The CSMS did not know the charging station that had just connected: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                });

            }
            finally
            {
                testBackendWebSockets1.OnHTTPResponse -= hold;
            }

        }

        #endregion

        #region ARequestSentWhileItIsAcceptedFollowsThe101()

        /// <summary>
        /// A request sent in the moment the charging station is registered and its
        /// 101 not yet sent - not waited for there - is answered once it is
        /// connected.
        /// </summary>
        [Test]
        public async Task ARequestSentWhileItIsAcceptedFollowsThe101()
        {

            var station = NewChargingStation("GD-RC-002");

            Task<SendRequestState>? early = null;

            // Subscribed after the server's own registration, which comes first:
            // the charging station is known here, and its 101 is still to be sent.
            OnNewWebSocketConnectionDelegate ask = async (timestamp, server, connection, sharedSubprotocols, selectedSubprotocol, eventTrackingId, ct) => {
                early = GetLocalListVersion(station);
                // Long enough for a request that did not wait to reach the socket -
                // where it would go ahead of the 101, and the station's client,
                // reading its way to the end of the HTTP response, would swallow it.
                await Task.Delay(500, ct);
            };

            testBackendWebSockets1!.OnWebSocketConnectionAccepted += ask;

            try
            {

                var upgrade = await Connect(station);

                Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols),
                            "The upgrade failed.");

                Assert.That(early,                   Is.Not.Null,
                            "The connection was never accepted.");

                var state = await early!;

                Assert.That(state.JSONResponse,      Is.Not.Null,
                            $"The request sent while the connection was accepted was not answered: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

            }
            finally
            {
                testBackendWebSockets1.OnWebSocketConnectionAccepted -= ask;
            }

        }

        #endregion

    }

}
