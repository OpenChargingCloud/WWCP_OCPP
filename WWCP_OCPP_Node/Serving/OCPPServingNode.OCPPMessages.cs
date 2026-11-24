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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The OCPP messages of this node - every one it sends and receives, over
    /// every connection, either way and in either OCPP version - as its OCPP
    /// log page shows them.
    /// </summary>
    public abstract partial class OCPPServingNode
    {

        #region Data

        private Boolean watchingOCPPMessages;

        #endregion

        #region Properties

        /// <summary>
        /// The OCPP messages this node sent and received, in memory - see
        /// <see cref="OCPPMessageLog"/>.
        /// </summary>
        public OCPPMessageLog  OCPPMessages
            => ocppMessages ??= new OCPPMessageLog(TimeProvider: TimeProvider);

        private OCPPMessageLog? ocppMessages;

        #endregion


        #region (protected) WatchOCPPMessages()

        /// <summary>
        /// Write down every OCPP message of the node's OCPP adapters - once,
        /// however often it is asked: when the server the stations connect to
        /// is built, which every kind does after its OCPP nodes exist.
        /// </summary>
        /// <remarks>
        /// At the adapters and not at a server or a client: they are where
        /// every message passes, whether it came over a WebSocket this node
        /// accepted, one it opened or SSH, and they know a message as a
        /// request, an answer or an error - which is what lets an answer be
        /// told by the action of what it answers.
        /// </remarks>
        protected void WatchOCPPMessages()
        {

            if (watchingOCPPMessages)
                return;

            watchingOCPPMessages = true;

            var node21 = Node21;

            if (node21 is not null)
                WatchOCPPMessages(node21.OCPP);

            var node16 = Node16;

            if (node16 is not null)
                WatchOCPPMessages(node16.OCPP);

        }

        #endregion

        #region (private) WatchOCPPMessages(OCPP 2.1)

        private void WatchOCPPMessages(OCPPv2_1.NetworkingNode.OCPPAdapter OCPP)
        {

            const String version = "2.1";

            OCPP.IN. OnJSONRequestMessageReceived       += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Request,       message.RequestId.ToString(), message.Action, PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONResponseMessageReceived      += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Response,      message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONRequestErrorMessageReceived  += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.RequestError,  message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONResponseErrorMessageReceived += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.ResponseError, message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONSendMessageReceived          += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Send,          message.MessageId.ToString(), message.Action, PeerOf(connection, message.NetworkPath.Source), message.ToJSON());

            OCPP.OUT.OnJSONRequestMessageSent           += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Request,       message.RequestId.ToString(), message.Action, PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONResponseMessageSent          += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Response,      message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONRequestErrorMessageSent      += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.RequestError,  message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONResponseErrorMessageSent     += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.ResponseError, message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONSendMessageSent              += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Send,          message.MessageId.ToString(), message.Action, PeerOf(connection, message.Destination.Next), message.ToJSON());

        }

        #endregion

        #region (private) WatchOCPPMessages(OCPP 1.6)

        private void WatchOCPPMessages(OCPPv1_6.NetworkingNode.OCPPAdapter OCPP)
        {

            const String version = "1.6";

            OCPP.IN. OnJSONRequestMessageReceived       += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Request,       message.RequestId.ToString(), message.Action, PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONResponseMessageReceived      += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Response,      message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONRequestErrorMessageReceived  += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.RequestError,  message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONResponseErrorMessageReceived += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.ResponseError, message.RequestId.ToString(), null,           PeerOf(connection, message.NetworkPath.Source), message.ToJSON());
            OCPP.IN. OnJSONSendMessageReceived          += (timestamp, sender, connection, message, ct)         => Received(version, connection, OCPPMessageKind.Send,          message.MessageId.ToString(), message.Action, PeerOf(connection, message.NetworkPath.Source), message.ToJSON());

            OCPP.OUT.OnJSONRequestMessageSent           += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Request,       message.RequestId.ToString(), message.Action, PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONResponseMessageSent          += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Response,      message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONRequestErrorMessageSent      += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.RequestError,  message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONResponseErrorMessageSent     += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.ResponseError, message.RequestId.ToString(), null,           PeerOf(connection, message.Destination.Next), message.ToJSON());
            OCPP.OUT.OnJSONSendMessageSent              += (timestamp, sender, connection, message, result, ct) => Sent    (version, result,     OCPPMessageKind.Send,          message.MessageId.ToString(), message.Action, PeerOf(connection, message.Destination.Next), message.ToJSON());

        }

        #endregion


        #region (private) Received(Version, Connection, Kind, MessageId, Action, Peer, Message)

        /// <summary>
        /// A message that came in: written down as it came.
        /// </summary>
        private Task Received(String              Version,
                              IMessageConnection? Connection,
                              OCPPMessageKind     Kind,
                              String              MessageId,
                              String?             Action,
                              String?             Peer,
                              JArray              Message)
        {

            OCPPMessages.Add(OCPPMessageDirection.In, Kind, Version, Peer, MessageId, Action, Message);

            return Task.CompletedTask;

        }

        #endregion

        #region (private) Sent    (Version, Result,     Kind, MessageId, Action, Peer, Message)

        /// <summary>
        /// A message that was sent - written down only where it went over the
        /// wire: one that found no connection to go over, or could not be
        /// written to it, is no OCPP message anybody received.
        /// </summary>
        private Task Sent(String              Version,
                          SentMessageResults  Result,
                          OCPPMessageKind     Kind,
                          String              MessageId,
                          String?             Action,
                          String?             Peer,
                          JArray              Message)
        {

            if (Result is SentMessageResults.Success or SentMessageResults.Broadcast)
                OCPPMessages.Add(OCPPMessageDirection.Out, Kind, Version, Peer, MessageId, Action, Message);

            return Task.CompletedTask;

        }

        #endregion

        #region (private) PeerOf(Connection, Routed)

        /// <summary>
        /// The node at the other end: the one the connection was registered
        /// as, else the one the message names - where it came from, or where
        /// it goes - else where the connection comes from.
        /// </summary>
        private String? PeerOf(IMessageConnection?  Connection,
                               NetworkingNode_Id    Routed)
        {

            if (Connection?.NetworkingNodeIdOf() is NetworkingNode_Id registered && registered.IsNotNullOrEmpty)
                return Readable(registered.ToString());

            if (Routed.IsNotNullOrEmpty && Routed != NetworkingNode_Id.Zero)
                return Readable(Routed.ToString());

            return Connection?.RemoteSocket.ToString();

        }

        /// <summary>
        /// A name as somebody reading the log knows it. A connection of this
        /// node that names no node at its other end is routed as
        /// "connection-" and its id (see WWCPNode.NextHopOf), which says
        /// nothing to anybody: it is shown as the connection was written
        /// down - its description, else where it goes.
        /// </summary>
        private String Readable(String Name)
        {

            const String prefix = "connection-";

            if (!Name.StartsWith(prefix, StringComparison.Ordinal))
                return Name;

            var id         = Name[prefix.Length..];
            var connection = Connections?.Connections.FirstOrDefault(entry => entry.Id == id);

            if (connection is null)
                return Name;

            var description = connection.Description.FirstText();

            return description.IsNotNullOrEmpty()
                       ? description
                       : connection.URL.ToString();

        }

        #endregion

    }

}
