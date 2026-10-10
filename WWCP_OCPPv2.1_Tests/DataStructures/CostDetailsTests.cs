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


#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// The cost details of a transaction: in CBOR, the volume of a cost
    /// dimension is a metrological value in the unit of its dimension, the
    /// energy of the usage one in Wh; in JSON, IdleTIme is spelled as the
    /// schema spells it, and a total has no tax rates.
    /// </summary>
    [TestFixture]
    public class CostDetailsTests
    {

        [TestCase(CostDimensions.Energy,        "WattHour")]
        [TestCase(CostDimensions.MaxCurrent,    "Ampere")]
        [TestCase(CostDimensions.MinCurrent,    "Ampere")]
        [TestCase(CostDimensions.MaxPower,      "Watt")]
        [TestCase(CostDimensions.MinPower,      "Watt")]
        [TestCase(CostDimensions.IdleTime,      "Second")]
        [TestCase(CostDimensions.ChargingTime,  "Second")]
        public void TheVolumeIsAMetrologicalValueOfTheUnitOfItsDimension(CostDimensions Type, String Unit)
        {

            var dimension = new CostDimension(Type, -12.5m);
            var cbor      = CBORValue.Parse(dimension.ToCBOR().ToByteArray());

            Assert.That(cbor.TryGetValue(CBORValue.FromText("volume"), out var volume), Is.True);
            Assert.That(MetrologicalValue.TryParse(volume, out var metrologicalValue, out var errorResponse), Is.True, errorResponse);
            Assert.That(metrologicalValue.Unit.ToString(), Is.EqualTo(org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Parse(Unit).ToString()));
            Assert.That(metrologicalValue.Value,           Is.EqualTo(-12.5m));

            Assert.That(CostDimension.TryParseCBOR(cbor, out var fromCBOR, out errorResponse), Is.True, errorResponse);
            Assert.That(fromCBOR, Is.EqualTo(dimension));

        }

        [Test]
        public void AVolumeOfAnotherPrefixIsInTheUnitOfItsDimension()
        {

            var cbor = CBORValue.FromMap([
                           new (CBORValue.FromText("type"),    CBORValue.FromText("Energy")),
                           new (CBORValue.FromText("volume"),  new MetrologicalValue(1.5m, org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour, SIPrefix.Kilo).ToCBOR())
                       ]);

            Assert.That(CostDimension.TryParseCBOR(cbor, out var dimension, out var errorResponse), Is.True, errorResponse);
            Assert.That(dimension!.Volume, Is.EqualTo(1500m));

        }

        [Test]
        public void AVolumeOfAnotherUnitIsRefused()
        {

            // Energy in A: not what the dimension is.
            var cbor = CBORValue.FromMap([
                           new (CBORValue.FromText("type"),    CBORValue.FromText("Energy")),
                           new (CBORValue.FromText("volume"),  new MetrologicalValue(16m, org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Ampere).ToCBOR())
                       ]);

            Assert.That(CostDimension.TryParseCBOR(cbor, out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("is not in"));

        }

        [Test]
        public void IdleTimeIsWrittenAsTheSchemaSpellsItAndReadBothWays()
        {

            Assert.That(new CostDimension(CostDimensions.IdleTime, 60).ToJSON()["type"]!.Value<String>(), Is.EqualTo("IdleTIme"));

            foreach (var text in new[] { "IdleTIme", "IdleTime" })
            {
                Assert.That(CostDimension.TryParse(JObject.Parse($$"""{ "type": "{{text}}", "volume": 60 }"""), out var dimension, out var errorResponse), Is.True, errorResponse);
                Assert.That(dimension!.Type, Is.EqualTo(CostDimensions.IdleTime));
            }

        }

        [Test]
        public void ATotalHasNoTaxRates()
        {

            var totalCost = new TotalCost(
                                Currency.EUR,
                                TariffCosts.NormalCost,
                                new Price(10m, 11.9m, [ new TaxRate(TaxType.Parse("VAT"), Percentage.Parse(19)) ])
                            );

            Assert.That(totalCost.ToJSON()["total"]!["taxRates"], Is.Null);
            Assert.That(CBORValue.Parse(totalCost.ToCBOR().ToByteArray()).TryGetValue(CBORValue.FromText("total"), out var total), Is.True);
            Assert.That(total.TryGetValue(CBORValue.FromText("taxRates"), out _), Is.False);

        }

        [Test]
        public void TheEnergyOfTheUsageIsAMetrologicalValueInWh()
        {

            var usage = new TotalUsage(WattHour.FromWh(12345.6m), TimeSpan.FromSeconds(3600), TimeSpan.FromSeconds(600));
            var cbor  = CBORValue.Parse(usage.ToCBOR().ToByteArray());

            Assert.That(cbor.TryGetValue(CBORValue.FromText("energy"), out var energy), Is.True);
            Assert.That(energy.HasTag(CBORTag.MetrologicalValue), Is.True, cbor.ToDiagnosticString());

            Assert.That(TotalUsage.TryParseCBOR(cbor, out var fromCBOR, out var errorResponse), Is.True, errorResponse);
            Assert.That(fromCBOR,                                  Is.EqualTo(usage));
            Assert.That(usage.ToJSON()["energy"]!.Value<Decimal>(), Is.EqualTo(12345.6m));

        }

    }

}
