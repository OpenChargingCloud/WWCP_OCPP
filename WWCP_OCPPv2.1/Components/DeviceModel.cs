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

using System.Diagnostics.CodeAnalysis;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Look up the component configurations of the given name, at the given
    /// EVSE or, without one, at the node itself.
    /// </summary>
    /// <param name="Name">The case insensitive name of the component.</param>
    /// <param name="EVSE">The optional EVSE of the component.</param>
    public delegate IEnumerable<ComponentConfig> ComponentConfigLookupDelegate(String  Name,
                                                                               EVSE?   EVSE);

    /// <summary>
    /// Send one part of a report as a NotifyReport request.
    /// </summary>
    /// <param name="SequenceNumber">The number of this part, starting at 0.</param>
    /// <param name="ReportData">The report data of this part.</param>
    /// <param name="ToBeContinued">Whether more parts follow.</param>
    public delegate Task NotifyReportPartDelegate(UInt32                     SequenceNumber,
                                                  IReadOnlyList<ReportData>  ReportData,
                                                  Boolean                    ToBeContinued);


    /// <summary>
    /// The Device Model of a node, read, written and reported as OCPP 2.1 says,
    /// for every node that keeps its components as component configurations.
    /// </summary>
    /// <remarks>
    /// Component and variable names and instances are compared case insensitively.
    /// A write-only variable is never read out, neither by GetVariables nor in a report.
    /// </remarks>
    public static class DeviceModel
    {

        #region GetVariables        (VariableData, ComponentConfigs)

        /// <summary>
        /// Read the given variables.
        /// </summary>
        /// <param name="VariableData">The variables to read.</param>
        /// <param name="ComponentConfigs">The lookup of the node's component configurations.</param>
        public static IReadOnlyList<GetVariableResult> GetVariables(IEnumerable<GetVariableData>   VariableData,
                                                                    ComponentConfigLookupDelegate  ComponentConfigs)

            => [.. VariableData.Select(getVariableData => GetVariable(getVariableData, ComponentConfigs))];

        #endregion

        #region SetVariablesAsync   (Request, ComponentConfigs, CancellationToken = default)

        /// <summary>
        /// Set the variables of the given SetVariables request, one after the other.
        /// </summary>
        /// <param name="Request">A SetVariables request.</param>
        /// <param name="ComponentConfigs">The lookup of the node's component configurations.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public static async Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(SetVariablesRequest            Request,
                                                                                    ComponentConfigLookupDelegate  ComponentConfigs,
                                                                                    CancellationToken              CancellationToken   = default)
        {

            // BASE and ACID (OCPP_CSE) need every entry before the first is set. Until
            // that exists, no entry is set rather than some of them.
            if (Request.DataConsistencyModel.HasValue &&
                Request.DataConsistencyModel.Value != DataConsistencyModel.IndependentRequests)
            {
                return [.. Request.VariableData.Select(setVariableData => new SetVariableResult(
                                                                              AttributeStatus:       SetVariableStatus.Rejected,
                                                                              Component:             setVariableData.Component,
                                                                              Variable:              setVariableData.Variable,
                                                                              AttributeType:         setVariableData.AttributeType,
                                                                              AttributeStatusInfo:   new StatusInfo(
                                                                                                         "UnsupportedParam",
                                                                                                         $"The data consistency model '{Request.DataConsistencyModel}' is not supported!"
                                                                                                     )
                                                                          ))];
            }

            var setVariableResults = new List<SetVariableResult>();

            foreach (var setVariableData in Request.VariableData)
                setVariableResults.Add(
                    await SetVariableAsync(
                              setVariableData,
                              ComponentConfigs,
                              CancellationToken
                          ).ConfigureAwait(false)
                );

            return setVariableResults;

        }

        #endregion


        #region BaseReport          (ComponentConfigs, ReportBase)

        /// <summary>
        /// The report data of the given predefined report.
        /// </summary>
        /// <param name="ComponentConfigs">All component configurations of the node.</param>
        /// <param name="ReportBase">The predefined report.</param>
        public static (GenericDeviceModelStatus Status, IReadOnlyList<ReportData> ReportData)

            BaseReport(IEnumerable<ComponentConfig>  ComponentConfigs,
                       ReportBase                    ReportBase)

        {

            // A summary of "the most relevant" variables is not defined anywhere precisely
            // enough to build one, and OCPP 2.1 lets a station say so.
            var fullInventory = ReportBase.Equals(ReportBase.FullInventory);

            if (!fullInventory &&
                !ReportBase.Equals(ReportBase.ConfigurationInventory))
            {
                return (GenericDeviceModelStatus.NotSupported, []);
            }

            var reportData = ToReportData(
                                 ComponentConfigs,
                                 variableConfig => fullInventory ||
                                                   variableConfig.Attributes?.Mutability != MutabilityTypes.ReadOnly
                             );

            return (reportData.Count > 0
                        ? GenericDeviceModelStatus.Accepted
                        : GenericDeviceModelStatus.EmptyResultSet,
                    reportData);

        }

        #endregion

        #region Report              (ComponentConfigs, ComponentCriteria, ComponentVariables)

        /// <summary>
        /// The report data of a custom report.
        /// </summary>
        /// <remarks>
        /// A component variable without an instance or EVSE asks for every instance and
        /// every EVSE, one without a variable for all of the component's variables. A
        /// component criterion keeps a component whose variable of that name is true.
        /// Without either, everything is reported.
        /// </remarks>
        /// <param name="ComponentConfigs">All component configurations of the node.</param>
        /// <param name="ComponentCriteria">The criteria a reported component meets at least one of.</param>
        /// <param name="ComponentVariables">The components and variables to report.</param>
        public static (GenericDeviceModelStatus Status, IReadOnlyList<ReportData> ReportData)

            Report(IEnumerable<ComponentConfig>    ComponentConfigs,
                   IEnumerable<ComponentCriteria>  ComponentCriteria,
                   IEnumerable<ComponentVariable>  ComponentVariables)

        {

            var criteria     = ComponentCriteria. ToList();
            var asked        = ComponentVariables.ToList();

            var components   = ComponentConfigs.Where(componentConfig => criteria.Count == 0 ||
                                                                         criteria.Any(criterion => IsTrue(componentConfig, criterion.ToString()))).
                                                ToList();

            var reportData   = asked.Count == 0

                                   ? ToReportData(components)

                                   : [.. components.SelectMany(componentConfig => componentConfig.VariableConfigs.
                                                                                      Where(variableConfig => asked.Any(componentVariable => Matches(componentConfig, variableConfig, componentVariable))).
                                                                                      Select(variableConfig => ToReportData(componentConfig, variableConfig)))];

            return (reportData.Count > 0
                        ? GenericDeviceModelStatus.Accepted
                        : GenericDeviceModelStatus.EmptyResultSet,
                    reportData);

        }

        #endregion

        #region SendReportAsync     (ReportData, PartSize, SendPart)

        /// <summary>
        /// Send the given report data in parts of at most the given size, each one
        /// numbered and marked whether more follow.
        /// </summary>
        /// <param name="ReportData">The report data.</param>
        /// <param name="PartSize">The largest number of report data in one part.</param>
        /// <param name="SendPart">Send one part as a NotifyReport request.</param>
        public static async Task SendReportAsync(IReadOnlyList<ReportData>  ReportData,
                                                 UInt16                     PartSize,
                                                 NotifyReportPartDelegate   SendPart)
        {

            var parts = ReportData.Chunk(Math.Max((UInt16) 1, PartSize)).ToList();

            for (var sequenceNumber = 0; sequenceNumber < parts.Count; sequenceNumber++)
                await SendPart(
                          (UInt32) sequenceNumber,
                          parts[sequenceNumber],
                          sequenceNumber < parts.Count - 1
                      ).ConfigureAwait(false);

        }

        #endregion


        #region (private static) GetVariable        (GetVariableData, ComponentConfigs)

        private static GetVariableResult GetVariable(GetVariableData                GetVariableData,
                                                     ComponentConfigLookupDelegate  ComponentConfigs)
        {

            var component = GetVariableData.Component;
            var variable  = GetVariableData.Variable;

            if (!TryFind(component, variable, ComponentConfigs, out var componentConfig, out var variableConfig, out var notFound))
                return new GetVariableResult(
                           AttributeStatus:       componentConfig is null
                                                      ? GetVariableStatus.UnknownComponent
                                                      : GetVariableStatus.UnknownVariable,
                           Component:             component,
                           Variable:              variable,
                           AttributeStatusInfo:   notFound
                       );

            var attributeType = variableConfig.Attributes?.Type ?? AttributeTypes.Actual;

            if ((GetVariableData.AttributeType ?? AttributeTypes.Actual) != attributeType)
                return new GetVariableResult(
                           AttributeStatus:       GetVariableStatus.NotSupportedAttributeType,
                           Component:             component,
                           Variable:              variable,
                           AttributeType:         GetVariableData.AttributeType,
                           AttributeStatusInfo:   new StatusInfo(
                                                      "UnsupportedParam",
                                                      $"{NameOf(component, variable)} has no {GetVariableData.AttributeType} attribute!"
                                                  )
                       );

            // A write-only variable, like a password, is never read out.
            if (variableConfig.Attributes?.Mutability == MutabilityTypes.WriteOnly)
                return new GetVariableResult(
                           AttributeStatus:       GetVariableStatus.Rejected,
                           Component:             component,
                           Variable:              variable,
                           AttributeType:         attributeType,
                           AttributeStatusInfo:   new StatusInfo(
                                                      "WriteOnly",
                                                      $"{NameOf(component, variable)} is write-only!"
                                                  )
                       );

            return new GetVariableResult(
                       AttributeStatus:       GetVariableStatus.Accepted,
                       Component:             component,
                       Variable:              variable,
                       AttributeValue:        variableConfig.Value,
                       AttributeType:         attributeType
                   );

        }

        #endregion

        #region (private static) SetVariableAsync   (SetVariableData, ComponentConfigs, CancellationToken)

        private static async Task<SetVariableResult> SetVariableAsync(SetVariableData                SetVariableData,
                                                                      ComponentConfigLookupDelegate  ComponentConfigs,
                                                                      CancellationToken              CancellationToken)
        {

            var component = SetVariableData.Component;
            var variable  = SetVariableData.Variable;

            if (!TryFind(component, variable, ComponentConfigs, out var componentConfig, out var variableConfig, out var notFound))
                return new SetVariableResult(
                           AttributeStatus:       componentConfig is null
                                                      ? SetVariableStatus.UnknownComponent
                                                      : SetVariableStatus.UnknownVariable,
                           Component:             component,
                           Variable:              variable,
                           AttributeType:         SetVariableData.AttributeType,
                           AttributeStatusInfo:   notFound
                       );

            var attributeType = variableConfig.Attributes?.Type ?? AttributeTypes.Actual;

            if ((SetVariableData.AttributeType ?? AttributeTypes.Actual) != attributeType)
                return new SetVariableResult(
                           AttributeStatus:       SetVariableStatus.NotSupportedAttributeType,
                           Component:             component,
                           Variable:              variable,
                           AttributeType:         SetVariableData.AttributeType,
                           AttributeStatusInfo:   new StatusInfo(
                                                      "UnsupportedParam",
                                                      $"{NameOf(component, variable)} has no {SetVariableData.AttributeType} attribute!"
                                                  )
                       );

            if (variableConfig.Attributes?.Mutability == MutabilityTypes.ReadOnly)
                return new SetVariableResult(
                           AttributeStatus:       SetVariableStatus.Rejected,
                           Component:             component,
                           Variable:              variable,
                           AttributeType:         attributeType,
                           AttributeStatusInfo:   new StatusInfo(
                                                      "ReadOnly",
                                                      $"{NameOf(component, variable)} is read-only!"
                                                  )
                       );

            ValueSetterResponse response;

            try
            {

                response = await variableConfig.SetAsync(
                                     SetVariableData.AttributeValue,
                                     SetVariableData.OldAttributeValue,
                                     CancellationToken
                                 ).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                response = ValueSetterResponse.Rejected(
                               variableConfig.Value,
                               "InternalError",
                               $"{NameOf(component, variable)} could not be set: {e.Message}"
                           );
            }

            return new SetVariableResult(
                       AttributeStatus:       response.Status,
                       Component:             component,
                       Variable:              variable,
                       AttributeType:         attributeType,
                       AttributeStatusInfo:   response.StatusInfo
                   );

        }

        #endregion

        #region (private static) TryFind            (Component, Variable, ComponentConfigs, out ComponentConfig, out VariableConfig, out NotFound)

        private static Boolean TryFind(Component                                   Component,
                                       Variable                                    Variable,
                                       ComponentConfigLookupDelegate               ComponentConfigs,
                                       out ComponentConfig?                        ComponentConfig,
                                       [NotNullWhen(true)]  out VariableConfig?    VariableConfig,
                                       [NotNullWhen(false)] out StatusInfo?        NotFound)
        {

            VariableConfig   = null;
            NotFound         = null;

            ComponentConfig  = ComponentConfigs(Component.Name, Component.EVSE).
                                   FirstOrDefault(componentConfig => ComponentConfigStore.SameInstance(componentConfig.Instance, Component.Instance));

            if (ComponentConfig is null)
            {
                NotFound = new StatusInfo(
                               "NotFound",
                               $"Unknown component '{Component.Name}{(Component.Instance is not null ? $"({Component.Instance})" : "")}'!"
                           );
                return false;
            }

            var variablesOfThatName = ComponentConfig.VariableConfigs.
                                          Where(variableConfig => String.Equals(variableConfig.Name, Variable.Name, StringComparison.OrdinalIgnoreCase)).
                                          ToList();

            VariableConfig = variablesOfThatName.FirstOrDefault(variableConfig => ComponentConfigStore.SameInstance(variableConfig.Instance, Variable.Instance));

            if (VariableConfig is null)
            {
                NotFound = new StatusInfo(
                               "NotFound",
                               variablesOfThatName.Count == 0
                                   ? $"Unknown variable '{Variable.Name}' of component '{Component.Name}'!"
                                   : $"Unknown instance '{Variable.Instance}' of variable '{Variable.Name}' of component '{Component.Name}'!"
                           );
                return false;
            }

            return true;

        }

        #endregion

        #region (private static) ToReportData       (ComponentConfigs, Include = null)

        private static IReadOnlyList<ReportData> ToReportData(IEnumerable<ComponentConfig>   ComponentConfigs,
                                                              Func<VariableConfig, Boolean>? Include   = null)

            => [.. ComponentConfigs.
                       OrderBy (componentConfig => componentConfig.Name,           StringComparer.OrdinalIgnoreCase).
                       ThenBy  (componentConfig => componentConfig.EVSE?.Id.Value).
                       ThenBy  (componentConfig => componentConfig.Instance ?? "", StringComparer.OrdinalIgnoreCase).
                       SelectMany(componentConfig => componentConfig.VariableConfigs.
                                                         Where (variableConfig => Include is null || Include(variableConfig)).
                                                         Select(variableConfig => ToReportData(componentConfig, variableConfig)))];

        #endregion

        #region (private static) ToReportData       (ComponentConfig, VariableConfig)

        private static ReportData ToReportData(ComponentConfig  ComponentConfig,
                                               VariableConfig   VariableConfig)
        {

            var mutability = VariableConfig.Attributes?.Mutability ?? MutabilityTypes.ReadWrite;

            return new ReportData(
                       Component:                new Component(
                                                     ComponentConfig.Name,
                                                     ComponentConfig.Instance,
                                                     ComponentConfig.EVSE
                                                 ),
                       Variable:                 new Variable(
                                                     VariableConfig.Name,
                                                     VariableConfig.Instance
                                                 ),
                       VariableAttributes:       [
                                                     new VariableAttribute(
                                                         Type:         VariableConfig.Attributes?.Type ?? AttributeTypes.Actual,
                                                         // A write-only variable, like a password, is never read out.
                                                         Value:        mutability == MutabilityTypes.WriteOnly
                                                                           ? null
                                                                           : ValueOf(VariableConfig),
                                                         Mutability:   mutability,
                                                         Persistent:   VariableConfig.Attributes?.Persistent,
                                                         Constant:     VariableConfig.Attributes?.Constant
                                                     )
                                                 ],
                       VariableCharacteristics:  VariableConfig.Characteristics
                   );

        }

        #endregion

        #region (private static) ValueOf            (VariableConfig)

        /// <summary>
        /// The value of the given variable, or none when its getter fails:
        /// one variable must not cost the whole report.
        /// </summary>
        private static String? ValueOf(VariableConfig VariableConfig)
        {
            try
            {
                return VariableConfig.Value;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region (private static) Matches            (ComponentConfig, VariableConfig, ComponentVariable)

        private static Boolean Matches(ComponentConfig    ComponentConfig,
                                       VariableConfig     VariableConfig,
                                       ComponentVariable  ComponentVariable)

            => String.Equals(ComponentConfig.Name, ComponentVariable.Component.Name, StringComparison.OrdinalIgnoreCase) &&

               (ComponentVariable.Component.Instance is null ||
                ComponentConfigStore.SameInstance(ComponentConfig.Instance, ComponentVariable.Component.Instance))       &&

               (ComponentVariable.Component.EVSE is null ||
                (ComponentConfig.EVSE?.Id == ComponentVariable.Component.EVSE.Id &&
                 (!ComponentVariable.Component.EVSE.ConnectorId.HasValue ||
                  ComponentConfig.EVSE?.ConnectorId == ComponentVariable.Component.EVSE.ConnectorId)))                     &&

               (ComponentVariable.Variable is null ||
                (String.Equals(VariableConfig.Name, ComponentVariable.Variable.Name, StringComparison.OrdinalIgnoreCase) &&
                 (ComponentVariable.Variable.Instance is null ||
                  ComponentConfigStore.SameInstance(VariableConfig.Instance, ComponentVariable.Variable.Instance))));

        #endregion

        #region (private static) IsTrue             (ComponentConfig, VariableName)

        private static Boolean IsTrue(ComponentConfig  ComponentConfig,
                                      String           VariableName)
        {

            var value = ComponentConfig.VariableConfigs.
                            FirstOrDefault(variableConfig => String.Equals(variableConfig.Name, VariableName, StringComparison.OrdinalIgnoreCase) &&
                                                             variableConfig.Instance is null &&
                                                             variableConfig.Attributes?.Mutability != MutabilityTypes.WriteOnly) is VariableConfig variable
                            ? ValueOf(variable)
                            : null;

            return String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";

        }

        #endregion

        #region (private static) NameOf             (Component, Variable)

        private static String NameOf(Component  Component,
                                     Variable   Variable)

            => $"{Component.Name}{(Component.Instance is not null ? $"({Component.Instance})" : "")}/" +
               $"{Variable. Name}{(Variable. Instance is not null ? $"({Variable. Instance})" : "")}";

        #endregion

    }

}
