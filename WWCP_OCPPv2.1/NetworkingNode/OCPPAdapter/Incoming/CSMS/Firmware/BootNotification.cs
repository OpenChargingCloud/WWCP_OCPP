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

    /// <summary>
    /// The OCPP HTTP Web Socket Adapter for incoming requests.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationRequestReceivedDelegate?  OnBootNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a BootNotification request was received for processing.
        /// </summary>
        public event OnBootNotificationDelegate?                 OnBootNotification;

        #endregion

        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_BootNotification(DateTime              RequestTimestamp,
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

                if (BootNotificationRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     DestinationId,
                                                     NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     parentNetworkingNode.OCPP.CustomBootNotificationRequestParser,
                                                     parentNetworkingNode.OCPP.CustomChargingStationParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    BootNotificationResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = BootNotificationResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnBootNotificationRequest event

                    var logger = OnBootNotificationRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnBootNotificationRequestReceivedDelegate>().
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
                                  nameof(OnBootNotificationRequestReceived),
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

                            var responseTasks = OnBootNotification?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                parentNetworkingNode,
                                                                                                                                WebSocketConnection,
                                                                                                                                request,
                                                                                                                                CancellationToken)).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : BootNotificationResponse.Failed(request, $"Undefined {nameof(OnBootNotification)}!");

                        }
                        catch (Exception e)
                        {

                            response = BootNotificationResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnBootNotification),
                                      e
                                  );

                        }
                    }

                    response ??= BootNotificationResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnBootNotificationResponse event

                    await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterOUT).SendOnBootNotificationResponseSent(Timestamp.Now,
                                                                                                                        parentNetworkingNode,
                                                                                                                        WebSocketConnection,
                                                                                                                        request,
                                                                                                                        response,
                                                                                                                        response.Runtime);

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
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
                                       nameof(Receive_BootNotification)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_BootNotification)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP HTTP Web Socket Adapter for outgoing requests.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a BootNotification was sent.
        /// </summary>
        public event OnBootNotificationResponseSentDelegate?  OnBootNotificationResponseSent;

        #endregion

        #region Send OnBootNotificationResponse event

        public async Task SendOnBootNotificationResponseSent(DateTime                  Timestamp,
                                                             IEventSender              Sender,
                                                             IWebSocketConnection      Connection,
                                                             BootNotificationRequest   Request,
                                                             BootNotificationResponse  Response,
                                                             TimeSpan                  Runtime)
        {

            var logger = OnBootNotificationResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType <OnBootNotificationResponseSentDelegate>().
                                              Select (filterDelegate => filterDelegate.Invoke(Timestamp,
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
                              nameof(OnBootNotificationResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
