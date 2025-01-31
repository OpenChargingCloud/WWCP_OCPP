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
    /// Extension methods for reset status.
    /// </summary>
    public static class ResetStatusExtensions
    {

        /// <summary>
        /// Indicates whether this reset status is null or empty.
        /// </summary>
        /// <param name="ResetStatus">A reset status.</param>
        public static Boolean IsNullOrEmpty(this ResetStatus? ResetStatus)
            => !ResetStatus.HasValue || ResetStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this reset status is null or empty.
        /// </summary>
        /// <param name="ResetStatus">A reset status.</param>
        public static Boolean IsNotNullOrEmpty(this ResetStatus? ResetStatus)
            => ResetStatus.HasValue && ResetStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A reset status.
    /// </summary>
    public readonly struct ResetStatus : IId,
                                        IEquatable<ResetStatus>,
                                        IComparable<ResetStatus>
    {

        #region Data

        private readonly static Dictionary<String, ResetStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this reset status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this reset status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the reset status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reset status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a reset status.</param>
        private ResetStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ResetStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ResetStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        public static ResetStatus Parse(String Text)
        {

            if (TryParse(Text, out var resetStatus))
                return resetStatus;

            throw new ArgumentException($"Invalid text representation of a reset status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        public static ResetStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var resetStatus))
                return resetStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ResetStatus)

        /// <summary>
        /// Try to parse the given text as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        /// <param name="ResetStatus">The parsed reset status.</param>
        public static Boolean TryParse(String Text, out ResetStatus ResetStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ResetStatus))
                    ResetStatus = Register(Text);

                return true;

            }

            ResetStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this reset status.
        /// </summary>
        public ResetStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Unknown reset status.
        /// </summary>
        public static ResetStatus  Unknown      { get; }
            = Register("Unknown");

        /// <summary>
        /// Command will be executed.
        /// </summary>
        public static ResetStatus  Accepted     { get; }
            = Register("Accepted");

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        public static ResetStatus  Rejected     { get; }
            = Register("Rejected");

        /// <summary>
        /// Reset command is scheduled as the charging station is still busy with a process that cannot be interrupted.
        /// Reset will be executed when this process is finished.
        /// </summary>
        public static ResetStatus  Scheduled    { get; }
            = Register("Scheduled");

        #endregion


        #region Operator overloading

        #region Operator == (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ResetStatus ResetStatus1,
                                           ResetStatus ResetStatus2)

            => ResetStatus1.Equals(ResetStatus2);

        #endregion

        #region Operator != (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ResetStatus ResetStatus1,
                                           ResetStatus ResetStatus2)

            => !ResetStatus1.Equals(ResetStatus2);

        #endregion

        #region Operator <  (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ResetStatus ResetStatus1,
                                          ResetStatus ResetStatus2)

            => ResetStatus1.CompareTo(ResetStatus2) < 0;

        #endregion

        #region Operator <= (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ResetStatus ResetStatus1,
                                           ResetStatus ResetStatus2)

            => ResetStatus1.CompareTo(ResetStatus2) <= 0;

        #endregion

        #region Operator >  (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ResetStatus ResetStatus1,
                                          ResetStatus ResetStatus2)

            => ResetStatus1.CompareTo(ResetStatus2) > 0;

        #endregion

        #region Operator >= (ResetStatus1, ResetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetStatus1">A reset status.</param>
        /// <param name="ResetStatus2">Another reset status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ResetStatus ResetStatus1,
                                           ResetStatus ResetStatus2)

            => ResetStatus1.CompareTo(ResetStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<ResetStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reset status.
        /// </summary>
        /// <param name="Object">A reset status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ResetStatus resetStatus
                   ? CompareTo(resetStatus)
                   : throw new ArgumentException("The given object is not a reset status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ResetStatus)

        /// <summary>
        /// Compares two reset status.
        /// </summary>
        /// <param name="ResetStatus">A reset status to compare with.</param>
        public Int32 CompareTo(ResetStatus ResetStatus)

            => String.Compare(InternalId,
                              ResetStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ResetStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reset status for equality.
        /// </summary>
        /// <param name="Object">A reset status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetStatus resetStatus &&
                   Equals(resetStatus);

        #endregion

        #region Equals(ResetStatus)

        /// <summary>
        /// Compares two reset status for equality.
        /// </summary>
        /// <param name="ResetStatus">A reset status to compare with.</param>
        public Boolean Equals(ResetStatus ResetStatus)

            => String.Equals(InternalId,
                             ResetStatus.InternalId,
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
