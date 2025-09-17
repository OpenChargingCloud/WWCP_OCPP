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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetCertificateChainStatus request.
    /// </summary>
    public class GetCertificateChainStatusRequest : ARequest<GetCertificateChainStatusRequest>,
                                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getCertificateChainStatusRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of certificate status requests.
        /// </summary>
        [Mandatory]
        public IEnumerable<CertificateStatusRequestInfo>  CertificateStatusRequests    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetCertificateChainStatus request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="CertificateStatusRequests">An enumeration of certificate status requests.</param>
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
        public GetCertificateChainStatusRequest(SourceRouting                              Destination,
                                                IEnumerable<CertificateStatusRequestInfo>  CertificateStatusRequests,

                                                IEnumerable<KeyPair>?                      SignKeys              = null,
                                                IEnumerable<SignInfo>?                     SignInfos             = null,
                                                IEnumerable<Signature>?                    Signatures            = null,

                                                CustomData?                                CustomData            = null,

                                                Request_Id?                                RequestId             = null,
                                                DateTimeOffset?                            RequestTimestamp      = null,
                                                TimeSpan?                                  RequestTimeout        = null,
                                                EventTracking_Id?                          EventTrackingId       = null,
                                                NetworkPath?                               NetworkPath           = null,
                                                SerializationFormats?                      SerializationFormat   = null,
                                                CancellationToken                          CancellationToken     = default)

            : base(Destination,
                   nameof(GetCertificateChainStatusRequest)[..^7],

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

            this.CertificateStatusRequests = CertificateStatusRequests.Distinct();

            unchecked
            {
                hashCode = this.CertificateStatusRequests.CalcHashCode() * 3 ^
                           base.                          GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetCertificateChainStatusRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CertificateStatusSourceEnumType": {
        //             "description": "Source of status: OCSP, CRL",
        //             "javaType": "CertificateStatusSourceEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "CRL",
        //                 "OCSP"
        //             ]
        //         },
        //         "HashAlgorithmEnumType": {
        //             "description": "Used algorithms for the hashes provided.",
        //             "javaType": "HashAlgorithmEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "SHA256",
        //                 "SHA384",
        //                 "SHA512"
        //             ]
        //         },
        //         "CertificateHashDataType": {
        //             "javaType": "CertificateHashData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "hashAlgorithm": {
        //                     "$ref": "#/definitions/HashAlgorithmEnumType"
        //                 },
        //                 "issuerNameHash": {
        //                     "description": "The hash of the issuer\u2019s distinguished\r\nname (DN), that must be calculated over the DER\r\nencoding of the issuer\u2019s name field in the certificate\r\nbeing checked.",
        //                     "type": "string",
        //                     "maxLength": 128
        //                 },
        //                 "issuerKeyHash": {
        //                     "description": "The hash of the DER encoded public key:\r\nthe value (excluding tag and length) of the subject\r\npublic key field in the issuer\u2019s certificate.",
        //                     "type": "string",
        //                     "maxLength": 128
        //                 },
        //                 "serialNumber": {
        //                     "description": "The string representation of the\r\nhexadecimal value of the serial number without the\r\nprefix \"0x\" and without leading zeroes.",
        //                     "type": "string",
        //                     "maxLength": 40
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "hashAlgorithm",
        //                 "issuerNameHash",
        //                 "issuerKeyHash",
        //                 "serialNumber"
        //             ]
        //         },
        //         "CertificateStatusRequestInfoType": {
        //             "description": "Data necessary to request the revocation status of a certificate.",
        //             "javaType": "CertificateStatusRequestInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "certificateHashData": {
        //                     "$ref": "#/definitions/CertificateHashDataType"
        //                 },
        //                 "source": {
        //                     "$ref": "#/definitions/CertificateStatusSourceEnumType"
        //                 },
        //                 "urls": {
        //                     "description": "URL(s) of _source_.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "type": "string",
        //                         "maxLength": 2000
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "source",
        //                 "urls",
        //                 "certificateHashData"
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
        //         "certificateStatusRequests": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/CertificateStatusRequestInfoType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 4
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "certificateStatusRequests"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetCertificateChainStatus request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCertificateChainStatusRequestParser">A delegate to parse custom GetCertificateChainStatus requests.</param>
        public static GetCertificateChainStatusRequest Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             SourceRouting                                                   Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTimeOffset?                                                 RequestTimestamp                               = null,
                                                             TimeSpan?                                                       RequestTimeout                                 = null,
                                                             EventTracking_Id?                                               EventTrackingId                                = null,
                                                             CustomJObjectParserDelegate<GetCertificateChainStatusRequest>?  CustomGetCertificateChainStatusRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getCertificateChainStatusRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetCertificateChainStatusRequestParser))
            {
                return getCertificateChainStatusRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetCertificateChainStatus request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetCertificateChainStatusRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCertificateChainStatus request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetCertificateChainStatusRequest">The parsed GetCertificateChainStatus request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCertificateChainStatusRequestParser">A delegate to parse custom GetCertificateChainStatus requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       SourceRouting                                                   Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out GetCertificateChainStatusRequest?      GetCertificateChainStatusRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTimeOffset?                                                 RequestTimestamp                               = null,
                                       TimeSpan?                                                       RequestTimeout                                 = null,
                                       EventTracking_Id?                                               EventTrackingId                                = null,
                                       CustomJObjectParserDelegate<GetCertificateChainStatusRequest>?  CustomGetCertificateChainStatusRequestParser   = null)
        {

            try
            {

                GetCertificateChainStatusRequest = null;

                #region CertificateStatusRequests    [mandatory]

                if (!JSON.ParseMandatoryHashSet("ocspRequestData",
                                                "OCSP request data",
                                                CertificateStatusRequestInfo.TryParse,
                                                out HashSet<CertificateStatusRequestInfo> CertificateStatusRequests,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                   [optional, OCPP_CSE]

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

                #region CustomData                   [optional]

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


                GetCertificateChainStatusRequest = new GetCertificateChainStatusRequest(

                                                       Destination,
                                                       CertificateStatusRequests,

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

                if (CustomGetCertificateChainStatusRequestParser is not null)
                    GetCertificateChainStatusRequest = CustomGetCertificateChainStatusRequestParser(JSON,
                                                                                                    GetCertificateChainStatusRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCertificateChainStatusRequest  = null;
                ErrorResponse                     = "The given JSON representation of a GetCertificateChainStatus request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCertificateChainStatusRequestSerializer = null, CustomCertificateStatusRequestInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCertificateChainStatusRequestSerializer">A delegate to serialize custom GetCertificateChainStatus requests.</param>
        /// <param name="CustomCertificateStatusRequestInfoSerializer">A delegate to serialize custom CertificateStatusRequestInfos.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashDatas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                             IncludeJSONLDContext                               = false,
                              CustomJObjectSerializerDelegate<GetCertificateChainStatusRequest>?  CustomGetCertificateChainStatusRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateStatusRequestInfo>?      CustomCertificateStatusRequestInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?               CustomCertificateHashDataSerializer                = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                    DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("certificateStatusRequests",   new JArray(CertificateStatusRequests.Select(certificateStatusRequest => certificateStatusRequest.ToJSON(CustomCertificateStatusRequestInfoSerializer,
                                                                                                                                                                                      CustomCertificateHashDataSerializer,
                                                                                                                                                                                      CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",                  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                  CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCertificateChainStatusRequestSerializer is not null
                       ? CustomGetCertificateChainStatusRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCertificateChainStatusRequest1, GetCertificateChainStatusRequest2)

        /// <summary>
        /// Compares two GetCertificateChainStatus requests for equality.
        /// </summary>
        /// <param name="GetCertificateChainStatusRequest1">A GetCertificateChainStatus request.</param>
        /// <param name="GetCertificateChainStatusRequest2">Another GetCertificateChainStatus request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCertificateChainStatusRequest? GetCertificateChainStatusRequest1,
                                           GetCertificateChainStatusRequest? GetCertificateChainStatusRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCertificateChainStatusRequest1, GetCertificateChainStatusRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCertificateChainStatusRequest1 is null || GetCertificateChainStatusRequest2 is null)
                return false;

            return GetCertificateChainStatusRequest1.Equals(GetCertificateChainStatusRequest2);

        }

        #endregion

        #region Operator != (GetCertificateChainStatusRequest1, GetCertificateChainStatusRequest2)

        /// <summary>
        /// Compares two GetCertificateChainStatus requests for inequality.
        /// </summary>
        /// <param name="GetCertificateChainStatusRequest1">A GetCertificateChainStatus request.</param>
        /// <param name="GetCertificateChainStatusRequest2">Another GetCertificateChainStatus request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCertificateChainStatusRequest? GetCertificateChainStatusRequest1,
                                           GetCertificateChainStatusRequest? GetCertificateChainStatusRequest2)

            => !(GetCertificateChainStatusRequest1 == GetCertificateChainStatusRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCertificateChainStatusRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCertificateChainStatus requests for equality.
        /// </summary>
        /// <param name="Object">A GetCertificateChainStatus request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCertificateChainStatusRequest getCertificateChainStatusRequest &&
                   Equals(getCertificateChainStatusRequest);

        #endregion

        #region Equals(GetCertificateChainStatusRequest)

        /// <summary>
        /// Compares two GetCertificateChainStatus requests for equality.
        /// </summary>
        /// <param name="GetCertificateChainStatusRequest">A GetCertificateChainStatus request to compare with.</param>
        public override Boolean Equals(GetCertificateChainStatusRequest? GetCertificateChainStatusRequest)

            => GetCertificateChainStatusRequest is not null &&

               CertificateStatusRequests.ToHashSet().SetEquals(GetCertificateChainStatusRequest.CertificateStatusRequests) &&

               base.    GenericEquals(GetCertificateChainStatusRequest);

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

            => $"{CertificateStatusRequests.Count()} certificate status requests";

        #endregion

    }

}
