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
    public class SetVariableMonitoringCommand(ICSMSCLI CLI) : ACLICommand<ICSMSCLI>(CLI),
                                                              ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(SetVariableMonitoringCommand)[..^7].ToLowerFirstChar();

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

            //    foreach (var severity in SeveritiesExtensions.All)
            //    {

            //        if (severity.ToString().Equals    (Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [ SuggestionResponse.ParameterCompleted($"{Arguments[0]} {severity.ToString().ToLower()}") ];

            //        if (severity.ToString().StartsWith(Arguments[1], StringComparison.OrdinalIgnoreCase))
            //            return [ SuggestionResponse.ParameterPrefix   ($"{Arguments[0]} {severity.ToString().ToLower()}") ];

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

                //var variableMonitoringIds = new List<VariableMonitoring_Id>();

                //foreach (var variableMonitoringIdText in Arguments.Skip(1))
                //{
                //    if (VariableMonitoring_Id.TryParse(variableMonitoringIdText, out var variableMonitoringId))
                //        variableMonitoringIds.Add(variableMonitoringId);
                //}

                //if (variableMonitoringIds.Count == 0)
                //    return [$"Invalid variable monitoring Ids: '{Arguments.Skip(1).AggregateWith(", ")}'!"];

                var response = await cli.OCPP.OUT.SetVariableMonitoring(
                                         new SetVariableMonitoringRequest(
                                             Destination:      sourceRoute,
                                             MonitoringData:   [
                                                                   new SetMonitoringData(
                                                                       Value:                           8,
                                                                       MonitorType:                     MonitorType.Delta,
                                                                       Severity:                        Severities.Warning,
                                                                       Component:                       new Component(
                                                                                                            Name: Arguments[1],
                                                                                                            Instance: null,
                                                                                                            EVSE: new EVSE(EVSE_Id.Parse(1), ConnectorId: Connector_Id.Parse(1))
                                                                                                        ),
                                                                       Variable:                        new Variable(
                                                                                                            Name: Arguments[2],
                                                                                                            Instance: null
                                                                                                        ),
                                                                       VariableMonitoringId:            null,
                                                                       Transaction:                     null, //Boolean
                                                                       PeriodicEventStreamParameters:   new PeriodicEventStreamParameters(
                                                                                                            MaxItems:  10,
                                                                                                            MaxTime:   TimeSpan.FromSeconds(10)
                                                                                                        )
                                                                   )
                                                               ]
                                         )
                                     );

                return [
                    $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                    response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                ];

            }

            return [ $"Usage: {CommandName} <value> <monitor type> <severity> <component name> <variable name> [VariableMonitoringId]" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <value> <monitor type> <severity> <component name> <variable name> [VariableMonitoringId] - Configure a variable monitoring";

        #endregion

    }

}
