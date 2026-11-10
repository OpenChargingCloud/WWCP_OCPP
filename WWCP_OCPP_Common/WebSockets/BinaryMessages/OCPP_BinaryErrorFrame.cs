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

using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// The binary representation of a request error (CALLERROR) and of a response
    /// error (CALLRESULTERROR), shared by both.
    /// </summary>
    /// <remarks>
    /// The same header as every other binary OCPP message, then the three fields of
    /// a JSON error:
    ///
    ///     1 byte                message type: 4 = request error, 5 = response error
    ///     2 bytes + UTF-8       destination node identification (empty in standard mode)
    ///     1 byte                number of network path entries,
    ///                             each 2 bytes + UTF-8
    ///     2 bytes + UTF-8       request identification
    ///     2 bytes + UTF-8       error code
    ///     8 bytes + UTF-8       error description
    ///     8 bytes + UTF-8       error details, as a JSON object
    ///
    /// The message types are those of the JSON array's first element. Errors used to
    /// be written as that JSON text into a binary frame, which no binary parser read:
    /// the asker of a binary request waited out its whole timeout for an error that had
    /// been sent.
    /// </remarks>
    internal static class OCPP_BinaryErrorFrame
    {

        #region Message types

        /// <summary>
        /// The message type of a request error (CALLERROR).
        /// </summary>
        internal const Byte RequestError   = 4;

        /// <summary>
        /// The message type of a response error (CALLRESULTERROR).
        /// </summary>
        internal const Byte ResponseError  = 5;

        #endregion


        #region (internal) Write  (MessageType, NetworkingMode, Destination, NetworkPath, RequestId, ErrorCode, ErrorDescription, ErrorDetails)

        /// <summary>
        /// Return the binary representation of an error.
        /// </summary>
        internal static Byte[] Write(Byte            MessageType,
                                     NetworkingMode  NetworkingMode,
                                     SourceRouting   Destination,
                                     NetworkPath     NetworkPath,
                                     Request_Id      RequestId,
                                     ResultCode      ErrorCode,
                                     String          ErrorDescription,
                                     JObject         ErrorDetails)
        {

            var stream = new MemoryStream();

            stream.WriteByte(MessageType);

            if (NetworkingMode == NetworkingMode.OverlayNetwork)
            {

                WriteShortText(stream, Destination.Next.ToString());

                stream.WriteByte((Byte) (NetworkPath?.Length ?? 0));

                if (NetworkPath is not null)
                    foreach (var sourceNodeId in NetworkPath)
                        WriteShortText(stream, sourceNodeId.ToString());

            }

            // Standard mode, and every other: no destination, no network path. Errors are
            // often built with NetworkingMode.Unknown, and the transports set the mode of
            // their connection only just before they write - a header of nothing at all
            // would have shifted every field that follows.
            else
                stream.Write(new Byte[3]);

            WriteShortText(stream, RequestId.ToString());
            WriteShortText(stream, ErrorCode.ToString());
            WriteLongText (stream, ErrorDescription);
            WriteLongText (stream, ErrorDetails.ToString(Formatting.None));

            return stream.ToArray();

        }

        #endregion

        #region (internal) TryRead(Binary, MessageType, ImplicitSourceNodeId, out Fields, out ErrorResponse)

        /// <summary>
        /// The fields of a binary error.
        /// </summary>
        internal sealed record Fields(NetworkingMode  NetworkingMode,
                                      SourceRouting   Destination,
                                      NetworkPath     NetworkPath,
                                      Request_Id      RequestId,
                                      ResultCode      ErrorCode,
                                      String          ErrorDescription,
                                      JObject         ErrorDetails);


        /// <summary>
        /// Try to read a binary error of the given message type.
        /// </summary>
        /// <param name="Binary">The binary to be read.</param>
        /// <param name="MessageType">The expected message type.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the connection.</param>
        /// <param name="Error">The fields read.</param>
        /// <param name="ErrorResponse">Why the binary could not be read.</param>
        internal static Boolean TryRead(Byte[]              Binary,
                                        Byte                MessageType,
                                        NetworkingNode_Id?  ImplicitSourceNodeId,
                                        out Fields?         Error,
                                        out String?         ErrorResponse)
        {

            Error          = null;
            ErrorResponse  = null;

            if (Binary is null || Binary.Length == 0)
            {
                ErrorResponse = "The given binary error message is empty!";
                return false;
            }

            if (Binary[0] != MessageType)
            {
                ErrorResponse = $"The received message type '{Binary[0]}' is invalid!";
                return false;
            }

            try
            {

                var stream = new MemoryStream(Binary);
                stream.ReadByte();

                #region Destination and network path

                if (!TryReadShortText(stream, out var destinationText))
                {
                    ErrorResponse = "The binary error message ends within its destination!";
                    return false;
                }

                var destinationNodeId = NetworkingNode_Id.Zero;

                if (destinationText.Length > 0 &&
                    !NetworkingNode_Id.TryParse(destinationText, out destinationNodeId))
                {
                    ErrorResponse = $"The received destination node identification '{destinationText}' is invalid!";
                    return false;
                }

                var networkPathLength = stream.ReadByte();
                if (networkPathLength < 0)
                {
                    ErrorResponse = "The binary error message ends before its network path!";
                    return false;
                }

                var networkPath = new List<NetworkingNode_Id>();

                for (var i = 0; i < networkPathLength; i++)
                {

                    if (!TryReadShortText(stream, out var sourceNodeIdText))
                    {
                        ErrorResponse = "The binary error message ends within its network path!";
                        return false;
                    }

                    if (!NetworkingNode_Id.TryParse(sourceNodeIdText, out var sourceNodeId))
                    {
                        ErrorResponse = $"The received source node identification '{sourceNodeIdText}' is invalid!";
                        return false;
                    }

                    networkPath.Add(sourceNodeId);

                }

                // As the binary response parser: only when the sender recorded nothing.
                if (networkPath.Count == 0 &&
                    ImplicitSourceNodeId.HasValue &&
                    ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                {
                    networkPath.Add(ImplicitSourceNodeId.Value);
                }

                #endregion

                #region Request identification, error code, description and details

                if (!TryReadShortText(stream, out var requestIdText) ||
                    !Request_Id.TryParse(requestIdText, out var requestId))
                {
                    ErrorResponse = "The received request identification is invalid!";
                    return false;
                }

                if (!TryReadShortText(stream, out var errorCodeText) ||
                    !ResultCode.TryParse(errorCodeText, out var errorCode))
                {
                    ErrorResponse = "The received error code is invalid!";
                    return false;
                }

                if (!TryReadLongText(stream, out var errorDescription))
                {
                    ErrorResponse = "The binary error message ends within its error description!";
                    return false;
                }

                if (!TryReadLongText(stream, out var errorDetailsText))
                {
                    ErrorResponse = "The binary error message ends within its error details!";
                    return false;
                }

                JObject errorDetails;

                try
                {
                    errorDetails = errorDetailsText.Length == 0
                                       ? []
                                       : JObject.Parse(errorDetailsText);
                }
                catch (Exception e)
                {
                    ErrorResponse = $"The received error details are not a JSON object: {e.Message}";
                    return false;
                }

                if (stream.Position != stream.Length)
                {
                    ErrorResponse = $"The binary error message has {stream.Length - stream.Position} byte(s) after its error details!";
                    return false;
                }

                #endregion

                Error = new Fields(
                            destinationText.Length == 0 && networkPathLength == 0
                                ? NetworkingMode.Standard
                                : NetworkingMode.OverlayNetwork,
                            SourceRouting.To(destinationNodeId),
                            NetworkPath.From(networkPath),
                            requestId,
                            errorCode,
                            errorDescription,
                            errorDetails
                        );

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given binary representation of an OCPP error message is invalid: " + e.Message;
                return false;
            }

        }

        #endregion


        #region (private) Write/read a text with a 2 or 8 byte length

        private static void WriteShortText(MemoryStream Stream, String Text)
        {

            var bytes = Encoding.UTF8.GetBytes(Text);

            if (bytes.Length > UInt16.MaxValue)
                throw new ArgumentException($"A text of {bytes.Length} bytes does not fit a 2 byte length!", nameof(Text));

            Stream.WriteUInt16((UInt16) bytes.Length);
            Stream.Write      (bytes);

        }

        private static void WriteLongText(MemoryStream Stream, String Text)
        {

            var bytes = Encoding.UTF8.GetBytes(Text);

            Stream.WriteUInt64((UInt64) bytes.LongLength);
            Stream.Write      (bytes);

        }

        private static Boolean TryReadShortText(MemoryStream Stream, out String Text)
        {

            Text = "";

            if (Stream.Length - Stream.Position < 2)
                return false;

            var length = Stream.ReadUInt16();

            if (Stream.Length - Stream.Position < length)
                return false;

            Text = length == 0 ? "" : Stream.ReadUTF8String(length);
            return true;

        }

        private static Boolean TryReadLongText(MemoryStream Stream, out String Text)
        {

            Text = "";

            if (Stream.Length - Stream.Position < 8)
                return false;

            var length = Stream.ReadUInt64();

            if ((UInt64) (Stream.Length - Stream.Position) < length)
                return false;

            Text = length == 0 ? "" : Encoding.UTF8.GetString(Stream.ReadBytes(length));
            return true;

        }

        #endregion

    }

}
