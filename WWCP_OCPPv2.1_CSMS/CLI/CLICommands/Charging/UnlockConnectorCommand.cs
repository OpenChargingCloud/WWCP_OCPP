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
using System.Diagnostics.Eventing.Reader;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS.CommandLine
{

    /// <summary>
    /// Get logs
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    //[CLIContext([ DefaultStrings.OCPPv2_0_1,
    //              DefaultStrings.OCPPv2_1 ])]
    public class UnlockConnectorCommand(ICSMSCLI CLI) : ACLICommand<ICSMSCLI>(CLI),
                                                        ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(UnlockConnectorCommand)[..^7].ToLowerFirstChar();

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

            //if (Arguments.Length == 2 &&
            //    CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            //{

            //    foreach (var reportBase in ReportBase.All)
            //    {

            //        if (reportBase.ToString().Equals    (Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {reportBase.ToString().ToLower()}") ];

            //        if (reportBase.ToString().StartsWith(Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [ SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {reportBase.ToString().ToLower()}") ];

            //    }

            //    return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]}") ];

            //}

            //if (Arguments.Length == 3 &&
            //    CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            //{

            //    foreach (var componentCriteria in ComponentCriteriaExtensions.All)
            //    {

            //        if (componentCriteria.ToString().Equals(Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [SuggestionResponse.ParameterCompleted($"{Arguments[0]} {componentCriteria.ToString().ToLower()}")];

            //        if (componentCriteria.ToString().StartsWith(Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [SuggestionResponse.ParameterPrefix($"{Arguments[0]} {componentCriteria.ToString().ToLower()}")];

            //    }

            //    return [SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]}")];

            //}


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


            if (Arguments.Length >= 3)
            {

                if (!EVSE_Id.     TryParse(Arguments[1], out var evseId))
                    return [$"Invalid EVSE Id '{Arguments[1]}'"];

                if (!Connector_Id.TryParse(Arguments[2], out var connectorId))
                    return [ $"Invalid connector Id '{Arguments[2]}'" ];

                var response = await cli.OCPP.OUT.UnlockConnector(
                                         new UnlockConnectorRequest(
                                             Destination:   sourceRoute,
                                             EVSEId:        evseId,
                                             ConnectorId:   connectorId
                                         )
                                     );

                return [
                    $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                    response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                ];

            }

            return [ $"Usage: {CommandName} <EVSEId> <connectorId>" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <EVSEId> <connectorId> - Unlock the given EVSE connector";

        #endregion

    }

}
