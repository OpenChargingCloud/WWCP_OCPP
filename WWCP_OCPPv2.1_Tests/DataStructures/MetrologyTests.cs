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

using org.GraphDefined.Vanaheimr.Illias;

using static cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Numbers with a unit read as Styx's metrological values, and written
    /// back under the keys and in the units the schemas give them.
    /// </summary>
    /// <remarks>
    /// A schedule writes its limits and setpoints as plain numbers in its
    /// chargingRateUnit. Read, they have to carry that unit, not "Unknown".
    /// </remarks>
    [TestFixture]
    public class MetrologyTests
    {

        #region ChargingSchedule_InWatts()

        [Test]
        public void ChargingSchedule_InWatts()

            => ReadAndWrittenAsTheSchemaSays<ChargingSchedule>(
                   $$"""
                   {
                       "id":                     1,
                       "chargingRateUnit":       "W",
                       "minChargingRate":        1400,
                       "powerTolerance":         500,
                       "limitAtSoC":             { "soc": 80, "limit": 7400 },
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 11000, "dischargeLimit": -5000, "setpoint": 3000, "v2xBaseline": 2500 } ],
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingSchedule.TryParse,
                   value => value.ToJSON(),
                   value => {

                       var period = value.ChargingSchedulePeriods.Single();

                       Assert.That(period.Limit?.         Unit,  Is.EqualTo(ChargingRateUnits.Watts));
                       Assert.That(period.DischargeLimit?.Unit,  Is.EqualTo(ChargingRateUnits.Watts));
                       Assert.That(period.Setpoint?.      Unit,  Is.EqualTo(ChargingRateUnits.Watts));
                       Assert.That(value. MinChargingRate?.Unit, Is.EqualTo(ChargingRateUnits.Watts));
                       Assert.That(value. LimitAtSoC?.Limit.Unit, Is.EqualTo(ChargingRateUnits.Watts));

                       Assert.That(period.V2XBaseline,           Is.EqualTo(Watt.FromW(2500)));
                       Assert.That(value. PowerTolerance,        Is.EqualTo(Watt.FromW(500)));

                   }
               );

        #endregion

        #region ChargingSchedule_InAmperes()

        [Test]
        public void ChargingSchedule_InAmperes()

            => ReadAndWrittenAsTheSchemaSays<ChargingSchedule>(
                   """
                   {
                       "id":                     2,
                       "chargingRateUnit":       "A",
                       "minChargingRate":        6,
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 16, "limit_L2": 16, "limit_L3": 10 } ]
                   }
                   """,
                   OCPPv2_1.ChargingSchedule.TryParse,
                   value => value.ToJSON(),
                   value => {

                       var period = value.ChargingSchedulePeriods.Single();

                       Assert.That(period.Limit?.   Unit,         Is.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(period.Limit_L2?.Unit,         Is.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(period.Limit_L3?.Unit,         Is.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(value. MinChargingRate?.Unit,  Is.EqualTo(ChargingRateUnits.Amperes));

                   }
               );

        #endregion

        #region CompositeSchedule_InAmperes()

        [Test]
        public void CompositeSchedule_InAmperes()

            => ReadAndWrittenAsTheSchemaSays<CompositeSchedule>(
                   """
                   {
                       "evseId":                 1,
                       "duration":               3600,
                       "scheduleStart":          "2026-10-09T12:00:00Z",
                       "chargingRateUnit":       "A",
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 32 }, { "startPeriod": 900, "limit": 16 }, { "startPeriod": 1800, "limit": 6 } ]
                   }
                   """,
                   OCPPv2_1.CompositeSchedule.TryParse,
                   value => value.ToJSON(),
                   value => {

                       Assert.That(value.ChargingSchedulePeriods.Select(period => period.Limit?.Unit),
                                   Is.All.EqualTo(ChargingRateUnits.Amperes));

                       Assert.That(value.ChargingSchedulePeriods.Select(period => period.StartPeriod.TotalSeconds),
                                   Is.EqualTo(new[] { 0, 900, 1800 }));

                   }
               );

        #endregion

    }

}
