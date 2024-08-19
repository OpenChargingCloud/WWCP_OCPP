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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetLog request.
    /// </summary>
    public class GetLogRequest : ARequest<GetLogRequest>,
                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getLogRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of the certificates requested.
        /// </summary>
        [Mandatory]
        public LogType        LogType          { get; }

        /// <summary>
        /// The unique identification of this request.
        /// </summary>
        [Mandatory]
        public Int32          LogRequestId     { get; }

        /// <summary>
        /// This field specifies the requested log and the location to which the log should be sent.
        /// </summary>
        [Mandatory]
        public LogParameters  Log              { get; }

        /// <summary>
        /// This specifies how many times the Charge Point must try to upload the log before giving up.
        /// If this field is not present, it is left to Charge Point to decide how many times it wants to retry.
        /// </summary>
        [Optional]
        public Byte?          Retries          { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field is not present,
        /// it is left to Charge Point to decide how long to wait between attempts.
        /// </summary>
        [Optional]
        public TimeSpan?      RetryInterval    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetLog request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="LogType">The type of the certificates requested.</param>
        /// <param name="LogRequestId">The unique identification of this request.</param>
        /// <param name="Log">This field specifies the requested log and the location to which the log should be sent.</param>
        /// <param name="Retries">This specifies how many times the Charge Point must try to upload the log before giving up. If this field is not present, it is left to Charge Point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to Charge Point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetLogRequest(SourceRouting            Destination,
                             LogType                  LogType,
                             Int32                    LogRequestId,
                             LogParameters            Log,
                             Byte?                    Retries               = null,
                             TimeSpan?                RetryInterval         = null,

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
                   nameof(GetLogRequest)[..^7],

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

            this.LogType        = LogType;
            this.LogRequestId   = LogRequestId;
            this.Log            = Log;
            this.Retries        = Retries;
            this.RetryInterval  = RetryInterval;


            unchecked
            {

                hashCode = this.LogType.       GetHashCode()       * 13 ^
                           this.LogRequestId.  GetHashCode()       * 11 ^
                           this.Log.           GetHashCode()       *  7 ^
                          (this.Retries?.      GetHashCode() ?? 0) *  5 ^
                          (this.RetryInterval?.GetHashCode() ?? 0) *  3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetLogRequest",
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
        //     "LogEnumType": {
        //       "description": "This contains the type of log file that the Charging Station\r\nshould send.",
        //       "javaType": "LogEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "DiagnosticsLog",
        //         "SecurityLog"
        //       ]
        //     },
        //     "LogParametersType": {
        //       "description": "Log\r\nurn:x-enexis:ecdm:uid:2:233373\r\nGeneric class for the configuration of logging entries.",
        //       "javaType": "LogParameters",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "remoteLocation": {
        //           "description": "Log. Remote_ Location. URI\r\nurn:x-enexis:ecdm:uid:1:569484\r\nThe URL of the location at the remote system where the log should be stored.",
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "oldestTimestamp": {
        //           "description": "Log. Oldest_ Timestamp. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569477\r\nThis contains the date and time of the oldest logging information to include in the diagnostics.",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "latestTimestamp": {
        //           "description": "Log. Latest_ Timestamp. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569482\r\nThis contains the date and time of the latest logging information to include in the diagnostics.",
        //           "type": "string",
        //           "format": "date-time"
        //         }
        //       },
        //       "required": [
        //         "remoteLocation"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "log": {
        //       "$ref": "#/definitions/LogParametersType"
        //     },
        //     "logType": {
        //       "$ref": "#/definitions/LogEnumType"
        //     },
        //     "requestId": {
        //       "description": "The Id of this request\r\n",
        //       "type": "integer"
        //     },
        //     "retries": {
        //       "description": "This specifies how many times the Charging Station must try to upload the log before giving up. If this field is not present, it is left to Charging Station to decide how many times it wants to retry.",
        //       "type": "integer"
        //     },
        //     "retryInterval": {
        //       "description": "The interval in seconds after which a retry may be attempted. If this field is not present, it is left to Charging Station to decide how long to wait between attempts.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "logType",
        //     "requestId",
        //     "log"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetLogRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetLog request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetLogRequestParser">A delegate to parse custom GetLog requests.</param>
        public static GetLogRequest Parse(JObject                                      JSON,
                                          Request_Id                                   RequestId,
                                          SourceRouting                            Destination,
                                          NetworkPath                                  NetworkPath,
                                          DateTime?                                    RequestTimestamp            = null,
                                          TimeSpan?                                    RequestTimeout              = null,
                                          EventTracking_Id?                            EventTrackingId             = null,
                                          CustomJObjectParserDelegate<GetLogRequest>?  CustomGetLogRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getLogRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetLogRequestParser))
            {
                return getLogRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetLog request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetLogRequest, out ErrorResponse, CustomGetLogRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetLog request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetLogRequest">The parsed GetLog request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetLogRequestParser">A delegate to parse custom GetLog requests.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       Request_Id                                   RequestId,
                                       SourceRouting                            Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out GetLogRequest?      GetLogRequest,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       DateTime?                                    RequestTimestamp            = null,
                                       TimeSpan?                                    RequestTimeout              = null,
                                       EventTracking_Id?                            EventTrackingId             = null,
                                       CustomJObjectParserDelegate<GetLogRequest>?  CustomGetLogRequestParser   = null)
        {

            try
            {

                GetLogRequest = null;

                #region LogType              [mandatory]

                if (!JSON.ParseMandatory("logType",
                                         "log type",
                                         OCPPv2_1.LogType.TryParse,
                                         out LogType LogType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LogRequestId         [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "log request identification",
                                         out Int32 LogRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Log                  [mandatory]

                if (!JSON.ParseMandatoryJSON("log",
                                             "log parameters",
                                             LogParameters.TryParse,
                                             out LogParameters? Log,
                                             out ErrorResponse) ||
                     Log is null)
                {
                    return false;
                }

                #endregion

                #region Retries              [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval        [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                GetLogRequest = new GetLogRequest(

                                    Destination,
                                    LogType,
                                    LogRequestId,
                                    Log,
                                    Retries,
                                    RetryInterval,

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

                if (CustomGetLogRequestParser is not null)
                    GetLogRequest = CustomGetLogRequestParser(JSON,
                                                              GetLogRequest);

                return true;

            }
            catch (Exception e)
            {
                GetLogRequest  = null;
                ErrorResponse  = "The given JSON representation of a GetLog request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLogRequestSerializer = null, CustomLogParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLogRequestSerializer">A delegate to serialize custom GetLog requests.</param>
        /// <param name="CustomLogParametersSerializer">A delegate to serialize custom log parameters.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLogRequest>?  CustomGetLogRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<LogParameters>?  CustomLogParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?      CustomSignatureSerializer       = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("logType",         LogType.   ToString()),
                                 new JProperty("requestId",       LogRequestId),
                                 new JProperty("log",             Log.       ToJSON(CustomLogParametersSerializer)),

                           Retries.HasValue
                               ? new JProperty("retries",         Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",   (Int32) Math.Round(RetryInterval.Value.TotalSeconds, 0))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLogRequestSerializer is not null
                       ? CustomGetLogRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetLogRequest1, GetLogRequest2)

        /// <summary>
        /// Compares two GetLog requests for equality.
        /// </summary>
        /// <param name="GetLogRequest1">A GetLog request.</param>
        /// <param name="GetLogRequest2">Another GetLog request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLogRequest? GetLogRequest1,
                                           GetLogRequest? GetLogRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLogRequest1, GetLogRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetLogRequest1 is null || GetLogRequest2 is null)
                return false;

            return GetLogRequest1.Equals(GetLogRequest2);

        }

        #endregion

        #region Operator != (GetLogRequest1, GetLogRequest2)

        /// <summary>
        /// Compares two GetLog requests for inequality.
        /// </summary>
        /// <param name="GetLogRequest1">A GetLog request.</param>
        /// <param name="GetLogRequest2">Another GetLog request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLogRequest? GetLogRequest1,
                                           GetLogRequest? GetLogRequest2)

            => !(GetLogRequest1 == GetLogRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLogRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetLog requests for equality.
        /// </summary>
        /// <param name="Object">A GetLog request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLogRequest getLogRequest &&
                   Equals(getLogRequest);

        #endregion

        #region Equals(GetLogRequest)

        /// <summary>
        /// Compares two GetLog requests for equality.
        /// </summary>
        /// <param name="GetLogRequest">A GetLog request to compare with.</param>
        public override Boolean Equals(GetLogRequest? GetLogRequest)

            => GetLogRequest is not null &&

               LogType.     Equals(GetLogRequest.LogType)      &&
               LogRequestId.Equals(GetLogRequest.LogRequestId) &&
               Log.         Equals(GetLogRequest.Log)          &&

            ((!Retries.      HasValue && !GetLogRequest.Retries.      HasValue) ||
              (Retries.      HasValue &&  GetLogRequest.Retries.      HasValue && Retries.      Value.Equals(GetLogRequest.Retries.Value)))       &&

             ((RetryInterval.HasValue && !GetLogRequest.RetryInterval.HasValue) ||
              (RetryInterval.HasValue &&  GetLogRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(GetLogRequest.RetryInterval.Value))) &&

               base. GenericEquals(GetLogRequest);

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

            => $"{LogType} ({LogRequestId})";

        #endregion

    }

}
