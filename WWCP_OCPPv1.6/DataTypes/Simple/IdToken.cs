/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP identification token.
    /// </summary>
    public struct IdToken : IId,
                            IEquatable<IdToken>,
                            IComparable<IdToken>
    {

        #region Data

        private readonly String _Value;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP identification token.
        /// </summary>
        /// <param name="Token">A string.</param>
        private IdToken(String  Token)
        {
            this._Value = Token;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an OCPP identification token.
        /// </summary>
        public static IdToken Parse(String Text)
            => new IdToken(Text);

        #endregion

        #region TryParse(Text, out IdToken)

        /// <summary>
        /// Parse the given string as an OCPP identification token.
        /// </summary>
        public static Boolean TryParse(String Text, out IdToken IdToken)
        {

            IdToken = new IdToken(Text);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this identification token.
        /// </summary>
        public IdToken Clone
            => new IdToken(new String(_Value.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdToken IdToken1, IdToken IdToken2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(IdToken1, IdToken2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) IdToken1 == null) || ((Object) IdToken2 == null))
                return false;

            if ((Object) IdToken1 == null)
                throw new ArgumentNullException(nameof(IdToken1),  "The given id token must not be null!");

            return IdToken1.Equals(IdToken2);

        }

        #endregion

        #region Operator != (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdToken IdToken1, IdToken IdToken2)
            => !(IdToken1 == IdToken2);

        #endregion

        #region Operator <  (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IdToken IdToken1, IdToken IdToken2)
        {

            if ((Object) IdToken1 == null)
                throw new ArgumentNullException(nameof(IdToken1),  "The given id token must not be null!");

            return IdToken1.CompareTo(IdToken2) < 0;

        }

        #endregion

        #region Operator <= (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IdToken IdToken1, IdToken IdToken2)
            => !(IdToken1 > IdToken2);

        #endregion

        #region Operator >  (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IdToken IdToken1, IdToken IdToken2)
        {

            if ((Object) IdToken1 == null)
                throw new ArgumentNullException(nameof(IdToken1),  "The given id token must not be null!");

            return IdToken1.CompareTo(IdToken2) > 0;

        }

        #endregion

        #region Operator >= (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id token.</param>
        /// <param name="IdToken2">Another id token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IdToken IdToken1, IdToken IdToken2)
            => !(IdToken1 < IdToken2);

        #endregion

        #endregion

        #region IComparable<IdToken> Members

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
            if (!(Object is IdToken))
                throw new ArgumentException("The given object is not a IdToken!", nameof(Object));

            return CompareTo((IdToken) Object);

        }

        #endregion

        #region CompareTo(IdToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken">An object to compare with.</param>
        public Int32 CompareTo(IdToken IdToken)
        {

            if ((Object) IdToken == null)
                throw new ArgumentNullException(nameof(IdToken),  "The given id token must not be null!");

            return String.Compare(_Value, IdToken._Value, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<IdToken> Members

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
            if (!(Object is IdToken))
                return false;

            return this.Equals((IdToken) Object);

        }

        #endregion

        #region Equals(IdToken)

        /// <summary>
        /// Compares two id tokens for equality.
        /// </summary>
        /// <param name="IdToken">An id token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdToken IdToken)
        {

            if ((Object) IdToken == null)
                return false;

            return _Value.Equals(IdToken._Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => _Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => _Value;

        #endregion


    }

}
