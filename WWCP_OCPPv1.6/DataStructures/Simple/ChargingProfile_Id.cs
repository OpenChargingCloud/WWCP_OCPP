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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for charging profile identifications.
    /// </summary>
    public static class ChargingProfileIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging profile identification is null or empty.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingProfile_Id? ChargingProfileId)
            => !ChargingProfileId.HasValue || ChargingProfileId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging profile identification is null or empty.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingProfile_Id? ChargingProfileId)
            => ChargingProfileId.HasValue && ChargingProfileId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charging profile identification.
    /// </summary>
    public readonly struct ChargingProfile_Id : IId,
                                                IEquatable<ChargingProfile_Id>,
                                                IComparable<ChargingProfile_Id>
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
        /// The length of the charging profile identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile identification.
        /// </summary>
        /// <param name="Number">A number.</param>
        private ChargingProfile_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random charging profile identification.
        /// </summary>
        public static ChargingProfile_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            throw new ArgumentException("Invalid text representation of a charging profile identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a charging profile identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging profile identification.</param>
        public static ChargingProfile_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a charging profile identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging profile identification.</param>
        public static ChargingProfile_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ChargingProfileId)

        /// <summary>
        /// Try to parse the given text as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        /// <param name="ChargingProfileId">The parsed charging profile identification.</param>
        public static Boolean TryParse(String Text, out ChargingProfile_Id ChargingProfileId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                ChargingProfileId = new ChargingProfile_Id(number);
                return true;
            }

            ChargingProfileId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out ChargingProfileId)

        /// <summary>
        /// Try to parse the given number as a charging profile identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging profile identification.</param>
        /// <param name="ChargingProfileId">The parsed charging profile identification.</param>
        public static Boolean TryParse(UInt64 Number, out ChargingProfile_Id ChargingProfileId)
        {

            ChargingProfileId = new ChargingProfile_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging profile identification.
        /// </summary>
        public ChargingProfile_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.Equals(ChargingProfileId2);

        #endregion

        #region Operator != (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => !ChargingProfileId1.Equals(ChargingProfileId2);

        #endregion

        #region Operator <  (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfile_Id ChargingProfileId1,
                                          ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) < 0;

        #endregion

        #region Operator <= (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) <= 0;

        #endregion

        #region Operator >  (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfile_Id ChargingProfileId1,
                                          ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) > 0;

        #endregion

        #region Operator >= (ChargingProfileId1, ChargingProfileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId1">A charging profile identification.</param>
        /// <param name="ChargingProfileId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfile_Id ChargingProfileId1,
                                           ChargingProfile_Id ChargingProfileId2)

            => ChargingProfileId1.CompareTo(ChargingProfileId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingProfileId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging profile identifications.
        /// </summary>
        /// <param name="Object">A charging profile identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingProfile_Id chargingProfileId
                   ? CompareTo(chargingProfileId)
                   : throw new ArgumentException("The given object is not a charging profile identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfileId)

        /// <summary>
        /// Compares two charging profile identifications.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification to compare with.</param>
        public Int32 CompareTo(ChargingProfile_Id ChargingProfileId)

            => Value.CompareTo(ChargingProfileId.Value);

        #endregion

        #endregion

        #region IEquatable<ChargingProfileId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="Object">A charging profile identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfile_Id chargingProfileId &&
                   Equals(chargingProfileId);

        #endregion

        #region Equals(ChargingProfileId)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification to compare with.</param>
        public Boolean Equals(ChargingProfile_Id ChargingProfileId)

            => Value.Equals(ChargingProfileId.Value);

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
