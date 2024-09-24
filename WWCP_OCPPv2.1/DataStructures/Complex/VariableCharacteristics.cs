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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An additional case insensitive identifier to use for the authorization
    /// and the type of authorization to support multiple forms of identifiers.
    /// </summary>
    public class VariableCharacteristics : ACustomData,
                                           IEquatable<VariableCharacteristics>
    {

        #region Properties

        /// <summary>
        /// The data type of this variable.
        /// </summary>
        [Mandatory]
        public DataTypes            DataType              { get; }

        /// <summary>
        /// Whether this variable supports monitoring.
        /// </summary>
        [Mandatory]
        public Boolean              SupportsMonitoring    { get; }

        /// <summary>
        /// The optional unit of the variable.
        /// When the transmitted value has a unit, this field SHALL be included.
        /// </summary>
        [Optional]
        public UnitsOfMeasure?      Unit                  { get; }

        /// <summary>
        /// The optional minimal value of this variable.
        /// </summary>
        [Optional]
        public Decimal?             MinLimit              { get; }

        /// <summary>
        /// The optional maximal value of this variable.
        /// When the datatype of this Variable is String, OptionList, SequenceList or MemberList, this field defines the maximum length of the (CSV) string.
        /// </summary>
        [Optional]
        public Decimal?             MaxLimit              { get; }

        /// <summary>
        /// The optional enumeration of allowed values when variable is Option/Member/SequenceList.
        /// * OptionList:   The (actual) variable value must be a single value from the reported(CSV) enumeration list.
        /// * MemberList:   The (actual) variable value may be an (unordered) (sub-)set of the reported(CSV) valid values list.
        /// * SequenceList: The (actual) variable value may be an ordered(priority, etc) (sub-)set of the reported(CSV) valid values.
        /// This is a comma separated list.
        /// The Configuration Variable ConfigurationValueSize can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valueList.
        /// The max size of these values will always remain equal.
        /// </summary>
        [Optional]
        public IEnumerable<String>  ValuesList            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable characteristics.
        /// </summary>
        /// <param name="DataType">The data type of this variable.</param>
        /// <param name="SupportsMonitoring">Whether this variable supports monitoring.</param>
        /// <param name="Unit">The optional unit of the variable. When the transmitted value has a unit, this field SHALL be included.</param>
        /// <param name="MinLimit">The optional minimal value of this variable.</param>
        /// <param name="MaxLimit">The optional maximal value of this variable. When the datatype of this Variable is String, OptionList, SequenceList or MemberList, this field defines the maximum length of the (CSV) string.</param>
        /// <param name="ValuesList">The optional CSV list of allowed values when variable is Option/Member/SequenceList.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public VariableCharacteristics(DataTypes             DataType,
                                       Boolean               SupportsMonitoring   = false,
                                       UnitsOfMeasure?       Unit                 = null,
                                       Decimal?              MinLimit             = null,
                                       Decimal?              MaxLimit             = null,
                                       IEnumerable<String>?  ValuesList           = null,
                                       CustomData?           CustomData           = null)

            : base(CustomData)

        {

            this.DataType            = DataType;
            this.SupportsMonitoring  = SupportsMonitoring;
            this.Unit                = Unit;
            this.MinLimit            = MinLimit;
            this.MaxLimit            = MaxLimit;
            this.ValuesList          = ValuesList?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.DataType.          GetHashCode()       * 17 ^
                           this.SupportsMonitoring.GetHashCode()       * 13 ^
                          (this.Unit?.             GetHashCode() ?? 0) * 11 ^
                          (this.MinLimit?.         GetHashCode() ?? 0) *  7 ^
                          (this.MaxLimit?.         GetHashCode() ?? 0) *  5 ^
                           this.ValuesList.        CalcHashCode()      *  3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "VariableCharacteristicsType": {
        //   "description": "Fixed read-only parameters of a variable.",
        //   "javaType": "VariableCharacteristics",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "unit": {
        //       "description": "Unit of the variable. When the transmitted value has a unit, this field SHALL be included.",
        //       "type": "string",
        //       "maxLength": 16
        //     },
        //     "dataType": {
        //       "$ref": "#/definitions/DataEnumType"
        //     },
        //     "minLimit": {
        //       "description": "Minimum possible value of this variable.",
        //       "type": "number"
        //     },
        //     "maxLimit": {
        //       "description": "Maximum possible value of this variable.
        //                       When the datatype of this Variable is String, OptionList, SequenceList or MemberList, this field defines the maximum length of the (CSV) string.",
        //       "type": "number"
        //     },
        //     "valuesList": {
        //       "description": "Allowed values when variable is Option/Member/SequenceList.
        //                       * OptionList:   The (Actual) Variable value must be a single value from the reported (CSV) enumeration list.
        //                       * MemberList:   The (Actual) Variable value  may be an (unordered) (sub-)set of the reported (CSV) valid values list.
        //                       * SequenceList: The (Actual) Variable value  may be an ordered (priority, etc)  (sub-)set of the reported (CSV) valid values.
        //                       This is a comma separated list.
        //                       The Configuration Variable <<configkey-configuration-value-size,ConfigurationValueSize>> can be used to limit SetVariableData.attributeValue
        //                       and VariableCharacteristics.valueList.
        //                       The max size of these values will always remain equal.",
        //       "type": "string",
        //       "maxLength": 1000
        //     },
        //     "supportsMonitoring": {
        //       "description": "Flag indicating if this variable supports monitoring.",
        //       "type": "boolean"
        //     }
        //   },
        //   "required": [
        //     "dataType",
        //     "supportsMonitoring"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVariableCharacteristicsParser = null)

        /// <summary>
        /// Parse the given JSON representation of variable characteristics.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableCharacteristicsParser">A delegate to parse custom variable characteristics JSON objects.</param>
        public static VariableCharacteristics Parse(JObject                                                JSON,
                                                    CustomJObjectParserDelegate<VariableCharacteristics>?  CustomVariableCharacteristicsParser   = null)
        {

            if (TryParse(JSON,
                         out var variableCharacteristics,
                         out var errorResponse,
                         CustomVariableCharacteristicsParser))
            {
                return variableCharacteristics;
            }

            throw new ArgumentException("The given JSON representation of variable characteristics is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableCharacteristics, CustomVariableCharacteristicsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of variable characteristics.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableCharacteristics">The parsed variable characteristics.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out VariableCharacteristics?  VariableCharacteristics,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)

            => TryParse(JSON,
                        out VariableCharacteristics,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of variable characteristics.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableCharacteristics">The parsed variable characteristics.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableCharacteristicsParser">A delegate to parse custom variable characteristics JSON objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out VariableCharacteristics?      VariableCharacteristics,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       CustomJObjectParserDelegate<VariableCharacteristics>?  CustomVariableCharacteristicsParser)
        {

            try
            {

                VariableCharacteristics = default;

                #region DataType              [mandatory]

                if (!JSON.ParseMandatory("dataType",
                                         "data type",
                                         DataTypesExtensions.TryParse,
                                         out DataTypes DataType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SupportsMonitoring    [mandatory]

                if (!JSON.ParseMandatory("supportsMonitoring",
                                         "supports monitoring",
                                         out Boolean SupportsMonitoring,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Unit                  [optional]

                if (JSON.ParseOptional("unit",
                                       "unit of measure",
                                       UnitOfMeasure.TryParse,
                                       out UnitOfMeasure? unit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var Unit = unit.HasValue
                               ? new UnitsOfMeasure(unit.Value, 1)
                               : null;

                #endregion

                #region MinLimit              [optional]

                if (JSON.ParseOptional("minLimit",
                                       "min limit",
                                       out Decimal? MinLimit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxLimit              [optional]

                if (JSON.ParseOptional("maxLimit",
                                       "max limit",
                                       out Decimal? MaxLimit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValuesList            [optional]

                var ValuesList           = new HashSet<String>();
                var valuesListJSONToken  = JSON["valuesList"];

                if (valuesListJSONToken is not null)
                {

                    switch (valuesListJSONToken.Type)
                    {

                        case JTokenType.String:
                        {

                            var valueListString = JSON["valuesList"]?.Value<String>()?.Trim();

                        }
                        break;

                        case JTokenType.Array:
                        {
                            if (JSON.ParseOptionalHashSet("valuesList",
                                                            "values list",
                                                            s => s,
                                                            out ValuesList,
                                                            out ErrorResponse))
                            {
                                if (ErrorResponse is not null)
                                    return false;
                            }
                        }
                        break;

                    }

                }

                #endregion

                #region CustomData            [optional]

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


                VariableCharacteristics = new VariableCharacteristics(
                                              DataType,
                                              SupportsMonitoring,
                                              Unit,
                                              MinLimit,
                                              MaxLimit,
                                              ValuesList,
                                              CustomData
                                          );

                if (CustomVariableCharacteristicsParser is not null)
                    VariableCharacteristics = CustomVariableCharacteristicsParser(JSON,
                                                                                  VariableCharacteristics);

                return true;

            }
            catch (Exception e)
            {
                VariableCharacteristics  = default;
                ErrorResponse            = "The given JSON representation of variable characteristics is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableCharacteristicsSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableCharacteristicsSerializer">A delegate to serialize custom variable characteristics objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VariableCharacteristics>?  CustomVariableCharacteristicsSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("dataType",             DataType.  AsText()),
                                 new JProperty("supportsMonitoring",   SupportsMonitoring),

                           Unit is not null
                               ? new JProperty("unit",                 Unit.Unit. ToString())
                               : null,

                           MinLimit.HasValue
                               ? new JProperty("minLimit",             MinLimit.Value)
                               : null,

                           MaxLimit.HasValue
                               ? new JProperty("maxLimit",             MaxLimit.Value)
                               : null,

                           ValuesList.Any()
                               ? new JProperty("valuesList",           ValuesList.AggregateWith(','))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableCharacteristicsSerializer is not null
                       ? CustomVariableCharacteristicsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public VariableCharacteristics Clone()

            => new (

                   DataType,
                   SupportsMonitoring,
                   Unit?.Clone(),
                   MinLimit,
                   MaxLimit,
                   ValuesList.Any() ? ValuesList.Select(value => new String(value.ToCharArray())) : [],

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (VariableCharacteristics1, VariableCharacteristics2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableCharacteristics1">Variable characteristics.</param>
        /// <param name="VariableCharacteristics2">Other variable characteristics.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VariableCharacteristics? VariableCharacteristics1,
                                           VariableCharacteristics? VariableCharacteristics2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VariableCharacteristics1, VariableCharacteristics2))
                return true;

            // If one is null, but not both, return false.
            if (VariableCharacteristics1 is null || VariableCharacteristics2 is null)
                return false;

            return VariableCharacteristics1.Equals(VariableCharacteristics2);

        }

        #endregion

        #region Operator != (VariableCharacteristics1, VariableCharacteristics2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableCharacteristics1">Variable characteristics.</param>
        /// <param name="VariableCharacteristics2">Other variable characteristics.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VariableCharacteristics? VariableCharacteristics1,
                                           VariableCharacteristics? VariableCharacteristics2)

            => !(VariableCharacteristics1 == VariableCharacteristics2);

        #endregion

        #endregion

        #region IEquatable<VariableCharacteristics> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variable characteristics for equality.
        /// </summary>
        /// <param name="Object">Variable characteristics to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VariableCharacteristics variableCharacteristics &&
                   Equals(variableCharacteristics);

        #endregion

        #region Equals(VariableCharacteristics)

        /// <summary>
        /// Compares two variable characteristics for equality.
        /// </summary>
        /// <param name="VariableCharacteristics">Variable characteristics to compare with.</param>
        public Boolean Equals(VariableCharacteristics? VariableCharacteristics)

            => VariableCharacteristics is not null &&

               DataType.          Equals(VariableCharacteristics.DataType)           &&
               SupportsMonitoring.Equals(VariableCharacteristics.SupportsMonitoring) &&

             ((Unit is null      &&  VariableCharacteristics.Unit is null) ||
               Unit is not null  &&  VariableCharacteristics.Unit is not null  && Unit.          Equals(VariableCharacteristics.Unit))           &&

            ((!MinLimit.HasValue && !VariableCharacteristics.MinLimit.HasValue) ||
               MinLimit.HasValue &&  VariableCharacteristics.MinLimit.HasValue && MinLimit.Value.Equals(VariableCharacteristics.MinLimit.Value)) &&

            ((!MaxLimit.HasValue && !VariableCharacteristics.MaxLimit.HasValue) ||
               MaxLimit.HasValue &&  VariableCharacteristics.MaxLimit.HasValue && MaxLimit.Value.Equals(VariableCharacteristics.MaxLimit.Value)) &&

               ValuesList.Count().Equals(VariableCharacteristics.ValuesList.Count()) &&
               ValuesList.All(data => VariableCharacteristics.ValuesList.Contains(data)) &&

               base.              Equals(VariableCharacteristics);

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

            => new String?[] {

                   DataType.AsText(),

                   SupportsMonitoring
                       ? "supports monitoring"
                       : null,

                   Unit is not null
                       ? "Unit: " + Unit
                       : null,

                   MinLimit.HasValue
                       ? "MinLimit: " + MinLimit.Value
                       : null,

                   MaxLimit.HasValue
                       ? "MaxLimit: " + MaxLimit.Value
                       : null,

                   ValuesList.Any()
                       ? "ValuesList: " + ValuesList.AggregateWith('|')
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
