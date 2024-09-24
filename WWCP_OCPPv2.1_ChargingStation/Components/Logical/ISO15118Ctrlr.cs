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
    /// Communicates with an EV to exchange information and control charging using the ISO 15118 protocol.
    /// </summary>
    public class ISO15118Ctrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// ISO 15118 controller enabled.
        /// </summary>
        public Boolean?             Enabled                             { get; set; }

        /// <summary>
        /// Connected.
        /// </summary>
        public Boolean?             Active                              { get; set; }

        /// <summary>
        /// ISO 15118 communication session aborted.
        /// </summary>
        public Boolean?             Tripped                             { get; set; }

        /// <summary>
        /// ISO 15118 communication session ended.
        /// </summary>
        public Boolean?             Complete                            { get; set; }

        /// <summary>
        /// ISO 15118 controller fault.
        /// </summary>
        public Boolean?             Problem                             { get; set; }

        /// <summary>
        /// Start self-test by setting to true.
        /// </summary>
        public Boolean?             SelftestActiveSet                   { get; set; }

        /// <summary>
        /// Self-test running when reported as true.
        /// </summary>
        public Boolean?             SelftestActive                      { get; set; }

        /// <summary>
        /// Supports validation of a contract certificate when offline.
        /// </summary>
        public Boolean?             ContractValidationOffline           { get; set; }

        /// <summary>
        /// Contract certificates can be validated by the CSMS.
        /// </summary>
        public Boolean?             CentralContractValidationAllowed    { get; set; }

        /// <summary>
        /// The name of the EVSE in the string format as required by ISO 15118 and IEC 63119-2.
        /// </summary>
        public String?              EvseId                              { get; set; }

        /// <summary>
        /// If this variable is true, then ISO 15118 plug and charge is enabled.
        /// If this variable is false, then the Charging Station won’t initiate ISO 15118 CSRs.
        /// </summary>
        public Boolean?             PnCEnabled                          { get; set; }

        /// <summary>
        /// A string with the following comma-separated items: “<uri>,<major>,<minor>”.
        /// This is the protocol uri and version information that was agreed upon between EV and EVSE in the supportedAppProtocolReq handshake from ISO 15118.
        /// Example: "urn:iso:15118:2:2013:MsgDef,2,0"
        /// </summary>
        public String?              ProtocolAgreed                      { get; set; }

        /// <summary>
        /// A string with the following comma-separated items: “<uri>,<major>,<minor>”.
        /// This is information from the supportedAppProtocolReq message from ISO 15118.
        /// Variable has multiple instances, one for each priority.
        /// Example: "urn:iso:15118:2:2013:MsgDef,2,0"
        /// </summary>
        public String?              ProtocolSupportedByEV               { get; set; }

        /// <summary>
        /// If this variable is true, then Charging Station shall request a metering receipt from EV before sending a fiscal meter value to CSMS.
        /// </summary>
        public Boolean?             RequestMeteringReceipt              { get; set; }


        /// <summary>
        /// MaxEntriesSAScheduleType (15118-2) or MaximumSupportingPoints (15118-20) Number of allowed schedule periods.
        /// </summary>
        public UInt32?              MaxScheduleEntries                  { get; set; }

        /// <summary>
        /// RequestedEnergyTransferMode "AC_single_phase_core", "AC_three_phase_core", "DC_core, "DC_extended", "DC_combo_core", "DC_unique".
        /// </summary>
        public EnergyTransferMode?  RequestedEnergyTransferMode         { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ISO 15118 controller.
        /// </summary>
        /// <param name="Enabled">ISO 15118 controller enabled.</param>
        /// <param name="Active">Connected.</param>
        /// <param name="Tripped">ISO 15118 communication session aborted.</param>
        /// <param name="Complete">ISO 15118 communication session ended.</param>
        /// <param name="Problem">ISO 15118 controller fault.</param>
        /// <param name="SelftestActiveSet">Start self-test by setting to true.</param>
        /// <param name="SelftestActive">Self-test running when reported as true.</param>
        /// <param name="ContractValidationOffline">Supports validation of a contract certificate when offline.</param>
        /// <param name="CentralContractValidationAllowed">Contract certificates can be validated by the CSMS.</param>
        /// <param name="EvseId">The name of the EVSE in the string format as required by ISO 15118 and IEC 63119-2.</param>
        /// <param name="PnCEnabled">If this variable is true, then ISO 15118 plug and charge is enabled. If this variable is false, then the Charging Station won’t initiate ISO 15118 CSRs.</param>
        /// <param name="ProtocolAgreed">A string with the following comma-separated items: “<uri>,<major>,<minor>”. This is the protocol uri and version information that was agreed upon between EV and EVSE in the supportedAppProtocolReq handshake from ISO 15118. Example: "urn:iso:15118:2:2013:MsgDef,2,0"</param>
        /// <param name="ProtocolSupportedByEV">A string with the following comma-separated items: “<uri>,<major>,<minor>”. This is information from the supportedAppProtocolReq message from ISO 15118. Variable has multiple instances, one for each priority. Example: "urn:iso:15118:2:2013:MsgDef,2,0"</param>
        /// <param name="RequestMeteringReceipt">If this variable is true, then Charging Station shall request a metering receipt from EV before sending a fiscal meter value to CSMS.</param>
        /// 
        /// <param name="MaxScheduleEntries">MaxEntriesSAScheduleType (15118-2) or MaximumSupportingPoints (15118-20) Number of allowed schedule periods.</param>
        /// <param name="RequestedEnergyTransferMode">RequestedEnergyTransferMode "AC_single_phase_core", "AC_three_phase_core", "DC_core, "DC_extended", "DC_combo_core", "DC_unique".</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ISO15118Ctrlr(Boolean?             Enabled                            = null,
                             Boolean?             Active                             = null,
                             Boolean?             Tripped                            = null,
                             Boolean?             Complete                           = null,
                             Boolean?             Problem                            = null,
                             Boolean?             SelftestActiveSet                  = null,
                             Boolean?             SelftestActive                     = null,
                             Boolean?             ContractValidationOffline          = null,
                             Boolean?             CentralContractValidationAllowed   = null,
                             String?              EvseId                             = null,
                             Boolean?             PnCEnabled                         = null,
                             String?              ProtocolAgreed                     = null,
                             String?              ProtocolSupportedByEV              = null,
                             Boolean?             RequestMeteringReceipt             = null,

                             UInt32?              MaxScheduleEntries                 = null,
                             EnergyTransferMode?  RequestedEnergyTransferMode        = null,

                             String?              Instance                           = null,
                             CustomData?          CustomData                         = null)

            : base(nameof(ISO15118Ctrlr),
                   Instance,
                   I18NString.Create("Communicates with an EV to exchange information and control charging using the ISO 15118 protocol."),
                   CustomData)

        {

            this.Enabled                           = Enabled;
            this.Active                            = Active;
            this.Tripped                           = Tripped;
            this.Complete                          = Complete;
            this.Problem                           = Problem;
            this.SelftestActiveSet                 = SelftestActiveSet;
            this.SelftestActive                    = SelftestActive;
            this.ContractValidationOffline         = ContractValidationOffline;
            this.CentralContractValidationAllowed  = CentralContractValidationAllowed;
            this.EvseId                            = EvseId;
            this.PnCEnabled                        = PnCEnabled;
            this.ProtocolAgreed                    = ProtocolAgreed;
            this.ProtocolSupportedByEV             = ProtocolSupportedByEV;
            this.RequestMeteringReceipt            = RequestMeteringReceipt;

            this.MaxScheduleEntries                = MaxScheduleEntries;
            this.RequestedEnergyTransferMode       = RequestedEnergyTransferMode;       //ToDo: How does this list fit to ISO 15118?


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

                    Description:      I18NString.Create("ISO 15118 controller enabled.")

                )
            );

            #endregion

            #region Active

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Active",
                    ValueGetter:      () => this.Active.HasValue
                                                ? this.Active.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Connected.")

                )
            );

            #endregion

            #region Tripped

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Tripped",
                    ValueGetter:      () => this.Tripped.HasValue
                                                ? this.Tripped.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("ISO 15118 communication session aborted.")

                )
            );

            #endregion

            #region Complete

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Complete",
                    ValueGetter:      () => this.Complete.HasValue
                                                ? this.Complete.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("ISO 15118 communication session ended.")

                )
            );

            #endregion

            #region Problem

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Problem",
                    ValueGetter:      () => this.Problem.HasValue
                                                ? this.Problem.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("ISO 15118 controller fault.")

                )
            );

            #endregion

            #region SelftestActiveSet

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SelftestActiveSet",
                    ValueGetter:      () => this.SelftestActiveSet.HasValue
                                                ? this.SelftestActiveSet.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Start self-test by setting to true.")

                )
            );

            #endregion

            #region SelftestActive

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SelftestActive",
                    ValueGetter:      () => this.SelftestActive.HasValue
                                                ? this.SelftestActive.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Self-test running when reported as true.")

                )
            );

            #endregion

            #region ContractValidationOffline

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ContractValidationOffline",
                    ValueGetter:      () => this.ContractValidationOffline.HasValue
                                                ? this.ContractValidationOffline.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Supports validation of a contract certificate when offline.")

                )
            );

            #endregion

            #region CentralContractValidationAllowed

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "CentralContractValidationAllowed",
                    ValueGetter:      () => this.CentralContractValidationAllowed.HasValue
                                                ? this.CentralContractValidationAllowed.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Contract certificates can be validated by the CSMS.")

                )
            );

            #endregion

            #region EvseId

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "EvseId",
                    ValueGetter:      () => this.EvseId?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("The name of the EVSE in the string format as required by ISO 15118 and IEC 63119-2.")

                )
            );

            #endregion

            #region PnCEnabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "PnCEnabled",
                    ValueGetter:      () => this.PnCEnabled.HasValue
                                                ? this.PnCEnabled.Value
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
                                          "If this variable is true, then ISO 15118 plug and charge is enabled. " +
                                          "If this variable is false, then the Charging Station won’t initiate ISO 15118 CSRs."
                                      )

                )
            );

            #endregion

            #region ProtocolAgreed

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ProtocolAgreed",
                    ValueGetter:      () => this.ProtocolAgreed?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create(
                                          "A string with the following comma-separated items: “<uri>,<major>,<minor>”. " +
                                          "This is the protocol uri and version information that was agreed upon between " +
                                          "EV and EVSE in the supportedAppProtocolReq handshake from ISO 15118. " +
                                          "Example: \"urn:iso:15118:2:2013:MsgDef,2,0\""
                                      )

                )
            );

            #endregion

            #region ProtocolSupportedByEV

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ProtocolSupportedByEV",
                    ValueGetter:      () => this.ProtocolSupportedByEV?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create(
                                          "A string with the following comma-separated items: “<uri>,<major>,<minor>”. " +
                                          "This is information from the supportedAppProtocolReq message from ISO 15118. " +
                                          "Variable has multiple instances, one for each priority. " +
                                          "Example: \"urn:iso:15118:2:2013:MsgDef,2,0\""
                                      )

                )
            );

            #endregion

            #region RequestMeteringReceipt

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "RequestMeteringReceipt",
                    ValueGetter:      () => this.RequestMeteringReceipt.HasValue
                                                ? this.RequestMeteringReceipt.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable is true, then Charging Station shall request a metering receipt from EV before sending a fiscal meter value to CSMS.")

                )
            );

            #endregion


            // Specific ISO15118 interface data from vehicle:

            #region MaxScheduleEntries

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MaxScheduleEntries",
                    ValueGetter:      () => this.MaxScheduleEntries?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("MaxEntriesSAScheduleType (15118-2) or MaximumSupportingPoints (15118-20) Number of allowed schedule periods.")

                )
            );

            #endregion

            #region RequestedEnergyTransferMode     (How does this list fit to ISO 15118)?

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "RequestedEnergyTransferMode",
                    ValueGetter:      () => this.RequestedEnergyTransferMode?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.OptionList
                                      ),

                    Description:      I18NString.Create("RequestedEnergyTransferMode \"AC_single_phase_core\", \"AC_three_phase_core\", \"DC_core, \"DC_extended\", \"DC_combo_core\", \"DC_unique\".")

                )
            );

            #endregion


        }

        #endregion


    }

}
