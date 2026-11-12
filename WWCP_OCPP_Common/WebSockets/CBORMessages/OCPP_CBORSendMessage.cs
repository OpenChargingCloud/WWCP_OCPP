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
    /// An OCPP CBOR send message transport container: the array of a JSON send
    /// message, as CBOR, with the CBOR map of the message as its payload.
    /// </summary>
    /// <param name="MessageTimestamp">The message time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the message through the overlay network.</param>
    /// <param name="MessageId">An unique message identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">A CBOR message payload.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_CBORSendMessage(DateTimeOffset     MessageTimestamp,
                                      EventTracking_Id   EventTrackingId,
                                      NetworkingMode     NetworkingMode,
                                      SourceRouting      Destination,
                                      NetworkPath        NetworkPath,
                                      Request_Id         MessageId,
                                      String             Action,
                                      CBORValue          Payload,
                                      String?            ErrorMessage        = null,
                                      CancellationToken  CancellationToken   = default)
    {

        #region Properties

        /// <summary>
        /// The message time stamp.
        /// </summary>
        public DateTimeOffset     MessageTimestamp     { get; }      = MessageTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this message with other events.
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
        /// The (recorded) path of the message through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique message identification.
        /// </summary>
        public Request_Id         MessageId            { get; }      = MessageId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; }      = Action;

        /// <summary>
        /// The CBOR message payload.
        /// </summary>
        public CBORValue          Payload              { get; }      = Payload;

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?            ErrorMessage         { get; }      = ErrorMessage;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;


        /// <summary>
        /// The message has no errors.
        /// </summary>
        public Boolean            NoErrors
            => ErrorMessage is null;

        /// <summary>
        /// The message has errors.
        /// </summary>
        public Boolean            HasErrors
            => ErrorMessage is not null;

        #endregion


        #region (static) FromDatagram(Message, SerializedMessage)

        /// <summary>
        /// Create a new OCPP CBOR send message transport container based on the given message.
        /// </summary>
        /// <param name="Message">A message.</param>
        /// <param name="SerializedMessage">The message as CBOR.</param>
        public static OCPP_CBORSendMessage FromDatagram(IMessage   Message,
                                                        CBORValue  SerializedMessage)

            => new (Timestamp.Now,
                    Message.EventTrackingId,
                    NetworkingMode.Unknown,
                    Message.Destination,
                    Message.NetworkPath,
                    Message.MessageId,
                    Message.Action,
                    SerializedMessage,
                    null,
                    Message.CancellationToken);

        #endregion


        #region TryParse(CBORArray, out SendMessage, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given CBOR representation of a send message.
        /// </summary>
        /// <param name="CBORArray">The CBOR array to be parsed.</param>
        /// <param name="SendMessage">The parsed OCPP send message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="MessageTimestamp">An optional message time stamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        public static Boolean TryParse(CBORValue                                       CBORArray,
                                       [NotNullWhen(true)]  out OCPP_CBORSendMessage?  SendMessage,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTimeOffset?                                 MessageTimestamp       = null,
                                       EventTracking_Id?                               EventTrackingId        = null,
                                       NetworkingNode_Id?                              ImplicitSourceNodeId   = null,
                                       CancellationToken                               CancellationToken      = default)
        {

            SendMessage = null;

            if (!OCPP_CBORFrame.TrySplit(CBORArray,
                                         out var header,
                                         out var payload,
                                         out ErrorResponse))
            {
                return false;
            }

            if (!OCPP_JSONSendMessage.TryParse(header,
                                               out var jsonSendMessage,
                                               out ErrorResponse,
                                               MessageTimestamp,
                                               EventTrackingId,
                                               ImplicitSourceNodeId,
                                               CancellationToken))
            {
                // No error response: This is not a send message, but may be a message of another kind.
                return false;
            }

            SendMessage = new OCPP_CBORSendMessage(
                              jsonSendMessage.MessageTimestamp,
                              jsonSendMessage.EventTrackingId,
                              jsonSendMessage.NetworkingMode,
                              jsonSendMessage.Destination,
                              jsonSendMessage.NetworkPath,
                              jsonSendMessage.MessageId,
                              jsonSendMessage.Action,
                              payload,
                              null,
                              CancellationToken
                          );

            return true;

        }

        #endregion

        #region ToCBOR(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the CBOR array of this send message.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public CBORValue ToCBOR(NetworkingMode? ForcedNetworkingMode = null)

            => OCPP_CBORFrame.Join(
                   new OCPP_JSONSendMessage(
                       MessageTimestamp,
                       EventTrackingId,
                       NetworkingMode,
                       Destination,
                       NetworkPath,
                       MessageId,
                       Action,
                       [],
                       ErrorMessage,
                       CancellationToken
                   ).ToJSON(ForcedNetworkingMode),
                   Payload
               );

        #endregion

        #region ToByteArray(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the bytes of this send message, as they are sent in a binary frame.
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
        public OCPP_CBORSendMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
                                                     NetworkPath?     NewNetworkPath      = null,
                                                     NetworkingMode?  NewNetworkingMode   = null)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NewNetworkingMode ?? NetworkingMode,
                    NewDestination    ?? Destination,
                    NewNetworkPath    ?? NetworkPath,
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"SEND[{Action}/{MessageId}] => {Payload.ToDiagnosticString()}";

        #endregion

    }

}
