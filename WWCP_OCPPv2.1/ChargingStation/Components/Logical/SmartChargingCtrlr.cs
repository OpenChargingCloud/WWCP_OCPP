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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to smart charging.
    /// </summary>
    public class SmartChargingCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether Smart Charging is enabled.
        /// </summary>
        public Boolean?            Enabled                             { get; set; }

        /// <summary>
        /// Whether Smart Charging is supported.
        /// </summary>
        public Boolean?            Available                           { get; set; }

        /// <summary>
        /// If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging.
        /// </summary>
        public Boolean?            ACPhaseSwitchingSupported           { get; set; }

        /// <summary>
        /// Max StackLevel of a ChargingProfile. The number defined also indicates the max allowed number of installed charging schedules per Charging Profile Purposes.
        /// </summary>
        public Int16?              ProfileStackLevel                   { get; set; }

        /// <summary>
        /// A list of supported quantities for use in a ChargingSchedule. Allowed values: 'A' and 'W'.
        /// </summary>
        public IEnumerable<Char>?  RateUnit                            { get; set; }

        /// <summary>
        /// Maximum number of periods that may be defined per ChargingSchedule.
        /// </summary>
        public UInt16?             PeriodsPerSchedule                  { get; set; }

        /// <summary>
        /// Indicates whether a Charging Station should respond to external control signals that influence charging.
        /// </summary>
        public Boolean?            ExternalControlSignalsEnabled       { get; set; }

        /// <summary>
        /// Indicates if the Charging Station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message.
        /// This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval.
        /// Default is false when omitted.
        /// </summary>
        public Boolean?            NotifyChargingLimitWithSchedules    { get; set; }

        /// <summary>
        /// If defined and true, this Charging Station supports switching from 3 to 1 phase during a transaction.
        /// </summary>
        public Boolean?            Phases3to1                          { get; set; }

        /// <summary>
        /// Amount of Charging profiles currently installed on the Charging Station.
        /// MaxLimit used to limit number of Charging profiles installed at any time.
        /// </summary>
        public Byte?               Entries                             { get; set; }

        /// <summary>
        /// If at the Charging Station side a change in the limit in a ChargingProfile is lower than this percentage, the Charging Station MAY skip sending a NotifyChargingLimitRequest or a TransactionEventRequest message to the CSMS.
        /// It is RECOMMENDED to set this key to a low value.
        /// See Smart Charging signals to a Charging Station from multiple actors.
        /// </summary>
        public Byte?               LimitChangeSignificance             { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new smart charging controller.
        /// </summary>
        /// <param name="Enabled">Whether Smart Charging is enabled.</param>
        /// <param name="Available">Whether Smart Charging is supported.</param>
        /// <param name="ACPhaseSwitchingSupported">If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging.</param>
        /// <param name="ProfileStackLevel">Max StackLevel of a ChargingProfile. The number defined also indicates the max allowed number of installed charging schedules per Charging Profile Purposes.</param>
        /// <param name="RateUnit">A list of supported quantities for use in a ChargingSchedule. Allowed values: 'A' and 'W'.</param>
        /// <param name="PeriodsPerSchedule">Maximum number of periods that may be defined per ChargingSchedule.</param>
        /// <param name="ExternalControlSignalsEnabled">Indicates whether a Charging Station should respond to external control signals that influence charging.</param>
        /// <param name="NotifyChargingLimitWithSchedules">Indicates if the Charging Station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message. This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval. Default is false when omitted.</param>
        /// <param name="Phases3to1">If defined and true, this Charging Station supports switching from 3 to 1 phase during a transaction.</param>
        /// <param name="Entries">Amount of Charging profiles currently installed on the Charging Station. MaxLimit used to limit number of Charging profiles installed at any time.</param>
        /// <param name="LimitChangeSignificance"></param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SmartChargingCtrlr(Boolean?            Enabled                            = null,
                                  Boolean?            Available                          = null,
                                  Boolean?            ACPhaseSwitchingSupported          = null,
                                  Int16?              ProfileStackLevel                  = null,
                                  IEnumerable<Char>?  RateUnit                           = null,
                                  UInt16?             PeriodsPerSchedule                 = null,
                                  Boolean?            ExternalControlSignalsEnabled      = null,
                                  Boolean?            NotifyChargingLimitWithSchedules   = null,
                                  Boolean?            Phases3to1                         = null,
                                  Byte?               Entries                            = null,
                                  Byte?               LimitChangeSignificance            = null,

                                  String?             Instance                           = null,
                                  CustomData?         CustomData                         = null)

            : base(nameof(SmartChargingCtrlr),
                   Instance,
                   new[] {

                       #region Enabled

                       new VariableConfig(

                           Name:              "Enabled",
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

                           Description:       I18NString.Create("Whether Smart Charging is enabled."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Available

                       new VariableConfig(

                           Name:              "Available",
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

                           Description:       I18NString.Create("Whether Smart Charging is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ACPhaseSwitchingSupported

                       new VariableConfig(

                           Name:              "ACPhaseSwitchingSupported",
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

                           Description:       I18NString.Create("If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ProfileStackLevel

                       new VariableConfig(

                           Name:              "ProfileStackLevel",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Max StackLevel of a ChargingProfile. The number defined also indicates the max allowed number of installed charging schedules per Charging Profile Purposes."),

                           CustomData:        null

                       ),

                       #endregion

                       #region RateUnit

                       new VariableConfig(

                           Name:              "RateUnit",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("A list of supported quantities for use in a ChargingSchedule. Allowed values: 'A' and 'W'."),

                           CustomData:        null

                       ),

                       #endregion

                       #region PeriodsPerSchedule

                       new VariableConfig(

                           Name:              "PeriodsPerSchedule",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Maximum number of periods that may be defined per ChargingSchedule."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ExternalControlSignalsEnabled

                       new VariableConfig(

                           Name:              "ExternalControlSignalsEnabled",
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

                           Description:       I18NString.Create("Indicates whether a Charging Station should respond to external control signals that influence charging."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NotifyChargingLimitWithSchedules

                       new VariableConfig(

                           Name:              "NotifyChargingLimitWithSchedules",
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

                           Description:       I18NString.Create("Indicates if the Charging Station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message. This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval. Default is false when omitted."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Phases3to1

                       new VariableConfig(

                           Name:              "Phases3to1",
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

                           Description:       I18NString.Create("If defined and true, this Charging Station supports switching from 3 to 1 phase during a transaction."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Entries

                       new VariableConfig(

                           Name:              "Entries",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Amount of Charging profiles currently installed on the Charging Station. MaxLimit used to limit number of Charging profiles installed at any time."),

                           CustomData:        null

                       ),

                       #endregion

                       #region LimitChangeSignificance

                       new VariableConfig(

                           Name:              "LimitChangeSignificance",
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

                           Description:       I18NString.Create("If at the Charging Station side a change in the limit in a ChargingProfile is lower than this percentage, the Charging Station MAY skip sending a NotifyChargingLimitRequest or a TransactionEventRequest message to the CSMS. It is RECOMMENDED to set this key to a low value. See Smart Charging signals to a Charging Station from multiple actors."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to smart charging."),
                   CustomData)

        {

            this.Enabled                           = Enabled;
            this.Available                         = Available;
            this.ACPhaseSwitchingSupported         = ACPhaseSwitchingSupported;
            this.ProfileStackLevel                 = ProfileStackLevel;
            this.RateUnit                          = RateUnit;
            this.PeriodsPerSchedule                = PeriodsPerSchedule;
            this.ExternalControlSignalsEnabled     = ExternalControlSignalsEnabled;
            this.NotifyChargingLimitWithSchedules  = NotifyChargingLimitWithSchedules;
            this.Phases3to1                        = Phases3to1;
            this.Entries                           = Entries;
            this.LimitChangeSignificance           = LimitChangeSignificance;

        }

        #endregion


    }

}
