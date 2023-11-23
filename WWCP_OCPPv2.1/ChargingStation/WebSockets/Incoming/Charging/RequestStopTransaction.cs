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

    #region OnRequestStopTransaction

    /// <summary>
    /// A request stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnRequestStopTransactionRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                RequestStopTransactionRequest   Request);


    /// <summary>
    /// A request stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestStopTransactionResponse>

        OnRequestStopTransactionDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         WebSocketClientConnection       Connection,
                                         RequestStopTransactionRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A request stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnRequestStopTransactionResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 RequestStopTransactionRequest    Request,
                                                 RequestStopTransactionResponse   Response,
                                                 TimeSpan                         Runtime);

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

        public CustomJObjectParserDelegate<RequestStopTransactionRequest>?  CustomRequestStopTransactionRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a request stop transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                   OnRequestStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?     OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionDelegate?            OnRequestStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a request stop transaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?    OnRequestStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request stop transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                  OnRequestStopTransactionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_RequestStopTransaction(DateTime                   RequestTimestamp,
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

            #region Send OnRequestStopTransactionWSRequest event

            try
            {

                OnRequestStopTransactionWSRequest?.Invoke(Timestamp.Now,
                                                          WebSocketConnection,
                                                          chargingStationId,
                                                          EventTrackingId,
                                                          requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSRequest));
            }

            #endregion

            try
            {

                if (RequestStopTransactionRequest.TryParse(requestJSON,
                                                           requestId,
                                                           ChargingStationIdentity,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomRequestStopTransactionRequestParser) && request is not null) {

                    #region Send OnRequestStopTransactionRequest event

                    try
                    {

                        OnRequestStopTransactionRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    RequestStopTransactionResponse? response = null;

                    var results = OnRequestStopTransaction?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnRequestStopTransactionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= RequestStopTransactionResponse.Failed(request);

                    #endregion

                    #region Send OnRequestStopTransactionResponse event

                    try
                    {

                        OnRequestStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionResponse));
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
                                            "RequestStopTransaction",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "RequestStopTransaction",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnRequestStopTransactionWSResponse event

            try
            {

                OnRequestStopTransactionWSResponse?.Invoke(Timestamp.Now,
                                                           WebSocketConnection,
                                                           requestJSON,
                                                           OCPPResponse?.Message,
                                                           OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
