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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

using static cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Messages read in the shape the official OCPP 2.1 (Edition 2) JSON
    /// schemas give them, and written back in exactly that shape - and the
    /// vendor extensions without a schema read back as they are written.
    /// </summary>
    [TestFixture]
    public class MessagesAsTheSchemaSaysTests
    {

        #region Data

        private const String StatusInfo = """{ "reasonCode": "NoError", "additionalInfo": "All fine" }""";

        private static readonly Request_Id     requestId    = Request_Id.Parse("1");
        private static readonly SourceRouting  destination  = SourceRouting.CSMS;

        #endregion

        #region (private static) Request<T>(Sample, TryParse)

        /// <summary>
        /// The request the sample is, read as a response's parser needs it.
        /// </summary>
        private static T Request<T>(String Sample, Parser<T> TryParse)
        {

            Assert.That(TryParse(JObject.Parse(Sample), out var request, out var errorResponse), Is.True, errorResponse);

            return request!;

        }

        #endregion

        #region (private static) WrittenIsRead<T>(Sample, TryParse, ToJSON)

        /// <summary>
        /// For an extension without a schema: what is written from the sample is
        /// read again, and written again as it was the first time.
        /// </summary>
        private static JObject WrittenIsRead<T>(String           Sample,
                                                Parser<T>        TryParse,
                                                Func<T, JObject> ToJSON)
        {

            Assert.That(TryParse(JObject.Parse(Sample), out var value, out var errorResponse), Is.True, $"The sample was not read: {errorResponse}");

            var written = ToJSON(value!);

            Assert.That(TryParse(written, out var again, out errorResponse), Is.True,
                        $"What was written was not read: {errorResponse}{Environment.NewLine}{written.ToString(Newtonsoft.Json.Formatting.None)}");

            var writtenAgain = ToJSON(again!);

            Assert.That(JToken.DeepEquals(Normalized(writtenAgain), Normalized(written)), Is.True,
                        $"Written again: {Normalized(writtenAgain).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Written first: {Normalized(written).     ToString(Newtonsoft.Json.Formatting.None)}");

            return written;

        }

        #endregion


        #region PullDynamicScheduleUpdateResponse

        [Test]
        public void PullDynamicScheduleUpdateResponse()
        {

            var request = Request<PullDynamicScheduleUpdateRequest>(
                              """{ "chargingProfileId": 1 }""",
                              (JObject json, out PullDynamicScheduleUpdateRequest? value, out String? errorResponse) => PullDynamicScheduleUpdateRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse)
                          );

            ReadAndWrittenAsTheSchemaSays<PullDynamicScheduleUpdateResponse>(
                $$"""
                {
                    "status":     "Accepted",
                    "statusInfo": {{StatusInfo}},
                    "customData": {{Custom}}
                }
                """,
                (JObject json, out PullDynamicScheduleUpdateResponse? value, out String? errorResponse) => OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse.TryParse(request, json, destination, NetworkPath.Empty, out value, out errorResponse),
                value => value.ToJSON()
            );

        }

        #endregion

        #region SignCertificateRequest

        [Test]
        public void SignCertificateRequest()

            => ReadAndWrittenAsTheSchemaSays<SignCertificateRequest>(
                   $$"""
                   {
                       "csr":                 "-----BEGIN CERTIFICATE REQUEST-----\nMIIB\n-----END CERTIFICATE REQUEST-----",
                       "certificateType":     "V2GCertificate",
                       "hashRootCertificate": { "hashAlgorithm": "SHA256", "issuerNameHash": "aa", "issuerKeyHash": "bb", "serialNumber": "01" },
                       "requestId":           42,
                       "customData":          {{Custom}}
                   }
                   """,
                   (JObject json, out SignCertificateRequest? value, out String? errorResponse) => OCPPv2_1.CS.SignCertificateRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON(),
                   value => Assert.That(value.SignCertificateRequestId, Is.EqualTo(42))
               );

        [Test]
        public void SignCertificateRequest_WithOnlyItsCSR()

            => ReadAndWrittenAsTheSchemaSays<SignCertificateRequest>(
                   """{ "csr": "-----BEGIN CERTIFICATE REQUEST-----\nMIIB\n-----END CERTIFICATE REQUEST-----" }""",
                   (JObject json, out SignCertificateRequest? value, out String? errorResponse) => OCPPv2_1.CS.SignCertificateRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON(),
                   value => Assert.That(value.CertificateType, Is.Null)
               );

        #endregion

        #region LogStatusNotificationRequest

        [Test]
        public void LogStatusNotificationRequest()

            => ReadAndWrittenAsTheSchemaSays<LogStatusNotificationRequest>(
                   $$"""
                   {
                       "status":     "Uploaded",
                       "requestId":  1,
                       "statusInfo": {{StatusInfo}},
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out LogStatusNotificationRequest? value, out String? errorResponse) => OCPPv2_1.CS.LogStatusNotificationRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region FirmwareStatusNotificationRequest

        [Test]
        public void FirmwareStatusNotificationRequest()

            => ReadAndWrittenAsTheSchemaSays<FirmwareStatusNotificationRequest>(
                   $$"""
                   {
                       "status":     "Installed",
                       "requestId":  1,
                       "statusInfo": {{StatusInfo}},
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out FirmwareStatusNotificationRequest? value, out String? errorResponse) => OCPPv2_1.CS.FirmwareStatusNotificationRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region PublishFirmwareStatusNotificationRequest

        [Test]
        public void PublishFirmwareStatusNotificationRequest()

            => ReadAndWrittenAsTheSchemaSays<PublishFirmwareStatusNotificationRequest>(
                   $$"""
                   {
                       "status":     "Published",
                       "location":   [ "https://example.org/firmware.bin" ],
                       "requestId":  1,
                       "statusInfo": {{StatusInfo}},
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out PublishFirmwareStatusNotificationRequest? value, out String? errorResponse) => OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region ClearTariffsRequest

        [Test]
        public void ClearTariffsRequest()

            => ReadAndWrittenAsTheSchemaSays<ClearTariffsRequest>(
                   $$"""
                   {
                       "tariffIds":  [ "T1" ],
                       "evseId":     1,
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out ClearTariffsRequest? value, out String? errorResponse) => OCPPv2_1.CSMS.ClearTariffsRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region GetTariffsRequest

        [Test]
        public void GetTariffsRequest()

            => ReadAndWrittenAsTheSchemaSays<GetTariffsRequest>(
                   $$"""
                   {
                       "evseId":     0,
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out GetTariffsRequest? value, out String? errorResponse) => OCPPv2_1.CSMS.GetTariffsRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        [Test]
        public void GetTariffsRequest_WithoutEVSEId_IsRefused()

            => Assert.That(OCPPv2_1.CSMS.GetTariffsRequest.TryParse(new JObject(), requestId, destination, NetworkPath.Empty, out _, out _), Is.False);

        #endregion

        #region DataTransferRequest

        [Test]
        public void DataTransferRequest()

            => ReadAndWrittenAsTheSchemaSays<DataTransferRequest>(
                   $$"""
                   {
                       "vendorId":   "GraphDefined",
                       "messageId":  "Test",
                       "data":       { "a": 1 },
                       "customData": {{Custom}}
                   }
                   """,
                   (JObject json, out DataTransferRequest? value, out String? errorResponse) => OCPPv2_1.DataTransferRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region TriggerMessageRequest

        [Test]
        public void TriggerMessageRequest()

            => ReadAndWrittenAsTheSchemaSays<TriggerMessageRequest>(
                   $$"""
                   {
                       "requestedMessage": "CustomTrigger",
                       "customTrigger":    "Foo",
                       "evse":             { "id": 1 },
                       "customData":       {{Custom}}
                   }
                   """,
                   (JObject json, out TriggerMessageRequest? value, out String? errorResponse) => OCPPv2_1.CSMS.TriggerMessageRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region GetMonitoringReportRequest / SendLocalListRequest - lists the schema leaves optional

        [Test]
        public void GetMonitoringReportRequest_WithoutItsOptionalLists()

            => ReadAndWrittenAsTheSchemaSays<GetMonitoringReportRequest>(
                   """{ "requestId": 1 }""",
                   (JObject json, out GetMonitoringReportRequest? value, out String? errorResponse) => OCPPv2_1.CSMS.GetMonitoringReportRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        [Test]
        public void SendLocalListRequest_WithoutItsOptionalList()

            => ReadAndWrittenAsTheSchemaSays<SendLocalListRequest>(
                   """{ "versionNumber": 1, "updateType": "Full" }""",
                   (JObject json, out SendLocalListRequest? value, out String? errorResponse) => OCPPv2_1.CSMS.SendLocalListRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region AuthorizeResponse

        [Test]
        public void AuthorizeResponse()
        {

            var request = Request<AuthorizeRequest>(
                              """{ "idToken": { "idToken": "AABBCCDD", "type": "ISO14443" } }""",
                              (JObject json, out AuthorizeRequest? value, out String? errorResponse) => OCPPv2_1.CS.AuthorizeRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse)
                          );

            ReadAndWrittenAsTheSchemaSays<AuthorizeResponse>(
                $$"""
                {
                    "idTokenInfo": { "status": "Accepted" },
                    "tariff":      { "tariffId": "T1", "currency": "EUR", "energy": { "prices": [ { "priceKwh": 0.3 } ] } },
                    "customData":  {{Custom}}
                }
                """,
                (JObject json, out AuthorizeResponse? value, out String? errorResponse) => OCPPv2_1.CSMS.AuthorizeResponse.TryParse(request, json, destination, NetworkPath.Empty, out value, out errorResponse),
                value => value.ToJSON()
            );

        }

        #endregion

        #region TransactionEventRequest / TransactionEventResponse

        private const String TransactionEvent = """
            {
                "eventType":             "Updated",
                "timestamp":             "2026-10-09T12:00:00Z",
                "triggerReason":         "MeterValuePeriodic",
                "seqNo":                 1,
                "preconditioningStatus": "Ready",
                "evseSleep":             true,
                "transactionInfo":       { "transactionId": "TX1" }
            }
            """;

        private static Boolean TryParseTransactionEvent(JObject JSON, out TransactionEventRequest? Value, out String? ErrorResponse)

            => OCPPv2_1.CS.TransactionEventRequest.TryParse(JSON, requestId, destination, NetworkPath.Empty, out Value, out ErrorResponse);

        [Test]
        public void TransactionEventRequest()

            => ReadAndWrittenAsTheSchemaSays<TransactionEventRequest>(
                   TransactionEvent,
                   TryParseTransactionEvent,
                   value => value.ToJSON(),
                   value => Assert.That(value.EVSESleep, Is.True)
               );

        [Test]
        public void TransactionEventResponse()
        {

            var request = Request<TransactionEventRequest>(TransactionEvent, TryParseTransactionEvent);

            ReadAndWrittenAsTheSchemaSays<TransactionEventResponse>(
                $$"""
                {
                    "totalCost":                   1.5,
                    "updatedPersonalMessage":      { "format": "UTF8", "language": "de", "content": "Hallo" },
                    "updatedPersonalMessageExtra": [ { "format": "UTF8", "language": "en", "content": "Hello" } ],
                    "customData":                  {{Custom}}
                }
                """,
                (JObject json, out TransactionEventResponse? value, out String? errorResponse) => OCPPv2_1.CSMS.TransactionEventResponse.TryParse(request, json, destination, NetworkPath.Empty, out value, out errorResponse),
                value => value.ToJSON()
            );

        }

        #endregion


        #region ListDirectoryRequest (extension)

        [Test]
        public void ListDirectoryRequest_WithSHA512FileHashes()
        {

            var written = WrittenIsRead<ListDirectoryRequest>(
                              """{ "directoryPath": "/", "withSHA512FileHashes": true }""",
                              (JObject json, out ListDirectoryRequest? value, out String? errorResponse) => NetworkingNode.ListDirectoryRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                              value => value.ToJSON()
                          );

            Assert.That(written["withSHA512FileHashes"]?.Value<Boolean>(), Is.True);

        }

        #endregion

        #region ComponentConfig / VariableConfig (extension)

        [Test]
        public void ComponentConfig()

            // Written exactly as it was read: read again, a configuration that
            // lost its instance, variables and description would be stable too.
            => ReadAndWrittenAsTheSchemaSays<ComponentConfig>(
                   """
                   {
                       "name":            "OCPPCommCtrlr",
                       "instance":        "1",
                       "variableConfigs": [ { "name": "HeartbeatInterval", "instance": "2", "description": { "en": "Seconds between heartbeats" } } ],
                       "description":     { "en": "Communication" }
                   }
                   """,
                   OCPPv2_1.ComponentConfig.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region CDRChargingPeriod / ImageLink (extension)

        [Test]
        public void CDRChargingPeriod()

            => WrittenIsRead<CDRChargingPeriod>(
                   """{ "startPeriod": 0, "tariffId": "DE-GDF-T1" }""",
                   OCPPv2_1.CDRChargingPeriod.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void ImageLink()

            => WrittenIsRead<ImageLink>(
                   """{ "category": "Operator", "size": "100x100", "text": [ "https://example.org/logo.png" ] }""",
                   OCPPv2_1.ImageLink.TryParse,
                   value => value.ToJSON()
               );

        #endregion

        #region TransparencySoftware / Energy_Meter (extension)

        [Test]
        public void TransparencySoftware()

            => WrittenIsRead<OCPP.TransparencySoftware>(
                   """
                   {
                       "name":                "Chargy Transparency Software",
                       "version":             "1.0",
                       "open_source_license": { "id": "AGPL-3.0", "URLs": [ "https://www.gnu.org/licenses/agpl-3.0.html" ] },
                       "vendor":              "GraphDefined GmbH"
                   }
                   """,
                   OCPP.TransparencySoftware.TryParse,
                   value => value.ToJSON()
               );

        [Test]
        public void Energy_Meter()
        {

            var written = WrittenIsRead<Energy_Meter>(
                              """{ "id": "EM1", "name": { "de": "Zähler" }, "serialNumber": "0815" }""",
                              OCPP.Energy_Meter.TryParse,
                              value => value.ToJSON(Embedded: true)
                          );

            Assert.That(written["serialNumber"]?.Value<String>(), Is.EqualTo("0815"));
            Assert.That(written["name"],                          Is.Not.Null);

        }

        #endregion

        #region NotifyWebPaymentStartedRequest (DataTransfer extension)

        [Test]
        public void NotifyWebPaymentStartedRequest_AsADataTransfer()

            => WrittenIsRead<OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest>(
                   """{ "evseId": 1, "timeout": 300 }""",
                   (JObject json, out OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest? value, out String? errorResponse) => OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest.TryParse(json, requestId, destination, NetworkPath.Empty, out value, out errorResponse),
                   value => value.ToJSON()
               );

        #endregion

        #region GetExecutingEnvironmentResponse (extension)

        /// <summary>
        /// Its restart URL was handed to the JSON as a URL, which Newtonsoft
        /// cannot write: ToJSON() threw whenever there was one.
        /// </summary>
        [Test]
        public void GetExecutingEnvironmentResponse_WithARestartURL()
        {

            var response = new OCPPv2_1.CS.GetExecutingEnvironmentResponse(
                               new OCPPv2_1.CSMS.GetExecutingEnvironmentRequest(destination),
                               GenericStatus.Accepted,
                               ProcessId:   4711,
                               RestartURL:  org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse("https://example.org/restart")
                           );

            Assert.That(response.ToJSON()["restartURL"]?.Value<String>(), Is.EqualTo("https://example.org/restart"));

        }

        #endregion

        #region ChargingTicket (extension)

        [Test]
        public void ChargingTicket()
        {

            var keyPair = ECCKeyPair.GenerateKeys()!;
            var tariff  = new Tariff(
                              Id:        Tariff_Id.Parse("DE-GDF-T12345678"),
                              Currency:  org.GraphDefined.Vanaheimr.Illias.Currency.EUR,
                              Energy:    new TariffEnergy([ new TariffEnergyPrice(0.51M) ])
                          );

            // In the keys it was read in before: kWh, kW and mA.
            var sample  = new JObject(
                              new JProperty("id",                       ChargingTicket_Id.NewRandom("DE-GDF").ToString()),
                              new JProperty("providerId",               "DE-GDF"),
                              new JProperty("providerName",             new DisplayTexts(org.GraphDefined.Vanaheimr.Illias.Languages.en, "GraphDefined EMP").ToJSON()),
                              new JProperty("driverPublicKey",          keyPair.ToECCPublicKey().ToJSON(CryptoSerialization.RAW)),
                              new JProperty("chargingTariffs",          new JArray(tariff.ToJSON())),
                              new JProperty("created",                  "2026-10-09T12:00:00Z"),
                              new JProperty("notBefore",                "2026-10-09T12:00:00Z"),
                              new JProperty("notAfter",                 "2026-10-10T12:00:00Z"),
                              new JProperty("EVSEKind",                 "AC"),
                              new JProperty("maxKWh",                   20),
                              new JProperty("MaxKW",                    11),
                              new JProperty("maxDuration",              3600),
                              new JProperty("multipleSessions",         "Allowed"),
                              new JProperty("validationMethod",         "ShouldBeValidated"),
                              new JProperty("smartChargingMode",        "Smart"),
                              new JProperty("MeterValueSignatureMode",  "Mandatory"),
                              new JProperty("evDriverCommunication",    "EphemeralKeyAgreement")
                          );

            Assert.That(OCPPv2_1.ChargingTicket.TryParse(sample, out var ticket, out var errorResponse), Is.True, errorResponse);
            Assert.That(ticket!.MaxEnergy?.Value,  Is.EqualTo(20000));
            Assert.That(ticket. MaxPower?. Value,  Is.EqualTo(11000));
            Assert.That(ticket. MaxDuration,       Is.EqualTo(TimeSpan.FromHours(1)));

            var written = ticket.ToJSON();

            Assert.That(written["notAfter"],     Is.Not.Null);
            Assert.That(written["maxEnergy"]?.Value<Decimal>(),   Is.EqualTo(20000));
            Assert.That(written["maxDuration"]?.Value<Decimal>(), Is.EqualTo(3600));

            Assert.That(OCPPv2_1.ChargingTicket.TryParse(written, out var again, out errorResponse), Is.True,
                        $"What was written was not read: {errorResponse}");

            Assert.That(JToken.DeepEquals(Normalized(again!.ToJSON()), Normalized(written)), Is.True,
                        $"Written again: {Normalized(again.ToJSON()).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Written first: {Normalized(written).       ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

    }

}
