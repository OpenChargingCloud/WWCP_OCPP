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
    /// Extension methods for Preconditioning status.
    /// </summary>
    public static class PreconditioningStatusExtensions
    {

        /// <summary>
        /// Indicates whether this Preconditioning status is null or empty.
        /// </summary>
        /// <param name="PreconditioningStatus">A Preconditioning status.</param>
        public static Boolean IsNullOrEmpty(this PreconditioningStatus? PreconditioningStatus)
            => !PreconditioningStatus.HasValue || PreconditioningStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this Preconditioning status is null or empty.
        /// </summary>
        /// <param name="PreconditioningStatus">A Preconditioning status.</param>
        public static Boolean IsNotNullOrEmpty(this PreconditioningStatus? PreconditioningStatus)
            => PreconditioningStatus.HasValue && PreconditioningStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Preconditioning status.
    /// </summary>
    public readonly struct PreconditioningStatus : IId,
                                                   IEquatable<PreconditioningStatus>,
                                                   IComparable<PreconditioningStatus>
    {

        #region Data

        private readonly static Dictionary<String, PreconditioningStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this Preconditioning status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this Preconditioning status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the Preconditioning status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Preconditioning status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a Preconditioning status.</param>
        private PreconditioningStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PreconditioningStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PreconditioningStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a Preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a Preconditioning status.</param>
        public static PreconditioningStatus Parse(String Text)
        {

            if (TryParse(Text, out var PreconditioningStatus))
                return PreconditioningStatus;

            throw new ArgumentException($"Invalid text representation of a Preconditioning status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a Preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a Preconditioning status.</param>
        public static PreconditioningStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var PreconditioningStatus))
                return PreconditioningStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PreconditioningStatus)

        /// <summary>
        /// Try to parse the given text as a Preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a Preconditioning status.</param>
        /// <param name="PreconditioningStatus">The parsed Preconditioning status.</param>
        public static Boolean TryParse(String Text, out PreconditioningStatus PreconditioningStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PreconditioningStatus))
                    PreconditioningStatus = Register(Text);

                return true;

            }

            PreconditioningStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this Preconditioning status.
        /// </summary>
        public PreconditioningStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// No information available on the status of preconditioning.
        /// </summary>
        public static PreconditioningStatus  Unknown            { get; }
            = Register("Unknown");

        /// <summary>
        /// The battery is preconditioned and ready to react directly on a
        /// given setpoint for charging (and discharging when available).
        /// </summary>
        public static PreconditioningStatus  Ready              { get; }
            = Register("Ready");

        /// <summary>
        /// Busy with preconditioning the BMS. When done will move to status Ready.
        /// </summary>
        public static PreconditioningStatus  Preconditioning    { get; }
            = Register("Preconditioning");

        /// <summary>
        /// The battery is not preconditioned and not able to directly react to given setpoint.
        /// </summary>
        public static PreconditioningStatus  NotReady           { get; }
            = Register("NotReady");

        #endregion


        #region Operator overloading

        #region Operator == (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PreconditioningStatus PreconditioningStatus1,
                                           PreconditioningStatus PreconditioningStatus2)

            => PreconditioningStatus1.Equals(PreconditioningStatus2);

        #endregion

        #region Operator != (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PreconditioningStatus PreconditioningStatus1,
                                           PreconditioningStatus PreconditioningStatus2)

            => !PreconditioningStatus1.Equals(PreconditioningStatus2);

        #endregion

        #region Operator <  (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PreconditioningStatus PreconditioningStatus1,
                                          PreconditioningStatus PreconditioningStatus2)

            => PreconditioningStatus1.CompareTo(PreconditioningStatus2) < 0;

        #endregion

        #region Operator <= (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PreconditioningStatus PreconditioningStatus1,
                                           PreconditioningStatus PreconditioningStatus2)

            => PreconditioningStatus1.CompareTo(PreconditioningStatus2) <= 0;

        #endregion

        #region Operator >  (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PreconditioningStatus PreconditioningStatus1,
                                          PreconditioningStatus PreconditioningStatus2)

            => PreconditioningStatus1.CompareTo(PreconditioningStatus2) > 0;

        #endregion

        #region Operator >= (PreconditioningStatus1, PreconditioningStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PreconditioningStatus1">A Preconditioning status.</param>
        /// <param name="PreconditioningStatus2">Another Preconditioning status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PreconditioningStatus PreconditioningStatus1,
                                           PreconditioningStatus PreconditioningStatus2)

            => PreconditioningStatus1.CompareTo(PreconditioningStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<PreconditioningStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Preconditioning status.
        /// </summary>
        /// <param name="Object">A Preconditioning status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PreconditioningStatus PreconditioningStatus
                   ? CompareTo(PreconditioningStatus)
                   : throw new ArgumentException("The given object is not a Preconditioning status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PreconditioningStatus)

        /// <summary>
        /// Compares two Preconditioning status.
        /// </summary>
        /// <param name="PreconditioningStatus">A Preconditioning status to compare with.</param>
        public Int32 CompareTo(PreconditioningStatus PreconditioningStatus)

            => String.Compare(InternalId,
                              PreconditioningStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PreconditioningStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Preconditioning status for equality.
        /// </summary>
        /// <param name="Object">A Preconditioning status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PreconditioningStatus PreconditioningStatus &&
                   Equals(PreconditioningStatus);

        #endregion

        #region Equals(PreconditioningStatus)

        /// <summary>
        /// Compares two Preconditioning status for equality.
        /// </summary>
        /// <param name="PreconditioningStatus">A Preconditioning status to compare with.</param>
        public Boolean Equals(PreconditioningStatus PreconditioningStatus)

            => String.Equals(InternalId,
                             PreconditioningStatus.InternalId,
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
