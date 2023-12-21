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

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP HTTP Web Socket JSON response message.
    /// </summary>
    /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Payload">A JSON response message payload.</param>
    /// <param name="NetworkPath">The optional (recorded) path of the request through the overlay network.</param>
    public class OCPP_JSONResponseMessage(DateTime           ResponseTimestamp,
                                          EventTracking_Id   EventTrackingId,
                                          NetworkingMode     NetworkingMode,
                                          NetworkingNode_Id  DestinationNodeId,
                                          NetworkPath        NetworkPath,
                                          Request_Id         RequestId,
                                          JObject            Payload)
    {

        #region Properties

        public DateTime           ResponseTimestamp    { get; } = ResponseTimestamp;
        public EventTracking_Id   EventTrackingId      { get; } = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; } = NetworkingMode;

        /// <summary>
        /// The networking node identification of the message destination.
        /// </summary>
        public NetworkingNode_Id  DestinationNodeId    { get; } = DestinationNodeId;

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; } = NetworkPath;

        /// <summary>
        /// The unique request identification copied from the request.
        /// </summary>
        public Request_Id         RequestId            { get; } = RequestId;

        /// <summary>
        /// The JSON response message payload.
        /// </summary>
        public JObject            Payload              { get; } = Payload;

        #endregion


        #region TryParse(JSONArray, out ResponseMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given JSON representation of a response message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="ResponseMessage">The parsed OCPP WebSocket response message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP Web Sockets connection.</param>
        public static Boolean TryParse(JArray                         JSONArray,
                                       out OCPP_JSONResponseMessage?  ResponseMessage,
                                       out String?                    ErrorResponse,
                                       NetworkingNode_Id?             ImplicitSourceNodeId   = null)
        {

            ResponseMessage  = null;
            ErrorResponse    = null;

            try
            {

                // [
                //     3,                         // MessageType: CALLRESULT (Server-to-Client)
                //    "19223201",                 // RequestId copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                if (JSONArray.Count            == 3                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 3                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.Object)
                {

                    var networkPath = NetworkPath.Empty;

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {
                        networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                    }

                    if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var requestId))
                        return false;

                    if (JSONArray[2] is not JObject payload)
                        return false;

                    ResponseMessage = new OCPP_JSONResponseMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.Standard,
                                          NetworkingNode_Id.Zero,
                                          networkPath,
                                          requestId,
                                          payload
                                      );

                    return true;

                }


                // [
                //    3,                          // MessageType: CALLRESULT (Server-to-Client)
                //    DestinationNodeId,
                //    [ NetworkPath ],
                //    "19223201",                 // RequestId copied from request
                //    {
                //        "status":            "Accepted",
                //        "currentTime":       "2013-02-01T20:53:32.486Z",
                //        "heartbeatInterval":  300
                //    }
                // ]

                if (JSONArray.Count            == 5                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 3                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.Array    &&
                    JSONArray[3].Type          == JTokenType.String   &&
                    JSONArray[4].Type          == JTokenType.Object)
                {

                    if (!NetworkingNode_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var destinationNodeId))
                        return false;

                    if (JSONArray[2] is not JArray networkPathJSONArray ||
                        !NetworkPath.TryParse(networkPathJSONArray, out var networkPath, out _) || networkPath is null)
                        return false;

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {

                        if (networkPath.Length > 0 &&
                            networkPath.Last() != ImplicitSourceNodeId)
                        {
                            networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                        }

                        if (networkPath.Length == 0)
                            networkPath = networkPath.Append(ImplicitSourceNodeId.Value);

                    }

                    if (!Request_Id.TryParse(JSONArray[3]?.Value<String>() ?? "", out var requestId))
                        return false;

                    if (JSONArray[4] is not JObject payload)
                        return false;

                    ResponseMessage = new OCPP_JSONResponseMessage(
                                          Timestamp.Now,
                                          EventTracking_Id.New,
                                          NetworkingMode.NetworkingExtensions,
                                          destinationNodeId,
                                          networkPath,
                                          requestId,
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

        #region ToJSON(NetworkingMode = Standard)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="NetworkingMode">The networking mode to use.</param>
        public JArray ToJSON(NetworkingMode NetworkingMode = NetworkingMode.Standard)

            => NetworkingMode == NetworkingMode.Standard

                   // OCPP Standard:

                   // [
                   //     3,                         // MessageType: CALLRESULT (Server-to-Client)
                   //    "19223201",                 // RequestId copied from request
                   //    {
                   //        "status":            "Accepted",
                   //        "currentTime":       "2013-02-01T20:53:32.486Z",
                   //        "heartbeatInterval":  300
                   //    }
                   // ]

                   ? new (3,
                          RequestId.ToString(),
                          Payload)


                   // Networking Nodes Extensions:

                   // [
                   //     3,                         // MessageType: CALLRESULT (Server-to-Client)
                   //     "CSMS",
                   //     [ "CS01", "NN01" ],
                   //    "19223201",                 // RequestId copied from request
                   //    {
                   //        "status":            "Accepted",
                   //        "currentTime":       "2013-02-01T20:53:32.486Z",
                   //        "heartbeatInterval":  300
                   //    }
                   // ]

                   : new (3,
                          DestinationNodeId.ToString(),
                          NetworkPath.      ToJSON(),
                          RequestId.        ToString(),
                          Payload);

        #endregion


        public static OCPP_JSONResponseMessage From(NetworkingNode_Id  DestinationNodeId,
                                                    NetworkPath        NetworkPath,
                                                    Request_Id         RequestId,
                                                    JObject            Payload)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    DestinationNodeId,
                    NetworkPath,
                    RequestId,
                    Payload);

        public static OCPP_JSONResponseMessage From(NetworkingMode     NetworkingMode,
                                                    NetworkingNode_Id  DestinationNodeId,
                                                    NetworkPath        NetworkPath,
                                                    Request_Id         RequestId,
                                                    JObject            Payload)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode,
                    DestinationNodeId,
                    NetworkPath,
                    RequestId,
                    Payload);


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{RequestId} => {Payload.ToString(Newtonsoft.Json.Formatting.None)}";

        #endregion


    }

}
