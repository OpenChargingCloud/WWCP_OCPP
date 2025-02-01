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
    /// Extension methods for certificate status.
    /// </summary>
    public static class CertificateStatusExtensions
    {

        /// <summary>
        /// Indicates whether this certificate status is null or empty.
        /// </summary>
        /// <param name="CertificateStatus">A certificate status.</param>
        public static Boolean IsNullOrEmpty(this CertificateStatus? CertificateStatus)
            => !CertificateStatus.HasValue || CertificateStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this certificate status is null or empty.
        /// </summary>
        /// <param name="CertificateStatus">A certificate status.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateStatus? CertificateStatus)
            => CertificateStatus.HasValue && CertificateStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A certificate status.
    /// </summary>
    public readonly struct CertificateStatus : IId,
                                               IEquatable<CertificateStatus>,
                                               IComparable<CertificateStatus>
    {

        #region Data

        private readonly static Dictionary<String, CertificateStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                 InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this certificate status is null or empty.
        /// </summary>
        public readonly  Boolean                         IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this certificate status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                         IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the certificate status.
        /// </summary>
        public readonly  UInt64                          Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered log types.
        /// </summary>
        public static    IEnumerable<CertificateStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate status.</param>
        private CertificateStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CertificateStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CertificateStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static CertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var certificateStatus))
                return certificateStatus;

            throw new ArgumentException($"Invalid text representation of a certificate status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static CertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateStatus))
                return certificateStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateStatus)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        /// <param name="CertificateStatus">The parsed certificate status.</param>
        public static Boolean TryParse(String Text, out CertificateStatus CertificateStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CertificateStatus))
                    CertificateStatus = Register(Text);

                return true;

            }

            CertificateStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this certificate status.
        /// </summary>
        public CertificateStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Good
        /// </summary>
        public static CertificateStatus  Good       { get; }
            = Register("Good");

        /// <summary>
        /// Revoked
        /// </summary>
        public static CertificateStatus  Revoked    { get; }
            = Register("Revoked");

        /// <summary>
        /// Unknown
        /// </summary>
        public static CertificateStatus  Unknown    { get; }
            = Register("Unknown");

        /// <summary>
        /// Failed
        /// </summary>
        public static CertificateStatus  Failed     { get; }
            = Register("Failed");

        #endregion


        #region Operator overloading

        #region Operator == (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateStatus CertificateStatus1,
                                           CertificateStatus CertificateStatus2)

            => CertificateStatus1.Equals(CertificateStatus2);

        #endregion

        #region Operator != (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateStatus CertificateStatus1,
                                           CertificateStatus CertificateStatus2)

            => !CertificateStatus1.Equals(CertificateStatus2);

        #endregion

        #region Operator <  (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateStatus CertificateStatus1,
                                          CertificateStatus CertificateStatus2)

            => CertificateStatus1.CompareTo(CertificateStatus2) < 0;

        #endregion

        #region Operator <= (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateStatus CertificateStatus1,
                                           CertificateStatus CertificateStatus2)

            => CertificateStatus1.CompareTo(CertificateStatus2) <= 0;

        #endregion

        #region Operator >  (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateStatus CertificateStatus1,
                                          CertificateStatus CertificateStatus2)

            => CertificateStatus1.CompareTo(CertificateStatus2) > 0;

        #endregion

        #region Operator >= (CertificateStatus1, CertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatus1">A certificate status.</param>
        /// <param name="CertificateStatus2">Another certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateStatus CertificateStatus1,
                                           CertificateStatus CertificateStatus2)

            => CertificateStatus1.CompareTo(CertificateStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two certificate status.
        /// </summary>
        /// <param name="Object">A certificate status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateStatus certificateStatus
                   ? CompareTo(certificateStatus)
                   : throw new ArgumentException("The given object is not a certificate status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateStatus)

        /// <summary>
        /// Compares two certificate status.
        /// </summary>
        /// <param name="CertificateStatus">A certificate status to compare with.</param>
        public Int32 CompareTo(CertificateStatus CertificateStatus)

            => String.Compare(InternalId,
                              CertificateStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CertificateStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate status for equality.
        /// </summary>
        /// <param name="Object">A certificate status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateStatus certificateStatus &&
                   Equals(certificateStatus);

        #endregion

        #region Equals(CertificateStatus)

        /// <summary>
        /// Compares two certificate status for equality.
        /// </summary>
        /// <param name="CertificateStatus">A certificate status to compare with.</param>
        public Boolean Equals(CertificateStatus CertificateStatus)

            => String.Equals(InternalId,
                             CertificateStatus.InternalId,
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
