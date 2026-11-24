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
using org.GraphDefined.Vanaheimr.CLI;

using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.CommandLine;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving.CommandLine
{

    /// <summary>
    /// The key pins charging stations are let in by on the station port:
    /// listed, given, and taken away - on every node that serves charging
    /// stations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// stationPins lists every station's key pins; stationPins &lt;station&gt;
    /// one station's; stationPins &lt;station&gt; add &lt;pin&gt; [&lt;label&gt;]
    /// lets it in by the key pin of its TLS client key - adding the station
    /// where it is not listed yet - and stationPins &lt;station&gt; remove
    /// &lt;pin&gt; takes one away. A pin is SHA-256 over the station's public
    /// key, as 64 hexadecimal digits, with or without colons, or as curl writes
    /// it, "sha256//" and base64 (EVQI Transport, key pins, 5.1 and 5.8).
    /// </para>
    /// <para>
    /// A command of its own rather than a verb of stationKeys, because the two
    /// are different keys for different doors: an SSH key for OCPP-over-SSH,
    /// a TLS client key for the WebSocket port. One key is never both
    /// (EVQI Transport, key pins, 5.10).
    /// </para>
    /// <para>
    /// What the web interface asks of the same account is asked here: reading
    /// the given resource - the one the node's pages of the charging stations
    /// ask for, "stations" on a local controller and a gateway - to list,
    /// changing it to give or take a pin. The console may do everything. The
    /// node is named as it names itself: "this local controller", "this
    /// gateway".
    /// </para>
    /// <para>
    /// Not found by its type, as a node's own commands are: it needs the
    /// resource, so the command line of each kind of node that serves charging
    /// stations registers it with RegisterCommands. Registered twice, it would
    /// be run by neither.
    /// </para>
    /// </remarks>
    /// <remarks>
    /// What every node's key pins of its logins have is
    /// <see cref="LoginPinsCommand"/> in WWCP_Node; this is the charging
    /// stations', in their words.
    /// </remarks>
    public class StationPinsCommand : LoginPinsCommand
    {

        #region Data

        /// <summary>
        /// The name this is typed as.
        /// </summary>
        public const String CommandName = "stationPins";

        /// <summary>
        /// The node whose charging stations these are.
        /// </summary>
        private readonly OCPPServingNode node;

        #endregion

        #region Properties

        /// <summary>
        /// The resource read to list the key pins and changed to give or take
        /// one.
        /// </summary>
        public String  StationsResource
            => Resource;

        protected override WebSocketLogins        Logins                     => node.WebSocketLogins;
        protected override String                 LoginNoun                  => "charging station";
        protected override String                 LoginWord                  => "station";
        protected override String                 PinsAreFor                 => "on the station port";
        protected override String                 TLSClientCertificateWords  => "a TLS client certificate (security profile 3)";
        protected override IReadOnlyList<String>  LogTags                    => [ "ocpp", "station" ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The command for the given command line, whose node serves charging
        /// stations.
        /// </summary>
        /// <param name="CLI">The command line, of a node that serves charging stations.</param>
        /// <param name="StationsResource">The resource read to list the key pins and changed to give or take one: what the node's pages of the charging stations ask for.</param>
        public StationPinsCommand(NodeCLI  CLI,
                                  String   StationsResource)

            : base(CLI, CommandName, StationsResource)

        {

            this.node = CLI.Node as OCPPServingNode
                            ?? throw new ArgumentException($"The {CLI.Node.Kind.Name} of this command line serves no charging stations.", nameof(CLI));

        }

        #endregion


        #region (protected override) WhyNoLoginSignsIn()

        /// <summary>
        /// What keeps every station out - the station port.
        /// </summary>
        protected override IEnumerable<String> WhyNoLoginSignsIn()
        {

            var profiles = ClientAuthenticationProfiles.SecurityProfiles(node.OCPPServerSettings.ClientAuthentication);

            if (!node.OCPPServerEnabled)
                yield return "The charging station port is switched off: no charging station signs in.";

            else if (!profiles.Contains<Byte>(3))
                yield return "Security profile 3 is not allowed on the charging station port, and a client key is security profile 3: no key pin lets a station in.";

            else if (node.OCPPServerRunning && !node.OCPPServerTLS)
                yield return "The charging station port is not encrypted: no key pin lets a station in until it is.";

        }

        #endregion

    }

}
