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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestReceivedDelegate?  OnClearVariableMonitoringRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received for processing.
        /// </summary>
        public event OnClearVariableMonitoringDelegate?                 OnClearVariableMonitoring;

        #endregion

        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_ClearVariableMonitoring(DateTime              RequestTimestamp,
                                            IWebSocketConnection  WebSocketConnection,
                                            NetworkingNode_Id     DestinationId,
                                            NetworkPath           NetworkPath,
                                            EventTracking_Id      EventTrackingId,
                                            Request_Id            RequestId,
                                            JObject               JSONRequest,
                                            CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (ClearVariableMonitoringRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                            DestinationId,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestParser)) {

                    ClearVariableMonitoringResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = ClearVariableMonitoringResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnClearVariableMonitoringRequestReceived event

                    var logger = OnClearVariableMonitoringRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnClearVariableMonitoringRequestReceivedDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request
                                                                             )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnClearVariableMonitoringRequestReceived),
                                      e
                                  );
                        }
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnClearVariableMonitoring?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnClearVariableMonitoringDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : ClearVariableMonitoringResponse.Failed(request, $"Undefined {nameof(OnClearVariableMonitoring)}!");

                        }
                        catch (Exception e)
                        {

                            response = ClearVariableMonitoringResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnClearVariableMonitoring),
                                      e
                                  );

                        }
                    }

                    response ??= ClearVariableMonitoringResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                            parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearVariableMonitoringResponse event

                    await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterOUT).SendOnClearVariableMonitoringResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
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
                                       nameof(Receive_ClearVariableMonitoring)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_ClearVariableMonitoring)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a ClearVariableMonitoring was sent.
        /// </summary>
        public event OnClearVariableMonitoringResponseSentDelegate?  OnClearVariableMonitoringResponseSent;

        #endregion

        #region Send OnClearVariableMonitoringResponse event

        public async Task SendOnClearVariableMonitoringResponseSent(DateTime                         Timestamp,
                                                                    IEventSender                     Sender,
                                                                    IWebSocketConnection             Connection,
                                                                    ClearVariableMonitoringRequest   Request,
                                                                    ClearVariableMonitoringResponse  Response,
                                                                    TimeSpan                         Runtime)
        {

            var logger = OnClearVariableMonitoringResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnClearVariableMonitoringResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnClearVariableMonitoringResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
