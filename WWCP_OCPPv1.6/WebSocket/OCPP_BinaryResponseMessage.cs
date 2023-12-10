///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;

//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
//{

//    /// <summary>
//    /// An OCPP HTTP Web Socket binary response message.
//    /// </summary>
//    /// <param name="RequestId">An unique request identification.</param>
//    /// <param name="Payload">The binary response message payload.</param>
//    public class OCPP_BinaryResponseMessage(Request_Id  RequestId,
//                                            Byte[]      Payload)
//    {

//        #region Properties

//        /// <summary>
//        /// The unique request identification.
//        /// </summary>
//        public Request_Id  RequestId    { get; } = RequestId;

//        /// <summary>
//        /// The binary response message payload.
//        /// </summary>
//        public Byte[]      Payload      { get; } = Payload;

//        #endregion


//        #region TryParse(Binary, out BinaryResponseMessage, out ErrorResponse)

//        /// <summary>
//        /// Try to parse the given binary representation of a response message.
//        /// </summary>
//        /// <param name="Binary">The binary to be parsed.</param>
//        /// <param name="BinaryResponseMessage">The parsed OCPP WebSocket response message.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        public static Boolean TryParse(Byte[]                           Binary,
//                                       out OCPP_BinaryResponseMessage?  BinaryResponseMessage,
//                                       out String?                      ErrorResponse)
//        {

//            BinaryResponseMessage  = null;
//            ErrorResponse          = null;

//            if (Binary is null)
//                return false;

//            try
//            {

//                var stream             = new MemoryStream(Binary);

//                // MessageType: CALLRESULT (Server-to-Client)
//                var messageType       = stream.ReadByte();
//                if (messageType != 3)
//                {
//                    ErrorResponse = $"The received message type '{messageType}' is invalid!";
//                    return false;
//                }

//                var requestIdLength    = stream.ReadUInt16();
//                var requestIdText      = stream.ReadUTF8String(requestIdLength);

//                if (!Request_Id.TryParse(requestIdText, out var requestId))
//                {
//                    ErrorResponse  = $"The received request identification '{requestIdText}' is invalid!";
//                    return false;
//                }

//                var payloadLength      = stream.ReadUInt64();
//                var payload            = stream.ReadBytes(payloadLength);

//                BinaryResponseMessage  = new OCPP_BinaryResponseMessage(
//                                             requestId,
//                                             payload
//                                         );

//                return true;

//            }
//            catch (Exception e)
//            {
//                ErrorResponse = "The given binary representation of an OCPP WebSocket binary request message is invalid: " + e.Message;
//            }

//            return false;

//        }

//        #endregion

//        #region ToByteArray()

//        /// <summary>
//        /// Return a binary representation of this object.
//        /// </summary>
//        public Byte[] ToByteArray()
//        {

//            var binaryStream = new MemoryStream();

//            // MessageType: CALLRESULT (Server-to-Client)
//            binaryStream.WriteByte(3);

//            var requestIdBytes = RequestId.ToString().ToUTF8Bytes();
//            binaryStream.WriteUInt16((UInt16) requestIdBytes.Length);
//            binaryStream.Write(requestIdBytes,  0, requestIdBytes.Length);

//            binaryStream.WriteUInt64((UInt64) Payload.LongLength);
//            binaryStream.Write(Payload,         0, Payload.       Length); //ToDo: Fix me for >2 GB!

//            return binaryStream.ToArray();

//        }

//        #endregion


//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => $"{RequestId} => {Payload.ToBase64().SubstringMax(100)} [{Payload.Length} bytes]";

//        #endregion


//    }

//}
