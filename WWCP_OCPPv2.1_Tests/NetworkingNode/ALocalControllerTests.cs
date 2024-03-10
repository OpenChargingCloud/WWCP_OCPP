﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.LocalController;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode
{

    /// <summary>
    /// Local Controller test defaults.
    /// </summary>
    public abstract class ALocalControllerTests : ACSMSTests
    {

        #region Data

        protected TestLocalController?          localController1;
        protected TestLocalController?          localController2;
        protected TestLocalController?          localController3;

        protected OCPPWebSocketServer?          lcOCPPWebSocketServer01;
        protected OCPPWebSocketServer?          lcOCPPWebSocketServer02;
        protected OCPPWebSocketServer?          lcOCPPWebSocketServer03;


        protected List<LogJSONRequest>?         networkingNode1WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    networkingNode1WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         networkingNode1WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    networkingNode1WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       networkingNode1WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  networkingNode1WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       networkingNode1WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  networkingNode1WebSocketBinaryMessageResponsesReceived;


        protected List<LogJSONRequest>?         networkingNode2WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    networkingNode2WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         networkingNode2WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    networkingNode2WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       networkingNode2WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  networkingNode2WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       networkingNode2WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  networkingNode2WebSocketBinaryMessageResponsesReceived;


        protected List<LogJSONRequest>?         networkingNode3WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    networkingNode3WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         networkingNode3WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    networkingNode3WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       networkingNode3WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  networkingNode3WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       networkingNode3WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  networkingNode3WebSocketBinaryMessageResponsesReceived;

        #endregion

        #region Properties
        public Boolean  InitNetworkingNode1    { get; set; } = false;
        public Boolean  InitNetworkingNode2    { get; set; } = false;
        public Boolean  InitNetworkingNode3    { get; set; } = false;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            #region Networking Node #1

            if (InitNetworkingNode1)
            {

                networkingNode1WebSocketJSONMessagesReceived          = [];
                networkingNode1WebSocketJSONMessageResponsesSent      = [];
                networkingNode1WebSocketJSONMessagesSent              = [];
                networkingNode1WebSocketJSONMessageResponsesReceived  = [];

                var networkingNode1Id = NetworkingNode_Id.Parse("GD-NN001");

                localController1 = new TestLocalController(
                                      Id:                       networkingNode1Id,
                                      VendorName:               "GraphDefined OEM #1",
                                      Model:                    "VCP.1",
                                      Description:              I18NString.Create(Languages.en, "Our first virtual networking node!"),
                                      SerialNumber:             "SN-NN0001",
                                      FirmwareVersion:          "v0.1",
                                      Modem:                    new Modem(
                                                                    ICCID:   "0001",
                                                                    IMSI:    "1112"
                                                                ),
                                      DisableSendHeartbeats:    true,

                                      //HTTPBasicAuth:            new Tuple<String, String>("GDNN001", "1234"),
                                      DNSClient:                testCSMS01!.DNSClient
                                  );

                ClassicAssert.IsNotNull(localController1);


                lcOCPPWebSocketServer01 = localController1.AttachWebSocketServer(
                                              TCPPort:                 IPPort.Parse(9103),
                                              DisableWebSocketPings:   true,
                                              AutoStart:               true
                                          );

                ClassicAssert.IsNotNull(lcOCPPWebSocketServer01);


                if (testBackendWebSockets01 is not null)
                {

                    testCSMS01.AddOrUpdateHTTPBasicAuth(networkingNode1Id, "1234abcd");

                    var response = localController1.ConnectWebSocketClient(
                                       NetworkingNodeId:        NetworkingNode_Id.CSMS,
                                       RemoteURL:               URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + localController1.Id),
                                       HTTPAuthentication:      HTTPBasicAuthentication.Create(networkingNode1Id.ToString(), "1234abcd"),
                                       DisableWebSocketPings:   true,
                                       NetworkingMode:          OCPP.WebSockets.NetworkingMode.OverlayNetwork
                                   ).Result;

                    ClassicAssert.IsNotNull(response);

                    if (response is not null)
                    {

                        // HTTP/1.1 101 Switching Protocols
                        // Date:                    Mon, 02 Apr 2023 15:55:18 GMT
                        // Server:                  GraphDefined OCPP v2.0.1 HTTP/WebSocket/JSON CSMS API
                        // Connection:              Upgrade
                        // Upgrade:                 websocket
                        // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
                        // Sec-WebSocket-Protocol:  ocpp2.0.1
                        // Sec-WebSocket-Version:   13

                        ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                    response.HTTPStatusCode);
                        ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API",   response.Server);
                        ClassicAssert.AreEqual("Upgrade",                                                            response.Connection);
                        ClassicAssert.AreEqual("websocket",                                                          response.Upgrade);
                        ClassicAssert.IsTrue  (response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
                        ClassicAssert.AreEqual("13",                                                                 response.SecWebSocketVersion);
                    }


                    //var networkingNode1WebSocketClient = networkingNode1.CSClient as NetworkingNodeWSClient;
                    //ClassicAssert.IsNotNull(networkingNode1WebSocketClient);

                    //if (networkingNode1WebSocketClient is not null)
                    //{

                    //    networkingNode1WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                    //        networkingNode1WebSocketJSONMessagesReceived.        Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
                    //    };

                    //    networkingNode1WebSocketClient.OnJSONMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                    //        networkingNode1WebSocketJSONMessageResponsesSent.    Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                    //    };

                    //    networkingNode1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                    //        networkingNode1WebSocketJSONMessagesSent.            Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
                    //    };

                    //    networkingNode1WebSocketClient.OnJSONMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                    //        networkingNode1WebSocketJSONMessageResponsesReceived.Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                    //    };

                    //}

                }

            }

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override void ShutdownEachTest()
        {

            base.ShutdownEachTest();

            localController1 = null;
            localController2 = null;
            localController3 = null;

        }

        #endregion


    }

}
