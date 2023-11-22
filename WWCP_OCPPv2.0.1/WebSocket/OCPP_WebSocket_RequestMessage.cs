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

namespace cloud.charging.open.protocols.OCPPv2_0_1.WebSockets
{

    /// <summary>
    /// An OCPP WebSocket request message.
    /// </summary>
    public class OCPP_WebSocket_RequestMessage
    {

        #region Properties

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id                   RequestId       { get; }

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String                       Action          { get; }

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public JObject                      Message         { get; }

        /// <summary>
        /// The message type.
        /// </summary>
        public OCPP_WebSocket_MessageTypes  MessageType     { get; }

        /// <summary>
        /// The optional error message.
        /// </summary>
        public String?                      ErrorMessage    { get; }

        public Boolean NoErrors
            => ErrorMessage is null;

        public Boolean HasErrors
            => ErrorMessage is not null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP WebSocket response message.
        /// </summary>
        /// <param name="RequestId">An unique request identification.</param>
        /// <param name="Action">An OCPP action/method name.</param>
        /// <param name="Message">A JSON request message payload.</param>
        /// <param name="MessageType">A message type (default: CALL (2) )</param>
        /// <param name="ErrorMessage">An optional error message.</param>
        public OCPP_WebSocket_RequestMessage(Request_Id                   RequestId,
                                             String                       Action,
                                             JObject                      Message,
                                             OCPP_WebSocket_MessageTypes  MessageType    = OCPP_WebSocket_MessageTypes.CALL,
                                             String?                      ErrorMessage   = null)
        {

            this.RequestId     = RequestId;
            this.Action        = Action;
            this.Message       = Message ?? new JObject();
            this.MessageType   = MessageType;
            this.ErrorMessage  = ErrorMessage;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            // [
            //     2,                  // MessageType: CALL (Client-to-Server)
            //    "19223201",          // RequestId
            //    "BootNotification",  // Action
            //    {
            //        "chargePointVendor": "VendorX",
            //        "chargePointModel":  "SingleSocketCharger"
            //    }
            // ]

            => new ((Byte) MessageType,
                    RequestId.ToString(),
                    Action,
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


        #region TryParse(Text, out RequestMessage)

        /// <summary>
        /// Try to parse the given text representation of a request message.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestMessage">The parsed OCPP WebSocket request message.</param>
        public static Boolean TryParse(String Text, out OCPP_WebSocket_RequestMessage? RequestMessage)
        {

            RequestMessage = null;

            if (Text is null)
                return false;

            // [
            //     2,                  // MessageType: CALL (Client-to-Server)
            //    "19223201",          // RequestId
            //    "BootNotification",  // Action
            //    {
            //        "chargePointVendor": "VendorX",
            //        "chargePointModel":  "SingleSocketCharger"
            //    }
            // ]

            try
            {

                var json = JArray.Parse(Text);

                if (json.Count != 4)
                    return false;

                if (!Byte.TryParse(json[0].Value<String>(), out Byte messageTypeByte))
                    return false;

                var messageType = OCPP_WebSocket_MessageTypesExtensions.ParseMessageType(messageTypeByte);
                if (messageType == OCPP_WebSocket_MessageTypes.Undefined)
                    return false;

                if (!Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId))
                    return false;

                var action = json[2]?.Value<String>();
                if (action is null)
                    return false;

                if (json[3] is not JObject jsonMessage)
                    return false;

                RequestMessage = new OCPP_WebSocket_RequestMessage(requestId,
                                                                   action,
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

            => String.Concat(
                   RequestId,
                   " => ",
                   Message.ToString()
               );

        #endregion


    }

}
