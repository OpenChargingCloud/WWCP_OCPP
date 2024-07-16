/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// An OCPP JSON request message transport container.
    /// </summary>
    /// <param name="RequestTimestamp">The request time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="DestinationId">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">A JSON request message payload.</param>
    /// <param name="RequestTimeout">The request time out.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_JSONRequestMessage(DateTime           RequestTimestamp,
                                         EventTracking_Id   EventTrackingId,
                                         NetworkingMode     NetworkingMode,
                                         NetworkingNode_Id  DestinationId,
                                         NetworkPath        NetworkPath,
                                         Request_Id         RequestId,
                                         String             Action,
                                         JObject            Payload,
                                         DateTime?          RequestTimeout      = null,
                                         String?            ErrorMessage        = null,
                                         CancellationToken  CancellationToken   = default)
    {

        #region Data

        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(333);

        #endregion

        #region Properties

        /// <summary>
        /// The request time stamp.
        /// </summary>
        public DateTime           RequestTimestamp     { get; }      = RequestTimestamp;

        /// <summary>
        /// The event tracking identification.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification or Any- or Multicast address of the message destination.
        /// </summary>
        public NetworkingNode_Id  DestinationId        { get; }      = DestinationId;

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id         RequestId            { get; }      = RequestId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; }      = Action;

        /// <summary>
        /// The JSON request message payload.
        /// </summary>
        public JObject            Payload              { get; }      = Payload;

        /// <summary>
        /// The request time out.
        /// </summary>
        public DateTime           RequestTimeout       { get; set; } = RequestTimeout ?? (RequestTimestamp + DefaultTimeout);

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?            ErrorMessage         { get; }      = ErrorMessage;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;


        /// <summary>
        /// The request message has no errors.
        /// </summary>
        public Boolean            NoErrors
            => ErrorMessage is null;

        /// <summary>
        /// The request message has errors.
        /// </summary>
        public Boolean            HasErrors
            => ErrorMessage is not null;

        #endregion


        #region (static) FromRequest(Request, SerializedRequest)

        /// <summary>
        /// Create a new OCPP JSON request message transport container based on the given JSON request.
        /// </summary>
        /// <param name="Request">A request message.</param>
        /// <param name="SerializedRequest">The serialized request message.</param>
        public static OCPP_JSONRequestMessage FromRequest(IRequest  Request,
                                                          JObject   SerializedRequest)

            => new (Timestamp.Now,
                    Request.EventTrackingId,
                    NetworkingMode.Unknown,
                    Request.DestinationId,
                    Request.NetworkPath,
                    Request.RequestId,
                    Request.Action,
                    SerializedRequest,
                    Timestamp.Now + Request.RequestTimeout,
                    null,
                    Request.CancellationToken);

        #endregion


        #region TryParse(JSONArray, out RequestMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given JSON representation of a request message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="RequestMessage">The parsed OCPP WebSocket request message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP Web Sockets connection.</param>
        public static Boolean TryParse(JArray                                             JSONArray,
                                       [NotNullWhen(true)]  out OCPP_JSONRequestMessage?  RequestMessage,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp       = null,
                                       DateTime?                                          RequestTimeout         = null,
                                       EventTracking_Id?                                  EventTrackingId        = null,
                                       NetworkingNode_Id?                                 ImplicitSourceNodeId   = null,
                                       CancellationToken                                  CancellationToken      = default)
        {

            RequestMessage  = null;
            ErrorResponse   = null;

            try
            {

                #region OCPP standard mode

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
                    {
                        ErrorResponse = $"Could not parse the given request identification: {JSONArray[1]}";
                        return false;
                    }

                    var action = JSONArray[2]?.Value<String>()?.Trim();
                    if (action is null || action.IsNullOrEmpty())
                    {
                        ErrorResponse = $"Could not parse the given action: {JSONArray[2]}";
                        return false;
                    }

                    if (JSONArray[3] is not JObject payload)
                    {
                        ErrorResponse = $"Could not parse the given payload: {JSONArray[3]}";
                        return false;
                    }

                    var networkPath = NetworkPath.Empty;

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {
                        networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                    }


                    RequestMessage = new OCPP_JSONRequestMessage(
                                         RequestTimestamp ?? Timestamp.Now,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         NetworkingMode.Standard,
                                         NetworkingNode_Id.Zero,
                                         networkPath,
                                         requestId,
                                         action,
                                         payload,
                                         RequestTimeout,
                                         null,
                                         CancellationToken
                                     );

                    return true;

                }

                #endregion

                #region OCPP Overlay Network mode

                // [
                //    2,                   // MessageType: CALL (Client-to-Server)
                //    DestinationId,
                //    [ NetworkPath ],
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

                    if (!NetworkingNode_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var destinationNodeId))
                    {
                        ErrorResponse = $"Could not parse the given destination node identification: {JSONArray[1]}";
                        return false;
                    }

                    if (JSONArray[2] is not JArray networkPathJSONArray ||
                        !NetworkPath.      TryParse(networkPathJSONArray, out var networkPath, out _) || networkPath is null)
                    {
                        ErrorResponse = $"Could not parse the given network path: {JSONArray[2]}";
                        return false;
                    }

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {

                        //if (networkPath.Length > 0 &&
                        //    networkPath.Last() != ImplicitSourceNodeId)
                        //{
                        //    networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                        //}

                        if (networkPath.Length == 0)
                            networkPath = networkPath.Append(ImplicitSourceNodeId.Value);

                    }


                    if (!Request_Id.       TryParse(JSONArray[3]?.Value<String>() ?? "", out var requestId))
                    {
                        ErrorResponse = $"Could not parse the given request identification: {JSONArray[3]}";
                        return false;
                    }

                    var action = JSONArray[4]?.Value<String>()?.Trim();
                    if (action is null || action.IsNullOrEmpty())
                    {
                        ErrorResponse = $"Could not parse the given action: {JSONArray[4]}";
                        return false;
                    }

                    if (JSONArray[5] is not JObject payload)
                    {
                        ErrorResponse = $"Could not parse the given payload: {JSONArray[5]}";
                        return false;
                    }

                    RequestMessage = new OCPP_JSONRequestMessage(
                                         Timestamp.Now,
                                         EventTracking_Id.New,
                                         NetworkingMode.OverlayNetwork,
                                         destinationNodeId,
                                         networkPath,
                                         requestId,
                                         action,
                                         payload,
                                         RequestTimeout,
                                         null,
                                         CancellationToken
                                     );

                    return true;

                }

                #endregion

            }
            catch (Exception e)
            {
                ErrorResponse = $"Could not parse the given JSON request message: {e.Message}";
                return false;
            }

            ErrorResponse = $"Could not parse the given JSON request message: {JSONArray}";
            return false;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            => NetworkingMode switch {

                   #region OCPP Standard Mode

                   // [
                   //     2,                   // MessageType: CALL (Client-to-Server)
                   //    "19223201",           // RequestId
                   //    "BootNotification",   // Action
                   //    {
                   //        "chargingStation":  { ... },
                   //        "reason":           "FirmwareUpdate"
                   //    }
                   // ]
                   NetworkingMode.Unknown or
                   NetworkingMode.Standard
                       => new (2,
                               RequestId.ToString(),
                               Action,
                               Payload),

                   #endregion

                   #region OCPP Overlay Network Mode

                   // [
                   //     2,                    // MessageType: CALL (Client-to-Server)
                   //     "CSMS",               // Destination Identification/Any-/Multicast
                   //     [ "CS01", "NN01" ],   // Network Source Path
                   //    "19223201",            // RequestId
                   //    "BootNotification",    // Action
                   //    {
                   //        "chargingStation":  { ... },
                   //        "reason":           "FirmwareUpdate"
                   //    }
                   // ]
                   _   => new (2,
                               DestinationId.ToString(),
                               NetworkPath.      ToJSON(),
                               RequestId.        ToString(),
                               Action,
                               Payload)

                   #endregion

               };

        #endregion


        #region ChangeNetworking(NewDestinationId = null, NewNetworkPath = null, NewNetworkingMode = null)

        /// <summary>
        /// Change the destination identification, network (source) path and networking mode.
        /// </summary>
        /// <param name="NewDestinationId">An optional new destination identification.</param>
        /// <param name="NewNetworkPath">An optional new (source) network path.</param>
        /// <param name="NewNetworkingMode">An optional new networking mode.</param>
        public OCPP_JSONRequestMessage ChangeNetworking(NetworkingNode_Id?  NewDestinationId    = null,
                                                        NetworkPath?        NewNetworkPath      = null,
                                                        NetworkingMode?     NewNetworkingMode   = null)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NewNetworkingMode    ?? NetworkingMode,
                    NewDestinationId ?? DestinationId,
                    NewNetworkPath       ?? NetworkPath,
                    RequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region ChangeDestionationId(NewDestinationId)

        /// <summary>
        /// Change the destination identification.
        /// </summary>
        /// <param name="NewDestinationId">A new destination identification.</param>
        public OCPP_JSONRequestMessage ChangeDestionationId(NetworkingNode_Id NewDestinationId)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    NewDestinationId,
                    NetworkPath,
                    RequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region AppendToNetworkPath(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network path.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification to append.</param>
        public OCPP_JSONRequestMessage AppendToNetworkPath(NetworkingNode_Id NetworkingNodeId)

            => new (RequestTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    DestinationId,
                    NetworkPath.Append(NetworkingNodeId),
                    RequestId,
                    Action,
                    Payload,
                    RequestTimeout,
                    ErrorMessage,
                    CancellationToken);

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
