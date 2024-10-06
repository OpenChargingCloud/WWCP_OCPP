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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS
{

    /// <summary>
    /// Charge Point with Central system tests.
    /// </summary>
    public abstract class ACPwithCSTests
    {

        #region Data

        protected TestCentralSystemNode?  centralSystem;
        protected OCPPWebSocketServer?    centralSystemWSS;

        protected TestChargePointNode?    chargePoint;
        protected OCPPWebSocketClient?    chargePointWSC;

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async virtual Task SetupOnce()
        {

            Timestamp.Reset();

            centralSystem     = new TestCentralSystemNode(
                                    Id:                     NetworkingNode_Id.Parse("OCPPTest01"),
                                    VendorName:             "GraphDefined",
                                    Model:                  "CSSimulator1",
                                    HTTPUploadPort:         IPPort.Parse(9100),
                                    DNSClient:              new DNSClient(
                                                                SearchForIPv6DNSServers: false,
                                                                SearchForIPv4DNSServers: false
                                                            )
                                );

            Assert.That(centralSystem, Is.Not.Null);

                                // Will use a random TCP port!
            centralSystemWSS  = centralSystem.AttachWebSocketServer(
                                    RequireAuthentication:   false,
                                    AutoStart:               true
                                );

            Assert.That(centralSystemWSS, Is.Not.Null);



            //var ppp = ECCKeyPair.GenerateKeys();
            //var pk1 = ppp.PublicKeyBytes. ToHexString();
            //var pk2 = ppp.PrivateKeyBytes.ToHexString();

            chargePoint = new TestChargePointNode(

                              ChargeBoxId:               NetworkingNode_Id.Parse("GD001"),
                              ChargePointVendor:         "GraphDefined OEM #1",
                              ChargePointModel:          "VCP.1",
                              Connectors:                [
                                                             new ConnectorSpec(
                                                                 Availabilities.Operative,
                                                                 MaxPower:     Watt.ParseKW(11),
                                                                 EnergyMeter:  new OCPP.Energy_Meter(
                                                                                   Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                                                   Manufacturer:  "GraphDefined",
                                                                                   Model:         "eMeterOne",
                                                                                   SerialNumber:  "SN-EN0021",
                                                                                   KeyPairs:       [
                                                                                                       ECCKeyPair.ParsePrivateKey(
                                                                                                           "00a2c3055ad9038b0c05cb113b1938ea8f21cc5d05b73175f65fbb0b3dfab83211"
                                                                                                       )
                                                                                                   ]
                                                                               )
                                                             ),
                                                             new ConnectorSpec(
                                                                 Availabilities.Operative,
                                                                 MaxPower:     Watt.ParseKW(22),
                                                                 EnergyMeter:  new OCPP.Energy_Meter(
                                                                                   Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0012"),
                                                                                   Manufacturer:  "GraphDefined",
                                                                                   Model:         "eMeterOne",
                                                                                   SerialNumber:  "SN-EN0022",
                                                                                   KeyPairs:       [
                                                                                                       ECCKeyPair.ParsePrivateKey(
                                                                                                           "00a6e8c7b5ad7cceae504d54177ae83c19edce4e210dd69d00b9c641e8275ee94e"
                                                                                                       )
                                                                                                   ]
                                                                               )
                                                             )
                                                         ],

                              Description:               I18NString.Create(Languages.en, "Our first virtual charging station!"),
                              ChargePointSerialNumber:   "SN-CP0001",
                              ChargeBoxSerialNumber:     "SN-CB0001",
                              FirmwareVersion:           "v0.1",
                              Iccid:                     "0000",
                              IMSI:                      "1111",

                              UplinkEnergyMeter:         new OCPP.Energy_Meter(
                                                             Id:             OCPP.EnergyMeter_Id.Parse("SN-EN0011"),
                                                             Manufacturer:  "GraphDefined",
                                                             Model:         "eMeterOne",
                                                             SerialNumber:  "SN-EN0001",
                                                             KeyPairs:       [
                                                                                 ECCKeyPair.ParsePrivateKey(
                                                                                     "00f781fd21e8dbf07b7952f5f568805e739f8d2a9970a9edab003e934cc1c03919"
                                                                                 )
                                                                             ]
                                                         ),

                              //HTTPBasicAuth:             new Tuple<String, String>("OLI_001", "1234"),
                              //HTTPBasicAuth:             new Tuple<String, String>("GD001", "1234"),
                              DNSClient:                 centralSystem.DNSClient

                          );

            Assert.That(chargePoint, Is.Not.Null);



            centralSystem.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("test01"), "1234abcd");

            Assert.That(chargePoint.ChargeBoxSerialNumber, Is.Not.Null);

            var addChargeBoxAccessResult = await centralSystem.AddChargeBoxAccess(
                                                     ChargeBox_Id.Parse(chargePoint.ChargeBoxSerialNumber!),
                                                     ChargeBoxAccessTypes.Allowed
                                                 );

            Assert.That(addChargeBoxAccessResult.Result, Is.EqualTo(CommandResult.Success));



            var response1 = await chargePoint.ConnectOCPPWebSocketClient(URL.Parse($"http://127.0.0.1:{centralSystemWSS.IPPort}/{chargePoint.Id}"));

            Assert.That(response1, Is.Not.Null);

            if (response1 is not null)
            {

                // HTTP/1.1 101 Switching Protocols
                // Date:                    Mon, 31 Oct 2022 00:33:18 GMT
                // Server:                  GraphDefined OCPP v1.6 WebSocket Service
                // Connection:              Upgrade
                // Upgrade:                 websocket
                // Sec-WebSocket-Accept:    HSmrc0sMlYUkAGmm5OPpG2HaGWk=
                // Sec-WebSocket-Protocol:  ocpp1.6
                // Sec-WebSocket-Version:   13

                Assert.That(response1.HTTPStatusCode,                 Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(response1.Connection,                     Is.EqualTo(ConnectionType.Upgrade));
                Assert.That(response1.Upgrade,                        Is.EqualTo("websocket"));
                Assert.That(response1.SecWebSocketProtocol.First(),   Is.EqualTo("ocpp1.6"));

            }

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual void ShutdownOnce()
        {

            centralSystemWSS?.Shutdown();

            centralSystem     = null;
            centralSystemWSS  = null;

        }

        #endregion

    }

}
