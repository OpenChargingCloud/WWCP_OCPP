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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The DeleteCertificate request.
    /// </summary>
    public class DeleteCertificateRequest : ARequest<DeleteCertificateRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/deleteCertificateRequest");

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
        [Mandatory]
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:DeleteCertificateRequest",
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
        //     "HashAlgorithmEnumType": {
        //       "description": "Used algorithms for the hashes provided.",
        //       "javaType": "HashAlgorithmEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "CertificateHashDataType": {
        //       "javaType": "CertificateHashData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //           "description": "Hashed value of the Issuer DN (Distinguished Name).",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "description": "Hashed value of the issuers public key",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "description": "The serial number of the certificate.",
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
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "certificateHashData": {
        //       "$ref": "#/definitions/CertificateHashDataType"
        //     }
        //   },
        //   "required": [
        //     "certificateHashData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomDeleteCertificateRequestParser = null)

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
        /// <param name="CustomDeleteCertificateRequestParser">A delegate to parse custom DeleteCertificate requests.</param>
        public static DeleteCertificateRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null)
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
                         CustomDeleteCertificateRequestParser))
            {
                return deleteCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a DeleteCertificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DeleteCertificateRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

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
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out DeleteCertificateRequest?      DeleteCertificateRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null)
        {

            try
            {

                DeleteCertificateRequest = null;

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv2_1.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse) ||
                    CertificateHashData is null)
                {
                    return false;
                }

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

        #region ToJSON(CustomDeleteCertificateRequestSerializer = null, CustomCertificateHashDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateRequestSerializer">A delegate to serialize custom DeleteCertificate requests.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?       CustomCertificateHashDataSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",              DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("certificateHashData",   CertificateHashData. ToJSON(CustomCertificateHashDataSerializer,
                                                                                                    CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.          ToJSON(CustomCustomDataSerializer))
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
