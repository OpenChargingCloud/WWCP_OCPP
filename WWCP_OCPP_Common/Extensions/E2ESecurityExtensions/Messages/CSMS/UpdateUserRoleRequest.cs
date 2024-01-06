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

#endregion

namespace cloud.charging.open.protocols.OCPP.CSMS
{

    // After start-up, every charging station SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// A boot notification request.
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
        /// Create a new boot notification request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
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
        public UpdateUserRoleRequest(NetworkingNode_Id        NetworkingNodeId,

                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                     IEnumerable<Signature>?  Signatures          = null,

                                     CustomData?              CustomData          = null,

                                     Request_Id?              RequestId           = null,
                                     DateTime?                RequestTimestamp    = null,
                                     TimeSpan?                RequestTimeout      = null,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     NetworkPath?             NetworkPath         = null,
                                     CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
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
        //       "description": "This contains the reason for sending this message to the CSMS.\r\n",
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
        //       "description": "Charge_ Point\r\nurn:x-oca:ocpp:uid:2:233122\r\nThe physical system where an Electrical Vehicle (EV) can be charged.\r\n",
        //       "javaType": "ChargingStation",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "serialNumber": {
        //           "description": "Device. Serial_ Number. Serial_ Number\r\nurn:x-oca:ocpp:uid:1:569324\r\nVendor-specific device identifier.\r\n",
        //           "type": "string",
        //           "maxLength": 25
        //         },
        //         "model": {
        //           "description": "Device. Model. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569325\r\nDefines the model of the device.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "modem": {
        //           "$ref": "#/definitions/ModemType"
        //         },
        //         "vendorName": {
        //           "description": "Identifies the vendor (not necessarily in a unique manner).\r\n",
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
        //       "description": "Wireless_ Communication_ Module\r\nurn:x-oca:ocpp:uid:2:233306\r\nDefines parameters required for initiating and maintaining wireless communication with other devices.\r\n",
        //       "javaType": "Modem",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "iccid": {
        //           "description": "Wireless_ Communication_ Module. ICCID. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569327\r\nThis contains the ICCID of the modem’s SIM card.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "imsi": {
        //           "description": "Wireless_ Communication_ Module. IMSI. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569328\r\nThis contains the IMSI of the modem’s SIM card.\r\n",
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

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomUpdateUserRoleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UpdateUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomUpdateUserRoleRequestParser">A delegate to parse custom boot notification requests.</param>
        public static UpdateUserRoleRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  NetworkingNode_Id                                    NetworkingNodeId,
                                                  NetworkPath                                          NetworkPath,
                                                  CustomJObjectParserDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var updateUserRoleRequest,
                         out var errorResponse,
                         CustomUpdateUserRoleRequestParser))
            {
                return updateUserRoleRequest;
            }

            throw new ArgumentException("The given JSON representation of an UpdateUserRole request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out UpdateUserRoleRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UpdateUserRoleRequest">The parsed boot notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateUserRoleRequestParser">A delegate to parse custom boot notification requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       NetworkingNode_Id                                    NetworkingNodeId,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out UpdateUserRoleRequest?      UpdateUserRoleRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestParser)
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
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UpdateUserRoleRequest = new UpdateUserRoleRequest(

                                            NetworkingNodeId,


                                            null,
                                            null,
                                            Signatures,

                                            CustomData,

                                            RequestId,
                                            null,
                                            null,
                                            null,
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
        /// <param name="CustomUpdateUserRoleRequestSerializer">A delegate to serialize custom boot notification requests.</param>
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
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest1">A boot notification request.</param>
        /// <param name="UpdateUserRoleRequest2">Another boot notification request.</param>
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
        /// Compares two boot notification requests for inequality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest1">A boot notification request.</param>
        /// <param name="UpdateUserRoleRequest2">Another boot notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateUserRoleRequest? UpdateUserRoleRequest1,
                                           UpdateUserRoleRequest? UpdateUserRoleRequest2)

            => !(UpdateUserRoleRequest1 == UpdateUserRoleRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateUserRoleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="Object">A boot notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateUserRoleRequest updateUserRoleRequest &&
                   Equals(updateUserRoleRequest);

        #endregion

        #region Equals(UpdateUserRoleRequest)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="UpdateUserRoleRequest">A boot notification request to compare with.</param>
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
