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
    /// Extension methods for charging schedule identifications.
    /// </summary>
    public static class ChargingScheduleIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging schedule identification is null or empty.
        /// </summary>
        /// <param name="ChargingScheduleId">A charging schedule identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingSchedule_Id? ChargingScheduleId)
            => !ChargingScheduleId.HasValue || ChargingScheduleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging schedule identification is null or empty.
        /// </summary>
        /// <param name="ChargingScheduleId">A charging schedule identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingSchedule_Id? ChargingScheduleId)
            => ChargingScheduleId.HasValue && ChargingScheduleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charging schedule identification.
    /// </summary>
    public readonly struct ChargingSchedule_Id : IId,
                                          IEquatable<ChargingSchedule_Id>,
                                          IComparable<ChargingSchedule_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the charging schedule identification.
        /// </summary>
        public readonly UInt64 Value;

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
        /// The length of the charging schedule identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging schedule identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging schedule identification.</param>
        private ChargingSchedule_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom()

        /// <summary>
        /// Create a new random charging schedule identification.
        /// </summary>
        public static ChargingSchedule_Id NewRandom()

            => new (RandomExtensions.RandomUInt64());

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given string as a charging schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging schedule identification.</param>
        public static ChargingSchedule_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargingScheduleId))
                return chargingScheduleId;

            throw new ArgumentException($"Invalid text representation of a charging schedule identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse    (Number)

        /// <summary>
        /// Parse the given number as a charging schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging schedule identification.</param>
        public static ChargingSchedule_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a charging schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging schedule identification.</param>
        public static ChargingSchedule_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingScheduleId))
                return chargingScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse (Number)

        /// <summary>
        /// Try to parse the given number as a charging schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging schedule identification.</param>
        public static ChargingSchedule_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var chargingScheduleId))
                return chargingScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text,   out ChargingScheduleId)

        /// <summary>
        /// Try to parse the given text as a charging schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging schedule identification.</param>
        /// <param name="ChargingScheduleId">The parsed charging schedule identification.</param>
        public static Boolean TryParse(String Text, out ChargingSchedule_Id ChargingScheduleId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                ChargingScheduleId = new ChargingSchedule_Id(number);
                return true;
            }

            ChargingScheduleId = default;
            return false;

        }

        #endregion

        #region (static) TryParse (Number, out ChargingScheduleId)

        /// <summary>
        /// Try to parse the given number as a charging schedule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging schedule identification.</param>
        /// <param name="ChargingScheduleId">The parsed charging schedule identification.</param>
        public static Boolean TryParse(UInt64 Number, out ChargingSchedule_Id ChargingScheduleId)
        {

            ChargingScheduleId = new ChargingSchedule_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging schedule identification.
        /// </summary>
        public ChargingSchedule_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingSchedule_Id ChargingScheduleId1,
                                           ChargingSchedule_Id ChargingScheduleId2)

            => ChargingScheduleId1.Equals(ChargingScheduleId2);

        #endregion

        #region Operator != (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedule_Id ChargingScheduleId1,
                                           ChargingSchedule_Id ChargingScheduleId2)

            => !ChargingScheduleId1.Equals(ChargingScheduleId2);

        #endregion

        #region Operator <  (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingSchedule_Id ChargingScheduleId1,
                                          ChargingSchedule_Id ChargingScheduleId2)

            => ChargingScheduleId1.CompareTo(ChargingScheduleId2) < 0;

        #endregion

        #region Operator <= (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingSchedule_Id ChargingScheduleId1,
                                           ChargingSchedule_Id ChargingScheduleId2)

            => ChargingScheduleId1.CompareTo(ChargingScheduleId2) <= 0;

        #endregion

        #region Operator >  (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingSchedule_Id ChargingScheduleId1,
                                          ChargingSchedule_Id ChargingScheduleId2)

            => ChargingScheduleId1.CompareTo(ChargingScheduleId2) > 0;

        #endregion

        #region Operator >= (ChargingScheduleId1, ChargingScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingScheduleId1">A charging schedule identification.</param>
        /// <param name="ChargingScheduleId2">Another charging schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingSchedule_Id ChargingScheduleId1,
                                           ChargingSchedule_Id ChargingScheduleId2)

            => ChargingScheduleId1.CompareTo(ChargingScheduleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingScheduleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging schedule identifications.
        /// </summary>
        /// <param name="Object">A charging schedule identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingSchedule_Id chargingScheduleId
                   ? CompareTo(chargingScheduleId)
                   : throw new ArgumentException("The given object is not a charging schedule identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingScheduleId)

        /// <summary>
        /// Compares two charging schedule identifications.
        /// </summary>
        /// <param name="ChargingScheduleId">A charging schedule identification to compare with.</param>
        public Int32 CompareTo(ChargingSchedule_Id ChargingScheduleId)

            => Value.CompareTo(ChargingScheduleId.Value);

        #endregion

        #endregion

        #region IEquatable<ChargingScheduleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging schedule identifications for equality.
        /// </summary>
        /// <param name="Object">A charging schedule identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingSchedule_Id chargingScheduleId &&
                   Equals(chargingScheduleId);

        #endregion

        #region Equals(ChargingScheduleId)

        /// <summary>
        /// Compares two charging schedule identifications for equality.
        /// </summary>
        /// <param name="ChargingScheduleId">A charging schedule identification to compare with.</param>
        public Boolean Equals(ChargingSchedule_Id ChargingScheduleId)

            => Value.Equals(ChargingScheduleId.Value);

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
