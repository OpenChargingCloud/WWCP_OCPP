/*
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

    #region OnCancelReservation

    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    public delegate Task

        OnCancelReservationRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           CancelReservationRequest   Request);


    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CancelReservationResponse>

        OnCancelReservationDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    CancelReservationRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="Response">The cancel reservation response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCancelReservationResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            CancelReservationRequest    Request,
                                            CancelReservationResponse   Response,
                                            TimeSpan                    Runtime);

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

        public CustomJObjectParserDelegate<CancelReservationRequest>?  CustomCancelReservationRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a cancel reservation websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnCancelReservationWSRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?     OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate?            OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?    OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cancel reservation request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnCancelReservationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_CancelReservation(DateTime                   RequestTimestamp,
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

            #region Send OnCancelReservationWSRequest event

            try
            {

                OnCancelReservationWSRequest?.Invoke(Timestamp.Now,
                                                     WebSocketConnection,
                                                     chargingStationId,
                                                     EventTrackingId,
                                                     requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSRequest));
            }

            #endregion

            try
            {

                if (CancelReservationRequest.TryParse(requestJSON,
                                                      requestId,
                                                      ChargingStationIdentity,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomCancelReservationRequestParser) && request is not null) {

                    #region Send OnCancelReservationRequest event

                    try
                    {

                        OnCancelReservationRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    CancelReservationResponse? response = null;

                    var results = OnCancelReservation?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= CancelReservationResponse.Failed(request);

                    #endregion

                    #region Send OnCancelReservationResponse event

                    try
                    {

                        OnCancelReservationResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationResponse));
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
                                            "CancelReservation",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "CancelReservation",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnCancelReservationWSResponse event

            try
            {

                OnCancelReservationWSResponse?.Invoke(Timestamp.Now,
                                                      WebSocketConnection,
                                                      requestJSON,
                                                      OCPPResponse?.Message,
                                                      OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
