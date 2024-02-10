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
    /// Logical Component responsible for configuration relating to
    /// the display of messages to charging station users.
    /// </summary>
    public class DisplayMessageCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether this display message controller is enabled.
        /// </summary>
        public Boolean?                    Enabled                { get; set; }

        /// <summary>
        /// Whether the display of messages is supported.
        /// </summary>
        public Boolean?                    Available              { get; set; }

        /// <summary>
        /// The number of different messages that are currently configured in this charging station, via SetDisplayMessageRequest.
        /// </summary>
        public UInt32?                     DisplayMessages        { get; set; }

        /// <summary>
        /// The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored).
        /// </summary>
        public UInt32?                     PersonalMessageSize    { get; set; }

        /// <summary>
        /// The enumeration of supported message formats by this charging station.
        /// </summary>
        public IEnumerable<MessageFormat>  SupportedFormats       { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customization controller.
        /// </summary>
        /// <param name="Enabled">Whether this display message controller is enabled.</param>
        /// <param name="Available">Whether the display of messages is supported.</param>
        /// <param name="DisplayMessages">The number of different messages that are currently configured in this charging station, via SetDisplayMessageRequest.</param>
        /// <param name="PersonalMessageSize">The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored).</param>
        /// <param name="SupportedFormats">An enumeration of supported message formats by this charging station.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DisplayMessageCtrlr(Boolean?                     Enabled               = null,
                                   Boolean?                     Available             = null,
                                   UInt32?                      DisplayMessages       = null,
                                   UInt32?                      PersonalMessageSize   = null,
                                   IEnumerable<MessageFormat>?  SupportedFormats      = null,

                                   String?                      Instance              = null,
                                   CustomData?                  CustomData            = null)

            : base(nameof(DisplayMessageCtrlr),
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

                           Description:       I18NString.Create("Whether this display message controller is enabled."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Available

                       new VariableConfig(

                           Name:              "Available",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("Whether the display of messages is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region DisplayMessages

                       new VariableConfig(

                           Name:              "DisplayMessages",
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

                           Description:       I18NString.Create("The number of different messages that are currently configured in this charging station, via SetDisplayMessageRequest."),

                           CustomData:        null

                       ),

                       #endregion

                       #region PersonalMessageSize

                       new VariableConfig(

                           Name:              "PersonalMessageSize",
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

                           Description:       I18NString.Create("The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored)."),

                           CustomData:        null

                       ),

                       #endregion

                       #region SupportedFormats

                       new VariableConfig(

                           Name:              "SupportedFormats",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("The list of message formats supported by this charging station. Possible values: See MessageFormatEnumType."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the display of messages to charging station users."),
                   CustomData)

        {

            this.Enabled              = Enabled;
            this.Available            = Available;
            this.DisplayMessages      = DisplayMessages;
            this.PersonalMessageSize  = PersonalMessageSize;
            this.SupportedFormats     = SupportedFormats?.Distinct() ?? [];

        }

        #endregion


    }

}
