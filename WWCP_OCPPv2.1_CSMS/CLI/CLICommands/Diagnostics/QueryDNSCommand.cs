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
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS.CommandLine
{

    /// <summary>
    /// Have the charging station this command line is working with look a name
    /// up, the way it looks up names, or ask one of its name servers alone:
    /// QueryDNS of the white paper "Advanced Diagnostics for OCPP".
    /// </summary>
    /// <remarks>
    /// "queryDNS csms.example.org", "queryDNS _ntske._tcp.example.org SRV @dns-1":
    /// the name first, then the record types, and a name server after an "@",
    /// as the command line of the station itself takes them - but the server
    /// by the id of its DNSServerController only, which is all OCPP knows it
    /// by. The answer is the response as it came, in JSON, for whoever looks
    /// for a fault - a person, or an agent that reads it.
    /// </remarks>
    /// <param name="CLI">The command line interface.</param>
    public class QueryDNSCommand(ICSMSCLI CLI) : ACLICommand<ICSMSCLI>(CLI),
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
                 cli.GetRemoteSystemOCPPVersion() != DefaultStrings.OCPPv2_1)
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

            var response = await cli.OCPP.OUT.QueryDNS(
                                     new QueryDNSRequest(
                                         Destination:        sourceRoute,
                                         Name:               Arguments[1],
                                         RecordTypes:        types,
                                         ServerId:           servers.Length == 1 ? servers[0][1..] : null,
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

            => $"{CommandName} <name> [<record type> ...] [@<serverId>] - have the charging station look a name or an address up " +
                "the way it does, or ask one of its name servers alone, by the id of its DNSServerController";

        #endregion

    }

}
