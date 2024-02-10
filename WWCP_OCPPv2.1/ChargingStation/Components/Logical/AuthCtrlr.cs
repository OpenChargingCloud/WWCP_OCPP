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
    /// Logical Component responsible for configuration relating to the use of authorization for charging station use.
    /// </summary>
    public class AuthCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable reports a value of true, Authorization is enabled.
        /// </summary>
        public Boolean?  Enabled                          { get; set; }

        /// <summary>
        /// Maximum number of AdditionalInfo items that can be sent in one message.
        /// </summary>
        public UInt32?   AdditionalInfoItemsPerMessage    { get; set; }

        /// <summary>
        /// Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.
        /// </summary>
        public Boolean?  AuthorizeRemoteStart             { get; set; }

        /// <summary>
        /// When set to true this instructs the Charging Station to not issue any AuthorizationRequests, but only use Authorization Cache and Local Authorization List to determine validity of idTokens.
        /// </summary>
        public Boolean?  DisableRemoteAuthorization       { get; set; }

        /// <summary>
        /// Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.
        /// </summary>
        public Boolean?  LocalAuthorizeOffline            { get; set; }

        /// <summary>
        /// Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an AuthorizeResponse from the CSMS.
        /// </summary>
        public Boolean?  LocalPreAuthorize                { get; set; }

        /// <summary>
        /// IdTokens that have this id as groupId belong to the Master Pass Group.
        /// </summary>
        public String?   MasterPassGroupId                { get; set; }

        /// <summary>
        /// If this key exists, the charging station supports Unknown Offline Authorization.
        /// </summary>
        public Boolean?  OfflineTxForUnknownIdEnabled     { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization controller.
        /// </summary>
        /// <param name="Enabled">If this variable reports a value of true, Authorization is enabled.</param>
        /// <param name="AdditionalInfoItemsPerMessage">Maximum number of AdditionalInfo items that can be sent in one message.</param>
        /// <param name="AuthorizeRemoteStart">Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.</param>
        /// <param name="DisableRemoteAuthorization">When set to true this instructs the Charging Station to not issue any AuthorizationRequests, but only use Authorization Cache and Local Authorization List to determine validity of idTokens.</param>
        /// <param name="LocalAuthorizeOffline">Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.</param>
        /// <param name="LocalPreAuthorize">Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an AuthorizeResponse from the CSMS.</param>
        /// <param name="MasterPassGroupId">IdTokens that have this id as groupId belong to the Master Pass Group.</param>
        /// <param name="OfflineTxForUnknownIdEnabled">If this key exists, the charging station supports Unknown Offline Authorization.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AuthCtrlr(Boolean?     Enabled                         = null,
                         UInt32?      AdditionalInfoItemsPerMessage   = null,
                         Boolean?     AuthorizeRemoteStart            = null,
                         Boolean?     DisableRemoteAuthorization      = null,
                         Boolean?     LocalAuthorizeOffline           = null,
                         Boolean?     LocalPreAuthorize               = null,
                         String?      MasterPassGroupId               = null,
                         Boolean?     OfflineTxForUnknownIdEnabled    = null,

                         String?      Instance                        = null,
                         CustomData?  CustomData                      = null)

            : base(nameof(AuthCtrlr),
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

                           Description:       I18NString.Create("If this variable reports a value of true, Authorization is enabled."),

                           CustomData:        null

                       ),

                       #endregion

                       #region AdditionalInfoItemsPerMessage

                       new VariableConfig(

                           Name:              "AdditionalInfoItemsPerMessage",
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

                           Description:       I18NString.Create("Maximum number of AdditionalInfo items that can be sent in one message."),

                           CustomData:        null

                       ),

                       #endregion

                       #region AuthorizeRemoteStart

                       new VariableConfig(

                           Name:              "AuthorizeRemoteStart",
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

                           Description:       I18NString.Create("Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction."),

                           CustomData:        null

                       ),

                       #endregion

                       #region DisableRemoteAuthorization

                       new VariableConfig(

                           Name:              "DisableRemoteAuthorization",
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

                           Description:       I18NString.Create("When set to true this instructs the Charging Station to not issue any AuthorizationRequests, but only use Authorization Cache and Local Authorization List to determine validity of idTokens."),

                           CustomData:        null

                       ),

                       #endregion

                       #region LocalAuthorizeOffline

                       new VariableConfig(

                           Name:              "LocalAuthorizeOffline",
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

                           Description:       I18NString.Create("Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers."),

                           CustomData:        null

                       ),

                       #endregion

                       #region LocalPreAuthorize

                       new VariableConfig(

                           Name:              "LocalPreAuthorize",
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

                           Description:       I18NString.Create("Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an AuthorizeResponse from the CSMS."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MasterPassGroupId

                       new VariableConfig(

                           Name:              "MasterPassGroupId",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("IdTokens that have this id as groupId belong to the Master Pass Group."),

                           CustomData:        null

                       ),

                       #endregion

                       #region OfflineTxForUnknownIdEnabled

                       new VariableConfig(

                           Name:              "OfflineTxForUnknownIdEnabled",
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

                           Description:       I18NString.Create("If this key exists, the charging station supports Unknown Offline Authorization."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the use of authorization for Charging Station use."),
                   CustomData)

        {

            this.Enabled                        = Enabled;
            this.AdditionalInfoItemsPerMessage  = AdditionalInfoItemsPerMessage;
            this.AuthorizeRemoteStart           = AuthorizeRemoteStart;
            this.DisableRemoteAuthorization     = DisableRemoteAuthorization;
            this.LocalAuthorizeOffline          = LocalAuthorizeOffline;
            this.LocalPreAuthorize              = LocalPreAuthorize;
            this.MasterPassGroupId              = MasterPassGroupId;
            this.OfflineTxForUnknownIdEnabled   = OfflineTxForUnknownIdEnabled;

        }

        #endregion


    }

}
