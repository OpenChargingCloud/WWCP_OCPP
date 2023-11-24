﻿/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnSendLocalList

    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSendLocalListRequestDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       SendLocalListRequest   Request);


    /// <summary>
    /// A send local list request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendLocalListResponse>

        OnSendLocalListDelegate(DateTime                    Timestamp,
                                IEventSender                Sender,
                                WebSocketClientConnection   Connection,
                                SendLocalListRequest        Request,
                                CancellationToken           CancellationToken);


    /// <summary>
    /// A send local list response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSendLocalListResponseDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        SendLocalListRequest    Request,
                                        SendLocalListResponse   Response,
                                        TimeSpan                Runtime);

    #endregion


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SendLocalListRequest>?       CustomSendLocalListRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListResponse>?  CustomSendLocalListResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate?            OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?    OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnSendLocalListWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SendLocalList(DateTime                   RequestTimestamp,
                                  WebSocketClientConnection  WebSocketConnection,
                                  ChargingStation_Id         chargingStationId,
                                  EventTracking_Id           EventTrackingId,
                                  String                     requestText,
                                  Request_Id                 requestId,
                                  JObject                    requestJSON,
                                  CancellationToken          CancellationToken)

        {

            #region Send OnSendLocalListWSRequest event

            try
            {

                OnSendLocalListWSRequest?.Invoke(Timestamp.Now,
                                                 WebSocketConnection,
                                                 chargingStationId,
                                                 EventTrackingId,
                                                 requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (SendLocalListRequest.TryParse(requestJSON,
                                                  requestId,
                                                  ChargingStationIdentity,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomSendLocalListRequestParser) && request is not null) {

                    #region Send OnSendLocalListRequest event

                    try
                    {

                        OnSendLocalListRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SendLocalListResponse? response = null;

                    var results = OnSendLocalList?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSendLocalListDelegate)?.Invoke(Timestamp.Now,
                                                                                                               this,
                                                                                                               WebSocketConnection,
                                                                                                               request,
                                                                                                               CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= SendLocalListResponse.Failed(request);

                    #endregion

                    #region Send OnSendLocalListResponse event

                    try
                    {

                        OnSendLocalListResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        request,
                                                        response,
                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomSendLocalListResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_SendLocalList)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_SendLocalList)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnSendLocalListWSResponse event

            try
            {

                OnSendLocalListWSResponse?.Invoke(Timestamp.Now,
                                                  WebSocketConnection,
                                                  requestJSON,
                                                  OCPPResponse?.Payload,
                                                  OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
