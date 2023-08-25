/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for measurands.
    /// </summary>
    public static class MeasurandsExtensions
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

                case "Current.Import.Offered":
                    Measurand = Measurands.Current_Import_Offered;
                    return true;

                case "Current.Import.Minimum":
                    Measurand = Measurands.Current_Import_Minimum;
                    return true;

                case "Current.Export.Offered":
                    Measurand = Measurands.Current_Export_Offered;
                    return true;

                case "Current.Export.Minimum":
                    Measurand = Measurands.Current_Export_Minimum;
                    return true;

                case "Display.PresentSOC":
                    Measurand = Measurands.Display_PresentSOC;
                    return true;

                case "Display.MinimumSOC":
                    Measurand = Measurands.Display_MinimumSOC;
                    return true;

                case "Display.TargetSOC":
                    Measurand = Measurands.Display_TargetSOC;
                    return true;

                case "Display.MaximumSOC":
                    Measurand = Measurands.Display_MaximumSOC;
                    return true;

                case "Display.RemainingTimeToMinimumSOC":
                    Measurand = Measurands.Display_RemainingTimeToMinimumSOC;
                    return true;

                case "Display.RemainingTimeToTargetSOC":
                    Measurand = Measurands.Display_RemainingTimeToTargetSOC;
                    return true;

                case "Display.RemainingTimeToMaximumSOC":
                    Measurand = Measurands.Display_RemainingTimeToMaximumSOC;
                    return true;

                case "Display.ChargingComplete":
                    Measurand = Measurands.Display_ChargingComplete;
                    return true;

                case "Display.BatteryEnergyCapacity":
                    Measurand = Measurands.Display_BatteryEnergyCapacity;
                    return true;

                case "Display.InletHot":
                    Measurand = Measurands.Display_InletHot;
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


                case "Energy.Active.Setpoint.Interval":
                    Measurand = Measurands.Energy_Active_Setpoint_Interval;
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


                case "Energy.Apparent.Export":
                    Measurand = Measurands.Energy_Apparent_Export;
                    return true;

                case "Energy.Apparent.Import":
                    Measurand = Measurands.Energy_Apparent_Import;
                    return true;

                case "Energy.Apparent.Net":
                    Measurand = Measurands.Energy_Apparent_Net;
                    return true;


                case "EnergyRequest.Target":
                    Measurand = Measurands.EnergyRequest_Target;
                    return true;

                case "EnergyRequest.Minimum":
                    Measurand = Measurands.EnergyRequest_Minimum;
                    return true;

                case "EnergyRequest.Maximum":
                    Measurand = Measurands.EnergyRequest_Maximum;
                    return true;

                case "EnergyRequest.Minimum.V2X":
                    Measurand = Measurands.EnergyRequest_Minimum_V2X;
                    return true;

                case "EnergyRequest.Maximum.V2X":
                    Measurand = Measurands.EnergyRequest_Maximum_V2X;
                    return true;

                case "EnergyRequest.Bulk":
                    Measurand = Measurands.EnergyRequest_Bulk;
                    return true;


                case "Power.Active.Export":
                    Measurand = Measurands.Power_Active_Export;
                    return true;

                case "Power.Active.Import":
                    Measurand = Measurands.Power_Active_Import;
                    return true;


                case "Power.Active.Setpoint":
                    Measurand = Measurands.Power_Active_Setpoint;
                    return true;

                case "Power.Active.Residual":
                    Measurand = Measurands.Power_Active_Residual;
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


                case "Power.Import.Offered":
                    Measurand = Measurands.Power_Import_Offered;
                    return true;

                case "Power.Import.Minimum":
                    Measurand = Measurands.Power_Import_Minimum;
                    return true;

                case "Power.Export.Offered":
                    Measurand = Measurands.Power_Export_Offered;
                    return true;

                case "Power.Export.Minimum":
                    Measurand = Measurands.Power_Export_Minimum;
                    return true;


                case "Voltage":
                    Measurand = Measurands.Voltage;
                    return true;

                case "Voltage.Minimum":
                    Measurand = Measurands.Voltage_Minimum;
                    return true;

                case "Voltage.Maximum":
                    Measurand = Measurands.Voltage_Maximum;
                    return true;


                case "Frequency":
                    Measurand = Measurands.Frequency;
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

                   Measurands.Current_Export                     => "Current.Export",
                   Measurands.Current_Import                     => "Current.Import",
                   Measurands.Current_Offered                    => "Current.Offered",

                   Measurands.Current_Import_Offered             => "Current.Import.Offered",
                   Measurands.Current_Import_Minimum             => "Current.Import.Minimum",
                   Measurands.Current_Export_Offered             => "Current.Export.Offered",
                   Measurands.Current_Export_Minimum             => "Current.Export.Minimum",
                   Measurands.Display_PresentSOC                 => "Display.PresentSOC",
                   Measurands.Display_MinimumSOC                 => "Display.MinimumSOC",
                   Measurands.Display_TargetSOC                  => "Display.TargetSOC",
                   Measurands.Display_MaximumSOC                 => "Display.MaximumSOC",
                   Measurands.Display_RemainingTimeToMinimumSOC  => "Display.RemainingTimeToMinimumSOC",
                   Measurands.Display_RemainingTimeToTargetSOC   => "Display.RemainingTimeToTargetSOC",
                   Measurands.Display_RemainingTimeToMaximumSOC  => "Display.RemainingTimeToMaximumSOC",
                   Measurands.Display_ChargingComplete           => "Display.ChargingComplete",
                   Measurands.Display_BatteryEnergyCapacity      => "Display.BatteryEnergyCapacity",
                   Measurands.Display_InletHot                   => "Display.InletHot",

                   Measurands.Energy_Active_Export_Register      => "Energy.Active.Export.Register",
                   Measurands.Energy_Active_Import_Register      => "Energy.Active.Import.Register",

                   Measurands.Energy_Reactive_Export_Register    => "Energy.Reactive.Export.Register",
                   Measurands.Energy_Reactive_Import_Register    => "Energy.Reactive.Import.Register",

                   Measurands.Energy_Active_Export_Interval      => "Energy.Active.Export.Interval",
                   Measurands.Energy_Active_Import_Interval      => "Energy.Active.Import.Interval",
                   Measurands.Energy_Active_Setpoint_Interval    => "Energy.Active.Setpoint.Interval",
                   Measurands.Energy_Active_Net                  => "Energy.Active.Net",

                   Measurands.Energy_Reactive_Export_Interval    => "Energy.Reactive.Export.Interval",
                   Measurands.Energy_Reactive_Import_Interval    => "Energy.Reactive.Import.Interval",
                   Measurands.Energy_Reactive_Net                => "Energy.Reactive.Net",

                   Measurands.Energy_Apparent_Export             => "Energy.Apparent.Export",
                   Measurands.Energy_Apparent_Import             => "Energy.Apparent.Import",
                   Measurands.Energy_Apparent_Net                => "Energy.Apparent.Net",

                   Measurands.EnergyRequest_Target               => "EnergyRequest.Target",
                   Measurands.EnergyRequest_Minimum              => "EnergyRequest.Minimum",
                   Measurands.EnergyRequest_Maximum              => "EnergyRequest.Maximum",
                   Measurands.EnergyRequest_Minimum_V2X          => "EnergyRequest.Minimum.V2X",
                   Measurands.EnergyRequest_Maximum_V2X          => "EnergyRequest.Maximum.V2X",
                   Measurands.EnergyRequest_Bulk                 => "EnergyRequest.Bulk",

                   Measurands.Power_Active_Export                => "Power.Active.Export",
                   Measurands.Power_Active_Import                => "Power.Active.Import",
                   Measurands.Power_Active_Setpoint              => "Power.Active.Setpoint",
                   Measurands.Power_Active_Residual              => "Power.Active.Residual",
                   Measurands.Power_Reactive_Export              => "Power.Reactive.Export",
                   Measurands.Power_Reactive_Import              => "Power.Reactive.Import",
                   Measurands.Power_Factor                       => "Power.Factor",
                   Measurands.Power_Offered                      => "Power.Offered",

                   Measurands.Power_Import_Offered               => "Power.Import.Offered",
                   Measurands.Power_Import_Minimum               => "Power.Import.Minimum",
                   Measurands.Power_Export_Offered               => "Power.Export.Offered",
                   Measurands.Power_Export_Minimum               => "Power.Export.Minimum",

                   Measurands.Voltage                            => "Voltage",
                   Measurands.Voltage_Minimum                    => "Voltage.Minimum",
                   Measurands.Voltage_Maximum                    => "Voltage.Maximum",

                   Measurands.Frequency                          => "Frequency",
                   Measurands.SoC                                => "SoC",

                   _                                             => "Unknown"

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

        /// <summary>
        /// Instantaneous current flow from EV.
        /// </summary>
        Current_Export,

        /// <summary>
        /// Instantaneous current flow to EV.
        /// </summary>
        Current_Import,

        /// <summary>
        /// Maximum current offered to EV.
        /// Has been replaced by Current.Import.Offered.
        /// </summary>
        [Obsolete("Has been replaced by Current.Import.Offered.")]
        Current_Offered,

        /// <summary>
        /// Maximum current offered to EV.
        /// </summary>
        Current_Import_Offered,

        /// <summary>
        /// Minimum current the EV can be charged with, max(EV, EVSE).
        /// </summary>
        Current_Import_Minimum,

        /// <summary>
        /// Maximum current the EV can be discharged with, min(EV, EVSE).
        /// </summary>
        Current_Export_Offered,

        /// <summary>
        /// Minimum current the EV can be discharged with, max(EV, EVSE).
        /// </summary>
        Current_Export_Minimum,

        /// <summary>
        /// Current state of charge of the EV battery.
        /// </summary>
        Display_PresentSOC,

        /// <summary>
        /// Minimum State of Charge EV needs after charging of the EV
        /// battery the EV to keep throughout the charging session.
        /// </summary>
        Display_MinimumSOC,

        /// <summary>
        /// Target State of Charge of the EV battery EV needs after charging.
        /// </summary>
        Display_TargetSOC,

        /// <summary>
        /// The SOC at which the EV will prohibit any further charging.
        /// </summary>
        Display_MaximumSOC,

        /// <summary>
        /// The remaining time it takes to reach the minimum SOC. It is
        /// communicated as the offset in seconds from the point in time this
        /// value was received from EV.
        /// </summary>
        Display_RemainingTimeToMinimumSOC,

        /// <summary>
        /// The remaining time it takes to reach the TargetSOC. It is#
        /// communicated as the offset in seconds from the point in time this
        /// value was received from EV.
        /// </summary>
        Display_RemainingTimeToTargetSOC,

        /// <summary>
        /// The remaining time it takes to reach the maximum SOC. It is
        /// communicated as the offset in seconds from the point in time
        /// this value was received from EV.
        /// </summary>
        Display_RemainingTimeToMaximumSOC,

        /// <summary>
        /// Indication if the charging is complete from EV point of view (value = 1).
        /// Display.BatteryEnergyCapacity The calculated amount of electrical Energy in Wh
        /// stored in the battery when the displayed SOC equals 100 %.
        /// </summary>
        Display_ChargingComplete,

        /// <summary>
        /// The calculated amount of electrical Energy in Wh stored in the
        /// battery when the displayed SOC equals 100 %.
        /// </summary>
        Display_BatteryEnergyCapacity,

        /// <summary>
        /// Inlet temperature too high to accept specific operating condition.
        /// </summary>
        Display_InletHot,

        /// <summary>
        /// Numerical value read from the "active electrical energy" (Wh or kWh) register
        /// of the (most authoritative) electrical meter measuring energy exported (to the grid).
        /// </summary>
        Energy_Active_Export_Register,

        /// <summary>
        /// Numerical value read from the "active electrical energy" (Wh or kWh) register
        /// of the (most authoritative) electrical meter measuring energy imported (from the grid supply)
        /// </summary>
        Energy_Active_Import_Register,

        /// <summary>
        /// Numerical value read from the "reactive electrical energy" (varh or kvarh) register
        /// of the (most authoritative) electrical meter measuring energy exported (to the grid).
        /// </summary>
        Energy_Reactive_Export_Register,

        /// <summary>
        /// Numerical value read from the "reactive electrical energy" (varh or kvarh) register
        /// of the (most authoritative) electrical meter measuring energy imported (from the grid supply).
        /// </summary>
        Energy_Reactive_Import_Register,

        /// <summary>
        /// Absolute amount of "active electrical energy" (Wh or kWh) exported (to the grid)
        /// during an associated time "interval", specified by a Metervalues ReadingContext,
        /// and applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        Energy_Active_Export_Interval,

        /// <summary>
        /// Absolute amount of "active electrical energy" (Wh or kWh) imported (from the grid supply)
        /// during an associated time "interval", specified by a Metervalues ReadingContext,
        /// and applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        Energy_Active_Import_Interval,

        /// <summary>
        /// Energy during interval when Setpoint would be followed exactly, as
        /// calculated by Charging Station. Relevant when Setpoint changes
        /// frequently during an interval as result of LocalLoadBalancing or
        /// LocalFrequencyControl. Can be negative if energy was exported.
        /// </summary>
        Energy_Active_Setpoint_Interval,

        /// <summary>
        /// Numerical value read from the “net active electrical energy" (Wh or kWh) register.
        /// </summary>
        Energy_Active_Net,

        /// <summary>
        /// Absolute amount of "reactive electrical energy" (varh or kvarh) exported (to the grid)
        /// during an associated time "interval", specified by a Metervalues ReadingContext, and
        /// applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        Energy_Reactive_Export_Interval,

        /// <summary>
        /// Absolute amount of "reactive electrical energy" (varh or kvarh) imported (from the grid supply)
        /// during an associated time "interval", specified by a Metervalues ReadingContext, and
        /// applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        Energy_Reactive_Import_Interval,

        /// <summary>
        /// Numerical value read from the “net reactive electrical energy" (varh or kvarh) register.
        /// </summary>
        Energy_Reactive_Net,

        /// <summary>
        /// Numerical value read from the "apparent electrical export energy" (VAh or kVAh) register.
        /// </summary>
        Energy_Apparent_Export,

        /// <summary>
        /// Numerical value read from the "apparent electrical import energy" (VAh or kVAh) register.
        /// </summary>
        Energy_Apparent_Import,

        /// <summary>
        /// Numerical value read from the "apparent electrical energy" (VAh or kVAh) register.
        /// </summary>
        Energy_Apparent_Net,

        /// <summary>
        /// Energy to requested state of charge.
        /// </summary>
        EnergyRequest_Target,

        /// <summary>
        /// Energy to minimum allowed state of charge.
        /// </summary>
        EnergyRequest_Minimum,

        /// <summary>
        /// Energy to maximum state of charge.
        /// </summary>
        EnergyRequest_Maximum,

        /// <summary>
        /// Energy to minimum state of charge for cycling (V2X) activity.
        /// Positive value means that current state of charge is below V2X range.
        /// </summary>
        EnergyRequest_Minimum_V2X,

        /// <summary>
        /// Energy to maximum state of charge for cycling (V2X) activity.
        /// Negative value indicates that current state of charge is above V2X range.
        /// </summary>
        EnergyRequest_Maximum_V2X,

        /// <summary>
        /// Energy to end of bulk charging.
        /// </summary>
        EnergyRequest_Bulk,

        /// <summary>
        /// Instantaneous active power exported by EV. (W or kW)
        /// </summary>
        Power_Active_Export,

        /// <summary>
        /// Instantaneous active power imported by EV. (W or kW).
        /// </summary>
        Power_Active_Import,

        /// <summary>
        /// Power setpoint for charging or discharging (negative for
        /// discharging), that should be followed as closely as possible.
        /// </summary>
        Power_Active_Setpoint,

        /// <summary>
        /// Difference between the given charging setpoint and the actual
        /// power measured. Can be negative.
        /// </summary>
        Power_Active_Residual,

        /// <summary>
        /// Instantaneous reactive power exported by EV. (var or kvar).
        /// </summary>
        Power_Reactive_Export,

        /// <summary>
        /// Instantaneous reactive power imported by EV. (var or kvar).
        /// </summary>
        Power_Reactive_Import,

        /// <summary>
        /// Instantaneous power factor of total energy flow.
        /// </summary>
        Power_Factor,

        /// <summary>
        /// Maximum power offered to EV.
        /// Has been replaced by Power.Import.Offered.
        /// </summary>
        [Obsolete("Has been replaced by Power.Import.Offered.")]
        Power_Offered,

        /// <summary>
        /// Maximum power the EV can be charged with, min(EV, EVSE).
        /// </summary>
        Power_Import_Offered,

        /// <summary>
        /// Minimum power the EV can be charged with, max(EV, EVSE).
        /// </summary>
        Power_Import_Minimum,

        /// <summary>
        /// Maximum power the EV can be discharged with, min(EV, EVSE).
        /// </summary>
        Power_Export_Offered,

        /// <summary>
        /// Minimum power the EV can be discharged with, max(EV, EVSE).
        /// </summary>
        Power_Export_Minimum,

        /// <summary>
        /// Instantaneous DC or AC RMS supply voltage.
        /// </summary>
        Voltage,

        /// <summary>
        /// Minimum voltage the EV can be charged or discharged with, max(EV, EVSE).
        /// </summary>
        Voltage_Minimum,

        /// <summary>
        /// Maximum voltage the EV can be charged or discharged with, max(EV, EVSE).
        /// </summary>
        Voltage_Maximum,

        /// <summary>
        /// Instantaneous reading of powerline frequency.
        /// </summary>
        Frequency,

        /// <summary>
        /// State-of-Charge of charging vehicle in percentage.
        /// </summary>
        SoC,

    }

}
