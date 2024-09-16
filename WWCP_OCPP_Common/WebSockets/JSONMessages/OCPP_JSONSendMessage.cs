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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP JSON send message transport container.
    /// </summary>
    /// <param name="MessageTimestamp">The message sent time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the message through the overlay network.</param>
    /// <param name="MessageId">An unique message identification.</param>
    /// <param name="Action">An OCPP action/method name.</param>
    /// <param name="Payload">A JSON request message payload.</param>
    /// <param name="ErrorMessage">An optional error message, e.g. during sending of the message.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_JSONSendMessage(DateTime           MessageTimestamp,
                                      EventTracking_Id   EventTrackingId,
                                      NetworkingMode     NetworkingMode,
                                      SourceRouting      Destination,
                                      NetworkPath        NetworkPath,
                                      Request_Id         MessageId,
                                      String             Action,
                                      JObject            Payload,
                                      String?            ErrorMessage        = null,
                                      CancellationToken  CancellationToken   = default)
    {

        #region Properties

        /// <summary>
        /// The message sent time stamp.
        /// </summary>
        public DateTime           MessageTimestamp     { get; }      = MessageTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this message with other events.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification or Any- or Multicast address of the message destination.
        /// </summary>
        public SourceRouting      Destination          { get; }      = Destination;

        /// <summary>
        /// The (recorded) path of the message through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique message identification.
        /// </summary>
        public Request_Id         MessageId            { get; }      = MessageId;

        /// <summary>
        /// An OCPP action/method name.
        /// </summary>
        public String             Action               { get; }      = Action;

        /// <summary>
        /// The JSON send message payload.
        /// </summary>
        public JObject            Payload              { get; }      = Payload;

        /// <summary>
        /// The optional error message, e.g. during sending of the message.
        /// </summary>
        public String?            ErrorMessage         { get; }      = ErrorMessage;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;


        /// <summary>
        /// The message has no errors.
        /// </summary>
        public Boolean            NoErrors
            => ErrorMessage is null;

        /// <summary>
        /// The message has errors.
        /// </summary>
        public Boolean            HasErrors
            => ErrorMessage is not null;

        #endregion


        #region (static) FromDatagram(Message, SerializedMessage)

        /// <summary>
        /// Create a new OCPP JSONSendMessage transport container based on the given JSON message.
        /// </summary>
        /// <param name="Message">A send message.</param>
        /// <param name="SerializedMessage">The serialized send message.</param>
        public static OCPP_JSONSendMessage FromDatagram(IMessage  Message,
                                                        JObject   SerializedMessage)

            => new (Timestamp.Now,
                    Message.EventTrackingId,
                    NetworkingMode.Unknown,
                    Message.Destination,
                    Message.NetworkPath,
                    Message.RequestId,
                    Message.Action,
                    SerializedMessage,
                    null,
                    Message.CancellationToken);

        #endregion


        #region TryParse(JSONArray, out SendMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given JSON representation of a send message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="SendMessage">The parsed OCPP WebSocket send message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        public static Boolean TryParse(JArray                                          JSONArray,
                                       [NotNullWhen(true)]  out OCPP_JSONSendMessage?  SendMessage,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTime?                                       MessageTimestamp       = null,
                                       EventTracking_Id?                               EventTrackingId        = null,
                                       NetworkingNode_Id?                              ImplicitSourceNodeId   = null,
                                       CancellationToken                               CancellationToken      = default)
        {

            SendMessage  = null;
            ErrorResponse   = null;

            try
            {

                #region OCPP standard mode

                // [
                //     6,                           // MessageType: SEND
                //    "19223201",                   // MessageId
                //    "NotifyPeriodicEventStream",  // Action
                //    {
                //        "id":       123,
                //        "pending":  0,
                //        "data":     [ {"t": 1710863410, "v": 230.4}, {"t": 1710863420, "v": 230.2} ]
                //    }
                // ]

                if (JSONArray.Count            == 4                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 6                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.String   &&
                    JSONArray[3].Type          == JTokenType.Object)
                {

                    if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var messageId))
                    {
                        ErrorResponse = $"Could not parse the given message identification: {JSONArray[1]}";
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


                    SendMessage = new OCPP_JSONSendMessage(
                                         MessageTimestamp ?? Timestamp.Now,
                                         EventTrackingId  ?? EventTracking_Id.New,
                                         NetworkingMode.Standard,
                                         SourceRouting.Zero,
                                         networkPath,
                                         messageId,
                                         action,
                                         payload,
                                         null,
                                         CancellationToken
                                     );

                    return true;

                }

                #endregion

                #region OCPP Overlay Network mode

                // [
                //     6,                           // MessageType: SEND
                //    "CSMS" or [ "NN", "CSMS" ],   // Destination/Anycast/Multicast or Source Routing
                //    "CS"   or [ "CS", "NN" ],     // Source or Network Path
                //    "19223201",                   // RequestId
                //    "NotifyPeriodicEventStream",  // Action
                //    {
                //        "id":       123,
                //        "pending":  0,
                //        "data":     [ {"t": 1710863410, "v": 230.4}, {"t": 1710863420, "v": 230.2} ]
                //    }
                // ]

                if (JSONArray.Count            == 6                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 6                   &&
                   (JSONArray[1].Type          == JTokenType.String || JSONArray[1].Type == JTokenType.Array) &&
                   (JSONArray[2].Type          == JTokenType.String || JSONArray[2].Type == JTokenType.Array) &&
                    JSONArray[3].Type          == JTokenType.String   &&
                    JSONArray[4].Type          == JTokenType.String   &&
                    JSONArray[5].Type          == JTokenType.Object)
                {

                    #region Parse Source Routing

                    SourceRouting? sourceRouting = null;

                    if (JSONArray[1].Type == JTokenType.String)
                    {

                        if (NetworkingNode_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var destinationNode))
                            sourceRouting = SourceRouting.To(destinationNode);

                        else
                        {
                            ErrorResponse = $"Could not parse the given destination node: '{JSONArray[1]?.Value<String>() ?? ""}'!";
                            return false;
                        }

                    }

                    else if (JSONArray[1] is JArray hops)
                    {

                        if (hops.Count == 0)
                        {
                            ErrorResponse = $"The source routing must not be empty!";
                            return false;
                        }

                        var networkingNodeIds = new List<NetworkingNode_Id>();

                        foreach (var hop in hops)
                        {

                            if (NetworkingNode_Id.TryParse(hop?.Value<String>() ?? "", out var networkingNodeId))
                                networkingNodeIds.Add(networkingNodeId);

                            else
                            {
                                ErrorResponse = $"Could not parse the given networking node: '{hop?.Value<String>()}'!";
                                return false;
                            }

                        }

                        sourceRouting = SourceRouting.To(networkingNodeIds);

                    }

                    else
                    {
                        ErrorResponse = $"The source routing is invalid: '{JSONArray[1]?.Value<String>() ?? ""}'!";
                        return false;
                    }

                    #endregion

                    #region Parse Network Path

                    NetworkPath? networkPath = null;

                    if (JSONArray[2].Type == JTokenType.String)
                    {

                        if (NetworkingNode_Id.TryParse(JSONArray[2]?.Value<String>() ?? "", out var sourceNode))
                            networkPath = NetworkPath.From(sourceNode);

                        else
                        {
                            ErrorResponse = $"Could not parse the given source node: '{JSONArray[2]?.Value<String>() ?? ""}'!";
                            return false;
                        }

                    }

                    else if (JSONArray[2] is JArray hops)
                    {

                        if (hops.Count == 0)
                        {
                            ErrorResponse = $"The network path must not be empty!";
                            return false;
                        }

                        var networkingNodeIds = new List<NetworkingNode_Id>();

                        foreach (var hop in hops)
                        {

                            if (NetworkingNode_Id.TryParse(hop?.Value<String>() ?? "", out var networkingNodeId))
                                networkingNodeIds.Add(networkingNodeId);

                            else
                            {
                                ErrorResponse = $"Could not parse the given networking node identification: '{hop?.Value<String>()}'!";
                                return false;
                            }

                        }

                        networkPath = NetworkPath.From(networkingNodeIds);

                    }

                    else
                    {
                        ErrorResponse = $"The network path is invalid: '{JSONArray[2]?.Value<String>() ?? ""}'!";
                        return false;
                    }

                    #endregion

                    //if (ImplicitSourceNodeId.HasValue &&
                    //    ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    //{

                    //    //if (networkPath.Length > 0 &&
                    //    //    networkPath.Last() != ImplicitSourceNodeId)
                    //    //{
                    //    //    networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                    //    //}

                    //    if (networkPath.Length == 0)
                    //        networkPath = networkPath.Append(ImplicitSourceNodeId.Value);

                    //}

                    if (!Request_Id.TryParse(JSONArray[3]?.Value<String>() ?? "", out var messageId))
                    {
                        ErrorResponse = $"Could not parse the given message identification: {JSONArray[3]}";
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

                    SendMessage = new OCPP_JSONSendMessage(
                                      Timestamp.Now,
                                      EventTracking_Id.New,
                                      NetworkingMode.OverlayNetwork,
                                      sourceRouting,
                                      networkPath,
                                      messageId,
                                      action,
                                      payload,
                                      null,
                                      CancellationToken
                                  );

                    return true;

                }

                #endregion

            }
            catch (Exception e)
            {
                ErrorResponse = $"Could not parse the given JSON send message: {e.Message}";
                return false;
            }

            ErrorResponse = $"Could not parse the given JSON send message: {JSONArray}";
            return false;

        }

        #endregion

        #region ToJSON(ForcedNetworkingMode = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given </param>
        public JArray ToJSON(NetworkingMode? ForcedNetworkingMode = null)

            => (ForcedNetworkingMode ?? NetworkingMode) switch {

                   #region OCPP Standard Mode

                   // [
                   //     6,                           // MessageType: SEND
                   //    "19223201",                   // MessageId
                   //    "NotifyPeriodicEventStream",  // Action
                   //    {
                   //        "id":       123,
                   //        "pending":  0,
                   //        "data":     [ {"t": 1710863410, "v": 230.4}, {"t": 1710863420, "v": 230.2} ]
                   //    }
                   // ]
                   NetworkingMode.Unknown or
                   NetworkingMode.Standard
                       => new (6,
                               MessageId.ToString(),
                               Action,
                               Payload),

                   #endregion

                   #region OCPP Overlay Network Mode

                   // [
                   //     6,                           // MessageType: SEND
                   //    "CSMS" or [ "NN", "CSMS" ],   // Destination/Anycast/Multicast or Source Routing
                   //    "CS"   or [ "CS", "NN" ],     // Source or Network Path
                   //    "19223201",                   // RequestId
                   //    "NotifyPeriodicEventStream",  // Action
                   //    {
                   //        "id":       123,
                   //        "pending":  0,
                   //        "data":     [ {"t": 1710863410, "v": 230.4}, {"t": 1710863420, "v": 230.2} ]
                   //    }
                   // ]
                   _   => new (6,
                               Destination.Length == 1
                                   ? Destination.First().ToString()
                                   : new JArray(Destination.Select(hop => hop.ToString())),
                               NetworkPath.Length == 1
                                   ? NetworkPath.First().ToString()
                                   : new JArray(NetworkPath.Select(hop => hop.ToString())),
                               MessageId.ToString(),
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
        public OCPP_JSONSendMessage ChangeNetworking(SourceRouting?   NewDestination      = null,
                                                     NetworkPath?     NewNetworkPath      = null,
                                                     NetworkingMode?  NewNetworkingMode   = null)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NewNetworkingMode ?? NetworkingMode,
                    NewDestination    ?? Destination,
                    NewNetworkPath    ?? NetworkPath,
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region ChangeDestionation(NewDestination)

        /// <summary>
        /// Change the destination identification.
        /// </summary>
        /// <param name="NewDestinationId">A new destination identification.</param>
        public OCPP_JSONSendMessage ChangeDestionation(SourceRouting NewDestination)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    NewDestination,
                    NetworkPath,
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion

        #region AppendToNetworkPath(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network path.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification to append.</param>
        public OCPP_JSONSendMessage AppendToNetworkPath(NetworkingNode_Id NetworkingNodeId)

            => new (MessageTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath.Append(NetworkingNodeId),
                    MessageId,
                    Action,
                    Payload,
                    ErrorMessage,
                    CancellationToken);

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"SEND: {Action} ({MessageId}) => {Payload.ToString(Newtonsoft.Json.Formatting.None)}";

        #endregion


    }

}
