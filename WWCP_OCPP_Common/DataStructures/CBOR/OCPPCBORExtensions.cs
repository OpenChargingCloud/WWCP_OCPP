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
