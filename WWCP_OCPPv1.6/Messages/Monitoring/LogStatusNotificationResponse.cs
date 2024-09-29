/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A LogStatusNotification response.
    /// </summary>
    [SecurityExtensions]
    public class LogStatusNotificationResponse : AResponse<LogStatusNotificationRequest,
                                                           LogStatusNotificationResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/logStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new LogStatusNotification response.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public LogStatusNotificationResponse(LogStatusNotificationRequest  Request,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        { }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:LogStatusNotification.conf",
        //   "type": "object",
        //   "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a LogStatusNotification response.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">An optional delegate to parse custom LogStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static LogStatusNotificationResponse Parse(LogStatusNotificationRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var logStatusNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomLogStatusNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return logStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a LogStatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out LogStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a LogStatusNotification response.
        /// </summary
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="LogStatusNotificationResponse">The parsed LogStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">An optional delegate to parse custom LogStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(LogStatusNotificationRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out LogStatusNotificationResponse?      LogStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                LogStatusNotificationResponse  = null;

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                LogStatusNotificationResponse = new LogStatusNotificationResponse(

                                                    Request,

                                                    null,
                                                    ResponseTimestamp,

                                                    Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomLogStatusNotificationResponseParser is not null)
                    LogStatusNotificationResponse = CustomLogStatusNotificationResponseParser(JSON,
                                                                                              LogStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                LogStatusNotificationResponse  = null;
                ErrorResponse                  = "The given JSON representation of a LogStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogStatusNotificationResponseSerializer">A delegate to serialize custom LogStatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLogStatusNotificationResponseSerializer is not null
                       ? CustomLogStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The LogStatusNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        public static LogStatusNotificationResponse RequestError(LogStatusNotificationRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The LogStatusNotification failed.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static LogStatusNotificationResponse FormationViolation(LogStatusNotificationRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The LogStatusNotification failed.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static LogStatusNotificationResponse SignatureError(LogStatusNotificationRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The LogStatusNotification failed.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static LogStatusNotificationResponse Failed(LogStatusNotificationRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The LogStatusNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static LogStatusNotificationResponse ExceptionOccured(LogStatusNotificationRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A LogStatusNotification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another LogStatusNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LogStatusNotificationResponse1, LogStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (LogStatusNotificationResponse1 is null || LogStatusNotificationResponse2 is null)
                return false;

            return LogStatusNotificationResponse1.Equals(LogStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two LogStatusNotification responses for inequality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A LogStatusNotification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another LogStatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)

            => !(LogStatusNotificationResponse1 == LogStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<LogStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="Object">A LogStatusNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogStatusNotificationResponse logStatusNotificationResponse &&
                   Equals(logStatusNotificationResponse);

        #endregion

        #region Equals(LogStatusNotificationResponse)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse">A LogStatusNotification response to compare with.</param>
        public override Boolean Equals(LogStatusNotificationResponse? LogStatusNotificationResponse)

            => LogStatusNotificationResponse is not null;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "LogStatusNotification response";

        #endregion

    }

}
