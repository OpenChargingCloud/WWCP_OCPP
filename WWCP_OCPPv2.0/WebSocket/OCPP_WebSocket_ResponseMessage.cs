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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.WebSockets
{

    /// <summary>
    /// An OCPP web socket response message.
    /// </summary>
    public class OCPP_WebSocket_ResponseMessage
    {

        #region Properies

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id                   RequestId      { get; }

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public JObject                      Message        { get; }

        /// <summary>
        /// The message type.
        /// </summary>
        public OCPP_WebSocket_MessageTypes  MessageType    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP web socket response message.
        /// </summary>
        /// <param name="RequestId">An unique request identification.</param>
        /// <param name="Message">A JSON request message payload.</param>
        /// <param name="MessageType">A message type (default: CALLRESULT (3) )</param>
        public OCPP_WebSocket_ResponseMessage(Request_Id                   RequestId,
                                              JObject                      Message,
                                              OCPP_WebSocket_MessageTypes  MessageType = OCPP_WebSocket_MessageTypes.CALLRESULT)
        {

            this.RequestId    = RequestId;
            this.Message      = Message ?? new JObject();
            this.MessageType  = MessageType;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            // [
            //     3,                         // MessageType: CALLRESULT (Server-to-Client)
            //    "19223201",                 // RequestId copied from request
            //    {
            //        "status":            "Accepted",
            //        "currentTime":       "2013-02-01T20:53:32.486Z",
            //        "heartbeatInterval":  300
            //    }
            // ]

            => new ((Byte) MessageType,
                    RequestId.ToString(),
                    Message);

        #endregion

        #region ToByteArray(Format = None)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="Format">A JSON format.</param>
        public Byte[] ToByteArray(Formatting Format = Formatting.None)

            => Encoding.UTF8.GetBytes(ToJSON().ToString(Format));

        #endregion


        #region TryParse(Text, out ResponseMessage)

        /// <summary>
        /// Try to parse the given text representation of a response message.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="ResponseMessage">The parsed OCPP web socket response message.</param>
        public static Boolean TryParse(String Text, out OCPP_WebSocket_ResponseMessage? ResponseMessage)
        {

            ResponseMessage = null;

            if (Text is null)
                return false;

            // [
            //     3,                         // MessageType: CALLRESULT (Server-to-Client)
            //    "19223201",                 // RequestId copied from request
            //    {
            //        "status":            "Accepted",
            //        "currentTime":       "2013-02-01T20:53:32.486Z",
            //        "heartbeatInterval":  300
            //    }
            // ]

            try
            {
                var JSON = JArray.Parse(Text);

                if (JSON.Count != 3)
                    return false;

                if (!Byte.TryParse(JSON[0].Value<String>(), out Byte messageTypeByte))
                    return false;

                var messageType = OCPP_WebSocket_MessageTypesExtensions.ParseMessageType(messageTypeByte);
                if (messageType == OCPP_WebSocket_MessageTypes.Undefined)
                    return false;

                if (!Request_Id.TryParse(JSON[1]?.Value<String>() ?? "", out var responseId))
                    return false;

                if (JSON[2] is not JObject jsonMessage)
                    return false;

                ResponseMessage = new OCPP_WebSocket_ResponseMessage(responseId,
                                                                   jsonMessage,
                                                                   messageType);

                return true;

            }
            catch
            {
                return false;
            }

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(RequestId,
                             " => ",
                             Message.ToString());

        #endregion


    }

}
