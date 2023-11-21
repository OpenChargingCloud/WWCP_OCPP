/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for cryptographic encodings.
    /// </summary>
    public static class CryptoEncodingExtensions
    {

        /// <summary>
        /// Indicates whether this cryptographic encoding is null or empty.
        /// </summary>
        /// <param name="CryptoEncoding">A cryptographic encoding.</param>
        public static Boolean IsNullOrEmpty(this CryptoEncoding? CryptoEncoding)
            => !CryptoEncoding.HasValue || CryptoEncoding.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cryptographic encoding is null or empty.
        /// </summary>
        /// <param name="CryptoEncoding">A cryptographic encoding.</param>
        public static Boolean IsNotNullOrEmpty(this CryptoEncoding? CryptoEncoding)
            => CryptoEncoding.HasValue && CryptoEncoding.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A cryptographic encoding.
    /// </summary>
    public readonly struct CryptoEncoding : IId,
                                             IEquatable<CryptoEncoding>,
                                             IComparable<CryptoEncoding>
    {

        #region Data

        private readonly static Dictionary<String, CryptoEncoding>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cryptographic encoding is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cryptographic encoding is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cryptographic encoding.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cryptographic encoding based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cryptographic encoding.</param>
        private CryptoEncoding(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CryptoEncoding Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CryptoEncoding(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a cryptographic encoding.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic encoding.</param>
        public static CryptoEncoding Parse(String Text)
        {

            if (TryParse(Text, out var cryptoEncoding))
                return cryptoEncoding;

            throw new ArgumentException($"Invalid text representation of a cryptographic encoding: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cryptographic encoding.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic encoding.</param>
        public static CryptoEncoding? TryParse(String Text)
        {

            if (TryParse(Text, out var cryptoEncoding))
                return cryptoEncoding;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CryptoEncoding)

        /// <summary>
        /// Try to parse the given text as a cryptographic encoding.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic encoding.</param>
        /// <param name="CryptoEncoding">The parsed cryptographic encoding.</param>
        public static Boolean TryParse(String Text, out CryptoEncoding CryptoEncoding)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CryptoEncoding))
                    CryptoEncoding = Register(Text);

                return true;

            }

            CryptoEncoding = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this cryptographic encoding.
        /// </summary>
        public CryptoEncoding Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// base64
        /// </summary>
        public static CryptoEncoding base64    { get; }
            = Register("base64");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CryptoEncoding CryptoEncoding1,
                                           CryptoEncoding CryptoEncoding2)

            => CryptoEncoding1.Equals(CryptoEncoding2);

        #endregion

        #region Operator != (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CryptoEncoding CryptoEncoding1,
                                           CryptoEncoding CryptoEncoding2)

            => !CryptoEncoding1.Equals(CryptoEncoding2);

        #endregion

        #region Operator <  (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CryptoEncoding CryptoEncoding1,
                                          CryptoEncoding CryptoEncoding2)

            => CryptoEncoding1.CompareTo(CryptoEncoding2) < 0;

        #endregion

        #region Operator <= (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CryptoEncoding CryptoEncoding1,
                                           CryptoEncoding CryptoEncoding2)

            => CryptoEncoding1.CompareTo(CryptoEncoding2) <= 0;

        #endregion

        #region Operator >  (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CryptoEncoding CryptoEncoding1,
                                          CryptoEncoding CryptoEncoding2)

            => CryptoEncoding1.CompareTo(CryptoEncoding2) > 0;

        #endregion

        #region Operator >= (CryptoEncoding1, CryptoEncoding2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoEncoding1">A cryptographic encoding.</param>
        /// <param name="CryptoEncoding2">Another cryptographic encoding.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CryptoEncoding CryptoEncoding1,
                                           CryptoEncoding CryptoEncoding2)

            => CryptoEncoding1.CompareTo(CryptoEncoding2) >= 0;

        #endregion

        #endregion

        #region IComparable<CryptoEncoding> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cryptographic encodings.
        /// </summary>
        /// <param name="Object">A cryptographic encoding to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CryptoEncoding cryptoEncoding
                   ? CompareTo(cryptoEncoding)
                   : throw new ArgumentException("The given object is not a cryptographic encoding!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CryptoEncoding)

        /// <summary>
        /// Compares two cryptographic encodings.
        /// </summary>
        /// <param name="CryptoEncoding">A cryptographic encoding to compare with.</param>
        public Int32 CompareTo(CryptoEncoding CryptoEncoding)

            => String.Compare(InternalId,
                              CryptoEncoding.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CryptoEncoding> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cryptographic encodings for equality.
        /// </summary>
        /// <param name="Object">A cryptographic encoding to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CryptoEncoding cryptoEncoding &&
                   Equals(cryptoEncoding);

        #endregion

        #region Equals(CryptoEncoding)

        /// <summary>
        /// Compares two cryptographic encodings for equality.
        /// </summary>
        /// <param name="CryptoEncoding">A cryptographic encoding to compare with.</param>
        public Boolean Equals(CryptoEncoding CryptoEncoding)

            => String.Equals(InternalId,
                             CryptoEncoding.InternalId,
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
