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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.DataTypes
{

    /// <summary>
    /// Data structures and messages read in the shape the official OCPP 1.6
    /// JSON schemas give them, and written back in exactly that shape - and
    /// the vendor extensions read back as they are written.
    /// </summary>
    [TestFixture]
    public class AsTheSchemaSaysTests
    {

        #region Data

        public delegate Boolean Parser<T>(JObject JSON, out T? Value, out String? ErrorResponse);

        private static readonly Request_Id     requestId    = Request_Id.Parse("1");
        private static readonly SourceRouting  destination  = SourceRouting.CSMS;

        #endregion

        #region (private static) Normalized(Token)

        /// <summary>
        /// A JSON tree with every number as a decimal and every timestamp as a
        /// UTC instant, and its keys in order.
        /// </summary>
        private static JToken Normalized(JToken Token)

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
        /// <param name="Read">What has to be true of what was read - for a value that a round trip alone would not catch.</param>
        private static void ReadAndWrittenAsTheSchemaSays<T>(String           Sample,
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


        #region LogParameters

        [Test]
        public void LogParameters()

            => ReadAndWrittenAsTheSchemaSays<LogParameters>(
                   """
                   {
                       "remoteLocation":  "https://example.org/logs",
                       "oldestTimestamp": "2026-10-08T12:00:00Z",
                       "latestTimestamp": "2026-10-09T12:00:00Z"
                   }
                   """,
                   OCPPv1_6.LogParameters.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region MeterValue

        [Test]
        public void MeterValue()

            => ReadAndWrittenAsTheSchemaSays<MeterValue>(
                   """
                   {
                       "timestamp":    "2026-10-09T12:00:00Z",
                       "sampledValue": [ { "value": "1234", "context": "Sample.Clock", "format": "Raw", "measurand": "Power.Active.Import", "phase": "L1", "location": "Outlet", "unit": "W" } ]
                   }
                   """,
                   OCPPv1_6.MeterValue.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// What a sampled value leaves out is what OCPP 1.6 says it is when it
        /// is absent - not the first value of each enumeration.
        /// </summary>
        [Test]
        public void SampledValue_WithOnlyItsValue_HasTheDefaultsOfOCPP()
        {

            Assert.That(OCPPv1_6.SampledValue.TryParse(JObject.Parse("""{ "value": "1234" }"""), out var sampledValue, out var errorResponse), Is.True, errorResponse);

            Assert.That(sampledValue!.Context,   Is.EqualTo(ReadingContexts.SamplePeriodic));
            Assert.That(sampledValue. Format,    Is.EqualTo(ValueFormats.Raw));
            Assert.That(sampledValue. Measurand, Is.EqualTo(Measurands.EnergyActiveImportRegister));
            Assert.That(sampledValue. Location,  Is.EqualTo(Locations.Outlet));
            Assert.That(sampledValue. Unit,      Is.EqualTo(UnitsOfMeasure.Wh));

        }

        [Test]
        public void MeterValue_WithoutSampledValue_IsRefused()

            => Assert.That(OCPPv1_6.MeterValue.TryParse(JObject.Parse("""{ "timestamp": "2026-10-09T12:00:00Z" }"""), out _, out _), Is.False);

        #endregion

        #region SendLocalListRequest

        [Test]
        public void SendLocalListRequest_WithoutItsOptionalList()

            => ReadAndWrittenAsTheSchemaSays<CS.SendLocalListRequest>(
                   """{ "listVersion": 1, "updateType": "Full" }""",
                   (JObject json, out CS.SendLocalListRequest? value, out String? errorResponse) => CS.SendLocalListRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region StartTransactionRequest / StopTransactionRequest - meter values in Wh

        [Test]
        public void StartTransactionRequest()

            => ReadAndWrittenAsTheSchemaSays<CP.StartTransactionRequest>(
                   """
                   {
                       "connectorId":   1,
                       "idTag":         "AABBCCDD",
                       "meterStart":    1234,
                       "reservationId": 7,
                       "timestamp":     "2026-10-09T12:00:00Z"
                   }
                   """,
                   (JObject json, out CP.StartTransactionRequest? value, out String? errorResponse) => CP.StartTransactionRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON(),
                   value => Assert.That(value.MeterStart, Is.EqualTo(WattHour.FromWh(1234)))
               );

        [Test]
        public void StopTransactionRequest()

            => ReadAndWrittenAsTheSchemaSays<CP.StopTransactionRequest>(
                   """
                   {
                       "transactionId": 42,
                       "meterStop":     5678,
                       "timestamp":     "2026-10-09T13:00:00Z"
                   }
                   """,
                   (JObject json, out CP.StopTransactionRequest? value, out String? errorResponse) => CP.StopTransactionRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON(),
                   value => Assert.That(value.MeterStop, Is.EqualTo(WattHour.FromWh(5678)))
               );

        #endregion

        #region ChargingSchedule - its numbers in its chargingRateUnit

        [Test]
        public void ChargingSchedule_InAmperes()

            => ReadAndWrittenAsTheSchemaSays<ChargingSchedule>(
                   """
                   {
                       "chargingRateUnit":       "A",
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 16.5, "numberPhases": 3 }, { "startPeriod": 900, "limit": 6 } ],
                       "duration":               3600,
                       "minChargingRate":        6.5
                   }
                   """,
                   OCPPv1_6.ChargingSchedule.TryParse,
                   value => value.ToJSON(),
                   value => {
                       Assert.That(value.ChargingSchedulePeriods.Select(period => period.Limit.Unit), Is.All.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(value.ChargingSchedulePeriods.First().Limit.Value,                 Is.EqualTo(16.5M));
                       Assert.That(value.MinChargingRate?.Unit,                                       Is.EqualTo(ChargingRateUnits.Amperes));
                   }
               );

        [Test]
        public void ChargingSchedule_InWatts()

            => ReadAndWrittenAsTheSchemaSays<ChargingSchedule>(
                   """
                   {
                       "chargingRateUnit":       "W",
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 11000 } ]
                   }
                   """,
                   OCPPv1_6.ChargingSchedule.TryParse,
                   value => value.ToJSON(),
                   value => Assert.That(value.ChargingSchedulePeriods.Single().Limit.Unit, Is.EqualTo(ChargingRateUnits.Watts))
               );

        /// <summary>
        /// SOAP: a decimal limit is written and read in the invariant culture,
        /// not in the culture of the machine - on a German one 16.5 was "16,5".
        /// </summary>
        [Test]
        public void ChargingSchedule_AsXML_InAGermanCulture()
        {

            var culture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

                Assert.That(OCPPv1_6.ChargingSchedule.TryParse(JObject.Parse("""{ "chargingRateUnit": "A", "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 16.5 } ], "minChargingRate": 6.5 }"""),
                                                               out var schedule, out var errorResponse), Is.True, errorResponse);

                var xml = schedule!.ToXML();

                Assert.That(xml.ToString(), Does.Contain(">16.5<").And.Contain(">6.5<"));

                var again = OCPPv1_6.ChargingSchedule.Parse(xml);

                Assert.That(again.ChargingSchedulePeriods.Single().Limit.Value, Is.EqualTo(16.5M));
                Assert.That(again.MinChargingRate?.Value,                       Is.EqualTo(6.5M));

            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }

        }

        #endregion

        #region ChargingRateValue as CBOR

        /// <summary>
        /// In CBOR a charging rate value is a metrological value in its unit.
        /// </summary>
        [Test]
        public void ChargingRateValue_AsCBOR_IsAMetrologicalValue()
        {

            var cbor = ChargingRateValue.ParseAmperes(32).ToCBOR();

            Assert.That(cbor.HasTag(CBORTag.MetrologicalValue), Is.True);

            Assert.That(ChargingRateValue.TryParseCBOR(CBORValue.Parse(cbor.ToByteArray()), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again.Unit,  Is.EqualTo(ChargingRateUnits.Amperes));
            Assert.That(again.Value, Is.EqualTo(32));

        }

        #endregion

        #region NotifyWebPaymentStartedRequest (DataTransfer extension)

        [Test]
        public void NotifyWebPaymentStartedRequest_AsADataTransfer()
        {

            Assert.That(CS.NotifyWebPaymentStartedRequest.TryParse(JObject.Parse("""{ "connectorId": 1, "timeout": 300 }"""), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.True, errorResponse);

            var written = request!.ToJSON();

            Assert.That(CS.NotifyWebPaymentStartedRequest.TryParse(written, requestId, destination, NetworkPath.Empty, out var again, out errorResponse), Is.True,
                        $"What was written was not read: {errorResponse}{Environment.NewLine}{written.ToString(Newtonsoft.Json.Formatting.None)}");

            Assert.That(JToken.DeepEquals(Normalized(again!.ToJSON()), Normalized(written)), Is.True);

        }

        #endregion

    }

}
