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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// Charge point test defaults.
    /// </summary>
    public abstract class AChargePointTests : ACentralSystemTests
    {

        #region Data

        protected TestChargePointNode? chargePoint1;
        protected TestChargePointNode? chargePoint2;
        protected TestChargePointNode? chargePoint3;

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            chargePoint1  = new TestChargePointNode(

                                ChargeBoxId:              NetworkingNode_Id.Parse("GD001"),
                                ChargePointVendor:        "GraphDefined OEM #1",
                                ChargePointModel:         "VCP.1",
                                Connectors:               [
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#1",
                                                                  MaxPower:           Watt.ParseKW(22),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0011"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              )
                                                          ],

                                Description:              I18NString.Create(Languages.en, "Our first virtual charging station!"),
                                ChargePointSerialNumber:  "SN-CP0001",
                                ChargeBoxSerialNumber:    "SN-CB0001",
                                FirmwareVersion:          "v0.1",
                                Iccid:                    "0000",
                                IMSI:                     "1111",

                                UplinkEnergyMeter:        new OCPP.Energy_Meter(
                                                              Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                              Manufacturer:  "GraphDefined",
                                                              Model:         "Virtual Energy Meter",
                                                              SerialNumber:  "SN-EN1001"
                                                           //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                          ),

                                //HTTPBasicAuth:            new Tuple<String, String>("OLI_001", "1234"),
                                //HTTPBasicAuth:            new Tuple<String, String>("GD001", "1234"),
                                DNSClient:                testCentralSystem01!.DNSClient

                            );

            ClassicAssert.IsNotNull(chargePoint1);

            chargePoint2  = new TestChargePointNode(

                                ChargeBoxId:              NetworkingNode_Id.Parse("CP002"),
                                ChargePointVendor:        "GraphDefined OEM #2",
                                ChargePointModel:         "VCP.2",
                                Connectors:               [
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#1",
                                                                  MaxPower:           Watt.ParseKW(11),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0021"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              ),
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#2",
                                                                  MaxPower:           Watt.ParseKW(22),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0022"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              )
                                                          ],

                                Description:              I18NString.Create(Languages.en, "Our 2nd virtual charging station!"),
                                ChargePointSerialNumber:  "SN-CP0002",
                                ChargeBoxSerialNumber:    "SN-CB0002",
                                FirmwareVersion:          "v0.2",
                                Iccid:                    "3333",
                                IMSI:                     "4444",

                                UplinkEnergyMeter:        new OCPP.Energy_Meter(
                                                              Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                              Manufacturer:  "GraphDefined",
                                                              Model:         "Virtual Energy Meter",
                                                              SerialNumber:  "SN-EN1002"
                                                           //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                          ),

                                DNSClient:                testCentralSystem01!.DNSClient

                            );

            ClassicAssert.IsNotNull(chargePoint2);

            chargePoint3  = new TestChargePointNode(

                                ChargeBoxId:              NetworkingNode_Id.Parse("CP003"),
                                ChargePointVendor:        "GraphDefined OEM #3",
                                ChargePointModel:         "VCP.3",
                                Connectors:               [
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#1",
                                                                  MaxPower:           Watt.ParseKW(11),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0031"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              ),
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#2",
                                                                  MaxPower:           Watt.ParseKW(11),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0032"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              ),
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#3",
                                                                  MaxPower:           Watt.ParseKW(22),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0032"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              ),
                                                              new ConnectorSpec(
                                                                  Availabilities.Operative,
                                                                  PhysicalReference:  "#4",
                                                                  MaxPower:           Watt.ParseKW(22),
                                                                  EnergyMeter:        new OCPP.Energy_Meter(
                                                                                          Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                          Manufacturer:  "GraphDefined",
                                                                                          Model:         "Virtual Energy Meter",
                                                                                          SerialNumber:  "SN-EN0032"
                                                                                       //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                                                      )
                                                              )
                                                          ],

                                Description:              I18NString.Create(Languages.en, "Our 3rd virtual charging station!"),
                                ChargePointSerialNumber:  "SN-CP0003",
                                ChargeBoxSerialNumber:    "SN-CB0003",
                                FirmwareVersion:          "v0.3",
                                Iccid:                    "5555",
                                IMSI:                     "6666",

                                UplinkEnergyMeter:        new OCPP.Energy_Meter(
                                                              Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                              Manufacturer:  "GraphDefined",
                                                              Model:         "Virtual Energy Meter",
                                                              SerialNumber:  "SN-EN1003"
                                                           //   PublicKeys:    [ WWCP.PublicKey.Parse("0x...") ]
                                                          ),

                                DNSClient:                testCentralSystem01!.DNSClient

                            );

            ClassicAssert.IsNotNull(chargePoint3);

            if (testBackendWebSockets01 is not null)
            {

                var response1 = chargePoint1.ConnectOCPPWebSocketClient(URL.Parse("http://127.0.0.1:" + testBackendWebSockets01.IPPort.ToString() + "/" + chargePoint1.Id)).Result;

                ClassicAssert.IsNotNull(response1);

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

                    ClassicAssert.AreEqual(HTTPStatusCode.SwitchingProtocols,  response1.HTTPStatusCode);
                    ClassicAssert.AreEqual("Upgrade",                          response1.Connection);
                    ClassicAssert.AreEqual("websocket",                        response1.Upgrade);
                    ClassicAssert.AreEqual("ocpp1.6",                          response1.SecWebSocketProtocol.First());

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

            chargePoint1 = null;
            chargePoint2 = null;
            chargePoint3 = null;

        }

        #endregion


    }

}
