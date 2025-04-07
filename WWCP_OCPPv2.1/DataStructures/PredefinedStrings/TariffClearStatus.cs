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
    /// Extension methods for TariffClear status.
    /// </summary>
    public static class TariffClearStatusExtensions
    {

        /// <summary>
        /// Indicates whether this TariffClear status is null or empty.
        /// </summary>
        /// <param name="TariffClearStatus">A TariffClear status.</param>
        public static Boolean IsNullOrEmpty(this TariffClearStatus? TariffClearStatus)
            => !TariffClearStatus.HasValue || TariffClearStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this TariffClear status is null or empty.
        /// </summary>
        /// <param name="TariffClearStatus">A TariffClear status.</param>
        public static Boolean IsNotNullOrEmpty(this TariffClearStatus? TariffClearStatus)
            => TariffClearStatus.HasValue && TariffClearStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A TariffClear status.
    /// </summary>
    public readonly struct TariffClearStatus : IId,
                                                IEquatable<TariffClearStatus>,
                                                IComparable<TariffClearStatus>
    {

        #region Data

        private readonly static Dictionary<String, TariffClearStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this TariffClear status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this TariffClear status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the TariffClear status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffClear status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a TariffClear status.</param>
        private TariffClearStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TariffClearStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TariffClearStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a TariffClear status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffClear status.</param>
        public static TariffClearStatus Parse(String Text)
        {

            if (TryParse(Text, out var tariffClearStatus))
                return tariffClearStatus;

            throw new ArgumentException($"Invalid text representation of a TariffClear status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a TariffClear status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffClear status.</param>
        public static TariffClearStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffClearStatus))
                return tariffClearStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffClearStatus)

        /// <summary>
        /// Try to parse the given text as a TariffClear status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffClear status.</param>
        /// <param name="TariffClearStatus">The parsed TariffClear status.</param>
        public static Boolean TryParse(String Text, out TariffClearStatus TariffClearStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TariffClearStatus))
                    TariffClearStatus = Register(Text);

                return true;

            }

            TariffClearStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffClearStatus.
        /// </summary>
        public TariffClearStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static TariffClearStatus  Accepted    { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static TariffClearStatus  Rejected    { get; }
            = Register("Rejected");

        /// <summary>
        /// NoTariff
        /// </summary>
        public static TariffClearStatus  NoTariff    { get; }
            = Register("NoTariff");

        #endregion


        #region Operator overloading

        #region Operator == (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffClearStatus TariffClearStatus1,
                                           TariffClearStatus TariffClearStatus2)

            => TariffClearStatus1.Equals(TariffClearStatus2);

        #endregion

        #region Operator != (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffClearStatus TariffClearStatus1,
                                           TariffClearStatus TariffClearStatus2)

            => !TariffClearStatus1.Equals(TariffClearStatus2);

        #endregion

        #region Operator <  (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffClearStatus TariffClearStatus1,
                                          TariffClearStatus TariffClearStatus2)

            => TariffClearStatus1.CompareTo(TariffClearStatus2) < 0;

        #endregion

        #region Operator <= (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffClearStatus TariffClearStatus1,
                                           TariffClearStatus TariffClearStatus2)

            => TariffClearStatus1.CompareTo(TariffClearStatus2) <= 0;

        #endregion

        #region Operator >  (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffClearStatus TariffClearStatus1,
                                          TariffClearStatus TariffClearStatus2)

            => TariffClearStatus1.CompareTo(TariffClearStatus2) > 0;

        #endregion

        #region Operator >= (TariffClearStatus1, TariffClearStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffClearStatus1">A TariffClear status.</param>
        /// <param name="TariffClearStatus2">Another TariffClear status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffClearStatus TariffClearStatus1,
                                           TariffClearStatus TariffClearStatus2)

            => TariffClearStatus1.CompareTo(TariffClearStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffClearStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TariffClear status.
        /// </summary>
        /// <param name="Object">A TariffClear status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffClearStatus TariffClearStatus
                   ? CompareTo(TariffClearStatus)
                   : throw new ArgumentException("The given object is not a TariffClear status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffClearStatus)

        /// <summary>
        /// Compares two TariffClear status.
        /// </summary>
        /// <param name="TariffClearStatus">A TariffClear status to compare with.</param>
        public Int32 CompareTo(TariffClearStatus TariffClearStatus)

            => String.Compare(InternalId,
                              TariffClearStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffClearStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffClear status for equality.
        /// </summary>
        /// <param name="Object">A TariffClear status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffClearStatus TariffClearStatus &&
                   Equals(TariffClearStatus);

        #endregion

        #region Equals(TariffClearStatus)

        /// <summary>
        /// Compares two TariffClear status for equality.
        /// </summary>
        /// <param name="TariffClearStatus">A TariffClear status to compare with.</param>
        public Boolean Equals(TariffClearStatus TariffClearStatus)

            => String.Equals(InternalId,
                             TariffClearStatus.InternalId,
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
