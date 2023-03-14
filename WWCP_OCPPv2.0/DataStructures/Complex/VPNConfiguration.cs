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
    /// A VPN configuration.
    /// </summary>
    public class VPNConfiguration : ACustomData,
                                    IEquatable<VPNConfiguration>
    {

        #region Properties

        /// <summary>
        /// The VPN server URL.
        /// </summary>
        [Mandatory]
        public URL           ServerURL       { get; }

        /// <summary>
        /// The VPN login/user(name).
        /// </summary>
        [Mandatory]
        public String        Login           { get; }

        /// <summary>
        /// The VPN password.
        /// </summary>
        [Mandatory]
        public String        Password        { get; }

        /// <summary>
        /// The VPN shared secret/key.
        /// </summary>
        [Mandatory]
        public String        SharedSecret    { get; }

        /// <summary>
        /// The VPN protocol.
        /// </summary>
        [Mandatory]
        public VPNProtocols  Protocol        { get; }

        /// <summary>
        /// The optional VPN (user/access) group.
        /// </summary>
        [Optional]
        public String?       AccessGroup     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new VPN configuration.
        /// </summary>
        /// <param name="ServerURL">The VPN server URL.</param>
        /// <param name="Login">The VPN login/user(name).</param>
        /// <param name="Password">The VPN password.</param>
        /// <param name="SharedSecret">The VPN shared secret/key.</param>
        /// <param name="Protocol">The VPN protocol.</param>
        /// <param name="AccessGroup">An optional VPN (user/access) group.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VPNConfiguration(URL           ServerURL,
                                String        Login,
                                String        Password,
                                String        SharedSecret,
                                VPNProtocols  Protocol,
                                String?       AccessGroup   = null,
                                CustomData?   CustomData    = null)

            : base(CustomData)

        {

            this.ServerURL     = ServerURL;
            this.Login         = Login;
            this.Password      = Password;
            this.SharedSecret  = SharedSecret;
            this.Protocol      = Protocol;
            this.AccessGroup   = AccessGroup;

        }

        #endregion


        #region Documentation

        // "VPNType": {
        //   "description": "VPN\r\nurn:x-oca:ocpp:uid:2:233268\r\nVPN Configuration settings\r\n",
        //   "javaType": "VPN",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "server": {
        //       "description": "VPN. Server. URI\r\nurn:x-oca:ocpp:uid:1:569272\r\nVPN Server Address\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "user": {
        //       "description": "VPN. User. User_ Name\r\nurn:x-oca:ocpp:uid:1:569273\r\nVPN User\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "group": {
        //       "description": "VPN. Group. Group_ Name\r\nurn:x-oca:ocpp:uid:1:569274\r\nVPN group.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "password": {
        //       "description": "VPN. Password. Password\r\nurn:x-oca:ocpp:uid:1:569275\r\nVPN Password.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "key": {
        //       "description": "VPN. Key. VPN_ Key\r\nurn:x-oca:ocpp:uid:1:569276\r\nVPN shared secret.\r\n",
        //       "type": "string",
        //       "maxLength": 255
        //     },
        //     "type": {
        //       "$ref": "#/definitions/VPNEnumType"
        //     }
        //   },
        //   "required": [
        //     "server",
        //     "user",
        //     "password",
        //     "key",
        //     "type"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVPNConfigurationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a VPN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVPNConfigurationParser">A delegate to parse custom VPN configuration JSON objects.</param>
        public static VPNConfiguration Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<VPNConfiguration>?  CustomVPNConfigurationParser   = null)
        {

            if (TryParse(JSON,
                         out var vpnConfiguration,
                         out var errorResponse,
                         CustomVPNConfigurationParser))
            {
                return vpnConfiguration!;
            }

            throw new ArgumentException("The given JSON representation of a VPN configuration is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VPNConfiguration, CustomVPNConfigurationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a VPN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VPNConfiguration">The parsed VPN configuration.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out VPNConfiguration?  VPNConfiguration,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out VPNConfiguration,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of a VPN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VPNConfiguration">The parsed VPN configuration.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVPNConfigurationParser">A delegate to parse custom VPN configuration JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out VPNConfiguration?                           VPNConfiguration,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<VPNConfiguration>?  CustomVPNConfigurationParser)
        {

            try
            {

                VPNConfiguration = default;

                #region ServerURL       [mandatory]

                if (!JSON.ParseMandatory("server",
                                         "VPN server URL",
                                         URL.TryParse,
                                         out URL ServerURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Login           [mandatory]

                if (!JSON.ParseMandatoryText("user",
                                             "VPN login/user(name)",
                                             out String Login,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Password        [mandatory]

                if (!JSON.ParseMandatoryText("password",
                                             "VPN password",
                                             out String Password,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SharedSecret    [mandatory]

                if (!JSON.ParseMandatoryText("key",
                                             "VPN shared secret/key",
                                             out String SharedSecret,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Protocol        [mandatory]

                if (!JSON.ParseMandatory("name",
                                         "VPN configuration name",
                                         VPNProtocolsExtensions.TryParse,
                                         out VPNProtocols Protocol,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AccessGroup     [mandatory]

                var AccessGroup = JSON.GetString("group");

                #endregion

                #region CustomData      [optional]

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


                VPNConfiguration = new VPNConfiguration(ServerURL,
                                                        Login,
                                                        Password,
                                                        SharedSecret,
                                                        Protocol,
                                                        AccessGroup,
                                                        CustomData);

                if (CustomVPNConfigurationParser is not null)
                    VPNConfiguration = CustomVPNConfigurationParser(JSON,
                                                                    VPNConfiguration);

                return true;

            }
            catch (Exception e)
            {
                VPNConfiguration  = default;
                ErrorResponse     = "The given JSON representation of a VPN configuration is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVPNConfigurationSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVPNConfigurationSerializer">A delegate to serialize custom VPN configurations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VPNConfiguration>?  CustomVPNConfigurationSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("server",      ServerURL. ToString()),
                                 new JProperty("user",        Login),
                                 new JProperty("password",    Password),
                                 new JProperty("key",         SharedSecret),
                                 new JProperty("type",        Protocol.  AsText()),

                           AccessGroup is not null
                               ? new JProperty("group",       AccessGroup)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVPNConfigurationSerializer is not null
                       ? CustomVPNConfigurationSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VPNConfiguration1, VPNConfiguration2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VPNConfiguration1">A VPN configuration.</param>
        /// <param name="VPNConfiguration2">Another VPN configuration.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VPNConfiguration? VPNConfiguration1,
                                           VPNConfiguration? VPNConfiguration2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VPNConfiguration1, VPNConfiguration2))
                return true;

            // If one is null, but not both, return false.
            if (VPNConfiguration1 is null || VPNConfiguration2 is null)
                return false;

            return VPNConfiguration1.Equals(VPNConfiguration2);

        }

        #endregion

        #region Operator != (VPNConfiguration1, VPNConfiguration2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VPNConfiguration1">A VPN configuration.</param>
        /// <param name="VPNConfiguration2">Another VPN configuration.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VPNConfiguration? VPNConfiguration1,
                                           VPNConfiguration? VPNConfiguration2)

            => !(VPNConfiguration1 == VPNConfiguration2);

        #endregion

        #endregion

        #region IEquatable<VPNConfiguration> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two VPN configurations for equality.
        /// </summary>
        /// <param name="Object">A VPN configuration to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VPNConfiguration vpnConfiguration &&
                   Equals(vpnConfiguration);

        #endregion

        #region Equals(VPNConfiguration)

        /// <summary>
        /// Compares two VPN configurations for equality.
        /// </summary>
        /// <param name="VPNConfiguration">A VPN configuration to compare with.</param>
        public Boolean Equals(VPNConfiguration? VPNConfiguration)

            => VPNConfiguration is not null &&

               ServerURL.   Equals(VPNConfiguration.ServerURL)    &&
               Login.       Equals(VPNConfiguration.Login)        &&
               Password.    Equals(VPNConfiguration.Password)     &&
               SharedSecret.Equals(VPNConfiguration.SharedSecret) &&
               Protocol.    Equals(VPNConfiguration.Protocol)     &&

             ((AccessGroup is     null && VPNConfiguration.AccessGroup is     null) ||
              (AccessGroup is not null && VPNConfiguration.AccessGroup is not null && AccessGroup.Equals(VPNConfiguration.AccessGroup))) &&

               base.        Equals(VPNConfiguration);

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

                return ServerURL.   GetHashCode()       * 17 ^
                       Login.       GetHashCode()       * 13 ^
                       Password.    GetHashCode()       * 11 ^
                       SharedSecret.GetHashCode()       *  7 ^
                       Protocol.    GetHashCode()       *  5 ^
                      (AccessGroup?.GetHashCode() ?? 0) *  3 ^

                       base.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Login,
                   " / '", Password, "'",
                   " @ ",
                   ServerURL,
                   " (", Protocol, ")"

               );

        #endregion

    }

}
