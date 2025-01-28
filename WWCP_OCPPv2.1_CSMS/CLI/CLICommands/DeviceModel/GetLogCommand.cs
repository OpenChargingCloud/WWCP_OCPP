/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS.CommandLine
{

    /// <summary>
    /// Get logs
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    //[CLIContext([ DefaultStrings.OCPPv2_0_1,
    //              DefaultStrings.OCPPv2_1 ])]
    public class GetLogCommand(ICSMSCLI CLI) : ACLICommand<ICSMSCLI>(CLI),
                                               ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(GetLogCommand)[..^7].ToLowerFirstChar();

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

            if (Arguments.Length >= 2 &&
                CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            {
                if (!URL.TryParse(Arguments[1], out var url))
                    return [SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]}")];
            }

            if (Arguments.Length == 3 &&
                CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            {

                foreach (var logType in LogType.All)
                {

                    if (logType.ToString().Equals    (Arguments[2], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]} {logType.ToString().ToLower()}") ];

                    if (logType.ToString().StartsWith(Arguments[2], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {Arguments[1]} {logType.ToString().ToLower()}") ];

                }

                return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]}") ];

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


            if (Arguments.Length == 3)
            {

                if (!URL.TryParse(Arguments[1], out var remoteURL))
                    return [ $"Invalid URL: '{Arguments[1]}'!" ];

                if (!LogType.TryParse(Arguments[2], out var logType))
                    return [ $"Invalid log type: '{Arguments[2]}'!" ];

                var response = await cli.OCPP.OUT.GetLog(
                                         new GetLogRequest(
                                             Destination:     sourceRoute,
                                             LogType:         logType,
                                             LogRequestId:    (Int32) RandomExtensions.RandomUInt32(Int32.MaxValue),
                                             Log:             new LogParameters(
                                                                  remoteURL
                                                              ),
                                             Retries:         3,
                                             RetryInterval:   TimeSpan.FromSeconds(5)
                                         )
                                     );

                return [
                    $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                    response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                ];

            }

            return [ $"Usage: {CommandName} <URL> <{LogType.All.Select(_ => _.ToString()).AggregateWith("|")}>" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <URL> <{LogType.All.Select(_ => _.ToString()).AggregateWith("|")}> - Get the local auth list of the current networking node";

        #endregion

    }

}
