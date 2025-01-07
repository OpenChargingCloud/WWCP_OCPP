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
using System.Runtime.InteropServices;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using System.Drawing;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to the
    /// exchange and storage of charging station device model data.
    /// </summary>
    public class DeviceDataCtrlr : ALogicalComponentConfig
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
            /// Message Size (in bytes) - puts constraint on GetReportRequest message size.
            /// </summary>
            [Mandatory]
            public UInt32  GetReport       { get; set; }

            /// <summary>
            /// Message Size (in bytes) - puts constraint on GetVariablesRequest message size.
            /// </summary>
            [Mandatory]
            public UInt32  GetVariables    { get; set; }

            /// <summary>
            /// Message Size (in bytes) - puts constraint on SetVariablesRequest message size.
            /// </summary>
            [Mandatory]
            public UInt32  SetVariables    { get; set; }

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
            /// Maximum number of ComponentVariable entries that can be sent in one GetReportRequest message.
            /// </summary>
            [Mandatory]
            public UInt32  GetReport       { get; set; }

            /// <summary>
            /// Maximum number of GetVariableData objects in GetVariablesRequest.
            /// </summary>
            [Mandatory]
            public UInt32  GetVariables    { get; set; }

            /// <summary>
            /// Maximum number of SetVariableData objects in SetVariablesRequest.
            /// </summary>
            [Mandatory]
            public UInt32  SetVariables    { get; set; }

        }

        #endregion

        #region ValueSize

        /// <summary>
        /// Limit a field.
        /// </summary>
        [Mandatory]
        public ValueSizeClass ValueSize { get; }

        public class ValueSizeClass
        {

            /// <summary>
            /// This Configuration Variable can be used to limit the following fields: SetVariableData.attributeValue and VariableCharacteristics.valueList.
            /// The max size of these values will always remain equal.
            /// </summary>
            public UInt32?  Configuration    { get; }

            /// <summary>
            /// This Configuration Variable can be used to limit the following fields: GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue.
            /// The max size of these values will always remain equal.
            /// </summary>
            public UInt32?  Reporting        { get; }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new device data controller.
        /// </summary>
        /// <param name="BytesPerMessage">Message Size (in bytes) - maxLimit used to report constraint on message size.</param>
        /// <param name="ItemsPerMessage">Maximum number of entries that can be sent in one message.</param>
        /// <param name="ValueSize">Can be used to limit the following fields: SetVariableData.attributeValue, GetVariableResult.attributeValue, VariableAttribute.value, VariableCharacteristics.valueList and EventData.actualValue.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DeviceDataCtrlr(BytesPerMessageClass  BytesPerMessage,
                               ItemsPerMessageClass  ItemsPerMessage,
                               ValueSizeClass        ValueSize,

                               String?               Instance     = null,
                               CustomData?           CustomData   = null)

            : base(nameof(DeviceDataCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the exchange and storage of charging station device model data."),
                   CustomData)

        {

            this.BytesPerMessage  = BytesPerMessage;
            this.ItemsPerMessage  = ItemsPerMessage;
            this.ValueSize        = ValueSize;


            #region BytesPerMessage (GetReport)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    Instance:         "GetReport",
                    ValueGetter:      this.BytesPerMessage.GetReport.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts constraint on GetReportRequest message size.")

                )
            );

            #endregion

            #region BytesPerMessage (GetVariables)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    Instance:         "GetVariables",
                    ValueGetter:      this.BytesPerMessage.GetVariables.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts constraint on GetVariablesRequest message size.")

                )
            );

            #endregion

            #region BytesPerMessage (GetVariables)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    Instance:         "SetVariables",
                    ValueGetter:      this.BytesPerMessage.SetVariables.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts constraint on SetVariablesRequest message size.")

                )
            );

            #endregion


            #region ItemsPerMessage (GetReport)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    Instance:         "GetReport",
                    ValueGetter:      this.ItemsPerMessage.GetReport.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of ComponentVariable entries that can be sent in one GetReportRequest message.")

                )
            );

            #endregion

            #region ItemsPerMessage (GetVariables)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    Instance:         "GetVariables",
                    ValueGetter:      this.ItemsPerMessage.GetVariables.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of GetVariableData objects in GetVariablesRequest.")

                )
            );

            #endregion

            #region ItemsPerMessage (SetVariables)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    Instance:         "SetVariables",
                    ValueGetter:      this.ItemsPerMessage.SetVariables.ToString,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of SetVariableData objects in SetVariablesRequest.")

                )
            );

            #endregion


            #region ValueSize (Configuration)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ConfigurationValueSize",
                    ValueGetter:      () => this.ValueSize.Configuration?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    1000
                                      ),

                    Description:      I18NString.Create(
                                          "This Configuration Variable can be used to limit the following fields: SetVariableData.attributeValue and VariableCharacteristics.valueList. " +
                                          "The max size of these values will always remain equal."
                                      )

                )
            );

            #endregion

            #region ValueSize (Reporting)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ReportingValueSize",
                    ValueGetter:      () => this.ValueSize.Reporting?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    2500
                                      ),

                    Description:      I18NString.Create(
                                          "This Configuration Variable can be used to limit the following fields: GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. " +
                                          "The max size of these values will always remain equal."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
