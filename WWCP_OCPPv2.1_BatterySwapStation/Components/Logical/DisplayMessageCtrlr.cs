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
    /// Logical Component responsible for configuration relating to
    /// the display of messages to battery swap station users.
    /// </summary>
    public class DisplayMessageCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether this display message controller is enabled.
        /// </summary>
        public Boolean?                      Enabled                { get; set; }

        /// <summary>
        /// Whether the display of messages is supported.
        /// </summary>
        public Boolean?                      Available              { get; set; }

        /// <summary>
        /// The number of different messages that are currently configured in this battery swap station, via SetDisplayMessageRequest.
        /// </summary>
        [Mandatory]
        public UInt16                        DisplayMessages        { get; set; }

        /// <summary>
        /// The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored).
        /// </summary>
        public UInt32?                       PersonalMessageSize    { get; set; }

        /// <summary>
        /// The enumeration of supported message formats by this battery swap station.
        /// </summary>
        [Mandatory]
        public IEnumerable<MessageFormat>    SupportedFormats       { get; set; }

        /// <summary>
        /// The enumeration of supported message priorities by this battery swap station.
        /// </summary>
        [Mandatory]
        public IEnumerable<MessagePriority>  SupportedPriorities    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customization controller.
        /// </summary>
        /// <param name="DisplayMessages">The number of different messages that are currently configured in this battery swap station, via SetDisplayMessageRequest.</param>
        /// <param name="SupportedFormats">An enumeration of supported message formats by this battery swap station.</param>
        /// <param name="SupportedPriorities">An enumeration of supported message priorities by this battery swap station.</param>
        /// 
        /// <param name="Enabled">Whether this display message controller is enabled.</param>
        /// <param name="Available">Whether the display of messages is supported.</param>
        /// <param name="PersonalMessageSize">The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored).</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DisplayMessageCtrlr(UInt16                        DisplayMessages,
                                   IEnumerable<MessageFormat>    SupportedFormats,
                                   IEnumerable<MessagePriority>  SupportedPriorities,

                                   Boolean?                      Enabled               = null,
                                   Boolean?                      Available             = null,
                                   UInt32?                       PersonalMessageSize   = null,

                                   String?                       Instance              = null,
                                   CustomData?                   CustomData            = null)

            : base(nameof(DisplayMessageCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the display of messages to battery swap station users."),
                   CustomData)

        {

            this.DisplayMessages      = DisplayMessages;
            this.SupportedFormats     = SupportedFormats.   Distinct();
            this.SupportedPriorities  = SupportedPriorities.Distinct();

            this.Enabled              = Enabled;
            this.Available            = Available;
            this.PersonalMessageSize  = PersonalMessageSize;    // Where did this go in OCPP v2.1?


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

                    Description:      I18NString.Create("Whether this display message controller is enabled.")

                )
            );

            #endregion

            #region Available

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    Instance:         null,
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

                    Description:      I18NString.Create("Whether the display of messages is supported.")

                )
            );

            #endregion

            #region DisplayMessages

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DisplayMessages",
                    Instance:         null,
                    ValueGetter:      () => this.DisplayMessages.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    9999 // Not specified by OCPP!
                                      ),

                    Description:      I18NString.Create("The number of different messages that are currently configured in this battery swap station, via SetDisplayMessageRequest.")

                )
            );

            #endregion

            #region PersonalMessageSize  (Where did this go in OCPP v2.1?)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "PersonalMessageSize",
                    Instance:         null,
                    ValueGetter:      () => this.PersonalMessageSize?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("The max size (in characters) of the personal message element of the IdTokenInfo data (0 specifies no personal data may be stored).")

                )
            );

            #endregion

            #region SupportedFormats

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SupportedFormats",
                    Instance:         null,
                    ValueGetter:      () => this.SupportedFormats.Any()
                                                ? this.SupportedFormats.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  MessageFormat.Values.Select(messageFormat => messageFormat.ToString())
                                      ),

                    Description:      I18NString.Create("The enumeration of supported message formats by this battery swap station.")

                )
            );

            #endregion

            #region SupportedPriorities

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SupportedPriorities",
                    Instance:         null,
                    ValueGetter:      () => this.SupportedPriorities.Any()
                                                ? this.SupportedPriorities.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  MessagePriority.Values.Select(messagePriority => messagePriority.ToString())
                                      ),

                    Description:      I18NString.Create("The enumeration of supported message priorities by this battery swap station.")

                )
            );

            #endregion


        }

        #endregion


    }

}
