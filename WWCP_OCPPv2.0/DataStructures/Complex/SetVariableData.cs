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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Set variable data.
    /// </summary>
    public class SetVariableData : ACustomData,
                                   IEquatable<SetVariableData>
    {

        #region Properties

        /// <summary>
        /// The value to be assigned to the attribute of the variable.
        /// </summary>
        [Mandatory]
        public String           AttributeValue    { get; }

        /// <summary>
        /// The component for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Component        Component         { get; }

        /// <summary>
        /// The variable for which the variable monitor is created or updated.
        /// </summary>
        [Mandatory]
        public Variable         Variable          { get; }

        /// <summary>
        /// The optional type of the attribute: Actual, Target, MinSet, MaxSet.
        /// [Default: actual]
        /// </summary>
        [Optional]
        public AttributeTypes?  AttributeType     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable data.
        /// </summary>
        /// <param name="AttributeValue">The value to be assigned to the attribute of the variable.</param>
        /// <param name="Component">The component for which the variable monitor is created or updated.</param>
        /// <param name="Variable">The variable for which the variable monitor is created or updated.</param>
        /// <param name="AttributeType">An optional type of the attribute: Actual, Target, MinSet, MaxSet [Default: actual]</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetVariableData(String           AttributeValue,
                               Component        Component,
                               Variable         Variable,
                               AttributeTypes?  AttributeType   = null,
                               CustomData?      CustomData      = null)

            : base(CustomData)

        {

            this.AttributeValue  = AttributeValue;
            this.Component       = Component;
            this.Variable        = Variable;
            this.AttributeType   = AttributeType;

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
        //       "description": "Value to be assigned to attribute of variable.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-configuration-value-size,ConfigurationValueSize&gt;&gt; can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valueList. The max size of these values will always remain equal. \r\n",
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
        /// Parse the given JSON representation of set variable data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableDataParser">A delegate to parse custom set variable data JSON objects.</param>
        public static SetVariableData Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<SetVariableData>?  CustomVariableDataParser   = null)
        {

            if (TryParse(JSON,
                         out var setVariableData,
                         out var errorResponse,
                         CustomVariableDataParser))
            {
                return setVariableData!;
            }

            throw new ArgumentException("The given JSON representation of set variable data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableData, CustomVariableDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of set variable data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableData">The parsed set variable data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out SetVariableData?  VariableData,
                                       out String?           ErrorResponse)

            => TryParse(JSON,
                        out VariableData,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of set variable data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableData">The parsed set variable data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableDataParser">A delegate to parse custom set variable data JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out SetVariableData?                           VariableData,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<SetVariableData>?  CustomVariableDataParser)
        {

            try
            {

                VariableData = default;

                #region AttributeValue    [mandatory]

                if (!JSON.ParseMandatoryText("attributeValue",
                                             "attribute value",
                                             out String AttributeValue,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component         [mandatory]

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

                #region Variable          [mandatory]

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

                #region AttributeType     [optional]

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

                #region CustomData        [optional]

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


                VariableData = new SetVariableData(AttributeValue,
                                                   Component,
                                                   Variable,
                                                   AttributeType,
                                                   CustomData);

                if (CustomVariableDataParser is not null)
                    VariableData = CustomVariableDataParser(JSON,
                                                            VariableData);

                return true;

            }
            catch (Exception e)
            {
                VariableData   = default;
                ErrorResponse  = "The given JSON representation of set variable data is invalid: " + e.Message;
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

            var JSON = JSONObject.Create(

                                 new JProperty("attributeValue",  AttributeValue),

                                 new JProperty("component",       Component.          ToJSON(CustomComponentSerializer,
                                                                                             CustomEVSESerializer,
                                                                                             CustomCustomDataSerializer)),

                                 new JProperty("variable",        Variable.           ToJSON(CustomVariableSerializer,
                                                                                             CustomCustomDataSerializer)),

                           AttributeType.HasValue
                               ? new JProperty("attributeType",   AttributeType.Value.AsText())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableDataSerializer is not null
                       ? CustomVariableDataSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VariableData1, VariableData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableData1">A set variable data.</param>
        /// <param name="VariableData2">Another set variable data.</param>
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
        /// <param name="VariableData1">A set variable data.</param>
        /// <param name="VariableData2">Another set variable data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetVariableData? VariableData1,
                                           SetVariableData? VariableData2)

            => !(VariableData1 == VariableData2);

        #endregion

        #endregion

        #region IEquatable<VariableData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set variable data for equality.
        /// </summary>
        /// <param name="Object">A set variable data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariableData setVariableData &&
                   Equals(setVariableData);

        #endregion

        #region Equals(VariableData)

        /// <summary>
        /// Compares two set variable data for equality.
        /// </summary>
        /// <param name="VariableData">A set variable data to compare with.</param>
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return AttributeValue.GetHashCode()       * 11 ^
                       Component.     GetHashCode()       *  7 ^
                       Variable.      GetHashCode()       *  5 ^
                      (AttributeType?.GetHashCode() ?? 0) *  3 ^

                       base.          GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   AttributeValue.Substring(30),
                   " for component/variable: '",
                   Component.ToString(),
                   "' / '",
                   Variable.ToString(), "'",

                   AttributeType.HasValue
                       ? " [" + AttributeType.Value.AsText() + "]"
                       : ""

               );

        #endregion

    }

}
