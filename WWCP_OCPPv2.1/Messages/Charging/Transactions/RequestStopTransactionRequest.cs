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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The RequestStopTransaction request.
    /// </summary>
    public class RequestStopTransactionRequest : ARequest<RequestStopTransactionRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/requestStopTransactionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the transaction which the charing
        /// station is requested to stop.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RequestStopTransaction request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="TransactionId">The identification of the transaction which the charging station is requested to stop.</param>
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
        public RequestStopTransactionRequest(SourceRouting            Destination,
                                             Transaction_Id           TransactionId,

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
                   nameof(RequestStopTransactionRequest)[..^7],

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

            this.TransactionId = TransactionId;

            unchecked
            {
                hashCode = this.TransactionId.GetHashCode() * 3 ^
                           base.              GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:RequestStopTransactionRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
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
        //         "transactionId": {
        //             "description": "The identifier of the transaction which the Charging Station is requested to stop.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "transactionId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a RequestStopTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRequestStopTransactionRequestParser">A delegate to parse custom RequestStopTransaction requests.</param>
        public static RequestStopTransactionRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    RequestTimestamp                            = null,
                                                          TimeSpan?                                                    RequestTimeout                              = null,
                                                          EventTracking_Id?                                            EventTrackingId                             = null,
                                                          CustomJObjectParserDelegate<RequestStopTransactionRequest>?  CustomRequestStopTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var requestStopTransactionRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomRequestStopTransactionRequestParser))
            {
                return requestStopTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a RequestStopTransaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out RequestStopTransactionRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a RequestStopTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestStopTransactionRequest">The parsed RequestStopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRequestStopTransactionRequestParser">A delegate to parse custom RequestStopTransaction requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out RequestStopTransactionRequest?      RequestStopTransactionRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<RequestStopTransactionRequest>?  CustomRequestStopTransactionRequestParser   = null)
        {

            try
            {

                RequestStopTransactionRequest = null;

                #region TransactionId        [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                RequestStopTransactionRequest = new RequestStopTransactionRequest(

                                                    Destination,
                                                    TransactionId,

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

                if (CustomRequestStopTransactionRequestParser is not null)
                    RequestStopTransactionRequest = CustomRequestStopTransactionRequestParser(JSON,
                                                                                              RequestStopTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                RequestStopTransactionRequest  = null;
                ErrorResponse                  = "The given JSON representation of a RequestStopTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestStopTransactionRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestStopTransactionRequestSerializer">A delegate to serialize custom RequestStopTransaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                          IncludeJSONLDContext                            = false,
                              CustomJObjectSerializerDelegate<RequestStopTransactionRequest>?  CustomRequestStopTransactionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("transactionId",   TransactionId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRequestStopTransactionRequestSerializer is not null
                       ? CustomRequestStopTransactionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RequestStopTransactionRequest1, RequestStopTransactionRequest2)

        /// <summary>
        /// Compares two RequestStopTransaction requests for equality.
        /// </summary>
        /// <param name="RequestStopTransactionRequest1">A RequestStopTransaction request.</param>
        /// <param name="RequestStopTransactionRequest2">Another RequestStopTransaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestStopTransactionRequest? RequestStopTransactionRequest1,
                                           RequestStopTransactionRequest? RequestStopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestStopTransactionRequest1, RequestStopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RequestStopTransactionRequest1 is null || RequestStopTransactionRequest2 is null)
                return false;

            return RequestStopTransactionRequest1.Equals(RequestStopTransactionRequest2);

        }

        #endregion

        #region Operator != (RequestStopTransactionRequest1, RequestStopTransactionRequest2)

        /// <summary>
        /// Compares two RequestStopTransaction requests for inequality.
        /// </summary>
        /// <param name="RequestStopTransactionRequest1">A RequestStopTransaction request.</param>
        /// <param name="RequestStopTransactionRequest2">Another RequestStopTransaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestStopTransactionRequest? RequestStopTransactionRequest1,
                                           RequestStopTransactionRequest? RequestStopTransactionRequest2)

            => !(RequestStopTransactionRequest1 == RequestStopTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RequestStopTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RequestStopTransaction requests for equality.
        /// </summary>
        /// <param name="Object">A RequestStopTransaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStopTransactionRequest requestStopTransactionRequest &&
                   Equals(requestStopTransactionRequest);

        #endregion

        #region Equals(RequestStopTransactionRequest)

        /// <summary>
        /// Compares two RequestStopTransaction requests for equality.
        /// </summary>
        /// <param name="RequestStopTransactionRequest">A RequestStopTransaction request to compare with.</param>
        public override Boolean Equals(RequestStopTransactionRequest? RequestStopTransactionRequest)

            => RequestStopTransactionRequest is not null &&

               TransactionId.Equals(RequestStopTransactionRequest.TransactionId) &&

               base.  GenericEquals(RequestStopTransactionRequest);

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

            => TransactionId.ToString();

        #endregion

    }

}
