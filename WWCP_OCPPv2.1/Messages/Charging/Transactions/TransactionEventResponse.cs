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
    /// The TransactionEvent response.
    /// </summary>
    public class TransactionEventResponse : AResponse<TransactionEventRequest,
                                                      TransactionEventResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/transactionEventResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional final total cost of the charging transaction, including taxes.
        /// SHALL only be sent when charging has ended.
        /// In the currency configured with the Configuration Variable: Currency.
        /// When omitted, the transaction was NOT free.
        /// To indicate a free transaction, the central system SHALL send 0.00.
        /// </summary>
        [Optional]
        public Decimal?         TotalCost                 { get; }

        /// <summary>
        /// The optional charging priority from a business point of view.
        /// Default priority is 0.
        /// The range is from -9 to 9.
        /// Higher values indicate a higher priority.
        /// The charging priority in TransactionEventResponse is temporarily, so it may not be set in the IdTokenInfoType afterwards.
        /// Also the chargingPriority in TransactionEventResponse overrules the one in IdTokenInfoType.
        /// </summary>
        [Optional]
        public Int16?           ChargingPriority          { get; }

        /// <summary>
        /// The optional information about the authorization status, expiry and group id.
        /// This is required when the transactionEventRequest contained an identification token.
        /// </summary>
        [Optional]
        public IdTokenInfo?     IdTokenInfo               { get; }

        /// <summary>
        /// The optional personal message that should be shown to the EV driver.
        /// This can also be used to provide updated tariff information.
        /// </summary>
        [Optional]
        public MessageContent?  UpdatedPersonalMessage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a TransactionEvent response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="TotalCost">The optional final total cost of the charging transaction, including taxes.</param>
        /// <param name="ChargingPriority">The optional charging priority from a business point of view.</param>
        /// <param name="IdTokenInfo">The optional information about the authorization status, expiry and group id.</param>
        /// <param name="UpdatedPersonalMessage">The optional personal message that should be shown to the EV driver.</param>
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
        public TransactionEventResponse(TransactionEventRequest  Request,
                                        Decimal?                 TotalCost                = null,
                                        Int16?                   ChargingPriority         = null,
                                        IdTokenInfo?             IdTokenInfo              = null,
                                        MessageContent?          UpdatedPersonalMessage   = null,

                                        Result?                  Result                   = null,
                                        DateTime?                ResponseTimestamp        = null,

                                        SourceRouting?           Destination              = null,
                                        NetworkPath?             NetworkPath              = null,

                                        IEnumerable<KeyPair>?    SignKeys                 = null,
                                        IEnumerable<SignInfo>?   SignInfos                = null,
                                        IEnumerable<Signature>?  Signatures               = null,

                                        CustomData?              CustomData               = null,

                                        SerializationFormats?    SerializationFormat      = null,
                                        CancellationToken        CancellationToken        = default)

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

            this.TotalCost               = TotalCost;
            this.ChargingPriority        = ChargingPriority;
            this.IdTokenInfo             = IdTokenInfo;
            this.UpdatedPersonalMessage  = UpdatedPersonalMessage;

            unchecked
            {

                hashCode = (this.TotalCost?.             GetHashCode() ?? 0) * 11 ^
                           (this.ChargingPriority?.      GetHashCode() ?? 0) *  7 ^
                           (this.IdTokenInfo?.           GetHashCode() ?? 0) *  5 ^
                           (this.UpdatedPersonalMessage?.GetHashCode() ?? 0) *  3 ^
                            base.                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:TransactionEventResponse",
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
        //     "totalCost": {
        //       "description": "SHALL only be sent when charging has ended. Final total cost of this transaction, including taxes. In the currency configured with the Configuration Variable: &lt;&lt;configkey-currency,`Currency`&gt;&gt;. When omitted, the transaction was NOT free. To indicate a free transaction, the CSMS SHALL send 0.00.\r\n\r\n",
        //       "type": "number"
        //     },
        //     "chargingPriority": {
        //       "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; is temporarily, so it may not be set in the &lt;&lt;cmn_idtokeninfotype,IdTokenInfoType&gt;&gt; afterwards. Also the chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules the one in &lt;&lt;cmn_idtokeninfotype,IdTokenInfoType&gt;&gt;.  \r\n",
        //       "type": "integer"
        //     },
        //     "idTokenInfo": {
        //       "$ref": "#/definitions/IdTokenInfoType"
        //     },
        //     "updatedPersonalMessage": {
        //       "$ref": "#/definitions/MessageContentType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomTransactionEventRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TransactionEvent response.
        /// </summary>
        /// <param name="Request">The TransactionEvent request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTransactionEventResponseParser">A delegate to parse custom TransactionEvent responses.</param>
        public static TransactionEventResponse Parse(TransactionEventRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                       Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<TransactionEventResponse>?  CustomTransactionEventResponseParser   = null,
                                                     CustomJObjectParserDelegate<IdTokenInfo>?               CustomIdTokenInfoSerializer            = null,
                                                     CustomJObjectParserDelegate<IdToken>?                   CustomIdTokenSerializer                = null,
                                                     CustomJObjectParserDelegate<AdditionalInfo>?            CustomAdditionalInfoSerializer         = null,
                                                     CustomJObjectParserDelegate<MessageContent>?            CustomMessageContentSerializer         = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {


            if (TryParse(Request,
                         JSON,
                     Destination,
                         NetworkPath,
                         out var transactionEventResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomTransactionEventResponseParser,
                         CustomIdTokenInfoSerializer,
                         CustomIdTokenSerializer,
                         CustomAdditionalInfoSerializer,
                         CustomMessageContentSerializer,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return transactionEventResponse;
            }

            throw new ArgumentException("The given JSON representation of a TransactionEvent response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out TransactionEventResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a TransactionEvent response.
        /// </summary>
        /// <param name="Request">The TransactionEvent request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TransactionEventResponse">The parsed TransactionEvent response.</param>
        /// <param name="CustomTransactionEventResponseParser">A delegate to parse custom TransactionEvent responses.</param>
        public static Boolean TryParse(TransactionEventRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out TransactionEventResponse?      TransactionEventResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<TransactionEventResponse>?  CustomTransactionEventResponseParser   = null,
                                       CustomJObjectParserDelegate<IdTokenInfo>?               CustomIdTokenInfoSerializer            = null,
                                       CustomJObjectParserDelegate<IdToken>?                   CustomIdTokenSerializer                = null,
                                       CustomJObjectParserDelegate<AdditionalInfo>?            CustomAdditionalInfoSerializer         = null,
                                       CustomJObjectParserDelegate<MessageContent>?            CustomMessageContentSerializer         = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                TransactionEventResponse = null;

                #region TotalCost                 [optional]

                if (JSON.ParseOptional("totalCost",
                                       "total cost",
                                       out Decimal? TotalCost,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingPriority          [optional]

                if (JSON.ParseOptional("chargingPriority",
                                       "charging priority",
                                       out Int16? ChargingPriority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IdTokenInfo               [optional]

                if (JSON.ParseOptionalJSON("idTokenInfo",
                                           "identification token information",
                                           OCPPv2_1.IdTokenInfo.TryParse,
                                           out IdTokenInfo? IdTokenInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UpdatedPersonalMessage    [optional]

                if (JSON.ParseOptionalJSON("updatedPersonalMessage",
                                           "updated personal message",
                                           MessageContent.TryParse,
                                           out MessageContent? UpdatedPersonalMessage,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                TransactionEventResponse = new TransactionEventResponse(

                                               Request,
                                               TotalCost,
                                               ChargingPriority,
                                               IdTokenInfo,
                                               UpdatedPersonalMessage,
                                               null,
                                               ResponseTimestamp,

                                           Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomTransactionEventResponseParser is not null)
                    TransactionEventResponse = CustomTransactionEventResponseParser(JSON,
                                                                                    TransactionEventResponse);

                return true;

            }
            catch (Exception e)
            {
                TransactionEventResponse  = null;
                ErrorResponse             = "The given JSON representation of a TransactionEvent response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransactionEventResponseSerializer = null, CustomIdTokenInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransactionEventResponseSerializer">A delegate to serialize custom TransactionEvent responses.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens infos.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<TransactionEventResponse>?  CustomTransactionEventResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?               CustomIdTokenInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<IdToken>?                   CustomIdTokenSerializer                    = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?            CustomAdditionalInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<MessageContent>?            CustomMessageContentSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                 DefaultJSONLDContext.  ToString())
                               : null,

                           TotalCost.HasValue
                               ? new JProperty("totalCost",                TotalCost)
                               : null,

                           ChargingPriority.HasValue
                               ? new JProperty("chargingPriority",         ChargingPriority)
                               : null,

                           IdTokenInfo is not null
                               ? new JProperty("idTokenInfo",              IdTokenInfo.           ToJSON(CustomIdTokenInfoSerializer,
                                                                                                         CustomIdTokenSerializer,
                                                                                                         CustomAdditionalInfoSerializer,
                                                                                                         CustomMessageContentSerializer,
                                                                                                         CustomCustomDataSerializer))
                               : null,

                           UpdatedPersonalMessage is not null
                               ? new JProperty("updatedPersonalMessage",   UpdatedPersonalMessage.ToJSON(CustomMessageContentSerializer,
                                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",               new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                      CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.            ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTransactionEventResponseSerializer is not null
                       ? CustomTransactionEventResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The TransactionEvent failed because of a request error.
        /// </summary>
        /// <param name="Request">The TransactionEvent request.</param>
        public static TransactionEventResponse RequestError(TransactionEventRequest  Request,
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
                   null,
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
        /// The TransactionEvent failed.
        /// </summary>
        /// <param name="Request">The TransactionEvent request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TransactionEventResponse FormationViolation(TransactionEventRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The TransactionEvent failed.
        /// </summary>
        /// <param name="Request">The TransactionEvent request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TransactionEventResponse SignatureError(TransactionEventRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The TransactionEvent failed.
        /// </summary>
        /// <param name="Request">The TransactionEvent request.</param>
        /// <param name="Description">An optional error description.</param>
        public static TransactionEventResponse Failed(TransactionEventRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The TransactionEvent failed because of an exception.
        /// </summary>
        /// <param name="Request">The TransactionEvent request.</param>
        /// <param name="Exception">The exception.</param>
        public static TransactionEventResponse ExceptionOccured(TransactionEventRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (TransactionEventResponse1, TransactionEventResponse2)

        /// <summary>
        /// Compares two TransactionEvent responses for equality.
        /// </summary>
        /// <param name="TransactionEventResponse1">A TransactionEvent response.</param>
        /// <param name="TransactionEventResponse2">Another TransactionEvent response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TransactionEventResponse? TransactionEventResponse1,
                                           TransactionEventResponse? TransactionEventResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TransactionEventResponse1, TransactionEventResponse2))
                return true;

            // If one is null, but not both, return false.
            if (TransactionEventResponse1 is null || TransactionEventResponse2 is null)
                return false;

            return TransactionEventResponse1.Equals(TransactionEventResponse2);

        }

        #endregion

        #region Operator != (TransactionEventResponse1, TransactionEventResponse2)

        /// <summary>
        /// Compares two TransactionEvent responses for inequality.
        /// </summary>
        /// <param name="TransactionEventResponse1">A TransactionEvent response.</param>
        /// <param name="TransactionEventResponse2">Another TransactionEvent response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TransactionEventResponse? TransactionEventResponse1,
                                           TransactionEventResponse? TransactionEventResponse2)

            => !(TransactionEventResponse1 == TransactionEventResponse2);

        #endregion

        #endregion

        #region IEquatable<TransactionEventResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TransactionEvent responses for equality.
        /// </summary>
        /// <param name="Object">A TransactionEvent response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransactionEventResponse transactionEventResponse &&
                   Equals(transactionEventResponse);

        #endregion

        #region Equals(TransactionEventResponse)

        /// <summary>
        /// Compares two TransactionEvent responses for equality.
        /// </summary>
        /// <param name="TransactionEventResponse">A TransactionEvent response to compare with.</param>
        public override Boolean Equals(TransactionEventResponse? TransactionEventResponse)

            => TransactionEventResponse is not null &&

            ((!TotalCost.             HasValue    && !TransactionEventResponse.TotalCost.             HasValue)    ||
               TotalCost.             HasValue    &&  TransactionEventResponse.TotalCost.             HasValue    && TotalCost.       Value.Equals(TransactionEventResponse.TotalCost.       Value)) &&

            ((!ChargingPriority.      HasValue    && !TransactionEventResponse.ChargingPriority.      HasValue)    ||
               ChargingPriority.      HasValue    &&  TransactionEventResponse.ChargingPriority.      HasValue    && ChargingPriority.Value.Equals(TransactionEventResponse.ChargingPriority.Value)) &&

             ((IdTokenInfo            is     null &&  TransactionEventResponse.IdTokenInfo            is     null) ||
               IdTokenInfo            is not null &&  TransactionEventResponse.IdTokenInfo            is not null && IdTokenInfo.           Equals(TransactionEventResponse.IdTokenInfo))            &&

             ((UpdatedPersonalMessage is     null &&  TransactionEventResponse.UpdatedPersonalMessage is     null) ||
               UpdatedPersonalMessage is not null &&  TransactionEventResponse.UpdatedPersonalMessage is not null && UpdatedPersonalMessage.Equals(TransactionEventResponse.UpdatedPersonalMessage)) &&

               base.GenericEquals(TransactionEventResponse);

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

            => new String?[] {

                   TotalCost.HasValue
                       ? "Total cost: "               + TotalCost
                       : null,

                   ChargingPriority.HasValue
                       ? "Charging priority: "        + TotalCost
                       : null,

                   IdTokenInfo is not null
                       ? "Id token info: "            + IdTokenInfo
                       : null,

                   UpdatedPersonalMessage is not null
                       ? "Updated personal message: " + UpdatedPersonalMessage
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
