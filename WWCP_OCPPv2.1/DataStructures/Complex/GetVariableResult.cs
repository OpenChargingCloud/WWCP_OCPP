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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A get variable result.
    /// </summary>
    public class GetVariableResult : ACustomData,
                                     IEquatable<GetVariableResult>
    {

        #region Properties

        /// <summary>
        /// The result status.
        /// </summary>
        [Mandatory]
        public GetVariableStatus  AttributeStatus        { get; }

        /// <summary>
        /// The component for which the variable was requested.
        /// </summary>
        [Mandatory]
        public Component          Component              { get; }

        /// <summary>
        /// The variable for which the variable was requested.
        /// </summary>
        [Mandatory]
        public Variable           Variable               { get; }

        /// <summary>
        /// The optional response value.
        /// [max 2500]
        /// </summary>
        [Optional]
        public String?            AttributeValue         { get; }

        /// <summary>
        /// The optional attribute type.
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
        /// Create a new get variable result.
        /// </summary>
        /// <param name="AttributeStatus">The result status.</param>
        /// <param name="Component">The component for which the variable was requested.</param>
        /// <param name="Variable">The variable for which the variable was requested.</param>
        /// <param name="AttributeValue">An optional response value [max 2500].</param>
        /// <param name="AttributeType">An optional attribute type.</param>
        /// <param name="AttributeStatusInfo">Optional detailed attribute status information.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public GetVariableResult(GetVariableStatus  AttributeStatus,
                                 Component          Component,
                                 Variable           Variable,
                                 String?            AttributeValue        = null,
                                 AttributeTypes?    AttributeType         = null,
                                 StatusInfo?        AttributeStatusInfo   = null,
                                 CustomData?        CustomData            = null)

            : base(CustomData)

        {

            this.AttributeStatus      = AttributeStatus;
            this.Component            = Component;
            this.Variable             = Variable;
            this.AttributeValue       = AttributeValue;
            this.AttributeType        = AttributeType;
            this.AttributeStatusInfo  = AttributeStatusInfo;


            unchecked
            {

                hashCode = this.AttributeStatus.     GetHashCode()       * 17 ^
                           this.Component.           GetHashCode()       * 13 ^
                           this.Variable.            GetHashCode()       * 11 ^
                          (this.AttributeValue?.     GetHashCode() ?? 0) *  7 ^
                          (this.AttributeType?.      GetHashCode() ?? 0) *  5 ^
                          (this.AttributeStatusInfo?.GetHashCode() ?? 0) *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "GetVariableResultType": {
        //   "description": "Class to hold results of GetVariables request.\r\n",
        //   "javaType": "GetVariableResult",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "attributeStatusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "attributeStatus": {
        //       "$ref": "#/definitions/GetVariableStatusEnumType"
        //     },
        //     "attributeType": {
        //       "$ref": "#/definitions/AttributeEnumType"
        //     },
        //     "attributeValue": {
        //       "description":
        //           "Value of requested attribute type of component-variable.
        //            This field can only be empty when the given status is NOT accepted.
        //            The Configuration Variable <<configkey-reporting-value-size,ReportingValueSize>> can be used to limit
        //            GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue.
        //            The max size of these values will always remain equal.",
        //       "type": "string",
        //       "maxLength": 2500
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

        #region (static) Parse   (JSON, CustomGetVariableResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetVariableResultParser">An optional delegate to parse custom get variable results.</param>
        public static GetVariableResult Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<GetVariableResult>?  CustomGetVariableResultParser   = null)
        {

            if (TryParse(JSON,
                         out var getVariableResult,
                         out var errorResponse,
                         CustomGetVariableResultParser) &&
                getVariableResult is not null)
            {
                return getVariableResult;
            }

            throw new ArgumentException("The given JSON representation of a get variable result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out GetVariableResult, out ErrorResponse, CustomGetVariableResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetVariableResult">The parsed get variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out GetVariableResult?  GetVariableResult,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out GetVariableResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get variable result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetVariableResult">The parsed get variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetVariableResultParser">An optional delegate to parse custom get variable results.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out GetVariableResult?      GetVariableResult,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<GetVariableResult>?  CustomGetVariableResultParser   = null)
        {

            try
            {

                GetVariableResult = null;

                #region AttributeStatus        [mandatory]

                if (!JSON.ParseMandatory("attributeStatus",
                                         "attribute status",
                                         GetVariableStatusExtensions.TryParse,
                                         out GetVariableStatus AttributeStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component              [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "requested component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable               [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "requested variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AttributeValue         [optional]

                if (JSON.ParseOptional("attributeValue",
                                       "attribute value",
                                       out String? AttributeValue,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetVariableResult = new GetVariableResult(
                                        AttributeStatus,
                                        Component,
                                        Variable,
                                        AttributeValue,
                                        AttributeType,
                                        AttributeStatusInfo,
                                        CustomData
                                    );

                if (CustomGetVariableResultParser is not null)
                    GetVariableResult = CustomGetVariableResultParser(JSON,
                                                                      GetVariableResult);

                return true;

            }
            catch (Exception e)
            {
                GetVariableResult  = null;
                ErrorResponse      = "The given JSON representation of a get variable result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetVariableResultSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetVariableResultSerializer">A delegate to serialize custom get variable results.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetVariableResult>?  CustomGetVariableResultSerializer   = null,
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

                           AttributeValue is not null
                               ? new JProperty("attributeValue",        AttributeValue)
                               : null,

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

            return CustomGetVariableResultSerializer is not null
                       ? CustomGetVariableResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetVariableResult1, GetVariableResult2)

        /// <summary>
        /// Compares two get variable results for equality.
        /// </summary>
        /// <param name="GetVariableResult1">A get variable result.</param>
        /// <param name="GetVariableResult2">Another get variable result.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetVariableResult? GetVariableResult1,
                                           GetVariableResult? GetVariableResult2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetVariableResult1, GetVariableResult2))
                return true;

            // If one is null, but not both, return false.
            if (GetVariableResult1 is null || GetVariableResult2 is null)
                return false;

            return GetVariableResult1.Equals(GetVariableResult2);

        }

        #endregion

        #region Operator != (GetVariableResult1, GetVariableResult2)

        /// <summary>
        /// Compares two get variable results for inequality.
        /// </summary>
        /// <param name="GetVariableResult1">A get variable result.</param>
        /// <param name="GetVariableResult2">Another get variable result.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator !=(GetVariableResult? GetVariableResult1,
                                           GetVariableResult? GetVariableResult2)

            => !(GetVariableResult1 == GetVariableResult2);

        #endregion

        #endregion

        #region IEquatable<GetVariableResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get variable results for equality.
        /// </summary>
        /// <param name="Object">A get variable result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetVariableResult getVariableResult &&
                   Equals(getVariableResult);

        #endregion

        #region Equals(GetVariableResult)

        /// <summary>
        /// Compares two get variable results for equality.
        /// </summary>
        /// <param name="GetVariableResult">A get variable result to compare with.</param>
        public Boolean Equals(GetVariableResult? GetVariableResult)

            => GetVariableResult is not null &&

               AttributeStatus.Equals(GetVariableResult.AttributeStatus) &&
               Component.      Equals(GetVariableResult.Component)       &&
               Variable.       Equals(GetVariableResult.Variable)        &&

             ((AttributeValue      is null     &&  GetVariableResult.AttributeValue      is     null) ||
               AttributeValue      is not null &&  GetVariableResult.AttributeValue      is not null && AttributeValue.     Equals(GetVariableResult.AttributeValue))      &&

            ((!AttributeType.      HasValue    && !GetVariableResult.AttributeType.      HasValue)    ||
               AttributeType.      HasValue    &&  GetVariableResult.AttributeType.      HasValue    && AttributeType.Value.Equals(GetVariableResult.AttributeType.Value)) &&

             ((AttributeStatusInfo is null     &&  GetVariableResult.AttributeStatusInfo is     null) ||
               AttributeStatusInfo is not null &&  GetVariableResult.AttributeStatusInfo is not null && AttributeStatusInfo.Equals(GetVariableResult.AttributeStatusInfo)) &&

               base.           Equals(GetVariableResult);

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

                   AttributeStatus,

                   AttributeValue is not null
                       ? $" => {AttributeValue.SubstringMax(30)}"
                       : "",

                   AttributeType.HasValue
                       ? $" [{AttributeType}]"
                       : "",

                   AttributeStatusInfo is not null
                       ? $", {AttributeStatusInfo}"
                       : ""

               );

        #endregion


    }

}
