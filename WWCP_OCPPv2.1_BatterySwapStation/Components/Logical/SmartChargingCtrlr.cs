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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.BSS
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
        public Boolean?                        Enabled                             { get; set; }

        /// <summary>
        /// Whether Smart Charging is supported.
        /// </summary>
        public Boolean?                        Available                           { get; set; }

        /// <summary>
        /// If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging.
        /// </summary>
        public Boolean?                        ACPhaseSwitchingSupported           { get; set; }

        /// <summary>
        /// Max stack level of a charging profile.
        /// The number defined also indicates the max allowed number of installed charging schedules per charging profile purpose.
        /// </summary>
        [Mandatory]
        public UInt16                          ProfileStackLevel                   { get; set; }

        /// <summary>
        /// A list of supported quantities for use in a charging schedule.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingRateUnits>  RateUnit                            { get; set; }

        /// <summary>
        /// Maximum number of periods that may be defined per charging schedule.
        /// </summary>
        [Mandatory]
        public UInt16                          PeriodsPerSchedule                  { get; set; }

        /// <summary>
        /// Indicates whether a battery swap station should respond to external control signals that influence charging.
        /// </summary>
        public Boolean?                        ExternalControlSignalsEnabled       { get; set; }

        /// <summary>
        /// Indicates if the battery swap station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message.
        /// This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval.
        /// Default is false when omitted.
        /// </summary>
        public Boolean?                        NotifyChargingLimitWithSchedules    { get; set; }

        /// <summary>
        /// If defined and true, this battery swap station supports switching from 3 to 1 phase during a transaction.
        /// </summary>
        public Boolean?                        Phases3to1                          { get; set; }

        /// <summary>
        /// Amount of charging profiles currently installed on the battery swap station.
        /// MaxLimit used to limit number of charging profiles installed at any time.
        /// </summary>
        [Mandatory]
        public UInt16                          Entries                             { get; set; }

        /// <summary>
        /// If at the battery swap station side a change in the limit in a charging profile is lower than this percentage,
        /// the battery swap station MAY skip sending a NotifyChargingLimitRequest or a TransactionEventRequest message to the CSMS.
        /// It is RECOMMENDED to set this key to a low value.
        /// See Smart Charging signals to a battery swap station from multiple actors.
        /// </summary>
        [Mandatory]
        public Percentage                      LimitChangeSignificance             { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new smart charging controller.
        /// </summary>
        /// <param name="Enabled">Whether Smart Charging is enabled.</param>
        /// <param name="Available">Whether Smart Charging is supported.</param>
        /// <param name="ACPhaseSwitchingSupported">If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging.</param>
        /// <param name="ProfileStackLevel">Max StackLevel of a charging profile. The number defined also indicates the max allowed number of installed charging schedules per charging profile Purposes.</param>
        /// <param name="RateUnit">A list of supported quantities for use in a charging schedule.</param>
        /// <param name="PeriodsPerSchedule">Maximum number of periods that may be defined per charging schedule.</param>
        /// <param name="ExternalControlSignalsEnabled">Indicates whether a battery swap station should respond to external control signals that influence charging.</param>
        /// <param name="NotifyChargingLimitWithSchedules">Indicates if the battery swap station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message. This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval. Default is false when omitted.</param>
        /// <param name="Phases3to1">If defined and true, this battery swap station supports switching from 3 to 1 phase during a transaction.</param>
        /// <param name="Entries">Amount of charging profiles currently installed on the battery swap station. MaxLimit used to limit number of charging profiles installed at any time.</param>
        /// <param name="LimitChangeSignificance"></param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SmartChargingCtrlr(IEnumerable<ChargingRateUnits>  RateUnit,
                                  UInt16                          PeriodsPerSchedule,
                                  UInt16                          ProfileStackLevel,
                                  UInt16                          Entries,
                                  Percentage                      LimitChangeSignificance,

                                  Boolean?                        Enabled                            = null,
                                  Boolean?                        Available                          = null,
                                  Boolean?                        ACPhaseSwitchingSupported          = null,
                                  Boolean?                        ExternalControlSignalsEnabled      = null,
                                  Boolean?                        NotifyChargingLimitWithSchedules   = null,
                                  Boolean?                        Phases3to1                         = null,

                                  String?                         Instance                           = null,
                                  CustomData?                     CustomData                         = null)

            : base(nameof(SmartChargingCtrlr),
                   Instance,
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


            #region Enabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    ValueGetter:      () => this.Enabled.HasValue
                                                ? this.Enabled.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether Smart Charging is enabled.")

                )
            );

            #endregion

            #region Available

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    ValueGetter:      () => this.Available.HasValue
                                                ? this.Available.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether Smart Charging is supported.")

                )
            );

            #endregion

            #region ACPhaseSwitchingSupported

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ACPhaseSwitchingSupported",
                    ValueGetter:      () => this.ACPhaseSwitchingSupported.HasValue
                                                ? this.ACPhaseSwitchingSupported.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If defined and true, this EVSE supports the selection of which phase to use for 1 phase AC charging.")

                )
            );

            #endregion

            #region ProfileStackLevel

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ProfileStackLevel",
                    ValueGetter:      () => this.ProfileStackLevel.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                           "Max stack level of a charging profile. " +
                                           "The number defined also indicates the max allowed number of installed charging schedules per charging profile purpose."
                                       )

                )
            );

            #endregion

            #region RateUnit

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "RateUnit",
                    ValueGetter:      () => this.RateUnit.AggregateWith(','),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  [ "A", "W" ]
                                      ),

                    Description:      I18NString.Create("A list of supported quantities for use in a charging schedule.")

                )
            );

            #endregion

            #region PeriodsPerSchedule

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "PeriodsPerSchedule",
                    ValueGetter:      () => this.PeriodsPerSchedule.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of periods that may be defined per charging schedule.")

                )
            );

            #endregion

            #region ExternalControlSignalsEnabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ExternalControlSignalsEnabled",
                    ValueGetter:      () => this.ExternalControlSignalsEnabled.HasValue
                                                ? this.ExternalControlSignalsEnabled.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Indicates whether a battery swap station should respond to external control signals that influence charging.")

                )
            );

            #endregion

            #region NotifyChargingLimitWithSchedules

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "NotifyChargingLimitWithSchedules",
                    ValueGetter:      () => this.NotifyChargingLimitWithSchedules.HasValue
                                                ? this.NotifyChargingLimitWithSchedules.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create(
                                           "Indicates if the battery swap station should include the externally set charging limit/schedule in the message when it sends a NotifyChargingLimitRequest message. " +
                                           "This might increase the data usage significantly, especially when an external system sends new profiles/limits with a short interval. " +
                                           "Default is false when omitted."
                                       )

                )
            );

            #endregion

            #region Phases3to1

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Phases3to1",
                    ValueGetter:      () => this.Phases3to1.HasValue
                                                ? this.Phases3to1.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If defined and true, this battery swap station supports switching from 3 to 1 phase during a transaction.")

                )
            );

            #endregion

            #region Entries

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Entries",
                    ValueGetter:      () => this.Entries.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                           "Amount of charging profiles currently installed on the battery swap station. " +
                                           "MaxLimit used to limit number of charging profiles installed at any time."
                                       )

                )
            );

            #endregion

            #region LimitChangeSignificance

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "LimitChangeSignificance",
                    ValueGetter:      () => this.LimitChangeSignificance.Value.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create(
                                           "If at the battery swap station side a change in the limit in a charging profile is lower than this percentage, " +
                                           "the battery swap station MAY skip sending a NotifyChargingLimitRequest or a TransactionEventRequest message to the CSMS. " +
                                           "It is RECOMMENDED to set this key to a low value. " +
                                           "See Smart Charging signals to a battery swap station from multiple actors."
                                       )

                )
            );

            #endregion


        }

        #endregion


    }

}
