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

using org.GraphDefined.Vanaheimr.CLI;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CentralSystem.CommandLine
{

    /// <summary>
    /// Have the charge point this command line is working with look a name
    /// up, the way it looks up names, or ask one of its name servers alone:
    /// QueryDNS of the white paper "Advanced Diagnostics for OCPP".
    /// </summary>
    /// <remarks>
    /// "queryDNS csms.example.org", "queryDNS _ntske._tcp.example.org SRV @1":
    /// the name first, then the record types, and a name server after an "@",
    /// as the command line of the charge point itself takes them - but the
    /// server by its place in the configuration key dns.servers only, counted
    /// from 1, which is all OCPP 1.6 knows it by. The answer is the response
    /// as it came, in JSON, for whoever looks for a fault - a person, or an
    /// agent that reads it.
    /// </remarks>
    /// <param name="CLI">The command line interface.</param>
    public class QueryDNSCommand(ICentralSystemCLI CLI) : ACLICommand<ICentralSystemCLI>(CLI),
                                                          ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(QueryDNSCommand)[..^7].ToLowerFirstChar();

        #endregion

        #region Suggest(Arguments)

        public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
        {

            // No suggestions without a defined RemoteSystemId and matching OCPP version!
            if (!cli.RemoteSystemIdIsSet() ||
                 cli.GetRemoteSystemOCPPVersion() != DefaultStrings.OCPPv1_6)
            {
                return [];
            }

            if (Arguments.Length == 1)
            {

                if (CommandName.Equals    (Arguments[0], StringComparison.OrdinalIgnoreCase))
                    return [ SuggestionResponse.CommandHelp(Help()) ];

                if (CommandName.StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase))
                    return [ SuggestionResponse.CommandCompleted(CommandName) ];

            }

            return [];

        }

        #endregion

        #region Execute(Arguments, CancellationToken)

        public override async Task<String[]> Execute(String[]           Arguments,
                                                     CancellationToken  CancellationToken)
        {

            // No execution without a defined RemoteSystemId!
            var sourceRoute = cli.GetRemoteSystemSourceRoute();
            if (sourceRoute is null)
                return [];

            if (Arguments.Length < 2)
                return [ $"Usage: {Help()}" ];

            var servers  = Arguments.Skip(2).Where(argument =>  argument.StartsWith('@')).ToArray();
            var types    = Arguments.Skip(2).Where(argument => !argument.StartsWith('@')).ToArray();

            if (servers.Length > 1)
                return [ $"Name one name server at most, not {String.Join(" and ", servers)}." ];

            // More than eight record types are sent all the same: the station
            // answers them with TooManyRecordTypes, which is what somebody
            // testing it wants to see.

            UInt32? server = null;

            if (servers.Length == 1)
            {

                if (!UInt32.TryParse(servers[0][1..], NumberStyles.None, CultureInfo.InvariantCulture, out var place) || place < 1)
                    return [ $"Name the name server by its place in dns.servers, counted from 1, not '{servers[0]}'." ];

                server = place;

            }

            var response = await cli.OCPP.OUT.QueryDNS(
                                     new QueryDNSRequest(
                                         Destination:        sourceRoute,
                                         Name:               Arguments[1],
                                         RecordTypes:        types,
                                         Server:             server,
                                         CancellationToken:  CancellationToken
                                     )
                                 );

            return [
                $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
            ];

        }

        #endregion

        #region Help()

        public override String Help()

            => $"{CommandName} <name> [<record type> ...] [@<n>] - have the charge point look a name or an address up " +
                "the way it does, or ask one of its name servers alone, the n-th of its configuration key dns.servers";

        #endregion

    }

}
