/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

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