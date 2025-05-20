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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.tests.LocalController;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// Charging Station test defaults.
    /// </summary>
    public abstract class AChargingStationTests : ALocalControllerTests
    {

        #region Charging Station #1 Data

        protected TestChargingStationNode?      chargingStation1;

        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation1WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation1WebSocketBinarySendMessagesReceived;

        #endregion

        #region Charging Station #1 Data

        protected TestChargingStationNode?      chargingStation2;

        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation2WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation2WebSocketBinarySendMessagesReceived;

        #endregion

        #region Charging Station #3 Data

        protected TestChargingStationNode?      chargingStation3;

        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         chargingStation3WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       chargingStation3WebSocketBinarySendMessagesReceived;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override async Task SetupEachTest()
        {

            await base.SetupEachTest();

            #region Initial checks

            if (testCSMS1              is null ||
                testBackendWebSockets1 is null)
            {
                Assert.Fail($"{nameof(AChargingStationTests)} preconditions failed!");
                return;
            }

            if (InitLocalController1 &&
               (localController1       is null ||
                lcOCPPWebSocketServer1 is null))
            {
                Assert.Fail($"{nameof(AChargingStationTests)} preconditions failed!");
                return;
            }

            #endregion

            #region Charging Station #1

            chargingStation1WebSocketJSONRequestsSent                = [];
            chargingStation1WebSocketJSONResponsesSent               = [];
            chargingStation1WebSocketJSONRequestErrorsSent           = [];
            chargingStation1WebSocketJSONResponseErrorsSent          = [];
            chargingStation1WebSocketJSONSendMessagesSent            = [];

            chargingStation1WebSocketJSONRequestsReceived            = [];
            chargingStation1WebSocketJSONResponsesReceived           = [];
            chargingStation1WebSocketJSONRequestErrorsReceived       = [];
            chargingStation1WebSocketJSONResponseErrorsReceived      = [];
            chargingStation1WebSocketJSONSendMessagesReceived        = [];

            chargingStation1WebSocketBinaryRequestsSent              = [];
            chargingStation1WebSocketBinaryResponsesSent             = [];
            chargingStation1WebSocketBinaryRequestErrorsSent         = [];
            chargingStation1WebSocketBinaryResponseErrorsSent        = [];
            chargingStation1WebSocketBinarySendMessagesSent          = [];

            chargingStation1WebSocketBinaryRequestsReceived          = [];
            chargingStation1WebSocketBinaryResponsesReceived         = [];
            chargingStation1WebSocketBinaryRequestErrorsReceived     = [];
            chargingStation1WebSocketBinaryResponseErrorsReceived    = [];
            chargingStation1WebSocketBinarySendMessagesReceived      = [];


            chargingStation1 = new TestChargingStationNode(

                                   Id:                      NetworkingNode_Id.Parse("GD-CP001"),
                                   VendorName:              "GraphDefined OEM #1",
                                   Model:                   "VCP.1",
                                   Description:             I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                   SerialNumber:            "SN-CS0001",
                                   FirmwareVersion:         "v0.1",

                                   EVSEs:                   [
                                                                new EVSESpec(
                                                                    AdminStatus:         OperationalStatus.Operative,
                                                                    ConnectorTypes:      [ ConnectorType.cType2 ],
                                                                    MeterType:           "MT1",
                                                                    MeterSerialNumber:   "MSN1",
                                                                    MeterPublicKey:      "MPK1"
                                                                )
                                                            ],
                                   Modem:                   new Modem(
                                                                ICCID:   "0000",
                                                                IMSI:    "1111"
                                                            ),
                                   UplinkEnergyMeter:       new Energy_Meter(
                                                                Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                Model:          "Virtual Energy Meter",
                                                                SerialNumber:   "SN-EN0001"
                                                                //PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                            ),
                                   DisableSendHeartbeats:   true,

                                   //HTTPBasicAuth:           new Tuple<String, String>("OLI_001", "1234"),
                                   //HTTPBasicAuth:           new Tuple<String, String>("GD001", "1234"),
                                   DNSClient:               testCSMS1.DNSClient

                               );

            Assert.That(chargingStation1,  Is.Not.Null);

            #region Add "legal" Network Time Client Controllers

            var ptb1 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "ptb1",
                               URL.Parse("udp://ptbtime1.ptb.de"),
                               Description:   I18NString.Create("ptb 1")
                           )
                       );

            var ptb2 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "ptb2",
                               URL.Parse("udp://ptbtime2.ptb.de"),
                               Description:   I18NString.Create("ptb 2")
                           )
                       );

            var ptb3 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "ptb3",
                               URL.Parse("udp://ptbtime3.ptb.de"),
                               Description:   I18NString.Create("ptb 3")
                           )
                       );

            var ptb4 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "ptb4",
                               URL.Parse("udp://ptbtime4.ptb.de"),
                               Description:   I18NString.Create("ptb 4")
                           )
                       );

            chargingStation1.AddComponent(
                new NTPClientGroupController(
                    "legal",
                    [ ptb1, ptb2, ptb3 ]
                )
            );

            #endregion

            #region Add "local" Network Time Client Controllers

            var lc1 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "lc1",
                               URL.Parse("udp://lc1.local"),
                               Description:   I18NString.Create("local 1")
                           )
                       );

            var lc2 = chargingStation1.AddComponentX(
                           new NTPClientController(
                               "lc2",
                               URL.Parse("udp://lc2.local"),
                               Description:   I18NString.Create("local 2")
                           )
                       );

            chargingStation1.AddComponent(
                new NTPClientGroupController(
                    "local",
                    [ lc1, lc2 ]
                )
            );

            #endregion


            if (testBackendWebSockets1 is not null ||
                lcOCPPWebSocketServer1 is not null)
            {

                testCSMS1.              AddOrUpdateHTTPBasicAuth(chargingStation1.Id, "1234abcd");
                lcOCPPWebSocketServer1?.AddOrUpdateHTTPBasicAuth(chargingStation1.Id, "1234abcd");

                var response = chargingStation1.ConnectOCPPWebSocketClient(
                                   NextHopNetworkingNodeId:   NetworkingNode_Id.CSMS,
                                   RemoteURL:                 URL.Parse("http://127.0.0.1:" + (lcOCPPWebSocketServer1?.IPPort ?? testBackendWebSockets1?.IPPort).ToString() + "/" + chargingStation1.Id),
                                   HTTPAuthentication:        HTTPBasicAuthentication.Create(chargingStation1.Id.ToString(), "1234abcd"),
                                   DisableWebSocketPings:     true
                               ).GetAwaiter().GetResult();

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

                    if (lcOCPPWebSocketServer1 is not null)
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API"));
                    else
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} WebSocket Server"));

                    Assert.That(response.Connection,                                                      Is.EqualTo(ConnectionType.Upgrade));
                    Assert.That(response.Upgrade,                                                         Is.EqualTo("websocket"));
                    Assert.That(response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId),   Is.True);
                    Assert.That(response.SecWebSocketVersion,                                             Is.EqualTo("13"));

                }


                var chargingStation1WebSocketClient = chargingStation1.OCPPWebSocketClients.First();
                Assert.That(chargingStation1WebSocketClient,  Is.Not.Null);

                chargingStation1WebSocketClient.OnJSONMessageSent     += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          message,
                                                                          sentStatus,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation1WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation1WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation1WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation1WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation1WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };


                chargingStation1WebSocketClient.OnJSONMessageReceived += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          sourceNodeId,
                                                                          message,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation1WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation1WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation1WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation1WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation1WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };

            }

            #endregion

            #region Charging Station #2

            chargingStation2WebSocketJSONRequestsSent                = [];
            chargingStation2WebSocketJSONResponsesSent               = [];
            chargingStation2WebSocketJSONRequestErrorsSent           = [];
            chargingStation2WebSocketJSONResponseErrorsSent          = [];
            chargingStation2WebSocketJSONSendMessagesSent            = [];

            chargingStation2WebSocketJSONRequestsReceived            = [];
            chargingStation2WebSocketJSONResponsesReceived           = [];
            chargingStation2WebSocketJSONRequestErrorsReceived       = [];
            chargingStation2WebSocketJSONResponseErrorsReceived      = [];
            chargingStation2WebSocketJSONSendMessagesReceived        = [];

            chargingStation2WebSocketBinaryRequestsSent              = [];
            chargingStation2WebSocketBinaryResponsesSent             = [];
            chargingStation2WebSocketBinaryRequestErrorsSent         = [];
            chargingStation2WebSocketBinaryResponseErrorsSent        = [];
            chargingStation2WebSocketBinarySendMessagesSent          = [];

            chargingStation2WebSocketBinaryRequestsReceived          = [];
            chargingStation2WebSocketBinaryResponsesReceived         = [];
            chargingStation2WebSocketBinaryRequestErrorsReceived     = [];
            chargingStation2WebSocketBinaryResponseErrorsReceived    = [];
            chargingStation2WebSocketBinarySendMessagesReceived      = [];


            chargingStation2 = new TestChargingStationNode(

                                   Id:                      NetworkingNode_Id.Parse("GD-CP002"),
                                   VendorName:              "GraphDefined OEM #2",
                                   Model:                   "VCP.2",
                                   Description:             I18NString.Create(Languages.en, "Our 2nd virtual charging station!"),
                                   SerialNumber:            "SN-CS0002",
                                   FirmwareVersion:         "v0.2",
                                   EVSEs:                   [
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
                                   Modem:                   new Modem(
                                                                ICCID:   "3333",
                                                                IMSI:    "4444"
                                                            ),
                                   UplinkEnergyMeter:       new Energy_Meter(
                                                                Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                Model:          "Virtual Energy Meter",
                                                                SerialNumber:   "SN-EN0001"
                                                             //   PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                            ),
                                   DisableSendHeartbeats:   true,

                                   DNSClient:               testCSMS1.DNSClient

                               );

            Assert.That(chargingStation2,  Is.Not.Null);

            if (testBackendWebSockets1 is not null)
            {

                testCSMS1.AddOrUpdateHTTPBasicAuth(chargingStation2.Id, "1234abcd");

                var response = chargingStation2.ConnectOCPPWebSocketClient(
                                   NextHopNetworkingNodeId:   NetworkingNode_Id.CSMS,
                                   RemoteURL:                 URL.Parse("http://127.0.0.1:" + testBackendWebSockets1.IPPort.ToString() + "/" + chargingStation2.Id),
                                   HTTPAuthentication:        HTTPBasicAuthentication.Create(chargingStation2.Id.ToString(), "1234abcd"),
                                   DisableWebSocketPings:     true
                               ).GetAwaiter().GetResult();

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

                    if (lcOCPPWebSocketServer1 is not null)
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API"));
                    else
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} WebSocket Server"));

                    Assert.That(response.Connection,                                                      Is.EqualTo(ConnectionType.Upgrade));
                    Assert.That(response.Upgrade,                                                         Is.EqualTo("websocket"));
                    Assert.That(response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId),   Is.True);
                    Assert.That(response.SecWebSocketVersion,                                             Is.EqualTo("13"));

                }


                var chargingStation2WebSocketClient = chargingStation1.OCPPWebSocketClients.First();
                Assert.That(chargingStation2WebSocketClient,  Is.Not.Null);

                chargingStation2WebSocketClient.OnJSONMessageSent     += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          message,
                                                                          sentStatus,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation2WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation2WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation2WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation2WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation2WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };


                chargingStation2WebSocketClient.OnJSONMessageReceived += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          sourceNodeId,
                                                                          message,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation2WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation2WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation2WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation2WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation2WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };

            }

            #endregion

            #region Charging Station #3

            chargingStation3WebSocketJSONRequestsSent = [];
            chargingStation3WebSocketJSONResponsesSent               = [];
            chargingStation3WebSocketJSONRequestErrorsSent           = [];
            chargingStation3WebSocketJSONResponseErrorsSent          = [];
            chargingStation3WebSocketJSONSendMessagesSent            = [];

            chargingStation3WebSocketJSONRequestsReceived            = [];
            chargingStation3WebSocketJSONResponsesReceived           = [];
            chargingStation3WebSocketJSONRequestErrorsReceived       = [];
            chargingStation3WebSocketJSONResponseErrorsReceived      = [];
            chargingStation3WebSocketJSONSendMessagesReceived        = [];

            chargingStation3WebSocketBinaryRequestsSent              = [];
            chargingStation3WebSocketBinaryResponsesSent             = [];
            chargingStation3WebSocketBinaryRequestErrorsSent         = [];
            chargingStation3WebSocketBinaryResponseErrorsSent        = [];
            chargingStation3WebSocketBinarySendMessagesSent          = [];

            chargingStation3WebSocketBinaryRequestsReceived          = [];
            chargingStation3WebSocketBinaryResponsesReceived         = [];
            chargingStation3WebSocketBinaryRequestErrorsReceived     = [];
            chargingStation3WebSocketBinaryResponseErrorsReceived    = [];
            chargingStation3WebSocketBinarySendMessagesReceived      = [];


            chargingStation3 = new TestChargingStationNode(

                                   Id:                      NetworkingNode_Id.Parse("GD-CP003"),
                                   VendorName:              "GraphDefined OEM #3",
                                   Model:                   "VCP.3",
                                   Description:             I18NString.Create(Languages.en, "Our 3rd virtual charging station!"),
                                   SerialNumber:            "SN-CS0003",
                                   FirmwareVersion:         "v0.3",
                                   EVSEs:                   [
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
                                   Modem:                   new Modem(
                                                                ICCID:   "5555",
                                                                IMSI:    "6666"
                                                            ),
                                   UplinkEnergyMeter:       new Energy_Meter(
                                                                Id:             EnergyMeter_Id.Parse("SN-EN0001"),
                                                                Model:          "Virtual Energy Meter",
                                                                SerialNumber:   "SN-EN0001"
                                                            //    PublicKeys:     [ ECCPublicKey.ParseASN1("0xcafebabe") ]
                                                            ),
                                   DisableSendHeartbeats:   true,

                                   DNSClient:               testCSMS1.DNSClient

                               );

            Assert.That(chargingStation3,  Is.Not.Null);

            if (testBackendWebSockets1 is not null ||
                lcOCPPWebSocketServer1 is not null)
            {

                testCSMS1.              AddOrUpdateHTTPBasicAuth(chargingStation3.Id, "1234abcd");
                lcOCPPWebSocketServer1?.AddOrUpdateHTTPBasicAuth(chargingStation3.Id, "1234abcd");

                var response = await chargingStation3.ConnectOCPPWebSocketClient(
                                         NextHopNetworkingNodeId:   NetworkingNode_Id.CSMS,
                                         RemoteURL:                 URL.Parse("http://127.0.0.1:" + (lcOCPPWebSocketServer1?.IPPort ?? testBackendWebSockets1?.IPPort).ToString() + "/" + chargingStation3.Id),
                                         HTTPAuthentication:        HTTPBasicAuthentication.Create(chargingStation3.Id.ToString(), "1234abcd"),
                                         DisableWebSocketPings:     true
                                     );

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

                    if (lcOCPPWebSocketServer1 is not null)
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} Networking Node HTTP/WebSocket/JSON API"));
                    else
                        Assert.That(response.Server,                                                      Is.EqualTo($"GraphDefined OCPP {Version.String} WebSocket Server"));

                    Assert.That(response.Connection,                                                      Is.EqualTo(ConnectionType.Upgrade));
                    Assert.That(response.Upgrade,                                                         Is.EqualTo("websocket"));
                    Assert.That(response.SecWebSocketProtocol.Contains(Version.WebSocketSubProtocolId),   Is.True);
                    Assert.That(response.SecWebSocketVersion,                                             Is.EqualTo("13"));

                }


                var chargingStation3WebSocketClient = chargingStation3.OCPPWebSocketClients.First();
                Assert.That(chargingStation3WebSocketClient,  Is.Not.Null);


                chargingStation3WebSocketClient.OnJSONMessageSent     += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          message,
                                                                          sentStatus,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation3WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation3WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation3WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation3WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation3WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };


                chargingStation3WebSocketClient.OnJSONMessageReceived += (timestamp,
                                                                          client,
                                                                          connection,
                                                                          eventTrackingId,
                                                                          messageTimestamp,
                                                                          sourceNodeId,
                                                                          message,
                                                                          ct) => {

                    switch (message[0].Value<Byte>())
                    {

                        case 2: // JSON Request Message
                            chargingStation3WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 3: // JSON Response Message
                            chargingStation3WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 4: // JSON Request Error Message
                            chargingStation3WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 5: // JSON Response Error Message
                            chargingStation3WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                        case 6: // JSON Send Message
                            chargingStation3WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                            break;

                    }

                    return Task.CompletedTask;

                };

            }

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            await base.ShutdownEachTest();

            chargingStation1 = null;
            chargingStation2 = null;
            chargingStation3 = null;

        }

        #endregion


    }

}
