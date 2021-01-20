/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
    /// A charge box identification.
    /// </summary>
    public struct ChargeBox_Id : IId,
                                 IEquatable<ChargeBox_Id>,
                                 IComparable<ChargeBox_Id>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge box identification.
        /// </summary>
        /// <param name="Token">A string.</param>
        private ChargeBox_Id(String  Token)
        {
            this.InternalId = Token;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charge box identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge box identification.</param>
        public static ChargeBox_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a charge box identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out ChargeBox_Id chargeBoxId))
                return chargeBoxId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a charge box identification is invalid!");

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a charge box identification.
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
        /// Try to parse the given string as a charge box identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge box identification.</param>
        /// <param name="ChargeBoxId">The parsed charge box identification.</param>
        public static Boolean TryParse(String Text, out ChargeBox_Id ChargeBoxId)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                ChargeBoxId = default;
                return false;
            }

            #endregion

            try
            {
                ChargeBoxId = new ChargeBox_Id(Text);
                return true;
            }
            catch (Exception)
            { }

            ChargeBoxId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge box identification.
        /// </summary>
        public ChargeBox_Id Clone
            => new ChargeBox_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargeBoxId1, ChargeBoxId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargeBoxId1 == null) || ((Object) ChargeBoxId2 == null))
                return false;

            if ((Object) ChargeBoxId1 == null)
                throw new ArgumentNullException(nameof(ChargeBoxId1),  "The given charge box identification must not be null!");

            return ChargeBoxId1.Equals(ChargeBoxId2);

        }

        #endregion

        #region Operator != (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
            => !(ChargeBoxId1 == ChargeBoxId2);

        #endregion

        #region Operator <  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
        {

            if ((Object) ChargeBoxId1 == null)
                throw new ArgumentNullException(nameof(ChargeBoxId1),  "The given charge box identification must not be null!");

            return ChargeBoxId1.CompareTo(ChargeBoxId2) < 0;

        }

        #endregion

        #region Operator <= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
            => !(ChargeBoxId1 > ChargeBoxId2);

        #endregion

        #region Operator >  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
        {

            if ((Object) ChargeBoxId1 == null)
                throw new ArgumentNullException(nameof(ChargeBoxId1),  "The given charge box identification must not be null!");

            return ChargeBoxId1.CompareTo(ChargeBoxId2) > 0;

        }

        #endregion

        #region Operator >= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">An charge box identification.</param>
        /// <param name="ChargeBoxId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargeBox_Id ChargeBoxId1, ChargeBox_Id ChargeBoxId2)
            => !(ChargeBoxId1 < ChargeBoxId2);

        #endregion

        #endregion

        #region IComparable<ChargeBoxId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a charge box identification.
            if (!(Object is ChargeBox_Id))
                throw new ArgumentException("The given object is not a charge box identification!", nameof(Object));

            return CompareTo((ChargeBox_Id) Object);

        }

        #endregion

        #region CompareTo(ChargeBoxId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId">An object to compare with.</param>
        public Int32 CompareTo(ChargeBox_Id ChargeBoxId)
        {

            if ((Object) ChargeBoxId == null)
                throw new ArgumentNullException(nameof(ChargeBoxId),  "The given charge box identification must not be null!");

            return String.Compare(InternalId, ChargeBoxId.InternalId, StringComparison.Ordinal);

        }

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
        {

            if (Object is null)
                return false;

            // Check if the given object is a charge box identification.
            if (!(Object is ChargeBox_Id))
                return false;

            return this.Equals((ChargeBox_Id) Object);

        }

        #endregion

        #region Equals(ChargeBoxId)

        /// <summary>
        /// Compares two charge box identifications for equality.
        /// </summary>
        /// <param name="ChargeBoxId">An charge box identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargeBox_Id ChargeBoxId)
        {

            if ((Object) ChargeBoxId == null)
                return false;

            return InternalId.Equals(ChargeBoxId.InternalId);

        }

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
            => InternalId;

        #endregion


    }

}
