/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    // After start-up, every charging station SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// A boot notification request.
    /// </summary>
    public class BootNotificationRequest : ARequest<BootNotificationRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/bootNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// A physical system where an electrical vehicle (EV) can be charged.
        /// </summary>
        [Mandatory]
        public ChargingStation  ChargingStation    { get; }

        /// <summary>
        /// The the reason for sending this boot notification to the CSMS.
        /// </summary>
        [Mandatory]
        public BootReason       Reason             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new boot notification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="ChargingStation">A physical system where an electrical vehicle (EV) can be charged.</param>
        /// <param name="Reason">The the reason for sending this boot notification to the CSMS.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public BootNotificationRequest(SourceRouting            Destination,
                                       ChargingStation          ChargingStation,
                                       BootReason               Reason,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTime?                RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       NetworkPath?             NetworkPath           = null,
                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(BootNotificationRequest)[..^7],

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

            this.ChargingStation  = ChargingStation;
            this.Reason           = Reason;

            unchecked
            {
                hashCode = this.ChargingStation.GetHashCode() * 5 ^
                           this.Reason.         GetHashCode() * 3 ^
                           base.                GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:BootNotificationRequest",
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
        //     "BootReasonEnumType": {
        //       "description": "This contains the reason for sending this message to the CSMS.",
        //       "javaType": "BootReasonEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ApplicationReset",
        //         "FirmwareUpdate",
        //         "LocalReset",
        //         "PowerUp",
        //         "RemoteReset",
        //         "ScheduledReset",
        //         "Triggered",
        //         "Unknown",
        //         "Watchdog"
        //       ]
        //     },
        //     "ChargingStationType": {
        //       "description": "Charge_ Point\r\nurn:x-oca:ocpp:uid:2:233122\r\nThe physical system where an Electrical Vehicle (EV) can be charged.",
        //       "javaType": "ChargingStation",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "serialNumber": {
        //           "description": "Device. Serial_ Number. Serial_ Number\r\nurn:x-oca:ocpp:uid:1:569324\r\nVendor-specific device identifier.",
        //           "type": "string",
        //           "maxLength": 25
        //         },
        //         "model": {
        //           "description": "Device. Model. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569325\r\nDefines the model of the device.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "modem": {
        //           "$ref": "#/definitions/ModemType"
        //         },
        //         "vendorName": {
        //           "description": "Identifies the vendor (not necessarily in a unique manner).",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "firmwareVersion": {
        //           "description": "This contains the firmware version of the Charging Station.\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "model",
        //         "vendorName"
        //       ]
        //     },
        //     "ModemType": {
        //       "description": "Wireless_ Communication_ Module\r\nurn:x-oca:ocpp:uid:2:233306\r\nDefines parameters required for initiating and maintaining wireless communication with other devices.",
        //       "javaType": "Modem",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "iccid": {
        //           "description": "Wireless_ Communication_ Module. ICCID. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569327\r\nThis contains the ICCID of the modem’s SIM card.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "imsi": {
        //           "description": "Wireless_ Communication_ Module. IMSI. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569328\r\nThis contains the IMSI of the modem’s SIM card.",
        //           "type": "string",
        //           "maxLength": 20
        //         }
        //       }
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingStation": {
        //       "$ref": "#/definitions/ChargingStationType"
        //     },
        //     "reason": {
        //       "$ref": "#/definitions/BootReasonEnumType"
        //     }
        //   },
        //   "required": [
        //     "reason",
        //     "chargingStation"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON,   RequestId, SourceRouting, NetworkPath, CustomBootNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        /// <param name="CustomChargingStationParser">An optional delegate to parse custom charging stations.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static BootNotificationRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                      Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                                    CustomJObjectParserDelegate<ChargingStation>?          CustomChargingStationParser           = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {


            if (TryParse(JSON,
                         RequestId,
                     Destination,
                         NetworkPath,
                         out var bootNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomBootNotificationRequestParser,
                         CustomChargingStationParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a BootNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Binary, RequestId, SourceRouting, NetworkPath, CustomBootNotificationRequestParser = null)

        /// <summary>
        /// Parse the given binary representation of a BootNotification request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        /// <param name="CustomChargingStationParser">An optional delegate to parse custom charging stations.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static BootNotificationRequest Parse(Byte[]                                                 Binary,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                      Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                                    CustomJObjectParserDelegate<ChargingStation>?          CustomChargingStationParser           = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {


            if (TryParse(Binary,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var bootNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomBootNotificationRequestParser,
                         CustomChargingStationParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given binary representation of a BootNotification request is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(JSON,   RequestId, SourceRouting, NetworkPath, out BootNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        /// <param name="CustomChargingStationParser">An optional delegate to parse custom charging stations.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationRequest?      BootNotificationRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<ChargingStation>?          CustomChargingStationParser           = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                BootNotificationRequest = null;

                #region ChargingStation      [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingStation",
                                             "charging station",
                                             (JObject json, [NotNullWhen(true)]  out ChargingStation? chargingStation, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.ChargingStation.TryParse(json, out chargingStation, out errorResponse, CustomChargingStationParser, CustomCustomDataParser),
                                             out ChargingStation? ChargingStation,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Reason               [mandatory]

                if (!JSON.ParseMandatory("reason",
                                         "boot reason",
                                         BootReason.TryParse,
                                         out BootReason Reason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationRequest = new BootNotificationRequest(

                                          Destination,
                                              ChargingStation,
                                              Reason,

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

                if (CustomBootNotificationRequestParser is not null)
                    BootNotificationRequest = CustomBootNotificationRequestParser(JSON,
                                                                                  BootNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationRequest  = null;
                ErrorResponse            = "The given JSON representation of a BootNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Binary, RequestId, SourceRouting, NetworkPath, out BootNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given binary representation of a BootNotification request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        /// <param name="CustomChargingStationParser">An optional delegate to parse custom charging stations.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(Byte[]                                                 Binary,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationRequest?      BootNotificationRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<ChargingStation>?          CustomChargingStationParser           = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                BootNotificationRequest = null;

                var JSON = JObject.Parse(Binary.ToUTF8String());

                #region ChargingStation      [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingStation",
                                             "charging station",
                                             (JObject json, [NotNullWhen(true)]  out ChargingStation? chargingStation, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.ChargingStation.TryParse(json, out chargingStation, out errorResponse, CustomChargingStationParser, CustomCustomDataParser),
                                             out ChargingStation? ChargingStation,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Reason               [mandatory]

                if (!JSON.ParseMandatory("reason",
                                         "boot reason",
                                         BootReason.TryParse,
                                         out BootReason Reason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationRequest = new BootNotificationRequest(

                                          Destination,
                                              ChargingStation,
                                              Reason,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData,

                                              RequestId,
                                              RequestTimestamp,
                                              RequestTimeout,
                                              EventTrackingId,
                                              NetworkPath,

                                              SerializationFormats.JSON_UTF8_Binary

                                          );

                if (CustomBootNotificationRequestParser is not null)
                    BootNotificationRequest = CustomBootNotificationRequestParser(JSON,
                                                                                  BootNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationRequest  = null;
                ErrorResponse            = "The given JSON representation of a BootNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON  (CustomBootNotificationRequestSerializer = null, CustomChargingStationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationRequestSerializer">A delegate to serialize custom boot notification requests.</param>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStations.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingStation",   ChargingStation.ToJSON(CustomChargingStationSerializer)),
                                 new JProperty("reason",            Reason.         ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBootNotificationRequestSerializer is not null
                       ? CustomBootNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region ToBinary(CustomBootNotificationRequestSerializer = null, CustomChargingStationSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationRequestSerializer">A delegate to serialize custom boot notification requests.</param>
        /// <param name="CustomChargingStationSerializer"></param>
        /// <param name="CustomBinarySignatureSerializer"></param>
        /// <param name="IncludeSignatures"></param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
                               CustomBinarySerializerDelegate<ChargingStation>?          CustomChargingStationSerializer           = null,
                               CustomBinarySerializerDelegate<Signature>?                CustomBinarySignatureSerializer           = null,
                               Boolean                                                   IncludeSignatures                         = true)
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

            return CustomBootNotificationRequestSerializer is not null
                       ? CustomBootNotificationRequestSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationRequest? BootNotificationRequest1,
                                           BootNotificationRequest? BootNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationRequest1, BootNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationRequest1 is null || BootNotificationRequest2 is null)
                return false;

            return BootNotificationRequest1.Equals(BootNotificationRequest2);

        }

        #endregion

        #region Operator != (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for inequality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationRequest? BootNotificationRequest1,
                                           BootNotificationRequest? BootNotificationRequest2)

            => !(BootNotificationRequest1 == BootNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="Object">A boot notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationRequest bootNotificationRequest &&
                   Equals(bootNotificationRequest);

        #endregion

        #region Equals(BootNotificationRequest)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest">A boot notification request to compare with.</param>
        public override Boolean Equals(BootNotificationRequest? BootNotificationRequest)

            => BootNotificationRequest is not null &&

               ChargingStation.Equals(BootNotificationRequest.ChargingStation) &&
               Reason.         Equals(BootNotificationRequest.Reason)          &&

               base.    GenericEquals(BootNotificationRequest);

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

            => $"Boot reason: {Reason}";

        #endregion

    }

}
