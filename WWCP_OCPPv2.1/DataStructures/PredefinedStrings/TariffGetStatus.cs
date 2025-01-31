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
    /// Extension methods for TariffGet status.
    /// </summary>
    public static class TariffGetStatusExtensions
    {

        /// <summary>
        /// Indicates whether this TariffGet status is null or empty.
        /// </summary>
        /// <param name="TariffGetStatus">A TariffGet status.</param>
        public static Boolean IsNullOrEmpty(this TariffGetStatus? TariffGetStatus)
            => !TariffGetStatus.HasValue || TariffGetStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this TariffGet status is null or empty.
        /// </summary>
        /// <param name="TariffGetStatus">A TariffGet status.</param>
        public static Boolean IsNotNullOrEmpty(this TariffGetStatus? TariffGetStatus)
            => TariffGetStatus.HasValue && TariffGetStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A TariffGet status.
    /// </summary>
    public readonly struct TariffGetStatus : IId,
                                                IEquatable<TariffGetStatus>,
                                                IComparable<TariffGetStatus>
    {

        #region Data

        private readonly static Dictionary<String, TariffGetStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this TariffGet status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this TariffGet status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the TariffGet status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffGet status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a TariffGet status.</param>
        private TariffGetStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TariffGetStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TariffGetStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a TariffGet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffGet status.</param>
        public static TariffGetStatus Parse(String Text)
        {

            if (TryParse(Text, out var tariffGetStatus))
                return tariffGetStatus;

            throw new ArgumentException($"Invalid text representation of a TariffGet status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a TariffGet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffGet status.</param>
        public static TariffGetStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffGetStatus))
                return tariffGetStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffGetStatus)

        /// <summary>
        /// Try to parse the given text as a TariffGet status.
        /// </summary>
        /// <param name="Text">A text representation of a TariffGet status.</param>
        /// <param name="TariffGetStatus">The parsed TariffGet status.</param>
        public static Boolean TryParse(String Text, out TariffGetStatus TariffGetStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TariffGetStatus))
                    TariffGetStatus = Register(Text);

                return true;

            }

            TariffGetStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffGetStatus.
        /// </summary>
        public TariffGetStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static TariffGetStatus  Accepted    { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static TariffGetStatus  Rejected    { get; }
            = Register("Rejected");

        /// <summary>
        /// NoTariff
        /// </summary>
        public static TariffGetStatus  NoTariff    { get; }
            = Register("NoTariff");

        #endregion


        #region Operator overloading

        #region Operator == (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffGetStatus TariffGetStatus1,
                                           TariffGetStatus TariffGetStatus2)

            => TariffGetStatus1.Equals(TariffGetStatus2);

        #endregion

        #region Operator != (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffGetStatus TariffGetStatus1,
                                           TariffGetStatus TariffGetStatus2)

            => !TariffGetStatus1.Equals(TariffGetStatus2);

        #endregion

        #region Operator <  (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffGetStatus TariffGetStatus1,
                                          TariffGetStatus TariffGetStatus2)

            => TariffGetStatus1.CompareTo(TariffGetStatus2) < 0;

        #endregion

        #region Operator <= (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffGetStatus TariffGetStatus1,
                                           TariffGetStatus TariffGetStatus2)

            => TariffGetStatus1.CompareTo(TariffGetStatus2) <= 0;

        #endregion

        #region Operator >  (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffGetStatus TariffGetStatus1,
                                          TariffGetStatus TariffGetStatus2)

            => TariffGetStatus1.CompareTo(TariffGetStatus2) > 0;

        #endregion

        #region Operator >= (TariffGetStatus1, TariffGetStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffGetStatus1">A TariffGet status.</param>
        /// <param name="TariffGetStatus2">Another TariffGet status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffGetStatus TariffGetStatus1,
                                           TariffGetStatus TariffGetStatus2)

            => TariffGetStatus1.CompareTo(TariffGetStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffGetStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TariffGet status.
        /// </summary>
        /// <param name="Object">A TariffGet status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffGetStatus TariffGetStatus
                   ? CompareTo(TariffGetStatus)
                   : throw new ArgumentException("The given object is not a TariffGet status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffGetStatus)

        /// <summary>
        /// Compares two TariffGet status.
        /// </summary>
        /// <param name="TariffGetStatus">A TariffGet status to compare with.</param>
        public Int32 CompareTo(TariffGetStatus TariffGetStatus)

            => String.Compare(InternalId,
                              TariffGetStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffGetStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffGet status for equality.
        /// </summary>
        /// <param name="Object">A TariffGet status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffGetStatus TariffGetStatus &&
                   Equals(TariffGetStatus);

        #endregion

        #region Equals(TariffGetStatus)

        /// <summary>
        /// Compares two TariffGet status for equality.
        /// </summary>
        /// <param name="TariffGetStatus">A TariffGet status to compare with.</param>
        public Boolean Equals(TariffGetStatus TariffGetStatus)

            => String.Equals(InternalId,
                             TariffGetStatus.InternalId,
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
