/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CentralSystem.CommandLine
{

    /// <summary>
    /// Get the current configuration of the current networking node.
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    //[CLIContext([ DefaultStrings.OCPPv1_6 ])]
    public class ChangeConfigurationCommand(ICentralSystemCLI CLI) : ACLICommand<ICentralSystemCLI>(CLI),
                                                                     ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(ChangeConfigurationCommand)[..^7].ToLowerFirstChar();

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

            if (Arguments.Length >= 1)
            {

                if (CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
                {

                    if (Arguments.Length == 2)
                        return [SuggestionResponse.ParameterPrefix($"{Arguments[0]} {Arguments[1]}")];

                    if (Arguments.Length >= 3)
                        return [SuggestionResponse.ParameterPrefix($"{Arguments[0]} {Arguments[1]} {Arguments[2]}")];


                    if (CommandName.Equals    (Arguments[0], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.CommandHelp(Help()) ];

                    if (CommandName.StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.CommandCompleted(CommandName) ];

                }

                if (CommandName.StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase))
                    return [SuggestionResponse.CommandCompleted(CommandName)];

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

                var response = await cli.OCPP.OUT.ChangeConfiguration(
                                         new ChangeConfigurationRequest(
                                             Destination:  sourceRoute,
                                             Key:          Arguments[1].Trim(),
                                             Value:        Arguments[2].Trim()
                                         )
                                     );

                return [
                    $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                    response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                ];

            }

            return [ $"Usage: {CommandName} [key] [value]" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <{ResetType.All.Select(_ => _.ToString()).AggregateWith("|")}> - Reset the current networking node";

        #endregion

    }

}
