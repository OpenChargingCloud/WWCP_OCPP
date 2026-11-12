/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// A charging station and a CSMS speak OCPP over a WebSocket that was
    /// opened over HTTP/2 (RFC 8441): with the password, the subprotocol and
    /// the routing they have over HTTP/1.1, in both directions, on the port
    /// the CSMS takes WebSockets over HTTP/1.1 on.
    /// </summary>
    [TestFixture]
    public class OverHTTP2Tests
    {

        #region Data

        private TestCSMSNode?                                csms;
        private OCPPWebSocketServer?                         server;
        private TestChargingStationNode?                     chargingStation;
        private X509Certificate2?                            serverCertificate;
        private readonly ConcurrentQueue<HTTPVersion>        openedOver = new();

        #endregion

        #region SetUp / TearDown

        [OneTimeSetUp]
        public void MakeACertificate()
        {

            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var request   = new CertificateRequest("CN=127.0.0.1", key, HashAlgorithmName.SHA256);

            var names     = new SubjectAlternativeNameBuilder();
            names.AddIpAddress(System.Net.IPAddress.Loopback);
            request.CertificateExtensions.Add(names.Build());

            using var selfSigned = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));

            // Windows wants a key it can find again for the server side of a handshake.
            serverCertificate = X509CertificateLoader.LoadPkcs12(selfSigned.Export(X509ContentType.Pfx), null);

        }

        [OneTimeTearDown]
        public void DisposeOfTheCertificate()
        {
            serverCertificate?.Dispose();
            serverCertificate = null;
        }

        [TearDown]
        public async Task TakeThemAway()
        {

            if (chargingStation is not null)
                await chargingStation.Stop();

            if (csms is not null)
                await csms.Stop();

            if (server is not null)
                await server.DisposeAsync();

            chargingStation  = null;
            csms             = null;
            server           = null;

        }

        #endregion

        #region (private) ACSMS(WithTLS, AcceptHTTP2 = true)

        private void ACSMS(Boolean WithTLS, Boolean AcceptHTTP2 = true)
        {

            openedOver.Clear();

            csms    = new TestCSMSNode(
                          Id:                        NetworkingNode_Id.Parse("CSMS-H2"),
                          VendorName:                "GraphDefined",
                          Model:                     "OCPP-CSMS-Test-Server",
                          HTTPAPI_Disabled:          true,
                          HTTPDownloadAPI_Disabled:  true,
                          HTTPUploadAPI_Disabled:    true,
                          WebPaymentsAPI_Disabled:   true,
                          WebAPI_Disabled:           true,
                          DNSClient:                 new DNSClient(
                                                         SearchForIPv6DNSServers: false,
                                                         SearchForIPv4DNSServers: false
                                                     )
                      );

            server  = csms.AttachWebSocketServer(
                          TCPPort:                    null,
                          RequireAuthentication:      true,
                          DisableWebSocketPings:      true,
                          ServerCertificateSelector:  WithTLS
                                                          ? (tcpServer, tcpClient) => serverCertificate!
                                                          : null,
                          AutoStart:                  true
                      );

            if (!AcceptHTTP2)
                server.AcceptHTTP2 = false;

            server.OnNewWebSocketConnection += (timestamp, sender, connection, eventTrackingId, sharedSubprotocols, selectedSubprotocol, cancellationToken) => {
                openedOver.Enqueue(connection.HTTPVersion);
                return Task.CompletedTask;
            };

            csms.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("CS-H2"), "1234abcd");

        }

        #endregion

        #region (private) Dial(Over, WithTLS, Password = "1234abcd")

        private async Task<HTTPResponse> Dial(WebSocketHTTPVersion?  Over,
                                              Boolean                WithTLS,
                                              String                 Password = "1234abcd")
        {

            chargingStation = new TestChargingStationNode(
                                  Id:                     NetworkingNode_Id.Parse("CS-H2"),
                                  VendorName:             "GraphDefined",
                                  Model:                  "VCS.H2",
                                  HTTPAPI_Disabled:       true,
                                  WebAPI_Disabled:        true,
                                  DisableSendHeartbeats:  true,
                                  DNSClient:              csms!.DNSClient
                              );

            return await chargingStation.ConnectOCPPWebSocketClient(
                             RemoteURL:                   URL.Parse($"{(WithTLS ? "wss" : "ws")}://127.0.0.1:{server!.IPPort}/CS-H2"),
                             NextHopNetworkingNodeId:     NetworkingNode_Id.CSMS,
                             HTTPAuthentication:          HTTPBasicAuthentication.Create("CS-H2", Password),
                             DisableWebSocketPings:       true,
                             WebSocketOver:               Over,
                             RemoteCertificateValidator:  (sender, certificate, certificateChain, tlsClient, policyErrors) =>
                                                              certificate is not null && certificate.Thumbprint == serverCertificate!.Thumbprint
                                                                  ? TLSValidationResult.Success()
                                                                  : TLSValidationResult.Failed("Unexpected server certificate!")
                         );

        }

        #endregion

        #region (private) OCPPGoesBothWays()

        private async Task OCPPGoesBothWays()
        {

            var boot   = await chargingStation!.SendBootNotification(
                                   BootReason:  BootReason.PowerUp
                               );

            var reset  = await csms!.Reset(
                                   Destination:  SourceRouting.To(chargingStation.Id),
                                   ResetType:    ResetType.OnIdle
                               );

            Assert.Multiple(() => {
                Assert.That(boot. Result.ResultCode,  Is.EqualTo(ResultCode.OK),                  "the station's BootNotification: " + boot. Result.Description);
                Assert.That(boot. Status,             Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That(reset.Result.ResultCode,  Is.EqualTo(ResultCode.OK),                  "the CSMS's Reset: "              + reset.Result.Description);
            });

        }

        #endregion


        #region OCPPOverHTTP2(WithTLS, Over)

        /// <summary>
        /// Without TLS HTTP/2 is spoken from the first byte; with TLS the two
        /// agree on it (ALPN) - where nothing else will do, and where the
        /// server chooses.
        /// </summary>
        [TestCase(false, WebSocketHTTPVersion.HTTP2)]
        [TestCase(true,  WebSocketHTTPVersion.HTTP2)]
        [TestCase(true,  WebSocketHTTPVersion.Auto)]
        public async Task OCPPOverHTTP2(Boolean WithTLS, WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS);

            var response = await Dial(Over, WithTLS);

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), response.EntirePDU);

            Assert.That(SpinWait.SpinUntil(() => !openedOver.IsEmpty, TimeSpan.FromSeconds(10)), Is.True,
                        "The CSMS took no WebSocket.");

            Assert.Multiple(() => {
                Assert.That(openedOver,                                                              Is.EqualTo(new[] { HTTPVersion.HTTP_2_0 }),  "what the CSMS says it was opened over");
                Assert.That(response.SecWebSocketProtocol?.Contains(Version.WebSocketSubProtocolId), Is.True,                                     "the subprotocol the CSMS chose");
            });

            await OCPPGoesBothWays();

        }

        #endregion

        #region OCPPOverHTTP11AsBefore(WithTLS, Over)

        /// <summary>
        /// A station that says nothing, or HTTP/1.1, is spoken to as before by
        /// a CSMS that takes HTTP/2 as well; and so is one that leaves it to a
        /// CSMS that does not, or that it reaches without TLS.
        /// </summary>
        [TestCase(false, null,                          true)]
        [TestCase(true,  null,                          true)]
        [TestCase(true,  WebSocketHTTPVersion.HTTP1_1,  true)]
        [TestCase(false, WebSocketHTTPVersion.Auto,     true)]
        [TestCase(true,  WebSocketHTTPVersion.Auto,     false)]
        public async Task OCPPOverHTTP11AsBefore(Boolean WithTLS, WebSocketHTTPVersion? Over, Boolean AcceptHTTP2)
        {

            ACSMS(WithTLS, AcceptHTTP2);

            var response = await Dial(Over, WithTLS);

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), response.EntirePDU);

            Assert.That(SpinWait.SpinUntil(() => !openedOver.IsEmpty, TimeSpan.FromSeconds(10)), Is.True,
                        "The CSMS took no WebSocket.");

            Assert.That(openedOver, Is.EqualTo(new[] { HTTPVersion.HTTP_1_1 }));

            await OCPPGoesBothWays();

        }

        #endregion

        #region TheWrongPasswordIsRefusedOverHTTP2AsOverHTTP11(Over)

        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        public async Task TheWrongPasswordIsRefusedOverHTTP2AsOverHTTP11(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var response = await Dial(Over, WithTLS: true, Password: "not-the-password");

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.Unauthorized),  response.EntirePDU);
                Assert.That(openedOver,               Is.Empty,                                 "The CSMS took a WebSocket of somebody with the wrong password.");
            });

        }

        #endregion

        #region ACSMSThatDoesNotTakeHTTP2IsNotReachedOverIt()

        [Test]
        public async Task ACSMSThatDoesNotTakeHTTP2IsNotReachedOverIt()
        {

            ACSMS(WithTLS: true, AcceptHTTP2: false);

            var response = await Dial(WebSocketHTTPVersion.HTTP2, WithTLS: true);

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.Not.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(openedOver,               Is.Empty,  "A WebSocket was opened over something else.");
            });

        }

        #endregion

    }

}
