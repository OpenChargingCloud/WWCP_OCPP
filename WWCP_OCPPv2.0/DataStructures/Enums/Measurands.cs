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

        #region Parse(Text)

        public static Measurands Parse(String Text)

            => Text.Trim() switch {
                   "Current.Export"                   => Measurands.Current_Export,
                   "Current.Import"                   => Measurands.Current_Import,
                   "Current.Offered"                  => Measurands.Current_Offered,
                   "Energy.Active.Export.Register"    => Measurands.Energy_Active_Export_Register,
                   "Energy.Active.Import.Register"    => Measurands.Energy_Active_Import_Register,
                   "Energy.Reactive.Export.Register"  => Measurands.Energy_Reactive_Export_Register,
                   "Energy.Reactive.Import.Register"  => Measurands.Energy_Reactive_Import_Register,
                   "Energy.Active.Export.Interval"    => Measurands.Energy_Active_Export_Interval,
                   "Energy.Active.Import.Interval"    => Measurands.Energy_Active_Import_Interval,
                   "Energy.Active.Net"                => Measurands.Energy_Active_Net,
                   "Energy.Reactive.Export.Interval"  => Measurands.Energy_Reactive_Export_Interval,
                   "Energy.Reactive.Import.Interval"  => Measurands.Energy_Reactive_Import_Interval,
                   "Energy.Reactive.Net"              => Measurands.Energy_Reactive_Net,
                   "Energy.Apparent.Net"              => Measurands.Energy_Apparent_Net,
                   "Energy.Apparent.Import"           => Measurands.Energy_Apparent_Import,
                   "Energy.Apparent.Export"           => Measurands.Energy_Apparent_Export,
                   "Frequency"                        => Measurands.Frequency,
                   "Power.Active.Export"              => Measurands.Power_Active_Export,
                   "Power.Active.Import"              => Measurands.Power_Active_Import,
                   "Power.Factor"                     => Measurands.Power_Factor,
                   "Power.Offered"                    => Measurands.Power_Offered,
                   "Power.Reactive.Export"            => Measurands.Power_Reactive_Export,
                   "Power.Reactive.Import"            => Measurands.Power_Reactive_Import,
                   "SoC"                              => Measurands.SoC,
                   "Voltage"                          => Measurands.Voltage,
                   _                                  => Measurands.Unknown
               };

        #endregion

        #region AsText(this Measurands)

        public static String AsText(this Measurands Measurands)

            => Measurands switch {
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
                Measurands.Energy_Apparent_Net              => "Energy.Apparent.Net",
                Measurands.Energy_Apparent_Import           => "Energy.Apparent.Import",
                Measurands.Energy_Apparent_Export           => "Energy.Apparent.Export",
                Measurands.Frequency                        => "Frequency",
                Measurands.Power_Active_Export              => "Power.Active.Export",
                Measurands.Power_Active_Import              => "Power.Active.Import",
                Measurands.Power_Factor                     => "Power.Factor",
                Measurands.Power_Offered                    => "Power.Offered",
                Measurands.Power_Reactive_Export            => "Power.Reactive.Export",
                Measurands.Power_Reactive_Import            => "Power.Reactive.Import",
                Measurands.SoC                              => "SoC",
                Measurands.Voltage                          => "Voltage",
                _                                           => "Unknown"
            };

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
