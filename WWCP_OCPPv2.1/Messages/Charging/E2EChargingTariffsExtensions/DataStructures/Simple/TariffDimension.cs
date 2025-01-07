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
    /// Extension methods for tariff dimensions.
    /// </summary>
    public static class TariffDimensionExtensions
    {

        /// <summary>
        /// Indicates whether this tariff dimension is null or empty.
        /// </summary>
        /// <param name="TariffDimension">A tariff dimension.</param>
        public static Boolean IsNullOrEmpty(this TariffDimension? TariffDimension)
            => !TariffDimension.HasValue || TariffDimension.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff dimension is NOT null or empty.
        /// </summary>
        /// <param name="TariffDimension">A tariff dimension.</param>
        public static Boolean IsNotNullOrEmpty(this TariffDimension? TariffDimension)
            => TariffDimension.HasValue && TariffDimension.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tariff dimension.
    /// </summary>
    public readonly struct TariffDimension : IId<TariffDimension>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff dimension is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff dimension is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff dimension.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff dimension based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff dimension.</param>
        private TariffDimension(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff dimension.
        /// </summary>
        /// <param name="Text">A text representation of a tariff dimension.</param>
        public static TariffDimension Parse(String Text)
        {

            if (TryParse(Text, out var tariffDimension))
                return tariffDimension;

            throw new ArgumentException($"Invalid text representation of a tariff dimension: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff dimension.
        /// </summary>
        /// <param name="Text">A text representation of a tariff dimension.</param>
        public static TariffDimension? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffDimension))
                return tariffDimension;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffDimension)

        /// <summary>
        /// Try to parse the given text as a tariff dimension.
        /// </summary>
        /// <param name="Text">A text representation of a tariff dimension.</param>
        /// <param name="TariffDimension">The parsed tariff dimension.</param>
        public static Boolean TryParse(String Text, out TariffDimension TariffDimension)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffDimension = new TariffDimension(Text);
                    return true;
                }
                catch
                { }
            }

            TariffDimension = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this tariff dimension.
        /// </summary>
        public TariffDimension Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Defined in kWh, step_size multiplier: 1 Wh.
        /// </summary>
        public static TariffDimension ENERGY
            => new ("ENERGY");

        /// <summary>
        /// Flat fee without unit for step_size.
        /// </summary>
        public static TariffDimension FLAT
            => new ("FLAT");

        /// <summary>
        /// Reservation time (before) charging: defined in hours, step_size multiplier: 1 second.
        /// </summary>
        public static TariffDimension RESERVATION_HOURS
            => new ("RESERVATION_HOURS");

        /// <summary>
        /// Time charging: defined in hours, step_size multiplier: 1 second.
        /// Can also be used in combination with a RESERVATION restriction to describe
        /// the price of the reservation time.
        /// </summary>
        public static TariffDimension CHARGE_HOURS
            => new ("CHARGE_HOURS");

        /// <summary>
        /// Time not charging: defined in hours, step_size multiplier: 1 second.
        /// </summary>
        public static TariffDimension IDLE_HOURS
            => new ("IDLE_HOURS");

        #endregion


        #region Operator overloading

        #region Operator == (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(TariffDimension TariffDimension1,
                                           TariffDimension TariffDimension2)

            => TariffDimension1.Equals(TariffDimension2);

        #endregion

        #region Operator != (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(TariffDimension TariffDimension1,
                                           TariffDimension TariffDimension2)

            => !TariffDimension1.Equals(TariffDimension2);

        #endregion

        #region Operator <  (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(TariffDimension TariffDimension1,
                                          TariffDimension TariffDimension2)

            => TariffDimension1.CompareTo(TariffDimension2) < 0;

        #endregion

        #region Operator <= (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(TariffDimension TariffDimension1,
                                           TariffDimension TariffDimension2)

            => TariffDimension1.CompareTo(TariffDimension2) <= 0;

        #endregion

        #region Operator >  (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(TariffDimension TariffDimension1,
                                          TariffDimension TariffDimension2)

            => TariffDimension1.CompareTo(TariffDimension2) > 0;

        #endregion

        #region Operator >= (TariffDimension1, TariffDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffDimension1">A tariff dimension.</param>
        /// <param name="TariffDimension2">Another tariff dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(TariffDimension TariffDimension1,
                                           TariffDimension TariffDimension2)

            => TariffDimension1.CompareTo(TariffDimension2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffDimension> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff dimensions.
        /// </summary>
        /// <param name="Object">A tariff dimension to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffDimension tariffDimension
                   ? CompareTo(tariffDimension)
                   : throw new ArgumentException("The given object is not a tariff dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffDimension)

        /// <summary>
        /// Compares two tariff dimensions.
        /// </summary>
        /// <param name="TariffDimension">A tariff dimension to compare with.</param>
        public Int32 CompareTo(TariffDimension TariffDimension)

            => String.Compare(InternalId,
                              TariffDimension.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff dimensions for equality.
        /// </summary>
        /// <param name="Object">A tariff dimension to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffDimension tariffDimension &&
                   Equals(tariffDimension);

        #endregion

        #region Equals(TariffDimension)

        /// <summary>
        /// Compares two tariff dimensions for equality.
        /// </summary>
        /// <param name="TariffDimension">A tariff dimension to compare with.</param>
        public Boolean Equals(TariffDimension TariffDimension)

            => String.Equals(InternalId,
                             TariffDimension.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
