/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License")";
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

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for measurands.
    /// </summary>
    public static class MeasurandsExtentions
    {

        #region Parse(Text)

        public static Measurands Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Current.Export":
                    return Measurands.Current_Export;

                case "Current.Import":
                    return Measurands.Current_Import;

                case "Current.Offered":
                    return Measurands.Current_Offered;

                case "Energy.Active.Export.Register":
                    return Measurands.Energy_Active_Export_Register;

                case "Energy.Active.Import.Register":
                    return Measurands.Energy_Active_Import_Register;

                case "Energy.Reactive.Export.Register":
                    return Measurands.Energy_Reactive_Export_Register;

                case "Energy.Reactive.Import.Register":
                    return Measurands.Energy_Reactive_Import_Register;

                case "Energy.Active.Export.Interval":
                    return Measurands.Energy_Active_Export_Interval;

                case "Energy.Active.Import.Interval":
                    return Measurands.Energy_Active_Import_Interval;

                case "Energy.Active.Net":
                    return Measurands.Energy_Active_Net;

                case "Energy.Reactive.Export.Interval":
                    return Measurands.Energy_Reactive_Export_Interval;

                case "Energy.Reactive.Import.Interval":
                    return Measurands.Energy_Reactive_Import_Interval;

                case "Energy.Reactive.Net":
                    return Measurands.Energy_Reactive_Net;

                case "Energy.Apparent.Net":
                    return Measurands.Energy_Apparent_Net;

                case "Energy.Apparent.Import":
                    return Measurands.Energy_Apparent_Import;

                case "Energy.Apparent.Export":
                    return Measurands.Energy_Apparent_Export;

                case "Frequency":
                    return Measurands.Frequency;

                case "Power.Active.Export":
                    return Measurands.Power_Active_Export;

                case "Power.Active.Import":
                    return Measurands.Power_Active_Import;

                case "Power.Factor":
                    return Measurands.Power_Factor;

                case "Power.Offered":
                    return Measurands.Power_Offered;

                case "Power.Reactive.Export":
                    return Measurands.Power_Reactive_Export;

                case "Power.Reactive.Import":
                    return Measurands.Power_Reactive_Import;

                case "SoC":
                    return Measurands.SoC;

                case "Voltage":
                    return Measurands.Voltage;


                default:
                    return Measurands.Unknown;

            }

        }

        #endregion

        #region AsText(this Measurands)

        public static String AsText(this Measurands Measurands)
        {

            switch (Measurands)
            {

                case Measurands.Current_Export:
                    return "Current.Export";

                case Measurands.Current_Import:
                    return "Current.Import";

                case Measurands.Current_Offered:
                    return "Current.Offered";

                case Measurands.Energy_Active_Export_Register:
                    return "Energy.Active.Export.Register";

                case Measurands.Energy_Active_Import_Register:
                    return "Energy.Active.Import.Register";

                case Measurands.Energy_Reactive_Export_Register:
                    return "Energy.Reactive.Export.Register";

                case Measurands.Energy_Reactive_Import_Register:
                    return "Energy.Reactive.Import.Register";

                case Measurands.Energy_Active_Export_Interval:
                    return "Energy.Active.Export.Interval";

                case Measurands.Energy_Active_Import_Interval:
                    return "Energy.Active.Import.Interval";

                case Measurands.Energy_Active_Net:
                    return "Energy.Active.Net";

                case Measurands.Energy_Reactive_Export_Interval:
                    return "Energy.Reactive.Export.Interval";

                case Measurands.Energy_Reactive_Import_Interval:
                    return "Energy.Reactive.Import.Interval";

                case Measurands.Energy_Reactive_Net:
                    return "Energy.Reactive.Net";

                case Measurands.Energy_Apparent_Net:
                    return "Energy.Apparent.Net";

                case Measurands.Energy_Apparent_Import:
                    return "Energy.Apparent.Import";

                case Measurands.Energy_Apparent_Export:
                    return "Energy.Apparent.Export";

                case Measurands.Frequency:
                    return "Frequency";

                case Measurands.Power_Active_Export:
                    return "Power.Active.Export";

                case Measurands.Power_Active_Import:
                    return "Power.Active.Import";

                case Measurands.Power_Factor:
                    return "Power.Factor";

                case Measurands.Power_Offered:
                    return "Power.Offered";

                case Measurands.Power_Reactive_Export:
                    return "Power.Reactive.Export";

                case Measurands.Power_Reactive_Import:
                    return "Power.Reactive.Import";

                case Measurands.SoC:
                    return "SoC";

                case Measurands.Voltage:
                    return "Voltage";


                default:
                    return "Unknown";

            }

        }

        #endregion

    }

    /// <summary>
    /// The measurands.
    /// </summary>
    public enum Measurands
    {

        /// <summary>
        /// Unknown charging state.
        /// </summary>
        Unknown,

        Current_Export,
        Current_Import,
        Current_Offered,
        Energy_Active_Export_Register,
        Energy_Active_Import_Register,
        Energy_Reactive_Export_Register,
        Energy_Reactive_Import_Register,
        Energy_Active_Export_Interval,
        Energy_Active_Import_Interval,
        Energy_Active_Net,
        Energy_Reactive_Export_Interval,
        Energy_Reactive_Import_Interval,
        Energy_Reactive_Net,
        Energy_Apparent_Net,
        Energy_Apparent_Import,
        Energy_Apparent_Export,
        Frequency,
        Power_Active_Export,
        Power_Active_Import,
        Power_Factor,
        Power_Offered,
        Power_Reactive_Export,
        Power_Reactive_Import,
        SoC,
        Voltage

    }

}
