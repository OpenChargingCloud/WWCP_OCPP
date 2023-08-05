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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// The result of a set variable request.
    /// </summary>
    public class SetVariableResult : ACustomData,
                                     IEquatable<SetVariableResult>
    {

        #region Properties

        /// <summary>
        /// The status of the set variable request.
        /// </summary>
        [Mandatory]
        public SetVariableStatus  Status                  { get; }

        /// <summary>
        /// The component for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Component          Component               { get; }

        /// <summary>
        /// The variable for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Variable           Variable                { get; }

        /// <summary>
        /// The optional type of the attribute: Actual, Target, MinSet, MaxSet.
        /// [Default: actual]
        /// </summary>
        [Optional]
        public AttributeTypes?    AttributeType          { get; }

        /// <summary>
        /// Optional detailed attribute status information.
        /// </summary>
        [Optional]
        public StatusInfo?        AttributeStatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set variable result.
        /// </summary>
        /// <param name="Status">The result status of the set variable request.</param>
        /// <param name="Component">The component for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="AttributeType">The optional type of the attribute: Actual, Target, MinSet, MaxSet [Default: actual]</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetVariableResult(SetVariableStatus  Status,
                                 Component          Component,
                                 Variable           Variable,
                                 AttributeTypes?    AttributeType         = null,
                                 StatusInfo?        AttributeStatusInfo   = null,
                                 CustomData?        CustomData            = null)

            : base(CustomData)

        {

            this.Status               = Status;
            this.Component            = Component;
            this.Variable             = Variable;
            this.AttributeType        = AttributeType;
            this.AttributeStatusInfo  = AttributeStatusInfo;

        }

        #endregion


        #region Documentation

        // "SetVariableResultType": {
        //   "description": "Class to hold result of SetVariableMonitoring request.\r\n",
        //   "javaType": "SetVariableResult",
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
        //       "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n\r\n",
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

        #region (static) Parse   (JSON, CustomSetVariableResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetVariableResultParser">A delegate to parse custom set variable result JSON objects.</param>
        public static SetVariableResult Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<SetVariableResult>?  CustomSetVariableResultParser   = null)
        {

            if (TryParse(JSON,
                         out var setVariableResult,
                         out var errorResponse,
                         CustomSetVariableResultParser))
            {
                return setVariableResult!;
            }

            throw new ArgumentException("The given JSON representation of a set variable result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetVariableResult, CustomSetVariableResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a set variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetVariableResult">The parsed set variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out SetVariableResult?  SetVariableResult,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        out SetVariableResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a set variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetVariableResult">The parsed set variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetVariableResultParser">A delegate to parse custom set variable result JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out SetVariableResult?                           SetVariableResult,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<SetVariableResult>?  CustomSetVariableResultParser)
        {

            try
            {

                SetVariableResult = default;

                #region Status                 [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         SetVariableStatusExtensions.TryParse,
                                         out SetVariableStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component              [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_0_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Component is null)
                    return false;

                #endregion

                #region Variable               [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_0_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Variable is null)
                    return false;

                #endregion

                #region AttributeType          [optional]

                if (JSON.ParseOptional("attributeType",
                                       "attribute type",
                                       AttributeTypesExtensions.TryParse,
                                       out AttributeTypes? AttributeType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AttributeStatusInfo    [optional]

                if (JSON.ParseOptionalJSON("attributeStatusInfo",
                                           "detailed attribute status info",
                                           StatusInfo.TryParse,
                                           out StatusInfo? AttributeStatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetVariableResult = new SetVariableResult(Status,
                                                          Component,
                                                          Variable,
                                                          AttributeType,
                                                          AttributeStatusInfo,
                                                          CustomData);

                if (CustomSetVariableResultParser is not null)
                    SetVariableResult = CustomSetVariableResultParser(JSON,
                                                                      SetVariableResult);

                return true;

            }
            catch (Exception e)
            {
                SetVariableResult  = default;
                ErrorResponse      = "The given JSON representation of a set variable result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetVariableResultSerializer = null, CustomComponentSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetVariableResultSerializer">A delegate to serialize custom set variable results.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetVariableResult>?  CustomSetVariableResultSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?          CustomComponentSerializer           = null,
                              CustomJObjectSerializerDelegate<EVSE>?               CustomEVSESerializer                = null,
                              CustomJObjectSerializerDelegate<Variable>?           CustomVariableSerializer            = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?         CustomStatusInfoSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",                Status.             AsText()),

                                 new JProperty("component",             Component.          ToJSON(CustomComponentSerializer,
                                                                                                   CustomEVSESerializer,
                                                                                                   CustomCustomDataSerializer)),

                                 new JProperty("variable",              Variable.           ToJSON(CustomVariableSerializer,
                                                                                                   CustomCustomDataSerializer)),

                           AttributeType.HasValue
                               ? new JProperty("attributeType",         AttributeType.Value.AsText())
                               : null,

                           AttributeStatusInfo is not null
                               ? new JProperty("attributeStatusInfo",   AttributeStatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                                   CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetVariableResultSerializer is not null
                       ? CustomSetVariableResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetVariableResult1, SetVariableResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetVariableResult1">A set variable result.</param>
        /// <param name="SetVariableResult2">Another set variable result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetVariableResult? SetVariableResult1,
                                           SetVariableResult? SetVariableResult2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetVariableResult1, SetVariableResult2))
                return true;

            // If one is null, but not both, return false.
            if (SetVariableResult1 is null || SetVariableResult2 is null)
                return false;

            return SetVariableResult1.Equals(SetVariableResult2);

        }

        #endregion

        #region Operator != (SetVariableResult1, SetVariableResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetVariableResult1">A set variable result.</param>
        /// <param name="SetVariableResult2">Another set variable result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetVariableResult? SetVariableResult1,
                                           SetVariableResult? SetVariableResult2)

            => !(SetVariableResult1 == SetVariableResult2);

        #endregion

        #endregion

        #region IEquatable<SetVariableResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set variable results for equality.
        /// </summary>
        /// <param name="Object">A set variable result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariableResult setVariableResult &&
                   Equals(setVariableResult);

        #endregion

        #region Equals(SetVariableResult)

        /// <summary>
        /// Compares two set variable results for equality.
        /// </summary>
        /// <param name="SetVariableResult">A set variable result to compare with.</param>
        public Boolean Equals(SetVariableResult? SetVariableResult)

            => SetVariableResult is not null &&

               Status.     Equals(Equals(SetVariableResult.Status))      &&
               Component.  Equals(Equals(SetVariableResult.Component))   &&
               Variable.   Equals(Equals(SetVariableResult.Variable))    &&

            ((!AttributeType.      HasValue    && !SetVariableResult.AttributeType.      HasValue)    ||
               AttributeType.      HasValue    &&  SetVariableResult.AttributeType.      HasValue    && AttributeType.Value.Equals(SetVariableResult.AttributeType.Value)) &&

             ((AttributeStatusInfo is     null &&  SetVariableResult.AttributeStatusInfo is     null) ||
               AttributeStatusInfo is not null &&  SetVariableResult.AttributeStatusInfo is not null && AttributeStatusInfo.Equals(SetVariableResult.AttributeStatusInfo)) &&

               base.       Equals(SetVariableResult);

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

                return Status.              GetHashCode()       * 13 ^
                       Component.           GetHashCode()       * 11 ^
                       Variable.            GetHashCode()       *  7 ^
                      (AttributeType?.      GetHashCode() ?? 0) *  5 ^
                      (AttributeStatusInfo?.GetHashCode() ?? 0) *  3 ^

                       base.                GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   "Component/variable: '",
                   Component.ToString(),
                   "' / '",
                   Variable. ToString(),
                   "': ",
                   Status.   AsText(),

                   AttributeType.HasValue
                       ? " [" + AttributeType.Value.AsText() + "]"
                       : ""

               );

        #endregion

    }

}
