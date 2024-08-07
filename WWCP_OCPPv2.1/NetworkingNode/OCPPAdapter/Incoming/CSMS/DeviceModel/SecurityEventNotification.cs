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

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestReceivedDelegate?  OnSecurityEventNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received for processing.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?                 OnSecurityEventNotification;

        #endregion

        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_SecurityEventNotification(DateTime              RequestTimestamp,
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

                if (SecurityEventNotificationRequest.TryParse(JSONRequest,
                                                              RequestId,
                                                              DestinationId,
                                                              NetworkPath,
                                                              out var request,
                                                              out var errorResponse,
                                                              RequestTimestamp,
                                                              parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                              EventTrackingId,
                                                              parentNetworkingNode.OCPP.CustomSecurityEventNotificationRequestParser)) {

                    SecurityEventNotificationResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSecurityEventNotificationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SecurityEventNotificationResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSecurityEventNotificationRequestReceived event

                    var logger = OnSecurityEventNotificationRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnSecurityEventNotificationRequestReceivedDelegate>().
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
                                      nameof(OnSecurityEventNotificationRequestReceived),
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

                            var responseTasks = OnSecurityEventNotification?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnSecurityEventNotificationDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : SecurityEventNotificationResponse.Failed(request, $"Undefined {nameof(OnSecurityEventNotification)}!");

                        }
                        catch (Exception e)
                        {

                            response = SecurityEventNotificationResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnSecurityEventNotification),
                                      e
                                  );

                        }
                    }

                    response ??= SecurityEventNotificationResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSecurityEventNotificationResponseSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSecurityEventNotificationResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnSecurityEventNotificationResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomSecurityEventNotificationResponseSerializer,
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
                                       nameof(Receive_SecurityEventNotification)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SecurityEventNotification)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseSentDelegate?  OnSecurityEventNotificationResponseSent;

        #endregion

        #region Send OnSecurityEventNotificationResponse event

        public async Task SendOnSecurityEventNotificationResponseSent(DateTime                           Timestamp,
                                                                      IEventSender                       Sender,
                                                                      IWebSocketConnection               Connection,
                                                                      SecurityEventNotificationRequest   Request,
                                                                      SecurityEventNotificationResponse  Response,
                                                                      TimeSpan                           Runtime)
        {

            var logger = OnSecurityEventNotificationResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnSecurityEventNotificationResponseSentDelegate>().
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
                              nameof(OnSecurityEventNotificationResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
