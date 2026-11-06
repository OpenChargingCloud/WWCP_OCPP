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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Server;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.SSH
{

    /// <summary>
    /// OCPP 1.6 over the SSH subsystem "ocpp@open.charging.cloud": a central system serving it beside its
    /// WebSocket server, and a charge point connecting by an "ssh://" URL.
    /// </summary>
    [TestFixture]
    public class OCPPOverSSHTests : ACentralSystemTests
    {

        #region Data

        private readonly ISshHostKey  hostKey      = SshHostKey.GenerateEd25519();
        private readonly ISshHostKey  chargerKey   = SshHostKey.GenerateEd25519();

        private OCPPSshServer?        sshServer;
        private SshServer?            listener;
        private TestChargePointNode?  chargePoint;

        #endregion

        #region SetupEachTest() / ShutdownEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            sshServer  = testCentralSystem01!.AttachOCPPSshSubsystem();

            listener   = new SshServer(new SshServerOptions {
                             HostKeys       = [ hostKey ],
                             Authenticator  = SshUserAuthenticator.ForAuthorizedKeys(chargerKey.PublicKeyBlob),
                             Subsystems     = (session, name, ct) => ValueTask.FromResult(name == OCPPSshServer.OCPPSubsystem ? sshServer.Handler : null)
                         });

            listener.StartAsync(new IPSocket(IPv4Address.Localhost, IPPort.Auto), CancellationToken.None).GetAwaiter().GetResult();

        }

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            if (chargePoint is not null)
                await chargePoint.Stop();

            chargePoint = null;

            if (listener is not null)
                await listener.DisposeAsync();

            listener = null;

            await base.ShutdownEachTest();

        }

        #endregion

        #region (private) ConnectedChargePoint(Id)

        /// <summary>
        /// A charge point connected over SSH, and registered by the central system.
        /// </summary>
        private async Task<TestChargePointNode> ConnectedChargePoint(String Id)
        {

            chargePoint = new TestChargePointNode(
                              ChargeBoxId:            NetworkingNode_Id.Parse(Id),
                              Connectors:             [ new ConnectorSpec(Availabilities.Operative, MaxPower: Watt.FromKW(22)) ],
                              ChargePointVendor:      "GraphDefined OEM #1",
                              ChargePointModel:       "VCP.1",
                              DisableSendHeartbeats:  true,
                              DNSClient:              testCentralSystem01!.DNSClient
                          );

            var result = await chargePoint.ConnectOCPPClient(
                                   URL.Parse($"ssh://{Id}@127.0.0.1:{listener!.LocalEndPoint.Port}"),
                                   Ssh: new SshClientOptions {
                                            Username       = Id,
                                            VerifyHostKey  = blob => blob.AsSpan().SequenceEqual(hostKey.PublicKeyBlob),
                                            Credentials    = [ chargerKey ]
                                        }
                               );

            Assert.That(result.IsConnected, Is.True, result.ToString());

            var until = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);
            while (!sshServer!.ConnectedNetworkingNodeIds.Contains(chargePoint.Id))
            {
                if (DateTimeOffset.UtcNow > until)
                    Assert.Fail($"'{Id}' was not registered within 10 s.");
                await Task.Delay(10);
            }

            return chargePoint;

        }

        #endregion


        #region BootAndHeartbeat()

        /// <summary>
        /// A charge point boots and sends a heartbeat over SSH, speaking OCPP 1.6 to its central system -
        /// "CentralSystem" to it, as over a WebSocket.
        /// </summary>
        [Test]
        public async Task BootAndHeartbeat()
        {

            var chargePoint  = await ConnectedChargePoint("GD-SSH16-1");

            var boot         = await chargePoint.SendBootNotification();
            var heartbeat    = await chargePoint.SendHeartbeat();

            Assert.That(boot.Status,                                   Is.EqualTo(RegistrationStatus.Accepted));
            Assert.That(heartbeat.Result.ResultCode,                   Is.EqualTo(ResultCode.OK), heartbeat.Result.ToString());

            Assert.That(sshServer!.GetConnectionsFor(chargePoint.Id).Single().Subprotocol, Is.EqualTo(Version.WebSocketSubProtocolId));

            Assert.That(chargePoint.Routing.LookupNetworkingNode(NetworkingNode_Id.CentralSystem, out var toCentralSystem), Is.True);
            Assert.That(toCentralSystem!.Client,                       Is.InstanceOf<OCPPSshClient>());
            Assert.That(toCentralSystem.Client!.Transport,             Is.EqualTo(MessageTransport.SSH));

        }

        #endregion

        #region TheCentralSystemAsksTheChargePoint()

        /// <summary>
        /// A request from the central system reaches the charge point over SSH, and is answered.
        /// </summary>
        [Test]
        public async Task TheCentralSystemAsksTheChargePoint()
        {

            var chargePoint  = await ConnectedChargePoint("GD-SSH16-2");

            var state        = await testCentralSystem01!.OCPP.OUT.SendJSONRequestAndWait(
                                         new OCPP_JSONRequestMessage(
                                             RequestTimestamp:   Timestamp.Now,
                                             EventTrackingId:    EventTracking_Id.New,
                                             NetworkingMode:     NetworkingMode.Standard,
                                             Destination:        SourceRouting.To(chargePoint.Id),
                                             NetworkPath:        NetworkPath.From(testCentralSystem01.Id),
                                             RequestId:          Request_Id.NewRandom(),
                                             Action:             "GetLocalListVersion",
                                             Payload:            new JObject(),
                                             RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                                         )
                                     );

            Assert.That(state.JSONResponse, Is.Not.Null, $"The charge point was not reached: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

        }

        #endregion

    }

}
