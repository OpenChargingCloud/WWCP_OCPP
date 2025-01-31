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
    /// A BootNotification response.
    /// </summary>
    public class BootNotificationResponse : AResponse<BootNotificationRequest,
                                                      BootNotificationResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/bootNotificationResponse");

        /// <summary>
        /// The default heartbeat interval in seconds.
        /// </summary>
        public static TimeSpan DefaultInterval = TimeSpan.FromMinutes(5);

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The registration status.
        /// </summary>
        [Mandatory]
        public RegistrationStatus  Status        { get; }

        /// <summary>
        /// The current time at the CSMS as UTC.
        /// </summary>
        [Mandatory]
        public DateTime            CurrentTime   { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next BootNotification
        /// request.
        /// </summary>
        [Mandatory]
        public TimeSpan            Interval      { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BootNotification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the CSMS as UTC.</param>
        /// <param name="Interval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
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
        public BootNotificationResponse(BootNotificationRequest  Request,
                                        RegistrationStatus       Status,
                                        DateTime                 CurrentTime,
                                        TimeSpan                 Interval,
                                        StatusInfo?              StatusInfo            = null,

                                        Result?                  Result                = null,
                                        DateTime?                ResponseTimestamp     = null,

                                        SourceRouting?           Destination           = null,
                                        NetworkPath?             NetworkPath           = null,

                                        IEnumerable<KeyPair>?    SignKeys              = null,
                                        IEnumerable<SignInfo>?   SignInfos             = null,
                                        IEnumerable<Signature>?  Signatures            = null,

                                        CustomData?              CustomData            = null,

                                        SerializationFormats?    SerializationFormat   = null,
                                        CancellationToken        CancellationToken     = default)

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

            this.Status       = Status;
            this.CurrentTime  = CurrentTime;
            this.Interval     = Interval;
            this.StatusInfo   = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 11 ^
                           this.CurrentTime.GetHashCode()       *  7 ^
                           this.Interval.   GetHashCode()       *  5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) *  3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:BootNotificationResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "RegistrationStatusEnumType": {
        //             "description": "This contains whether the Charging Station has been registered\r\nwithin the CSMS.",
        //             "javaType": "RegistrationStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Pending",
        //                 "Rejected"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
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
        //         "currentTime": {
        //             "description": "This contains the CSMS\u2019s current time.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "interval": {
        //             "description": "When &lt;&lt;cmn_registrationstatusenumtype,Status&gt;&gt; is Accepted, this contains the heartbeat interval in seconds. If the CSMS returns something other than Accepted, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.",
        //             "type": "integer"
        //         },
        //         "status": {
        //             "$ref": "#/definitions/RegistrationStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "currentTime",
        //         "interval",
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON,   Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a BootNotification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static BootNotificationResponse Parse(BootNotificationRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                                     CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomBootNotificationResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a BootNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Request, Binary, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a BootNotification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static BootNotificationResponse Parse(BootNotificationRequest                                 Request,
                                                     Byte[]                                                  Binary,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                                     CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {


            if (TryParse(Request,
                         Binary,
                         Destination,
                         NetworkPath,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomBootNotificationResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationResponse;
            }

            throw new ArgumentException("The given binary representation of a BootNotification response is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Request, JSON,   Destination, NetworkPath, out BootNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationResponse">The parsed BootNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(BootNotificationRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationResponse?      BootNotificationResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                BootNotificationResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         OCPPv2_1.RegistrationStatus.TryParse,
                                         out RegistrationStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == RegistrationStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region CurrentTime    [mandatory]

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Interval       [mandatory]

                if (!JSON.ParseMandatory("interval",
                                         "heartbeat interval",
                                         out TimeSpan Interval,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              (JObject json, [NotNullWhen(true)] out Signature? signature, [NotNullWhen(false)] out String? errorResponse)
                                                  => Signature.TryParse(json, out signature, out errorResponse, CustomSignatureParser, CustomCustomDataParser),
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                => WWCP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationResponse = new BootNotificationResponse(

                                               Request,
                                               RegistrationStatus,
                                               CurrentTime,
                                               Interval,
                                               StatusInfo,

                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomBootNotificationResponseParser is not null)
                    BootNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                    BootNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given JSON representation of a BootNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, Binary, Destination, NetworkPath, out BootNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationResponse">The parsed BootNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(BootNotificationRequest                                 Request,
                                       Byte[]                                                  Binary,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationResponse?      BootNotificationResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                BootNotificationResponse = null;

                var JSON = JObject.Parse(Binary.ToUTF8String());

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         OCPPv2_1.RegistrationStatus.TryParse,
                                         out RegistrationStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == RegistrationStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region CurrentTime    [mandatory]

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Interval       [mandatory]

                if (!JSON.ParseMandatory("interval",
                                         "heartbeat interval",
                                         out TimeSpan Interval,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              (JObject json, [NotNullWhen(true)] out Signature? signature, [NotNullWhen(false)] out String? errorResponse)
                                                  => Signature.TryParse(json, out signature, out errorResponse, CustomSignatureParser, CustomCustomDataParser),
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                => WWCP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationResponse = new BootNotificationResponse(

                                               Request,
                                               RegistrationStatus,
                                               CurrentTime,
                                               Interval,
                                               StatusInfo,

                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomBootNotificationResponseParser is not null)
                    BootNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                    BootNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given JSON representation of a BootNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON   (CustomBootNotificationResponseSerializer = null,       CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationResponseSerializer">A delegate to serialize custom BootNotification responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<BootNotificationResponse>?  CustomBootNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                CustomStatusInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",        Status.              ToString()),
                                 new JProperty("currentTime",   CurrentTime.         ToIso8601()),
                                 new JProperty("interval",      (UInt32) Interval.TotalSeconds),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                            CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBootNotificationResponseSerializer is not null
                       ? CustomBootNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region ToBinary (CustomBinaryBootNotificationResponseSerializer = null, CustomBinaryStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBinaryBootNotificationResponseSerializer">A delegate to serialize custom BootNotification responses.</param>
        /// <param name="CustomBinaryStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomBinarySignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomBinaryCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        /// <param name="IncludeSignatures"></param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<BootNotificationResponse>?  CustomBinaryBootNotificationResponseSerializer   = null,
                               CustomBinarySerializerDelegate<StatusInfo>?                CustomBinaryStatusInfoSerializer                 = null,
                               CustomBinarySerializerDelegate<Signature>?                 CustomBinarySignatureSerializer                  = null,
                               CustomBinarySerializerDelegate<CustomData>?                CustomBinaryCustomDataSerializer                 = null,
                               Boolean                                                    IncludeSignatures                                = true)
        {

            var binaryStream = new MemoryStream();

            switch (SerializationFormat)
            {

                case SerializationFormats.JSON_UTF8_Binary:
                    var json = ToJSON();
                    if (!IncludeSignatures)
                        json.Remove("signatures");
                    binaryStream.Write(json.ToUTF8Bytes());
                    break;

            }

            var binary = binaryStream.ToArray();

            return CustomBinaryBootNotificationResponseSerializer is not null
                       ? CustomBinaryBootNotificationResponseSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The BootNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        public static BootNotificationResponse RequestError(BootNotificationRequest  Request,
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
                   RegistrationStatus.Rejected,
                   Timestamp.Now,
                   DefaultInterval,
                   null,
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
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BootNotificationResponse FormationViolation(BootNotificationRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    RegistrationStatus.Rejected,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BootNotificationResponse SignatureError(BootNotificationRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    RegistrationStatus.SignatureError,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static BootNotificationResponse Failed(BootNotificationRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    RegistrationStatus.Error,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static BootNotificationResponse ExceptionOccured(BootNotificationRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    RegistrationStatus.Error,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two BootNotification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A BootNotification response.</param>
        /// <param name="BootNotificationResponse2">Another BootNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationResponse1, BootNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationResponse1 is null || BootNotificationResponse2 is null)
                return false;

            return BootNotificationResponse1.Equals(BootNotificationResponse2);

        }

        #endregion

        #region Operator != (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two BootNotification responses for inequality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A BootNotification response.</param>
        /// <param name="BootNotificationResponse2">Another BootNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)

            => !(BootNotificationResponse1 == BootNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BootNotification responses for equality.
        /// </summary>
        /// <param name="Object">A BootNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(BootNotificationResponse)

        /// <summary>
        /// Compares two BootNotification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse">A BootNotification response to compare with.</param>
        public override Boolean Equals(BootNotificationResponse? BootNotificationResponse)

            => BootNotificationResponse is not null &&

               Status.     Equals(BootNotificationResponse.Status)      &&
               CurrentTime.Equals(BootNotificationResponse.CurrentTime) &&
               Interval.   Equals(BootNotificationResponse.Interval)    &&

             ((StatusInfo is     null && BootNotificationResponse.StatusInfo is     null) ||
              (StatusInfo is not null && BootNotificationResponse.StatusInfo is not null && StatusInfo.Equals(BootNotificationResponse.StatusInfo))) &&

               base.GenericEquals(BootNotificationResponse);

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

                   $"'{Status}', current time: {CurrentTime}, ",

                   Status == RegistrationStatus.Accepted
                       ? $"heartbeat interval: {Interval.TotalSeconds} seconds"
                       : $"reconnect in: {Interval.TotalSeconds} seconds"

               );

        #endregion

    }

}
