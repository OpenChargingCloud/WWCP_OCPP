/*
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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_Connect_Tests
    {

        #region Data

        protected TestCSMS?                         testCSMS01;
        protected CSMSWSServer?                     testBackendWebSockets01;

        protected ConcurrentList<LogJSONRequest>?         csmsWebSocketTextMessagesReceived;
        protected ConcurrentList<LogDataJSONResponse>?         csmsWebSocketTextMessageResponsesSent;
        protected ConcurrentList<LogJSONRequest>?         csmsWebSocketTextMessagesSent;
        protected ConcurrentList<LogDataJSONResponse>?         csmsWebSocketTextMessageResponsesReceived;


        protected TestChargingStation?              chargingStation1;
        protected TestChargingStation?              chargingStation2;
        protected TestChargingStation?              chargingStation3;

        protected ConcurrentList<LogJSONRequest>?         chargingStation1WebSocketJSONMessagesReceived;
        protected ConcurrentList<LogJSONRequest>?         chargingStation2WebSocketJSONMessagesReceived;
        protected ConcurrentList<LogJSONRequest>?         chargingStation3WebSocketJSONMessagesReceived;

        protected ConcurrentList<LogDataJSONResponse>?         chargingStation1WebSocketJSONMessageResponsesReceived;
        protected ConcurrentList<LogDataJSONResponse>?         chargingStation2WebSocketJSONMessageResponsesReceived;
        protected ConcurrentList<LogDataJSONResponse>?         chargingStation3WebSocketJSONMessageResponsesReceived;

        protected ConcurrentList<LogDataJSONResponse>?         chargingStation1WebSocketJSONMessageResponsesSent;
        protected ConcurrentList<LogDataJSONResponse>?         chargingStation2WebSocketJSONMessageResponsesSent;
        protected ConcurrentList<LogDataJSONResponse>?         chargingStation3WebSocketJSONMessageResponsesSent;

        protected ConcurrentList<LogJSONRequest>?         chargingStation1WebSocketJSONMessagesSent;
        protected ConcurrentList<LogJSONRequest>?         chargingStation2WebSocketJSONMessagesSent;
        protected ConcurrentList<LogJSONRequest>?         chargingStation3WebSocketJSONMessagesSent;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public virtual void SetupEachTest()
        {


        }

        #endregion


        #region SetupCSMS()

        public virtual void SetupCSMS(Boolean  RequireAuthentication   = true,
                                      Boolean  DisableWebSocketPings   = true)
        {

            Timestamp.Reset();

            testCSMS01      = new TestCSMS(
                                  Id:                      NetworkingNode_Id.Parse("OCPPTest01"),
                                  RequireAuthentication:   RequireAuthentication,
                                  HTTPUploadPort:          IPPort.Parse(9100),
                                  DNSClient:               new DNSClient(
                                                               SearchForIPv6DNSServers: false,
                                                               SearchForIPv4DNSServers: false
                                                           )
                              );

            ClassicAssert.IsNotNull(testCSMS01);

            testBackendWebSockets01  = testCSMS01.AttachWebSocketService(
                                           TCPPort:                 IPPort.Parse(9101),
                                           DisableWebSocketPings:   DisableWebSocketPings,
                                           AutoStart:               true
                                       );

            ClassicAssert.IsNotNull(testBackendWebSockets01);


            csmsWebSocketTextMessagesReceived          = new ConcurrentList<LogJSONRequest>();
            csmsWebSocketTextMessageResponsesSent      = new ConcurrentList<LogDataJSONResponse>();
            csmsWebSocketTextMessagesSent              = new ConcurrentList<LogJSONRequest>();
            csmsWebSocketTextMessageResponsesReceived  = new ConcurrentList<LogDataJSONResponse>();


            testBackendWebSockets01.OnServerStarted               += (timestamp, server, eventTrackingId, cancellationToken) => {
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnValidateTCPConnection       += (timestamp, server, connection, eventTrackingId, cancellationToken) => {
                return Task.FromResult(ConnectionFilterResponse.Accepted());
            };

            testBackendWebSockets01.OnNewTCPConnection            += (timestamp, server, connection, eventTrackingId, cancellationToken) => {
                return Task.CompletedTask;
            };

            // OnHTTPRequest

            // OnValidateWebSocketConnection

            testBackendWebSockets01.OnNewWebSocketConnection      += (timestamp, server, newConnection, networkingNodeId, sharedSubprotocols, eventTrackingId, cancellationToken) => {
                return Task.CompletedTask;
            };

            // OnHTTPResponse





            testBackendWebSockets01.OnTextMessageReceived         += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
                csmsWebSocketTextMessagesReceived.        TryAdd(new LogJSONRequest(timestamp, JArray.Parse(requestMessage)));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnJSONMessageResponseSent     += (timestamp, webSocketServer, webSocketConnection, networkingNodeId, networkPath, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage, cancellationToken) => {
                csmsWebSocketTextMessageResponsesSent.    TryAdd(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage ?? []));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnTextMessageSent             += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
                csmsWebSocketTextMessagesSent.            TryAdd(new LogJSONRequest(timestamp, JArray.Parse(requestMessage)));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnJSONMessageResponseReceived += (timestamp, webSocketServer, webSocketConnection, networkingNodeId, networkPath, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage, cancellationToken) => {
                csmsWebSocketTextMessageResponsesReceived.TryAdd(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage ?? []));
                return Task.CompletedTask;
            };



            #region Charging Station #1

            chargingStation1WebSocketJSONMessagesReceived          = new ConcurrentList<LogJSONRequest>();
            chargingStation1WebSocketJSONMessageResponsesSent      = new ConcurrentList<LogDataJSONResponse>();
            chargingStation1WebSocketJSONMessagesSent              = new ConcurrentList<LogJSONRequest>();
            chargingStation1WebSocketJSONMessageResponsesReceived  = new ConcurrentList<LogDataJSONResponse>();

            chargingStation1 = new TestChargingStation(
                                    Id:                       NetworkingNode_Id.Parse("GD001"),
                                    VendorName:               "GraphDefined OEM #1",
                                    Model:                    "VCP.1",
                                    Description:              I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                    SerialNumber:             "SN-CS0001",
                                    FirmwareVersion:          "v0.1",
                                    Modem:                    new Modem(
                                                                  ICCID:   "0000",
                                                                  IMSI:    "1111"
                                                              ),
                                    EVSEs:                    new[] {
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
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0001",
                                    MeterPublicKey:           "0xcafebabe",

                                    DisableSendHeartbeats:    true,

                                    //HTTPBasicAuth:            new Tuple<String, String>("OLI_001", "1234"),
                                    //HTTPBasicAuth:            new Tuple<String, String>("GD001", "1234"),
                                    DNSClient:                testCSMS01!.DNSClient
                                );

            ClassicAssert.IsNotNull(chargingStation1);


            if (testBackendWebSockets01 is not null)
            {

                testCSMS01.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("test01"), "1234abcd");

                var response1 = chargingStation1.ConnectWebSocket(
                                    RemoteURL:               URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.Id),
                                    HTTPAuthentication:      HTTPBasicAuthentication.Create("test01", "1234abcd"),
                                    DisableWebSocketPings:   true
                                ).Result;

                ClassicAssert.IsNotNull(response1);

                if (response1 is not null)
                {

                    // HTTP/1.1 101 Switching Protocols
                    // Date:                    Mon, 02 Apr 2023 15:55:18 GMT
                    // Server:                  GraphDefined OCPP v2.0.1 HTTP/WebSocket/JSON CSMS API
                    // Connection:              Upgrade
                    // Upgrade:                 websocket
                    // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
                    // Sec-WebSocket-Protocol:  ocpp2.0.1
                    // Sec-WebSocket-Version:   13

                    ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                    response1.HTTPStatusCode);
                    ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API",   response1.Server);
                    ClassicAssert.AreEqual("Upgrade",                                                            response1.Connection);
                    ClassicAssert.AreEqual("websocket",                                                          response1.Upgrade);
                    ClassicAssert.IsTrue  (response1.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
                    ClassicAssert.AreEqual("13",                                                                 response1.SecWebSocketVersion);

                }


                var chargingStation1WebSocketClient = chargingStation1.CSClient as ChargingStationWSClient;
                ClassicAssert.IsNotNull(chargingStation1WebSocketClient);

                if (chargingStation1WebSocketClient is not null)
                {

                    chargingStation1WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                        chargingStation1WebSocketJSONMessagesReceived.        TryAdd(new LogJSONRequest(timestamp, JArray.Parse(message)));
                    };

                    chargingStation1WebSocketClient.OnJSONMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketJSONMessageResponsesSent.    TryAdd(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                    };

                    chargingStation1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                        chargingStation1WebSocketJSONMessagesSent.            TryAdd(new LogJSONRequest(timestamp, JArray.Parse(message)));
                    };

                    chargingStation1WebSocketClient.OnJSONMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketJSONMessageResponsesReceived.TryAdd(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                    };

                }

            }

            #endregion

        }

        private Task TestBackendWebSockets01_OnNewTCPConnection()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

            testBackendWebSockets01?.Shutdown();

            testCSMS01               = null;
            testBackendWebSockets01  = null;

            chargingStation1         = null;
            chargingStation2         = null;
            chargingStation3         = null;

        }

        #endregion


        #region Init_Test()

        /// <summary>
        /// A test for creating charging stations.
        /// </summary>
        [Test]
        public void ChargingStation_Init_Test()
        {

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                ClassicAssert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
                ClassicAssert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
                ClassicAssert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

            }

        }

        #endregion


    }

}
