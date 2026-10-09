/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Converts a metrological value into a value of the given type.
    /// </summary>
    public delegate Boolean MetrologicalConverter<T>(MetrologicalValue MetrologicalValue, out T Value);


    /// <summary>
    /// The building blocks of the CBOR representations of OCPP data structures
    /// and messages, written and read directly as CBOR, without JSON between.
    /// </summary>
    /// <remarks>
    /// The conventions:
    /// - A map has the text keys of the JSON object, without the suffixes that
    ///   name a unit there: the unit is part of the value.
    /// - A number with a unit is a metrological value (tag 44252), and so is a
    ///   duration, in seconds.
    /// - A timestamp is tag 1: epoch seconds, an integer for a whole second and
    ///   a float otherwise, read back to the microsecond.
    /// - An optional value that is not there is left out, never written as null.
    /// </remarks>
    public static class OCPPCBORExtensions
    {

        #region Map(params Properties)

        /// <summary>
        /// A CBOR map of the given properties, leaving out those without a value.
        /// </summary>
        /// <param name="Properties">The properties: a text key and an optional value.</param>
        public static CBORValue Map(params IEnumerable<(String Key, CBORValue? Value)> Properties)
        {

            var map = new CBORMap();

            foreach (var (key, value) in Properties)
                if (value.HasValue)
                    map.Add(CBORValue.FromText(key), value.Value);

            return map;

        }

        #endregion

        #region Text(Text), UInt(Number), Int(Number), Number(Number), Flag(Value)

        /// <summary>
        /// The given text, or nothing - for an optional value of a map.
        /// </summary>
        public static CBORValue? Text(String? Text)
            => Text is not null ? (CBORValue?) CBORValue.FromText(Text) : null;

        /// <summary>
        /// The given unsigned number, or nothing - for an optional value of a map.
        /// </summary>
        public static CBORValue? UInt(UInt64? Number)
            => Number.HasValue ? (CBORValue?) CBORValue.FromUInt64(Number.Value) : null;

        /// <summary>
        /// The given signed number, or nothing - for an optional value of a map.
        /// </summary>
        public static CBORValue? Int(Int64? Number)
            => Number.HasValue ? (CBORValue?) CBORValue.FromInt64(Number.Value) : null;

        /// <summary>
        /// The given decimal number, or nothing - for an optional value of a map.
        /// </summary>
        public static CBORValue? Number(Decimal? Number)
            => Number.HasValue ? (CBORValue?) CBORValue.FromDecimal(Number.Value) : null;

        /// <summary>
        /// The given boolean, or nothing - for an optional value of a map.
        /// </summary>
        public static CBORValue? Flag(Boolean? Value)
            => Value.HasValue ? (CBORValue?) CBORValue.FromBoolean(Value.Value) : null;

        #endregion

        #region Array(Values, ToCBOR)

        /// <summary>
        /// A CBOR array of the given values - or nothing for none, as an optional
        /// list that is empty is left out.
        /// </summary>
        /// <param name="Values">An enumeration of values.</param>
        /// <param name="ToCBOR">How to write each value.</param>
        public static CBORValue? Array<T>(IEnumerable<T>?     Values,
                                          Func<T, CBORValue>  ToCBOR)
        {

            var values = Values?.ToArray() ?? [];

            return values.Length > 0
                       ? (CBORValue?) CBORValue.FromArray(values.Select(ToCBOR))
                       : null;

        }

        #endregion


        #region Timestamps: ToCBOR(Timestamp), TryParseTimestamp(CBOR, out Timestamp, out ErrorResponse)

        /// <summary>
        /// The given timestamp as tag 1: epoch seconds, as an integer for a whole
        /// second and as a float otherwise.
        /// </summary>
        /// <param name="Timestamp">A timestamp.</param>
        public static CBORValue ToCBOR(this DateTimeOffset Timestamp)
        {

            var ticks = (Timestamp.ToUniversalTime() - DateTimeOffset.UnixEpoch).Ticks;

            return CBORValue.Tagged(
                       CBORTag.EpochDateTime,
                       ticks % TimeSpan.TicksPerSecond == 0
                           ? CBORValue.FromInt64 (ticks / TimeSpan.TicksPerSecond)
                           : CBORValue.FromDouble(ticks / (Double) TimeSpan.TicksPerSecond)
                   );

        }


        /// <summary>
        /// Try to read the given CBOR value as a timestamp: tag 1 (or 0), to the
        /// microsecond - which is what a float of epoch seconds holds exactly.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseTimestamp(CBORValue                         CBOR,
                                                out DateTimeOffset                Timestamp,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var map = new CBORMap { { CBORValue.FromText("t"), CBOR } };

            if (!((CBORValue) map).ParseMandatoryTimestamp("t", "timestamp", out Timestamp, out ErrorResponse))
                return false;

            var microseconds = (Timestamp - DateTimeOffset.UnixEpoch).Ticks / 10.0;
            Timestamp        = DateTimeOffset.UnixEpoch.AddTicks((Int64) Math.Round(microseconds, MidpointRounding.AwayFromZero) * 10);

            return true;

        }

        #endregion

        #region Durations:  ToCBOR(Duration),  TryParseDuration (CBOR, out Duration,  out ErrorResponse)

        /// <summary>
        /// The given duration as a metrological value in seconds.
        /// </summary>
        /// <param name="Duration">A duration.</param>
        public static CBORValue ToCBOR(this TimeSpan Duration)

            => new MetrologicalValue(
                   Duration.Ticks / (Decimal) TimeSpan.TicksPerSecond,
                   UnitOfMeasure.Second
               ).ToCBOR();


        /// <summary>
        /// Try to read the given CBOR value as a duration: a metrological value
        /// in seconds, minutes or hours, with any SI prefix.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Duration">The duration.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseDuration(CBORValue                         CBOR,
                                               out TimeSpan                      Duration,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Duration = default;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!metrologicalValue.TryToBaseUnit(out var baseValue))
            {
                ErrorResponse = $"The duration '{metrologicalValue}' could not be converted!";
                return false;
            }

            Decimal seconds;

                 if (baseValue.Unit == UnitOfMeasure.Second) seconds = baseValue.Value;
            else if (baseValue.Unit == UnitOfMeasure.Minute) seconds = baseValue.Value * 60;
            else if (baseValue.Unit == UnitOfMeasure.Hour)   seconds = baseValue.Value * 3600;
            else
            {
                ErrorResponse = $"The duration '{metrologicalValue}' is not in seconds, minutes or hours!";
                return false;
            }

            try
            {
                Duration = TimeSpan.FromTicks((Int64) Math.Round(seconds * TimeSpan.TicksPerSecond, MidpointRounding.AwayFromZero));
                return true;
            }
            catch (OverflowException)
            {
                ErrorResponse = $"The duration '{metrologicalValue}' is out of range!";
                return false;
            }

        }

        #endregion

        #region Metrology:  TryParseMetrological<T>(CBOR, Converter, UnitName, out Value, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR value as a metrological value of the given kind.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Converter">Converts the metrological value, e.g. MetrologyCBORExtensions.TryToWatt.</param>
        /// <param name="UnitName">The name of the unit, for the error response.</param>
        /// <param name="Value">The value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseMetrological<T>(CBORValue                         CBOR,
                                                      MetrologicalConverter<T>          Converter,
                                                      String                            UnitName,
                                                      out T                             Value,
                                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default!;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!Converter(metrologicalValue, out Value))
            {
                ErrorResponse = $"The value '{metrologicalValue}' is not in {UnitName}!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseWatt              (CBORValue CBOR, out Watt               Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToWatt,               "W",    out Value, out ErrorResponse);

        public static Boolean TryParseWattHour          (CBORValue CBOR, out WattHour           Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToWattHour,           "Wh",   out Value, out ErrorResponse);

        public static Boolean TryParseAmpere            (CBORValue CBOR, out Ampere             Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToAmpere,             "A",    out Value, out ErrorResponse);

        public static Boolean TryParseVolt              (CBORValue CBOR, out Volt               Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToVolt,               "V",    out Value, out ErrorResponse);

        public static Boolean TryParseVoltAmpere        (CBORValue CBOR, out VoltAmpere         Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToVoltAmpere,         "VA",   out Value, out ErrorResponse);

        public static Boolean TryParseVoltAmpereReactive(CBORValue CBOR, out VoltAmpereReactive Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToVoltAmpereReactive, "var",  out Value, out ErrorResponse);

        public static Boolean TryParseHertz             (CBORValue CBOR, out Hertz              Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToHertz,              "Hz",   out Value, out ErrorResponse);

        public static Boolean TryParseSiemens           (CBORValue CBOR, out Siemens            Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToSiemens,            "S",    out Value, out ErrorResponse);

        public static Boolean TryParseCelsius           (CBORValue CBOR, out Celsius            Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToCelsius,            "°C",   out Value, out ErrorResponse);

        public static Boolean TryParseOhm               (CBORValue CBOR, out Ohm                Value, [NotNullWhen(false)] out String? ErrorResponse)
            => TryParseMetrological(CBOR, MetrologyCBORExtensions.TryToOhm,                "Ω",    out Value, out ErrorResponse);

        #endregion

        #region Metrology:  ToCBOR(Watt, WattHour, Ampere, Volt, VoltAmpere, VoltAmpereReactive, Hertz, Siemens, Celsius)

        public static CBORValue ToCBOR(this Watt               Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this WattHour           Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Ampere             Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Volt               Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this VoltAmpere         Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this VoltAmpereReactive Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Hertz              Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Siemens            Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Celsius            Value) => Value.AsMetrologicalValue().ToCBOR();
        public static CBORValue ToCBOR(this Ohm                Value) => Value.AsMetrologicalValue().ToCBOR();

        #endregion

        #region Percentages: ToCBOR(Percentage...), TryParsePercentage...(CBOR, out Value, out ErrorResponse)

        /// <summary>
        /// The given percentage as a metrological value in %.
        /// </summary>
        public static CBORValue ToCBOR(this Percentage       Value) => new MetrologicalValue(Value.Value,           UnitOfMeasure.Percent).ToCBOR();

        /// <summary>
        /// The given percentage as a metrological value in %.
        /// </summary>
        public static CBORValue ToCBOR(this PercentageByte   Value) => new MetrologicalValue(Value.Value,           UnitOfMeasure.Percent).ToCBOR();

        /// <summary>
        /// The given percentage as a metrological value in %.
        /// </summary>
        public static CBORValue ToCBOR(this PercentageDouble Value) => new MetrologicalValue((Decimal) Value.Value, UnitOfMeasure.Percent).ToCBOR();


        /// <summary>
        /// Try to read the given CBOR value as a number of percent: a metrological value in %.
        /// </summary>
        private static Boolean TryParsePercent(CBORValue                         CBOR,
                                               out Decimal                       Percent,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Percent = default;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!metrologicalValue.TryToBaseUnit(out var baseValue) ||
                 baseValue.Unit != UnitOfMeasure.Percent)
            {
                ErrorResponse = $"The value '{metrologicalValue}' is not in %!";
                return false;
            }

            Percent = baseValue.Value;
            return true;

        }

        public static Boolean TryParsePercentage(CBORValue CBOR, out Percentage Value, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Value = default;

            if (!TryParsePercent(CBOR, out var percent, out ErrorResponse))
                return false;

            if (!Percentage.TryParse(percent, out Value))
            {
                ErrorResponse = $"Invalid percentage '{percent}'!";
                return false;
            }

            return true;

        }

        public static Boolean TryParsePercentageByte(CBORValue CBOR, out PercentageByte Value, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Value = default;

            if (!TryParsePercent(CBOR, out var percent, out ErrorResponse))
                return false;

            if (percent != Math.Truncate(percent) || percent < Byte.MinValue || percent > Byte.MaxValue ||
                !PercentageByte.TryParse((Byte) percent, out Value))
            {
                ErrorResponse = $"Invalid percentage '{percent}': a whole number of percent is expected!";
                return false;
            }

            return true;

        }

        public static Boolean TryParsePercentageDouble(CBORValue CBOR, out PercentageDouble Value, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Value = default;

            if (!TryParsePercent(CBOR, out var percent, out ErrorResponse))
                return false;

            if (!PercentageDouble.TryParse((Double) percent, out Value))
            {
                ErrorResponse = $"Invalid percentage '{percent}'!";
                return false;
            }

            return true;

        }

        #endregion

        #region Signed percentages: ToCBOR(SignedPercentage), TryParseSignedPercentage(CBOR, out Value, out ErrorResponse)

        /// <summary>
        /// The given signed percentage as a metrological value in %.
        /// </summary>
        public static CBORValue ToCBOR(this SignedPercentage Value) => new MetrologicalValue(Value.Value, UnitOfMeasure.Percent).ToCBOR();

        public static Boolean TryParseSignedPercentage(CBORValue CBOR, out SignedPercentage Value, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Value = default;

            if (!TryParsePercent(CBOR, out var percent, out ErrorResponse))
                return false;

            if (!SignedPercentage.TryParse(percent, out Value))
            {
                ErrorResponse = $"Invalid signed percentage '{percent}'!";
                return false;
            }

            return true;

        }

        #endregion

        #region Quantities: QuantityToCBOR(Value, Unit), TryParseQuantity(CBOR, Unit, out Value, out ErrorResponse)

        /// <summary>
        /// A number in a unit that has no type of its own - a ramp rate in % per
        /// second - as a metrological value of the given unit.
        /// </summary>
        /// <param name="Value">The number.</param>
        /// <param name="Unit">Its unit, without a prefix.</param>
        public static CBORValue QuantityToCBOR(Decimal         Value,
                                               UnitExpression  Unit)

            => new MetrologicalValue(Value, Unit).ToCBOR();


        /// <summary>
        /// Try to read the given CBOR value as a number in the given unit: a
        /// metrological value of that unit, with or without a prefix.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Unit">The unit, without a prefix.</param>
        /// <param name="Value">The number in the unit.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseQuantity(CBORValue                         CBOR,
                                               UnitExpression                    Unit,
                                               out Decimal                       Value,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!metrologicalValue.TryToBaseUnit(out var baseValue) ||
                 baseValue.Unit != Unit)
            {
                ErrorResponse = $"The value '{metrologicalValue}' is not in {Unit}!";
                return false;
            }

            Value = baseValue.Value;
            return true;

        }

        /// <summary>
        /// % per second: a ramp rate.
        /// </summary>
        public static UnitExpression PercentPerSecond { get; }

            = new (new UnitFactor(UnitOfMeasure.Percent, 1),
                   new UnitFactor(UnitOfMeasure.Second, -1));

        #endregion

        #region BASE64: BASE64AsBytes(Text)

        /// <summary>
        /// A text that is BASE64 in JSON - a signature - as the bytes it encodes,
        /// or, when it is no BASE64, as the text it is.
        /// </summary>
        /// <param name="Text">A BASE64 text.</param>
        public static CBORValue? BASE64AsBytes(String? Text)
        {

            if (Text is null)
                return null;

            var bytes = new Byte[Text.Length];

            return Convert.TryFromBase64String(Text, bytes, out var length) &&
                   Convert.ToBase64String(bytes, 0, length) == Text

                       ? CBORValue.FromBytes(bytes[..length])
                       : CBORValue.FromText(Text);

        }

        #endregion

        #region Rates:  RateToCBOR(Amount, PerUnit, PerExponent), TryParseRate(CBOR, PerUnit, PerExponent, out Amount, out ErrorResponse)

        /// <summary>
        /// An amount per unit - a price per kWh, per minute - as a metrological value
        /// of the unit to the power of -1, per the unit itself and without a prefix,
        /// so that nobody has to guess what a prefix of a reciprocal unit means:
        /// 0.30 per kWh is 0.0003 Wh^-1. What the amount is of (a currency) is
        /// said elsewhere.
        /// </summary>
        /// <param name="Amount">The amount per (10^PerExponent) units.</param>
        /// <param name="PerUnit">The unit the amount is per.</param>
        /// <param name="PerExponent">The power of ten of the unit the amount is per: 3 for per kWh.</param>
        public static CBORValue RateToCBOR(Decimal        Amount,
                                           UnitOfMeasure  PerUnit,
                                           Int32          PerExponent = 0)

            => new MetrologicalValue(
                   Amount / MathHelpers.Pow10(PerExponent),
                   new UnitExpression(new UnitFactor(PerUnit, -1))
               ).ToCBOR();


        /// <summary>
        /// Try to read the given CBOR value as an amount per unit: a metrological value
        /// of the unit to the power of -1.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="PerUnit">The unit the amount is per.</param>
        /// <param name="PerExponent">The power of ten of the unit the amount is per: 3 for per kWh.</param>
        /// <param name="Amount">The amount per (10^PerExponent) units.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseRate(CBORValue                         CBOR,
                                           UnitOfMeasure                     PerUnit,
                                           Int32                             PerExponent,
                                           out Decimal                       Amount,
                                           [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Amount = default;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!metrologicalValue.TryToBaseUnit(out var baseValue) ||
                 baseValue.Unit != new UnitExpression(new UnitFactor(PerUnit, -1)))
            {
                ErrorResponse = $"The value '{metrologicalValue}' is not per {PerUnit}!";
                return false;
            }

            // Without the zeros the division by the power of ten left behind.
            Amount = baseValue.Value * MathHelpers.Pow10(PerExponent) / 1.0000000000000000000000000000m;
            return true;

        }

        #endregion

        #region Mandatory values: ParseMandatoryValue<T>(CBOR, PropertyName, PropertyDescription, TryParse, out Value, out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory property with a parser of a value type.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The text key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="TryParse">How to read the property.</param>
        /// <param name="Value">The value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseMandatoryValue<T>(this CBORValue                    CBOR,
                                                     String                            PropertyName,
                                                     String                            PropertyDescription,
                                                     TryCBORStructParser<T>            TryParse,
                                                     out T                             Value,
                                                     [NotNullWhen(false)] out String?  ErrorResponse)

            where T : struct

            => CBOR.ParseMandatory(PropertyName,
                                   PropertyDescription,
                                   (CBORValue property, out T parsed, out String? errorResponse) => TryParse(property, out parsed, out errorResponse),
                                   out Value,
                                   out ErrorResponse);

        #endregion

        #region Optional values: ParseOptionalValue<T>(CBOR, PropertyName, PropertyDescription, TryParse, out Value, out ErrorResponse)

        /// <summary>
        /// Parse the given optional property into a nullable value - which
        /// Styx's ParseOptional of an unconstrained T cannot tell from default.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The text key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="TryParse">How to read the property.</param>
        /// <param name="Value">The value, or null when the property is not there.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <returns>Whether the property is there - an error response tells whether it was read.</returns>
        public static Boolean ParseOptionalValue<T>(this CBORValue          CBOR,
                                                    String                  PropertyName,
                                                    String                  PropertyDescription,
                                                    TryCBORStructParser<T>  TryParse,
                                                    out T?                  Value,
                                                    out String?             ErrorResponse)

            where T : struct

        {

            Value = null;

            if (!CBOR.ParseOptional(PropertyName,
                                    PropertyDescription,
                                    (CBORValue property, out T parsed, out String? errorResponse) => TryParse(property, out parsed, out errorResponse),
                                    out T parsedValue,
                                    out ErrorResponse))
            {
                return ErrorResponse is not null;
            }

            Value = parsedValue;
            return true;

        }

        #endregion

        #region Lists: ParseMandatoryList<T>(...), ParseOptionalList<T>(...)

        /// <summary>
        /// Read every item of the given array with the given parser.
        /// </summary>
        private static Boolean TryParseList<T>(IReadOnlyList<CBORValue>          Items,
                                               String                            PropertyName,
                                               TryCBORParser<T>                  TryParse,
                                               out List<T>                       Values,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Values         = new List<T>(Items.Count);
            ErrorResponse  = null;

            for (var i = 0; i < Items.Count; i++)
            {

                if (!TryParse(Items[i], out var value, out var errorResponse))
                {
                    ErrorResponse = $"CBOR property '{PropertyName}', item {i}: {errorResponse}";
                    return false;
                }

                Values.Add(value!);

            }

            return true;

        }


        /// <summary>
        /// Parse the given mandatory array property, every item with the given parser.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The text key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="TryParse">How to read each item.</param>
        /// <param name="Values">The items read.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseMandatoryList<T>(this CBORValue                    CBOR,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    TryCBORParser<T>                  TryParse,
                                                    out List<T>                       Values,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Values = [];

            if (!CBOR.ParseMandatoryArray(PropertyName, PropertyDescription, out var items, out ErrorResponse))
                return false;

            return TryParseList(items, PropertyName, TryParse, out Values, out ErrorResponse);

        }


        /// <summary>
        /// Parse the given optional array property, every item with the given parser.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The text key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="TryParse">How to read each item.</param>
        /// <param name="Values">The items read - none when the property is not there.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <returns>Whether the property is there - an error response tells whether it was read.</returns>
        public static Boolean ParseOptionalList<T>(this CBORValue    CBOR,
                                                   String            PropertyName,
                                                   String            PropertyDescription,
                                                   TryCBORParser<T>  TryParse,
                                                   out List<T>       Values,
                                                   out String?       ErrorResponse)
        {

            Values         = [];
            ErrorResponse  = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR value is not a map!";
                return true;
            }

            if (!CBOR.TryGetValue(CBORValue.FromText(PropertyName), out _))
                return false;

            if (!CBOR.ParseMandatoryArray(PropertyName, PropertyDescription, out var items, out ErrorResponse))
                return true;

            TryParseList(items, PropertyName, TryParse, out Values, out ErrorResponse);
            return true;

        }

        #endregion

        #region Signatures: ToCBOR(Signature), TryParseSignature(CBOR, out Signature, out ErrorResponse)

        /// <summary>
        /// The given cryptographic signature as CBOR: the keys of its JSON object,
        /// its key identification and value as byte strings - what the JSON has
        /// to encode as text - and what is not its default only.
        /// </summary>
        /// <param name="Signature">A cryptographic signature.</param>
        public static CBORValue ToCBOR(this Signature Signature)

            => Map(
                   ("keyId",           CBORValue.FromBytes(Signature.KeyId)),
                   ("value",           CBORValue.FromBytes(Signature.Value)),
                   ("signingMethod",   Signature.SigningMethod != CryptoSigningMethod.JSON      ? (CBORValue?) CBORValue.FromText(Signature.SigningMethod.ToString()) : null),
                   ("encodingMethod",  Signature.Encoding      != CryptoEncoding.     BASE64    ? (CBORValue?) CBORValue.FromText(Signature.Encoding.     ToString()) : null),
                   ("algorithm",       Signature.Algorithm     != CryptoAlgorithm.    Secp256r1 ? (CBORValue?) CBORValue.FromText(Signature.Algorithm.    ToString()) : null),
                   ("name",            Text(Signature.Name.IsNotNullOrEmpty() ? Signature.Name : null)),
                   ("description",     Signature.Description is not null && Signature.Description.IsNotNullOrEmpty()
                                           ? (CBORValue?) CBORValue.FromMap(Signature.Description.Select(text => new KeyValuePair<CBORValue, CBORValue>(CBORValue.FromText(text.Language.ToString()), CBORValue.FromText(text.Text))))
                                           : null),
                   ("timestamp",       Signature.Timestamp?.ToCBOR()),
                   ("customData",      Signature.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a cryptographic signature.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Signature">The cryptographic signature.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseSignature(CBORValue                            CBOR,
                                                [NotNullWhen(true)]  out Signature?  Signature,
                                                [NotNullWhen(false)] out String?     ErrorResponse)
        {

            Signature = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR representation of a signature is not a map!";
                return false;
            }

            if (!CBOR.ParseMandatoryBytes("keyId", "key identification", out var keyId, out ErrorResponse) ||
                !CBOR.ParseMandatoryBytes("value", "signature value",    out var value, out ErrorResponse))
            {
                return false;
            }

            CryptoSigningMethod? signingMethod = null;
            CryptoEncoding?      encoding      = null;
            CryptoAlgorithm?     algorithm     = null;

            if (CBOR.ParseOptionalText("signingMethod", "signing method", out var signingMethodText, out ErrorResponse))
            {
                if (!CryptoSigningMethod.TryParse(signingMethodText!, out var parsed)) { ErrorResponse = $"Invalid signing method '{signingMethodText}'!"; return false; }
                signingMethod = parsed;
            }
            else if (ErrorResponse is not null)
                return false;

            if (CBOR.ParseOptionalText("encodingMethod", "encoding method", out var encodingText, out ErrorResponse))
            {
                if (!CryptoEncoding.TryParse(encodingText!, out var parsed)) { ErrorResponse = $"Invalid encoding method '{encodingText}'!"; return false; }
                encoding = parsed;
            }
            else if (ErrorResponse is not null)
                return false;

            if (CBOR.ParseOptionalText("algorithm", "crypto algorithm", out var algorithmText, out ErrorResponse))
            {
                if (!CryptoAlgorithm.TryParse(algorithmText!, out var parsed)) { ErrorResponse = $"Invalid crypto algorithm '{algorithmText}'!"; return false; }
                algorithm = parsed;
            }
            else if (ErrorResponse is not null)
                return false;

            CBOR.ParseOptionalText("name", "name", out var name, out ErrorResponse);
            if (ErrorResponse is not null)
                return false;

            I18NString? description = null;

            if (CBOR.TryGetValue(CBORValue.FromText("description"), out var descriptionCBOR))
            {

                if (descriptionCBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The description of a signature is not a map of texts by language!";
                    return false;
                }

                description = I18NString.Empty;

                foreach (var entry in descriptionCBOR.AsMap())
                {
                    if (!Enum.TryParse<Languages>(entry.Key.AsText(), ignoreCase: true, out var language))
                    {
                        ErrorResponse = $"Invalid language '{entry.Key.AsText()}' of a description!";
                        return false;
                    }
                    description = description.Set(language, entry.Value.AsText());
                }

            }

            CBOR.ParseOptionalValue("timestamp", "timestamp", TryParseTimestamp, out DateTimeOffset? timestamp, out ErrorResponse);
            if (ErrorResponse is not null)
                return false;

            CBOR.ParseOptional("customData", "custom data", TryParseCustomData, out CustomData? customData, out ErrorResponse);
            if (ErrorResponse is not null)
                return false;

            Signature = new Signature(
                            keyId,
                            value,
                            algorithm,
                            signingMethod,
                            encoding,
                            name,
                            description,
                            timestamp,
                            customData
                        );

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region CustomData: ToCBOR(CustomData), TryParseCustomData(CBOR, out CustomData, out ErrorResponse)

        /// <summary>
        /// The given custom data as CBOR: its JSON object, converted by Styx's
        /// CBORJSON, so that every value keeps its kind.
        /// </summary>
        /// <param name="CustomData">Custom data.</param>
        public static CBORValue ToCBOR(this CustomData CustomData)

            => CBORJSON.ToCBOR(CustomData.ToJSON());


        /// <summary>
        /// Try to read the given CBOR value as custom data.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="CustomData">The custom data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCustomData(CBORValue                             CBOR,
                                                 [NotNullWhen(true)] out CustomData?   CustomData,
                                                 [NotNullWhen(false)] out String?      ErrorResponse)
        {

            CustomData = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given custom data is not a CBOR map!";
                return false;
            }

            try
            {
                return WWCP.CustomData.TryParse((JObject) CBORJSON.ToJSON(CBOR), out CustomData, out ErrorResponse);
            }
            catch (Exception e)
            {
                ErrorResponse = "The given custom data could not be read: " + e.Message;
                return false;
            }

        }

        #endregion

    }


    /// <summary>
    /// Reads a CBOR value as a value type.
    /// </summary>
    public delegate Boolean TryCBORStructParser<T>(CBORValue                         CBOR,
                                                   out T                             Value,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)

        where T : struct;

}
