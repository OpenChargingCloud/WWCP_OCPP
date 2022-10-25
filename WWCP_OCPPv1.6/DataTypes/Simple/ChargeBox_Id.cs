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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for charge box identifications.
    /// </summary>
    public static class ChargeBoxIdExtentions
    {

        /// <summary>
        /// Indicates whether this charge box identification is null or empty.
        /// </summary>
        /// <param name="ChargeBoxId">A charge box identification.</param>
        public static Boolean IsNullOrEmpty(this ChargeBox_Id? ChargeBoxId)
            => !ChargeBoxId.HasValue || ChargeBoxId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charge box identification is null or empty.
        /// </summary>
        /// <param name="ChargeBoxId">A charge box identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargeBox_Id? ChargeBoxId)
            => ChargeBoxId.HasValue && ChargeBoxId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charge box identification.
    /// </summary>
    public readonly struct ChargeBox_Id : IId,
                                          IEquatable<ChargeBox_Id>,
                                          IComparable<ChargeBox_Id>
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
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the charge box identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge box identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the charge box identification.</param>
        private ChargeBox_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Length = 30)

        /// <summary>
        /// Create a new random charge box identification.
        /// </summary>
        /// <param name="Length">The expected length of the charge box identification.</param>
        public static ChargeBox_Id Random(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as charge box identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge box identification.</param>
        public static ChargeBox_Id Parse(String Text)
        {

            if (TryParse(Text, out ChargeBox_Id chargeBoxId))
                return chargeBoxId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a charge box identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a charge box identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as charge box identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge box identification.</param>
        public static ChargeBox_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ChargeBox_Id chargeBoxId))
                return chargeBoxId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ChargeBoxId)

        /// <summary>
        /// Try to parse the given text as charge box identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge box identification.</param>
        /// <param name="ChargeBoxId">The parsed charge box identification.</param>
        public static Boolean TryParse(String Text, out ChargeBox_Id ChargeBoxId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ChargeBoxId = new ChargeBox_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            ChargeBoxId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge box identification.
        /// </summary>
        public ChargeBox_Id Clone

            => new ChargeBox_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargeBox_Id ChargeBoxId1,
                                           ChargeBox_Id ChargeBoxId2)

            => ChargeBoxId1.Equals(ChargeBoxId2);

        #endregion

        #region Operator != (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargeBox_Id ChargeBoxId1,
                                           ChargeBox_Id ChargeBoxId2)

            => !ChargeBoxId1.Equals(ChargeBoxId2);

        #endregion

        #region Operator <  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargeBox_Id ChargeBoxId1,
                                          ChargeBox_Id ChargeBoxId2)

            => ChargeBoxId1.CompareTo(ChargeBoxId2) < 0;

        #endregion

        #region Operator <= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargeBox_Id ChargeBoxId1,
                                           ChargeBox_Id ChargeBoxId2)

            => ChargeBoxId1.CompareTo(ChargeBoxId2) <= 0;

        #endregion

        #region Operator >  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargeBox_Id ChargeBoxId1,
                                          ChargeBox_Id ChargeBoxId2)

            => ChargeBoxId1.CompareTo(ChargeBoxId2) > 0;

        #endregion

        #region Operator >= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargeBox_Id ChargeBoxId1,
                                           ChargeBox_Id ChargeBoxId2)

            => ChargeBoxId1.CompareTo(ChargeBoxId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargeBoxId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargeBox_Id chargeBoxId
                   ? CompareTo(chargeBoxId)
                   : throw new ArgumentException("The given object is not a charge box identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargeBoxId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId">An object to compare with.</param>
        public Int32 CompareTo(ChargeBox_Id ChargeBoxId)

            => String.Compare(InternalId,
                              ChargeBoxId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargeBoxId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargeBox_Id chargeBoxId &&
                   Equals(chargeBoxId);

        #endregion

        #region Equals(ChargeBoxId)

        /// <summary>
        /// Compares two charge box identifications for equality.
        /// </summary>
        /// <param name="ChargeBoxId">A charge box identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargeBox_Id ChargeBoxId)

            => String.Equals(InternalId,
                             ChargeBoxId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
