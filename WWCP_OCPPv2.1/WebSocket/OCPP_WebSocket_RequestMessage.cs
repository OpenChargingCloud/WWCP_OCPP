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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// A OCPP WebSocket request message.
    /// </summary>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Message">A JSON request message payload.</param>
    /// <param name="ErrorMessage">An optional error message.</param>
    public class OCPP_WebSocket_RequestMessage(Request_Id  RequestId,
                                               String      Action,
                                               JObject     Message,
                                               String?     ErrorMessage   = null)
    {

        #region Properties

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id  RequestId       { get; } = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String      Action          { get; } = Action;

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public JObject     Message         { get; } = Message;

        /// <summary>
        /// The optional error message.
        /// </summary>
        public String?     ErrorMessage    { get; } = ErrorMessage;


        public Boolean NoErrors
            => ErrorMessage is null;

        public Boolean HasErrors
            => ErrorMessage is not null;

        #endregion


        #region TryParse(JSONArray, out RequestMessage)

        /// <summary>
        /// Try to parse the given JSON representation of a request message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="RequestMessage">The parsed OCPP WebSocket request message.</param>
        public static Boolean TryParse(JArray JSONArray, out OCPP_WebSocket_RequestMessage? RequestMessage)
        {

            RequestMessage = null;

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

                if (JSONArray.Count            != 4                   ||
                    JSONArray[0].Type          != JTokenType.Integer  ||
                    JSONArray[0].Value<Byte>() != 2                   ||
                    JSONArray[1].Type          != JTokenType.String   ||
                    JSONArray[2].Type          != JTokenType.String   ||
                    JSONArray[3].Type          != JTokenType.Object)
                {
                    return false;
                }

                if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var requestId))
                    return false;

                var action = JSONArray[2]?.Value<String>()?.Trim();
                if (action is null || action.IsNullOrEmpty())
                    return false;

                if (JSONArray[3] is not JObject jsonMessage)
                    return false;

                RequestMessage = new OCPP_WebSocket_RequestMessage(
                                     requestId,
                                     action,
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
            //     2,                  // MessageType: CALL (Client-to-Server)
            //    "19223201",          // RequestId
            //    "BootNotification",  // Action
            //    {
            //        "chargePointVendor": "VendorX",
            //        "chargePointModel":  "SingleSocketCharger"
            //    }
            // ]

            => new (2,
                    RequestId.ToString(),
                    Action,
                    Message);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action} ({RequestId}) => {Message}";

        #endregion


    }

}
