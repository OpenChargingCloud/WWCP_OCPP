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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    internal static partial class MessageSamples
    {

        private const String Custom      = """{ "vendorId": "GraphDefined" }""";
        private const String StatusInfo  = """{ "reasonCode": "NoError", "additionalInfo": "fine" }""";
        private const String IdToken     = """{ "idToken": "AABBCCDD", "type": "ISO14443" }""";
        private const String Tariff      = """{ "tariffId": "T1", "currency": "EUR", "energy": { "prices": [ { "priceKwh": 0.39 } ] } }""";

        /// <summary>
        /// A sample of each message of the extensions of this implementation -
        /// testing, binary streams, E2E charging tariffs and security, ... - that
        /// has no official schema: every property its parser reads, written as its
        /// writer writes it.
        /// </summary>
        /// <remarks>
        /// Not here: the binary messages (BinaryDataTransfer, GetFile, SendFile,
        /// SecureDataTransfer), which have no JSON, and their responses.
        /// </remarks>
        public static readonly IReadOnlyDictionary<String, String> OfTheExtensions = new Dictionary<String, String> {

            // Certificate revocation lists
            { "NotifyCRLRequest",                     $$"""{ "requestId": 2, "status": "Available", "location": "https://example.org/crl", "customData": {{Custom}} }""" },
            { "NotifyCRLResponse",                    $$"""{ "customData": {{Custom}} }""" },

            // E2E charging tariffs
            { "GetDefaultChargingTariffRequest",      $$"""{ "evseIds": [ 1, 2 ], "customData": {{Custom}} }""" },
            { "GetDefaultChargingTariffResponse",     $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "chargingTariffs": [ {{Tariff}} ], "chargingTariffMap": { "T1": [ "1", "2" ] }, "customData": {{Custom}} }""" },
            { "GetUserChargingTariffRequest",         $$"""{ "idToken": {{IdToken}}, "customData": {{Custom}} }""" },
            // Its class has no tariffs yet - its parser reads them and drops them.
            { "GetUserChargingTariffResponse",        $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "RemoveDefaultChargingTariffRequest",   $$"""{ "chargingTariffId": "T1", "evseIds": [ 1, 2 ], "customData": {{Custom}} }""" },
            { "RemoveDefaultChargingTariffResponse",  $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "evseStatusInfos": [ { "evseId": 1, "status": "NotFound", "reasonCode": "NoTariff", "additionalInfo": "none", "customData": {{Custom}} } ], "customData": {{Custom}} }""" },
            { "SetDefaultE2EChargingTariffRequest",   $$"""{ "chargingTariff": {{Tariff}}, "evseIds": [ 1, 2 ], "customData": {{Custom}} }""" },
            { "SetDefaultE2EChargingTariffResponse",  $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "evseStatusInfos": [ { "evseId": 1, "status": "TooLarge", "customData": {{Custom}} } ], "customData": {{Custom}} }""" },
            { "SetUserChargingTariffRequest",         $$"""{ "idToken": {{IdToken}}, "chargingTariff": {{Tariff}}, "customData": {{Custom}} }""" },
            { "SetUserChargingTariffResponse",        $$"""{ "status": "Rejected", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },

            // Binary streams: files and directories
            { "DeleteFileRequest",                    $$"""{ "fileName": "/logs/2026.txt", "fileSHA256": "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8=", "fileSHA512": "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+Pw==", "customData": {{Custom}} }""" },
            { "DeleteFileResponse",                   $$"""{ "fileName": "/logs/2026.txt", "status": "Locked", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "GetFileRequest",                       $$"""{ "fileName": "/logs/2026.txt", "priority": 2, "customData": {{Custom}} }""" },
            { "ListDirectoryRequest",                 $$"""{ "directoryPath": "/logs", "format": "JSONWithMetadata", "withFileSizes": true, "withFileDates": true, "withSHA256FileHashes": true, "withSHA512FileHashes": true, "customData": {{Custom}} }""" },
            { "ListDirectoryResponse",                $$"""{ "directoryPath": "/logs", "status": "Success", "directoryListing": { "2026.txt": null, "old": { "2025.txt": null } }, "customData": {{Custom}} }""" },

            // E2E security: signature policies and user roles
            { "UpdateSignaturePolicyRequest",         $$"""{ "customData": {{Custom}} }""" },
            { "UpdateSignaturePolicyResponse",        $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "DeleteUserRoleRequest",                $$"""{ "customData": {{Custom}} }""" },
            { "DeleteUserRoleResponse",               $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "UpdateUserRoleRequest",                $$"""{ "customData": {{Custom}} }""" },
            { "UpdateUserRoleResponse",               $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },

            // NTS key establishment
            { "NTSKERequest",                         $$"""{ "aeadAlgorithm": "AES_256_GCM", "customData": {{Custom}} }""" },
            { "NTSKEResponse",                        $$"""{ "serverInfos": [ { "c2sKey": "AQID", "s2cKey": "BAUG", "cookies": [ "BwgJ" ], "urls": [ "https://time.example" ], "aeadAlgorithm": "AES_256_GCM" } ], "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },

            // Web payments: NotifyWebPaymentStarted is a message of OCPP 2.1, NotifyWebPaymentFailed a DataTransfer of the vendor "cloud.charging.open"
            { "NotifyWebPaymentFailedRequest",       $$"""{ "vendorId": "cloud.charging.open", "messageId": "NotifyWebPaymentFailed", "data": { "evseId": 1, "errorMessage": { "en": "declined" } }, "customData": {{Custom}} }""" },
            { "NotifyWebPaymentFailedResponse",       $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },

            // Testing extensions
            { "AdjustTimeScaleRequest",              $$"""{ "scale": 2.5, "customData": {{Custom}} }""" },
            { "AdjustTimeScaleResponse",              $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "AttachCableRequest",                   $$"""{ "evseId": 1, "resistorValue": 2700.5, "connectorId": 1, "customData": {{Custom}} }""" },
            { "AttachCableResponse",                  $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "GetPWMValueRequest",                   $$"""{ "evseId": 1, "customData": {{Custom}} }""" },
            { "GetPWMValueResponse",                  $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "SetCPVoltageRequest",                  $$"""{ "voltage": 9.5, "voltageError": 2.5, "processingDelay": 2, "transitionTime": 3, "customData": {{Custom}} }""" },
            { "SetCPVoltageResponse",                 $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "SetCEVoltageRequest",                  $$"""{ "voltage": 9.5, "voltageError": 2.5, "processingDelay": 2, "transitionTime": 3, "customData": {{Custom}} }""" },
            { "SetCEVoltageResponse",                 $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "SetIDVoltageRequest",                  $$"""{ "voltage": 9.5, "voltageError": 2.5, "processingDelay": 2, "transitionTime": 3, "customData": {{Custom}} }""" },
            { "SetIDVoltageResponse",                 $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "SetErrorStateRequest",                 $$"""{ "faultType": "ResidualCurrent", "evse": { "id": 1, "connectorId": 1 }, "processingDelay": 2, "duration": 60, "customData": {{Custom}} }""" },
            { "SetErrorStateResponse",                $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "SwipeRFIDCardRequest",                 $$"""{ "idToken": {{IdToken}}, "readerId": 1, "simulationMode": "Hardware", "processingDelay": 2, "customData": {{Custom}} }""" },
            { "SwipeRFIDCardResponse",                $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" },
            { "GetExecutingEnvironmentRequest",       $$"""{ "customData": {{Custom}} }""" },
            { "GetExecutingEnvironmentResponse",      $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "processId": 4711, "restartURL": "https://example.org/restart", "restartSecret": "secret", "customData": {{Custom}} }""" },
            { "TimeTravelRequest",                    $$"""{ "timestamp": "2026-10-09T12:00:00Z", "customData": {{Custom}} }""" },
            { "TimeTravelResponse",                   $$"""{ "status": "Accepted", "statusInfo": {{StatusInfo}}, "customData": {{Custom}} }""" }

        };

        /// <summary>
        /// Every sample: of the messages with an official schema, and of the extensions.
        /// </summary>
        public static IReadOnlyDictionary<String, String> All
            => all.Value;

        // Read at its first use - after the samples of both parts of this class are there.
        private static readonly Lazy<IReadOnlyDictionary<String, String>> all

            = new (() => FromTheSchemas.Concat(OfTheExtensions).ToDictionary(sample => sample.Key, sample => sample.Value));

    }

}
