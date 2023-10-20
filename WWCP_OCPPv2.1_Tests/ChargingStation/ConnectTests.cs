/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class ConnectTests
    {

        #region Data

        protected TestCSMS?                         testCSMS01;
        protected CSMSWSServer?                     testBackendWebSockets01;

        protected ConcurrentList<LogData1>?         csmsWebSocketTextMessagesReceived;
        protected ConcurrentList<LogData2>?         csmsWebSocketTextMessageResponsesSent;
        protected ConcurrentList<LogData1>?         csmsWebSocketTextMessagesSent;
        protected ConcurrentList<LogData2>?         csmsWebSocketTextMessageResponsesReceived;


        protected TestChargingStation?              chargingStation1;
        protected TestChargingStation?              chargingStation2;
        protected TestChargingStation?              chargingStation3;

        protected ConcurrentList<LogData1>?         chargingStation1WebSocketTextMessagesReceived;
        protected ConcurrentList<LogData1>?         chargingStation2WebSocketTextMessagesReceived;
        protected ConcurrentList<LogData1>?         chargingStation3WebSocketTextMessagesReceived;

        protected ConcurrentList<LogData2>?         chargingStation1WebSocketTextMessageResponsesReceived;
        protected ConcurrentList<LogData2>?         chargingStation2WebSocketTextMessageResponsesReceived;
        protected ConcurrentList<LogData2>?         chargingStation3WebSocketTextMessageResponsesReceived;

        protected ConcurrentList<LogData2>?         chargingStation1WebSocketTextMessageResponsesSent;
        protected ConcurrentList<LogData2>?         chargingStation2WebSocketTextMessageResponsesSent;
        protected ConcurrentList<LogData2>?         chargingStation3WebSocketTextMessageResponsesSent;

        protected ConcurrentList<LogData1>?         chargingStation1WebSocketTextMessagesSent;
        protected ConcurrentList<LogData1>?         chargingStation2WebSocketTextMessagesSent;
        protected ConcurrentList<LogData1>?         chargingStation3WebSocketTextMessagesSent;

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
                                  Id:                      CSMS_Id.Parse("OCPPTest01"),
                                  RequireAuthentication:   RequireAuthentication,
                                  HTTPUploadPort:          IPPort.Parse(9100),
                                  DNSClient:               new DNSClient(
                                                               SearchForIPv6DNSServers: false,
                                                               SearchForIPv4DNSServers: false
                                                           )
                              );

            Assert.IsNotNull(testCSMS01);

            testBackendWebSockets01  = testCSMS01.CreateWebSocketService(
                                           TCPPort:                 IPPort.Parse(9101),
                                           DisableWebSocketPings:   DisableWebSocketPings,
                                           AutoStart:               true
                                       );

            Assert.IsNotNull(testBackendWebSockets01);


            csmsWebSocketTextMessagesReceived          = new ConcurrentList<LogData1>();
            csmsWebSocketTextMessageResponsesSent      = new ConcurrentList<LogData2>();
            csmsWebSocketTextMessagesSent              = new ConcurrentList<LogData1>();
            csmsWebSocketTextMessageResponsesReceived  = new ConcurrentList<LogData2>();


            testBackendWebSockets01.OnServerStarted               += (timestamp, server, eventTrackingId) => {
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

            testBackendWebSockets01.OnNewWebSocketConnection      += (timestamp, server, connection, eventTrackingId, cancellationToken) => {
                return Task.CompletedTask;
            };

            // OnHTTPResponse





            testBackendWebSockets01.OnTextMessageReceived         += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage) => {
                csmsWebSocketTextMessagesReceived.        TryAdd(new LogData1(timestamp, requestMessage));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnTextMessageResponseSent     += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                csmsWebSocketTextMessageResponsesSent.    TryAdd(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage ?? "-"));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnTextMessageSent             += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage) => {
                csmsWebSocketTextMessagesSent.            TryAdd(new LogData1(timestamp, requestMessage));
                return Task.CompletedTask;
            };

            testBackendWebSockets01.OnTextMessageResponseReceived += (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                csmsWebSocketTextMessageResponsesReceived.TryAdd(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage ?? "-"));
                return Task.CompletedTask;
            };



            #region Charging Station #1

            chargingStation1WebSocketTextMessagesReceived          = new ConcurrentList<LogData1>();
            chargingStation1WebSocketTextMessageResponsesSent      = new ConcurrentList<LogData2>();
            chargingStation1WebSocketTextMessagesSent              = new ConcurrentList<LogData1>();
            chargingStation1WebSocketTextMessageResponsesReceived  = new ConcurrentList<LogData2>();

            chargingStation1 = new TestChargingStation(
                                    Id:              ChargingStation_Id.Parse("GD001"),
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
                                                                                                   Id:    Connector_Id.Parse(1)
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

            Assert.IsNotNull(chargingStation1);


            if (testBackendWebSockets01 is not null)
            {

                testCSMS01.AddOrUpdateHTTPBasicAuth(ChargingStation_Id.Parse("test01"), "1234abcd");

                var response1 = chargingStation1.ConnectWebSocket(
                                    From:                    "From:GD001",
                                    To:                      "To:OCPPTest01",
                                    RemoteURL:               URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.Id),
                                    HTTPAuthentication:      HTTPBasicAuthentication.Create("test01", "1234abcd"),
                                    DisableWebSocketPings:   true
                                ).Result;

                Assert.IsNotNull(response1);

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

                    Assert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                    response1.HTTPStatusCode);
                    Assert.AreEqual($"GraphDefined OCPP {Version.String} HTTP/WebSocket/JSON CSMS API",   response1.Server);
                    Assert.AreEqual("Upgrade",                                                            response1.Connection);
                    Assert.AreEqual("websocket",                                                          response1.Upgrade);
                    Assert.IsTrue  (response1.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
                    Assert.AreEqual("13",                                                                 response1.SecWebSocketVersion);

                }


                var chargingStation1WebSocketClient = chargingStation1.CSClient as ChargingStationWSClient;
                Assert.IsNotNull(chargingStation1WebSocketClient);

                if (chargingStation1WebSocketClient is not null)
                {

                    chargingStation1WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message) => {
                        chargingStation1WebSocketTextMessagesReceived.        TryAdd(new LogData1(timestamp, message));
                    };

                    chargingStation1WebSocketClient.OnTextMessageResponseSent     += async (timestamp, client, webSocketFrame, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketTextMessageResponsesSent.    TryAdd(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage));
                    };

                    chargingStation1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message) => {
                        chargingStation1WebSocketTextMessagesSent.            TryAdd(new LogData1(timestamp, message));
                    };

                    chargingStation1WebSocketClient.OnTextMessageResponseReceived += async (timestamp, client, webSocketFrame, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketTextMessageResponsesReceived.TryAdd(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage));
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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
                Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
                Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

            }

        }

        #endregion

    }

}
