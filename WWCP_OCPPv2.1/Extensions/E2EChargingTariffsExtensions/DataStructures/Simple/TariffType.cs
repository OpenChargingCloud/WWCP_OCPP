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

namespace cloud.charging.open.protocols.OCPPv2_1
{


    /// <summary>
    /// Extension methods for tariff types.
    /// </summary>
    public static class TariffTypeExtensions
    {

        /// <summary>
        /// Indicates whether this tariff type is null or empty.
        /// </summary>
        /// <param name="TariffType">A tariff type.</param>
        public static Boolean IsNullOrEmpty(this TariffType? TariffType)
            => !TariffType.HasValue || TariffType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff type is NOT null or empty.
        /// </summary>
        /// <param name="TariffType">A tariff type.</param>
        public static Boolean IsNotNullOrEmpty(this TariffType? TariffType)
            => TariffType.HasValue && TariffType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tariff type.
    /// </summary>
    public readonly struct TariffType : IId<TariffType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff type.
        /// </summary>
        public UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff type.</param>
        private TariffType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff type.
        /// </summary>
        /// <param name="Text">A text representation of a tariff type.</param>
        public static TariffType Parse(String Text)
        {

            if (TryParse(Text, out var tariffDimension))
                return tariffDimension;

            throw new ArgumentException($"Invalid text representation of a tariff type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff type.
        /// </summary>
        /// <param name="Text">A text representation of a tariff type.</param>
        public static TariffType? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffDimension))
                return tariffDimension;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffType)

        /// <summary>
        /// Try to parse the given text as a tariff type.
        /// </summary>
        /// <param name="Text">A text representation of a tariff type.</param>
        /// <param name="TariffType">The parsed tariff type.</param>
        public static Boolean TryParse(String Text, out TariffType TariffType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffType = new TariffType(Text);
                    return true;
                }
                catch
                { }
            }

            TariffType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this tariff type.
        /// </summary>
        public TariffType Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Used to describe that a tariff is valid when ad-hoc payment is used at the charging station
        /// (for example: Debit or credit card payment terminal).
        /// </summary>
        public static TariffType AD_HOC_PAYMENT
            => new ("AD_HOC_PAYMENT");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// CHEAP is set for the charging session.
        /// </summary>
        public static TariffType PROFILE_CHEAP
            => new ("PROFILE_CHEAP");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// FAST is set for the charging session.
        /// </summary>
        public static TariffType PROFILE_FAST
            => new ("PROFILE_FAST");

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// GREEN is set for the charging session.
        /// </summary>
        public static TariffType PROFILE_GREEN
            => new("PROFILE_GREEN");

        /// <summary>
        /// Used to describe that a tariff is valid when using an RFID, without
        /// any charging preference, or when Charging Preference:
        /// REGULAR is set for the charging session.
        /// </summary>
        public static TariffType REGULAR
            => new ("REGULAR");

        #endregion


        #region Operator overloading

        #region Operator == (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(TariffType TariffType1,
                                           TariffType TariffType2)

            => TariffType1.Equals(TariffType2);

        #endregion

        #region Operator != (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(TariffType TariffType1,
                                           TariffType TariffType2)

            => !TariffType1.Equals(TariffType2);

        #endregion

        #region Operator <  (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(TariffType TariffType1,
                                          TariffType TariffType2)

            => TariffType1.CompareTo(TariffType2) < 0;

        #endregion

        #region Operator <= (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(TariffType TariffType1,
                                           TariffType TariffType2)

            => TariffType1.CompareTo(TariffType2) <= 0;

        #endregion

        #region Operator >  (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(TariffType TariffType1,
                                          TariffType TariffType2)

            => TariffType1.CompareTo(TariffType2) > 0;

        #endregion

        #region Operator >= (TariffType1, TariffType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffType1">A tariff type.</param>
        /// <param name="TariffType2">Another tariff type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(TariffType TariffType1,
                                           TariffType TariffType2)

            => TariffType1.CompareTo(TariffType2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff types.
        /// </summary>
        /// <param name="Object">A tariff type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffType tariffDimension
                   ? CompareTo(tariffDimension)
                   : throw new ArgumentException("The given object is not a tariff type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffType)

        /// <summary>
        /// Compares two tariff types.
        /// </summary>
        /// <param name="TariffType">A tariff type to compare with.</param>
        public Int32 CompareTo(TariffType TariffType)

            => String.Compare(InternalId,
                              TariffType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff types for equality.
        /// </summary>
        /// <param name="Object">A tariff type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffType tariffDimension &&
                   Equals(tariffDimension);

        #endregion

        #region Equals(TariffType)

        /// <summary>
        /// Compares two tariff types for equality.
        /// </summary>
        /// <param name="TariffType">A tariff type to compare with.</param>
        public Boolean Equals(TariffType TariffType)

            => String.Equals(InternalId,
                             TariffType.InternalId,
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
