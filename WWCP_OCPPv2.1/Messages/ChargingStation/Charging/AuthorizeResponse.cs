/*
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
    /// An authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<CS.AuthorizeRequest,
                                               AuthorizeResponse>,
                                     IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/authorizeResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification token info.
        /// </summary>
        [Mandatory]
        public IdTokenInfo                  IdTokenInfo              { get; }

        /// <summary>
        /// The optional certificate status information.
        /// When all certificates are valid: return 'Accepted', but when one of the certificates was revoked, return 'CertificateRevoked'.
        /// </summary>
        [Optional]
        public AuthorizeCertificateStatus?  CertificateStatus        { get; }

        /// <summary>
        /// The optional energy transfer modes accepted by the CSMS.
        /// </summary>
        [Optional]
        public EnergyTransferMode?          AllowedEnergyTransfer    { get; }

        /// <summary>
        /// The optional maximum cost/energy/time limit allowed for this charging session.
        /// </summary>
        [Optional]
        public TransactionLimits?           TransactionLimits        { get; }

        #endregion

        #region Constructor(s)

        #region AuthorizeResponse(Request, IdTokenInfo, CertificateStatus = null, ...)

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="IdTokenInfo">The identification token info.</param>
        /// <param name="CertificateStatus">The optional certificate status information.</param>
        /// <param name="AllowedEnergyTransfer">Optional energy transfer modes accepted by the CSMS.</param>
        /// <param name="TransactionLimits">Optional maximum cost/energy/time limit allowed for this charging session.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AuthorizeResponse(CS.AuthorizeRequest          Request,
                                 IdTokenInfo                  IdTokenInfo,
                                 AuthorizeCertificateStatus?  CertificateStatus       = null,
                                 EnergyTransferMode?          AllowedEnergyTransfer   = null,
                                 TransactionLimits?           TransactionLimits       = null,

                                 DateTime?                    ResponseTimestamp       = null,

                                 IEnumerable<KeyPair>?        SignKeys                = null,
                                 IEnumerable<SignInfo>?       SignInfos               = null,
                                 IEnumerable<OCPP.Signature>? Signatures              = null,

                                 CustomData?                  CustomData              = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.IdTokenInfo            = IdTokenInfo;
            this.CertificateStatus      = CertificateStatus;
            this.AllowedEnergyTransfer  = AllowedEnergyTransfer;
            this.TransactionLimits      = TransactionLimits;


            unchecked
            {

                hashCode = this.IdTokenInfo.           GetHashCode()       * 5 ^
                          (this.CertificateStatus?.    GetHashCode() ?? 0) * 3 ^
                          (this.AllowedEnergyTransfer?.GetHashCode() ?? 0) * 3 ^
                          (this.TransactionLimits?.    GetHashCode() ?? 0) * 3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion

        #region AuthorizeResponse(Request, Result)

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public AuthorizeResponse(CS.AuthorizeRequest  Request,
                                 Result               Result)

            : base(Request,
                   Result)

        {

            this.IdTokenInfo = new IdTokenInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:AuthorizeResponse",
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
        //     "AuthorizationStatusEnumType": {
        //       "description": "ID_ Token. Status. Authorization_ Status\r\nurn:x-oca:ocpp:uid:1:569372\r\nCurrent status of the ID Token.\r\n",
        //       "javaType": "AuthorizationStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Blocked",
        //         "ConcurrentTx",
        //         "Expired",
        //         "Invalid",
        //         "NoCredit",
        //         "NotAllowedTypeEVSE",
        //         "NotAtThisLocation",
        //         "NotAtThisTime",
        //         "Unknown"
        //       ]
        //     },
        //     "AuthorizeCertificateStatusEnumType": {
        //       "description": "Certificate status information. \r\n- if all certificates are valid: return 'Accepted'.\r\n- if one of the certificates was revoked, return 'CertificateRevoked'.\r\n",
        //       "javaType": "AuthorizeCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "SignatureError",
        //         "CertificateExpired",
        //         "CertificateRevoked",
        //         "NoCertificateAvailable",
        //         "CertChainError",
        //         "ContractCancelled"
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
        //     "MessageFormatEnumType": {
        //       "description": "Message_ Content. Format. Message_ Format_ Code\r\nurn:x-enexis:ecdm:uid:1:570848\r\nFormat of the message.\r\n",
        //       "javaType": "MessageFormatEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ASCII",
        //         "HTML",
        //         "URI",
        //         "UTF8"
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
        //     "IdTokenInfoType": {
        //       "description": "ID_ Token\r\nurn:x-oca:ocpp:uid:2:233247\r\nContains status information about an identifier.\r\nIt is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.\r\n",
        //       "javaType": "IdTokenInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "status": {
        //           "$ref": "#/definitions/AuthorizationStatusEnumType"
        //         },
        //         "cacheExpiryDateTime": {
        //           "description": "ID_ Token. Expiry. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569373\r\nDate and Time after which the token must be considered invalid.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "chargingPriority": {
        //           "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules this one. \r\n",
        //           "type": "integer"
        //         },
        //         "language1": {
        //           "description": "ID_ Token. Language1. Language_ Code\r\nurn:x-oca:ocpp:uid:1:569374\r\nPreferred user interface language of identifier user. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 8
        //         },
        //         "evseId": {
        //           "description": "Only used when the IdToken is only valid for one or more specific EVSEs, not for the entire Charging Station.\r\n\r\n",
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "type": "integer"
        //           },
        //           "minItems": 1
        //         },
        //         "groupIdToken": {
        //           "$ref": "#/definitions/IdTokenType"
        //         },
        //         "language2": {
        //           "description": "ID_ Token. Language2. Language_ Code\r\nurn:x-oca:ocpp:uid:1:569375\r\nSecond preferred user interface language of identifier user. Don’t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n",
        //           "type": "string",
        //           "maxLength": 8
        //         },
        //         "personalMessage": {
        //           "$ref": "#/definitions/MessageContentType"
        //         }
        //       },
        //       "required": [
        //         "status"
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
        //     "MessageContentType": {
        //       "description": "Message_ Content\r\nurn:x-enexis:ecdm:uid:2:234490\r\nContains message details, for a message to be displayed on a Charging Station.\r\n\r\n",
        //       "javaType": "MessageContent",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "format": {
        //           "$ref": "#/definitions/MessageFormatEnumType"
        //         },
        //         "language": {
        //           "description": "Message_ Content. Language. Language_ Code\r\nurn:x-enexis:ecdm:uid:1:570849\r\nMessage language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n",
        //           "type": "string",
        //           "maxLength": 8
        //         },
        //         "content": {
        //           "description": "Message_ Content. Content. Message\r\nurn:x-enexis:ecdm:uid:1:570852\r\nMessage contents.\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "format",
        //         "content"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "idTokenInfo": {
        //       "$ref": "#/definitions/IdTokenInfoType"
        //     },
        //     "certificateStatus": {
        //       "$ref": "#/definitions/AuthorizeCertificateStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "idTokenInfo"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomAuthorizeResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static AuthorizeResponse Parse(CS.AuthorizeRequest                              Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var authorizeResponse,
                         out var errorResponse,
                         CustomAuthorizeResponseParser) &&
                authorizeResponse is not null)
            {
                return authorizeResponse;
            }

            throw new ArgumentException("The given JSON representation of an authorize response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AuthorizeResponse, out ErrorResponse, out ErrorResponse, CustomAuthorizeResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static Boolean TryParse(CS.AuthorizeRequest                              Request,
                                       JObject                                          JSON,
                                       out AuthorizeResponse?                           AuthorizeResponse,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null)
        {

            try
            {

                AuthorizeResponse = null;

                #region IdTokenInfo              [mandatory]

                if (!JSON.ParseMandatoryJSON("idTokenInfo",
                                             "identification tag information",
                                             OCPPv2_1.IdTokenInfo.TryParse,
                                             out IdTokenInfo? IdTokenInfo,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (IdTokenInfo is null)
                    return false;

                #endregion

                #region CertificateStatus        [optional]

                if (JSON.ParseOptional("certificateStatus",
                                       "certificate status",
                                       AuthorizeCertificateStatusExtensions.TryParse,
                                       out AuthorizeCertificateStatus? CertificateStatus,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AllowedEnergyTransfer    [optional]

                if (JSON.ParseOptional("allowedEnergyTransfer",
                                       "allowed energy transfer",
                                       EnergyTransferMode.TryParse,
                                       out EnergyTransferMode? AllowedEnergyTransfer,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionLimits        [optional]

                if (JSON.ParseOptionalJSON("transactionLimit",
                                           "transaction limits",
                                           OCPPv2_1.TransactionLimits.TryParse,
                                           out TransactionLimits? TransactionLimits,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures               [optional, OCPP_CSE]

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

                #region CustomData               [optional]

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


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,
                                        IdTokenInfo,
                                        CertificateStatus,
                                        AllowedEnergyTransfer,
                                        TransactionLimits,
                                        null,

                                        null,
                                        null,
                                        Signatures,

                                        CustomData

                                    );

                if (CustomAuthorizeResponseParser is not null)
                    AuthorizeResponse = CustomAuthorizeResponseParser(JSON,
                                                                      AuthorizeResponse);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given JSON representation of an authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomIdTokenInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom authorize responses.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification token infos.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomTransactionLimitsSerializer">A delegate to serialize custom transaction limits.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?        CustomIdTokenInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<TransactionLimits>?  CustomTransactionLimitsSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idTokenInfo",             IdTokenInfo.                ToJSON(CustomIdTokenInfoSerializer,
                                                                                                             CustomIdTokenSerializer,
                                                                                                             CustomAdditionalInfoSerializer,
                                                                                                             CustomMessageContentSerializer,
                                                                                                             CustomCustomDataSerializer)),

                           CertificateStatus.HasValue
                               ? new JProperty("certificateStatus",       CertificateStatus.    Value.AsText())
                               : null,

                           AllowedEnergyTransfer.HasValue
                               ? new JProperty("allowedEnergyTransfer",   AllowedEnergyTransfer.Value.ToString())
                               : null,

                           TransactionLimits is not null
                               ? new JProperty("transactionLimits",       TransactionLimits.          ToJSON(CustomTransactionLimitsSerializer,
                                                                                                             CustomCustomDataSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",              new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.                 ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeResponseSerializer is not null
                       ? CustomAuthorizeResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The authentication failed.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public static AuthorizeResponse Failed(CS.AuthorizeRequest Request)

            => new (Request,
                    new IdTokenInfo(
                        AuthorizationStatus.Invalid
                    ));

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeResponse1 is null || AuthorizeResponse2 is null)
                return false;

            return AuthorizeResponse1.Equals(AuthorizeResponse2);

        }

        #endregion

        #region Operator != (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two authorize responses for inequality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)

            => !(AuthorizeResponse1 == AuthorizeResponse2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="Object">An authorize response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeResponse authorizeResponse &&
                   Equals(authorizeResponse);

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">An authorize response to compare with.</param>
        public override Boolean Equals(AuthorizeResponse? AuthorizeResponse)

            => AuthorizeResponse is not null &&

               IdTokenInfo.Equals(AuthorizeResponse.IdTokenInfo) &&

            ((!CertificateStatus.    HasValue    && !AuthorizeResponse.CertificateStatus.    HasValue) ||
               CertificateStatus.    HasValue    &&  AuthorizeResponse.CertificateStatus.    HasValue    && CertificateStatus.    Value.Equals(AuthorizeResponse.CertificateStatus.Value))     &&

            ((!AllowedEnergyTransfer.HasValue    && !AuthorizeResponse.AllowedEnergyTransfer.HasValue) ||
               AllowedEnergyTransfer.HasValue    &&  AuthorizeResponse.AllowedEnergyTransfer.HasValue    && AllowedEnergyTransfer.Value.Equals(AuthorizeResponse.AllowedEnergyTransfer.Value)) &&

             ((TransactionLimits     is null     &&  AuthorizeResponse.TransactionLimits     is null)  ||
               TransactionLimits     is not null &&  AuthorizeResponse.TransactionLimits     is not null && TransactionLimits.          Equals(AuthorizeResponse.TransactionLimits))           &&

               base.GenericEquals(AuthorizeResponse);

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

            => IdTokenInfo.ToString();

        #endregion

    }

}
