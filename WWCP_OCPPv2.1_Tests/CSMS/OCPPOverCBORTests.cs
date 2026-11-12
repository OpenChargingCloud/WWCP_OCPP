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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

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
    /// as CBOR in a binary frame, in both directions, is read as that at the far end
    /// and answered in CBOR - by the same processors that answer it in JSON. Over a
    /// connection that agreed on JSON no CBOR message goes out at all.
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

        #region (private) ACSMSThatSpeaksJSONAlone()

        /// <summary>
        /// A WebSocket server of the CSMS that is handed the JSON subprotocols alone.
        /// </summary>
        private OCPPWebSocketServer ACSMSThatSpeaksJSONAlone()

            => cborWebSockets = testCSMS1!.AttachWebSocketServer(
                                    TCPPort:                 null,
                                    Description:             I18NString.Create($"{testCSMS1.Id} HTTP WebSocket Server without CBOR"),
                                    RequireAuthentication:   true,
                                    SecWebSocketProtocols:   versions,
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

            var upgrade = await Connect("GD-CBOR-002", ACSMSThatSpeaksJSONAlone(), MessageEncodings.Auto);

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

            var upgrade = await Connect("GD-CBOR-004", ACSMSThatSpeaksJSONAlone(), MessageEncodings.CBOR);

            Assert.That(upgrade.HTTPStatusCode,  Is.Not.EqualTo(HTTPStatusCode.SwitchingProtocols));

        }

        #endregion


        #region AServerTakesCBORByItself()

        /// <summary>
        /// The WebSocket server every CSMS of these tests has was handed no
        /// subprotocols at all, and takes "ocpp2.1cbor" from whoever offers it.
        /// </summary>
        [Test]
        public async Task AServerTakesCBORByItself()
        {

            var upgrade = await Connect("GD-CBOR-008", testBackendWebSockets1!, MessageEncodings.Auto);

            Assert.Multiple(() => {
                Assert.That(upgrade.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(AgreedOn,                Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));
            });

        }

        #endregion


        #region (private) Frames(Server)

        /// <summary>
        /// Count the frames between the charging station and the given server, by kind.
        /// </summary>
        private (Func<Int32> Text, Func<List<Byte[]>> AtTheCSMS, Func<List<Byte[]>> AtTheStation) Frames(OCPPWebSocketServer Server)
        {

            var text          = 0;
            var atTheCSMS     = new List<Byte[]>();
            var atTheStation  = new List<Byte[]>();
            var client        = chargingStation!.OCPPWebSocketClients.Single();

            Server.OnJSONMessageReceived   += (timestamp, webSocketServer, connection, messageTimestamp, eventTrackingId, sourceNodeId, jsonMessage, cancellationToken) => {
                Interlocked.Increment(ref text);
                return Task.CompletedTask;
            };

            client.OnJSONMessageReceived   += (timestamp, webSocketClient, connection, eventTrackingId, messageTimestamp, sourceNodeId, jsonMessage, cancellationToken) => {
                Interlocked.Increment(ref text);
                return Task.CompletedTask;
            };

            Server.OnBinaryMessageReceived += (timestamp, webSocketServer, connection, messageTimestamp, eventTrackingId, sourceNodeId, binaryMessage, cancellationToken) => {
                lock (atTheCSMS)
                    atTheCSMS.Add(binaryMessage);
                return Task.CompletedTask;
            };

            client.OnBinaryMessageReceived += (timestamp, webSocketClient, connection, eventTrackingId, messageTimestamp, sourceNodeId, binaryMessage, cancellationToken) => {
                lock (atTheStation)
                    atTheStation.Add(binaryMessage);
                return Task.CompletedTask;
            };

            return (() => text,
                    () => { lock (atTheCSMS)    return [.. atTheCSMS];    },
                    () => { lock (atTheStation) return [.. atTheStation]; });

        }

        #endregion

        #region ARequestOfTheStation_GoesOutAsCBORAndIsAnsweredInCBOR()

        /// <summary>
        /// The charging station sends its BootNotification the way it always does. It
        /// crosses the wire as a CBOR request in a binary frame, the CSMS processes it
        /// as it processes every BootNotification, and its answer comes back as a CBOR
        /// response - with nothing in a text frame on the way.
        /// </summary>
        [Test]
        public async Task ARequestOfTheStation_GoesOutAsCBORAndIsAnsweredInCBOR()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-005", server, MessageEncodings.Auto);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
            Assert.That(AgreedOn,               Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

            var frames    = Frames(server);
            var seen      = new List<BootNotificationRequest>();

            testCSMS1!.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, request, cancellationToken) => {
                lock (seen)
                    seen.Add(request);
                return Task.CompletedTask;
            };

            var response  = await chargingStation!.SendBootNotification(BootReason.PowerUp);

            Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK), response.Result.Description);

            var atTheCSMS     = frames.AtTheCSMS();
            var atTheStation  = frames.AtTheStation();

            Assert.Multiple(() => {
                Assert.That(response.Status,       Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That(seen,                  Has.Count.EqualTo(1));
                Assert.That(seen[0].Reason,        Is.EqualTo(BootReason.PowerUp));
                Assert.That(frames.Text(),         Is.Zero, "Text frames over a CBOR connection");
                Assert.That(atTheCSMS,             Has.Count.EqualTo(1));
                Assert.That(atTheStation,          Has.Count.EqualTo(1));
            });

            Assert.That(OCPP_CBORFrame.TryParse(atTheCSMS[0],    out var requestArray,  out var e1),  Is.True, e1);
            Assert.That(OCPP_CBORRequestMessage. TryParse(requestArray,  out var request,     out var e2), Is.True, e2);

            Assert.That(OCPP_CBORFrame.TryParse(atTheStation[0], out var responseArray, out var e3),  Is.True, e3);
            Assert.That(OCPP_CBORResponseMessage.TryParse(responseArray, out var cborResponse, out var e4), Is.True, e4);

            Assert.Multiple(() => {
                Assert.That(request!.Action,           Is.EqualTo("BootNotification"));
                Assert.That(cborResponse!.RequestId,   Is.EqualTo(request.RequestId));
                // What CBOR says better than JSON is said that way: the time of the CSMS
                // is a tagged point in time, not a text.
                Assert.That(cborResponse.Payload.ToDiagnosticString(), Does.Not.Contain("\"20"));
            });

        }

        #endregion

        #region ARequestOfTheCSMS_GoesOutAsCBORAndIsAnsweredInCBOR()

        /// <summary>
        /// The other way round: the CSMS asks the charging station to reset.
        /// </summary>
        [Test]
        public async Task ARequestOfTheCSMS_GoesOutAsCBORAndIsAnsweredInCBOR()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-006", server, MessageEncodings.CBOR);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
            Assert.That(AgreedOn,               Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

            var frames    = Frames(server);
            var seen      = new List<ResetRequest>();

            chargingStation!.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, request, cancellationToken) => {
                lock (seen)
                    seen.Add(request);
                return Task.CompletedTask;
            };

            var response  = await testCSMS1!.Reset(
                                      Destination:  SourceRouting.To(chargingStation.Id),
                                      ResetType:    ResetType.OnIdle
                                  );

            Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK), response.Result.Description);

            Assert.Multiple(() => {
                Assert.That(seen,                    Has.Count.EqualTo(1));
                Assert.That(seen[0].ResetType,       Is.EqualTo(ResetType.OnIdle));
                Assert.That(frames.Text(),           Is.Zero, "Text frames over a CBOR connection");
                Assert.That(frames.AtTheStation(),   Has.Count.EqualTo(1));
                Assert.That(frames.AtTheCSMS(),      Has.Count.EqualTo(1));
            });

        }

        #endregion

        #region ARequestThatCannotBeRead_IsAnsweredWithARequestError()

        /// <summary>
        /// A CBOR request whose payload is not what its action asks for is answered
        /// with a request error in CBOR, and one of an action nobody knows as well:
        /// whoever asked does not wait out its timeout.
        /// </summary>
        [Test]
        public async Task ARequestThatCannotBeRead_IsAnsweredWithARequestError()
        {

            var server    = ACSMSThatSpeaksCBOR();
            var upgrade   = await Connect("GD-CBOR-009", server, MessageEncodings.CBOR);

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            var frames    = Frames(server);
            var client    = chargingStation!.OCPPWebSocketClients.Single();

            var broken    = Request_Id.NewRandom();
            var unknown   = Request_Id.NewRandom();

            // A BootNotification without its charging station and its reason.
            Assert.That((await client.SendCBORRequest(new OCPP_CBORRequestMessage(
                                                          Timestamp.Now,
                                                          EventTracking_Id.New,
                                                          NetworkingMode.Standard,
                                                          SourceRouting.To(NetworkingNode_Id.CSMS),
                                                          NetworkPath.From(chargingStation.Id),
                                                          broken,
                                                          "BootNotification",
                                                          CBORValue.FromMap([])
                                                      ))).Result,
                        Is.EqualTo(SentMessageResults.Success));

            Assert.That((await client.SendCBORRequest(new OCPP_CBORRequestMessage(
                                                          Timestamp.Now,
                                                          EventTracking_Id.New,
                                                          NetworkingMode.Standard,
                                                          SourceRouting.To(NetworkingNode_Id.CSMS),
                                                          NetworkPath.From(chargingStation.Id),
                                                          unknown,
                                                          "NobodyKnowsThis",
                                                          CBORValue.FromMap([])
                                                      ))).Result,
                        Is.EqualTo(SentMessageResults.Success));

            var until = Timestamp.Now + TimeSpan.FromSeconds(10);

            while (frames.AtTheStation().Count < 2 && Timestamp.Now < until)
                await Task.Delay(25);

            var errors = new Dictionary<Request_Id, OCPP_CBORRequestErrorMessage>();

            foreach (var frame in frames.AtTheStation())
            {
                Assert.That(OCPP_CBORFrame.TryParse(frame, out var array, out var e1), Is.True, e1);
                Assert.That(OCPP_CBORRequestErrorMessage.TryParse(array, out var error, out var e2), Is.True, e2);
                errors[error!.RequestId] = error;
            }

            Assert.That(errors.Keys, Is.EquivalentTo(new[] { broken, unknown }));

            Assert.Multiple(() => {
                Assert.That(errors[broken]. ErrorCode,  Is.EqualTo(ResultCode.FormationViolation));
                // Read blind, and answered as an unknown action in JSON is.
                Assert.That(errors[unknown].ErrorCode,  Is.EqualTo(ResultCode.ProtocolError));
                Assert.That(frames.Text(),              Is.Zero, "Text frames over a CBOR connection");
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
