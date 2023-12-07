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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_0_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.tests
{

    /// <summary>
    /// Charging station test defaults.
    /// </summary>
    public abstract class AChargingStationTests : ACSMSTests
    {

        #region Data

        protected TestChargingStation?  chargingStation1;
        protected TestChargingStation?  chargingStation2;
        protected TestChargingStation?  chargingStation3;

        protected List<LogData1>?       chargingStation1WebSocketTextMessagesReceived;
        protected List<LogData1>?       chargingStation2WebSocketTextMessagesReceived;
        protected List<LogData1>?       chargingStation3WebSocketTextMessagesReceived;

        protected List<LogData2>?       chargingStation1WebSocketTextMessageResponsesReceived;
        protected List<LogData2>?       chargingStation2WebSocketTextMessageResponsesReceived;
        protected List<LogData2>?       chargingStation3WebSocketTextMessageResponsesReceived;

        protected List<LogData2>?       chargingStation1WebSocketTextMessageResponsesSent;
        protected List<LogData2>?       chargingStation2WebSocketTextMessageResponsesSent;
        protected List<LogData2>?       chargingStation3WebSocketTextMessageResponsesSent;

        protected List<LogData1>?       chargingStation1WebSocketTextMessagesSent;
        protected List<LogData1>?       chargingStation2WebSocketTextMessagesSent;
        protected List<LogData1>?       chargingStation3WebSocketTextMessagesSent;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            #region Charging Station #1

            chargingStation1WebSocketTextMessagesReceived          = new List<LogData1>();
            chargingStation1WebSocketTextMessageResponsesSent      = new List<LogData2>();
            chargingStation1WebSocketTextMessagesSent              = new List<LogData1>();
            chargingStation1WebSocketTextMessageResponsesReceived  = new List<LogData2>();

            chargingStation1 = new TestChargingStation(
                                    ChargeBoxId:              ChargeBox_Id.Parse("GD001"),
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
                                                                      Status:              OperationalStatus.Operative,
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

                testCSMS01.AddHTTPBasicAuth(ChargeBox_Id.Parse("test01"), "1234abcd");

                var response1 = chargingStation1.ConnectWebSocket(
                                                     From:                    "From:GD001",
                                                     To:                      "To:OCPPTest01",
                                                     RemoteURL:               URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.ChargeBoxId),
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
                    Assert.AreEqual($"GraphDefined OCPP {Version.Number} HTTP/WebSocket/JSON CSMS API",   response1.Server);
                    Assert.AreEqual("Upgrade",                                                            response1.Connection);
                    Assert.AreEqual("websocket",                                                          response1.Upgrade);
                    Assert.IsTrue  (response1.SecWebSocketProtocol.Contains($"ocpp{Version.Number[1..]}"));
                    Assert.AreEqual("13",                                                                 response1.SecWebSocketVersion);

                }


                var chargingStation1WebSocketClient = chargingStation1.CSClient as ChargingStationWSClient;
                Assert.IsNotNull(chargingStation1WebSocketClient);

                if (chargingStation1WebSocketClient is not null)
                {

                    chargingStation1WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                        chargingStation1WebSocketTextMessagesReceived.        Add(new LogData1(timestamp, message));
                    };

                    chargingStation1WebSocketClient.OnTextMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketTextMessageResponsesSent.    Add(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage));
                    };

                    chargingStation1WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                        chargingStation1WebSocketTextMessagesSent.            Add(new LogData1(timestamp, message));
                    };

                    chargingStation1WebSocketClient.OnTextMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                        chargingStation1WebSocketTextMessageResponsesReceived.Add(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage));
                    };

                }

            }

            #endregion

            #region Charging Station #2

            chargingStation2WebSocketTextMessagesReceived          = new List<LogData1>();
            chargingStation2WebSocketTextMessageResponsesSent      = new List<LogData2>();
            chargingStation2WebSocketTextMessagesSent              = new List<LogData1>();
            chargingStation2WebSocketTextMessageResponsesReceived  = new List<LogData2>();

            chargingStation2  = new TestChargingStation(
                                    ChargeBoxId:              ChargeBox_Id.Parse("CP002"),
                                    VendorName:               "GraphDefined OEM #2",
                                    Model:                    "VCP.2",
                                    Description:              I18NString.Create(Languages.en, "Our 2nd virtual charging station!"),
                                    SerialNumber:             "SN-CS0002",
                                    FirmwareVersion:          "v0.2",
                                    Modem:                    new Modem(
                                                                  ICCID:   "3333",
                                                                  IMSI:    "4444"
                                                              ),
                                    EVSEs:                    new[] {
                                                                  new ChargingStationEVSE(
                                                                      Id:                  EVSE_Id.Parse(1),
                                                                      Status:              OperationalStatus.Operative,
                                                                      MeterType:           "MT2",
                                                                      MeterSerialNumber:   "MSN2",
                                                                      MeterPublicKey:      "MPK2",
                                                                      Connectors:          new[] {
                                                                                               new ChargingStationConnector(
                                                                                                   Id:    Connector_Id.Parse(1)
                                                                                               )
                                                                                           }
                                                                  )
                                                              },
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0002",
                                    MeterPublicKey:           "0xbabecafe",

                                    DisableSendHeartbeats:    true,

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation2);

            #endregion

            #region Charging Station #3

            chargingStation3WebSocketTextMessagesReceived          = new List<LogData1>();
            chargingStation3WebSocketTextMessageResponsesSent      = new List<LogData2>();
            chargingStation3WebSocketTextMessagesSent              = new List<LogData1>();
            chargingStation3WebSocketTextMessageResponsesReceived  = new List<LogData2>();

            chargingStation3 = new TestChargingStation(
                                    ChargeBoxId:              ChargeBox_Id.Parse("CP003"),
                                    VendorName:               "GraphDefined OEM #3",
                                    Model:                    "VCP.3",
                                    Description:              I18NString.Create(Languages.en, "Our 3rd virtual charging station!"),
                                    SerialNumber:             "SN-CS0003",
                                    FirmwareVersion:          "v0.3",
                                    Modem:                    new Modem(
                                                                  ICCID:   "5555",
                                                                  IMSI:    "6666"
                                                              ),
                                    EVSEs:                    new[] {
                                                                  new ChargingStationEVSE(
                                                                      Id:                  EVSE_Id.Parse(1),
                                                                      Status:              OperationalStatus.Operative,
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3",
                                                                      MeterPublicKey:      "MPK3",
                                                                      Connectors:          new[] {
                                                                                               new ChargingStationConnector(
                                                                                                   Id:    Connector_Id.Parse(1)
                                                                                               )
                                                                                           }
                                                                  )
                                                              },
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0003",
                                    MeterPublicKey:           "0xbacafebe",

                                    DisableSendHeartbeats:    true,

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation3);

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override void ShutdownEachTest()
        {

            base.ShutdownEachTest();

            chargingStation1 = null;
            chargingStation2 = null;
            chargingStation3 = null;

        }

        #endregion


    }

}
