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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Server;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.SSH
{

    /// <summary>
    /// OCPP 2.1 over the SSH subsystem "ocpp@open.charging.cloud": a CSMS serving it beside its WebSocket
    /// server, and charging stations connecting with <see cref="OCPP.NetworkingNode.AOCPPNetworkingNode.ConnectOCPPClient"/>
    /// and an "ssh://" URL. Everything a WebSocket carries: requests both ways, binary messages, and the
    /// station coming back after its connection went.
    /// </summary>
    [TestFixture]
    public class OCPPOverSSHTests : ACSMSTests
    {

        #region Data

        private readonly ISshHostKey      hostKey     = SshHostKey.GenerateEd25519();
        private readonly ISshHostKey      stationKey  = SshHostKey.GenerateEd25519();
        private readonly ISshHostKey      strangerKey = SshHostKey.GenerateEd25519();

        private OCPPSshServer?            sshServer;
        private SshServer?                listener;

        private TestChargingStationNode?  station;

        #endregion

        #region SetupOnce() / StopTheSSHServer() / StopTheStation()

        [OneTimeSetUp]
        public override async Task SetupOnce()
        {

            await base.SetupOnce();

            sshServer  = testCSMS1!.AttachOCPPSshSubsystem();

            listener   = new SshServer(new SshServerOptions {
                             HostKeys       = [ hostKey ],
                             Authenticator  = SshUserAuthenticator.ForAuthorizedKeys(stationKey.PublicKeyBlob),
                             Subsystems     = (session, name, ct) => ValueTask.FromResult(name == OCPPSshServer.OCPPSubsystem ? sshServer.Handler : null)
                         });

            await listener.StartAsync(new IPSocket(IPv4Address.Localhost, IPPort.Auto), CancellationToken.None);

        }

        [OneTimeTearDown]
        public async Task StopTheSSHServer()
        {
            if (listener is not null)
                await listener.DisposeAsync();
        }

        [TearDown]
        public async Task StopTheStation()
        {

            if (station is not null)
                await station.Stop();

            station = null;

        }

        #endregion

        #region (private) Helpers

        /// <summary>
        /// What the charging stations of this fixture offer when they sign in:
        /// what the node offers by itself, unless the fixture runs over CBOR.
        /// </summary>
        private IEnumerable<String>? Offered

            => Encoding == MessageEncodings.JSON
                   ? null
                   : OCPPSubprotocols.Offer([ Version.WebSocketSubProtocolId, "ocpp2.0.1" ], Encoding);

        /// <summary>
        /// What a connection of this fixture agrees on.
        /// </summary>
        protected String AgreedOn

            => Encoding == MessageEncodings.JSON
                   ? Version.WebSocketSubProtocolId
                   : Version.WebSocketSubProtocolIdCBOR;

        private TestChargingStationNode NewStation(String Id)

            => station = new TestChargingStationNode(
                             Id:                     NetworkingNode_Id.Parse(Id),
                             VendorName:             "GraphDefined OEM #1",
                             Model:                  "VCP.1",
                             DisableSendHeartbeats:  true,
                             HTTPAPI_Disabled:       true,
                             WebAPI_Disabled:        true,
                             DNSClient:              testCSMS1!.DNSClient
                         );

        private URL SshURL(String Login, String? ConnectAs = null)

            => URL.Parse($"ssh://{Login}@127.0.0.1:{listener!.LocalEndPoint.Port}{(ConnectAs is not null ? $"/{ConnectAs}" : "")}");

        private SshClientOptions Login(String Username, ISshHostKey? Key = null)

            => new () {
                   Username       = Username,
                   VerifyHostKey  = blob => blob.AsSpan().SequenceEqual(hostKey.PublicKeyBlob),
                   Credentials    = [ Key ?? stationKey ]
               };

        /// <summary>
        /// A charging station connected over SSH, and registered by the CSMS - which it is after the WELCOME,
        /// and so possibly a moment after the station heard it.
        /// </summary>
        private async Task<TestChargingStationNode> ConnectedStation(String Id)
        {

            var chargingStation  = NewStation(Id);
            var result           = await chargingStation.ConnectOCPPClient(SshURL(Id), Subprotocols: Offered, Ssh: Login(Id));

            Assert.That(result.IsConnected, Is.True, result.ToString());

            await Until(() => sshServer!.ConnectedNetworkingNodeIds.Contains(chargingStation.Id), $"Registering '{Id}'");

            return chargingStation;

        }

        private static async Task Until(Func<Boolean> Condition, String What)
        {
            var until = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);
            while (!Condition())
            {
                if (DateTimeOffset.UtcNow > until)
                    Assert.Fail($"{What} did not happen within 10 s.");
                await Task.Delay(10);
            }
        }

        private Task<SendRequestState> GetLocalListVersion(NetworkingNode_Id Destination)

            => testCSMS1!.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:   Timestamp.Now,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkingMode:     NetworkingMode.Standard,
                       Destination:        SourceRouting.To(Destination),
                       NetworkPath:        NetworkPath.From(testCSMS1.Id),
                       RequestId:          Request_Id.NewRandom(),
                       Action:             "GetLocalListVersion",
                       Payload:            new JObject(),
                       RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                   )
               );

        #endregion


        #region BootAndHeartbeat()

        /// <summary>
        /// A station boots and sends a heartbeat over SSH; the CSMS sees them come over an SSH connection,
        /// signed in as the station, speaking OCPP 2.1.
        /// </summary>
        [Test]
        public async Task BootAndHeartbeat()
        {

            var chargingStation = await ConnectedStation("GD-SSH-001");

            IMessageConnection? bootCameOver = null;

            OnBootNotificationRequestReceivedDelegate seeBoot = (timestamp, sender, connection, request, ct) => {
                bootCameOver = connection;
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnBootNotificationRequestReceived += seeBoot;

            try
            {

                var boot       = await chargingStation.SendBootNotification(BootReason: BootReason.PowerUp);
                var heartbeat  = await chargingStation.SendHeartbeat();

                Assert.That(boot.Result.ResultCode,       Is.EqualTo(ResultCode.OK), boot.Result.ToString());
                Assert.That(boot.Status,                  Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That(heartbeat.Result.ResultCode,  Is.EqualTo(ResultCode.OK), heartbeat.Result.ToString());

                Assert.That(bootCameOver,                 Is.InstanceOf<ISshMessageConnection>());
                Assert.That(bootCameOver!.Transport,      Is.EqualTo(MessageTransport.SSH));
                Assert.That(bootCameOver.Role,            Is.EqualTo(ConnectionRole.Server));
                Assert.That(bootCameOver.Subprotocol,     Is.EqualTo(AgreedOn));
                Assert.That(((ISshMessageConnection) bootCameOver).Username, Is.EqualTo("GD-SSH-001"));

                // The station's route to its CSMS is the SSH client.
                Assert.That(chargingStation.Routing.LookupNetworkingNode(NetworkingNode_Id.CSMS, out var toCSMS), Is.True);
                Assert.That(toCSMS!.Client,               Is.InstanceOf<OCPPSshClient>());

            }
            finally
            {
                testCSMS1.OCPP.IN.OnBootNotificationRequestReceived -= seeBoot;
            }

        }

        #endregion

        #region TheCSMSAsksTheStation()

        /// <summary>
        /// A request from the CSMS reaches the station over its SSH connection, and is answered.
        /// </summary>
        [Test]
        public async Task TheCSMSAsksTheStation()
        {

            var chargingStation  = await ConnectedStation("GD-SSH-002");

            var state            = await GetLocalListVersion(chargingStation.Id);

            Assert.That(state.JSONResponse, Is.Not.Null, $"The station was not reached: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

            Assert.That(testCSMS1!.Routing.LookupNetworkingNode(chargingStation.Id, out var toStation), Is.True);
            Assert.That(toStation!.Server, Is.SameAs(sshServer));

        }

        #endregion

        #region BinaryMessagesBothWays()

        /// <summary>
        /// Binary data transfers, from the station and to it.
        /// </summary>
        [Test]
        public async Task BinaryMessagesBothWays()
        {

            var chargingStation  = await ConnectedStation("GD-SSH-003");

            var data             = "Hello world!".ToUTF8Bytes();

            var fromStation      = await chargingStation.TransferBinaryData(
                                             VendorId:             Vendor_Id. GraphDefined,
                                             MessageId:            Message_Id.GraphDefined_TestMessage,
                                             Data:                 data,
                                             SerializationFormat:  SerializationFormats.BinaryTextIds
                                         );

            var toStation        = await testCSMS1!.TransferBinaryData(
                                             Destination:          SourceRouting.To(chargingStation.Id),
                                             VendorId:             Vendor_Id. GraphDefined,
                                             MessageId:            Message_Id.GraphDefined_TestMessage,
                                             Data:                 data
                                         );

            Assert.That(fromStation.Result.ResultCode,  Is.EqualTo(ResultCode.OK), fromStation.Result.ToString());
            Assert.That(fromStation.Status,             Is.EqualTo(BinaryDataTransferStatus.Accepted));
            Assert.That(fromStation.Data?.ToUTF8String(), Is.EqualTo(data.Reverse().ToUTF8String()));

            Assert.That(toStation.  Result.ResultCode,  Is.EqualTo(ResultCode.OK), toStation.Result.ToString());
            Assert.That(toStation.  Status,             Is.EqualTo(BinaryDataTransferStatus.Accepted));
            Assert.That(toStation.  Data?.ToUTF8String(), Is.EqualTo(data.Reverse().ToUTF8String()));

        }

        #endregion

        #region AStationComesBack()

        /// <summary>
        /// A station whose connection went - the CSMS's SSH subsystem told everybody it was going away - comes
        /// back as its reconnect policy says, and is reached again.
        /// </summary>
        [Test]
        public async Task AStationComesBack()
        {

            var chargingStation = NewStation("GD-SSH-004");

            chargingStation.ReconnectPolicy = new WebSocketClientReconnectPolicy(
                                                  InitialDelay:  TimeSpan.FromMilliseconds(100),
                                                  MaxDelay:      TimeSpan.FromMilliseconds(200)
                                              );

            var result      = await chargingStation.ConnectOCPPClient(SshURL("GD-SSH-004"), Subprotocols: Offered, Ssh: Login("GD-SSH-004"));

            Assert.That(result.IsConnected, Is.True, result.ToString());

            var sshClient   = (OCPPSshClient) result.Client!;
            var first       = sshClient.Connection;

            await Until(() => sshServer!.ConnectedNetworkingNodeIds.Contains(chargingStation.Id), "Registering");

            await sshServer!.Stop("Going away for a moment.");

            await Until(() => sshClient.Connection is not null && !ReferenceEquals(sshClient.Connection, first) && sshClient.IsConnected, "The station's reconnect");
            await Until(() => sshServer.GetConnectionsFor(chargingStation.Id).Any(), "Registering the station again");

            var state = await GetLocalListVersion(chargingStation.Id);

            Assert.That(state.JSONResponse, Is.Not.Null, $"The station was not reached again: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

        }

        #endregion

        #region AnOverlayStationFindsTheWayBack()

        /// <summary>
        /// A station on an overlay network, connected over SSH: the CSMS writes its requests with their
        /// network path, as the station's HELLO asked, and the station answers the CSMS by the name in that
        /// path - "OCPPTest01", which it knows only as "CSMS" until a request from it arrives over its SSH
        /// connection, and learns to reach that way.
        /// </summary>
        [Test]
        public async Task AnOverlayStationFindsTheWayBack()
        {

            var chargingStation  = NewStation("GD-SSH-008");
            var result           = await chargingStation.ConnectOCPPClient(SshURL("GD-SSH-008"), NetworkingMode: NetworkingMode.OverlayNetwork, Subprotocols: Offered, Ssh: Login("GD-SSH-008"));

            Assert.That(result.IsConnected, Is.True, result.ToString());

            await Until(() => sshServer!.ConnectedNetworkingNodeIds.Contains(chargingStation.Id), "Registering");

            Assert.That(sshServer!.GetConnectionsFor(chargingStation.Id).Single().TryGetCustomDataAs<NetworkingMode>(WebSocketKeys.NetworkingMode),
                        Is.EqualTo(NetworkingMode.OverlayNetwork));

            Assert.That(chargingStation.Routing.LookupNetworkingNode(testCSMS1!.Id, out _), Is.False,
                        "The station knew the CSMS by its own name before it heard from it.");

            var state = await GetLocalListVersion(chargingStation.Id);

            Assert.That(state.JSONResponse, Is.Not.Null, $"The answer did not come back: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

            Assert.That(chargingStation.Routing.LookupNetworkingNode(testCSMS1.Id, out var wayBack), Is.True);
            Assert.That(wayBack!.Client, Is.InstanceOf<OCPPSshClient>());

        }

        #endregion

        #region ALocalControllerConnectsAsItsStation()

        /// <summary>
        /// A local controller's login, allowed to, connects as one of its stations - the station named by the
        /// URL's path, as with a WebSocket - and the CSMS knows it as that station.
        /// </summary>
        [Test]
        public async Task ALocalControllerConnectsAsItsStation()
        {

            var stationId = NetworkingNode_Id.Parse("GD-SSH-005");

            sshServer!.AllowToActFor(NetworkingNode_Id.Parse("LC-SSH-01"), [ stationId ]);

            try
            {

                var chargingStation  = NewStation(stationId.ToString());
                var result           = await chargingStation.ConnectOCPPClient(SshURL("LC-SSH-01", ConnectAs: stationId.ToString()), Subprotocols: Offered, Ssh: Login("LC-SSH-01"));

                Assert.That(result.IsConnected, Is.True, result.ToString());

                await Until(() => sshServer.ConnectedNetworkingNodeIds.Contains(stationId), "Registering the station");

                var boot = await chargingStation.SendBootNotification(BootReason: BootReason.PowerUp);

                Assert.That(boot.Status, Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That((await GetLocalListVersion(stationId)).JSONResponse, Is.Not.Null);

            }
            finally
            {
                sshServer.DisallowToActFor(NetworkingNode_Id.Parse("LC-SSH-01"), [ stationId ]);
            }

        }

        #endregion

        #region WhatIsNotConnectedSaysWhy()

        /// <summary>
        /// A login the CSMS does not take is no connection, and says why; an ssh:// URL without a login, or a
        /// URL of no transport, is the caller's mistake.
        /// </summary>
        [Test]
        public async Task WhatIsNotConnectedSaysWhy()
        {

            var chargingStation  = NewStation("GD-SSH-006");

            var stranger         = await chargingStation.ConnectOCPPClient(SshURL("GD-SSH-006"), Subprotocols: Offered, Ssh: Login("GD-SSH-006", strangerKey));

            Assert.That(stranger.IsConnected,  Is.False);
            Assert.That(stranger.Transport,    Is.EqualTo(MessageTransport.SSH));
            Assert.That(stranger.Error,        Is.Not.Null);
            Assert.That(stranger.Client,       Is.InstanceOf<OCPPSshClient>());
            Assert.That(sshServer!.ConnectedNetworkingNodeIds, Does.Not.Contain(chargingStation.Id));

            await Assert.ThrowsAsync<ArgumentException>(() => chargingStation.ConnectOCPPClient(SshURL("GD-SSH-006")));
            await Assert.ThrowsAsync<ArgumentException>(() => chargingStation.ConnectOCPPClient(URL.Parse("ftp://127.0.0.1/GD-SSH-006")));

            // The URL's login and the SSH login's must not differ.
            await Assert.ThrowsAsync<ArgumentException>(() => chargingStation.ConnectOCPPClient(SshURL("GD-SSH-006"), Subprotocols: Offered, Ssh: Login("somebody-else")));

        }

        #endregion

        #region (class) LoopbackDNS

        /// <summary>
        /// A DNS client that answers every name with 127.0.0.1, and counts what it was asked.
        /// </summary>
        private sealed class LoopbackDNS : IDNSClient
        {

            private Int32 asked;

            public Int32 Asked
                => Volatile.Read(ref asked);

            public Task<DNSInfo> Query(DomainName                           DomainName,
                                       IEnumerable<DNSResourceRecordTypes>  ResourceRecordTypes,
                                       TimeSpan?                            Timeout             = null,
                                       Boolean?                             RecursionDesired    = true,
                                       Boolean?                             ForceUpdate         = false,
                                       CancellationToken                    CancellationToken   = default)

                => Query(DNSServiceName.Parse(DomainName.FullName), ResourceRecordTypes, Timeout, RecursionDesired, ForceUpdate, CancellationToken);

            public Task<DNSInfo> Query(DNSServiceName                       DNSServiceName,
                                       IEnumerable<DNSResourceRecordTypes>  ResourceRecordTypes,
                                       TimeSpan?                            Timeout             = null,
                                       Boolean?                             RecursionDesired    = true,
                                       Boolean?                             ForceUpdate         = false,
                                       CancellationToken                    CancellationToken   = default)
            {

                Interlocked.Increment(ref asked);

                return Task.FromResult(new DNSInfo(
                           new DNSServerConfig(IPv4Address.Localhost, IPPort.Parse(53)),
                           QueryId:                0,
                           IsAuthoritativeAnswer:  true,
                           IsTruncated:            false,
                           RecursionDesired:       true,
                           RecursionAvailable:     true,
                           ResponseCode:           DNSResponseCodes.NoError,
                           Answers:                ResourceRecordTypes.Contains(DNSResourceRecordTypes.A)
                                                       ? [ new A(DNSServiceName, DNSQueryClasses.IN, TimeSpan.FromMinutes(1), IPv4Address.Localhost) ]
                                                       : [],
                           Authorities:            [],
                           AdditionalRecords:      [],
                           IsValid:                true,
                           IsTimeout:              false,
                           Timeout:                TimeSpan.FromSeconds(5),
                           Runtime:                TimeSpan.Zero
                       ));

            }

            public void Dispose()
            { }

            public ValueTask DisposeAsync()
                => ValueTask.CompletedTask;

            public override String ToString()
                => nameof(LoopbackDNS);

        }

        #endregion

        #region AHostNameIsResolvedWithTheDNSClientGiven()

        /// <summary>
        /// The host name of an ssh:// URL is resolved with the DNS client handed to ConnectOCPPClient - a name
        /// the resolver of the operating system does not know - as it is over a WebSocket.
        /// </summary>
        [Test]
        public async Task AHostNameIsResolvedWithTheDNSClientGiven()
        {

            var chargingStation  = NewStation("GD-SSH-008");
            var dns              = new LoopbackDNS();

            // .test is a name no resolver of an operating system answers, and one a name resolution library
            // hands to its DNS client as any other (RFC 6761 §6.2) - unlike .invalid, which it answers itself
            // with "no such name" and asks no DNS client for (§6.4).
            var result           = await chargingStation.ConnectOCPPClient(
                                             URL.Parse($"ssh://GD-SSH-008@csms.test:{listener!.LocalEndPoint.Port}"),
                                             Ssh:        Login("GD-SSH-008"),
                                             DNSClient:  dns
                                         );

            Assert.That(result.IsConnected,  Is.True, result.ToString());
            Assert.That(dns.Asked,           Is.GreaterThan(0), "The DNS client given was not asked.");

        }

        #endregion

        #region ASignatureFormat_IsSaidOfAnSSHConnection()

        /// <summary>
        /// Which signatures a message gets is said of an ssh:// connection as of a
        /// WebSocket: of its client, and of the node at its far end. Two charging
        /// stations of one policy, the connection of the first says COSE, the node at
        /// the far end of the second's both - and a CSMS that answers the first in COSE.
        /// </summary>
        [Test]
        public async Task ASignatureFormat_IsSaidOfAnSSHConnection()
        {

            var first    = NewStation("GD-SSH-011");
            var result   = await first.ConnectOCPPClient(SshURL("GD-SSH-011"), Subprotocols: Offered, Ssh: Login("GD-SSH-011"));

            Assert.That(result.IsConnected,  Is.True, result.ToString());
            Assert.That(result.Client,       Is.Not.Null);

            await Until(() => sshServer!.ConnectedNetworkingNodeIds.Contains(first.Id), "Registering 'GD-SSH-011'");

            static String FormatsOf(IEnumerable<Signature> Signatures)
                => String.Join("+", Signatures.Select(signature => signature.IsCOSE ? "COSE" : "JWS").Order());

            var received = new ConcurrentList<BootNotificationRequest>();

            OnBootNotificationRequestReceivedDelegate seeBoot = (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnBootNotificationRequestReceived += seeBoot;

            try
            {

                testCSMS1.OCPP.SignaturePolicy.AddSigningRule(OCPPv2_1.CSMS.BootNotificationResponse.DefaultJSONLDContext, ECCKeyPair.GenerateKeys()!);

                // Of the client the station connected with, and of the station by its name.
                first.    OCPP.SetSignatureFormats(result.Client!, SignatureFormats.COSE);
                testCSMS1.OCPP.SetSignatureFormats(first.Id,       SignatureFormats.COSE);

                Assert.That(first.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.EqualTo(SignatureFormats.COSE),
                            "What was said of the SSH client is not what its route to the CSMS says.");

                var boot1 = await first.SendBootNotification(
                                      BootReason:  BootReason.PowerUp,
                                      SignInfos:   [ ECCKeyPair.GenerateKeys()!.ToSignInfo1("ahzf", I18NString.Create("Just a test!"), Timestamp.Now) ]
                                  );

                // Saying nothing again is what the policy says.
                first.    OCPP.SetSignatureFormats(result.Client!, null);
                testCSMS1.OCPP.SetSignatureFormats(first.Id,       null);

                var boot2 = await first.SendBootNotification(
                                      BootReason:  BootReason.PowerUp,
                                      SignInfos:   [ ECCKeyPair.GenerateKeys()!.ToSignInfo1("ahzf", I18NString.Create("Just a test!"), Timestamp.Now) ]
                                  );

                Assert.Multiple(() => {

                    Assert.That(boot1.Result.ResultCode,                              Is.EqualTo(ResultCode.OK), boot1.Result.ToString());
                    Assert.That(boot2.Result.ResultCode,                              Is.EqualTo(ResultCode.OK), boot2.Result.ToString());

                    Assert.That(received.Select(request => FormatsOf(request.Signatures)),  Is.EqualTo(new[] { "COSE", "JWS" }));

                    Assert.That(FormatsOf(boot1.Signatures),                          Is.EqualTo("COSE"));
                    Assert.That(FormatsOf(boot2.Signatures),                          Is.EqualTo("JWS"));

                    Assert.That(first.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.Null);

                });

            }
            finally
            {
                testCSMS1.OCPP.IN.OnBootNotificationRequestReceived -= seeBoot;
                testCSMS1.OCPP.SetSignatureFormats(first.Id, null);
            }

        }

        #endregion

        #region ConnectOCPPClientOverAWebSocket()

        /// <summary>
        /// The same entry connects over an HTTP WebSocket, by a ws:// or http:// URL - and says so.
        /// </summary>
        [Test]
        public async Task ConnectOCPPClientOverAWebSocket()
        {

            var chargingStation = NewStation("GD-SSH-007");

            testCSMS1!.AddOrUpdateHTTPBasicAuth(chargingStation.Id, "1234abcd");

            var result = await chargingStation.ConnectOCPPClient(
                                   URL.Parse($"ws://127.0.0.1:{testBackendWebSockets1!.IPPort}/{chargingStation.Id}"),
                                   NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                   HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), "1234abcd")
                               );

            Assert.That(result.IsConnected,                  Is.True, result.ToString());
            Assert.That(result.Transport,                    Is.EqualTo(MessageTransport.WebSocket));
            Assert.That(result.HTTPResponse?.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));
            Assert.That(result.Client,                       Is.InstanceOf<OCPPWebSocketClient>());
            Assert.That(result.Connection,                   Is.InstanceOf<IWebSocketConnection>());

            var boot = await chargingStation.SendBootNotification(BootReason: BootReason.PowerUp);

            Assert.That(boot.Status, Is.EqualTo(RegistrationStatus.Accepted));

        }

        #endregion

    }

}
