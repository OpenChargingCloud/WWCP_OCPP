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

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// Section 7 of the white paper "NTS Key Establishment over SSH", from the
    /// CSMS to the time: it reads the key the station signs in with, has it
    /// entered at the NTS-KE server, switches a time server to NTSv4SSH with
    /// the host key it is pinned to in one SetVariables request - and the
    /// station's next check of the time makes its key exchange over SSH, the
    /// host key it saw and the cookies it holds readable as variables.
    /// </summary>
    /// <remarks>
    /// Against a server of Norn's own with an SSH listener of its own, on the
    /// loopback interface, addressed by its address: no name to resolve.
    /// </remarks>
    [TestFixture]
    [NonParallelizable]
    public class NTSv4SSHEndToEndTests
    {

        #region Data

        private static readonly IPPort  tlsPort  = FindFreeTCPPort();
        private static readonly IPPort  sshPort  = FindFreeTCPPort();
        private static readonly IPPort  ntpPort  = FindFreeUDPPort();

        private String      directory  = "";
        private WWCPNode?   node;
        private NTSServer?  server;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory = Path.Combine(Path.GetTempPath(), "ntsv4ssh-" + Guid.NewGuid().ToString("N")[..12]);
            Directory.CreateDirectory(directory);

            var configFile = Path.Combine(directory, WWCPConfigFile.DefaultFileName);

            // One group, of one server - over TLS, at a port nobody listens on,
            // until the CSMS says otherwise.
            File.WriteAllText(configFile, $$"""
                { "nts": { "groups": {
                    "legal": { "enabled": false, "servers": [ "ptbtime1.example.invalid" ] },
                    "local": { "enabled": true, "minServers": 1,
                               "servers": [ { "id": "time-ssh", "hostname": "127.0.0.1", "ntsKEPort": {{tlsPort}}, "ntpPort": {{ntpPort}} } ] }
                } } }
                """);

            node = new WWCPNode(
                       HTTPPort:          IPPort.Parse(TestPorts.Free()),
                       AccountsPath:      Path.Combine(directory, "accounts"),
                       ConfigFile:        new WWCPConfigFile(configFile),
                       CertificatesPath:  Path.Combine(directory, "certificates"),
                       LogToConsole:      false,
                       BridgeDebugLog:    false
                   );

        }

        [TearDown]
        public async Task TearDown()
        {

            if (server is not null)
                await server.ShutdownAsync();

            if (node is not null)
                await node.DisposeAsync();

            try { Directory.Delete(directory, recursive: true); } catch { }

        }

        #endregion


        #region ACSMSSwitchesATimeServerToNTSv4SSH()

        [Test]
        public async Task ACSMSSwitchesATimeServerToNTSv4SSH()
        {

            var model     = new NTSDeviceModel(node!, Log: null);

            // The CSMS reads the key the station signs in with, and has it
            // entered at the NTS-KE server.
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

            // Over TLS, at a port nobody listens on: no time.
            var before    = await node!.SyncTimeAsync(node.NTSGroupOf("local")!);

            var hostKey   = SshFingerprint.Sha256(server.SSHHostKeys[0].PublicKeyBlob);

            // NTSv4SSH, its port and its host key, in one request.
            var results   = await Set(model,
                                      ("SSHHostKeyPins", hostKey),
                                      ("SSHPort",        sshPort.ToString()),
                                      ("Mode",           "NTSv4SSH"));

            var after     = await node.SyncTimeAsync(node.NTSGroupOf("local")!);

            Assert.Multiple(() => {

                Assert.That(before.Value<Boolean>("ok"),                                   Is.False, "a server nobody listens for answered over TLS");

                Assert.That(results.Select(result => result.AttributeStatus),              Is.All.EqualTo(OCPP21.SetVariableStatus.Accepted),
                            String.Join(" | ", results.Select(result => result.AttributeStatusInfo?.AdditionalInfo)));

                Assert.That(after.Value<Boolean>("ok"),                                    Is.True,  after.ToString());
                Assert.That(model.ClientValue("time-ssh", "Mode"),                         Is.EqualTo("NTSv4SSH"));
                Assert.That(model.ClientValue("time-ssh", "SSHHostKeyFingerprint"),        Is.EqualTo(hostKey));
                Assert.That(Int32.Parse(model.ClientValue("time-ssh", "Cookies") ?? "0"),  Is.GreaterThan(0));
                Assert.That(model.ClientValue("time-ssh", "Problem"),                      Is.EqualTo("false"));
                Assert.That(model.ClientValue("time-ssh", "CertificateFingerprint"),       Is.EqualTo(""), "a certificate, over SSH");

            });

        }

        #endregion

        #region AHostKeyThatIsNotPinnedIsAProblem()

        [Test]
        public async Task AHostKeyThatIsNotPinnedIsAProblem()
        {

            var model     = new NTSDeviceModel(node!, Log: null);
            var clientKey = model.ClientValue("time-ssh", "SSHClientPublicKey");

            server = new NTSServer(
                         NTSKEPort:           tlsPort,
                         NTSPort:             ntpPort,
                         MasterKeysFilePath:  null,
                         SSHPort:             sshPort,
                         SSHAuthenticator:    SshUserAuthenticator.ForAuthorizedKeys(SshPublicKey.Parse(clientKey!).Blob)
                     );

            await server.Start();

            await Set(model,
                      ("SSHHostKeyPins", "SHA256:" + new String('A', 43)),
                      ("SSHPort",        sshPort.ToString()),
                      ("Mode",           "NTSv4SSH"));

            var synced = await node!.SyncTimeAsync(node.NTSGroupOf("local")!);

            Assert.Multiple(() => {
                Assert.That(synced.Value<Boolean>("ok"),                                Is.False);
                Assert.That(model.ClientValue("time-ssh", "Problem"),                   Is.EqualTo("true"));
                Assert.That(model.ClientValue("time-ssh", "SSHHostKeyFingerprint"),     Is.EqualTo(SshFingerprint.Sha256(server.SSHHostKeys[0].PublicKeyBlob)),
                            "the host key it showed, which is the one to pin");
            });

        }

        #endregion


        #region ACentralSystemOfOCPP16SwitchesATimeServerToNTSv4SSH()

        /// <summary>
        /// The same through the configuration keys of OCPP 1.6, one key at a
        /// time: the key the station signs in with is the common
        /// "ntske.sshClientPublicKey"; an SSH key before the mode is refused;
        /// the mode, the port and the n-th server's host key, in that order -
        /// and the station's next check of the time goes over SSH.
        /// </summary>
        [Test]
        public async Task ACentralSystemOfOCPP16SwitchesATimeServerToNTSv4SSH()
        {

            var model      = new NTSDeviceModel(node!, Log: null);
            OCPP16.CP.IConfigurationKeySource keys = model;

            var clientKey  = keys.Get("ntske.sshClientPublicKey")?.Value;

            Assert.That(clientKey, Does.StartWith("ssh-ed25519 "));

            server = new NTSServer(
                         NTSKEPort:           tlsPort,
                         NTSPort:             ntpPort,
                         MasterKeysFilePath:  null,
                         SSHPort:             sshPort,
                         SSHAuthenticator:    SshUserAuthenticator.ForAuthorizedKeys(SshPublicKey.Parse(clientKey!).Blob)
                     );

            await server.Start();

            var hostKey    = SshFingerprint.Sha256(server.SSHHostKeys[0].PublicKeyBlob);

            var early      = await keys.ChangeAsync("ntske.local.sshPorts",         sshPort.ToString(), "CS-16");
            var mode       = await keys.ChangeAsync("ntp.local.mode",               "NTSv4SSH",         "CS-16");
            var port       = await keys.ChangeAsync("ntske.local.sshPorts",         sshPort.ToString(), "CS-16");
            var pin        = await keys.ChangeAsync("ntske.local.sshHostKeyPins.1", hostKey,            "CS-16");

            var after      = await node!.SyncTimeAsync(node.NTSGroupOf("local")!);

            Assert.Multiple(() => {

                Assert.That(early,                                                         Is.EqualTo(OCPP16.ConfigurationStatus.Rejected), "an SSH key before the mode");
                Assert.That((mode, port, pin),                                             Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted)));

                Assert.That(after.Value<Boolean>("ok"),                                    Is.True,  after.ToString());
                Assert.That(keys.Get("ntp.local.mode")?.Value,                             Is.EqualTo("NTSv4SSH"));
                Assert.That(model.ClientValue("time-ssh", "SSHHostKeyFingerprint"),        Is.EqualTo(hostKey));
                Assert.That(Int32.Parse(model.ClientValue("time-ssh", "Cookies") ?? "0"),  Is.GreaterThan(0));

                Assert.That(node.Log.Recent(100, null, null).Select(entry => entry.Message), Has.Some.Contains("(by ocpp:CS-16)"));

            });

        }

        #endregion

        #region (private static) Set(Model, params Variables)

        /// <summary>
        /// Variables of the server's NTPClientController, set in one ACID
        /// request as the Device Model hands them over.
        /// </summary>
        private static async Task<OCPP21.SetVariableResult[]> Set(NTSDeviceModel                      Model,
                                                                  params (String Name, String Value)[] Variables)
        {

            OCPP21.ComponentConfig[] components = [ new OCPP21.ClockController(Model), .. OCPP21.NTPController.AllOf(Model) ];

            var request = new OCPP21.CSMS.SetVariablesRequest(
                              Destination:           SourceRouting.To(NetworkingNode_Id.Parse("cs")),
                              VariableData:          [.. Variables.Select(variable => new OCPP21.SetVariableData(
                                                                                          Component:       new OCPP21.Component(Name: "NTPClientController", Instance: "time-ssh"),
                                                                                          Variable:        new OCPP21.Variable (Name: variable.Name),
                                                                                          AttributeValue:  variable.Value
                                                                                      ))],
                              DataConsistencyModel:  OCPP21.DataConsistencyModel.ACID
                          );

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(request,
                                                                  (name, evse) => components.Where(component => String.Equals(component.Name, name, StringComparison.OrdinalIgnoreCase)),
                                                                  default,
                                                                  Model.NewInstance)];

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
