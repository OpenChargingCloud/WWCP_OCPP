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
    /// A CHAdeMO controller component communicates with an EV using the wired CANbus protocol
    /// to exchange information and control charging using the CHAdeMO protocol.
    /// </summary>
    public class CHAdeMOCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// CHAdeMO controller enabled
        /// </summary>
        public Boolean?  Enabled                  { get; set; }

        /// <summary>
        /// Connected
        /// </summary>
        public Boolean?  Active                   { get; set; }

        /// <summary>
        /// Protocol session ended normally
        /// </summary>
        public Boolean?  Complete                 { get; set; }

        /// <summary>
        /// CHAdeMO protocol terminated abnormally
        /// </summary>
        public Boolean?  Tripped                  { get; set; }

        /// <summary>
        /// CHAdeMO controller fault
        /// </summary>
        public Boolean?  Problem                  { get; set; }

        /// <summary>
        /// Start self-test by setting to true
        /// </summary>
        public Boolean?  SelftestActiveSet        { get; set; }

        /// <summary>
        /// Self-test running when reported as true
        /// </summary>
        public Boolean?  SelftestActive           { get; set; }



        // Specific CHAdeMO interface data from vehicle...

        /// <summary>
        /// CHAdeMO protocol number (H'102.0)
        /// </summary>
        public UInt32?   CHAdeMOProtocolNumber    { get; set; }

        /// <summary>
        /// Vehicle status (H'102.5.3)
        /// </summary>
        public Boolean?  VehicleStatus            { get; set; }

        /// <summary>
        /// Vehicle is compatible with dynamic control (H'110.0.0)
        /// </summary>
        public Boolean?  DynamicControl           { get; set; }

        /// <summary>
        /// Vehicle is compatible with high current control (H'110.0.1)
        /// </summary>
        public Boolean?  HighCurrentControl       { get; set; }

        /// <summary>
        /// Vehicle is compatible with high voltage control (H'110.1.2)
        /// </summary>
        public Boolean?  HighVoltageControl       { get; set; }

        /// <summary>
        /// Auto manufacturer code (H'700.0) A single byte manufacturer code assigned by CHAdeMO association
        /// </summary>
        public UInt32?   AutoManufacturerCode     { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CHAdeMO controller.
        /// </summary>
        /// <param name="Enabled">CHAdeMO controller enabled</param>
        /// <param name="Active">Connected</param>
        /// <param name="Complete">Protocol session ended normally</param>
        /// <param name="Tripped">CHAdeMO protocol terminated abnormally</param>
        /// <param name="Problem">CHAdeMO controller fault</param>
        /// <param name="SelftestActiveSet">Start self-test by setting to true</param>
        /// <param name="SelftestActive">Self-test running when reported as true</param>
        /// 
        /// <param name="CHAdeMOProtocolNumber">CHAdeMO protocol number (H'102.0)</param>
        /// <param name="VehicleStatus">Vehicle status (H'102.5.3)</param>
        /// <param name="DynamicControl">Vehicle is compatible with dynamic control (H'110.0.0)</param>
        /// <param name="HighCurrentControl">Vehicle is compatible with high current control (H'110.0.1)</param>
        /// <param name="HighVoltageControl">Vehicle is compatible with high voltage control (H'110.1.2)</param>
        /// <param name="AutoManufacturerCode">Auto manufacturer code (H'700.0) A single byte manufacturer code assigned by CHAdeMO association</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CHAdeMOCtrlr(Boolean?     Enabled,
                            Boolean?     Active,
                            Boolean?     Complete,
                            Boolean?     Tripped,
                            Boolean?     Problem,
                            Boolean?     SelftestActiveSet,
                            Boolean?     SelftestActive,

                            UInt32?      CHAdeMOProtocolNumber,
                            Boolean?     VehicleStatus,
                            Boolean?     DynamicControl,
                            Boolean?     HighCurrentControl,
                            Boolean?     HighVoltageControl,
                            UInt32?      AutoManufacturerCode,

                            String?      Instance     = null,
                            CustomData?  CustomData   = null)

            : base(nameof(CHAdeMOCtrlr),
                   Instance,
                   I18NString.Create("A CHAdeMO Controller component communicates with an EV using the wired CANbus protocol to exchange information and control charging using the CHAdeMO protocol."),
                   CustomData)

        {

            this.Enabled                 = Enabled;
            this.Active                  = Active;
            this.Complete                = Complete;
            this.Tripped                 = Tripped;
            this.Problem                 = Problem;
            this.SelftestActiveSet       = SelftestActiveSet;
            this.SelftestActive          = SelftestActive;

            this.CHAdeMOProtocolNumber   = CHAdeMOProtocolNumber;
            this.VehicleStatus           = VehicleStatus;
            this.DynamicControl          = DynamicControl;
            this.HighCurrentControl      = HighCurrentControl;
            this.HighVoltageControl      = HighVoltageControl;
            this.AutoManufacturerCode    = AutoManufacturerCode;


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

                    Description:      I18NString.Create("CHAdeMO controller enabled")

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

                    Description:      I18NString.Create("Connected")

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

                    Description:      I18NString.Create("Protocol session ended normally")

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

                    Description:      I18NString.Create("CHAdeMO protocol terminated abnormally")

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

                    Description:      I18NString.Create("CHAdeMO controller fault")

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

                    Description:      I18NString.Create("Start self-test by setting to true")

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
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Self-test running when reported as true")

                )
            );

            #endregion


            #region CHAdeMOProtocolNumber

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "CHAdeMOProtocolNumber",
                    ValueGetter:      () => this.CHAdeMOProtocolNumber?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("CHAdeMO protocol number (H'102.0)")

                )
            );

            #endregion

            #region VehicleStatus

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "VehicleStatus",
                    ValueGetter:      () => this.VehicleStatus.HasValue
                                                ? this.VehicleStatus.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Vehicle status (H'102.5.3)")

                )
            );

            #endregion

            #region DynamicControl

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DynamicControl",
                    ValueGetter:      () => this.DynamicControl.HasValue
                                                ? this.DynamicControl.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Vehicle is compatible with dynamic control (H'110.0.0)")

                )
            );

            #endregion

            #region HighCurrentControl

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "HighCurrentControl",
                    ValueGetter:      () => this.HighCurrentControl.HasValue
                                                ? this.HighCurrentControl.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Vehicle is compatible with high current control (H'110.0.1)")

                )
            );

            #endregion

            #region HighVoltageControl

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "HighVoltageControl",
                    ValueGetter:      () => this.HighVoltageControl.HasValue
                                                ? this.HighVoltageControl.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Vehicle is compatible with high voltage control (H'110.1.2)")

                )
            );

            #endregion

            #region AutoManufacturerCode

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "AutoManufacturerCode",
                    ValueGetter:      () => this.HighVoltageControl.HasValue
                                                ? this.HighVoltageControl.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Auto manufacturer code (H'700.0) A single byte manufacturer code assigned by CHAdeMO association")

                )
            );

            #endregion


        }

        #endregion


    }

}
