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
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to the exchange of monitoring event data.
    /// </summary>
    public class MonitoringCtrlr : ALogicalComponentConfig
    {

        #region Properties

        #region BytesPerMessage

        /// <summary>
        /// Message Size (in bytes) - puts constraint on message size.
        /// </summary>
        [Mandatory]
        public BytesPerMessageClass  BytesPerMessage    { get; }

        public class BytesPerMessageClass
        {

            /// <summary>
            /// Message Size (in bytes) - puts constraint on ClearVariableMonitoringRequest message size.
            /// </summary>
            public UInt16?  ClearVariableMonitoring    { get; set; }

            /// <summary>
            /// Message Size (in bytes) - puts constraint on SetVariableMonitoringRequest message size.
            /// </summary>
            [Mandatory]
            public UInt32   SetVariableMonitoring      { get; set; }

        }

        #endregion

        #region ItemsPerMessage

        /// <summary>
        /// Maximum number of entries that can be sent in one message.
        /// </summary>
        [Mandatory]
        public ItemsPerMessageClass  ItemsPerMessage    { get; }

        public class ItemsPerMessageClass
        {

            /// <summary>
            /// Maximum number of IDs in a ClearVariableMonitoringRequest.
            /// </summary>
            public UInt16?  ClearVariableMonitoring    { get; set; }

            /// <summary>
            /// Maximum number of SetMonitoringData elements that can be sent in one SetVariableMonitoringRequest message.
            /// </summary>
            [Mandatory]
            public UInt16   SetVariableMonitoring      { get; set; }

        }

        #endregion


        /// <summary>
        /// Whether monitoring is enabled.
        /// </summary>
        public Boolean?         Enabled                   { get; set; }

        /// <summary>
        /// Whether monitoring is supported.
        /// </summary>
        public Boolean?         Available                 { get; set; }

        /// <summary>
        /// Currently used MonitoringBase. (readonly)
        /// </summary>
        public MonitoringBase?  MonitoringBase            { get; }

        /// <summary>
        /// Currently use MonitoringLevel (readonly)
        /// </summary>
        public Int32?           MonitoringLevel           { get; }

        /// <summary>
        /// When set and the charging station is offline, the charging station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here.
        /// Value ranging from 0 (Emergency) to 9 (Debug).
        /// </summary>
        public Byte?            OfflineQueuingSeverity    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new monitoring controller.
        /// </summary>
        /// <param name="ItemsPerMessage">Maximum number of items.</param>
        /// <param name="BytesPerMessage">Message Size (in bytes) - puts constraint on message size.</param>
        /// 
        /// <param name="Enabled">Whether monitoring is enabled.</param>
        /// <param name="Available">Whether monitoring is supported.</param>
        /// 
        /// <param name="MonitoringBase">Currently used MonitoringBase. (readonly)</param>
        /// <param name="MonitoringLevel">Currently use MonitoringLevel (readonly)</param>
        /// <param name="OfflineQueuingSeverity">When set and the charging station is offline, the charging station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here. Value ranging from 0 (Emergency) to 9 (Debug).</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public MonitoringCtrlr(ItemsPerMessageClass  ItemsPerMessage,
                               BytesPerMessageClass  BytesPerMessage,

                               Boolean?              Enabled                  = null,
                               Boolean?              Available                = null,
                               MonitoringBase?       MonitoringBase           = null, //ToDo: Where did this go in OCPP v2.1?
                               Int32?                MonitoringLevel          = null, //ToDo: Where did this go in OCPP v2.1?
                               Byte?                 OfflineQueuingSeverity   = null, //ToDo: Refactor me!

                               String?               Instance                 = null,
                               CustomData?           CustomData               = null)

            : base(nameof(MonitoringCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the exchange of monitoring event data."),
                   CustomData)

        {

            this.ItemsPerMessage         = ItemsPerMessage;
            this.BytesPerMessage         = BytesPerMessage;

            this.Enabled                 = Enabled;
            this.Available               = Available;
            this.MonitoringBase          = MonitoringBase;
            this.MonitoringLevel         = MonitoringLevel;
            this.OfflineQueuingSeverity  = OfflineQueuingSeverity;


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

                    Description:      I18NString.Create("Whether monitoring is enabled.")

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

                    Description:      I18NString.Create("Whether monitoring is supported.")

                )
            );

            #endregion

            #region ItemsPerMessage (ClearVariableMonitoring)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    Instance:         "ClearVariableMonitoring",
                    ValueGetter:      () => this.ItemsPerMessage.ClearVariableMonitoring?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of IDs in a ClearVariableMonitoringRequest.")

                )
            );

            #endregion

            #region ItemsPerMessage (SetVariableMonitoring)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    Instance:         "SetVariableMonitoring",
                    ValueGetter:      this.ItemsPerMessage.SetVariableMonitoring.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of SetMonitoringData elements that can be sent in one SetVariableMonitoringRequest message.")

                )
            );

            #endregion

            #region BytesPerMessage (ClearVariableMonitoring)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    Instance:         "ClearVariableMonitoring",
                    ValueGetter:      () => this.BytesPerMessage.ClearVariableMonitoring?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts constraint on ClearVariableMonitoringRequest message size.")

                )
            );

            #endregion

            #region BytesPerMessage (SetVariableMonitoring)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    Instance:         "SetVariableMonitoring",
                    ValueGetter:      this.BytesPerMessage.SetVariableMonitoring.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts constraint on SetVariableMonitoringRequest message size.")

                )
            );

            #endregion

            #region MonitoringBase

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MonitoringBase",
                    ValueGetter:      () => this.MonitoringBase?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.OptionList
                                      ),

                    Description:      I18NString.Create("Currently used MonitoringBase.")

                )
            );

            #endregion

            #region MonitoringLevel

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MonitoringLevel",
                    ValueGetter:      () => this.MonitoringLevel?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Currently use MonitoringLevel.")

                )
            );

            #endregion

            #region OfflineQueuingSeverity

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "OfflineQueuingSeverity",
                    ValueGetter:      () => this.OfflineQueuingSeverity?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("When set and the charging station is offline, the charging station shall queue any notifyEventRequest messages triggered by a monitor with a severity number equal to or lower than the severity configured here. Value ranging from 0 (Emergency) to 9 (Debug).")

                )
            );

            #endregion


        }

        #endregion


    }

}
