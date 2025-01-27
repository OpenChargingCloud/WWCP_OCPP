﻿/*
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
    /// The Authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<AuthorizeRequest,
                                               AuthorizeResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/authorizeResponse");

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

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="IdTokenInfo">The identification token info.</param>
        /// <param name="CertificateStatus">The optional certificate status information.</param>
        /// <param name="AllowedEnergyTransfer">Optional energy transfer modes accepted by the CSMS.</param>
        /// <param name="TransactionLimits">Optional maximum cost/energy/time limit allowed for this charging session.</param>
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
        public AuthorizeResponse(AuthorizeRequest             Request,
                                 IdTokenInfo                  IdTokenInfo,
                                 AuthorizeCertificateStatus?  CertificateStatus       = null,
                                 EnergyTransferMode?          AllowedEnergyTransfer   = null,
                                 TransactionLimits?           TransactionLimits       = null,

                                 Result?                      Result                  = null,
                                 DateTime?                    ResponseTimestamp       = null,

                                 SourceRouting?               Destination             = null,
                                 NetworkPath?                 NetworkPath             = null,

                                 IEnumerable<KeyPair>?        SignKeys                = null,
                                 IEnumerable<SignInfo>?       SignInfos               = null,
                                 IEnumerable<Signature>?      Signatures              = null,

                                 CustomData?                  CustomData              = null,

                                 SerializationFormats?        SerializationFormat     = null,
                                 CancellationToken            CancellationToken       = default)

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
        //       "description": "ID_ Token. Status. Authorization_ Status\r\nurn:x-oca:ocpp:uid:1:569372\r\nCurrent status of the ID Token.",
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
        //       "description": "Certificate status information. \r\n- if all certificates are valid: return 'Accepted'.\r\n- if one of the certificates was revoked, return 'CertificateRevoked'.",
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
        //       "description": "Enumeration of possible idToken types.",
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
        //       "description": "Message_ Content. Format. Message_ Format_ Code\r\nurn:x-enexis:ecdm:uid:1:570848\r\nFormat of the message.",
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
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //       "javaType": "AdditionalInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalIdToken": {
        //           "description": "This field specifies the additional IdToken.",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.",
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
        //       "description": "ID_ Token\r\nurn:x-oca:ocpp:uid:2:233247\r\nContains status information about an identifier.\r\nIt is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.",
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
        //           "description": "ID_ Token. Expiry. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569373\r\nDate and Time after which the token must be considered invalid.",
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
        //           "description": "ID_ Token. Language2. Language_ Code\r\nurn:x-oca:ocpp:uid:1:569375\r\nSecond preferred user interface language of identifier user. Don’t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
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
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
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
        //           "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
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
        //           "description": "Message_ Content. Language. Language_ Code\r\nurn:x-enexis:ecdm:uid:1:570849\r\nMessage language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static AuthorizeResponse Parse(AuthorizeRequest                                 Request,
                                              JObject                                          JSON,
                                              SourceRouting                                Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                              CustomJObjectParserDelegate<IdTokenInfo>?        CustomIdTokenInfoParser         = null,
                                              CustomJObjectParserDelegate<IdToken>?            CustomIdTokenParser             = null,
                                              CustomJObjectParserDelegate<AdditionalInfo>?     CustomAdditionalInfoParser      = null,
                                              CustomJObjectParserDelegate<MessageContent>?     CustomMessageContentParser      = null,
                                              CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var authorizeResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAuthorizeResponseParser,
                         CustomIdTokenInfoParser,
                         CustomIdTokenParser,
                         CustomAdditionalInfoParser,
                         CustomMessageContentParser,
                         CustomTransactionLimitsParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static Boolean TryParse(AuthorizeRequest                                 Request,
                                       JObject                                          JSON,
                                       SourceRouting                                Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeResponse?      AuthorizeResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                       CustomJObjectParserDelegate<IdTokenInfo>?        CustomIdTokenInfoParser         = null,
                                       CustomJObjectParserDelegate<IdToken>?            CustomIdTokenParser             = null,
                                       CustomJObjectParserDelegate<AdditionalInfo>?     CustomAdditionalInfoParser      = null,
                                       CustomJObjectParserDelegate<MessageContent>?     CustomMessageContentParser      = null,
                                       CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
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
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData               [optional]

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


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,
                                        IdTokenInfo,
                                        CertificateStatus,
                                        AllowedEnergyTransfer,
                                        TransactionLimits,

                                        null,
                                        ResponseTimestamp,

                                        Destination,
                                        NetworkPath,

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
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?        CustomIdTokenInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<TransactionLimits>?  CustomTransactionLimitsSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                DefaultJSONLDContext.       ToString())
                               : null,

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
        /// The Authorize failed because of a request error.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        public static AuthorizeResponse RequestError(AuthorizeRequest         Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTime?                ResponseTimestamp   = null,

                                                     SourceRouting?           Destination         = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
                   IdTokenInfo.Error(),
                   null,
                   null,
                   null,
                   OCPPv2_1.Result.FromErrorResponse(
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
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse FormationViolation(AuthorizeRequest  Request,
                                                           String            ErrorDescription)

            => new (Request,
                    IdTokenInfo.Error(AuthorizationStatus.ParsingError),
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse SignatureError(AuthorizeRequest  Request,
                                                       String            ErrorDescription)

            => new (Request,
                    IdTokenInfo.Error(AuthorizationStatus.SignatureError),
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AuthorizeResponse Failed(AuthorizeRequest  Request,
                                               String?           Description   = null)

            => new (Request,
                    IdTokenInfo.Error(),
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The Authorize failed because of an exception.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Exception">The exception.</param>
        public static AuthorizeResponse ExceptionOccured(AuthorizeRequest  Request,
                                                         Exception         Exception)

            => new (Request,
                    IdTokenInfo.Error(),
                    Result:  OCPPv2_1.Result.FromException(Exception));

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
