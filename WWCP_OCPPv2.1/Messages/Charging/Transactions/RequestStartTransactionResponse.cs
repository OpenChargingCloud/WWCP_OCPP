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
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The RequestStartTransaction response.
    /// </summary>
    public class RequestStartTransactionResponse : AResponse<RequestStartTransactionRequest,
                                                             RequestStartTransactionResponse>,
                                                   IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/requestStartTransactionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status indicating whether the charging station accepts the request
        /// to start a charging transaction.
        /// </summary>
        [Mandatory]
        public RequestStartStopStatus  Status           { get; }

        /// <summary>
        /// The optional transaction identification of an already started transaction, when
        /// the transaction was already started by the charging station before the
        /// RequestStartTransactionRequest was received.
        /// For example when the cable was plugged in first.
        /// </summary>
        [Optional]
        public Transaction_Id?         TransactionId    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?             StatusInfo       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RequestStartTransaction response.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charging station accepts the request to start a charging transaction.</param>
        /// <param name="TransactionId">An optional transaction identification of an already started transaction, when the transaction was already started by the charging station before the RequestStartTransactionRequest was received. For example when the cable was plugged in first.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        public RequestStartTransactionResponse(RequestStartTransactionRequest  Request,
                                               RequestStartStopStatus          Status,
                                               Transaction_Id?                 TransactionId         = null,
                                               StatusInfo?                     StatusInfo            = null,

                                               Result?                         Result                = null,
                                               DateTime?                       ResponseTimestamp     = null,

                                               SourceRouting?                  Destination           = null,
                                               NetworkPath?                    NetworkPath           = null,

                                               IEnumerable<KeyPair>?           SignKeys              = null,
                                               IEnumerable<SignInfo>?          SignInfos             = null,
                                               IEnumerable<Signature>?         Signatures            = null,

                                               CustomData?                     CustomData            = null,

                                               SerializationFormats?           SerializationFormat   = null,
                                               CancellationToken               CancellationToken     = default)

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

        {

            this.Status         = Status;
            this.TransactionId  = TransactionId;
            this.StatusInfo     = StatusInfo;

            unchecked
            {

                hashCode = this.Status.        GetHashCode()       * 7 ^
                          (this.TransactionId?.GetHashCode() ?? 0) * 5 ^
                          (this.StatusInfo?.   GetHashCode() ?? 0) * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:RequestStartTransactionResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "RequestStartStopStatusEnumType": {
        //             "description": "Status indicating whether the Charging Station accepts the request to start a transaction.",
        //             "javaType": "RequestStartStopStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
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
        //         "status": {
        //             "$ref": "#/definitions/RequestStartStopStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "transactionId": {
        //             "description": "When the transaction was already started by the Charging Station before the RequestStartTransactionRequest was received, for example: cable plugged in first. This contains the transactionId of the already started transaction.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a RequestStartTransaction response.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRequestStartTransactionResponseParser">A delegate to parse custom RequestStartTransaction responses.</param>
        public static RequestStartTransactionResponse Parse(RequestStartTransactionRequest                                 Request,
                                                            JObject                                                        JSON,
                                                            SourceRouting                                                  Destination,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTime?                                                      ResponseTimestamp                             = null,
                                                            CustomJObjectParserDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseParser   = null,
                                                            CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                                            CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                                            CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var requestStartTransactionResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomRequestStartTransactionResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return requestStartTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a RequestStartTransaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out RequestStartTransactionResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a RequestStartTransaction response.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestStartTransactionResponse">The parsed RequestStartTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRequestStartTransactionResponseParser">A delegate to parse custom RequestStartTransaction responses.</param>
        public static Boolean TryParse(RequestStartTransactionRequest                                 Request,
                                       JObject                                                        JSON,
                                       SourceRouting                                                  Destination,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out RequestStartTransactionResponse?      RequestStartTransactionResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTime?                                                      ResponseTimestamp                             = null,
                                       CustomJObjectParserDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                       CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                       CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            try
            {

                RequestStartTransactionResponse = null;

                #region Status           [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "request start stop status",
                                         RequestStartStopStatus.TryParse,
                                         out RequestStartStopStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId    [optional]

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

                #region StatusInfo       [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                RequestStartTransactionResponse = new RequestStartTransactionResponse(

                                                      Request,
                                                      Status,
                                                      TransactionId,
                                                      StatusInfo,

                                                      null,
                                                      ResponseTimestamp,

                                                      Destination,
                                                      NetworkPath,

                                                      null,
                                                      null,
                                                      Signatures,

                                                      CustomData

                                                  );

                if (CustomRequestStartTransactionResponseParser is not null)
                    RequestStartTransactionResponse = CustomRequestStartTransactionResponseParser(JSON,
                                                                                                  RequestStartTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                RequestStartTransactionResponse  = null;
                ErrorResponse                    = "The given JSON representation of a RequestStartTransaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestStartTransactionResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestStartTransactionResponseSerializer">A delegate to serialize custom RequestStartTransaction responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                              = false,
                              CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",          Status.              ToString()),

                           TransactionId.HasValue
                               ? new JProperty("transactionId",   TransactionId.Value.Value)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",      StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                               CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRequestStartTransactionResponseSerializer is not null
                       ? CustomRequestStartTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The RequestStartTransaction failed because of a request error.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request.</param>
        public static RequestStartTransactionResponse RequestError(RequestStartTransactionRequest  Request,
                                                                   EventTracking_Id                EventTrackingId,
                                                                   ResultCode                      ErrorCode,
                                                                   String?                         ErrorDescription    = null,
                                                                   JObject?                        ErrorDetails        = null,
                                                                   DateTime?                       ResponseTimestamp   = null,

                                                                   SourceRouting?                  Destination         = null,
                                                                   NetworkPath?                    NetworkPath         = null,

                                                                   IEnumerable<KeyPair>?           SignKeys            = null,
                                                                   IEnumerable<SignInfo>?          SignInfos           = null,
                                                                   IEnumerable<Signature>?         Signatures          = null,

                                                                   CustomData?                     CustomData          = null)

            => new (

                   Request,
                   RequestStartStopStatus.Rejected,
                   null,
                   null,
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
        /// The RequestStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RequestStartTransactionResponse FormationViolation(RequestStartTransactionRequest  Request,
                                                                         String                               ErrorDescription)

            => new (Request,
                    RequestStartStopStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The RequestStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RequestStartTransactionResponse SignatureError(RequestStartTransactionRequest  Request,
                                                                     String                               ErrorDescription)

            => new (Request,
                    RequestStartStopStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The RequestStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request.</param>
        /// <param name="Description">An optional error description.</param>
        public static RequestStartTransactionResponse Failed(RequestStartTransactionRequest  Request,
                                                             String?                              Description   = null)

            => new (Request,
                    RequestStartStopStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The RequestStartTransaction failed because of an exception.
        /// </summary>
        /// <param name="Request">The RequestStartTransaction request.</param>
        /// <param name="Exception">The exception.</param>
        public static RequestStartTransactionResponse ExceptionOccurred(RequestStartTransactionRequest  Request,
                                                                       Exception                            Exception)

            => new (Request,
                    RequestStartStopStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (RequestStartTransactionResponse1, RequestStartTransactionResponse2)

        /// <summary>
        /// Compares two RequestStartTransaction responses for equality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse1">A RequestStartTransaction response.</param>
        /// <param name="RequestStartTransactionResponse2">Another RequestStartTransaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestStartTransactionResponse? RequestStartTransactionResponse1,
                                           RequestStartTransactionResponse? RequestStartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestStartTransactionResponse1, RequestStartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RequestStartTransactionResponse1 is null || RequestStartTransactionResponse2 is null)
                return false;

            return RequestStartTransactionResponse1.Equals(RequestStartTransactionResponse2);

        }

        #endregion

        #region Operator != (RequestStartTransactionResponse1, RequestStartTransactionResponse2)

        /// <summary>
        /// Compares two RequestStartTransaction responses for inequality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse1">A RequestStartTransaction response.</param>
        /// <param name="RequestStartTransactionResponse2">Another RequestStartTransaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestStartTransactionResponse? RequestStartTransactionResponse1,
                                           RequestStartTransactionResponse? RequestStartTransactionResponse2)

            => !(RequestStartTransactionResponse1 == RequestStartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RequestStartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RequestStartTransaction responses for equality.
        /// </summary>
        /// <param name="Object">A RequestStartTransaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStartTransactionResponse requestStartTransactionResponse &&
                   Equals(requestStartTransactionResponse);

        #endregion

        #region Equals(RequestStartTransactionResponse)

        /// <summary>
        /// Compares two RequestStartTransaction responses for equality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse">A RequestStartTransaction response to compare with.</param>
        public override Boolean Equals(RequestStartTransactionResponse? RequestStartTransactionResponse)

            => RequestStartTransactionResponse is not null &&

               Status.Equals(RequestStartTransactionResponse.Status) &&

             ((!TransactionId.HasValue    && !RequestStartTransactionResponse.TransactionId.HasValue) ||
                TransactionId.HasValue    &&  RequestStartTransactionResponse.TransactionId.HasValue    && TransactionId.Value.Equals(RequestStartTransactionResponse.TransactionId.Value)) &&

             ((StatusInfo     is     null &&  RequestStartTransactionResponse.StatusInfo    is null)  ||
               StatusInfo     is not null &&  RequestStartTransactionResponse.StatusInfo    is not null && StatusInfo.         Equals(RequestStartTransactionResponse.StatusInfo))          &&

               base.GenericEquals(RequestStartTransactionResponse);

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

            => Status.ToString();

        #endregion

    }

}
