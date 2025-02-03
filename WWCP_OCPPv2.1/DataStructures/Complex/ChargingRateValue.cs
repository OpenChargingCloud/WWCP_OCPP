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
    /// A charging rate unit value (Watt or Ampere).
    /// </summary>
    public readonly struct ChargingRateValue : IEquatable<ChargingRateValue>,
                                               IComparable<ChargingRateValue>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the charging rate units.
        /// </summary>
        public Decimal            Value           { get; }

        /// <summary>
        /// The value of the Amperes as Int32.
        /// </summary>
        public Int32              IntegerValue    { get; }

        /// <summary>
        /// The charging rate unit.
        /// </summary>
        public ChargingRateUnits  Unit            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChargingRateValue value.
        /// </summary>
        /// <param name="Value">The value of the charging rate value.</param>
        /// <param name="Unit">The unit of the charging rate value (Watt or Ampere).</param>
        private ChargingRateValue(Decimal            Value,
                                  ChargingRateUnits  Unit)
        {

            this.Value  = Value;
            this.Unit   = Unit;

        }

        #endregion


        #region (static) Parse           (Text)

        /// <summary>
        /// Parse the given string as a charging rate value.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate value.</param>
        public static ChargingRateValue Parse(String Text)
        {

            if (TryParse(Text, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid text representation of a charging rate value: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse           (Number)

        /// <summary>
        /// Parse the given number as a charging rate value
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue Parse(Decimal Number)
        {

            if (TryParse(Number, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid numeric representation of a charging rate value: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseAmperes    (Number)

        /// <summary>
        /// Parse the given number as Amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue ParseAmperes(Decimal Number)
        {

            if (TryParseAmperes(Number, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid numeric representation of a charging rate value: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as Amperes.
        /// </summary>
        /// <param name="Amperes">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue ParseAmperes(Ampere Amperes)
        {

            if (TryParseAmperes(Amperes, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid numeric representation of a charging rate value: '{Amperes}'!",
                                        nameof(Amperes));

        }

        #endregion

        #region (static) ParseWatts      (Number)

        /// <summary>
        /// Parse the given number as Watts.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue ParseWatts(Decimal Number)
        {

            if (TryParseWatts(Number, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid numeric representation of a charging rate value: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as Watts.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue ParseWatts(Watt Number)
        {

            if (TryParseWatts(Number, out var chargingRateValue))
                return chargingRateValue;

            throw new ArgumentException($"Invalid numeric representation of a charging rate value: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse        (Text)

        /// <summary>
        /// Try to parse the given text as a charging rate value.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate value.</param>
        public static ChargingRateValue? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }

        #endregion

        #region (static) TryParse        (Number)

        /// <summary>
        /// Try to parse the given number as a charging rate value
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue? TryParse(Decimal Number)
        {

            if (TryParse(Number, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }

        #endregion

        #region (static) TryParseAmperes (Number)

        /// <summary>
        /// Try to parse the given number as Amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue? TryParseAmperes(Decimal Number)
        {

            if (TryParseAmperes(Number, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as Amperes.
        /// </summary>
        /// <param name="Amperes">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue? TryParseAmperes(Ampere Amperes)
        {

            if (TryParseAmperes(Amperes, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }

        #endregion

        #region (static) TryParseWatts   (Number)

        /// <summary>
        /// Try to parse the given number as Watts.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue? TryParseWatts(Decimal Number)
        {

            if (TryParseWatts(Number, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as Watts.
        /// </summary>
        /// <param name="Watts">A numeric representation of a charging rate value.</param>
        public static ChargingRateValue? TryParseWatts(Watt Watts)
        {

            if (TryParseWatts(Watts, out var chargingRateValue))
                return chargingRateValue;

            return null;

        }

        #endregion


        #region (static) TryParse        (Text,   out ChargingRateValue)

        /// <summary>
        /// Parse the given string as a charging rate value.
        /// </summary>
        /// <param name="Text">A text representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParse(String Text, out ChargingRateValue ChargingRateValue)
        {

            try
            {

                Text = Text.Trim();

                var factor  = 0;
                var unit    = ChargingRateUnits.Unknown;

                if (Text.EndsWith("kW") || Text.EndsWith("KW")) {
                    factor  = 1000;
                    unit    = ChargingRateUnits.Watts;
                }

                else if (Text.EndsWith("MW")) {
                    factor  = 1000000;
                    unit    = ChargingRateUnits.Watts;
                }

                else if (Text.EndsWith("GW")) {
                    factor  = 1000000;
                    unit    = ChargingRateUnits.Watts;
                }

                else if (Text.EndsWith("A")) {
                    unit    = ChargingRateUnits.Amperes;
                }

                if (Decimal.TryParse(Text, out var value))
                {

                    ChargingRateValue = new ChargingRateValue(
                                            value / factor,
                                            unit
                                        );

                    return true;

                }

            }
            catch
            { }

            ChargingRateValue = default;
            return false;

        }

        #endregion

        #region (static) TryParse        (Number, out ChargingRateValue)

        /// <summary>
        /// Parse the given number as a charging rate value
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParse(Decimal                Number,
                                       out ChargingRateValue  ChargingRateValue)
        {

            ChargingRateValue = new ChargingRateValue(
                                    Number,
                                    ChargingRateUnits.Unknown
                                );

            return true;

        }

        #endregion

        #region (static) TryParseAmperes (Number, out ChargingRateValue)

        /// <summary>
        /// Parse the given number as Amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParseAmperes(Decimal                Number,
                                              out ChargingRateValue  ChargingRateValue)
        {

            ChargingRateValue = new ChargingRateValue(
                                    Number,
                                    ChargingRateUnits.Amperes
                                );

            return true;

        }


        /// <summary>
        /// Parse the given number as Amperes.
        /// </summary>
        /// <param name="Amperes">A numeric representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParseAmperes(Ampere                 Amperes,
                                              out ChargingRateValue  ChargingRateValue)
        {

            ChargingRateValue = new ChargingRateValue(
                                    Amperes.Value,
                                    ChargingRateUnits.Amperes
                                );

            return true;

        }

        #endregion

        #region (static) TryParseWatts   (Number, out ChargingRateValue)

        /// <summary>
        /// Parse the given number as Watts.
        /// </summary>
        /// <param name="Number">A numeric representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParseWatts(Decimal                Number,
                                            out ChargingRateValue  ChargingRateValue)
        {

            ChargingRateValue = new ChargingRateValue(
                                    Number,
                                    ChargingRateUnits.Watts
                                );

            return true;

        }


        /// <summary>
        /// Parse the given number as Watts.
        /// </summary>
        /// <param name="Watts">A numeric representation of a charging rate value.</param>
        /// <param name="ChargingRateValue">The parsed charging rate value.</param>
        public static Boolean TryParseWatts(Watt                   Watts,
                                            out ChargingRateValue  ChargingRateValue)
        {

            ChargingRateValue = new ChargingRateValue(
                                    Watts.Value,
                                    ChargingRateUnits.Watts
                                );

            return true;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this charging rate value.
        /// </summary>
        public ChargingRateValue Clone()

            => new (
                   Value,
                   Unit
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingRateValue ChargingRateValue1,
                                           ChargingRateValue ChargingRateValue2)

            => ChargingRateValue1.Equals(ChargingRateValue2);

        #endregion

        #region Operator != (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingRateValue ChargingRateValue1,
                                           ChargingRateValue ChargingRateValue2)

            => !ChargingRateValue1.Equals(ChargingRateValue2);

        #endregion

        #region Operator <  (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingRateValue ChargingRateValue1,
                                          ChargingRateValue ChargingRateValue2)

            => ChargingRateValue1.CompareTo(ChargingRateValue2) < 0;

        #endregion

        #region Operator <= (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingRateValue ChargingRateValue1,
                                           ChargingRateValue ChargingRateValue2)

            => ChargingRateValue1.CompareTo(ChargingRateValue2) <= 0;

        #endregion

        #region Operator >  (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingRateValue ChargingRateValue1,
                                          ChargingRateValue ChargingRateValue2)

            => ChargingRateValue1.CompareTo(ChargingRateValue2) > 0;

        #endregion

        #region Operator >= (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingRateValue ChargingRateValue1,
                                           ChargingRateValue ChargingRateValue2)

            => ChargingRateValue1.CompareTo(ChargingRateValue2) >= 0;

        #endregion

        #region Operator +  (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Accumulates two ChargingRateValues.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        public static ChargingRateValue operator + (ChargingRateValue ChargingRateValue1,
                                                    ChargingRateValue ChargingRateValue2)
        {

            if (ChargingRateValue1.Unit == ChargingRateValue2.Unit)
                return ChargingRateValue1.Unit == ChargingRateUnits.Amperes
                           ? ChargingRateValue.ParseAmperes(ChargingRateValue1.Value + ChargingRateValue2.Value)
                           : ChargingRateValue.ParseWatts  (ChargingRateValue1.Value + ChargingRateValue2.Value);

            throw new ArgumentException($"The charging rate units are not equal: {ChargingRateValue1.Unit.AsText()} vs. {ChargingRateValue2.Unit.AsText()}!");

        }

        #endregion

        #region Operator -  (ChargingRateValue1, ChargingRateValue2)

        /// <summary>
        /// Substracts two ChargingRateValues.
        /// </summary>
        /// <param name="ChargingRateValue1">A ChargingRateValue.</param>
        /// <param name="ChargingRateValue2">Another ChargingRateValue.</param>
        public static ChargingRateValue operator - (ChargingRateValue ChargingRateValue1,
                                                    ChargingRateValue ChargingRateValue2)
        {

            if (ChargingRateValue1.Unit == ChargingRateValue2.Unit)
                return ChargingRateValue1.Unit == ChargingRateUnits.Amperes
                           ? ChargingRateValue.ParseAmperes(ChargingRateValue1.Value - ChargingRateValue2.Value)
                           : ChargingRateValue.ParseWatts  (ChargingRateValue1.Value - ChargingRateValue2.Value);

            throw new ArgumentException($"The charging rate units are not equal: {ChargingRateValue1.Unit.AsText()} vs. {ChargingRateValue2.Unit.AsText()}!");

        }

        #endregion

        #endregion

        #region IComparable<ChargingRateValue> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ChargingRateValues.
        /// </summary>
        /// <param name="Object">A ChargingRateValue to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingRateValue chargingRateValue
                   ? CompareTo(chargingRateValue)
                   : throw new ArgumentException("The given object is not a charging rate value!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingRateValue)

        /// <summary>
        /// Compares two ChargingRateValues.
        /// </summary>
        /// <param name="ChargingRateValue">A ChargingRateValue to compare with.</param>
        public Int32 CompareTo(ChargingRateValue ChargingRateValue)
        {

            var c = Value.CompareTo(ChargingRateValue.Value);

            if (c == 0)
                c = Unit. CompareTo(ChargingRateValue.Unit);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingRateValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChargingRateValues for equality.
        /// </summary>
        /// <param name="Object">A ChargingRateValue to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingRateValue chargingRateValue &&
                   Equals(chargingRateValue);

        #endregion

        #region Equals(ChargingRateValue)

        /// <summary>
        /// Compares two ChargingRateValues for equality.
        /// </summary>
        /// <param name="ChargingRateValue">A ChargingRateValue to compare with.</param>
        public Boolean Equals(ChargingRateValue ChargingRateValue)

            => Value.Equals(ChargingRateValue.Value) &&
               Unit. Equals(ChargingRateValue.Unit);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode() ^
               Unit. GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Value} {Unit.AsText()}";

        #endregion

    }

}
