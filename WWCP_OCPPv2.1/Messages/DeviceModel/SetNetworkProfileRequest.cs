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
    /// The SetNetworkProfile request.
    /// </summary>
    public class SetNetworkProfileRequest : ARequest<SetNetworkProfileRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setNetworkProfileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

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
        /// Create a new SetNetworkProfile request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConfigurationSlot">The slot in which the configuration should be stored.</param>
        /// <param name="NetworkConnectionProfile">The network connection configuration.</param>
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
        public SetNetworkProfileRequest(SourceRouting             Destination,
                                        Int32                     ConfigurationSlot,
                                        NetworkConnectionProfile  NetworkConnectionProfile,

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
                   nameof(SetNetworkProfileRequest)[..^7],

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

            this.ConfigurationSlot         = ConfigurationSlot;
            this.NetworkConnectionProfile  = NetworkConnectionProfile;

            unchecked
            {

                hashCode = this.ConfigurationSlot.       GetHashCode() * 3 ^
                           this.NetworkConnectionProfile.GetHashCode() * 3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetNetworkProfileRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "APNAuthenticationEnumType": {
        //             "description": "Authentication method.",
        //             "javaType": "APNAuthenticationEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "PAP",
        //                 "CHAP",
        //                 "NONE",
        //                 "AUTO"
        //             ]
        //         },
        //         "OCPPInterfaceEnumType": {
        //             "description": "Applicable Network Interface. Charging Station is allowed to use a different network interface to connect if the given one does not work.",
        //             "javaType": "OCPPInterfaceEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Wired0",
        //                 "Wired1",
        //                 "Wired2",
        //                 "Wired3",
        //                 "Wireless0",
        //                 "Wireless1",
        //                 "Wireless2",
        //                 "Wireless3",
        //                 "Any"
        //             ]
        //         },
        //         "OCPPTransportEnumType": {
        //             "description": "Defines the transport protocol (e.g. SOAP or JSON). Note: SOAP is not supported in OCPP 2.x, but is supported by earlier versions of OCPP.",
        //             "javaType": "OCPPTransportEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "SOAP",
        //                 "JSON"
        //             ]
        //         },
        //         "OCPPVersionEnumType": {
        //             "description": "*(2.1)* This field is ignored, since the OCPP version to use is determined during the websocket handshake. The field is only kept for backwards compatibility with the OCPP 2.0.1 JSON schema.",
        //             "javaType": "OCPPVersionEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "OCPP12",
        //                 "OCPP15",
        //                 "OCPP16",
        //                 "OCPP20",
        //                 "OCPP201",
        //                 "OCPP21"
        //             ]
        //         },
        //         "VPNEnumType": {
        //             "description": "Type of VPN",
        //             "javaType": "VPNEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "IKEv2",
        //                 "IPSec",
        //                 "L2TP",
        //                 "PPTP"
        //             ]
        //         },
        //         "APNType": {
        //             "description": "Collection of configuration data needed to make a data-connection over a cellular network.\r\n\r\nNOTE: When asking a GSM modem to dial in, it is possible to specify which mobile operator should be used. This can be done with the mobile country code (MCC) in combination with a mobile network code (MNC). Example: If your preferred network is Vodafone Netherlands, the MCC=204 and the MNC=04 which means the key PreferredNetwork = 20404 Some modems allows to specify a preferred network, which means, if this network is not available, a different network is used. If you specify UseOnlyPreferredNetwork and this network is not available, the modem will not dial in.",
        //             "javaType": "APN",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "apn": {
        //                     "description": "The Access Point Name as an URL.",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "apnUserName": {
        //                     "description": "APN username.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "apnPassword": {
        //                     "description": "*(2.1)* APN Password.",
        //                     "type": "string",
        //                     "maxLength": 64
        //                 },
        //                 "simPin": {
        //                     "description": "SIM card pin code.",
        //                     "type": "integer"
        //                 },
        //                 "preferredNetwork": {
        //                     "description": "Preferred network, written as MCC and MNC concatenated. See note.",
        //                     "type": "string",
        //                     "maxLength": 6
        //                 },
        //                 "useOnlyPreferredNetwork": {
        //                     "description": "Default: false. Use only the preferred Network, do\r\nnot dial in when not available. See Note.",
        //                     "type": "boolean",
        //                     "default": false
        //                 },
        //                 "apnAuthentication": {
        //                     "$ref": "#/definitions/APNAuthenticationEnumType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "apn",
        //                 "apnAuthentication"
        //             ]
        //         },
        //         "NetworkConnectionProfileType": {
        //             "description": "The NetworkConnectionProfile defines the functional and technical parameters of a communication link.",
        //             "javaType": "NetworkConnectionProfile",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "apn": {
        //                     "$ref": "#/definitions/APNType"
        //                 },
        //                 "ocppVersion": {
        //                     "$ref": "#/definitions/OCPPVersionEnumType"
        //                 },
        //                 "ocppInterface": {
        //                     "$ref": "#/definitions/OCPPInterfaceEnumType"
        //                 },
        //                 "ocppTransport": {
        //                     "$ref": "#/definitions/OCPPTransportEnumType"
        //                 },
        //                 "messageTimeout": {
        //                     "description": "Duration in seconds before a message send by the Charging Station via this network connection times-out.\r\nThe best setting depends on the underlying network and response times of the CSMS.\r\nIf you are looking for a some guideline: use 30 seconds as a starting point.",
        //                     "type": "integer"
        //                 },
        //                 "ocppCsmsUrl": {
        //                     "description": "URL of the CSMS(s) that this Charging Station communicates with, without the Charging Station identity part. +\r\nThe SecurityCtrlr.Identity field is appended to _ocppCsmsUrl_ to provide the full websocket URL.",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "securityProfile": {
        //                     "description": "This field specifies the security profile used when connecting to the CSMS with this NetworkConnectionProfile.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "identity": {
        //                     "description": "*(2.1)* Charging Station identity to be used as the basic authentication username.",
        //                     "type": "string",
        //                     "maxLength": 48
        //                 },
        //                 "basicAuthPassword": {
        //                     "description": "*(2.1)* BasicAuthPassword to use for security profile 1 or 2.",
        //                     "type": "string",
        //                     "maxLength": 64
        //                 },
        //                 "vpn": {
        //                     "$ref": "#/definitions/VPNType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "ocppInterface",
        //                 "ocppTransport",
        //                 "messageTimeout",
        //                 "ocppCsmsUrl",
        //                 "securityProfile"
        //             ]
        //         },
        //         "VPNType": {
        //             "description": "VPN Configuration settings",
        //             "javaType": "VPN",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "server": {
        //                     "description": "VPN Server Address",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "user": {
        //                     "description": "VPN User",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "group": {
        //                     "description": "VPN group.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "password": {
        //                     "description": "*(2.1)* VPN Password.",
        //                     "type": "string",
        //                     "maxLength": 64
        //                 },
        //                 "key": {
        //                     "description": "VPN shared secret.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "$ref": "#/definitions/VPNEnumType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "server",
        //                 "user",
        //                 "password",
        //                 "key",
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
        //         "configurationSlot": {
        //             "description": "Slot in which the configuration should be stored.",
        //             "type": "integer"
        //         },
        //         "connectionData": {
        //             "$ref": "#/definitions/NetworkConnectionProfileType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "configurationSlot",
        //         "connectionData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetNetworkProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetNetworkProfileRequestParser">A delegate to parse custom SetNetworkProfile requests.</param>
        public static SetNetworkProfileRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setNetworkProfileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetNetworkProfileRequestParser))
            {
                return setNetworkProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetNetworkProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetNetworkProfileRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetNetworkProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetNetworkProfileRequest">The parsed SetNetworkProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetNetworkProfileRequestParser">A delegate to parse custom SetNetworkProfile requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out SetNetworkProfileRequest?      SetNetworkProfileRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestParser   = null)
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
                                             OCPPv2_1.NetworkConnectionProfile.TryParse,
                                             out NetworkConnectionProfile? NetworkConnectionProfile,
                                             out ErrorResponse) ||
                     NetworkConnectionProfile is null)
                {
                    return false;
                }

                #endregion

                #region Signatures                  [optional, OCPP_CSE]

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

                #region CustomData                  [optional]

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


                SetNetworkProfileRequest = new SetNetworkProfileRequest(

                                               Destination,
                                               ConfigurationSlot,
                                               NetworkConnectionProfile,

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

                if (CustomSetNetworkProfileRequestParser is not null)
                    SetNetworkProfileRequest = CustomSetNetworkProfileRequestParser(JSON,
                                                                                    SetNetworkProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                SetNetworkProfileRequest  = null;
                ErrorResponse             = "The given JSON representation of a SetNetworkProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetNetworkProfileRequestSerializer = null, CustomNetworkConnectionProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetNetworkProfileRequestSerializer">A delegate to serialize custom SetNetworkProfile requests.</param>
        /// <param name="CustomNetworkConnectionProfileSerializer">A delegate to serialize custom network connection profiles.</param>
        /// <param name="CustomVPNConfigurationSerializer">A delegate to serialize custom VPN configurations.</param>
        /// <param name="CustomAPNConfigurationSerializer">A delegate to serialize custom APN configurations.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<VPNConfiguration>?          CustomVPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<APNConfiguration>?          CustomAPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",           DefaultJSONLDContext.    ToString())
                               : null,

                                 new JProperty("configurationSlot",  ConfigurationSlot),

                                 new JProperty("connectionData",     NetworkConnectionProfile.ToJSON(CustomNetworkConnectionProfileSerializer,
                                                                                                     CustomVPNConfigurationSerializer,
                                                                                                     CustomAPNConfigurationSerializer,
                                                                                                     CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

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
        /// Compares two SetNetworkProfile requests for equality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest1">A SetNetworkProfile request.</param>
        /// <param name="SetNetworkProfileRequest2">Another SetNetworkProfile request.</param>
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
        /// Compares two SetNetworkProfile requests for inequality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest1">A SetNetworkProfile request.</param>
        /// <param name="SetNetworkProfileRequest2">Another SetNetworkProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetNetworkProfileRequest? SetNetworkProfileRequest1,
                                           SetNetworkProfileRequest? SetNetworkProfileRequest2)

            => !(SetNetworkProfileRequest1 == SetNetworkProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetNetworkProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetNetworkProfile requests for equality.
        /// </summary>
        /// <param name="Object">A SetNetworkProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetNetworkProfileRequest setNetworkProfileRequest &&
                   Equals(setNetworkProfileRequest);

        #endregion

        #region Equals(SetNetworkProfileRequest)

        /// <summary>
        /// Compares two SetNetworkProfile requests for equality.
        /// </summary>
        /// <param name="SetNetworkProfileRequest">A SetNetworkProfile request to compare with.</param>
        public override Boolean Equals(SetNetworkProfileRequest? SetNetworkProfileRequest)

            => SetNetworkProfileRequest is not null &&

               ConfigurationSlot.       Equals(SetNetworkProfileRequest.ConfigurationSlot)        &&
               NetworkConnectionProfile.Equals(SetNetworkProfileRequest.NetworkConnectionProfile) &&

               base.             GenericEquals(SetNetworkProfileRequest);

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

            => $"{ConfigurationSlot}: {NetworkConnectionProfile}";

        #endregion

    }

}
