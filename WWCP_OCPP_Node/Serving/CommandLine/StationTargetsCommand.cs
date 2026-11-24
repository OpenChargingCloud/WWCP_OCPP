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
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving.CommandLine
{

    /// <summary>
    /// Where what the charging stations send up is forwarded to: the targets -
    /// a priority list of back ends each - listed, written and removed, and a
    /// station forwarded to one, or to the back end in charge again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// stationTargets lists the targets, the stations forwarded to each, and
    /// whether this node forwards at all; stationTargets set &lt;target&gt;
    /// &lt;peer&gt; [&lt;peer&gt; ...] makes a target, or changes the peers it
    /// tries and their order; stationTargets remove &lt;target&gt; removes one
    /// no station is forwarded to; stationTargets forward &lt;station&gt;
    /// &lt;target&gt;|none forwards a station to a target, or to the back end in
    /// charge.
    /// </para>
    /// <para>
    /// What the web interface asks of the same account is asked here: reading
    /// the given resource to list, changing it to write. The console may do
    /// everything. Registered by the command line of each kind of node that
    /// serves charging stations, as stationPins is.
    /// </para>
    /// </remarks>
    public class StationTargetsCommand : ACLICommand<NodeCLI>,
                                         ICLICommand
    {

        #region Data

        /// <summary>
        /// The name this is typed as.
        /// </summary>
        public const String CommandName = "stationTargets";

        /// <summary>
        /// The node whose charging stations these are.
        /// </summary>
        private readonly OCPPServingNode node;

        #endregion

        #region Properties

        /// <summary>
        /// The resource read to list the targets and changed to write them.
        /// </summary>
        public String  StationsResource    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The command for the given command line, whose node serves charging
        /// stations.
        /// </summary>
        /// <param name="CLI">The command line, of a node that serves charging stations.</param>
        /// <param name="StationsResource">The resource read to list the targets and changed to write them: what the node's pages of the charging stations ask for.</param>
        public StationTargetsCommand(NodeCLI  CLI,
                                     String   StationsResource)

            : base(CLI)

        {

            this.node              = CLI.Node as OCPPServingNode
                                         ?? throw new ArgumentException($"The {CLI.Node.Kind.Name} of this command line serves no charging stations.", nameof(CLI));

            this.StationsResource  = StationsResource;

        }

        #endregion


        #region Suggest(Arguments)

        public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
        {

            if (Arguments.Length == 1)
                return CommandName.StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase)
                           ? [ SuggestionResponse.CommandCompleted(CommandName) ]
                           : [];

            return [];

        }

        #endregion

        #region Execute(Arguments, CancellationToken)

        public override Task<String[]> Execute(String[]           Arguments,
                                               CancellationToken  CancellationToken)
        {

            var rest  = Arguments.Skip(1).ToArray();
            var verb  = rest.Length == 0 ? "list" : rest[0].ToLowerInvariant();

            var needs = verb == "list"
                            ? Permission.Read(StationsResource)
                            : Permission.Edit(StationsResource);

            if (!cli.MayDo(needs, CommandName, out var refused))
                return Task.FromResult<String[]>([ refused ]);

            return Task.FromResult(
                       verb switch {
                           "list"                         => List(),
                           "set"     when rest.Length >= 3 => Set    (rest[1], rest[2..]),
                           "remove"  when rest.Length == 2 => Remove (rest[1]),
                           "forward" when rest.Length == 3 => Forward(rest[1], rest[2]),
                           _                               => [ $"Usage: {Help()}" ]
                       }
                   );

        }

        #endregion

        #region Help()

        public override String Help()

            => $"{CommandName} [set <target> <peer> [<peer> ...] | remove <target> | forward <station> <target>|none] - where what the charging stations send up is forwarded to";

        #endregion


        #region (private) List()

        private String[] List()
        {

            var logins  = node.WebSocketLogins;
            var lines   = new List<String> {
                              node.Forwards
                                  ? $"This {node.NodeNoun} forwards{(node.Forwarding.Enabled is null ? ", as its kind does" : "")}."
                                  : $"This {node.NodeNoun} forwards nothing: what is not for itself is refused."
                          };

            if (logins.Targets.Count == 0)
                lines.Add($"No target: every charging station goes to the back end in charge of this {node.NodeNoun}.");

            foreach (var target in logins.Targets)
            {

                var stations = logins.Logins.Where(login => login.Target == target.Id).Select(login => login.Login.ToString()).ToArray();

                lines.Add($"{target.Id}: {String.Join(" > ", target.PeerIds.Select(PeerName))}" +
                          (stations.Length > 0 ? $"  ({String.Join(", ", stations)})" : "  (no station)"));

            }

            return [ .. lines ];

        }

        /// <summary>
        /// A peer as a line names it: its id, what it is, and whether it is connected now.
        /// </summary>
        private String PeerName(String Id)
        {

            if (node.Connections?.Peers.FirstOrDefault(peer => peer.Id == Id) is not PeerEntry peer)
                return $"{Id} (gone)";

            var states     = node.ConnectionStates;
            var connected  = peer.EndpointIds.Any(endpoint => states.TryGetValue(endpoint, out var state) && state.Status == ConnectionStatus.Connected);

            return $"{Id} '{peer.Description.FirstText()}' ({(connected ? "connected" : "not connected")})";

        }

        #endregion

        #region (private) Set(Target, Peers) / Remove(Target)

        private String[] Set(String    Target,
                             String[]  Peers)
        {

            var existed = node.WebSocketLogins.GetTarget(Target) is not null;

            if (!node.WebSocketLogins.TryAddOrUpdateTarget(Target, Peers, null, out var error, out _))
                return [ $"The target was not written: {error}" ];

            cli.Node.Log.Notice(
                $"{cli.Caller.Asker} wrote the target '{Target.Trim().ToLowerInvariant()}': {String.Join(", then ", Peers.Select(peer => $"'{peer}'"))}.",
                cli.Caller.Tags([ "ocpp", "station", "forwarding" ])
            );

            return [ $"The target '{Target.Trim().ToLowerInvariant()}' {(existed ? "tries" : "is made, and tries")} {String.Join(", then ", Peers.Select(PeerName))}." ];

        }

        private String[] Remove(String Target)
        {

            if (!node.WebSocketLogins.TryRemoveTarget(Target, out var error, out _))
                return [ $"The target was not removed: {error}" ];

            cli.Node.Log.Notice(
                $"{cli.Caller.Asker} removed the target '{Target.Trim().ToLowerInvariant()}'.",
                cli.Caller.Tags([ "ocpp", "station", "forwarding" ])
            );

            return [ $"The target '{Target.Trim().ToLowerInvariant()}' is removed." ];

        }

        #endregion

        #region (private) Forward(Station, Target)

        private String[] Forward(String  Station,
                                 String  Target)
        {

            if (User_Id.TryParse(Station) is not User_Id login)
                return [ $"'{Station}' is no charging station of this {node.NodeNoun}." ];

            var none = Target.Equals("none", StringComparison.OrdinalIgnoreCase);

            if (!node.WebSocketLogins.TrySetTarget(login, none ? null : Target, out var error, out _))
                return [ $"The charging station was not forwarded: {error}" ];

            cli.Node.Log.Notice(
                none
                    ? $"{cli.Caller.Asker} forwarded the charging station '{login}' to the back end in charge."
                    : $"{cli.Caller.Asker} forwarded the charging station '{login}' to the target '{Target.Trim().ToLowerInvariant()}'.",
                cli.Caller.Tags([ "ocpp", "station", "forwarding" ])
            );

            return [ none
                         ? $"What '{login}' sends up goes to the back end in charge of this {node.NodeNoun} now."
                         : $"What '{login}' sends up goes to the target '{Target.Trim().ToLowerInvariant()}' now." ];

        }

        #endregion

    }

}
