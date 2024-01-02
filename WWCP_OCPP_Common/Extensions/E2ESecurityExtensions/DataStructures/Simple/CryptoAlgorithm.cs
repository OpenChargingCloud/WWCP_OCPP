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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for cryptographic algorithms.
    /// </summary>
    public static class CryptoAlgorithmExtensions
    {

        /// <summary>
        /// Indicates whether this cryptographic algorithm is null or empty.
        /// </summary>
        /// <param name="CryptoAlgorithm">A cryptographic algorithm.</param>
        public static Boolean IsNullOrEmpty(this CryptoAlgorithm? CryptoAlgorithm)
            => !CryptoAlgorithm.HasValue || CryptoAlgorithm.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cryptographic algorithm is null or empty.
        /// </summary>
        /// <param name="CryptoAlgorithm">A cryptographic algorithm.</param>
        public static Boolean IsNotNullOrEmpty(this CryptoAlgorithm? CryptoAlgorithm)
            => CryptoAlgorithm.HasValue && CryptoAlgorithm.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A cryptographic algorithm.
    /// </summary>
    public readonly struct CryptoAlgorithm : IId,
                                             IEquatable<CryptoAlgorithm>,
                                             IComparable<CryptoAlgorithm>
    {

        #region Data

        private readonly static Dictionary<String, CryptoAlgorithm>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cryptographic algorithm is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cryptographic algorithm is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cryptographic algorithm.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cryptographic algorithm based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cryptographic algorithm.</param>
        private CryptoAlgorithm(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CryptoAlgorithm Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CryptoAlgorithm(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a cryptographic algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic algorithm.</param>
        public static CryptoAlgorithm Parse(String Text)
        {

            if (TryParse(Text, out var cryptoAlgorithm))
                return cryptoAlgorithm;

            throw new ArgumentException($"Invalid text representation of a cryptographic algorithm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cryptographic algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic algorithm.</param>
        public static CryptoAlgorithm? TryParse(String Text)
        {

            if (TryParse(Text, out var cryptoAlgorithm))
                return cryptoAlgorithm;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CryptoAlgorithm)

        /// <summary>
        /// Try to parse the given text as a cryptographic algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm">The parsed cryptographic algorithm.</param>
        public static Boolean TryParse(String Text, out CryptoAlgorithm CryptoAlgorithm)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CryptoAlgorithm))
                    CryptoAlgorithm = Register(Text);

                return true;

            }

            CryptoAlgorithm = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this cryptographic algorithm.
        /// </summary>
        public CryptoAlgorithm Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// secp256r1
        /// </summary>
        public static CryptoAlgorithm secp256r1    { get; }
            = Register("secp256r1");

        /// <summary>
        /// secp384r1
        /// </summary>
        public static CryptoAlgorithm secp384r1    { get; }
            = Register("secp384r1");

        /// <summary>
        /// secp521r1
        /// </summary>
        public static CryptoAlgorithm secp521r1    { get; }
            = Register("secp521r1");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CryptoAlgorithm CryptoAlgorithm1,
                                           CryptoAlgorithm CryptoAlgorithm2)

            => CryptoAlgorithm1.Equals(CryptoAlgorithm2);

        #endregion

        #region Operator != (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CryptoAlgorithm CryptoAlgorithm1,
                                           CryptoAlgorithm CryptoAlgorithm2)

            => !CryptoAlgorithm1.Equals(CryptoAlgorithm2);

        #endregion

        #region Operator <  (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CryptoAlgorithm CryptoAlgorithm1,
                                          CryptoAlgorithm CryptoAlgorithm2)

            => CryptoAlgorithm1.CompareTo(CryptoAlgorithm2) < 0;

        #endregion

        #region Operator <= (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CryptoAlgorithm CryptoAlgorithm1,
                                           CryptoAlgorithm CryptoAlgorithm2)

            => CryptoAlgorithm1.CompareTo(CryptoAlgorithm2) <= 0;

        #endregion

        #region Operator >  (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CryptoAlgorithm CryptoAlgorithm1,
                                          CryptoAlgorithm CryptoAlgorithm2)

            => CryptoAlgorithm1.CompareTo(CryptoAlgorithm2) > 0;

        #endregion

        #region Operator >= (CryptoAlgorithm1, CryptoAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoAlgorithm1">A cryptographic algorithm.</param>
        /// <param name="CryptoAlgorithm2">Another cryptographic algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CryptoAlgorithm CryptoAlgorithm1,
                                           CryptoAlgorithm CryptoAlgorithm2)

            => CryptoAlgorithm1.CompareTo(CryptoAlgorithm2) >= 0;

        #endregion

        #endregion

        #region IComparable<CryptoAlgorithm> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cryptographic algorithms.
        /// </summary>
        /// <param name="Object">A cryptographic algorithm to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CryptoAlgorithm cryptoAlgorithm
                   ? CompareTo(cryptoAlgorithm)
                   : throw new ArgumentException("The given object is not a cryptographic algorithm!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CryptoAlgorithm)

        /// <summary>
        /// Compares two cryptographic algorithms.
        /// </summary>
        /// <param name="CryptoAlgorithm">A cryptographic algorithm to compare with.</param>
        public Int32 CompareTo(CryptoAlgorithm CryptoAlgorithm)

            => String.Compare(InternalId,
                              CryptoAlgorithm.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CryptoAlgorithm> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cryptographic algorithms for equality.
        /// </summary>
        /// <param name="Object">A cryptographic algorithm to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CryptoAlgorithm cryptoAlgorithm &&
                   Equals(cryptoAlgorithm);

        #endregion

        #region Equals(CryptoAlgorithm)

        /// <summary>
        /// Compares two cryptographic algorithms for equality.
        /// </summary>
        /// <param name="CryptoAlgorithm">A cryptographic algorithm to compare with.</param>
        public Boolean Equals(CryptoAlgorithm CryptoAlgorithm)

            => String.Equals(InternalId,
                             CryptoAlgorithm.InternalId,
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
