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
        public Boolean   AuthorizeRemoteStart             { get; set; }

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
        /// Meaning they can stop any ongoing transaction, but cannot start transactions.
        /// This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.
        /// </summary>
        public String?   MasterPassGroupId                { get; set; }

        /// <summary>
        /// If this key exists, the charging station supports Unknown Offline Authorization.
        /// If this key reports a value of true, Unknown Offline Authorization is enabled.
        /// </summary>
        public Boolean?  OfflineTxForUnknownIdEnabled     { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization controller.
        /// </summary>
        /// <param name="AuthorizeRemoteStart">Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.</param>
        /// <param name="LocalAuthorizeOffline">Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.</param>
        /// <param name="LocalPreAuthorize">Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an AuthorizeResponse from the CSMS.</param>
        /// 
        /// <param name="Enabled">If this variable reports a value of true, Authorization is enabled.</param>
        /// <param name="AdditionalInfoItemsPerMessage">Maximum number of AdditionalInfo items that can be sent in one message.</param>
        /// <param name="DisableRemoteAuthorization">When set to true this instructs the Charging Station to not issue any AuthorizationRequests, but only use Authorization Cache and Local Authorization List to determine validity of idTokens.</param>
        /// <param name="MasterPassGroupId">IdTokens that have this id as groupId belong to the Master Pass Group. Meaning they can stop any ongoing transaction, but cannot start transactions. This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.</param>
        /// <param name="OfflineTxForUnknownIdEnabled">If this key exists, the charging station supports Unknown Offline Authorization. If this key reports a value of true, Unknown Offline Authorization is enabled.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AuthCtrlr(Boolean      AuthorizeRemoteStart,
                         Boolean      LocalAuthorizeOffline,
                         Boolean      LocalPreAuthorize,

                         Boolean?     Enabled                         = null,
                         UInt32?      AdditionalInfoItemsPerMessage   = null,
                         Boolean?     DisableRemoteAuthorization      = null,
                         String?      MasterPassGroupId               = null,
                         Boolean?     OfflineTxForUnknownIdEnabled    = null,

                         String?      Instance                        = null,
                         CustomData?  CustomData                      = null)

            : base(nameof(AuthCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the use of authorization for Charging Station use."),
                   CustomData)

        {

            this.AuthorizeRemoteStart           = AuthorizeRemoteStart;
            this.LocalAuthorizeOffline          = LocalAuthorizeOffline;
            this.LocalPreAuthorize              = LocalPreAuthorize;

            this.Enabled                        = Enabled;
            this.AdditionalInfoItemsPerMessage  = AdditionalInfoItemsPerMessage;
            this.DisableRemoteAuthorization     = DisableRemoteAuthorization;
            this.MasterPassGroupId              = MasterPassGroupId;
            this.OfflineTxForUnknownIdEnabled   = OfflineTxForUnknownIdEnabled;


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

                    Description:      I18NString.Create("If set to FALSE, then authorization is switched off. Transactions are still possible, but no authorization will take place. This implies, that the value of idToken in transaction events SHALL be NoAuthorization.")

                )
            );

            #endregion

            #region AdditionalInfoItemsPerMessage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "AdditionalInfoItemsPerMessage",
                    ValueGetter:      () => this.AdditionalInfoItemsPerMessage?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum number of AdditionalInfo items that can be sent in one message.")

                )
            );

            #endregion

            #region AuthorizeRemoteStart

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "AuthorizeRemoteStart",
                    ValueGetter:      () => this.AuthorizeRemoteStart
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.")

                )
            );

            #endregion

            #region DisableRemoteAuthorization

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DisableRemoteAuthorization",
                    ValueGetter:      () => this.DisableRemoteAuthorization == true
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("When set to true this instructs the Charging Station to not issue any AuthorizationRequests, but only use Authorization Cache and Local Authorization List to determine validity of idTokens.")

                )
            );

            #endregion

            #region LocalAuthorizeOffline

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "LocalAuthorizeOffline",
                    ValueGetter:      () => this.LocalAuthorizeOffline == true
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.")

                )
            );

            #endregion

            #region LocalPreAuthorize

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "LocalPreAuthorize",
                    ValueGetter:      () => this.LocalPreAuthorize == true
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an AuthorizeResponse from the CSMS.")

                )
            );

            #endregion

            #region MasterPassGroupId

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MasterPassGroupId",
                    ValueGetter:      () => this.MasterPassGroupId,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String,
                                          MaxLimit:    36
                                      ),

                    Description:      I18NString.Create("IdTokens that have this id as groupId belong to the Master Pass Group. Meaning they can stop any ongoing transaction, but cannot start transactions. This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.")

                )
            );

            #endregion

            #region OfflineTxForUnknownIdEnabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "OfflineTxForUnknownIdEnabled",
                    ValueGetter:      () => this.Enabled == true
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this key exists, the charging station supports Unknown Offline Authorization. If this key reports a value of true, Unknown Offline Authorization is enabled.")

                )
            );

            #endregion


        }

        #endregion


    }

}
