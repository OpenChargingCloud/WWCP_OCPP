/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP HTTP Web Socket binary request message.
    /// </summary>
    /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">The binary request message payload.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    public class OCPP_BinaryRequestMessage(NetworkingNode_Id  DestinationNodeId,
                                           NetworkPath        NetworkPath,
                                           Request_Id         RequestId,
                                           String             Action,
                                           Byte[]             Payload,
                                           String?            ErrorMessage   = null)
    {

        #region Properties

        /// <summary>
        /// The networking node identification of the message destination.
        /// </summary>
        public NetworkingNode_Id  DestinationNodeId    { get; } = DestinationNodeId;

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; } = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id         RequestId            { get; } = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; } = Action;

        /// <summary>
        /// The binary request message payload.
        /// </summary>
        public Byte[]             Payload             { get; } = Payload;

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?            ErrorMessage        { get; } = ErrorMessage;


        public Boolean            NoErrors
            => ErrorMessage is null;

        public Boolean            HasErrors
            => ErrorMessage is not null;

        #endregion


        #region TryParse(Binary, out BinaryRequestMessage, out ErrorResponse)

        /// <summary>
        /// Try to parse the given binary representation of a request message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryRequestMessage">The parsed OCPP WebSocket request message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Byte[]                          Binary,
                                       out OCPP_BinaryRequestMessage?  BinaryRequestMessage,
                                       out String?                     ErrorResponse)
        {

            BinaryRequestMessage  = null;
            ErrorResponse         = null;

            if (Binary is null)
                return false;

            try
            {

                var stream            = new MemoryStream(Binary);

                // MessageType: CALL (Client-to-Server)
                var messageType       = stream.ReadByte();
                if (messageType != 2)
                {
                    ErrorResponse = $"The received message type '{messageType}' is invalid!";
                    return false;
                }

                var requestIdLength   = stream.ReadUInt16();
                var requestIdText     = stream.ReadUTF8String(requestIdLength);

                if (!Request_Id.TryParse(requestIdText, out var requestId))
                {
                    ErrorResponse = $"The received request identification '{requestIdText}' is invalid!";
                    return false;
                }

                var actionLength      = stream.ReadUInt16();
                var action            = stream.ReadUTF8String(actionLength).Trim();

                if (action.IsNullOrEmpty())
                {
                    ErrorResponse = $"The received OCPP action must not be null or empty!";
                    return false;
                }

                var payloadLength     = stream.ReadUInt64();
                var payload           = stream.ReadBytes(payloadLength);

                BinaryRequestMessage  = new OCPP_BinaryRequestMessage(
                                            NetworkingNode_Id.Zero,
                                            NetworkPath.Empty,
                                            requestId,
                                            action,
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

        #region ToByteArray(NetworkingMode = Standard)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="NetworkingMode">The networking mode to use.</param>
        public Byte[] ToByteArray(NetworkingMode NetworkingMode = NetworkingMode.Standard)
        {

            var binaryStream = new MemoryStream();

            // MessageType: CALL (Client-to-Server)
            binaryStream.WriteByte(2);

            var requestIdBytes = RequestId.ToString().ToUTF8Bytes();
            binaryStream.WriteUInt16((UInt16) requestIdBytes.Length);
            binaryStream.Write(requestIdBytes,  0, requestIdBytes. Length);

            var actionBytes    = Action.   ToString().ToUTF8Bytes();
            binaryStream.WriteUInt16((UInt16) actionBytes.Length);
            binaryStream.Write(actionBytes,     0, actionBytes.Length);

            binaryStream.WriteUInt64((UInt64) Payload.LongLength);
            binaryStream.Write(Payload,   0, Payload. Length); //ToDo: Fix me for >2 GB!

            return binaryStream.ToArray();

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action} ({RequestId}) => {Payload.ToBase64().SubstringMax(100)} [{Payload.Length} bytes]";

        #endregion


    }

}
