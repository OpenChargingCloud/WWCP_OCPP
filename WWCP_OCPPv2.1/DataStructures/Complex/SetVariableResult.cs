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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The result of a set variable request.
    /// </summary>
    public class SetVariableResult : ACustomData,
                                     IEquatable<SetVariableResult>
    {

        #region Properties

        /// <summary>
        /// The result status of setting the variable.
        /// </summary>
        [Mandatory]
        public SetVariableStatus  AttributeStatus         { get; }

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
        /// <param name="AttributeStatus">The result status of setting the variable.</param>
        /// <param name="Component">The component for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="AttributeType">The optional type of the attribute: Actual, Target, MinSet, MaxSet [Default: actual]</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetVariableResult(SetVariableStatus  AttributeStatus,
                                 Component          Component,
                                 Variable           Variable,
                                 AttributeTypes?    AttributeType         = null,
                                 StatusInfo?        AttributeStatusInfo   = null,
                                 CustomData?        CustomData            = null)

            : base(CustomData)

        {

            this.AttributeStatus      = AttributeStatus;
            this.Component            = Component;
            this.Variable             = Variable;
            this.AttributeType        = AttributeType;
            this.AttributeStatusInfo  = AttributeStatusInfo;

            unchecked
            {

                hashCode = this.AttributeStatus.     GetHashCode()       * 13 ^
                           this.Component.           GetHashCode()       * 11 ^
                           this.Variable.            GetHashCode()       *  7 ^
                          (this.AttributeType?.      GetHashCode() ?? 0) *  5 ^
                          (this.AttributeStatusInfo?.GetHashCode() ?? 0) *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "SetVariableResultType": {
        //   "javaType": "SetVariableResult",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "attributeType": {
        //       "$ref": "#/definitions/AttributeEnumType"
        //     },
        //     "attributeStatus": {
        //       "$ref": "#/definitions/SetVariableStatusEnumType"
        //     },
        //     "attributeStatusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "component": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "variable": {
        //       "$ref": "#/definitions/VariableType"
        //     }
        //   },
        //   "required": [
        //     "attributeStatus",
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
                         CustomSetVariableResultParser) &&
                setVariableResult is not null)
            {
                return setVariableResult;
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
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out SetVariableResult?  SetVariableResult,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

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
                                       [NotNullWhen(true)]  out SetVariableResult?      SetVariableResult,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<SetVariableResult>?  CustomSetVariableResultParser)
        {

            try
            {

                SetVariableResult = default;

                #region AttributeStatus        [mandatory]

                if (!JSON.ParseMandatory("attributeStatus",
                                         "attribute status",
                                         SetVariableStatusExtensions.TryParse,
                                         out SetVariableStatus AttributeStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component              [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
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
                                             OCPPv2_1.Variable.TryParse,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetVariableResult = new SetVariableResult(
                                        AttributeStatus,
                                        Component,
                                        Variable,
                                        AttributeType,
                                        AttributeStatusInfo,
                                        CustomData
                                    );

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

                                 new JProperty("attributeStatus",       AttributeStatus.    AsText()),

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

               AttributeStatus.     Equals(Equals(SetVariableResult.AttributeStatus))      &&
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

                   $"Component/variable: '{Component}'/'{Variable}': {AttributeStatus.AsText()}",

                   AttributeType.HasValue
                       ? $" [{AttributeType.Value.AsText()}]"
                       : ""

               );

        #endregion

    }

}
