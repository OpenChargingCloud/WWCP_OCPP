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

#region Usings

using System;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Extention methods for the measurands.
    /// </summary>
    public static class MeasurandExtentions
    {

        #region AsMeasurand(Text)

        public static Measurands Parse(this String Text)
        {

            switch (Text)
            {

                case "Current.Export":
                    return Measurands.CurrentExport;

                case "Current.Import":
                    return Measurands.CurrentImport;

                case "Current.Offered":
                    return Measurands.CurrentOffered;

                case "Energy.Active.Export.Register":
                    return Measurands.EnergyReactiveExportRegister;

                case "Energy.Active.Import.Register":
                    return Measurands.EnergyActiveImportRegister;

                case "Energy.Reactive.Export.Register":
                    return Measurands.EnergyReactiveExportRegister;

                case "Energy.Reactive.Import.Register":
                    return Measurands.EnergyReactiveImportRegister;

                case "Energy.Active.Export.Interval":
                    return Measurands.EnergyActiveExportInterval;

                case "Energy.Active.Import.Interval":
                    return Measurands.EnergyActiveImportInterval;

                case "Energy.Reactive.Export.Interval":
                    return Measurands.EnergyReactiveExportInterval;

                case "Energy.Reactive.Import.Interval":
                    return Measurands.EnergyReactiveImportInterval;

                case "Frequency":
                    return Measurands.Frequency;

                case "Power.Active.Export":
                    return Measurands.PowerActiveExport;

                case "Power.Active.Import":
                    return Measurands.PowerActiveImport;

                case "Power.Factor":
                    return Measurands.PowerFactor;

                case "Power.Offered":
                    return Measurands.PowerOffered;

                case "Power.Reactive.Export":
                    return Measurands.PowerReactiveExport;

                case "Power.Reactive.Import":
                    return Measurands.PowerReactiveImport;

                case "RPM":
                    return Measurands.RPM;

                case "SoC":
                    return Measurands.SoC;

                case "Temperature":
                    return Measurands.Temperature;

                case "Voltage":
                    return Measurands.Voltage;


                default:
                    return Measurands.EnergyActiveImportRegister;

            }

        }

        #endregion

        #region AsText(this Measurand)

        public static String AsText(this Measurands Measurand)
        {

            switch (Measurand)
            {

                case Measurands.CurrentExport:
                    return "Current.Export";

                case Measurands.CurrentImport:
                    return "Current.Import";

                case Measurands.CurrentOffered:
                    return "Current.Offered";

                case Measurands.EnergyActiveExportRegister:
                    return "Energy.Active.Export.Register";

                case Measurands.EnergyActiveImportRegister:
                    return "Energy.Active.Import.Register";

                case Measurands.EnergyReactiveExportRegister:
                    return "Energy.Reactive.Export.Register";

                case Measurands.EnergyReactiveImportRegister:
                    return "Energy.Reactive.Import.Register";

                case Measurands.EnergyActiveExportInterval:
                    return "Energy.Active.Export.Interval";

                case Measurands.EnergyActiveImportInterval:
                    return "Energy.Active.Import.Interval";

                case Measurands.EnergyReactiveExportInterval:
                    return "Energy.Reactive.Export.Interval";

                case Measurands.EnergyReactiveImportInterval:
                    return "Energy.Reactive.Import.Interval";

                case Measurands.Frequency:
                    return "Frequency";

                case Measurands.PowerActiveExport:
                    return "Power.Active.Export";

                case Measurands.PowerActiveImport:
                    return "Power.Active.Import";

                case Measurands.PowerFactor:
                    return "Power.Factor";

                case Measurands.PowerOffered:
                    return "Power.Offered";

                case Measurands.PowerReactiveExport:
                    return "Power.Reactive.Export";

                case Measurands.PowerReactiveImport:
                    return "Power.Reactive.Import";

                case Measurands.RPM:
                    return "RPM";

                case Measurands.SoC:
                    return "SoC";

                case Measurands.Temperature:
                    return "Temperature";

                case Measurands.Voltage:
                    return "Voltage";


                default:
                    return "Energy.Active.Import.Register";

            }

        }

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