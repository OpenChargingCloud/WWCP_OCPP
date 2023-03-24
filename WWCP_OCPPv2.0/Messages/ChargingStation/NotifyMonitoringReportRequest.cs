﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// A notify monitoring report request.
    /// </summary>
    public class NotifyMonitoringReportRequest : ARequest<NotifyMonitoringReportRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the notify monitoring report request.
        /// </summary>
        [Mandatory]
        public Int32                        NotifyMonitoringReportRequestId    { get; }

        /// <summary>
        /// The sequence number of this message.
        /// First message starts at 0.
        /// </summary>
        [Mandatory]
        public UInt32                       SequenceNumber                     { get; }

        /// <summary>
        /// The timestamp of the moment this message was generated at the charging station.
        /// </summary>
        [Mandatory]
        public DateTime                     GeneratedAt                        { get; }

        /// <summary>
        /// The enumeration of event data.
        /// A single event data element contains only the component, variable
        /// and variable monitoring data that caused the event.
        /// </summary>
        [Mandatory]
        public IEnumerable<MonitoringData>  MonitoringData                     { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the monitoring
        /// data follows in an upcoming NotifyCustomerInformationRequest message.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                     ToBeContinued                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify monitoring report request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyMonitoringReportRequest(ChargeBox_Id                 ChargeBoxId,
                                             Int32                        NotifyMonitoringReportRequestId,
                                             UInt32                       SequenceNumber,
                                             DateTime                     GeneratedAt,
                                             IEnumerable<MonitoringData>  MonitoringData,
                                             Boolean?                     ToBeContinued       = null,
                                             CustomData?                  CustomData          = null,

                                             Request_Id?                  RequestId           = null,
                                             DateTime?                    RequestTimestamp    = null,
                                             TimeSpan?                    RequestTimeout      = null,
                                             EventTracking_Id?            EventTrackingId     = null,
                                             CancellationToken?           CancellationToken   = null)

            : base(ChargeBoxId,
                   "NotifyMonitoringReport",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            if (!MonitoringData.Any())
                throw new ArgumentException("The given enumeration of monitoring data must not be empty!",
                                            nameof(MonitoringData));

            this.NotifyMonitoringReportRequestId  = NotifyMonitoringReportRequestId;
            this.SequenceNumber                   = SequenceNumber;
            this.GeneratedAt                      = GeneratedAt;
            this.MonitoringData                   = MonitoringData.Distinct();
            this.ToBeContinued                    = ToBeContinued;

        }

        #endregion


        //WTF: monitor is optional, but minItems == 1?

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyMonitoringReportRequest",
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
        //     "MonitorEnumType": {
        //       "description": "The type of this monitor, e.g. a threshold, delta or periodic monitor. \r\n",
        //       "javaType": "MonitorEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "UpperThreshold",
        //         "LowerThreshold",
        //         "Delta",
        //         "Periodic",
        //         "PeriodicClockAligned"
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
        //     "MonitoringDataType": {
        //       "description": "Class to hold parameters of SetVariableMonitoring request.\r\n",
        //       "javaType": "MonitoringData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         },
        //         "variableMonitoring": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/VariableMonitoringType"
        //           },
        //           "minItems": 1
        //         }
        //       },
        //       "required": [
        //         "component",
        //         "variable",
        //         "variableMonitoring"
        //       ]
        //     },
        //     "VariableMonitoringType": {
        //       "description": "A monitoring setting for a variable.\r\n",
        //       "javaType": "VariableMonitoring",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identifies the monitor.\r\n",
        //           "type": "integer"
        //         },
        //         "transaction": {
        //           "description": "Monitor only active when a transaction is ongoing on a component relevant to this transaction. \r\n",
        //           "type": "boolean"
        //         },
        //         "value": {
        //           "description": "Value for threshold or delta monitoring.\r\nFor Periodic or PeriodicClockAligned this is the interval in seconds.\r\n",
        //           "type": "number"
        //         },
        //         "type": {
        //           "$ref": "#/definitions/MonitorEnumType"
        //         },
        //         "severity": {
        //           "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id",
        //         "transaction",
        //         "value",
        //         "type",
        //         "severity"
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
        //     "monitor": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/MonitoringDataType"
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The id of the GetMonitoringRequest that requested this report.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "tbc": {
        //       "description": "“to be continued” indicator. Indicates whether another part of the monitoringData follows in an upcoming notifyMonitoringReportRequest message. Default value when omitted is false.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "seqNo": {
        //       "description": "Sequence number of this message. First message starts at 0.\r\n",
        //       "type": "integer"
        //     },
        //     "generatedAt": {
        //       "description": "Timestamp of the moment this message was generated at the Charging Station.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "seqNo",
        //     "generatedAt"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyMonitoringReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify monitoring report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyMonitoringReportRequestParser">A delegate to parse custom notify monitoring report requests.</param>
        public static NotifyMonitoringReportRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          ChargeBox_Id                                                 ChargeBoxId,
                                                          CustomJObjectParserDelegate<NotifyMonitoringReportRequest>?  CustomNotifyMonitoringReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyMonitoringReportRequest,
                         out var errorResponse,
                         CustomNotifyMonitoringReportRequestParser))
            {
                return notifyMonitoringReportRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify monitoring report request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyMonitoringReportRequest, out ErrorResponse, CustomNotifyMonitoringReportRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify monitoring report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyMonitoringReportRequest">The parsed notify monitoring report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyMonitoringReportRequestParser">A delegate to parse custom notify monitoring report requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       ChargeBox_Id                                                 ChargeBoxId,
                                       out NotifyMonitoringReportRequest?                           NotifyMonitoringReportRequest,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyMonitoringReportRequest>?  CustomNotifyMonitoringReportRequestParser)
        {

            try
            {

                NotifyMonitoringReportRequest = null;

                #region NotifyMonitoringReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "notify monitoring report request identification",
                                         out Int32 NotifyMonitoringReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SequenceNumber                     [mandatory]

                if (!JSON.ParseMandatory("seqNo",
                                         "sequence number",
                                         out UInt32 SequenceNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region GeneratedAt                        [mandatory]

                if (!JSON.ParseMandatory("generatedAt",
                                         "generated at",
                                         out DateTime GeneratedAt,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MonitoringData                     [mandatory]

                if (!JSON.ParseMandatoryHashSet("monitor",
                                                "monitoring data",
                                                OCPPv2_0.MonitoringData.TryParse,
                                                out HashSet<MonitoringData> MonitoringData,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued                      [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                         [optional]

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

                #region ChargeBoxId                        [optional, OCPP_CSE]

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


                NotifyMonitoringReportRequest = new NotifyMonitoringReportRequest(ChargeBoxId,
                                                                                  NotifyMonitoringReportRequestId,
                                                                                  SequenceNumber,
                                                                                  GeneratedAt,
                                                                                  MonitoringData,
                                                                                  ToBeContinued,
                                                                                  CustomData,
                                                                                  RequestId);

                if (CustomNotifyMonitoringReportRequestParser is not null)
                    NotifyMonitoringReportRequest = CustomNotifyMonitoringReportRequestParser(JSON,
                                                                                              NotifyMonitoringReportRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyMonitoringReportRequest  = null;
                ErrorResponse                  = "The given JSON representation of a notify monitoring report request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyMonitoringReportRequestSerializer = null, CustomMonitoringDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyMonitoringReportRequestSerializer">A delegate to serialize custom NotifyMonitoringReport requests.</param>
        /// <param name="CustomMonitoringDataSerializer">A delegate to serialize custom MonitoringData objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomVariableMonitoringSerializer">A delegate to serialize custom variable monitoring objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?  CustomNotifyMonitoringReportRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MonitoringData>?                 CustomMonitoringDataSerializer                  = null,
                              CustomJObjectSerializerDelegate<Component>?                      CustomComponentSerializer                       = null,
                              CustomJObjectSerializerDelegate<EVSE>?                           CustomEVSESerializer                            = null,
                              CustomJObjectSerializerDelegate<Variable>?                       CustomVariableSerializer                        = null,
                              CustomJObjectSerializerDelegate<VariableMonitoring>?             CustomVariableMonitoringSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("requestId",     NotifyMonitoringReportRequestId.ToString()),
                                 new JProperty("seqNo",         SequenceNumber),
                                 new JProperty("generatedAt",   GeneratedAt.                    ToIso8601()),

                                 new JProperty("monitor",       new JArray(MonitoringData.Select(monitoringData => monitoringData.ToJSON(CustomMonitoringDataSerializer,
                                                                                                                                         CustomComponentSerializer,
                                                                                                                                         CustomEVSESerializer,
                                                                                                                                         CustomVariableSerializer,
                                                                                                                                         CustomVariableMonitoringSerializer,
                                                                                                                                         CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",           ToBeContinued.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.                     ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyMonitoringReportRequestSerializer is not null
                       ? CustomNotifyMonitoringReportRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyMonitoringReportRequest1, NotifyMonitoringReportRequest2)

        /// <summary>
        /// Compares two notify monitoring report requests for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequest1">A notify monitoring report request.</param>
        /// <param name="NotifyMonitoringReportRequest2">Another notify monitoring report request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyMonitoringReportRequest? NotifyMonitoringReportRequest1,
                                           NotifyMonitoringReportRequest? NotifyMonitoringReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyMonitoringReportRequest1, NotifyMonitoringReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyMonitoringReportRequest1 is null || NotifyMonitoringReportRequest2 is null)
                return false;

            return NotifyMonitoringReportRequest1.Equals(NotifyMonitoringReportRequest2);

        }

        #endregion

        #region Operator != (NotifyMonitoringReportRequest1, NotifyMonitoringReportRequest2)

        /// <summary>
        /// Compares two notify monitoring report requests for inequality.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequest1">A notify monitoring report request.</param>
        /// <param name="NotifyMonitoringReportRequest2">Another notify monitoring report request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyMonitoringReportRequest? NotifyMonitoringReportRequest1,
                                           NotifyMonitoringReportRequest? NotifyMonitoringReportRequest2)

            => !(NotifyMonitoringReportRequest1 == NotifyMonitoringReportRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyMonitoringReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify monitoring report requests for equality.
        /// </summary>
        /// <param name="Object">A notify monitoring report request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyMonitoringReportRequest notifyMonitoringReportRequest &&
                   Equals(notifyMonitoringReportRequest);

        #endregion

        #region Equals(NotifyMonitoringReportRequest)

        /// <summary>
        /// Compares two notify monitoring report requests for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequest">A notify monitoring report request to compare with.</param>
        public override Boolean Equals(NotifyMonitoringReportRequest? NotifyMonitoringReportRequest)

            => NotifyMonitoringReportRequest is not null &&

               NotifyMonitoringReportRequestId.Equals(NotifyMonitoringReportRequest.NotifyMonitoringReportRequestId) &&
               SequenceNumber.                 Equals(NotifyMonitoringReportRequest.SequenceNumber)                  &&
               GeneratedAt.                    Equals(NotifyMonitoringReportRequest.GeneratedAt)                     &&

               MonitoringData.Count().Equals(NotifyMonitoringReportRequest.MonitoringData.Count())     &&
               MonitoringData.All(data => NotifyMonitoringReportRequest.MonitoringData.Contains(data)) &&

            ((!ToBeContinued.HasValue && !NotifyMonitoringReportRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  NotifyMonitoringReportRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(NotifyMonitoringReportRequest.ToBeContinued.Value)) &&

               base.GenericEquals(NotifyMonitoringReportRequest);

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

                return NotifyMonitoringReportRequestId.GetHashCode()       * 17 ^
                       SequenceNumber.                 GetHashCode()       * 13 ^
                       GeneratedAt.                    GetHashCode()       * 11 ^
                       MonitoringData.                 CalcHashCode()      *  7 ^
                      (ToBeContinued?.                 GetHashCode() ?? 0) *  5 ^
                      (CustomData?.                    GetHashCode() ?? 0) *  3 ^

                       base.                           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   NotifyMonitoringReportRequestId,
                   ": ",
                   GeneratedAt.ToIso8601()

               );

        #endregion

    }

}
