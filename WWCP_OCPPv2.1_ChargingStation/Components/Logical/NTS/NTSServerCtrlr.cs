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

using org.GraphDefined.Vanaheimr.Illias;

using org.GraphDefined.Vanaheimr.Norn.NTS;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration of a single
    /// Network Time Security (NTS) server.
    /// </summary>
    public class NTSServerCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// The optional DNS hostname or IP address of the NTP server.
        /// When undefined, the hostname of the NTS Key Exchange (KE) server will be used.
        /// </summary>
        public DomainName?            NTP_Hostname                                   { get; set; }

        /// <summary>
        /// The optional NTP UDP/TCP port number.
        /// When undefined, the default NTP port number 123 will be used.
        /// </summary>
        public IPPort?                NTP_Port                                       { get; set; }


        public IPVersionPreference?   NTP_IPVersionPreference                        { get; set; }  // IPv4Only / IPv6Only / PreferIPv6 / PreferIPv4

        public Byte?                  NTP_IPHopLimit                                 { get; set; }



        /// <summary>
        /// The NTP protocol version to use (Default: 4).
        /// </summary>
        public String?                NTP_Version                                    { get; set; }


        public TimeSpan?              NTP_MinPollInterval                            { get; set; }
        public TimeSpan?              NTP_MaxPollInterval                            { get; set; }
        public NTPRandomizationMode?  NTP_RandomizationMode                          { get; set; } // None, Uniform, Jitter (Default), Poisson
        public PercentageDouble?      NTP_RandomizationJitter                        { get; set; } // 25% jitter factor by default

        /// <summary>
        /// Whether to perform an initial burst of NTP requests at startup or after
        /// network recovery to quickly synchronize time (e.g. 4 fast requests within
        /// a short time window).
        /// Might cause a thundering herd if too many clients do this at the same time!
        /// </summary>
        public Boolean?               NTP_AllowInitialBurst                          { get; set; }



        /// <summary>
        /// These values specifies a correction (in milliseconds) which will be applied to measured time offsets.
        /// This can compensate known stable asymmetries in network or processing delays.
        /// For example, if packets sent to the source were on average delayed by 100 microseconds more
        /// than packets sent from the source back, the correction would be -0.05 (-50 microseconds).
        /// </summary>
        public TimeSpan?              NTP_DelayAsymmetry                             { get; set; }

        /// <summary>
        /// These values can fine-tune the offset calculations when network delay variability is greater
        /// in one direction than the other.
        /// Use only if you know your network has a consistent asymmetry.
        /// </summary>
        public Double?                NTP_DelayVarAsymmetry                          { get; set; }

        /// <summary>
        /// Occasional requests to an NTP/NTS server may be delayed due to network caching effects such
        /// as ARP or DNS resolution, firewall state establishment, TLS tunnel setup, and similar factors.
        /// To prevent inaccurate delay measurements, a preflight NTP packet is sent and its response
        /// ignored before the actual measurement takes place.
        /// The configured values define the time intervals since the last measurement that trigger
        /// sending preflight NTP packets.
        /// </summary>
        public TimeSpan?              NTP_Preflight                                  { get; set; }

        public UInt16?                NTP_RateLimit                                  { get; set; }

        public TimeSpan?              NTP_RequestTimeout                             { get; set; }

        /// <summary>
        /// The number of consecutive measurement errors that should lead to an entry within the security log book.
        /// When the measurements recovered from the error another log book entry shall be added.
        /// </summary>
        public UInt16?                NTP_ErrorLoggingThreshold                      { get; set; }



        /// <summary>
        /// DNS hostname or IP address of the NTS Key Exchange (KE) server.
        /// This is the server that the Charging Station will connect to for time synchronization and key exchange.
        /// </summary>
        public DomainName?            NTS_KE_Hostname                                { get; set; }

        /// <summary>
        /// The optional NTS Key Exchange (KE) TCP port number.
        /// When undefined, the default NTS KE port number 4460 will be used.
        /// </summary>
        public IPPort?                NTS_KE_Port                                    { get; set; }
        public IPVersionPreference?   NTS_KE_IPVersionPreference                     { get; set; }
        public Byte?                  NTS_KE_IPHopLimit                              { get; set; }
        public String?                NTS_KE_MinTLSVersion                           { get; set; }  // TLS v1.3
        public IEnumerable<String>    NTS_KE_RootCACertificates                      { get; set; }
        public String?                NTS_KE_RootCACertificate_Validation            { get; set; }  // Strict, Trust_on_first_use, skip_lifetime_validation, skip_validation
        public String?                NTS_KE_RootCACertificate_Logging               { get; set; }  // MetrologicalLogging, SecurityLogging, NoLogging

        // Are intermediate certificates really not needed? What would be use cases for them?

        public IEnumerable<String>    NTS_KE_ServerCertificates                      { get; set; }
        public String?                NTS_KE_ServerCertificate_Validation            { get; set; }  // Strict, Trust_on_first_use, skip_lifetime_validation, skip_validation
        public String?                NTS_KE_ServerCertificate_Logging               { get; set; }  // MetrologicalLogging, SecurityLogging, NoLogging

        /// <summary>
        /// Check for specific TLS extensions in the server certificate,
        /// e.g. support for NTS, signed NTS responses, ...
        /// </summary>
        public String?                NTS_KE_ServerCertificate_Extensions            { get; set; }

        /// <summary>
        /// Whether the notBefore and notAfter timestamp checks of NTS and NTP-over-TLS TLS certificates
        /// can be skipped on the first time sync request per server immediately after a reboot, as the
        /// device might have started with a wrong internal time, e.g. due to not having an RTC or
        /// backup battery.
        /// </summary>
        public Boolean?               NTS_KE_NoTLSCertificateTimeCheckAfterReboot    { get; set; }

        /// <summary>
        /// NTS_KE_RevocationMode: OCSP, CRL, None
        /// </summary>
        public String?                NTS_KE_CertificateRevocationCheck              { get; set; }

        public TimeSpan?              NTS_KE_RequestTimeout                          { get; set; }
        public UInt16?                NTS_KE_RateLimit                               { get; set; }

        /// <summary>
        /// Refreshing the NTS keys and cookies should be started after the given time span
        /// since the last NTS-KE handshakes (randomly between given min and max values).
        /// </summary>
        public TimeSpan?              NTS_KE_MinCookieRefreshTime                    { get; set; }

        /// <summary>
        /// Refreshing the NTS keys and cookies must be completed within the given time span
        /// since the last NTS-KE handshakes (randomly between given min and max values).
        /// </summary>
        public TimeSpan?              NTS_KE_MaxCookieRefreshTime                    { get; set; }

        /// <summary>
        /// The desired number of NTS cookies that should be kept available
        /// for authenticated NTP messages with this NTS server.
        /// </summary>
        public UInt16?                NTS_KE_DesiredNumberOfCookies                  { get; set; }

        /// <summary>
        /// The optional Authenticated Encryption with Associated Data (AEAD) algorithm
        /// used for NTS authentication of NTP messages.
        /// </summary>
        public AEADAlgorithms?        NTS_KE_AEADAlgorithm                           { get; set; }

        /// <summary>
        /// Whether NTS responses shall be digital signed (GraphDefined NTS extension)
        /// to provide authenticity and integrity protection even for the NTP packets.
        /// </summary>
        public Boolean?               NTS_KE_SignedResponses                         { get; set; }



        /// <summary>
        /// When this variable reports a value of true, this NTS server
        /// can be used for time synchronization.
        /// </summary>
        public Boolean?               Enabled                                        { get; set; }

        /// <summary>
        /// The optional name of the group this NTS server belongs to, e.g. "PTB Legal Time Servers",
        /// "NIST Legal Time Servers", "Local Time Servers", ...
        /// </summary>
        public String?                Group                                          { get; set; }

        /// <summary>
        /// The priority of this NTS server within the group of NTS servers.
        /// When multiple servers are configured, the Charging Station will try to synchronize
        /// with the server(s) with the highest priority/-ies first.
        /// </summary>
        public Byte?                  Priority                                       { get; set; }

        /// <summary>
        /// The weight of this NTS server within the group of NTS servers.
        /// When multiple servers are configured with the same priority, the weight can be used to
        /// define a relative distribution of synchronization requests among these servers
        /// (e.g. for load balancing or testing purposes).
        /// </summary>
        public Byte?                  Weight                                         { get; set; }

        /// <summary>
        /// The physical location of the NTS server, e.g. "PTB Braunschweig, Germany", "NIST Boulder, USA", ...
        /// </summary>
        public String?                ServerLocation                                 { get; set; }

        /// <summary>
        /// The time zone of the NTS server, e.g. "UTC", "Europe/Berlin", "America/New_York", ...
        /// </summary>
        public String?                ServerTimeZone                                 { get; set; }

        /// <summary>
        /// Configured local time offset in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?                TimeOffset                                     { get; set; }




        // Monitoring

        /// <summary>
        /// The current timestamp of the last successful time synchronization with this NTS server.
        /// </summary>
        public DateTimeOffset         CurrentTimestamp                               { get; }

        /// <summary>
        /// The timestamp of the last successful time synchronization with this NTS server.
        /// </summary>
        public DateTimeOffset?        LastSuccessfulSync                             { get; }

        /// <summary>
        /// The current accuracy of the time synchronization with this NTS server: +-X.YZ ms.
        /// </summary>
        public TimeSpan?              Accurancy                                      { get; }

        /// <summary>
        /// The current synchronization status with this NTS server, e.g. "Synchronized",
        /// "Unsynchronized", "SyncInProgress", "NoResponse", "CertificateError",
        /// "TLSHandshakeFailed", ...
        /// </summary>
        public String?                SyncStatus                                     { get; }

        /// <summary>
        /// The last error message related to synchronization with this NTS server.
        /// </summary>
        public String?                LastErrorMessage                               { get; }

        /// <summary>
        /// The timestamp of the next scheduled synchronization attempt with this NTS server.
        /// </summary>
        public DateTimeOffset?        NextScheduledSync                              { get; }


        /// <summary>
        /// The current time offset between the Charging Station and this NTS server in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?                NTP_Offset                                     { get; }

        /// <summary>
        /// The current stratum level of this NTS server, which is a measure of the distance to the reference clock.
        /// </summary>
        public Byte?                  NTP_Stratum                                    { get; }

        /// <summary>
        /// The current round-trip delay to this NTS server, which is a measure of the
        /// network latency and processing time.
        /// </summary>
        public TimeSpan?              NTP_Delay                                      { get; }

        /// <summary>
        /// The current jitter of the time synchronization with this NTS server, which is
        /// a measure of the variability of the time offset and delay measurements.
        /// </summary>
        public Double?                NTP_Jitter                                     { get; }

        // Shift register like ntpd's reachability register to track the reachability of the
        // server in the last 32 measurement attempts (1 bit per attempt, 1 = success, 0 = failure).
        //public UInt32?                Reachability                                   { get; }

        /// <summary>
        /// The current NTS Cookie Key for Client-to-Server (C2S) authentication with this NTS server.
        /// </summary>
        public String?                NTS_KE_C2S_Key                                 { get; }

        /// <summary>
        /// The current NTS Cookie Key for Server-to-Client (S2C) authentication with this NTS server.
        /// </summary>
        public String?                NTS_KE_S2C_Key                                 { get; }

        /// <summary>
        /// The current number of valid and unused NTS cookies left for
        /// authenticated NTP messages with this NTS server.
        /// </summary>
        public UInt16?                NTS_KE_NumberOfCookiesLeft                     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Network Time Security (NTS) server controller.
        /// </summary>
        /// 
        /// <param name="Enabled">If this variable reports a value of true, NTSorization is enabled.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTSServerCtrlr(DomainName             NTP_Hostname,

                              IPPort?                NTP_Port                                      = null,
                              IPVersionPreference?   NTP_IPVersionPreference                       = null,
                              Byte?                  NTP_IPHopLimit                                = null,
                              String?                NTP_Version                                   = null,
                              TimeSpan?              NTP_MinPollInterval                           = null,
                              TimeSpan?              NTP_MaxPollInterval                           = null,
                              NTPRandomizationMode?  NTP_RandomizationMode                         = null,
                              PercentageDouble?      NTP_RandomizationJitter                       = null,
                              Boolean?               NTP_AllowInitialBurst                         = null,
                              TimeSpan?              NTP_DelayAsymmetry                            = null,
                              Double?                NTP_DelayVarAsymmetry                         = null,
                              TimeSpan?              NTP_Preflight                                 = null,
                              UInt16?                NTP_RateLimit                                 = null,
                              TimeSpan?              NTP_RequestTimeout                            = null,
                              UInt16?                NTP_ErrorLoggingThreshold                     = null,

                              DomainName?            NTS_KE_Hostname                               = null,
                              IPPort?                NTS_KE_Port                                   = null,
                              IPVersionPreference?   NTS_KE_IPVersionPreference                    = null,
                              Byte?                  NTS_KE_IPHopLimit                             = null,
                              String?                NTS_KE_MinTLSVersion                          = null,
                              IEnumerable<String>?   NTS_KE_RootCACertificates                     = null,
                              String?                NTS_KE_RootCACertificate_Validation           = null,
                              String?                NTS_KE_RootCACertificate_Logging              = null,
                              IEnumerable<String>?   NTS_KE_ServerCertificates                     = null,
                              String?                NTS_KE_ServerCertificate_Validation           = null,
                              String?                NTS_KE_ServerCertificate_Logging              = null,
                              Boolean?               NTS_KE_NoTLSCertificateTimeCheckAfterReboot   = null,
                              TimeSpan?              NTS_KE_RequestTimeout                         = null,
                              UInt16?                NTS_KE_RateLimit                              = null,
                              TimeSpan?              NTS_KE_MinCookieRefreshTime                   = null,
                              UInt16?                NTS_KE_DesiredNumberOfCookies                 = null,

                              Boolean?               Enabled                                       = null,
                              String?                Instance                                      = null, // = NTP_Hostname?
                              String?                Group                                         = null,
                              Byte?                  Priority                                      = null,
                              Byte?                  Weight                                        = null,
                              String?                ServerLocation                                = null,
                              String?                ServerTimeZone                                = null,



                              String?                NTS_KE_C2S_Key                        = null,
                              String?                NTS_KE_S2C_Key                        = null,
                              UInt16?                NTS_KE_NumberOfCookiesLeft               = null,

                              String?                NTP_Offset                            = null,
                              Byte?                  NTP_Stratum                           = null,

                              TimeSpan?              NTP_Delay                             = null,
                              Double?                NTP_Jitter                            = null,
                              DateTimeOffset?        LastSuccessfulSync                    = null,
                              String?                SyncStatus                            = null,

                              CustomData?            CustomData                            = null)

            : base(nameof(NTSServerCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration of a single Network Time Security (NTS) server."),
                   CustomData)

        {

            this.NTP_Hostname                                 = NTP_Hostname;

            this.NTP_Port                                     = NTP_Port;
            this.NTP_IPVersionPreference                      = NTP_IPVersionPreference;
            this.NTP_IPHopLimit                               = NTP_IPHopLimit;
            this.NTP_Version                                  = NTP_Version;
            this.NTP_MinPollInterval                          = NTP_MinPollInterval;
            this.NTP_MaxPollInterval                          = NTP_MaxPollInterval;
            this.NTP_RandomizationMode                        = NTP_RandomizationMode;
            this.NTP_RandomizationJitter                      = NTP_RandomizationJitter;
            this.NTP_AllowInitialBurst                        = NTP_AllowInitialBurst;
            this.NTP_DelayAsymmetry                           = NTP_DelayAsymmetry;
            this.NTP_DelayVarAsymmetry                        = NTP_DelayVarAsymmetry;
            this.NTP_Preflight                                = NTP_Preflight;
            this.NTP_RateLimit                                = NTP_RateLimit;
            this.NTP_RequestTimeout                           = NTP_RequestTimeout;
            this.NTP_ErrorLoggingThreshold                    = NTP_ErrorLoggingThreshold;

            this.NTS_KE_Hostname                              = NTS_KE_Hostname;
            this.NTS_KE_Port                                  = NTS_KE_Port;
            this.NTS_KE_IPVersionPreference                   = NTS_KE_IPVersionPreference;
            this.NTS_KE_IPHopLimit                            = NTS_KE_IPHopLimit;
            this.NTS_KE_MinTLSVersion                         = NTS_KE_MinTLSVersion;
            this.NTS_KE_RootCACertificates                    = NTS_KE_RootCACertificates?.Distinct() ?? [];
            this.NTS_KE_RootCACertificate_Validation          = NTS_KE_RootCACertificate_Validation;
            this.NTS_KE_RootCACertificate_Logging             = NTS_KE_RootCACertificate_Logging;
            this.NTS_KE_ServerCertificates                    = NTS_KE_ServerCertificates?.Distinct() ?? [];
            this.NTS_KE_ServerCertificate_Validation          = NTS_KE_ServerCertificate_Validation;
            this.NTS_KE_ServerCertificate_Logging             = NTS_KE_ServerCertificate_Logging;
            this.NTS_KE_NoTLSCertificateTimeCheckAfterReboot  = NTS_KE_NoTLSCertificateTimeCheckAfterReboot;
            this.NTS_KE_RequestTimeout                        = NTS_KE_RequestTimeout;
            this.NTS_KE_RateLimit                             = NTS_KE_RateLimit;
            this.NTS_KE_MinCookieRefreshTime                  = NTS_KE_MinCookieRefreshTime;
            this.NTS_KE_DesiredNumberOfCookies                = NTS_KE_DesiredNumberOfCookies;


            this.Enabled                                      = Enabled;
            this.Group                                        = Group;
            this.Priority                                     = Priority;
            this.Weight                                       = Weight;
            this.ServerLocation                               = ServerLocation;
            this.ServerTimeZone                               = ServerTimeZone;


            // Monitoring
            this.NTS_KE_C2S_Key                               = NTS_KE_C2S_Key;
            this.NTS_KE_S2C_Key                               = NTS_KE_S2C_Key;
            this.NTS_KE_NumberOfCookiesLeft                   = NTS_KE_NumberOfCookiesLeft;

            this.NTP_Offset                                   = NTP_Offset;
            this.NTP_Stratum                                  = NTP_Stratum;

            this.NTP_Delay                                    = NTP_Delay;
            this.NTP_Jitter                                   = NTP_Jitter;
            this.LastSuccessfulSync                           = LastSuccessfulSync;
            this.SyncStatus                                   = SyncStatus;



            //#region Enabled

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "Enabled",
            //        ValueGetter:      () => this.Enabled.HasValue
            //                                    ? this.Enabled.Value
            //                                          ? "true"
            //                                          : "false"
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("If set to FALSE, then authorization is switched off. Transactions are still possible, but no authorization will take place. This implies, that the value of idToken in transaction events SHALL be NoNTSorization.")

            //    )
            //);

            //#endregion

            //#region AdditionalInfoItemsPerMessage

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "AdditionalInfoItemsPerMessage",
            //        ValueGetter:      () => this.AdditionalInfoItemsPerMessage?.ToString(),

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadOnly
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Integer
            //                          ),

            //        Description:      I18NString.Create("Maximum number of AdditionalInfo items that can be sent in one message.")

            //    )
            //);

            //#endregion

            //#region NTSorizeRemoteStart

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "NTSorizeRemoteStart",
            //        ValueGetter:      () => this.NTSorizeRemoteStart
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.")

            //    )
            //);

            //#endregion

            //#region DisableRemoteNTSorization

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "DisableRemoteNTSorization",
            //        ValueGetter:      () => this.DisableRemoteNTSorization == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("When set to true this instructs the Charging Station to not issue any NTSorizationRequests, but only use NTSorization Cache and Local NTSorization List to determine validity of idTokens.")

            //    )
            //);

            //#endregion

            //#region LocalNTSorizeOffline

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "LocalNTSorizeOffline",
            //        ValueGetter:      () => this.LocalNTSorizeOffline == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.")

            //    )
            //);

            //#endregion

            //#region LocalPreNTSorize

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "LocalPreNTSorize",
            //        ValueGetter:      () => this.LocalPreNTSorize == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an NTSorizeResponse from the CSMS.")

            //    )
            //);

            //#endregion

            //#region MasterPassGroupId

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "MasterPassGroupId",
            //        ValueGetter:      () => this.MasterPassGroupId,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String,
            //                              MaxLimit:    36
            //                          ),

            //        Description:      I18NString.Create("IdTokens that have this id as groupId belong to the Master Pass Group. Meaning they can stop any ongoing transaction, but cannot start transactions. This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.")

            //    )
            //);

            //#endregion

            //#region OfflineTxForUnknownIdEnabled

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "OfflineTxForUnknownIdEnabled",
            //        ValueGetter:      () => this.Enabled == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("If this key exists, the charging station supports Unknown Offline NTSorization. If this key reports a value of true, Unknown Offline NTSorization is enabled.")

            //    )
            //);

            //#endregion


        }

        #endregion



    }

}
