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
    public class StationKeysCommand : ACLICommand<NodeCLI>,
                                      ICLICommand
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
        public String  StationsResource    { get; }

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
            var verb  = rest.Length < 2 ? "list" : rest[1].ToLowerInvariant();

            var needs = verb == "list"
                            ? Permission.Read(StationsResource)
                            : Permission.Edit(StationsResource);

            if (!cli.MayDo(needs, CommandName, out var refused))
                return Task.FromResult<String[]>([ refused ]);

            return Task.FromResult(
                       rest.Length switch {
                           0 => EveryStationsKeys(),
                           1 => Listed(User_Id.Parse(rest[0])),
                           _ => verb switch {
                                    "add"                           => Add(User_Id.Parse(rest[0]), String.Join(" ", rest[2..])),
                                    "remove" when rest.Length == 3  => Remove(User_Id.Parse(rest[0]), rest[2]),
                                    _                               => [ $"Usage: {Help()}" ]
                                }
                       }
                   );

        }

        #endregion

        #region Help()

        public override String Help()

            => $"{CommandName} [<station> [add <authorized_keys line> | remove <fingerprint>]] - the SSH keys charging stations sign in with for OCPP-over-SSH";

        #endregion


        #region (private) EveryStationsKeys() / Listed(Login)

        private String[] EveryStationsKeys()
        {

            var lines = new List<String>();

            foreach (var login in node.WebSocketLogins.Logins.Where(login => login.HasSSHKey).OrderBy(login => login.Login))
            {
                lines.Add($"{login.Login}:");
                lines.AddRange(login.Keys.Select(key => "  " + Line(key)));
                lines.AddRange(WhyNoKeyLetsIn(login).Select(why => "  " + why));
            }

            if (lines.Count == 0)
                lines.Add($"No charging station of this {node.Kind.Name} has an SSH key.");

            lines.AddRange(WhyNoStationSignsIn());

            return [ .. lines ];

        }

        private String[] Listed(User_Id Login)
        {

            if (!node.WebSocketLogins.TryGet(Login, out var login))
                return [ $"'{Login}' is no charging station of this {node.Kind.Name}." ];

            if (!login.HasSSHKey)
                return [ $"'{login.Login}' has no SSH key." ];

            return [ .. login.Keys.Select(Line),
                     .. WhyNoKeyLetsIn(login),
                     .. WhyNoStationSignsIn() ];

        }

        /// <summary>
        /// One key, as a line: its fingerprint, when and by whom it was given,
        /// the options in front of it, and what it is called - as sshKeys
        /// shows an account's.
        /// </summary>
        private static String Line(UserSSHKey Key)
        {

            var options  = Key.Key.Options.Count > 0
                               ? "  " + String.Join(",", Key.Key.Options)
                               : "";

            var called   = Key.Label ?? (Key.Key.PublicKey.Comment is { Length: > 0 } comment ? comment : null);

            return $"{Key.Fingerprint}  {Key.Created.ToLocalTime():yyyy-MM-dd HH:mm}" +
                   (Key.CreatedBy is not null ? $" by {Key.CreatedBy}" : "") +
                   options +
                   (Key.IsDisabled ? ", switched off" : "") +
                   (called is not null ? $"  {called}" : "");

        }

        #endregion

        #region (private) Add(Login, Line)

        private String[] Add(User_Id Login,
                             String  Line)
        {

            if (Line.Trim().Length == 0)
                return [ $"Usage: {Help()}" ];

            var listed = node.WebSocketLogins.TryGet(Login, out _);

            if (!node.TryAddStationSSHKey(Login,
                                          Line,
                                          Label:        null,
                                          GroupId:      null,
                                          Description:  null,
                                          CreatedBy:    cli.Caller.Account?.Id.ToString() ?? "the console",
                                          out var key,
                                          out var error,
                                          out _))
            {
                return [ $"The key was not given: {error}" ];
            }

            node.Log.Notice(
                $"{cli.Caller.Asker} gave the charging station '{Login}' the SSH key {key.Fingerprint}.",
                cli.Caller.Tags("ocpp", "station", "auth", "ssh")
            );

            node.WebSocketLogins.TryGet(Login, out var login);

            return [ listed
                         ? $"'{Login}' signs in over SSH with the key {key.Fingerprint} now."
                         : $"'{Login}' is a charging station of this {node.Kind.Name} now, in the group '{login?.GroupId}', and signs in over SSH with the key {key.Fingerprint}.",
                     .. login is not null ? WhyNoKeyLetsIn(login) : [],
                     .. WhyNoStationSignsIn() ];

        }

        #endregion

        #region (private) Remove(Login, Fingerprint)

        private String[] Remove(User_Id Login,
                                String  Fingerprint)
        {

            if (!node.WebSocketLogins.TryGet(Login, out var login))
                return [ $"'{Login}' is no charging station of this {node.Kind.Name}." ];

            var wanted   = UserSSHKey.FingerprintFromURL(Fingerprint)["SHA256:".Length..];
            var matching = login.Keys.
                               Where(key => key.Fingerprint["SHA256:".Length..].StartsWith(wanted, StringComparison.Ordinal)).
                               ToArray();

            if (wanted.Length == 0 || matching.Length == 0)
                return [ $"'{login.Login}' has no key {Fingerprint}." ];

            if (matching.Length > 1 && !matching.Any(key => key.Fingerprint == "SHA256:" + wanted))
                return [ $"'{Fingerprint}' is the beginning of {matching.Length} keys of '{login.Login}': {String.Join(", ", matching.Select(key => key.Fingerprint))}." ];

            var key = matching.FirstOrDefault(key => key.Fingerprint == "SHA256:" + wanted) ?? matching[0];

            if (!node.WebSocketLogins.TryRemoveSSHKey(login.Login, key.Fingerprint, out var error, out _))
                return [ $"The key was not taken away: {error}" ];

            node.Log.Notice(
                $"{cli.Caller.Asker} took the SSH key {key.Fingerprint} of the charging station '{login.Login}' away.",
                cli.Caller.Tags("ocpp", "station", "auth", "ssh")
            );

            return [ $"The key {key.Fingerprint} lets '{login.Login}' in no more." ];

        }

        #endregion


        #region (private) WhyNoKeyLetsIn(Station) / WhyNoStationSignsIn()

        /// <summary>
        /// What keeps this station out whatever key it has - the station, or
        /// its group.
        /// </summary>
        private IEnumerable<String> WhyNoKeyLetsIn(WebSocketLogin Station)
        {

            if (!Station.Enabled)
                yield return $"'{Station.Login}' is switched off: no key lets it in.";

            var group = node.WebSocketLogins.GroupOf(Station);

            if (group is null)
                yield return $"The group '{Station.GroupId}' of '{Station.Login}' is gone: no key lets it in.";

            else if (!group.Enabled)
                yield return $"The group '{group.Id}' of '{Station.Login}' is switched off: no key lets it in.";

            else if (!group.Allows(AuthMethod.SSHKey))
                yield return $"The group '{group.Id}' of '{Station.Login}' does not accept SSH keys: no key lets it in.";

        }

        /// <summary>
        /// What keeps every station out - the node.
        /// </summary>
        private IEnumerable<String> WhyNoStationSignsIn()
        {

            if (!node.OCPPOverSSHServed)
                yield return "OCPP-over-SSH is switched off: no charging station signs in over SSH.";

            else if (node.OCPPServerSSHURL is null)
                yield return $"The SSH server of this {node.Kind.Name} is off: no charging station signs in over SSH.";

        }

        #endregion

    }

}
