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

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    /// <summary>
    /// The set network profile request.
    /// </summary>
    public class SetNetworkProfileRequest : ARequest<SetNetworkProfileRequest>
    {

        #region Properties

        /// <summary>
        /// The slot in which the configuration should be stored.
        /// </summary>
        public Int32                     ConfigurationSlot           { get; }

        /// <summary>
        /// The network connection configuration.
        /// </summary>
        [Mandatory]
        public NetworkConnectionProfile  NetworkConnectionProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set network profile request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConfigurationSlot">The slot in which the configuration should be stored.</param>
        /// <param name="NetworkConnectionProfile">The network connection configuration.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetNetworkProfileRequest(ChargeBox_Id              ChargeBoxId,
                                        Int32                     ConfigurationSlot,
                                        NetworkConnectionProfile  NetworkConnectionProfile,
                                        CustomData?               CustomData          = null,

                                        Request_Id?               RequestId           = null,
                                        DateTime?                 RequestTimestamp    = null,
                                        TimeSpan?                 RequestTimeout      = null,
                                        EventTracking_Id?         EventTrackingId     = null,
                                        CancellationToken?        CancellationToken   = null)

            : base(ChargeBoxId,
                   "SetNetworkProfile",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ConfigurationSlot         = ConfigurationSlot;
            this.NetworkConnectionProfile  = NetworkConnectionProfile;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetNetworkProfileRequest",
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
        //     "APNAuthenticationEnumType": {
        //       "description": "APN. APN_ Authentication. APN_ Authentication_ Code\r\nurn:x-oca:ocpp:uid:1:568828\r\nAuthentication method.\r\n",
        //       "javaType": "APNAuthenticationEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "CHAP",
        //         "NONE",
        //         "PAP",
        //         "AUTO"
        //       ]
        //     },
        //     "OCPPInterfaceEnumType": {
        //       "description": "Applicable Network Interface.\r\n",
        //       "javaType": "OCPPInterfaceEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Wired0",
        //         "Wired1",
        //         "Wired2",
        //         "Wired3",
        //         "Wireless0",
        //         "Wireless1",
        //         "Wireless2",
        //         "Wireless3"
        //       ]
        //     },
        //     "OCPPTransportEnumType": {
        //       "description": "Communication_ Function. OCPP_ Transport. OCPP_ Transport_ Code\r\nurn:x-oca:ocpp:uid:1:569356\r\nDefines the transport protocol (e.g. SOAP or JSON). Note: SOAP is not supported in OCPP 2.0, but is supported by other versions of OCPP.\r\n",
        //       "javaType": "OCPPTransportEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "JSON",
        //         "SOAP"
        //       ]
        //     },
        //     "OCPPVersionEnumType": {
        //       "description": "Communication_ Function. OCPP_ Version. OCPP_ Version_ Code\r\nurn:x-oca:ocpp:uid:1:569355\r\nDefines the OCPP version used for this communication function.\r\n",
        //       "javaType": "OCPPVersionEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "OCPP12",
        //         "OCPP15",
        //         "OCPP16",
        //         "OCPP20"
        //       ]
        //     },
        //     "VPNEnumType": {
        //       "description": "VPN. Type. VPN_ Code\r\nurn:x-oca:ocpp:uid:1:569277\r\nType of VPN\r\n",
        //       "javaType": "VPNEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "IKEv2",
        //         "IPSec",
        //         "L2TP",
        //         "PPTP"
        //       ]
        //     },
        //     "APNType": {
        //       "description": "APN\r\nurn:x-oca:ocpp:uid:2:233134\r\nCollection of configuration data needed to make a data-connection over a cellular network.\r\n\r\nNOTE: When asking a GSM modem to dial in, it is possible to specify which mobile operator should be used. This can be done with the mobile country code (MCC) in combination with a mobile network code (MNC). Example: If your preferred network is Vodafone Netherlands, the MCC=204 and the MNC=04 which means the key PreferredNetwork = 20404 Some modems allows to specify a preferred network, which means, if this network is not available, a different network is used. If you specify UseOnlyPreferredNetwork and this network is not available, the modem will not dial in.\r\n",
        //       "javaType": "APN",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "apn": {
        //           "description": "APN. APN. URI\r\nurn:x-oca:ocpp:uid:1:568814\r\nThe Access Point Name as an URL.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "apnUserName": {
        //           "description": "APN. APN. User_ Name\r\nurn:x-oca:ocpp:uid:1:568818\r\nAPN username.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "apnPassword": {
        //           "description": "APN. APN. Password\r\nurn:x-oca:ocpp:uid:1:568819\r\nAPN Password.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "simPin": {
        //           "description": "APN. SIMPIN. PIN_ Code\r\nurn:x-oca:ocpp:uid:1:568821\r\nSIM card pin code.\r\n",
        //           "type": "integer"
        //         },
        //         "preferredNetwork": {
        //           "description": "APN. Preferred_ Network. Mobile_ Network_ ID\r\nurn:x-oca:ocpp:uid:1:568822\r\nPreferred network, written as MCC and MNC concatenated. See note.\r\n",
        //           "type": "string",
        //           "maxLength": 6
        //         },
        //         "useOnlyPreferredNetwork": {
        //           "description": "APN. Use_ Only_ Preferred_ Network. Indicator\r\nurn:x-oca:ocpp:uid:1:568824\r\nDefault: false. Use only the preferred Network, do\r\nnot dial in when not available. See Note.\r\n",
        //           "type": "boolean",
        //           "default": false
        //         },
        //         "apnAuthentication": {
        //           "$ref": "#/definitions/APNAuthenticationEnumType"
        //         }
        //       },
        //       "required": [
        //         "apn",
        //         "apnAuthentication"
        //       ]
        //     },
        //     "NetworkConnectionProfileType": {
        //       "description": "Communication_ Function\r\nurn:x-oca:ocpp:uid:2:233304\r\nThe NetworkConnectionProfile defines the functional and technical parameters of a communication link.\r\n",
        //       "javaType": "NetworkConnectionProfile",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "apn": {
        //           "$ref": "#/definitions/APNType"
        //         },
        //         "ocppVersion": {
        //           "$ref": "#/definitions/OCPPVersionEnumType"
        //         },
        //         "ocppTransport": {
        //           "$ref": "#/definitions/OCPPTransportEnumType"
        //         },
        //         "ocppCsmsUrl": {
        //           "description": "Communication_ Function. OCPP_ Central_ System_ URL. URI\r\nurn:x-oca:ocpp:uid:1:569357\r\nURL of the CSMS(s) that this Charging Station  communicates with.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "messageTimeout": {
        //           "description": "Duration in seconds before a message send by the Charging Station via this network connection times-out.\r\nThe best setting depends on the underlying network and response times of the CSMS.\r\nIf you are looking for a some guideline: use 30 seconds as a starting point.\r\n",
        //           "type": "integer"
        //         },
        //         "securityProfile": {
        //           "description": "This field specifies the security profile used when connecting to the CSMS with this NetworkConnectionProfile.\r\n",
        //           "type": "integer"
        //         },
        //         "ocppInterface": {
        //           "$ref": "#/definitions/OCPPInterfaceEnumType"
        //         },
        //         "vpn": {
        //           "$ref": "#/definitions/VPNType"
        //         }
        //       },
        //       "required": [
        //         "ocppVersion",
        //         "ocppTransport",
        //         "ocppCsmsUrl",
        //         "messageTimeout",
        //         "securityProfile",
        //         "ocppInterface"
        //       ]
        //     },
        //     "VPNType": {
        //       "description": "VPN\r\nurn:x-oca:ocpp:uid:2:233268\r\nVPN Configuration settings\r\n",
        //       "javaType": "VPN",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "server": {
        //           "description": "VPN. Server. URI\r\nurn:x-oca:ocpp:uid:1:569272\r\nVPN Server Address\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "user": {
        //           "description": "VPN. User. User_ Name\r\nurn:x-oca:ocpp:uid:1:569273\r\nVPN User\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "group": {
        //           "description": "VPN. Group. Group_ Name\r\nurn:x-oca:ocpp:uid:1:569274\r\nVPN group.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "password": {
        //           "description": "VPN. Password. Password\r\nurn:x-oca:ocpp:uid:1:569275\r\nVPN Password.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "key": {
        //           "description": "VPN. Key. VPN_ Key\r\nurn:x-oca:ocpp:uid:1:569276\r\nVPN shared secret.\r\n",
        //           "type": "string",
        //           "maxLength": 255
        //         },
        //         "type": {
        //           "$ref": "#/definitions/VPNEnumType"
        //         }
        //       },
        //       "required": [
        //         "server",
        //         "user",
        //         "password",
        //         "key",
        //         "type"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "configurationSlot": {
        //       "description": "Slot in which the configuration should be stored.\r\n",
        //       "type": "integer"
        //     },
        //     "connectionData": {
        //       "$ref": "#/definitions/NetworkConnectionProfileType"
        //     }
        //   },
        //   "required": [
        //     "configurationSlot",
        //     "connectionData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSetNetworkProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set network profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSetNetworkProfileRequestParser">A delegate to parse custom set network profile requests.</param>
        public static SetNetworkProfileRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     ChargeBox_Id                                            ChargeBoxId,
                                                     CustomJObjectParserDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var setNetworkProfileRequest,
                         out var errorResponse,
                         CustomSetNetworkProfileRequestParser))
            {
                return setNetworkProfileRequest!;
            }

            throw new ArgumentException("The given JSON representation of a set network profile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SetNetworkProfileRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a set network profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetNetworkProfileRequest">The parsed set network profile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
                                       out SetNetworkProfileRequest?  SetNetworkProfileRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SetNetworkProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a set network profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetNetworkProfileRequest">The parsed set network profile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetNetworkProfileRequestParser">A delegate to parse custom set network profile requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       ChargeBox_Id                                            ChargeBoxId,
                                       out SetNetworkProfileRequest?                           SetNetworkProfileRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestParser)
        {

            try
            {

                SetNetworkProfileRequest = null;

                #region ConfigurationSlot           [mandatory]

                if (!JSON.ParseMandatory("configurationSlot",
                                         "configuration slot",
                                         out Int32 ConfigurationSlot,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NetworkConnectionProfile    [mandatory]

                if (!JSON.ParseMandatoryJSON("connectionData",
                                             "connection data",
                                             OCPPv2_0.NetworkConnectionProfile.TryParse,
                                             out NetworkConnectionProfile? NetworkConnectionProfile,
                                             out ErrorResponse) ||
                     NetworkConnectionProfile is null)
                {
                    return false;
                }

                #endregion

                #region CustomData                  [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                 [optional, OCPP_CSE]

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


                SetNetworkProfileRequest = new SetNetworkProfileRequest(ChargeBoxId,
                                                                        ConfigurationSlot,
                                                                        NetworkConnectionProfile,
                                                                        CustomData,
                                                                        RequestId);

                if (CustomSetNetworkProfileRequestParser is not null)
                    SetNetworkProfileRequest = CustomSetNetworkProfileRequestParser(JSON,
                                                                                    SetNetworkProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                SetNetworkProfileRequest  = null;
                ErrorResponse             = "The given JSON representation of a set network profile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetNetworkProfileRequestSerializer = null, CustomNetworkConnectionProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetNetworkProfileRequestSerializer">A delegate to serialize custom set network profile requests.</param>
        /// <param name="CustomNetworkConnectionProfileSerializer">A delegate to serialize custom network connection profiles.</param>
        /// <param name="CustomVPNConfigurationSerializer">A delegate to serialize custom VPN configurations.</param>
        /// <param name="CustomAPNConfigurationSerializer">A delegate to serialize custom APN configurations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<VPNConfiguration>?          CustomVPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<APNConfiguration>?          CustomAPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("configurationSlot",  ConfigurationSlot),

                                 new JProperty("connectionData",     NetworkConnectionProfile.ToJSON(CustomNetworkConnectionProfileSerializer,
                                                                                                     CustomVPNConfigurationSerializer,
                                                                                                     CustomAPNConfigurationSerializer,
                                                                                                     CustomCustomDataSerializer)),

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.              ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetNetworkProfileRequestSerializer is not null
                       ? CustomSetNetworkProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetNetworkProfileRequest1, SetNetworkProfileRequest2)

        /// <summary>
        /// Compares two set network profile requests for equality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest1">A set network profile request.</param>
        /// <param name="SetNetworkProfileRequest2">Another set network profile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetNetworkProfileRequest? SetNetworkProfileRequest1,
                                           SetNetworkProfileRequest? SetNetworkProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetNetworkProfileRequest1, SetNetworkProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetNetworkProfileRequest1 is null || SetNetworkProfileRequest2 is null)
                return false;

            return SetNetworkProfileRequest1.Equals(SetNetworkProfileRequest2);

        }

        #endregion

        #region Operator != (SetNetworkProfileRequest1, SetNetworkProfileRequest2)

        /// <summary>
        /// Compares two set network profile requests for inequality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest1">A set network profile request.</param>
        /// <param name="SetNetworkProfileRequest2">Another set network profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetNetworkProfileRequest? SetNetworkProfileRequest1,
                                           SetNetworkProfileRequest? SetNetworkProfileRequest2)

            => !(SetNetworkProfileRequest1 == SetNetworkProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetNetworkProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set network profile requests for equality.
        /// </summary>
        /// <param name="Object">A set network profile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetNetworkProfileRequest setNetworkProfileRequest &&
                   Equals(setNetworkProfileRequest);

        #endregion

        #region Equals(SetNetworkProfileRequest)

        /// <summary>
        /// Compares two set network profile requests for equality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest">A set network profile request to compare with.</param>
        public override Boolean Equals(SetNetworkProfileRequest? SetNetworkProfileRequest)

            => SetNetworkProfileRequest is not null &&

               ConfigurationSlot.       Equals(SetNetworkProfileRequest.ConfigurationSlot)        &&
               NetworkConnectionProfile.Equals(SetNetworkProfileRequest.NetworkConnectionProfile) &&

               base.             GenericEquals(SetNetworkProfileRequest);

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

                return ConfigurationSlot.       GetHashCode() * 5 ^
                       NetworkConnectionProfile.GetHashCode() * 3 ^
                       base.                    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   ConfigurationSlot,
                   ": ",
                   NetworkConnectionProfile

               );

        #endregion

    }

}
