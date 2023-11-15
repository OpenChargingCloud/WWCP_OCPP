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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Set monitoring data.
    /// </summary>
    public class SetMonitoringData : ACustomData,
                                     IEquatable<SetMonitoringData>
    {

        #region Properties

        /// <summary>
        /// The value for threshold or delta monitoring.
        /// For periodic or "periodic clock aligned" variable monitors this is the interval in seconds.
        /// </summary>
        [Mandatory]
        public Decimal                         Value                            { get; }

        /// <summary>
        /// The type of this monitor, e.g. a threshold, delta or periodic monitor.
        /// </summary>
        [Mandatory]
        public MonitorType                     MonitorType                      { get; }

        /// <summary>
        /// The severity that will be assigned to an event that is triggered by this monitor.
        /// </summary>
        [Mandatory]
        public Severities                      Severity                         { get; }

        /// <summary>
        /// The component for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Component                       Component                        { get; }

        /// <summary>
        /// The variable for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Variable                        Variable                         { get; }

        /// <summary>
        /// An optional unique monitoring identification for replace an existing variable monitor.
        /// The charging station handles the generation of monitoring identifications for new monitors.
        /// </summary>
        [Optional]
        public VariableMonitoring_Id?          VariableMonitoringId             { get; }

        /// <summary>
        /// This variable monitoring is only active when a transaction is ongoing on a setMonitoringData relevant to this transaction.
        /// Default: false
        /// </summary>
        [Optional]
        public Boolean?                        Transaction                      { get; }

        /// <summary>
        /// When present events from a periodic monitor (Periodic or PeriodicClockAligned)
        /// will be sent via a periodic event stream.
        /// </summary>
        [Optional]
        public PeriodicEventStreamParameters?  PeriodicEventStreamParameters    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new set monitoring data.
        /// </summary>
        /// <param name="Value">The value for threshold or delta monitoring. For periodic or "periodic clock aligned" variable monitors this is the interval in seconds.</param>
        /// <param name="MonitorType">The type of this monitor, e.g. a threshold, delta or periodic monitor.</param>
        /// <param name="Severity">The severity that will be assigned to an event that is triggered by this monitor.</param>
        /// <param name="Component">The setMonitoringData for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="VariableMonitoringId">An optional unique monitoring identification for replace an existing variable monitor. The charging station handles the generation of monitoring identifications for new monitors.</param>
        /// <param name="Transaction">This variable monitoring is only active when a transaction is ongoing on a setMonitoringData relevant to this transaction. Default: false</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetMonitoringData(Decimal                         Value,
                                 MonitorType                     MonitorType,
                                 Severities                      Severity,
                                 Component                       Component,
                                 Variable                        Variable,
                                 VariableMonitoring_Id?          VariableMonitoringId            = null,
                                 Boolean?                        Transaction                     = null,
                                 PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                 CustomData?                     CustomData                      = null)

            : base(CustomData)

        {

            this.Value                          = Value;
            this.MonitorType                    = MonitorType;
            this.Severity                       = Severity;
            this.Component                      = Component;
            this.Variable                       = Variable;
            this.VariableMonitoringId           = VariableMonitoringId;
            this.PeriodicEventStreamParameters  = PeriodicEventStreamParameters;
            this.Transaction                    = Transaction;


            unchecked
            {

                hashCode = this.Value.                         GetHashCode()       * 23 ^
                           this.MonitorType.                   GetHashCode()       * 19 ^
                           this.Severity.                      GetHashCode()       * 17 ^
                           this.Component.                     GetHashCode()       * 13 ^
                           this.Variable.                      GetHashCode()       * 11 ^
                          (this.VariableMonitoringId?.         GetHashCode() ?? 0) *  7 ^
                          (this.Transaction?.                  GetHashCode() ?? 0) *  5 ^
                          (this.PeriodicEventStreamParameters?.GetHashCode() ?? 0) *  3 ^
                           base.                               GetHashCode();

            }

        }

        #endregion


        #region Static methods...

        #region UpperThreshold(...)

        /// <summary>
        /// Triggers an event notice when the actual value of the variable rises above the "monitorValue".
        /// </summary>
        public static SetMonitoringData UpperThreshold(Decimal                         Value,
                                                       Severities                      Severity,
                                                       Component                       Component,
                                                       Variable                        Variable,
                                                       VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                       Boolean?                        Transaction                     = null,
                                                       PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                       CustomData?                     CustomData                      = null)

            => new (Value,
                    MonitorType.UpperThreshold,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region LowerThreshold(...)

        /// <summary>
        /// Triggers an event notice when the actual value of the variable drops below "monitorValue".
        /// </summary>
        public static SetMonitoringData LowerThreshold(Decimal                         Value,
                                                       Severities                      Severity,
                                                       Component                       Component,
                                                       Variable                        Variable,
                                                       VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                       Boolean?                        Transaction                     = null,
                                                       PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                       CustomData?                     CustomData                      = null)

            => new (Value,
                    MonitorType.LowerThreshold,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region Delta(...)

        /// <summary>
        /// Triggers an event notice when the actual value has changed more than plus or minus "monitorValue" since the
        /// time that this monitor was set or since the last time this event notice was sent, whichever was last.
        /// For variables that are not numeric, like boolean, string or enumerations, a monitor of type Delta will
        /// trigger an event notice whenever the variable changes, regardless of the value of monitorValue.
        /// </summary>
        public static SetMonitoringData Delta(Decimal                         Value,
                                              Severities                      Severity,
                                              Component                       Component,
                                              Variable                        Variable,
                                              VariableMonitoring_Id?          VariableMonitoringId            = null,
                                              Boolean?                        Transaction                     = null,
                                              PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                              CustomData?                     CustomData                      = null)

            => new (Value,
                    MonitorType.LowerThreshold,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region Periodic(...)

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the time that this monitor was set.
        /// </summary>
        public static SetMonitoringData Periodic(TimeSpan                        Interval,
                                                 Severities                      Severity,
                                                 Component                       Component,
                                                 Variable                        Variable,
                                                 VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                 Boolean?                        Transaction                     = null,
                                                 PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                 CustomData?                     CustomData                      = null)

            => new ((Int32) Math.Round(Interval.TotalSeconds, 0),
                    MonitorType.Periodic,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region PeriodicClockAligned(...)

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the nearest clock-aligned interval
        /// after this monitor was set.
        /// </summary>
        /// <example>A monitorValue of 900 will trigger event notices at 0, 15, 30 and 45 minutes after the hour, every hour.</example>
        public static SetMonitoringData PeriodicClockAligned(TimeSpan                        Interval,
                                                             Severities                      Severity,
                                                             Component                       Component,
                                                             Variable                        Variable,
                                                             VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                             Boolean?                        Transaction                     = null,
                                                             PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                             CustomData?                     CustomData                      = null)

            => new ((Int32) Math.Round(Interval.TotalSeconds, 0),
                    MonitorType.PeriodicClockAligned,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region TargetDelta(...)

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus monitorValue since the time that this monitor was set or since the last time this
        /// event notice was sent, whichever was last. Behavior of this type of monitor for a variable that
        /// is not numeric, is not defined.
        /// </summary>
        /// <example>When target = 100, monitorValue = 10, then an event is triggered when actual &lt; 90 or actual &gt; 110.</example>
        public static SetMonitoringData TargetDelta(TimeSpan                        Interval,
                                                    Severities                      Severity,
                                                    Component                       Component,
                                                    Variable                        Variable,
                                                    VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                    Boolean?                        Transaction                     = null,
                                                    PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                    CustomData?                     CustomData                      = null)

            => new ((Int32) Math.Round(Interval.TotalSeconds, 0),
                    MonitorType.TargetDelta,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #region TargetDeltaRelative(...)

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus (monitorValue * target value) since the time that this monitor was set or since the
        /// last time this event notice was sent, whichever was last. Behavior of this type of monitor for a
        /// variable that is not numeric, is not defined.
        /// </summary>
        /// <example>When target = 100, monitorValue = 0.1, then an event is triggered when actual &lt; 90 or actual &gt; 110.</example>
        public static SetMonitoringData TargetDeltaRelative(TimeSpan                        Interval,
                                                            Severities                      Severity,
                                                            Component                       Component,
                                                            Variable                        Variable,
                                                            VariableMonitoring_Id?          VariableMonitoringId            = null,
                                                            Boolean?                        Transaction                     = null,
                                                            PeriodicEventStreamParameters?  PeriodicEventStreamParameters   = null,
                                                            CustomData?                     CustomData                      = null)

            => new ((Int32) Math.Round(Interval.TotalSeconds, 0),
                    MonitorType.TargetDeltaRelative,
                    Severity,
                    Component,
                    Variable,
                    VariableMonitoringId,
                    Transaction,
                    PeriodicEventStreamParameters,
                    CustomData);

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // "SetMonitoringDataType": {
        //   "description": "Class to hold parameters of SetVariableMonitoring request.\r\n",
        //   "javaType": "SetMonitoringData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "An id SHALL only be given to replace an existing monitor. The Charging Station handles the generation of id's for new monitors.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "transaction": {
        //       "description": "Monitor only active when a transaction is ongoing on a component relevant to this transaction. Default = false.\r\n\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "value": {
        //       "description": "Value for threshold or delta monitoring.\r\nFor Periodic or PeriodicClockAligned this is the interval in seconds.\r\n\r\n",
        //       "type": "number"
        //     },
        //     "type": {
        //       "$ref": "#/definitions/MonitorEnumType"
        //     },
        //     "severity": {
        //       "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "component": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "variable": {
        //       "$ref": "#/definitions/VariableType"
        //     }
        //   },
        //   "required": [
        //     "value",
        //     "type",
        //     "severity",
        //     "component",
        //     "variable"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSetMonitoringDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of set monitoring data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetMonitoringDataParser">A delegate to parse custom set monitoring data JSON objects.</param>
        public static SetMonitoringData Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<SetMonitoringData>?  CustomSetMonitoringDataParser   = null)
        {

            if (TryParse(JSON,
                         out var setMonitoringData,
                         out var errorResponse,
                         CustomSetMonitoringDataParser) &&
                setMonitoringData is not null)
            {
                return setMonitoringData;
            }

            throw new ArgumentException("The given JSON representation of set monitoring data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetMonitoringData, CustomSetMonitoringDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a setMonitoringData.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringData">The parsed setMonitoringData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out SetMonitoringData?  SetMonitoringData,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        out SetMonitoringData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a setMonitoringData.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringData">The parsed setMonitoringData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringDataParser">A delegate to parse custom setMonitoringData JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out SetMonitoringData?                           SetMonitoringData,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<SetMonitoringData>?  CustomSetMonitoringDataParser)
        {

            try
            {

                SetMonitoringData = default;

                #region Value                            [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MonitorType                      [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "monitor type",
                                         OCPPv2_1.MonitorType.TryParse,
                                         out MonitorType MonitorType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Severity                         [mandatory]

                if (!JSON.ParseMandatory("severity",
                                         "severity",
                                         out Byte severity,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Severity = SeveritiesExtensions.TryParse(severity);

                if (!Severity.HasValue)
                    return false;

                #endregion

                #region Component                        [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse) ||
                     Component is null)
                {
                    return false;
                }

                #endregion

                #region Variable                         [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse) ||
                     Variable is null)
                {
                    return false;
                }

                #endregion

                #region VariableMonitoringId             [optional]

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

                #region Transaction                      [optional]

                if (JSON.ParseOptional("transaction",
                                       "transaction",
                                       out Boolean? Transaction,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PeriodicEventStreamParameters    [optional]

                if (JSON.ParseOptionalJSON("periodicEventStream",
                                           "periodic event stream",
                                           OCPPv2_1.PeriodicEventStreamParameters.TryParse,
                                           out PeriodicEventStreamParameters? PeriodicEventStreamParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                       [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringData = new SetMonitoringData(
                                        Value,
                                        MonitorType,
                                        Severity.Value,
                                        Component,
                                        Variable,
                                        VariableMonitoringId,
                                        Transaction,
                                        PeriodicEventStreamParameters,
                                        CustomData
                                    );

                if (CustomSetMonitoringDataParser is not null)
                    SetMonitoringData = CustomSetMonitoringDataParser(JSON,
                                                                      SetMonitoringData);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringData  = default;
                ErrorResponse      = "The given JSON representation of set monitoring data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringDataSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringDataSerializer">A delegate to serialize custom set monitoring data.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringData>?              CustomSetMonitoringDataSerializer               = null,
                              CustomJObjectSerializerDelegate<Component>?                      CustomComponentSerializer                       = null,
                              CustomJObjectSerializerDelegate<EVSE>?                           CustomEVSESerializer                            = null,
                              CustomJObjectSerializerDelegate<Variable>?                       CustomVariableSerializer                        = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("value",                 Value),

                                 new JProperty("type",                  MonitorType.ToString()),

                                 new JProperty("severity",              Severity.   AsNumber()),

                                 new JProperty("component",             Component.                    ToJSON(CustomComponentSerializer,
                                                                                                             CustomEVSESerializer,
                                                                                                             CustomCustomDataSerializer)),

                                 new JProperty("variable",              Variable.                     ToJSON(CustomVariableSerializer,
                                                                                                             CustomCustomDataSerializer)),

                           VariableMonitoringId.HasValue
                               ? new JProperty("id",                    VariableMonitoringId.Value.Value)
                               : null,

                           Transaction.         HasValue
                               ? new JProperty("transaction",           Transaction.               Value)
                               : null,

                           PeriodicEventStreamParameters is not null
                               ? new JProperty("periodicEventStream",   PeriodicEventStreamParameters.ToJSON(CustomPeriodicEventStreamParametersSerializer,
                                                                                                             CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.                   ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetMonitoringDataSerializer is not null
                       ? CustomSetMonitoringDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringData1, SetMonitoringData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetMonitoringData1">Set monitoring data.</param>
        /// <param name="SetMonitoringData2">Other set monitoring data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetMonitoringData? SetMonitoringData1,
                                           SetMonitoringData? SetMonitoringData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringData1, SetMonitoringData2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringData1 is null || SetMonitoringData2 is null)
                return false;

            return SetMonitoringData1.Equals(SetMonitoringData2);

        }

        #endregion

        #region Operator != (SetMonitoringData1, SetMonitoringData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetMonitoringData1">Set monitoring data.</param>
        /// <param name="SetMonitoringData2">Other set monitoring data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetMonitoringData? SetMonitoringData1,
                                           SetMonitoringData? SetMonitoringData2)

            => !(SetMonitoringData1 == SetMonitoringData2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set monitoring data for equality.
        /// </summary>
        /// <param name="Object">Set monitoring data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringData setMonitoringData &&
                   Equals(setMonitoringData);

        #endregion

        #region Equals(SetMonitoringData)

        /// <summary>
        /// Compares two set monitoring data for equality.
        /// </summary>
        /// <param name="SetMonitoringData">Set monitoring data to compare with.</param>
        public Boolean Equals(SetMonitoringData? SetMonitoringData)

            => SetMonitoringData is not null &&

               Value.      Equals(SetMonitoringData.Value)       &&
               MonitorType.Equals(SetMonitoringData.MonitorType) &&
               Severity.   Equals(SetMonitoringData.Severity)    &&
               Component.  Equals(SetMonitoringData.Component)   &&
               Variable.   Equals(SetMonitoringData.Variable)    &&

            ((!VariableMonitoringId.         HasValue    && !SetMonitoringData.VariableMonitoringId.         HasValue) ||
               VariableMonitoringId.         HasValue    &&  SetMonitoringData.VariableMonitoringId.         HasValue    && VariableMonitoringId.   Value.Equals(SetMonitoringData.VariableMonitoringId.Value))    &&

            ((!Transaction.                  HasValue    && !SetMonitoringData.Transaction.                  HasValue) ||
               Transaction.                  HasValue    &&  SetMonitoringData.Transaction.                  HasValue    && Transaction.            Value.Equals(SetMonitoringData.Transaction.         Value))    &&

             ((PeriodicEventStreamParameters is null     &&  SetMonitoringData.PeriodicEventStreamParameters is null)  ||
               PeriodicEventStreamParameters is not null &&  SetMonitoringData.PeriodicEventStreamParameters is not null && PeriodicEventStreamParameters.Equals(SetMonitoringData.PeriodicEventStreamParameters)) &&

               base.       Equals(SetMonitoringData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Value,
                   " (", MonitorType,
                   ", ", Severity,

                   VariableMonitoringId.HasValue
                       ? ", " + VariableMonitoringId.ToString()
                       : "",

                   Transaction.HasValue
                       ? Transaction.Value
                             ? ", transaction"
                             : ""
                       : "",

                   ")",

                   ", Component: " + Component.ToString(),
                   ", Variable: "  + Variable. ToString()

               );

        #endregion

    }

}
