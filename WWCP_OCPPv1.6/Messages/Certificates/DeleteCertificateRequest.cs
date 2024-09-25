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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The DeleteCertificate request.
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
        /// Create a DeleteCertificate request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
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
        public DeleteCertificateRequest(SourceRouting            Destination,
                                        CertificateHashData      CertificateHashData,

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
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.CertificateHashData = CertificateHashData;

            unchecked
            {

                hashCode = this.CertificateHashData.GetHashCode() * 3 ^
                           base.                    GetHashCode();

            }

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

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DeleteCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteCertificateRequestParser">An optional delegate to parse custom DeleteCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static DeleteCertificateRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var deleteCertificateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDeleteCertificateRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a DeleteCertificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DeleteCertificateRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteCertificateRequest">The parsed DeleteCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteCertificateRequestParser">A delegate to parse custom DeleteCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out DeleteCertificateRequest?      DeleteCertificateRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
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

                                               Destination,
                                               CertificateHashData,

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

                if (CustomDeleteCertificateRequestParser is not null)
                    DeleteCertificateRequest = CustomDeleteCertificateRequestParser(JSON,
                                                                                    DeleteCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateRequest  = null;
                ErrorResponse             = "The given JSON representation of a DeleteCertificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateRequestSerializer">A delegate to serialize custom DeleteCertificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?       CustomCertificateHashDataSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
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
        /// Compares two DeleteCertificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A DeleteCertificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another DeleteCertificate request.</param>
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
        /// Compares two DeleteCertificate requests for inequality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A DeleteCertificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another DeleteCertificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateRequest? DeleteCertificateRequest1,
                                           DeleteCertificateRequest? DeleteCertificateRequest2)

            => !(DeleteCertificateRequest1 == DeleteCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteCertificate requests for equality.
        /// </summary>
        /// <param name="Object">A DeleteCertificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateRequest deleteCertificateRequest &&
                   Equals(deleteCertificateRequest);

        #endregion

        #region Equals(DeleteCertificateRequest)

        /// <summary>
        /// Compares two DeleteCertificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest">A DeleteCertificate request to compare with.</param>
        public override Boolean Equals(DeleteCertificateRequest? DeleteCertificateRequest)

            => DeleteCertificateRequest is not null &&

               CertificateHashData.Equals(DeleteCertificateRequest.CertificateHashData) &&

               base.        GenericEquals(DeleteCertificateRequest);

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

            => CertificateHashData.ToString();

        #endregion

    }

}
