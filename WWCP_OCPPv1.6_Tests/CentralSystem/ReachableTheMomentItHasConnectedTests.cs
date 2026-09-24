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
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// A charge point can be asked something the moment it has connected.
    /// </summary>
    /// <remarks>
    /// The central system's WebSocket server used to answer the upgrade with 101
    /// Switching Protocols first and register the charge point afterwards. A
    /// request sent in between - by a central system that asks a charge point
    /// something as soon as it is there, or by a test - found no route to it and
    /// came back UnknownClient at once. The CSMSCLI nightly's suite met that
    /// three times in eight runs on Debian. And the charge point in turn wrote
    /// down who the central system is only once its connect had returned, so a
    /// request that reached it first could not be answered. Both register before
    /// that moment now: the server before its 101, the client before it reads
    /// its first frame.
    ///
    /// The first test holds the server for a moment after it has sent the 101,
    /// which is where the old order left the charge point unknown. The second
    /// sends a request in the moment the connection is registered and not yet
    /// answered: that request has to follow the 101 rather than go out ahead of
    /// it, and the charge point has to be able to answer it.
    /// </remarks>
    [TestFixture]
    public class ReachableTheMomentItHasConnectedTests : ACentralSystemTests
    {

        #region (private) NewChargePoint      (Id)

        private TestChargePointNode NewChargePoint(String Id)

            => new (
                   ChargeBoxId:        NetworkingNode_Id.Parse(Id),
                   Connectors:         [ new ConnectorSpec(Availabilities.Operative, MaxPower: Watt.FromKW(22)) ],
                   ChargePointVendor:  "GraphDefined OEM #1",
                   ChargePointModel:   "VCP.1",
                   DNSClient:          testCentralSystem01!.DNSClient
               );

        #endregion

        #region (private) Connect             (ChargePoint)

        private Task<HTTPResponse> Connect(TestChargePointNode ChargePoint)
        {

            testCentralSystem01!.AddOrUpdateHTTPBasicAuth(ChargePoint.Id, "1234abcd");

            return ChargePoint.ConnectOCPPWebSocketClient(
                       URL.Parse($"http://127.0.0.1:{testBackendWebSockets01!.IPPort}/{ChargePoint.Id}"),
                       HTTPAuthentication: HTTPBasicAuthentication.Create(ChargePoint.Id.ToString(), "1234abcd")
                   );

        }

        #endregion

        #region (private) GetLocalListVersion (ChargePoint)

        private Task<SendRequestState> GetLocalListVersion(TestChargePointNode ChargePoint)

            => testCentralSystem01!.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:   Timestamp.Now,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkingMode:     NetworkingMode.Standard,
                       Destination:        SourceRouting.To(ChargePoint.Id),
                       NetworkPath:        NetworkPath.From(testCentralSystem01.Id),
                       RequestId:          Request_Id.NewRandom(),
                       Action:             "GetLocalListVersion",
                       Payload:            new JObject(),
                       RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                   )
               );

        #endregion


        #region AChargePointCanBeAskedTheMomentItHasConnected()

        /// <summary>
        /// The server held for a moment after its 101: the charge point has its
        /// answer and is asked at once, while the server has not moved on yet.
        /// </summary>
        [Test]
        public async Task AChargePointCanBeAskedTheMomentItHasConnected()
        {

            // After the 101 has gone out, and for longer than the charge point
            // needs to read it.
            HTTPResponseLogDelegate hold = (timestamp, server, request, response, ct) => Task.Delay(1500, ct);

            testBackendWebSockets01!.OnHTTPResponse += hold;

            try
            {

                var chargePoint  = NewChargePoint("GD101");
                var upgrade      = await Connect(chargePoint);
                var state        = await GetLocalListVersion(chargePoint);

                Assert.Multiple(() => {

                    Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

                    Assert.That(state.JSONResponse,      Is.Not.Null,
                                $"The central system did not know the charge point that had just connected: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                });

            }
            finally
            {
                testBackendWebSockets01.OnHTTPResponse -= hold;
            }

        }

        #endregion

        #region ARequestSentWhileItIsAcceptedFollowsThe101()

        /// <summary>
        /// A request sent in the moment the charge point is registered and its 101
        /// not yet sent - not waited for there - is answered once it is connected.
        /// </summary>
        [Test]
        public async Task ARequestSentWhileItIsAcceptedFollowsThe101()
        {

            var chargePoint = NewChargePoint("GD102");

            Task<SendRequestState>? early = null;

            // Subscribed after the server's own registration, which comes first:
            // the charge point is known here, and its 101 is still to be sent.
            OnNewWebSocketConnectionDelegate ask = async (timestamp, server, connection, sharedSubprotocols, selectedSubprotocol, eventTrackingId, ct) => {
                early = GetLocalListVersion(chargePoint);
                // Long enough for a request that did not wait to reach the socket -
                // where it would go ahead of the 101, and the station's client,
                // reading its way to the end of the HTTP response, would swallow it.
                await Task.Delay(500, ct);
            };

            testBackendWebSockets01!.OnWebSocketConnectionAccepted += ask;

            try
            {

                var upgrade = await Connect(chargePoint);

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
                testBackendWebSockets01.OnWebSocketConnectionAccepted -= ask;
            }

        }

        #endregion

    }

}
