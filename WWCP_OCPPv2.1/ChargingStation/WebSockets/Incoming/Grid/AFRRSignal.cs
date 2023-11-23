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

    #region OnAFRRSignal

    /// <summary>
    /// An AFRR signal request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnAFRRSignalRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    AFRRSignalRequest   Request);


    /// <summary>
    /// An AFRR signal request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AFRRSignalResponse>

        OnAFRRSignalDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             AFRRSignalRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// An AFRR signal response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnAFRRSignalResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     AFRRSignalRequest    Request,
                                     AFRRSignalResponse   Response,
                                     TimeSpan             Runtime);

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

        public CustomJObjectParserDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an AFRRSignal websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnAFRRSignalWSRequest;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?     OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalDelegate?            OnAFRRSignal;

        /// <summary>
        /// An event sent whenever a response to an AFRRSignal request was sent.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?    OnAFRRSignalResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an AFRRSignal request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnAFRRSignalWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_AFRRSignal(DateTime                   RequestTimestamp,
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

            #region Send OnAFRRSignalWSRequest event

            try
            {

                OnAFRRSignalWSRequest?.Invoke(Timestamp.Now,
                                              WebSocketConnection,
                                              chargingStationId,
                                              EventTrackingId,
                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalWSRequest));
            }

            #endregion

            try
            {

                if (AFRRSignalRequest.TryParse(requestJSON,
                                               requestId,
                                               ChargingStationIdentity,
                                               out var request,
                                               out var errorResponse,
                                               CustomAFRRSignalRequestParser) && request is not null) {

                    #region Send OnAFRRSignalRequest event

                    try
                    {

                        OnAFRRSignalRequest?.Invoke(Timestamp.Now,
                                                    this,
                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    AFRRSignalResponse? response = null;

                    var results = OnAFRRSignal?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnAFRRSignalDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= AFRRSignalResponse.Failed(request);

                    #endregion

                    #region Send OnAFRRSignalResponse event

                    try
                    {

                        OnAFRRSignalResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalResponse));
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
                                            "AFRRSignal",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "AFRRSignal",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnAFRRSignalWSResponse event

            try
            {

                OnAFRRSignalWSResponse?.Invoke(Timestamp.Now,
                                               WebSocketConnection,
                                               requestJSON,
                                               OCPPResponse?.Message,
                                               OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
