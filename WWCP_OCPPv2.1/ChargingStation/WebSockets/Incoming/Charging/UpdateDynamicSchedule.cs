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

    #region OnUpdateDynamicSchedule

    /// <summary>
    /// A UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateDynamicScheduleRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               UpdateDynamicScheduleRequest   Request);


    /// <summary>
    /// A UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateDynamicScheduleResponse>

        OnUpdateDynamicScheduleDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        WebSocketClientConnection      Connection,
                                        UpdateDynamicScheduleRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A UpdateDynamicSchedule response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateDynamicScheduleResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                UpdateDynamicScheduleRequest    Request,
                                                UpdateDynamicScheduleResponse   Response,
                                                TimeSpan                        Runtime);

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

        public CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?       CustomUpdateDynamicScheduleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?  CustomUpdateDynamicScheduleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnUpdateDynamicScheduleWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?     OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleDelegate?            OnUpdateDynamicSchedule;

        /// <summary>
        /// An event sent whenever a response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?    OnUpdateDynamicScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnUpdateDynamicScheduleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_UpdateDynamicSchedule(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          ChargingStation_Id         chargingStationId,
                                          EventTracking_Id           EventTrackingId,
                                          String                     requestText,
                                          Request_Id                 requestId,
                                          JObject                    requestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnUpdateDynamicScheduleWSRequest event

            try
            {

                OnUpdateDynamicScheduleWSRequest?.Invoke(Timestamp.Now,
                                                         WebSocketConnection,
                                                         chargingStationId,
                                                         EventTrackingId,
                                                         requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (UpdateDynamicScheduleRequest.TryParse(requestJSON,
                                                          requestId,
                                                          ChargingStationIdentity,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomUpdateDynamicScheduleRequestParser) && request is not null) {

                    #region Send OnUpdateDynamicScheduleRequest event

                    try
                    {

                        OnUpdateDynamicScheduleRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateDynamicScheduleResponse? response = null;

                    var results = OnUpdateDynamicSchedule?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateDynamicScheduleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateDynamicScheduleResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateDynamicScheduleResponse event

                    try
                    {

                        OnUpdateDynamicScheduleResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomUpdateDynamicScheduleResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_UpdateDynamicSchedule)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_UpdateDynamicSchedule)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateDynamicScheduleWSResponse event

            try
            {

                OnUpdateDynamicScheduleWSResponse?.Invoke(Timestamp.Now,
                                                          WebSocketConnection,
                                                          requestJSON,
                                                          OCPPResponse?.Message,
                                                          OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
