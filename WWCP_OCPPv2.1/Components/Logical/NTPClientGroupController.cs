﻿/*
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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A generic Time Synchronization controller.
    /// 
    /// Well-known instances are:
    ///   - "legal" for legal time because of dynamic tariffs et.al
    ///   - "local" for local load balancing and time servers on Local Controllers
    /// </summary>
    public class NTPClientGroupController : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Contains the current date and time.
        /// </summary>
        [Mandatory]
        public DateTime                          DateTime                 { get; }



        public IEnumerable<NTPClientController>  NetworkTimeClients       { get; }

        public Boolean?                          AllowJumpsAfterReboot    { get; }


        public Byte?                             MinServers               { get; }

        public TimeSpan MaxDeviation { get; }

        public UInt16? ErrorLogging { get; }

        public UInt16? MaxLogging { get; }



        /// <summary>
        /// Configured local time offset in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?                  TimeOffset                          { get; set; }

        /// <summary>
        /// Date time of the next time offset transition.
        /// </summary>
        public DateTime?                NextTimeOffsetTransitionDateTime    { get; set; }

        /// <summary>
        /// Next local time offset in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?                  NextTimeOffsetTransition            { get; set; }

        /// <summary>
        /// Configured current local time zone in the format: "Europe/Oslo", "Asia/Singapore" etc.
        /// </summary>
        public String?                  TimeZone                            { get; set; }

        /// <summary>
        /// If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.
        /// </summary>
        public Int32?                   TimeAdjustmentReportingThreshold    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Network Time Synchronization controller.
        /// </summary>
        /// <param name="DateTime">Contains the current date and time.</param>
        /// <param name="TimeOffset">Configured local time offset in the format: "+01:00", "-02:00" etc.</param>
        /// <param name="NextTimeOffsetTransitionDateTime">Date time of the next time offset transition.</param>
        /// <param name="NextTimeOffsetTransition">Next local time offset in the format: "+01:00", "-02:00" etc.</param>
        /// <param name="TimeZone">Configured current local time zone in the format: "Europe/Oslo", "Asia/Singapore" etc.</param>
        /// <param name="TimeAdjustmentReportingThreshold">If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTPClientGroupController(String                            Instance,
                                        IEnumerable<NTPClientController>  NetworkTimeClients,

                                        I18NString?                       Description                        = null,

                                        String?                           TimeOffset                         = null,
                                        DateTime?                         NextTimeOffsetTransitionDateTime   = null,
                                        String?                           NextTimeOffsetTransition           = null,
                                        String?                           TimeZone                           = null,
                                        Int32?                            TimeAdjustmentReportingThreshold   = null,

                                        DateTime?                         DateTime                           = null,

                                        CustomData?                       CustomData                         = null)

            : base(nameof(NTPClientGroupController),
                   Instance,
                   Description ?? I18NString.Create("A Network Time Synchronization Group controller for Network Time Protocol (NTPv4) and Network Time Security (NTS)."),
                   CustomData)

        {

            this.NetworkTimeClients                = NetworkTimeClients.Distinct();

            this.TimeOffset                        = TimeOffset;
            this.NextTimeOffsetTransitionDateTime  = NextTimeOffsetTransitionDateTime;
            this.NextTimeOffsetTransition          = NextTimeOffsetTransition;
            this.TimeZone                          = TimeZone;
            this.TimeAdjustmentReportingThreshold  = TimeAdjustmentReportingThreshold;

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

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "NextTimeOffsetTransitionDateTime",
                    ValueGetter:      () => this.NextTimeOffsetTransitionDateTime.HasValue
                                                ? this.NextTimeOffsetTransitionDateTime.Value.ToISO8601()
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.DateTime
                                      ),

                    Description:      I18NString.Create(
                                          "Date time of the next time offset transition. On this date time, the clock displayed to the EV driver " +
                                          "will be given the new offset as configured via 'TimeOffsetNextTransition'. " +
                                          "This can be used to manually configure the next start or end of a daylight saving time period."
                                      )

                )
            );

            #endregion

            #region TimeOffsetNextTransition

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TimeOffsetNextTransition",
                    Instance:         "NextTransition",
                    ValueGetter:      () => this.NextTimeOffsetTransition,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create(
                                          "Next local time offset in the format: \"+01:00\", \"-02:00\" etc. New offset that will be set on the next time " +
                                          "offset transition as configured via 'NextTimeOffsetTransitionDateTime'. " +
                                          "This can be used to manually configure the offset for the start or end of the daylight saving time period."
                                      )

                )
            );

            #endregion

            #region TimeZone

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TimeZone",
                    ValueGetter:      () => this.TimeZone,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("Configured current local time zone in the format: \"Europe/Oslo\", \"Asia/Singapore\" etc.")

                )
            );

            #endregion

            #region TimeAdjustmentReportingThreshold

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TimeAdjustmentReportingThreshold",
                    ValueGetter:      () => this.TimeAdjustmentReportingThreshold.HasValue
                                                ? this.TimeAdjustmentReportingThreshold.Value.ToString()
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.")

                )
            );

            #endregion


        }

        #endregion


    }

}
