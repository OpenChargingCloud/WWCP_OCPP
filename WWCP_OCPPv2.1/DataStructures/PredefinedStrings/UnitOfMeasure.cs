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
    /// Extension methods for units of measure.
    /// </summary>
    public static class UnitOfMeasureExtensions
    {

        /// <summary>
        /// Indicates whether this unit of measure is null or empty.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure.</param>
        public static Boolean IsNullOrEmpty(this UnitOfMeasure? UnitOfMeasure)
            => !UnitOfMeasure.HasValue || UnitOfMeasure.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this unit of measure is null or empty.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure.</param>
        public static Boolean IsNotNullOrEmpty(this UnitOfMeasure? UnitOfMeasure)
            => UnitOfMeasure.HasValue && UnitOfMeasure.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A unit of measure.
    /// </summary>
    public readonly struct UnitOfMeasure : IId,
                                           IEquatable<UnitOfMeasure>,
                                           IComparable<UnitOfMeasure>
    {

        #region Data

        private readonly static Dictionary<String, UnitOfMeasure>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this unit of measure is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this unit of measure is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the unit of measure.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit of measure.
        /// </summary>
        /// <param name="Text">The text representation of an unit of measure.</param>
        private UnitOfMeasure(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static UnitOfMeasure Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new UnitOfMeasure(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a unit of measure.
        /// </summary>
        /// <param name="Text">A text representation of a unit of measure.</param>
        public static UnitOfMeasure Parse(String Text)
        {

            if (TryParse(Text, out var unitOfMeasure))
                return unitOfMeasure;

            throw new ArgumentException($"Invalid text representation of a unit of measure: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a unit of measure.
        /// </summary>
        /// <param name="Text">A text representation of a unit of measure.</param>
        public static UnitOfMeasure? TryParse(String Text)
        {

            if (TryParse(Text, out var unitOfMeasure))
                return unitOfMeasure;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out UnitOfMeasure)

        /// <summary>
        /// Try to parse the given text as a unit of measure.
        /// </summary>
        /// <param name="Text">A text representation of a unit of measure.</param>
        /// <param name="UnitOfMeasure">The parsed unit of measure.</param>
        public static Boolean TryParse(String Text, out UnitOfMeasure UnitOfMeasure)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                // What WWCP_OCPP wrote before it wrote the texts of the
                // specification, still read as the units it meant.
                Text = Text switch {
                           "Amperes"     => "A",
                           "Voltage"     => "V",
                           "Watts"       => "W",
                           "VoltAmpere"  => "VA",
                           "Kelvin"      => "K",
                           "TimeSpan"    => "s",
                           _             => Text
                       };

                if (!lookup.TryGetValue(Text, out UnitOfMeasure))
                    UnitOfMeasure = Register(Text);

                return true;

            }

            UnitOfMeasure = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this unit of measure.
        /// </summary>
        public UnitOfMeasure Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// A: Amperes (current).
        /// </summary>
        public static UnitOfMeasure Amperes                { get; }
            = Register("A");

        /// <summary>
        /// ASU: Arbitrary Strength Unit (signal strength).
        /// </summary>
        public static UnitOfMeasure ASU                    { get; }
            = Register("ASU");

        /// <summary>
        /// B: Bytes.
        /// </summary>
        public static UnitOfMeasure Bytes                  { get; }
            = Register("B");

        /// <summary>
        /// Celsius: Degrees (temperature).
        /// </summary>
        public static UnitOfMeasure Celsius                { get; }
            = Register("Celsius");

        /// <summary>
        /// dB: Decibel (for example signal strength).
        /// </summary>
        public static UnitOfMeasure dB                     { get; }
            = Register("dB");

        /// <summary>
        /// dBm: Power relative to 1 mW (10 log(P/1 mW)).
        /// </summary>
        public static UnitOfMeasure dBm                    { get; }
            = Register("dBm");

        /// <summary>
        /// Deg: Degrees (angle/rotation).
        /// </summary>
        public static UnitOfMeasure Degrees                { get; }
            = Register("Deg");

        /// <summary>
        /// Fahrenheit: Degrees (temperature).
        /// </summary>
        public static UnitOfMeasure Fahrenheit             { get; }
            = Register("Fahrenheit");

        /// <summary>
        /// Hz: Hertz (frequency).
        /// </summary>
        public static UnitOfMeasure Hertz                  { get; }
            = Register("Hz");

        /// <summary>
        /// mHz: Millihertz (frequency).
        /// </summary>
        public static UnitOfMeasure MilliHertz             { get; }
            = Register("mHz");

        /// <summary>
        /// K: Degrees Kelvin (temperature).
        /// </summary>
        public static UnitOfMeasure Kelvin                 { get; }
            = Register("K");

        /// <summary>
        /// lx: Lux (light intensity).
        /// </summary>
        public static UnitOfMeasure Lux                    { get; }
            = Register("lx");

        /// <summary>
        /// m: Meter (length).
        /// </summary>
        public static UnitOfMeasure Meter                  { get; }
            = Register("m");

        /// <summary>
        /// ms2: m/s² (acceleration).
        /// </summary>
        public static UnitOfMeasure MeterPerSecondSquared  { get; }
            = Register("ms2");

        /// <summary>
        /// N: Newtons (force).
        /// </summary>
        public static UnitOfMeasure Newton                 { get; }
            = Register("N");

        /// <summary>
        /// Ohm: Ohm (impedance).
        /// </summary>
        public static UnitOfMeasure Ohm                    { get; }
            = Register("Ohm");

        /// <summary>
        /// kPa: Kilopascal (pressure).
        /// </summary>
        public static UnitOfMeasure kPa                    { get; }
            = Register("kPa");

        /// <summary>
        /// Percent: Percentage.
        /// </summary>
        public static UnitOfMeasure Percent                { get; }
            = Register("Percent");

        /// <summary>
        /// RH: Relative humidity %.
        /// </summary>
        public static UnitOfMeasure RelativeHumidity       { get; }
            = Register("RH");

        /// <summary>
        /// RPM: Revolutions per minute.
        /// </summary>
        public static UnitOfMeasure RPM                    { get; }
            = Register("RPM");

        /// <summary>
        /// s: Seconds (time).
        /// </summary>
        public static UnitOfMeasure TimeSpan               { get; }
            = Register("s");

        /// <summary>
        /// V: Voltage (DC or r.m.s. AC).
        /// </summary>
        public static UnitOfMeasure Voltage                { get; }
            = Register("V");

        /// <summary>
        /// VA: Volt-ampere (apparent power).
        /// </summary>
        public static UnitOfMeasure VoltAmpere             { get; }
            = Register("VA");

        /// <summary>
        /// kVA: Kilovolt-ampere (apparent power).
        /// </summary>
        public static UnitOfMeasure kVA                    { get; }
            = Register("kVA");

        /// <summary>
        /// VAh: Volt-ampere-hours (apparent energy).
        /// </summary>
        public static UnitOfMeasure VAh                    { get; }
            = Register("VAh");

        /// <summary>
        /// kVAh: Kilovolt-ampere-hours (apparent energy).
        /// </summary>
        public static UnitOfMeasure kVAh                   { get; }
            = Register("kVAh");

        /// <summary>
        /// var: Vars (reactive power).
        /// </summary>
        public static UnitOfMeasure var                    { get; }
            = Register("var");

        /// <summary>
        /// kvar: Kilovars (reactive power).
        /// </summary>
        public static UnitOfMeasure kvar                   { get; }
            = Register("kvar");

        /// <summary>
        /// varh: Var-hours (reactive energy).
        /// </summary>
        public static UnitOfMeasure varh                   { get; }
            = Register("varh");

        /// <summary>
        /// kvarh: Kilovar-hours (reactive energy).
        /// </summary>
        public static UnitOfMeasure kvarh                  { get; }
            = Register("kvarh");

        /// <summary>
        /// W: Watts (power).
        /// </summary>
        public static UnitOfMeasure Watts                  { get; }
            = Register("W");

        /// <summary>
        /// kW: Kilowatts (power).
        /// </summary>
        public static UnitOfMeasure kW                     { get; }
            = Register("kW");

        /// <summary>
        /// Wh: Watt-hours (energy) - the default.
        /// </summary>
        public static UnitOfMeasure Wh                     { get; }
            = Register("Wh");

        /// <summary>
        /// kWh: Kilowatt-hours (energy).
        /// </summary>
        public static UnitOfMeasure kWh                    { get; }
            = Register("kWh");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnitOfMeasure UnitOfMeasure1,
                                           UnitOfMeasure UnitOfMeasure2)

            => UnitOfMeasure1.Equals(UnitOfMeasure2);

        #endregion

        #region Operator != (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnitOfMeasure UnitOfMeasure1,
                                           UnitOfMeasure UnitOfMeasure2)

            => !UnitOfMeasure1.Equals(UnitOfMeasure2);

        #endregion

        #region Operator <  (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UnitOfMeasure UnitOfMeasure1,
                                          UnitOfMeasure UnitOfMeasure2)

            => UnitOfMeasure1.CompareTo(UnitOfMeasure2) < 0;

        #endregion

        #region Operator <= (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UnitOfMeasure UnitOfMeasure1,
                                           UnitOfMeasure UnitOfMeasure2)

            => UnitOfMeasure1.CompareTo(UnitOfMeasure2) <= 0;

        #endregion

        #region Operator >  (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UnitOfMeasure UnitOfMeasure1,
                                          UnitOfMeasure UnitOfMeasure2)

            => UnitOfMeasure1.CompareTo(UnitOfMeasure2) > 0;

        #endregion

        #region Operator >= (UnitOfMeasure1, UnitOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitOfMeasure1">A unit of measure.</param>
        /// <param name="UnitOfMeasure2">Another unit of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UnitOfMeasure UnitOfMeasure1,
                                           UnitOfMeasure UnitOfMeasure2)

            => UnitOfMeasure1.CompareTo(UnitOfMeasure2) >= 0;

        #endregion

        #endregion

        #region IComparable<UnitOfMeasure> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two units of measure.
        /// </summary>
        /// <param name="Object">A unit of measure to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is UnitOfMeasure unitOfMeasure
                   ? CompareTo(unitOfMeasure)
                   : throw new ArgumentException("The given object is not a unit of measure!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(UnitOfMeasure)

        /// <summary>
        /// Compares two units of measure.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure to compare with.</param>
        public Int32 CompareTo(UnitOfMeasure UnitOfMeasure)

            => String.Compare(InternalId,
                              UnitOfMeasure.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<UnitOfMeasure> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="Object">A unit of measure to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnitOfMeasure unitOfMeasure &&
                   Equals(unitOfMeasure);

        #endregion

        #region Equals(UnitOfMeasure)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure to compare with.</param>
        public Boolean Equals(UnitOfMeasure UnitOfMeasure)

            => String.Equals(InternalId,
                             UnitOfMeasure.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
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
