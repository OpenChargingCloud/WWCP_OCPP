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

    #region OnReset

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnResetRequestDelegate(DateTime       Timestamp,
                               IEventSender   Sender,
                               ResetRequest   Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ResetResponse>

        OnResetDelegate(DateTime                    Timestamp,
                        IEventSender                Sender,
                        WebSocketClientConnection   Connection,
                        ResetRequest                Request,
                        CancellationToken           CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnResetResponseDelegate(DateTime        Timestamp,
                                IEventSender    Sender,
                                ResetRequest    Request,
                                ResetResponse   Response,
                                TimeSpan        Runtime);

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

        public CustomJObjectParserDelegate<ResetRequest>?  CustomResetRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate?        OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate?               OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?       OnResetResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnResetWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_Reset(JArray                     requestJSON,
                          JObject                    requestData,
                          Request_Id                 requestId,
                          ChargingStation_Id         chargingStationId,
                          WebSocketClientConnection  WebSocketConnection,
                          CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnResetWSRequest event

            try
            {

                OnResetWSRequest?.Invoke(Timestamp.Now,
                                         this,
                                         requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSRequest));
            }

            #endregion

            try
            {

                if (ResetRequest.TryParse(requestData,
                                          requestId,
                                          ChargingStationIdentity,
                                          out var request,
                                          out var errorResponse,
                                          CustomResetRequestParser) && request is not null) {

                    #region Send OnResetRequest event

                    try
                    {

                        OnResetRequest?.Invoke(Timestamp.Now,
                                               this,
                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ResetResponse? response = null;

                    var results = OnReset?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnResetDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ResetResponse.Failed(request);

                    #endregion

                    #region Send OnResetResponse event

                    try
                    {

                        OnResetResponse?.Invoke(Timestamp.Now,
                                                this,
                                                request,
                                                response,
                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetResponse));
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
                                            "Reset",
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "Reset",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnResetWSResponse event

            try
            {

                OnResetWSResponse?.Invoke(Timestamp.Now,
                                          this,
                                          requestJSON,
                                          OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? []);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
