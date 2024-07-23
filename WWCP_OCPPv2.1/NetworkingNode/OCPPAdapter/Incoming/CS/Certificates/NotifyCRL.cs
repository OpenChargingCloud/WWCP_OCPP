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
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLRequestReceivedDelegate?  OnNotifyCRLRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received for processing.
        /// </summary>
        public event OnNotifyCRLDelegate?                 OnNotifyCRL;

        #endregion

        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_NotifyCRL(DateTime              RequestTimestamp,
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

                if (NotifyCRLRequest.TryParse(JSONRequest,
                                              RequestId,
                                              DestinationId,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              RequestTimestamp,
                                              parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                              EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomNotifyCRLRequestParser)) {

                    NotifyCRLResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyCRLRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = NotifyCRLResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyCRLRequestReceived event

                    var logger = OnNotifyCRLRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnNotifyCRLRequestReceivedDelegate>().
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
                                      nameof(OnNotifyCRLRequestReceived),
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

                            var responseTasks = OnNotifyCRL?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnNotifyCRLDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : NotifyCRLResponse.Failed(request, $"Undefined {nameof(OnNotifyCRL)}!");

                        }
                        catch (Exception e)
                        {

                            response = NotifyCRLResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnNotifyCRL),
                                      e
                                  );

                        }
                    }

                    response ??= NotifyCRLResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyCRLResponseSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyCRLResponse event

                    await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterOUT).SendOnNotifyCRLResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomNotifyCRLResponseSerializer,
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
                                       nameof(Receive_NotifyCRL)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyCRL)[8..],
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
        /// An event sent whenever a response to a NotifyCRL was sent.
        /// </summary>
        public event OnNotifyCRLResponseSentDelegate?  OnNotifyCRLResponseSent;

        #endregion

        #region Send OnNotifyCRLResponse event

        public async Task SendOnNotifyCRLResponseSent(DateTime              Timestamp,
                                                      IEventSender          Sender,
                                                      IWebSocketConnection  Connection,
                                                      NotifyCRLRequest      Request,
                                                      NotifyCRLResponse     Response,
                                                      TimeSpan              Runtime)
        {

            var logger = OnNotifyCRLResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnNotifyCRLResponseSentDelegate>().
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
                              nameof(OnNotifyCRLResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
