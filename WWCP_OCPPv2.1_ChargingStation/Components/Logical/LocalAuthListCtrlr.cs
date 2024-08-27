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
    /// Logical Component responsible for configuration relating to the use of Local Authorization Lists for charging station use.
    /// </summary>
    public class LocalAuthListCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable exists and reports a value of true, Local Authorization List is enabled.
        /// </summary>
        public Boolean?  Enabled                 { get; set; }

        /// <summary>
        /// Amount of IdTokens currently in the Local Authorization List.
        /// The maxLimit of this variable SHALL be provided to report the maximum number of IdTokens that can be stored in the Local Authorization List.
        /// </summary>
        public UInt32?   Entries                 { get; set; }

        /// <summary>
        /// If this variable exists and reports a value of true, Local Authorization List is supported.
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
        /// <param name="Enabled">If this variable exists and reports a value of true, Local Authorization List is enabled.</param>
        /// <param name="Entries">Amount of IdTokens currently in the Local Authorization List. The maxLimit of this variable SHALL be provided to report the maximum number of IdTokens that can be stored in the Local Authorization List.</param>
        /// <param name="Available">If this variable exists and reports a value of true, Local Authorization List is supported.</param>
        /// <param name="ItemsPerMessage">Maximum number of identifications that can be sent in a single SendLocalListRequest.</param>
        /// <param name="BytesPerMessage">Message Size (in bytes) - puts a constraint on SendLocalListRequest message size.</param>
        /// <param name="Storage">Indicates the number of bytes currently used by the Local Authorization List. MaxLimit indicates the maximum number of bytes that can be used by the Local Authorization List.</param>
        /// <param name="DisablePostAuthorize">When set to true this variable disables the behavior to request authorization for an idToken that is stored in the local authorization list with a status other than Accepted, as stated in C14.FR.03.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public LocalAuthListCtrlr(Boolean?     Enabled                = null,
                                  UInt32?      Entries                = null,
                                  Boolean?     Available              = null,
                                  UInt32?      ItemsPerMessage        = null,
                                  UInt32?      BytesPerMessage        = null,
                                  UInt32?      Storage                = null,
                                  Boolean?     DisablePostAuthorize   = null,

                                  String?      Instance               = null,
                                  CustomData?  CustomData             = null)

            : base(nameof(LocalAuthListCtrlr),
                   Instance,
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

                    Description:      I18NString.Create(
                                          "If this variable exists and reports a value of true, " +
                                          "Local Authorization List is enabled."
                                      )

                )
            );

            #endregion

            #region Entries

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Entries",
                    ValueGetter:      () => this.Entries?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    1000 // Not specified by OCPP!
                                      ),

                    Description:      I18NString.Create(
                                          "Amount of IdTokens currently in the Local Authorization List. " +
                                          "The maxLimit of this variable SHALL be provided to report the maximum " +
                                          "number of IdTokens that can be stored in the Local Authorization List."
                                      )

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

                    Description:      I18NString.Create("If this variable exists and reports a value of true, Local Authorization List is supported.")

                )
            );

            #endregion

            #region ItemsPerMessage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ItemsPerMessage",
                    ValueGetter:      () => this.ItemsPerMessage?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of identifications that can be sent in a single SendLocalListRequest.")

                )
            );

            #endregion

            #region BytesPerMessage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BytesPerMessage",
                    ValueGetter:      () => this.BytesPerMessage?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Message Size (in bytes) - puts a constraint on SendLocalListRequest message size.")

                )
            );

            #endregion

            #region Storage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Storage",
                    ValueGetter:      () => this.Storage?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    9999 // Not specified by OCPP!
                                      ),

                    Description:      I18NString.Create(
                                          "Indicates the number of bytes currently used by the Local Authorization List. " +
                                          "MaxLimit indicates the maximum number of bytes that can be used by the Local Authorization List."
                                      )

                )
            );

            #endregion

            #region DisablePostAuthorize   (Does this still exist in OCPP v2.1?)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DisablePostAuthorize",
                    ValueGetter:      () => this.DisablePostAuthorize.HasValue
                                                ? this.DisablePostAuthorize.Value
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
                                          "When set to true this variable disables the behavior to request authorization for an idToken " +
                                          "that is stored in the local authorization list with a status other than Accepted, as stated in C14.FR.03."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
