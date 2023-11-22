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


        #region TryParse(Binary, out BinaryRequestMessage)

        /// <summary>
        /// Try to parse the given binary representation of a request message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryRequestMessage">The parsed OCPP WebSocket request message.</param>
        public static Boolean TryParse(Byte[] Binary, out OCPP_WebSocket_BinaryRequestMessage? BinaryRequestMessage)
        {

            BinaryRequestMessage = null;

            if (Binary is null)
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

                //var json = JArray.Parse(Binary);

                //if (json.Count != 4)
                //    return false;

                //if (!Byte.TryParse(json[0].Value<String>(), out var messageTypeByte))
                //    return false;

                //var messageType = OCPP_WebSocket_MessageTypesExtensions.ParseMessageType(messageTypeByte);
                //if (messageType == OCPP_WebSocket_MessageTypes.Undefined)
                //    return false;

                //if (!Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var requestId))
                //    return false;

                //var action = json[2]?.Value<String>();
                //if (action is null)
                //    return false;

                //if (json[3] is not JObject jsonMessage)
                //    return false;

                var requestId  = Request_Id.Parse("1");
                var action     = "test";

                BinaryRequestMessage = new OCPP_WebSocket_BinaryRequestMessage(
                                           requestId,
                                           action,
                                           Binary
                                       );

                return true;

            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region ToByteArray()

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        public Byte[] ToByteArray()

            => BinaryMessage;

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action} ({RequestId}) => {BinaryMessage}";

        #endregion


    }

}
