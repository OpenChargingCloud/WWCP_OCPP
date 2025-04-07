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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for measurands.
    /// </summary>
    public static class MeasurandExtensions
    {

        /// <summary>
        /// Indicates whether this measurand is null or empty.
        /// </summary>
        /// <param name="Measurand">A measurand.</param>
        public static Boolean IsNullOrEmpty(this Measurand? Measurand)
            => !Measurand.HasValue || Measurand.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this measurand is null or empty.
        /// </summary>
        /// <param name="Measurand">A measurand.</param>
        public static Boolean IsNotNullOrEmpty(this Measurand? Measurand)
            => Measurand.HasValue && Measurand.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A measurand.
    /// </summary>
    public readonly struct Measurand : IId,
                                       IEquatable<Measurand>,
                                       IComparable<Measurand>
    {

        #region Data

        private readonly static Dictionary<String, Measurand>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                         InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this measurand is null or empty.
        /// </summary>
        public readonly  Boolean                 IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this measurand is NOT null or empty.
        /// </summary>
        public readonly  Boolean                 IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the measurand.
        /// </summary>
        public readonly  UInt64                  Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered measurands.
        /// </summary>
        public static    IEnumerable<Measurand>  Values
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new measurand based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a measurand.</param>
        private Measurand(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static Measurand Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new Measurand(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurand Parse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            throw new ArgumentException($"Invalid text representation of a measurand: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurand? TryParse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Measurand)

        /// <summary>
        /// Try to parse the given text as measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        /// <param name="Measurand">The parsed measurand.</param>
        public static Boolean TryParse(String Text, out Measurand Measurand)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out Measurand))
                    Measurand = Register(Text);

                return true;

            }

            Measurand = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this measurand.
        /// </summary>
        public Measurand Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Instantaneous current flow from EV.
        /// </summary>
        public static Measurand  Current_Export                       { get; }
            = Register("Current.Export");

        /// <summary>
        /// Instantaneous current flow to EV.
        /// </summary>
        public static Measurand  Current_Import                       { get; }
            = Register("Current.Import");

        /// <summary>
        /// Maximum current offered to EV.
        /// Has been replaced by Current.Import.Offered.
        /// </summary>
        [Obsolete("Has been replaced by Current.Import.Offered.")]
        public static Measurand  Current_Offered                      { get; }
            = Register("Current.Offered");

        /// <summary>
        /// Maximum current offered to EV.
        /// </summary>
        public static Measurand  Current_Import_Offered               { get; }
            = Register("Current.Import.Offered");

        /// <summary>
        /// Minimum current the EV can be charged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Current_Import_Minimum               { get; }
            = Register("Current.Import.Minimum");

        /// <summary>
        /// Maximum current the EV can be discharged with, min(EV, EVSE).
        /// </summary>
        public static Measurand  Current_Export_Offered               { get; }
            = Register("Current.Export.Offered");

        /// <summary>
        /// Minimum current the EV can be discharged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Current_Export_Minimum               { get; }
            = Register("Current.Export.Minimum");

        /// <summary>
        /// Current state of charge of the EV battery.
        /// </summary>
        public static Measurand  Display_PresentSOC                   { get; }
            = Register("Display.PresentSOC");

        /// <summary>
        /// Minimum State of Charge EV needs after charging of the EV
        /// battery the EV to keep throughout the charging session.
        /// </summary>
        public static Measurand  Display_MinimumSOC                   { get; }
            = Register("Display.MinimumSOC");

        /// <summary>
        /// Target State of Charge of the EV battery EV needs after charging.
        /// </summary>
        public static Measurand  Display_TargetSOC                    { get; }
            = Register("Display.TargetSOC");

        /// <summary>
        /// The SOC at which the EV will prohibit any further charging.
        /// </summary>
        public static Measurand  Display_MaximumSOC                   { get; }
            = Register("Display.MaximumSOC");

        /// <summary>
        /// The remaining time it takes to reach the minimum SOC. It is
        /// communicated as the offset in seconds from the point in time this
        /// value was received from EV.
        /// </summary>
        public static Measurand  Display_RemainingTimeToMinimumSOC    { get; }
            = Register("Display.RemainingTimeToMinimumSOC");

        /// <summary>
        /// The remaining time it takes to reach the TargetSOC. It is#
        /// communicated as the offset in seconds from the point in time this
        /// value was received from EV.
        /// </summary>
        public static Measurand  Display_RemainingTimeToTargetSOC     { get; }
            = Register("Display.RemainingTimeToTargetSOC");

        /// <summary>
        /// The remaining time it takes to reach the maximum SOC. It is
        /// communicated as the offset in seconds from the point in time
        /// this value was received from EV.
        /// </summary>
        public static Measurand  Display_RemainingTimeToMaximumSOC    { get; }
            = Register("Display.RemainingTimeToMaximumSOC");

        /// <summary>
        /// Indication if the charging is complete from EV point of view (value = 1).
        /// Display.BatteryEnergyCapacity The calculated amount of electrical Energy in Wh
        /// stored in the battery when the displayed SOC equals 100 %.
        /// </summary>
        public static Measurand  Display_ChargingComplete             { get; }
            = Register("Display.ChargingComplete");

        /// <summary>
        /// The calculated amount of electrical Energy in Wh stored in the
        /// battery when the displayed SOC equals 100 %.
        /// </summary>
        public static Measurand Display_BatteryEnergyCapacity        { get; }
            = Register("Display.BatteryEnergyCapacity");

        /// <summary>
        /// Inlet temperature too high to accept specific operating condition.
        /// </summary>
        public static Measurand  Display_InletHot                     { get; }
            = Register("Display.InletHot");

        /// <summary>
        /// Calculated energy loss after energy meter.
        /// Will be reset to 0 at start of transaction.
        /// Unit is Wh.
        /// </summary>
        public static Measurand  Energy_Active_Export_CableLoss       { get; }
            = Register("Energy.Active.Export.CableLoss");

        /// <summary>
        /// Numerical value read from the "active electrical energy" (Wh or kWh) register
        /// of the (most authoritative) electrical meter measuring energy exported (to the grid).
        /// </summary>
        public static Measurand  Energy_Active_Export_Register        { get; }
            = Register("Energy.Active.Export.Register");

        /// <summary>
        /// Calculated energy loss after energy meter.
        /// Will be reset to 0 at start of transaction.
        /// Unit is Wh.
        /// </summary>
        public static Measurand  Energy_Active_Import_CableLoss       { get; }
            = Register("Energy.Active.Import.CableLoss");

        /// <summary>
        /// Numerical value read from the "active electrical energy" (Wh or kWh) register
        /// of the (most authoritative) electrical meter measuring energy imported (from the grid supply)
        /// </summary>
        public static Measurand  Energy_Active_Import_Register        { get; }
            = Register("Energy.Active.Import.Register");

        /// <summary>
        /// Numerical value read from the "reactive electrical energy" (varh or kvarh) register
        /// of the (most authoritative) electrical meter measuring energy exported (to the grid).
        /// </summary>
        public static Measurand  Energy_Reactive_Export_Register      { get; }
            = Register("Energy.Reactive.Export.Register");

        /// <summary>
        /// Numerical value read from the "reactive electrical energy" (varh or kvarh) register
        /// of the (most authoritative) electrical meter measuring energy imported (from the grid supply).
        /// </summary>
        public static Measurand  Energy_Reactive_Import_Register      { get; }
            = Register("Energy.Reactive.Import.Register");

        /// <summary>
        /// Absolute amount of "active electrical energy" (Wh or kWh) exported (to the grid)
        /// during an associated time "interval", specified by a Metervalues ReadingContext,
        /// and applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        public static Measurand  Energy_Active_Export_Interval        { get; }
            = Register("Energy.Active.Export.Interval");

        /// <summary>
        /// Absolute amount of "active electrical energy" (Wh or kWh) imported (from the grid supply)
        /// during an associated time "interval", specified by a Metervalues ReadingContext,
        /// and applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        public static Measurand  Energy_Active_Import_Interval        { get; }
            = Register("Energy.Active.Import.Interval");

        /// <summary>
        /// Energy during interval when Setpoint would be followed exactly, as
        /// calculated by Charging Station. Relevant when Setpoint changes
        /// frequently during an interval as result of LocalLoadBalancing or
        /// LocalFrequencyControl. Can be negative if energy was exported.
        /// </summary>
        public static Measurand  Energy_Active_Setpoint_Interval      { get; }
            = Register("Energy.Active.Setpoint.Interval");

        /// <summary>
        /// Numerical value read from the “net active electrical energy" (Wh or kWh) register.
        /// </summary>
        public static Measurand  Energy_Active_Net                    { get; }
            = Register("Energy.Active.Net");

        /// <summary>
        /// Absolute amount of "reactive electrical energy" (varh or kvarh) exported (to the grid)
        /// during an associated time "interval", specified by a Metervalues ReadingContext, and
        /// applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        public static Measurand  Energy_Reactive_Export_Interval      { get; }
            = Register("Energy.Reactive.Export.Interval");

        /// <summary>
        /// Absolute amount of "reactive electrical energy" (varh or kvarh) imported (from the grid supply)
        /// during an associated time "interval", specified by a Metervalues ReadingContext, and
        /// applicable interval duration configuration values (in seconds) for
        /// ClockAlignedDataInterval and TxnMeterValueSampleInterval.
        /// </summary>
        public static Measurand  Energy_Reactive_Import_Interval      { get; }
            = Register("Energy.Reactive.Import.Interval");

        /// <summary>
        /// Numerical value read from the “net reactive electrical energy" (varh or kvarh) register.
        /// </summary>
        public static Measurand  Energy_Reactive_Net                  { get; }
            = Register("Energy.Reactive.Net");

        /// <summary>
        /// Numerical value read from the "apparent electrical energy" (VAh or kVAh) register.
        /// </summary>
        public static Measurand  Energy_Apparent_Net                  { get; }
            = Register("Energy.Apparent.Net");

        /// <summary>
        /// Numerical value read from the "apparent electrical import energy" (VAh or kVAh) register.
        /// </summary>
        public static Measurand  Energy_Apparent_Import               { get; }
            = Register("Energy.Apparent.Import");

        /// <summary>
        /// Numerical value read from the "apparent electrical export energy" (VAh or kVAh) register.
        /// </summary>
        public static Measurand  Energy_Apparent_Export               { get; }
            = Register("Energy.Apparent.Export");

        /// <summary>
        /// Energy to requested state of charge.
        /// </summary>
        public static Measurand  EnergyRequest_Target                 { get; }
            = Register("EnergyRequest.Target");

        /// <summary>
        /// Energy to minimum allowed state of charge.
        /// </summary>
        public static Measurand  EnergyRequest_Minimum                { get; }
            = Register("EnergyRequest.Minimum");

        /// <summary>
        /// Energy to maximum state of charge.
        /// </summary>
        public static Measurand  EnergyRequest_Maximum                { get; }
            = Register("EnergyRequest.Maximum");

        /// <summary>
        /// Energy to minimum state of charge for cycling (V2X) activity.
        /// Positive value means that current state of charge is below V2X range.
        /// </summary>
        public static Measurand  EnergyRequest_Minimum_V2X            { get; }
            = Register("EnergyRequest.Minimum.V2X");

        /// <summary>
        /// Energy to maximum state of charge for cycling (V2X) activity.
        /// Negative value indicates that current state of charge is above V2X range.
        /// </summary>
        public static Measurand  EnergyRequest_Maximum_V2X            { get; }
            = Register("EnergyRequest.Maximum.V2X");

        /// <summary>
        /// Energy to end of bulk charging.
        /// </summary>
        public static Measurand  EnergyRequest_Bulk                   { get; }
            = Register("EnergyRequest.Bulk");

        /// <summary>
        /// Instantaneous active power exported by EV. (W or kW)
        /// </summary>
        public static Measurand  Power_Active_Export                  { get; }
            = Register("Power.Active.Export");

        /// <summary>
        /// Instantaneous active power imported by EV. (W or kW).
        /// </summary>
        public static Measurand  Power_Active_Import                  { get; }
            = Register("Power.Active.Import");

        /// <summary>
        /// Power setpoint for charging or discharging (negative for
        /// discharging), that should be followed as closely as possible.
        /// </summary>
        public static Measurand  Power_Active_Setpoint                { get; }
            = Register("Power.Active.Setpoint");

        /// <summary>
        /// Difference between the given charging setpoint and the actual
        /// power measured. Can be negative.
        /// </summary>
        public static Measurand  Power_Active_Residual                { get; }
            = Register("Power.Active.Residual");

        /// <summary>
        /// Instantaneous reactive power exported by EV. (var or kvar).
        /// </summary>
        public static Measurand  Power_Reactive_Export                { get; }
            = Register("Power.Reactive.Export");

        /// <summary>
        /// Instantaneous reactive power imported by EV. (var or kvar).
        /// </summary>
        public static Measurand  Power_Reactive_Import                { get; }
            = Register("Power.Reactive.Import");

        /// <summary>
        /// Instantaneous power factor of total energy flow.
        /// </summary>
        public static Measurand  Power_Factor                         { get; }
            = Register("Power.Factor");

        /// <summary>
        /// Maximum power offered to EV.
        /// Has been replaced by Power.Import.Offered.
        /// </summary>
        [Obsolete("Has been replaced by Power.Import.Offered.")]
        public static Measurand  Power_Offered                        { get; }
            = Register("Power.Offered");

        /// <summary>
        /// Maximum power the EV can be charged with, min(EV, EVSE).
        /// </summary>
        public static Measurand  Power_Import_Offered                 { get; }
            = Register("Power.Import.Offered");

        /// <summary>
        /// Minimum power the EV can be charged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Power_Import_Minimum                 { get; }
            = Register("Power.Import.Minimum");

        /// <summary>
        /// Maximum power the EV can be discharged with, min(EV, EVSE).
        /// </summary>
        public static Measurand  Power_Export_Offered                 { get; }
            = Register("Power.Export.Offered");

        /// <summary>
        /// Minimum power the EV can be discharged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Power_Export_Minimum                 { get; }
            = Register("Power.Export.Minimum");

        /// <summary>
        /// Instantaneous DC or AC RMS supply voltage.
        /// </summary>
        public static Measurand  Voltage                              { get; }
            = Register("Voltage");

        /// <summary>
        /// Minimum voltage the EV can be charged or discharged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Voltage_Minimum                      { get; }
            = Register("Voltage.Minimum");

        /// <summary>
        /// Maximum voltage the EV can be charged or discharged with, max(EV, EVSE).
        /// </summary>
        public static Measurand  Voltage_Maximum                      { get; }
            = Register("Voltage.Maximum");

        /// <summary>
        /// Instantaneous reading of powerline frequency.
        /// </summary>
        public static Measurand  Frequency                            { get; }
            = Register("Frequency");

        /// <summary>
        /// State-of-Charge of charging vehicle in percentage.
        /// </summary>
        public static Measurand  SoC                                  { get; }
            = Register("SoC");

        #endregion


        #region Operator overloading

        #region Operator == (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.Equals(Measurand2);

        #endregion

        #region Operator != (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Measurand Measurand1,
                                           Measurand Measurand2)

            => !Measurand1.Equals(Measurand2);

        #endregion

        #region Operator <  (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Measurand Measurand1,
                                          Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) < 0;

        #endregion

        #region Operator <= (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) <= 0;

        #endregion

        #region Operator >  (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Measurand Measurand1,
                                          Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) > 0;

        #endregion

        #region Operator >= (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) >= 0;

        #endregion

        #endregion

        #region IComparable<Measurand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two measurands.
        /// </summary>
        /// <param name="Object">A measurand to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Measurand measurand
                   ? CompareTo(measurand)
                   : throw new ArgumentException("The given object is not measurand!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Measurand)

        /// <summary>
        /// Compares two measurands.
        /// </summary>
        /// <param name="Measurand">A measurand to compare with.</param>
        public Int32 CompareTo(Measurand Measurand)

            => String.Compare(InternalId,
                              Measurand.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<Measurand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two measurands for equality.
        /// </summary>
        /// <param name="Object">A measurand to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Measurand measurand &&
                   Equals(measurand);

        #endregion

        #region Equals(Measurand)

        /// <summary>
        /// Compares two measurands for equality.
        /// </summary>
        /// <param name="Measurand">A measurand to compare with.</param>
        public Boolean Equals(Measurand Measurand)

            => String.Equals(InternalId,
                             Measurand.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
