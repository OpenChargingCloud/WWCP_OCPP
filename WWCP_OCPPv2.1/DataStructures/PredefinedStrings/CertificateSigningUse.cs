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
    /// Extension methods for certificate signing uses.
    /// </summary>
    public static class CertificateSigningUseExtensions
    {

        /// <summary>
        /// Indicates whether this certificate signing use is null or empty.
        /// </summary>
        /// <param name="CertificateSigningUse">A certificate signing use.</param>
        public static Boolean IsNullOrEmpty(this CertificateSigningUse? CertificateSigningUse)
            => !CertificateSigningUse.HasValue || CertificateSigningUse.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this certificate signing use is null or empty.
        /// </summary>
        /// <param name="CertificateSigningUse">A certificate signing use.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateSigningUse? CertificateSigningUse)
            => CertificateSigningUse.HasValue && CertificateSigningUse.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A certificate signing use.
    /// </summary>
    public readonly struct CertificateSigningUse : IId,
                                                   IEquatable<CertificateSigningUse>,
                                                   IComparable<CertificateSigningUse>
    {

        #region Data

        private readonly static Dictionary<String, CertificateSigningUse>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this certificate signing use is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this certificate signing use is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the certificate signing use.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate signing use based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate signing use.</param>
        private CertificateSigningUse(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CertificateSigningUse Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CertificateSigningUse(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        public static CertificateSigningUse Parse(String Text)
        {

            if (TryParse(Text, out var certificateSigningUse))
                return certificateSigningUse;

            throw new ArgumentException($"Invalid text representation of a certificate signing use: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        public static CertificateSigningUse? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateSigningUse))
                return certificateSigningUse;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateSigningUse)

        /// <summary>
        /// Try to parse the given text as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        /// <param name="CertificateSigningUse">The parsed certificate signing use.</param>
        public static Boolean TryParse(String Text, out CertificateSigningUse CertificateSigningUse)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CertificateSigningUse))
                    CertificateSigningUse = Register(Text);

                return true;

            }

            CertificateSigningUse = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this certificate signing use.
        /// </summary>
        public CertificateSigningUse Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Client side certificate used by the Charging Station to connect the the CSMS.
        /// </summary>
        public static CertificateSigningUse ChargingStationCertificate    { get; }
            = Register("ChargingStationCertificate");

        /// <summary>
        /// Use for certificate for 15118 connections.
        /// This means that the certificate should be derived from the V2G root.
        /// </summary>
        public static CertificateSigningUse V2GCertificate                { get; }
            = Register("V2GCertificate");

        #endregion


        #region Operator overloading

        #region Operator == (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateSigningUse CertificateSigningUse1,
                                           CertificateSigningUse CertificateSigningUse2)

            => CertificateSigningUse1.Equals(CertificateSigningUse2);

        #endregion

        #region Operator != (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateSigningUse CertificateSigningUse1,
                                           CertificateSigningUse CertificateSigningUse2)

            => !CertificateSigningUse1.Equals(CertificateSigningUse2);

        #endregion

        #region Operator <  (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateSigningUse CertificateSigningUse1,
                                          CertificateSigningUse CertificateSigningUse2)

            => CertificateSigningUse1.CompareTo(CertificateSigningUse2) < 0;

        #endregion

        #region Operator <= (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateSigningUse CertificateSigningUse1,
                                           CertificateSigningUse CertificateSigningUse2)

            => CertificateSigningUse1.CompareTo(CertificateSigningUse2) <= 0;

        #endregion

        #region Operator >  (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateSigningUse CertificateSigningUse1,
                                          CertificateSigningUse CertificateSigningUse2)

            => CertificateSigningUse1.CompareTo(CertificateSigningUse2) > 0;

        #endregion

        #region Operator >= (CertificateSigningUse1, CertificateSigningUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateSigningUse1">A certificate signing use.</param>
        /// <param name="CertificateSigningUse2">Another certificate signing use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateSigningUse CertificateSigningUse1,
                                           CertificateSigningUse CertificateSigningUse2)

            => CertificateSigningUse1.CompareTo(CertificateSigningUse2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateSigningUse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two certificate signing uses.
        /// </summary>
        /// <param name="Object">A certificate signing use to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateSigningUse certificateSigningUse
                   ? CompareTo(certificateSigningUse)
                   : throw new ArgumentException("The given object is not a certificate signing use!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateSigningUse)

        /// <summary>
        /// Compares two certificate signing uses.
        /// </summary>
        /// <param name="CertificateSigningUse">A certificate signing use to compare with.</param>
        public Int32 CompareTo(CertificateSigningUse CertificateSigningUse)

            => String.Compare(InternalId,
                              CertificateSigningUse.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CertificateSigningUse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate signing uses for equality.
        /// </summary>
        /// <param name="Object">A certificate signing use to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateSigningUse certificateSigningUse &&
                   Equals(certificateSigningUse);

        #endregion

        #region Equals(CertificateSigningUse)

        /// <summary>
        /// Compares two certificate signing uses for equality.
        /// </summary>
        /// <param name="CertificateSigningUse">A certificate signing use to compare with.</param>
        public Boolean Equals(CertificateSigningUse CertificateSigningUse)

            => String.Equals(InternalId,
                             CertificateSigningUse.InternalId,
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
