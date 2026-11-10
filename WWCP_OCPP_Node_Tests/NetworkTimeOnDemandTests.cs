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

using System.Net;
using System.Net.Sockets;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.TestKit;

using cloud.charging.open.protocols.OCPP.Node.TestKit;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// Section 3.2.7 of the white paper "Secure Time Synchronization", from the
    /// request of the CSMS to what the station tells it: SyncNetworkTime runs a
    /// round of the group, TestTimeServer asks one client step by step, both
    /// are answered at once and told afterwards - and refused with a reason
    /// where nothing could be asked.
    /// </summary>
    /// <remarks>
    /// Against a server of Norn's own with an SSH listener of its own, on the
    /// loopback interface, addressed by its address: no name to resolve. The
    /// requests are handed to the processors the bridge gave the station's
    /// OCPP 2.1 node, as its adapter hands them over; what is told is taken
    /// where the bridge tells it, since the station dials no CSMS.
    /// </remarks>
    [TestFixture]
    [NonParallelizable]
    public class NetworkTimeOnDemandTests
    {

        #region Data

        private static readonly IPPort             tlsPort  = FindFreeTCPPort();
        private static readonly IPPort             sshPort  = FindFreeTCPPort();
        private static readonly IPPort             ntpPort  = FindFreeUDPPort();
        private static readonly NetworkingNode_Id  csms     = NetworkingNode_Id.Parse("csms01");

        private String         directory  = "";
        private IBridgedNode?  bridged;
        private NTSServer?     server;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory = Path.Combine(Path.GetTempPath(), "nts-on-demand-" + Guid.NewGuid().ToString("N")[..12]);
            Directory.CreateDirectory(directory);

            // The legal group switched off; a local one of one server, over TLS
            // at a port nobody listens on until a test says otherwise; and a
            // group whose one server never answers.
            //
            // With the name servers of BareNodes, from the documentation's
            // ranges, where nothing answers: the servers here are addressed by
            // their addresses, which a round asks no name server about. It used
            // to ask them for the A and AAAA records of "127.0.0.1", and waited
            // for them past the 20 s a test waits for what is told.
            bridged = new BareNodes().New(directory, JObject.Parse($$"""
                { "nts": { "groups": {
                    "legal": { "enabled": false, "servers": [ { "id": "ptb1", "hostname": "ptbtime1.example.invalid" } ] },
                    "local": { "enabled": true, "minServers": 1, "timeoutSeconds": 3,
                               "servers": [ { "id": "time-ssh", "hostname": "127.0.0.1", "ntsKEPort": {{tlsPort}}, "ntpPort": {{ntpPort}} } ] },
                    "grid":  { "enabled": true, "minServers": 1, "timeoutSeconds": 2,
                               "servers": [ { "id": "blackhole", "hostname": "192.0.2.1" } ] }
                } } }
                """));

        }

        [TearDown]
        public async Task TearDown()
        {

            if (server is not null)
                await server.ShutdownAsync();

            if (bridged is not null)
                await bridged.DisposeAsync();

            try { Directory.Delete(directory, recursive: true); } catch { }

        }

        #endregion


        #region ARoundAskedForIsARoundAndToldAfterwards()

        /// <summary>
        /// SyncNetworkTime is Accepted at once; the round it runs is a round of
        /// the group, which the group remembers, and what came of it is told
        /// with the requestId of the CSMS and the client ids of OCPP.
        /// </summary>
        [Test]
        public async Task ARoundAskedForIsARoundAndToldAfterwards()
        {

            await AnNTSServerOverSSH();

            var told      = Told();
            var response  = await Station.SyncNetworkTimeProcessor!(DateTimeOffset.UtcNow,
                                                                   Sender,
                                                                   null!,
                                                                   SyncNetworkTime(4711, "local"),
                                                                   default);

            var round     = (await Within(told, TimeSpan.FromSeconds(20))).Round;
            var group     = bridged!.Node.NTSGroupOf("local")!;

            Assert.Multiple(() => {

                Assert.That(response.Status,                             Is.EqualTo(OCPP21.GenericStatus.Accepted), response.StatusInfo?.AdditionalInfo);

                Assert.That(round,                                       Is.Not.Null);
                Assert.That(round!.Group,                                Is.EqualTo("local"));
                Assert.That(round.OK,                                    Is.True, round.Error);
                Assert.That(round.Answered,                              Is.EqualTo(1));
                Assert.That(round.Required,                              Is.EqualTo(1));
                Assert.That(round.Offset,                                Is.Not.Null);
                Assert.That(round.LegalTime,                             Is.Null, "not the legal group");
                Assert.That(round.Clients.Select(client => client.Client), Is.EqualTo(new[] { "time-ssh" }));
                Assert.That(round.Clients.Single().Host,                 Is.EqualTo("127.0.0.1"));
                Assert.That(round.Clients.Single().OK,                   Is.True);

                Assert.That(group.LastCheck,                             Is.Not.Null, "a round, which the group remembers");

                Assert.That(bridged.Node.Log.Recent(200, Tag: "nts").Select(entry => entry.Message),
                            Has.Some.EqualTo($"'{csms}' asked this {bridged.Node.Kind.Name} over OCPP to check its time against the NTS group 'local' (request 4711)."),
                            "who asked, in the log");

            });

        }

        #endregion

        #region ATestSaysEveryStageAndIsNoRound()

        /// <summary>
        /// TestTimeServer tells every step with its stage, the offset and the
        /// round trip it found - and is no round: the group has never been
        /// checked after it.
        /// </summary>
        [Test]
        public async Task ATestSaysEveryStageAndIsNoRound()
        {

            await AnNTSServerOverSSH();

            var told      = Told();
            var response  = await Station.TestTimeServerProcessor!(DateTimeOffset.UtcNow,
                                                                  Sender,
                                                                  null!,
                                                                  TestTimeServer(4712, "TIME-SSH"),
                                                                  default);

            var test      = (await Within(told, TimeSpan.FromSeconds(20))).Test;

            Assert.Multiple(() => {

                Assert.That(response.Status,           Is.EqualTo(OCPP21.GenericStatus.Accepted), response.StatusInfo?.AdditionalInfo);

                Assert.That(test,                      Is.Not.Null);
                Assert.That(test!.OK,                  Is.True, String.Join(Environment.NewLine, test.Steps));
                Assert.That(test.Client,               Is.EqualTo("TIME-SSH"), "as the CSMS named it");
                Assert.That(test.Host,                 Is.EqualTo("127.0.0.1"));
                Assert.That(test.FailedAt,             Is.Null);
                Assert.That(test.Offset,               Is.Not.Null);
                Assert.That(test.RoundTrip,            Is.Not.Null);
                Assert.That(test.Steps.Select(step => step.Stage).Distinct(),
                            Is.EqualTo(new[] { "Configuration", "Connection", "ServerAuthentication", "KeyExchange", "TimeRequest" }));
                Assert.That(test.Steps.Select(step => step.Level),
                            Is.All.AnyOf("Info", "Notice", "Warning", "Error"));
                Assert.That(test.Steps.Select(step => step.Elapsed),
                            Is.Ordered);

                Assert.That(bridged!.Node.NTSGroupOf("local")!.LastCheck,  Is.Null, "a test is no round");

            });

        }

        #endregion

        #region ATestOfAServerNobodyListensToSaysWhereItEnded()

        /// <summary>
        /// A key exchange whose connection is refused ends the test at the
        /// connection, which is what the CSMS is told.
        /// </summary>
        [Test]
        public async Task ATestOfAServerNobodyListensToSaysWhereItEnded()
        {

            var told      = Told();
            var response  = await Station.TestTimeServerProcessor!(DateTimeOffset.UtcNow,
                                                                  Sender,
                                                                  null!,
                                                                  TestTimeServer(4713, "time-ssh"),
                                                                  default);

            var test      = (await told.Task.WaitAsync(TimeSpan.FromSeconds(20))).Test;

            Assert.Multiple(() => {
                Assert.That(response.Status,         Is.EqualTo(OCPP21.GenericStatus.Accepted));
                Assert.That(test!.OK,                Is.False);
                Assert.That(test.FailedAt,           Is.EqualTo("Connection"), String.Join(Environment.NewLine, test.Steps));
                Assert.That(test.Steps.Last().Level, Is.EqualTo("Error"));
            });

        }

        #endregion

        #region NothingAskedIsRefusedWithAReason()

        /// <summary>
        /// A group or client that does not exist, a group that is switched off,
        /// and a request while another runs: Rejected, with the reason code of
        /// the white paper and what there is instead.
        /// </summary>
        [Test]
        public async Task NothingAskedIsRefusedWithAReason()
        {

            var unknownGroup   = await Station.SyncNetworkTimeProcessor!(DateTimeOffset.UtcNow, Sender, null!, SyncNetworkTime(1, "nope"), default);
            var unknownClient  = await Station.TestTimeServerProcessor! (DateTimeOffset.UtcNow, Sender, null!, TestTimeServer (2, "nope"), default);
            var legalIsOff     = await Station.SyncNetworkTimeProcessor! (DateTimeOffset.UtcNow, Sender, null!, SyncNetworkTime(3, null),   default);
            var ofALegalOff    = await Station.TestTimeServerProcessor! (DateTimeOffset.UtcNow, Sender, null!, TestTimeServer (4, "ptb1"), default);

            // The server of the grid never answers, so its test runs until its
            // timeout - and a request while it runs is Busy.
            var told           = Told();
            var running        = await Station.TestTimeServerProcessor! (DateTimeOffset.UtcNow, Sender, null!, TestTimeServer (5, "blackhole"), default);
            var busy           = await Station.SyncNetworkTimeProcessor!(DateTimeOffset.UtcNow, Sender, null!, SyncNetworkTime(6, "local"),    default);

            var blackhole      = (await told.Task.WaitAsync(TimeSpan.FromSeconds(30))).Test;

            Assert.Multiple(() => {

                Assert.That(unknownGroup.Status,                     Is.EqualTo(OCPP21.GenericStatus.Rejected));
                Assert.That(unknownGroup.StatusInfo?.ReasonCode,     Is.EqualTo("UnknownGroup"));
                Assert.That(unknownGroup.StatusInfo?.AdditionalInfo, Does.Contain("legal, local, grid"));

                Assert.That(unknownClient.Status,                    Is.EqualTo(OCPP21.GenericStatus.Rejected));
                Assert.That(unknownClient.StatusInfo?.ReasonCode,    Is.EqualTo("UnknownClient"));
                Assert.That(unknownClient.StatusInfo?.AdditionalInfo, Does.Contain("ptb1, time-ssh, blackhole"));

                Assert.That(legalIsOff.StatusInfo?.ReasonCode,       Is.EqualTo("Disabled"), "no group is the legal one, which is switched off");
                Assert.That(ofALegalOff.StatusInfo?.ReasonCode,      Is.EqualTo("Disabled"), "a group that is switched off asks nobody, not even for a test");

                Assert.That(running.Status,                          Is.EqualTo(OCPP21.GenericStatus.Accepted));
                Assert.That(busy.Status,                             Is.EqualTo(OCPP21.GenericStatus.Rejected));
                Assert.That(busy.StatusInfo?.ReasonCode,             Is.EqualTo("Busy"));

                Assert.That(blackhole?.OK,                           Is.False);
                Assert.That(blackhole?.Client,                       Is.EqualTo("blackhole"));

            });

        }

        #endregion


        #region OCPP16NamesItsServerByGroupAndName()

        /// <summary>
        /// OCPP 1.6 has no client ids: a TestTimeServer names its server by its
        /// group and its name, and a round is told with its clients named by
        /// their host names alone.
        /// </summary>
        [Test]
        public async Task OCPP16NamesItsServerByGroupAndName()
        {

            await AnNTSServerOverSSH();

            var chargePoint  = (OCPP16.INetworkTimeOnDemandNode) bridged!.OCPPv1_6Node;
            var sender       = (IEventSender) bridged.OCPPv1_6Node;
            var told         = new TaskCompletionSource<OCPP16.CP.NotifyNetworkTimeResultRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            bridged.Bridge.OnNetworkTimeResult16 += notification => told.TrySetResult(notification);

            var unknown      = await chargePoint.TestTimeServerProcessor!(DateTimeOffset.UtcNow, sender, null!,
                                                                         new OCPP16.CS.TestTimeServerRequest(SourceRouting.To(NetworkingNode_Id.Parse("test01")), 1, "time.example", "local", NetworkPath: NetworkPath.From(csms)),
                                                                         default);

            var response     = await chargePoint.SyncNetworkTimeProcessor!(DateTimeOffset.UtcNow, sender, null!,
                                                                          new OCPP16.CS.SyncNetworkTimeRequest(SourceRouting.To(NetworkingNode_Id.Parse("test01")), 2, "local", NetworkPath: NetworkPath.From(csms)),
                                                                          default);

            var round        = (await told.Task.WaitAsync(TimeSpan.FromSeconds(20))).Round;

            Assert.Multiple(() => {

                Assert.That(unknown.Status,                       Is.EqualTo(OCPP16.GenericStatus.Rejected));
                Assert.That(unknown.StatusInfo?.ReasonCode,       Is.EqualTo("UnknownServer"));
                Assert.That(unknown.StatusInfo?.AdditionalInfo,   Does.Contain("ntp.local.servers").And.Contain("127.0.0.1"));

                Assert.That(response.Status,                      Is.EqualTo(OCPP16.GenericStatus.Accepted), response.StatusInfo?.AdditionalInfo);
                Assert.That(round?.OK,                            Is.True, round?.Error);
                Assert.That(round?.Clients.Single().Client,       Is.Null, "no client ids in OCPP 1.6");
                Assert.That(round?.Clients.Single().Host,         Is.EqualTo("127.0.0.1"));

            });

        }

        #endregion


        #region (private) Station / Sender

        /// <summary>
        /// The station's OCPP 2.1 node, as the bridge gave it its processors.
        /// </summary>
        private OCPP21.INetworkTimeOnDemandNode  Station
            => (OCPP21.INetworkTimeOnDemandNode) bridged!.OCPPv2_1Node;

        /// <summary>
        /// The station's OCPP 2.1 node, as its adapter names the sender.
        /// </summary>
        private IEventSender                     Sender
            => (IEventSender) bridged!.OCPPv2_1Node;

        #endregion

        #region (private) Told()

        /// <summary>
        /// The first outcome the bridge tells.
        /// </summary>
        private TaskCompletionSource<OCPP21.CS.NotifyNetworkTimeResultRequest> Told()
        {

            var told = new TaskCompletionSource<OCPP21.CS.NotifyNetworkTimeResultRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            bridged!.Bridge.OnNetworkTimeResult += notification => told.TrySetResult(notification);

            return told;

        }

        #endregion

        #region (private) Within(Told, Timeout)

        /// <summary>
        /// What is told within the given time - or the failure, with what the
        /// node's log says of the time servers.
        /// </summary>
        private async Task<OCPP21.CS.NotifyNetworkTimeResultRequest> Within(TaskCompletionSource<OCPP21.CS.NotifyNetworkTimeResultRequest>  Told,
                                                                            TimeSpan                                                         Timeout)
        {

            if (await Task.WhenAny(Told.Task, Task.Delay(Timeout)) != Told.Task)
                Assert.Fail("Nothing was told within " + Timeout + ":" + Environment.NewLine +
                            String.Join(Environment.NewLine, bridged!.Node.Log.Recent(40, Tag: "nts").Select(entry => entry.Message)));

            return await Told.Task;

        }

        #endregion

        #region (private static) SyncNetworkTime(Id, Group) / TestTimeServer(Id, Client)

        private static OCPP21.CSMS.SyncNetworkTimeRequest SyncNetworkTime(Int32 Id, String? Group)

            => new (SourceRouting.To(NetworkingNode_Id.Parse("test02")),
                    Id,
                    Group,
                    NetworkPath: NetworkPath.From(csms));

        private static OCPP21.CSMS.TestTimeServerRequest TestTimeServer(Int32 Id, String Client)

            => new (SourceRouting.To(NetworkingNode_Id.Parse("test02")),
                    Id,
                    Client,
                    NetworkPath: NetworkPath.From(csms));

        #endregion

        #region (private) AnNTSServerOverSSH()

        /// <summary>
        /// An NTS server of Norn's own that lets the station's key in over SSH,
        /// and the local group's server switched to it - in one SetVariables
        /// request, as a CSMS would.
        /// </summary>
        private async Task AnNTSServerOverSSH()
        {

            var model     = bridged!.Bridge.NTS;
            var clientKey = model.ClientValue("time-ssh", "SSHClientPublicKey");

            Assert.That(clientKey, Does.StartWith("ssh-ed25519 "));

            server = new NTSServer(
                         NTSKEPort:           tlsPort,
                         NTSPort:             ntpPort,
                         MasterKeysFilePath:  null,
                         SSHPort:             sshPort,
                         SSHAuthenticator:    SshUserAuthenticator.ForAuthorizedKeys(SshPublicKey.Parse(clientKey!).Blob)
                     );

            await server.Start();

            OCPP21.ComponentConfig[] components = [ new OCPP21.ClockController(model), .. OCPP21.NTPController.AllOf(model) ];

            var results = await OCPP21.DeviceModel.SetVariablesAsync(
                                    new OCPP21.CSMS.SetVariablesRequest(
                                        Destination:           SourceRouting.To(NetworkingNode_Id.Parse("test02")),
                                        VariableData:          [.. new[] {
                                                                   ("SSHHostKeyPins", SshFingerprint.Sha256(server.SSHHostKeys[0].PublicKeyBlob)),
                                                                   ("SSHPort",        sshPort.ToString()),
                                                                   ("Mode",           "NTSv4SSH")
                                                               }.Select(variable => new OCPP21.SetVariableData(
                                                                                        Component:       new OCPP21.Component(Name: "NTPClientController", Instance: "time-ssh"),
                                                                                        Variable:        new OCPP21.Variable (Name: variable.Item1),
                                                                                        AttributeValue:  variable.Item2
                                                                                    ))],
                                        DataConsistencyModel:  OCPP21.DataConsistencyModel.ACID
                                    ),
                                    (name, evse) => components.Where(component => String.Equals(component.Name, name, StringComparison.OrdinalIgnoreCase)),
                                    default,
                                    model.NewInstance
                                );

            Assert.That(results.Select(result => result.AttributeStatus), Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted),
                        String.Join(" | ", results.Select(result => result.AttributeStatusInfo?.AdditionalInfo)));

        }

        #endregion

        #region (private static) FindFreeTCPPort() / FindFreeUDPPort()

        private static IPPort FindFreeTCPPort()
        {
            using var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            return IPPort.Parse(((IPEndPoint) listener.LocalEndpoint).Port);
        }

        private static IPPort FindFreeUDPPort()
        {
            using var udpClient = new UdpClient(new IPEndPoint(System.Net.IPAddress.Loopback, 0));
            return IPPort.Parse(((IPEndPoint) udpClient.Client.LocalEndPoint!).Port);
        }

        #endregion

    }

}
