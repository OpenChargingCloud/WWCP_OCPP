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
    /// Trigger a message at the current networking node.
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    //[CLIContext([ DefaultStrings.OCPPv1_6 ])]
    public class ExtendedTriggerMessageCommand(ICentralSystemCLI CLI) : ACLICommand<ICentralSystemCLI>(CLI),
                                                                        ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(ExtendedTriggerMessageCommand)[..^7].ToLowerFirstChar();

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

                    if (Arguments.Length > 3)
                        Arguments = Arguments.Take(3).ToArray();

                    if (Arguments.Length == 1)
                    {

                        var list = new List<SuggestionResponse>();

                        foreach (var messageTrigger in MessageTrigger.All)
                            list.Add(SuggestionResponse.ParameterCompleted($"{Arguments[0]} {messageTrigger}"));

                        return list;

                    }

                    if (Arguments.Length == 2)
                    {

                        foreach (var trigger in MessageTrigger.All)
                            if (trigger.ToString().Equals    (Arguments[1], StringComparison.OrdinalIgnoreCase))
                                return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {trigger.ToString().ToLower()}") ];


                        var list = new List<SuggestionResponse>();

                        foreach (var trigger in MessageTrigger.All)
                            if (trigger.ToString().StartsWith(Arguments[1], StringComparison.OrdinalIgnoreCase))
                                list.Add(SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {trigger.ToString().ToLower()}"));

                        return list;

                    }

                    if (Arguments.Length == 3)
                    {

                        if (Connector_Id.TryParse(Arguments[2], out var connectorId))
                             return [ SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {Arguments[1]} {connectorId}") ];


                        return [SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]}")];

                    }

                }


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

            if (Arguments.Length == 2)
            {

                if (MessageTrigger.IsDefined(Arguments[1], out var messageTrigger))
                {

                    var response = await cli.OCPP.OUT.ExtendedTriggerMessage(
                                             new ExtendedTriggerMessageRequest(
                                                 Destination:       sourceRoute,
                                                 RequestedMessage:  messageTrigger
                                             )
                                         );

                    return [
                        $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                        response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                    ];

                }

            }

            if (Arguments.Length == 3)
            {

                if (MessageTrigger.IsDefined(Arguments[1], out var messageTrigger) &&
                    Connector_Id.  TryParse (Arguments[2], out var connectorId))
                {

                    var response = await cli.OCPP.OUT.ExtendedTriggerMessage(
                                             new ExtendedTriggerMessageRequest(
                                                 Destination:       sourceRoute,
                                                 RequestedMessage:  messageTrigger,
                                                 ConnectorId:       connectorId
                                             )
                                         );

                    return [
                        $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                        response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                    ];

                }

            }

            return [ $"Usage: {CommandName} <message trigger> [connectorId]" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <{MessageTrigger.All.Select(_ => _.ToString()).AggregateWith("|")}>  [connectorId] - Trigger a message at the current networking node";

        #endregion

    }

}
