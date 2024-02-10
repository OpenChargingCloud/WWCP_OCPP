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
    /// Logical Component responsible for configuration relating to the
    /// exchange and storage of charging station device model data.
    /// </summary>
    public class DeviceDataCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Message Size (in bytes) - maxLimit used to report constraint on message size. Which message is specified in the instance.
        /// </summary>
        public UInt32?  BytesPerMessage    { get; set; }

        /// <summary>
        /// Maximum number of entries that can be sent in one message. Which entries in which message is specified in the instance.
        /// </summary>
        public UInt32?  ItemsPerMessage    { get; set; }

        /// <summary>
        /// Can be used to limit the following fields: SetVariableData.attributeValue, GetVariableResult.attributeValue, VariableAttribute.value, VariableCharacteristics.valueList and EventData.actualValue.
        /// </summary>
        public UInt32?  ValueSize          { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new device data controller.
        /// </summary>
        /// <param name="BytesPerMessage">Message Size (in bytes) - maxLimit used to report constraint on message size. Which message is specified in the instance.</param>
        /// <param name="ItemsPerMessage">Maximum number of entries that can be sent in one message. Which entries in which message is specified in the instance.</param>
        /// <param name="ValueSize">Can be used to limit the following fields: SetVariableData.attributeValue, GetVariableResult.attributeValue, VariableAttribute.value, VariableCharacteristics.valueList and EventData.actualValue.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeviceDataCtrlr(UInt32?      BytesPerMessage   = null,
                               UInt32?      ItemsPerMessage   = null,
                               UInt32?      ValueSize         = null,

                               String?      Instance          = null,
                               CustomData?  CustomData        = null)

            : base(nameof(DeviceDataCtrlr),
                   Instance,
                   new[] {

                       #region BytesPerMessage

                       new VariableConfig(

                           Name:              "BytesPerMessage",
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

                           Description:       I18NString.Create("Message Size (in bytes) - maxLimit used to report constraint on message size. Which message is specified in the instance."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ItemsPerMessage

                       new VariableConfig(

                           Name:              "ItemsPerMessage",
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

                           Description:       I18NString.Create("Maximum number of entries that can be sent in one message. Which entries in which message is specified in the instance."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ValueSize

                       new VariableConfig(

                           Name:              "ValueSize",
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

                           Description:       I18NString.Create("Can be used to limit the following fields: SetVariableData.attributeValue, GetVariableResult.attributeValue, VariableAttribute.value, VariableCharacteristics.valueList and EventData.actualValue."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the exchange and storage of charging station device model data."),
                   CustomData)

        {

            this.BytesPerMessage  = BytesPerMessage;
            this.ItemsPerMessage  = ItemsPerMessage;
            this.ValueSize        = ValueSize;

        }

        #endregion


    }

}
