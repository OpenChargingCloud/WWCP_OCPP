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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The get transaction status request.
    /// </summary>
    public class GetTransactionStatusRequest : ARequest<GetTransactionStatusRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getTransactionStatusRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional transaction identification for which its status is requested.
        /// </summary>
        [Optional]
        public Transaction_Id?  TransactionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get transaction status request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="TransactionId">An optional transaction identification for which its status is requested.</param>
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
        public GetTransactionStatusRequest(NetworkingNode_Id        NetworkingNodeId,
                                           Transaction_Id?          TransactionId       = null,

                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                                           CustomData?              CustomData          = null,

                                           Request_Id?              RequestId           = null,
                                           DateTime?                RequestTimestamp    = null,
                                           TimeSpan?                RequestTimeout      = null,
                                           EventTracking_Id?        EventTrackingId     = null,
                                           NetworkPath?             NetworkPath         = null,
                                           CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(GetTransactionStatusRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.TransactionId = TransactionId;

            unchecked
            {
                hashCode = this.TransactionId.GetHashCode() * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetTransactionStatusRequest",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "transactionId": {
        //       "description": "The Id of the transaction for which the status is requested.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomGetTransactionStatusRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get transaction status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetTransactionStatusRequestParser">A delegate to parse custom get transaction status requests.</param>
        public static GetTransactionStatusRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        NetworkingNode_Id                                          NetworkingNodeId,
                                                        NetworkPath                                                NetworkPath,
                                                        CustomJObjectParserDelegate<GetTransactionStatusRequest>?  CustomGetTransactionStatusRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var getTransactionStatusRequest,
                         out var errorResponse,
                         CustomGetTransactionStatusRequestParser) &&
                getTransactionStatusRequest is not null)
            {
                return getTransactionStatusRequest;
            }

            throw new ArgumentException("The given JSON representation of a get transaction status request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out GetTransactionStatusRequest, out ErrorResponse, CustomGetTransactionStatusRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get transaction status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetTransactionStatusRequest">The parsed get transaction status request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       NetworkingNode_Id                 NetworkingNodeId,
                                       NetworkPath                       NetworkPath,
                                       out GetTransactionStatusRequest?  GetTransactionStatusRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out GetTransactionStatusRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get transaction status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetTransactionStatusRequest">The parsed get transaction status request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetTransactionStatusRequestParser">A delegate to parse custom get transaction status requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       NetworkingNode_Id                                          NetworkingNodeId,
                                       NetworkPath                                                NetworkPath,
                                       out GetTransactionStatusRequest?                           GetTransactionStatusRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetTransactionStatusRequest>?  CustomGetTransactionStatusRequestParser)
        {

            try
            {

                GetTransactionStatusRequest = null;

                #region TransactionId        [optional]

                if (JSON.ParseOptional("customData",
                                       "custom data",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetTransactionStatusRequest = new GetTransactionStatusRequest(

                                                  NetworkingNodeId,
                                                  TransactionId,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData,

                                                  RequestId,
                                                  null,
                                                  null,
                                                  null,
                                                  NetworkPath

                                              );

                if (CustomGetTransactionStatusRequestParser is not null)
                    GetTransactionStatusRequest = CustomGetTransactionStatusRequestParser(JSON,
                                                                                          GetTransactionStatusRequest);

                return true;

            }
            catch (Exception e)
            {
                GetTransactionStatusRequest  = null;
                ErrorResponse                = "The given JSON representation of a get transaction status request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetTransactionStatusRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetTransactionStatusRequestSerializer">A delegate to serialize custom get transaction status requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?  CustomGetTransactionStatusRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           TransactionId.HasValue
                               ? new JProperty("transactionId",   TransactionId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetTransactionStatusRequestSerializer is not null
                       ? CustomGetTransactionStatusRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetTransactionStatusRequest1, GetTransactionStatusRequest2)

        /// <summary>
        /// Compares two get transaction status requests for equality.
        /// </summary>
        /// <param name="GetTransactionStatusRequest1">A get transaction status request.</param>
        /// <param name="GetTransactionStatusRequest2">Another get transaction status request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetTransactionStatusRequest? GetTransactionStatusRequest1,
                                           GetTransactionStatusRequest? GetTransactionStatusRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetTransactionStatusRequest1, GetTransactionStatusRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetTransactionStatusRequest1 is null || GetTransactionStatusRequest2 is null)
                return false;

            return GetTransactionStatusRequest1.Equals(GetTransactionStatusRequest2);

        }

        #endregion

        #region Operator != (GetTransactionStatusRequest1, GetTransactionStatusRequest2)

        /// <summary>
        /// Compares two get transaction status requests for inequality.
        /// </summary>
        /// <param name="GetTransactionStatusRequest1">A get transaction status request.</param>
        /// <param name="GetTransactionStatusRequest2">Another get transaction status request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetTransactionStatusRequest? GetTransactionStatusRequest1,
                                           GetTransactionStatusRequest? GetTransactionStatusRequest2)

            => !(GetTransactionStatusRequest1 == GetTransactionStatusRequest2);

        #endregion

        #endregion

        #region IEquatable<GetTransactionStatusRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get transaction status requests for equality.
        /// </summary>
        /// <param name="Object">A get transaction status request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetTransactionStatusRequest getTransactionStatusRequest &&
                   Equals(getTransactionStatusRequest);

        #endregion

        #region Equals(GetTransactionStatusRequest)

        /// <summary>
        /// Compares two get transaction status requests for equality.
        /// </summary>
        /// <param name="GetTransactionStatusRequest">A get transaction status request to compare with.</param>
        public override Boolean Equals(GetTransactionStatusRequest? GetTransactionStatusRequest)

            => GetTransactionStatusRequest is not null &&

             ((!TransactionId.HasValue && !GetTransactionStatusRequest.TransactionId.HasValue) ||
                TransactionId.HasValue &&  GetTransactionStatusRequest.TransactionId.HasValue && TransactionId.Value.Equals(GetTransactionStatusRequest.TransactionId.Value)) &&

               base.GenericEquals(GetTransactionStatusRequest);

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

            => TransactionId.HasValue
                   ? TransactionId.Value.ToString()
                   : "GetTransactionStatusRequest";

        #endregion

    }

}
