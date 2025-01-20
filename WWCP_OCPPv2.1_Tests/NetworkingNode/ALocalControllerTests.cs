﻿/*
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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.LocalController;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.LocalController
{

    /// <summary>
    /// Local Controller test defaults.
    /// </summary>
    public abstract class ALocalControllerTests : ACSMSTests
    {

        #region LocalController #1 Data

        protected TestLocalControllerNode?      localController1;
        protected OCPPWebSocketServer?          lcOCPPWebSocketServer1;

        protected List<LogJSONRequest>?         lc1WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         lc1WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         lc1WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         lc1WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         lc1WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         lc1WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         lc1WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         lc1WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         lc1WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         lc1WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       lc1WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       lc1WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       lc1WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       lc1WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       lc1WebSocketBinarySendMessagesReceived;

        #endregion

        #region LocalController #2 Data

        protected TestLocalControllerNode?      localController2;
        protected OCPPWebSocketServer?          lcOCPPWebSocketServer2;

        protected List<LogJSONRequest>?         lc2WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         lc2WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         lc2WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         lc2WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         lc2WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         lc2WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         lc2WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         lc2WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         lc2WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         lc2WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       lc2WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       lc2WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       lc2WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       lc2WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       lc2WebSocketBinarySendMessagesReceived;

        #endregion

        #region Properties

        public Boolean  InitLocalController1    { get; set; } = false;
        public Boolean  InitLocalController2    { get; set; } = false;

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public override async Task SetupOnce()
        {

            await base.SetupOnce();

            #region Local Controller #1

            if (InitLocalController1)
            {

                lc1WebSocketJSONRequestsSent                = [];
                lc1WebSocketJSONResponsesSent               = [];
                lc1WebSocketJSONRequestErrorsSent           = [];
                lc1WebSocketJSONResponseErrorsSent          = [];
                lc1WebSocketJSONSendMessagesSent            = [];

                lc1WebSocketJSONRequestsReceived            = [];
                lc1WebSocketJSONResponsesReceived           = [];
                lc1WebSocketJSONRequestErrorsReceived       = [];
                lc1WebSocketJSONResponseErrorsReceived      = [];
                lc1WebSocketJSONSendMessagesReceived        = [];

                lc1WebSocketBinaryRequestsSent              = [];
                lc1WebSocketBinaryResponsesSent             = [];
                lc1WebSocketBinaryRequestErrorsSent         = [];
                lc1WebSocketBinaryResponseErrorsSent        = [];
                lc1WebSocketBinarySendMessagesSent          = [];

                lc1WebSocketBinaryRequestsReceived          = [];
                lc1WebSocketBinaryResponsesReceived         = [];
                lc1WebSocketBinaryRequestErrorsReceived     = [];
                lc1WebSocketBinaryResponseErrorsReceived    = [];
                lc1WebSocketBinarySendMessagesReceived      = [];


                localController1 = new TestLocalControllerNode(

                                       Id:                               NetworkingNode_Id.Parse("GD-NN001"),
                                       VendorName:                       "GraphDefined OEM #1",
                                       Model:                            "VCP.1",
                                       SerialNumber:                     "SN-NN0001",
                                       SoftwareVersion:                  "v0.1",
                                       Modem:                            new Modem(
                                                                             ICCID:   "0001",
                                                                             IMSI:    "1112"
                                                                         ),
                                       Description:                      I18NString.Create(Languages.en, "Our first virtual networking node!"),
                                       CustomData:                       null,

                                       SignaturePolicy:                  null,
                                       ForwardingSignaturePolicy:        null,

                                       HTTPAPI_Disabled:                 false,
                                       HTTPAPI_Port:                     null,
                                       HTTPAPI_ServerName:               null,
                                       HTTPAPI_ServiceName:              null,
                                       HTTPAPI_RobotEMailAddress:        null,
                                       HTTPAPI_RobotGPGPassphrase:       null,
                                       HTTPAPI_EventLoggingDisabled:     true,

                                       HTTPDownloadAPI:                  null,
                                       HTTPDownloadAPI_Disabled:         false,
                                       HTTPDownloadAPI_Path:             null,
                                       HTTPDownloadAPI_FileSystemPath:   null,

                                       HTTPUploadAPI:                    null,
                                       HTTPUploadAPI_Disabled:           false,
                                       HTTPUploadAPI_Path:               null,
                                       HTTPUploadAPI_FileSystemPath:     null,

                                       WebAPI:                           null,
                                       WebAPI_Disabled:                  false,
                                       WebAPI_Path:                      null,

                                       DefaultRequestTimeout:            null,

                                       DisableSendHeartbeats:            true,
                                       SendHeartbeatsEvery:              null,

                                       DisableMaintenanceTasks:          false,
                                       MaintenanceEvery:                 null,
                                       DNSClient:                        dnsClient

                                  );

                Assert.That(localController1,  Is.Not.Null);

                lcOCPPWebSocketServer1 = localController1.AttachWebSocketServer(

                                             HTTPServiceName:              null,
                                             IPAddress:                    null,
                                             TCPPort:                      IPPort.Parse(9103),
                                             Description:                  null,

                                             RequireAuthentication:        true,
                                             SecWebSocketProtocols:        null,
                                             DisableWebSocketPings:        true,
                                             WebSocketPingEvery:           null,
                                             SlowNetworkSimulationDelay:   null,

                                             ServerCertificateSelector:    null,
                                             ClientCertificateValidator:   null,
                                             LocalCertificateSelector:     null,
                                             AllowedTLSProtocols:          null,
                                             ClientCertificateRequired:    null,
                                             CheckCertificateRevocation:   null,

                                             ServerThreadNameCreator:      null,
                                             ServerThreadPrioritySetter:   null,
                                             ServerThreadIsBackground:     null,
                                             ConnectionIdBuilder:          null,
                                             ConnectionTimeout:            null,
                                             MaxClientConnections:         null,

                                             AutoStart:                    true

                                         );

                Assert.That(lcOCPPWebSocketServer1,  Is.Not.Null);


                if (testCSMS1              is not null &&
                    testBackendWebSockets1 is not null)
                {

                    testCSMS1.AddOrUpdateHTTPBasicAuth(localController1.Id, "1234abcd");

                    var response = localController1.ConnectOCPPWebSocketClient(
                                       NextHopNetworkingNodeId:   NetworkingNode_Id.CSMS,
                                       RemoteURL:                 URL.Parse("http://127.0.0.1:" + testBackendWebSockets1.IPPort.ToString() + "/" + localController1.Id),
                                       HTTPAuthentication:        HTTPBasicAuthentication.Create(localController1.Id.ToString(), "1234abcd"),
                                       DisableWebSocketPings:     true,
                                       NetworkingMode:            NetworkingMode.OverlayNetwork
                                   ).Result;

                    Assert.That(response,  Is.Not.Null);


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

                        Assert.That(response.HTTPStatusCode,                                                  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                        Assert.That(response.Server,                                                          Is.EqualTo($"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API"));
                        Assert.That(response.Connection,                                                      Is.EqualTo(ConnectionType.Upgrade));
                        Assert.That(response.Upgrade,                                                         Is.EqualTo("websocket"));
                        Assert.That(response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId),   Is.True);
                        Assert.That(response.SecWebSocketVersion,                                             Is.EqualTo("13"));

                    }


                    //var networkingNode1WebSocketClient = networkingNode1.CSClient as LocalControllerWSClient;
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

        #region SetupEachTest()

        [SetUp]
        public override async Task SetupEachTest()
        {
            await base.SetupEachTest();
        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            await base.ShutdownEachTest();

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public override async Task ShutdownOnce()
        {

            if (lcOCPPWebSocketServer1 is not null)
                await lcOCPPWebSocketServer1.Shutdown();

            if (lcOCPPWebSocketServer2 is not null)
                await lcOCPPWebSocketServer2.Shutdown();

            localController1         = null;
            localController2         = null;

            lcOCPPWebSocketServer1   = null;
            lcOCPPWebSocketServer2   = null;

            await base.ShutdownOnce();

        }

        #endregion


    }

}
