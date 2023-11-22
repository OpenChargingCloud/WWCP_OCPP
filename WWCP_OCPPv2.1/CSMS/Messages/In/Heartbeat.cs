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

    #region OnHeartbeat

    /// <summary>
    /// A heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    public delegate Task

        OnHeartbeatRequestDelegate(DateTime           Timestamp,
                                   IEventSender       Sender,
                                   HeartbeatRequest   Request);


    /// <summary>
    /// A heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The heartbeat charging station heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this charging station heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime            Timestamp,
                            IEventSender        Sender,
                            HeartbeatRequest    Request,
                            CancellationToken   CancellationToken);


    /// <summary>
    /// A heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat response.</param>
    /// <param name="Sender">The sender of the charging station heartbeat response.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    /// <param name="Response">The charging station heartbeat response.</param>
    /// <param name="Runtime">The runtime of the charging station heartbeat response.</param>
    public delegate Task

        OnHeartbeatResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    HeartbeatRequest    Request,
                                    HeartbeatResponse   Response,
                                    TimeSpan            Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a Heartbeat WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnHeartbeatWSRequest;

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a Heartbeat was received.
        /// </summary>
        public event OnHeartbeatDelegate?            OnHeartbeat;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a Heartbeat was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnHeartbeatWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_Heartbeat(JArray                     json,
                              JObject                    requestData,
                              Request_Id                 requestId,
                              ChargingStation_Id         chargingStationId,
                              WebSocketServerConnection  Connection,
                              String                     OCPPTextMessage,
                              CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            #region Send OnHeartbeatWSRequest event

            try
            {

                OnHeartbeatWSRequest?.Invoke(Timestamp.Now,
                                             this,
                                             json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSRequest));
            }

            #endregion

            try
            {

                if (HeartbeatRequest.TryParse(requestData,
                                              requestId,
                                              chargingStationId,
                                              out var request,
                                              out var errorResponse,
                                              CustomHeartbeatRequestParser) && request is not null) {

                    #region Send OnHeartbeatRequest event

                    try
                    {

                        OnHeartbeatRequest?.Invoke(Timestamp.Now,
                                                   this,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    HeartbeatResponse? response = null;

                    var responseTasks = OnHeartbeat?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnHeartbeatDelegate)?.Invoke(Timestamp.Now,
                                                                                                                 this,
                                                                                                                 request,
                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= HeartbeatResponse.Failed(request);

                    #endregion

                    #region Send OnHeartbeatResponse event

                    try
                    {

                        OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                    this,
                                                    request,
                                                    response,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                            requestId,
                                            ResultCodes.FormationViolation,
                                            "The given 'Heartbeat' request could not be parsed!",
                                            new JObject(
                                                new JProperty("request",       OCPPTextMessage),
                                                new JProperty("errorResponse", errorResponse)
                                            )
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                        requestId,
                                        ResultCodes.FormationViolation,
                                        "Processing the given 'Heartbeat' request led to an exception!",
                                        JSONObject.Create(
                                            new JProperty("request",    OCPPTextMessage),
                                            new JProperty("exception",  e.Message),
                                            new JProperty("stacktrace", e.StackTrace)
                                        )
                                    );

            }

            #region Send OnHeartbeatWSResponse event

            try
            {

                OnHeartbeatWSResponse?.Invoke(Timestamp.Now,
                                              this,
                                              json,
                                              OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnHeartbeatWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
