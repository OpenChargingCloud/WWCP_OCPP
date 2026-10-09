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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

using System.Diagnostics.CodeAnalysis;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A network connection profile.
    /// </summary>
    public class NetworkConnectionProfile : ACustomData,
                                            ICBORSerializable<NetworkConnectionProfile>,
                                            IEquatable<NetworkConnectionProfile>
    {

        #region Properties

        /// <summary>
        /// The optional OCPP version to be used.
        /// </summary>
        /// <remarks>
        /// Optional in OCPP 2.1, and it was required here: a profile without
        /// one, which the schema allows, was refused.
        /// </remarks>
        [Optional]
        public OCPPVersion?        Version              { get; }

        /// <summary>
        /// The optional identity (security profile 1 and 2 only) the charging
        /// station uses at the CSMS, when it differs from the one it is
        /// configured with.
        /// </summary>
        [Optional]
        public String?             Identity             { get; }

        /// <summary>
        /// The optional basic authentication password (security profile 1 and
        /// 2 only) for this connection.
        /// </summary>
        [Optional]
        public String?             BasicAuthPassword    { get; }

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
        public NetworkInterface    NetworkInterface     { get; }

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
        /// <param name="Identity">The optional identity (security profile 1 and 2 only) the charging station uses at the CSMS.</param>
        /// <param name="BasicAuthPassword">The optional basic authentication password (security profile 1 and 2 only).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NetworkConnectionProfile(OCPPVersion?        Version,
                                        TransportProtocols  Transport,
                                        URL                 CentralServiceURL,
                                        TimeSpan            MessageTimeout,
                                        SecurityProfiles    SecurityProfile,
                                        NetworkInterface    NetworkInterface,
                                        VPNConfiguration?   VPNConfiguration    = null,
                                        APNConfiguration?   APNConfiguration    = null,
                                        String?             Identity            = null,
                                        String?             BasicAuthPassword   = null,
                                        CustomData?         CustomData          = null)

            : base(CustomData)

        {

            this.Version            = Version;
            this.Identity           = Identity;
            this.BasicAuthPassword  = BasicAuthPassword;
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
        //   "description": "Communication_ Function\r\nurn:x-oca:ocpp:uid:2:233304\r\nThe NetworkConnectionProfile defines the functional and technical parameters of a communication link.",
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
        //       "description": "Communication_ Function. OCPP_ Central_ System_ URL. URI\r\nurn:x-oca:ocpp:uid:1:569357\r\nURL of the CSMS(s) that this Charging Station  communicates with.",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "messageTimeout": {
        //       "description": "Duration in seconds before a message send by the Charging Station via this network connection times-out.\r\nThe best setting depends on the underlying network and response times of the CSMS.\r\nIf you are looking for a some guideline: use 30 seconds as a starting point.",
        //       "type": "integer"
        //     },
        //     "securityProfile": {
        //       "description": "This field specifies the security profile used when connecting to the CSMS with this NetworkConnectionProfile.",
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
                         CustomNetworkConnectionProfileParser) &&
                networkConnectionProfile is not null)
            {
                return networkConnectionProfile;
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

                #region Version              [optional]

                if (JSON.ParseOptional("ocppVersion",
                                       "OCPP version",
                                       OCPPVersion.TryParse,
                                       out OCPPVersion? Version,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Identity             [optional]

                var Identity           = JSON.GetString("identity");

                #endregion

                #region BasicAuthPassword    [optional]

                var BasicAuthPassword  = JSON.GetString("basicAuthPassword");

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
                                         out TimeSpan MessageTimeout,
                                         out ErrorResponse))
                {
                    return false;
                }

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
                                         OCPPv2_1.NetworkInterface.TryParse,
                                         out NetworkInterface NetworkInterface,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VPNConfiguration     [optional]

                if (JSON.ParseOptionalJSON("vpn",
                                           "VPN configuration",
                                           OCPPv2_1.VPNConfiguration.TryParse,
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
                                           OCPPv2_1.APNConfiguration.TryParse,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NetworkConnectionProfile = new NetworkConnectionProfile(
                                               Version,
                                               Transport,
                                               CentralServiceURL,
                                               MessageTimeout,
                                               SecurityProfile,
                                               NetworkInterface,
                                               VPNConfiguration,
                                               APNConfiguration,
                                               Identity,
                                               BasicAuthPassword,
                                               CustomData
                                           );

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

                           Version.HasValue
                               ? new JProperty("ocppVersion",       Version.Value.    ToString())
                               : null,

                                 new JProperty("ocppTransport",     Transport.        AsText()),
                                 new JProperty("ocppCsmsUrl",       CentralServiceURL.ToString()),
                                 new JProperty("messageTimeout",    (UInt32) Math.Round(MessageTimeout.TotalSeconds, 0)),
                                 new JProperty("securityProfile",   SecurityProfile.  AsNumber()),
                                 new JProperty("ocppInterface",     NetworkInterface. ToString()),

                           VPNConfiguration is not null
                               ? new JProperty("vpn",               VPNConfiguration. ToJSON(CustomVPNConfigurationSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           APNConfiguration is not null
                               ? new JProperty("apn",               APNConfiguration. ToJSON(CustomAPNConfigurationSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           Identity is not null
                               ? new JProperty("identity",          Identity)
                               : null,

                           BasicAuthPassword is not null
                               ? new JProperty("basicAuthPassword", BasicAuthPassword)
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


        #region (static) TryParseCBOR(CBOR, out NetworkConnectionProfile, out ErrorResponse, CustomNetworkConnectionProfileParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a network connection profile.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="NetworkConnectionProfile">The network connection profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out NetworkConnectionProfile?  NetworkConnectionProfile,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out NetworkConnectionProfile,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a network connection profile.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="NetworkConnectionProfile">The network connection profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNetworkConnectionProfileParser">An optional delegate to read custom network connection profiles.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out NetworkConnectionProfile?           NetworkConnectionProfile,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<NetworkConnectionProfile>?  CustomNetworkConnectionProfileParser)
        {

            try
            {

                NetworkConnectionProfile = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a network connection profile is not a map!";
                    return false;
                }

                OCPPVersion? Version = null;

                if (CBOR.ParseOptionalText("ocppVersion",
                                           "OCPP version",
                                           out var VersionText,
                                           out ErrorResponse))
                {

                    if (!OCPPVersion.TryParse(VersionText!, out var VersionValue))
                    {
                        ErrorResponse = $"Invalid OCPP version '{VersionText}'!";
                        return false;
                    }

                    Version = VersionValue;

                }

                if (ErrorResponse is not null)
                    return false;

                if (!CBOR.ParseMandatoryText("ocppTransport",
                                             "OCPP transport",
                                             out var TransportText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!TransportProtocolsExtensions.TryParse(TransportText, out var Transport))
                {
                    ErrorResponse = $"Invalid OCPP transport '{TransportText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("ocppCsmsUrl",
                                             "CSMS URL",
                                             out var CentralServiceURLText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!URL.TryParse(CentralServiceURLText, out var CentralServiceURL))
                {
                    ErrorResponse = $"Invalid CSMS URL '{CentralServiceURLText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("messageTimeout",
                                              "message timeout",
                                              OCPPCBORExtensions.TryParseDuration,
                                              out TimeSpan MessageTimeout,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("securityProfile",
                                               "security profile",
                                               out var securityProfileNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (securityProfileNumber > Byte.MaxValue || !OCPPSecurityProfilesExtensions.TryParse((Byte) securityProfileNumber, out var SecurityProfile))
                {
                    ErrorResponse = $"Invalid security profile '{securityProfileNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("ocppInterface",
                                             "OCPP interface",
                                             out var NetworkInterfaceText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv2_1.NetworkInterface.TryParse(NetworkInterfaceText, out var NetworkInterface))
                {
                    ErrorResponse = $"Invalid OCPP interface '{NetworkInterfaceText}'!";
                    return false;
                }

                CBOR.ParseOptional("vpn",
                                   "VPN configuration",
                                   OCPPv2_1.VPNConfiguration.TryParseCBOR,
                                   out VPNConfiguration? VPNConfiguration,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("apn",
                                   "APN configuration",
                                   OCPPv2_1.APNConfiguration.TryParseCBOR,
                                   out APNConfiguration? APNConfiguration,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("identity",
                                       "identity",
                                       out var Identity,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("basicAuthPassword",
                                       "basic auth password",
                                       out var BasicAuthPassword,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                NetworkConnectionProfile = new NetworkConnectionProfile(
                                               Version,
                                               Transport,
                                               CentralServiceURL,
                                               MessageTimeout,
                                               SecurityProfile,
                                               NetworkInterface,
                                               VPNConfiguration,
                                               APNConfiguration,
                                               Identity,
                                               BasicAuthPassword,
                                               CustomData
                                           );

                if (CustomNetworkConnectionProfileParser is not null)
                    NetworkConnectionProfile = CustomNetworkConnectionProfileParser(CBOR,
                                                          NetworkConnectionProfile);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NetworkConnectionProfile  = default;
                ErrorResponse  = "The given CBOR representation of a network connection profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<NetworkConnectionProfile>.TryParse(CBOR, out NetworkConnectionProfile, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a network connection profile - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<NetworkConnectionProfile>.TryParse(CBORValue                         CBOR,
                                                                          out NetworkConnectionProfile                  Value,
                                                                          [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomNetworkConnectionProfileSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this network connection profile: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomNetworkConnectionProfileSerializer">A delegate to serialize custom network connection profiles.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<NetworkConnectionProfile>? CustomNetworkConnectionProfileSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("ocppVersion",                            OCPPCBORExtensions.Text(Version?.ToString())),
                           ("ocppTransport",                          CBORValue.FromText(Transport.AsText())),
                           ("ocppCsmsUrl",                            CBORValue.FromText(CentralServiceURL.ToString())),
                           ("messageTimeout",                         MessageTimeout.ToCBOR()),
                           ("securityProfile",                        CBORValue.FromUInt64(SecurityProfile.AsNumber())),
                           ("ocppInterface",                          CBORValue.FromText(NetworkInterface.ToString())),
                           ("vpn",                                    VPNConfiguration?.ToCBOR()),
                           ("apn",                                    APNConfiguration?.ToCBOR()),
                           ("identity",                               OCPPCBORExtensions.Text(Identity)),
                           ("basicAuthPassword",                      OCPPCBORExtensions.Text(BasicAuthPassword)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomNetworkConnectionProfileSerializer is not null
                       ? CustomNetworkConnectionProfileSerializer(this, cbor)
                       : cbor;

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

               Nullable.Equals(Version, NetworkConnectionProfile.Version)           &&
               String.Equals(Identity,          NetworkConnectionProfile.Identity,          StringComparison.Ordinal) &&
               String.Equals(BasicAuthPassword, NetworkConnectionProfile.BasicAuthPassword, StringComparison.Ordinal) &&
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
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (Version?.         GetHashCode() ?? 0) * 23 ^
                      (Identity?.         GetHashCode() ?? 0) * 31 ^
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
                   " (", Version?.       ToString() ?? "any OCPP version",
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
