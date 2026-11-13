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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The payload of an OCPP 2.1 message, taken from its JSON to its CBOR and back:
    /// <see cref="MessagesAsCBOR"/> for the messages of OCPP 2.1.
    /// </summary>
    public static class OCPPMessagesAsCBOR
    {

        /// <summary>
        /// The translation of the messages of OCPP 2.1.
        /// </summary>
        public static MessagesAsCBOR Of { get; } = new (typeof(OCPPMessagesAsCBOR).Assembly);


        /// <inheritdoc cref="MessagesAsCBOR.Requests"/>
        public static IEnumerable<String> Requests
            => Of.Requests;

        /// <inheritdoc cref="MessagesAsCBOR.Responses"/>
        public static IEnumerable<String> Responses
            => Of.Responses;

        /// <inheritdoc cref="MessagesAsCBOR.Messages"/>
        public static IEnumerable<String> Messages
            => Of.Messages;

        /// <inheritdoc cref="MessagesAsCBOR.SignedPayloadOf"/>
        public static Byte[]? SignedPayloadOf(ISignableMessage Message)

            => Of.SignedPayloadOf(Message);

        /// <inheritdoc cref="MessagesAsCBOR.KnowsRequest"/>
        public static Boolean KnowsRequest(String Action)

            => Of.KnowsRequest(Action);

        /// <inheritdoc cref="MessagesAsCBOR.KnowsResponse"/>
        public static Boolean KnowsResponse(String Action)

            => Of.KnowsResponse(Action);

        /// <inheritdoc cref="MessagesAsCBOR.TryReadRequest"/>
        public static Boolean TryReadRequest(String                           Action,
                                             JObject                          JSON,
                                             Request_Id                       RequestId,
                                             SourceRouting                    Destination,
                                             NetworkPath                      NetworkPath,
                                             [NotNullWhen(true)]  out Object? Request,
                                             [NotNullWhen(false)] out String? ErrorResponse)

            => Of.TryReadRequest(Action, JSON, RequestId, Destination, NetworkPath, out Request, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryRequestToCBOR"/>
        public static Boolean TryRequestToCBOR(String                           Action,
                                               JObject                          JSON,
                                               Request_Id                       RequestId,
                                               SourceRouting                    Destination,
                                               NetworkPath                      NetworkPath,
                                               out CBORValue                    CBOR,
                                               out Object?                      Request,
                                               [NotNullWhen(false)] out String? ErrorResponse)

            => Of.TryRequestToCBOR(Action, JSON, RequestId, Destination, NetworkPath, out CBOR, out Request, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryRequestToJSON"/>
        public static Boolean TryRequestToJSON(String                            Action,
                                               CBORValue                         CBOR,
                                               Request_Id                        RequestId,
                                               SourceRouting                     Destination,
                                               NetworkPath                       NetworkPath,
                                               [NotNullWhen(true)]  out JObject? JSON,
                                               out Object?                       Request,
                                               [NotNullWhen(false)] out String?  ErrorResponse)

            => Of.TryRequestToJSON(Action, CBOR, RequestId, Destination, NetworkPath, out JSON, out Request, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryResponseToCBOR"/>
        public static Boolean TryResponseToCBOR(String                           Action,
                                                Object?                          Request,
                                                JObject                          JSON,
                                                Request_Id                       RequestId,
                                                SourceRouting                    Destination,
                                                NetworkPath                      NetworkPath,
                                                out CBORValue                    CBOR,
                                                [NotNullWhen(false)] out String? ErrorResponse)

            => Of.TryResponseToCBOR(Action, Request, JSON, RequestId, Destination, NetworkPath, out CBOR, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryResponseToJSON"/>
        public static Boolean TryResponseToJSON(String                            Action,
                                                Object?                           Request,
                                                CBORValue                         CBOR,
                                                Request_Id                        RequestId,
                                                SourceRouting                     Destination,
                                                NetworkPath                       NetworkPath,
                                                [NotNullWhen(true)]  out JObject? JSON,
                                                [NotNullWhen(false)] out String?  ErrorResponse)

            => Of.TryResponseToJSON(Action, Request, CBOR, RequestId, Destination, NetworkPath, out JSON, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryMessageToCBOR"/>
        public static Boolean TryMessageToCBOR(String                           Action,
                                               JObject                          JSON,
                                               Request_Id                       MessageId,
                                               SourceRouting                    Destination,
                                               NetworkPath                      NetworkPath,
                                               out CBORValue                    CBOR,
                                               [NotNullWhen(false)] out String? ErrorResponse)

            => Of.TryMessageToCBOR(Action, JSON, MessageId, Destination, NetworkPath, out CBOR, out ErrorResponse);

        /// <inheritdoc cref="MessagesAsCBOR.TryMessageToJSON"/>
        public static Boolean TryMessageToJSON(String                            Action,
                                               CBORValue                         CBOR,
                                               Request_Id                        MessageId,
                                               SourceRouting                     Destination,
                                               NetworkPath                       NetworkPath,
                                               [NotNullWhen(true)]  out JObject? JSON,
                                               [NotNullWhen(false)] out String?  ErrorResponse)

            => Of.TryMessageToJSON(Action, CBOR, MessageId, Destination, NetworkPath, out JSON, out ErrorResponse);

    }

}
