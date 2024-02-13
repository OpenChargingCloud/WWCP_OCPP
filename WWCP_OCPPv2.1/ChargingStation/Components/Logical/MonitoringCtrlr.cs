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
    /// Logical Component responsible for configuration relating to the exchange of monitoring event data.
    /// </summary>
    public class MonitoringCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether Monitoring is enabled.
        /// </summary>
        public Boolean?                      Enabled                   { get; set; }

        /// <summary>
        /// Whether Monitoring is supported.
        /// </summary>
        public Boolean?                      Available                 { get; set; }

        /// <summary>
        /// Maximum number of items.
        /// </summary>
        public Int32?                        ItemsPerMessage           { get; set; }

        /// <summary>
        /// Message Size (in bytes) - puts constraint on message size.
        /// </summary>
        public Int32?                        BytesPerMessage           { get; set; }

        /// <summary>
        /// Currently used MonitoringBase. (readonly)
        /// </summary>
        public IEnumerable<MonitoringBase>?  MonitoringBase            { get; }

        /// <summary>
        /// Currently use MonitoringLevel (readonly)
        /// </summary>
        public Int32?                        MonitoringLevel           { get; }

        /// <summary>
        /// When set and the charging station is offline, the charging station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here.
        /// Value ranging from 0 (Emergency) to 9 (Debug).
        /// </summary>
        public Byte?                         OfflineQueuingSeverity    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new monitoring controller.
        /// </summary>
        /// <param name="Enabled">Whether Monitoring is enabled.</param>
        /// <param name="Available">Whether Monitoring is supported.</param>
        /// <param name="ItemsPerMessage">Maximum number of items.</param>
        /// <param name="BytesPerMessage">Message Size (in bytes) - puts constraint on message size.</param>
        /// <param name="MonitoringBase">Currently used MonitoringBase. (readonly)</param>
        /// <param name="MonitoringLevel">Currently use MonitoringLevel (readonly)</param>
        /// <param name="OfflineQueuingSeverity">When set and the charging station is offline, the charging station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here. Value ranging from 0 (Emergency) to 9 (Debug).</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MonitoringCtrlr(Boolean?                      Enabled                  = null,
                               Boolean?                      Available                = null,
                               Int32?                        ItemsPerMessage          = null,
                               Int32?                        BytesPerMessage          = null,
                               IEnumerable<MonitoringBase>?  MonitoringBase           = null,
                               Int32?                        MonitoringLevel          = null,
                               Byte?                         OfflineQueuingSeverity   = null, //ToDo: Refactor me!

                               String?                       Instance                 = null,
                               CustomData?                   CustomData               = null)

            : base(nameof(MonitoringCtrlr),
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

                           Description:       I18NString.Create("Whether Monitoring is enabled."),

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

                           Description:       I18NString.Create("Whether Monitoring is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ItemsPerMessage

                       new VariableConfig(

                           Name:              "ItemsPerMessage",
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

                           Description:       I18NString.Create("Maximum number of items."),

                           CustomData:        null

                       ),

                       #endregion

                       #region BytesPerMessage

                       new VariableConfig(

                           Name:              "BytesPerMessage",
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

                           Description:       I18NString.Create("Message Size (in bytes) - puts constraint on message size."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MonitoringBase

                       new VariableConfig(

                           Name:              "MonitoringBase",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.OptionList
                                                   )
                                               },

                           Description:       I18NString.Create("Currently used MonitoringBase."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MonitoringLevel

                       new VariableConfig(

                           Name:              "MonitoringLevel",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Currently use MonitoringLevel."),

                           CustomData:        null

                       ),

                       #endregion

                       #region OfflineQueuingSeverity

                       new VariableConfig(

                           Name:              "OfflineQueuingSeverity",
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

                           Description:       I18NString.Create("When set and the Charging Station is offline, the Charging Station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here. Value ranging from 0 (Emergency) to 9 (Debug)."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the exchange of monitoring event data."),
                   CustomData)

        {

            this.Enabled                 = Enabled;
            this.Available               = Available;
            this.ItemsPerMessage         = ItemsPerMessage;
            this.BytesPerMessage         = BytesPerMessage;
            this.MonitoringBase          = MonitoringBase;
            this.MonitoringLevel         = MonitoringLevel;
            this.OfflineQueuingSeverity  = OfflineQueuingSeverity;

        }

        #endregion


    }

}
