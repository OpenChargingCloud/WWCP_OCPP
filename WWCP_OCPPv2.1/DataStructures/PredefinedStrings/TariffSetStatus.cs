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
    /// Extension methods for TariffSet status.
    /// </summary>
    public static class TariffSetStatusExtensions
    {

        /// <summary>
        /// Indicates whether this TariffSet status is null or empty.
        /// </summary>
        /// <param name="TariffSetStatus">A TariffSet status.</param>
        public static Boolean IsNullOrEmpty(this TariffSetStatus? TariffSetStatus)
            => !TariffSetStatus.HasValue || TariffSetStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this TariffSet status is null or empty.
        /// </summary>
        /// <param name="TariffSetStatus">A TariffSet status.</param>
        public static Boolean IsNotNullOrEmpty(this TariffSetStatus? TariffSetStatus)
            => TariffSetStatus.HasValue && TariffSetStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A TariffSet status.
    /// </summary>
    public readonly struct TariffSetStatus : IId,
                                                IEquatable<TariffSetStatus>,
                                                IComparable<TariffSetStatus>
    {

        #region Data

        private readonly static Dictionary<String, TariffSetStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this TariffSet status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this TariffSet status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the TariffSet status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffSet status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a TariffSet status.</param>
        private TariffSetStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TariffSetStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TariffSetStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a TariffSet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffSet status.</param>
        public static TariffSetStatus Parse(String Text)
        {

            if (TryParse(Text, out var tariffGetStatus))
                return tariffGetStatus;

            throw new ArgumentException($"Invalid text representation of a TariffSet status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a TariffSet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffSet status.</param>
        public static TariffSetStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffGetStatus))
                return tariffGetStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffSetStatus)

        /// <summary>
        /// Try to parse the given text as a TariffSet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffSet status.</param>
        /// <param name="TariffSetStatus">The parsed TariffSet status.</param>
        public static Boolean TryParse(String Text, out TariffSetStatus TariffSetStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TariffSetStatus))
                    TariffSetStatus = Register(Text);

                return true;

            }

            TariffSetStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffSetStatus.
        /// </summary>
        public TariffSetStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static TariffSetStatus  Accepted                 { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static TariffSetStatus  Rejected                 { get; }
            = Register("Rejected");

        /// <summary>
        /// Too many elements
        /// </summary>
        public static TariffSetStatus  TooManyElements          { get; }
            = Register("TooManyElements");

        /// <summary>
        /// Condition not supported
        /// </summary>
        public static TariffSetStatus  ConditionNotSupported    { get; }
            = Register("ConditionNotSupported");

        /// <summary>
        /// Duplicate tariff identification
        /// </summary>
        public static TariffSetStatus  DuplicateTariffId        { get; }
            = Register("DuplicateTariffId");

        #endregion


        #region Operator overloading

        #region Operator == (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffSetStatus TariffSetStatus1,
                                           TariffSetStatus TariffSetStatus2)

            => TariffSetStatus1.Equals(TariffSetStatus2);

        #endregion

        #region Operator != (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffSetStatus TariffSetStatus1,
                                           TariffSetStatus TariffSetStatus2)

            => !TariffSetStatus1.Equals(TariffSetStatus2);

        #endregion

        #region Operator <  (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffSetStatus TariffSetStatus1,
                                          TariffSetStatus TariffSetStatus2)

            => TariffSetStatus1.CompareTo(TariffSetStatus2) < 0;

        #endregion

        #region Operator <= (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffSetStatus TariffSetStatus1,
                                           TariffSetStatus TariffSetStatus2)

            => TariffSetStatus1.CompareTo(TariffSetStatus2) <= 0;

        #endregion

        #region Operator >  (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffSetStatus TariffSetStatus1,
                                          TariffSetStatus TariffSetStatus2)

            => TariffSetStatus1.CompareTo(TariffSetStatus2) > 0;

        #endregion

        #region Operator >= (TariffSetStatus1, TariffSetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffSetStatus1">A TariffSet status.</param>
        /// <param name="TariffSetStatus2">Another TariffSet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffSetStatus TariffSetStatus1,
                                           TariffSetStatus TariffSetStatus2)

            => TariffSetStatus1.CompareTo(TariffSetStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffSetStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TariffSet status.
        /// </summary>
        /// <param name="Object">A TariffSet status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffSetStatus TariffSetStatus
                   ? CompareTo(TariffSetStatus)
                   : throw new ArgumentException("The given object is not a TariffSet status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffSetStatus)

        /// <summary>
        /// Compares two TariffSet status.
        /// </summary>
        /// <param name="TariffSetStatus">A TariffSet status to compare with.</param>
        public Int32 CompareTo(TariffSetStatus TariffSetStatus)

            => String.Compare(InternalId,
                              TariffSetStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffSetStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffSet status for equality.
        /// </summary>
        /// <param name="Object">A TariffSet status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffSetStatus TariffSetStatus &&
                   Equals(TariffSetStatus);

        #endregion

        #region Equals(TariffSetStatus)

        /// <summary>
        /// Compares two TariffSet status for equality.
        /// </summary>
        /// <param name="TariffSetStatus">A TariffSet status to compare with.</param>
        public Boolean Equals(TariffSetStatus TariffSetStatus)

            => String.Equals(InternalId,
                             TariffSetStatus.InternalId,
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
