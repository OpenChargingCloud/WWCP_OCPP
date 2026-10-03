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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Unit tests for optional values that TryParse reads where ToJSON()
    /// writes them, and as it writes them.
    /// </summary>
    [TestFixture]
    public class ReadAsWrittenTests
    {

        #region Data

        private static JObject With(JObject JSON, String Property, JToken Value)
        {
            JSON[Property] = Value;
            return JSON;
        }

        private static JObject AnAPNConfiguration()

            => new APNConfiguration(
                   AccessPointName:       "apn1",
                   AuthenticationMethod:  APNAuthenticationMethods.PAP,
                   PreferredNetwork:      "20404",
                   OnlyPreferredNetwork:  true
               ).ToJSON();

        private static JObject AChargingSchedulePeriod(PhasesToUse? PhaseToUse)

            => new ChargingSchedulePeriod(
                   StartPeriod:     TimeSpan.Zero,
                   NumberOfPhases:  1,
                   PhaseToUse:      PhaseToUse
               ).ToJSON();

        #endregion


        #region APNConfiguration_UseOnlyPreferredNetwork_IsReadAsWritten()

        [Test]
        public void APNConfiguration_UseOnlyPreferredNetwork_IsReadAsWritten()
        {

            Assert.That(APNConfiguration.TryParse(AnAPNConfiguration(), out var apnConfiguration, out var errorResponse), Is.True, errorResponse);

            Assert.That(apnConfiguration!.PreferredNetwork,      Is.EqualTo("20404"));
            Assert.That(apnConfiguration. OnlyPreferredNetwork,  Is.True);

        }

        #endregion

        #region APNConfiguration_UseOnlyPreferredNetworkNotValid_IsRefused()

        [Test]
        public void APNConfiguration_UseOnlyPreferredNetworkNotValid_IsRefused()
        {

            Assert.That(APNConfiguration.TryParse(With(AnAPNConfiguration(), "useOnlyPreferredNetwork", "maybe"), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion


        #region ChargingSchedulePeriod_PhaseToUse_IsReadAsWritten(PhaseToUse)

        [TestCase(PhasesToUse.One)]
        [TestCase(PhasesToUse.Two)]
        [TestCase(PhasesToUse.Three)]
        public void ChargingSchedulePeriod_PhaseToUse_IsReadAsWritten(PhasesToUse PhaseToUse)
        {

            Assert.That(ChargingSchedulePeriod.TryParse(AChargingSchedulePeriod(PhaseToUse), out var period, out var errorResponse), Is.True, errorResponse);
            Assert.That(period!.PhaseToUse, Is.EqualTo(PhaseToUse));

        }

        #endregion

        #region ChargingSchedulePeriod_WithoutPhaseToUse_IsRead()

        [Test]
        public void ChargingSchedulePeriod_WithoutPhaseToUse_IsRead()
        {

            Assert.That(ChargingSchedulePeriod.TryParse(AChargingSchedulePeriod(null), out var period, out var errorResponse), Is.True, errorResponse);
            Assert.That(period!.PhaseToUse, Is.Null);

        }

        #endregion

        #region ChargingSchedulePeriod_PhaseToUseNotValid_IsRefused(Value)

        [TestCase(0)]
        [TestCase(4)]
        public void ChargingSchedulePeriod_PhaseToUseNotValid_IsRefused(Int32 Value)
        {

            Assert.That(ChargingSchedulePeriod.TryParse(With(AChargingSchedulePeriod(null), "phaseToUse", Value), out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

    }

}
