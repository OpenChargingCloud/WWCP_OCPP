/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for the measurands.
    /// </summary>
    public static class MeasurandExtensions
    {

        #region Parse(Text)

        public static Measurands Parse(String Text)

            => Text.Trim() switch {
                   "Current.Export"                   => Measurands.CurrentExport,
                   "Current.Import"                   => Measurands.CurrentImport,
                   "Current.Offered"                  => Measurands.CurrentOffered,
                   "Energy.Active.Export.Register"    => Measurands.EnergyReactiveExportRegister,
                   "Energy.Active.Import.Register"    => Measurands.EnergyActiveImportRegister,
                   "Energy.Reactive.Export.Register"  => Measurands.EnergyReactiveExportRegister,
                   "Energy.Reactive.Import.Register"  => Measurands.EnergyReactiveImportRegister,
                   "Energy.Active.Export.Interval"    => Measurands.EnergyActiveExportInterval,
                   "Energy.Active.Import.Interval"    => Measurands.EnergyActiveImportInterval,
                   "Energy.Reactive.Export.Interval"  => Measurands.EnergyReactiveExportInterval,
                   "Energy.Reactive.Import.Interval"  => Measurands.EnergyReactiveImportInterval,
                   "Frequency"                        => Measurands.Frequency,
                   "Power.Active.Export"              => Measurands.PowerActiveExport,
                   "Power.Active.Import"              => Measurands.PowerActiveImport,
                   "Power.Factor"                     => Measurands.PowerFactor,
                   "Power.Offered"                    => Measurands.PowerOffered,
                   "Power.Reactive.Export"            => Measurands.PowerReactiveExport,
                   "Power.Reactive.Import"            => Measurands.PowerReactiveImport,
                   "RPM"                              => Measurands.RPM,
                   "SoC"                              => Measurands.SoC,
                   "Temperature"                      => Measurands.Temperature,
                   "Voltage"                          => Measurands.Voltage,
                   _                                  => Measurands.EnergyActiveImportRegister
               };

        #endregion

        #region AsText(this Measurand)

        public static String AsText(this Measurands Measurand)

            => Measurand switch {
                   Measurands.CurrentExport                 => "Current.Export",
                   Measurands.CurrentImport                 => "Current.Import",
                   Measurands.CurrentOffered                => "Current.Offered",
                   Measurands.EnergyActiveExportRegister    => "Energy.Active.Export.Register",
                   Measurands.EnergyActiveImportRegister    => "Energy.Active.Import.Register",
                   Measurands.EnergyReactiveExportRegister  => "Energy.Reactive.Export.Register",
                   Measurands.EnergyReactiveImportRegister  => "Energy.Reactive.Import.Register",
                   Measurands.EnergyActiveExportInterval    => "Energy.Active.Export.Interval",
                   Measurands.EnergyActiveImportInterval    => "Energy.Active.Import.Interval",
                   Measurands.EnergyReactiveExportInterval  => "Energy.Reactive.Export.Interval",
                   Measurands.EnergyReactiveImportInterval  => "Energy.Reactive.Import.Interval",
                   Measurands.Frequency                     => "Frequency",
                   Measurands.PowerActiveExport             => "Power.Active.Export",
                   Measurands.PowerActiveImport             => "Power.Active.Import",
                   Measurands.PowerFactor                   => "Power.Factor",
                   Measurands.PowerOffered                  => "Power.Offered",
                   Measurands.PowerReactiveExport           => "Power.Reactive.Export",
                   Measurands.PowerReactiveImport           => "Power.Reactive.Import",
                   Measurands.RPM                           => "RPM",
                   Measurands.SoC                           => "SoC",
                   Measurands.Temperature                   => "Temperature",
                   Measurands.Voltage                       => "Voltage",
                   _                                        => "Energy.Active.Import.Register"
               };

        #endregion

    }


    /// <summary>
    /// Allowable values of the optional "measurand" field of a value element,
    /// used in MeterValues request and StopTransaction request messages.
    /// Default value of "measurand" is always "Energy.Active.Import.Register".
    /// </summary>
    public enum Measurands
    {

        /// <summary>
        /// Instantaneous current flow from EV.
        /// </summary>
        CurrentExport,

        /// <summary>
        /// Instantaneous current flow to EV.
        /// </summary>
        CurrentImport,

        /// <summary>
        /// Maximum current offered to EV.
        /// </summary>
        CurrentOffered,

        /// <summary>
        /// Energy exported by EV (Wh or kWh).
        /// </summary>
        EnergyActiveExportRegister,

        /// <summary>
        /// Energy imported by EV (Wh or kWh).
        /// </summary>
        EnergyActiveImportRegister,

        /// <summary>
        /// Reactive energy exported by EV (varh or kvarh).
        /// </summary>
        EnergyReactiveExportRegister,

        /// <summary>
        /// Reactive energy imported by EV (varh or kvarh).
        /// </summary>
        EnergyReactiveImportRegister,

        /// <summary>
        /// Energy exported by EV (Wh or kWh).
        /// </summary>
        EnergyActiveExportInterval,

        /// <summary>
        /// Energy imported by EV (Wh or kWh).
        /// </summary>
        EnergyActiveImportInterval,

        /// <summary>
        /// Reactive energy exported by EV. (varh or kvarh).
        /// </summary>
        EnergyReactiveExportInterval,

        /// <summary>
        /// Reactive energy imported by EV. (varh or kvarh).
        /// </summary>
        EnergyReactiveImportInterval,

        /// <summary>
        /// Instantaneous reading of powerline frequency.
        /// </summary>
        Frequency,

        /// <summary>
        /// Instantaneous active power exported by EV. (W or kW).
        /// </summary>
        PowerActiveExport,

        /// <summary>
        /// Instantaneous active power imported by EV. (W or kW).
        /// </summary>
        PowerActiveImport,

        /// <summary>
        /// Instantaneous power factor of total energy flow.
        /// </summary>
        PowerFactor,

        /// <summary>
        /// Maximum power offered to EV.
        /// </summary>
        PowerOffered,

        /// <summary>
        /// Instantaneous reactive power exported by EV. (var or kvar).
        /// </summary>
        PowerReactiveExport,

        /// <summary>
        /// Instantaneous reactive power imported by EV. (var or kvar).
        /// </summary>
        PowerReactiveImport,

        /// <summary>
        /// Fan speed in RPM.
        /// </summary>
        RPM,

        /// <summary>
        /// State of charge of charging vehicle in percentage.
        /// </summary>
        SoC,

        /// <summary>
        /// Temperature reading inside of the charge point.
        /// </summary>
        Temperature,

        /// <summary>
        /// Instantaneous AC RMS supply voltage.
        /// </summary>
        Voltage

    }

}