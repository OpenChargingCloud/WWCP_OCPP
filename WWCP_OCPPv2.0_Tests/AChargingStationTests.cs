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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{

    /// <summary>
    /// Charging station test defaults.
    /// </summary>
    public abstract class AChargingStationTests : ACSMSTests
    {

        #region Data

        protected TestChargingStation? chargingStation1;
        protected TestChargingStation? chargingStation2;
        protected TestChargingStation? chargingStation3;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            chargingStation1  = new TestChargingStation(
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
                                    EVSEs:                    new ChargingStationEVSE[] {
                                                                  new ChargingStationEVSE(
                                                                      Id:                  EVSE_Id.Parse(1),
                                                                      Status:              OperationalStatus.Operative,
                                                                      MeterType:           "MT1",
                                                                      MeterSerialNumber:   "MSN1",
                                                                      MeterPublicKey:      "MPK1",
                                                                      Connectors:          new ChargingStationConnector[] {
                                                                                               new ChargingStationConnector(
                                                                                                   Id:    Connector_Id.Parse(1)
                                                                                               )
                                                                                           }
                                                                  )
                                                              },
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0001",
                                    MeterPublicKey:           "0xcafebabe",

                                    //HTTPBasicAuth:            new Tuple<String, String>("OLI_001", "1234"),
                                    //HTTPBasicAuth:            new Tuple<String, String>("GD001", "1234"),
                                    DNSClient:                testCSMS01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation1);

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
                                    EVSEs:                    new ChargingStationEVSE[] {
                                                                  new ChargingStationEVSE(
                                                                      Id:                  EVSE_Id.Parse(1),
                                                                      Status:              OperationalStatus.Operative,
                                                                      MeterType:           "MT2",
                                                                      MeterSerialNumber:   "MSN2",
                                                                      MeterPublicKey:      "MPK2",
                                                                      Connectors:          new ChargingStationConnector[] {
                                                                                               new ChargingStationConnector(
                                                                                                   Id:    Connector_Id.Parse(1)
                                                                                               )
                                                                                           }
                                                                  )
                                                              },
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0002",
                                    MeterPublicKey:           "0xbabecafe",

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation2);

            chargingStation3  = new TestChargingStation(
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
                                    EVSEs:                    new ChargingStationEVSE[] {
                                                                  new ChargingStationEVSE(
                                                                      Id:                  EVSE_Id.Parse(1),
                                                                      Status:              OperationalStatus.Operative,
                                                                      MeterType:           "MT3",
                                                                      MeterSerialNumber:   "MSN3",
                                                                      MeterPublicKey:      "MPK3",
                                                                      Connectors:          new ChargingStationConnector[] {
                                                                                               new ChargingStationConnector(
                                                                                                   Id:    Connector_Id.Parse(1)
                                                                                               )
                                                                                           }
                                                                  )
                                                              },
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0003",
                                    MeterPublicKey:           "0xbacafebe",

                                    DNSClient:                testCSMS01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation3);

            if (testBackendWebSockets01 is not null)
            {

                var response1 = chargingStation1.ConnectWebSocket("From:GD001",
                                                                  "To:OCPPTest01",
                                                                  URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.ChargeBoxId)).Result;

                Assert.IsNotNull(response1);

                if (response1 is not null)
                {

                    // HTTP/1.1 101 Switching Protocols
                    // Date:                    Mon, 31 Oct 2022 00:33:18 GMT
                    // Server:                  GraphDefined OCPP v1.6 HTTP/WebSocket/JSON Central System API
                    // Connection:              Upgrade
                    // Upgrade:                 websocket
                    // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
                    // Sec-WebSocket-Protocol:  ocpp1.6
                    // Sec-WebSocket-Version:   13

                    Assert.AreEqual(HTTPStatusCode.SwitchingProtocols,  response1.HTTPStatusCode);
                    Assert.AreEqual("Upgrade",                          response1.Connection);
                    Assert.AreEqual("websocket",                        response1.Upgrade);
                    Assert.AreEqual("ocpp1.6",                          response1.SecWebSocketProtocol);

                }


                //var response2 = chargingStation1.ConnectWebSocket("From:GD002",
                //                                                  "To:OCPPTest01",
                //                                                  URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.ChargeBoxId)).Result;

                //var response3 = chargingStation1.ConnectWebSocket("From:GD003",
                //                                                  "To:OCPPTest01",
                //                                                  URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargingStation1.ChargeBoxId)).Result;

            }

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
