﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP HTTP WebSocket binary response message.
    /// </summary>
    /// <param name="ResponseTimestamp">The response time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The OCPP networking mode to use.</param>
    /// <param name="Destination">The networking node identification of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Payload">The binary response message payload.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_BinaryResponseMessage(DateTime           ResponseTimestamp,
                                            EventTracking_Id   EventTrackingId,
                                            NetworkingMode     NetworkingMode,
                                            SourceRouting      Destination,
                                            NetworkPath        NetworkPath,
                                            Request_Id         RequestId,
                                            Byte[]             Payload,
                                            CancellationToken  CancellationToken = default)
    {

        #region Properties

        /// <summary>
        /// The response time stamp.
        /// </summary>
        public DateTime           ResponseTimestamp    { get; }      = ResponseTimestamp;

        /// <summary>
        /// The event tracking identification.
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
        /// The (recorded) path of the response through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id         RequestId            { get; }      = RequestId;

        /// <summary>
        /// The binary response message payload.
        /// </summary>
        public Byte[]             Payload              { get; }      = Payload;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;

        #endregion


        #region TryParse(Binary, out BinaryResponseMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given binary representation of a response message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryResponseMessage">The parsed OCPP WebSocket response message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        public static Boolean TryParse(Byte[]                           Binary,
                                       out OCPP_BinaryResponseMessage?  BinaryResponseMessage,
                                       out String?                      ErrorResponse,
                                       NetworkingNode_Id?               ImplicitSourceNodeId   = null)
        {

            BinaryResponseMessage  = null;
            ErrorResponse          = null;

            if (Binary is null)
                return false;

            try
            {

                var stream                   = new MemoryStream(Binary);

                // MessageType: CALLRESULT (Server-to-Client)
                var messageType       = stream.ReadByte();
                if (messageType != 3)
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
                        ErrorResponse = $"The received source node identification '{sourceNodeIdText}' is invalid!";
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


                var requestIdLength          = stream.ReadUInt16();
                var requestIdText            = stream.ReadUTF8String(requestIdLength);

                if (!Request_Id.TryParse(requestIdText, out var requestId))
                {
                    ErrorResponse  = $"The received request identification '{requestIdText}' is invalid!";
                    return false;
                }

                var payloadLength            = stream.ReadUInt64();
                var payload                  = stream.ReadBytes(payloadLength);

                BinaryResponseMessage        = new OCPP_BinaryResponseMessage(
                                                   Timestamp.Now,
                                                   EventTracking_Id.New,
                                                   destinationNodeIdLength == 0 && networkPathLength == 0
                                                       ? NetworkingMode.Standard
                                                       : NetworkingMode.OverlayNetwork,
                                                   SourceRouting.To(destinationNodeId),
                                                   NetworkPath.From(networkPath),
                                                   requestId,
                                                   payload
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

            // MessageType: CALLRESULT (Server-to-Client)
            binaryStream.WriteByte  (3);

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

            var requestIdBytes = RequestId.ToString().ToUTF8Bytes();
            binaryStream.WriteUInt16((UInt16) requestIdBytes.Length);
            binaryStream.Write      (requestIdBytes);

            binaryStream.WriteUInt64((UInt64) Payload.       LongLength);
            binaryStream.Write      (Payload); //ToDo: Fix me for >2 GB!

            return binaryStream.ToArray();

        }

        #endregion


        public static OCPP_BinaryResponseMessage From(SourceRouting  Destination,
                                                      NetworkPath    NetworkPath,
                                                      Request_Id     RequestId,
                                                      Byte[]         Payload)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    Destination,
                    NetworkPath,
                    RequestId,
                    Payload);


        public static OCPP_BinaryResponseMessage From(NetworkingMode  NetworkingMode,
                                                      SourceRouting   Destination,
                                                      NetworkPath     NetworkPath,
                                                      Request_Id      RequestId,
                                                      Byte[]          Payload)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode,
                    Destination,
                    NetworkPath,
                    RequestId,
                    Payload);


        #region ChangeNetworking(NewDestinationId = null, NewNetworkPath = null, NewNetworkingMode = null)

        /// <summary>
        /// Change the destination node identification and network (source) path.
        /// </summary>
        /// <param name="NewDestinationId">An optional new destination node identification.</param>
        /// <param name="NewNetworkPath">An optional new (source) network path.</param>
        /// <param name="NewNetworkingMode">An optional new networking mode.</param>
        public OCPP_BinaryResponseMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
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

        #region ChangeDestionationId(NewDestinationId)

        /// <summary>
        /// Change the destination identification.
        /// </summary>
        /// <param name="NewDestinationId">A new destination identification.</param>
        public OCPP_BinaryResponseMessage ChangeDestionationId(SourceRouting NewDestination)

            => new (ResponseTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    NewDestination,
                    NetworkPath,
                    RequestId,
                    Payload,
                    CancellationToken);

        #endregion

        #region AppendToNetworkPath(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network (source) path.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification to append.</param>
        public OCPP_BinaryResponseMessage AppendToNetworkPath(NetworkingNode_Id NetworkingNodeId)

            => new (ResponseTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath.Append(NetworkingNodeId),
                    RequestId,
                    Payload,
                    CancellationToken);

        #endregion

        #region ChangeNetworkingMode(NewNetworkingMode)

        /// <summary>
        /// Change the networking mode.
        /// </summary>
        /// <param name="NetworkingNodeId">A new networking mode.</param>
        public OCPP_BinaryResponseMessage ChangeNetworkingMode(NetworkingMode NewNetworkingMode)

            => new (ResponseTimestamp,
                    EventTrackingId,
                    NewNetworkingMode,
                    Destination,
                    NetworkPath,
                    RequestId,
                    Payload,
                    CancellationToken);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{RequestId} => {Payload.ToBase64().SubstringMax(100)} [{Payload.Length} bytes]";

        #endregion


    }

}
