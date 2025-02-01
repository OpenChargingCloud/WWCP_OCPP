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
    /// A data structure for requesting variable data.
    /// </summary>
    public class GetVariableData : ACustomData,
                                   IEquatable<GetVariableData>
    {

        #region Properties

        /// <summary>
        /// The component for which the variable is requested.
        /// </summary>
        [Mandatory]
        public Component        Component        { get; }

        /// <summary>
        /// The variable for which the attribute value is requested.
        /// </summary>
        [Mandatory]
        public Variable         Variable         { get; }

        /// <summary>
        /// The optional attribute type.
        /// </summary>
        [Optional]
        public AttributeTypes?  AttributeType    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetVariableData data structure.
        /// </summary>
        /// <param name="Component">A component for which the variable is requested.</param>
        /// <param name="Variable">A variable for which the attribute value is requested.</param>
        /// <param name="AttributeType">An optional attribute type.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public GetVariableData(Component        Component,
                               Variable         Variable,
                               AttributeTypes?  AttributeType   = null,
                               CustomData?      CustomData      = null)

            : base(CustomData)

        {

            this.Component      = Component;
            this.Variable       = Variable;
            this.AttributeType  = AttributeType;

            unchecked
            {

                hashCode = this.Component.     GetHashCode()       * 7 ^
                           this.Variable.      GetHashCode()       * 5 ^
                          (this.AttributeType?.GetHashCode() ?? 0) * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Class to hold parameters for GetVariables request.\r\n",
        //     "javaType": "GetVariableData",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "attributeType": {
        //             "$ref": "#/definitions/AttributeEnumType"
        //         },
        //         "component": {
        //             "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //             "$ref": "#/definitions/VariableType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "component",
        //         "variable"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomGetVariableDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetVariableData data structure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetVariableDataParser">An optional delegate to parse custom GetVariableData data structure JSON objects.</param>
        public static GetVariableData Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<GetVariableData>?  CustomGetVariableDataParser   = null)
        {

            if (TryParse(JSON,
                         out var getVariableData,
                         out var errorResponse,
                         CustomGetVariableDataParser))
            {
                return getVariableData;
            }

            throw new ArgumentException("The given JSON representation of a GetVariableData data structure is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out GetVariableData, CustomGetVariableDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetVariableData data structure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetVariableData">The parsed GetVariableData data structure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out GetVariableData?  GetVariableData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out GetVariableData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetVariableData data structure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetVariableData">The parsed GetVariableData data structure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetVariableDataParser">An optional delegate to parse custom GetVariableData data structure JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out GetVariableData?      GetVariableData,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<GetVariableData>?  CustomGetVariableDataParser)
        {

            try
            {

                GetVariableData = default;

                #region Component        [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable         [optional]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AttributeType    [optional]

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

                #region CustomData       [optional]

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


                GetVariableData = new GetVariableData(
                                      Component,
                                      Variable,
                                      AttributeType,
                                      CustomData
                                  );

                if (CustomGetVariableDataParser is not null)
                    GetVariableData = CustomGetVariableDataParser(JSON,
                                                                  GetVariableData);

                return true;

            }
            catch (Exception e)
            {
                GetVariableData  = default;
                ErrorResponse    = "The given JSON representation of a GetVariableData data structure is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetVariableDataSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetVariableDataSerializer">A delegate to serialize custom GetVariableData data structures.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetVariableData>?  CustomGetVariableDataSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?        CustomComponentSerializer         = null,
                              CustomJObjectSerializerDelegate<EVSE>?             CustomEVSESerializer              = null,
                              CustomJObjectSerializerDelegate<Variable>?         CustomVariableSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

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

            return CustomGetVariableDataSerializer is not null
                       ? CustomGetVariableDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetVariableData1, GetVariableData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetVariableData1">An GetVariableData data structure.</param>
        /// <param name="GetVariableData2">Another GetVariableData data structure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GetVariableData? GetVariableData1,
                                           GetVariableData? GetVariableData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetVariableData1, GetVariableData2))
                return true;

            // If one is null, but not both, return false.
            if (GetVariableData1 is null || GetVariableData2 is null)
                return false;

            return GetVariableData1.Equals(GetVariableData2);

        }

        #endregion

        #region Operator != (GetVariableData1, GetVariableData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetVariableData1">An GetVariableData data structure.</param>
        /// <param name="GetVariableData2">Another GetVariableData data structure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GetVariableData? GetVariableData1,
                                           GetVariableData? GetVariableData2)

            => !(GetVariableData1 == GetVariableData2);

        #endregion

        #endregion

        #region IEquatable<GetVariableData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetVariableData data structures for equality.
        /// </summary>
        /// <param name="Object">An GetVariableData data structure to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetVariableData getVariableData &&
                   Equals(getVariableData);

        #endregion

        #region Equals(GetVariableData)

        /// <summary>
        /// Compares two GetVariableData data structures for equality.
        /// </summary>
        /// <param name="GetVariableData">An GetVariableData data structure to compare with.</param>
        public Boolean Equals(GetVariableData? GetVariableData)

            => GetVariableData is not null &&

               Component.Equals(GetVariableData.Component) &&
               Variable. Equals(GetVariableData.Variable)  &&

            ((!AttributeType.HasValue && !GetVariableData.AttributeType.HasValue) ||
               AttributeType.HasValue &&  GetVariableData.AttributeType.HasValue && AttributeType.Value.Equals(GetVariableData.AttributeType.Value)) &&

               base.     Equals(GetVariableData);

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

                   $"'{Component}' / '{Variable}'",

                   AttributeType.HasValue
                       ? $" ({AttributeType.Value.AsText()})"
                       : ""

               );

        #endregion

    }

}
