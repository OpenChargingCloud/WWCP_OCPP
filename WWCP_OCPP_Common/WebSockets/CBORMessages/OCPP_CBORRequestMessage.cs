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
    /// An OCPP CBOR request message transport container: the array of a JSON request
    /// message, as CBOR, with the CBOR map of the request as its payload.
    /// </summary>
    /// <param name="RequestTimestamp">The request time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">A CBOR request message payload.</param>
    /// <param name="RequestTimeout">The request time out.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_CBORRequestMessage(DateTimeOffset     RequestTimestamp,
                                         EventTracking_Id   EventTrackingId,
                                         NetworkingMode     NetworkingMode,
                                         SourceRouting      Destination,
                                         NetworkPath        NetworkPath,
                                         Request_Id         RequestId,
                                         String             Action,
                                         CBORValue          Payload,
                                         DateTimeOffset?    RequestTimeout      = null,
                                         String?            ErrorMessage        = null,
                                         CancellationToken  CancellationToken   = default)
    {

        #region Properties

        /// <summary>
        /// The request time stamp.
        /// </summary>
        public DateTimeOffset     RequestTimestamp     { get; }      = RequestTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this request message with other events.
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
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id         RequestId            { get; }      = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; }      = Action;

        /// <summary>
        /// The CBOR request message payload.
        /// </summary>
        public CBORValue          Payload              { get; }      = Payload;

        /// <summary>
        /// The request time out.
        /// </summary>
        public DateTimeOffset     RequestTimeout       { get; set; } = RequestTimeout ?? (RequestTimestamp + OCPP_JSONRequestMessage.DefaultTimeout);

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?            ErrorMessage         { get; }      = ErrorMessage;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;


        /// <summary>
        /// The request message has no errors.
        /// </summary>
        public Boolean            NoErrors
            => ErrorMessage is null;

        /// <summary>
        /// The request message has errors.
        /// </summary>
        public Boolean            HasErrors
            => ErrorMessage is not null;

        #endregion


        #region (static) FromRequest(Request, SerializedRequest)

        /// <summary>
        /// Create a new OCPP CBOR request message transport container based on the given request.
        /// </summary>
        /// <param name="Request">A request message.</param>
        /// <param name="SerializedRequest">The request message as CBOR.</param>
        public static OCPP_CBORRequestMessage FromRequest(IRequest   Request,
                                                          CBORValue  SerializedRequest)

            => new (Timestamp.Now,
                    Request.EventTrackingId,
                    NetworkingMode.Unknown,
                    Request.Destination,
                    Request.NetworkPath,
                    Request.RequestId,
                    Request.Action,
                    SerializedRequest,
                    Timestamp.Now + Request.RequestTimeout,
                    null,
                    Request.CancellationToken);

        #endregion


        #region TryParse(CBORArray, out RequestMessage, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given CBOR representation of a request message.
        /// </summary>
        /// <param name="CBORArray">The CBOR array to be parsed.</param>
        /// <param name="RequestMessage">The parsed OCPP request message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request time stamp.</param>
        /// <param name="RequestTimeout">An optional request time out.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        public static Boolean TryParse(CBORValue                                          CBORArray,
                                       [NotNullWhen(true)]  out OCPP_CBORRequestMessage?  RequestMessage,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTimeOffset?                                    RequestTimestamp       = null,
                                       DateTimeOffset?                                    RequestTimeout         = null,
                                       EventTracking_Id?                                  EventTrackingId        = null,
                                       NetworkingNode_Id?                                 ImplicitSourceNodeId   = null,
                                       CancellationToken                                  CancellationToken      = default)
        {

            RequestMessage = null;

            if (!OCPP_CBORFrame.TrySplit(CBORArray,
                                         out var header,
                                         out var payload,
                                         out ErrorResponse))
            {
                return false;
            }

            if (!OCPP_JSONRequestMessage.TryParse(header,
                                                  out var jsonRequestMessage,
                                                  out ErrorResponse,
                                                  RequestTimestamp,
                                                  RequestTimeout,
                                                  EventTrackingId,
                                                  ImplicitSourceNodeId,
                                                  CancellationToken))
            {
                // No error response: This is not a request message, but may be a message of another kind.
                return false;
            }

            RequestMessage = new OCPP_CBORRequestMessage(
                                 jsonRequestMessage.RequestTimestamp,
                                 jsonRequestMessage.EventTrackingId,
                                 jsonRequestMessage.NetworkingMode,
                                 jsonRequestMessage.Destination,
                                 jsonRequestMessage.NetworkPath,
                                 jsonRequestMessage.RequestId,
                                 jsonRequestMessage.Action,
                                 payload,
                                 jsonRequestMessage.RequestTimeout,
                                 null,
                                 CancellationToken
                             );

            return true;

        }

        #endregion

        #region ToCBOR(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the CBOR array of this request message.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public CBORValue ToCBOR(NetworkingMode? ForcedNetworkingMode = null)

            => OCPP_CBORFrame.Join(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp,
                       EventTrackingId,
                       NetworkingMode,
                       Destination,
                       NetworkPath,
                       RequestId,
                       Action,
                       [],
                       RequestTimeout,
                       ErrorMessage,
                       CancellationToken
                   ).ToJSON(ForcedNetworkingMode),
                   Payload
               );

        #endregion

        #region ToByteArray(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the bytes of this request message, as they are sent in a binary frame.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public Byte[] ToByteArray(NetworkingMode? ForcedNetworkingMode = null)

            => ToCBOR(ForcedNetworkingMode).ToByteArray();

        #endregion


        #region ChangeRequestId(NewRequestId)

        /// <summary>
        /// Change the request identification.
        /// </summary>
        /// <param name="NewRequestId">A new request identification.</param>
        public OCPP_CBORRequestMessage ChangeRequestId(Request_Id NewRequestId)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath,
                    NewRequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region ChangeNetworking(NewDestination = null, NewNetworkPath = null, NewNetworkingMode = null)

        /// <summary>
        /// Change the destination, the network path and the networking mode.
        /// </summary>
        /// <param name="NewDestination">An optional new destination.</param>
        /// <param name="NewNetworkPath">An optional new network path.</param>
        /// <param name="NewNetworkingMode">An optional new networking mode.</param>
        public OCPP_CBORRequestMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
                                                        NetworkPath?     NewNetworkPath      = null,
                                                        NetworkingMode?  NewNetworkingMode   = null)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NewNetworkingMode ?? NetworkingMode,
                    NewDestination    ?? Destination,
                    NewNetworkPath    ?? NetworkPath,
                    RequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region AppendToNetworkPath(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network path.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification.</param>
        public OCPP_CBORRequestMessage AppendToNetworkPath(NetworkingNode_Id NetworkingNodeId)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath.Append(NetworkingNodeId),
                    RequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"REQUEST[{Action}/{RequestId}] => {Payload.ToDiagnosticString()}";

        #endregion

    }

}
