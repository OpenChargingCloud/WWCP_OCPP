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
    /// Logical Component responsible for configuration relating to the use of Local Authorization Lists for charging station use.
    /// </summary>
    public class LocalAuthListCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether Local Authorization List is enabled.
        /// </summary>
        public Boolean?  Enabled                 { get; set; }

        /// <summary>
        /// Amount of IdTokens currently in the Local Authorization List.
        /// The maxLimit of this variable SHALL be provided to report the maximum number of IdTokens that can be stored in the Local Authorization List.
        /// </summary>
        public UInt32?   Entries                 { get; set; }

        /// <summary>
        /// Whether Local Authorization List is supported.
        /// </summary>
        public Boolean?  Available               { get; set; }

        /// <summary>
        /// Maximum number of identifications that can be sent in a single SendLocalListRequest.
        /// </summary>
        public UInt32?   ItemsPerMessage         { get; set; }

        /// <summary>
        /// Message Size (in bytes) - puts a constraint on SendLocalListRequest message size.
        /// </summary>
        public UInt32?   BytesPerMessage         { get; set; }

        /// <summary>
        /// Indicates the number of bytes currently used by the Local Authorization List.
        /// MaxLimit indicates the maximum number of bytes that can be used by the Local Authorization List.
        /// </summary>
        public UInt32?   Storage                 { get; set; }

        /// <summary>
        /// When set to true this variable disables the behavior to request authorization for an idToken that is stored in the local authorization list with a status other than Accepted, as stated in C14.FR.03.
        /// </summary>
        public Boolean?  DisablePostAuthorize    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new local authorization list controller.
        /// </summary>
        /// <param name="Enabled">Whether Local Authorization List is enabled.</param>
        /// <param name="Entries">Amount of IdTokens currently in the Local Authorization List. The maxLimit of this variable SHALL be provided to report the maximum number of IdTokens that can be stored in the Local Authorization List.</param>
        /// <param name="Available">Whether Local Authorization List is supported.</param>
        /// <param name="ItemsPerMessage">Maximum number of identifications that can be sent in a single SendLocalListRequest.</param>
        /// <param name="BytesPerMessage">Message Size (in bytes) - puts a constraint on SendLocalListRequest message size.</param>
        /// <param name="Storage">Indicates the number of bytes currently used by the Local Authorization List. MaxLimit indicates the maximum number of bytes that can be used by the Local Authorization List.</param>
        /// <param name="DisablePostAuthorize">When set to true this variable disables the behavior to request authorization for an idToken that is stored in the local authorization list with a status other than Accepted, as stated in C14.FR.03.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public LocalAuthListCtrlr(Boolean?     Enabled,
                                  UInt32?      Entries,
                                  Boolean?     Available,
                                  UInt32?      ItemsPerMessage,
                                  UInt32?      BytesPerMessage,
                                  UInt32?      Storage,
                                  Boolean?     DisablePostAuthorize,

                                  String?      Instance     = null,
                                  CustomData?  CustomData   = null)

            : base(nameof(LocalAuthListCtrlr),
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

                           Description:       I18NString.Create("Whether Local Authorization List is enabled."),

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

                           Description:       I18NString.Create("Amount of IdTokens currently in the Local Authorization List. The maxLimit of this variable SHALL be provided to report the maximum number of IdTokens that can be stored in the Local Authorization List."),

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

                           Description:       I18NString.Create("Whether Local Authorization List is supported."),

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

                           Description:       I18NString.Create("Maximum number of identifications that can be sent in a single SendLocalListRequest."),

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

                           Description:       I18NString.Create("Message Size (in bytes) - puts a constraint on SendLocalListRequest message size."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Storage

                       new VariableConfig(

                           Name:              "Storage",
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

                           Description:       I18NString.Create("Indicates the number of bytes currently used by the Local Authorization List. MaxLimit indicates the maximum number of bytes that can be used by the Local Authorization List."),

                           CustomData:        null

                       ),

                       #endregion

                       #region DisablePostAuthorize

                       new VariableConfig(

                           Name:              "DisablePostAuthorize",
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

                           Description:       I18NString.Create("When set to true this variable disables the behavior to request authorization for an idToken that is stored in the local authorization list with a status other than Accepted, as stated in C14.FR.03."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the use of Local Authorization Lists for charging station use."),
                   CustomData)

        {

            this.Enabled               = Enabled;
            this.Entries               = Entries;
            this.Available             = Available;
            this.ItemsPerMessage       = ItemsPerMessage;
            this.BytesPerMessage       = BytesPerMessage;
            this.Storage               = Storage;
            this.DisablePostAuthorize  = DisablePostAuthorize;

        }

        #endregion


    }

}
