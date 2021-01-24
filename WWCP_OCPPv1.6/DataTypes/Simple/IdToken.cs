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
    /// An identification token.
    /// </summary>
    public readonly struct IdToken : IId,
                                     IEquatable<IdToken>,
                                     IComparable<IdToken>
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
        /// Create a new identification token.
        /// </summary>
        /// <param name="Token">A string (20 characters).</param>
        private IdToken(String  Token)
        {
            this.InternalId = Token;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an identification token.
        /// </summary>
        /// <param name="Text">A text representation of an identification token.</param>
        public static IdToken Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an identification token must not be null or empty!");

            #endregion

            if (TryParse(Text, out IdToken chargeBoxId))
                return chargeBoxId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of an identification token is invalid!");

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an identification token.
        /// </summary>
        /// <param name="Text">A text representation of an identification token.</param>
        public static IdToken? TryParse(String Text)
        {

            if (TryParse(Text, out IdToken chargeBoxId))
                return chargeBoxId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IdToken)

        /// <summary>
        /// Try to parse the given string as an identification token.
        /// </summary>
        /// <param name="Text">A text representation of an identification token.</param>
        /// <param name="IdToken">The parsed identification token.</param>
        public static Boolean TryParse(String Text, out IdToken IdToken)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                IdToken = default;
                return false;
            }

            #endregion

            try
            {
                IdToken = new IdToken(Text);
                return true;
            }
            catch (Exception)
            { }

            IdToken = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this identification token.
        /// </summary>
        public IdToken Clone
            => new IdToken(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdToken IdToken1,
                                           IdToken IdToken2)

            => IdToken1.Equals(IdToken2);

        #endregion

        #region Operator != (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdToken IdToken1,
                                           IdToken IdToken2)

            => !(IdToken1 == IdToken2);

        #endregion

        #region Operator <  (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IdToken IdToken1,
                                          IdToken IdToken2)

            => IdToken1.CompareTo(IdToken2) < 0;

        #endregion

        #region Operator <= (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IdToken IdToken1,
                                           IdToken IdToken2)

            => !(IdToken1 > IdToken2);

        #endregion

        #region Operator >  (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IdToken IdToken1,
                                          IdToken IdToken2)

            => IdToken1.CompareTo(IdToken2) > 0;

        #endregion

        #region Operator >= (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">A identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IdToken IdToken1,
                                           IdToken IdToken2)

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

            => Object is IdToken idToken
                   ? CompareTo(idToken)
                   : throw new ArgumentException("The given object is not an identification token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IdToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken">An object to compare with.</param>
        public Int32 CompareTo(IdToken IdToken)

            => InternalId.CompareTo(IdToken.InternalId);

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

            => Object is IdToken idToken &&
                   Equals(idToken);

        #endregion

        #region Equals(IdToken)

        /// <summary>
        /// Compares two identification tokens for equality.
        /// </summary>
        /// <param name="IdToken">A identification token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdToken IdToken)

            => InternalId.Equals(IdToken.InternalId);

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
