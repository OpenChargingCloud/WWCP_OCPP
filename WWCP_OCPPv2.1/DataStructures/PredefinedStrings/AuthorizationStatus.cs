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
    /// Extension methods for authorization status.
    /// </summary>
    public static class AuthorizationStatusExtensions
    {

        /// <summary>
        /// Indicates whether this authorization status is null or empty.
        /// </summary>
        /// <param name="AuthorizationStatus">An authorization status.</param>
        public static Boolean IsNullOrEmpty(this AuthorizationStatus? AuthorizationStatus)
            => !AuthorizationStatus.HasValue || AuthorizationStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this authorization status is null or empty.
        /// </summary>
        /// <param name="AuthorizationStatus">An authorization status.</param>
        public static Boolean IsNotNullOrEmpty(this AuthorizationStatus? AuthorizationStatus)
            => AuthorizationStatus.HasValue && AuthorizationStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An authorization status.
    /// </summary>
    public readonly struct AuthorizationStatus : IId,
                                                 IEquatable<AuthorizationStatus>,
                                                 IComparable<AuthorizationStatus>
    {

        #region Data

        private readonly static Dictionary<String, AuthorizationStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                       InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this authorization status is null or empty.
        /// </summary>
        public readonly  Boolean                           IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this authorization status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                           IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the authorization status.
        /// </summary>
        public readonly  UInt64                            Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered authorization status.
        /// </summary>
        public static    IEnumerable<AuthorizationStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an authorization status.</param>
        private AuthorizationStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static AuthorizationStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new AuthorizationStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        public static AuthorizationStatus Parse(String Text)
        {

            if (TryParse(Text, out var authorizationStatus))
                return authorizationStatus;

            throw new ArgumentException($"Invalid text representation of an authorization status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        public static AuthorizationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var authorizationStatus))
                return authorizationStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthorizationStatus)

        /// <summary>
        /// Try to parse the given text as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        /// <param name="AuthorizationStatus">The parsed authorization status.</param>
        public static Boolean TryParse(String Text, out AuthorizationStatus AuthorizationStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out AuthorizationStatus))
                    AuthorizationStatus = Register(Text);

                return true;

            }

            AuthorizationStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this authorization status.
        /// </summary>
        public AuthorizationStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Identifier is allowed for charging.
        /// </summary>
        public static AuthorizationStatus  Unknown               { get; }
            = Register("Unknown");

        /// <summary>
        /// Identifier is allowed for charging.
        /// </summary>
        public static AuthorizationStatus  Accepted              { get; }
            = Register("Accepted");

        /// <summary>
        /// Identifier has been blocked. Not allowed for charging.
        /// </summary>
        public static AuthorizationStatus  Blocked               { get; }
            = Register("Blocked");

        /// <summary>
        /// Identifier is already involved in another transaction and multiple transactions are not allowed.
        /// </summary>
        public static AuthorizationStatus  ConcurrentTx          { get; }
            = Register("ConcurrentTx");

        /// <summary>
        /// Identifier has expired. Not allowed for charging.
        /// </summary>
        public static AuthorizationStatus  Expired               { get; }
            = Register("Expired");

        /// <summary>
        /// Identifier is unknown. Not allowed for charging.
        /// </summary>
        public static AuthorizationStatus  Invalid               { get; }
            = Register("DiagnosticsLog");

        /// <summary>
        /// NoCredit
        /// </summary>
        public static AuthorizationStatus  NoCredit              { get; }
            = Register("NoCredit");

        /// <summary>
        /// NotAllowedTypeEVSE
        /// </summary>
        public static AuthorizationStatus  NotAllowedTypeEVSE    { get; }
            = Register("NotAllowedTypeEVSE");

        /// <summary>
        /// NotAtThisLocation
        /// </summary>
        public static AuthorizationStatus  NotAtThisLocation     { get; }
            = Register("NotAtThisLocation");

        /// <summary>
        /// NotAtThisTime
        /// </summary>
        public static AuthorizationStatus  NotAtThisTime         { get; }
            = Register("NotAtThisTime");


        // Not part of OCPP v2.1!

        /// <summary>
        /// Filtered
        /// </summary>
        public static AuthorizationStatus  Filtered              { get; }
            = Register("Filtered");

        /// <summary>
        /// Error
        /// </summary>
        public static AuthorizationStatus  Error                 { get; }
            = Register("Error");

        /// <summary>
        /// RequestError
        /// </summary>
        public static AuthorizationStatus  RequestError          { get; }
            = Register("RequestError");

        /// <summary>
        /// ParsingError
        /// </summary>
        public static AuthorizationStatus  ParsingError          { get; }
            = Register("ParsingError");

        /// <summary>
        /// SignatureError
        /// </summary>
        public static AuthorizationStatus  SignatureError        { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationStatus AuthorizationStatus1,
                                           AuthorizationStatus AuthorizationStatus2)

            => AuthorizationStatus1.Equals(AuthorizationStatus2);

        #endregion

        #region Operator != (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationStatus AuthorizationStatus1,
                                           AuthorizationStatus AuthorizationStatus2)

            => !AuthorizationStatus1.Equals(AuthorizationStatus2);

        #endregion

        #region Operator <  (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AuthorizationStatus AuthorizationStatus1,
                                          AuthorizationStatus AuthorizationStatus2)

            => AuthorizationStatus1.CompareTo(AuthorizationStatus2) < 0;

        #endregion

        #region Operator <= (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AuthorizationStatus AuthorizationStatus1,
                                           AuthorizationStatus AuthorizationStatus2)

            => AuthorizationStatus1.CompareTo(AuthorizationStatus2) <= 0;

        #endregion

        #region Operator >  (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AuthorizationStatus AuthorizationStatus1,
                                          AuthorizationStatus AuthorizationStatus2)

            => AuthorizationStatus1.CompareTo(AuthorizationStatus2) > 0;

        #endregion

        #region Operator >= (AuthorizationStatus1, AuthorizationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationStatus1">An authorization status.</param>
        /// <param name="AuthorizationStatus2">Another authorization status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AuthorizationStatus AuthorizationStatus1,
                                           AuthorizationStatus AuthorizationStatus2)

            => AuthorizationStatus1.CompareTo(AuthorizationStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthorizationStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two authorization status.
        /// </summary>
        /// <param name="Object">An authorization status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AuthorizationStatus authorizationStatus
                   ? CompareTo(authorizationStatus)
                   : throw new ArgumentException("The given object is not an authorization status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthorizationStatus)

        /// <summary>
        /// Compares two authorization status.
        /// </summary>
        /// <param name="AuthorizationStatus">An authorization status to compare with.</param>
        public Int32 CompareTo(AuthorizationStatus AuthorizationStatus)

            => String.Compare(InternalId,
                              AuthorizationStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthorizationStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorization status for equality.
        /// </summary>
        /// <param name="Object">An authorization status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizationStatus authorizationStatus &&
                   Equals(authorizationStatus);

        #endregion

        #region Equals(AuthorizationStatus)

        /// <summary>
        /// Compares two authorization status for equality.
        /// </summary>
        /// <param name="AuthorizationStatus">An authorization status to compare with.</param>
        public Boolean Equals(AuthorizationStatus AuthorizationStatus)

            => String.Equals(InternalId,
                             AuthorizationStatus.InternalId,
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
