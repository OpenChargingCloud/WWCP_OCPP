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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
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

        public CustomJObjectParserDelegate<NotifyCRLRequest>?       CustomNotifyCRLRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyCRL websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnNotifyCRLWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLRequestReceivedDelegate?        OnNotifyCRLRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLDelegate?               OnNotifyCRL;

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLResponseSentDelegate?       OnNotifyCRLResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a NotifyCRL request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnNotifyCRLWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_NotifyCRL(DateTime              RequestTimestamp,
                              IWebSocketConnection  WebSocketConnection,
                              NetworkingNode_Id     DestinationId,
                              NetworkPath           NetworkPath,
                              EventTracking_Id      EventTrackingId,
                              Request_Id            RequestId,
                              JObject               RequestJSON,
                              CancellationToken     CancellationToken)

        {

            #region Send OnNotifyCRLWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCRLWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyCRLWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyCRLRequest.TryParse(RequestJSON,
                                              RequestId,
                                              DestinationId,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              CustomNotifyCRLRequestParser)) {

                    #region Send OnNotifyCRLRequest event

                    try
                    {

                        OnNotifyCRLRequestReceived?.Invoke(Timestamp.Now,
                                                   parentNetworkingNode,
                                                   WebSocketConnection,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyCRLRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyCRLResponse? response = null;

                    var results = OnNotifyCRL?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnNotifyCRLDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= NotifyCRLResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyCRLResponse event

                    try
                    {

                        OnNotifyCRLResponseSent?.Invoke(Timestamp.Now,
                                                    parentNetworkingNode,
                                                    WebSocketConnection,
                                                    request,
                                                    response,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyCRLResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomNotifyCRLResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_NotifyCRL)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_NotifyCRL)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnNotifyCRLWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnNotifyCRLWSResponse?.Invoke(endTime,
                                              parentNetworkingNode,
                                              WebSocketConnection,
                                              DestinationId,
                                              NetworkPath,
                                              EventTrackingId,
                                              RequestTimestamp,
                                              RequestJSON,
                                              OCPPResponse?.Payload,
                                              OCPPErrorResponse?.ToJSON(),
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyCRLWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLResponseSentDelegate? OnNotifyCRLResponseSent;

    }

}
