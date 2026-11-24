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

using org.GraphDefined.Vanaheimr.Illias;
using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.CLI;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.CommandLine;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving.CommandLine
{

    /// <summary>
    /// The SSH keys charging stations sign in with for OCPP-over-SSH: listed,
    /// given, and taken away - on every node that serves charging stations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// stationKeys lists every station's keys; stationKeys &lt;station&gt; one
    /// station's; stationKeys &lt;station&gt; add &lt;authorized_keys line&gt;
    /// gives it a key - adding the station where it is not listed yet, with
    /// from= and expiry-time= holding as in OpenSSH's authorized_keys - and
    /// stationKeys &lt;station&gt; remove &lt;fingerprint&gt; takes one away: a
    /// session the key opened ends at once. A fingerprint may be written as
    /// OpenSSH writes it, "SHA256:...", or by enough of its beginning to be only
    /// one key's.
    /// </para>
    /// <para>
    /// What the web interface asks of the same account is asked here: reading
    /// the given resource - the one the node's pages of the charging stations
    /// ask for, "stations" on a local controller and a gateway - to list,
    /// changing it to give or take a key. The console may do everything.
    /// </para>
    /// <para>
    /// A key does not let a station in on its own: the station must be switched
    /// on, its group must take SSH keys, and the node must serve OCPP-over-SSH.
    /// Where one of them is missing, the answer says so - as the web interface
    /// does beside the key. The node is named as it names itself: "this local
    /// controller", "this gateway".
    /// </para>
    /// <para>
    /// Not found by its type, as a node's own commands are: it needs the
    /// resource, so the command line of each kind of node that serves charging
    /// stations registers it with RegisterCommands. Registered twice, it would
    /// be run by neither.
    /// </para>
    /// </remarks>
    /// <remarks>
    /// What every node's keys of its logins have is
    /// <see cref="LoginKeysCommand"/> in WWCP_Node; this is the charging
    /// stations', in their words.
    /// </remarks>
    public class StationKeysCommand : LoginKeysCommand
    {

        #region Data

        /// <summary>
        /// The name this is typed as.
        /// </summary>
        public const String CommandName = "stationKeys";

        /// <summary>
        /// The node whose charging stations these are.
        /// </summary>
        private readonly OCPPServingNode node;

        #endregion

        #region Properties

        /// <summary>
        /// The resource read to list the keys and changed to give or take one.
        /// </summary>
        public String  StationsResource
            => Resource;

        protected override WebSocketLogins        Logins      => node.WebSocketLogins;
        protected override String                 LoginNoun   => "charging station";
        protected override String                 LoginWord   => "station";
        protected override String                 KeysAreFor  => "for OCPP-over-SSH";
        protected override IReadOnlyList<String>  LogTags     => [ "ocpp", "station" ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The command for the given command line, whose node serves charging
        /// stations.
        /// </summary>
        /// <param name="CLI">The command line, of a node that serves charging stations.</param>
        /// <param name="StationsResource">The resource read to list the keys and changed to give or take one: what the node's pages of the charging stations ask for.</param>
        public StationKeysCommand(NodeCLI  CLI,
                                  String   StationsResource)

            : base(CLI, CommandName, StationsResource)

        {

            this.node = CLI.Node as OCPPServingNode
                            ?? throw new ArgumentException($"The {CLI.Node.Kind.Name} of this command line serves no charging stations.", nameof(CLI));

        }

        #endregion


        #region (protected override) TryAddKey(...) / WhyNoLoginSignsIn()

        /// <summary>
        /// Refused for the name of an account of the node, as the web
        /// interface refuses it.
        /// </summary>
        protected override Boolean TryAddKey(User_Id                               Login,
                                             String                                Line,
                                             String                                CreatedBy,
                                             [NotNullWhen(true)]  out UserSSHKey?  Key,
                                             [NotNullWhen(false)] out String?      Error)

            => node.TryAddStationSSHKey(Login, Line, null, null, null, CreatedBy, out Key, out Error, out _);

        /// <summary>
        /// What keeps every station out - the node.
        /// </summary>
        protected override IEnumerable<String> WhyNoLoginSignsIn()
        {

            if (!node.OCPPOverSSHServed)
                yield return "OCPP-over-SSH is switched off: no charging station signs in over SSH.";

            else if (node.OCPPServerSSHURL is null)
                yield return $"The SSH server of this {node.Kind.Name} is off: no charging station signs in over SSH.";

        }

        #endregion

    }

}
