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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Charging station test defaults.
    /// </summary>
    public abstract class AChargingStationTests : ALocalControllerTests
    {

        #region Data

        protected TestChargingStationNode?      chargingStation1;
        protected TestChargingStationNode?      chargingStation2;
        protected TestChargingStationNode?      chargingStation3;


        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    chargingStation1WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    chargingStation1WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  chargingStation1WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  chargingStation1WebSocketBinaryMessageResponsesReceived;


        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    chargingStation2WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    chargingStation2WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  chargingStation2WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  chargingStation2WebSocketBinaryMessageResponsesReceived;


        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONMessagesReceived;
        protected List<LogDataJSONResponse>?    chargingStation3WebSocketJSONMessageResponsesSent;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONMessagesSent;
        protected List<LogDataJSONResponse>?    chargingStation3WebSocketJSONMessageResponsesReceived;

        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryMessagesReceived;
        protected List<LogDataBinaryResponse>?  chargingStation3WebSocketBinaryMessageResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryMessagesSent;
        protected List<LogDataBinaryResponse>?  chargingStation3WebSocketBinaryMessageResponsesReceived;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            #region Charging Station #1

            chargingStation1WebSocketJSONMessagesReceived          = [];
            chargingStation1WebSocketJSONMessageResponsesSent      = [];
            chargingStation1WebSocketJSONMessagesSent              = [];
            chargingStation1WebSocketJSONMessageResponsesReceived  = [];

            var chargingStation1Id = NetworkingNode_Id.Parse("GD-CP001");

            chargingStation1 = new TestChargingStationNode(
                                    Id:                       chargingStation1Id,
                                    VendorName:               "GraphDefined OEM #1",
                                    Model:                    "VCP.1",
                                    Description:              I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                    SerialNumber:             "SN-CS0001",
                                    FirmwareVersion:          "v0.1",

                                    EVSEs:                    [
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.cType2 ],
                                                                      MeterType:           "MT1",
                                                                      MeterSerialNumber:   "MSN1",
                                                                      MeterPublicKey:      "MPK1"
                                                                  )
                                                              ],
                                    Modem:                    new Modem(
                                                                  ICCID:   "0000",
                                                                  IMSI:    "1111"
                                                              ),
                                    UplinkEnergyMeter:        new Energy_Meter(
                                                                  Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                  Model:          "Virtual Energy Meter",
                                                                  SerialNumber:   "SN-EN0001"
                                                            //      PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                              ),
                                    DisableSendHeartbeats:    true,

                                    //HTTPBasicAuth:            new Tuple<String, String>("OLI_001", "1234"),
                                    //HTTPBasicAuth:            new Tuple<String, String>("GD001", "1234"),
                                    DNSClient:                testCSMS01!.DNSClient
                                );

            ClassicAssert.IsNotNull(chargingStation1);

            if (testBackendWebSockets01 is not null ||
                lcOCPPWebSocketServer01 is not null)
            {

                testCSMS01.              AddOrUpdateHTTPBasicAuth(chargingStation1Id, "1234abcd");
                lcOCPPWebSocketServer01?.AddOrUpdateHTTPBasicAuth(chargingStation1Id, "1234abcd");

                var response = chargingStation1.ConnectOCPPWebSocketClient(
                                   NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                   RemoteURL:                URL.Parse("http://127.0.0.1:" + (lcOCPPWebSocketServer01?.IPPort ?? testBackendWebSockets01?.IPPort).ToString() + "/" + chargingStation1.Id),
                                   HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation1Id.ToString(), "1234abcd"),
                                   DisableWebSocketPings:    true
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

                    ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,                                                   response.HTTPStatusCode);

                    if (lcOCPPWebSocketServer01 is not null)
                        ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API",   response.Server);
                    else
                        ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} WebSocket Server",                         response.Server);

                    ClassicAssert.AreEqual("Upgrade",                                                                           response.Connection);
                    ClassicAssert.AreEqual("websocket",                                                                         response.Upgrade);
                    ClassicAssert.IsTrue  (response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
                    ClassicAssert.AreEqual("13",                                                                                response.SecWebSocketVersion);

                }


                //var chargingStation1WebSocketClient = chargingStation1.CSClient as ChargingStationWSClient;
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

            }

            #endregion

            #region Charging Station #2

            chargingStation2WebSocketJSONMessagesReceived          = [];
            chargingStation2WebSocketJSONMessageResponsesSent      = [];
            chargingStation2WebSocketJSONMessagesSent              = [];
            chargingStation2WebSocketJSONMessageResponsesReceived  = [];

            var chargingStation2Id = NetworkingNode_Id.Parse("GD-CP002");

            chargingStation2  = new TestChargingStationNode(
                                    Id:                       chargingStation2Id,
                                    VendorName:               "GraphDefined OEM #2",
                                    Model:                    "VCP.2",
                                    Description:              I18NString.Create(Languages.en, "Our 2nd virtual charging station!"),
                                    SerialNumber:             "SN-CS0002",
                                    FirmwareVersion:          "v0.2",
                                    EVSEs:                    [
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.cType2 ],
                                                                      MeterType:           "MT2",
                                                                      MeterSerialNumber:   "MSN2.1",
                                                                      MeterPublicKey:      "MPK2.1"
                                                                  ),
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.cCCS2 ],
                                                                      MeterType:           "MT2",
                                                                      MeterSerialNumber:   "MSN2.2",
                                                                      MeterPublicKey:      "MPK2.1"
                                                                  )
                                                              ],
                                    Modem:                    new Modem(
                                                                  ICCID:   "3333",
                                                                  IMSI:    "4444"
                                                              ),
                                    UplinkEnergyMeter:        new Energy_Meter(
                                                                  Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                  Model:          "Virtual Energy Meter",
                                                                  SerialNumber:   "SN-EN0001"
                                                               //   PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                              ),
                                    DisableSendHeartbeats:    true,

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            ClassicAssert.IsNotNull(chargingStation2);

            if (testBackendWebSockets01 is not null)
            {

                testCSMS01.AddOrUpdateHTTPBasicAuth(chargingStation2Id, "1234abcd");

                var response = chargingStation2.ConnectOCPPWebSocketClient(
                                   NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                   RemoteURL:                URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation2.Id),
                                   HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation2Id.ToString(), "1234abcd"),
                                   DisableWebSocketPings:    true
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
                    ClassicAssert.AreEqual($"GraphDefined OCPP {Version.String} WebSocket Server",              response.Server);
                    ClassicAssert.AreEqual("Upgrade",                                                            response.Connection);
                    ClassicAssert.AreEqual("websocket",                                                          response.Upgrade);
                    ClassicAssert.IsTrue  (response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId));
                    ClassicAssert.AreEqual("13",                                                                 response.SecWebSocketVersion);

                }


                //var chargingStation2WebSocketClient = chargingStation2.CSClient as ChargingStationWSClient;
                //ClassicAssert.IsNotNull(chargingStation2WebSocketClient);

                //if (chargingStation2WebSocketClient is not null)
                //{

                //    chargingStation2WebSocketClient.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                //        chargingStation2WebSocketJSONMessagesReceived.        Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
                //    };

                //    chargingStation2WebSocketClient.OnJSONMessageResponseSent     += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                //        chargingStation2WebSocketJSONMessageResponsesSent.    Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                //    };

                //    chargingStation2WebSocketClient.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, webSocketFrame, eventTrackingId, message, cancellationToken) => {
                //        chargingStation2WebSocketJSONMessagesSent.            Add(new LogJSONRequest(timestamp, JArray.Parse(message)));
                //    };

                //    chargingStation2WebSocketClient.OnJSONMessageResponseReceived += async (timestamp, client, eventTrackingId, requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage) => {
                //        chargingStation2WebSocketJSONMessageResponsesReceived.Add(new LogDataJSONResponse(requestTimestamp, jsonRequestMessage, binaryRequestMessage, responseTimestamp, responseMessage));
                //    };

                //}

            }


            #endregion

            #region Charging Station #3

            chargingStation3WebSocketJSONMessagesReceived          = [];
            chargingStation3WebSocketJSONMessageResponsesSent      = [];
            chargingStation3WebSocketJSONMessagesSent              = [];
            chargingStation3WebSocketJSONMessageResponsesReceived  = [];

            var chargingStation3Id = NetworkingNode_Id.Parse("GD-CP003");

            chargingStation3 = new TestChargingStationNode(
                                    Id:                       chargingStation3Id,
                                    VendorName:               "GraphDefined OEM #3",
                                    Model:                    "VCP.3",
                                    Description:              I18NString.Create(Languages.en, "Our 3rd virtual charging station!"),
                                    SerialNumber:             "SN-CS0003",
                                    FirmwareVersion:          "v0.3",
                                    EVSEs:                    [
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.sType2 ],
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3.1",
                                                                      MeterPublicKey:      "MPK3.1"
                                                                  ),
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.sType2 ],
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3.2",
                                                                      MeterPublicKey:      "MPK3.2"
                                                                  ),
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.cCCS2 ],
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3.3",
                                                                      MeterPublicKey:      "MPK3.3"
                                                                  ),
                                                                  new EVSESpec(
                                                                      AdminStatus:         OperationalStatus.Operative,
                                                                      ConnectorTypes:      [ ConnectorType.cCCS2 ],
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3.4",
                                                                      MeterPublicKey:      "MPK3.4"
                                                                  )
                                                              ],
                                    Modem:                    new Modem(
                                                                  ICCID:   "5555",
                                                                  IMSI:    "6666"
                                                              ),
                                    UplinkEnergyMeter:        new Energy_Meter(
                                                                  Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                  Model:          "Virtual Energy Meter",
                                                                  SerialNumber:   "SN-EN0001"
                                                          //        PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                              ),
                                    DisableSendHeartbeats:    true,

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            ClassicAssert.IsNotNull(chargingStation3);

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
