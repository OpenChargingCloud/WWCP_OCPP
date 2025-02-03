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
    /// The SendLocalList request.
    /// </summary>
    public class SendLocalListRequest : ARequest<SendLocalListRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/sendLocalListRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

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
        /// Create a SendLocalList request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="VersionNumber">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of authorization data elements is available in the configuration key: SendLocalListMaxLength.</param>
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
        public SendLocalListRequest(SourceRouting                    Destination,
                                    UInt64                           VersionNumber,
                                    UpdateTypes                      UpdateType,
                                    IEnumerable<AuthorizationData>?  LocalAuthorizationList   = null,

                                    IEnumerable<KeyPair>?            SignKeys                 = null,
                                    IEnumerable<SignInfo>?           SignInfos                = null,
                                    IEnumerable<Signature>?          Signatures               = null,

                                    CustomData?                      CustomData               = null,

                                    Request_Id?                      RequestId                = null,
                                    DateTime?                        RequestTimestamp         = null,
                                    TimeSpan?                        RequestTimeout           = null,
                                    EventTracking_Id?                EventTrackingId          = null,
                                    NetworkPath?                     NetworkPath              = null,
                                    SerializationFormats?            SerializationFormat      = null,
                                    CancellationToken                CancellationToken        = default)

            : base(Destination,
                   nameof(SendLocalListRequest)[..^7],

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

            this.VersionNumber           = VersionNumber;
            this.UpdateType              = UpdateType;
            this.LocalAuthorizationList  = LocalAuthorizationList ?? Array.Empty<AuthorizationData>();

            unchecked
            {

                hashCode = this.VersionNumber.         GetHashCode() * 7 ^
                           this.UpdateType.            GetHashCode() * 5 ^
                           this.LocalAuthorizationList.GetHashCode() * 3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SendLocalListRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "AuthorizationStatusEnumType": {
        //             "description": "Current status of the ID Token.",
        //             "javaType": "AuthorizationStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Blocked",
        //                 "ConcurrentTx",
        //                 "Expired",
        //                 "Invalid",
        //                 "NoCredit",
        //                 "NotAllowedTypeEVSE",
        //                 "NotAtThisLocation",
        //                 "NotAtThisTime",
        //                 "Unknown"
        //             ]
        //         },
        //         "MessageFormatEnumType": {
        //             "description": "Format of the message.",
        //             "javaType": "MessageFormatEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ASCII",
        //                 "HTML",
        //                 "URI",
        //                 "UTF8",
        //                 "QRCODE"
        //             ]
        //         },
        //         "UpdateEnumType": {
        //             "description": "This contains the type of update (full or differential) of this request.",
        //             "javaType": "UpdateEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Differential",
        //                 "Full"
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
        //         "AuthorizationData": {
        //             "description": "Contains the identifier to use for authorization.",
        //             "javaType": "AuthorizationData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "idToken": {
        //                     "$ref": "#/definitions/IdTokenType"
        //                 },
        //                 "idTokenInfo": {
        //                     "$ref": "#/definitions/IdTokenInfoType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken"
        //             ]
        //         },
        //         "IdTokenInfoType": {
        //             "description": "Contains status information about an identifier.\r\nIt is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.",
        //             "javaType": "IdTokenInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "status": {
        //                     "$ref": "#/definitions/AuthorizationStatusEnumType"
        //                 },
        //                 "cacheExpiryDateTime": {
        //                     "description": "Date and Time after which the token must be considered invalid.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingPriority": {
        //                     "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules this one. ",
        //                     "type": "integer"
        //                 },
        //                 "groupIdToken": {
        //                     "$ref": "#/definitions/IdTokenType"
        //                 },
        //                 "language1": {
        //                     "description": "Preferred user interface language of identifier user. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "language2": {
        //                     "description": "Second preferred user interface language of identifier user. Don\u2019t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "evseId": {
        //                     "description": "Only used when the IdToken is only valid for one or more specific EVSEs, not for the entire Charging Station.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "type": "integer",
        //                         "minimum": 0.0
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "personalMessage": {
        //                     "$ref": "#/definitions/MessageContentType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "status"
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
        //         "MessageContentType": {
        //             "description": "Contains message details, for a message to be displayed on a Charging Station.",
        //             "javaType": "MessageContent",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "format": {
        //                     "$ref": "#/definitions/MessageFormatEnumType"
        //                 },
        //                 "language": {
        //                     "description": "Message language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "content": {
        //                     "description": "*(2.1)* Required. Message contents. +\r\nMaximum length supported by Charging Station is given in OCPPCommCtrlr.FieldLength[\"MessageContentType.content\"].\r\n    Maximum length defaults to 1024.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "format",
        //                 "content"
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
        //         "localAuthorizationList": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/AuthorizationData"
        //             },
        //             "minItems": 1
        //         },
        //         "versionNumber": {
        //             "description": "In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.",
        //             "type": "integer"
        //         },
        //         "updateType": {
        //             "$ref": "#/definitions/UpdateEnumType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "versionNumber",
        //         "updateType"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SendLocalList request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom SendLocalList requests.</param>
        public static SendLocalListRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<SendLocalListRequest>?  CustomSendLocalListRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var sendLocalListRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSendLocalListRequestParser))
            {
                return sendLocalListRequest;
            }

            throw new ArgumentException("The given JSON representation of a SendLocalList request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SendLocalListRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SendLocalList request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SendLocalListRequest">The parsed SendLocalList request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom SendLocalList requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out SendLocalListRequest?      SendLocalListRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<SendLocalListRequest>?  CustomSendLocalListRequestParser   = null)
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


                SendLocalListRequest = new SendLocalListRequest(

                                           Destination,
                                           VersionNumber,
                                           UpdateType,
                                           LocalAuthorizationList,

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

                if (CustomSendLocalListRequestParser is not null)
                    SendLocalListRequest = CustomSendLocalListRequestParser(JSON,
                                                                            SendLocalListRequest);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListRequest  = null;
                ErrorResponse         = "The given JSON representation of a SendLocalList request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSendLocalListRequestSerializer = null, CustomAuthorizationDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListRequestSerializer">A delegate to serialize custom SendLocalList requests.</param>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom authorization data objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext                   = false,
                              CustomJObjectSerializerDelegate<SendLocalListRequest>?  CustomSendLocalListRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<AuthorizationData>?     CustomAuthorizationDataSerializer      = null,
                              CustomJObjectSerializerDelegate<IdToken>?               CustomIdTokenSerializer                = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?        CustomAdditionalInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?           CustomIdTokenInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<MessageContent>?        CustomMessageContentSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                 DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("versionNumber",            VersionNumber),
                                 new JProperty("updateType",               UpdateType.          AsText()),

                           LocalAuthorizationList.IsNeitherNullNorEmpty()
                               ? new JProperty("localAuthorizationList",   new JArray(LocalAuthorizationList.Select(item      => item.     ToJSON(CustomAuthorizationDataSerializer,
                                                                                                                                                  CustomIdTokenSerializer,
                                                                                                                                                  CustomAdditionalInfoSerializer,
                                                                                                                                                  CustomIdTokenInfoSerializer,
                                                                                                                                                  CustomMessageContentSerializer,
                                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",               new JArray(Signatures.            Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.          ToJSON(CustomCustomDataSerializer))
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
        /// Compares two SendLocalList requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A SendLocalList request.</param>
        /// <param name="SendLocalListRequest2">Another SendLocalList request.</param>
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
        /// Compares two SendLocalList requests for inequality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A SendLocalList request.</param>
        /// <param name="SendLocalListRequest2">Another SendLocalList request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListRequest? SendLocalListRequest1,
                                           SendLocalListRequest? SendLocalListRequest2)

            => !(SendLocalListRequest1 == SendLocalListRequest2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SendLocalList requests for equality.
        /// </summary>
        /// <param name="Object">A SendLocalList request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendLocalListRequest sendLocalListRequest &&
                   Equals(sendLocalListRequest);

        #endregion

        #region Equals(SendLocalListRequest)

        /// <summary>
        /// Compares two SendLocalList requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest">A SendLocalList request to compare with.</param>
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
                   UpdateType.AsText(),
                   " of ",
                   LocalAuthorizationList.Count(),
                   LocalAuthorizationList.Count(), " authorization list entries [version " + VersionNumber + "]"
               );

        #endregion

    }

}
