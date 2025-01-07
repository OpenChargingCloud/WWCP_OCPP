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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for registration statuss.
    /// </summary>
    public static class RegistrationStatusExtensions
    {

        /// <summary>
        /// Indicates whether this registration status is null or empty.
        /// </summary>
        /// <param name="RegistrationStatus">A registration status.</param>
        public static Boolean IsNullOrEmpty(this RegistrationStatus? RegistrationStatus)
            => !RegistrationStatus.HasValue || RegistrationStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this registration status is null or empty.
        /// </summary>
        /// <param name="RegistrationStatus">A registration status.</param>
        public static Boolean IsNotNullOrEmpty(this RegistrationStatus? RegistrationStatus)
            => RegistrationStatus.HasValue && RegistrationStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A registration status.
    /// </summary>
    public readonly struct RegistrationStatus : IId,
                                                IEquatable<RegistrationStatus>,
                                                IComparable<RegistrationStatus>
    {

        #region Data

        private readonly static Dictionary<String, RegistrationStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this registration status is null or empty.
        /// </summary>
        public readonly Boolean                        IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this registration status is NOT null or empty.
        /// </summary>
        public readonly Boolean                        IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the registration status.
        /// </summary>
        public readonly UInt64                         Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static IEnumerable<RegistrationStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new registration status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a registration status.</param>
        private RegistrationStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static RegistrationStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new RegistrationStatus(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        public static RegistrationStatus Parse(String Text)
        {

            if (TryParse(Text, out var registrationStatus))
                return registrationStatus;

            throw new ArgumentException($"Invalid text representation of a registration status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        public static RegistrationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var registrationStatus))
                return registrationStatus;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out RegistrationStatus)

        /// <summary>
        /// Try to parse the given text as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        /// <param name="RegistrationStatus">The parsed registration status.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out RegistrationStatus  RegistrationStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out RegistrationStatus))
                    RegistrationStatus = Register(Text);

                return true;

            }

            RegistrationStatus = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out RegistrationStatus)

        /// <summary>
        /// Check whether the given text is a defined registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        /// <param name="RegistrationStatus">The validated registration status.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out RegistrationStatus  RegistrationStatus)

            => lookup.TryGetValue(Text.Trim(), out RegistrationStatus);

        #endregion

        #region Clone

        /// <summary>
        /// Clone this registration status.
        /// </summary>
        public RegistrationStatus Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Unknown registration status.
        /// </summary>
        public static RegistrationStatus Unknown      { get; }
            = Register("Unknown");


        /// <summary>
        /// Charge point is accepted by the central system.
        /// </summary>
        public static RegistrationStatus Accepted      { get; }
            = Register("Accepted");

        /// <summary>
        /// The central system is not yet ready to accept the
        /// charging station. The central system may send messages
        /// to retrieve information or prepare the charging station.
        /// </summary>
        public static RegistrationStatus Pending      { get; }
            = Register("Pending");

        /// <summary>
        /// Charge point is not accepted by the central system.
        /// This may happen when the charging station identification
        /// is not (yet) known by the central system.
        /// </summary>
        public static RegistrationStatus Rejected      { get; }
            = Register("Rejected");


        public static RegistrationStatus Error      { get; }
            = Register("Error");

        public static RegistrationStatus SignatureError      { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RegistrationStatus RegistrationStatus1,
                                           RegistrationStatus RegistrationStatus2)

            => RegistrationStatus1.Equals(RegistrationStatus2);

        #endregion

        #region Operator != (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RegistrationStatus RegistrationStatus1,
                                           RegistrationStatus RegistrationStatus2)

            => !RegistrationStatus1.Equals(RegistrationStatus2);

        #endregion

        #region Operator <  (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RegistrationStatus RegistrationStatus1,
                                          RegistrationStatus RegistrationStatus2)

            => RegistrationStatus1.CompareTo(RegistrationStatus2) < 0;

        #endregion

        #region Operator <= (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RegistrationStatus RegistrationStatus1,
                                           RegistrationStatus RegistrationStatus2)

            => RegistrationStatus1.CompareTo(RegistrationStatus2) <= 0;

        #endregion

        #region Operator >  (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RegistrationStatus RegistrationStatus1,
                                          RegistrationStatus RegistrationStatus2)

            => RegistrationStatus1.CompareTo(RegistrationStatus2) > 0;

        #endregion

        #region Operator >= (RegistrationStatus1, RegistrationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegistrationStatus1">A registration status.</param>
        /// <param name="RegistrationStatus2">Another registration status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RegistrationStatus RegistrationStatus1,
                                           RegistrationStatus RegistrationStatus2)

            => RegistrationStatus1.CompareTo(RegistrationStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<RegistrationStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two registration statuss.
        /// </summary>
        /// <param name="Object">A registration status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RegistrationStatus registrationStatus
                   ? CompareTo(registrationStatus)
                   : throw new ArgumentException("The given object is not a registration status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RegistrationStatus)

        /// <summary>
        /// Compares two registration statuss.
        /// </summary>
        /// <param name="RegistrationStatus">A registration status to compare with.</param>
        public Int32 CompareTo(RegistrationStatus RegistrationStatus)

            => String.Compare(InternalId,
                              RegistrationStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RegistrationStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two registration statuss for equality.
        /// </summary>
        /// <param name="Object">A registration status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RegistrationStatus registrationStatus &&
                   Equals(registrationStatus);

        #endregion

        #region Equals(RegistrationStatus)

        /// <summary>
        /// Compares two registration statuss for equality.
        /// </summary>
        /// <param name="RegistrationStatus">A registration status to compare with.</param>
        public Boolean Equals(RegistrationStatus RegistrationStatus)

            => String.Equals(InternalId,
                             RegistrationStatus.InternalId,
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
