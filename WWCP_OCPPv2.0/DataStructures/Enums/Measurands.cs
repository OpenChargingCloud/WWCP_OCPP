/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for measurands.
    /// </summary>
    public static class MeasurandsExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurands Parse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            return Measurands.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurands? TryParse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            return null;

        }

        #endregion

        #region TryParse(Text, out Measurand)

        /// <summary>
        /// Try to parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        /// <param name="Measurand">The parsed measurand.</param>
        public static Boolean TryParse(String Text, out Measurands Measurand)
        {
            switch (Text.Trim())
            {

                case "Current.Export":
                    Measurand = Measurands.Current_Export;
                    return true;

                case "Current.Import":
                    Measurand = Measurands.Current_Import;
                    return true;

                case "Current.Offered":
                    Measurand = Measurands.Current_Offered;
                    return true;


                case "Energy.Active.Export.Register":
                    Measurand = Measurands.Energy_Active_Export_Register;
                    return true;

                case "Energy.Active.Import.Register":
                    Measurand = Measurands.Energy_Active_Import_Register;
                    return true;


                case "Energy.Reactive.Export.Register":
                    Measurand = Measurands.Energy_Reactive_Export_Register;
                    return true;

                case "Energy.Reactive.Import.Register":
                    Measurand = Measurands.Energy_Reactive_Import_Register;
                    return true;


                case "Energy.Active.Export.Interval":
                    Measurand = Measurands.Energy_Active_Export_Interval;
                    return true;

                case "Energy.Active.Import.Interval":
                    Measurand = Measurands.Energy_Active_Import_Interval;
                    return true;

                case "Energy.Active.Net":
                    Measurand = Measurands.Energy_Active_Net;
                    return true;


                case "Energy.Reactive.Import.Interval":
                    Measurand = Measurands.Energy_Reactive_Import_Interval;
                    return true;

                case "Energy.Reactive.Export.Interval":
                    Measurand = Measurands.Energy_Reactive_Export_Interval;
                    return true;

                case "Energy.Reactive.Net":
                    Measurand = Measurands.Energy_Reactive_Net;
                    return true;


                case "Energy.Apparent.Import":
                    Measurand = Measurands.Energy_Apparent_Import;
                    return true;

                case "Energy.Apparent.Export":
                    Measurand = Measurands.Energy_Apparent_Export;
                    return true;

                case "Energy.Apparent.Net":
                    Measurand = Measurands.Energy_Apparent_Net;
                    return true;


                case "Power.Active.Export":
                    Measurand = Measurands.Power_Active_Export;
                    return true;

                case "Power.Active.Import":
                    Measurand = Measurands.Power_Active_Import;
                    return true;


                case "Power.Reactive.Export":
                    Measurand = Measurands.Power_Reactive_Export;
                    return true;

                case "Power.Reactive.Import":
                    Measurand = Measurands.Power_Reactive_Import;
                    return true;


                case "Power.Factor":
                    Measurand = Measurands.Power_Factor;
                    return true;

                case "Power.Offered":
                    Measurand = Measurands.Power_Offered;
                    return true;


                case "Frequency":
                    Measurand = Measurands.Frequency;
                    return true;

                case "Voltage":
                    Measurand = Measurands.Voltage;
                    return true;

                case "SoC":
                    Measurand = Measurands.SoC;
                    return true;


                default:
                    Measurand = Measurands.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Measurand)

        public static String AsText(this Measurands Measurand)

            => Measurand switch {

                   Measurands.Current_Export                   => "Current.Export",
                   Measurands.Current_Import                   => "Current.Import",
                   Measurands.Current_Offered                  => "Current.Offered",

                   Measurands.Energy_Active_Export_Register    => "Energy.Active.Export.Register",
                   Measurands.Energy_Active_Import_Register    => "Energy.Active.Import.Register",

                   Measurands.Energy_Reactive_Export_Register  => "Energy.Reactive.Export.Register",
                   Measurands.Energy_Reactive_Import_Register  => "Energy.Reactive.Import.Register",

                   Measurands.Energy_Active_Export_Interval    => "Energy.Active.Export.Interval",
                   Measurands.Energy_Active_Import_Interval    => "Energy.Active.Import.Interval",
                   Measurands.Energy_Active_Net                => "Energy.Active.Net",

                   Measurands.Energy_Reactive_Export_Interval  => "Energy.Reactive.Export.Interval",
                   Measurands.Energy_Reactive_Import_Interval  => "Energy.Reactive.Import.Interval",
                   Measurands.Energy_Reactive_Net              => "Energy.Reactive.Net",

                   Measurands.Energy_Apparent_Import           => "Energy.Apparent.Import",
                   Measurands.Energy_Apparent_Export           => "Energy.Apparent.Export",
                   Measurands.Energy_Apparent_Net              => "Energy.Apparent.Net",

                   Measurands.Power_Active_Export              => "Power.Active.Export",
                   Measurands.Power_Active_Import              => "Power.Active.Import",

                   Measurands.Power_Reactive_Export            => "Power.Reactive.Export",
                   Measurands.Power_Reactive_Import            => "Power.Reactive.Import",

                   Measurands.Power_Factor                     => "Power.Factor",
                   Measurands.Power_Offered                    => "Power.Offered",

                   Measurands.Frequency                        => "Frequency",
                   Measurands.Voltage                          => "Voltage",
                   Measurands.SoC                              => "SoC",
                   _                                           => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// Measurands.
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

        Energy_Apparent_Export,
        Energy_Apparent_Import,
        Energy_Apparent_Net,

        Power_Active_Export,
        Power_Active_Import,

        Power_Reactive_Export,
        Power_Reactive_Import,

        Power_Factor,
        Power_Offered,

        Frequency,
        Voltage,
        SoC

    }

}
