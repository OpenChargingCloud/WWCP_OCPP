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
    /// Extension methods for Power During Cessations.
    /// </summary>
    public static class PowerDuringCessationExtensions
    {

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        /// <param name="PowerDuringCessation">A DER unit.</param>
        public static Boolean IsNullOrEmpty(this PowerDuringCessation? PowerDuringCessation)
            => !PowerDuringCessation.HasValue || PowerDuringCessation.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        /// <param name="PowerDuringCessation">A DER unit.</param>
        public static Boolean IsNotNullOrEmpty(this PowerDuringCessation? PowerDuringCessation)
            => PowerDuringCessation.HasValue && PowerDuringCessation.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A PowerDuringCessation.
    /// (Cessation = Event where normal operation stops)
    /// </summary>
    public readonly struct PowerDuringCessation : IId,
                                                  IEquatable<PowerDuringCessation>,
                                                  IComparable<PowerDuringCessation>
    {

        #region Data

        private readonly static Dictionary<String, PowerDuringCessation>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        public readonly  Boolean                      IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this DER unit is NOT null or empty.
        /// </summary>
        public readonly  Boolean                      IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the DER unit.
        /// </summary>
        public readonly  UInt64                       Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered DER units.
        /// </summary>
        public static    IEnumerable<PowerDuringCessation>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER unit based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a DER unit.</param>
        private PowerDuringCessation(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PowerDuringCessation Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PowerDuringCessation(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        public static PowerDuringCessation Parse(String Text)
        {

            if (TryParse(Text, out var derUnit))
                return derUnit;

            throw new ArgumentException($"Invalid text representation of a DER unit: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        public static PowerDuringCessation? TryParse(String Text)
        {

            if (TryParse(Text, out var derUnit))
                return derUnit;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PowerDuringCessation)

        /// <summary>
        /// Try to parse the given text as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        /// <param name="PowerDuringCessation">The parsed DER unit.</param>
        public static Boolean TryParse(String Text, out PowerDuringCessation PowerDuringCessation)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PowerDuringCessation))
                    PowerDuringCessation = Register(Text);

                return true;

            }

            PowerDuringCessation = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this DER unit.
        /// </summary>
        public PowerDuringCessation Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Active
        /// </summary>
        public static PowerDuringCessation  Active      { get; }
            = Register("Active");

        /// <summary>
        /// Reactive
        /// </summary>
        public static PowerDuringCessation  Reactive    { get; }
            = Register("Reactive");

        #endregion


        #region Operator overloading

        #region Operator == (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PowerDuringCessation PowerDuringCessation1,
                                           PowerDuringCessation PowerDuringCessation2)

            => PowerDuringCessation1.Equals(PowerDuringCessation2);

        #endregion

        #region Operator != (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PowerDuringCessation PowerDuringCessation1,
                                           PowerDuringCessation PowerDuringCessation2)

            => !PowerDuringCessation1.Equals(PowerDuringCessation2);

        #endregion

        #region Operator <  (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PowerDuringCessation PowerDuringCessation1,
                                          PowerDuringCessation PowerDuringCessation2)

            => PowerDuringCessation1.CompareTo(PowerDuringCessation2) < 0;

        #endregion

        #region Operator <= (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PowerDuringCessation PowerDuringCessation1,
                                           PowerDuringCessation PowerDuringCessation2)

            => PowerDuringCessation1.CompareTo(PowerDuringCessation2) <= 0;

        #endregion

        #region Operator >  (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PowerDuringCessation PowerDuringCessation1,
                                          PowerDuringCessation PowerDuringCessation2)

            => PowerDuringCessation1.CompareTo(PowerDuringCessation2) > 0;

        #endregion

        #region Operator >= (PowerDuringCessation1, PowerDuringCessation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PowerDuringCessation1">A DER unit.</param>
        /// <param name="PowerDuringCessation2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PowerDuringCessation PowerDuringCessation1,
                                           PowerDuringCessation PowerDuringCessation2)

            => PowerDuringCessation1.CompareTo(PowerDuringCessation2) >= 0;

        #endregion

        #endregion

        #region IComparable<PowerDuringCessation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two DER units.
        /// </summary>
        /// <param name="Object">A DER unit to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PowerDuringCessation derUnit
                   ? CompareTo(derUnit)
                   : throw new ArgumentException("The given object is not a DER unit!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PowerDuringCessation)

        /// <summary>
        /// Compares two DER units.
        /// </summary>
        /// <param name="PowerDuringCessation">A DER unit to compare with.</param>
        public Int32 CompareTo(PowerDuringCessation PowerDuringCessation)

            => String.Compare(InternalId,
                              PowerDuringCessation.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PowerDuringCessation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER units for equality.
        /// </summary>
        /// <param name="Object">A DER unit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PowerDuringCessation derUnit &&
                   Equals(derUnit);

        #endregion

        #region Equals(PowerDuringCessation)

        /// <summary>
        /// Compares two DER units for equality.
        /// </summary>
        /// <param name="PowerDuringCessation">A DER unit to compare with.</param>
        public Boolean Equals(PowerDuringCessation PowerDuringCessation)

            => String.Equals(InternalId,
                             PowerDuringCessation.InternalId,
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
