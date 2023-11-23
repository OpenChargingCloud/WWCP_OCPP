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

    #region OnNotifyAllowedEnergyTransfer

    /// <summary>
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferRequestDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     NotifyAllowedEnergyTransferRequest   Request);


    /// <summary>
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyAllowedEnergyTransferResponse>

        OnNotifyAllowedEnergyTransferDelegate(DateTime                             Timestamp,
                                              IEventSender                         Sender,
                                              WebSocketClientConnection            Connection,
                                              NotifyAllowedEnergyTransferRequest   Request,
                                              CancellationToken                    CancellationToken);


    /// <summary>
    /// A NotifyAllowedEnergyTransfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferResponseDelegate(DateTime                              Timestamp,
                                                      IEventSender                          Sender,
                                                      NotifyAllowedEnergyTransferRequest    Request,
                                                      NotifyAllowedEnergyTransferResponse   Response,
                                                      TimeSpan                              Runtime);

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

        public CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                        OnNotifyAllowedEnergyTransferWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferDelegate?            OnNotifyAllowedEnergyTransfer;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?    OnNotifyAllowedEnergyTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                       OnNotifyAllowedEnergyTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyAllowedEnergyTransfer(DateTime                   RequestTimestamp,
                                                WebSocketClientConnection  WebSocketConnection,
                                                ChargingStation_Id         chargingStationId,
                                                EventTracking_Id           EventTrackingId,
                                                String                     requestText,
                                                Request_Id                 requestId,
                                                JObject                    requestJSON,
                                                CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnNotifyAllowedEnergyTransferWSRequest event

            try
            {

                OnNotifyAllowedEnergyTransferWSRequest?.Invoke(Timestamp.Now,
                                                               WebSocketConnection,
                                                               chargingStationId,
                                                               EventTrackingId,
                                                               requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferWSRequest));
            }

            #endregion

            try
            {

                if (NotifyAllowedEnergyTransferRequest.TryParse(requestJSON,
                                                                requestId,
                                                                ChargingStationIdentity,
                                                                out var request,
                                                                out var errorResponse,
                                                                CustomNotifyAllowedEnergyTransferRequestParser) && request is not null) {

                    #region Send OnNotifyAllowedEnergyTransferRequest event

                    try
                    {

                        OnNotifyAllowedEnergyTransferRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyAllowedEnergyTransferResponse? response = null;

                    var results = OnNotifyAllowedEnergyTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnNotifyAllowedEnergyTransferDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= NotifyAllowedEnergyTransferResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyAllowedEnergyTransferResponse event

                    try
                    {

                        OnNotifyAllowedEnergyTransferResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      request,
                                                                      response,
                                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "NotifyAllowedEnergyTransfer",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "NotifyAllowedEnergyTransfer",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnNotifyAllowedEnergyTransferWSResponse event

            try
            {

                OnNotifyAllowedEnergyTransferWSResponse?.Invoke(Timestamp.Now,
                                                                WebSocketConnection,
                                                                requestJSON,
                                                                OCPPResponse?.Message,
                                                                OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
