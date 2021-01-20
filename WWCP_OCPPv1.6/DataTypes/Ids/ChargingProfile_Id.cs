/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A charging profile identification.
    /// </summary>
    public readonly struct ChargingProfile_Id : IId,
                                                IEquatable<ChargingProfile_Id>,
                                                IComparable<ChargingProfile_Id>
    {

        #region Data

        private readonly UInt64 InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP charging profile identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private ChargingProfile_Id(UInt64 Token)
        {
            this.InternalId = Token;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a charging profile identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out ChargingProfile_Id chargingProfileId))
                return chargingProfileId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a charging profile identification is invalid!");

        }

        #endregion

        #region (static) Parse   (Integer)

        /// <summary>
        /// Parse the given number as a charging profile identification.
        /// </summary>
        public static ChargingProfile_Id Parse(UInt64 Integer)
            => new ChargingProfile_Id(Integer);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        public static ChargingProfile_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ChargingProfile_Id chargingProfileId))
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

            if (TryParse(Number, out ChargingProfile_Id chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ChargingProfileId)

        /// <summary>
        /// Try to parse the given string as a charging profile identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile identification.</param>
        /// <param name="ChargingProfileId">The parsed charging profile identification.</param>
        public static Boolean TryParse(String Text, out ChargingProfile_Id ChargingProfileId)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                ChargingProfileId = default;
                return false;
            }

            #endregion

            if (UInt64.TryParse(Text, out UInt64 number))
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
            => new ChargingProfile_Id(InternalId);

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

            => !(ChargingProfileId1 == ChargingProfileId2);

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

            => !(ChargingProfileId1 > ChargingProfileId2);

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

            => !(ChargingProfileId1 < ChargingProfileId2);

        #endregion

        #endregion

        #region IComparable<ChargingProfileId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingProfile_Id chargingProfileId
                   ? CompareTo(chargingProfileId)
                   : throw new ArgumentException("The given object is not a charging profile identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfileId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileId">An object to compare with.</param>
        public Int32 CompareTo(ChargingProfile_Id ChargingProfileId)

            => InternalId.CompareTo(ChargingProfileId.InternalId);

        #endregion

        #endregion

        #region IEquatable<ChargingProfileId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingProfile_Id chargingProfileId &&
                   Equals(chargingProfileId);

        #endregion

        #region Equals(ChargingProfileId)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="ChargingProfileId">A charging profile identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingProfile_Id ChargingProfileId)

            => InternalId.Equals(ChargingProfileId.InternalId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId.ToString();

        #endregion

    }

}
