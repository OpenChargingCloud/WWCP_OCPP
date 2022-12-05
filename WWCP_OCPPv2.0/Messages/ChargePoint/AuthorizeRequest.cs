/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// An authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        [Mandatory]
        public IdToken                       IdToken                        { get; }

        /// <summary>
        /// The optional X.509 certificated presented by the electric vehicle/user (PEM format) 5500
        /// </summary>
        [Optional]
        public Certificate?                  Certificate                    { get; }

        /// <summary>
        /// Optional information to verify the electric vehicle/user contract certificate via OCSP. [0...4]
        /// </summary>
        [Optional]
        public IEnumerable<OCSPRequestData>  ISO15118CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorize request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public AuthorizeRequest(ChargeBox_Id                   ChargeBoxId,

                                IdToken                        IdToken,
                                Certificate?                   Certificate                   = null,
                                IEnumerable<OCSPRequestData>?  ISO15118CertificateHashData   = null,

                                CustomData?                    CustomData                    = null,
                                Request_Id?                    RequestId                     = null,
                                DateTime?                      RequestTimestamp              = null,
                                TimeSpan?                      RequestTimeout                = null,
                                EventTracking_Id?              EventTrackingId               = null,
                                CancellationToken?             CancellationToken             = null)

            : base(ChargeBoxId,
                   "Authorize",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.IdToken                      = IdToken;
            this.Certificate                  = Certificate;
            this.ISO15118CertificateHashData  = ISO15118CertificateHashData?.Distinct() ?? Array.Empty<OCSPRequestData>();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:AuthorizeRequest",
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
        //       "description": "Used algorithms for the hashes provided.\r\n",
        //       "javaType": "HashAlgorithmEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "IdTokenEnumType": {
        //       "description": "Enumeration of possible idToken types.\r\n",
        //       "javaType": "IdTokenEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Central",
        //         "eMAID",
        //         "ISO14443",
        //         "ISO15693",
        //         "KeyCode",
        //         "Local",
        //         "MacAddress",
        //         "NoAuthorization"
        //       ]
        //     },
        //     "AdditionalInfoType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "AdditionalInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalIdToken": {
        //           "description": "This field specifies the additional IdToken.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "additionalIdToken",
        //         "type"
        //       ]
        //     },
        //     "IdTokenType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "IdToken",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalInfo": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/AdditionalInfoType"
        //           },
        //           "minItems": 1
        //         },
        //         "idToken": {
        //           "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "$ref": "#/definitions/IdTokenEnumType"
        //         }
        //       },
        //       "required": [
        //         "idToken",
        //         "type"
        //       ]
        //     },
        //     "OCSPRequestDataType": {
        //       "javaType": "OCSPRequestData",
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
        //           "description": "Hashed value of the Issuer DN (Distinguished Name).\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "description": "Hashed value of the issuers public key\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "description": "The serial number of the certificate.\r\n",
        //           "type": "string",
        //           "maxLength": 40
        //         },
        //         "responderURL": {
        //           "description": "This contains the responder URL (Case insensitive). \r\n\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber",
        //         "responderURL"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "certificate": {
        //       "description": "The X.509 certificated presented by EV and encoded in PEM format.\r\n",
        //       "type": "string",
        //       "maxLength": 5500
        //     },
        //     "iso15118CertificateHashData": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/OCSPRequestDataType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 4
        //     }
        //   },
        //   "required": [
        //     "idToken"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom authorize requests.</param>
        public static AuthorizeRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             ChargeBox_Id                                    ChargeBoxId,
                                             CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var authorizeRequest,
                         out var errorResponse,
                         CustomAuthorizeRequestParser))
            {
                return authorizeRequest!;
            }

            throw new ArgumentException("The given JSON representation of an authorize request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out AuthorizeRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom authorize requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       ChargeBox_Id                                    ChargeBoxId,
                                       out AuthorizeRequest?                           AuthorizeRequest,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdToken                        [mandatory]

                if (!JSON.ParseMandatoryJSON("idTag",
                                             "identification tag",
                                             OCPPv2_0.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (IdToken is null)
                    return false;

                #endregion

                #region Certificate                    [optional]

                if (JSON.ParseOptional("certificate",
                                       "PEM encoded electric vehicle/user certificate",
                                       OCPPv2_0.Certificate.TryParse,
                                       out Certificate? Certificate,
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

                #region CustomData                     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                AuthorizeRequest = new AuthorizeRequest(ChargeBoxId,
                                                        IdToken,
                                                        Certificate,
                                                        ISO15118CertificateHashData,
                                                        CustomData,
                                                        RequestId);

                if (CustomAuthorizeRequestParser is not null)
                    AuthorizeRequest = CustomAuthorizeRequestParser(JSON,
                                                                    AuthorizeRequest);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = null;
                ErrorResponse     = "The given JSON representation of an authorize request is invalid: " + e.Message;
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
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?           CustomIdTokenSerializer            = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?    CustomAdditionalInfoSerializer     = null,
                              CustomJObjectSerializerDelegate<OCSPRequestData>?   CustomOCSPRequestDataSerializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("idToken",                            IdToken.    ToJSON(CustomIdTokenSerializer,
                                                                                                  CustomAdditionalInfoSerializer,
                                                                                                  CustomCustomDataSerializer)),

                           Certificate is not null
                               ? new JProperty("certificate",                  Certificate.ToString())
                               : null,

                           ISO15118CertificateHashData is not null && ISO15118CertificateHashData.Any()
                               ? new JProperty("iso15118CertificateHashData",  new JArray(ISO15118CertificateHashData.SafeSelect(hashData => hashData.ToJSON(CustomOCSPRequestDataSerializer,
                                                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeRequestSerializer is not null
                       ? CustomAuthorizeRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two authorize requests for equality.
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
        /// Compares two authorize requests for inequality.
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
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="Object">An authorize request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeRequest authorizeRequest &&
                   Equals(authorizeRequest);

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">An authorize request to compare with.</param>
        public override Boolean Equals(AuthorizeRequest? AuthorizeRequest)

            => AuthorizeRequest is not null &&

               IdToken.Equals(AuthorizeRequest.IdToken) &&

             ((Certificate is     null && AuthorizeRequest.Certificate is     null) ||
              (Certificate is not null && AuthorizeRequest.Certificate is not null && Certificate.Equals(AuthorizeRequest.Certificate))) &&

               ISO15118CertificateHashData.Count().Equals(AuthorizeRequest.ISO15118CertificateHashData.Count())     &&
               ISO15118CertificateHashData.All(data =>    AuthorizeRequest.ISO15118CertificateHashData.Contains(data)) &&

               base.GenericEquals(AuthorizeRequest);

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

                return IdToken.                    GetHashCode()       * 7 ^
                      (Certificate?.               GetHashCode() ?? 0) * 5 ^
                       ISO15118CertificateHashData.CalcHashCode()      * 3 ^

                       base.                       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IdToken.ToString();

        #endregion

    }

}
