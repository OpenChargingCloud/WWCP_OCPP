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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A data structure for setting variable data.
    /// </summary>
    public class SetVariableData : ACustomData,
                                   IEquatable<SetVariableData>
    {

        #region Properties

        /// <summary>
        /// The component for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Component        Component            { get; }

        /// <summary>
        /// The variable for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Variable         Variable             { get; }

        /// <summary>
        /// The value to be assigned to the attribute of the variable.
        /// </summary>
        [Mandatory]
        public String           AttributeValue       { get; }

        /// <summary>
        /// The optional old attribute value of the variable for safe
        /// conditional changes within a distributed system.
        /// </summary>
        [Optional]
        public String?          OldAttributeValue    { get; }

        /// <summary>
        /// The optional type of the attribute: Actual, Target, MinSet, MaxSet.
        /// [Default: actual]
        /// </summary>
        [Optional]
        public AttributeTypes?  AttributeType        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetVariableData request.
        /// </summary>
        /// <param name="Component">The component for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="AttributeValue">The value to be assigned to the attribute of the variable.</param>
        /// <param name="OldAttributeValue">An optional old attribute value of the variable for safe conditional changes within a distributed system.</param>
        /// <param name="AttributeType">An optional type of the attribute: Actual, Target, MinSet, MaxSet [Default: actual]</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetVariableData(Component        Component,
                               Variable         Variable,
                               String           AttributeValue,
                               String?          OldAttributeValue   = null,
                               AttributeTypes?  AttributeType       = null,
                               CustomData?      CustomData          = null)

            : base(CustomData)

        {

            this.Component          = Component;
            this.Variable           = Variable;
            this.AttributeValue     = AttributeValue;
            this.OldAttributeValue  = OldAttributeValue;
            this.AttributeType      = AttributeType;

            unchecked
            {

                hashCode = this.Component.         GetHashCode()       * 13 ^
                           this.Variable.          GetHashCode()       * 11 ^
                           this.AttributeValue.    GetHashCode()       *  7 ^
                          (this.OldAttributeValue?.GetHashCode() ?? 0) *  5 ^
                          (this.AttributeType?.    GetHashCode() ?? 0) *  3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "SetVariableDataType": {
        //   "javaType": "SetVariableData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "attributeType": {
        //       "$ref": "#/definitions/AttributeEnumType"
        //     },
        //     "attributeValue": {
        //       "description":
        //           "Value to be assigned to attribute of variable.
        //            The Configuration Variable <<configkey-configuration-value-size, ConfigurationValueSize>>; can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valueList.
        //            The max size of these values will always remain equal.",
        //       "type": "string",
        //       "maxLength": 1000
        //     },
        //     "component": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "variable": {
        //       "$ref": "#/definitions/VariableType"
        //     }
        //   },
        //   "required": [
        //     "attributeValue",
        //     "component",
        //     "variable"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVariableDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of SetVariableData.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableDataParser">An optional delegate to parse custom SetVariableData JSON objects.</param>
        public static SetVariableData Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<SetVariableData>?  CustomVariableDataParser   = null)
        {

            if (TryParse(JSON,
                         out var setVariableData,
                         out var errorResponse,
                         CustomVariableDataParser) &&
                setVariableData is not null)
            {
                return setVariableData;
            }

            throw new ArgumentException("The given JSON representation of SetVariableData is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableData, CustomVariableDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of SetVariableData.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableData">The parsed SetVariableData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out SetVariableData?  VariableData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out VariableData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of SetVariableData.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableData">The parsed SetVariableData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableDataParser">An optional delegate to parse custom SetVariableData JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out SetVariableData?      VariableData,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<SetVariableData>?  CustomVariableDataParser)
        {

            try
            {

                VariableData = default;

                #region Component            [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable             [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AttributeValue       [mandatory]

                if (!JSON.ParseMandatoryText("attributeValue",
                                             "attribute value",
                                             out String? AttributeValue,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OldAttributeValue    [optional, OCPP_CSE]

                var OldAttributeValue = JSON.GetString("oldAttributeValue");

                #endregion

                #region AttributeType        [optional]

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

                #region CustomData           [optional]

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


                VariableData = new SetVariableData(
                                   Component,
                                   Variable,
                                   AttributeValue,
                                   OldAttributeValue,
                                   AttributeType,
                                   CustomData
                               );

                if (CustomVariableDataParser is not null)
                    VariableData = CustomVariableDataParser(JSON,
                                                            VariableData);

                return true;

            }
            catch (Exception e)
            {
                VariableData   = default;
                ErrorResponse  = "The given JSON representation of SetVariableData is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableDataSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableDataSerializer">A delegate to serialize custom VariableData objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetVariableData>?  CustomVariableDataSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?        CustomComponentSerializer      = null,
                              CustomJObjectSerializerDelegate<EVSE>?             CustomEVSESerializer           = null,
                              CustomJObjectSerializerDelegate<Variable>?         CustomVariableSerializer       = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("component",           Component.          ToJSON(CustomComponentSerializer,
                                                                                                 CustomEVSESerializer,
                                                                                                 CustomCustomDataSerializer)),

                                 new JProperty("variable",            Variable.           ToJSON(CustomVariableSerializer,
                                                                                                 CustomCustomDataSerializer)),

                                 new JProperty("attributeValue",      AttributeValue),

                           OldAttributeValue is not null
                               ? new JProperty("oldAttributeValue",   OldAttributeValue)
                               : null,

                           AttributeType.HasValue
                               ? new JProperty("attributeType",       AttributeType.Value.AsText())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableDataSerializer is not null
                       ? CustomVariableDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VariableData1, VariableData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableData1">A SetVariableData.</param>
        /// <param name="VariableData2">Another SetVariableData.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetVariableData? VariableData1,
                                           SetVariableData? VariableData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VariableData1, VariableData2))
                return true;

            // If one is null, but not both, return false.
            if (VariableData1 is null || VariableData2 is null)
                return false;

            return VariableData1.Equals(VariableData2);

        }

        #endregion

        #region Operator != (VariableData1, VariableData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableData1">A SetVariableData.</param>
        /// <param name="VariableData2">Another SetVariableData.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetVariableData? VariableData1,
                                           SetVariableData? VariableData2)

            => !(VariableData1 == VariableData2);

        #endregion

        #endregion

        #region IEquatable<VariableData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetVariableData for equality.
        /// </summary>
        /// <param name="Object">A SetVariableData to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariableData setVariableData &&
                   Equals(setVariableData);

        #endregion

        #region Equals(VariableData)

        /// <summary>
        /// Compares two SetVariableData for equality.
        /// </summary>
        /// <param name="VariableData">A SetVariableData to compare with.</param>
        public Boolean Equals(SetVariableData? VariableData)

            => VariableData is not null &&

               AttributeValue.Equals(VariableData.AttributeValue) &&
               Component.     Equals(VariableData.Component)      &&
               Variable.      Equals(VariableData.Variable)       &&

            ((!AttributeType.HasValue && !VariableData.AttributeType.HasValue) ||
               AttributeType.HasValue &&  VariableData.AttributeType.HasValue && AttributeType.Value.Equals(VariableData.AttributeType.Value)) &&

               base.          Equals(VariableData);

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

                   $"'{Component}' / '{Variable}' => {AttributeValue.SubstringMax(30)}",

                   AttributeType.HasValue
                       ? $" ({AttributeType.Value.AsText()})"
                       : "",

                   OldAttributeValue is not null
                       ? $", only when old value == '{OldAttributeValue}'"
                       : ""

               );

        #endregion

    }

}
