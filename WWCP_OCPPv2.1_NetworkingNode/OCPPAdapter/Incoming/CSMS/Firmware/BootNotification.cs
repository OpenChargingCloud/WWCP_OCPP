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
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<BootNotificationRequest>?       CustomBootNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<BootNotificationResponse>?  CustomBootNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BootNotification WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                      OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationRequestReceivedDelegate?     OnBootNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a BootNotification was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationDelegate?            OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a BootNotification was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationResponseSentDelegate?    OnBootNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a BootNotification was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?          OnBootNotificationWSResponse;

        #endregion


        ///// <summary>
        ///// An event fired whenever a response to a boot notification request was received.
        ///// </summary>
        //public event OCPPv2_1.CS.OnBootNotificationResponseReceivedDelegate?    OnBootNotificationResponseReceived;

        //public async Task RaiseOnBootNotificationResponseIN(DateTime                   Timestamp,
        //                                                    IEventSender               Sender,
        //                                                    BootNotificationRequest    Request,
        //                                                    BootNotificationResponse   Response,
        //                                                    TimeSpan                   Runtime)
        //{

        //    var requestLogger = OnBootNotificationResponseReceived;
        //    if (requestLogger is not null)
        //    {

        //        try
        //        {
        //            await Task.WhenAll(
        //                      requestLogger.GetInvocationList().
        //                                    OfType <OCPPv2_1.CS.OnBootNotificationResponseReceivedDelegate>().
        //                                    Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
        //                                                                                      Sender,
        //                                                                                      Request,
        //                                                                                      Response,
        //                                                                                      Runtime)).
        //                                    ToArray()
        //                  );
        //        }
        //        catch (Exception e)
        //        {
        //            await parentNetworkingNode.HandleErrors(
        //                      nameof(OCPPWebSocketAdapterIN),
        //                      nameof(OnBootNotificationResponseReceived),
        //                      e
        //                  );
        //        }

        //    }

        //}



        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_BootNotification(DateTime              RequestTimestamp,
                                     IWebSocketConnection  WebSocketConnection,
                                     NetworkingNode_Id     DestinationNodeId,
                                     NetworkPath           NetworkPath,
                                     EventTracking_Id      EventTrackingId,
                                     Request_Id            RequestId,
                                     JObject               JSONRequest,
                                     CancellationToken     CancellationToken)

        {

            #region Send OnBootNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBootNotificationWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBootNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (BootNotificationRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     DestinationNodeId,
                                                     NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     CustomBootNotificationRequestParser,
                                                     parentNetworkingNode.OCPP.CustomChargingStationParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Send OnBootNotificationRequest event

                    try
                    {

                        OnBootNotificationRequestReceived?.Invoke(Timestamp.Now,
                                                          parentNetworkingNode,
                                                          WebSocketConnection,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBootNotificationRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    BootNotificationResponse? response = null;

                    var responseTasks = OnBootNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= BootNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnBootNotificationResponse event

                    try
                    {

                        OnBootNotificationResponseSent?.Invoke(Timestamp.Now,
                                                           parentNetworkingNode,
                                                           WebSocketConnection,
                                                           request,
                                                           response,
                                                           response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBootNotificationResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomBootNotificationResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_BootNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_BootNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnBootNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnBootNotificationWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBootNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a BootNotification was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationResponseSentDelegate? OnBootNotificationResponseSent;

    }

}
