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
    /// Extension methods for tariff identifications.
    /// </summary>
    public static class TariffIdExtensions
    {

        /// <summary>
        /// Indicates whether this tariff identification is null or empty.
        /// </summary>
        /// <param name="TariffId">A tariff identification.</param>
        public static Boolean IsNullOrEmpty(this Tariff_Id? TariffId)
            => !TariffId.HasValue || TariffId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff identification is NOT null or empty.
        /// </summary>
        /// <param name="TariffId">A tariff identification.</param>
        public static Boolean IsNotNullOrEmpty(this Tariff_Id? TariffId)
            => TariffId.HasValue && TariffId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tariff.
    /// </summary>
    public readonly struct Tariff_Id : IId<Tariff_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff identification is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff identification is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff identification.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff identification.</param>
        private Tariff_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) New(ProviderId = null, Suffix = null)

        /// <summary>
        /// Create a new (random) tariff identification based
        /// on the given optional e-mobility provider identification and the given suffix.
        /// When the suffix is empty, an UUIDv7 will be used (timestamp + random characters).
        /// </summary>
        /// <param name="ProviderId">An optional e-mobility provider identification as prefix.</param>
        /// <param name="Suffix">The optional suffix to be used.</param>
        public static Tariff_Id New(Provider_Id?  ProviderId   = null,
                                    String?       Suffix       = null)

            => new ($"{ProviderId?.ToString() ?? ""}T{Suffix ?? UUIDv7.Generate().ToString()}");

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff identification.</param>
        public static Tariff_Id Parse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            throw new ArgumentException($"Invalid text representation of a tariff identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff identification.</param>
        public static Tariff_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out TariffId)

        /// <summary>
        /// Try to parse the given text as a tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff identification.</param>
        /// <param name="TariffId">The parsed tariff identification.</param>
        public static Boolean TryParse(String Text, out Tariff_Id TariffId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffId = new Tariff_Id(Text);
                    return true;
                }
                catch
                { }
            }

            TariffId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff identification.
        /// </summary>
        public Tariff_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => TariffId1.Equals(TariffId2);

        #endregion

        #region Operator != (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => !TariffId1.Equals(TariffId2);

        #endregion

        #region Operator <  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff_Id TariffId1,
                                          Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) < 0;

        #endregion

        #region Operator <= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) <= 0;

        #endregion

        #region Operator >  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff_Id TariffId1,
                                          Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) > 0;

        #endregion

        #region Operator >= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A tariff identification.</param>
        /// <param name="TariffId2">Another tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff identifications.
        /// </summary>
        /// <param name="Object">A tariff identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tariff_Id tariffId
                   ? CompareTo(tariffId)
                   : throw new ArgumentException("The given object is not a tariff identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffId)

        /// <summary>
        /// Compares two tariff identifications.
        /// </summary>
        /// <param name="TariffId">A tariff identification to compare with.</param>
        public Int32 CompareTo(Tariff_Id TariffId)

            => String.Compare(InternalId,
                              TariffId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff identifications for equality.
        /// </summary>
        /// <param name="Object">A tariff identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tariff_Id tariffId &&
                   Equals(tariffId);

        #endregion

        #region Equals(TariffId)

        /// <summary>
        /// Compares two tariff identifications for equality.
        /// </summary>
        /// <param name="TariffId">A tariff identification to compare with.</param>
        public Boolean Equals(Tariff_Id TariffId)

            => String.Equals(InternalId,
                             TariffId.InternalId,
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
