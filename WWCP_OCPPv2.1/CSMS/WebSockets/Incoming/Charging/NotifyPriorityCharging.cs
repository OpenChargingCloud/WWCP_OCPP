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

    #region OnNotifyPriorityCharging

    /// <summary>
    /// A NotifyPriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyPriorityChargingRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyPriorityChargingRequest   Request);


    /// <summary>
    /// A NotifyPriorityCharging at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyPriorityChargingResponse>

        OnNotifyPriorityChargingDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         NotifyPriorityChargingRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A NotifyPriorityCharging response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyPriorityChargingResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 NotifyPriorityChargingRequest    Request,
                                                 NotifyPriorityChargingResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?       CustomNotifyPriorityChargingRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnNotifyPriorityChargingWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?     OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a NotifyPriorityCharging was received.
        /// </summary>
        public event OnNotifyPriorityChargingDelegate?            OnNotifyPriorityCharging;

        /// <summary>
        /// An event sent whenever a response to a NotifyPriorityCharging was sent.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?    OnNotifyPriorityChargingResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyPriorityCharging was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnNotifyPriorityChargingWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyPriorityCharging(JArray                     json,
                                           JObject                    requestData,
                                           Request_Id                 requestId,
                                           ChargingStation_Id         chargingStationId,
                                           WebSocketServerConnection  Connection,
                                           String                     OCPPTextMessage,
                                           CancellationToken          CancellationToken)

        {

            #region Send OnNotifyPriorityChargingWSRequest event

            try
            {

                OnNotifyPriorityChargingWSRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyPriorityChargingRequest.TryParse(requestData,
                                                           requestId,
                                                           chargingStationId,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomNotifyPriorityChargingRequestParser) && request is not null) {

                    #region Send OnNotifyPriorityChargingRequest event

                    try
                    {

                        OnNotifyPriorityChargingRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyPriorityChargingResponse? response = null;

                    var responseTasks = OnNotifyPriorityCharging?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyPriorityChargingDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              this,
                                                                                                                              request,
                                                                                                                              CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyPriorityChargingResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyPriorityChargingResponse event

                    try
                    {

                        OnNotifyPriorityChargingResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomNotifyPriorityChargingResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_NotifyPriorityCharging)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_NotifyPriorityCharging)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnNotifyPriorityChargingWSResponse event

            try
            {

                OnNotifyPriorityChargingWSResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           json,
                                                           OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyPriorityChargingWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
