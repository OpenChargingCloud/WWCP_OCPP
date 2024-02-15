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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Attribute data of a variable.
    /// </summary>
    public class VariableAttribute : ACustomData,
                                     IEquatable<VariableAttribute>
    {

        #region Properties

        /// <summary>
        /// The optional type of the attribute.
        /// </summary>
        [Optional]
        public AttributeTypes?   Type          { get; }

        /// <summary>
        /// The optional value of the attribute.
        /// May only be omitted when mutability is set to 'WriteOnly'.
        /// The Configuration Variable ReportingValueSize can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue.
        /// The max size of these values will always remain equal.
        /// </summary>
        [Optional]
        public String?           Value         { get; }

        /// <summary>
        /// The optional mutability of this attribute.
        /// Default when omitted: Read/Write.
        /// </summary>
        [Optional]
        public MutabilityTypes?  Mutability    { get; }

        /// <summary>
        /// Optional persistency of the attribute across system reboots or power down.
        /// Default when omitted: false (no persistency).
        /// </summary>
        [Optional]
        public Boolean?          Persistent    { get; }

        /// <summary>
        /// Optional constancy of the attribute at runtime.
        /// Default when omitted: false.
        /// </summary>
        [Optional]
        public Boolean?          Constant      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable attribute.
        /// </summary>
        /// <param name="Type">The optional type of the attribute.</param>
        /// <param name="Value">The optional value of the attribute. May only be omitted when mutability is set to 'WriteOnly'.</param>
        /// <param name="Mutability">The optional mutability of this attribute. Default when omitted: Read/Write.</param>
        /// <param name="Persistent">Optional persistency of the attribute across system reboots or power down. Default when omitted: false (no persistency).</param>
        /// <param name="Constant">Optional constancy of the attribute at runtime. Default when omitted: false.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VariableAttribute(AttributeTypes?   Type,
                                 String?           Value,
                                 MutabilityTypes?  Mutability,
                                 Boolean?          Persistent,
                                 Boolean?          Constant,
                                 CustomData?       CustomData   = null)

            : base(CustomData)

        {

            this.Type        = Type;
            this.Value       = Value;
            this.Mutability  = Mutability;
            this.Persistent  = Persistent;
            this.Constant    = Constant;

        }

        #endregion


        #region Documentation

        // "VariableAttributeType": {
        //   "description": "Attribute data of a variable.\r\n",
        //   "javaType": "VariableAttribute",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "type": {
        //       "$ref": "#/definitions/AttributeEnumType"
        //     },
        //     "value": {
        //       "description": "Value of the attribute. May only be omitted when mutability is set to 'WriteOnly'.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-reporting-value-size,ReportingValueSize&gt;&gt; can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. The max size of these values will always remain equal. \r\n",
        //       "type": "string",
        //       "maxLength": 2500
        //     },
        //     "mutability": {
        //       "$ref": "#/definitions/MutabilityEnumType"
        //     },
        //     "persistent": {
        //       "description": "If true, value will be persistent across system reboots or power down. Default when omitted is false.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "constant": {
        //       "description": "If true, value that will never be changed by the Charging Station at runtime. Default when omitted is false.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVariableAttributeParser = null)

        /// <summary>
        /// Parse the given JSON representation of a variable attribute.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableAttributeParser">An optional delegate to parse custom variable attribute JSON objects.</param>
        public static VariableAttribute Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<VariableAttribute>?  CustomVariableAttributeParser   = null)
        {

            if (TryParse(JSON,
                         out var variableAttribute,
                         out var errorResponse,
                         CustomVariableAttributeParser))
            {
                return variableAttribute!;
            }

            throw new ArgumentException("The given JSON representation of a variable attribute is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableAttribute, CustomVariableAttributeParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a variable attribute.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableAttribute">The parsed variable attribute.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out VariableAttribute?  VariableAttribute,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        out VariableAttribute,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a variable attribute.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableAttribute">The parsed variable attribute.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableAttributeParser">An optional delegate to parse custom variable attribute JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out VariableAttribute?                           VariableAttribute,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<VariableAttribute>?  CustomVariableAttributeParser)
        {

            try
            {

                VariableAttribute = default;

                #region AttributeType    [optional]

                if (JSON.ParseOptional("type",
                                       "attribute type",
                                       AttributeTypesExtensions.TryParse,
                                       out AttributeTypes? AttributeType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Value            [optional]

                var Value = JSON.GetString("value");

                #endregion

                #region Mutability       [optional]

                if (JSON.ParseOptional("mutability",
                                       "attribute type",
                                       MutabilityTypesExtensions.TryParse,
                                       out MutabilityTypes? Mutability,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Persistent       [optional]

                if (JSON.ParseOptional("persistent",
                                       "persistent",
                                       out Boolean? Persistent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Constant         [optional]

                if (JSON.ParseOptional("constant",
                                       "constant",
                                       out Boolean? Constant,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData       [optional]

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


                VariableAttribute = new VariableAttribute(
                                        AttributeType,
                                        Value,
                                        Mutability,
                                        Persistent,
                                        Constant,
                                        CustomData
                                    );

                if (CustomVariableAttributeParser is not null)
                    VariableAttribute = CustomVariableAttributeParser(JSON,
                                                                      VariableAttribute);

                return true;

            }
            catch (Exception e)
            {
                VariableAttribute  = default;
                ErrorResponse      = "The given JSON representation of a variable attribute is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableAttributeSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableAttributeSerializer">A delegate to serialize custom variable attribute objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VariableAttribute>?  CustomVariableAttributeSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           Type.HasValue
                               ? new JProperty("type",         Type.      Value.AsText())
                               : null,

                           Value is not null
                               ? new JProperty("value",        Value)
                               : null,

                           Mutability.HasValue
                               ? new JProperty("mutability",   Mutability.Value.AsText())
                               : null,

                           Persistent.HasValue
                               ? new JProperty("persistent",   Persistent.Value)
                               : null,

                           Constant.HasValue
                               ? new JProperty("constant",     Constant.  Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableAttributeSerializer is not null
                       ? CustomVariableAttributeSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VariableAttribute1, VariableAttribute2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableAttribute1">A variable attribute.</param>
        /// <param name="VariableAttribute2">Another variable attribute.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VariableAttribute? VariableAttribute1,
                                           VariableAttribute? VariableAttribute2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VariableAttribute1, VariableAttribute2))
                return true;

            // If one is null, but not both, return false.
            if (VariableAttribute1 is null || VariableAttribute2 is null)
                return false;

            return VariableAttribute1.Equals(VariableAttribute2);

        }

        #endregion

        #region Operator != (VariableAttribute1, VariableAttribute2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableAttribute1">A variable attribute.</param>
        /// <param name="VariableAttribute2">Another variable attribute.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VariableAttribute? VariableAttribute1,
                                           VariableAttribute? VariableAttribute2)

            => !(VariableAttribute1 == VariableAttribute2);

        #endregion

        #endregion

        #region IEquatable<VariableAttribute> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variable attributes for equality.
        /// </summary>
        /// <param name="Object">A variable attribute to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VariableAttribute variableAttribute &&
                   Equals(variableAttribute);

        #endregion

        #region Equals(VariableAttribute)

        /// <summary>
        /// Compares two variable attributes for equality.
        /// </summary>
        /// <param name="VariableAttribute">A variable attribute to compare with.</param>
        public Boolean Equals(VariableAttribute? VariableAttribute)

            => VariableAttribute is not null &&

            ((!Type.      HasValue && !VariableAttribute.Type.      HasValue) ||
               Type.      HasValue &&  VariableAttribute.Type.      HasValue && Type.      Value.Equals(VariableAttribute.Type.      Value)) &&

               String.Equals(Value, VariableAttribute.Value, StringComparison.Ordinal) &&

            ((!Mutability.HasValue && !VariableAttribute.Mutability.HasValue) ||
               Mutability.HasValue &&  VariableAttribute.Mutability.HasValue && Mutability.Value.Equals(VariableAttribute.Mutability.Value)) &&

            ((!Persistent.HasValue && !VariableAttribute.Persistent.HasValue) ||
               Persistent.HasValue &&  VariableAttribute.Persistent.HasValue && Persistent.Value.Equals(VariableAttribute.Persistent.Value)) &&

            ((!Constant.  HasValue && !VariableAttribute.Constant.  HasValue) ||
               Constant.  HasValue &&  VariableAttribute.Constant.  HasValue && Constant.  Value.Equals(VariableAttribute.Constant.  Value)) &&

               base.  Equals(VariableAttribute);

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

                return (Type?.      GetHashCode() ?? 0) * 13 ^
                       (Value?.     GetHashCode() ?? 0) * 11 ^
                       (Mutability?.GetHashCode() ?? 0) *  7 ^
                       (Persistent?.GetHashCode() ?? 0) *  5 ^
                       (Constant?.  GetHashCode() ?? 0) *  3 ^

                       base.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new String?[] {

                   Type.HasValue
                       ? "Type: " + Type.Value.AsText()
                       : null,

                   Value is not null && Value.IsNotNullOrEmpty()
                       ? "Value: " + Value.SubstringMax(20)
                       : null,

                   Mutability.HasValue
                       ? "Mutability: " + Mutability.Value.AsText()
                       : null,

                   Persistent.HasValue
                       ? Persistent.Value
                             ? "persistent"
                             : "not persistent"
                       : null,

                   Constant.HasValue
                       ? Constant.Value
                             ? "constant"
                             : "not constant"
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
