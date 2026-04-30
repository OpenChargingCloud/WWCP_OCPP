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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration of Domain Name System (NTS) Servers
    /// directly in OCPP with a focus on NTS-over-TLS (DoT) and NTS-over-HTTPS (DoH) for
    /// secure communication.
    /// </summary>
    public class NTSCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable reports a value of true, NTSorization is enabled.
        /// </summary>
        public Boolean?  Enabled                          { get; set; }



        // SystemTime | LocalGridTime | CSMSTIme | LegalTime


        // "contact":     "ntpadmin@ptb.de"
        // "releaseDate": "2026-03-03"
        // "version":     "1.0"


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization controller.
        /// </summary>
        /// <param name="NTSorizeRemoteStart">Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.</param>
        /// <param name="LocalNTSorizeOffline">Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.</param>
        /// <param name="LocalPreNTSorize">Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an NTSorizeResponse from the CSMS.</param>
        /// 
        /// <param name="Enabled">If this variable reports a value of true, NTSorization is enabled.</param>
        /// <param name="AdditionalInfoItemsPerMessage">Maximum number of AdditionalInfo items that can be sent in one message.</param>
        /// <param name="DisableRemoteNTSorization">When set to true this instructs the Charging Station to not issue any NTSorizationRequests, but only use NTSorization Cache and Local NTSorization List to determine validity of idTokens.</param>
        /// <param name="MasterPassGroupId">IdTokens that have this id as groupId belong to the Master Pass Group. Meaning they can stop any ongoing transaction, but cannot start transactions. This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.</param>
        /// <param name="OfflineTxForUnknownIdEnabled">If this key exists, the charging station supports Unknown Offline NTSorization. If this key reports a value of true, Unknown Offline NTSorization is enabled.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTSCtrlr(

                        String?      Instance                        = null,
                        CustomData?  CustomData                      = null)

            : base(nameof(NTSCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the use of authorization for Charging Station use."),
                   CustomData)

        {


            this.Enabled                        = Enabled;


            //#region Enabled

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "Enabled",
            //        ValueGetter:      () => this.Enabled.HasValue
            //                                    ? this.Enabled.Value
            //                                          ? "true"
            //                                          : "false"
            //                                    : null,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("If set to FALSE, then authorization is switched off. Transactions are still possible, but no authorization will take place. This implies, that the value of idToken in transaction events SHALL be NoNTSorization.")

            //    )
            //);

            //#endregion

            //#region AdditionalInfoItemsPerMessage

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "AdditionalInfoItemsPerMessage",
            //        ValueGetter:      () => this.AdditionalInfoItemsPerMessage?.ToString(),

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadOnly
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Integer
            //                          ),

            //        Description:      I18NString.Create("Maximum number of AdditionalInfo items that can be sent in one message.")

            //    )
            //);

            //#endregion

            //#region NTSorizeRemoteStart

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "NTSorizeRemoteStart",
            //        ValueGetter:      () => this.NTSorizeRemoteStart
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether a remote request to start a transaction in the form of RequestStartTransactionRequest message should be authorized beforehand like a local action to start a transaction.")

            //    )
            //);

            //#endregion

            //#region DisableRemoteNTSorization

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "DisableRemoteNTSorization",
            //        ValueGetter:      () => this.DisableRemoteNTSorization == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("When set to true this instructs the Charging Station to not issue any NTSorizationRequests, but only use NTSorization Cache and Local NTSorization List to determine validity of idTokens.")

            //    )
            //);

            //#endregion

            //#region LocalNTSorizeOffline

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "LocalNTSorizeOffline",
            //        ValueGetter:      () => this.LocalNTSorizeOffline == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether the Charging Station, when Offline, will start a transaction for locally authorized identifiers.")

            //    )
            //);

            //#endregion

            //#region LocalPreNTSorize

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "LocalPreNTSorize",
            //        ValueGetter:      () => this.LocalPreNTSorize == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("Whether the Charging Station, when online, will start a transaction for locally authorized identifiers without waiting for or requesting an NTSorizeResponse from the CSMS.")

            //    )
            //);

            //#endregion

            //#region MasterPassGroupId

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "MasterPassGroupId",
            //        ValueGetter:      () => this.MasterPassGroupId,

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.String,
            //                              MaxLimit:    36
            //                          ),

            //        Description:      I18NString.Create("IdTokens that have this id as groupId belong to the Master Pass Group. Meaning they can stop any ongoing transaction, but cannot start transactions. This can, for example, be used by law enforcement personal to stop any ongoing transaction when an EV has to be towed away.")

            //    )
            //);

            //#endregion

            //#region OfflineTxForUnknownIdEnabled

            //variableConfigs.Add(
            //    new VariableConfig(

            //        Name:             "OfflineTxForUnknownIdEnabled",
            //        ValueGetter:      () => this.Enabled == true
            //                                    ? "true"
            //                                    : "false",

            //        Attributes:       new VariableAttribute(
            //                              Mutability:  MutabilityTypes.ReadWrite
            //                          ),

            //        Characteristics:  new VariableCharacteristics(
            //                              DataType:    DataTypes.Boolean
            //                          ),

            //        Description:      I18NString.Create("If this key exists, the charging station supports Unknown Offline NTSorization. If this key reports a value of true, Unknown Offline NTSorization is enabled.")

            //    )
            //);

            //#endregion


        }

        #endregion


    }

}
