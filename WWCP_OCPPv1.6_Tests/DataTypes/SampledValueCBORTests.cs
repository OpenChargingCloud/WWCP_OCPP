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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.DataTypes
{

    /// <summary>
    /// A sampled value in CBOR: a raw value in a metrological unit is a
    /// metrological value of Styx, without its unit beside it - any other
    /// value the text it is, with its unit.
    /// </summary>
    [TestFixture]
    public class SampledValueCBORTests
    {

        #region (private static) ThroughCBOR(SampledValue)

        private static (CBORValue CBOR, SampledValue SampledValue) ThroughCBOR(SampledValue SampledValue)
        {

            var cbor = CBORValue.Parse(SampledValue.ToCBOR().ToByteArray());

            Assert.That(SampledValue.TryParseCBOR(cbor, out var fromCBOR, out var errorResponse), Is.True, errorResponse);
            Assert.That(JToken.DeepEquals(fromCBOR!.ToJSON(), SampledValue.ToJSON()), Is.True,
                        $"Read from CBOR: {fromCBOR.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Written:        {SampledValue.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}");

            return (cbor, fromCBOR);

        }

        #endregion


        [TestCase("1234.5",  UnitsOfMeasure.kWh,     "WattHour",               3)]
        [TestCase("17",      UnitsOfMeasure.Wh,      "WattHour",               0)]
        [TestCase("-3.25",   UnitsOfMeasure.kvarh,   "VoltAmpereReactiveHour", 3)]
        [TestCase("11000",   UnitsOfMeasure.Watts,   "Watt",                   0)]
        [TestCase("16.0",    UnitsOfMeasure.Amperes, "Ampere",                 0)]
        [TestCase("230",     UnitsOfMeasure.Voltage, "Volt",                   0)]
        [TestCase("21.5",    UnitsOfMeasure.Celsius, "Celsius",                0)]
        [TestCase("80",      UnitsOfMeasure.Percent, "Percent",                0)]
        public void ARawValueIsAMetrologicalValue(String Value, UnitsOfMeasure Unit, String MetrologicalUnit, Int32 Exponent)
        {

            var (cbor, sampledValue) = ThroughCBOR(new SampledValue(Value, Format: ValueFormats.Raw, Unit: Unit));

            Assert.That(cbor.TryGetValue(CBORValue.FromText("value"), out var value), Is.True);
            Assert.That(value.HasTag(CBORTag.MetrologicalValue),                     Is.True, cbor.ToDiagnosticString());
            Assert.That(cbor.TryGetValue(CBORValue.FromText("unit"), out _),         Is.False, "No unit beside a metrological value");

            Assert.That(MetrologicalValue.TryParse(value, out var metrologicalValue, out var errorResponse), Is.True, errorResponse);
            Assert.That(metrologicalValue.Unit.ToString(),   Is.EqualTo(UnitOfMeasure.Parse(MetrologicalUnit).ToString()));
            Assert.That(metrologicalValue.Prefix.Exponent,   Is.EqualTo(Exponent));

            Assert.That(sampledValue.Value,                  Is.EqualTo(Value));
            Assert.That(sampledValue.Unit,                   Is.EqualTo(Unit));

        }

        [TestCase("1234.5",   ValueFormats.SignedData, UnitsOfMeasure.kWh)]
        [TestCase("98.6",     ValueFormats.Raw,        UnitsOfMeasure.Fahrenheit)]
        [TestCase("n/a",      ValueFormats.Raw,        UnitsOfMeasure.Wh)]
        [TestCase("1e3",      ValueFormats.Raw,        UnitsOfMeasure.Wh)]
        [TestCase("+5",       ValueFormats.Raw,        UnitsOfMeasure.Wh)]
        public void AnyOtherValueIsItsTextWithItsUnit(String Value, ValueFormats Format, UnitsOfMeasure Unit)
        {

            var (cbor, sampledValue) = ThroughCBOR(new SampledValue(Value, Format: Format, Unit: Unit));

            Assert.That(cbor.TryGetValue(CBORValue.FromText("value"), out var value), Is.True);
            Assert.That(value.Kind,                                                    Is.EqualTo(CBORValueKind.TextString));
            Assert.That(cbor.TryGetValue(CBORValue.FromText("unit"), out var unit),   Is.True);
            Assert.That(unit.AsText(),                                                 Is.EqualTo(Unit.AsText()));

            Assert.That(sampledValue.Value,                  Is.EqualTo(Value));
            Assert.That(sampledValue.Unit,                   Is.EqualTo(Unit));

        }

        [Test]
        public void AMetrologicalValueOfAnotherPrefixIsInTheUnitOfOCPP()
        {

            // 2.5 MWh: in kWh, the largest unit of OCPP 1.6 not above it.
            var cbor = OnlyAValue(new MetrologicalValue(2.5m, UnitOfMeasure.WattHour, SIPrefix.Mega));

            Assert.That(SampledValue.TryParseCBOR(cbor, out var sampledValue, out var errorResponse), Is.True, errorResponse);
            Assert.That(Decimal.Parse(sampledValue!.Value, System.Globalization.CultureInfo.InvariantCulture), Is.EqualTo(2500m));
            Assert.That(sampledValue.Unit,                                                                       Is.EqualTo(UnitsOfMeasure.kWh));

            // 500 mWh: in Wh.
            cbor = OnlyAValue(new MetrologicalValue(500m, UnitOfMeasure.WattHour, SIPrefix.Milli));

            Assert.That(SampledValue.TryParseCBOR(cbor, out sampledValue, out errorResponse), Is.True, errorResponse);
            Assert.That(Decimal.Parse(sampledValue!.Value, System.Globalization.CultureInfo.InvariantCulture), Is.EqualTo(0.5m));
            Assert.That(sampledValue.Unit,                                                                       Is.EqualTo(UnitsOfMeasure.Wh));

        }

        [Test]
        public void AMetrologicalValueOfAUnitOCPPHasNotIsRefused()
        {

            var cbor = OnlyAValue(new MetrologicalValue(3m, UnitOfMeasure.Meter));

            Assert.That(SampledValue.TryParseCBOR(cbor, out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("none of OCPP 1.6"));

        }

        [Test]
        public void AUnitBesideAMetrologicalValueIsRefused()
        {

            var cbor = CBORValue.FromMap([
                           new (CBORValue.FromText("value"), new MetrologicalValue(3m, UnitOfMeasure.WattHour).ToCBOR()),
                           new (CBORValue.FromText("unit"),  CBORValue.FromText("kWh"))
                       ]);

            Assert.That(SampledValue.TryParseCBOR(cbor, out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("no unit beside it"));

        }

        [Test]
        public void CelciusIsReadAsCelsius()
        {

            var cbor = CBORValue.FromMap([
                           new (CBORValue.FromText("value"), CBORValue.FromText("n/a")),
                           new (CBORValue.FromText("unit"),  CBORValue.FromText("Celcius"))
                       ]);

            Assert.That(SampledValue.TryParseCBOR(cbor, out var sampledValue, out var errorResponse), Is.True, errorResponse);
            Assert.That(sampledValue!.Unit, Is.EqualTo(UnitsOfMeasure.Celsius));

        }


        /// <summary>
        /// A sampled value of only the given metrological value.
        /// </summary>
        private static CBORValue OnlyAValue(MetrologicalValue Value)

            => CBORValue.FromMap([
                   new (CBORValue.FromText("value"), Value.ToCBOR())
               ]);

    }

}
