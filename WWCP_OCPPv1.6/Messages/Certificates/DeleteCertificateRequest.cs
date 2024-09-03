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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The delete certificate request.
    /// </summary>
    [SecurityExtensions]
    public class DeleteCertificateRequest : ARequest<DeleteCertificateRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/deleteCertificateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Indicates the certificate which should be deleted.
        /// </summary>
        public CertificateHashData  CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a delete certificate request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
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
        public DeleteCertificateRequest(NetworkingNode_Id             NetworkingNodeId,
                                        CertificateHashData           CertificateHashData,

                                        IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                        IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                        IEnumerable<Signature>?  Signatures          = null,

                                        CustomData?                   CustomData          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        NetworkPath?                  NetworkPath         = null,
                                        CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(DeleteCertificateRequest)[..^7],

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

            this.CertificateHashData = CertificateHashData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:DeleteCertificate.req",
        //   "definitions": {
        //     "HashAlgorithmEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "CertificateHashDataType": {
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "type": "string",
        //           "maxLength": 40
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "certificateHashData": {
        //         "$ref": "#/definitions/CertificateHashDataType"
        //     }
        // },
        //   "required": [
        //     "certificateHashData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomDeleteCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomDeleteCertificateRequestParser">An optional delegate to parse custom delete certificate requests.</param>
        public static DeleteCertificateRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     NetworkingNode_Id                                       NetworkingNodeId,
                                                     NetworkPath                                             NetworkPath,
                                                     CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var deleteCertificateRequest,
                         out var errorResponse,
                         CustomDeleteCertificateRequestParser) &&
                deleteCertificateRequest is not null)
            {
                return deleteCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a delete certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out DeleteCertificateRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteCertificateRequest">The parsed DeleteCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       NetworkingNode_Id              NetworkingNodeId,
                                       NetworkPath                    NetworkPath,
                                       out DeleteCertificateRequest?  DeleteCertificateRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out DeleteCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteCertificateRequest">The parsed delete certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteCertificateRequestParser">An optional delegate to parse custom delete certificate requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       NetworkingNode_Id                                       NetworkingNodeId,
                                       NetworkPath                                             NetworkPath,
                                       out DeleteCertificateRequest?                           DeleteCertificateRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser)
        {

            try
            {

                DeleteCertificateRequest = null;

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv1_6.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (CertificateHashData is null)
                    return false;

                #endregion

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                DeleteCertificateRequest = new DeleteCertificateRequest(

                                               NetworkingNodeId,
                                               CertificateHashData,

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

                if (CustomDeleteCertificateRequestParser is not null)
                    DeleteCertificateRequest = CustomDeleteCertificateRequestParser(JSON,
                                                                                    DeleteCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateRequest  = null;
                ErrorResponse             = "The given JSON representation of a delete certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateRequestSerializer">A delegate to serialize custom delete certificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?       CustomCertificateHashDataSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON(CustomCertificateHashDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteCertificateRequestSerializer is not null
                       ? CustomDeleteCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateRequest1, DeleteCertificateRequest2)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A delete certificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another delete certificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteCertificateRequest? DeleteCertificateRequest1,
                                           DeleteCertificateRequest? DeleteCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteCertificateRequest1, DeleteCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteCertificateRequest1 is null || DeleteCertificateRequest2 is null)
                return false;

            return DeleteCertificateRequest1.Equals(DeleteCertificateRequest2);

        }

        #endregion

        #region Operator != (DeleteCertificateRequest1, DeleteCertificateRequest2)

        /// <summary>
        /// Compares two delete certificate requests for inequality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A delete certificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another delete certificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateRequest? DeleteCertificateRequest1,
                                           DeleteCertificateRequest? DeleteCertificateRequest2)

            => !(DeleteCertificateRequest1 == DeleteCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="Object">A delete certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateRequest deleteCertificateRequest &&
                   Equals(deleteCertificateRequest);

        #endregion

        #region Equals(DeleteCertificateRequest)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest">A delete certificate request to compare with.</param>
        public override Boolean Equals(DeleteCertificateRequest? DeleteCertificateRequest)

            => DeleteCertificateRequest is not null &&

               CertificateHashData.Equals(DeleteCertificateRequest.CertificateHashData) &&

               base.        GenericEquals(DeleteCertificateRequest);

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

                return CertificateHashData.GetHashCode() * 3 ^
                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CertificateHashData.ToString();

        #endregion

    }

}
