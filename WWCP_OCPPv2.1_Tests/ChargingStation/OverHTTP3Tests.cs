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
    /// opened over HTTP/3 (RFC 9220): with the password, the subprotocol and
    /// the routing they have over HTTP/1.1, in both directions, on the UDP
    /// port of the number of the port the CSMS takes WebSockets over
    /// HTTP/1.1 on. A CSMS with TLS takes them without being told to, and
    /// says so to every station it accepts (Alt-Svc).
    /// </summary>
    [TestFixture]
    public class OverHTTP3Tests
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
                          Id:                        NetworkingNode_Id.Parse("CSMS-H3"),
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

            csms.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("CS-H3"), "1234abcd");

        }

        #endregion

        #region (private) Dial(Over, WithTLS, Password = "1234abcd")

        private async Task<HTTPResponse> Dial(WebSocketHTTPVersion?  Over,
                                              Boolean                WithTLS,
                                              String                 Password = "1234abcd")
        {

            chargingStation = new TestChargingStationNode(
                                  Id:                     NetworkingNode_Id.Parse("CS-H3"),
                                  VendorName:             "GraphDefined",
                                  Model:                  "VCS.H3",
                                  HTTPAPI_Disabled:       true,
                                  WebAPI_Disabled:        true,
                                  DisableSendHeartbeats:  true,
                                  DNSClient:              csms!.DNSClient
                              );

            return await chargingStation.ConnectOCPPWebSocketClient(
                             RemoteURL:                   URL.Parse($"{(WithTLS ? "wss" : "ws")}://127.0.0.1:{server!.IPPort}/CS-H3"),
                             NextHopNetworkingNodeId:     NetworkingNode_Id.CSMS,
                             HTTPAuthentication:          HTTPBasicAuthentication.Create("CS-H3", Password),
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


        #region OCPPOverHTTP3()

        [Test]
        public async Task OCPPOverHTTP3()
        {

            ACSMS(WithTLS: true);

            var response = await Dial(WebSocketHTTPVersion.HTTP3, WithTLS: true);

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), response.EntirePDU);

            Assert.That(SpinWait.SpinUntil(() => !openedOver.IsEmpty, TimeSpan.FromSeconds(10)), Is.True,
                        "The CSMS took no WebSocket.");

            Assert.Multiple(() => {
                Assert.That(openedOver,                                                              Is.EqualTo(new[] { HTTPVersion.HTTP_3_0 }),  "what the CSMS says it was opened over");
                Assert.That(response.SecWebSocketProtocol?.Contains(Version.WebSocketSubProtocolId), Is.True,                                     "the subprotocol the CSMS chose");
            });

            await OCPPGoesBothWays();

        }

        #endregion

        #region AStationThatLeavesItToTheCSMSComesOverHTTP3TheSecondTime()

        /// <summary>
        /// The CSMS says with the first WebSocket it accepts that it is reached
        /// over HTTP/3, and a station that leaves the version to it goes there
        /// the next time.
        /// </summary>
        [Test]
        public async Task AStationThatLeavesItToTheCSMSComesOverHTTP3TheSecondTime()
        {

            ACSMS(WithTLS: true);

            var first = await Dial(WebSocketHTTPVersion.Auto, WithTLS: true);

            Assert.That(first.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), first.EntirePDU);

            Assert.That(first.AltSvc?.Services.Select(service => (service.ProtocolId, service.Port)),
                        Is.EqualTo(new[] { ("h3", server!.IPPort.ToUInt16()) }),
                        "what the CSMS says of HTTP/3");

            await chargingStation!.Stop();

            var second = await Dial(WebSocketHTTPVersion.Auto, WithTLS: true);

            Assert.That(second.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), second.EntirePDU);

            Assert.That(SpinWait.SpinUntil(() => openedOver.Count == 2, TimeSpan.FromSeconds(10)), Is.True,
                        "The CSMS took no second WebSocket.");

            Assert.That(openedOver, Is.EqualTo(new[] { HTTPVersion.HTTP_2_0, HTTPVersion.HTTP_3_0 }));

            await OCPPGoesBothWays();

        }

        #endregion

        #region TheWrongPasswordIsRefusedOverHTTP3AsOverHTTP11()

        [Test]
        public async Task TheWrongPasswordIsRefusedOverHTTP3AsOverHTTP11()
        {

            ACSMS(WithTLS: true);

            var response = await Dial(WebSocketHTTPVersion.HTTP3, WithTLS: true, Password: "not-the-password");

            Assert.Multiple(() => {
                Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.Unauthorized),  response.EntirePDU);
                Assert.That(openedOver,               Is.Empty,                                 "The CSMS took a WebSocket of somebody with the wrong password.");
            });

        }

        #endregion

        #region ACSMSThatDoesNotTakeHTTP3IsNotReachedOverItAndSaysNothingOfIt()

        [Test]
        public async Task ACSMSThatDoesNotTakeHTTP3IsNotReachedOverItAndSaysNothingOfIt()
        {

            ACSMS(WithTLS: true, AcceptHTTP3: false);

            var refused = await Dial(WebSocketHTTPVersion.HTTP3, WithTLS: true);

            Assert.Multiple(() => {
                Assert.That(refused.HTTPStatusCode,  Is.Not.EqualTo(HTTPStatusCode.SwitchingProtocols));
                Assert.That(openedOver,              Is.Empty,  "A WebSocket was opened over something else.");
            });

            await chargingStation!.Stop();

            var asBefore = await Dial(WebSocketHTTPVersion.Auto, WithTLS: true);

            Assert.Multiple(() => {
                Assert.That(asBefore.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols),  asBefore.EntirePDU);
                Assert.That(asBefore.AltSvc,          Is.Null);
            });

        }

        #endregion

        #region ACSMSWithoutTLSTakesNoHTTP3()

        [Test]
        public async Task ACSMSWithoutTLSTakesNoHTTP3()
        {

            ACSMS(WithTLS: false);

            var response = await Dial(null, WithTLS: false);

            Assert.Multiple(() => {
                Assert.That(server!.HTTP3Port,        Is.Null);
                Assert.That(server.WhyNoHTTP3,        Does.Contain("no TLS"));
                Assert.That(response.HTTPStatusCode,  Is.EqualTo(HTTPStatusCode.SwitchingProtocols),  response.EntirePDU);
                Assert.That(response.AltSvc,          Is.Null);
            });

            await OCPPGoesBothWays();

        }

        #endregion

    }

}
