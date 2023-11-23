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

    #region OnReserveNow

    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnReserveNowRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    ReserveNowRequest   Request);


    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReserveNowResponse>

        OnReserveNowDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             ReserveNowRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// A reserve now response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnReserveNowResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     ReserveNowRequest    Request,
                                     ReserveNowResponse   Response,
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

        public CustomJObjectParserDelegate<ReserveNowRequest>?       CustomReserveNowRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReserveNowResponse>?  CustomReserveNowResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reserve now websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnReserveNowWSRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?     OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate?            OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?    OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reserve now request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnReserveNowWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_ReserveNow(DateTime                   RequestTimestamp,
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

            #region Send OnReserveNowWSRequest event

            try
            {

                OnReserveNowWSRequest?.Invoke(Timestamp.Now,
                                              WebSocketConnection,
                                              chargingStationId,
                                              EventTrackingId,
                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSRequest));
            }

            #endregion

            try
            {

                if (ReserveNowRequest.TryParse(requestJSON,
                                               requestId,
                                               ChargingStationIdentity,
                                               out var request,
                                               out var errorResponse,
                                               CustomReserveNowRequestParser) && request is not null) {

                    #region Send OnReserveNowRequest event

                    try
                    {

                        OnReserveNowRequest?.Invoke(Timestamp.Now,
                                                    this,
                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ReserveNowResponse? response = null;

                    var results = OnReserveNow?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ReserveNowResponse.Failed(request);

                    #endregion

                    #region Send OnReserveNowResponse event

                    try
                    {

                        OnReserveNowResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomReserveNowResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "ReserveNow",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "ReserveNow",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnReserveNowWSResponse event

            try
            {

                OnReserveNowWSResponse?.Invoke(Timestamp.Now,
                                               WebSocketConnection,
                                               requestJSON,
                                               OCPPResponse?.Message,
                                               OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
