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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The result of a set variable request.
    /// </summary>
    public class SetVariableResult : ACustomData,
                                     ICBORSerializable<SetVariableResult>,
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


        #region (static) TryParseCBOR(CBOR, out SetVariableResult, out ErrorResponse, CustomSetVariableResultParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a set variable result.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="SetVariableResult">The set variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out SetVariableResult?  SetVariableResult,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out SetVariableResult,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a set variable result.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="SetVariableResult">The set variable result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetVariableResultParser">An optional delegate to read custom set variable results.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out SetVariableResult?           SetVariableResult,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<SetVariableResult>?  CustomSetVariableResultParser)
        {

            try
            {

                SetVariableResult = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a set variable result is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("attributeStatus",
                                             "attribute status",
                                             out var AttributeStatusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!SetVariableStatusExtensions.TryParse(AttributeStatusText, out var AttributeStatus))
                {
                    ErrorResponse = $"Invalid attribute status '{AttributeStatusText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatory("component",
                                         "component",
                                         OCPPv2_1.Component.TryParseCBOR,
                                         out Component? Component,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatory("variable",
                                         "variable",
                                         OCPPv2_1.Variable.TryParseCBOR,
                                         out Variable? Variable,
                                         out ErrorResponse))
                {
                    return false;
                }

                AttributeTypes? AttributeType = null;

                if (CBOR.ParseOptionalText("attributeType",
                                           "attribute type",
                                           out var AttributeTypeText,
                                           out ErrorResponse))
                {

                    if (!AttributeTypesExtensions.TryParse(AttributeTypeText!, out var AttributeTypeValue))
                    {
                        ErrorResponse = $"Invalid attribute type '{AttributeTypeText}'!";
                        return false;
                    }

                    AttributeType = AttributeTypeValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("attributeStatusInfo",
                                   "attribute status information",
                                   OCPPv2_1.StatusInfo.TryParseCBOR,
                                   out StatusInfo? AttributeStatusInfo,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                SetVariableResult = new SetVariableResult(
                                        AttributeStatus,
                                        Component,
                                        Variable,
                                        AttributeType,
                                        AttributeStatusInfo,
                                        CustomData
                                    );

                if (CustomSetVariableResultParser is not null)
                    SetVariableResult = CustomSetVariableResultParser(CBOR,
                                                   SetVariableResult);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SetVariableResult  = default;
                ErrorResponse  = "The given CBOR representation of a set variable result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<SetVariableResult>.TryParse(CBOR, out SetVariableResult, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a set variable result - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<SetVariableResult>.TryParse(CBORValue                         CBOR,
                                                                   out SetVariableResult                  Value,
                                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomSetVariableResultSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this set variable result: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSetVariableResultSerializer">A delegate to serialize custom set variable results.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SetVariableResult>? CustomSetVariableResultSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("attributeStatus",         CBORValue.FromText(AttributeStatus.AsText())),
                           ("component",               Component.ToCBOR()),
                           ("variable",                Variable. ToCBOR()),
                           ("attributeType",           OCPPCBORExtensions.Text(AttributeType?.AsText())),
                           ("attributeStatusInfo",     AttributeStatusInfo?.ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomSetVariableResultSerializer is not null
                       ? CustomSetVariableResultSerializer(this, cbor)
                       : cbor;

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
