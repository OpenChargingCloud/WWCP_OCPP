/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using System.Diagnostics.CodeAnalysis;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP HTTP Web Socket binary send message.
    /// </summary>
    /// <param name="MessageTimestamp">The message time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the message through the overlay network.</param>
    /// <param name="MessagetId">An unique message identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">The binary message payload.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_BinarySendMessage(DateTime           MessageTimestamp,
                                        EventTracking_Id   EventTrackingId,
                                        NetworkingMode     NetworkingMode,
                                        SourceRouting      Destination,
                                        NetworkPath        NetworkPath,
                                        Request_Id         MessagetId,
                                        String             Action,
                                        Byte[]             Payload,
                                        String?            ErrorMessage        = null,
                                        CancellationToken  CancellationToken   = default)
    {

        #region Properties

        /// <summary>
        /// The message time stamp.
        /// </summary>
        public DateTime           MessageTimestamp     { get; }      = MessageTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this message with other events.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification of the message destination.
        /// </summary>
        public SourceRouting      Destination          { get; }      = Destination;

        /// <summary>
        /// The (recorded) path of the message through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique message identification.
        /// </summary>
        public Request_Id         MessageId            { get; }      = MessagetId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; }      = Action;

        /// <summary>
        /// The binary message payload.
        /// </summary>
        public Byte[]             Payload              { get; }      = Payload;

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


        #region (static) FromMessage(Message, SerializedMessage)

        /// <summary>
        /// Create a new OCPP binary send message transport container based on the given binary send message.
        /// </summary>
        /// <param name="Message">A send message.</param>
        /// <param name="SerializedMessage">The serialized send message.</param>
        public static OCPP_BinarySendMessage FromMessage(IMessage  Message,
                                                         Byte[]    SerializedMessage)

            => new (Timestamp.Now,
                    Message.EventTrackingId,
                    NetworkingMode.Unknown,
                    Message.Destination,
                    Message.NetworkPath,
                    Message.RequestId,
                    Message.Action,
                    SerializedMessage,
                    null,
                    Message.CancellationToken);

        #endregion


        #region TryParse(Binary, out BinarySendMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given binary representation of a send message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinarySendMessage">The parsed OCPP WebSocket send message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP Web Sockets connection.</param>
        public static Boolean TryParse(Byte[]                                            Binary,
                                       [NotNullWhen(true)]  out OCPP_BinarySendMessage?  BinarySendMessage,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         RequestTimestamp       = null,
                                       EventTracking_Id?                                 EventTrackingId        = null,
                                       NetworkingNode_Id?                                ImplicitSourceNodeId   = null,
                                       CancellationToken                                 CancellationToken      = default)
        {

            BinarySendMessage  = null;
            ErrorResponse      = null;

            if (Binary is null)
                return false;

            try
            {

                var stream                   = new MemoryStream(Binary);

                // MessageType: CALL (Client-to-Server)
                var messageType              = stream.ReadByte();
                if (messageType != 2)
                {
                    ErrorResponse = $"The received message type '{messageType}' is invalid!";
                    return false;
                }


                // DestinationId
                var destinationNodeId        = NetworkingNode_Id.Zero;
                var destinationNodeIdLength  = stream.ReadUInt16();
                if (destinationNodeIdLength > 0)
                {

                    var destinationNodeIdText    = stream.ReadUTF8String(destinationNodeIdLength);

                    if (!NetworkingNode_Id.TryParse(destinationNodeIdText, out destinationNodeId))
                    {
                        ErrorResponse = $"The received destination node identification '{destinationNodeId}' is invalid!";
                        return false;
                    }

                }


                // NetworkPath
                var networkPathLength        = stream.ReadByte();
                var networkPath              = new List<NetworkingNode_Id>();

                for (var i=0; i<networkPathLength; i++)
                {

                    var sourceNodeIdLength   = stream.ReadUInt16();
                    var sourceNodeIdText     = stream.ReadUTF8String(sourceNodeIdLength);

                    if (!NetworkingNode_Id.TryParse(sourceNodeIdText, out var sourceNodeId))
                    {
                        ErrorResponse = $"The source node identification '{sourceNodeIdText}' at location {i+1} of the received network path is invalid!";
                        return false;
                    }

                    networkPath.Add(sourceNodeId);

                }

                if (ImplicitSourceNodeId.HasValue &&
                    ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                {

                    if (networkPath.Count > 0 &&
                        networkPath.Last() != ImplicitSourceNodeId)
                    {
                        networkPath.Add(ImplicitSourceNodeId.Value);
                    }

                    if (networkPath.Count == 0)
                        networkPath.Add(ImplicitSourceNodeId.Value);

                }


                // RequestId
                var requestIdLength          = stream.ReadUInt16();
                var requestIdText            = stream.ReadUTF8String(requestIdLength);

                if (!Request_Id.TryParse(requestIdText, out var requestId))
                {
                    ErrorResponse = $"The received request identification '{requestIdText}' is invalid!";
                    return false;
                }


                // Action
                var actionLength             = stream.ReadUInt16();
                var action                   = stream.ReadUTF8String(actionLength).Trim();

                if (action.IsNullOrEmpty())
                {
                    ErrorResponse = $"The received OCPP action must not be null or empty!";
                    return false;
                }


                // Payload
                var payloadLength            = stream.ReadUInt64();
                var payload                  = stream.ReadBytes(payloadLength);


                BinarySendMessage         = new OCPP_BinarySendMessage(
                                                   RequestTimestamp ?? Timestamp.Now,
                                                   EventTrackingId  ?? EventTracking_Id.New,
                                                   destinationNodeIdLength == 0 && networkPathLength == 0
                                                       ? NetworkingMode.Standard
                                                       : NetworkingMode.OverlayNetwork,
                                                   SourceRouting.To(destinationNodeId),
                                                   new NetworkPath(networkPath),
                                                   requestId,
                                                   action,
                                                   payload,
                                                   null,
                                                   CancellationToken
                                               );

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given binary representation of an OCPP WebSocket binary request message is invalid: " + e.Message;
            }

            return false;

        }

        #endregion

        #region ToByteArray()

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        public Byte[] ToByteArray()
        {

            var binaryStream = new MemoryStream();

            // MessageType: CALL (Client-to-Server)
            binaryStream.WriteByte(2);

            if (NetworkingMode == NetworkingMode.Standard)
            {
                // DestinationId length == 0x0000
                // NetworkPath       length == 0x00
                binaryStream.Write(new Byte[3]);
            }
            else if (NetworkingMode == NetworkingMode.OverlayNetwork)
            {

                var destinationNodeIdBytes = Destination.Next.ToString().ToUTF8Bytes();
                binaryStream.WriteUInt16((UInt16) destinationNodeIdBytes.Length);
                binaryStream.Write      (destinationNodeIdBytes);

                binaryStream.WriteByte  ((Byte) (NetworkPath?.Length ?? 0));

                if (NetworkPath is not null)
                {
                    foreach (var sourceNodeId in NetworkPath)
                    {
                        var sourceNodeIdBytes = sourceNodeId.ToString().ToUTF8Bytes();
                        binaryStream.WriteUInt16((UInt16) sourceNodeIdBytes.Length);
                        binaryStream.Write      (sourceNodeIdBytes);
                    }
                }

            }

            var requestIdBytes = MessageId.ToString().ToUTF8Bytes();
            binaryStream.WriteUInt16((UInt16) requestIdBytes.Length);
            binaryStream.Write      (requestIdBytes);

            var actionBytes    = Action.   ToString().ToUTF8Bytes();
            binaryStream.WriteUInt16((UInt16) actionBytes.   Length);
            binaryStream.Write      (actionBytes);

            binaryStream.WriteUInt64((UInt64) Payload.       LongLength);
            binaryStream.Write      (Payload); //ToDo: Fix me for >2 GB!

            return binaryStream.ToArray();

        }

        #endregion


        #region ChangeNetworking(NewDestinationId = null, NewNetworkPath = null, NewNetworkingMode = null)

        /// <summary>
        /// Change the destination identification, network (source) path and networking mode.
        /// </summary>
        /// <param name="NewDestinationId">An optional new destination identification.</param>
        /// <param name="NewNetworkPath">An optional new (source) network path.</param>
        /// <param name="NewNetworkingMode">An optional new networking mode.</param>
        public OCPP_BinarySendMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
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

        #region ChangeDestionationId(NewDestinationId)

        /// <summary>
        /// Change the destination identification.
        /// </summary>
        /// <param name="NewDestinationId">A new destination identification.</param>
        public OCPP_BinarySendMessage ChangeDestionationId(SourceRouting NewDestination)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    NewDestination,
                    NetworkPath,
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region AppendToNetworkPath(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network path.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification to append.</param>
        public OCPP_BinarySendMessage AppendToNetworkPath(NetworkingNode_Id NetworkingNodeId)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath.Append(NetworkingNodeId),
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion


        //public OCPP_BinarySendMessage ChangeDestinationId(NetworkingNode_Id NewDestinationId,
        //                                                  NetworkPath       NewNetworkPath)

        //    => new (MessageTimestamp,
        //            EventTrackingId,
        //            NetworkingMode,
        //            NewDestinationId,
        //            NewNetworkPath,
        //            MessageId,
        //            Action,
        //            Payload,
        //            ErrorMessage,
        //            CancellationToken);


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"SEND[{Action}/{MessageId}] => {Payload.ToBase64().SubstringMax(100)} [{Payload.Length} bytes]";

        #endregion


    }

}
