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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Whether a connection speaks CBOR is agreed on where it is made, by its
    /// subprotocol: "ocpp2.1cbor" next to "ocpp2.1".
    ///
    /// A charging station that offers both gets CBOR from a CSMS that speaks it and
    /// JSON from one that does not; one that offers CBOR alone is not connected to
    /// a CSMS without it. Over a connection that agreed on CBOR a message goes out
    /// as CBOR in a binary frame, in both directions, and is read as that at the far
    /// end - which, as long as the message processors do not serve CBOR, answers a
    /// request with a request error in CBOR. Over a connection that agreed on JSON
    /// no CBOR message goes out at all.
    /// </summary>
    [TestFixture]
    public class OCPPOverCBORTests : ACSMSTests
    {

        #region Data

        private static readonly String[]   versions       = [ Version.WebSocketSubProtocolId, "ocpp2.0.1" ];

        private TestChargingStationNode?   chargingStation;
        private OCPPWebSocketServer?       cborWebSockets;

        #endregion

        #region StopTheChargingStation()

        [TearDown]
        public async Task StopTheChargingStation()
        {

            if (chargingStation is not null)
                await chargingStation.Stop();

            chargingStation = null;

            if (cborWebSockets is not null)
                await cborWebSockets.Shutdown();

            cborWebSockets = null;

        }

        #endregion

        #region (private) ACSMSThatSpeaksCBOR()

        /// <summary>
        /// A second WebSocket server of the CSMS, which takes "ocpp2.1cbor" as well.
        /// </summary>
        private OCPPWebSocketServer ACSMSThatSpeaksCBOR()

            => cborWebSockets = testCSMS1!.AttachWebSocketServer(
                                    TCPPort:                 null,
                                    Description:             I18NString.Create($"{testCSMS1.Id} HTTP WebSocket Server with CBOR"),
                                    RequireAuthentication:   true,
                                    SecWebSocketProtocols:   OCPPSubprotocols.Offer(versions, MessageEncodings.Auto),
                                    DisableWebSocketPings:   true,
                                    AutoStart:               true
                                );

        #endregion

        #region (private) Connect(Id, Server, Encoding)

        private async Task<HTTPResponse> Connect(String               Id,
                                                 OCPPWebSocketServer  Server,
                                                 MessageEncodings     Encoding)
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

            testCSMS1.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

            return await chargingStation.ConnectOCPPWebSocketClient(
                             NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                             RemoteURL:                URL.Parse($"http://127.0.0.1:{Server.IPPort}/{chargingStation.Id}"),
                             HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd"),
                             SecWebSocketProtocols:    OCPPSubprotocols.Offer(versions, Encoding),
                             DisableWebSocketPings:    true
                         );

        }

        #endregion

        #region (private) AgreedOn / AHeartbeat

        private String? AgreedOn
            => chargingStation!.WWCPWebSocketClients.OfType<WWCPWebSocketClient>().Single().WebSocketClientConnection?.Subprotocol;

        private static OCPP_CBORRequestMessage AHeartbeat(NetworkingNode_Id  From,
                                                          NetworkingNode_Id  To,
                                                          Request_Id         RequestId)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.To(To),
                    NetworkPath.From(From),
                    RequestId,
                    "Heartbeat",
                    CBORValue.FromMap([]));

        #endregion


        #region OfferingBoth_AgreesOnCBORWhereTheCSMSSpeaksIt()

        [Test]
        public async Task OfferingBoth_AgreesOnCBORWhereTheCSMSSpeaksIt()
        {

            var upgrade = await Connect("GD-CBOR-001", ACSMSThatSpeaksCBOR(), MessageEncodings.Auto);

            Assert.Multiple(() => {
                Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(AgreedOn,                Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));
            });

        }

        #endregion

        #region OfferingBoth_FallsBackToJSONWhereTheCSMSDoesNot()

        [Test]
        public async Task OfferingBoth_FallsBackToJSONWhereTheCSMSDoesNot()
        {

            var upgrade = await Connect("GD-CBOR-002", testBackendWebSockets1!, MessageEncodings.Auto);

            Assert.Multiple(() => {
                Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(AgreedOn,                Is.EqualTo(Version.WebSocketSubProtocolId));
            });

        }

        #endregion

        #region OfferingJSON_GetsJSONFromACSMSThatSpeaksCBOR()

        [Test]
        public async Task OfferingJSON_GetsJSONFromACSMSThatSpeaksCBOR()
        {

            var upgrade = await Connect("GD-CBOR-003", ACSMSThatSpeaksCBOR(), MessageEncodings.JSON);

            Assert.Multiple(() => {
                Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(AgreedOn,                Is.EqualTo(Version.WebSocketSubProtocolId));
            });

        }

        #endregion

        #region OfferingCBORAlone_IsNotConnectedToACSMSWithoutIt()

        [Test]
        public async Task OfferingCBORAlone_IsNotConnectedToACSMSWithoutIt()
        {

            var upgrade = await Connect("GD-CBOR-004", testBackendWebSockets1!, MessageEncodings.CBOR);

            Assert.That(upgrade.HTTPStatusCode,  Is.Not.EqualTo(HTTPStatusCode.SwitchingProtocols));

        }

        #endregion


        #region ARequestOfTheStation_GoesOutAsCBORAndIsAnsweredInCBOR()

        [Test]
        public async Task ARequestOfTheStation_GoesOutAsCBORAndIsAnsweredInCBOR()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-005", server, MessageEncodings.Auto);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            var atTheCSMS     = new TaskCompletionSource<Byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            var atTheStation  = new TaskCompletionSource<Byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            server.OnBinaryMessageReceived += (timestamp, webSocketServer, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) => {
                atTheCSMS.TrySetResult(binaryMessage);
                return Task.CompletedTask;
            };

            var client = chargingStation!.OCPPWebSocketClients.Single();

            client.OnBinaryMessageReceived += (timestamp, webSocketClient, connection, eventTrackingId, messageTimestamp, sourceNodeId, binaryMessage, cancellationToken) => {
                atTheStation.TrySetResult(binaryMessage);
                return Task.CompletedTask;
            };

            var requestId  = Request_Id.NewRandom();
            var sent       = await client.SendCBORRequest(AHeartbeat(chargingStation.Id, NetworkingNode_Id.CSMS, requestId));

            Assert.That(sent.Result, Is.EqualTo(SentMessageResults.Success));

            var received   = await atTheCSMS.   Task.WaitAsync(TimeSpan.FromSeconds(10));
            var answer     = await atTheStation.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.That(OCPP_CBORFrame.TryParse(received, out var requestArray, out var e1),  Is.True, e1);
            Assert.That(OCPP_CBORRequestMessage.TryParse(requestArray, out var request, out var e2), Is.True, e2);

            Assert.That(OCPP_CBORFrame.TryParse(answer,   out var answerArray,  out var e3),  Is.True, e3);
            Assert.That(OCPP_CBORRequestErrorMessage.TryParse(answerArray, out var requestError, out var e4), Is.True, e4);

            Assert.Multiple(() => {
                Assert.That(request!.RequestId,             Is.EqualTo(requestId));
                Assert.That(request.Action,                 Is.EqualTo("Heartbeat"));
                Assert.That(requestError!.RequestId,        Is.EqualTo(requestId));
                Assert.That(requestError.ErrorCode,         Is.EqualTo(ResultCode.NotImplemented));
                Assert.That(requestError.ErrorDescription,  Does.Contain("Heartbeat"));
            });

        }

        #endregion

        #region ARequestOfTheCSMS_GoesOutAsCBORAndIsAnsweredInCBOR()

        [Test]
        public async Task ARequestOfTheCSMS_GoesOutAsCBORAndIsAnsweredInCBOR()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-006", server, MessageEncodings.CBOR);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
            Assert.That(AgreedOn,               Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

            var atTheCSMS = new TaskCompletionSource<Byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            server.OnBinaryMessageReceived += (timestamp, webSocketServer, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) => {
                atTheCSMS.TrySetResult(binaryMessage);
                return Task.CompletedTask;
            };

            var requestId  = Request_Id.NewRandom();
            var sent       = await server.SendCBORRequest(AHeartbeat(testCSMS1!.Id, chargingStation!.Id, requestId));

            Assert.That(sent.Result, Is.EqualTo(SentMessageResults.Success));

            var answer     = await atTheCSMS.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.That(OCPP_CBORFrame.TryParse(answer, out var answerArray, out var e1), Is.True, e1);
            Assert.That(OCPP_CBORRequestErrorMessage.TryParse(answerArray, out var requestError, out var e2), Is.True, e2);

            Assert.Multiple(() => {
                Assert.That(requestError!.RequestId,  Is.EqualTo(requestId));
                Assert.That(requestError.ErrorCode,   Is.EqualTo(ResultCode.NotImplemented));
            });

        }

        #endregion

        #region OverAConnectionThatAgreedOnJSON_NoCBORGoesOut()

        [Test]
        public async Task OverAConnectionThatAgreedOnJSON_NoCBORGoesOut()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-007", server, MessageEncodings.JSON);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            var binaryFrames = 0;

            server.OnBinaryMessageReceived += (timestamp, webSocketServer, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) => {
                Interlocked.Increment(ref binaryFrames);
                return Task.CompletedTask;
            };

            var fromTheStation  = await chargingStation!.OCPPWebSocketClients.Single().SendCBORRequest(AHeartbeat(chargingStation.Id, NetworkingNode_Id.CSMS, Request_Id.NewRandom()));
            var fromTheCSMS     = await server.SendCBORRequest(AHeartbeat(testCSMS1!.Id, chargingStation.Id, Request_Id.NewRandom()));

            await Task.Delay(250);

            Assert.Multiple(() => {
                Assert.That(fromTheStation.Result,  Is.EqualTo(SentMessageResults.TransmissionFailed));
                Assert.That(fromTheCSMS.Result,     Is.EqualTo(SentMessageResults.UnknownClient));
                Assert.That(binaryFrames,           Is.Zero);
            });

        }

        #endregion

    }

}
