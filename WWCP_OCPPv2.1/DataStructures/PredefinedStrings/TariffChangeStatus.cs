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
    /// Extension methods for TariffChange status.
    /// </summary>
    public static class TariffChangeStatusExtensions
    {

        /// <summary>
        /// Indicates whether this TariffChange status is null or empty.
        /// </summary>
        /// <param name="TariffChangeStatus">A TariffChange status.</param>
        public static Boolean IsNullOrEmpty(this TariffChangeStatus? TariffChangeStatus)
            => !TariffChangeStatus.HasValue || TariffChangeStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this TariffChange status is null or empty.
        /// </summary>
        /// <param name="TariffChangeStatus">A TariffChange status.</param>
        public static Boolean IsNotNullOrEmpty(this TariffChangeStatus? TariffChangeStatus)
            => TariffChangeStatus.HasValue && TariffChangeStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A TariffChange status.
    /// </summary>
    public readonly struct TariffChangeStatus : IId,
                                                IEquatable<TariffChangeStatus>,
                                                IComparable<TariffChangeStatus>
    {

        #region Data

        private readonly static Dictionary<String, TariffChangeStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this TariffChange status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this TariffChange status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the TariffChange status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffChange status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a TariffChange status.</param>
        private TariffChangeStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TariffChangeStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TariffChangeStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a TariffChange status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffChange status.</param>
        public static TariffChangeStatus Parse(String Text)
        {

            if (TryParse(Text, out var tariffChangeStatus))
                return tariffChangeStatus;

            throw new ArgumentException($"Invalid text representation of a TariffChange status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a TariffChange status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffChange status.</param>
        public static TariffChangeStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffChangeStatus))
                return tariffChangeStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffChangeStatus)

        /// <summary>
        /// Try to parse the given text as a TariffChange status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffChange status.</param>
        /// <param name="TariffChangeStatus">The parsed TariffChange status.</param>
        public static Boolean TryParse(String Text, out TariffChangeStatus TariffChangeStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TariffChangeStatus))
                    TariffChangeStatus = Register(Text);

                return true;

            }

            TariffChangeStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffChangeStatus.
        /// </summary>
        public TariffChangeStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static TariffChangeStatus  Accepted                 { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static TariffChangeStatus  Rejected                 { get; }
            = Register("Rejected");

        /// <summary>
        /// Too many elements
        /// </summary>
        public static TariffChangeStatus  TooManyElements          { get; }
            = Register("TooManyElements");

        /// <summary>
        /// Condition not supported
        /// </summary>
        public static TariffChangeStatus  ConditionNotSupported    { get; }
            = Register("ConditionNotSupported");

        /// <summary>
        /// Tx not found
        /// </summary>
        public static TariffChangeStatus  TxNotFound               { get; }
            = Register("TxNotFound");

        /// <summary>
        /// No currency change
        /// </summary>
        public static TariffChangeStatus  NoCurrencyChange         { get; }
            = Register("NoCurrencyChange");

        #endregion


        #region Operator overloading

        #region Operator == (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffChangeStatus TariffChangeStatus1,
                                           TariffChangeStatus TariffChangeStatus2)

            => TariffChangeStatus1.Equals(TariffChangeStatus2);

        #endregion

        #region Operator != (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffChangeStatus TariffChangeStatus1,
                                           TariffChangeStatus TariffChangeStatus2)

            => !TariffChangeStatus1.Equals(TariffChangeStatus2);

        #endregion

        #region Operator <  (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffChangeStatus TariffChangeStatus1,
                                          TariffChangeStatus TariffChangeStatus2)

            => TariffChangeStatus1.CompareTo(TariffChangeStatus2) < 0;

        #endregion

        #region Operator <= (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffChangeStatus TariffChangeStatus1,
                                           TariffChangeStatus TariffChangeStatus2)

            => TariffChangeStatus1.CompareTo(TariffChangeStatus2) <= 0;

        #endregion

        #region Operator >  (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffChangeStatus TariffChangeStatus1,
                                          TariffChangeStatus TariffChangeStatus2)

            => TariffChangeStatus1.CompareTo(TariffChangeStatus2) > 0;

        #endregion

        #region Operator >= (TariffChangeStatus1, TariffChangeStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffChangeStatus1">A TariffChange status.</param>
        /// <param name="TariffChangeStatus2">Another TariffChange status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffChangeStatus TariffChangeStatus1,
                                           TariffChangeStatus TariffChangeStatus2)

            => TariffChangeStatus1.CompareTo(TariffChangeStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffChangeStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TariffChange status.
        /// </summary>
        /// <param name="Object">A TariffChange status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffChangeStatus TariffChangeStatus
                   ? CompareTo(TariffChangeStatus)
                   : throw new ArgumentException("The given object is not a TariffChange status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffChangeStatus)

        /// <summary>
        /// Compares two TariffChange status.
        /// </summary>
        /// <param name="TariffChangeStatus">A TariffChange status to compare with.</param>
        public Int32 CompareTo(TariffChangeStatus TariffChangeStatus)

            => String.Compare(InternalId,
                              TariffChangeStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffChangeStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffChange status for equality.
        /// </summary>
        /// <param name="Object">A TariffChange status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffChangeStatus TariffChangeStatus &&
                   Equals(TariffChangeStatus);

        #endregion

        #region Equals(TariffChangeStatus)

        /// <summary>
        /// Compares two TariffChange status for equality.
        /// </summary>
        /// <param name="TariffChangeStatus">A TariffChange status to compare with.</param>
        public Boolean Equals(TariffChangeStatus TariffChangeStatus)

            => String.Equals(InternalId,
                             TariffChangeStatus.InternalId,
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
