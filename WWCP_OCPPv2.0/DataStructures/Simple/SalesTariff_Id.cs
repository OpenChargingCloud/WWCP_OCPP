/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for sales tariff identifications.
    /// </summary>
    public static class SalesTariffIdExtensions
    {

        /// <summary>
        /// Indicates whether this sales tariff identification is null or empty.
        /// </summary>
        /// <param name="SalesTariffId">A sales tariff identification.</param>
        public static Boolean IsNullOrEmpty(this SalesTariff_Id? SalesTariffId)
            => !SalesTariffId.HasValue || SalesTariffId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this sales tariff identification is null or empty.
        /// </summary>
        /// <param name="SalesTariffId">A sales tariff identification.</param>
        public static Boolean IsNotNullOrEmpty(this SalesTariff_Id? SalesTariffId)
            => SalesTariffId.HasValue && SalesTariffId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A sales tariff identification.
    /// </summary>
    public readonly struct SalesTariff_Id : IId,
                                                IEquatable<SalesTariff_Id>,
                                                IComparable<SalesTariff_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the transaction identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => Value == 0;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => Value != 0;

        /// <summary>
        /// The length of the sales tariff identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sales tariff identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a sales tariff identification.</param>
        private SalesTariff_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random sales tariff identification.
        /// </summary>
        public static SalesTariff_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a sales tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a sales tariff identification.</param>
        public static SalesTariff_Id Parse(String Text)
        {

            if (TryParse(Text, out var salesTariffId))
                return salesTariffId;

            throw new ArgumentException("Invalid text representation of a sales tariff identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a sales tariff identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a sales tariff identification.</param>
        public static SalesTariff_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a sales tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a sales tariff identification.</param>
        public static SalesTariff_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var salesTariffId))
                return salesTariffId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a sales tariff identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a sales tariff identification.</param>
        public static SalesTariff_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var salesTariffId))
                return salesTariffId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out SalesTariffId)

        /// <summary>
        /// Try to parse the given text as a sales tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a sales tariff identification.</param>
        /// <param name="SalesTariffId">The parsed sales tariff identification.</param>
        public static Boolean TryParse(String Text, out SalesTariff_Id SalesTariffId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                SalesTariffId = new SalesTariff_Id(number);
                return true;
            }

            SalesTariffId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out SalesTariffId)

        /// <summary>
        /// Try to parse the given number as a sales tariff identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a sales tariff identification.</param>
        /// <param name="SalesTariffId">The parsed sales tariff identification.</param>
        public static Boolean TryParse(UInt64 Number, out SalesTariff_Id SalesTariffId)
        {

            SalesTariffId = new SalesTariff_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this sales tariff identification.
        /// </summary>
        public SalesTariff_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SalesTariff_Id SalesTariffId1,
                                           SalesTariff_Id SalesTariffId2)

            => SalesTariffId1.Equals(SalesTariffId2);

        #endregion

        #region Operator != (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SalesTariff_Id SalesTariffId1,
                                           SalesTariff_Id SalesTariffId2)

            => !SalesTariffId1.Equals(SalesTariffId2);

        #endregion

        #region Operator <  (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SalesTariff_Id SalesTariffId1,
                                          SalesTariff_Id SalesTariffId2)

            => SalesTariffId1.CompareTo(SalesTariffId2) < 0;

        #endregion

        #region Operator <= (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SalesTariff_Id SalesTariffId1,
                                           SalesTariff_Id SalesTariffId2)

            => SalesTariffId1.CompareTo(SalesTariffId2) <= 0;

        #endregion

        #region Operator >  (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SalesTariff_Id SalesTariffId1,
                                          SalesTariff_Id SalesTariffId2)

            => SalesTariffId1.CompareTo(SalesTariffId2) > 0;

        #endregion

        #region Operator >= (SalesTariffId1, SalesTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffId1">A sales tariff identification.</param>
        /// <param name="SalesTariffId2">Another sales tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SalesTariff_Id SalesTariffId1,
                                           SalesTariff_Id SalesTariffId2)

            => SalesTariffId1.CompareTo(SalesTariffId2) >= 0;

        #endregion

        #endregion

        #region IComparable<SalesTariffId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two sales tariff identifications.
        /// </summary>
        /// <param name="Object">A sales tariff identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SalesTariff_Id salesTariffId
                   ? CompareTo(salesTariffId)
                   : throw new ArgumentException("The given object is not a sales tariff identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SalesTariffId)

        /// <summary>
        /// Compares two sales tariff identifications.
        /// </summary>
        /// <param name="SalesTariffId">A sales tariff identification to compare with.</param>
        public Int32 CompareTo(SalesTariff_Id SalesTariffId)

            => Value.CompareTo(SalesTariffId.Value);

        #endregion

        #endregion

        #region IEquatable<SalesTariffId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sales tariff identifications for equality.
        /// </summary>
        /// <param name="Object">A sales tariff identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SalesTariff_Id salesTariffId &&
                   Equals(salesTariffId);

        #endregion

        #region Equals(SalesTariffId)

        /// <summary>
        /// Compares two sales tariff identifications for equality.
        /// </summary>
        /// <param name="SalesTariffId">A sales tariff identification to compare with.</param>
        public Boolean Equals(SalesTariff_Id SalesTariffId)

            => Value.Equals(SalesTariffId.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
