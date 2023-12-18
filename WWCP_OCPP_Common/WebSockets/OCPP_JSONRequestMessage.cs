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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP HTTP Web Socket JSON request message.
    /// </summary>
    /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">A JSON request message payload.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    public class OCPP_JSONRequestMessage(NetworkingMode     NetworkingMode,
                                         NetworkingNode_Id  DestinationNodeId,
                                         NetworkPath        NetworkPath,
                                         Request_Id         RequestId,
                                         String             Action,
                                         JObject            Payload,
                                         String?            ErrorMessage   = null)
    {

        #region Properties

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode      NetworkingMode       { get; } = NetworkingMode;

        /// <summary>
        /// The networking node identification of the message destination.
        /// </summary>
        public NetworkingNode_Id?  DestinationNodeId    { get; } = DestinationNodeId;

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath?        NetworkPath          { get; } = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id          RequestId            { get; } = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String              Action               { get; } = Action;

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public JObject             Payload              { get; } = Payload;

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?             ErrorMessage         { get; } = ErrorMessage;


        public Boolean             NoErrors
            => ErrorMessage is null;

        public Boolean             HasErrors
            => ErrorMessage is not null;

        #endregion


        #region TryParse(JSONArray, out RequestMessage, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a request message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="RequestMessage">The parsed OCPP WebSocket request message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray                        JSONArray,
                                       out OCPP_JSONRequestMessage?  RequestMessage,
                                       out String?                   ErrorResponse)
        {

            RequestMessage  = null;
            ErrorResponse   = null;

            try
            {

                // [
                //     2,                  // MessageType: CALL (Client-to-Server)
                //    "19223201",          // RequestId
                //    "BootNotification",  // Action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (JSONArray.Count            == 4                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 2                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.String   &&
                    JSONArray[3].Type          == JTokenType.Object)
                {

                    if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var requestId))
                        return false;

                    var action = JSONArray[2]?.Value<String>()?.Trim();
                    if (action is null || action.IsNullOrEmpty())
                        return false;

                    if (JSONArray[3] is not JObject payload)
                        return false;

                    RequestMessage = new OCPP_JSONRequestMessage(
                                         NetworkingMode.Standard,
                                         NetworkingNode_Id.Zero,
                                         NetworkPath.Empty,
                                         requestId,
                                         action,
                                         payload
                                     );

                    return true;

                }


                // [
                //     2,                  // MessageType: CALL (Client-to-Server)
                //     Destination,
                //     NetworkPath,
                //    "19223201",          // RequestId
                //    "BootNotification",  // Action
                //    {
                //        "chargePointVendor": "VendorX",
                //        "chargePointModel":  "SingleSocketCharger"
                //    }
                // ]

                if (JSONArray.Count            == 6                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 2                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.Array    &&
                    JSONArray[3].Type          == JTokenType.String   &&
                    JSONArray[4].Type          == JTokenType.String   &&
                    JSONArray[5].Type          == JTokenType.Object)
                {

                    if (!NetworkingNode_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var networkingNodeId))
                        return false;

                    if (JSONArray[2] is not JArray networkPathJSONArray ||
                        !NetworkPath.      TryParse(networkPathJSONArray, out var networkPath, out _))
                        return false;

                    if (!Request_Id.       TryParse(JSONArray[3]?.Value<String>() ?? "", out var requestId))
                        return false;

                    var action = JSONArray[4]?.Value<String>()?.Trim();
                    if (action is null || action.IsNullOrEmpty())
                        return false;

                    if (JSONArray[5] is not JObject payload)
                        return false;

                    RequestMessage = new OCPP_JSONRequestMessage(
                                         NetworkingMode.NetworkingExtensions,
                                         networkingNodeId,
                                         networkPath ?? NetworkPath.Empty,
                                         requestId,
                                         action,
                                         payload
                                     );

                    return true;

                }

            }
            catch
            { }

            return false;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            => NetworkingMode switch {

                   // [
                   //     2,                  // MessageType: CALL (Client-to-Server)
                   //     "CSMS",
                   //     [ "CS01", "NN01" ],
                   //    "19223201",          // RequestId
                   //    "BootNotification",  // Action
                   //    {
                   //        "chargePointVendor": "VendorX",
                   //        "chargePointModel":  "SingleSocketCharger"
                   //    }
                   // ]
                   NetworkingMode.NetworkingExtensions
                       => new (2,
                               DestinationNodeId?.ToString() ?? "",
                               NetworkPath?.      ToJSON()   ?? [],
                               RequestId.         ToString(),
                               Action,
                               Payload),


                    // [
                   //     2,                  // MessageType: CALL (Client-to-Server)
                   //    "19223201",          // RequestId
                   //    "BootNotification",  // Action
                   //    {
                   //        "chargePointVendor": "VendorX",
                   //        "chargePointModel":  "SingleSocketCharger"
                   //    }
                   // ]
                   _   => new (2,
                               RequestId.ToString(),
                               Action,
                               Payload)

                };

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action} ({RequestId}) => {Payload.ToString(Newtonsoft.Json.Formatting.None)}";

        #endregion


    }

}
