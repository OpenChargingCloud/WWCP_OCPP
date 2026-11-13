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

            // E2E security: signer roles and the signature policy
            { "AddSignerRoleRequest",                 $$"""{ "signerRole": { "role": "support", "description": "Telephone support", "permissions": [ "state:read", "reset:run" ], "keys": [ { "kid": "NzbLsXh8uDCcd-6MNwXF4W_7noWXFZAfHkxZsRGC9Xs", "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" }, "label": "Technician 4711" } ], "version": 7, "signatures": [ { "protected": "eyJhbGciOiJFUzI1NiJ9", "header": { "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" } }, "signature": "DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q" } ] }, "customData": {{Custom}} }""" },
            { "AddSignerRoleResponse",                $$"""{ "status": "Rejected", "reasonCode": "VersionNotNewer", "additionalInfo": "The role is known in version 7.", "customData": {{Custom}} }""" },
            { "UpdateSignerRoleRequest",              $$"""{ "signerRole": { "role": "support", "description": "Telephone support", "permissions": [ "state:read", "reset:run" ], "keys": [ { "kid": "NzbLsXh8uDCcd-6MNwXF4W_7noWXFZAfHkxZsRGC9Xs", "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" }, "label": "Technician 4711" } ], "version": 7, "signatures": [ { "protected": "eyJhbGciOiJFUzI1NiJ9", "header": { "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" } }, "signature": "DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q" } ] }, "customData": {{Custom}} }""" },
            { "UpdateSignerRoleResponse",             $$"""{ "status": "Rejected", "reasonCode": "VersionNotNewer", "additionalInfo": "The role is known in version 7.", "customData": {{Custom}} }""" },
            { "DeleteSignerRoleRequest",              $$"""{ "role": "support", "version": 7, "customData": {{Custom}} }""" },
            { "DeleteSignerRoleResponse",             $$"""{ "status": "Rejected", "reasonCode": "VersionNotNewer", "additionalInfo": "The role is known in version 7.", "customData": {{Custom}} }""" },
            { "GetSignerRolesRequest",                $$"""{ "roles": [ "support", "systemadmin" ], "customData": {{Custom}} }""" },
            { "GetSignerRolesResponse",               $$"""{ "status": "Accepted", "signerRoles": [ { "role": "support", "description": "Telephone support", "permissions": [ "state:read", "reset:run" ], "keys": [ { "kid": "NzbLsXh8uDCcd-6MNwXF4W_7noWXFZAfHkxZsRGC9Xs", "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" }, "label": "Technician 4711" } ], "version": 7, "signatures": [ { "protected": "eyJhbGciOiJFUzI1NiJ9", "header": { "jwk": { "crv": "P-256", "kty": "EC", "x": "f83OJ3D2xF1Bg8vub9tLe1gHMzV76e8Tus9uPHvRVEU", "y": "x_FEzRu9m36HLN_tue659LNpXW6pCyStikYjKIWI5a0" } }, "signature": "DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q" } ] } ], "customData": {{Custom}} }""" },
            { "SetSignaturePolicyRequest",            $$"""{ "policy": { "version": 3, "entries": [ { "id": "firmware", "match": { "action": "UpdateFirmware", "direction": "CSMS->CS" }, "require": { "anyOf": [ "release-manager" ], "count": 2 }, "onViolation": "Refuse" } ], "signatures": [ { "protected": "eyJhbGciOiJFUzI1NiJ9", "signature": "DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q" } ] }, "customData": {{Custom}} }""" },
            { "SetSignaturePolicyResponse",           $$"""{ "status": "Rejected", "reasonCode": "VersionNotNewer", "additionalInfo": "The role is known in version 7.", "customData": {{Custom}} }""" },
            { "GetSignaturePolicyRequest",            $$"""{ "customData": {{Custom}} }""" },
            { "GetSignaturePolicyResponse",           $$"""{ "status": "Accepted", "policy": { "version": 3, "entries": [ { "id": "firmware", "match": { "action": "UpdateFirmware", "direction": "CSMS->CS" }, "require": { "anyOf": [ "release-manager" ], "count": 2 }, "onViolation": "Refuse" } ], "signatures": [ { "protected": "eyJhbGciOiJFUzI1NiJ9", "signature": "DtEhU3ljbEg8L38VWAfUAqOyKAM6-Xx-F4GawxaepmXFCgfTjDxw5djxLa8ISlSApmWQxfKTUJqPP3-Kg6NU1Q" } ] }, "customData": {{Custom}} }""" },

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
            { "QueryDNSRequest",                      $$"""{ "name": "ntske1.cpo.example", "recordTypes": [ "A", "AAAA" ], "server": 2, "customData": {{Custom}} }""" },
            { "QueryDNSResponse",                     $$"""{ "status": "Accepted", "reasonCode": "UnknownServer", "name": "1.2.0.192.in-addr.arpa.", "reverseLookupOf": "192.0.2.1", "recordTypes": [ "PTR", "TYPE65534" ], "server": 2, "answeredBy": "tls://[2001:db8::53]:853", "responseCode": "NoError", "authoritative": true, "truncated": false, "timedOut": false, "dnssec": "Insecure", "runtime": 38, "answers": [ { "name": "1.2.0.192.in-addr.arpa.", "type": "PTR", "ttl": 3600, "value": "resolver1.cpo.example." } ], "omittedAnswers": 3, "certificates": [ { "server": "2001:db8::53", "accepted": false, "status": "PinMismatch", "certificateFingerprint": "41C7", "rootFingerprint": "9D2E" } ], "error": "none", "customData": {{Custom}} }""" },

            // NTS: synchronization and server tests on demand (Secure Time Synchronization, section 3.3.5)
            { "SyncNetworkTimeRequest",               $$"""{ "requestId": 4711, "group": "legal", "customData": {{Custom}} }""" },
            { "SyncNetworkTimeResponse",              $$"""{ "status": "Rejected", "statusInfo": { "reasonCode": "Busy", "additionalInfo": "A round runs." }, "customData": {{Custom}} }""" },
            { "TestTimeServerRequest",                $$"""{ "requestId": 4712, "server": "ptbtime2.ptb.de", "group": "legal", "customData": {{Custom}} }""" },
            { "TestTimeServerResponse",               $$"""{ "status": "Rejected", "statusInfo": { "reasonCode": "UnknownServer", "additionalInfo": "Not listed." }, "customData": {{Custom}} }""" },
            { "NotifyNetworkTimeResultRequest",       $$"""{ "requestId": 4712, "generatedAt": "2026-11-10T14:04:01Z", "test": { "host": "ptbtime2.ptb.de", "ok": false, "failedAt": "TimeRequest", "duration": 3.096, "offset": 0.0012, "roundTrip": 0.019, "steps": [ { "elapsed": 0, "stage": "Configuration", "level": "Info", "text": "Asking." }, { "elapsed": 3.096, "stage": "TimeRequest", "level": "Error", "text": "No answer." } ] }, "customData": {{Custom}} }""" },
            { "NotifyNetworkTimeResultResponse",      $$"""{ "customData": {{Custom}} }""" },

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

            = new (AllSamples);

        // A method, not a lambda in the initializer: there the compiler takes the other part's fields as possibly null.
        private static IReadOnlyDictionary<String, String> AllSamples()
            => FromTheSchemas.Concat(OfTheExtensions).ToDictionary(sample => sample.Key, sample => sample.Value);

    }

}
