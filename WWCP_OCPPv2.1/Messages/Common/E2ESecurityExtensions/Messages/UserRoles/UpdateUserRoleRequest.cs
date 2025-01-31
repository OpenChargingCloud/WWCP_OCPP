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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The UpdateUserRole request.
    /// </summary>
    public class UpdateUserRoleRequest : ARequest<UpdateUserRoleRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/updateUserRoleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateUserRole request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingStation">A physical system where an electrical vehicle (EV) can be charged.</param>
        /// <param name="Reason">The the reason for sending this UpdateUserRole to the CSMS.</param>
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
        public UpdateUserRoleRequest(SourceRouting            Destination,

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
                   nameof(UpdateUserRoleRequest)[..^7],

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


            unchecked
            {

                hashCode =  base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UpdateUserRoleRequest",
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
        //           "description": "This contains the firmware version of the Charging Station.",
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

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomUpdateUserRoleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UpdateUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUpdateUserRoleRequestParser">An optional delegate to parse custom UpdateUserRole requests.</param>
        public static UpdateUserRoleRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  SourceRouting                                    Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            RequestTimestamp                    = null,
                                                  TimeSpan?                                            RequestTimeout                      = null,
                                                  EventTracking_Id?                                    EventTrackingId                     = null,
                                                  CustomJObjectParserDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var updateUserRoleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomUpdateUserRoleRequestParser))
            {
                return updateUserRoleRequest;
            }

            throw new ArgumentException("The given JSON representation of an UpdateUserRole request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out UpdateUserRoleRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UpdateUserRoleRequest">The parsed UpdateUserRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUpdateUserRoleRequestParser">An optional delegate to parse custom UpdateUserRole requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out UpdateUserRoleRequest?      UpdateUserRoleRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            RequestTimestamp                    = null,
                                       TimeSpan?                                            RequestTimeout                      = null,
                                       EventTracking_Id?                                    EventTrackingId                     = null,
                                       CustomJObjectParserDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestParser   = null)
        {

            try
            {

                UpdateUserRoleRequest = null;

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UpdateUserRoleRequest = new UpdateUserRoleRequest(

                                            Destination,


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

                if (CustomUpdateUserRoleRequestParser is not null)
                    UpdateUserRoleRequest = CustomUpdateUserRoleRequestParser(JSON,
                                                                              UpdateUserRoleRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateUserRoleRequest  = null;
                ErrorResponse          = "The given JSON representation of an UpdateUserRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateUserRoleRequestSerializer = null, CustomChargingStationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateUserRoleRequestSerializer">A delegate to serialize custom UpdateUserRole requests.</param>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStations.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(


                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateUserRoleRequestSerializer is not null
                       ? CustomUpdateUserRoleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateUserRoleRequest1, UpdateUserRoleRequest2)

        /// <summary>
        /// Compares two UpdateUserRole requests for equality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest1">A UpdateUserRole request.</param>
        /// <param name="UpdateUserRoleRequest2">Another UpdateUserRole request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateUserRoleRequest? UpdateUserRoleRequest1,
                                           UpdateUserRoleRequest? UpdateUserRoleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateUserRoleRequest1, UpdateUserRoleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateUserRoleRequest1 is null || UpdateUserRoleRequest2 is null)
                return false;

            return UpdateUserRoleRequest1.Equals(UpdateUserRoleRequest2);

        }

        #endregion

        #region Operator != (UpdateUserRoleRequest1, UpdateUserRoleRequest2)

        /// <summary>
        /// Compares two UpdateUserRole requests for inequality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest1">A UpdateUserRole request.</param>
        /// <param name="UpdateUserRoleRequest2">Another UpdateUserRole request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateUserRoleRequest? UpdateUserRoleRequest1,
                                           UpdateUserRoleRequest? UpdateUserRoleRequest2)

            => !(UpdateUserRoleRequest1 == UpdateUserRoleRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateUserRoleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateUserRole requests for equality.
        /// </summary>
        /// <param name="Object">A UpdateUserRole request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateUserRoleRequest updateUserRoleRequest &&
                   Equals(updateUserRoleRequest);

        #endregion

        #region Equals(UpdateUserRoleRequest)

        /// <summary>
        /// Compares two UpdateUserRole requests for equality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest">A UpdateUserRole request to compare with.</param>
        public override Boolean Equals(UpdateUserRoleRequest? UpdateUserRoleRequest)

            => UpdateUserRoleRequest is not null &&

               
               base.    GenericEquals(UpdateUserRoleRequest);

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

            => $"Boot reason: ";

        #endregion

    }

}
