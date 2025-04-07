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
    /// Extension methods for cache entry replacement policys.
    /// </summary>
    public static class CacheEntryReplacementPolicyExtensions
    {

        /// <summary>
        /// Indicates whether this cache entry replacement policy is null or empty.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy">A cache entry replacement policy.</param>
        public static Boolean IsNullOrEmpty(this CacheEntryReplacementPolicy? CacheEntryReplacementPolicy)
            => !CacheEntryReplacementPolicy.HasValue || CacheEntryReplacementPolicy.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cache entry replacement policy is null or empty.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy">A cache entry replacement policy.</param>
        public static Boolean IsNotNullOrEmpty(this CacheEntryReplacementPolicy? CacheEntryReplacementPolicy)
            => CacheEntryReplacementPolicy.HasValue && CacheEntryReplacementPolicy.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A cache entry replacement policy.
    /// </summary>
    public readonly struct CacheEntryReplacementPolicy : IId,
                           IEquatable<CacheEntryReplacementPolicy>,
                           IComparable<CacheEntryReplacementPolicy>
    {

        #region Data

        private readonly static Dictionary<String, CacheEntryReplacementPolicy>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                           InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cache entry replacement policy is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cache entry replacement policy is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cache entry replacement policy.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cache entry replacement policy based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cache entry replacement policy.</param>
        private CacheEntryReplacementPolicy(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CacheEntryReplacementPolicy Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CacheEntryReplacementPolicy(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a cache entry replacement policy.
        /// </summary>
        /// <param name="Text">A text representation of a cache entry replacement policy.</param>
        public static CacheEntryReplacementPolicy Parse(String Text)
        {

            if (TryParse(Text, out var idTokenType))
                return idTokenType;

            throw new ArgumentException($"Invalid text representation of a cache entry replacement policy: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as cache entry replacement policy.
        /// </summary>
        /// <param name="Text">A text representation of a cache entry replacement policy.</param>
        public static CacheEntryReplacementPolicy? TryParse(String Text)
        {

            if (TryParse(Text, out var idTokenType))
                return idTokenType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CacheEntryReplacementPolicy)

        /// <summary>
        /// Try to parse the given text as cache entry replacement policy.
        /// </summary>
        /// <param name="Text">A text representation of a cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy">The parsed cache entry replacement policy.</param>
        public static Boolean TryParse(String Text, out CacheEntryReplacementPolicy CacheEntryReplacementPolicy)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CacheEntryReplacementPolicy))
                    CacheEntryReplacementPolicy = Register(Text);

                return true;

            }

            CacheEntryReplacementPolicy = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this cache entry replacement policy.
        /// </summary>
        public CacheEntryReplacementPolicy Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Least Recently Used (LRU)
        /// </summary>
        public static CacheEntryReplacementPolicy  LRU       { get; }
            = Register("LRU");

        /// <summary>
        /// LeastFrequentlyUsed (LFU)
        /// </summary>
        public static CacheEntryReplacementPolicy  LFU       { get; }
            = Register("LFU");

        /// <summary>
        /// First-In First-Out (FIFO)
        /// </summary>
        public static CacheEntryReplacementPolicy  FIFO      { get; }
            = Register("FIFO");

        /// <summary>
        /// CUSTOM
        /// </summary>
        public static CacheEntryReplacementPolicy  CUSTOM    { get; }
            = Register("CUSTOM");

        #endregion


        #region Operator overloading

        #region Operator == (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                           CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => CacheEntryReplacementPolicy1.Equals(CacheEntryReplacementPolicy2);

        #endregion

        #region Operator != (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                           CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => !CacheEntryReplacementPolicy1.Equals(CacheEntryReplacementPolicy2);

        #endregion

        #region Operator <  (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                          CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => CacheEntryReplacementPolicy1.CompareTo(CacheEntryReplacementPolicy2) < 0;

        #endregion

        #region Operator <= (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                           CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => CacheEntryReplacementPolicy1.CompareTo(CacheEntryReplacementPolicy2) <= 0;

        #endregion

        #region Operator >  (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                          CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => CacheEntryReplacementPolicy1.CompareTo(CacheEntryReplacementPolicy2) > 0;

        #endregion

        #region Operator >= (CacheEntryReplacementPolicy1, CacheEntryReplacementPolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy1">A cache entry replacement policy.</param>
        /// <param name="CacheEntryReplacementPolicy2">Another cache entry replacement policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CacheEntryReplacementPolicy CacheEntryReplacementPolicy1,
                                           CacheEntryReplacementPolicy CacheEntryReplacementPolicy2)

            => CacheEntryReplacementPolicy1.CompareTo(CacheEntryReplacementPolicy2) >= 0;

        #endregion

        #endregion

        #region IComparable<CacheEntryReplacementPolicy> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cache entry replacement policys.
        /// </summary>
        /// <param name="Object">cache entry replacement policy to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CacheEntryReplacementPolicy idTokenType
                   ? CompareTo(idTokenType)
                   : throw new ArgumentException("The given object is not cache entry replacement policy!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CacheEntryReplacementPolicy)

        /// <summary>
        /// Compares two cache entry replacement policys.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy">cache entry replacement policy to compare with.</param>
        public Int32 CompareTo(CacheEntryReplacementPolicy CacheEntryReplacementPolicy)

            => String.Compare(InternalId,
                              CacheEntryReplacementPolicy.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<CacheEntryReplacementPolicy> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cache entry replacement policys for equality.
        /// </summary>
        /// <param name="Object">cache entry replacement policy to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CacheEntryReplacementPolicy idTokenType &&
                   Equals(idTokenType);

        #endregion

        #region Equals(CacheEntryReplacementPolicy)

        /// <summary>
        /// Compares two cache entry replacement policys for equality.
        /// </summary>
        /// <param name="CacheEntryReplacementPolicy">cache entry replacement policy to compare with.</param>
        public Boolean Equals(CacheEntryReplacementPolicy CacheEntryReplacementPolicy)

            => String.Equals(InternalId,
                             CacheEntryReplacementPolicy.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
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
