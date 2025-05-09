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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The BatterySwap request.
    /// </summary>
    public class BatterySwapRequest : ARequest<BatterySwapRequest>,
                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/aFRRSignalRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The value of the BatterySwap in v2xSignalWattCurve. Usually between -1 and 1.
        /// </summary>
        [Mandatory]
        public Int32                     BatterySwapRequestId    { get; }

        /// <summary>
        /// The battery swap event type.
        /// </summary>
        [Mandatory]
        public BatterySwapEvent          EventType               { get; }

        /// <summary>
        /// The identification token of the user that requested the battery swap.
        /// </summary>
        [Mandatory]
        public IdToken                   IdToken                 { get; }

        /// <summary>
        /// The enumeration of battery swap data.
        /// </summary>
        [Mandatory]
        public IEnumerable<BatteryData>  BatteryData             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
        /// <param name="Signal">The value of the BatterySwap in v2xSignalWattCurve. Usually between -1 and 1.</param>
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
        public BatterySwapRequest(SourceRouting             Destination,
                                  Int32                     BatterySwapRequestId,
                                  BatterySwapEvent          EventType,
                                  IdToken                   IdToken,
                                  IEnumerable<BatteryData>  BatteryData,

                                  IEnumerable<KeyPair>?     SignKeys              = null,
                                  IEnumerable<SignInfo>?    SignInfos             = null,
                                  IEnumerable<Signature>?   Signatures            = null,

                                  CustomData?               CustomData            = null,

                                  Request_Id?               RequestId             = null,
                                  DateTime?                 RequestTimestamp      = null,
                                  TimeSpan?                 RequestTimeout        = null,
                                  EventTracking_Id?         EventTrackingId       = null,
                                  NetworkPath?              NetworkPath           = null,
                                  SerializationFormats?     SerializationFormat   = null,
                                  CancellationToken         CancellationToken     = default)

            : base(Destination,
                   nameof(BatterySwapRequest)[..^7],

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

            this.BatterySwapRequestId  = BatterySwapRequestId;
            this.EventType             = EventType;
            this.IdToken               = IdToken;
            this.BatteryData           = BatteryData;

            unchecked
            {

                hashCode = this.BatterySwapRequestId.GetHashCode() * 11 ^
                           this.EventType.           GetHashCode() *  7 ^
                           this.IdToken.             GetHashCode() *  5 ^
                           this.BatteryData.         GetHashCode() *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:BatterySwapRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "BatterySwapEventEnumType": {
        //             "description": "Battery in/out",
        //             "javaType": "BatterySwapEventEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "BatteryIn",
        //                 "BatteryOut",
        //                 "BatteryOutTimeout"
        //             ]
        //         },
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "This field specifies the additional IdToken.",
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
        //         "BatteryDataType": {
        //             "javaType": "BatteryData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evseId": {
        //                     "description": "Slot number where battery is inserted or removed.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "serialNumber": {
        //                     "description": "Serial number of battery.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "soC": {
        //                     "description": "State of charge",
        //                     "type": "number",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "soH": {
        //                     "description": "State of health",
        //                     "type": "number",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "productionDate": {
        //                     "description": "Production date of battery.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "vendorInfo": {
        //                     "description": "Vendor-specific info from battery in undefined format.",
        //                     "type": "string",
        //                     "maxLength": 500
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "evseId",
        //                 "serialNumber",
        //                 "soC",
        //                 "soH"
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
        //                     "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
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
        //         "batteryData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/BatteryDataType"
        //             },
        //             "minItems": 1
        //         },
        //         "eventType": {
        //             "$ref": "#/definitions/BatterySwapEventEnumType"
        //         },
        //         "idToken": {
        //             "$ref": "#/definitions/IdTokenType"
        //         },
        //         "requestId": {
        //             "description": "RequestId to correlate BatteryIn/Out events and optional RequestBatterySwapRequest.",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "eventType",
        //         "requestId",
        //         "idToken",
        //         "batteryData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a BatterySwap request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBatterySwapRequestParser">A delegate to parse custom BatterySwap requests.</param>
        public static BatterySwapRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         RequestTimestamp                 = null,
                                               TimeSpan?                                         RequestTimeout                   = null,
                                               EventTracking_Id?                                 EventTrackingId                  = null,
                                               CustomJObjectParserDelegate<BatterySwapRequest>?  CustomBatterySwapRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var batterySwapRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomBatterySwapRequestParser))
            {
                return batterySwapRequest;
            }

            throw new ArgumentException("The given JSON representation of a BatterySwap request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out BatterySwapRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a BatterySwap request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BatterySwapRequest">The parsed BatterySwap request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomBatterySwapRequestParser">A delegate to parse custom BatterySwap requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out BatterySwapRequest?      BatterySwapRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         RequestTimestamp                 = null,
                                       TimeSpan?                                         RequestTimeout                   = null,
                                       EventTracking_Id?                                 EventTrackingId                  = null,
                                       CustomJObjectParserDelegate<BatterySwapRequest>?  CustomBatterySwapRequestParser   = null)
        {

            try
            {

                BatterySwapRequest = null;

                #region BatterySwapRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "battery swap request identification",
                                         out Int32 BatterySwapRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EventType               [mandatory]

                if (!JSON.ParseMandatory("eventType",
                                         "event type",
                                         BatterySwapEvent.TryParse,
                                         out BatterySwapEvent EventType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdToken                 [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region BatteryData             [mandatory]

                if (!JSON.ParseMandatoryHashSet("batteryData",
                                                "battery data",
                                                OCPPv2_1.BatteryData.TryParse,
                                                out HashSet<BatteryData> BatteryData,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures              [optional, OCPP_CSE]

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

                #region CustomData              [optional]

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


                BatterySwapRequest = new BatterySwapRequest(

                                         Destination,

                                         BatterySwapRequestId,
                                         EventType,
                                         IdToken,
                                         BatteryData,

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

                if (CustomBatterySwapRequestParser is not null)
                    BatterySwapRequest = CustomBatterySwapRequestParser(JSON,
                                                                        BatterySwapRequest);

                return true;

            }
            catch (Exception e)
            {
                BatterySwapRequest  = null;
                ErrorResponse       = "The given JSON representation of a BatterySwap request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBatterySwapRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBatterySwapRequestSerializer">A delegate to serialize custom BatterySwap requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomBatteryDataSerializer">A delegate to serialize custom battery data.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                               IncludeJSONLDContext                 = false,
                              CustomJObjectSerializerDelegate<BatterySwapRequest>?  CustomBatterySwapRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?             CustomIdTokenSerializer              = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?      CustomAdditionalInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<BatteryData>?         CustomBatteryDataSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",    BatterySwapRequestId),

                                 new JProperty("eventType",    EventType.           ToString()),

                                 new JProperty("idToken",      IdToken.             ToJSON(CustomIdTokenSerializer,
                                                                                           CustomAdditionalInfoSerializer,
                                                                                           CustomCustomDataSerializer)),

                                 new JProperty("batteryData",  new JArray(BatteryData.Select(batteryData => batteryData.ToJSON(CustomBatteryDataSerializer,
                                                                                                                               CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures. Select(signature   => signature.  ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBatterySwapRequestSerializer is not null
                       ? CustomBatterySwapRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BatterySwapRequest1, BatterySwapRequest2)

        /// <summary>
        /// Compares two BatterySwap requests for equality.
        /// </summary>
        /// <param name="BatterySwapRequest1">A BatterySwap request.</param>
        /// <param name="BatterySwapRequest2">Another BatterySwap request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BatterySwapRequest? BatterySwapRequest1,
                                           BatterySwapRequest? BatterySwapRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BatterySwapRequest1, BatterySwapRequest2))
                return true;

            // If one is null, but not both, return false.
            if (BatterySwapRequest1 is null || BatterySwapRequest2 is null)
                return false;

            return BatterySwapRequest1.Equals(BatterySwapRequest2);

        }

        #endregion

        #region Operator != (BatterySwapRequest1, BatterySwapRequest2)

        /// <summary>
        /// Compares two BatterySwap requests for inequality.
        /// </summary>
        /// <param name="BatterySwapRequest1">A BatterySwap request.</param>
        /// <param name="BatterySwapRequest2">Another BatterySwap request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BatterySwapRequest? BatterySwapRequest1,
                                           BatterySwapRequest? BatterySwapRequest2)

            => !(BatterySwapRequest1 == BatterySwapRequest2);

        #endregion

        #endregion

        #region IEquatable<BatterySwapRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BatterySwap requests for equality.
        /// </summary>
        /// <param name="Object">A BatterySwap request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BatterySwapRequest batterySwapRequest &&
                   Equals(batterySwapRequest);

        #endregion

        #region Equals(BatterySwapRequest)

        /// <summary>
        /// Compares two BatterySwap requests for equality.
        /// </summary>
        /// <param name="BatterySwapRequest">A BatterySwap request to compare with.</param>
        public override Boolean Equals(BatterySwapRequest? BatterySwapRequest)

            => BatterySwapRequest is not null &&

               BatterySwapRequestId.   Equals   (BatterySwapRequest.BatterySwapRequestId) &&
               EventType.              Equals   (BatterySwapRequest.EventType)            &&
               IdToken.                Equals   (BatterySwapRequest.IdToken)              &&
               BatteryData.ToHashSet().SetEquals(BatterySwapRequest.BatteryData)          &&

               base.               GenericEquals(BatterySwapRequest);

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

            => $"'{EventType}' ({BatterySwapRequestId}) for '{IdToken}'";

        #endregion

    }

}
