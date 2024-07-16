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
    /// Report data.
    /// </summary>
    public class ReportData : ACustomData,
                              IEquatable<ReportData>
    {

        #region Properties

        /// <summary>
        /// The component for which a report of the monitoring report was requested.
        /// </summary>
        [Mandatory]
        public Component                       Component                  { get; }

        /// <summary>
        /// The variable for which the monitoring report was is requested.
        /// </summary>
        [Mandatory]
        public Variable                        Variable                   { get; }

        /// <summary>
        /// The attribute data of the variable.
        /// </summary>
        [Mandatory]
        public IEnumerable<VariableAttribute>  VariableAttributes         { get; }

        /// <summary>
        /// Optional fixed read-only parameters of a variable.
        /// </summary>
        [Optional]
        public VariableCharacteristics?        VariableCharacteristics    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new monitoring data.
        /// </summary>
        /// <param name="Component">The component for which a report of the monitoring report was requested.</param>
        /// <param name="Variable">The variable for which the monitoring report was is requested.</param>
        /// <param name="VariableAttributes">An enumeration of monitors for the given report data pair.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ReportData(Component                       Component,
                          Variable                        Variable,
                          IEnumerable<VariableAttribute>  VariableAttributes,
                          VariableCharacteristics?        VariableCharacteristics   = null,
                          CustomData?                     CustomData                = null)

            : base(CustomData)

        {

            if (!VariableAttributes.Any())
                throw new ArgumentException("The given enumeration of variable attributes must not be empty!",
                                            nameof(VariableAttributes));

            this.Component                = Component;
            this.Variable                 = Variable;
            this.VariableAttributes       = VariableAttributes.Distinct();
            this.VariableCharacteristics  = VariableCharacteristics;

            unchecked
            {

                hashCode = this.Component.               GetHashCode()       * 11 ^
                           this.Variable.                GetHashCode()       *  7 ^
                           this.VariableAttributes.      CalcHashCode()      *  5 ^
                          (this.VariableCharacteristics?.GetHashCode() ?? 0) *  3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "ReportDataType": {
        //   "description": "Class to report components, variables and variable attributes and characteristics.\r\n",
        //   "javaType": "ReportData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "component": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "variable": {
        //       "$ref": "#/definitions/VariableType"
        //     },
        //     "variableAttribute": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/VariableAttributeType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 4
        //     },
        //     "variableCharacteristics": {
        //       "$ref": "#/definitions/VariableCharacteristicsType"
        //     }
        //   },
        //   "required": [
        //     "component",
        //     "variable",
        //     "variableAttribute"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomReportDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of report data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomReportDataParser">A delegate to parse custom report data JSON objects.</param>
        public static ReportData Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<ReportData>?  CustomReportDataParser   = null)
        {

            if (TryParse(JSON,
                         out var reportData,
                         out var errorResponse,
                         CustomReportDataParser))
            {
                return reportData;
            }

            throw new ArgumentException("The given JSON representation of report data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ReportData, CustomReportDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of report data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReportData">The parsed report data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out ReportData?  ReportData,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out ReportData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of report data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReportData">The parsed report data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReportDataParser">A delegate to parse custom report data JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out ReportData?      ReportData,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<ReportData>?  CustomReportDataParser)
        {

            try
            {

                ReportData = default;

                #region Component                  [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable                   [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VariableAttributes         [mandatory]

                if (!JSON.ParseMandatoryHashSet("variableAttribute",
                                                "variable attribute",
                                                VariableAttribute.TryParse,
                                                out HashSet<VariableAttribute> VariableAttributes,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VariableCharacteristics    [optional]

                if (JSON.ParseMandatoryJSON("variableCharacteristics",
                                            "variable characteristics",
                                            OCPPv2_1.VariableCharacteristics.TryParse,
                                            out VariableCharacteristics? VariableCharacteristics,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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


                ReportData = new ReportData(
                                 Component,
                                 Variable,
                                 VariableAttributes,
                                 VariableCharacteristics,
                                 CustomData
                             );

                if (CustomReportDataParser is not null)
                    ReportData = CustomReportDataParser(JSON,
                                                        ReportData);

                return true;

            }
            catch (Exception e)
            {
                ReportData     = default;
                ErrorResponse  = "The given JSON representation of report data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReportDataSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReportDataSerializer">A delegate to serialize custom report data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variable objects.</param>
        /// <param name="CustomVariableAttributeSerializer">A delegate to serialize custom variable attribute objects.</param>
        /// <param name="CustomVariableCharacteristicsSerializer">A delegate to serialize custom variable characteristics objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReportData>?               CustomReportDataSerializer                = null,
                              CustomJObjectSerializerDelegate<Component>?                CustomComponentSerializer                 = null,
                              CustomJObjectSerializerDelegate<EVSE>?                     CustomEVSESerializer                      = null,
                              CustomJObjectSerializerDelegate<Variable>?                 CustomVariableSerializer                  = null,
                              CustomJObjectSerializerDelegate<VariableAttribute>?        CustomVariableAttributeSerializer         = null,
                              CustomJObjectSerializerDelegate<VariableCharacteristics>?  CustomVariableCharacteristicsSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("component",                 Component.              ToJSON(CustomComponentSerializer,
                                                                                                           CustomEVSESerializer,
                                                                                                           CustomCustomDataSerializer)),

                                 new JProperty("variable",                  Variable.               ToJSON(CustomVariableSerializer,
                                                                                                           CustomCustomDataSerializer)),

                                 new JProperty("variableAttribute",         new JArray(VariableAttributes.Select(variableAttribute => variableAttribute.ToJSON(CustomVariableAttributeSerializer,
                                                                                                                                                               CustomCustomDataSerializer)))),

                           VariableCharacteristics is not null
                               ? new JProperty("variableCharacteristics",   VariableCharacteristics.ToJSON(CustomVariableCharacteristicsSerializer,
                                                                                                           CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReportDataSerializer is not null
                       ? CustomReportDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReportData1, ReportData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReportData1">Report data.</param>
        /// <param name="ReportData2">Other report data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReportData? ReportData1,
                                           ReportData? ReportData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReportData1, ReportData2))
                return true;

            // If one is null, but not both, return false.
            if (ReportData1 is null || ReportData2 is null)
                return false;

            return ReportData1.Equals(ReportData2);

        }

        #endregion

        #region Operator != (ReportData1, ReportData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReportData1">Report data.</param>
        /// <param name="ReportData2">Other report data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReportData? ReportData1,
                                           ReportData? ReportData2)

            => !(ReportData1 == ReportData2);

        #endregion

        #endregion

        #region IEquatable<ReportData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two report data for equality.
        /// </summary>
        /// <param name="Object">Report data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportData reportData &&
                   Equals(reportData);

        #endregion

        #region Equals(ReportData)

        /// <summary>
        /// Compares two report data for equality.
        /// </summary>
        /// <param name="ReportData">Report data to compare with.</param>
        public Boolean Equals(ReportData? ReportData)

            => ReportData is not null &&

               Component.Equals(ReportData.Component) &&
               Variable. Equals(ReportData.Variable)  &&

               VariableAttributes.Count().Equals(ReportData.VariableAttributes.Count())     &&
               VariableAttributes.All(data => ReportData.VariableAttributes.Contains(data)) &&

             ((VariableCharacteristics is     null && ReportData.VariableCharacteristics is     null) ||
              (VariableCharacteristics is not null && ReportData.VariableCharacteristics is not null && VariableCharacteristics.Equals(ReportData.VariableCharacteristics))) &&

               base.     Equals(ReportData);

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

                   "Component: ", Component.ToString(), ", ",
                   "Variable: ",  Variable. ToString(), ", ",

                   VariableAttributes.Count(), " variable attribute(s)",

                   VariableCharacteristics is not null
                       ? ", " + VariableCharacteristics.ToString()
                       : ""

               );

        #endregion

    }

}
