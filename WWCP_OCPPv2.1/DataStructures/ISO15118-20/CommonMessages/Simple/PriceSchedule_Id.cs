/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// Extension methods for price schedule identifications.
    /// </summary>
    public static class PriceScheduleIdExtensions
    {

        /// <summary>
        /// Indicates whether this price schedule identification is null or empty.
        /// </summary>
        /// <param name="PriceScheduleId">A price schedule identification.</param>
        public static Boolean IsNullOrEmpty(this PriceSchedule_Id? PriceScheduleId)
            => !PriceScheduleId.HasValue || PriceScheduleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this price schedule identification is null or empty.
        /// </summary>
        /// <param name="PriceScheduleId">A price schedule identification.</param>
        public static Boolean IsNotNullOrEmpty(this PriceSchedule_Id? PriceScheduleId)
            => PriceScheduleId.HasValue && PriceScheduleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A price schedule identification.
    /// </summary>
    public readonly struct PriceSchedule_Id : IId,
                                             IEquatable<PriceSchedule_Id>,
                                             IComparable<PriceSchedule_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the price schedule identification.
        /// </summary>
        public readonly UInt16 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the price schedule identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price schedule identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        private PriceSchedule_Id(UInt16 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price schedule identification.</param>
        public static PriceSchedule_Id Parse(String Text)
        {

            if (TryParse(Text, out var priceScheduleId))
                return priceScheduleId;

            throw new ArgumentException($"Invalid text representation of a price schedule identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a price schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a price schedule identification.</param>
        public static PriceSchedule_Id Parse(UInt16 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price schedule identification.</param>
        public static PriceSchedule_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var priceScheduleId))
                return priceScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a price schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a price schedule identification.</param>
        public static PriceSchedule_Id? TryParse(UInt16 Number)
        {

            if (TryParse(Number, out var priceScheduleId))
                return priceScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out PriceScheduleId)

        /// <summary>
        /// Try to parse the given text as a price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price schedule identification.</param>
        /// <param name="PriceScheduleId">The parsed price schedule identification.</param>
        public static Boolean TryParse(String Text, out PriceSchedule_Id PriceScheduleId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt16.TryParse(Text, out var number))
            {
                PriceScheduleId = new PriceSchedule_Id(number);
                return true;
            }

            PriceScheduleId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out PriceScheduleId)

        /// <summary>
        /// Try to parse the given number as a price schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a price schedule identification.</param>
        /// <param name="PriceScheduleId">The parsed price schedule identification.</param>
        public static Boolean TryParse(UInt16 Number, out PriceSchedule_Id PriceScheduleId)
        {

            PriceScheduleId = new PriceSchedule_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this price schedule identification.
        /// </summary>
        public PriceSchedule_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceSchedule_Id PriceScheduleId1,
                                           PriceSchedule_Id PriceScheduleId2)

            => PriceScheduleId1.Equals(PriceScheduleId2);

        #endregion

        #region Operator != (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceSchedule_Id PriceScheduleId1,
                                           PriceSchedule_Id PriceScheduleId2)

            => !PriceScheduleId1.Equals(PriceScheduleId2);

        #endregion

        #region Operator <  (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceSchedule_Id PriceScheduleId1,
                                          PriceSchedule_Id PriceScheduleId2)

            => PriceScheduleId1.CompareTo(PriceScheduleId2) < 0;

        #endregion

        #region Operator <= (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceSchedule_Id PriceScheduleId1,
                                           PriceSchedule_Id PriceScheduleId2)

            => PriceScheduleId1.CompareTo(PriceScheduleId2) <= 0;

        #endregion

        #region Operator >  (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceSchedule_Id PriceScheduleId1,
                                          PriceSchedule_Id PriceScheduleId2)

            => PriceScheduleId1.CompareTo(PriceScheduleId2) > 0;

        #endregion

        #region Operator >= (PriceScheduleId1, PriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceScheduleId1">A price schedule identification.</param>
        /// <param name="PriceScheduleId2">Another price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceSchedule_Id PriceScheduleId1,
                                           PriceSchedule_Id PriceScheduleId2)

            => PriceScheduleId1.CompareTo(PriceScheduleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PriceScheduleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two price schedule identifications.
        /// </summary>
        /// <param name="Object">A price schedule identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PriceSchedule_Id priceScheduleId
                   ? CompareTo(priceScheduleId)
                   : throw new ArgumentException("The given object is not a price schedule identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceScheduleId)

        /// <summary>
        /// Compares two price schedule identifications.
        /// </summary>
        /// <param name="PriceScheduleId">A price schedule identification to compare with.</param>
        public Int32 CompareTo(PriceSchedule_Id PriceScheduleId)

            => Value.CompareTo(PriceScheduleId.Value);

        #endregion

        #endregion

        #region IEquatable<PriceScheduleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price schedule identifications for equality.
        /// </summary>
        /// <param name="Object">A price schedule identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceSchedule_Id priceScheduleId &&
                   Equals(priceScheduleId);

        #endregion

        #region Equals(PriceScheduleId)

        /// <summary>
        /// Compares two price schedule identifications for equality.
        /// </summary>
        /// <param name="PriceScheduleId">A price schedule identification to compare with.</param>
        public Boolean Equals(PriceSchedule_Id PriceScheduleId)

            => Value.Equals(PriceScheduleId.Value);

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
