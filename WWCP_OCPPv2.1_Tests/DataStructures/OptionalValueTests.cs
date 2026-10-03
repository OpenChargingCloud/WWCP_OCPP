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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Unit tests for optional values: one there but not valid is refused,
    /// with a reason, and one not there is no reason to refuse.
    /// </summary>
    [TestFixture]
    public class OptionalValueTests
    {

        #region Data

        private static JObject With(JObject JSON, String Property, JToken Value)
        {
            JSON[Property] = Value;
            return JSON;
        }

        private static JObject AFirmware()

            => new Firmware(
                   URL.Parse("https://example.org/firmware.bin"),
                   DateTimeOffset.UtcNow
               ).ToJSON();

        private static JObject ATariffAssignment()

            => new TariffAssignment(
                   Tariff_Id.Parse("DE-GDF-T12345678"),
                   TariffKind.DefaultTariff
               ).ToJSON();

        /// <summary>
        /// A charging ticket up to its provider URL - not one ChargingTicket.ToJSON()
        /// writes: it names no property as TryParse does.
        /// </summary>
        private static JObject AChargingTicketUpToItsProviderURL()
        {

            var keyPair = ECCKeyPair.GenerateKeys()!;

            var tariff  = new Tariff(
                              Id:        Tariff_Id.Parse("DE-GDF-T12345678"),
                              Currency:  Currency.EUR,
                              Energy:    new TariffEnergy(
                                             [ new TariffEnergyPrice(0.51M) ],
                                             [ TaxRate.VAT(15) ]
                                         )
                          );

            return new JObject(
                       new JProperty("id",               ChargingTicket_Id.NewRandom("DE-GDF").ToString()),
                       new JProperty("providerId",       "DE-GDF"),
                       new JProperty("providerName",     new DisplayTexts(Languages.en, "GraphDefined EMP").ToJSON()),
                       new JProperty("driverPublicKey",  keyPair.ToECCPublicKey().ToJSON(CryptoSerialization.RAW)),
                       new JProperty("chargingTariffs",  new JArray(tariff.ToJSON()))
                   );

        }

        #endregion


        #region Firmware_InstallDateTimeNotValid_IsRefused()

        [Test]
        public void Firmware_InstallDateTimeNotValid_IsRefused()
        {

            Assert.That(Firmware.TryParse(AFirmware(), out _, out var errorResponse), Is.True, errorResponse);

            Assert.That(Firmware.TryParse(With(AFirmware(), "installDateTime", "not a timestamp"), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region TariffAssignment_EVSEIdsNotValid_AreRefused()

        [Test]
        public void TariffAssignment_EVSEIdsNotValid_AreRefused()
        {

            Assert.That(TariffAssignment.TryParse(ATariffAssignment(), out _, out var errorResponse), Is.True, errorResponse);

            Assert.That(TariffAssignment.TryParse(With(ATariffAssignment(), "evseIds", new JArray("not a number")), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region TariffPrices_StepSizeNotValid_IsRefused()

        [Test]
        public void TariffPrices_StepSizeNotValid_IsRefused()
        {

            var energyPrice = new TariffEnergyPrice(0.51M).ToJSON();
            var timePrice   = new TariffTimePrice  (6.50M).ToJSON();

            Assert.That(TariffEnergyPrice.TryParse(energyPrice, out _, out var errorResponse), Is.True, errorResponse);
            Assert.That(TariffTimePrice.  TryParse(timePrice,   out _, out     errorResponse), Is.True, errorResponse);

            Assert.That(TariffEnergyPrice.TryParse(With(energyPrice, "stepSize", "not a number"), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

            Assert.That(TariffTimePrice.  TryParse(With(timePrice,   "stepSize", "not a number"), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region ChargingTicket_ProviderURLNotValid_IsRefused()

        /// <summary>
        /// The parse goes on to the properties after the provider URL - and stops at
        /// the first of them, as this ticket ends with it - while it is not there or
        /// valid, and refuses it while it is not valid.
        /// </summary>
        [Test]
        public void ChargingTicket_ProviderURLNotValid_IsRefused()
        {

            ChargingTicket.TryParse(AChargingTicketUpToItsProviderURL(), out _, out var errorResponse);
            Assert.That(errorResponse, Does.Contain("created"));

            ChargingTicket.TryParse(With(AChargingTicketUpToItsProviderURL(), "providerURL", "https://example.org"), out _, out errorResponse);
            Assert.That(errorResponse, Does.Contain("created"));

            Assert.That(ChargingTicket.TryParse(With(AChargingTicketUpToItsProviderURL(), "providerURL", "no scheme://example.org"), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("provider URL"));

        }

        #endregion

    }

}
