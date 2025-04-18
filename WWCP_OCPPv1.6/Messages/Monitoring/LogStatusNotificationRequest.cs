﻿/*
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
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The LogStatusNotification request.
    /// </summary>
    [SecurityExtensions]
    public class LogStatusNotificationRequest : ARequest<LogStatusNotificationRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/logStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the log upload.
        /// </summary>
        [Mandatory]
        public UploadLogStatus  Status          { get; }

        /// <summary>
        /// The optional request id that was provided in the GetLog request that started this log upload.
        /// </summary>
        [Optional]
        public Int32?           LogRequestId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new LogStatusNotification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Status">The status of the log upload.</param>
        /// <param name="LogRquestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public LogStatusNotificationRequest(SourceRouting            Destination,
                                            UploadLogStatus          Status,
                                            Int32?                   LogRquestId           = null,

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
                   nameof(LogStatusNotificationRequest)[..^7],

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

            this.Status        = Status;
            this.LogRequestId  = LogRquestId;

            unchecked
            {

                hashCode = this.Status.       GetHashCode()       * 5 ^
                          (this.LogRequestId?.GetHashCode() ?? 0) * 3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:LogStatusNotification.req",
        //   "definitions": {
        //     "UploadLogStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "BadMessage",
        //         "Idle",
        //         "NotSupportedOperation",
        //         "PermissionDenied",
        //         "Uploaded",
        //         "UploadFailure",
        //         "Uploading"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/UploadLogStatusEnumType"
        //     },
        //     "requestId": {
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a LogStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomLogStatusNotificationRequestParser">A delegate to parse custom LogStatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static LogStatusNotificationRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<LogStatusNotificationRequest>?  CustomLogStatusNotificationRequestParser   = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var logStatusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomLogStatusNotificationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return logStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a LogStatusNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out LogStatusNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a LogStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="LogStatusNotificationRequest">The parsed LogStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomLogStatusNotificationRequestParser">A delegate to parse custom LogStatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out LogStatusNotificationRequest?      LogStatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<LogStatusNotificationRequest>?  CustomLogStatusNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                       CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            try
            {

                LogStatusNotificationRequest = null;

                #region Status          [mandatory]

                if (!JSON.MapMandatory("status",
                                       "status",
                                       UploadLogStatusExtensions.Parse,
                                       out UploadLogStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LogRequestId    [optional]

                if (!JSON.ParseOptional("requestId",
                                        "request identification",
                                        out Int32? LogRequestId,
                                        out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures      [optional, OCPP_CSE]

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

                #region CustomData      [optional]

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


                LogStatusNotificationRequest = new LogStatusNotificationRequest(

                                                   Destination,
                                                   Status,
                                                   LogRequestId,

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

                if (CustomLogStatusNotificationRequestParser is not null)
                    LogStatusNotificationRequest = CustomLogStatusNotificationRequestParser(JSON,
                                                                                            LogStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                LogStatusNotificationRequest  = null;
                ErrorResponse                 = "The given JSON representation of a LogStatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogStatusNotificationSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogStatusNotificationSerializer">A delegate to serialize custom LogStatusNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogStatusNotificationRequest>?  CustomLogStatusNotificationSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           LogRequestId.HasValue
                               ? new JProperty("requestId",    LogRequestId.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLogStatusNotificationSerializer is not null
                       ? CustomLogStatusNotificationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (LogStatusNotificationRequest1, LogStatusNotificationRequest2)

        /// <summary>
        /// Compares two LogStatusNotification requests for equality.
        /// </summary>
        /// <param name="LogStatusNotificationRequest1">A LogStatusNotification request.</param>
        /// <param name="LogStatusNotificationRequest2">Another LogStatusNotification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LogStatusNotificationRequest? LogStatusNotificationRequest1,
                                           LogStatusNotificationRequest? LogStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LogStatusNotificationRequest1, LogStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (LogStatusNotificationRequest1 is null || LogStatusNotificationRequest2 is null)
                return false;

            return LogStatusNotificationRequest1.Equals(LogStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (LogStatusNotificationRequest1, LogStatusNotificationRequest2)

        /// <summary>
        /// Compares two LogStatusNotification requests for inequality.
        /// </summary>
        /// <param name="LogStatusNotificationRequest1">A LogStatusNotification request.</param>
        /// <param name="LogStatusNotificationRequest2">Another LogStatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogStatusNotificationRequest? LogStatusNotificationRequest1,
                                           LogStatusNotificationRequest? LogStatusNotificationRequest2)

            => !(LogStatusNotificationRequest1 == LogStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<LogStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two LogStatusNotification requests for equality.
        /// </summary>
        /// <param name="Object">A LogStatusNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogStatusNotificationRequest logStatusNotificationRequest &&
                   Equals(logStatusNotificationRequest);

        #endregion

        #region Equals(LogStatusNotificationRequest)

        /// <summary>
        /// Compares two LogStatusNotification requests for equality.
        /// </summary>
        /// <param name="LogStatusNotificationRequest">A LogStatusNotification request to compare with.</param>
        public override Boolean Equals(LogStatusNotificationRequest? LogStatusNotificationRequest)

            => LogStatusNotificationRequest is not null &&

               Status.     Equals(LogStatusNotificationRequest.Status) &&

            ((!LogRequestId.HasValue && !LogStatusNotificationRequest.LogRequestId.HasValue) ||
              (LogRequestId.HasValue &&  LogStatusNotificationRequest.LogRequestId.HasValue && LogRequestId.Value.Equals(LogStatusNotificationRequest.LogRequestId.Value))) &&

               base.GenericEquals(LogStatusNotificationRequest);

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

                   Status,

                   LogRequestId.HasValue
                       ? $" ({LogRequestId})"
                       :  ""

               );

        #endregion

    }

}
