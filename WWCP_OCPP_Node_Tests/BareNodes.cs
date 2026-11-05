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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.TestKit;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// A bare WWCPNode with its bridges: the test nodes of OCPP 2.1 and 1.6,
    /// handed to a DeviceModelBridge, and nothing else around them.
    /// </summary>
    public sealed class BareNodes : IBridgedNodes
    {

        #region New(Directory, Configuration, Clock = null)

        /// <inheritdoc/>
        public IBridgedNode New(String         Directory,
                                JObject        Configuration,
                                TimeProvider?  Clock   = null)

            => new BareNode(Made(Directory, Configuration, Clock));

        #endregion

        #region Started(Directory, Configuration)

        /// <inheritdoc/>
        public async Task<IBridgedNode> Started(String   Directory,
                                                JObject  Configuration)
        {

            var attempt = 0;

            // Made again from its directory, on fresh ports, where another test
            // run took one before it could bind it - its file as it was written.
            var node = await TestPorts.StartedOnFreshPorts(() => attempt++ == 0
                                                                     ? Made(Directory, Configuration, null)
                                                                     : Made(Directory, null,          null));

            return new BareNode(node);

        }

        #endregion

        #region (private static) Made(Directory, Configuration, Clock)

        /// <summary>
        /// A WWCPNode in the given directory, with name servers written down
        /// rather than looked for: from the documentation's ranges, so that a
        /// query that escapes by mistake has nowhere to arrive.
        /// </summary>
        private static WWCPNode Made(String         Directory,
                                     JObject?       Configuration,
                                     TimeProvider?  Clock)
        {

            System.IO.Directory.CreateDirectory(Directory);

            var configFile = Path.Combine(Directory, WWCPConfigFile.DefaultFileName);

            if (Configuration is not null)
                File.WriteAllText(configFile, Configuration.ToString());

            var node = new WWCPNode(
                       HTTPPort:          IPPort.Parse(TestPorts.Free()),
                       AccountsPath:      Path.Combine(Directory, "accounts"),
                       ConfigFile:        new WWCPConfigFile(configFile),
                       DNSClient:         new DNSClient([
                                              new DNSServerConfig(IPv4Address.Parse("192.0.2.53"),   IPPort.DNS),
                                              new DNSServerConfig(IPv6Address.Parse("2001:db8::53"), IPPort.DNS)
                                          ]),
                       CertificatesPath:  Path.Combine(Directory, "certificates"),
                       LogToConsole:      false,
                       BridgeDebugLog:    false,
                       TimeProvider:      Clock
                   );

            // The JSON API every kind derives its own from, which the page
            // saves through.
            _ = new NodeHTTPAPI(node.HTTPServer, node, node.ExtAPI, node.Log);

            return node;

        }

        #endregion

    }


    /// <summary>
    /// A bare WWCPNode with its bridges and the OCPP nodes handed to them.
    /// </summary>
    public sealed class BareNode : IBridgedNode
    {

        #region Data

        private readonly List<(String Host, String Version)>  dialled  = [];
        private readonly List<AWWCPNetworkingNode>             made     = [];
        private          OCPPv2_1.CS.TestChargingStationNode   chargingStation;
        private          OCPPv1_6.TestChargePointNode          chargePoint;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WWCPNode                             Node            { get; }

        /// <inheritdoc/>
        public DeviceModelBridge                    Bridge          { get; }

        /// <inheritdoc/>
        public OCPPv2_1.IDeviceModelNode            OCPPv2_1Node
            => chargingStation;

        /// <inheritdoc/>
        public OCPPv1_6.CP.IConfigurationKeysNode   OCPPv1_6Node
            => chargePoint;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The given node with its bridges.
        /// </summary>
        public BareNode(WWCPNode Node)
        {

            this.Node    = Node;

            this.Bridge  = new DeviceModelBridge(Node,
                                                 NamesToKeepResolving,
                                                 DialsOCPP2_1: () => Dials("2.1"),
                                                 DialsOCPP1_6: () => Dials("1.6"));

            (chargePoint, chargingStation) = Built();

        }

        #endregion


        #region TryDial(URL, OCPPVersion, out Error)

        /// <inheritdoc/>
        public Boolean TryDial(String       URL,
                               String       OCPPVersion,
                               out String?  Error)
        {

            if (!Uri.TryCreate(URL, UriKind.Absolute, out var uri))
            {
                Error = $"'{URL}' is not a URL.";
                return false;
            }

            if (OCPPVersion is not "1.6" and not "2.1")
            {
                Error = $"'{OCPPVersion}' is not an OCPP version: 1.6 or 2.1.";
                return false;
            }

            lock (dialled)
                dialled.Add((uri.Host, OCPPVersion));

            Error = null;
            return true;

        }

        #endregion

        #region NewOCPPNodes()

        /// <inheritdoc/>
        public void NewOCPPNodes()
        {
            (chargePoint, chargingStation) = Built();
        }

        #endregion


        #region (private) Built()

        /// <summary>
        /// The two OCPP nodes, handed to the bridge.
        /// </summary>
        private (OCPPv1_6.TestChargePointNode, OCPPv2_1.CS.TestChargingStationNode) Built()
        {

            var chargePoint      = new OCPPv1_6.TestChargePointNode(
                                       ChargeBoxId:               NetworkingNode_Id.Parse("test01"),
                                       Connectors:                [],
                                       Description:               null,
                                       ChargePointVendor:         "GraphDefined",
                                       ChargePointModel:          "WWCP_OCPP_Node_Tests",
                                       ChargePointSerialNumber:   null,
                                       ChargeBoxSerialNumber:     null,
                                       FirmwareVersion:           null,
                                       Iccid:                     null,
                                       IMSI:                      null,
                                       UplinkEnergyMeter:         null,
                                       DisableSendHeartbeats:     true,
                                       DNSClient:                 Node.DNSClient,
                                       Clock:                     Node.TimeProvider
                                   );

            var chargingStation  = new OCPPv2_1.CS.TestChargingStationNode(
                                       Id:                             NetworkingNode_Id.Parse("test02"),
                                       VendorName:                     "GraphDefined",
                                       Model:                          "WWCP_OCPP_Node_Tests",
                                       Description:                    I18NString.Empty,
                                       SerialNumber:                   null,
                                       FirmwareVersion:                null,
                                       Modem:                          null,
                                       EVSEs:                          [],
                                       UplinkEnergyMeter:              null,
                                       DefaultRequestTimeout:          null,
                                       SignaturePolicy:                null,
                                       ForwardingSignaturePolicy:      null,
                                       HTTPAPI_Disabled:               true,
                                       HTTPAPI_Port:                   null,
                                       HTTPAPI_ServerName:             null,
                                       HTTPAPI_ServiceName:            null,
                                       HTTPAPI_RobotEMailAddress:      null,
                                       HTTPAPI_RobotGPGPassphrase:     null,
                                       HTTPAPI_EventLoggingDisabled:   true,
                                       WebAPI:                         null,
                                       WebAPI_Disabled:                true,
                                       WebAPI_Path:                    null,
                                       ControlWebSocketServer:         null,
                                       DisableSendHeartbeats:          true,
                                       SendHeartbeatsEvery:            null,
                                       DisableMaintenanceTasks:        true,
                                       MaintenanceEvery:               null,
                                       CustomData:                     null,
                                       DNSClient:                      Node.DNSClient,
                                       Clock:                          Node.TimeProvider
                                   );

            lock (made)
            {
                made.Add(chargePoint);
                made.Add(chargingStation);
            }

            Bridge.Configure(chargingStation);
            Bridge.Configure(chargePoint);

            return (chargePoint, chargingStation);

        }

        #endregion

        #region (private) Dials(Version) / NamesToKeepResolving()

        private Boolean Dials(String Version)
        {
            lock (dialled)
                return dialled.Any(connection => connection.Version == Version);
        }

        /// <summary>
        /// The names the CSMS connections are dialled by, leaving out those
        /// dialled by an address.
        /// </summary>
        private IEnumerable<String> NamesToKeepResolving()
        {
            lock (dialled)
                return [.. dialled.Select  (connection => connection.Host).
                                   Where   (host       => !System.Net.IPAddress.TryParse(host.Trim('[', ']'), out _)).
                                   Distinct(StringComparer.OrdinalIgnoreCase)];
        }

        #endregion


        #region DisposeAsync()

        /// <summary>
        /// Stop every OCPP node made for this node, and let the node go.
        /// </summary>
        public async ValueTask DisposeAsync()
        {

            AWWCPNetworkingNode[] nodes;

            lock (made)
                nodes = [.. made];

            foreach (var node in nodes)
            {
                try
                {
                    await node.Stop(Message: "The test is over.");
                }
                catch
                { }
            }

            await Node.DisposeAsync();

        }

        #endregion

    }

}
