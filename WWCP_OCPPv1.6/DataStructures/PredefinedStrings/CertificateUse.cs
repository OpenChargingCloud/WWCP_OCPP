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
    /// Extension methods for install certificate uses.
    /// </summary>
    public static class CertificateUseExtensions
    {

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        /// <param name="CertificateUse">A certificate use.</param>
        public static Boolean IsNullOrEmpty(this CertificateUse? CertificateUse)
            => !CertificateUse.HasValue || CertificateUse.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        /// <param name="CertificateUse">A certificate use.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateUse? CertificateUse)
            => CertificateUse.HasValue && CertificateUse.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A certificate use.
    /// </summary>
    public readonly struct CertificateUse : IId,
                                            IEquatable<CertificateUse>,
                                            IComparable<CertificateUse>
    {

        #region Data

        private readonly static Dictionary<String, CertificateUse>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this install certificate use is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the install certificate use.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered install certificate uses.
        /// </summary>
        public static IEnumerable<CertificateUse>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new install certificate use based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate use.</param>
        private CertificateUse(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CertificateUse Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CertificateUse(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        public static CertificateUse Parse(String Text)
        {

            if (TryParse(Text, out var certificateUse))
                return certificateUse;

            throw new ArgumentException($"Invalid text representation of a certificate use: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        public static CertificateUse? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateUse))
                return certificateUse;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateUse)

        /// <summary>
        /// Try to parse the given text as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        /// <param name="CertificateUse">The parsed install certificate use.</param>
        public static Boolean TryParse(String Text, out CertificateUse CertificateUse)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CertificateUse))
                    CertificateUse = Register(Text);

                return true;

            }

            CertificateUse = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this install certificate use.
        /// </summary>
        public CertificateUse Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Use for certificate of the V2G Root.
        /// </summary>
        public static CertificateUse  V2GRootCertificate              { get; } // Backported!
            = Register("V2GRootCertificate");

        /// <summary>
        /// Use for certificate from an e-mobility service provider.
        /// To support PnC charging with contracts from service providers that not derived their certificates from the V2G root.
        /// </summary>
        public static CertificateUse  MORootCertificate               { get; } // Backported!
            = Register("MORootCertificate");

        /// <summary>
        /// Root certificate, used by the CA to sign the Central System and Charge Point certificate.
        /// </summary>
        public static CertificateUse  CentralSystemRootCertificate    { get; }
            = Register("CentralSystemRootCertificate");

        /// <summary>
        /// Root certificate for verification of the Manufacturer certificate.
        /// </summary>
        public static CertificateUse  ManufacturerRootCertificate     { get; }
            = Register("ManufacturerRootCertificate");

        /// <summary>
        /// OEM root certificate for 2-way TLS with the electric vehicle.
        /// </summary>
        public static CertificateUse  OEMRootCertificate              { get; } // Backported!
            = Register("OEMRootCertificate");

        /// <summary>
        /// Root certificate for verification of the NetworkTime certificates.
        /// </summary>
        public static CertificateUse  NetworkTimeRootCertificate      { get; }
            = Register("NetworkTimeRootCertificate");

        #endregion


        #region Operator overloading

        #region Operator == (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateUse CertificateUse1,
                                           CertificateUse CertificateUse2)

            => CertificateUse1.Equals(CertificateUse2);

        #endregion

        #region Operator != (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateUse CertificateUse1,
                                           CertificateUse CertificateUse2)

            => !CertificateUse1.Equals(CertificateUse2);

        #endregion

        #region Operator <  (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateUse CertificateUse1,
                                          CertificateUse CertificateUse2)

            => CertificateUse1.CompareTo(CertificateUse2) < 0;

        #endregion

        #region Operator <= (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateUse CertificateUse1,
                                           CertificateUse CertificateUse2)

            => CertificateUse1.CompareTo(CertificateUse2) <= 0;

        #endregion

        #region Operator >  (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateUse CertificateUse1,
                                          CertificateUse CertificateUse2)

            => CertificateUse1.CompareTo(CertificateUse2) > 0;

        #endregion

        #region Operator >= (CertificateUse1, CertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateUse1">A certificate use.</param>
        /// <param name="CertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateUse CertificateUse1,
                                           CertificateUse CertificateUse2)

            => CertificateUse1.CompareTo(CertificateUse2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateUse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two install certificate uses.
        /// </summary>
        /// <param name="Object">A certificate use to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateUse certificateUse
                   ? CompareTo(certificateUse)
                   : throw new ArgumentException("The given object is not a certificate use!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateUse)

        /// <summary>
        /// Compares two install certificate uses.
        /// </summary>
        /// <param name="CertificateUse">A certificate use to compare with.</param>
        public Int32 CompareTo(CertificateUse CertificateUse)

            => String.Compare(InternalId,
                              CertificateUse.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CertificateUse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate uses for equality.
        /// </summary>
        /// <param name="Object">A certificate use to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateUse certificateUse &&
                   Equals(certificateUse);

        #endregion

        #region Equals(CertificateUse)

        /// <summary>
        /// Compares two install certificate uses for equality.
        /// </summary>
        /// <param name="CertificateUse">A certificate use to compare with.</param>
        public Boolean Equals(CertificateUse CertificateUse)

            => String.Equals(InternalId,
                             CertificateUse.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
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
