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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyDERAlarm request.
    /// </summary>
    public class NotifyDERAlarmRequest : ARequest<NotifyDERAlarmRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyDERAlarmRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;


        /// <summary>
        /// The DER control type.
        /// </summary>
        [Mandatory]
        public DERControlType       ControlType           { get; }

        /// <summary>
        /// The timestamp of the alarm start or end.
        /// </summary>
        [Mandatory]
        public DateTime             Timestamp             { get; }

        /// <summary>
        /// The type of grid fault that caused this alarm.
        /// </summary>
        [Optional]
        public GridEventFaultType?  GridEventFaultType    { get; }

        /// <summary>
        /// Whether the alarm has ended.
        /// </summary>
        [Optional]
        public Boolean?             AlarmEnded            { get; }

        /// <summary>
        /// Optional information provided by the EV.
        /// </summary>
        [Optional]
        public String?              ExtraInfo             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyDERAlarm request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ControlType">The DER control type.</param>
        /// <param name="Timestamp">The timestamp of the alarm start or end.</param>
        /// <param name="GridEventFaultType">The type of grid fault that caused this alarm.</param>
        /// <param name="AlarmEnded">Whether the alarm has ended.</param>
        /// <param name="ExtraInfo">Optional information provided by the EV.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyDERAlarmRequest(SourceRouting            Destination,
                                     DERControlType           ControlType,
                                     DateTime                 Timestamp,
                                     GridEventFaultType?      GridEventFaultType    = null,
                                     Boolean?                 AlarmEnded            = null,
                                     String?                  ExtraInfo             = null,

                                     IEnumerable<KeyPair>?    SignKeys              = null,
                                     IEnumerable<SignInfo>?   SignInfos             = null,
                                     IEnumerable<Signature>?  Signatures            = null,

                                     CustomData?              CustomData            = null,

                                     Request_Id?              RequestId             = null,
                                     DateTime?                RequestTimestamp      = null,
                                     TimeSpan?                RequestTimeout        = null,
                                     EventTracking_Id?        EventTrackingId       = null,
                                     NetworkPath?             NetworkPath           = null,
                                     SerializationFormats?    SerializationFormat   = null,
                                     CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyDERAlarmRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ControlType     = ControlType;
            this.Timestamp       = Timestamp;
            this.GridEventFaultType  = GridEventFaultType;
            this.AlarmEnded      = AlarmEnded;
            this.ExtraInfo       = ExtraInfo;

            unchecked
            {

                hashCode = this.ControlType.        GetHashCode()       * 13 ^
                           this.Timestamp.          GetHashCode()       * 11 ^
                          (this.GridEventFaultType?.GetHashCode() ?? 0) *  7 ^
                          (this.AlarmEnded?.        GetHashCode() ?? 0) *  5 ^
                          (this.ExtraInfo?.         GetHashCode() ?? 0) *  3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyDERAlarmRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DERControlEnumType": {
        //             "description": "Name of DER control, e.g. LFMustTrip",
        //             "javaType": "DERControlEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EnterService",
        //                 "FreqDroop",
        //                 "FreqWatt",
        //                 "FixedPFAbsorb",
        //                 "FixedPFInject",
        //                 "FixedVar",
        //                 "Gradients",
        //                 "HFMustTrip",
        //                 "HFMayTrip",
        //                 "HVMustTrip",
        //                 "HVMomCess",
        //                 "HVMayTrip",
        //                 "LimitMaxDischarge",
        //                 "LFMustTrip",
        //                 "LVMustTrip",
        //                 "LVMomCess",
        //                 "LVMayTrip",
        //                 "PowerMonitoringMustTrip",
        //                 "VoltVar",
        //                 "VoltWatt",
        //                 "WattPF",
        //                 "WattVar"
        //             ]
        //         },
        //         "GridEventFaultEnumType": {
        //             "description": "Type of grid event that caused this",
        //             "javaType": "GridEventFaultEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "CurrentImbalance",
        //                 "LocalEmergency",
        //                 "LowInputPower",
        //                 "OverCurrent",
        //                 "OverFrequency",
        //                 "OverVoltage",
        //                 "PhaseRotation",
        //                 "RemoteEmergency",
        //                 "UnderFrequency",
        //                 "UnderVoltage",
        //                 "VoltageImbalance"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "controlType": {
        //             "$ref": "#/definitions/DERControlEnumType"
        //         },
        //         "gridEventFault": {
        //             "$ref": "#/definitions/GridEventFaultEnumType"
        //         },
        //         "alarmEnded": {
        //             "description": "True when error condition has ended.\r\nAbsent or false when alarm has started.",
        //             "type": "boolean"
        //         },
        //         "timestamp": {
        //             "description": "Time of start or end of alarm.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "extraInfo": {
        //             "description": "Optional info provided by EV.",
        //             "type": "string",
        //             "maxLength": 200
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "controlType",
        //         "timestamp"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyDERAlarm request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDERAlarmRequestParser">A delegate to parse custom NotifyDERAlarm requests.</param>
        public static NotifyDERAlarmRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            RequestTimestamp                    = null,
                                                  TimeSpan?                                            RequestTimeout                      = null,
                                                  EventTracking_Id?                                    EventTrackingId                     = null,
                                                  CustomJObjectParserDelegate<NotifyDERAlarmRequest>?  CustomNotifyDERAlarmRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyDERAlarmRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyDERAlarmRequestParser))
            {
                return notifyDERAlarmRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyDERAlarm request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyDERAlarmRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyDERAlarm request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyDERAlarmRequest">The parsed NotifyDERAlarm request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDERAlarmRequestParser">A delegate to parse custom NotifyDERAlarm requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out NotifyDERAlarmRequest?      NotifyDERAlarmRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            RequestTimestamp                    = null,
                                       TimeSpan?                                            RequestTimeout                      = null,
                                       EventTracking_Id?                                    EventTrackingId                     = null,
                                       CustomJObjectParserDelegate<NotifyDERAlarmRequest>?  CustomNotifyDERAlarmRequestParser   = null)
        {

            try
            {

                NotifyDERAlarmRequest = null;

                #region ControlType           [mandatory]

                if (!JSON.ParseMandatory("controlType",
                                         "control type",
                                         DERControlType.TryParse,
                                         out DERControlType ControlType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp             [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "alarm timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region GridEventFaultType    [optional]

                if (JSON.ParseOptional("gridEventFault",
                                       "grid event fault type",
                                       OCPPv2_1.GridEventFaultType.TryParse,
                                       out GridEventFaultType? GridEventFaultType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AlarmEnded            [optional]

                if (JSON.ParseOptional("alarmEnded",
                                       "alarm ended",
                                       out Boolean? AlarmEnded,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ControlId             [optional]

                var ExtraInfo = JSON["extraInfo"]?.Value<String>();

                #endregion


                #region Signatures            [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
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


                NotifyDERAlarmRequest = new NotifyDERAlarmRequest(

                                            Destination,
                                            ControlType,
                                            Timestamp,
                                            GridEventFaultType,
                                            AlarmEnded,
                                            ExtraInfo,

                                            null,
                                            null,
                                            Signatures,

                                            CustomData,

                                            RequestId,
                                            RequestTimestamp,
                                            RequestTimeout,
                                            EventTrackingId,
                                            NetworkPath

                                        );

                if (CustomNotifyDERAlarmRequestParser is not null)
                    NotifyDERAlarmRequest = CustomNotifyDERAlarmRequestParser(JSON,
                                                                              NotifyDERAlarmRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyDERAlarmRequest  = null;
                ErrorResponse          = "The given JSON representation of a NotifyDERAlarm request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyDERAlarmRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyDERAlarmRequestSerializer">A delegate to serialize custom NotifyDERAlarm requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                  IncludeJSONLDContext                    = false,
                              CustomJObjectSerializerDelegate<NotifyDERAlarmRequest>?  CustomNotifyDERAlarmRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",         DefaultJSONLDContext.    ToString())
                               : null,

                                 new JProperty("controlType",      ControlType.             ToString()),
                                 new JProperty("timestamp",        Timestamp.               ToIso8601()),

                           GridEventFaultType.HasValue
                               ? new JProperty("gridEventFault",   GridEventFaultType.Value.ToString())
                               : null,

                           AlarmEnded.        HasValue
                               ? new JProperty("alarmEnded",       AlarmEnded.        Value)
                               : null,

                           ExtraInfo.IsNotNullOrEmpty()
                               ? new JProperty("extraInfo",        ExtraInfo)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.              ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyDERAlarmRequestSerializer is not null
                       ? CustomNotifyDERAlarmRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyDERAlarmRequest1, NotifyDERAlarmRequest2)

        /// <summary>
        /// Compares two NotifyDERAlarm requests for equality.
        /// </summary>
        /// <param name="NotifyDERAlarmRequest1">A NotifyDERAlarm request.</param>
        /// <param name="NotifyDERAlarmRequest2">Another NotifyDERAlarm request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyDERAlarmRequest? NotifyDERAlarmRequest1,
                                           NotifyDERAlarmRequest? NotifyDERAlarmRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyDERAlarmRequest1, NotifyDERAlarmRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyDERAlarmRequest1 is null || NotifyDERAlarmRequest2 is null)
                return false;

            return NotifyDERAlarmRequest1.Equals(NotifyDERAlarmRequest2);

        }

        #endregion

        #region Operator != (NotifyDERAlarmRequest1, NotifyDERAlarmRequest2)

        /// <summary>
        /// Compares two NotifyDERAlarm requests for inequality.
        /// </summary>
        /// <param name="NotifyDERAlarmRequest1">A NotifyDERAlarm request.</param>
        /// <param name="NotifyDERAlarmRequest2">Another NotifyDERAlarm request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyDERAlarmRequest? NotifyDERAlarmRequest1,
                                           NotifyDERAlarmRequest? NotifyDERAlarmRequest2)

            => !(NotifyDERAlarmRequest1 == NotifyDERAlarmRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyDERAlarmRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyDERAlarm requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyDERAlarm request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyDERAlarmRequest notifyDERAlarmRequest &&
                   Equals(notifyDERAlarmRequest);

        #endregion

        #region Equals(NotifyDERAlarmRequest)

        /// <summary>
        /// Compares two NotifyDERAlarm requests for equality.
        /// </summary>
        /// <param name="NotifyDERAlarmRequest">A NotifyDERAlarm request to compare with.</param>
        public override Boolean Equals(NotifyDERAlarmRequest? NotifyDERAlarmRequest)

            => NotifyDERAlarmRequest is not null &&

               ControlType.Equals(NotifyDERAlarmRequest.ControlType) &&
               Timestamp.  Equals(NotifyDERAlarmRequest.Timestamp)   &&

            ((!GridEventFaultType.HasValue && !NotifyDERAlarmRequest.GridEventFaultType.HasValue) ||
               GridEventFaultType.HasValue &&  NotifyDERAlarmRequest.GridEventFaultType.HasValue && GridEventFaultType.Value.Equals(NotifyDERAlarmRequest.GridEventFaultType.Value)) &&

            ((!AlarmEnded.        HasValue && !NotifyDERAlarmRequest.AlarmEnded.        HasValue) ||
               AlarmEnded.        HasValue &&  NotifyDERAlarmRequest.AlarmEnded.        HasValue && AlarmEnded.        Value.Equals(NotifyDERAlarmRequest.AlarmEnded.        Value)) &&

               String.   Equals(ExtraInfo, NotifyDERAlarmRequest.ExtraInfo) &&

               base.     GenericEquals(NotifyDERAlarmRequest);

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

                   $"'{ControlType}' @ '{Timestamp}' ",

                   AlarmEnded.HasValue
                       ? AlarmEnded.Value
                           ? "ended"
                           : "started"
                       : "started",

                   GridEventFaultType.HasValue
                       ? $", grid fault: '{GridEventFaultType}'"
                       : "",

                   ExtraInfo.IsNotNullOrEmpty()
                       ? $", EV info: '{ExtraInfo}'"
                       : ""

                );

        #endregion

    }

}
