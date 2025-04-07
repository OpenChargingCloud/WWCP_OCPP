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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for energy meter identifications.
    /// </summary>
    public static class EnergyMeterIdExtensions
    {

        /// <summary>
        /// Indicates whether this energy meter identification is null or empty.
        /// </summary>
        /// <param name="EnergyMeterId">An energy meter identification.</param>
        public static Boolean IsNullOrEmpty(this EnergyMeter_Id? EnergyMeterId)
            => !EnergyMeterId.HasValue || EnergyMeterId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this energy meter identification is null or empty.
        /// </summary>
        /// <param name="EnergyMeterId">An energy meter identification.</param>
        public static Boolean IsNotNullOrEmpty(this EnergyMeter_Id? EnergyMeterId)
            => EnergyMeterId.HasValue && EnergyMeterId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an energy meter.
    /// </summary>
    public readonly struct EnergyMeter_Id : IId,
                                            IEquatable <EnergyMeter_Id>,
                                            IComparable<EnergyMeter_Id>

    {

        #region Data

        /// <summary>
        /// The internal energy meter identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this energy meter identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this energy meter identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the energy meter identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new energy meter identification based on the given string.
        /// </summary>
        private EnergyMeter_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) NewRandom  (Length = 15)

        /// <summary>
        /// Create a new random energy meter identification.
        /// </summary>
        /// <param name="Length">The expected length of the random energy meter identification.</param>
        public static EnergyMeter_Id NewRandom(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an energy meter identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy meter identification.</param>
        public static EnergyMeter_Id Parse(String Text)
        {

            if (TryParse(Text, out EnergyMeter_Id energyMeterId))
                return energyMeterId;

            throw new ArgumentException($"Invalid text representation of an energy meter identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an energy meter identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy meter identification.</param>
        public static EnergyMeter_Id? TryParse(String? Text)
        {

            if (Text is not null && TryParse(Text, out EnergyMeter_Id energyMeterId))
                return energyMeterId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnergyMeterId)

        /// <summary>
        /// Try to parse the given string as an energy meter identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy meter identification.</param>
        /// <param name="EnergyMeterId">The parsed energy meter identification.</param>
        public static Boolean TryParse(String Text, out EnergyMeter_Id EnergyMeterId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EnergyMeterId = new EnergyMeter_Id(Text);
                    return true;
                }
                catch
                { }
            }

            EnergyMeterId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this energy meter identification.
        /// </summary>
        public EnergyMeter_Id Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (EnergyMeter_Id EnergyMeterId1,
                                           EnergyMeter_Id EnergyMeterId2)

            => EnergyMeterId1.Equals(EnergyMeterId2);

        #endregion

        #region Operator != (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (EnergyMeter_Id EnergyMeterId1,
                                           EnergyMeter_Id EnergyMeterId2)

            => !EnergyMeterId1.Equals(EnergyMeterId2);

        #endregion

        #region Operator <  (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator < (EnergyMeter_Id EnergyMeterId1,
                                          EnergyMeter_Id EnergyMeterId2)

            => EnergyMeterId1.CompareTo(EnergyMeterId2) < 0;

        #endregion

        #region Operator <= (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator <= (EnergyMeter_Id EnergyMeterId1,
                                           EnergyMeter_Id EnergyMeterId2)

            => EnergyMeterId1.CompareTo(EnergyMeterId2) <= 0;

        #endregion

        #region Operator >  (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator > (EnergyMeter_Id EnergyMeterId1,
                                          EnergyMeter_Id EnergyMeterId2)

            => EnergyMeterId1.CompareTo(EnergyMeterId2) > 0;

        #endregion

        #region Operator >= (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">An energy meter identification.</param>
        /// <param name="EnergyMeterId2">Another energy meter identification.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator >= (EnergyMeter_Id EnergyMeterId1,
                                           EnergyMeter_Id EnergyMeterId2)

            => EnergyMeterId1.CompareTo(EnergyMeterId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnergyMeter_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergyMeter_Id energyMeterId
                   ? CompareTo(energyMeterId)
                   : throw new ArgumentException("The given object is not an energy meter identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyMeterId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId">An object to compare with.</param>
        public Int32 CompareTo(EnergyMeter_Id EnergyMeterId)

            => String.Compare(InternalId,
                              EnergyMeterId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EnergyMeter_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Object? Object)

            => Object is EnergyMeter_Id energyMeterId &&
                   Equals(energyMeterId);

        #endregion

        #region Equals(EnergyMeterId)

        /// <summary>
        /// Compares two energy meter identifications for equality.
        /// </summary>
        /// <param name="EnergyMeterId">An energy meter identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyMeter_Id EnergyMeterId)

            => String.Equals(InternalId,
                             EnergyMeterId.InternalId,
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
