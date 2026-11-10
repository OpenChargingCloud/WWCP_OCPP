/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Server;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.SSH
{

    /// <summary>
    /// The messages of the white paper "OCPP over SSH" (section 9.3): SignSSHKey and SSHKeySigned between a
    /// charging station and its CSMS, InstallSSHKey, DeleteSSHKey and GetInstalledSSHKeys from a CSMS to a
    /// Local Controller - as JSON, and over a connection. And the request gate of a node, which keeps a
    /// request from being processed and says why.
    /// </summary>
    [TestFixture]
    public class SSHKeyMessagesTests : ACSMSTests
    {

        #region Data

        private const String PublicKey  = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAINgMp5THIxMsb4dglcgSkZ9A2oWPpBrzZxM3Rk8UqWwC cs001";

        private readonly ISshHostKey      hostKey     = SshHostKey.GenerateEd25519();
        private readonly ISshHostKey      stationKey  = SshHostKey.GenerateEd25519();

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

        private async Task<TestChargingStationNode> ConnectedStation(String Id)
        {

            station = new TestChargingStationNode(
                          Id:                     NetworkingNode_Id.Parse(Id),
                          VendorName:             "GraphDefined OEM #1",
                          Model:                  "VCP.1",
                          DisableSendHeartbeats:  true,
                          HTTPAPI_Disabled:       true,
                          WebAPI_Disabled:        true,
                          DNSClient:              testCSMS1!.DNSClient
                      );

            var result = await station.ConnectOCPPClient(URL.Parse($"ssh://{Id}@127.0.0.1:{listener!.LocalEndPoint.Port}"),
                                                         Ssh: new SshClientOptions {
                                                                  Username          = Id,
                                                                  VerifyHostKey     = blob => blob.AsSpan().SequenceEqual(hostKey.PublicKeyBlob),
                                                                  Credentials       = [ stationKey ],
                                                                  RequireStrictKex  = true
                                                              });

            Assert.That(result.IsConnected, Is.True, result.ToString());

            var until = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);
            while (!sshServer!.ConnectedNetworkingNodeIds.Contains(station.Id))
            {
                if (DateTimeOffset.UtcNow > until)
                    Assert.Fail($"Registering '{Id}' did not happen within 10 s.");
                await Task.Delay(10);
            }

            return station;

        }

        private static readonly NetworkPath  Path         = NetworkPath.From(NetworkingNode_Id.Parse("csms"));
        private static readonly Request_Id   RequestId    = Request_Id.Parse("1");

        #endregion


        #region TheRequestsAndResponsesGoThroughJSONUnchanged()

        /// <summary>
        /// Every request and response of the five, written as JSON and read again, is what it was; a purpose
        /// that is none, and a request without what it must have, are refused.
        /// </summary>
        [Test]
        public void TheRequestsAndResponsesGoThroughJSONUnchanged()
        {

            var to             = SourceRouting.To(NetworkingNode_Id.Parse("lc01"));
            var cs001          = NetworkingNode_Id.Parse("cs001");

            var signSSHKey     = new SignSSHKeyRequest         (to, SSHKeyPurpose.OCPP, PublicKey);
            var sshKeySigned   = new SSHKeySignedRequest       (to, SSHKeyPurpose.NTS,  PublicKey, Certificate: "ssh-ed25519-cert-v01@openssh.com AAAA");
            var installSSHKey  = new InstallSSHKeyRequest      (to, cs001, SSHKeyPurpose.SFTP, PublicKey);
            var deleteSSHKey   = new DeleteSSHKeyRequest       (to, cs001, "SHA256:hOEj1ea61PSArpO3/qj2Drlp024D2eiJ7DJQNNLl+hU");
            var getAll         = new GetInstalledSSHKeysRequest(to);
            var getSome        = new GetInstalledSSHKeysRequest(to, cs001, SSHKeyPurpose.OCPP);

            Assert.Multiple(() => {

                Assert.That(SignSSHKeyRequest.         Parse(signSSHKey.   ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(signSSHKey.ToJSON().ToString()));
                Assert.That(SSHKeySignedRequest.       Parse(sshKeySigned. ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(sshKeySigned.ToJSON().ToString()));
                Assert.That(InstallSSHKeyRequest.      Parse(installSSHKey.ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(installSSHKey.ToJSON().ToString()));
                Assert.That(DeleteSSHKeyRequest.       Parse(deleteSSHKey. ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(deleteSSHKey.ToJSON().ToString()));
                Assert.That(GetInstalledSSHKeysRequest.Parse(getAll.       ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(getAll.ToJSON().ToString()));
                Assert.That(GetInstalledSSHKeysRequest.Parse(getSome.      ToJSON(), RequestId, to, Path).ToJSON().ToString(), Is.EqualTo(getSome.ToJSON().ToString()));

                Assert.That(installSSHKey.ToJSON().ToString(Newtonsoft.Json.Formatting.None),
                            Is.EqualTo($$"""{"networkingNodeId":"cs001","purpose":"SFTP","publicKey":"{{PublicKey}}"}"""));

                Assert.That(getAll.ToJSON().ToString(Newtonsoft.Json.Formatting.None), Is.EqualTo("{}"));

                Assert.That(SignSSHKeyRequest.TryParse(JObject.Parse($$"""{ "purpose": "Shell", "publicKey": "{{PublicKey}}" }"""),
                                                       RequestId, to, Path, out _, out var noPurpose), Is.False);
                Assert.That(noPurpose, Does.Contain("purpose"));

                Assert.That(InstallSSHKeyRequest.TryParse(JObject.Parse("""{ "networkingNodeId": "cs001", "purpose": "OCPP" }"""),
                                                          RequestId, to, Path, out _, out var noKey), Is.False);
                Assert.That(noKey, Does.Contain("publicKey"));

            });

            var installed = new InstallSSHKeyResponse(installSSHKey, GenericStatus.Accepted, HostKeys: [ "SHA256:host1", "SHA256:host2" ]);
            var listed    = new GetInstalledSSHKeysResponse(getAll, GenericStatus.Accepted,
                                                            Keys:      [ new InstalledSSHKey(cs001, SSHKeyPurpose.OCPP, PublicKey, "SHA256:key1") ],
                                                            HostKeys:  [ "SHA256:host1" ]);
            var refused   = new DeleteSSHKeyResponse(deleteSSHKey, GenericStatus.Rejected, new StatusInfo("NotFound", "No such key of cs001."));

            Assert.Multiple(() => {

                Assert.That(InstallSSHKeyResponse.      Parse(installSSHKey, installed.ToJSON(), to, Path).HostKeys, Is.EqualTo(new[] { "SHA256:host1", "SHA256:host2" }));
                Assert.That(GetInstalledSSHKeysResponse.Parse(getAll,        listed.   ToJSON(), to, Path).ToJSON().ToString(), Is.EqualTo(listed.ToJSON().ToString()));
                Assert.That(DeleteSSHKeyResponse.       Parse(deleteSSHKey,  refused.  ToJSON(), to, Path).StatusInfo?.ReasonCode, Is.EqualTo("NotFound"));
                Assert.That(listed.ToJSON()["keys"]![0]!.Value<String>("keyFingerprint"), Is.EqualTo("SHA256:key1"));

            });

        }

        #endregion

        #region AStationAndItsCSMSRotateAKeyOverSSH()

        /// <summary>
        /// A station sends SignSSHKey to its CSMS over SSH and is answered; the CSMS confirms the key with
        /// SSHKeySigned, and the station is asked.
        /// </summary>
        [Test]
        public async Task AStationAndItsCSMSRotateAKeyOverSSH()
        {

            var chargingStation = await ConnectedStation("GD-SSHKEY-001");

            SignSSHKeyRequest?    signed     = null;
            SSHKeySignedRequest?  confirmed  = null;

            OnSignSSHKeyDelegate atTheCSMS = (timestamp, sender, connection, request, ct) => {
                signed = request;
                return Task.FromResult(new SignSSHKeyResponse(request, GenericStatus.Accepted));
            };

            OnSSHKeySignedDelegate atTheStation = (timestamp, sender, connection, request, ct) => {
                confirmed = request;
                return Task.FromResult(new SSHKeySignedResponse(request, GenericStatus.Accepted));
            };

            testCSMS1!.OCPP.IN.OnSignSSHKey       += atTheCSMS;
            chargingStation.OCPP.IN.OnSSHKeySigned  += atTheStation;

            try
            {

                var signResponse   = await chargingStation.OCPP.OUT.SignSSHKey(new SignSSHKeyRequest(SourceRouting.To(NetworkingNode_Id.CSMS),
                                                                                                     SSHKeyPurpose.OCPP,
                                                                                                     PublicKey));

                var signedResponse = await testCSMS1.OCPP.OUT.SSHKeySigned(new SSHKeySignedRequest(SourceRouting.To(chargingStation.Id),
                                                                                                   SSHKeyPurpose.OCPP,
                                                                                                   PublicKey));

                Assert.Multiple(() => {
                    Assert.That(signResponse.Result.ResultCode,    Is.EqualTo(ResultCode.OK), signResponse.Result.ToString());
                    Assert.That(signResponse.Status,               Is.EqualTo(GenericStatus.Accepted));
                    Assert.That(signed?.PublicKey,                 Is.EqualTo(PublicKey));
                    Assert.That(signed?.Purpose,                   Is.EqualTo(SSHKeyPurpose.OCPP));
                    Assert.That(signedResponse.Result.ResultCode,  Is.EqualTo(ResultCode.OK), signedResponse.Result.ToString());
                    Assert.That(signedResponse.Status,             Is.EqualTo(GenericStatus.Accepted));
                    Assert.That(confirmed?.PublicKey,              Is.EqualTo(PublicKey));
                });

            }
            finally
            {
                testCSMS1.OCPP.IN.OnSignSSHKey       -= atTheCSMS;
                chargingStation.OCPP.IN.OnSSHKeySigned  -= atTheStation;
            }

        }

        #endregion

        #region ACSMSInstallsListsAndDeletesKeysAtANode()

        /// <summary>
        /// InstallSSHKey, GetInstalledSSHKeys and DeleteSSHKey from a CSMS to a node, here a station standing
        /// in for a Local Controller, with the host keys the installing answer names.
        /// </summary>
        [Test]
        public async Task ACSMSInstallsListsAndDeletesKeysAtANode()
        {

            var node   = await ConnectedStation("GD-SSHKEY-LC");
            var keys   = new List<InstalledSSHKey>();
            var cs001  = NetworkingNode_Id.Parse("cs001");

            node.OCPP.IN.OnInstallSSHKey       += (timestamp, sender, connection, request, ct) => {
                keys.Add(new InstalledSSHKey(request.NetworkingNodeId, request.Purpose, request.PublicKey, "SHA256:key1"));
                return Task.FromResult(new InstallSSHKeyResponse(request, GenericStatus.Accepted, HostKeys: [ "SHA256:lcHostKey" ]));
            };

            node.OCPP.IN.OnGetInstalledSSHKeys += (timestamp, sender, connection, request, ct) =>
                Task.FromResult(new GetInstalledSSHKeysResponse(request, GenericStatus.Accepted,
                                                                Keys:      keys.Where(key => request.NetworkingNodeId is null || key.NetworkingNodeId == request.NetworkingNodeId),
                                                                HostKeys:  [ "SHA256:lcHostKey" ]));

            node.OCPP.IN.OnDeleteSSHKey        += (timestamp, sender, connection, request, ct) =>
                Task.FromResult(keys.RemoveAll(key => key.KeyFingerprint == request.KeyFingerprint) > 0
                                    ? new DeleteSSHKeyResponse(request, GenericStatus.Accepted)
                                    : new DeleteSSHKeyResponse(request, GenericStatus.Rejected, new StatusInfo("NotFound")));

            var to         = SourceRouting.To(node.Id);
            var installed  = await testCSMS1!.OCPP.OUT.InstallSSHKey      (new InstallSSHKeyRequest      (to, cs001, SSHKeyPurpose.OCPP, PublicKey));
            var listed     = await testCSMS1.  OCPP.OUT.GetInstalledSSHKeys(new GetInstalledSSHKeysRequest(to, cs001));
            var deleted    = await testCSMS1.  OCPP.OUT.DeleteSSHKey       (new DeleteSSHKeyRequest       (to, cs001, "SHA256:key1"));
            var again      = await testCSMS1.  OCPP.OUT.DeleteSSHKey       (new DeleteSSHKeyRequest       (to, cs001, "SHA256:key1"));

            Assert.Multiple(() => {
                Assert.That(installed.Status,                   Is.EqualTo(GenericStatus.Accepted), installed.Result.ToString());
                Assert.That(installed.HostKeys,                 Is.EqualTo(new[] { "SHA256:lcHostKey" }));
                Assert.That(listed.Keys.Select(key => key.PublicKey), Is.EqualTo(new[] { PublicKey }));
                Assert.That(listed.HostKeys,                    Is.EqualTo(new[] { "SHA256:lcHostKey" }));
                Assert.That(deleted.Status,                     Is.EqualTo(GenericStatus.Accepted));
                Assert.That(again.Status,                       Is.EqualTo(GenericStatus.Rejected));
                Assert.That(again.StatusInfo?.ReasonCode,       Is.EqualTo("NotFound"));
            });

        }

        #endregion

        #region AGateKeepsARequestFromBeingProcessedAndSaysWhy()

        /// <summary>
        /// A node whose request gate says no answers with a SecurityError that carries the gate's sentence,
        /// and processes nothing; what the gate lets through is processed as before.
        /// </summary>
        [Test]
        public async Task AGateKeepsARequestFromBeingProcessedAndSaysWhy()
        {

            var chargingStation = await ConnectedStation("GD-SSHKEY-GATE");

            chargingStation.OCPP.IN.RequestGate = (connection, action, payload) =>
                action == "Reset" ? "This station is on probation." : null;

            var reset = await testCSMS1!.OCPP.OUT.SendJSONRequestAndWait(
                            new OCPP_JSONRequestMessage(
                                RequestTimestamp:  Timestamp.Now,
                                EventTrackingId:   EventTracking_Id.New,
                                NetworkingMode:    NetworkingMode.Standard,
                                Destination:       SourceRouting.To(chargingStation.Id),
                                NetworkPath:       NetworkPath.From(testCSMS1.Id),
                                RequestId:         Request_Id.NewRandom(),
                                Action:            "Reset",
                                Payload:           new JObject(new JProperty("type", "Immediate")),
                                RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(5)
                            )
                        );

            var list  = await testCSMS1.OCPP.OUT.SendJSONRequestAndWait(
                            new OCPP_JSONRequestMessage(
                                RequestTimestamp:  Timestamp.Now,
                                EventTrackingId:   EventTracking_Id.New,
                                NetworkingMode:    NetworkingMode.Standard,
                                Destination:       SourceRouting.To(chargingStation.Id),
                                NetworkPath:       NetworkPath.From(testCSMS1.Id),
                                RequestId:         Request_Id.NewRandom(),
                                Action:            "GetLocalListVersion",
                                Payload:           new JObject(),
                                RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(5)
                            )
                        );

            Assert.Multiple(() => {
                Assert.That(reset.JSONResponse,                               Is.Null, "A request the gate kept was processed.");
                Assert.That(reset.JSONRequestErrorMessage?.ErrorCode,         Is.EqualTo(ResultCode.SecurityError));
                Assert.That(reset.JSONRequestErrorMessage?.ErrorDescription,  Is.EqualTo("This station is on probation."));
                Assert.That(list.JSONResponse,                                Is.Not.Null, "A request the gate let through was not processed.");
            });

        }

        #endregion

    }

}
