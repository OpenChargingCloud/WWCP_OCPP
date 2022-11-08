/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using System.Net.NetworkInformation;
using Org.BouncyCastle.Crypto.Tls;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static cloud.charging.open.protocols.OCPPv1_6.ConfigurationKey;
using System.Configuration;
using System.Security.Policy;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    public enum AccessRights
    {
        ReadWrite,
        ReadOnly,
        WriteOnly
    }

    public enum KeyStatus
    {
        Mandatory,
        Optional
    }

    public enum ValueTypes
    {
        Unknown,
        Boolean,
        Integer,
        String
    }

    public enum ValueUnits
    {
        Undefined,
        Seconds
    }



    /// <summary>
    /// A configuration key value pair.
    /// </summary>
    public readonly struct ConfigurationKey : IEquatable<ConfigurationKey>,
                                              IComparable<ConfigurationKey>,
                                              IComparable
    {

        #region Data

        /// <summary>
        /// The maximum length of a configuration key.
        /// </summary>
        public const UInt16  MaxConfigurationKeyLength     = 50;

        /// <summary>
        /// The maximum length of a configuration value.
        /// </summary>
        public const UInt16  MaxConfigurationValueLength   = 500;

        #endregion

        #region Properties

        /// <summary>
        /// A configuration key.
        /// </summary>
        public String        Key             { get; }

        /// <summary>
        /// Whether the configuration key is mandatory or optional.
        /// </summary>
        public KeyStatus     KeyStatus       { get; }

        /// <summary>
        /// This configuration value is: read/write, read-only, write-only.
        /// </summary>
        public AccessRights  AccessRights    { get; }

        /// <summary>
        /// The type of the value.
        /// </summary>
        public ValueTypes    ValueType       { get; }

        /// <summary>
        /// The unit of the value.
        /// </summary>
        public ValueUnits    ValueUnit       { get; }

        /// <summary>
        /// The configuration value or 'null' when the key exists but
        /// the value is not (yet) defined.
        /// </summary>
        public String?       Value           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new configuration key.
        /// </summary>
        /// <param name="Key">A configuration key.</param>
        /// <param name="KeyStatus">Whether the configuration key is mandatory or optional.</param>
        /// <param name="AccessRights">This configuration value is: read/write, read-only, write-only.</param>
        /// <param name="ValueType">The type of the value.</param>
        /// <param name="ValueUnit">The unit of the value.</param>
        /// <param name="Value">The configuration value or 'null' when the key exists but the value is not (yet) defined.</param>
        public ConfigurationKey(String        Key,
                                AccessRights  AccessRights,
                                String?       Value   = null)
        {

            this.Key           = Key;
            this.KeyStatus     = KeyStatus.Optional;
            this.AccessRights  = AccessRights;
            this.ValueType     = ValueTypes.Unknown;
            this.ValueUnit     = ValueUnits.Undefined;
            this.Value         = Value;

        }

        /// <summary>
        /// Create a new configuration key.
        /// </summary>
        /// <param name="Key">A configuration key.</param>
        /// <param name="KeyStatus">Whether the configuration key is mandatory or optional.</param>
        /// <param name="AccessRights">This configuration value is: read/write, read-only, write-only.</param>
        /// <param name="ValueType">The type of the value.</param>
        /// <param name="ValueUnit">The unit of the value.</param>
        /// <param name="Value">The configuration value or 'null' when the key exists but the value is not (yet) defined.</param>
        public ConfigurationKey(String        Key,
                                KeyStatus     KeyStatus,
                                AccessRights  AccessRights,
                                ValueTypes    ValueType,
                                ValueUnits    ValueUnit,
                                String?       Value   = null)
        {

            this.Key           = Key;
            this.KeyStatus     = KeyStatus;
            this.AccessRights  = AccessRights;
            this.ValueType     = ValueType;
            this.ValueUnit     = ValueUnit;
            this.Value         = Value;

        }

        #endregion


        #region Documentation

        // <ns:configurationKey>
        //
        //    <ns:key>?</ns:key>
        //    <ns:readonly>?</ns:readonly>
        //
        //    <!--Optional:-->
        //    <ns:value>?</ns:value>
        //
        // </ns:configurationKey>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationResponse",
        //     "title":   "configurationKey",
        //     "type": "array",
        //     "items": {
        //         "type": "object",
        //         "properties": {
        //             "key": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             },
        //             "readonly": {
        //                 "type": "boolean"
        //             },
        //             "value": {
        //                 "type": "string",
        //                 "maxLength": 500
        //             }
        //         },
        //         "additionalProperties": false,
        //         "required": [
        //             "key",
        //             "readonly"
        //         ]
        //     }
        // }

        #endregion

        #region (static) Parse   (XML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a configuration key value pair.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ConfigurationKey Parse(XElement              XML,
                                             OnExceptionDelegate?  OnException   = null)
        {

            if (TryParse(XML,
                         out var configurationKey,
                         OnException))
            {
                return configurationKey;
            }

            throw new ArgumentException("The given XML representation of a configuration key value pair is invalid: ", // + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON)

        /// <summary>
        /// Parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        public static ConfigurationKey Parse(JObject JSON)
        {

            if (TryParse2(JSON,
                         out var configurationKey,
                         out var errorResponse))
            {
                return configurationKey;
            }

            throw new ArgumentException("The given JSON representation of a configuration key value pair is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out ConfigurationKey, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a configuration key value pair.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              XML,
                                       out ConfigurationKey  ConfigurationKey,
                                       OnExceptionDelegate?  OnException   = null)
        {

            try
            {

                ConfigurationKey = new ConfigurationKey(
                                       XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CP + "key"),
                                       XML.MapBooleanOrFail     (OCPPNS.OCPPv1_6_CP + "readonly")
                                           ? AccessRights.ReadOnly
                                           : AccessRights.ReadWrite,
                                       XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "value")
                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                ConfigurationKey = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, out ConfigurationKey, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        public static Boolean TryParse2(JObject               JSON,
                                       out ConfigurationKey  ConfigurationKey,
                                       out String?           ErrorResponse)
        {

            try
            {

                ConfigurationKey = default;

                #region Key

                if (!JSON.ParseMandatoryText("key",
                                             "configuration key",
                                             out String Key,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Readonly

                if (!JSON.ParseMandatory("readonly",
                                         "readonly",
                                         out Boolean Readonly,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value

                if (JSON.ParseOptional("value",
                                       "value",
                                       out String Value,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                ConfigurationKey = new ConfigurationKey(Key,
                                                        Readonly
                                                            ? AccessRights.ReadOnly
                                                            : AccessRights.ReadWrite,
                                                        Value);

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse    = e.Message;
                ConfigurationKey = default;
                return false;
            }

        }

        #endregion

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:configurationKey"]</param>
        public XElement ToXML(XName? XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "configurationKey",

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",       Key.SubstringMax(MaxConfigurationKeyLength)),
                   new XElement(OCPPNS.OCPPv1_6_CP + "readonly",  AccessRights == AccessRights.ReadOnly),

                   Value is not null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "value",  Value.SubstringMax(MaxConfigurationValueLength))
                       : null

               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConfigurationKey>?  CustomConfigurationKeySerializer  = null)
        {

            var json = JSONObject.Create(

                           new JProperty("key",          Key.SubstringMax(MaxConfigurationKeyLength)),
                           new JProperty("readonly",     AccessRights == AccessRights.ReadOnly),

                           Value != null
                               ? new JProperty("value",  Value.SubstringMax(MaxConfigurationValueLength))
                               : null

                       );

            return CustomConfigurationKeySerializer is not null
                       ? CustomConfigurationKeySerializer(this, json)
                       : json;

        }

        #endregion


        #region Well-known configuration keys

        /// <summary>
        /// The basic authentication password is used for HTTP Basic Authentication, minimal length: 16 bytes.
        /// It is strongly advised to be randomly generated binary to get maximal entropy.Hexadecimal represented(20 bytes maximum,
        /// represented as a string of up to 40 hexadecimal digits).
        /// This configuration key is write-only, so that it cannot be accidentally stored in plaintext by the Central System when
        /// it reads out all configuration keys.
        /// This configuration key is required unless only "security profile 3 - TLS with client side certificates" is implemented.
        /// </summary>
        public static readonly ConfigurationKey AuthorizationKey                = new (Keys.WebSocketPingInterval,
                                                                                       KeyStatus.   Optional,
                                                                                       AccessRights.WriteOnly,
                                                                                       ValueTypes.  String,
                                                                                       ValueUnits.  Undefined);

        /// <summary>
        /// Charge Point sending HTTP web socket pings every n seconds.
        /// No pings, when n == 0.
        /// REJECTED, when n &lt; 0.
        /// optional, RW, integer, seconds
        /// </summary>
        public static readonly ConfigurationKey WebSocketPingInterval           = new (Keys.WebSocketPingInterval,
                                                                                       KeyStatus.   Optional,
                                                                                       AccessRights.ReadWrite,
                                                                                       ValueTypes.  Integer,
                                                                                       ValueUnits.  Seconds);


        public static class Keys
        {

            /// XXXMaxLength indicates that XXX is a list of values!


            /// <summary>
            /// The basic authentication password is used for HTTP Basic Authentication, minimal length: 16 bytes.
            /// It is strongly advised to be randomly generated binary to get maximal entropy.Hexadecimal represented(20 bytes maximum,
            /// represented as a string of up to 40 hexadecimal digits).
            /// This configuration key is write-only, so that it cannot be accidentally stored in plaintext by the Central System when
            /// it reads out all configuration keys.
            /// This configuration key is required unless only "security profile 3 - TLS with client side certificates" is implemented.
            /// optional, W, String
            /// </summary>
            public const String AuthorizationKey                            = "AuthorizationKey";

            /// <summary>
            /// Charge Point sending HTTP web socket pings every n seconds.
            /// No pings, when n == 0.
            /// REJECTED, when n &lt; 0.
            /// optional, RW, integer, seconds
            /// </summary>
            public const String WebSocketPingInterval                       = "WebSocketPingInterval";

            /// <summary>
            /// Controls whether a Charge Point will authorize a user when offline using
            /// the Authorization Cache and/or the Local Authorization List.
            /// required, EW, Boolean
            /// </summary>
            public const String LocalAuthorizeOffline                       = "LocalAuthorizeOffline";

            /// <summary>
            /// Controls whether a Charge Point will use the Authorization Cache and/or
            /// the Local Authorization List to start a transaction without waiting for
            /// an authorization response from the Central System.
            /// required, EW, Boolean
            /// </summary>
            public const String LocalPreAuthorize                           = "LocalPreAuthorize";


            /// <summary>
            /// If this key exists, the Charge Point supports an Authorization Cache.
            /// If this key reports a value of true, the Authorization Cache is enabled.
            /// optional, RW, Boolean
            /// </summary>
            public const String AuthorizationCacheEnabled                   = "AuthorizationCacheEnabled";

            /// <summary>
            /// Whether a remote request to start a transaction in the form of a RemoteStartTransaction.req
            /// message should be authorized beforehand like a local action to start a transaction.
            /// required, R or RW, Boolean
            /// </summary>
            public const String AuthorizeRemoteTxRequests                   = "AuthorizeRemoteTxRequests";


            /// <summary>
            /// Number of times to blink Charge Point lighting when signalling.
            /// optional, RW, integer, times
            /// </summary>
            public const String BlinkRepeat                                 = "BlinkRepeat";

            /// <summary>
            /// Maximum number of identifications that can be stored in the Local Authorization List.
            /// required, R, integer
            /// </summary>
            public const String LocalAuthListMaxLength                      = "LocalAuthListMaxLength";

            /// <summary>
            /// Whether the Local Authorization List is enabled.
            /// required, RW, Boolean
            /// </summary>
            public const String LocalAuthListEnabled                        = "LocalAuthListEnabled";

            /// <summary>
            /// Maximum number of identifications that can be send in a single SendLocalList.req.
            /// required, R, integer
            /// </summary>
            public const String SendLocalListMaxLength                      = "SendLocalListMaxLength";

            /// <summary>
            /// A list of supported features, e.g. "LocalAuthListManagement".
            /// required, R
            /// </summary>
            public const String SupportedFeatureProfiles                    = "SupportedFeatureProfiles";

            /// <summary>
            /// Maximum number of items in a SupportedFeatureProfiles list.
            /// optional, R, integer
            /// </summary>
            public const String SupportedFeatureProfilesMaxLength           = "SupportedFeatureProfilesMaxLength";


            /// <summary>
            /// If this key exists, the Charge Point supports Unknown Offline Authorization.
            /// If this key reports a value of true, Unknown Offline Authorization is enabled.
            /// optional, RW, Boolean
            /// </summary>
            public const String AllowOfflineTxForUnknownId                  = "AllowOfflineTxForUnknownId";

            /// <summary>
            /// Whether the Charge Point will stop an ongoing transaction when it receives a non- Accepted
            /// authorization status in a StartTransaction.conf for this transaction.
            /// required, EW, Boolean
            /// </summary>
            public const String StopTransactionOnInvalidId                  = "StopTransactionOnInvalidId";

            /// <summary>
            /// Maximum energy in Wh delivered when an identifier is invalidated by the Central System after start of a transaction.
            /// optional, RW, integer, Wh
            /// </summary>
            public const String MaxEnergyOnInvalidId                        = "MaxEnergyOnInvalidId";

            /// <summary>
            /// How often the Charge Point should try to submit a transaction-related message when the
            /// Central System fails to process it.
            /// required, RW, integer, times
            /// </summary>
            public const String TransactionMessageAttempts                  = "TransactionMessageAttempts";

            /// <summary>
            /// Before every retransmission, a charge point SHOULD wait as many seconds as specified
            /// in its TransactionMessageRetryInterval key, multiplied by the number of preceding
            /// transmissions of this same message.
            /// required, RW, integer, seconds
            /// </summary>
            public const String TransactionMessageRetryInterval             = "TransactionMessageRetryInterval";



            /// <summary>
            /// Max StackLevel of a ChargingProfile. The number defined also indicates the max allowed number
            /// of installed charging schedules per Charging Profile Purposes.
            /// required, R, integer
            /// </summary>
            public const String ChargeProfileMaxStackLevel                  = "ChargeProfileMaxStackLevel";

            /// <summary>
            /// A list of supported quantities for use in a ChargingSchedule. Allowed values: 'Current' and 'Power'.
            /// required, R
            /// </summary>
            public const String ChargingScheduleAllowedChargingRateUnit     = "ChargingScheduleAllowedChargingRateUnit";

            /// <summary>
            /// Maximum number of periods that may be defined per ChargingSchedule.
            /// required, R, integer
            /// </summary>
            public const String ChargingScheduleMaxPeriods                  = "ChargingScheduleMaxPeriods";

            /// <summary>
            /// Maximum number of Charging profiles installed at a time.
            /// required, R, integer
            /// </summary>
            public const String MaxChargingProfilesInstalled                = "MaxChargingProfilesInstalled";

            /// <summary>
            /// If defined and true, this Charge Point support switching from 3 to 1 phase during a Transaction.
            /// optional, R, Boolean
            /// </summary>
            public const String ConnectorSwitch3to1PhaseSupported           = "ConnectorSwitch3to1PhaseSupported";


            /// <summary>
            /// A comma separated list that prescribes the set of measurands to be included
            /// in a MeterValues.req PDU, every MeterValueSampleInterval seconds.
            /// Where applicable, the Measurand is combined with the optional phase; for instance:
            /// Voltage.L1
            /// Default: "Energy.Active.Import.Register"
            /// required, RW
            /// </summary>
            public const String MeterValuesSampledData                      = "MeterValuesSampledData";

            /// <summary>
            /// The maximum amount of elements in the MeterValuesSampledData list.
            /// optional, R, integer
            /// </summary>
            public const String MeterValuesSampledDataMaxLength             = "MeterValuesSampledDataMaxLength";

            /// <summary>
            /// The time (in seconds) between sampling of metering (or other) data, intended to be
            /// transmitted by "MeterValues" PDUs.Samples are acquired and transmitted periodically
            /// at this interval from the start of the charging transaction.
            /// A value of "0" (numeric zero), by convention, is to be interpreted to mean that no
            /// sampled data should be transmitted.
            /// required, RW, integer, seconds
            /// </summary>
            public const String MeterValueSampleInterval                    = "MeterValueSampleInterval";

            /// <summary>
            /// A comma separated list that prescribes the sampled measurands to be included in the
            /// TransactionData element of StopTransaction.req PDU, every MeterValueSampleInterval
            /// seconds from the start of the charging session
            /// required, RW
            /// </summary>
            public const String StopTxnSampledData                          = "StopTxnSampledData";

            /// <summary>
            /// The maximum amount of elements in the StopTxnSampledData list.
            /// optional, R, integer
            /// </summary>
            public const String StopTxnSampledDataMaxLength                 = "StopTxnSampledDataMaxLength";



            /// <summary>
            /// Size (in seconds) of the clock-aligned data interval. This is the size (in seconds)
            /// of the set of evenly spaced aggregation intervals per day, starting at 00:00:00 (midnight).
            /// For example, a value of 900 (15 minutes) indicates that every day should be broken into
            /// 96 15-minute intervals.
            /// When clock aligned data is being transmitted, the interval in question is identified by
            /// the start time and (optional) duration interval value, represented according to the
            /// ISO8601 standard. All "per-period" data (e.g.energy readings) should be accumulated
            /// (for "flow" type measurands such as energy), or averaged(for other values) across the
            /// entire interval(or partial interval, at the beginning or end of a Transaction), and
            /// transmitted(if so enabled) at the end of each interval, bearing the interval start time
            /// timestamp.
            /// A value of "0" (numeric zero), by convention, is to be interpreted to mean that no
            /// clock-aligned data should be transmitted.
            /// required, RW, integer, seconds
            /// </summary>
            public const String ClockAlignedDataInterval                    = "ClockAlignedDataInterval";


            /// <summary>
            /// Interval *from beginning of status: 'Preparing' until incipient Transaction is
            /// automatically canceled, due to failure of EV driver to (correctly) insert the charging
            /// cable connector(s) into the appropriate socket(s). The Charge Point SHALL go back to
            /// the original state, probably: 'Available'.
            /// required, RW, integer, seconds
            /// </summary>
            public const String ConnectionTimeOut                           = "ConnectionTimeOut";


            /// <summary>
            /// The phase rotation per connector in respect to the connector’s electrical meter
            /// (or if absent, the grid connection). Possible values per connector are:
            /// NotApplicable(for Single phase or DC Charge Points)
            /// Unknown(not (yet) known)
            /// RST(Standard Reference Phasing)
            /// RTS(Reversed Reference Phasing)
            /// SRT(Reversed 240 degree rotation)
            /// STR(Standard 120 degree rotation)
            /// TRS(Standard 240 degree rotation)
            /// TSR(Reversed 120 degree rotation)
            /// R can be identified as phase 1 (L1), S as phase 2 (L2), T as phase 3 (L3).
            /// If known, the Charge Point MAY also report the phase rotation between the
            /// grid connection and the main energymeter by using index number Zero(0).
            /// Values are reported in CSL, formatted: 0.RST, 1.RST, 2.RTS
            /// required, RW, CSL
            /// </summary>
            public const String ConnectorPhaseRotation                      = "ConnectorPhaseRotation";

            /// <summary>
            /// Maximum number of items in a ConnectorPhaseRotation Configuration Key.
            /// required, R, integer
            /// </summary>
            public const String ConnectorPhaseRotationMaxLength             = "ConnectorPhaseRotationMaxLength";


            /// <summary>
            /// Maximum number of requested configuration keys in a GetConfiguration.req PDU.
            /// required, R, integer
            /// </summary>
            public const String GetConfigurationMaxKeys                     = "GetConfigurationMaxKeys";


            /// <summary>
            /// Interval of inactivity (no OCPP exchanges) with central system after which the Charge Point should send a Heartbeat.req PDU
            /// required, RW, integer, seconds
            /// </summary>
            public const String HeartbeatInterval                           = "HeartbeatInterval";

            /// <summary>
            /// Percentage of maximum intensity at which to illuminate Charge Point lighting
            /// optional, RW, integer, %
            /// </summary>
            public const String LightIntensity                              = "LightIntensity";


            /// <summary>
            /// A comma separated list that prescribes the set of Clock-aligned measurand(s) to be included in a
            /// MeterValues.req PDU, every ClockAlignedDataInterval seconds
            /// required, RW
            /// </summary>
            public const String MeterValuesAlignedData                      = "MeterValuesAlignedData";

            /// <summary>
            /// The maximum amount of elements in the MeterValuesAlignedData list.
            /// optional, R, integer
            /// </summary>
            public const String MeterValuesAlignedDataMaxLength             = "MeterValuesAlignedDataMaxLength";

            /// <summary>
            /// A comma separated list that prescribes the set of clock-aligned periodic measurands
            /// to be included  in the TransactionData element of StopTransaction.req MeterValues.req
            /// PDU for every ClockAlignedDataInterval of the transaction.
            /// required, RW
            /// </summary>
            public const String StopTxnAlignedData                          = "StopTxnAlignedData";

            /// <summary>
            /// The maximum amount of elements in the StopTxnAlignedData list.
            /// optional, R, integer
            /// </summary>
            public const String StopTxnAlignedDataMaxLength                 = "StopTxnAlignedDataMaxLength";


            /// <summary>
            /// The minimum duration that a Charge Point or Connector status is stable before a
            /// StatusNotification.req PDU is sent to the Central System.
            /// optional, RW, integer, seconds
            /// </summary>
            public const String MinimumStatusDuration                       = "MinimumStatusDuration";


            /// <summary>
            /// The number of physical charging connectors of this Charge Point.
            /// required, R, integer
            /// </summary>
            public const String NumberOfConnectors                          = "NumberOfConnectors";


            /// <summary>
            /// Number of times to retry an unsuccessful reset of the Charge Point.
            /// required, integer, times
            /// </summary>
            public const String ResetRetries                                = "ResetRetries";

            /// <summary>
            /// When set to true, the Charge Point SHALL administratively stop the transaction when the cable is unplugged from the EV.
            /// required, RW, Boolean
            /// </summary>
            public const String StopTransactionOnEVSideDisconnect           = "StopTransactionOnEVSideDisconnect";

            /// <summary>
            /// When set to true, the Charge Point SHALL unlock the cable on Charge Point side when the cable is unplugged at the EV.
            /// required, RW, Boolean
            /// </summary>
            public const String UnlockConnectorOnEVSideDisconnect           = "UnlockConnectorOnEVSideDisconnect";



            /// <summary>
            /// If this configuration key is present and set to true: Charge Point support reservations on connector 0.
            /// optional, R, Boolean
            /// </summary>
            public const String ReserveConnectorZeroSupported               = "ReserveConnectorZeroSupported";




            /// <summary>
            /// When set to true, only one certificate (plus a temporarily fallback certificate) of certificateType CentralSystemRootCertificate is
            /// allowed to be installed at a time.When installing a new Central System Root certificate, the new certificate SHALL replace the
            /// old one AND the new Central System Root Certificate MUST be signed by the old Central System Root Certificate it is replacing.
            /// This configuration key is required unless only "security profile 1 - Unsecured Transport with Basic Authentication" is
            /// implemented.Please note that security profile 1 SHOULD only be used in trusted networks.
            /// 
            /// Note: When using this additional security mechanism please be aware that the Charge Point needs to perform a full certificate chain
            /// verification when the new Central System Root certificate is being installed.However, once the old Central System Root certificate
            /// is set as the fallback certificate, the Charge Point needs to perform a partial certificate chain verification when verifying the
            /// server certificate during the TLS handshake. Otherwise the verification will fail once the old Central System Root (fallback)
            /// certificate is either expired or removed.
            /// optional, R, Boolean
            /// </summary>
            public const String AdditionalRootCertificateCheck              = "AdditionalRootCertificateCheck";

            /// <summary>
            /// This configuration key can be used to limit the size of the 'certificateChain' field from the CertificateSigned.req PDU.
            /// The value of this configuration key has a maximum limit of 10.000 characters.
            /// optional, R, integer
            /// </summary>
            public const String CertificateSignedMaxChainSize               = "CertificateSignedMaxChainSize";

            /// <summary>
            /// Maximum number of Root/CA certificates that can be installed in the Charge Point.
            /// optional, R, integer
            /// </summary>
            public const String CertificateStoreMaxLength                   = "CertificateStoreMaxLength";

            /// <summary>
            /// This configuration key contains CPO name (or an organization trusted by the CPO) as used in the Charge Point Certificate.
            /// This is the CPO name that is to be used in a CSR send via: SignCertificate.req.
            /// optional, RW, String
            /// </summary>
            public const String CpoName                                     = "CpoName";

            /// <summary>
            /// This configuration key is used to set the security profile used by the Charge Point.
            /// 
            /// The value of this configuration key can only be increased to a higher level, not decreased to a lower level, if the Charge Point
            /// receives a lower value then currently configured,the Charge Point SHALL Rejected the ChangeConfiguration.req
            /// Before accepting the new value, the Charge Point SHALL check if all the prerequisites for the new Security Profile are met, if
            /// not, the Charge Point SHALL Rejected the ChangeConfiguration.req.
            /// 
            /// After the security profile was successfully changed, the Charge Point disconnects from the Central System and SHALL
            /// reconnect using the new configured Security Profile.
            /// 
            /// Default, when no security profile is yet configured: 0.
            /// optional, RW, integer
            /// </summary>
            public const String SecurityProfile                             = "SecurityProfile";


            // CertificateSignedMaxSize

            // public const String Central System Certificate
            // public const String Central System Root Certificate
            // public const String Charge Point Certificate
            // public const String Firmware Signing Certificate
            // public const String Manufacturer Root Certificate

        }

        #endregion


        #region Operator overloading

        #region Operator == (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key value pair.</param>
        /// <param name="ConfigurationKey2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.Equals(ConfigurationKey2);

        #endregion

        #region Operator != (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key value pair.</param>
        /// <param name="ConfigurationKey2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => !ConfigurationKey1.Equals(ConfigurationKey2);

        #endregion

        #region Operator <  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConfigurationKey ConfigurationKey1,
                                          ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) < 0;

        #endregion

        #region Operator <= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) <= 0;

        #endregion

        #region Operator >  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConfigurationKey ConfigurationKey1,
                                          ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) > 0;

        #endregion

        #region Operator >= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConfigurationKey> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two configuration key value pairs.
        /// </summary>
        /// <param name="Object">A configuration key value pair to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConfigurationKey configurationKey
                   ? CompareTo(configurationKey)
                   : throw new ArgumentException("The given object is not a configuration key value pair!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConfigurationKey)

        /// <summary>
        /// Compares two configuration key value pairs.
        /// </summary>
        /// <param name="ConfigurationKey">A configuration key value pair to compare with.</param>
        public Int32 CompareTo(ConfigurationKey ConfigurationKey)
        {

            var c = Key.CompareTo(ConfigurationKey.Key);

            if (c == 0)
                c = AccessRights.CompareTo(ConfigurationKey.AccessRights);

            if (c == 0 && Value is not null && ConfigurationKey.Value is not null)
                c = Value.CompareTo(ConfigurationKey.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ConfigurationKey> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="Object">A configuration key value pair to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConfigurationKey configurationKey &&
                   Equals(configurationKey);

        #endregion

        #region Equals(ConfigurationKey)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="ConfigurationKey">A configuration key value pair to compare with.</param>
        public Boolean Equals(ConfigurationKey ConfigurationKey)

            => Key.         Equals(ConfigurationKey.Key)          &&
               AccessRights.Equals(ConfigurationKey.AccessRights) &&

             ((Value is     null && ConfigurationKey.Value is     null) ||
              (Value is not null && ConfigurationKey.Value is not null && Value.Equals(ConfigurationKey.Value)));

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

                return Key.         GetHashCode() * 5 ^
                       AccessRights.GetHashCode() * 3 ^

                      (Value?.      GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Key,

                             Value is not null
                                 ? " = " + Value
                                 : "",

                             " (", AccessRights.ToString(), ")");

        #endregion

    }

}
