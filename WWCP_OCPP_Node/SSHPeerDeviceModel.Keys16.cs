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

using System.Globalization;
using System.Text.RegularExpressions;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The SSH peers of a node as the configuration keys of OCPP 1.6 have them:
    /// what SSHPeerController is to OCPP 2.1, read from the node at every read
    /// and changed as OCPP 2.1 changes it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "sshPeers" lists the names of the peers, and "sshPeers.clientPublicKey"
    /// is the key the node signs in with at them. Each peer has keys of its
    /// own, under its name: "sshPeer.&lt;name&gt;.url" - where it is and who the
    /// node signs in as, "ssh://user@host:port" - and ".hostKeyPins",
    /// ".hostCAs", ".onPinMismatch", ".hostKeyFingerprint", ".connected" and
    /// ".problem".
    /// </para>
    /// <para>
    /// ChangeConfiguration changes one key at a time, and a peer nobody could
    /// believe is never stored. So a peer is added by its "url" alone, which
    /// then says what it is held to as well, after a '#':
    /// "ssh://cs001@10.10.0.7:2222#SHA256:..." - one or more host key pins,
    /// comma separated. A peer that is there takes a "url" without them, and
    /// keeps its pins; with them, they are its pins from then on. A name
    /// "sshPeers" leaves out is a peer no longer; one it adds is refused, as
    /// its name alone is no peer.
    /// </para>
    /// </remarks>
    public sealed partial class SSHPeerDeviceModel : OCPP16.CP.IConfigurationKeySource
    {

        #region Data

        /// <summary>
        /// The key that lists the peers.
        /// </summary>
        public const String PeersKey16            = "sshPeers";

        /// <summary>
        /// The key of the key the node signs in with.
        /// </summary>
        public const String ClientPublicKeyKey16  = "sshPeers.clientPublicKey";

        // What a peer's keys are called after its name, the variable each
        // stands for - null for the URL, which stands for three - and whether
        // it is written.
        private static readonly (String Key, String? Variable, Boolean Written)[] peerKeys16 = [
            ("url",                 null,                  true),
            ("hostKeyPins",         "HostKeyPins",         true),
            ("hostCAs",             "HostCAs",             true),
            ("onPinMismatch",       "OnPinMismatch",       true),
            ("hostKeyFingerprint",  "HostKeyFingerprint",  false),
            ("connected",           "Connected",           false),
            ("problem",             "Problem",             false)
        ];

        private static readonly Regex peerKey16  = new ("^sshPeer\\.([A-Za-z0-9][A-Za-z0-9._-]{0,31})\\.([A-Za-z]+)$",
                                                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        // ssh://[user@]host[:port][#pin,pin] - the host a name, an IPv4 address,
        // or an IPv6 address in brackets.
        private static readonly Regex peerURL16  = new ("^ssh://(?:([A-Za-z0-9._][A-Za-z0-9._@-]*)@)?(\\[[0-9A-Fa-f:.]+\\]|[^\\s:/#@\\[\\]]+)(?::([0-9]{1,5}))?/?(?:#(.*))?$",
                                                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        #endregion

        #region Keys

        /// <summary>
        /// The keys of the SSH peers, with the keys of each peer there is now.
        /// </summary>
        public IEnumerable<String> Keys
        {
            get
            {

                var keys = new List<String> { PeersKey16, ClientPublicKeyKey16 };

                foreach (var peer in PeerIds)
                    foreach (var (key, _, _) in peerKeys16)
                        keys.Add($"sshPeer.{peer}.{key}");

                return keys;

            }
        }

        #endregion

        #region Get(Key)

        /// <summary>
        /// The key of this name with what it says now, or null where it is none
        /// of the SSH peers' keys - one of a peer the node does not have among
        /// them.
        /// </summary>
        public OCPP16.ConfigurationKey? Get(String Key)
        {

            if (String.Equals(Key, PeersKey16, StringComparison.OrdinalIgnoreCase))
                return new OCPP16.ConfigurationKey(PeersKey16, OCPP16.AccessRights.ReadWrite, NodeValue("Peers") ?? "");

            if (String.Equals(Key, ClientPublicKeyKey16, StringComparison.OrdinalIgnoreCase))
                return new OCPP16.ConfigurationKey(ClientPublicKeyKey16, OCPP16.AccessRights.ReadOnly, NodeValue("ClientPublicKey") ?? "");

            if (!TryPeerKey(Key, out var peer, out var name, out var variable, out var written) ||
                PeerIds.FirstOrDefault(id => String.Equals(id, peer, StringComparison.OrdinalIgnoreCase)) is not String id)
            {
                return null;
            }

            var value = variable is null
                            ? URLOf(id)
                            : PeerValue(id, variable);

            return value is null
                       ? null
                       : new OCPP16.ConfigurationKey($"sshPeer.{id}.{name}",
                                                     written ? OCPP16.AccessRights.ReadWrite : OCPP16.AccessRights.ReadOnly,
                                                     value);

        }

        #endregion

        #region ChangeAsync(Key, Value, Sender, CancellationToken = default)

        /// <summary>
        /// Change the key of this name as a CSMS of OCPP 2.1 changes the
        /// variables it stands for - or answer null where it is none of the SSH
        /// peers' keys.
        /// </summary>
        public async Task<OCPP16.ConfigurationStatus?> ChangeAsync(String             Key,
                                                                   String             Value,
                                                                   String?            Sender,
                                                                   CancellationToken  CancellationToken = default)
        {

            var entries = new List<OCPP21.SetVariableData>();

            if (String.Equals(Key, ClientPublicKeyKey16, StringComparison.OrdinalIgnoreCase))
                return OCPP16.ConfigurationStatus.Rejected;

            if (String.Equals(Key, PeersKey16, StringComparison.OrdinalIgnoreCase))
            {

                var names = ListOf(Value);

                // A name alone is no peer: one is added by its "url".
                if (names.Any(name => !PeerIds.Contains(name, StringComparer.OrdinalIgnoreCase)))
                    return OCPP16.ConfigurationStatus.Rejected;

                entries.Add(NodeEntry("Peers", String.Join(",", names)));

            }

            else
            {

                if (!TryPeerKey(Key, out var peer, out _, out var variable, out var written))
                    return null;

                var id = PeerIds.FirstOrDefault(known => String.Equals(known, peer, StringComparison.OrdinalIgnoreCase));

                // The keys of a peer the node does not have are none of its
                // keys - but its "url", which adds it.
                if (id is null && variable is not null)
                    return null;

                if (!written)
                    return OCPP16.ConfigurationStatus.Rejected;

                if (variable is not null)
                    entries.Add(PeerEntry(id!, variable, Value.Trim()));

                else
                {

                    if (!TryURL(Value, out var user, out var host, out var port, out var pins))
                        return OCPP16.ConfigurationStatus.Rejected;

                    // New, it says what it is held to as well; and is a peer
                    // from the moment its name is one.
                    if (id is null)
                    {

                        if (pins is null || pins.Count == 0)
                            return OCPP16.ConfigurationStatus.Rejected;

                        entries.Add(NodeEntry("Peers", String.Join(",", PeerIds.Append(peer))));

                    }

                    var named = id ?? peer;

                    entries.Add(PeerEntry(named, "Address",   host));
                    entries.Add(PeerEntry(named, "Port",      (port ?? 22).ToString(CultureInfo.InvariantCulture)));
                    entries.Add(PeerEntry(named, "UserName",  user ?? ""));

                    if (pins is not null)
                        entries.Add(PeerEntry(named, "HostKeyPins", String.Join(",", pins)));

                }

            }

            OCPP21.ComponentConfig[] components = [.. OCPP21.SSHPeerController.AllOf(this)];

            var results = await OCPP21.DeviceModel.SetVariablesAsync(
                                    new OCPP21.CSMS.SetVariablesRequest(
                                        Destination:           SourceRouting.CSMS,
                                        VariableData:          entries,
                                        DataConsistencyModel:  entries.Count > 1 ? OCPP21.DataConsistencyModel.ACID : null,
                                        NetworkPath:           NetworkingNode_Id.TryParse(Sender ?? "", out var sender)
                                                                   ? NetworkPath.From(sender)
                                                                   : NetworkPath.Empty
                                    ),
                                    (componentName, evse) => components.Where(component => String.Equals(component.Name, componentName, StringComparison.OrdinalIgnoreCase)),
                                    CancellationToken,
                                    NewInstance
                                );

            return results.All(result => result.AttributeStatus == OCPP21.SetVariableStatus.Accepted)
                       ? OCPP16.ConfigurationStatus.Accepted
                       : OCPP16.ConfigurationStatus.Rejected;

        }

        #endregion


        #region (private) TryPeerKey(Key, out Peer, out Name, out Variable, out Written)

        /// <summary>
        /// Whether the given key is one of a peer's: "sshPeer.&lt;name&gt;.&lt;what&gt;".
        /// </summary>
        private static Boolean TryPeerKey(String       Key,
                                          out String   Peer,
                                          out String   Name,
                                          out String?  Variable,
                                          out Boolean  Written)
        {

            Peer      = "";
            Name      = "";
            Variable  = null;
            Written   = false;

            var match = peerKey16.Match(Key ?? "");

            if (!match.Success)
                return false;

            var what = match.Groups[2].Value;

            foreach (var (key, variable, written) in peerKeys16)
            {
                if (String.Equals(key, what, StringComparison.OrdinalIgnoreCase))
                {
                    Peer      = match.Groups[1].Value;
                    Name      = key;
                    Variable  = variable;
                    Written   = written;
                    return true;
                }
            }

            return false;

        }

        #endregion

        #region (private) URLOf(Peer), TryURL(Text, out User, out Host, out Port, out Pins)

        /// <summary>
        /// Where a peer is and who the node signs in as there, as a URL says
        /// it: without the user where it is the node's own, without the port
        /// where it is 22, and never with what it is held to.
        /// </summary>
        private String? URLOf(String Peer)
        {

            if (PeerValue(Peer, "Address") is not String address)
                return null;

            var user  = PeerValue(Peer, "UserName") ?? "";
            var port  = PeerValue(Peer, "Port")     ?? "22";

            return "ssh://" +
                   (user.Length > 0 ? user + "@" : "") +
                   (address.Contains(':') ? $"[{address}]" : address) +
                   (port != "22" ? ":" + port : "");

        }

        private static Boolean TryURL(String              Text,
                                      out String?         User,
                                      out String          Host,
                                      out UInt16?         Port,
                                      out List<String>?   Pins)
        {

            User  = null;
            Host  = "";
            Port  = null;
            Pins  = null;

            var match = peerURL16.Match((Text ?? "").Trim());

            if (!match.Success)
                return false;

            User  = match.Groups[1].Success ? match.Groups[1].Value : null;
            Host  = match.Groups[2].Value.Trim('[', ']');

            if (match.Groups[3].Success)
            {

                if (!UInt16.TryParse(match.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var port) || port == 0)
                    return false;

                Port = port;

            }

            if (match.Groups[4].Success)
                Pins = ListOf(match.Groups[4].Value);

            return true;

        }

        #endregion

        #region (private static) NodeEntry(Variable, Value), PeerEntry(Peer, Variable, Value)

        private static OCPP21.SetVariableData NodeEntry(String Variable, String Value)

            => new (
                   Component:       new OCPP21.Component(Name: Component),
                   Variable:        new OCPP21.Variable (Name: Variable),
                   AttributeValue:  Value
               );

        private static OCPP21.SetVariableData PeerEntry(String Peer, String Variable, String Value)

            => new (
                   Component:       new OCPP21.Component(Name: Component, Instance: Peer),
                   Variable:        new OCPP21.Variable (Name: Variable),
                   AttributeValue:  Value
               );

        #endregion

    }

}
