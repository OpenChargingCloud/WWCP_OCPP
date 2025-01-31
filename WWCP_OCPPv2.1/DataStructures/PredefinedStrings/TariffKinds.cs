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
    /// Extension methods for tariff kinds.
    /// </summary>
    public static class TariffKindExtensions
    {

        /// <summary>
        /// Indicates whether this tariff kind is null or empty.
        /// </summary>
        /// <param name="TariffKind">A tariff kind.</param>
        public static Boolean IsNullOrEmpty(this TariffKind? TariffKind)
            => !TariffKind.HasValue || TariffKind.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff kind is null or empty.
        /// </summary>
        /// <param name="TariffKind">A tariff kind.</param>
        public static Boolean IsNotNullOrEmpty(this TariffKind? TariffKind)
            => TariffKind.HasValue && TariffKind.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A tariff kind.
    /// </summary>
    public readonly struct TariffKind : IId,
                                        IEquatable<TariffKind>,
                                        IComparable<TariffKind>
    {

        #region Data

        private readonly static Dictionary<String, TariffKind>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff kind is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff kind is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff kind.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff kind based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff kind.</param>
        private TariffKind(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TariffKind Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TariffKind(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        public static TariffKind Parse(String Text)
        {

            if (TryParse(Text, out var tariffKind))
                return tariffKind;

            throw new ArgumentException("The given text representation of a tariff kind is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        public static TariffKind? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffKind))
                return tariffKind;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffKind)

        /// <summary>
        /// Try to parse the given text as tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        /// <param name="TariffKind">The parsed tariff kind.</param>
        public static Boolean TryParse(String Text, out TariffKind TariffKind)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TariffKind))
                    TariffKind = Register(Text);

                return true;

            }

            TariffKind = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff kind.
        /// </summary>
        public TariffKind Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Default Tariff
        /// </summary>
        public static TariffKind  DefaultTariff    { get; }
            = Register("DefaultTariff");

        /// <summary>
        /// Driver Tariff
        /// </summary>
        public static TariffKind  DriverTariff     { get; }
            = Register("DriverTariff");

        #endregion


        #region Operator overloading

        #region Operator == (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffKind TariffKind1,
                                           TariffKind TariffKind2)

            => TariffKind1.Equals(TariffKind2);

        #endregion

        #region Operator != (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffKind TariffKind1,
                                           TariffKind TariffKind2)

            => !TariffKind1.Equals(TariffKind2);

        #endregion

        #region Operator <  (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffKind TariffKind1,
                                          TariffKind TariffKind2)

            => TariffKind1.CompareTo(TariffKind2) < 0;

        #endregion

        #region Operator <= (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffKind TariffKind1,
                                           TariffKind TariffKind2)

            => TariffKind1.CompareTo(TariffKind2) <= 0;

        #endregion

        #region Operator >  (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffKind TariffKind1,
                                          TariffKind TariffKind2)

            => TariffKind1.CompareTo(TariffKind2) > 0;

        #endregion

        #region Operator >= (TariffKind1, TariffKind2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffKind1">A tariff kind.</param>
        /// <param name="TariffKind2">Another tariff kind.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffKind TariffKind1,
                                           TariffKind TariffKind2)

            => TariffKind1.CompareTo(TariffKind2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffKind> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff kinds.
        /// </summary>
        /// <param name="Object">A tariff kind to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffKind tariffKind
                   ? CompareTo(tariffKind)
                   : throw new ArgumentException("The given object is not tariff kind!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffKind)

        /// <summary>
        /// Compares two tariff kinds.
        /// </summary>
        /// <param name="TariffKind">A tariff kind to compare with.</param>
        public Int32 CompareTo(TariffKind TariffKind)

            => String.Compare(InternalId,
                              TariffKind.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffKind> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff kinds for equality.
        /// </summary>
        /// <param name="Object">A tariff kind to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffKind tariffKind &&
                   Equals(tariffKind);

        #endregion

        #region Equals(TariffKind)

        /// <summary>
        /// Compares two tariff kinds for equality.
        /// </summary>
        /// <param name="TariffKind">A tariff kind to compare with.</param>
        public Boolean Equals(TariffKind TariffKind)

            => String.Equals(InternalId,
                             TariffKind.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
