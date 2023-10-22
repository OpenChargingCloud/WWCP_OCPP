/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
    /// Extension methods for charging tariff identifications.
    /// </summary>
    public static class ChargingTariffIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging tariff identification is null or empty.
        /// </summary>
        /// <param name="ChargingTariffId">A charging tariff identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingTariff_Id? ChargingTariffId)
            => !ChargingTariffId.HasValue || ChargingTariffId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging tariff identification is NOT null or empty.
        /// </summary>
        /// <param name="ChargingTariffId">A charging tariff identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingTariff_Id? ChargingTariffId)
            => ChargingTariffId.HasValue && ChargingTariffId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging tariff.
    /// </summary>
    public readonly struct ChargingTariff_Id : IId<ChargingTariff_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charging tariff identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging tariff identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charging tariff identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charging tariff identification.</param>
        private ChargingTariff_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(EMPId, Length = 20)

        /// <summary>
        /// Create a new random charging tariff identification based
        /// on an e-mobility provider Identification, the current timestamp
        /// and a random number.
        /// </summary>
        /// <param name="Length">The expected length of the charging tariff identification.</param>
        public static ChargingTariff_Id NewRandom(String  EMPId,
                                          Byte    Length = 30)

            => new (String.Concat(
                        EMPId,                           "-",
                        Timestamp.Now.ToUnixTimestamp(), "-",
                        RandomExtensions.RandomString(Length)
                    ));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        public static ChargingTariff_Id Parse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            throw new ArgumentException($"Invalid text representation of a charging tariff identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        public static ChargingTariff_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out ChargingTariffId)

        /// <summary>
        /// Try to parse the given text as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        /// <param name="ChargingTariffId">The parsed charging tariff identification.</param>
        public static Boolean TryParse(String Text, out ChargingTariff_Id ChargingTariffId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ChargingTariffId = new ChargingTariff_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ChargingTariffId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging tariff identification.
        /// </summary>
        public ChargingTariff_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingTariff_Id ChargingTariffId1,
                                           ChargingTariff_Id ChargingTariffId2)

            => ChargingTariffId1.Equals(ChargingTariffId2);

        #endregion

        #region Operator != (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingTariff_Id ChargingTariffId1,
                                           ChargingTariff_Id ChargingTariffId2)

            => !ChargingTariffId1.Equals(ChargingTariffId2);

        #endregion

        #region Operator <  (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingTariff_Id ChargingTariffId1,
                                          ChargingTariff_Id ChargingTariffId2)

            => ChargingTariffId1.CompareTo(ChargingTariffId2) < 0;

        #endregion

        #region Operator <= (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingTariff_Id ChargingTariffId1,
                                           ChargingTariff_Id ChargingTariffId2)

            => ChargingTariffId1.CompareTo(ChargingTariffId2) <= 0;

        #endregion

        #region Operator >  (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingTariff_Id ChargingTariffId1,
                                          ChargingTariff_Id ChargingTariffId2)

            => ChargingTariffId1.CompareTo(ChargingTariffId2) > 0;

        #endregion

        #region Operator >= (ChargingTariffId1, ChargingTariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariffId1">A charging tariff identification.</param>
        /// <param name="ChargingTariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingTariff_Id ChargingTariffId1,
                                           ChargingTariff_Id ChargingTariffId2)

            => ChargingTariffId1.CompareTo(ChargingTariffId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingTariffId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging tariff identifications.
        /// </summary>
        /// <param name="Object">A charging tariff identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingTariff_Id tariffId
                   ? CompareTo(tariffId)
                   : throw new ArgumentException("The given object is not a charging tariff identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingTariffId)

        /// <summary>
        /// Compares two charging tariff identifications.
        /// </summary>
        /// <param name="ChargingTariffId">A charging tariff identification to compare with.</param>
        public Int32 CompareTo(ChargingTariff_Id ChargingTariffId)

            => String.Compare(InternalId,
                              ChargingTariffId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargingTariffId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariff identifications for equality.
        /// </summary>
        /// <param name="Object">A charging tariff identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingTariff_Id tariffId &&
                   Equals(tariffId);

        #endregion

        #region Equals(ChargingTariffId)

        /// <summary>
        /// Compares two charging tariff identifications for equality.
        /// </summary>
        /// <param name="ChargingTariffId">A charging tariff identification to compare with.</param>
        public Boolean Equals(ChargingTariff_Id ChargingTariffId)

            => String.Equals(InternalId,
                             ChargingTariffId.InternalId,
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
