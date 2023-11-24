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

using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// A OCPP WebSocket binary request message.
    /// </summary>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="BinaryMessage">A binary request message payload.</param>
    /// <param name="ErrorMessage">An optional error message.</param>
    public class OCPP_WebSocket_BinaryRequestMessage(Request_Id  RequestId,
                                                     String      Action,
                                                     Byte[]      BinaryMessage,
                                                     String?     ErrorMessage   = null)
    {

        #region Properties

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id  RequestId        { get; } = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String      Action           { get; } = Action;

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public Byte[]      BinaryMessage    { get; } = BinaryMessage;

        /// <summary>
        /// The optional error message.
        /// </summary>
        public String?     ErrorMessage     { get; } = ErrorMessage;


        public Boolean NoErrors
            => ErrorMessage is null;

        public Boolean HasErrors
            => ErrorMessage is not null;

        #endregion


        #region TryParse(Binary, out BinaryRequestMessage, out ErrorResponse)

        /// <summary>
        /// Try to parse the given binary representation of a request message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryRequestMessage">The parsed OCPP WebSocket request message.</param>
        public static Boolean TryParse(Byte[]                                    Binary,
                                       out OCPP_WebSocket_BinaryRequestMessage?  BinaryRequestMessage,
                                       out String?                               ErrorResponse)
        {

            BinaryRequestMessage  = null;
            ErrorResponse         = null;

            if (Binary is null)
                return false;

            try
            {

                var stream  = new MemoryStream(Binary);
                var format  = BinaryFormatsExtensions.Parse(stream.ReadUInt16());

                Request_Id? requestId  = null;
                String?     action     = null;
                Byte[]?     data       = null;

                switch (format)
                {

                    case BinaryFormats.Compact:
                        {

                            //binaryStream.Write(BitConverter.GetBytes(VendorId.  NumericId));
                            //binaryStream.Write(BitConverter.GetBytes(MessageId?.NumericId ?? 0));
                            //binaryStream.Write(BitConverter.GetBytes((UInt32) (Data?.LongLength ?? 0)));

                            //if (Data is not null)
                            //    binaryStream.Write(Data,                                          0, (Int32) (Data?.LongLength ?? 0));

                            //var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                            //binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                            //if (IncludeSignatures) {
                            //    foreach (var signature in Signatures)
                            //    {
                            //        var binarySignature = signature.ToBinary();
                            //        binaryStream.Write(BitConverter.GetBytes((UInt16) binarySignature.Length));
                            //        binaryStream.Write(binarySignature);
                            //    }
                            //}

                        }
                        break;

                    case BinaryFormats.TextIds:
                        {

                            var requestIdLength  = stream.ReadUInt16();
                            var requestIdText    = stream.ReadUTF8String(requestIdLength);

                            if (Request_Id.TryParse(requestIdText, out var _requestId))
                                requestId = _requestId;
                            else
                            {
                                ErrorResponse = $"The received request identification '{requestIdText}' is invalid!";
                                return false;
                            }

                            var actionLength     = stream.ReadUInt16();
                                action           = stream.ReadUTF8String(actionLength).Trim();

                            if (action.IsNullOrEmpty())
                            {
                                ErrorResponse = $"The received OCPP action must not be null or empty!";
                                return false;
                            }

                            var dataLength       = stream.ReadUInt64();
                                data             = stream.ReadBytes(dataLength);

                        }
                        break;


                    case BinaryFormats.TagLengthValue:
                        {

                            //var vendorIdBytes  = VendorId.  ToString().ToUTF8Bytes();
                            //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.VendorId),   0, 2);
                            //binaryStream.Write(BitConverter.GetBytes((UInt16) vendorIdBytes.Length),  0, 2);
                            //binaryStream.Write(vendorIdBytes,                                         0, vendorIdBytes. Length);

                            //if (MessageId.HasValue) {
                            //    var messageIdBytes = MessageId?.ToString().ToUTF8Bytes() ?? [];
                            //    binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.MessageId),  0, 2);
                            //    binaryStream.Write(BitConverter.GetBytes((UInt16) messageIdBytes.Length), 0, 2);
                            //    binaryStream.Write(messageIdBytes,                                        0, messageIdBytes.Length);
                            //}

                            //var data = Data                                          ?? [];
                            //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.Data),       0, 2);
                            //binaryStream.Write(BitConverter.GetBytes((UInt16) 0),                     0, 2);
                            //binaryStream.Write(BitConverter.GetBytes((UInt32) data.Length),           0, 4);
                            //binaryStream.Write(data,                                                  0, data.          Length);

                            //var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                            //binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                            //if (signaturesCount > 0)
                            //{

                            //}

                        }
                        break;

                }

                if (requestId.HasValue &&
                    requestId.IsNotNullOrEmpty() &&
                    action is not null &&
                    action.   IsNotNullOrEmpty() &&
                    data   is not null)
                {

                    BinaryRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                               requestId.Value,
                                               action,
                                               data
                                           );

                    return true;

                }

            }
            catch (Exception e)
            {
                ErrorResponse = "The given binary representation of an OCPP WebSocket binary request message is invalid: " + e.Message;
            }

            return false;

        }

        #endregion

        #region ToByteArray(Format = TextIds)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="Format">The binary data format.</param>
        public Byte[] ToByteArray(BinaryFormats Format = BinaryFormats.TextIds)
        {

            var binaryStream = new MemoryStream();

            binaryStream.Write(Format.AsBytes(), 0, 2);

            switch (Format)
            {

                case BinaryFormats.Compact: {

                    //binaryStream.Write(BitConverter.GetBytes(VendorId.  NumericId));
                    //binaryStream.Write(BitConverter.GetBytes(MessageId?.NumericId ?? 0));
                    //binaryStream.Write(BitConverter.GetBytes((UInt32) (Data?.LongLength ?? 0)));

                    //if (Data is not null)
                    //    binaryStream.Write(Data,                                          0, (Int32) (Data?.LongLength ?? 0));

                    //var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                    //binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                    //if (IncludeSignatures) {
                    //    foreach (var signature in Signatures)
                    //    {
                    //        var binarySignature = signature.ToBinary();
                    //        binaryStream.Write(BitConverter.GetBytes((UInt16) binarySignature.Length));
                    //        binaryStream.Write(binarySignature);
                    //    }
                    //}

                }
                break;

                case BinaryFormats.TextIds: {

                    var vendorIdBytes  = RequestId.ToString().ToUTF8Bytes();
                    binaryStream.WriteUInt16((UInt16) vendorIdBytes.Length);
                    binaryStream.Write(vendorIdBytes,   0, vendorIdBytes. Length);

                    var messageIdBytes = Action.   ToString().ToUTF8Bytes();
                    binaryStream.WriteUInt16((UInt16) messageIdBytes.Length);
                    binaryStream.Write(messageIdBytes,  0, messageIdBytes.Length);

                    binaryStream.WriteUInt64((UInt64) BinaryMessage.LongLength);
                    binaryStream.Write(BinaryMessage,   0, BinaryMessage. Length); //ToDo: Fix me for >2 GB!

                }
                break;


                case BinaryFormats.TagLengthValue: {

                    //var vendorIdBytes  = VendorId.  ToString().ToUTF8Bytes();
                    //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.VendorId),   0, 2);
                    //binaryStream.Write(BitConverter.GetBytes((UInt16) vendorIdBytes.Length),  0, 2);
                    //binaryStream.Write(vendorIdBytes,                                         0, vendorIdBytes. Length);

                    //if (MessageId.HasValue) {
                    //    var messageIdBytes = MessageId?.ToString().ToUTF8Bytes() ?? [];
                    //    binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.MessageId),  0, 2);
                    //    binaryStream.Write(BitConverter.GetBytes((UInt16) messageIdBytes.Length), 0, 2);
                    //    binaryStream.Write(messageIdBytes,                                        0, messageIdBytes.Length);
                    //}

                    //var data = Data                                          ?? [];
                    //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.Data),       0, 2);
                    //binaryStream.Write(BitConverter.GetBytes((UInt16) 0),                     0, 2);
                    //binaryStream.Write(BitConverter.GetBytes((UInt32) data.Length),           0, 4);
                    //binaryStream.Write(data,                                                  0, data.          Length);

                    //var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                    //binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                    //if (signaturesCount > 0)
                    //{
                        
                    //}

                }
                break;
            }

            return binaryStream.ToArray();

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action} ({RequestId}) => {BinaryMessage.ToBase64().SubstringMax(100)}";

        #endregion


    }

}
