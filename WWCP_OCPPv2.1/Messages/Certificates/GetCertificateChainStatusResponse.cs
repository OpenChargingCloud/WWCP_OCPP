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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetCertificateChainStatus response.
    /// </summary>
    public class GetCertificateChainStatusResponse : AResponse<GetCertificateChainStatusRequest,
                                                               GetCertificateChainStatusResponse>,
                                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getCertificateChainStatusResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of certificate status.
        /// </summary>
        [Mandatory]
        public IEnumerable<CertificateStatusInfo>  CertificateStatus    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetCertificateChainStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request leading to this response.</param>
        /// <param name="CertificateStatus">An enumeration of certificate status.</param>
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
        public GetCertificateChainStatusResponse(GetCertificateChainStatusRequest    Request,
                                                 IEnumerable<CertificateStatusInfo>  CertificateStatus,

                                                 Result?                             Result                = null,
                                                 DateTimeOffset?                     ResponseTimestamp     = null,

                                                 SourceRouting?                      Destination           = null,
                                                 NetworkPath?                        NetworkPath           = null,

                                                 IEnumerable<KeyPair>?               SignKeys              = null,
                                                 IEnumerable<SignInfo>?              SignInfos             = null,
                                                 IEnumerable<Signature>?             Signatures            = null,

                                                 CustomData?                         CustomData            = null,

                                                 SerializationFormats?               SerializationFormat   = null,
                                                 CancellationToken                   CancellationToken     = default)

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

            this.CertificateStatus = CertificateStatus.Distinct();

            unchecked
            {

                hashCode = this.CertificateStatus.CalcHashCode() * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetCertificateChainStatusResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CertificateStatusEnumType": {
        //             "description": "Status of certificate: good, revoked or unknown.",
        //             "javaType": "CertificateStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Good",
        //                 "Revoked",
        //                 "Unknown",
        //                 "Failed"
        //             ]
        //         },
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
        //         "CertificateStatusType": {
        //             "description": "Revocation status of certificate",
        //             "javaType": "CertificateStatus",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "certificateHashData": {
        //                     "$ref": "#/definitions/CertificateHashDataType"
        //                 },
        //                 "source": {
        //                     "$ref": "#/definitions/CertificateStatusSourceEnumType"
        //                 },
        //                 "status": {
        //                     "$ref": "#/definitions/CertificateStatusEnumType"
        //                 },
        //                 "nextUpdate": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "source",
        //                 "status",
        //                 "nextUpdate",
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
        //         "certificateStatus": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/CertificateStatusType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 4
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "certificateStatus"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetCertificateChainStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCertificateChainStatusResponseParser">A delegate to parse custom GetCertificateChainStatus responses.</param>
        public static GetCertificateChainStatusResponse Parse(GetCertificateChainStatusRequest                                 Request,
                                                              JObject                                                          JSON,
                                                              SourceRouting                                                    Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTimeOffset?                                                  ResponseTimestamp                               = null,
                                                              CustomJObjectParserDelegate<GetCertificateChainStatusResponse>?  CustomGetCertificateChainStatusResponseParser   = null,
                                                              CustomJObjectParserDelegate<StatusInfo>?                         CustomStatusInfoParser                          = null,
                                                              CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                                              CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getCertificateChainStatusResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetCertificateChainStatusResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getCertificateChainStatusResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetCertificateChainStatus response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetCertificateChainStatusResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCertificateChainStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetCertificateChainStatusResponse">The parsed GetCertificateChainStatus response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCertificateChainStatusResponseParser">A delegate to parse custom GetCertificateChainStatus responses.</param>
        public static Boolean TryParse(GetCertificateChainStatusRequest                                 Request,
                                       JObject                                                          JSON,
                                       SourceRouting                                                    Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out GetCertificateChainStatusResponse?      GetCertificateChainStatusResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTimeOffset?                                                  ResponseTimestamp                               = null,
                                       CustomJObjectParserDelegate<GetCertificateChainStatusResponse>?  CustomGetCertificateChainStatusResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                         CustomStatusInfoParser                          = null,
                                       CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                       CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            try
            {

                GetCertificateChainStatusResponse = null;

                #region CertificateStatus    [mandatory]

                if (!JSON.ParseMandatoryHashSet("status",
                                                "GetCertificateChainStatus",
                                                CertificateStatusInfo.TryParse,
                                                out HashSet<CertificateStatusInfo> CertificateStatus,
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


                GetCertificateChainStatusResponse = new GetCertificateChainStatusResponse(

                                                        Request,
                                                        CertificateStatus,

                                                        null,
                                                        ResponseTimestamp,

                                                        Destination,
                                                        NetworkPath,

                                                        null,
                                                        null,
                                                        Signatures,

                                                        CustomData

                                                    );

                if (CustomGetCertificateChainStatusResponseParser is not null)
                    GetCertificateChainStatusResponse = CustomGetCertificateChainStatusResponseParser(JSON,
                                                                                                      GetCertificateChainStatusResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCertificateChainStatusResponse  = null;
                ErrorResponse                      = "The given JSON representation of a GetCertificateChainStatus response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCertificateChainStatusResponseSerializer = null, CustomCertificateStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCertificateChainStatusResponseSerializer">A delegate to serialize custom GetCertificateChainStatus responses.</param>
        /// <param name="CustomCertificateStatusInfoSerializer">A delegate to serialize custom CertificateStatusInfo objects.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashDatas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                              IncludeJSONLDContext                                = false,
                              CustomJObjectSerializerDelegate<GetCertificateChainStatusResponse>?  CustomGetCertificateChainStatusResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateStatusInfo>?              CustomCertificateStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?                CustomCertificateHashDataSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",            DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("certificateStatus",   new JArray(CertificateStatus.Select(certificateStatus => certificateStatus.ToJSON(CustomCertificateStatusInfoSerializer,
                                                                                                                                                        CustomCertificateHashDataSerializer,
                                                                                                                                                        CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.       Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCertificateChainStatusResponseSerializer is not null
                       ? CustomGetCertificateChainStatusResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetCertificateChainStatus failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request.</param>
        public static GetCertificateChainStatusResponse RequestError(GetCertificateChainStatusRequest  Request,
                                                                     EventTracking_Id                  EventTrackingId,
                                                                     ResultCode                        ErrorCode,
                                                                     String?                           ErrorDescription    = null,
                                                                     JObject?                          ErrorDetails        = null,
                                                                     DateTimeOffset?                   ResponseTimestamp   = null,

                                                                     SourceRouting?                    Destination         = null,
                                                                     NetworkPath?                      NetworkPath         = null,

                                                                     IEnumerable<KeyPair>?             SignKeys            = null,
                                                                     IEnumerable<SignInfo>?            SignInfos           = null,
                                                                     IEnumerable<Signature>?           Signatures          = null,

                                                                     CustomData?                       CustomData          = null)

            => new (

                   Request,
                   [],
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
        /// The GetCertificateChainStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateChainStatusResponse FormationViolation(GetCertificateChainStatusRequest  Request,
                                                                           String                            ErrorDescription)

            => new (Request,
                    [],
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCertificateChainStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateChainStatusResponse SignatureError(GetCertificateChainStatusRequest  Request,
                                                                       String                            ErrorDescription)

            => new (Request,
                    [],
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCertificateChainStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetCertificateChainStatusResponse Failed(GetCertificateChainStatusRequest  Request,
                                                               String?                           Description   = null)

            => new (Request,
                    [],
                    Result.Server(Description));


        /// <summary>
        /// The GetCertificateChainStatus failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetCertificateChainStatus request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetCertificateChainStatusResponse ExceptionOccurred(GetCertificateChainStatusRequest  Request,
                                                                         Exception                         Exception)

            => new (Request,
                    [],
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetCertificateChainStatusResponse1, GetCertificateChainStatusResponse2)

        /// <summary>
        /// Compares two GetCertificateChainStatus responses for equality.
        /// </summary>
        /// <param name="GetCertificateChainStatusResponse1">A GetCertificateChainStatus response.</param>
        /// <param name="GetCertificateChainStatusResponse2">Another GetCertificateChainStatus response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCertificateChainStatusResponse? GetCertificateChainStatusResponse1,
                                           GetCertificateChainStatusResponse? GetCertificateChainStatusResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCertificateChainStatusResponse1, GetCertificateChainStatusResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCertificateChainStatusResponse1 is null || GetCertificateChainStatusResponse2 is null)
                return false;

            return GetCertificateChainStatusResponse1.Equals(GetCertificateChainStatusResponse2);

        }

        #endregion

        #region Operator != (GetCertificateChainStatusResponse1, GetCertificateChainStatusResponse2)

        /// <summary>
        /// Compares two GetCertificateChainStatus responses for inequality.
        /// </summary>
        /// <param name="GetCertificateChainStatusResponse1">A GetCertificateChainStatus response.</param>
        /// <param name="GetCertificateChainStatusResponse2">Another GetCertificateChainStatus response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCertificateChainStatusResponse? GetCertificateChainStatusResponse1,
                                           GetCertificateChainStatusResponse? GetCertificateChainStatusResponse2)

            => !(GetCertificateChainStatusResponse1 == GetCertificateChainStatusResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCertificateChainStatusResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCertificateChainStatus responses for equality.
        /// </summary>
        /// <param name="Object">A GetCertificateChainStatus response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCertificateChainStatusResponse getCertificateChainStatusResponse &&
                   Equals(getCertificateChainStatusResponse);

        #endregion

        #region Equals(GetCertificateChainStatusResponse)

        /// <summary>
        /// Compares two GetCertificateChainStatus responses for equality.
        /// </summary>
        /// <param name="GetCertificateChainStatusResponse">A GetCertificateChainStatus response to compare with.</param>
        public override Boolean Equals(GetCertificateChainStatusResponse? GetCertificateChainStatusResponse)

            => GetCertificateChainStatusResponse is not null &&

               CertificateStatus.ToHashSet().SetEquals(GetCertificateChainStatusResponse.CertificateStatus) &&

               base.GenericEquals(GetCertificateChainStatusResponse);

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

            => $"{CertificateStatus.Count()} certificate status";

        #endregion

    }

}
