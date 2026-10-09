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

using static cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Data structures read in the shape the official OCPP 2.1 (Edition 2)
    /// JSON schemas give them, written as CBOR and read back from its bytes
    /// as the same - under the same keys.
    /// </summary>
    [TestFixture]
    public class CBORAsTheSchemaSaysTests
    {

        #region StatusInfo, AdditionalInfo, IdToken

        [Test]
        public void StatusInfo()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<StatusInfo>(
                   $$"""{ "reasonCode": "NoError", "additionalInfo": "All fine", "customData": {{Custom}} }""",
                   OCPPv2_1.StatusInfo.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void StatusInfo_WithItsReasonCodeAlone()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<StatusInfo>(
                   """{ "reasonCode": "NoError" }""",
                   OCPPv2_1.StatusInfo.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void AdditionalInfo()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<AdditionalInfo>(
                   $$"""{ "additionalIdToken": "12345", "type": "ContractId", "customData": {{Custom}} }""",
                   OCPPv2_1.AdditionalInfo.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void IdToken()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<IdToken>(
                   $$"""
                   {
                       "idToken":        "AABBCCDD",
                       "type":           "ISO14443",
                       "additionalInfo": [ { "additionalIdToken": "12345", "type": "ContractId" }, { "additionalIdToken": "DE-GDF-C1", "type": "eMAID" } ],
                       "customData":     {{Custom}}
                   }
                   """,
                   OCPPv2_1.IdToken.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region EVSE, Component, Variable

        [Test]
        public void EVSE()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<EVSE>(
                   $$"""{ "id": 2, "connectorId": 1, "customData": {{Custom}} }""",
                   OCPPv2_1.EVSE.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void EVSE_WithoutAConnector()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<EVSE>(
                   """{ "id": 0 }""",
                   OCPPv2_1.EVSE.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void Component()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<Component>(
                   $$"""{ "name": "Connector", "instance": "Left", "evse": { "id": 1, "connectorId": 2 }, "customData": {{Custom}} }""",
                   OCPPv2_1.Component.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void Variable()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<Variable>(
                   $$"""{ "name": "Power", "instance": "Max", "customData": {{Custom}} }""",
                   OCPPv2_1.Variable.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region MessageContent

        [Test]
        public void MessageContent()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<MessageContent>(
                   $$"""{ "format": "UTF8", "language": "de", "content": "Hallo", "customData": {{Custom}} }""",
                   OCPPv2_1.MessageContent.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region V2X curves, LimitAtSoC, ChargingSchedulePeriod

        [Test]
        public void V2XFreqWattEntry()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<V2XFreqWattEntry>(
                   $$"""{ "frequency": 49.8, "power": -3000, "customData": {{Custom}} }""",
                   OCPPv2_1.V2XFreqWattEntry.TryParse,
                   value => value.ToJSON(),
                   value => Assert.That(value.Power, Is.EqualTo(Watt.FromW(-3000)))
               );

        [Test]
        public void V2XSignalWattEntry()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<V2XSignalWattEntry>(
                   $$"""{ "signal": 0.5, "power": 7400, "customData": {{Custom}} }""",
                   OCPPv2_1.V2XSignalWattEntry.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void LimitAtSoC()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<LimitAtSoC>(
                   $$"""{ "soc": 80, "limit": 7400, "customData": {{Custom}} }""",
                   OCPPv2_1.LimitAtSoC.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ChargingSchedulePeriod()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingSchedulePeriod>(
                   $$"""
                   {
                       "startPeriod":            900,
                       "numberPhases":           1,
                       "phaseToUse":             2,
                       "limit":                  16,   "limit_L2":            16,  "limit_L3":            10,
                       "dischargeLimit":         -16,  "dischargeLimit_L2":   -16, "dischargeLimit_L3":   -10,
                       "setpoint":               8,    "setpoint_L2":         8,   "setpoint_L3":         6,
                       "setpointReactive":       2,    "setpointReactive_L2": 2,   "setpointReactive_L3": 1,
                       "preconditioningRequest": true,
                       "evseSleep":              false,
                       "operationMode":          "LocalFrequency",
                       "v2xBaseline":            2500,
                       "v2xFreqWattCurve":       [ { "frequency": 49.8, "power": -3000 }, { "frequency": 50.2, "power": 3000 } ],
                       "v2xSignalWattCurve":     [ { "signal": 0, "power": 0 } ],
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingSchedulePeriod.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region ChargingSchedule with a SalesTariff

        [Test]
        public void ChargingSchedule_WithASalesTariff()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingSchedule>(
                   $$"""
                   {
                       "id":                     3,
                       "startSchedule":          "2026-10-09T12:00:00.123Z",
                       "duration":               7200,
                       "chargingRateUnit":       "A",
                       "minChargingRate":        6,
                       "useLocalTime":           false,
                       "randomizedDelay":        30,
                       "signatureId":            12,
                       "digestValue":            "AAEC",
                       "powerTolerance":         200,
                       "limitAtSoC":             { "soc": 90, "limit": 10 },
                       "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 32 }, { "startPeriod": 3600, "limit": 16 } ],
                       "salesTariff":            {
                                                     "id":                     5,
                                                     "salesTariffDescription": "Night",
                                                     "numEPriceLevels":        2,
                                                     "salesTariffEntry":       [ {
                                                                                   "relativeTimeInterval": { "start": 0, "duration": 3600 },
                                                                                   "ePriceLevel":          1,
                                                                                   "consumptionCost":      [ { "startValue": 0, "cost": [ { "costKind": "CarbonDioxideEmission", "amount": 120, "amountMultiplier": -1 } ] } ]
                                                                               },
                                                                               { "relativeTimeInterval": { "start": 3600 } } ],
                                                     "customData":             {{Custom}}
                                                 },
                       "customData":             {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingSchedule.TryParse,
                   value => value.ToJSON(),
                   value => {
                       Assert.That(value.ChargingSchedulePeriods.Select(period => period.Limit?.Unit), Is.All.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(value.LimitAtSoC?.Limit.Unit, Is.EqualTo(ChargingRateUnits.Amperes));
                       Assert.That(value.MinChargingRate?.Unit,  Is.EqualTo(ChargingRateUnits.Amperes));
                   }
               );

        #endregion

        #region ChargingProfile, ChargingScheduleUpdate

        [Test]
        public void ChargingProfile()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingProfile>(
                   $$"""
                   {
                       "id":                          100,
                       "stackLevel":                  2,
                       "chargingProfilePurpose":      "TxProfile",
                       "chargingProfileKind":         "Recurring",
                       "recurrencyKind":              "Daily",
                       "validFrom":                   "2026-10-09T00:00:00Z",
                       "validTo":                     "2026-12-31T23:59:59Z",
                       "transactionId":               "TX-4711",
                       "maxOfflineDuration":          600,
                       "invalidAfterOfflineDuration": true,
                       "dynUpdateInterval":           60,
                       "dynUpdateTime":               "2026-10-09T12:30:00Z",
                       "priceScheduleSignature":      "MEUCIQ",
                       "chargingSchedule":            [ {
                                                          "id":                     1,
                                                          "chargingRateUnit":       "W",
                                                          "chargingSchedulePeriod": [ { "startPeriod": 0, "limit": 11000, "operationMode": "ChargingOnly" } ]
                                                      } ],
                       "customData":                  {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingProfile.TryParse,
                   value => value.ToJSON(),
                   value => Assert.That(value.ChargingSchedules.Single().ChargingSchedulePeriods.Single().Limit?.Unit, Is.EqualTo(ChargingRateUnits.Watts))
               );

        [Test]
        public void ChargingScheduleUpdate()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ChargingScheduleUpdate>(
                   $$"""
                   {
                       "limit":            16,  "limit_L2":            16,  "limit_L3":            10,
                       "dischargeLimit":   -16, "dischargeLimit_L2":   -16, "dischargeLimit_L3":   -10,
                       "setpoint":         8,   "setpoint_L2":         8,   "setpoint_L3":         6,
                       "setpointReactive": 2,   "setpointReactive_L2": 2,   "setpointReactive_L3": 1,
                       "customData":       {{Custom}}
                   }
                   """,
                   OCPPv2_1.ChargingScheduleUpdate.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region Refused

        /// <summary>
        /// A map without a mandatory key, or with a key of the wrong kind, is refused with a reason.
        /// </summary>
        [Test]
        public void WhatIsNotValid_IsRefused()
        {

            Assert.That(OCPPv2_1.StatusInfo.    TryParseCBOR(CBORValue.FromText("NoError"),                                                          out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("not a map"));

            Assert.That(OCPPv2_1.IdToken.       TryParseCBOR(OCPP.OCPPCBORExtensions.Map(("idToken", CBORValue.FromText("A"))),                       out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("type"));

            Assert.That(OCPPv2_1.EVSE.          TryParseCBOR(OCPP.OCPPCBORExtensions.Map(("id", CBORValue.FromText("one"))),                          out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

            Assert.That(OCPPv2_1.EVSE.          TryParseCBOR(OCPP.OCPPCBORExtensions.Map(("id", CBORValue.FromUInt64(70000))),                        out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("70000"));

            Assert.That(OCPPv2_1.MessageContent.TryParseCBOR(OCPP.OCPPCBORExtensions.Map(("content", CBORValue.FromText("Hallo"))),                   out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("format"));

            Assert.That(OCPPv2_1.IdToken.       TryParseCBOR(OCPP.OCPPCBORExtensions.Map(("idToken",        CBORValue.FromText("A")),
                                                                                         ("type",           CBORValue.FromText("ISO14443")),
                                                                                         ("additionalInfo", CBORValue.FromArray(CBORValue.FromText("x")))), out _, out errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("additionalInfo").And.Contain("item 0"));

        }

        #endregion

    }

}
