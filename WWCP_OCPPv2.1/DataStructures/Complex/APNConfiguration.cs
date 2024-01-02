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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An APN configuration.
    /// </summary>
    public class APNConfiguration : ACustomData,
                                    IEquatable<APNConfiguration>
    {

        #region Properties

        /// <summary>
        /// The access point name.
        /// </summary>
        [Mandatory]
        public String                    AccessPointName         { get; }

        /// <summary>
        /// The APN authentication method.
        /// </summary>
        [Mandatory]
        public APNAuthenticationMethods  AuthenticationMethod    { get; }

        /// <summary>
        /// The optional APN username.
        /// </summary>
        [Optional]
        public String?                   Username                { get; }

        /// <summary>
        /// The optional APN password.
        /// </summary>
        [Optional]
        public String?                   Password                { get; }

        /// <summary>
        /// The optional PIN code of the SIM card.
        /// </summary>
        [Optional]
        public String?                   SIMPINCode              { get; }

        /// <summary>
        /// The optional preferred network, written as string concatenation of MCC and MNC.
        /// </summary>
        /// <example>When MCC = "204" and MNC = "04" => PreferredNetwork = "20404"</example>
        [Optional]
        public String?                   PreferredNetwork        { get; }

        /// <summary>
        /// Use only the preferred mobile phone network.
        /// Do not dial into a roaming network when the preferred network is not available.
        /// </summary>
        [Optional]
        public Boolean?                  OnlyPreferredNetwork    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new APN configuration.
        /// </summary>
        /// <param name="AccessPointName">The access point name.</param>
        /// <param name="AuthenticationMethod">The APN authentication method.</param>
        /// <param name="Username">An optional APN username.</param>
        /// <param name="Password">An optional APN password.</param>
        /// <param name="SIMPINCode">An optional PIN code of the SIM card.</param>
        /// <param name="PreferredNetwork">An optional preferred network, written as string concatenation of MCC and MNC.</param>
        /// <param name="OnlyPreferredNetwork">Use only the preferred mobile phone network. Do not dial into a roaming network when the preferred network is not available.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public APNConfiguration(String                    AccessPointName,
                                APNAuthenticationMethods  AuthenticationMethod,
                                String?                   Username               = null,
                                String?                   Password               = null,
                                String?                   SIMPINCode             = null,
                                String?                   PreferredNetwork       = null,
                                Boolean?                  OnlyPreferredNetwork   = null,
                                CustomData?               CustomData             = null)

            : base(CustomData)

        {

            this.AccessPointName       = AccessPointName;
            this.AuthenticationMethod  = AuthenticationMethod;
            this.Username              = Username;
            this.Password              = Password;
            this.SIMPINCode            = SIMPINCode;
            this.PreferredNetwork      = PreferredNetwork;
            this.OnlyPreferredNetwork  = OnlyPreferredNetwork;

        }

        #endregion


        #region Documentation

        // "APNType": {
        //   "description": "APN\r\nurn:x-oca:ocpp:uid:2:233134\r\nCollection of configuration data needed to make a data-connection over a cellular network.\r\n\r\nNOTE: When asking a GSM modem to dial in, it is possible to specify which mobile operator should be used. This can be done with the mobile country code (MCC) in combination with a mobile network code (MNC). Example: If your preferred network is Vodafone Netherlands, the MCC=204 and the MNC=04 which means the key PreferredNetwork = 20404 Some modems allows to specify a preferred network, which means, if this network is not available, a different network is used. If you specify UseOnlyPreferredNetwork and this network is not available, the modem will not dial in.\r\n",
        //   "javaType": "APN",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "apn": {
        //       "description": "APN. APN. URI\r\nurn:x-oca:ocpp:uid:1:568814\r\nThe Access Point Name as an URL.\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "apnUserName": {
        //       "description": "APN. APN. User_ Name\r\nurn:x-oca:ocpp:uid:1:568818\r\nAPN username.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "apnPassword": {
        //       "description": "APN. APN. Password\r\nurn:x-oca:ocpp:uid:1:568819\r\nAPN Password.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "simPin": {
        //       "description": "APN. SIMPIN. PIN_ Code\r\nurn:x-oca:ocpp:uid:1:568821\r\nSIM card pin code.\r\n",
        //       "type": "integer"
        //     },
        //     "preferredNetwork": {
        //       "description": "APN. Preferred_ Network. Mobile_ Network_ ID\r\nurn:x-oca:ocpp:uid:1:568822\r\nPreferred network, written as MCC and MNC concatenated. See note.\r\n",
        //       "type": "string",
        //       "maxLength": 6
        //     },
        //     "useOnlyPreferredNetwork": {
        //       "description": "APN. Use_ Only_ Preferred_ Network. Indicator\r\nurn:x-oca:ocpp:uid:1:568824\r\nDefault: false. Use only the preferred Network, do\r\nnot dial in when not available. See Note.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "apnAuthentication": {
        //       "$ref": "#/definitions/APNAuthenticationEnumType"
        //     }
        //   },
        //   "required": [
        //     "apn",
        //     "apnAuthentication"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomAPNConfigurationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a APN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAPNConfigurationParser">A delegate to parse custom APN configuration JSON objects.</param>
        public static APNConfiguration Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<APNConfiguration>?  CustomAPNConfigurationParser   = null)
        {

            if (TryParse(JSON,
                         out var apnConfiguration,
                         out var errorResponse,
                         CustomAPNConfigurationParser) &&
                apnConfiguration is not null)
            {
                return apnConfiguration;
            }

            throw new ArgumentException("The given JSON representation of a APN configuration is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out APNConfiguration, CustomAPNConfigurationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a APN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="APNConfiguration">The parsed APN configuration.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out APNConfiguration?  APNConfiguration,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out APNConfiguration,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a APN configuration.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="APNConfiguration">The parsed APN configuration.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAPNConfigurationParser">A delegate to parse custom APN configuration JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out APNConfiguration?                           APNConfiguration,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<APNConfiguration>?  CustomAPNConfigurationParser)
        {

            try
            {

                APNConfiguration = default;

                #region AccessPointName         [mandatory]

                if (!JSON.ParseMandatoryText("apn",
                                             "access point name",
                                             out String AccessPointName,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AuthenticationMethod    [mandatory]

                if (!JSON.ParseMandatory("apnAuthentication",
                                         "APN authentication method",
                                         APNAuthenticationMethodsExtensions.TryParse,
                                         out APNAuthenticationMethods AuthenticationMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Username                [optional]

                var Username = JSON.GetString("apnUserName");

                #endregion

                #region Password                [optional]

                var Password = JSON.GetString("apnPassword");

                #endregion

                #region SIMPINCode              [optional]

                var SIMPINCode = JSON.GetString("simPin");

                #endregion

                #region PreferredNetwork        [optional]

                var PreferredNetwork = JSON.GetString("preferredNetwork");

                #endregion

                #region OnlyPreferredNetwork    [optional]

                if (JSON.ParseOptional("preferredNetwork",
                                       "APN authentication method",
                                       out Boolean? OnlyPreferredNetwork,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

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


                APNConfiguration = new APNConfiguration(AccessPointName,
                                                        AuthenticationMethod,
                                                        Username,
                                                        Password,
                                                        SIMPINCode,
                                                        PreferredNetwork,
                                                        OnlyPreferredNetwork,
                                                        CustomData);

                if (CustomAPNConfigurationParser is not null)
                    APNConfiguration = CustomAPNConfigurationParser(JSON,
                                                                    APNConfiguration);

                return true;

            }
            catch (Exception e)
            {
                APNConfiguration  = default;
                ErrorResponse     = "The given JSON representation of a APN configuration is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAPNConfigurationSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAPNConfigurationSerializer">A delegate to serialize custom APN configurations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<APNConfiguration>?  CustomAPNConfigurationSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("apn",                       AccessPointName),
                                 new JProperty("apnAuthentication",         AuthenticationMethod.AsText()),

                           Username             is not null
                               ? new JProperty("apnUserName",               Username)
                               : null,

                           Password             is not null
                               ? new JProperty("apnPassword",               Password)
                               : null,

                           SIMPINCode           is not null
                               ? new JProperty("simPin",                    SIMPINCode)
                               : null,

                           PreferredNetwork     is not null
                               ? new JProperty("preferredNetwork",          PreferredNetwork)
                               : null,

                           OnlyPreferredNetwork is not null
                               ? new JProperty("useOnlyPreferredNetwork",   OnlyPreferredNetwork)
                               : null,

                           CustomData           is not null
                               ? new JProperty("customData",                CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAPNConfigurationSerializer is not null
                       ? CustomAPNConfigurationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (APNConfiguration1, APNConfiguration2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="APNConfiguration1">A APN configuration.</param>
        /// <param name="APNConfiguration2">Another APN configuration.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (APNConfiguration? APNConfiguration1,
                                           APNConfiguration? APNConfiguration2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(APNConfiguration1, APNConfiguration2))
                return true;

            // If one is null, but not both, return false.
            if (APNConfiguration1 is null || APNConfiguration2 is null)
                return false;

            return APNConfiguration1.Equals(APNConfiguration2);

        }

        #endregion

        #region Operator != (APNConfiguration1, APNConfiguration2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="APNConfiguration1">A APN configuration.</param>
        /// <param name="APNConfiguration2">Another APN configuration.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (APNConfiguration? APNConfiguration1,
                                           APNConfiguration? APNConfiguration2)

            => !(APNConfiguration1 == APNConfiguration2);

        #endregion

        #endregion

        #region IEquatable<APNConfiguration> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two APN configurations for equality.
        /// </summary>
        /// <param name="Object">A APN configuration to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is APNConfiguration apnConfiguration &&
                   Equals(apnConfiguration);

        #endregion

        #region Equals(APNConfiguration)

        /// <summary>
        /// Compares two APN configurations for equality.
        /// </summary>
        /// <param name="APNConfiguration">A APN configuration to compare with.</param>
        public Boolean Equals(APNConfiguration? APNConfiguration)

            => APNConfiguration is not null &&

               AccessPointName.     Equals(APNConfiguration.AccessPointName)      &&
               AuthenticationMethod.Equals(APNConfiguration.AuthenticationMethod) &&

             ((Username             is     null &&  APNConfiguration.Username         is     null)  ||
              (Username             is not null &&  APNConfiguration.Username         is not null  && Username.                  Equals(APNConfiguration.Username)))                  &&

             ((Password             is     null &&  APNConfiguration.Password         is     null)  ||
              (Password             is not null &&  APNConfiguration.Password         is not null  && Password.                  Equals(APNConfiguration.Password)))                  &&

             ((SIMPINCode           is     null &&  APNConfiguration.SIMPINCode       is     null)  ||
              (SIMPINCode           is not null &&  APNConfiguration.SIMPINCode       is not null  && SIMPINCode.                Equals(APNConfiguration.SIMPINCode)))                &&

             ((PreferredNetwork     is     null &&  APNConfiguration.PreferredNetwork is     null)  ||
              (PreferredNetwork     is not null &&  APNConfiguration.PreferredNetwork is not null  && PreferredNetwork.          Equals(APNConfiguration.PreferredNetwork)))          &&

            ((!OnlyPreferredNetwork.HasValue    && !APNConfiguration.OnlyPreferredNetwork.HasValue) ||
               OnlyPreferredNetwork.HasValue    &&  APNConfiguration.OnlyPreferredNetwork.HasValue && OnlyPreferredNetwork.Value.Equals(APNConfiguration.OnlyPreferredNetwork.Value)) &&

               base.                Equals(APNConfiguration);

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

                return AccessPointName.      GetHashCode()       * 19 ^
                       AuthenticationMethod. GetHashCode()       * 17 ^
                      (Username?.            GetHashCode() ?? 0) * 13 ^
                      (Password?.            GetHashCode() ?? 0) * 11^
                      (SIMPINCode?.          GetHashCode() ?? 0) *  7 ^
                      (PreferredNetwork?.    GetHashCode() ?? 0) *  5 ^
                      (OnlyPreferredNetwork?.GetHashCode() ?? 0) *  3 ^

                       base.                 GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Username,
                   " / '", Password, "'",
                   " @ ",
                   AccessPointName,
                   " (", PreferredNetwork, ")"

               );

        #endregion

    }

}
