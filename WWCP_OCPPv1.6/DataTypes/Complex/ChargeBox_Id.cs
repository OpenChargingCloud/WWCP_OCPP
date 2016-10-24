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

        #region TryParse(Text, out ChargeBox_Id)

        /// <summary>
        /// Parse the given string as an OCPP charge box identification.
        /// </summary>
        public static Boolean TryParse(String Text, out ChargeBox_Id ChargeBox_Id)
        {

            ChargeBox_Id = new ChargeBox_Id(Text);

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

        #region Operator == (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargeBox_Id1, ChargeBox_Id2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargeBox_Id1 == null) || ((Object) ChargeBox_Id2 == null))
                return false;

            if ((Object) ChargeBox_Id1 == null)
                throw new ArgumentNullException(nameof(ChargeBox_Id1),  "The given id token must not be null!");

            return ChargeBox_Id1.Equals(ChargeBox_Id2);

        }

        #endregion

        #region Operator != (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
            => !(ChargeBox_Id1 == ChargeBox_Id2);

        #endregion

        #region Operator <  (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
        {

            if ((Object) ChargeBox_Id1 == null)
                throw new ArgumentNullException(nameof(ChargeBox_Id1),  "The given id token must not be null!");

            return ChargeBox_Id1.CompareTo(ChargeBox_Id2) < 0;

        }

        #endregion

        #region Operator <= (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
            => !(ChargeBox_Id1 > ChargeBox_Id2);

        #endregion

        #region Operator >  (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
        {

            if ((Object) ChargeBox_Id1 == null)
                throw new ArgumentNullException(nameof(ChargeBox_Id1),  "The given id token must not be null!");

            return ChargeBox_Id1.CompareTo(ChargeBox_Id2) > 0;

        }

        #endregion

        #region Operator >= (ChargeBox_Id1, ChargeBox_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id1">An id token.</param>
        /// <param name="ChargeBox_Id2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargeBox_Id ChargeBox_Id1, ChargeBox_Id ChargeBox_Id2)
            => !(ChargeBox_Id1 < ChargeBox_Id2);

        #endregion

        #endregion

        #region IComparable<ChargeBox_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a id token.
            var ChargeBox_Id = Object as ChargeBox_Id;
            if ((Object) ChargeBox_Id == null)
                throw new ArgumentException("The given object is not a ChargeBox_Id!", nameof(Object));

            return CompareTo(ChargeBox_Id);

        }

        #endregion

        #region CompareTo(ChargeBox_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox_Id">An object to compare with.</param>
        public Int32 CompareTo(ChargeBox_Id ChargeBox_Id)
        {

            if ((Object) ChargeBox_Id == null)
                throw new ArgumentNullException(nameof(ChargeBox_Id),  "The given id token must not be null!");

            return String.Compare(Value, ChargeBox_Id.Value, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<ChargeBox_Id> Members

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

            // Check if the given object is a id token.
            var ChargeBox_Id = Object as ChargeBox_Id;
            if ((Object) ChargeBox_Id == null)
                return false;

            return this.Equals(ChargeBox_Id);

        }

        #endregion

        #region Equals(ChargeBox_Id)

        /// <summary>
        /// Compares two id tokens for equality.
        /// </summary>
        /// <param name="ChargeBox_Id">An id token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargeBox_Id ChargeBox_Id)
        {

            if ((Object) ChargeBox_Id == null)
                return false;

            return Value.Equals(ChargeBox_Id.Value);

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
