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

using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for units of measure.
    /// </summary>
    public static class UnitsOfMeasureExtensions
    {

        #region Parse(Text)

        public static UnitsOfMeasure Parse(String Text)

            => Text.Trim() switch {
                   "Celsius"     => UnitsOfMeasure.Celsius,
                   "Celcius"     => UnitsOfMeasure.Celsius,
                   "Fahrenheit"  => UnitsOfMeasure.Fahrenheit,
                   "kWh"         => UnitsOfMeasure.kWh,
                   "varh"        => UnitsOfMeasure.varh,
                   "kvarh"       => UnitsOfMeasure.kvarh,
                   "W"           => UnitsOfMeasure.Watts,
                   "kW"          => UnitsOfMeasure.kW,
                   "VA"          => UnitsOfMeasure.VoltAmpere,
                   "kVA"         => UnitsOfMeasure.kVA,
                   "var"         => UnitsOfMeasure.var,
                   "kvar"        => UnitsOfMeasure.kvar,
                   "A"           => UnitsOfMeasure.Amperes,
                   "V"           => UnitsOfMeasure.Voltage,
                   "K"           => UnitsOfMeasure.Kelvin,
                   "Percent"     => UnitsOfMeasure.Percent,
                   _             => UnitsOfMeasure.Wh
               };

        #endregion

        #region AsText(this UnitOfMeasure)

        public static String AsText(this UnitsOfMeasure UnitOfMeasure)

            => UnitOfMeasure switch {
                   UnitsOfMeasure.Celsius     => "Celsius",
                   UnitsOfMeasure.Fahrenheit  => "Fahrenheit",
                   UnitsOfMeasure.kWh         => "kWh",
                   UnitsOfMeasure.varh        => "varh",
                   UnitsOfMeasure.kvarh       => "kvarh",
                   UnitsOfMeasure.Watts       => "W",
                   UnitsOfMeasure.kW          => "kW",
                   UnitsOfMeasure.VoltAmpere  => "VA",
                   UnitsOfMeasure.kVA         => "kVA",
                   UnitsOfMeasure.var         => "var",
                   UnitsOfMeasure.kvar        => "kvar",
                   UnitsOfMeasure.Amperes     => "A",
                   UnitsOfMeasure.Voltage     => "V",
                   UnitsOfMeasure.Kelvin      => "K",
                   UnitsOfMeasure.Percent     => "Percent",
                   _                          => "Wh"
               };

        #endregion

        #region AsJSONText(this UnitOfMeasure)

        /// <summary>
        /// The text of the given unit of measure in JSON: "Celcius" as the JSON
        /// schemas spell it - the errata of OCPP-J 1.6 (2025-04) advise charge
        /// points to keep sending it, central systems to read both spellings.
        /// </summary>
        public static String AsJSONText(this UnitsOfMeasure UnitOfMeasure)

            => UnitOfMeasure == UnitsOfMeasure.Celsius
                   ? "Celcius"
                   : UnitOfMeasure.AsText();

        #endregion

        #region Metrology: TryToMetrologicalValue(Value, out MetrologicalValue), TryFromMetrologicalValue(...)

        /// <summary>
        /// The units of measure that are metrological units: the unit and the
        /// power of ten its prefix is.
        /// </summary>
        private static readonly Dictionary<UnitsOfMeasure, (UnitOfMeasure Unit, Int32 Exponent)> metrologicalUnits = new() {
            { UnitsOfMeasure.Wh,          (UnitOfMeasure.WattHour,                0) },
            { UnitsOfMeasure.kWh,         (UnitOfMeasure.WattHour,                3) },
            { UnitsOfMeasure.varh,        (UnitOfMeasure.VoltAmpereReactiveHour,  0) },
            { UnitsOfMeasure.kvarh,       (UnitOfMeasure.VoltAmpereReactiveHour,  3) },
            { UnitsOfMeasure.Watts,       (UnitOfMeasure.Watt,                    0) },
            { UnitsOfMeasure.kW,          (UnitOfMeasure.Watt,                    3) },
            { UnitsOfMeasure.VoltAmpere,  (UnitOfMeasure.VoltAmpere,              0) },
            { UnitsOfMeasure.kVA,         (UnitOfMeasure.VoltAmpere,              3) },
            { UnitsOfMeasure.var,         (UnitOfMeasure.VoltAmpereReactive,      0) },
            { UnitsOfMeasure.kvar,        (UnitOfMeasure.VoltAmpereReactive,      3) },
            { UnitsOfMeasure.Amperes,     (UnitOfMeasure.Ampere,                  0) },
            { UnitsOfMeasure.Voltage,     (UnitOfMeasure.Volt,                    0) },
            { UnitsOfMeasure.Kelvin,      (UnitOfMeasure.Kelvin,                  0) },
            { UnitsOfMeasure.Celsius,     (UnitOfMeasure.Celsius,                 0) },
            { UnitsOfMeasure.Percent,     (UnitOfMeasure.Percent,                 0) }
        };


        /// <summary>
        /// The given text of a value in this unit as a metrological value - for
        /// every unit that is a metrological unit, and a text that is a decimal
        /// number written as a decimal writes it again.
        /// </summary>
        /// <param name="UnitOfMeasure">A unit of measure.</param>
        /// <param name="Value">The text of a value in this unit.</param>
        /// <param name="MetrologicalValue">The metrological value.</param>
        public static Boolean TryToMetrologicalValue(this UnitsOfMeasure    UnitOfMeasure,
                                                     String                 Value,
                                                     out MetrologicalValue  MetrologicalValue)
        {

            MetrologicalValue = default;

            if (!metrologicalUnits.TryGetValue(UnitOfMeasure, out var unit) ||
                !Decimal.TryParse(Value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value) ||
                 value.ToString(CultureInfo.InvariantCulture) != Value ||
                !SIPrefix.TryFrom(unit.Exponent, out var prefix))
            {
                return false;
            }

            MetrologicalValue = new MetrologicalValue(value, unit.Unit, prefix);
            return true;

        }


        /// <summary>
        /// The text of the value and the unit of the given metrological value: of
        /// the units with its metrological unit the one whose prefix is the
        /// value's (kWh for 10^3), else the largest one not above it (kWh for
        /// 10^6, with the value in kWh), else the unit itself (Wh for 10^-3).
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Value">The text of the value in the unit.</param>
        /// <param name="UnitOfMeasure">The unit of measure.</param>
        public static Boolean TryFromMetrologicalValue(MetrologicalValue   MetrologicalValue,
                                                       out String          Value,
                                                       out UnitsOfMeasure  UnitOfMeasure)
        {

            Value          = "";
            UnitOfMeasure  = UnitsOfMeasure.Wh;

            var exponent   = MetrologicalValue.Prefix.Exponent;

            var candidates = metrologicalUnits.Where(entry => MetrologicalValue.Unit == entry.Value.Unit).
                                               OrderByDescending(entry => entry.Value.Exponent).
                                               ToArray();

            if (candidates.Length == 0)
                return false;

            var chosen     = candidates.Any (entry => entry.Value.Exponent == exponent)
                                 ? candidates.First(entry => entry.Value.Exponent == exponent)
                                 : candidates.Any (entry => entry.Value.Exponent <= exponent)
                                       ? candidates.First(entry => entry.Value.Exponent <= exponent)
                                       : candidates.Last();

            if (!SIPrefix.TryFrom(chosen.Value.Exponent, out var prefix))
                return false;

            try
            {
                Value      = MetrologicalValue.ConvertTo(prefix).Value.ToString(CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                return false;
            }

            UnitOfMeasure  = chosen.Key;
            return true;

        }

        #endregion

    }


    /// <summary>
    /// Allowable values of the optional "unit" field of a Value element,
    /// as used in the MeterValues request and StopTransaction request
    /// messages. Default value of "unit" is always "Wh".
    /// </summary>
    public enum UnitsOfMeasure
    {

        /// <summary>
        /// Degrees (temperature)
        /// </summary>
        Celsius,

        /// <summary>
        /// Degrees (temperature)
        /// </summary>
        Fahrenheit,

        /// <summary>
        /// Watt-hours (energy)
        /// </summary>
        Wh,

        /// <summary>
        /// kiloWatt-hours (energy)
        /// </summary>
        kWh,

        /// <summary>
        /// Var-hours (reactive energy)
        /// </summary>
        varh,

        /// <summary>
        /// kilovar-hours (reactive energy)
        /// </summary>
        kvarh,

        /// <summary>
        /// Watts (power)
        /// </summary>
        Watts,

        /// <summary>
        /// kiloWatts (power)
        /// </summary>
        kW,

        /// <summary>
        /// VoltAmpere (apparent power)
        /// </summary>
        VoltAmpere,

        /// <summary>
        /// kiloVolt Ampere (apparent power)
        /// </summary>
        kVA,

        /// <summary>
        /// Vars (reactive power)
        /// </summary>
        var,

        /// <summary>
        /// kilovars (reactive power)
        /// </summary>
        kvar,

        /// <summary>
        /// Amperes (current)
        /// </summary>
        Amperes,

        /// <summary>
        /// Voltage (r.m.s. AC)
        /// </summary>
        Voltage,

        /// <summary>
        /// Degrees Kelvin (temperature)
        /// </summary>
        Kelvin,

        /// <summary>
        /// Percentage
        /// </summary>
        Percent

    }

}
