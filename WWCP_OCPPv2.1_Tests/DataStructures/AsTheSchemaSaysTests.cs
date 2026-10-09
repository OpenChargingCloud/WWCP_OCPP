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

using System.Globalization;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Data structures read in the shape the official OCPP 2.1 (Edition 2)
    /// JSON schemas give them, and written back in exactly that shape.
    /// </summary>
    /// <remarks>
    /// Every sample below has every property its schema object has, named as
    /// the schema names it. Read and written again, it has to come out the same:
    /// the same keys, nothing left out and nothing added, and the same values -
    /// numbers compared as numbers and timestamps as instants, so that a value
    /// written in another unit (kW for W) is as much a failure as a key written
    /// under another name.
    /// </remarks>
    [TestFixture]
    public class AsTheSchemaSaysTests
    {

        #region Data

        public delegate Boolean Parser<T>(JObject JSON, out T? Value, out String? ErrorResponse);

        internal const String Custom = """{ "vendorId": "GraphDefined" }""";

        #endregion

        #region (private static) Normalized(Token)

        /// <summary>
        /// A JSON tree with every number as a decimal and every timestamp as a
        /// UTC instant, so that 7 and 7.0 and two ways of writing one instant
        /// compare equal - and nothing else does.
        /// </summary>
        internal static JToken Normalized(JToken Token)

            => Token switch {

                   JObject json  => new JObject(json.Properties().OrderBy(property => property.Name, StringComparer.Ordinal).
                                                     Select(property => new JProperty(property.Name, Normalized(property.Value)))),

                   JArray array  => new JArray(array.Select(Normalized)),

                   JValue value when value.Type is JTokenType.Integer or JTokenType.Float
                                 => new JValue(Convert.ToDecimal(value.Value, CultureInfo.InvariantCulture)),

                   JValue value when value.Type == JTokenType.Date
                                 => new JValue(((DateTimeOffset) (value.Value is DateTime dateTime ? new DateTimeOffset(dateTime) : (DateTimeOffset) value.Value!)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),

                   JValue value when value.Type == JTokenType.String &&
                                     value.Value<String>()!.Contains('T') &&
                                     DateTimeOffset.TryParse(value.Value<String>(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var instant)
                                 => new JValue(instant.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),

                   _ => Token.DeepClone()

               };

        #endregion

        #region (private static) ReadAndWrittenAsTheSchemaSays(Sample, TryParse, ToJSON)

        /// <summary>
        /// The sample is read, and what is written from it is the sample.
        /// </summary>
        /// <param name="Sample">A JSON object in the shape the schema gives it, with every property it has.</param>
        /// <param name="TryParse">The parser.</param>
        /// <param name="ToJSON">The serializer.</param>
        /// <param name="Read">What has to be true of what was read - for a value that a round trip alone would not catch, such as one read and written in the same wrong unit.</param>
        internal static void ReadAndWrittenAsTheSchemaSays<T>(String           Sample,
                                                             Parser<T>        TryParse,
                                                             Func<T, JObject> ToJSON,
                                                             Action<T>?       Read   = null)
        {

            var sample = JObject.Parse(Sample);

            Assert.That(TryParse(sample, out var value, out var errorResponse), Is.True,
                        $"The sample in the schema's shape was not read: {errorResponse}");

            Read?.Invoke(value!);

            var written = ToJSON(value!);

            Assert.That(JToken.DeepEquals(Normalized(written), Normalized(sample)), Is.True,
                        $"Written:  {Normalized(written).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Expected: {Normalized(sample). ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

        #region (internal static) ReadAndWrittenAsTheSchemaSaysAndAsCBOR(Sample, TryParse, ToJSON, Read = null, KeysInCBOR = null)

        /// <summary>
        /// The sample is read and written as the schema says - and what was read
        /// is written as CBOR and read back from its bytes as the same: written
        /// as JSON again it is the sample, and its maps have the keys of the
        /// sample's objects.
        /// </summary>
        /// <param name="Sample">A JSON object in the shape the schema gives it, with every property it has.</param>
        /// <param name="TryParse">The JSON parser.</param>
        /// <param name="ToJSON">The JSON serializer.</param>
        /// <param name="Read">What has to be true of what was read.</param>
        /// <param name="KeysInCBOR">A key of the JSON objects that is another one in CBOR - one whose suffix names a unit there.</param>
        internal static void ReadAndWrittenAsTheSchemaSaysAndAsCBOR<T>(String                       Sample,
                                                                      Parser<T>                    TryParse,
                                                                      Func<T, JObject>             ToJSON,
                                                                      Action<T>?                   Read         = null,
                                                                      IDictionary<String, String>? KeysInCBOR   = null)

            where T : ICBORSerializable<T>

        {

            ReadAndWrittenAsTheSchemaSays(Sample, TryParse, ToJSON, Read);

            var sample = JObject.Parse(Sample);
            TryParse(sample, out var value, out _);

            var bytes  = value!.ToCBOR().ToByteArray();
            var cbor   = CBORValue.Parse(bytes);

            Assert.That(T.TryParse(cbor, out var fromCBOR, out var errorResponse), Is.True,
                        $"What was written as CBOR was not read: {errorResponse}{Environment.NewLine}{cbor.ToDiagnosticString()}");

            Read?.Invoke(fromCBOR);

            var written = ToJSON(fromCBOR);

            Assert.That(JToken.DeepEquals(Normalized(written), Normalized(sample)), Is.True,
                        $"Read from CBOR: {Normalized(written).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Expected:       {Normalized(sample). ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"CBOR:           {cbor.ToDiagnosticString()}");

            SameKeys(sample, cbor, "", KeysInCBOR ?? new Dictionary<String, String>());

        }

        /// <summary>
        /// The keys of every object of the JSON are the keys of every map of the
        /// CBOR, but for those that name a unit in JSON.
        /// </summary>
        private static void SameKeys(JToken JSON, CBORValue CBOR, String Path, IDictionary<String, String> KeysInCBOR)
        {

            if (JSON is JObject jsonObject)
            {

                // A metrological value or a timestamp is one tagged value in CBOR, whatever it is in JSON.
                if (CBOR.Kind != CBORValueKind.Map)
                    return;

                var expected = jsonObject.Properties().Select(property => KeysInCBOR.TryGetValue(property.Name, out var key) ? key : property.Name).OrderBy(key => key, StringComparer.Ordinal).ToArray();
                var actual   = CBOR.AsMap().Select(entry => entry.Key.AsText()).OrderBy(key => key, StringComparer.Ordinal).ToArray();

                Assert.That(actual, Is.EqualTo(expected), $"The keys of the map at '{Path}'");

                foreach (var property in jsonObject.Properties())
                    if (CBOR.TryGetValue(CBORValue.FromText(KeysInCBOR.TryGetValue(property.Name, out var key) ? key : property.Name), out var value))
                        SameKeys(property.Value,
                                 value,
                                 $"{Path}/{property.Name}",
                                 KeysInCBOR);

            }

            else if (JSON is JArray jsonArray && CBOR.Kind == CBORValueKind.Array)
            {
                var cborArray = CBOR.AsArray();
                Assert.That(cborArray.Count, Is.EqualTo(jsonArray.Count), $"The items of the array at '{Path}'");
                for (var i = 0; i < jsonArray.Count; i++)
                    SameKeys(jsonArray[i], cborArray[i], $"{Path}[{i}]", KeysInCBOR);
            }

        }

        #endregion


        #region CompositeSchedule

        [Test]
        public void CompositeSchedule()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<CompositeSchedule>(
                   $$"""
                   {
                       "evseId":                 1,
                       "duration":               3600,
                       "scheduleStart":          "2026-10-09T12:00:00Z",
                       "chargingRateUnit":       "W",
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 11000 } ],
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.CompositeSchedule.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region Hysteresis

        [Test]
        public void Hysteresis()

            => ReadAndWrittenAsTheSchemaSays<Hysteresis>(
                   $$"""
                   {
                       "hysteresisHigh":     50.2,
                       "hysteresisLow":      49.8,
                       "hysteresisDelay":    30,
                       "hysteresisGradient": 0.5,
                       "customData":         {{Custom}}
                   }
                   """,
                   OCPPv2_1.Hysteresis.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// Every value of a hysteresis is optional.
        /// </summary>
        [Test]
        public void Hysteresis_Empty()

            => ReadAndWrittenAsTheSchemaSays<Hysteresis>(
                   "{ }",
                   OCPPv2_1.Hysteresis.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region EVPriceRule, EVPowerScheduleEntry, EVPowerSchedule, EVAbsolutePriceScheduleEntry, EVAbsolutePriceSchedule

        /// <summary>
        /// Power is in W in OCPP 2.1 - "in kW" appears nowhere in its specification.
        /// </summary>
        [Test]
        public void EVPriceRule()

            => ReadAndWrittenAsTheSchemaSays<EVPriceRule>(
                   $$"""
                   { "energyFee": 0.39, "powerRangeStart": 11000, "customData": {{Custom}} }
                   """,
                   OCPPv2_1.EVPriceRule.TryParse,
                   value => value.ToJSON(),
                   value => Assert.That(value.PowerRangeStart.Value, Is.EqualTo(11000m), "11000 was not read as 11 kW")
               );

        [Test]
        public void EVPowerScheduleEntry()

            => ReadAndWrittenAsTheSchemaSays<EVPowerScheduleEntry>(
                   $$"""
                   { "duration": 900, "power": -7400, "customData": {{Custom}} }
                   """,
                   OCPPv2_1.EVPowerScheduleEntry.TryParse,
                   value => value.ToJSON(),
                   value => Assert.That(value.Power.Value, Is.EqualTo(-7400m), "-7400 was not read as 7.4 kW of discharge")
               );

        [Test]
        public void EVPowerSchedule()

            => ReadAndWrittenAsTheSchemaSays<EVPowerSchedule>(
                   $$"""
                   {
                       "evPowerScheduleEntries": [ { "duration": 900, "power": -7400 } ],
                       "timeAnchor":             "2026-10-09T12:00:00Z",
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.EVPowerSchedule.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void EVAbsolutePriceScheduleEntry()

            => ReadAndWrittenAsTheSchemaSays<EVAbsolutePriceScheduleEntry>(
                   $$"""
                   {
                       "duration":    900,
                       "evPriceRule": [ { "energyFee": 0.39, "powerRangeStart": 0 } ],
                       "customData":  {{Custom}}
                   }
                   """,
                   OCPPv2_1.EVAbsolutePriceScheduleEntry.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void EVAbsolutePriceSchedule()

            => ReadAndWrittenAsTheSchemaSays<EVAbsolutePriceSchedule>(
                   $$"""
                   {
                       "timeAnchor":                     "2026-10-09T12:00:00Z",
                       "currency":                       "EUR",
                       "evAbsolutePriceScheduleEntries": [ { "duration": 900, "evPriceRule": [ { "energyFee": 0.39, "powerRangeStart": 0 } ] } ],
                       "priceAlgorithm":                 "urn:iso:std:iso:15118:-20:PriceAlgorithm:1-Power",
                       "customData":                     {{Custom}}
                   }
                   """,
                   OCPPv2_1.EVAbsolutePriceSchedule.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region MessageContent, MessageInfo

        [Test]
        public void MessageContent()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<MessageContent>(
                   $$"""
                   { "format": "UTF8", "language": "de", "content": "Willkommen", "customData": {{Custom}} }
                   """,
                   OCPPv2_1.MessageContent.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// The format of a message content is required.
        /// </summary>
        [Test]
        public void MessageContent_WithoutFormat_IsRefused()
        {
            Assert.That(OCPPv2_1.MessageContent.TryParse(JObject.Parse("""{ "content": "Willkommen" }"""), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);
        }

        [Test]
        public void MessageInfo()

            => ReadAndWrittenAsTheSchemaSays<MessageInfo>(
                   $$"""
                   {
                       "display":        { "name": "DisplayMessageCtrlr", "instance": "front" },
                       "id":             7,
                       "priority":       "AlwaysFront",
                       "state":          "Idle",
                       "startDateTime":  "2026-10-09T12:00:00Z",
                       "endDateTime":    "2026-10-10T12:00:00Z",
                       "transactionId":  "tx-1",
                       "message":        { "format": "UTF8", "content": "Willkommen" },
                       "messageExtra":   [ { "format": "UTF8", "language": "en", "content": "Welcome" } ],
                       "customData":     {{Custom}}
                   }
                   """,
                   OCPPv2_1.MessageInfo.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region RationalNumber, StreamDataElement, StatusInfo

        [Test]
        public void RationalNumber()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<RationalNumber>(
                   $$"""
                   { "exponent": -2, "value": 3900, "customData": {{Custom}} }
                   """,
                   OCPPv2_1.RationalNumber.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void StreamDataElement()

            => ReadAndWrittenAsTheSchemaSays<StreamDataElement>(
                   $$"""
                   { "t": 1.5, "v": "230.1", "customData": {{Custom}} }
                   """,
                   OCPPv2_1.StreamDataElement.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void StatusInfo()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<StatusInfo>(
                   $$"""
                   { "reasonCode": "NoError", "additionalInfo": "fine", "customData": {{Custom}} }
                   """,
                   OCPPv2_1.StatusInfo.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region TariffAssignment

        [Test]
        public void TariffAssignment()

            => ReadAndWrittenAsTheSchemaSays<TariffAssignment>(
                   $$"""
                   {
                       "tariffId":   "tariff-1",
                       "tariffKind": "DefaultTariff",
                       "validFrom":  "2026-10-09T12:00:00Z",
                       "evseIds":    [ 1, 2 ],
                       "idTokens":   [ "04A2112233" ],
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.TariffAssignment.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region TaxRate, Price

        [Test]
        public void TaxRate()

            => ReadAndWrittenAsTheSchemaSays<TaxRate>(
                   $$"""
                   { "type": "VAT", "tax": 19, "stack": 0, "customData": {{Custom}} }
                   """,
                   (JObject json, out TaxRate value, out String? errorResponse) => OCPPv2_1.TaxRate.TryParse(json, out value, out errorResponse),
                   value => value.ToJSON()
               );

        [Test]
        public void Price()

            => ReadAndWrittenAsTheSchemaSays<Price>(
                   $$"""
                   {
                       "exclTax":    10,
                       "inclTax":    11.9,
                       "taxRates":   [ { "type": "VAT", "tax": 19 } ],
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out Price value, out String? errorResponse) => OCPPv2_1.Price.TryParse(json, out value, out errorResponse),
                   value => value.ToJSON()
               );

        /// <summary>
        /// Either of the two values may be absent, as long as the other is there.
        /// </summary>
        [Test]
        public void Price_OnlyIncludingTaxes()

            => ReadAndWrittenAsTheSchemaSays<Price>(
                   """{ "inclTax": 11.9 }""",
                   (JObject json, out Price value, out String? errorResponse) => OCPPv2_1.Price.TryParse(json, out value, out errorResponse),
                   value => value.ToJSON()
               );

        [Test]
        public void Price_WithNeitherValue_IsRefused()
        {
            Assert.That(OCPPv2_1.Price.TryParse(JObject.Parse("""{ "taxRates": [ { "type": "VAT", "tax": 19 } ] }"""), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);
        }

        #endregion

        #region TariffConditions, TariffEnergy, TariffTime, TariffFixed, ClearTariffsResult

        private const String Conditions = """
            {
                "startTimeOfDay":  "08:00",
                "endTimeOfDay":    "18:30",
                "dayOfWeek":       [ "Monday", "Friday" ],
                "validFromDate":   "2026-10-01",
                "validToDate":     "2026-12-31",
                "evseKind":        "AC",
                "minEnergy":       20000,
                "maxEnergy":       50000,
                "minCurrent":      5,
                "maxCurrent":      32,
                "minPower":        5000,
                "maxPower":        22000,
                "minTime":         600,
                "maxTime":         7200,
                "minChargingTime": 300,
                "maxChargingTime": 3600,
                "minIdleTime":     60,
                "maxIdleTime":     900,
                "customData":      { "vendorId": "GraphDefined" }
            }
            """;

        [Test]
        public void TariffConditions()

            => ReadAndWrittenAsTheSchemaSays<TariffConditions>(
                   Conditions,
                   OCPPv2_1.TariffConditions.TryParse,
                   value => value.ToJSON()!
               );

        [Test]
        public void TariffEnergy()

            => ReadAndWrittenAsTheSchemaSays<TariffEnergy>(
                   $$"""
                   {
                       "prices":     [ { "priceKwh": 0.39, "conditions": {{Conditions}}, "customData": {{Custom}} } ],
                       "taxRates":   [ { "type": "VAT", "tax": 19 } ],
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.TariffEnergy.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void TariffTime()

            => ReadAndWrittenAsTheSchemaSays<TariffTime>(
                   $$"""
                   {
                       "prices":     [ { "priceMinute": 0.05, "conditions": { "minIdleTime": 60 }, "customData": {{Custom}} } ],
                       "taxRates":   [ { "type": "VAT", "tax": 19 } ],
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.TariffTime.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void TariffFixed()

            => ReadAndWrittenAsTheSchemaSays<TariffFixed>(
                   $$"""
                   {
                       "prices":     [ { "priceFixed": 1.5, "conditions": { "dayOfWeek": [ "Sunday" ], "evseKind": "DC" }, "customData": {{Custom}} } ],
                       "taxRates":   [ { "type": "VAT", "tax": 19 } ],
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.TariffFixed.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ClearTariffsResult()

            => ReadAndWrittenAsTheSchemaSays<ClearTariffsResult>(
                   $$"""
                   {
                       "statusInfo": { "reasonCode": "NoTariff" },
                       "tariffId":   "tariff-1",
                       "status":     "Accepted",
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.ClearTariffsResult.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region ChargingSchedule, ChargingNeeds, DERChargingParameters

        [Test]
        public void ChargingSchedule()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingSchedule>(
                   $$"""
                   {
                       "id":                     1,
                       "limitAtSoC":             { "soc": 80, "limit": 7400 },
                       "chargingRateUnit":       "W",
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 11000 } ],
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingSchedule.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// A number in the key the supported controls were read from refused the whole object.
        /// </summary>
        [Test]
        public void DERChargingParameters()

            => ReadAndWrittenAsTheSchemaSays<DERChargingParameters>(
                   $$"""
                   {
                       "evSupportedDERControl":          [ "FreqDroop", "FixedPFInject" ],
                       "evOverExcitedMaxDischargePower": 7000,
                       "evInverterManufacturer":         "GraphDefined",
                       "customData":                     {{Custom}}
                   }
                   """,
                   OCPPv2_1.DERChargingParameters.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ChargingNeeds()

            => ReadAndWrittenAsTheSchemaSays<ChargingNeeds>(
                   $$"""
                   {
                       "requestedEnergyTransfer": "DC",
                       "availableEnergyTransfer": [ "DC", "AC_three_phase" ],
                       "derChargingParameters":   { "evSupportedDERControl": [ "FreqDroop" ] },
                       "customData":              {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingNeeds.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region ISO 15118-20 price schedules

        private const String Rational = """{ "exponent": -2, "value": 39 }""";

        [Test]
        public void PriceRule()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ISO15118_20.CommonMessages.PriceRule>(
                   $$"""
                   {
                       "parkingFeePeriod":              900,
                       "carbonDioxideEmission":         300,
                       "renewableGenerationPercentage": 60,
                       "energyFee":                     {{Rational}},
                       "parkingFee":                    {{Rational}},
                       "powerRangeStart":               { "exponent": 3, "value": 11 },
                       "customData":                    {{Custom}}
                   }
                   """,
                   ISO15118_20.CommonMessages.PriceRule.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void PriceRuleStack()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ISO15118_20.CommonMessages.PriceRuleStack>(
                   $$"""
                   {
                       "duration":   3600,
                       "priceRule":  [ { "energyFee": {{Rational}}, "powerRangeStart": { "exponent": 0, "value": 0 } } ],
                       "customData": {{Custom}}
                   }
                   """,
                   ISO15118_20.CommonMessages.PriceRuleStack.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void PriceLevelSchedule()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ISO15118_20.CommonMessages.PriceLevelSchedule>(
                   $$"""
                   {
                       "priceLevelScheduleEntries": [ { "duration": 3600, "priceLevel": 2, "customData": {{Custom}} } ],
                       "timeAnchor":                "2026-10-09T12:00:00Z",
                       "priceScheduleId":           7,
                       "priceScheduleDescription":  "night",
                       "numberOfPriceLevels":       4,
                       "customData":                {{Custom}}
                   }
                   """,
                   ISO15118_20.CommonMessages.PriceLevelSchedule.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void AbsolutePriceSchedule()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ISO15118_20.CommonMessages.AbsolutePriceSchedule>(
                   $$"""
                   {
                       "timeAnchor":                 "2026-10-09T12:00:00Z",
                       "priceScheduleID":            7,
                       "priceScheduleDescription":   "day",
                       "currency":                   "EUR",
                       "language":                   "de",
                       "priceAlgorithm":             "urn:iso:std:iso:15118:-20:PriceAlgorithm:1-Power",
                       "minimumCost":                {{Rational}},
                       "maximumCost":                { "exponent": 0, "value": 50 },
                       "priceRuleStacks":            [ { "duration": 3600, "priceRule": [ { "energyFee": {{Rational}}, "powerRangeStart": { "exponent": 0, "value": 0 } } ] } ],
                       "taxRules":                   [ {
                                                         "taxRuleID":                   1,
                                                         "taxRuleName":                 "VAT",
                                                         "taxIncludedInPrice":          true,
                                                         "appliesToEnergyFee":          true,
                                                         "appliesToParkingFee":         true,
                                                         "appliesToOverstayFee":        false,
                                                         "appliesToMinimumMaximumCost": true,
                                                         "taxRate":                     { "exponent": -2, "value": 19 },
                                                         "customData":                  {{Custom}}
                                                     } ],
                       "overstayRuleList":           {
                                                         "overstayPowerThreshold": { "exponent": 3, "value": 1 },
                                                         "overstayRule":           [ {
                                                                                       "overstayFee":             {{Rational}},
                                                                                       "overstayRuleDescription": "after 4 h",
                                                                                       "startTime":               14400,
                                                                                       "overstayFeePeriod":       900,
                                                                                       "customData":              {{Custom}}
                                                                                   } ],
                                                         "overstayTimeThreshold":  14400,
                                                         "customData":             {{Custom}}
                                                     },
                       "additionalSelectedServices": [ { "serviceFee": {{Rational}}, "serviceName": "WiFi", "customData": {{Custom}} } ],
                       "customData":                 {{Custom}}
                   }
                   """,
                   ISO15118_20.CommonMessages.AbsolutePriceSchedule.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region UnitOfMeasure - the texts of the specification

        /// <summary>
        /// The units are written as the specification's "Standardized Units of
        /// Measure" name them - W, A, V, VA, K, s - not Watts, Amperes, Voltage,
        /// VoltAmpere, Kelvin and TimeSpan, which no other side understands.
        /// </summary>
        [TestCase("W")]
        [TestCase("A")]
        [TestCase("V")]
        [TestCase("VA")]
        [TestCase("K")]
        [TestCase("s")]
        [TestCase("Hz")]
        [TestCase("kVAh")]
        public void UnitsOfMeasure_AsTheSpecificationNamesThem(String Unit)

            => ReadAndWrittenAsTheSchemaSays<UnitsOfMeasure>(
                   $$"""{ "unit": "{{Unit}}", "multiplier": 3 }""",
                   OCPPv2_1.UnitsOfMeasure.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void UnitOfMeasure_WritesTheTextsOfTheSpecification()
        {
            Assert.That(OCPPv2_1.UnitOfMeasure.Watts.     ToString(), Is.EqualTo("W"));
            Assert.That(OCPPv2_1.UnitOfMeasure.Amperes.   ToString(), Is.EqualTo("A"));
            Assert.That(OCPPv2_1.UnitOfMeasure.Voltage.   ToString(), Is.EqualTo("V"));
            Assert.That(OCPPv2_1.UnitOfMeasure.VoltAmpere.ToString(), Is.EqualTo("VA"));
            Assert.That(OCPPv2_1.UnitOfMeasure.Kelvin.    ToString(), Is.EqualTo("K"));
            Assert.That(OCPPv2_1.UnitOfMeasure.TimeSpan.  ToString(), Is.EqualTo("s"));
        }

        /// <summary>
        /// What WWCP_OCPP wrote before is still read as the unit it meant.
        /// </summary>
        [Test]
        public void UnitOfMeasure_WrittenTheOldWay_IsStillRead()
        {
            Assert.That(OCPPv2_1.UnitOfMeasure.Parse("Watts"),   Is.EqualTo(OCPPv2_1.UnitOfMeasure.Watts));
            Assert.That(OCPPv2_1.UnitOfMeasure.Parse("Amperes"), Is.EqualTo(OCPPv2_1.UnitOfMeasure.Amperes));
            Assert.That(OCPPv2_1.UnitOfMeasure.Parse("TimeSpan"),Is.EqualTo(OCPPv2_1.UnitOfMeasure.TimeSpan));
        }

        #endregion

        #region DEREnterService, ReactivePowerParameters - values Newtonsoft cannot write as they are

        /// <summary>
        /// Its voltages and frequencies were handed to the JSON as Volt and
        /// Hertz, which Newtonsoft cannot write: ToJSON() threw.
        /// </summary>
        [Test]
        public void DEREnterService()

            => ReadAndWrittenAsTheSchemaSays<DEREnterService>(
                   $$"""
                   {
                       "priority":    1,
                       "highVoltage": 253,
                       "lowVoltage":  207,
                       "highFreq":    50.2,
                       "lowFreq":     49.8,
                       "delay":       30,
                       "randomDelay": 10,
                       "rampRate":    60,
                       "customData":  {{Custom}}
                   }
                   """,
                   OCPPv2_1.DEREnterService.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// Its reference voltage was handed to the JSON as a Percentage: ToJSON() threw.
        /// </summary>
        [Test]
        public void ReactivePowerParameters()

            => ReadAndWrittenAsTheSchemaSays<ReactivePowerParameters>(
                   $$"""
                   {
                       "vRef":                       100,
                       "autonomousVRefEnable":       true,
                       "autonomousVRefTimeConstant": 300,
                       "customData":                 {{Custom}}
                   }
                   """,
                   OCPPv2_1.ReactivePowerParameters.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// Its operation mode was handed to the JSON as an OperationMode: ToJSON() threw.
        /// </summary>
        [Test]
        public void ChargingSchedulePeriod_WithAnOperationMode()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingSchedulePeriod>(
                   """{ "startPeriod": 0, "limit": 11000, "operationMode": "CentralSetpoint", "setpoint": 7400 }""",
                   OCPPv2_1.ChargingSchedulePeriod.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region IdTokenInfo, ReportData, NetworkConnectionProfile

        [Test]
        public void IdTokenInfo()

            => ReadAndWrittenAsTheSchemaSays<IdTokenInfo>(
                   $$"""
                   {
                       "status":              "Accepted",
                       "cacheExpiryDateTime": "2026-10-10T12:00:00Z",
                       "chargingPriority":    1,
                       "groupIdToken":        { "idToken": "GROUP1", "type": "Central" },
                       "language1":           "de",
                       "language2":           "en",
                       "evseId":              [ 1 ],
                       "personalMessage":     { "format": "UTF8", "content": "Hallo" },
                       "customData":          {{Custom}}
                   }
                   """,
                   OCPPv2_1.IdTokenInfo.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ReportData()

            => ReadAndWrittenAsTheSchemaSays<ReportData>(
                   $$"""
                   {
                       "component":         { "name": "OCPPCommCtrlr" },
                       "variable":          { "name": "HeartbeatInterval" },
                       "variableAttribute": [ { "type": "Actual", "value": "300", "mutability": "ReadOnly", "persistent": true, "constant": true } ],
                       "customData":        {{Custom}}
                   }
                   """,
                   OCPPv2_1.ReportData.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void NetworkConnectionProfile()

            => ReadAndWrittenAsTheSchemaSays<NetworkConnectionProfile>(
                   $$"""
                   {
                       "ocppInterface":     "Wired0",
                       "ocppTransport":     "JSON",
                       "messageTimeout":    30,
                       "ocppCsmsUrl":       "wss://csms.example.org/ocpp",
                       "securityProfile":   2,
                       "identity":          "CS01",
                       "basicAuthPassword": "0123456789abcdef",
                       "customData":        {{Custom}}
                   }
                   """,
                   OCPPv2_1.NetworkConnectionProfile.TryParse,
                   value => value.ToJSON()
               );

        #endregion

    }

}
