/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// The result of a set variable monitoring request.
    /// </summary>
    public class SetMonitoringResult : ACustomData,
                                       IEquatable<SetMonitoringResult>
    {

        #region Properties

        /// <summary>
        /// The status of the set monitoring request.
        /// </summary>
        [Mandatory]
        public SetMonitoringStatus     Status                  { get; }

        /// <summary>
        /// The type of this monitor, e.g. a threshold, delta or periodic monitor.
        /// </summary>
        [Mandatory]
        public MonitorTypes            MonitorType             { get; }

        /// <summary>
        /// The severity that will be assigned to an event that is triggered by this monitor.
        /// </summary>
        [Mandatory]
        public Severities              Severity                { get; }

        /// <summary>
        /// The component for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Component               Component               { get; }

        /// <summary>
        /// The variable for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Variable                Variable                { get; }

        /// <summary>
        /// The optional unique variable monitoring identification given to the variable monitor by the charging station.
        /// The identification is only returned when the monitor was successfully created.
        /// </summary>
        [Optional]
        public VariableMonitoring_Id?  VariableMonitoringId    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?             StatusInfo              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set monitoring result.
        /// </summary>
        /// <param name="Status">The status of the set monitoring request.</param>
        /// <param name="MonitorType">The type of this monitor, e.g. a threshold, delta or periodic monitor.</param>
        /// <param name="Severity">The severity that will be assigned to an event that is triggered by this monitor.</param>
        /// <param name="Component">The component for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="VariableMonitoringId">The optional unique variable monitoring identification given to the variable monitor by the charging station. The identification is only returned when the monitor was successfully created.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetMonitoringResult(SetMonitoringStatus     Status,
                                   MonitorTypes            MonitorType,
                                   Severities              Severity,
                                   Component               Component,
                                   Variable                Variable,
                                   VariableMonitoring_Id?  VariableMonitoringId,
                                   StatusInfo?             StatusInfo,
                                   CustomData?             CustomData   = null)

            : base(CustomData)

        {

            this.Status                = Status;
            this.MonitorType           = MonitorType;
            this.Severity              = Severity;
            this.Component             = Component;
            this.Variable              = Variable;
            this.VariableMonitoringId  = VariableMonitoringId;
            this.StatusInfo            = StatusInfo;

        }

        #endregion


        #region Documentation

        // "SetMonitoringResultType": {
        //   "description": "Class to hold result of SetVariableMonitoring request.\r\n",
        //   "javaType": "SetMonitoringResult",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "Id given to the VariableMonitor by the Charging Station. The Id is only returned when status is accepted. Installed VariableMonitors should have unique id's but the id's of removed Installed monitors should have unique id's but the id's of removed monitors MAY be reused.\r\n",
        //       "type": "integer"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/SetMonitoringStatusEnumType"
        //     },
        //     "type": {
        //       "$ref": "#/definitions/MonitorEnumType"
        //     },
        //     "component": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "variable": {
        //       "$ref": "#/definitions/VariableType"
        //     },
        //     "severity": {
        //       "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status",
        //     "type",
        //     "severity",
        //     "component",
        //     "variable"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSetMonitoringResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetMonitoringResultParser">A delegate to parse custom set monitoring result JSON objects.</param>
        public static SetMonitoringResult Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<SetMonitoringResult>?  CustomSetMonitoringResultParser   = null)
        {

            if (TryParse(JSON,
                         out var setMonitoringResult,
                         out var errorResponse,
                         CustomSetMonitoringResultParser))
            {
                return setMonitoringResult!;
            }

            throw new ArgumentException("The given JSON representation of a set monitoring result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetMonitoringResult, CustomSetMonitoringResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a set monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringResult">The parsed set monitoring result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                   JSON,
                                       out SetMonitoringResult?  SetMonitoringResult,
                                       out String?               ErrorResponse)

            => TryParse(JSON,
                        out SetMonitoringResult,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of a set monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringResult">The parsed set monitoring result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringResultParser">A delegate to parse custom set monitoring result JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       out SetMonitoringResult?                           SetMonitoringResult,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<SetMonitoringResult>?  CustomSetMonitoringResultParser)
        {

            try
            {

                SetMonitoringResult = default;

                #region Status                  [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "set monitoring status",
                                         SetMonitoringStatusExtensions.TryParse,
                                         out SetMonitoringStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type                    [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "monitor type",
                                         MonitorTypesExtensions.TryParse,
                                         out MonitorTypes Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Severity                [mandatory]

                if (!JSON.ParseMandatory("severity",
                                         "severity",
                                         Severities.TryParse,
                                         out Severities Severity,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component               [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_0.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Component is null)
                    return false;

                #endregion

                #region Variable                [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_0.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Variable is null)
                    return false;

                #endregion

                #region VariableMonitoringId    [optional]

                if (JSON.ParseOptional("id",
                                       "variable monitoring identification",
                                       VariableMonitoring_Id.TryParse,
                                       out VariableMonitoring_Id? VariableMonitoringId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo              [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringResult = new SetMonitoringResult(Status,
                                                              Type,
                                                              Severity,
                                                              Component,
                                                              Variable,
                                                              VariableMonitoringId,
                                                              StatusInfo,
                                                              CustomData);

                if (CustomSetMonitoringResultParser is not null)
                    SetMonitoringResult = CustomSetMonitoringResultParser(JSON,
                                                                          SetMonitoringResult);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringResult  = default;
                ErrorResponse        = "The given JSON representation of a set monitoring result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringResultSerializer = null, CustomComponentSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringResultSerializer">A delegate to serialize custom set monitoring result objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringResult>?  CustomSetMonitoringResultSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?            CustomComponentSerializer             = null,
                              CustomJObjectSerializerDelegate<EVSE>?                 CustomEVSESerializer                  = null,
                              CustomJObjectSerializerDelegate<Variable>?             CustomVariableSerializer              = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?           CustomStatusInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("status",      Status.     AsText()),

                                 new JProperty("type",        MonitorType.AsText()),

                                 new JProperty("severity",    (Byte) Severity),

                                 new JProperty("component",   Component.  ToJSON(CustomComponentSerializer,
                                                                                 CustomEVSESerializer,
                                                                                 CustomCustomDataSerializer)),

                                 new JProperty("variable",    Variable.   ToJSON(CustomVariableSerializer,
                                                                                 CustomCustomDataSerializer)),

                           VariableMonitoringId.HasValue
                               ? new JProperty("id",          VariableMonitoringId.Value.Value)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo. ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetMonitoringResultSerializer is not null
                       ? CustomSetMonitoringResultSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringResult1, SetMonitoringResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetMonitoringResult1">A set monitoring result.</param>
        /// <param name="SetMonitoringResult2">Another set monitoring result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetMonitoringResult? SetMonitoringResult1,
                                           SetMonitoringResult? SetMonitoringResult2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringResult1, SetMonitoringResult2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringResult1 is null || SetMonitoringResult2 is null)
                return false;

            return SetMonitoringResult1.Equals(SetMonitoringResult2);

        }

        #endregion

        #region Operator != (SetMonitoringResult1, SetMonitoringResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetMonitoringResult1">A set monitoring result.</param>
        /// <param name="SetMonitoringResult2">Another set monitoring result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetMonitoringResult? SetMonitoringResult1,
                                           SetMonitoringResult? SetMonitoringResult2)

            => !(SetMonitoringResult1 == SetMonitoringResult2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set monitoring results for equality.
        /// </summary>
        /// <param name="Object">A set monitoring result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringResult setMonitoringResult &&
                   Equals(setMonitoringResult);

        #endregion

        #region Equals(SetMonitoringResult)

        /// <summary>
        /// Compares two set monitoring results for equality.
        /// </summary>
        /// <param name="SetMonitoringResult">A set monitoring result to compare with.</param>
        public Boolean Equals(SetMonitoringResult? SetMonitoringResult)

            => SetMonitoringResult is not null &&

               Status.     Equals(Equals(SetMonitoringResult.Status))      &&
               MonitorType.Equals(Equals(SetMonitoringResult.MonitorType)) &&
               Severity.   Equals(Equals(SetMonitoringResult.Severity))    &&
               Component.  Equals(Equals(SetMonitoringResult.Component))   &&
               Variable.   Equals(Equals(SetMonitoringResult.Variable))    &&

            ((!VariableMonitoringId.HasValue    && !SetMonitoringResult.VariableMonitoringId.HasValue)    ||
               VariableMonitoringId.HasValue    &&  SetMonitoringResult.VariableMonitoringId.HasValue    && VariableMonitoringId.Value.Equals(SetMonitoringResult.VariableMonitoringId.Value)) &&

             ((StatusInfo           is     null &&  SetMonitoringResult.StatusInfo           is     null) ||
               StatusInfo           is not null &&  SetMonitoringResult.StatusInfo           is not null && StatusInfo.                Equals(SetMonitoringResult.StatusInfo)) &&

               base.  Equals(SetMonitoringResult);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.               GetHashCode()       * 19 ^
                       MonitorType.          GetHashCode()       * 17 ^
                       Severity.             GetHashCode()       * 13 ^
                       Component.            GetHashCode()       * 11 ^
                       Variable.             GetHashCode()       *  7 ^
                      (VariableMonitoringId?.GetHashCode() ?? 0) *  5 ^
                      (StatusInfo?.          GetHashCode() ?? 0) *  3 ^

                       base.                 GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   VariableMonitoringId,
                   " (",            MonitorType.AsText(),
                   ", ",            Severity.   AsText(),
                   ", component: ", Component.  ToString(),
                   ", variable: ",  Variable.   ToString(),
                   "): ",           Status.     AsText()

               );

        #endregion

    }

}
