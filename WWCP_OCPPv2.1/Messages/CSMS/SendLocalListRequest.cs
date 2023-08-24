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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The send local list request.
    /// </summary>
    public class SendLocalListRequest : ARequest<SendLocalListRequest>
    {

        #region Properties

        /// <summary>
        /// In case of a full update this is the version number of the
        /// full list. In case of a differential update it is the
        /// version number of the list after the update has been applied.
        /// </summary>
        [Mandatory]
        public UInt64                          VersionNumber             { get; }

        /// <summary>
        /// The type of update (full or differential).
        /// </summary>
        [Mandatory]
        public UpdateTypes                     UpdateType                { get; }

        /// <summary>
        /// In case of a full update this contains the list of values that
        /// form the new local authorization list.
        /// In case of a differential update it contains the changes to be
        /// applied to the local authorization list in the charge point.
        /// Maximum number of AuthorizationData elements is available in
        /// the configuration key: SendLocalListMaxLength.
        /// </summary>
        [Optional]
        public IEnumerable<AuthorizationData>  LocalAuthorizationList    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a send local list request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VersionNumber">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of authorization data elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SendLocalListRequest(ChargeBox_Id                     ChargeBoxId,
                                    UInt64                           VersionNumber,
                                    UpdateTypes                      UpdateType,
                                    IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,
                                    CustomData?                      CustomData               = null,

                                    Request_Id?                      RequestId                = null,
                                    DateTime?                        RequestTimestamp         = null,
                                    TimeSpan?                        RequestTimeout           = null,
                                    EventTracking_Id?                EventTrackingId          = null,
                                    CancellationToken?               CancellationToken        = null)

            : base(ChargeBoxId,
                   "SendLocalList",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.VersionNumber           = VersionNumber;
            this.UpdateType              = UpdateType;
            this.LocalAuthorizationList  = LocalAuthorizationList ?? Array.Empty<AuthorizationData>();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SendLocalListRequest",
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
        //     "UpdateEnumType": {
        //       "description": "This contains the type of update (full or differential) of this request.\r\n",
        //       "javaType": "UpdateEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Differential",
        //         "Full"
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
        //     "AuthorizationData": {
        //       "description": "Contains the identifier to use for authorization.\r\n",
        //       "javaType": "AuthorizationData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "idToken": {
        //           "$ref": "#/definitions/IdTokenType"
        //         },
        //         "idTokenInfo": {
        //           "$ref": "#/definitions/IdTokenInfoType"
        //         }
        //       },
        //       "required": [
        //         "idToken"
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
        //     "localAuthorizationList": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/AuthorizationData"
        //       },
        //       "minItems": 1
        //     },
        //     "versionNumber": {
        //       "description": "In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.\r\n",
        //       "type": "integer"
        //     },
        //     "updateType": {
        //       "$ref": "#/definitions/UpdateEnumType"
        //     }
        //   },
        //   "required": [
        //     "versionNumber",
        //     "updateType"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSendLocalListRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a send local list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom send local list requests.</param>
        public static SendLocalListRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 ChargeBox_Id                                        ChargeBoxId,
                                                 CustomJObjectParserDelegate<SendLocalListRequest>?  CustomSendLocalListRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var sendLocalListRequest,
                         out var errorResponse,
                         CustomSendLocalListRequestParser))
            {
                return sendLocalListRequest!;
            }

            throw new ArgumentException("The given JSON representation of a send local list request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SendLocalListRequest, out ErrorResponse, CustomSendLocalListRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a send local list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed send local list request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       Request_Id                 RequestId,
                                       ChargeBox_Id               ChargeBoxId,
                                       out SendLocalListRequest?  SendLocalListRequest,
                                       out String?                ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SendLocalListRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a send local list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed send local list request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom send local list requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       ChargeBox_Id                                        ChargeBoxId,
                                       out SendLocalListRequest?                           SendLocalListRequest,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<SendLocalListRequest>?  CustomSendLocalListRequestParser)
        {

            try
            {

                SendLocalListRequest = null;

                #region VersionNumber             [mandatory]

                if (!JSON.ParseMandatory("versionNumber",
                                         "list version number",
                                         out UInt64 VersionNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UpdateType                [mandatory]

                if (!JSON.ParseMandatory("updateType",
                                         "update type",
                                         UpdateTypesExtensions.TryParse,
                                         out UpdateTypes UpdateType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LocalAuthorizationList    [mandatory]

                if (!JSON.ParseMandatoryJSON("localAuthorizationList",
                                             "local authorization list",
                                             AuthorizationData.TryParse,
                                             out IEnumerable<AuthorizationData> LocalAuthorizationList,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId               [optional, OCPP_CSE]

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


                SendLocalListRequest = new SendLocalListRequest(ChargeBoxId,
                                                                VersionNumber,
                                                                UpdateType,
                                                                LocalAuthorizationList,
                                                                CustomData,
                                                                RequestId);

                if (CustomSendLocalListRequestParser is not null)
                    SendLocalListRequest = CustomSendLocalListRequestParser(JSON,
                                                                            SendLocalListRequest);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListRequest  = null;
                ErrorResponse         = "The given JSON representation of a send local list request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSendLocalListRequestSerializer = null, CustomAuthorizationDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListRequestSerializer">A delegate to serialize custom send local list requests.</param>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom authorization data objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendLocalListRequest>?  CustomSendLocalListRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<AuthorizationData>?     CustomAuthorizationDataSerializer      = null,
                              CustomJObjectSerializerDelegate<IdToken>?               CustomIdTokenSerializer                = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?        CustomAdditionalInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?           CustomIdTokenInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<MessageContent>?        CustomMessageContentSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("versionNumber",            VersionNumber),
                                 new JProperty("updateType",               UpdateType.AsText()),

                           LocalAuthorizationList.IsNeitherNullNorEmpty()
                               ? new JProperty("localAuthorizationList",   new JArray(LocalAuthorizationList.Select(item => item.ToJSON(CustomAuthorizationDataSerializer,
                                                                                                                                        CustomIdTokenSerializer,
                                                                                                                                        CustomAdditionalInfoSerializer,
                                                                                                                                        CustomIdTokenInfoSerializer,
                                                                                                                                        CustomMessageContentSerializer,
                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSendLocalListRequestSerializer is not null
                       ? CustomSendLocalListRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two send local list requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A send local list request.</param>
        /// <param name="SendLocalListRequest2">Another send local list request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListRequest? SendLocalListRequest1,
                                           SendLocalListRequest? SendLocalListRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListRequest1, SendLocalListRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SendLocalListRequest1 is null || SendLocalListRequest2 is null)
                return false;

            return SendLocalListRequest1.Equals(SendLocalListRequest2);

        }

        #endregion

        #region Operator != (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two send local list requests for inequality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A send local list request.</param>
        /// <param name="SendLocalListRequest2">Another send local list request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListRequest? SendLocalListRequest1,
                                           SendLocalListRequest? SendLocalListRequest2)

            => !(SendLocalListRequest1 == SendLocalListRequest2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send local list requests for equality.
        /// </summary>
        /// <param name="Object">A send local list request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendLocalListRequest sendLocalListRequest &&
                   Equals(sendLocalListRequest);

        #endregion

        #region Equals(SendLocalListRequest)

        /// <summary>
        /// Compares two send local list requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest">A send local list request to compare with.</param>
        public override Boolean Equals(SendLocalListRequest? SendLocalListRequest)

            => SendLocalListRequest is not null &&

               VersionNumber.Equals(SendLocalListRequest.VersionNumber) &&
               UpdateType.   Equals(SendLocalListRequest.UpdateType)    &&

               LocalAuthorizationList.Count().Equals(SendLocalListRequest.LocalAuthorizationList.Count())                               &&
               LocalAuthorizationList.All(authorizationData => SendLocalListRequest.LocalAuthorizationList.Contains(authorizationData)) &&

               base.  GenericEquals(SendLocalListRequest);

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

                return VersionNumber.         GetHashCode() * 7 ^
                       UpdateType.            GetHashCode() * 5 ^
                       LocalAuthorizationList.GetHashCode() * 3 ^

                       base.                  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   UpdateType.AsText(),
                   " of ",
                   LocalAuthorizationList.Count(),
                   LocalAuthorizationList.Count(), " authorization list entries [version " + VersionNumber + "]"
               );

        #endregion

    }

}
