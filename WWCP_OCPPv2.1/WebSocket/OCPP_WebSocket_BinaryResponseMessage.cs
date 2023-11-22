﻿/*
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
    /// An OCPP WebSocket binary response message.
    /// </summary>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="BinaryMessage">A binary response message payload.</param>
    public class OCPP_WebSocket_BinaryResponseMessage(Request_Id  RequestId,
                                                      Byte[]      BinaryMessage)
    {

        #region Properties

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id  RequestId        { get; } = RequestId;

        /// <summary>
        /// The binary response message payload.
        /// </summary>
        public Byte[]      BinaryMessage    { get; } = BinaryMessage;

        #endregion


        #region TryParse(Binary, out BinaryResponseMessage)

        /// <summary>
        /// Try to parse the given binary representation of a response message.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryResponseMessage">The parsed OCPP WebSocket response message.</param>
        public static Boolean TryParse(Byte[] Binary, out OCPP_WebSocket_BinaryResponseMessage? BinaryResponseMessage)
        {

            BinaryResponseMessage = null;

            if (Binary is null)
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

                //var json = JArray.Parse(Text);

                //if (json.Count != 3)
                //    return false;

                //if (!Byte.TryParse(json[0].Value<String>(), out var messageTypeByte))
                //    return false;

                //var messageType = OCPP_WebSocket_MessageTypesExtensions.ParseMessageType(messageTypeByte);
                //if (messageType == OCPP_WebSocket_MessageTypes.Undefined)
                //    return false;

                //if (!Request_Id.TryParse(json[1]?.Value<String>() ?? "", out var responseId))
                //    return false;

                //if (json[2] is not JObject jsonMessage)
                //    return false;

                var requestId = Request_Id.Parse("1");

                BinaryResponseMessage = new OCPP_WebSocket_BinaryResponseMessage(
                                            requestId,
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

            => $"{RequestId} => {BinaryMessage}";

        #endregion


    }

}
