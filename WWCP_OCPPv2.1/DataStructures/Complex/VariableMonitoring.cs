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
    /// A variable monitoring.
    /// </summary>
    public class VariableMonitoring : ACustomData,
                                      IEquatable<VariableMonitoring>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the monitor.
        /// </summary>
        [Mandatory]
        public VariableMonitoring_Id  Id              { get; }

        /// <summary>
        /// Whether the monitor is only active when a transaction is ongoing on a related component.
        /// </summary>
        [Mandatory]
        public Boolean                Transaction    { get; }

        /// <summary>
        /// Value for threshold or delta monitoring.
        /// For periodic or periodicClockAligned monitors this is their monitoring interval in seconds.
        /// </summary>
        [Mandatory]
        public Decimal                Value          { get; }

        /// <summary>
        /// The enumeration of monitors for the given variable monitoring pair.
        /// </summary>
        [Mandatory]
        public MonitorType            Type           { get; }

        /// <summary>
        /// The severity that will be assigned to an event that is triggered by this monitor.
        /// </summary>
        [Mandatory]
        public Severities             Severity       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable monitoring.
        /// </summary>
        /// <param name="Id">The unique identification of the monitor.</param>
        /// <param name="Transaction">Whether the monitor is only active when a transaction is ongoing on a related component.</param>
        /// <param name="Value">Value for threshold or delta monitoring. For periodic or periodicClockAligned monitors this is their monitoring interval in seconds.</param>
        /// <param name="Type">The enumeration of monitors for the given variable monitoring pair.</param>
        /// <param name="Severity">The severity that will be assigned to an event that is triggered by this monitor.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public VariableMonitoring(VariableMonitoring_Id  Id,
                                  Boolean                Transaction,
                                  Decimal                Value,
                                  MonitorType            Type,
                                  Severities             Severity,
                                  CustomData?            CustomData   = null)

            : base(CustomData)

        {

            this.Id           = Id;
            this.Transaction  = Transaction;
            this.Value        = Value;
            this.Type         = Type;
            this.Severity     = Severity;

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A monitoring setting for a variable.",
        //     "javaType": "VariableMonitoring",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "id": {
        //             "description": "Identifies the monitor.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "transaction": {
        //             "description": "Monitor only active when a transaction is ongoing on a component relevant to this transaction. ",
        //             "type": "boolean"
        //         },
        //         "value": {
        //             "description": "Value for threshold or delta monitoring.\r\nFor Periodic or PeriodicClockAligned this is the interval in seconds.",
        //             "type": "number"
        //         },
        //         "type": {
        //             "$ref": "#/definitions/MonitorEnumType"
        //         },
        //         "severity": {
        //             "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "eventNotificationType": {
        //             "$ref": "#/definitions/EventNotificationEnumType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "transaction",
        //         "value",
        //         "type",
        //         "severity",
        //         "eventNotificationType"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVariableMonitoringParser = null)

        /// <summary>
        /// Parse the given JSON representation of a variable monitoring.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableMonitoringParser">A delegate to parse custom variable monitoring JSON objects.</param>
        public static VariableMonitoring Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<VariableMonitoring>?  CustomVariableMonitoringParser   = null)
        {

            if (TryParse(JSON,
                         out var variableMonitoring,
                         out var errorResponse,
                         CustomVariableMonitoringParser))
            {
                return variableMonitoring;
            }

            throw new ArgumentException("The given JSON representation of a variable monitoring is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableMonitoring, CustomVariableMonitoringParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a variable monitoring.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableMonitoring">The parsed variable monitoring.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out VariableMonitoring?  VariableMonitoring,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out VariableMonitoring,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a variable monitoring.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableMonitoring">The parsed variable monitoring.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableMonitoringParser">A delegate to parse custom variable monitoring JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out VariableMonitoring?      VariableMonitoring,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<VariableMonitoring>?  CustomVariableMonitoringParser)
        {

            try
            {

                VariableMonitoring = default;

                #region Id             [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "variable monitoring identification",
                                         VariableMonitoring_Id.TryParse,
                                         out VariableMonitoring_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Transaction    [mandatory]

                if (!JSON.ParseMandatory("transaction",
                                         "transaction",
                                         out Boolean Transaction,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value          [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type           [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "monitor type",
                                         MonitorType.TryParse,
                                         out MonitorType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Severity       [mandatory]

                if (!JSON.ParseMandatory("severity",
                                         "severity",
                                         out Byte severity,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Severity = SeveritiesExtensions.TryParse(severity);

                if (!Severity.HasValue)
                    return false;

                #endregion

                #region CustomData     [optional]

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


                VariableMonitoring = new VariableMonitoring(
                                         Id,
                                         Transaction,
                                         Value,
                                         Type,
                                         Severity!.Value,
                                         CustomData
                                     );

                if (CustomVariableMonitoringParser is not null)
                    VariableMonitoring = CustomVariableMonitoringParser(JSON,
                                                                        VariableMonitoring);

                return true;

            }
            catch (Exception e)
            {
                VariableMonitoring  = default;
                ErrorResponse       = "The given JSON representation of a variable monitoring is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableMonitoringSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableMonitoringSerializer">A delegate to serialize custom VariableMonitoring objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VariableMonitoring>?  CustomVariableMonitoringSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.        Value),
                                 new JProperty("transaction",   Transaction),
                                 new JProperty("value",         Value),
                                 new JProperty("type",          Type.      ToString()),
                                 new JProperty("severity",      Severity.  AsNumber()),

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableMonitoringSerializer is not null
                       ? CustomVariableMonitoringSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public VariableMonitoring Clone()

            => new (

                   Id.  Clone(),
                   Transaction,
                   Value,
                   Type.Clone(),
                   Severity,

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (VariableMonitoring1, VariableMonitoring2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoring1">A variable monitoring.</param>
        /// <param name="VariableMonitoring2">Another variable monitoring.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VariableMonitoring? VariableMonitoring1,
                                           VariableMonitoring? VariableMonitoring2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VariableMonitoring1, VariableMonitoring2))
                return true;

            // If one is null, but not both, return false.
            if (VariableMonitoring1 is null || VariableMonitoring2 is null)
                return false;

            return VariableMonitoring1.Equals(VariableMonitoring2);

        }

        #endregion

        #region Operator != (VariableMonitoring1, VariableMonitoring2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoring1">A variable monitoring.</param>
        /// <param name="VariableMonitoring2">Another variable monitoring.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VariableMonitoring? VariableMonitoring1,
                                           VariableMonitoring? VariableMonitoring2)

            => !(VariableMonitoring1 == VariableMonitoring2);

        #endregion

        #endregion

        #region IEquatable<VariableMonitoring> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variable monitorings for equality.
        /// </summary>
        /// <param name="Object">A variable monitoring to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VariableMonitoring variableMonitoring &&
                   Equals(variableMonitoring);

        #endregion

        #region Equals(VariableMonitoring)

        /// <summary>
        /// Compares two variable monitorings for equality.
        /// </summary>
        /// <param name="VariableMonitoring">A variable monitoring to compare with.</param>
        public Boolean Equals(VariableMonitoring? VariableMonitoring)

            => VariableMonitoring is not null &&

               Id.         Equals(VariableMonitoring.Id)          &&
               Transaction.Equals(VariableMonitoring.Transaction) &&
               Value.      Equals(VariableMonitoring.Value)       &&
               Type.       Equals(VariableMonitoring.Type)        &&
               Severity.   Equals(VariableMonitoring.Severity)    &&

               base.       Equals(VariableMonitoring);

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

                return Id.         GetHashCode() * 13 ^
                       Transaction.GetHashCode() * 11 ^
                       Value.      GetHashCode() *  7 ^
                       Type.       GetHashCode() *  5 ^
                       Severity.   GetHashCode() *  3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id, ": ",
                   Value,
                   " (" , Type, "/",
                   Severity, ")"

               );

        #endregion

    }

}
