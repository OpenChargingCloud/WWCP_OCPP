/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.adapters.OCPPv2_0.CP
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
        public IdToken                       IdToken                        { get; }

        /// <summary>
        /// The optional X.509 certificated presented by the electric vehicle/user (PEM format) 5500
        /// </summary>
        public String                        Certificate                    { get; }

        /// <summary>
        /// Optional information to verify the electric vehicle/user contract certificate via OCSP. [0...4]
        /// </summary>
        public IEnumerable<OCSPRequestData>  ISO15118CertificateHashData    { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData                    CustomData                     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an authorize request.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public AuthorizeRequest(IdToken                       IdToken,
                                String                        Certificate                   = null,
                                IEnumerable<OCSPRequestData>  ISO15118CertificateHashData   = null,
                                CustomData                    CustomData                    = null)
        {

            this.IdToken                      = IdToken                     ?? throw new ArgumentNullException(nameof(IdToken), "The given identification token must not be null!");
            this.Certificate                  = Certificate;
            this.ISO15118CertificateHashData  = ISO15118CertificateHashData ?? new OCSPRequestData[0];
            this.CustomData                   = CustomData;

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

        #region (static) Parse   (AuthorizeRequestText, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(JObject              AuthorizeRequestJSON,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(AuthorizeRequestJSON,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (AuthorizeRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(String               AuthorizeRequestText,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(AuthorizeRequestText,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestJSON, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestJSON">The JSON to be parsed.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject               AuthorizeRequestJSON,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdToken

                if (!AuthorizeRequestJSON.ParseMandatory3("idTag",
                                                          "identification tag",
                                                          OCPPv2_0.IdToken.TryParse,
                                                          out IdToken  IdToken,
                                                          out String   ErrorResponse,
                                                          OnException))
                {
                    return false;
                }

                #endregion

                #region Certificate

                if (AuthorizeRequestJSON.ParseOptional("certificate",
                                                       "electric vehicle/user certificate",
                                                       out String  Certificate,
                                                       out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ISO15118CertificateHashData

                if (AuthorizeRequestJSON.ParseOptionalHashSet("iso15118CertificateHashData",
                                                              "electric vehicle/user certificate",
                                                              OCSPRequestData.TryParse,
                                                              out HashSet<OCSPRequestData>  ISO15118CertificateHashData,
                                                              out                           ErrorResponse,
                                                              OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (AuthorizeRequestJSON.ParseOptionalJSON("customData",
                                                           "custom data",
                                                           OCPPv2_0.CustomData.TryParse,
                                                           out CustomData  CustomData,
                                                           out             ErrorResponse,
                                                           OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                AuthorizeRequest = new AuthorizeRequest(IdToken,
                                                        Certificate?.Trim(),
                                                        ISO15118CertificateHashData,
                                                        CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AuthorizeRequestJSON, e);

                AuthorizeRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestText, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to be parsed.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                AuthorizeRequestText,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequestText = AuthorizeRequestText?.Trim();

                if (AuthorizeRequestText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(AuthorizeRequestText),
                             out AuthorizeRequest,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AuthorizeRequestText, e);
            }

            AuthorizeRequest = null;
            return false;

        }

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom authorize requests.</param>
        /// <param name="CustomIdTokenResponseSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoResponseSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomOCSPRequestDataResponseSerializer">A delegate to serialize custom OCSPRequestDatas.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest> CustomAuthorizeRequestSerializer          = null,
                              CustomJObjectSerializerDelegate<IdToken>          CustomIdTokenResponseSerializer           = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>   CustomAdditionalInfoResponseSerializer    = null,
                              CustomJObjectSerializerDelegate<OCSPRequestData>  CustomOCSPRequestDataResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>       CustomCustomDataResponseSerializer        = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("idToken",                            IdToken.ToJSON(CustomIdTokenResponseSerializer,
                                                                                              CustomAdditionalInfoResponseSerializer,
                                                                                              CustomCustomDataResponseSerializer)),

                           Certificate.IsNotNullOrEmpty()
                               ? new JProperty("certificate",                  Certificate)
                               : null,

                           ISO15118CertificateHashData.SafeAny()
                               ? new JProperty("iso15118CertificateHashData",  new JArray(ISO15118CertificateHashData.SafeSelect(hashData => hashData.ToJSON(CustomOCSPRequestDataResponseSerializer,
                                                                                                                                                             CustomCustomDataResponseSerializer))))
                               : null,

                           CustomData != null
                               ? new JProperty("customData",                   CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomAuthorizeRequestSerializer != null
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
        public static Boolean operator == (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeRequest1, AuthorizeRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((AuthorizeRequest1 is null) || (AuthorizeRequest2 is null))
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
        public static Boolean operator != (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is AuthorizeRequest AuthorizeRequest))
                return false;

            return Equals(AuthorizeRequest);

        }

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">A authorize request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(AuthorizeRequest AuthorizeRequest)
        {

            if (AuthorizeRequest is null)
                return false;

            return IdToken.Equals(AuthorizeRequest.IdToken);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => IdToken.GetHashCode();

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
