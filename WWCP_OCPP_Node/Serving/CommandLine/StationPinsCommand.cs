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
    public class StationPinsCommand : ACLICommand<NodeCLI>,
                                      ICLICommand
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
        public String  StationsResource    { get; }

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
                           0 => EveryStationsPins(),
                           1 => Listed(User_Id.Parse(rest[0])),
                           _ => verb switch {
                                    "add"    when rest.Length >= 3  => Add   (User_Id.Parse(rest[0]), rest[2], rest.Length > 3 ? String.Join(" ", rest[3..]) : null),
                                    "remove" when rest.Length == 3  => Remove(User_Id.Parse(rest[0]), rest[2]),
                                    _                               => [ $"Usage: {Help()}" ]
                                }
                       }
                   );

        }

        #endregion

        #region Help()

        public override String Help()

            => $"{CommandName} [<station> [add <key pin> [<label>] | remove <key pin>]] - the key pins charging stations are let in by on the station port";

        #endregion


        #region (private) EveryStationsPins() / Listed(Login)

        private String[] EveryStationsPins()
        {

            var lines = new List<String>();

            foreach (var login in node.WebSocketLogins.Logins.Where(login => login.HasKeyPin).OrderBy(login => login.Login))
            {
                lines.Add($"{login.Login}:");
                lines.AddRange(login.Pins.Select(pin => "  " + Line(pin)));
                lines.AddRange(WhyNoPinLetsIn(login).Select(why => "  " + why));
            }

            if (lines.Count == 0)
                lines.Add($"No charging station of this {node.Kind.Name} has a key pin.");

            lines.AddRange(WhyNoStationSignsIn());

            return [ .. lines ];

        }

        private String[] Listed(User_Id Login)
        {

            if (!node.WebSocketLogins.TryGet(Login, out var login))
                return [ $"'{Login}' is no charging station of this {node.Kind.Name}." ];

            if (!login.HasKeyPin)
                return [ $"'{login.Login}' has no key pin." ];

            return [ .. login.Pins.Select(Line),
                     .. WhyNoPinLetsIn(login),
                     .. WhyNoStationSignsIn() ];

        }

        /// <summary>
        /// One key pin, as a line: the pin, when and by whom it was given, and
        /// what it is called.
        /// </summary>
        private static String Line(LoginKeyPin Pin)

            => $"{Pin.Pin}  {Pin.Added.ToLocalTime():yyyy-MM-dd HH:mm}" +
               (Pin.AddedBy is not null ? $" by {Pin.AddedBy}" : "") +
               (Pin.Label   is not null ? $"  {Pin.Label}"     : "");

        #endregion

        #region (private) Add(Login, Pin, Label)

        private String[] Add(User_Id  Login,
                             String   Pin,
                             String?  Label)
        {

            var listed = node.WebSocketLogins.TryGet(Login, out _);

            if (!node.WebSocketLogins.TryAddKeyPin(Login,
                                                 Pin,
                                                 Label,
                                                 GroupId:      null,
                                                 Description:  null,
                                                 AddedBy:      cli.Caller.Account?.Id.ToString() ?? "the console",
                                                 out var keyPin,
                                                 out var error,
                                                 out _))
            {
                return [ $"The key pin was not given: {error}" ];
            }

            node.Log.Notice(
                $"{cli.Caller.Asker} gave the charging station '{Login}' the key pin {keyPin.Pin}.",
                cli.Caller.Tags("ocpp", "station", "auth", "tls")
            );

            node.WebSocketLogins.TryGet(Login, out var login);

            return [ listed
                         ? $"'{Login}' is let in by the key pin {keyPin.Pin} now."
                         : $"'{Login}' is a charging station of this {node.Kind.Name} now, in the group '{login?.GroupId}', and is let in by the key pin {keyPin.Pin}.",
                     .. login is not null ? WhyNoPinLetsIn(login) : [],
                     .. WhyNoStationSignsIn() ];

        }

        #endregion

        #region (private) Remove(Login, Pin)

        private String[] Remove(User_Id Login,
                                String  Pin)
        {

            if (!node.WebSocketLogins.TryGet(Login, out var login))
                return [ $"'{Login}' is no charging station of this {node.Kind.Name}." ];

            if (!org.GraphDefined.Vanaheimr.Hermod.KeyPin.TryParse(Pin, out var pin) ||
                login.FindKeyPin(pin) is null)
                return [ $"'{login.Login}' has no key pin {Pin}." ];

            if (!node.WebSocketLogins.TryRemoveKeyPin(login.Login, pin.ToHex(), out var error, out _))
                return [ $"The key pin was not taken away: {error}" ];

            node.Log.Notice(
                $"{cli.Caller.Asker} took the key pin {pin} of the charging station '{login.Login}' away.",
                cli.Caller.Tags("ocpp", "station", "auth", "tls")
            );

            return [ $"The key pin {pin} lets '{login.Login}' in no more." ];

        }

        #endregion


        #region (private) WhyNoPinLetsIn(Station) / WhyNoStationSignsIn()

        /// <summary>
        /// What keeps this station out whatever pin it has - the station, or its
        /// group.
        /// </summary>
        private IEnumerable<String> WhyNoPinLetsIn(WebSocketLogin Station)
        {

            if (!Station.Enabled)
                yield return $"'{Station.Login}' is switched off: no key pin lets it in.";

            var group = node.WebSocketLogins.GroupOf(Station);

            if (group is null)
                yield return $"The group '{Station.GroupId}' of '{Station.Login}' is gone: no key pin lets it in.";

            else if (!group.Enabled)
                yield return $"The group '{group.Id}' of '{Station.Login}' is switched off: no key pin lets it in.";

            else if (!group.Allows(AuthMethod.KeyPin))
                yield return $"The group '{group.Id}' of '{Station.Login}' does not accept key pins: no key pin lets it in.";

            else if (!group.Allows(WebSocketClientAuthentication.TLSClientCertificate))
                yield return $"The group '{group.Id}' of '{Station.Login}' does not accept a TLS client certificate (security profile 3), which a client key is: no key pin lets it in.";

        }

        /// <summary>
        /// What keeps every station out - the station port.
        /// </summary>
        private IEnumerable<String> WhyNoStationSignsIn()
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
