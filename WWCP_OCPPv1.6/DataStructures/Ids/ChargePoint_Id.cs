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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for charge point identifications.
    /// </summary>
    public static class ChargePointIdExtensions
    {

        /// <summary>
        /// Indicates whether this charge point identification is null or empty.
        /// </summary>
        /// <param name="ChargePointId">A charge point identification.</param>
        public static Boolean IsNullOrEmpty(this ChargePoint_Id? ChargePointId)
            => !ChargePointId.HasValue || ChargePointId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charge point identification is null or empty.
        /// </summary>
        /// <param name="ChargePointId">A charge point identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargePoint_Id? ChargePointId)
            => ChargePointId.HasValue && ChargePointId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charge point identification.
    /// </summary>
    public readonly struct ChargePoint_Id : IId,
                                            IEquatable<ChargePoint_Id>,
                                            IComparable<ChargePoint_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charge point identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a charge point identification.</param>
        private ChargePoint_Id(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 30)

        /// <summary>
        /// Create a new random charge point identification.
        /// </summary>
        /// <param name="Length">The expected length of the charge point identification.</param>
        public static ChargePoint_Id NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as charge point identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge point identification.</param>
        public static ChargePoint_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargePointId))
                return chargePointId;

            throw new ArgumentException($"Invalid text representation of a charge point identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as charge point identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge point identification.</param>
        public static ChargePoint_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargePointId))
                return chargePointId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ChargePointId)

        /// <summary>
        /// Try to parse the given text as charge point identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge point identification.</param>
        /// <param name="ChargePointId">The parsed charge point identification.</param>
        public static Boolean TryParse(String Text, out ChargePoint_Id ChargePointId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                ChargePointId = new ChargePoint_Id(Text);
                return true;
            }

            ChargePointId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge point identification.
        /// </summary>
        public ChargePoint_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargePoint_Id ChargePointId1,
                                           ChargePoint_Id ChargePointId2)

            => ChargePointId1.Equals(ChargePointId2);

        #endregion

        #region Operator != (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargePoint_Id ChargePointId1,
                                           ChargePoint_Id ChargePointId2)

            => !ChargePointId1.Equals(ChargePointId2);

        #endregion

        #region Operator <  (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargePoint_Id ChargePointId1,
                                          ChargePoint_Id ChargePointId2)

            => ChargePointId1.CompareTo(ChargePointId2) < 0;

        #endregion

        #region Operator <= (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargePoint_Id ChargePointId1,
                                           ChargePoint_Id ChargePointId2)

            => ChargePointId1.CompareTo(ChargePointId2) <= 0;

        #endregion

        #region Operator >  (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargePoint_Id ChargePointId1,
                                          ChargePoint_Id ChargePointId2)

            => ChargePointId1.CompareTo(ChargePointId2) > 0;

        #endregion

        #region Operator >= (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A charge point identification.</param>
        /// <param name="ChargePointId2">Another charge point identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargePoint_Id ChargePointId1,
                                           ChargePoint_Id ChargePointId2)

            => ChargePointId1.CompareTo(ChargePointId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargePointId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charge point identifications.
        /// </summary>
        /// <param name="Object">A charge point identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargePoint_Id chargePointId
                   ? CompareTo(chargePointId)
                   : throw new ArgumentException("The given object is not a charge point identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargePointId)

        /// <summary>
        /// Compares two charge point identifications.
        /// </summary>
        /// <param name="ChargePointId">A charge point identification to compare with.</param>
        public Int32 CompareTo(ChargePoint_Id ChargePointId)

            => String.Compare(InternalId,
                              ChargePointId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargePointId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charge point identifications for equality.
        /// </summary>
        /// <param name="Object">A charge point identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargePoint_Id chargePointId &&
                   Equals(chargePointId);

        #endregion

        #region Equals(ChargePointId)

        /// <summary>
        /// Compares two charge point identifications for equality.
        /// </summary>
        /// <param name="ChargePointId">A charge point identification to compare with.</param>
        public Boolean Equals(ChargePoint_Id ChargePointId)

            => String.Equals(InternalId,
                             ChargePointId.InternalId,
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
