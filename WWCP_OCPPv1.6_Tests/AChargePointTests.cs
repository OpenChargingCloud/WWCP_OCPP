/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using NUnit.Framework;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Charge point test defaults.
    /// </summary>
    public abstract class AChargePointTests : ACentralSystemTests
    {

        #region Data

        protected TestChargePoint? chargingStation1;
        protected TestChargePoint? chargingStation2;
        protected TestChargePoint? chargingStation3;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            chargingStation1  = new TestChargePoint(
                                    ChargeBoxId:              ChargeBox_Id.Parse("GD001"),
                                    ChargePointVendor:        "GraphDefined OEM #1",
                                    ChargePointModel:         "VCP.1",
                                    NumberOfConnectors:       2,

                                    Description:              I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                    ChargePointSerialNumber:  "SN-CP0001",
                                    ChargeBoxSerialNumber:    "SN-CB0001",
                                    FirmwareVersion:          "v0.1",
                                    Iccid:                    "0000",
                                    IMSI:                     "1111",
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0001",
                                    MeterPublicKey:           "0xcafebabe",

                                    //HTTPBasicAuth:            new Tuple<String, String>("OLI_001", "1234"),
                                    //HTTPBasicAuth:            new Tuple<String, String>("GD001", "1234"),
                                    DNSClient:                testCentralSystem01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation1);

            chargingStation2  = new TestChargePoint(
                                    ChargeBoxId:              ChargeBox_Id.Parse("CP002"),
                                    ChargePointVendor:        "GraphDefined OEM #2",
                                    ChargePointModel:         "VCP.2",
                                    NumberOfConnectors:       2,

                                    Description:              I18NString.Create(Languages.en, "Our 2nd virtual charging station!"),
                                    ChargePointSerialNumber:  "SN-CP0002",
                                    ChargeBoxSerialNumber:    "SN-CB0002",
                                    FirmwareVersion:          "v0.2",
                                    Iccid:                    "3333",
                                    IMSI:                     "4444",
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0002",
                                    MeterPublicKey:           "0xbabecafe",

                                    DNSClient:                testCentralSystem01!.DNSClient
                                );

            Assert.IsNotNull(chargingStation2);

            chargingStation3  = new TestChargePoint(
                                    ChargeBoxId:              ChargeBox_Id.Parse("CP003"),
                                    ChargePointVendor:        "GraphDefined OEM #3",
                                    ChargePointModel:         "VCP.3",
                                    NumberOfConnectors:       4,

                                    Description:              I18NString.Create(Languages.en, "Our 3rd virtual charging station!"),
                                    ChargePointSerialNumber:  "SN-CP0003",
                                    ChargeBoxSerialNumber:    "SN-CB0003",
                                    FirmwareVersion:          "v0.3",
                                    Iccid:                    "5555",
                                    IMSI:                     "6666",
                                    MeterType:                "Virtual Energy Meter",
                                    MeterSerialNumber:        "SN-EN0003",
                                    MeterPublicKey:           "0xbacafebe",

                                    DNSClient:                testCentralSystem01!.DNSClient
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
