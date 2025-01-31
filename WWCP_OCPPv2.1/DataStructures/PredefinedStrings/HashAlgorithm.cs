/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extension methods for hash algorithms.
    /// </summary>
    public static class HashAlgorithmExtensions
    {

        /// <summary>
        /// Indicates whether this hash algorithm is null or empty.
        /// </summary>
        /// <param name="HashAlgorithm">A hash algorithm.</param>
        public static Boolean IsNullOrEmpty(this HashAlgorithm? HashAlgorithm)
            => !HashAlgorithm.HasValue || HashAlgorithm.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this hash algorithm is null or empty.
        /// </summary>
        /// <param name="HashAlgorithm">A hash algorithm.</param>
        public static Boolean IsNotNullOrEmpty(this HashAlgorithm? HashAlgorithm)
            => HashAlgorithm.HasValue && HashAlgorithm.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A hash algorithm.
    /// </summary>
    public readonly struct HashAlgorithm : IId,
                                           IEquatable<HashAlgorithm>,
                                           IComparable<HashAlgorithm>
    {

        #region Data

        private readonly static Dictionary<String, HashAlgorithm>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this hash algorithm is null or empty.
        /// </summary>
        public readonly  Boolean                     IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this hash algorithm is NOT null or empty.
        /// </summary>
        public readonly  Boolean                     IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the hash algorithm.
        /// </summary>
        public readonly  UInt64                      Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered hash algorithms.
        /// </summary>
        public static    IEnumerable<HashAlgorithm>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new hash algorithm based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a hash algorithm.</param>
        private HashAlgorithm(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static HashAlgorithm Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new HashAlgorithm(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        public static HashAlgorithm Parse(String Text)
        {

            if (TryParse(Text, out var hashAlgorithm))
                return hashAlgorithm;

            throw new ArgumentException($"Invalid text representation of a hash algorithm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        public static HashAlgorithm? TryParse(String Text)
        {

            if (TryParse(Text, out var hashAlgorithm))
                return hashAlgorithm;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out HashAlgorithm)

        /// <summary>
        /// Try to parse the given text as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        /// <param name="HashAlgorithm">The parsed hash algorithm.</param>
        public static Boolean TryParse(String Text, out HashAlgorithm HashAlgorithm)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out HashAlgorithm))
                    HashAlgorithm = Register(Text);

                return true;

            }

            HashAlgorithm = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this hash algorithm.
        /// </summary>
        public HashAlgorithm Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The SHA-256 hash algorithm.
        /// </summary>
        public static HashAlgorithm  SHA256    { get; }
            = Register("SHA256");

        /// <summary>
        /// The SHA-384 hash algorithm.
        /// </summary>
        public static HashAlgorithm  SHA384    { get; }
            = Register("SHA384");

        /// <summary>
        /// The SHA-512 hash algorithm.
        /// </summary>
        public static HashAlgorithm  SHA512    { get; }
            = Register("SHA512");

        #endregion


        #region Operator overloading

        #region Operator == (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (HashAlgorithm HashAlgorithm1,
                                           HashAlgorithm HashAlgorithm2)

            => HashAlgorithm1.Equals(HashAlgorithm2);

        #endregion

        #region Operator != (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (HashAlgorithm HashAlgorithm1,
                                           HashAlgorithm HashAlgorithm2)

            => !HashAlgorithm1.Equals(HashAlgorithm2);

        #endregion

        #region Operator <  (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (HashAlgorithm HashAlgorithm1,
                                          HashAlgorithm HashAlgorithm2)

            => HashAlgorithm1.CompareTo(HashAlgorithm2) < 0;

        #endregion

        #region Operator <= (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (HashAlgorithm HashAlgorithm1,
                                           HashAlgorithm HashAlgorithm2)

            => HashAlgorithm1.CompareTo(HashAlgorithm2) <= 0;

        #endregion

        #region Operator >  (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (HashAlgorithm HashAlgorithm1,
                                          HashAlgorithm HashAlgorithm2)

            => HashAlgorithm1.CompareTo(HashAlgorithm2) > 0;

        #endregion

        #region Operator >= (HashAlgorithm1, HashAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HashAlgorithm1">A hash algorithm.</param>
        /// <param name="HashAlgorithm2">Another hash algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (HashAlgorithm HashAlgorithm1,
                                           HashAlgorithm HashAlgorithm2)

            => HashAlgorithm1.CompareTo(HashAlgorithm2) >= 0;

        #endregion

        #endregion

        #region IComparable<HashAlgorithm> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two hash algorithms.
        /// </summary>
        /// <param name="Object">A hash algorithm to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is HashAlgorithm hashAlgorithm
                   ? CompareTo(hashAlgorithm)
                   : throw new ArgumentException("The given object is not a hash algorithm!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(HashAlgorithm)

        /// <summary>
        /// Compares two hash algorithms.
        /// </summary>
        /// <param name="HashAlgorithm">A hash algorithm to compare with.</param>
        public Int32 CompareTo(HashAlgorithm HashAlgorithm)

            => String.Compare(InternalId,
                              HashAlgorithm.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<HashAlgorithm> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two hash algorithms for equality.
        /// </summary>
        /// <param name="Object">A hash algorithm to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HashAlgorithm hashAlgorithm &&
                   Equals(hashAlgorithm);

        #endregion

        #region Equals(HashAlgorithm)

        /// <summary>
        /// Compares two hash algorithms for equality.
        /// </summary>
        /// <param name="HashAlgorithm">A hash algorithm to compare with.</param>
        public Boolean Equals(HashAlgorithm HashAlgorithm)

            => String.Equals(InternalId,
                             HashAlgorithm.InternalId,
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
