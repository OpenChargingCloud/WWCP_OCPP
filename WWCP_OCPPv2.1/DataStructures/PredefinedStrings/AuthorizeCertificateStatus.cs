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
    /// Extension methods for authorize certificate status.
    /// </summary>
    public static class AuthorizeCertificateStatusExtensions
    {

        /// <summary>
        /// Indicates whether this authorize certificate status is null or empty.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus">An authorize certificate status.</param>
        public static Boolean IsNullOrEmpty(this AuthorizeCertificateStatus? AuthorizeCertificateStatus)
            => !AuthorizeCertificateStatus.HasValue || AuthorizeCertificateStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this authorize certificate status is null or empty.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus">An authorize certificate status.</param>
        public static Boolean IsNotNullOrEmpty(this AuthorizeCertificateStatus? AuthorizeCertificateStatus)
            => AuthorizeCertificateStatus.HasValue && AuthorizeCertificateStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An authorize certificate status.
    /// </summary>
    public readonly struct AuthorizeCertificateStatus : IId,
                                                        IEquatable<AuthorizeCertificateStatus>,
                                                        IComparable<AuthorizeCertificateStatus>
    {

        #region Data

        private readonly static Dictionary<String, AuthorizeCertificateStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this authorize certificate status is null or empty.
        /// </summary>
        public readonly  Boolean                                  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this authorize certificate status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                                  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the authorize certificate status.
        /// </summary>
        public readonly  UInt64                                   Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered authorize certificate status.
        /// </summary>
        public static    IEnumerable<AuthorizeCertificateStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorize certificate status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an authorize certificate status.</param>
        private AuthorizeCertificateStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static AuthorizeCertificateStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new AuthorizeCertificateStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        public static AuthorizeCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            throw new ArgumentException($"Invalid text representation of an authorize certificate status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        public static AuthorizeCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthorizeCertificateStatus)

        /// <summary>
        /// Try to parse the given text as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus">The parsed authorize certificate status.</param>
        public static Boolean TryParse(String Text, out AuthorizeCertificateStatus AuthorizeCertificateStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out AuthorizeCertificateStatus))
                    AuthorizeCertificateStatus = Register(Text);

                return true;

            }

            AuthorizeCertificateStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this authorize certificate status.
        /// </summary>
        public AuthorizeCertificateStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static AuthorizeCertificateStatus  Accepted                  { get; }
            = Register("Accepted");

        /// <summary>
        /// Signature Error
        /// </summary>
        public static AuthorizeCertificateStatus  SignatureError            { get; }
            = Register("SignatureError");

        /// <summary>
        /// Certificate Expired
        /// </summary>
        public static AuthorizeCertificateStatus  CertificateExpired        { get; }
            = Register("CertificateExpired");

        /// <summary>
        /// Certificate Revoked
        /// </summary>
        public static AuthorizeCertificateStatus  CertificateRevoked        { get; }
            = Register("CertificateRevoked");

        /// <summary>
        /// NoCertificateAvailable
        /// </summary>
        public static AuthorizeCertificateStatus  NoCertificateAvailable    { get; }
            = Register("NoCertificateAvailable");

        /// <summary>
        /// Cert Chain Error
        /// </summary>
        public static AuthorizeCertificateStatus  CertChainError            { get; }
            = Register("CertChainError");

        /// <summary>
        /// Contract Cancelled
        /// </summary>
        public static AuthorizeCertificateStatus  ContractCancelled         { get; }
            = Register("ContractCancelled");

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                           AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => AuthorizeCertificateStatus1.Equals(AuthorizeCertificateStatus2);

        #endregion

        #region Operator != (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                           AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => !AuthorizeCertificateStatus1.Equals(AuthorizeCertificateStatus2);

        #endregion

        #region Operator <  (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                          AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => AuthorizeCertificateStatus1.CompareTo(AuthorizeCertificateStatus2) < 0;

        #endregion

        #region Operator <= (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                           AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => AuthorizeCertificateStatus1.CompareTo(AuthorizeCertificateStatus2) <= 0;

        #endregion

        #region Operator >  (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                          AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => AuthorizeCertificateStatus1.CompareTo(AuthorizeCertificateStatus2) > 0;

        #endregion

        #region Operator >= (AuthorizeCertificateStatus1, AuthorizeCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus1">An authorize certificate status.</param>
        /// <param name="AuthorizeCertificateStatus2">Another authorize certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizeCertificateStatus AuthorizeCertificateStatus1,
                                           AuthorizeCertificateStatus AuthorizeCertificateStatus2)

            => AuthorizeCertificateStatus1.CompareTo(AuthorizeCertificateStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthorizeCertificateStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two authorize certificate status.
        /// </summary>
        /// <param name="Object">An authorize certificate status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthorizeCertificateStatus bootReason
                   ? CompareTo(bootReason)
                   : throw new ArgumentException("The given object is not an authorize certificate status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizeCertificateStatus)

        /// <summary>
        /// Compares two authorize certificate status.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus">An authorize certificate status to compare with.</param>
        public Int32 CompareTo(AuthorizeCertificateStatus AuthorizeCertificateStatus)

            => String.Compare(InternalId,
                              AuthorizeCertificateStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthorizeCertificateStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorize certificate status for equality.
        /// </summary>
        /// <param name="Object">An authorize certificate status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeCertificateStatus bootReason &&
                   Equals(bootReason);

        #endregion

        #region Equals(AuthorizeCertificateStatus)

        /// <summary>
        /// Compares two authorize certificate status for equality.
        /// </summary>
        /// <param name="AuthorizeCertificateStatus">An authorize certificate status to compare with.</param>
        public Boolean Equals(AuthorizeCertificateStatus AuthorizeCertificateStatus)

            => String.Equals(InternalId,
                             AuthorizeCertificateStatus.InternalId,
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
