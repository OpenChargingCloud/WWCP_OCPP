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

using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Norn.NTS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A Network Time Synchronization controller for Network Time Protocol (NTPv4)
    /// and Network Time Security (NTS).
    /// 
    /// Well-known instances are:
    ///   - "legal" for legal time because of dynamic tariffs et.al
    ///   - "local" for local load balancing and time servers on Local Controllers
    /// </summary>
    public class NTPClientController : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether the NTP/NTS server is enabled or not.
        /// </summary>
        public Boolean                  Enabled                             { get; }


        /// <summary>
        /// The NTP/NTS server URL.
        /// May depend on the device’s country or location.
        /// </summary>
        public URL                      ServerURL                           { get; }

        /// <summary>
        /// The optional NTP/NTS server port.
        /// </summary>
        public IPPort?                  ServerPort                          { get; }

        /// <summary>
        /// The optional NTP/NTS server mode.
        /// NTPv4 may be prohibited unless additional security measures are implemented!
        /// </summary>
        public NTPMode                  Mode                                { get; }

        /// <summary>
        /// These values specifies a correction (in milliseconds) which will be applied to measured time offsets.
        /// This can compensate known stable asymmetries in network or processing delays.
        /// For example, if packets sent to the source were on average delayed by 100 microseconds more
        /// than packets sent from the source back, the correction would be -0.05 (-50 microseconds).
        /// </summary>
        public TimeSpan?                DelayAsymmetry                      { get; }

        /// <summary>
        /// These values can fine-tune the offset calculations when network delay variability is greater
        /// in one direction than the other.
        /// Use only if you know your network has a consistent asymmetry.
        /// </summary>
        public Double?                  DelayVarAsymmetry                   { get; }

        /// <summary>
        /// The optional priority of the NTP/NTS server.
        /// Servers with lower values are queried first.
        /// Servers with the same value are queried in parallel.
        /// </summary>
        public Byte?                    Priority                            { get; }

        /// <summary>
        /// The minimal time span between randomized NTP/NTS time sync requests.
        /// </summary>
        public TimeSpan?                MinInterval                         { get; }

        /// <summary>
        /// The maximal time span between randomized NTP/NTS time sync requests.
        /// </summary>
        public TimeSpan?                MaxInterval                         { get; }

        /// <summary>
        /// Occasional requests to an NTP/NTS server may be delayed due to network caching effects such
        /// as ARP or DNS resolution, firewall state establishment, TLS tunnel setup, and similar factors.
        /// To prevent inaccurate delay measurements, a preflight NTP packet is sent and its response
        /// ignored before the actual measurement takes place.
        /// The configured values define the time intervals since the last measurement that trigger
        /// sending preflight NTP packets.
        /// </summary>
        public TimeSpan?                Preflight                           { get; }

        /// <summary>
        /// The number of consecutive measurement errors that should lead to an entry within the security log book.
        /// When the measurements recovered from the error another log book entry shall be added.
        /// </summary>
        public UInt16?                  ErrorLogging                        { get; }


        /// <summary>
        /// The optional NTSKE server URL.
        /// When empty and the mode is NTSv4, the ServerURL will be used for NTS-KE.
        /// </summary>
        public URL?                     NTSKEServerURL                      { get; }

        /// <summary>
        /// The optional NTP/NTS server port.
        /// </summary>
        public IPPort?                  NTSKEServerPort                     { get; }

        public String?                  RootCAs                             { get; }

        public Boolean?                 NoCertTimeCheckAfterReboot          { get; }

        /// <summary>
        /// Refreshing the NTS keys and cookies should be started after the given time span
        /// since the last NTS-KE handshakes (randomly between given min and max values).
        /// </summary>
        public TimeSpan?                MinRefresh                          { get; }

        /// <summary>
        /// Refreshing the NTS keys and cookies must be completed within the given time span
        /// since the last NTS-KE handshakes (randomly between given min and max values).
        /// </summary>
        public TimeSpan?                MaxRefresh                          { get; }

        public AEADAlgorithms?          AEADAlgorithm                       { get; }

        public Boolean?                 SignedResponses                     { get; }



        /// <summary>
        /// Configured local time offset in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?                  TimeOffset                          { get; set; }



        /// <summary>
        /// Contains the current date and time.
        /// </summary>
        [Mandatory]
        public DateTimeOffset           DateTime                            { get; }

        public Double                   Accurancy                           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Network Time Synchronization controller.
        /// </summary>
        /// <param name="Id">The identification of the NTP client controller.</param>
        /// <param name="ServerURL">The URL of the NTP server.</param>
        /// 
        /// <param name="TimeOffset">Configured local time offset in the format: "+01:00", "-02:00" etc.</param>
        /// <param name="DateTime">Contains the current date and time.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTPClientController(String           Id,
                                   URL              ServerURL,

                                   Boolean?         Enabled                      = null,
                                   I18NString?      Description                  = null,
                                   IPPort?          ServerPort                   = null,
                                   NTPMode?         Mode                         = null,

                                   TimeSpan?        DelayAsymmetry               = null,
                                   Double?          DelayVarAsymmetry            = null,
                                   Byte?            Priority                     = null,
                                   TimeSpan?        MinInterval                  = null,
                                   TimeSpan?        MaxInterval                  = null,
                                   TimeSpan?        Preflight                    = null,

                                   URL?             NTSKEServerURL               = null,
                                   IPPort?          NTSKEServerPort              = null,
                                   String?          RootCAs                      = null,
                                   Boolean?         NoCertTimeCheckAfterReboot   = null,
                                   TimeSpan?        MinRefresh                   = null,
                                   TimeSpan?        MaxRefresh                   = null,
                                   AEADAlgorithms?  AEADAlgorithm                = null,
                                   Boolean?         SignedResponses              = null,


                                   String?          TimeOffset                         = null,

                                   DateTime?        DateTime                           = null,

                                   CustomData?      CustomData                         = null)

            : base(nameof(NTPClientController),
                   Id,
                   Description ?? I18NString.Create("A Network Time Synchronization controller for Network Time Protocol (NTPv4) and Network Time Security (NTS)."),
                   CustomData)

        {

            this.ServerURL                         = ServerURL;

            this.Enabled                           = Enabled                    ?? true;
            this.ServerPort                        = ServerPort                 ?? IPPort.Parse(123);
            this.DelayAsymmetry                    = DelayAsymmetry;
            this.DelayVarAsymmetry                 = DelayVarAsymmetry;
            this.Priority                          = Priority;
            this.MinInterval                       = MinInterval;
            this.MaxInterval                       = MaxInterval;
            this.Preflight                         = Preflight;

            this.NTSKEServerURL                    = NTSKEServerURL;
            this.NTSKEServerPort                   = NTSKEServerPort            ?? IPPort.Parse(4460);
            this.RootCAs                           = RootCAs;
            this.NoCertTimeCheckAfterReboot        = NoCertTimeCheckAfterReboot ?? false;
            this.MinRefresh                        = MinRefresh;
            this.MaxRefresh                        = MaxRefresh;
            this.AEADAlgorithm                     = AEADAlgorithm;
            this.SignedResponses                   = SignedResponses;

            this.Mode                              = Mode                       ?? (NTSKEServerURL is not null
                                                                                       ? NTPMode.NTSv4
                                                                                       : NTPMode.NTPv4);


            this.TimeOffset                        = TimeOffset;

            this.DateTime                          = DateTime ?? Timestamp.Now;


            #region DateTime

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DateTime",
                    ValueGetter:      () => this.DateTime.ToISO8601(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.DateTime
                                      ),

                    Description:      I18NString.Create("Contains the current date and time.")

                )
            );

            #endregion

            #region TimeSource

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "TimeSource",
            //        ValueGetter:      () => this.TimeSource.Any()
            //                                    ? this.TimeSource.AggregateWith(',')
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create(
            //                              "Via this variable, the Charging Station provides the CSMS with the option to configure a clock source, if more than 1 are implemented. " +
            //                              "\"NTP,Heartbeat\" means, use NTP, but when none of the NTP servers responses, use time synchronization via Heartbeat."
            //                          )

            //    )
            //);

            #endregion

            #region NtpServerUri

            //variableConfigs.Add(
            //    new VariableConfig(

            //        // Single digit, multiple servers allowed, primary NtpServer has instance '1', the secondary has instance '2'. etc
            //        Name:             "NtpServerUri",
            //        ValueGetter:      () => this.NtpServerUri,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create(
            //                              "This contains the address of the NTP server. Multiple NTP servers can be configured as backups, etc. " +
            //                              "If the NTP client supports it, it can also connect to multiple NTP servers simultaneous to get a " +
            //                              "more reliable time source. Variable instance value is single digit NTP priority (1=highest)."
            //                          )

            //    )
            //);

            #endregion

            #region NtpSource

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "NtpSource",
            //        ValueGetter:      () => this.NtpSource.HasValue
            //                                    ? this.NtpSource.Value.AsText()
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create(
            //                              "When an NTP client is implemented, this variable can be used to configure the client: " +
            //                              "Use the NTP server provided via DHCP, or use the manually configured NTP server."
            //                          )

            //    )
            //);

            #endregion

            #region TimeOffset

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TimeOffset",
                    ValueGetter:      () => this.TimeOffset,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create(
                                          "Configured local time offset in the format: \"+01:00\", \"-02:00\" etc. " +
                                          "When a TimeOffset is used, it is advised not to implement: TimeZone. " +
                                          "If a Charging Station has implemented both TimeOffset and TimeZone it is RECOMMENDED to not use both at the same time."
                                      )

                )
            );

            #endregion

            #region NextTimeOffsetTransitionDateTime

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "NextTimeOffsetTransitionDateTime",
            //        ValueGetter:      () => this.NextTimeOffsetTransitionDateTime.HasValue
            //                                    ? this.NextTimeOffsetTransitionDateTime.Value.ToISO8601()
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.DateTime
            //                          ),

            //        Description:      I18NString.Create(
            //                              "Date time of the next time offset transition. On this date time, the clock displayed to the EV driver " +
            //                              "will be given the new offset as configured via 'TimeOffsetNextTransition'. " +
            //                              "This can be used to manually configure the next start or end of a daylight saving time period."
            //                          )

            //    )
            //);

            #endregion

            #region TimeOffsetNextTransition

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "TimeOffsetNextTransition",
            //        Instance:         "NextTransition",
            //        ValueGetter:      () => this.NextTimeOffsetTransition,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create(
            //                              "Next local time offset in the format: \"+01:00\", \"-02:00\" etc. New offset that will be set on the next time " +
            //                              "offset transition as configured via 'NextTimeOffsetTransitionDateTime'. " +
            //                              "This can be used to manually configure the offset for the start or end of the daylight saving time period."
            //                          )

            //    )
            //);

            #endregion

            #region TimeZone

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "TimeZone",
            //        ValueGetter:      () => this.TimeZone,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create("Configured current local time zone in the format: \"Europe/Oslo\", \"Asia/Singapore\" etc.")

            //    )
            //);

            #endregion

            #region TimeAdjustmentReportingThreshold

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "TimeAdjustmentReportingThreshold",
            //        ValueGetter:      () => this.TimeAdjustmentReportingThreshold.HasValue
            //                                    ? this.TimeAdjustmentReportingThreshold.Value.ToString()
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String
            //                          ),

            //        Description:      I18NString.Create("If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.")

            //    )
            //);

            #endregion


        }

        #endregion


    }

}
