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

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP CBOR response message transport container: the array of a JSON response
    /// message, as CBOR, with the CBOR map of the response as its payload.
    /// </summary>
    /// <param name="ResponseTimestamp">The response time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the response through the overlay network.</param>
    /// <param name="RequestId">The unique request identification copied from the request.</param>
    /// <param name="Payload">A CBOR response message payload.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_CBORResponseMessage(DateTimeOffset     ResponseTimestamp,
                                          EventTracking_Id   EventTrackingId,
                                          NetworkingMode     NetworkingMode,
                                          SourceRouting      Destination,
                                          NetworkPath        NetworkPath,
                                          Request_Id         RequestId,
                                          CBORValue          Payload,
                                          CancellationToken  CancellationToken = default)
    {

        #region Properties

        /// <summary>
        /// The response time stamp.
        /// </summary>
        public DateTimeOffset     ResponseTimestamp    { get; }      = ResponseTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this response message with other events.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification or Any- or Multicast address of the message destination.
        /// </summary>
        public SourceRouting      Destination          { get; }      = Destination;

        /// <summary>
        /// The (recorded) path of the response through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification copied from the request.
        /// </summary>
        public Request_Id         RequestId            { get; }      = RequestId;

        /// <summary>
        /// The CBOR response message payload.
        /// </summary>
        public CBORValue          Payload              { get; }      = Payload;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;

        #endregion


        #region TryParse(CBORArray, out ResponseMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given CBOR representation of a response message.
        /// </summary>
        /// <param name="CBORArray">The CBOR array to be parsed.</param>
        /// <param name="ResponseMessage">The parsed OCPP response message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        public static Boolean TryParse(CBORValue                                           CBORArray,
                                       [NotNullWhen(true)]  out OCPP_CBORResponseMessage?  ResponseMessage,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       NetworkingNode_Id?                                  ImplicitSourceNodeId   = null)
        {

            ResponseMessage = null;

            if (!OCPP_CBORFrame.TrySplit(CBORArray,
                                         out var header,
                                         out var payload,
                                         out ErrorResponse))
            {
                return false;
            }

            if (!OCPP_JSONResponseMessage.TryParse(header,
                                                   out var jsonResponseMessage,
                                                   out ErrorResponse,
                                                   ImplicitSourceNodeId))
            {
                // No error response: This is not a response message, but may be a message of another kind.
                return false;
            }

            ResponseMessage = new OCPP_CBORResponseMessage(
                                  jsonResponseMessage.ResponseTimestamp,
                                  jsonResponseMessage.EventTrackingId,
                                  jsonResponseMessage.NetworkingMode,
                                  jsonResponseMessage.Destination,
                                  jsonResponseMessage.NetworkPath,
                                  jsonResponseMessage.RequestId,
                                  payload,
                                  jsonResponseMessage.CancellationToken
                              );

            return true;

        }

        #endregion

        #region ToCBOR(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the CBOR array of this response message.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public CBORValue ToCBOR(NetworkingMode? ForcedNetworkingMode = null)

            => OCPP_CBORFrame.Join(
                   new OCPP_JSONResponseMessage(
                       ResponseTimestamp,
                       EventTrackingId,
                       NetworkingMode,
                       Destination,
                       NetworkPath,
                       RequestId,
                       [],
                       CancellationToken
                   ).ToJSON(ForcedNetworkingMode),
                   Payload
               );

        #endregion

        #region ToByteArray(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the bytes of this response message, as they are sent in a binary frame.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public Byte[] ToByteArray(NetworkingMode? ForcedNetworkingMode = null)

            => ToCBOR(ForcedNetworkingMode).ToByteArray();

        #endregion


        #region ChangeNetworking(NewDestination = null, NewNetworkPath = null, NewNetworkingMode = null)

        /// <summary>
        /// Change the destination, the network path and the networking mode.
        /// </summary>
        /// <param name="NewDestination">An optional new destination.</param>
        /// <param name="NewNetworkPath">An optional new network path.</param>
        /// <param name="NewNetworkingMode">An optional new networking mode.</param>
        public OCPP_CBORResponseMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
                                                         NetworkPath?     NewNetworkPath      = null,
                                                         NetworkingMode?  NewNetworkingMode   = null)

            => new (ResponseTimestamp,
                    EventTrackingId,
                    NewNetworkingMode ?? NetworkingMode,
                    NewDestination    ?? Destination,
                    NewNetworkPath    ?? NetworkPath,
                    RequestId,
                    Payload,
                    CancellationToken);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"RESPONSE[{RequestId}] => {Payload.ToDiagnosticString()}";

        #endregion

    }

}
