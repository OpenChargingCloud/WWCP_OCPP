﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Provides a means to configure management of time tracking by charging station.
    /// </summary>
    public class ClockCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Contains the current date and time.
        /// </summary>
        public DateTime?  DateTime                            { get; set; }

        /// <summary>
        /// This contains the address of the NTP server. Multiple NTP servers can be configured as backups, etc. If the NTP client supports it, it can also connect to multiple NTP servers simultaneous to get a more reliable time source. Variable instance value is single digit NTP priority (1=highest).
        /// </summary>
        public String?    NtpServerUri                        { get; set; }

        /// <summary>
        /// When an NTP client is implemented, this variable can be used to configure the client: Use the NTP server provided via DHCP, or use the manually configured NTP server.
        /// </summary>
        public String?    NtpSource                           { get; set; }

        /// <summary>
        /// Configured local time offset in the format: "+01:00", "-02:00" etc.
        /// </summary>
        public String?    TimeOffset                          { get; set; }

        /// <summary>
        /// Date time of the next time offset transition.
        /// </summary>
        public DateTime?  NextTimeOffsetTransitionDateTime    { get; set; }

        /// <summary>
        /// Via this variable, the Charging Station provides the CSMS with the option to configure a clock source, if more than 1 are implemented.
        /// </summary>
        public String?    TimeSource                          { get; set; }

        /// <summary>
        /// Configured current local time zone in the format: "Europe/Oslo", "Asia/Singapore" etc.
        /// </summary>
        public String?    TimeZone                            { get; set; }

        /// <summary>
        /// If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.
        /// </summary>
        public Int32?     TimeAdjustmentReportingThreshold    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clock controller.
        /// </summary>
        /// <param name="DateTime">Contains the current date and time.</param>
        /// <param name="NtpServerUri">This contains the address of the NTP server. Multiple NTP servers can be configured as backups, etc. If the NTP client supports it, it can also connect to multiple NTP servers simultaneous to get a more reliable time source. Variable instance value is single digit NTP priority (1=highest).</param>
        /// <param name="NtpSource">When an NTP client is implemented, this variable can be used to configure the client: Use the NTP server provided via DHCP, or use the manually configured NTP server.</param>
        /// <param name="TimeOffset">Configured local time offset in the format: "+01:00", "-02:00" etc.</param>
        /// <param name="NextTimeOffsetTransitionDateTime">Date time of the next time offset transition.</param>
        /// <param name="TimeSource">Via this variable, the Charging Station provides the CSMS with the option to configure a clock source, if more than 1 are implemented.</param>
        /// <param name="TimeZone">Configured current local time zone in the format: "Europe/Oslo", "Asia/Singapore" etc.</param>
        /// <param name="TimeAdjustmentReportingThreshold">If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClockCtrlr(DateTime?     DateTime,
                          String?       NtpServerUri,
                          String?       NtpSource,
                          String?       TimeOffset,
                          DateTime?     NextTimeOffsetTransitionDateTime,
                          String?       TimeSource,
                          String?       TimeZone,
                          Int32?        TimeAdjustmentReportingThreshold,

                          String?       Instance     = null,
                          CustomData?   CustomData   = null)

            : base(nameof(ClockCtrlr),
                   Instance,
                   new[] {

                       #region DateTime

                       new VariableConfig(

                           Name:              "DateTime",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("Contains the current date and time."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NtpServerUri

                       new VariableConfig(

                           Name:              "NtpServerUri",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("This contains the address of the NTP server. Multiple NTP servers can be configured as backups, etc. If the NTP client supports it, it can also connect to multiple NTP servers simultaneous to get a more reliable time source. Variable instance value is single digit NTP priority (1=highest)."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NtpSource

                       new VariableConfig(

                           Name:              "NtpSource",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("When an NTP client is implemented, this variable can be used to configure the client: Use the NTP server provided via DHCP, or use the manually configured NTP server."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TimeOffset

                       new VariableConfig(

                           Name:              "TimeOffset",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("Configured local time offset in the format: \"+01:00\", \"-02:00\" etc."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NextTimeOffsetTransitionDateTime

                       new VariableConfig(

                           Name:              "NextTimeOffsetTransitionDateTime",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.DateTime
                                                   )
                                               },

                           Description:       I18NString.Create("Date time of the next time offset transition."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TimeSource

                       new VariableConfig(

                           Name:              "TimeSource",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("Via this variable, the Charging Station provides the CSMS with the option to configure a clock source, if more than 1 are implemented."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TimeZone

                       new VariableConfig(

                           Name:              "TimeZone",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("Configured current local time zone in the format: \"Europe/Oslo\", \"Asia/Singapore\" etc."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TimeAdjustmentReportingThreshold

                       new VariableConfig(

                           Name:              "TimeAdjustmentReportingThreshold",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("If set, then time adjustments with an absolute value in seconds larger than this need to be reported as a security event SettingSystemTime."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Provides a means to configure management of time tracking by charging station."),
                   CustomData)

        {

            this.DateTime                          = DateTime;
            this.NtpServerUri                      = NtpServerUri;
            this.NtpSource                         = NtpSource;
            this.TimeOffset                        = TimeOffset;
            this.NextTimeOffsetTransitionDateTime  = NextTimeOffsetTransitionDateTime;
            this.TimeSource                        = TimeSource;
            this.TimeZone                          = TimeZone;
            this.TimeAdjustmentReportingThreshold  = TimeAdjustmentReportingThreshold;

        }

        #endregion


    }

}
