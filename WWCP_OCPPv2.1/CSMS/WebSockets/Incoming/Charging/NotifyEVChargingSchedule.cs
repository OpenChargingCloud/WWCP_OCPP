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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnNotifyEVChargingSchedule

    /// <summary>
    /// A NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyEVChargingScheduleRequestDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  NotifyEVChargingScheduleRequest   Request);


    /// <summary>
    /// A NotifyEVChargingSchedule at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyEVChargingScheduleResponse>

        OnNotifyEVChargingScheduleDelegate(DateTime                          Timestamp,
                                           IEventSender                      Sender,
                                           NotifyEVChargingScheduleRequest   Request,
                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A NotifyEVChargingSchedule response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyEVChargingScheduleResponseDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   NotifyEVChargingScheduleRequest    Request,
                                                   NotifyEVChargingScheduleResponse   Response,
                                                   TimeSpan                           Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?       CustomNotifyEVChargingScheduleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                    OnNotifyEVChargingScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?     OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleDelegate?            OnNotifyEVChargingSchedule;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?    OnNotifyEVChargingScheduleResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                   OnNotifyEVChargingScheduleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyEVChargingSchedule(JArray                     json,
                                             JObject                    requestData,
                                             Request_Id                 requestId,
                                             ChargingStation_Id         chargingStationId,
                                             WebSocketServerConnection  Connection,
                                             String                     OCPPTextMessage,
                                             CancellationToken          CancellationToken)

        {

            #region Send OnNotifyEVChargingScheduleWSRequest event

            try
            {

                OnNotifyEVChargingScheduleWSRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyEVChargingScheduleRequest.TryParse(requestData,
                                                             requestId,
                                                             chargingStationId,
                                                             out var request,
                                                             out var errorResponse,
                                                             CustomNotifyEVChargingScheduleRequestParser) && request is not null) {

                    #region Send OnNotifyEVChargingScheduleRequest event

                    try
                    {

                        OnNotifyEVChargingScheduleRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyEVChargingScheduleResponse? response = null;

                    var responseTasks = OnNotifyEVChargingSchedule?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyEVChargingScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                request,
                                                                                                                                CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyEVChargingScheduleResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyEVChargingScheduleResponse event

                    try
                    {

                        OnNotifyEVChargingScheduleResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request,
                                                                   response,
                                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomNotifyEVChargingScheduleResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_NotifyEVChargingSchedule)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_NotifyEVChargingSchedule)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnNotifyEVChargingScheduleWSResponse event

            try
            {

                OnNotifyEVChargingScheduleWSResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             json,
                                                             OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
