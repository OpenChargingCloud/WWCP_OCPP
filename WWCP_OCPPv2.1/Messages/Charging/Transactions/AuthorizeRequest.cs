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
    /// An Authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/authorizeRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        [Mandatory]
        public IdToken                       IdToken                        { get; }

        /// <summary>
        /// The X.509 certificate chain presented by EV and encoded in PEM format.
        /// Order of certificates in chain is from leaf up to (but excluding) root certificate.
        /// Only needed in case of central contract validation when Charging Station cannot validate the contract certificate (PEM format).
        /// </summary>
        [Optional]
        public OCPP.CertificateChain?        CertificateChain               { get; }

        /// <summary>
        /// Optional information to verify the electric vehicle/user contract certificate via OCSP.
        /// [0...4]
        /// </summary>
        [Optional]
        public IEnumerable<OCSPRequestData>  ISO15118CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorize request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="CertificateChain">The X.509 certificate chain presented by EV and encoded in PEM format. Order of certificates in chain is from leaf up to (but excluding) root certificate. Only needed in case of central contract validation when Charging Station cannot validate the contract certificate (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
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
        public AuthorizeRequest(SourceRouting                  Destination,
                                IdToken                        IdToken,
                                OCPP.CertificateChain?         CertificateChain              = null,
                                IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                                IEnumerable<KeyPair>?          SignKeys                      = null,
                                IEnumerable<SignInfo>?         SignInfos                     = null,
                                IEnumerable<Signature>?        Signatures                    = null,

                                CustomData?                    CustomData                    = null,

                                Request_Id?                    RequestId                     = null,
                                DateTime?                      RequestTimestamp              = null,
                                TimeSpan?                      RequestTimeout                = null,
                                EventTracking_Id?              EventTrackingId               = null,
                                NetworkPath?                   NetworkPath                   = null,
                                SerializationFormats?          SerializationFormat           = null,
                                CancellationToken              CancellationToken             = default)

            : base(Destination,
                   nameof(AuthorizeRequest)[..^7],

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

            this.IdToken                      = IdToken;
            this.CertificateChain             = CertificateChain;
            this.ISO15118CertificateHashData  = ISO15118CertificateHashData?.Distinct() ?? [];


            unchecked
            {

                hashCode = this.IdToken.                    GetHashCode()       * 7 ^
                          (this.CertificateChain?.          GetHashCode() ?? 0) * 5 ^
                           this.ISO15118CertificateHashData.CalcHashCode()      * 3 ^
                           base.                            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:AuthorizeRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
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
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "*(2.1)* This field specifies the additional IdToken.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "_additionalInfo_ can be used to send extra information to CSMS in addition to the regular authorization with _IdToken_. _AdditionalInfo_ contains one or more custom _types_, which need to be agreed upon by all parties involved. When the _type_ is not supported, the CSMS/Charging Station MAY ignore the _additionalInfo_.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "additionalIdToken",
        //                 "type"
        //             ]
        //         },
        //         "IdTokenType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "IdToken",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalInfo": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalInfoType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "idToken": {
        //                     "description": "*(2.1)* IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "*(2.1)* Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken",
        //                 "type"
        //             ]
        //         },
        //         "OCSPRequestDataType": {
        //             "description": "Information about a certificate for an OCSP check.",
        //             "javaType": "OCSPRequestData",
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
        //                 "responderURL": {
        //                     "description": "This contains the responder URL (Case insensitive). ",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "hashAlgorithm",
        //                 "issuerNameHash",
        //                 "issuerKeyHash",
        //                 "serialNumber",
        //                 "responderURL"
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
        //         "idToken": {
        //             "$ref": "#/definitions/IdTokenType"
        //         },
        //         "certificate": {
        //             "description": "*(2.1)* The X.509 certificate chain presented by EV and encoded in PEM format. Order of certificates in chain is from leaf up to (but excluding) root certificate. +\r\nOnly needed in case of central contract validation when Charging Station cannot validate the contract certificate.",
        //             "type": "string",
        //             "maxLength": 10000
        //         },
        //         "iso15118CertificateHashData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/OCSPRequestDataType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 4
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "idToken"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom authorize requests.</param>
        public static AuthorizeRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             SourceRouting                                   Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTime?                                       RequestTimestamp               = null,
                                             TimeSpan?                                       RequestTimeout                 = null,
                                             EventTracking_Id?                               EventTrackingId                = null,
                                             CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var authorizeRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAuthorizeRequestParser))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given JSON representation of an Authorize request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AuthorizeRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom authorize requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeRequest?      AuthorizeRequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTime?                                       RequestTimestamp               = null,
                                       TimeSpan?                                       RequestTimeout                 = null,
                                       EventTracking_Id?                               EventTrackingId                = null,
                                       CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdToken                        [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification tag",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateChain               [optional]

                if (JSON.ParseOptional("certificate",
                                       "PEM encoded electric vehicle/user certificate chain",
                                       OCPP.CertificateChain.TryParse,
                                       out OCPP.CertificateChain? CertificateChain,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ISO15118CertificateHashData    [optional]

                if (JSON.ParseOptionalJSON("iso15118CertificateHashData",
                                           "electric vehicle/user certificate",
                                           OCSPRequestData.TryParse,
                                           out IEnumerable<OCSPRequestData> ISO15118CertificateHashData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                     [optional, OCPP_CSE]

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

                #region CustomData                     [optional]

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


                AuthorizeRequest = new AuthorizeRequest(

                                       Destination,
                                       IdToken,
                                       CertificateChain,
                                       ISO15118CertificateHashData,

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

                if (CustomAuthorizeRequestParser is not null)
                    AuthorizeRequest = CustomAuthorizeRequestParser(JSON,
                                                                    AuthorizeRequest);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = null;
                ErrorResponse     = "The given JSON representation of an Authorize request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom authorize requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional infos.</param>
        /// <param name="CustomOCSPRequestDataSerializer">A delegate to serialize custom OCSP request data.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                             IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?           CustomIdTokenSerializer            = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?    CustomAdditionalInfoSerializer     = null,
                              CustomJObjectSerializerDelegate<OCSPRequestData>?   CustomOCSPRequestDataSerializer    = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("idToken",                       IdToken.             ToJSON(CustomIdTokenSerializer,
                                                                                                            CustomAdditionalInfoSerializer,
                                                                                                            CustomCustomDataSerializer)),

                           CertificateChain is not null
                               ? new JProperty("certificate",                   CertificateChain.    ToString())
                               : null,

                           ISO15118CertificateHashData is not null && ISO15118CertificateHashData.Any()
                               ? new JProperty("iso15118CertificateHashData",   new JArray(ISO15118CertificateHashData.SafeSelect(ocspRequestData => ocspRequestData.ToJSON(CustomOCSPRequestDataSerializer,
                                                                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",                    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeRequestSerializer is not null
                       ? CustomAuthorizeRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeRequest? AuthorizeRequest1,
                                           AuthorizeRequest? AuthorizeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeRequest1, AuthorizeRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeRequest1 is null || AuthorizeRequest2 is null)
                return false;

            return AuthorizeRequest1.Equals(AuthorizeRequest2);

        }

        #endregion

        #region Operator != (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two Authorize requests for inequality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeRequest? AuthorizeRequest1,
                                           AuthorizeRequest? AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="Object">An Authorize request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeRequest authorizeRequest &&
                   Equals(authorizeRequest);

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">An Authorize request to compare with.</param>
        public override Boolean Equals(AuthorizeRequest? AuthorizeRequest)

            => AuthorizeRequest is not null &&

               IdToken.Equals(AuthorizeRequest.IdToken) &&

             ((CertificateChain is     null && AuthorizeRequest.CertificateChain is     null) ||
              (CertificateChain is not null && AuthorizeRequest.CertificateChain is not null && CertificateChain.Equals(AuthorizeRequest.CertificateChain))) &&

               ISO15118CertificateHashData.Count().Equals(AuthorizeRequest.ISO15118CertificateHashData.Count())     &&
               ISO15118CertificateHashData.All(data =>    AuthorizeRequest.ISO15118CertificateHashData.Contains(data)) &&

               base.GenericEquals(AuthorizeRequest);

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

                   IdToken.ToString(),

                   CertificateChain is not null
                       ? $", with {CertificateChain.Length} certificates"
                       : "",

                   ISO15118CertificateHashData is not null
                       ? $", with {ISO15118CertificateHashData.Count()} OCSP request data"
                       : ""

               );

        #endregion

    }

}
