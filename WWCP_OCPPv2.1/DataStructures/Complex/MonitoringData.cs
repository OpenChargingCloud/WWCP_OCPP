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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Monitoring data.
    /// </summary>
    public class MonitoringData : ACustomData,
                                  IEquatable<MonitoringData>
    {

        #region Properties

        /// <summary>
        /// The component for which a report of the monitoring report was requested.
        /// </summary>
        [Mandatory]
        public Component                        Component              { get; }

        /// <summary>
        /// The variable for which the monitoring report was is requested.
        /// </summary>
        [Mandatory]
        public Variable                         Variable               { get; }

        /// <summary>
        /// The enumeration of monitors for the given component variable pair.
        /// </summary>
        [Mandatory]
        public IEnumerable<VariableMonitoring>  VariableMonitorings    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new monitoring data.
        /// </summary>
        /// <param name="Component">The component for which a report of the monitoring report was requested.</param>
        /// <param name="Variable">The variable for which the monitoring report was is requested.</param>
        /// <param name="VariableMonitorings">An enumeration of monitors for the given component variable pair.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MonitoringData(Component                        Component,
                              Variable                         Variable,
                              IEnumerable<VariableMonitoring>  VariableMonitorings,
                              CustomData?                      CustomData   = null)

            : base(CustomData)

        {

            if (!VariableMonitorings.Any())
                throw new ArgumentException("The given enumeration of cariable monitorings must not be empty!",
                                            nameof(VariableMonitorings));

            this.Component            = Component;
            this.Variable             = Variable;
            this.VariableMonitorings  = VariableMonitorings.Distinct();

        }

        #endregion


        #region Documentation

        // "MonitoringDataType": {
        //   "description": "Class to hold parameters of SetVariableMonitoring request.\r\n",
        //   "javaType": "MonitoringData",
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
        //     "variableMonitoring": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/VariableMonitoringType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "component",
        //     "variable",
        //     "variableMonitoring"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomMonitoringDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMonitoringDataParser">A delegate to parse custom component variable JSON objects.</param>
        public static MonitoringData Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<MonitoringData>?  CustomMonitoringDataParser   = null)
        {

            if (TryParse(JSON,
                         out var componentVariable,
                         out var errorResponse,
                         CustomMonitoringDataParser))
            {
                return componentVariable!;
            }

            throw new ArgumentException("The given JSON representation of a component variable is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MonitoringData, CustomMonitoringDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MonitoringData">The parsed component variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out MonitoringData?  MonitoringData,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out MonitoringData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MonitoringData">The parsed component variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMonitoringDataParser">A delegate to parse custom component variable JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out MonitoringData?                           MonitoringData,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<MonitoringData>?  CustomMonitoringDataParser)
        {

            try
            {

                MonitoringData = default;

                #region Component              [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse) ||
                     Component is null)
                {
                    return false;
                }

                #endregion

                #region Variable               [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse) ||
                     Variable is null)
                {
                    return false;
                }

                #endregion

                #region VariableMonitorings    [mandatory]

                if (!JSON.ParseMandatoryHashSet("variableMonitoring",
                                                "variable monitorings",
                                                VariableMonitoring.TryParse,
                                                out HashSet<VariableMonitoring> VariableMonitorings,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MonitoringData = new MonitoringData(Component,
                                                    Variable,
                                                    VariableMonitorings,
                                                    CustomData);

                if (CustomMonitoringDataParser is not null)
                    MonitoringData = CustomMonitoringDataParser(JSON,
                                                                MonitoringData);

                return true;

            }
            catch (Exception e)
            {
                MonitoringData  = default;
                ErrorResponse      = "The given JSON representation of a component variable is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMonitoringDataSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMonitoringDataSerializer">A delegate to serialize custom monitoring data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variable objects.</param>
        /// <param name="CustomVariableMonitoringSerializer">A delegate to serialize custom variable monitoring objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MonitoringData>?      CustomMonitoringDataSerializer       = null,
                              CustomJObjectSerializerDelegate<Component>?           CustomComponentSerializer            = null,
                              CustomJObjectSerializerDelegate<EVSE>?                CustomEVSESerializer                 = null,
                              CustomJObjectSerializerDelegate<Variable>?            CustomVariableSerializer             = null,
                              CustomJObjectSerializerDelegate<VariableMonitoring>?  CustomVariableMonitoringSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("component",            Component. ToJSON(CustomComponentSerializer,
                                                                                         CustomEVSESerializer,
                                                                                         CustomCustomDataSerializer)),

                                 new JProperty("variable",             Variable.  ToJSON(CustomVariableSerializer,
                                                                                         CustomCustomDataSerializer)),

                                 new JProperty("variableMonitoring",   new JArray(VariableMonitorings.Select(variableMonitoring => variableMonitoring.ToJSON(CustomVariableMonitoringSerializer,
                                                                                                                                                             CustomCustomDataSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMonitoringDataSerializer is not null
                       ? CustomMonitoringDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MonitoringData1, MonitoringData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringData1">An component variable.</param>
        /// <param name="MonitoringData2">Another component variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MonitoringData? MonitoringData1,
                                           MonitoringData? MonitoringData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MonitoringData1, MonitoringData2))
                return true;

            // If one is null, but not both, return false.
            if (MonitoringData1 is null || MonitoringData2 is null)
                return false;

            return MonitoringData1.Equals(MonitoringData2);

        }

        #endregion

        #region Operator != (MonitoringData1, MonitoringData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringData1">An component variable.</param>
        /// <param name="MonitoringData2">Another component variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MonitoringData? MonitoringData1,
                                           MonitoringData? MonitoringData2)

            => !(MonitoringData1 == MonitoringData2);

        #endregion

        #endregion

        #region IEquatable<MonitoringData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two component variables for equality.
        /// </summary>
        /// <param name="Object">An component variable to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MonitoringData componentVariable &&
                   Equals(componentVariable);

        #endregion

        #region Equals(MonitoringData)

        /// <summary>
        /// Compares two component variables for equality.
        /// </summary>
        /// <param name="MonitoringData">An component variable to compare with.</param>
        public Boolean Equals(MonitoringData? MonitoringData)

            => MonitoringData is not null &&

               Component.Equals(MonitoringData.Component) &&

             ((Variable is     null && MonitoringData.Variable is     null) ||
              (Variable is not null && MonitoringData.Variable is not null && Variable.Equals(MonitoringData.Variable))) &&

               VariableMonitorings.Count().Equals(MonitoringData.VariableMonitorings.Count()) &&
               VariableMonitorings.All(entry => MonitoringData.VariableMonitorings.Contains(entry)) &&

               base.     Equals(MonitoringData);

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

                return Component.          GetHashCode()       * 7 ^
                      (Variable?.          GetHashCode() ?? 0) * 5 ^
                       VariableMonitorings.CalcHashCode()      * 3 ^

                       base.     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Component.ToString(),

                   Variable is not null
                       ? " (" + Variable.ToString() + ")"
                       : null

               );

        #endregion

    }

}
