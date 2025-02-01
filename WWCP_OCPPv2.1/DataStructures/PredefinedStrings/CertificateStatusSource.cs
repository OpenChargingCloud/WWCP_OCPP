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
    /// Extension methods for certificate status sources.
    /// </summary>
    public static class CertificateStatusSourceExtensions
    {

        /// <summary>
        /// Indicates whether this certificate status source is null or empty.
        /// </summary>
        /// <param name="CertificateStatusSource">A certificate status source.</param>
        public static Boolean IsNullOrEmpty(this CertificateStatusSource? CertificateStatusSource)
            => !CertificateStatusSource.HasValue || CertificateStatusSource.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this certificate status source is null or empty.
        /// </summary>
        /// <param name="CertificateStatusSource">A certificate status source.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateStatusSource? CertificateStatusSource)
            => CertificateStatusSource.HasValue && CertificateStatusSource.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A certificate status source.
    /// </summary>
    public readonly struct CertificateStatusSource : IId,
                                                   IEquatable<CertificateStatusSource>,
                                                   IComparable<CertificateStatusSource>
    {

        #region Data

        private readonly static Dictionary<String, CertificateStatusSource>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this certificate status source is null or empty.
        /// </summary>
        public readonly  Boolean                             IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this certificate status source is NOT null or empty.
        /// </summary>
        public readonly  Boolean                             IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the certificate status source.
        /// </summary>
        public readonly  UInt64                              Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered certificate status sources.
        /// </summary>
        public static    IEnumerable<CertificateStatusSource>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate status source based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate status source.</param>
        private CertificateStatusSource(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CertificateStatusSource Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CertificateStatusSource(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a certificate status source.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status source.</param>
        public static CertificateStatusSource Parse(String Text)
        {

            if (TryParse(Text, out var certificateStatusSource))
                return certificateStatusSource;

            throw new ArgumentException($"Invalid text representation of a certificate status source: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate status source.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status source.</param>
        public static CertificateStatusSource? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateStatusSource))
                return certificateStatusSource;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateStatusSource)

        /// <summary>
        /// Try to parse the given text as a certificate status source.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status source.</param>
        /// <param name="CertificateStatusSource">The parsed certificate status source.</param>
        public static Boolean TryParse(String Text, out CertificateStatusSource CertificateStatusSource)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CertificateStatusSource))
                    CertificateStatusSource = Register(Text);

                return true;

            }

            CertificateStatusSource = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this certificate status source.
        /// </summary>
        public CertificateStatusSource Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Certificate Revocation List (CRL)
        /// </summary>
        public static CertificateStatusSource  CRL     { get; }
            = Register("CRL");

        /// <summary>
        /// Online Certificate Status Protocol (OCSP)
        /// </summary>
        public static CertificateStatusSource  OCSP    { get; }
            = Register("OCSP");

        #endregion


        #region Operator overloading

        #region Operator == (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateStatusSource CertificateStatusSource1,
                                           CertificateStatusSource CertificateStatusSource2)

            => CertificateStatusSource1.Equals(CertificateStatusSource2);

        #endregion

        #region Operator != (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateStatusSource CertificateStatusSource1,
                                           CertificateStatusSource CertificateStatusSource2)

            => !CertificateStatusSource1.Equals(CertificateStatusSource2);

        #endregion

        #region Operator <  (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateStatusSource CertificateStatusSource1,
                                          CertificateStatusSource CertificateStatusSource2)

            => CertificateStatusSource1.CompareTo(CertificateStatusSource2) < 0;

        #endregion

        #region Operator <= (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateStatusSource CertificateStatusSource1,
                                           CertificateStatusSource CertificateStatusSource2)

            => CertificateStatusSource1.CompareTo(CertificateStatusSource2) <= 0;

        #endregion

        #region Operator >  (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateStatusSource CertificateStatusSource1,
                                          CertificateStatusSource CertificateStatusSource2)

            => CertificateStatusSource1.CompareTo(CertificateStatusSource2) > 0;

        #endregion

        #region Operator >= (CertificateStatusSource1, CertificateStatusSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusSource1">A certificate status source.</param>
        /// <param name="CertificateStatusSource2">Another certificate status source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateStatusSource CertificateStatusSource1,
                                           CertificateStatusSource CertificateStatusSource2)

            => CertificateStatusSource1.CompareTo(CertificateStatusSource2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateStatusSource> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two certificate status sources.
        /// </summary>
        /// <param name="Object">A certificate status source to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateStatusSource certificateStatusSource
                   ? CompareTo(certificateStatusSource)
                   : throw new ArgumentException("The given object is not a certificate status source!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateStatusSource)

        /// <summary>
        /// Compares two certificate status sources.
        /// </summary>
        /// <param name="CertificateStatusSource">A certificate status source to compare with.</param>
        public Int32 CompareTo(CertificateStatusSource CertificateStatusSource)

            => String.Compare(InternalId,
                              CertificateStatusSource.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CertificateStatusSource> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate status sources for equality.
        /// </summary>
        /// <param name="Object">A certificate status source to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateStatusSource certificateStatusSource &&
                   Equals(certificateStatusSource);

        #endregion

        #region Equals(CertificateStatusSource)

        /// <summary>
        /// Compares two certificate status sources for equality.
        /// </summary>
        /// <param name="CertificateStatusSource">A certificate status source to compare with.</param>
        public Boolean Equals(CertificateStatusSource CertificateStatusSource)

            => String.Equals(InternalId,
                             CertificateStatusSource.InternalId,
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
