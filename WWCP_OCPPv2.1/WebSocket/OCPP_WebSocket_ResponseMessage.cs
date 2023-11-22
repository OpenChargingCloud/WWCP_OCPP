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

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// A OCPP WebSocket response message.
    /// </summary>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Message">A JSON response message payload.</param>
    public class OCPP_WebSocket_ResponseMessage(Request_Id  RequestId,
                                                JObject     Message)
    {

        #region Properties

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id  RequestId    { get; } = RequestId;

        /// <summary>
        /// The JSON response message payload.
        /// </summary>
        public JObject     Message      { get; } = Message;

        #endregion


        #region TryParse(JSONArray, out ResponseMessage)

        /// <summary>
        /// Try to parse the given JSON representation of a response message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="ResponseMessage">The parsed OCPP WebSocket response message.</param>
        public static Boolean TryParse(JArray JSONArray, out OCPP_WebSocket_ResponseMessage? ResponseMessage)
        {

            ResponseMessage = null;

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

                if (JSONArray.Count            != 3                   ||
                    JSONArray[0].Type          != JTokenType.Integer  ||
                    JSONArray[0].Value<Byte>() != 3                   ||
                    JSONArray[1].Type          != JTokenType.String   ||
                    JSONArray[2].Type          != JTokenType.Object)
                {
                    return false;
                }

                if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var responseId))
                    return false;

                if (JSONArray[2] is not JObject jsonMessage)
                    return false;

                ResponseMessage = new OCPP_WebSocket_ResponseMessage(
                                      responseId,
                                      jsonMessage
                                  );

                return true;

            }
            catch
            {
                return false;
            }

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

            => new (3,
                    RequestId.ToString(),
                    Message);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{RequestId} => {Message}";

        #endregion


    }

}
