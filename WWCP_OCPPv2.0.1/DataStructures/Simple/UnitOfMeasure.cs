/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
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

        #region Properties

        /// <summary>
        /// The unit of measure.
        /// </summary>
        public String   Text    { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => Text.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => Text.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the unit of measure.
        /// </summary>
        public UInt64   Length
            => (UInt64) (Text?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit of measure.
        /// </summary>
        /// <param name="Text">The string representation of the unit of measure.</param>
        private UnitOfMeasure(String Text)
        {
            this.Text = Text;
        }

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
                UnitOfMeasure = new UnitOfMeasure(Text);
                return true;
            }

            UnitOfMeasure = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this unit of measure.
        /// </summary>
        public UnitOfMeasure Clone

            => new (
                   new String(Text?.ToCharArray())
               );

        #endregion


        #region Statics

        /// <summary>
        /// Celsius
        /// </summary>
        public readonly static UnitOfMeasure Celsius
            = new ("Celsius");

        /// <summary>
        /// Fahrenheit
        /// </summary>
        public readonly static UnitOfMeasure Fahrenheit
            = new ("Fahrenheit");

        /// <summary>
        /// Wh
        /// </summary>
        public readonly static UnitOfMeasure Wh
            = new ("Wh");

        /// <summary>
        /// kWh
        /// </summary>
        public readonly static UnitOfMeasure kWh
            = new ("kWh");

        /// <summary>
        /// varh
        /// </summary>
        public readonly static UnitOfMeasure varh
            = new ("varh");

        /// <summary>
        /// kvarh
        /// </summary>
        public readonly static UnitOfMeasure kvarh
            = new ("kvarh");

        /// <summary>
        /// Watts
        /// </summary>
        public readonly static UnitOfMeasure Watts
            = new ("Watts");

        /// <summary>
        /// kW
        /// </summary>
        public readonly static UnitOfMeasure kW
            = new ("kW");

        /// <summary>
        /// Watchdog.
        /// </summary>
        public readonly static UnitOfMeasure Watchdog
            = new ("Watchdog");

        /// <summary>
        /// VoltAmpere
        /// </summary>
        public readonly static UnitOfMeasure VoltAmpere
            = new ("VoltAmpere");

        /// <summary>
        /// kVA
        /// </summary>
        public readonly static UnitOfMeasure kVA
            = new ("kVA");

        /// <summary>
        /// var
        /// </summary>
        public readonly static UnitOfMeasure var
            = new ("var");

        /// <summary>
        /// kvar
        /// </summary>
        public readonly static UnitOfMeasure kvar
            = new ("kvar");

        /// <summary>
        /// Amperes
        /// </summary>
        public readonly static UnitOfMeasure Amperes
            = new ("Amperes");

        /// <summary>
        /// Voltage
        /// </summary>
        public readonly static UnitOfMeasure Voltage
            = new ("Voltage");

        /// <summary>
        /// Kelvin
        /// </summary>
        public readonly static UnitOfMeasure Kelvin
            = new ("Kelvin");

        /// <summary>
        /// Percent
        /// </summary>
        public readonly static UnitOfMeasure Percent
            = new ("Percent");

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

            => String.Compare(Text,
                              UnitOfMeasure.Text,
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

            => String.Equals(Text,
                             UnitOfMeasure.Text,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Text?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Text ?? "";

        #endregion

    }

}
