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

        #region MeterValue, SampledValue, UnitsOfMeasure, SignedMeterValue

        /// <summary>
        /// A sampled value's unit of measure may be left out in CBOR: it is in its metrological value.
        /// </summary>
        private static readonly Dictionary<String, String> unitOfMeasureInTheValue = new() { { "unitOfMeasure", "" } };

        [Test]
        public void MeterValue()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<MeterValue>(
                   $$"""
                   {
                       "timestamp":    "2026-10-09T12:00:00Z",
                       "sampledValue": [
                           { "value": 1234.5 },
                           { "value": 11.04,  "measurand": "Power.Active.Import",   "unitOfMeasure": { "unit": "kW" } },
                           { "value": 15.5,   "measurand": "Current.Import",        "unitOfMeasure": { "unit": "A" }, "phase": "L1" },
                           { "value": 230,    "measurand": "Voltage",               "unitOfMeasure": { "unit": "V" }, "phase": "L1-N" },
                           { "value": 499,    "measurand": "Frequency",             "unitOfMeasure": { "unit": "Hz",  "multiplier": -1 } },
                           { "value": 80,     "measurand": "SoC",                   "unitOfMeasure": { "unit": "Percent" }, "location": "EV" },
                           { "value": 3000,   "measurand": "RPM",                   "unitOfMeasure": { "unit": "RPM" } },
                           { "value": 12,     "context": "Sample.Periodic",         "unitOfMeasure": { "unit": "kWh", "multiplier": 3 } }
                       ],
                       "customData":   {{Custom}}
                   }
                   """,
                   OCPPv2_1.MeterValue.TryParse,
                   value => value.ToJSON(),
                   KeysInCBOR: unitOfMeasureInTheValue
               );

        /// <summary>
        /// A sampled value in a metrological unit is one metrological value in CBOR,
        /// its unit and multiplier its unit and SI prefix; one in a unit that is no
        /// metrological one keeps its unit of measure beside a plain number.
        /// </summary>
        [Test]
        public void SampledValue_IsAMetrologicalValueWhereItsUnitIsOne()
        {

            Assert.That(OCPPv2_1.SampledValue.TryParse(JObject.Parse("""{ "value": 12, "unitOfMeasure": { "unit": "kWh", "multiplier": 3 } }"""), out var inMWh, out var errorResponse), Is.True, errorResponse);

            var cbor = inMWh!.ToCBOR();

            Assert.That(cbor.TryGetValue(CBORValue.FromText("unitOfMeasure"), out _), Is.False);
            Assert.That(cbor.TryGetValue(CBORValue.FromText("value"), out var value),  Is.True);
            Assert.That(MetrologicalValue.TryParse(value, out var metrologicalValue, out errorResponse), Is.True, errorResponse);
            Assert.That(metrologicalValue.Unit == org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour, Is.True);
            Assert.That(metrologicalValue.Prefix.Exponent, Is.EqualTo(6));
            Assert.That(metrologicalValue.Value,           Is.EqualTo(12));

            Assert.That(OCPPv2_1.SampledValue.TryParse(JObject.Parse("""{ "value": 3000, "unitOfMeasure": { "unit": "RPM" } }"""), out var inRPM, out errorResponse), Is.True, errorResponse);

            cbor = inRPM!.ToCBOR();

            Assert.That(cbor.TryGetValue(CBORValue.FromText("unitOfMeasure"), out _), Is.True);
            Assert.That(cbor.TryGetValue(CBORValue.FromText("value"), out value),     Is.True);
            Assert.That(value.HasTag(CBORTag.MetrologicalValue), Is.False);

        }

        [Test]
        public void SampledValue_WithASignedMeterValue()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<SampledValue>(
                   $$"""
                   {
                       "value":            1234.5,
                       "context":          "Transaction.End",
                       "measurand":        "Energy.Active.Import.Register",
                       "location":         "Outlet",
                       "signedMeterValue": { "signedMeterData": "AAEC", "signingMethod": "ECDSA-secp256r1-SHA256", "encodingMethod": "OCMF", "publicKey": "MFkw", "customData": {{Custom}} },
                       "customData":       {{Custom}}
                   }
                   """,
                   OCPPv2_1.SampledValue.TryParse,
                   value => value.ToJSON(),
                   KeysInCBOR: unitOfMeasureInTheValue
               );

        [Test]
        public void UnitsOfMeasure()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<UnitsOfMeasure>(
                   $$"""{ "unit": "kvarh", "multiplier": -1, "customData": {{Custom}} }""",
                   OCPPv2_1.UnitsOfMeasure.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region Tariff

        /// <summary>
        /// A tariff with every part, signed: its energy price per kWh is "price" in
        /// CBOR, an amount per Wh, and its signature carries bytes, not BASE64.
        /// </summary>
        [Test]
        public void Tariff()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<Tariff>(
                   $$"""
                   {
                       "tariffId":         "DE-GDF-T1",
                       "currency":         "EUR",
                       "description":      [ { "format": "UTF8", "language": "de", "content": "Tagestarif" }, { "format": "UTF8", "language": "en", "content": "Day tariff" } ],
                       "validFrom":        "2026-10-01T00:00:00Z",
                       "minCost":          { "exclTax": 1.00, "inclTax": 1.19 },
                       "maxCost":          { "exclTax": 80.00 },
                       "fixedFee":         { "prices": [ { "priceFixed": 0.50 } ] },
                       "reservationFixed": { "prices": [ { "priceFixed": 1.00 } ] },
                       "reservationTime":  { "prices": [ { "priceMinute": 0.10 } ] },
                       "energy":           { "prices": [ { "priceKwh": 0.39, "conditions": { "startTimeOfDay": "06:00", "endTimeOfDay": "22:00", "minPower": 11000, "maxEnergy": 50000 } },
                                                         { "priceKwh": 0.29 } ],
                                             "taxRates": [ { "type": "VAT", "tax": 19, "stack": 0 } ] },
                       "chargingTime":     { "prices": [ { "priceMinute": 0.05, "conditions": { "dayOfWeek": [ "Saturday", "Sunday" ] } } ] },
                       "idleTime":         { "prices": [ { "priceMinute": 0.10, "conditions": { "minIdleTime": 3600 } } ] },
                       "signatures":       [ { "keyId": "AQID", "value": "MEUCIQ==", "name": "operator", "timestamp": "2026-10-09T12:00:00Z" } ],
                       "customData":       {{Custom}}
                   }
                   """,
                   OCPPv2_1.Tariff.TryParse,
                   value => value.ToJSON()
               );

        /// <summary>
        /// The energy price is an amount per Wh in CBOR: 0.39 per kWh is 0.00039 Wh^-1.
        /// </summary>
        [Test]
        public void TariffEnergyPrice_IsAnAmountPerWattHour()
        {

            Assert.That(OCPPv2_1.TariffEnergyPrice.TryParse(JObject.Parse("""{ "priceKwh": 0.39 }"""), out var price, out var errorResponse), Is.True, errorResponse);

            var cbor = price!.ToCBOR();

            Assert.That(cbor.TryGetValue(CBORValue.FromText("priceKwh"), out _),   Is.False);
            Assert.That(cbor.TryGetValue(CBORValue.FromText("price"), out var p),   Is.True);
            Assert.That(MetrologicalValue.TryParse(p, out var metrologicalValue, out errorResponse), Is.True, errorResponse);
            Assert.That(metrologicalValue.Value, Is.EqualTo(0.00039M));
            Assert.That(metrologicalValue.Unit == new UnitExpression(new UnitFactor(org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour, -1)), Is.True);

        }

        #endregion

        #region IdTokenInfo, Transaction

        /// <summary>
        /// Personal messages in more languages than the one JSON writes are all kept in CBOR.
        /// </summary>
        [Test]
        public void IdTokenInfo_WithPersonalMessagesInTwoLanguages()
        {

            Assert.That(OCPPv2_1.IdTokenInfo.TryParse(JObject.Parse("""
                                                                    {
                                                                        "status":               "Accepted",
                                                                        "personalMessage":      { "format": "UTF8", "language": "de", "content": "Hallo" },
                                                                        "personalMessageExtra": [ { "format": "UTF8", "language": "en", "content": "Hello" } ]
                                                                    }
                                                                    """), out var idTokenInfo, out var errorResponse), Is.True, errorResponse);

            Assert.That(OCPPv2_1.IdTokenInfo.TryParseCBOR(CBORValue.Parse(idTokenInfo!.ToCBOR().ToByteArray()), out var fromCBOR, out errorResponse), Is.True, errorResponse);
            Assert.That(fromCBOR!.PersonalMessage.Select(message => message.Content), Is.EqualTo(new[] { "Hallo", "Hello" }));

        }

        [Test]
        public void Transaction()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<Transaction>(
                   $$"""
                   {
                       "transactionId":     "TX1",
                       "chargingState":     "Charging",
                       "timeSpentCharging": 600,
                       "stoppedReason":     "Local",
                       "remoteStartId":     42,
                       "operationMode":     "ChargingOnly",
                       "tariffId":          "T1",
                       "transactionLimit":  { "maxCost": 12.5, "maxEnergy": 50000, "maxTime": 3600, "maxSoC": 80 },
                       "customData":        {{Custom}}
                   }
                   """,
                   OCPPv2_1.Transaction.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region The device model

        [Test]
        public void ComponentVariable()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ComponentVariable>(
                   $$"""{ "component": { "name": "OCPPCommCtrlr" }, "variable": { "name": "HeartbeatInterval" }, "customData": {{Custom}} }""",
                   OCPPv2_1.ComponentVariable.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void VariableAttribute()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<VariableAttribute>(
                   $$"""{ "type": "Target", "value": "300", "mutability": "ReadOnly", "persistent": true, "constant": true, "customData": {{Custom}} }""",
                   OCPPv2_1.VariableAttribute.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void VariableMonitoring()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<VariableMonitoring>(
                   $$"""{ "id": 9, "transaction": true, "value": 85.5, "type": "UpperThreshold", "severity": 3, "eventNotificationType": "CustomMonitor", "customData": {{Custom}} }""",
                   OCPPv2_1.VariableMonitoring.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void SetVariableData()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<SetVariableData>(
                   $$"""{ "attributeType": "Actual", "attributeValue": "300", "component": { "name": "OCPPCommCtrlr" }, "variable": { "name": "HeartbeatInterval" }, "customData": {{Custom}} }""",
                   OCPPv2_1.SetVariableData.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void SetVariableResult()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<SetVariableResult>(
                   $$"""{ "attributeType": "Actual", "attributeStatus": "Rejected", "attributeStatusInfo": { "reasonCode": "ReadOnly" }, "component": { "name": "OCPPCommCtrlr" }, "variable": { "name": "HeartbeatInterval" }, "customData": {{Custom}} }""",
                   OCPPv2_1.SetVariableResult.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void GetVariableData()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<GetVariableData>(
                   $$"""{ "attributeType": "Actual", "component": { "name": "OCPPCommCtrlr" }, "variable": { "name": "HeartbeatInterval" }, "customData": {{Custom}} }""",
                   OCPPv2_1.GetVariableData.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void GetVariableResult()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<GetVariableResult>(
                   $$"""{ "attributeStatus": "Accepted", "attributeType": "Actual", "attributeValue": "300", "attributeStatusInfo": { "reasonCode": "NoError" }, "component": { "name": "OCPPCommCtrlr" }, "variable": { "name": "HeartbeatInterval" }, "customData": {{Custom}} }""",
                   OCPPv2_1.GetVariableResult.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void MonitoringData()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<MonitoringData>(
                   $$"""
                   {
                       "component":          { "name": "Connector", "evse": { "id": 1, "connectorId": 1 } },
                       "variable":           { "name": "Temperature" },
                       "variableMonitoring": [ { "id": 9, "transaction": false, "value": 85.5, "type": "UpperThreshold", "severity": 3, "eventNotificationType": "CustomMonitor" } ],
                       "customData":         {{Custom}}
                   }
                   """,
                   OCPPv2_1.MonitoringData.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void SetMonitoringData()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<SetMonitoringData>(
                   $$"""
                   {
                       "id":                  9,
                       "periodicEventStream": { "interval": 60, "values": 10, "customData": {{Custom}} },
                       "transaction":         true,
                       "value":               85.5,
                       "type":                "PeriodicEventStream",
                       "severity":            8,
                       "component":           { "name": "Connector", "evse": { "id": 1, "connectorId": 1 } },
                       "variable":            { "name": "Temperature" },
                       "customData":          {{Custom}}
                   }
                   """,
                   OCPPv2_1.SetMonitoringData.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void SetMonitoringResult()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<SetMonitoringResult>(
                   $$"""
                   {
                       "id":         9,
                       "statusInfo": { "reasonCode": "NoError" },
                       "status":     "Accepted",
                       "type":       "UpperThreshold",
                       "component":  { "name": "Connector", "evse": { "id": 1, "connectorId": 1 } },
                       "variable":   { "name": "Temperature" },
                       "severity":   3,
                       "customData": {{Custom}}
                   }
                   """,
                   OCPPv2_1.SetMonitoringResult.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ClearMonitoringResult()

            => ReadAndWrittenAsTheSchemaSaysAndAsCBOR<ClearMonitoringResult>(
                   $$"""{ "status": "NotFound", "id": 9, "statusInfo": { "reasonCode": "UnknownMonitor" }, "customData": {{Custom}} }""",
                   OCPPv2_1.ClearMonitoringResult.TryParse,
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
