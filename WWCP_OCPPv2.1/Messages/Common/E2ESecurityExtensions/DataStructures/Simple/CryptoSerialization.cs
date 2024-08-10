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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for cryptographic serializations.
    /// </summary>
    public static class CryptoSerializationExtensions
    {

        /// <summary>
        /// Indicates whether this cryptographic serialization is null or empty.
        /// </summary>
        /// <param name="CryptoSerialization">A cryptographic serialization.</param>
        public static Boolean IsNullOrEmpty(this CryptoSerialization? CryptoSerialization)
            => !CryptoSerialization.HasValue || CryptoSerialization.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cryptographic serialization is null or empty.
        /// </summary>
        /// <param name="CryptoSerialization">A cryptographic serialization.</param>
        public static Boolean IsNotNullOrEmpty(this CryptoSerialization? CryptoSerialization)
            => CryptoSerialization.HasValue && CryptoSerialization.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A cryptographic serialization.
    /// </summary>
    public readonly struct CryptoSerialization : IId,
                                                 IEquatable<CryptoSerialization>,
                                                 IComparable<CryptoSerialization>
    {

        #region Data

        private readonly static Dictionary<String, CryptoSerialization>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cryptographic serialization is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cryptographic serialization is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cryptographic serialization.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cryptographic serialization based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cryptographic serialization.</param>
        private CryptoSerialization(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CryptoSerialization Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CryptoSerialization(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a cryptographic serialization.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic serialization.</param>
        public static CryptoSerialization Parse(String Text)
        {

            if (TryParse(Text, out var cryptoSerialization))
                return cryptoSerialization;

            throw new ArgumentException($"Invalid text representation of a cryptographic serialization: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cryptographic serialization.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic serialization.</param>
        public static CryptoSerialization? TryParse(String Text)
        {

            if (TryParse(Text, out var cryptoSerialization))
                return cryptoSerialization;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CryptoSerialization)

        /// <summary>
        /// Try to parse the given text as a cryptographic serialization.
        /// </summary>
        /// <param name="Text">A text representation of a cryptographic serialization.</param>
        /// <param name="CryptoSerialization">The parsed cryptographic serialization.</param>
        public static Boolean TryParse(String Text, out CryptoSerialization CryptoSerialization)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CryptoSerialization))
                    CryptoSerialization = Register(Text);

                return true;

            }

            CryptoSerialization = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this cryptographic serialization.
        /// </summary>
        public CryptoSerialization Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// RAW
        /// </summary>
        public static CryptoSerialization raw        { get; }
            = Register("raw");

        /// <summary>
        /// ECC "rs"
        /// </summary>
        public static CryptoSerialization ECC_rs     { get; }
            = Register("ECC_rs");

        /// <summary>
        /// ECC: [r, s]
        /// </summary>
        public static CryptoSerialization ECC_r_s    { get; }
            = Register("ECC_r_s");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CryptoSerialization CryptoSerialization1,
                                           CryptoSerialization CryptoSerialization2)

            => CryptoSerialization1.Equals(CryptoSerialization2);

        #endregion

        #region Operator != (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CryptoSerialization CryptoSerialization1,
                                           CryptoSerialization CryptoSerialization2)

            => !CryptoSerialization1.Equals(CryptoSerialization2);

        #endregion

        #region Operator <  (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CryptoSerialization CryptoSerialization1,
                                          CryptoSerialization CryptoSerialization2)

            => CryptoSerialization1.CompareTo(CryptoSerialization2) < 0;

        #endregion

        #region Operator <= (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CryptoSerialization CryptoSerialization1,
                                           CryptoSerialization CryptoSerialization2)

            => CryptoSerialization1.CompareTo(CryptoSerialization2) <= 0;

        #endregion

        #region Operator >  (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CryptoSerialization CryptoSerialization1,
                                          CryptoSerialization CryptoSerialization2)

            => CryptoSerialization1.CompareTo(CryptoSerialization2) > 0;

        #endregion

        #region Operator >= (CryptoSerialization1, CryptoSerialization2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CryptoSerialization1">A cryptographic serialization.</param>
        /// <param name="CryptoSerialization2">Another cryptographic serialization.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CryptoSerialization CryptoSerialization1,
                                           CryptoSerialization CryptoSerialization2)

            => CryptoSerialization1.CompareTo(CryptoSerialization2) >= 0;

        #endregion

        #endregion

        #region IComparable<CryptoSerialization> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cryptographic serializations.
        /// </summary>
        /// <param name="Object">A cryptographic serialization to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CryptoSerialization cryptoSerialization
                   ? CompareTo(cryptoSerialization)
                   : throw new ArgumentException("The given object is not a cryptographic serialization!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CryptoSerialization)

        /// <summary>
        /// Compares two cryptographic serializations.
        /// </summary>
        /// <param name="CryptoSerialization">A cryptographic serialization to compare with.</param>
        public Int32 CompareTo(CryptoSerialization CryptoSerialization)

            => String.Compare(InternalId,
                              CryptoSerialization.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CryptoSerialization> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cryptographic serializations for equality.
        /// </summary>
        /// <param name="Object">A cryptographic serialization to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CryptoSerialization cryptoSerialization &&
                   Equals(cryptoSerialization);

        #endregion

        #region Equals(CryptoSerialization)

        /// <summary>
        /// Compares two cryptographic serializations for equality.
        /// </summary>
        /// <param name="CryptoSerialization">A cryptographic serialization to compare with.</param>
        public Boolean Equals(CryptoSerialization CryptoSerialization)

            => String.Equals(InternalId,
                             CryptoSerialization.InternalId,
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
