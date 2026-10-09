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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

using Unit = org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// The building blocks of the CBOR representations: timestamps as tag 1,
    /// durations and numbers with a unit as metrological values (tag 44252),
    /// maps without what is not there, and custom data.
    /// </summary>
    [TestFixture]
    public class CBORFoundationTests
    {

        #region (private static) Bytes(CBOR)

        private static String Bytes(CBORValue CBOR)
            => Convert.ToHexString(CBOR.ToByteArray());

        private static CBORValue Read(CBORValue CBOR)
            => CBORValue.Parse(CBOR.ToByteArray());

        #endregion


        #region Timestamps

        [Test]
        public void Timestamp_OfAWholeSecond_IsTag1WithAnInteger()
        {

            var timestamp = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);
            var cbor      = timestamp.ToCBOR();

            Assert.That(cbor.HasTag(CBORTag.EpochDateTime), Is.True);
            Assert.That(cbor.UntaggedValue.Kind,            Is.EqualTo(CBORValueKind.UnsignedInteger));

            Assert.That(OCPPCBORExtensions.TryParseTimestamp(Read(cbor), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again, Is.EqualTo(timestamp));

        }

        [TestCase(123)]
        [TestCase(123456)]
        public void Timestamp_WithAFraction_IsReadBackToTheMicrosecond(Int32 Microseconds)
        {

            var timestamp = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.FromHours(2)).AddTicks(Microseconds * 10L * (Microseconds < 1000 ? 1000 : 1));

            Assert.That(OCPPCBORExtensions.TryParseTimestamp(Read(timestamp.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again,        Is.EqualTo(timestamp));
            Assert.That(again.Offset, Is.EqualTo(TimeSpan.Zero));

        }

        #endregion

        #region Durations

        [Test]
        public void Duration_IsAMetrologicalValueInSeconds()
        {

            var cbor = TimeSpan.FromSeconds(90).ToCBOR();

            Assert.That(cbor.HasTag(CBORTag.MetrologicalValue), Is.True);
            Assert.That(MetrologicalValue.TryParse(Read(cbor), out var metrologicalValue, out _), Is.True);
            Assert.That(metrologicalValue.Unit == Unit.Second, Is.True);
            Assert.That(metrologicalValue.Value,                       Is.EqualTo(90));

            Assert.That(OCPPCBORExtensions.TryParseDuration(Read(cbor), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again, Is.EqualTo(TimeSpan.FromSeconds(90)));

        }

        [Test]
        public void Duration_WithAFraction_StaysExact()
        {

            Assert.That(OCPPCBORExtensions.TryParseDuration(Read(TimeSpan.FromMilliseconds(1500.25).ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again, Is.EqualTo(TimeSpan.FromMilliseconds(1500.25)));

        }

        [Test]
        public void Duration_InMillisecondsOrMinutes_IsRead()
        {

            Assert.That(OCPPCBORExtensions.TryParseDuration(new MetrologicalValue(1500, Unit.Second, SIPrefix.Milli).ToCBOR(), out var milliseconds, out var errorResponse), Is.True, errorResponse);
            Assert.That(milliseconds, Is.EqualTo(TimeSpan.FromSeconds(1.5)));

            Assert.That(OCPPCBORExtensions.TryParseDuration(new MetrologicalValue(1.5M, Unit.Minute).ToCBOR(), out var minutes, out errorResponse), Is.True, errorResponse);
            Assert.That(minutes, Is.EqualTo(TimeSpan.FromSeconds(90)));

        }

        [Test]
        public void Duration_InAnotherUnit_IsRefused()
        {
            Assert.That(OCPPCBORExtensions.TryParseDuration(Watt.FromW(90).AsMetrologicalValue().ToCBOR(), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);
        }

        #endregion

        #region Metrology

        /// <summary>
        /// The example of the specification of tag 44252: 5 A is D9 ACDC 82 05 04.
        /// </summary>
        [Test]
        public void Ampere_IsTheBytesOfTheSpecification()
        {
            Assert.That(Bytes(Ampere.FromA(5).AsMetrologicalValue().ToCBOR()), Is.EqualTo("D9ACDC820504"));
        }

        [Test]
        public void Watt_InKilowatt_IsReadInWatt()
        {

            Assert.That(OCPPCBORExtensions.TryParseWatt(new MetrologicalValue(11, Unit.Watt, SIPrefix.Kilo).ToCBOR(), out var watt, out var errorResponse), Is.True, errorResponse);
            Assert.That(watt, Is.EqualTo(Watt.FromW(11000)));

        }

        [Test]
        public void Watt_InAnotherUnit_IsRefused()
        {
            Assert.That(OCPPCBORExtensions.TryParseWatt(Ampere.FromA(16).AsMetrologicalValue().ToCBOR(), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("W"));
        }

        #endregion

        #region ChargingRateValue

        [Test]
        public void ChargingRateValue_InWattsAndAmperes_AreMetrologicalValues()
        {

            foreach (var value in new[] { ChargingRateValue.ParseWatts(11000), ChargingRateValue.ParseAmperes(16.5M) })
            {

                var cbor = value.ToCBOR();

                Assert.That(cbor.HasTag(CBORTag.MetrologicalValue), Is.True);
                Assert.That(ChargingRateValue.TryParseCBOR(Read(cbor), out var again, out var errorResponse), Is.True, errorResponse);
                Assert.That(again.Unit,  Is.EqualTo(value.Unit));
                Assert.That(again.Value, Is.EqualTo(value.Value));

            }

        }

        [Test]
        public void ChargingRateValue_OfAnUnknownUnit_IsAPlainNumber()
        {

            var cbor = ChargingRateValue.Parse(7.5M).ToCBOR();

            Assert.That(cbor.HasTag(CBORTag.MetrologicalValue), Is.False);

            Assert.That(ChargingRateValue.TryParseCBOR(Read(cbor), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again.Unit,  Is.EqualTo(ChargingRateUnits.Unknown));
            Assert.That(again.Value, Is.EqualTo(7.5M));

        }

        [Test]
        public void ChargingRateValue_InVolts_IsRefused()
        {
            Assert.That(ChargingRateValue.TryParseCBOR(Volt.FromV(230).AsMetrologicalValue().ToCBOR(), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("neither in W nor in A"));
        }

        #endregion

        #region Maps, arrays and optional values

        [Test]
        public void Map_LeavesOutWhatIsNotThere()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("a", CBORValue.FromText("A")),
                           ("b", null),
                           ("c", OCPPCBORExtensions.Array(Array.Empty<Int32>(), number => CBORValue.FromInt64(number))),
                           ("d", OCPPCBORExtensions.Array(new[] { 1, 2 },        number => CBORValue.FromInt64(number)))
                       );

            Assert.That(cbor.Count,                                  Is.EqualTo(2));
            Assert.That(cbor.TryGetValue(CBORValue.FromText("b"), out _), Is.False);
            Assert.That(cbor.TryGetValue(CBORValue.FromText("c"), out _), Is.False);
            Assert.That(cbor.TryGetValue(CBORValue.FromText("d"), out var d), Is.True);
            Assert.That(d.Count, Is.EqualTo(2));

        }

        [Test]
        public void OptionalValue_IsNullWhenNotThere_AndAnErrorWhenNotValid()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("power",   Watt.FromW(7400).AsMetrologicalValue().ToCBOR()),
                           ("current", Watt.FromW(7400).AsMetrologicalValue().ToCBOR())
                       );

            Assert.That(cbor.ParseOptionalValue("power",   "power",   OCPPCBORExtensions.TryParseWatt,   out Watt?   power,   out var errorResponse), Is.True);
            Assert.That(errorResponse, Is.Null);
            Assert.That(power,         Is.EqualTo(Watt.FromW(7400)));

            Assert.That(cbor.ParseOptionalValue("energy",  "energy",  OCPPCBORExtensions.TryParseWattHour, out WattHour? energy, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Null);
            Assert.That(energy,        Is.Null);

            Assert.That(cbor.ParseOptionalValue("current", "current", OCPPCBORExtensions.TryParseAmpere,   out Ampere? current, out errorResponse), Is.True);
            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(current,       Is.Null);

        }

        #endregion

        #region CustomData

        [Test]
        public void CustomData_KeepsItsValues()
        {

            var customData = CustomData.Parse(JObject.Parse("""{ "vendorId": "GraphDefined", "count": 3, "ratio": 1.10, "on": true, "tags": [ "a", "b" ], "nested": { "x": null } }"""));

            Assert.That(OCPPCBORExtensions.TryParseCustomData(Read(customData.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(JToken.DeepEquals(again!.ToJSON(), customData.ToJSON()), Is.True,
                        $"{again.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}{customData.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

    }

}
