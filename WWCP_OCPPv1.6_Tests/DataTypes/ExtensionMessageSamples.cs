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

namespace cloud.charging.open.protocols.OCPPv1_6.tests.DataTypes
{

    internal static partial class MessageSamples
    {

        private const String Custom = """{ "vendorId": "GraphDefined" }""";

        /// <summary>
        /// A sample of each message of the extensions of this implementation
        /// that has no official schema: every property its parser reads,
        /// written as its writer writes it.
        /// </summary>
        /// <remarks>
        /// Not here: the binary messages (BinaryDataTransfer, SecureDataTransfer),
        /// which have no JSON, and the messages of signature policies, user roles
        /// and the network topology, which have no parser yet.
        /// </remarks>
        public static readonly IReadOnlyDictionary<String, String> OfTheExtensions = new Dictionary<String, String> {

            // Web payments: DataTransfers of the vendor "cloud.charging.open"
            { "NotifyWebPaymentStartedRequest",       $$"""{ "vendorId": "cloud.charging.open", "messageId": "NotifyWebPaymentStarted", "data": { "connectorId": 1, "timeout": 30 }, "customData": {{Custom}} }""" },
            { "NotifyWebPaymentStartedResponse",      $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "NotifyWebPaymentFailedRequest",        $$"""{ "vendorId": "cloud.charging.open", "messageId": "NotifyWebPaymentFailed", "data": { "connectorId": 1, "errorMessage": { "en": "declined" } }, "customData": {{Custom}} }""" },
            { "NotifyWebPaymentFailedResponse",       $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },

            // Testing extensions
            { "AdjustTimeScaleRequest",               $$"""{ "scale": 2.5, "customData": {{Custom}} }""" },
            { "AdjustTimeScaleResponse",              $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "AttachCableRequest",                   $$"""{ "connectorId": 1, "resistorValue": 2700.5, "customData": {{Custom}} }""" },
            { "AttachCableResponse",                  $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "GetPWMValueRequest",                   $$"""{ "connectorId": 1, "customData": {{Custom}} }""" },
            { "GetPWMValueResponse",                  $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "SetCPVoltageRequest",                  $$"""{ "voltage": 9.5, "voltageError": 2.5, "processingDelay": 2, "transitionTime": 3, "customData": {{Custom}} }""" },
            { "SetCPVoltageResponse",                 $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "SetErrorStateRequest",                 $$"""{ "faultType": "PhaseLoss", "connectorId": 1, "processingDelay": 2, "duration": 60, "customData": {{Custom}} }""" },
            { "SetErrorStateResponse",                $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "SwipeRFIDCardRequest",                 $$"""{ "idTag": "AABBCCDD", "readerId": 1, "simulationMode": "Hardware", "processingDelay": 2, "customData": {{Custom}} }""" },
            { "SwipeRFIDCardResponse",                $$"""{ "status": "Accepted", "customData": {{Custom}} }""" },
            { "GetExecutingEnvironmentRequest",       $$"""{ "customData": {{Custom}} }""" },
            { "GetExecutingEnvironmentResponse",      $$"""{ "status": "Accepted", "processId": 4711, "restartURL": "https://example.org/restart", "restartSecret": "secret", "customData": {{Custom}} }""" },
            { "TimeTravelRequest",                    $$"""{ "timestamp": "2026-10-09T12:00:00Z", "customData": {{Custom}} }""" },
            { "TimeTravelResponse",                   $$"""{ "status": "Accepted", "customData": {{Custom}} }""" }

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
