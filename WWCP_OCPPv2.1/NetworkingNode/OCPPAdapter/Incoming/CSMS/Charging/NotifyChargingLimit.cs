﻿/*
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyChargingLimitRequest>?       CustomNotifyChargingLimitRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                         OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitRequestReceivedDelegate?     OnNotifyChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitDelegate?            OnNotifyChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitResponseSentDelegate?    OnNotifyChargingLimitResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyChargingLimit was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?             OnNotifyChargingLimitWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_NotifyChargingLimit(DateTime                   RequestTimestamp,
                                        IWebSocketConnection  WebSocketConnection,
                                        NetworkingNode_Id          DestinationNodeId,
                                        NetworkPath                NetworkPath,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    JSONRequest,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnNotifyChargingLimitWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitWSRequest?.Invoke(startTime,
                                                       parentNetworkingNode,
                                                       WebSocketConnection,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       EventTrackingId,
                                                       RequestTimestamp,
                                                       JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyChargingLimitWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyChargingLimitRequest.TryParse(JSONRequest,
                                                        RequestId,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomNotifyChargingLimitRequestParser)) {

                    #region Send OnNotifyChargingLimitRequest event

                    try
                    {

                        OnNotifyChargingLimitRequestReceived?.Invoke(Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyChargingLimitRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyChargingLimitResponse? response = null;

                    var responseTasks = OnNotifyChargingLimit?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyChargingLimitDelegate)?.Invoke(Timestamp.Now,
                                                                                                                           parentNetworkingNode,
                                                                                                                           WebSocketConnection,
                                                                                                                           request,
                                                                                                                           CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyChargingLimitResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyChargingLimitResponse event

                    try
                    {

                        OnNotifyChargingLimitResponseSent?.Invoke(Timestamp.Now,
                                                              parentNetworkingNode,
                                                              WebSocketConnection,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyChargingLimitResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomNotifyChargingLimitResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_NotifyChargingLimit)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_NotifyChargingLimit)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnNotifyChargingLimitWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnNotifyChargingLimitWSResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        WebSocketConnection,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        EventTrackingId,
                                                        RequestTimestamp,
                                                        JSONRequest,
                                                        OCPPResponse?.Payload,
                                                        OCPPErrorResponse?.ToJSON(),
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyChargingLimitWSResponse));
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
        /// An event sent whenever a response to a NotifyChargingLimit was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitResponseSentDelegate? OnNotifyChargingLimitResponseSent;

    }

}
