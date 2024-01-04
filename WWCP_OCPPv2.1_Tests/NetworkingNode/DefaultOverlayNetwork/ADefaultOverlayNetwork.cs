﻿﻿/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode
{

    /// <summary>
    /// An OCPP Overlay Network consisting of a charging station connected to a networking node using normal networking mode and
    /// the networking node connected to the CSMS using the networking extensions.
    /// </summary>
    public abstract class ADefaultOverlayNetwork
    {

        #region Data

        protected TestCSMS?                     CSMS;

        protected CSMSWSServer?                 csmsWSServer;

        protected List<LogJSONRequest>          csmsWebSocketJSONMessagesReceived                        = [];
        protected List<LogDataJSONResponse>     csmsWebSocketJSONMessageResponsesSent                    = [];
        protected List<LogJSONRequest>          csmsWebSocketJSONMessagesSent                            = [];
        protected List<LogDataJSONResponse>     csmsWebSocketJSONMessageResponsesReceived                = [];

        protected List<LogBinaryRequest>        csmsWebSocketBinaryMessagesReceived                      = [];
        protected List<LogDataBinaryResponse>   csmsWebSocketBinaryMessageResponsesSent                  = [];
        protected List<LogBinaryRequest>        csmsWebSocketBinaryMessagesSent                          = [];
        protected List<LogDataBinaryResponse>   csmsWebSocketBinaryMessageResponsesReceived              = [];

        // -------------------------------------------------------------------------------------------------------

        protected TestNetworkingNode?           networkingNode;

        protected OCPPWebSocketServer?          nnOCPPWebSocketServer;

        protected List<LogJSONRequest>          networkingNodeWebSocketJSONMessagesReceived              = [];
        protected List<LogDataJSONResponse>     networkingNodeWebSocketJSONMessageResponsesSent          = [];
        protected List<LogJSONRequest>          networkingNodeWebSocketJSONMessagesSent                  = [];
        protected List<LogDataJSONResponse>     networkingNodeWebSocketJSONMessageResponsesReceived      = [];

        protected List<LogBinaryRequest>        networkingNodeWebSocketBinaryMessagesReceived            = [];
        protected List<LogDataBinaryResponse>   networkingNodeWebSocketBinaryMessageResponsesSent        = [];
        protected List<LogBinaryRequest>        networkingNodeWebSocketBinaryMessagesSent                = [];
        protected List<LogDataBinaryResponse>   networkingNodeWebSocketBinaryMessageResponsesReceived    = [];

        // -------------------------------------------------------------------------------------------------------

        protected TestChargingStation?          chargingStation;

        protected List<LogJSONRequest>          chargingStation1WebSocketJSONMessagesReceived             = [];
        protected List<LogDataJSONResponse>     chargingStation1WebSocketJSONMessageResponsesSent         = [];
        protected List<LogJSONRequest>          chargingStation1WebSocketJSONMessagesSent                 = [];
        protected List<LogDataJSONResponse>     chargingStation1WebSocketJSONMessageResponsesReceived     = [];

        protected List<LogBinaryRequest>        chargingStation1WebSocketBinaryMessagesReceived           = [];
        protected List<LogDataBinaryResponse>   chargingStation1WebSocketBinaryMessageResponsesSent       = [];
        protected List<LogBinaryRequest>        chargingStation1WebSocketBinaryMessagesSent               = [];
        protected List<LogDataBinaryResponse>   chargingStation1WebSocketBinaryMessageResponsesReceived   = [];

        #endregion

        #region Properties



        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async Task SetupOnce()
        {

            #region Init

            Timestamp.Reset();

            var dnsClient   = new DNSClient(
                                  SearchForIPv6DNSServers: false,
                                  SearchForIPv4DNSServers: false
                              );

            #endregion

            #region Create the CSMS

            CSMS          = new TestCSMS(
                                Id:                      NetworkingNode_Id.Parse("csms01"),
                                RequireAuthentication:   true,
                                HTTPUploadPort:          IPPort.Parse(9100),
                                DNSClient:               dnsClient
                            );

            Assert.That(CSMS,         Is.Not.Null);

            csmsWSServer  = CSMS.AttachWebSocketService(
                                TCPPort:                 IPPort.Parse(9101),
                                DisableWebSocketPings:   true,
                                AutoStart:               true
                            );

            Assert.That(csmsWSServer, Is.Not.Null);

            //csmsWSServer.OnTextMessageReceived         += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
            //    csmsWebSocketJSONMessagesReceived.        Add(new LogJSONRequest(timestamp, JArray.Parse(requestMessage)));
            //    return Task.CompletedTask;
            //};

            //csmsWSServer.OnJSONMessageResponseSent     += (timestamp, webSocketServer, webSocketConnection, networkingNodeId, networkPath, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage, cancellationToken) => {
            //    csmsWebSocketJSONMessageResponsesSent.    Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage ?? []));
            //    return Task.CompletedTask;
            //};

            //csmsWSServer.OnTextMessageSent             += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
            //    csmsWebSocketJSONMessagesSent.            Add(new LogJSONRequest(timestamp, JArray.Parse(requestMessage)));
            //    return Task.CompletedTask;
            //};

            //csmsWSServer.OnJSONMessageResponseReceived += (timestamp, webSocketServer, webSocketConnection, networkingNodeId, networkPath, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage, cancellationToken) => {
            //    csmsWebSocketJSONMessageResponsesReceived.Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage ?? []));
            //    return Task.CompletedTask;
            //};

            #endregion

            #region Create the Networking Node

            networkingNode  = new TestNetworkingNode(
                                  Id:                      NetworkingNode_Id.Parse("nn01"),
                                  VendorName:              "GraphDefined OEM #1",
                                  Model:                   "VCP.1",
                                  Description:             I18NString.Create(Languages.en, "Our first virtual networking node!"),
                                  SerialNumber:            "SN-NN0001",
                                  FirmwareVersion:         "v0.1",
                                  Modem:                   new Modem(
                                                               ICCID:   "0001",
                                                               IMSI:    "1112"
                                                           ),
                                  DisableSendHeartbeats:   true,

                                  //HTTPBasicAuth:           new Tuple<String, String>("GDNN001", "1234"),
                                  DNSClient:               dnsClient
                              );

            Assert.That(networkingNode, Is.Not.Null);


            nnOCPPWebSocketServer        = networkingNode.AttachWebSocketServer(
                                               TCPPort:                 IPPort.Parse(9103),
                                               DisableWebSocketPings:   true,
                                               AutoStart:               true
                                           );

            Assert.That(nnOCPPWebSocketServer, Is.Not.Null);



            CSMS.AddOrUpdateHTTPBasicAuth(networkingNode.Id, "1234abcd");

            var connectionSetupResponse1  = await networkingNode.ConnectWebSocketClient(
                                                      NetworkingNodeId:        NetworkingNode_Id.CSMS,
                                                      RemoteURL:               URL.Parse($"http://127.0.0.1:{csmsWSServer.IPPort}/{networkingNode.Id}"),
                                                      HTTPAuthentication:      HTTPBasicAuthentication.Create(networkingNode.Id.ToString(), "1234abcd"),
                                                      DisableWebSocketPings:   true,
                                                      NetworkingMode:          OCPP.WebSockets.NetworkingMode.OverlayNetwork
                                                  );

            Assert.That(connectionSetupResponse1, Is.Not.Null);

            // HTTP/1.1 101 Switching Protocols
            // Date:                    Mon, 02 Apr 2023 15:55:18 GMT
            // Server:                  GraphDefined OCPP v2.0.1 HTTP/WebSocket/JSON CSMS API
            // Connection:              Upgrade
            // Upgrade:                 websocket
            // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
            // Sec-WebSocket-Protocol:  ocpp2.0.1
            // Sec-WebSocket-Version:   13

            ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                    connectionSetupResponse1.HTTPStatusCode);
            ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API",   connectionSetupResponse1.Server);
            ClassicAssert.AreEqual("Upgrade",                                                            connectionSetupResponse1.Connection);
            ClassicAssert.AreEqual("websocket",                                                          connectionSetupResponse1.Upgrade);
            ClassicAssert.IsTrue  (connectionSetupResponse1.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
            ClassicAssert.AreEqual("13",                                                                 connectionSetupResponse1.SecWebSocketVersion);


            //networkingNode.OCPP.IN.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
            //    networkingNode1WebSocketJSONMessagesReceived.        Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
            //};

            //networkingNode1WebSocketClient.OnJSONMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
            //    networkingNode1WebSocketJSONMessageResponsesSent.    Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
            //};

            //networkingNode1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
            //    networkingNode1WebSocketJSONMessagesSent.            Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
            //};

            //networkingNode1WebSocketClient.OnJSONMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
            //    networkingNode1WebSocketJSONMessageResponsesReceived.Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
            //};

            #endregion

            #region Create the Charging Station

            chargingStation  = new TestChargingStation(
                                   Id:                      NetworkingNode_Id.Parse("GD-CP001"),
                                   VendorName:              "GraphDefined OEM #1",
                                   Model:                   "VCP.1",
                                   Description:             I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                   SerialNumber:            "SN-CS0001",
                                   FirmwareVersion:         "v0.1",
                                   Modem:                   new Modem(
                                                                ICCID:   "0000",
                                                                IMSI:    "1111"
                                                            ),
                                   EVSEs:                   new[] {
                                                                new ChargingStationEVSE(
                                                                    Id:                  EVSE_Id.Parse(1),
                                                                    AdminStatus:         OperationalStatus.Operative,
                                                                    MeterType:           "MT1",
                                                                    MeterSerialNumber:   "MSN1",
                                                                    MeterPublicKey:      "MPK1",
                                                                    Connectors:          new[] {
                                                                                             new ChargingStationConnector(
                                                                                                 Id:              Connector_Id.Parse(1),
                                                                                                 ConnectorType:   ConnectorType.sType2
                                                                                             )
                                                                                         }
                                                                )
                                                            },
                                   MeterType:               "Virtual Energy Meter",
                                   MeterSerialNumber:       "SN-EN0001",
                                   MeterPublicKey:          "0xcafebabe",

                                   DisableSendHeartbeats:   true,

                                   //HTTPBasicAuth:           new Tuple<String, String>("OLI_001", "1234"),
                                   //HTTPBasicAuth:           new Tuple<String, String>("GD001", "1234"),
                                   DNSClient:               dnsClient
                               );

            Assert.That(chargingStation, Is.Not.Null);


            nnOCPPWebSocketServer.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

            var connectionSetupResponse2  = await chargingStation.ConnectWebSocket(
                                                      RemoteURL:               URL.Parse("http://127.0.0.1:" + nnOCPPWebSocketServer.IPPort.ToString() + "/" + chargingStation.Id),
                                                      HTTPAuthentication:      HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd"),
                                                      DisableWebSocketPings:   true
                                                  );

            Assert.That(connectionSetupResponse2, Is.Not.Null);

            // HTTP/1.1 101 Switching Protocols
            // Date:                    Mon, 02 Apr 2023 15:55:18 GMT
            // Server:                  GraphDefined OCPP v2.0.1 HTTP/WebSocket/JSON CSMS API
            // Connection:              Upgrade
            // Upgrade:                 websocket
            // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
            // Sec-WebSocket-Protocol:  ocpp2.0.1
            // Sec-WebSocket-Version:   13

            ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                               connectionSetupResponse2.HTTPStatusCode);
            ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API",   connectionSetupResponse2.Server);
            ClassicAssert.AreEqual("Upgrade",                                                                       connectionSetupResponse2.Connection);
            ClassicAssert.AreEqual("websocket",                                                                     connectionSetupResponse2.Upgrade);
            ClassicAssert.IsTrue  (connectionSetupResponse2.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
            ClassicAssert.AreEqual("13",                                                                            connectionSetupResponse2.SecWebSocketVersion);


            //var chargingStation1WebSocketClient = chargingStation.CSClient as ChargingStationWSClient;
            //ClassicAssert.IsNotNull(chargingStation1WebSocketClient);

            //if (chargingStation1WebSocketClient is not null)
            //{

            //    chargingStation1WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
            //        chargingStation1WebSocketJSONMessagesReceived.        Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
            //    };

            //    chargingStation1WebSocketClient.OnJSONMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
            //        chargingStation1WebSocketJSONMessageResponsesSent.    Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
            //    };

            //    chargingStation1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
            //        chargingStation1WebSocketJSONMessagesSent.            Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
            //    };

            //    chargingStation1WebSocketClient.OnJSONMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
            //        chargingStation1WebSocketJSONMessageResponsesReceived.Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
            //    };

            //}

            #endregion

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual void ShutdownOnce()
        {

            csmsWSServer.Shutdown();

            CSMS             = null;
            csmsWSServer     = null;

            //networkingNode.Shutdown();
            networkingNode   = null;

            chargingStation  = null;

        }

        #endregion


    }

}
