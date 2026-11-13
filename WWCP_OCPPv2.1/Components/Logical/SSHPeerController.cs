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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where SSHPeerController reads the SSH peers of a node from: the hosts it
    /// keeps one SSH connection to each, which everything that goes to that
    /// host over SSH and says SSHConnection Shared is a channel of.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever holds the
    /// configuration answers each read. A source that is an
    /// <see cref="ISetVariablesSource"/> as well takes the writes of a
    /// SetVariables request.
    /// </remarks>
    public interface ISSHPeerControllerSource
    {

        /// <summary>
        /// The SSH peers, by the names that are the instances of the controller.
        /// </summary>
        IEnumerable<String>  PeerIds                  { get; }

        /// <summary>
        /// The variables of the instance without a name this source answers.
        /// </summary>
        IEnumerable<String>  NodeVariables            { get; }

        /// <summary>
        /// The variables of the instance without a name this source writes.
        /// </summary>
        IEnumerable<String>  WritableNodeVariables    => [];

        /// <summary>
        /// The variables of a peer this source answers.
        /// </summary>
        IEnumerable<String>  PeerVariables            { get; }

        /// <summary>
        /// The variables of a peer this source writes.
        /// </summary>
        IEnumerable<String>  WritablePeerVariables    => [];

        /// <summary>
        /// The value of a variable of the instance without a name; null for none.
        /// </summary>
        String? NodeValue(String Variable);

        /// <summary>
        /// The value of a variable of a peer; null for none.
        /// </summary>
        String? PeerValue(String Peer, String Variable);

    }


    /// <summary>
    /// The SSH peer controller: one instance per SSH peer of the node, named
    /// after the peer, with where it is, who the node signs in as, and what its
    /// host key is held to; and the instance without a name, with the list of
    /// the peers and the key the node signs in with at them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An SSH peer is a host the node keeps one SSH connection to. OCPP, the
    /// key exchange of a time server and name resolution are channels of that
    /// connection where they say SSHConnection Shared, and none of them owns
    /// it (white papers "OCPP over SSH", section 7.2; "NTS Key Establishment
    /// over SSH", section 7; "DNS over SSH", section 6.3).
    /// </para>
    /// <para>
    /// Peers come and go through Peers of the instance without a name, as name
    /// servers do through DNSClientController.Servers: a name it adds becomes a
    /// peer with the variables the same request sets for it, and one it leaves
    /// out is a peer no longer.
    /// </para>
    /// </remarks>
    public class SSHPeerController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the instance without a name.
        /// </summary>
        internal static readonly ControllerVariable[] NodeVariables = [

            new ("Peers",            DataTypes.String,   "The SSH peers of this node, comma separated: a name it adds becomes a peer with the variables the same request sets for it, one it leaves out is a peer no longer.", Writable: true, MayBeEmpty: true, MaxItems: 16),
            new ("ClientPublicKey",  DataTypes.String,   "The public key this node signs in with at its SSH peers, as a line of authorized_keys.")

        ];

        /// <summary>
        /// The variables of a peer.
        /// </summary>
        internal static readonly ControllerVariable[] PeerVariables = [

            new ("Address",             DataTypes.String,      "The host name or address of the SSH peer, as the servers that share its connection name it.",                                    MaxLimit: 253,                                       Writable: true),
            new ("Port",                DataTypes.Integer,     "The SSH port of the peer.",                                                                             MinLimit: 1, MaxLimit: 65535,                                         Writable: true),
            new ("UserName",            DataTypes.String,      "The user name of the SSH user authentication; empty for the node's own.",                                            MaxLimit: 32,                                            Writable: true, MayBeEmpty: true),
            new ("HostKeyPins",         DataTypes.String,      "Comma separated SHA-256 fingerprints of the host keys the peer may show, as SHA256:<base64> of ssh-keygen -l.",                                                                Writable: true, MayBeEmpty: true, MaxItems: 16),
            new ("HostCAs",             DataTypes.String,      "Comma separated SHA-256 fingerprints of the SSH CA keys that may sign the host certificate of the peer.",                                                                     Writable: true, MayBeEmpty: true, MaxItems: 4),
            new ("OnPinMismatch",       DataTypes.OptionList,  "What a host key that neither a pin nor a CA vouches for leads to.",                                                  ValuesList: [ "Refuse", "Record", "Accept" ],            Writable: true),
            new ("HostKeyFingerprint",  DataTypes.String,      "The SHA-256 fingerprint of the host key the peer showed at the last handshake."),
            new ("Connected",           DataTypes.Boolean,     "Whether the SSH connection to the peer lasts now: it is made when something first needs it."),
            new ("Problem",             DataTypes.Boolean,     "True while the host key the peer showed last was refused.")

        ];

        #endregion

        #region Properties

        /// <summary>
        /// The peer this instance is of; null for the instance without a name.
        /// </summary>
        public String?  Peer      { get; }

        /// <summary>
        /// Whether this stands for a peer a SetVariables request adds, which
        /// the source does not have yet - see <see cref="NewInstance"/>.
        /// </summary>
        public Boolean  IsNew     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The instance without a name: the list of the peers, and the key the
        /// node signs in with at them.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public SSHPeerController(ISSHPeerControllerSource Source)

            : base(nameof(SSHPeerController),
                   null,
                   I18NString.Create("The SSH peers of this node: the hosts it keeps one SSH connection to each."),
                   null)

        {

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.NodeVariables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableNodeVariables : [], StringComparer.OrdinalIgnoreCase);

            foreach (var variable in NodeVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.NodeValue(variable.Name),
                                              written.Contains(variable.Name),
                                              Monitored: true)
                );

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        /// <summary>
        /// The instance of one SSH peer.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Peer">The name of the peer; the instance.</param>
        public SSHPeerController(ISSHPeerControllerSource  Source,
                                 String                    Peer)

            : this(Source, Peer, false)

        { }

        private SSHPeerController(ISSHPeerControllerSource  Source,
                                  String                    Peer,
                                  Boolean                   IsNew)

            : base(nameof(SSHPeerController),
                   Peer,
                   I18NString.Create($"The SSH peer '{Peer}' of this node."),
                   null)

        {

            if (!IsNew && !Source.PeerIds.Contains(Peer, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"The source has no SSH peer '{Peer}'!", nameof(Peer));

            this.Peer   = Peer;
            this.IsNew  = IsNew;

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.PeerVariables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritablePeerVariables : [], StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: the host key at a handshake, and whether
            // the connection lasts.
            foreach (var variable in PeerVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.PeerValue(Peer, variable.Name),
                                              written.Contains(variable.Name),
                                              Monitored: true)
                );

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) NewInstance(Source, Peer)

        /// <summary>
        /// The controller of a peer the source does not have yet, standing in
        /// for it while a SetVariables request that adds it is set.
        /// </summary>
        /// <param name="Source">Where the values of the peer will come from.</param>
        /// <param name="Peer">The name of the new peer; its instance.</param>
        public static SSHPeerController NewInstance(ISSHPeerControllerSource  Source,
                                                    String                    Peer)

            => new (Source, Peer, true);

        #endregion

        #region (static) TryCheck(Peer, Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable can be set to the given value by its
        /// type, its limits and its values alone - or why not, as a
        /// SetVariables result says it.
        /// </summary>
        /// <param name="Peer">The name of the peer, or null for the instance without a name.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String?                               Peer,
                                       String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = (Peer is null ? NodeVariables : PeerVariables).
                               FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(SSHPeerController)}'!");
                return false;
            }

            return variable.TryCheck(Value,
                                     Peer is null
                                         ? $"{nameof(SSHPeerController)}.{variable.Name}"
                                         : $"{nameof(SSHPeerController)}[{Peer}].{variable.Name}",
                                     out Refusal);

        }

        #endregion

        #region (static) AllOf(Source)

        /// <summary>
        /// The instance without a name and the controllers of every peer of the
        /// given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<SSHPeerController> AllOf(ISSHPeerControllerSource Source)

            => new[] { new SSHPeerController(Source) }.
                   Concat(Source.PeerIds.
                                 Distinct(StringComparer.OrdinalIgnoreCase).
                                 Select  (peer => new SSHPeerController(Source, peer)));

        #endregion

    }

}
