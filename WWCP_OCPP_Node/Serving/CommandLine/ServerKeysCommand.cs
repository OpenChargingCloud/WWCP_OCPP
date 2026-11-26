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

using org.GraphDefined.Vanaheimr.CLI;

using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.CommandLine;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving.CommandLine
{

    /// <summary>
    /// The TLS server key of the charging station port and its key pins: shown,
    /// made, announced for the next rollover, and switched to - on every node
    /// that serves charging stations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// serverKeys lists the key pin the charging stations pin - the current key
    /// first, then the announced next one - and every key with its own;
    /// serverKeys create [&lt;algorithm&gt;] makes the server key where nothing
    /// is presented yet; serverKeys next [&lt;algorithm&gt;] announces the next
    /// key, whose pin is handed to the stations before the switch; serverKeys
    /// switch presents it from now on (EVQI Transport, key pins, 5.5 and 7.4).
    /// </para>
    /// <para>
    /// The keys are the server keys of the listener "ocpp" in the node's
    /// certificate store - the same ones the Server identities page and
    /// /v1/certificates/serverKeys/ocpp show and change.
    /// </para>
    /// <para>
    /// What the web interface asks of the same account is asked here: reading
    /// the certificates to list, changing them for the rest. The console may do
    /// everything. The node is named as it names itself: "this local
    /// controller", "this gateway".
    /// </para>
    /// <para>
    /// Not found by its type: the command line of each kind of node that serves
    /// charging stations registers it with RegisterCommands, as stationPins.
    /// Registered twice - or beside a copy of a kind's own of the same name -
    /// it would be run by neither.
    /// </para>
    /// </remarks>
    public class ServerKeysCommand : ACLICommand<NodeCLI>,
                                     ICLICommand
    {

        #region Data

        /// <summary>
        /// The name this is typed as.
        /// </summary>
        public const String CommandName = "serverKeys";

        /// <summary>
        /// The node whose charging station port this is.
        /// </summary>
        private readonly OCPPServingNode node;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The command for the given command line, whose node serves charging
        /// stations.
        /// </summary>
        /// <param name="CLI">The command line, of a node that serves charging stations.</param>
        public ServerKeysCommand(NodeCLI CLI)

            : base(CLI)

        {

            this.node = CLI.Node as OCPPServingNode
                            ?? throw new ArgumentException($"The {CLI.Node.Kind.Name} of this command line serves no charging stations.", nameof(CLI));

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
                            ? Permission.Read(NodeResources.Certificates)
                            : Permission.Edit(NodeResources.Certificates);

            if (!cli.MayDo(needs, CommandName, out var refused))
                return Task.FromResult<String[]>([ refused ]);

            return Task.FromResult(
                       verb switch {
                           "list"   when rest.Length <= 1  => Listed(),
                           "create" when rest.Length <= 2  => Create(rest.ElementAtOrDefault(1), AsNext: false),
                           "next"   when rest.Length <= 2  => Create(rest.ElementAtOrDefault(1), AsNext: true),
                           "switch" when rest.Length == 1  => Switch(),
                           _                               => [ $"Usage: {Help()}" ]
                       }
                   );

        }

        #endregion

        #region Help()

        public override String Help()

            => $"{CommandName} [list | create [<algorithm>] | next [<algorithm>] | switch] - the key the charging stations pin, and its next one";

        #endregion


        #region (private) Listed()

        private String[] Listed()
        {

            var store    = node.Certificates;
            var current  = store.GetCurrentAndNextKey(OCPPServingNode.StationListener, out var next);
            var shown    = node.StationCertificate?.Choose();
            var lines    = new List<String>();

            if (current is null)
            {

                // No server key: the port may present a certificate an
                // authority issued for a key made here, which is no key the
                // stations pin but a key all the same.
                if (shown is not null)
                    lines.Add($"key pin       {shown.Entry.KeyPin?.ToHex() ?? "?"}  {shown.Entry.Id}, {shown.Entry.KeyAlgorithm}, '{shown.Entry.Label}', " +
                              $"in the certificate an authority issued for it, until {shown.Entry.NotAfter:yyyy-MM-dd}" +
                              (shown.IsKeptOn ? " - not valid, and presented all the same: nothing else is" : ""));

                else
                    lines.Add("No key is presented to the charging stations. 'serverKeys create' makes one for them to pin.");

            }

            else
            {

                lines.Add($"key pin       {current.KeyPin.ToHex()}  {current.Id}, {current.Algorithm}, made {current.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm}, " +
                          (shown is { IsCarrier: false }
                               ? "in the certificate an authority issued for it"
                               : "in its carrier certificate"));

                if (!node.OCPPServerTLS)
                    lines.Add("              The charging station port is not encrypted, so it presents no key; it is from the next start.");

            }

            if (next is not null)
                lines.Add($"next key pin  {next.KeyPin.ToHex()}  {next.Id}, {next.Algorithm}, made {next.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm} - " +
                           "hand it to the charging stations, then 'serverKeys switch'");

            var others = store.ServerKeys(OCPPServingNode.StationListener).
                               Where(key => key.KeyPin != current?.KeyPin && key.KeyPin != next?.KeyPin).
                               ToArray();

            if (others.Length > 0)
            {
                lines.Add("other keys:");
                lines.AddRange(others.Select(key => {
                                                 var issued = store.IssuedFor(key).FirstOrDefault();
                                                 return $"  {key.Id}  {key.Algorithm}  key pin {key.KeyPin.ToHex()}  made {key.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm}" +
                                                        (issued is not null ? $", certificate until {issued.NotAfter:yyyy-MM-dd}" : ", no longer presented");
                                             }));
            }

            return [ .. lines ];

        }

        #endregion

        #region (private) Create(Algorithm, AsNext)

        private String[] Create(String?  Algorithm,
                                Boolean  AsNext)
        {

            if (!node.Certificates.TryCreateServerKey(OCPPServingNode.StationListener, Algorithm, AsNext, out var key, out var error, out _))
                return [ $"No key was made: {error}" ];

            cli.Node.Log.Notice(
                $"{cli.Caller.Asker} had this {node.Kind.Name} generate the {(AsNext ? "next " : "")}server key '{key.Id}' with the key pin {key.KeyPin.ToHex()}.",
                cli.Caller.Tags("ocpp", "tls")
            );

            return AsNext
                       ? [ $"next key pin  {key.KeyPin.ToHex()}  {key.Id}",
                           "Hand it to the charging stations; 'serverKeys switch' presents it once they all have it." ]
                       : [ $"key pin       {key.KeyPin.ToHex()}  {key.Id}",
                           .. node.OCPPServerTLS
                                  ? Array.Empty<String>()
                                  : [ "The charging station port is not encrypted yet; it is from the next start, in this key's carrier certificate." ] ];

        }

        #endregion

        #region (private) Switch()

        private String[] Switch()
        {

            if (!node.Certificates.TrySwitchToNextKey(OCPPServingNode.StationListener, null, out var key, out var error, out _))
                return [ $"Nothing was switched: {error}" ];

            cli.Node.Log.Notice(
                $"{cli.Caller.Asker} switched the charging station server to the server key '{key.Id}'.",
                cli.Caller.Tags("ocpp", "tls")
            );

            return [ $"The charging station server presents the key '{key.Id}' from the next handshake on, with the key pin {key.KeyPin.ToHex()}.",
                     "Charging stations that pinned only the key before are refused from then on." ];

        }

        #endregion

    }

}
