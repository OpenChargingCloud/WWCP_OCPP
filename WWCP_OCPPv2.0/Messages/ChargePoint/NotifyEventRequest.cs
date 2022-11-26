/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// A notify event request.
    /// </summary>
    public class NotifyEventRequest : ARequest<NotifyEventRequest>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the moment this message was generated at the charging station.
        /// </summary>
        [Mandatory]
        public DateTime                GeneratedAt       { get; }

        /// <summary>
        /// The sequence number of this message.
        /// First message starts at 0.
        /// </summary>
        [Mandatory]
        public UInt32                  SequenceNumber    { get; }

        /// <summary>
        /// The enumeration of event data.
        /// A single event data element contains only the component, variable
        /// and variable monitoring data that caused the event.
        /// </summary>
        [Mandatory]
        public IEnumerable<EventData>  EventData         { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the monitoring
        /// data follows in an upcoming NotifyCustomerInformationRequest message.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                ToBeContinued     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify event request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="EventData">The enumeration of event data.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyEventRequest(ChargeBox_Id            ChargeBoxId,

                                  DateTime                GeneratedAt,
                                  UInt32                  SequenceNumber,
                                  IEnumerable<EventData>  EventData,
                                  Boolean?                ToBeContinued       = null,

                                  CustomData?             CustomData          = null,
                                  Request_Id?             RequestId           = null,
                                  DateTime?               RequestTimestamp    = null,
                                  TimeSpan?               RequestTimeout      = null,
                                  EventTracking_Id?       EventTrackingId     = null,
                                  CancellationToken?      CancellationToken   = null)

            : base(ChargeBoxId,
                   "NotifyEvent",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            if (!EventData.Any())
                throw new ArgumentException("The given enumeration of event data must not be empty!",
                                            nameof(EventData));

            this.GeneratedAt     = GeneratedAt;
            this.SequenceNumber  = SequenceNumber;
            this.EventData       = EventData.Distinct();
            this.ToBeContinued   = ToBeContinued;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyEventRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "EventNotificationEnumType": {
        //       "description": "Specifies the event notification type of the message.\r\n\r\n",
        //       "javaType": "EventNotificationEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "HardWiredNotification",
        //         "HardWiredMonitor",
        //         "PreconfiguredMonitor",
        //         "CustomMonitor"
        //       ]
        //     },
        //     "EventTriggerEnumType": {
        //       "description": "Type of monitor that triggered this event, e.g. exceeding a threshold value.\r\n\r\n",
        //       "javaType": "EventTriggerEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Alerting",
        //         "Delta",
        //         "Periodic"
        //       ]
        //     },
        //     "ComponentType": {
        //       "description": "A physical or logical component\r\n",
        //       "javaType": "Component",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "evse": {
        //           "$ref": "#/definitions/EVSEType"
        //         },
        //         "name": {
        //           "description": "Name of the component. Name should be taken from the list of standardized component names whenever possible. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the component exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "name"
        //       ]
        //     },
        //     "EventDataType": {
        //       "description": "Class to report an event notification for a component-variable.\r\n",
        //       "javaType": "EventData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "eventId": {
        //           "description": "Identifies the event. This field can be referred to as a cause by other events.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "timestamp": {
        //           "description": "Timestamp of the moment the report was generated.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "trigger": {
        //           "$ref": "#/definitions/EventTriggerEnumType"
        //         },
        //         "cause": {
        //           "description": "Refers to the Id of an event that is considered to be the cause for this event.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "actualValue": {
        //           "description": "Actual value (_attributeType_ Actual) of the variable.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-reporting-value-size,ReportingValueSize&gt;&gt; can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. The max size of these values will always remain equal. \r\n\r\n",
        //           "type": "string",
        //           "maxLength": 2500
        //         },
        //         "techCode": {
        //           "description": "Technical (error) code as reported by component.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "techInfo": {
        //           "description": "Technical detail information as reported by component.\r\n",
        //           "type": "string",
        //           "maxLength": 500
        //         },
        //         "cleared": {
        //           "description": "_Cleared_ is set to true to report the clearing of a monitored situation, i.e. a 'return to normal'. \r\n\r\n",
        //           "type": "boolean"
        //         },
        //         "transactionId": {
        //           "description": "If an event notification is linked to a specific transaction, this field can be used to specify its transactionId.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variableMonitoringId": {
        //           "description": "Identifies the VariableMonitoring which triggered the event.\r\n",
        //           "type": "integer"
        //         },
        //         "eventNotificationType": {
        //           "$ref": "#/definitions/EventNotificationEnumType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
        //         "eventId",
        //         "timestamp",
        //         "trigger",
        //         "actualValue",
        //         "eventNotificationType",
        //         "component",
        //         "variable"
        //       ]
        //     },
        //     "EVSEType": {
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //       "javaType": "EVSE",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
        //       ]
        //     },
        //     "VariableType": {
        //       "description": "Reference key to a component-variable.\r\n",
        //       "javaType": "Variable",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "name": {
        //           "description": "Name of the variable. Name should be taken from the list of standardized variable names whenever possible. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the variable exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "name"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "generatedAt": {
        //       "description": "Timestamp of the moment this message was generated at the Charging Station.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "tbc": {
        //       "description": "“to be continued” indicator. Indicates whether another part of the report follows in an upcoming notifyEventRequest message. Default value when omitted is false. \r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "seqNo": {
        //       "description": "Sequence number of this message. First message starts at 0.\r\n",
        //       "type": "integer"
        //     },
        //     "eventData": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/EventDataType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "generatedAt",
        //     "seqNo",
        //     "eventData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyEventRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyEventRequestParser">A delegate to parse custom notify event requests.</param>
        public static NotifyEventRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               ChargeBox_Id                                      ChargeBoxId,
                                               CustomJObjectParserDelegate<NotifyEventRequest>?  CustomNotifyEventRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyEventRequest,
                         out var errorResponse,
                         CustomNotifyEventRequestParser))
            {
                return notifyEventRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify event request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyEventRequest, out ErrorResponse, CustomNotifyEventRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyEventRequest">The parsed notify event request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEventRequestParser">A delegate to parse custom notify event requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       ChargeBox_Id                                      ChargeBoxId,
                                       out NotifyEventRequest?                           NotifyEventRequest,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEventRequest>?  CustomNotifyEventRequestParser)
        {

            try
            {

                NotifyEventRequest = null;

                #region GeneratedAt       [mandatory]

                if (!JSON.ParseMandatory("generatedAt",
                                         "generated at",
                                         out DateTime GeneratedAt,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SequenceNumber    [mandatory]

                if (!JSON.ParseMandatory("seqNo",
                                         "sequence number",
                                         out UInt32 SequenceNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingNeeds     [mandatory]

                if (!JSON.ParseMandatoryHashSet("eventData",
                                                "event data",
                                                OCPPv2_0.EventData.TryParse,
                                                out HashSet<EventData> EventData,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued     [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
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

                #region ChargeBoxId       [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyEventRequest = new NotifyEventRequest(ChargeBoxId,
                                                            GeneratedAt,
                                                            SequenceNumber,
                                                            EventData,
                                                            ToBeContinued,
                                                            CustomData,
                                                            RequestId);

                if (CustomNotifyEventRequestParser is not null)
                    NotifyEventRequest = CustomNotifyEventRequestParser(JSON,
                                                                        NotifyEventRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEventRequest  = null;
                ErrorResponse       = "The given JSON representation of a notify event request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEventRequestSerializer = null, CustomEventDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEventRequestSerializer">A delegate to serialize custom NotifyEvent requests.</param>
        /// <param name="CustomEventDataSerializer">A delegate to serialize custom event data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom Component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE objects.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom Variable objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEventRequest>?  CustomNotifyEventRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<EventData>?           CustomEventDataSerializer            = null,
                              CustomJObjectSerializerDelegate<Component>?           CustomComponentSerializer            = null,
                              CustomJObjectSerializerDelegate<EVSE>?                CustomEVSESerializer                 = null,
                              CustomJObjectSerializerDelegate<Variable>?            CustomVariableSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("generatedAt",  GeneratedAt.ToIso8601()),

                                 new JProperty("seqNo",        SequenceNumber),

                                 new JProperty("eventData",    new JArray(EventData.Select(eventData => eventData.ToJSON(CustomEventDataSerializer,
                                                                                                                         CustomComponentSerializer,
                                                                                                                         CustomEVSESerializer,
                                                                                                                         CustomVariableSerializer,
                                                                                                                         CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",          ToBeContinued.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEventRequestSerializer is not null
                       ? CustomNotifyEventRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEventRequest1, NotifyEventRequest2)

        /// <summary>
        /// Compares two notify event requests for equality.
        /// </summary>
        /// <param name="NotifyEventRequest1">A notify event request.</param>
        /// <param name="NotifyEventRequest2">Another notify event request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEventRequest? NotifyEventRequest1,
                                           NotifyEventRequest? NotifyEventRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEventRequest1, NotifyEventRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEventRequest1 is null || NotifyEventRequest2 is null)
                return false;

            return NotifyEventRequest1.Equals(NotifyEventRequest2);

        }

        #endregion

        #region Operator != (NotifyEventRequest1, NotifyEventRequest2)

        /// <summary>
        /// Compares two notify event requests for inequality.
        /// </summary>
        /// <param name="NotifyEventRequest1">A notify event request.</param>
        /// <param name="NotifyEventRequest2">Another notify event request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEventRequest? NotifyEventRequest1,
                                           NotifyEventRequest? NotifyEventRequest2)

            => !(NotifyEventRequest1 == NotifyEventRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEventRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify event requests for equality.
        /// </summary>
        /// <param name="Object">A notify event request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEventRequest notifyEventRequest &&
                   Equals(notifyEventRequest);

        #endregion

        #region Equals(NotifyEventRequest)

        /// <summary>
        /// Compares two notify event requests for equality.
        /// </summary>
        /// <param name="NotifyEventRequest">A notify event request to compare with.</param>
        public override Boolean Equals(NotifyEventRequest? NotifyEventRequest)

            => NotifyEventRequest is not null &&

               GeneratedAt.   Equals(NotifyEventRequest.GeneratedAt)    &&
               SequenceNumber.Equals(NotifyEventRequest.SequenceNumber) &&

               EventData.Count().Equals(NotifyEventRequest.EventData.Count())     &&
               EventData.All(data => NotifyEventRequest.EventData.Contains(data)) &&

            ((!ToBeContinued.HasValue && !NotifyEventRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  NotifyEventRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(NotifyEventRequest.ToBeContinued.Value)) &&

               base.GenericEquals(NotifyEventRequest);

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

                return GeneratedAt.   GetHashCode()       * 13 ^
                       SequenceNumber.GetHashCode()       * 11 ^
                      //ToDo: Add EventData
                      (ToBeContinued?.GetHashCode() ?? 0) *  5 ^
                      (CustomData?.   GetHashCode() ?? 0) *  3 ^

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

                   GeneratedAt.ToIso8601(),
                   ": ",
                   EventData.ToString()

               );

        #endregion

    }

}
