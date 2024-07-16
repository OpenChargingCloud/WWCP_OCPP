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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyNetworkTopologyRequest>?       CustomNotifyNetworkTopologyRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                    OnNotifyNetworkTopologyWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyRequestReceivedDelegate?    OnNotifyNetworkTopologyRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyDelegate?                   OnNotifyNetworkTopology;

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseSentDelegate?       OnNotifyNetworkTopologyResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?        OnNotifyNetworkTopologyWSResponse;

        #endregion

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseReceivedDelegate?   OnNotifyNetworkTopologyResponseReceived;


        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_NotifyNetworkTopology(DateTime              RequestTimestamp,
                                          IWebSocketConnection  WebSocketConnection,
                                          NetworkingNode_Id     DestinationId,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          JObject               RequestJSON,
                                          CancellationToken     CancellationToken)

        {

            #region Send OnNotifyNetworkTopologyWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyWSRequest?.Invoke(startTime,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         DestinationId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestTimestamp,
                                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyNetworkTopologyWSRequest));
            }

            OCPP_Response? ocppResponse   = null;

            #endregion

            try
            {

                if (NotifyNetworkTopologyRequest.TryParse(RequestJSON,
                                                          RequestId,
                                                          DestinationId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomNotifyNetworkTopologyRequestParser)) {

                    #region Send OnNotifyNetworkTopologyRequestReceived event

                    try
                    {

                        OnNotifyNetworkTopologyRequestReceived?.Invoke(Timestamp.Now,
                                                                       parentNetworkingNode,
                                                                       WebSocketConnection,
                                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyNetworkTopologyRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyNetworkTopologyResponse? response = null;

                    var results = OnNotifyNetworkTopology?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnNotifyNetworkTopologyDelegate)?.Invoke(Timestamp.Now,
                                                                                                                       parentNetworkingNode,
                                                                                                                       WebSocketConnection,
                                                                                                                       request,
                                                                                                                       CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= NotifyNetworkTopologyResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyNetworkTopologyResponseSent event

                    try
                    {

                        OnNotifyNetworkTopologyResponseSent?.Invoke(Timestamp.Now,
                                                                    parentNetworkingNode,
                                                                    WebSocketConnection,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyNetworkTopologyResponseSent));
                    }

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomNotifyNetworkTopologyResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_NotifyNetworkTopology)[8..],
                                       RequestJSON,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {
                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyNetworkTopology)[8..],
                                   RequestJSON,
                                   e
                               );
            }

            #region Send OnNotifyNetworkTopologyWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnNotifyNetworkTopologyWSResponse?.Invoke(endTime,
                                                          parentNetworkingNode,
                                                          WebSocketConnection,
                                                          DestinationId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          RequestJSON,
                                                          ocppResponse.JSONResponseMessage?.Payload,
                                                          ocppResponse.JSONRequestErrorMessage?.   ToJSON(),
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyNetworkTopologyWSResponse));
            }

            #endregion

            return ocppResponse;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseSentDelegate? OnNotifyNetworkTopologyResponseSent;

    }

}
