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

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.LocalController;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.LocalController
{

    /// <summary>
    /// AttachWebSocketServer takes an HTTPServiceName, and that name is what the
    /// WebSocket handshake reports back in its HTTP Server header.
    /// </summary>
    [TestFixture]
    public class AttachWebSocketServerNameTests
    {

        private static DNSClient ADNSClient()

            => new (SearchForIPv6DNSServers: false,
                    SearchForIPv4DNSServers: false);


        #region TheGivenHTTPServiceName_ReachesTheServer()

        [Test]
        public async Task TheGivenHTTPServiceName_ReachesTheServer()
        {

            var localController = new TestLocalControllerNode(
                                      Id:                        NetworkingNode_Id.Parse("GD-NN-NAME"),
                                      VendorName:                "GraphDefined",
                                      Model:                     "VCP.1",
                                      HTTPAPI_Disabled:          true,
                                      HTTPDownloadAPI_Disabled:  true,
                                      HTTPUploadAPI_Disabled:    true,
                                      WebAPI_Disabled:           true,
                                      DNSClient:                 ADNSClient()
                                  );

            var expected  = "A name of our own choosing";

            var server    = localController.AttachWebSocketServer(
                                HTTPServiceName:  expected,
                                TCPPort:          null,   // Random port!
                                AutoStart:        false
                            );

            try
            {
                Assert.That(server.HTTPServiceName, Is.EqualTo(expected));
            }
            finally
            {
                await localController.Stop();
            }

        }

        #endregion

        #region TheGivenHTTPServiceName_IsWhatTheHandshakeReports()

        [Test]
        public async Task TheGivenHTTPServiceName_IsWhatTheHandshakeReports()
        {

            var localController  = new TestLocalControllerNode(
                                       Id:                        NetworkingNode_Id.Parse("GD-NN-NAME2"),
                                       VendorName:                "GraphDefined",
                                       Model:                     "VCP.1",
                                       HTTPAPI_Disabled:          true,
                                       HTTPDownloadAPI_Disabled:  true,
                                       HTTPUploadAPI_Disabled:    true,
                                       WebAPI_Disabled:           true,
                                       DNSClient:                 ADNSClient()
                                   );

            var expected         = "A name of our own choosing";

            var server           = localController.AttachWebSocketServer(
                                       HTTPServiceName:  expected,
                                       TCPPort:          null,   // Random port!
                                       AutoStart:        true
                                   );

            var chargingStation  = new TestChargingStationNode(
                                       Id:           NetworkingNode_Id.Parse("GD-CP-NAME2"),
                                       VendorName:   "GraphDefined",
                                       Model:        "VCP.1",
                                       DNSClient:    ADNSClient()
                                   );

            try
            {

                server.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

                var response = await chargingStation.ConnectOCPPWebSocketClient(
                                         RemoteURL:            URL.Parse($"http://127.0.0.1:{server.IPPort}/{chargingStation.Id}"),
                                         HTTPAuthentication:   HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd")
                                     );

                Assert.Multiple(() => {
                    Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
                    Assert.That(response.Server,          Is.EqualTo(expected));
                });

            }
            finally
            {
                await chargingStation.Stop();
                await localController.Stop();
            }

        }

        #endregion

    }

}
