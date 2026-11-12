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
    /// A charging station that dials with a one-time password ("Authorization:
    /// TOTP") is let in by a CSMS that knows its secret, and nobody else is -
    /// over HTTP/1.1, over HTTP/2 (RFC 8441) and over HTTP/3 (RFC 9220) alike:
    /// the handshake is translated for the two, and what it carries of the
    /// station is what the CSMS judges it by.
    /// </summary>
    [TestFixture]
    public class OneTimePasswordOverHTTPVersionsTests
    {

        #region Data

        private TestCSMSNode?                                csms;
        private OCPPWebSocketServer?                         server;
        private TestChargingStationNode?                     chargingStation;
        private X509Certificate2?                            serverCertificate;
        private readonly ConcurrentQueue<HTTPVersion>        openedOver = new();

        private const    String                              Station       = "CS-OTP";
        private readonly String                              sharedSecret  = $"totp-{Guid.NewGuid():N}";

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

            // What a CSMS said of itself is kept for the process, and this one is gone.
            if (server is not null)
                WebSocketAlternativeServices.Forget(URL.Parse($"wss://127.0.0.1:{server.IPPort}/"));

            if (csms is not null)
                await csms.Stop();

            if (server is not null)
                await server.DisposeAsync();

            chargingStation  = null;
            csms             = null;
            server           = null;

        }

        #endregion

        #region (private static) APortFreeForBoth()

        /// <summary>
        /// A port that is free for UDP as for TCP, on both loopback addresses, and
        /// outside the range clients are handed their ports from: Windows keeps
        /// ranges of ports for itself, for UDP and TCP apart, and the UDP port of
        /// the number of a TCP port the system handed out may be a client's.
        /// </summary>
        private static UInt16 APortFreeForBoth()
        {

            for (var attempt = 0; attempt < 200; attempt++)
            {

                var port = Random.Shared.Next(20000, 32768);

                try
                {

                    using var udp4 = new System.Net.Sockets.UdpClient(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback,     port));
                    using var udp6 = new System.Net.Sockets.UdpClient(new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Loopback, port));

                    var tcp4 = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback,     port);
                    tcp4.Start();
                    tcp4.Stop();

                    var tcp6 = new System.Net.Sockets.TcpListener(System.Net.IPAddress.IPv6Loopback, port);
                    tcp6.Start();
                    tcp6.Stop();

                    return (UInt16) port;

                }
                catch (System.Net.Sockets.SocketException)
                { }

            }

            throw new InvalidOperationException("No port is free for UDP and TCP.");

        }

        #endregion

        #region (private) ACSMS(WithTLS, AcceptHTTP3 = true)

        private void ACSMS(Boolean WithTLS, Boolean AcceptHTTP3 = true)
        {

            openedOver.Clear();

            var port = APortFreeForBoth();

            csms    = new TestCSMSNode(
                          Id:                        NetworkingNode_Id.Parse("CSMS-OTP"),
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
                          TCPPort:                    IPPort.Parse(port),
                          RequireAuthentication:      true,
                          DisableWebSocketPings:      true,
                          ServerCertificateSelector:  WithTLS
                                                          ? (tcpServer, tcpClient) => serverCertificate!
                                                          : null,
                          AutoStart:                  true
                      );

            if (!AcceptHTTP3)
                server.AcceptHTTP3 = false;

            else
            {

                // The listener tells of its start in its own time.
                SpinWait.SpinUntil(() => server.HTTP3Port is not null || server.WhyNoHTTP3 is not null, TimeSpan.FromSeconds(10));

                if (WithTLS)
                    Assert.That(server.HTTP3Port, Is.EqualTo(server.IPPort), "The CSMS takes no WebSockets over HTTP/3: " + server.WhyNoHTTP3);

            }

            server.OnNewWebSocketConnection += (timestamp, sender, connection, eventTrackingId, sharedSubprotocols, selectedSubprotocol, cancellationToken) => {
                openedOver.Enqueue(connection.HTTPVersion);
                return Task.CompletedTask;
            };

            // Known by its secret, and by nothing else: it has no password.
            server.ClientTOTPConfig[Station] = new TOTPConfig(sharedSecret, UseTLSExporterMaterial: false);

        }

        #endregion

        #region (private) Dial(Over, Login = Station, Secret = null, As = Station, BoundToTLS = false)

        private async Task<HTTPResponse> Dial(WebSocketHTTPVersion  Over,
                                              String                Login        = Station,
                                              String?               Secret       = null,
                                              String                As           = Station,
                                              Boolean               BoundToTLS   = false)
        {

            chargingStation = new TestChargingStationNode(
                                  Id:                     NetworkingNode_Id.Parse(As),
                                  VendorName:             "GraphDefined",
                                  Model:                  "VCS.OTP",
                                  HTTPAPI_Disabled:       true,
                                  WebAPI_Disabled:        true,
                                  DisableSendHeartbeats:  true,
                                  DNSClient:              csms!.DNSClient
                              );

            return await chargingStation.ConnectOCPPWebSocketClient(
                             RemoteURL:                   URL.Parse($"wss://127.0.0.1:{server!.IPPort}/{As}"),
                             NextHopNetworkingNodeId:     NetworkingNode_Id.CSMS,
                             DisableWebSocketPings:       true,
                             WebSocketOver:               Over,
                             TOTPConfig:                  new TOTPConfig(Secret ?? sharedSecret, UseTLSExporterMaterial: BoundToTLS),
                             TOTPLogin:                   Login,
                             RemoteCertificateValidator:  (sender, certificate, certificateChain, tlsClient, policyErrors) =>
                                                              certificate is not null && certificate.Thumbprint == serverCertificate!.Thumbprint
                                                                  ? TLSValidationResult.Success()
                                                                  : TLSValidationResult.Failed("Unexpected server certificate!")
                         );

        }

        #endregion

        #region (private static) VersionOf(Over)

        private static HTTPVersion VersionOf(WebSocketHTTPVersion Over)

            => Over switch {
                   WebSocketHTTPVersion.HTTP2  => HTTPVersion.HTTP_2_0,
                   WebSocketHTTPVersion.HTTP3  => HTTPVersion.HTTP_3_0,
                   _                           => HTTPVersion.HTTP_1_1
               };

        #endregion


        #region AStationWithAOneTimePasswordIsLetIn(Over)

        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP3)]
        public async Task AStationWithAOneTimePasswordIsLetIn(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var response = await Dial(Over);

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), response.EntirePDU);

            Assert.That(SpinWait.SpinUntil(() => !openedOver.IsEmpty, TimeSpan.FromSeconds(10)), Is.True,
                        "The CSMS took no WebSocket.");

            Assert.That(openedOver, Is.EqualTo(new[] { VersionOf(Over) }), "what the CSMS says it was opened over");

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

        #region AOneTimePasswordOfAnotherSecretIsRefused(Over)

        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP3)]
        public async Task AOneTimePasswordOfAnotherSecretIsRefused(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var response = await Dial(Over, Secret: "not-the-secret-of-this-station");

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.Unauthorized),  response.EntirePDU);
                Assert.That(openedOver,               Is.Empty,                                 "The CSMS took a WebSocket of somebody who does not know the secret.");
            });

        }

        #endregion

        #region AOneTimePasswordOpensNoConnectionAsAnotherStation(Over)

        /// <summary>
        /// The login is held against the path, as for a password: a right
        /// one-time password of the one station opens no connection as another.
        /// </summary>
        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP3)]
        public async Task AOneTimePasswordOpensNoConnectionAsAnotherStation(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var response = await Dial(Over, As: "CS-OTHER");

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.Not.EqualTo(HTTPStatusCode.SwitchingProtocols),  response.EntirePDU);
                Assert.That(openedOver,               Is.Empty,                                           "A one-time password of the one station opened a connection as another.");
            });

        }

        #endregion

        #region AStationTheCSMSDoesNotKnowIsRefused(Over)

        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP3)]
        public async Task AStationTheCSMSDoesNotKnowIsRefused(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var response = await Dial(Over, Login: "CS-UNKNOWN", As: "CS-UNKNOWN");

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.Unauthorized),  response.EntirePDU);
                Assert.That(openedOver,               Is.Empty,                                 "The CSMS took a WebSocket of a station it does not know.");
            });

        }

        #endregion

        #region AOneTimePasswordBoundToTLSIsNotSent(Over)

        /// <summary>
        /// A one-time password bound to the TLS session cannot be made, over
        /// none of them: the station says so, and asks nobody.
        /// </summary>
        [TestCase(WebSocketHTTPVersion.HTTP1_1)]
        [TestCase(WebSocketHTTPVersion.HTTP2)]
        [TestCase(WebSocketHTTPVersion.HTTP3)]
        public async Task AOneTimePasswordBoundToTLSIsNotSent(WebSocketHTTPVersion Over)
        {

            ACSMS(WithTLS: true);

            var refusal = await Assert.ThrowsAsync<InvalidOperationException>(() => Dial(Over, BoundToTLS: true));

            Assert.Multiple(() => {
                Assert.That(refusal!.Message,  Is.EqualTo(TOTPConfig.TLSChannelBindingNotAvailable));
                Assert.That(openedOver,        Is.Empty,  "The CSMS was asked all the same.");
            });

        }

        #endregion

    }

}
