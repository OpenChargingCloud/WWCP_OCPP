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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A network connection profile.
    /// </summary>
    public class NetworkConnectionProfile : ACustomData,
                                            IEquatable<NetworkConnectionProfile>
    {

        #region Properties

        /// <summary>
        /// The OCPP version to be used.
        /// </summary>
        [Mandatory]
        public OCPPVersions        Version              { get; }

        /// <summary>
        /// The OCPP transport protocol to be used.
        /// </summary>
        [Mandatory]
        public TransportProtocols  Transport            { get; }

        /// <summary>
        /// The URL of the central service (CSMS) that this charging station communicates with.
        /// </summary>
        [Mandatory]
        public URL                 CentralServiceURL    { get; }

        /// <summary>
        /// Duration before a message send by a charging station via this network connection times out.
        /// The best setting depends on the underlying network and response times of the central service (CSMS).
        /// If you are looking for a some guideline: use 30 seconds as a starting point.
        /// </summary>
        [Mandatory]
        public TimeSpan            MessageTimeout       { get; }

        /// <summary>
        /// The security profile to use when connecting to the central service (CSMS).
        /// </summary>
        [Mandatory]
        public SecurityProfiles    SecurityProfile      { get; }

        /// <summary>
        /// The network interface to use when connecting to the central service (CSMS).
        /// </summary>
        [Mandatory]
        public NetworkInterfaces   NetworkInterface     { get; }

        /// <summary>
        /// The optional VPN configuration to use when connecting to the central service (CSMS).
        /// </summary>
        [Optional]
        public VPNConfiguration?   VPNConfiguration     { get; }

        /// <summary>
        /// The optional APN configuration to use when connecting to the central service (CSMS).
        /// </summary>
        [Optional]
        public APNConfiguration?   APNConfiguration     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new network connection profile.
        /// </summary>
        /// <param name="Version">The OCPP version to be used.</param>
        /// <param name="Transport">The OCPP transport protocol to be used.</param>
        /// <param name="CentralServiceURL">The URL of the central service (CSMS) that this charging station communicates with.</param>
        /// <param name="MessageTimeout">Duration before a message send by a charging station via this network connection times out.</param>
        /// <param name="SecurityProfile">The security profile to use when connecting to the central service (CSMS).</param>
        /// <param name="NetworkInterface">The network interface to use when connecting to the central service (CSMS).</param>
        /// <param name="VPNConfiguration">An optional VPN configuration to use when connecting to the central service (CSMS).</param>
        /// <param name="APNConfiguration">An optional APN configuration to use when connecting to the central service (CSMS).</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NetworkConnectionProfile(OCPPVersions        Version,
                                        TransportProtocols  Transport,
                                        URL                 CentralServiceURL,
                                        TimeSpan            MessageTimeout,
                                        SecurityProfiles    SecurityProfile,
                                        NetworkInterfaces   NetworkInterface,
                                        VPNConfiguration?   VPNConfiguration,
                                        APNConfiguration?   APNConfiguration,
                                        CustomData?         CustomData   = null)

            : base(CustomData)

        {

            this.Version            = Version;
            this.Transport          = Transport;
            this.CentralServiceURL  = CentralServiceURL;
            this.MessageTimeout     = MessageTimeout;
            this.SecurityProfile    = SecurityProfile;
            this.NetworkInterface   = NetworkInterface;
            this.VPNConfiguration   = VPNConfiguration;
            this.APNConfiguration   = APNConfiguration;

        }

        #endregion


        #region Documentation

        // "NetworkConnectionProfileType": {
        //   "description": "Communication_ Function\r\nurn:x-oca:ocpp:uid:2:233304\r\nThe NetworkConnectionProfile defines the functional and technical parameters of a communication link.\r\n",
        //   "javaType": "NetworkConnectionProfile",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "apn": {
        //       "$ref": "#/definitions/APNType"
        //     },
        //     "ocppVersion": {
        //       "$ref": "#/definitions/OCPPVersionEnumType"
        //     },
        //     "ocppTransport": {
        //       "$ref": "#/definitions/OCPPTransportEnumType"
        //     },
        //     "ocppCsmsUrl": {
        //       "description": "Communication_ Function. OCPP_ Central_ System_ URL. URI\r\nurn:x-oca:ocpp:uid:1:569357\r\nURL of the CSMS(s) that this Charging Station  communicates with.\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "messageTimeout": {
        //       "description": "Duration in seconds before a message send by the Charging Station via this network connection times-out.\r\nThe best setting depends on the underlying network and response times of the CSMS.\r\nIf you are looking for a some guideline: use 30 seconds as a starting point.\r\n",
        //       "type": "integer"
        //     },
        //     "securityProfile": {
        //       "description": "This field specifies the security profile used when connecting to the CSMS with this NetworkConnectionProfile.\r\n",
        //       "type": "integer"
        //     },
        //     "ocppInterface": {
        //       "$ref": "#/definitions/OCPPInterfaceEnumType"
        //     },
        //     "vpn": {
        //       "$ref": "#/definitions/VPNType"
        //     }
        //   },
        //   "required": [
        //     "ocppVersion",
        //     "ocppTransport",
        //     "ocppCsmsUrl",
        //     "messageTimeout",
        //     "securityProfile",
        //     "ocppInterface"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomNetworkConnectionProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a network connection profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNetworkConnectionProfileParser">A delegate to parse custom network connection profile JSON objects.</param>
        public static NetworkConnectionProfile Parse(JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var networkConnectionProfile,
                         out var errorResponse,
                         CustomNetworkConnectionProfileParser))
            {
                return networkConnectionProfile!;
            }

            throw new ArgumentException("The given JSON representation of a network connection profile is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out NetworkConnectionProfile, CustomNetworkConnectionProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a network connection profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkConnectionProfile">The parsed network connection profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       out NetworkConnectionProfile?  NetworkConnectionProfile,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        out NetworkConnectionProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a network connection profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkConnectionProfile">The parsed network connection profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNetworkConnectionProfileParser">A delegate to parse custom network connection profile JSON objects.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       out NetworkConnectionProfile?                           NetworkConnectionProfile,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileParser)
        {

            try
            {

                NetworkConnectionProfile = default;

                #region Version              [mandatory]

                if (!JSON.ParseMandatory("ocppVersion",
                                         "OCPP version",
                                         OCPPVersionsExtensions.TryParse,
                                         out OCPPVersions Version,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Transport            [mandatory]

                if (!JSON.ParseMandatory("ocppTransport",
                                         "OCPP transport",
                                         TransportProtocolsExtensions.TryParse,
                                         out TransportProtocols Transport,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CentralServiceURL    [mandatory]

                if (!JSON.ParseMandatory("ocppCsmsUrl",
                                         "central service URL",
                                         URL.TryParse,
                                         out URL CentralServiceURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageTimeout       [mandatory]

                if (!JSON.ParseMandatory("messageTimeout",
                                         "message timeout",
                                         out UInt32 messageTimeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                var MessageTimeout = TimeSpan.FromSeconds(messageTimeout);

                #endregion

                #region SecurityProfile      [mandatory]

                if (!JSON.ParseMandatory("securityProfile",
                                         "OCPP security profile",
                                         OCPPSecurityProfilesExtensions.TryParse,
                                         out SecurityProfiles SecurityProfile,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NetworkInterface     [mandatory]

                if (!JSON.ParseMandatory("ocppInterface",
                                         "OCPP network interface",
                                         NetworkInterfacesExtensions.TryParse,
                                         out NetworkInterfaces NetworkInterface,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VPNConfiguration     [optional]

                if (JSON.ParseOptionalJSON("vpn",
                                           "VPN configuration",
                                           OCPPv2_0.VPNConfiguration.TryParse,
                                           out VPNConfiguration? VPNConfiguration,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region APNConfiguration     [optional]

                if (JSON.ParseOptionalJSON("apn",
                                           "APN configuration",
                                           OCPPv2_0.APNConfiguration.TryParse,
                                           out APNConfiguration? APNConfiguration,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

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


                NetworkConnectionProfile = new NetworkConnectionProfile(Version,
                                                                        Transport,
                                                                        CentralServiceURL,
                                                                        MessageTimeout,
                                                                        SecurityProfile,
                                                                        NetworkInterface,
                                                                        VPNConfiguration,
                                                                        APNConfiguration,
                                                                        CustomData);

                if (CustomNetworkConnectionProfileParser is not null)
                    NetworkConnectionProfile = CustomNetworkConnectionProfileParser(JSON,
                                                                                    NetworkConnectionProfile);

                return true;

            }
            catch (Exception e)
            {
                NetworkConnectionProfile  = default;
                ErrorResponse             = "The given JSON representation of a network connection profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNetworkConnectionProfileSerializer = null, CustomVPNConfigurationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNetworkConnectionProfileSerializer">A delegate to serialize custom network connection profiles.</param>
        /// <param name="CustomVPNConfigurationSerializer">A delegate to serialize custom VPN configurations.</param>
        /// <param name="CustomAPNConfigurationSerializer">A delegate to serialize custom APN configurations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<VPNConfiguration>?          CustomVPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<APNConfiguration>?          CustomAPNConfigurationSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("ocppVersion",       Version.          AsText()),
                                 new JProperty("ocppTransport",     Transport.        AsText()),
                                 new JProperty("ocppCsmsUrl",       CentralServiceURL.ToString()),
                                 new JProperty("messageTimeout",    (UInt32) Math.Round(MessageTimeout.TotalSeconds, 0)),
                                 new JProperty("securityProfile",   SecurityProfile.  AsNumber()),
                                 new JProperty("ocppInterface",     NetworkInterface. AsText()),

                           VPNConfiguration is not null
                               ? new JProperty("vpn",               VPNConfiguration. ToJSON(CustomVPNConfigurationSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           APNConfiguration is not null
                               ? new JProperty("apn",               APNConfiguration. ToJSON(CustomAPNConfigurationSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNetworkConnectionProfileSerializer is not null
                       ? CustomNetworkConnectionProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NetworkConnectionProfile1, NetworkConnectionProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkConnectionProfile1">A network connection profile.</param>
        /// <param name="NetworkConnectionProfile2">Another network connection profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkConnectionProfile? NetworkConnectionProfile1,
                                           NetworkConnectionProfile? NetworkConnectionProfile2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NetworkConnectionProfile1, NetworkConnectionProfile2))
                return true;

            // If one is null, but not both, return false.
            if (NetworkConnectionProfile1 is null || NetworkConnectionProfile2 is null)
                return false;

            return NetworkConnectionProfile1.Equals(NetworkConnectionProfile2);

        }

        #endregion

        #region Operator != (NetworkConnectionProfile1, NetworkConnectionProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkConnectionProfile1">A network connection profile.</param>
        /// <param name="NetworkConnectionProfile2">Another network connection profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkConnectionProfile? NetworkConnectionProfile1,
                                           NetworkConnectionProfile? NetworkConnectionProfile2)

            => !(NetworkConnectionProfile1 == NetworkConnectionProfile2);

        #endregion

        #endregion

        #region IEquatable<NetworkConnectionProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network connection profiles for equality.
        /// </summary>
        /// <param name="Object">A network connection profile to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkConnectionProfile networkConnectionProfile &&
                   Equals(networkConnectionProfile);

        #endregion

        #region Equals(NetworkConnectionProfile)

        /// <summary>
        /// Compares two network connection profiles for equality.
        /// </summary>
        /// <param name="NetworkConnectionProfile">A network connection profile to compare with.</param>
        public Boolean Equals(NetworkConnectionProfile? NetworkConnectionProfile)

            => NetworkConnectionProfile is not null &&

               Version.          Equals(NetworkConnectionProfile.Version)           &&
               Transport.        Equals(NetworkConnectionProfile.Transport)         &&
               CentralServiceURL.Equals(NetworkConnectionProfile.CentralServiceURL) &&
               MessageTimeout.   Equals(NetworkConnectionProfile.MessageTimeout)    &&
               SecurityProfile.  Equals(NetworkConnectionProfile.SecurityProfile)   &&
               NetworkInterface. Equals(NetworkConnectionProfile.NetworkInterface)  &&

             ((VPNConfiguration is     null && NetworkConnectionProfile.VPNConfiguration is     null) ||
              (VPNConfiguration is not null && NetworkConnectionProfile.VPNConfiguration is not null && VPNConfiguration.Equals(NetworkConnectionProfile.VPNConfiguration))) &&

             ((APNConfiguration is     null && NetworkConnectionProfile.APNConfiguration is     null) ||
              (APNConfiguration is not null && NetworkConnectionProfile.APNConfiguration is not null && APNConfiguration.Equals(NetworkConnectionProfile.APNConfiguration))) &&

               base.  Equals(NetworkConnectionProfile);

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

                return Version.          GetHashCode()       * 23 ^
                       Transport.        GetHashCode()       * 19 ^
                       CentralServiceURL.GetHashCode()       * 17 ^
                       MessageTimeout.   GetHashCode()       * 13 ^
                       SecurityProfile.  GetHashCode()       * 11 ^
                       NetworkInterface. GetHashCode()       *  7 ^
                      (VPNConfiguration?.GetHashCode() ?? 0) *  5 ^
                      (APNConfiguration?.GetHashCode() ?? 0) *  3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   CentralServiceURL,
                   " (", Version.        AsText(),
                   ", ", SecurityProfile.AsText(),
                   ") ",

                   VPNConfiguration is not null
                       ? ", VPN: " + VPNConfiguration.ToString()
                       : "",

                   APNConfiguration is not null
                       ? ", APN: " + APNConfiguration.ToString()
                       : ""

               );

        #endregion

    }

}
