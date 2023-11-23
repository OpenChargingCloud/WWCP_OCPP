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

    #region OnSetDefaultChargingTariff

    /// <summary>
    /// A SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetDefaultChargingTariffRequestDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  SetDefaultChargingTariffRequest   Request);


    /// <summary>
    /// A SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetDefaultChargingTariffResponse>

        OnSetDefaultChargingTariffDelegate(DateTime                          Timestamp,
                                           IEventSender                      Sender,
                                           WebSocketClientConnection         Connection,
                                           SetDefaultChargingTariffRequest   Request,
                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A SetDefaultChargingTariff response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetDefaultChargingTariffResponseDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   SetDefaultChargingTariffRequest    Request,
                                                   SetDefaultChargingTariffResponse   Response,
                                                   TimeSpan                           Runtime);

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

        public CustomJObjectParserDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                     OnSetDefaultChargingTariffWSRequest;

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestDelegate?     OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffDelegate?            OnSetDefaultChargingTariff;

        /// <summary>
        /// An event sent whenever a response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseDelegate?    OnSetDefaultChargingTariffResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                    OnSetDefaultChargingTariffWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SetDefaultChargingTariff(DateTime                   RequestTimestamp,
                                             WebSocketClientConnection  WebSocketConnection,
                                             ChargingStation_Id         chargingStationId,
                                             EventTracking_Id           EventTrackingId,
                                             String                     requestText,
                                             Request_Id                 requestId,
                                             JObject                    requestJSON,
                                             CancellationToken          CancellationToken)

        {

            #region Send OnSetDefaultChargingTariffWSRequest event

            try
            {

                OnSetDefaultChargingTariffWSRequest?.Invoke(Timestamp.Now,
                                                            WebSocketConnection,
                                                            chargingStationId,
                                                            EventTrackingId,
                                                            requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDefaultChargingTariffWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (SetDefaultChargingTariffRequest.TryParse(requestJSON,
                                                             requestId,
                                                             ChargingStationIdentity,
                                                             out var request,
                                                             out var errorResponse,
                                                             CustomSetDefaultChargingTariffRequestParser) && request is not null) {

                    #region Send OnSetDefaultChargingTariffRequest event

                    try
                    {

                        OnSetDefaultChargingTariffRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDefaultChargingTariffRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SetDefaultChargingTariffResponse? response = null;

                    var results = OnSetDefaultChargingTariff?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetDefaultChargingTariffDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetDefaultChargingTariffResponse.Failed(request);

                    #endregion

                    #region Send OnSetDefaultChargingTariffResponse event

                    try
                    {

                        OnSetDefaultChargingTariffResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request,
                                                                   response,
                                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDefaultChargingTariffResponse));
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
                                            nameof(Receive_SetDefaultChargingTariff)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_SetDefaultChargingTariff)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnSetDefaultChargingTariffWSResponse event

            try
            {

                OnSetDefaultChargingTariffWSResponse?.Invoke(Timestamp.Now,
                                                             WebSocketConnection,
                                                             requestJSON,
                                                             OCPPResponse?.Message,
                                                             OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDefaultChargingTariffWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
