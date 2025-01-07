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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for cryptographic signing methods.
    /// </summary>
    public static class CryptoSigningMethodExtensions
    {

        /// <summary>
        /// Indicates whether this cryptographic signing method is null or empty.
        /// </summary>
        /// <param name="CryptoSigningMethod">A cryptographic signing method.</param>
        public static Boolean IsNullOrEmpty(this CryptoSigningMethod? CryptoSigningMethod)
            => !CryptoSigningMethod.HasValue || CryptoSigningMethod.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cryptographic signing method is null or empty.
        /// </summary>
        /// <param name="CryptoSigningMethod">A cryptographic signing method.</param>
        public static Boolean IsNotNullOrEmpty(this CryptoSigningMethod? CryptoSigningMethod)
            => CryptoSigningMethod.HasValue && CryptoSigningMethod.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A cryptographic signing method.
    /// </summary>
    public readonly struct CryptoSigningMethod : IId,
                                                 IEquatable<CryptoSigningMethod>,
                                                 IComparable<CryptoSigningMethod>
    {

        #region Data

        private readonly static Dictionary<String, CryptoSigningMethod>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cryptographic signing method is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cryptographic signing method is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cryptographic signing method.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cryptographic signing method based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cryptographic signing method.</param>
        private CryptoSigningMethod(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CryptoSigningMethod Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CryptoSigningMethod(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a cryptographic signing method.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic signing method.</param>
        public static CryptoSigningMethod Parse(String Text)
        {

            if (TryParse(Text, out var cryptoSigningMethod))
                return cryptoSigningMethod;

            throw new ArgumentException($"Invalid text representation of a cryptographic signing method: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cryptographic signing method.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic signing method.</param>
        public static CryptoSigningMethod? TryParse(String Text)
        {

            if (TryParse(Text, out var cryptoSigningMethod))
                return cryptoSigningMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CryptoSigningMethod)

        /// <summary>
        /// Try to parse the given text as a cryptographic signing method.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod">The parsed cryptographic signing method.</param>
        public static Boolean TryParse(String Text, out CryptoSigningMethod CryptoSigningMethod)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CryptoSigningMethod))
                    CryptoSigningMethod = Register(Text);

                return true;

            }

            CryptoSigningMethod = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this cryptographic signing method.
        /// </summary>
        public CryptoSigningMethod Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// JSON
        /// </summary>
        public static CryptoSigningMethod  JSON         { get; }
            = Register("json");

        /// <summary>
        /// binary
        /// </summary>
        public static CryptoSigningMethod  Binary       { get; }
            = Register("binary");

        /// <summary>
        /// binary - Type Length Value
        /// </summary>
        public static CryptoSigningMethod  BinaryTLV    { get; }
            = Register("binaryTLV");

        #endregion


        #region Operator overloading

        #region Operator == (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CryptoSigningMethod CryptoSigningMethod1,
                                           CryptoSigningMethod CryptoSigningMethod2)

            => CryptoSigningMethod1.Equals(CryptoSigningMethod2);

        #endregion

        #region Operator != (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CryptoSigningMethod CryptoSigningMethod1,
                                           CryptoSigningMethod CryptoSigningMethod2)

            => !CryptoSigningMethod1.Equals(CryptoSigningMethod2);

        #endregion

        #region Operator <  (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CryptoSigningMethod CryptoSigningMethod1,
                                          CryptoSigningMethod CryptoSigningMethod2)

            => CryptoSigningMethod1.CompareTo(CryptoSigningMethod2) < 0;

        #endregion

        #region Operator <= (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CryptoSigningMethod CryptoSigningMethod1,
                                           CryptoSigningMethod CryptoSigningMethod2)

            => CryptoSigningMethod1.CompareTo(CryptoSigningMethod2) <= 0;

        #endregion

        #region Operator >  (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CryptoSigningMethod CryptoSigningMethod1,
                                          CryptoSigningMethod CryptoSigningMethod2)

            => CryptoSigningMethod1.CompareTo(CryptoSigningMethod2) > 0;

        #endregion

        #region Operator >= (CryptoSigningMethod1, CryptoSigningMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSigningMethod1">A cryptographic signing method.</param>
        /// <param name="CryptoSigningMethod2">Another cryptographic signing method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CryptoSigningMethod CryptoSigningMethod1,
                                           CryptoSigningMethod CryptoSigningMethod2)

            => CryptoSigningMethod1.CompareTo(CryptoSigningMethod2) >= 0;

        #endregion

        #endregion

        #region IComparable<CryptoSigningMethod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cryptographic signing methods.
        /// </summary>
        /// <param name="Object">A cryptographic signing method to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CryptoSigningMethod cryptoSigningMethod
                   ? CompareTo(cryptoSigningMethod)
                   : throw new ArgumentException("The given object is not a cryptographic signing method!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CryptoSigningMethod)

        /// <summary>
        /// Compares two cryptographic signing methods.
        /// </summary>
        /// <param name="CryptoSigningMethod">A cryptographic signing method to compare with.</param>
        public Int32 CompareTo(CryptoSigningMethod CryptoSigningMethod)

            => String.Compare(InternalId,
                              CryptoSigningMethod.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CryptoSigningMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cryptographic signing methods for equality.
        /// </summary>
        /// <param name="Object">A cryptographic signing method to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CryptoSigningMethod cryptoSigningMethod &&
                   Equals(cryptoSigningMethod);

        #endregion

        #region Equals(CryptoSigningMethod)

        /// <summary>
        /// Compares two cryptographic signing methods for equality.
        /// </summary>
        /// <param name="CryptoSigningMethod">A cryptographic signing method to compare with.</param>
        public Boolean Equals(CryptoSigningMethod CryptoSigningMethod)

            => String.Equals(InternalId,
                             CryptoSigningMethod.InternalId,
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
