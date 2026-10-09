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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The event data allows to report an event notification for a component-variable.
    /// </summary>
    public class EventData : ACustomData,
                             ICBORSerializable<EventData>,
                             IEquatable<EventData>
    {

        #region Properties

        /// <summary>
        /// The unique event identification.
        /// This field can be referred to as a cause by other events.
        /// </summary>
        [Mandatory]
        public Event_Id                EventId                  { get; }

        /// <summary>
        /// The timestamp when the event report was generated.
        /// </summary>
        [Mandatory]
        public DateTimeOffset          Timestamp                { get; }

        /// <summary>
        /// The type of monitor that triggered this event, e.g. exceeding a threshold value.
        /// </summary>
        [Mandatory]
        public EventTriggers           Trigger                  { get; }

        /// <summary>
        /// The actual value (attributeType "Actual") of the variable.
        /// The configuration variable "ReportingValueSize" can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue.
        /// The max size of these values will always remain equal.
        /// </summary>
        [Mandatory]
        public String                  ActualValue              { get; }

        /// <summary>
        /// The event notification type of the message.
        /// </summary>
        [Mandatory]
        public EventNotificationType   EventNotificationType    { get; }

        /// <summary>
        /// The component for which event is notified.
        /// </summary>
        [Mandatory]
        public Component               Component                { get; }

        /// <summary>
        /// The variable for which event is notified.
        /// </summary>
        [Mandatory]
        public Variable                Variable                 { get; }

        /// <summary>
        /// Severity associated with monitor in variableMonitoringId or the hardwired notification.
        /// Optional/Required
        /// </summary>
        [Optional]
        public Severities?             Severity                 { get; }

        /// <summary>
        /// The optional event identification that is considered to be the cause for this event.
        /// </summary>
        [Optional]
        public Event_Id?               Cause                    { get; }

        /// <summary>
        /// The optional technical (error) code as reported by the component.
        /// </summary>
        [Optional]
        public String?                 TechCode                 { get; }

        /// <summary>
        /// The optional technical detail information as reported by the component.
        /// </summary>
        [Optional]
        public String?                 TechInfo                 { get; }

        /// <summary>
        /// The optional "cleared" bit is set to true to report the clearing of a
        /// monitored situation, i.e.a 'return to normal'.
        /// </summary>
        [Optional]
        public Boolean?                Cleared                  { get; }

        /// <summary>
        /// The optional transaction identification links this event notification to a specific transaction.
        /// </summary>
        [Optional]
        public Transaction_Id?         TransactionId            { get; }

        /// <summary>
        /// The optional variable monitoring identification which triggered this event.
        /// </summary>
        [Optional]
        public VariableMonitoring_Id?  VariableMonitoringId     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a event data.
        /// </summary>
        /// <param name="EventId">The unique event identification. This field can be referred to as a cause by other events.</param>
        /// <param name="Timestamp">The timestamp when the event report was generated.</param>
        /// <param name="Trigger">The type of monitor that triggered this event, e.g. exceeding a threshold value.</param>
        /// <param name="ActualValue">The actual value (attributeType "Actual") of the variable.</param>
        /// <param name="EventNotificationType">The event notification type of the message.</param>
        /// <param name="Component">The component for which event is notified.</param>
        /// <param name="Variable">The variable for which event is notified.</param>
        /// <param name="Severity">Severity associated with monitor in variableMonitoringId or the hardwired notification [Optional/Required].</param>
        /// <param name="Cause">An optional event identification that is considered to be the cause for this event.</param>
        /// <param name="TechCode">An optional technical (error) code as reported by the component.</param>
        /// <param name="TechInfo">An optional technical detail information as reported by the component.</param>
        /// <param name="Cleared">The optional "cleared" bit is set to true to report the clearing of a monitored situation, i.e.a 'return to normal'.</param>
        /// <param name="TransactionId">An optional transaction identification links this event notification to a specific transaction.</param>
        /// <param name="VariableMonitoringId">An optional variable monitoring identification which triggered this event.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EventData(Event_Id                EventId,
                         DateTimeOffset          Timestamp,
                         EventTriggers           Trigger,
                         String                  ActualValue,
                         EventNotificationType   EventNotificationType,
                         Component               Component,
                         Variable                Variable,
                         Severities?             Severity               = null,
                         Event_Id?               Cause                  = null,
                         String?                 TechCode               = null,
                         String?                 TechInfo               = null,
                         Boolean?                Cleared                = null,
                         Transaction_Id?         TransactionId          = null,
                         VariableMonitoring_Id?  VariableMonitoringId   = null,
                         CustomData?             CustomData             = null)

            : base(CustomData)

        {

            this.EventId                = EventId;
            this.Timestamp              = Timestamp;
            this.Trigger                = Trigger;
            this.ActualValue            = ActualValue;
            this.EventNotificationType  = EventNotificationType;
            this.Component              = Component;
            this.Variable               = Variable;
            this.Severity               = Severity;
            this.Cause                  = Cause;
            this.TechCode               = TechCode;
            this.TechInfo               = TechInfo;
            this.Cleared                = Cleared;
            this.TransactionId          = TransactionId;
            this.VariableMonitoringId   = VariableMonitoringId;

            unchecked
            {

                hashCode = this.EventId.              GetHashCode()       * 53 ^
                           this.Timestamp.            GetHashCode()       * 47 ^
                           this.Trigger.              GetHashCode()       * 43 ^
                           this.ActualValue.          GetHashCode()       * 41 ^
                           this.EventNotificationType.GetHashCode()       * 37 ^
                           this.Component.            GetHashCode()       * 31 ^
                           this.Variable.             GetHashCode()       * 29 ^
                          (this.Severity?.            GetHashCode() ?? 0) * 23 ^
                          (this.Cause?.               GetHashCode() ?? 0) * 19 ^
                          (this.TechCode?.            GetHashCode() ?? 0) * 17 ^
                          (this.TechInfo?.            GetHashCode() ?? 0) * 11 ^
                          (this.Cleared?.             GetHashCode() ?? 0) *  7 ^
                          (this.TransactionId?.       GetHashCode() ?? 0) *  5 ^
                          (this.VariableMonitoringId?.GetHashCode() ?? 0) *  3 ^
                           base.                      GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Class to report an event notification for a component-variable.",
        //     "javaType": "EventData",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "eventId": {
        //             "description": "Identifies the event. This field can be referred to as a cause by other events.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "timestamp": {
        //             "description": "Timestamp of the moment the report was generated.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "trigger": {
        //             "$ref": "#/definitions/EventTriggerEnumType"
        //         },
        //         "cause": {
        //             "description": "Refers to the Id of an event that is considered to be the cause for this event.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "actualValue": {
        //             "description": "Actual value (_attributeType_ Actual) of the variable.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-reporting-value-size,ReportingValueSize&gt;&gt; can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. The max size of these values will always remain equal. ",
        //             "type": "string",
        //             "maxLength": 2500
        //         },
        //         "techCode": {
        //             "description": "Technical (error) code as reported by component.",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "techInfo": {
        //             "description": "Technical detail information as reported by component.",
        //             "type": "string",
        //             "maxLength": 500
        //         },
        //         "cleared": {
        //             "description": "_Cleared_ is set to true to report the clearing of a monitored situation, i.e. a 'return to normal'. ",
        //             "type": "boolean"
        //         },
        //         "transactionId": {
        //             "description": "If an event notification is linked to a specific transaction, this field can be used to specify its transactionId.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "component": {
        //             "$ref": "#/definitions/ComponentType"
        //         },
        //         "variableMonitoringId": {
        //             "description": "Identifies the VariableMonitoring which triggered the event.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "eventNotificationType": {
        //             "$ref": "#/definitions/EventNotificationEnumType"
        //         },
        //         "variable": {
        //             "$ref": "#/definitions/VariableType"
        //         },
        //         "severity": {
        //             "description": "*(2.1)* Severity associated with the monitor in _variableMonitoringId_ or with the hardwired notification.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "eventId",
        //         "timestamp",
        //         "trigger",
        //         "actualValue",
        //         "eventNotificationType",
        //         "component",
        //         "variable"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEventDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of event data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEventDataParser">A delegate to parse custom event data JSON objects.</param>
        public static EventData Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<EventData>?  CustomEventDataParser   = null)
        {

            if (TryParse(JSON,
                         out var eventData,
                         out var errorResponse,
                         CustomEventDataParser))
            {
                return eventData;
            }

            throw new ArgumentException("The given JSON representation of event data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EventData, CustomEventDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of event data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EventData">The parsed eventData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out EventData?  EventData,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out EventData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of event data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EventData">The parsed eventData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEventDataParser">A delegate to parse custom event data JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out EventData?      EventData,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<EventData>?  CustomEventDataParser)
        {

            try
            {

                EventData = default;

                #region EventId                  [mandatory]

                if (!JSON.ParseMandatory("eventId",
                                         "event identification",
                                         Event_Id.TryParse,
                                         out Event_Id EventId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp                [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Trigger                  [mandatory]

                if (!JSON.ParseMandatory("trigger",
                                         "event trigger",
                                         EventTriggersExtensions.TryParse,
                                         out EventTriggers Trigger,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ActualValue              [mandatory]

                if (!JSON.ParseMandatoryText("actualValue",
                                             "actual value",
                                             out String? ActualValue,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EventNotificationType    [mandatory]

                if (!JSON.ParseMandatory("eventNotificationType",
                                         "event notification type",
                                         OCPPv2_1.EventNotificationType.TryParse,
                                         out EventNotificationType EventNotificationType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Component                [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable                 [mandatory]

                if (!JSON.ParseMandatoryJSON("variable",
                                             "variable",
                                             OCPPv2_1.Variable.TryParse,
                                             out Variable? Variable,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Severity                 [optional]

                // The schema's number 0-9; the text it was written as before is still read.
                Severities? Severity = null;

                if (JSON["severity"] is JToken severityToken)
                {

                    if (!(severityToken.Type == JTokenType.Integer
                              ? severityToken.Value<Int64>() is >= 0 and <= Byte.MaxValue && SeveritiesExtensions.TryParse((Byte) severityToken.Value<Int64>(), out var severity)
                              : SeveritiesExtensions.TryParse(severityToken.Value<String>() ?? "", out severity)))
                    {
                        ErrorResponse = $"Invalid severity '{severityToken}'!";
                        return false;
                    }

                    Severity = severity;

                }

                #endregion

                #region Cause                    [optional]

                if (JSON.ParseOptional("cause",
                                       "cause",
                                       Event_Id.TryParse,
                                       out Event_Id? Cause,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TechCode                 [optional]

                var TechCode = JSON.GetString("techCode");

                #endregion

                #region TechInfo                 [optional]

                var TechInfo = JSON.GetString("techInfo");

                #endregion

                #region Cleared                  [optional]

                if (JSON.ParseOptional("cleared",
                                       "cleared",
                                       out Boolean? Cleared,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionId            [optional]

                if (JSON.ParseOptional("transactionId",
                                       "transaction identification",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VariableMonitoringId     [optional]

                if (JSON.ParseOptional("variableMonitoringId",
                                       "variable monitoring identification",
                                       VariableMonitoring_Id.TryParse,
                                       out VariableMonitoring_Id? VariableMonitoringId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData               [optional]

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


                EventData = new EventData(
                                EventId,
                                Timestamp,
                                Trigger,
                                ActualValue,
                                EventNotificationType,
                                Component,
                                Variable,
                                Severity,
                                Cause,
                                TechCode,
                                TechInfo,
                                Cleared,
                                TransactionId,
                                VariableMonitoringId,
                                CustomData
                            );

                if (CustomEventDataParser is not null)
                    EventData = CustomEventDataParser(JSON,
                                                      EventData);

                return true;

            }
            catch (Exception e)
            {
                EventData      = default;
                ErrorResponse  = "The given JSON representation of event data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEventDataSerializer = null, CustomComponentSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEventDataSerializer">A delegate to serialize custom event data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EventData>?   CustomEventDataSerializer    = null,
                              CustomJObjectSerializerDelegate<Component>?   CustomComponentSerializer    = null,
                              CustomJObjectSerializerDelegate<EVSE>?        CustomEVSESerializer         = null,
                              CustomJObjectSerializerDelegate<Variable>?    CustomVariableSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("eventId",                EventId.Value),
                                 new JProperty("timestamp",              Timestamp.                 ToISO8601()),
                                 new JProperty("trigger",                Trigger.                   AsText()),
                                 new JProperty("actualValue",            ActualValue),
                                 new JProperty("eventNotificationType",  EventNotificationType.     ToString()),

                                 new JProperty("component",              Component.                 ToJSON(CustomComponentSerializer,
                                                                                                           CustomEVSESerializer)),

                                 new JProperty("variable",               Variable.                  ToJSON(CustomVariableSerializer,
                                                                                                           CustomCustomDataSerializer)),

                           Severity.HasValue
                               ? new JProperty("severity",               Severity.            Value.AsNumber())
                               : null,

                           Cause is not null
                               ? new JProperty("cause",                  Cause.               Value.Value)
                               : null,

                           TechCode is not null
                               ? new JProperty("techCode",               TechCode)
                               : null,

                           TechInfo is not null
                               ? new JProperty("techInfo",               TechInfo)
                               : null,

                           Cleared is not null
                               ? new JProperty("cleared",                Cleared.             Value)
                               : null,

                           TransactionId is not null
                               ? new JProperty("transactionId",          TransactionId.       Value.ToString())
                               : null,

                           VariableMonitoringId is not null
                               ? new JProperty("variableMonitoringId",   VariableMonitoringId.Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.                ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEventDataSerializer is not null
                       ? CustomEventDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out EventData, out ErrorResponse, CustomEventDataParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of an event data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="EventData">The event data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out EventData?  EventData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out EventData,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of an event data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="EventData">The event data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEventDataParser">An optional delegate to read custom event data.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out EventData?           EventData,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<EventData>?  CustomEventDataParser)
        {

            try
            {

                EventData = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an event data is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("eventId",
                                               "event identification",
                                               out var EventIdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (EventIdNumber > UInt64.MaxValue || !Event_Id.TryParse((UInt64) EventIdNumber, out var EventId))
                {
                    ErrorResponse = $"Invalid event identification '{EventIdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("timestamp",
                                              "timestamp",
                                              OCPPCBORExtensions.TryParseTimestamp,
                                              out DateTimeOffset Timestamp,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryText("trigger",
                                             "event trigger",
                                             out var TriggerText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!EventTriggersExtensions.TryParse(TriggerText, out var Trigger))
                {
                    ErrorResponse = $"Invalid event trigger '{TriggerText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("actualValue",
                                             "actual value",
                                             out var ActualValue,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryText("eventNotificationType",
                                             "event notification type",
                                             out var EventNotificationTypeText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv2_1.EventNotificationType.TryParse(EventNotificationTypeText, out var EventNotificationType))
                {
                    ErrorResponse = $"Invalid event notification type '{EventNotificationTypeText}'!";
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

                Severities? Severity = null;

                if (CBOR.ParseOptionalUInt64("severity",
                                             "severity",
                                             out var severityNumber,
                                             out ErrorResponse))
                {

                    if (severityNumber is not UInt64 severityValue || severityValue > Byte.MaxValue || !SeveritiesExtensions.TryParse((Byte) severityValue, out var severity))
                    {
                        ErrorResponse = $"Invalid severity '{severityNumber}'!";
                        return false;
                    }

                    Severity = severity;

                }

                if (ErrorResponse is not null)
                    return false;

                Event_Id? Cause = null;

                if (CBOR.ParseOptionalUInt64("cause",
                                             "cause",
                                             out var CauseNumber,
                                             out ErrorResponse))
                {

                    if (CauseNumber is not UInt64 CauseValue || CauseValue > UInt64.MaxValue || !Event_Id.TryParse((UInt64) CauseValue, out var CauseId))
                    {
                        ErrorResponse = $"Invalid cause '{CauseNumber}'!";
                        return false;
                    }

                    Cause = CauseId;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("techCode",
                                       "technical code",
                                       out var TechCode,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("techInfo",
                                       "technical information",
                                       out var TechInfo,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("cleared",
                                          "cleared",
                                          out var Cleared,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                Transaction_Id? TransactionId = null;

                if (CBOR.ParseOptionalText("transactionId",
                                           "transaction identification",
                                           out var TransactionIdText,
                                           out ErrorResponse))
                {

                    if (!Transaction_Id.TryParse(TransactionIdText!, out var TransactionIdValue))
                    {
                        ErrorResponse = $"Invalid transaction identification '{TransactionIdText}'!";
                        return false;
                    }

                    TransactionId = TransactionIdValue;

                }

                if (ErrorResponse is not null)
                    return false;

                VariableMonitoring_Id? VariableMonitoringId = null;

                if (CBOR.ParseOptionalUInt64("variableMonitoringId",
                                             "variable monitoring identification",
                                             out var VariableMonitoringIdNumber,
                                             out ErrorResponse))
                {

                    if (VariableMonitoringIdNumber is not UInt64 VariableMonitoringIdValue || VariableMonitoringIdValue > UInt64.MaxValue || !VariableMonitoring_Id.TryParse((UInt64) VariableMonitoringIdValue, out var VariableMonitoringIdId))
                    {
                        ErrorResponse = $"Invalid variable monitoring identification '{VariableMonitoringIdNumber}'!";
                        return false;
                    }

                    VariableMonitoringId = VariableMonitoringIdId;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                EventData = new EventData(
                                EventId,
                                Timestamp,
                                Trigger,
                                ActualValue,
                                EventNotificationType,
                                Component,
                                Variable,
                                Severity,
                                Cause,
                                TechCode,
                                TechInfo,
                                Cleared,
                                TransactionId,
                                VariableMonitoringId,
                                CustomData
                            );

                if (CustomEventDataParser is not null)
                    EventData = CustomEventDataParser(CBOR,
                                           EventData);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                EventData  = default;
                ErrorResponse  = "The given CBOR representation of an event data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<EventData>.TryParse(CBOR, out EventData, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of an event data - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<EventData>.TryParse(CBORValue                         CBOR,
                                                           out EventData                  Value,
                                                           [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomEventDataSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this event data: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomEventDataSerializer">A delegate to serialize custom event data.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<EventData>? CustomEventDataSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("eventId",                 CBORValue.FromUInt64(EventId.Value)),
                           ("timestamp",               Timestamp.ToCBOR()),
                           ("trigger",                 CBORValue.FromText(Trigger.AsText())),
                           ("actualValue",             CBORValue.FromText(ActualValue)),
                           ("eventNotificationType",   CBORValue.FromText(EventNotificationType.ToString())),
                           ("component",               Component.ToCBOR()),
                           ("variable",                Variable. ToCBOR()),
                           ("severity",                OCPPCBORExtensions.UInt(Severity?.AsNumber())),
                           ("cause",                   OCPPCBORExtensions.UInt(Cause?.Value)),
                           ("techCode",                OCPPCBORExtensions.Text(TechCode)),
                           ("techInfo",                OCPPCBORExtensions.Text(TechInfo)),
                           ("cleared",                 OCPPCBORExtensions.Flag(Cleared)),
                           ("transactionId",           OCPPCBORExtensions.Text(TransactionId?.ToString())),
                           ("variableMonitoringId",    OCPPCBORExtensions.UInt(VariableMonitoringId?.Value)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomEventDataSerializer is not null
                       ? CustomEventDataSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (EventData1, EventData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventData1">Event data.</param>
        /// <param name="EventData2">Other event data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EventData? EventData1,
                                           EventData? EventData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EventData1, EventData2))
                return true;

            // If one is null, but not both, return false.
            if (EventData1 is null || EventData2 is null)
                return false;

            return EventData1.Equals(EventData2);

        }

        #endregion

        #region Operator != (EventData1, EventData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventData1">Event data.</param>
        /// <param name="EventData2">Other event data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EventData? EventData1,
                                           EventData? EventData2)

            => !(EventData1 == EventData2);

        #endregion

        #endregion

        #region IEquatable<EventData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two event data for equality.
        /// </summary>
        /// <param name="Object">Event data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EventData eventData &&
                   Equals(eventData);

        #endregion

        #region Equals(EventData)

        /// <summary>
        /// Compares two event data for equality.
        /// </summary>
        /// <param name="EventData">Event data to compare with.</param>
        public Boolean Equals(EventData? EventData)

            => EventData is not null &&

               EventId.              Equals(EventData.EventId)                                      &&
               Timestamp.            Equals(EventData.Timestamp)                                    &&
               Trigger.              Equals(EventData.Trigger)                                      &&
               ActualValue.          Equals(EventData.ActualValue)                                  &&
               EventNotificationType.Equals(EventData.EventNotificationType)                        &&
               Component.            Equals(EventData.Component)                                    &&
               Variable.             Equals(EventData.Variable)                                     &&

            ((!Severity.            HasValue    && !EventData.Severity.            HasValue) ||
              (Severity.            HasValue    &&  EventData.Severity.            HasValue    && Severity.            Value.Equals(EventData.Severity.            Value))) &&

            ((!Cause.               HasValue    && !EventData.Cause.               HasValue) ||
              (Cause.               HasValue    &&  EventData.Cause.               HasValue    && Cause.               Value.Equals(EventData.Cause.               Value))) &&

             ((TechCode             is null     &&  EventData.TechCode             is null)  ||
              (TechCode             is not null &&  EventData.TechCode             is not null && TechCode.                  Equals(EventData.TechCode)))                   &&

             ((TechInfo             is null     &&  EventData.TechInfo             is null)  ||
              (TechInfo             is not null &&  EventData.TechInfo             is not null && TechInfo.                  Equals(EventData.TechInfo)))                   &&

            ((!Cleared.             HasValue    && !EventData.Cleared.             HasValue) ||
              (Cleared.             HasValue    &&  EventData.Cleared.             HasValue    && Cleared.             Value.Equals(EventData.Cleared.             Value))) &&

            ((!TransactionId.       HasValue    && !EventData.TransactionId.       HasValue) ||
              (TransactionId.       HasValue    &&  EventData.TransactionId.       HasValue    && TransactionId.       Value.Equals(EventData.TransactionId.       Value))) &&

            ((!VariableMonitoringId.HasValue    && !EventData.VariableMonitoringId.HasValue) ||
              (VariableMonitoringId.HasValue    &&  EventData.VariableMonitoringId.HasValue    && VariableMonitoringId.Value.Equals(EventData.VariableMonitoringId.Value))) &&

             ((CustomData           is null     &&  EventData.CustomData           is null) ||
              (CustomData           is not null &&  EventData.CustomData           is not null && CustomData.                Equals(EventData.CustomData)))                 &&

               base.                 Equals(EventData);

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

                   EventId.ToString(),
                   ", ",
                   Timestamp.ToISO8601(),
                   ": ",
                   ActualValue.SubstringMax(30)

               );

        #endregion

    }

}
