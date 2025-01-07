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
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CentralSystem.CommandLine
{

    /// <summary>
    /// Start a charging session at the current charging station.
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    public class RemoteStartCommand(ICentralSystemCLI CLI) : ACLICommand<ICentralSystemCLI>(CLI),
                                                             ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(RemoteStartCommand)[..^7].ToLowerFirstChar();

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

            if (Arguments.Length > 2 &&
                CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            {
                Arguments = Arguments.Take(2).ToArray();
            }


            if (Arguments.Length == 2 &&
                CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            {

                foreach (var resetType in ResetType.All)
                {

                    if (resetType.ToString().Equals    (Arguments[1], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {resetType.ToString().ToLower()}") ];

                    if (resetType.ToString().StartsWith(Arguments[1], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {resetType.ToString().ToLower()}") ];

                }

                return [ SuggestionResponse.CommandCompleted(CommandName) ];

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


            if (Arguments.Length == 2 || Arguments.Length == 3)
            {

                if (IdToken.TryParse(Arguments[1], out var idToken))
                {

                    Connector_Id? connectorId = null;

                    if (Arguments.Length == 3)
                    {

                        if (Connector_Id.TryParse(Arguments[2], out var _connectorId))
                            connectorId = _connectorId;
                        else
                            return [ $"Invalid connector identification: '{Arguments[2]}'!" ];

                    }

                    var response  = await cli.OCPP.OUT.RemoteStartTransaction(
                                              new RemoteStartTransactionRequest(
                                                  Destination:           sourceRoute,
                                                  IdTag:                 idToken,
                                                  ConnectorId:           connectorId,
                                                  ChargingProfile:       null,

                                                  SignKeys:              null,
                                                  SignInfos:             null,
                                                  Signatures:            null,

                                                  CustomData:            null,

                                                  RequestId:             null,
                                                  RequestTimestamp:      null,
                                                  RequestTimeout:        null,
                                                  EventTrackingId:       null,
                                                  NetworkPath:           null,
                                                  SerializationFormat:   null,
                                                  CancellationToken:     CancellationToken
                                              )
                                          );

                    return [
                        $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                        response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                    ];

                }

                return [ $"Invalid idToken: '{Arguments[2]}'!" ];

            }

            return [ $"Usage: {CommandName} <idToken> [connectorId]" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <idToken> [connectorId] - Start a charging session";

        #endregion

    }

}
