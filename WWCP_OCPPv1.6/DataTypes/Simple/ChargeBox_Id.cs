/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP charge box identification.
    /// </summary>
    public class ChargeBox_Id
    {

        #region Properties

        /// <summary>
        /// The value of the charge box identification.
        /// </summary>
        public String Value { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP charge box identification.
        /// </summary>
        /// <param name="Token">A string.</param>
        private ChargeBox_Id(String  Token)
        {
            this.Value = Token;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an OCPP charge box identification.
        /// </summary>
        public static ChargeBox_Id Parse(String Text)
            => new ChargeBox_Id(Text);

        #endregion

        #region TryParse(Text, out ChargeBoxId)

        /// <summary>
        /// Parse the given string as an OCPP charge box identification.
        /// </summary>
        public static Boolean TryParse(String Text, out ChargeBox_Id ChargeBoxId)
        {

            ChargeBoxId = new ChargeBox_Id(Text);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge box identification.
        /// </summary>
        public ChargeBox_Id Clone
            => new ChargeBox_Id(new String(Value.ToCharArray()));

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
            if (Object.ReferenceEquals(ChargeBoxId1, ChargeBoxId2))
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

            if (Object == null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a charge box identification.
            var ChargeBox_Id = Object as ChargeBox_Id;
            if ((Object) ChargeBox_Id == null)
                throw new ArgumentException("The given object is not a charge box identification!", nameof(Object));

            return CompareTo(ChargeBox_Id);

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

            return String.Compare(Value, ChargeBoxId.Value, StringComparison.Ordinal);

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

            if (Object == null)
                return false;

            // Check if the given object is a charge box identification.
            var ChargeBox_Id = Object as ChargeBox_Id;
            if ((Object) ChargeBox_Id == null)
                return false;

            return this.Equals(ChargeBox_Id);

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

            return Value.Equals(ChargeBoxId.Value);

        }

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Value;

        #endregion


    }

}
