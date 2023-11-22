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

    #region OnNotifyEVChargingNeeds

    /// <summary>
    /// A notify EV charging needs request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyEVChargingNeedsRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               NotifyEVChargingNeedsRequest   Request);


    /// <summary>
    /// A notify EV charging needs at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyEVChargingNeedsResponse>

        OnNotifyEVChargingNeedsDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        NotifyEVChargingNeedsRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A notify EV charging needs response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyEVChargingNeedsResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyEVChargingNeedsRequest    Request,
                                                NotifyEVChargingNeedsResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?     OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsDelegate?            OnNotifyEVChargingNeeds;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingNeeds was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?    OnNotifyEVChargingNeedsResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEVChargingNeeds was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnNotifyEVChargingNeedsWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyEVChargingNeeds(JArray                     json,
                                          JObject                    requestData,
                                          Request_Id                 requestId,
                                          ChargingStation_Id         chargingStationId,
                                          WebSocketServerConnection  Connection,
                                          String                     OCPPTextMessage,
                                          CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            #region Send OnNotifyEVChargingNeedsWSRequest event

            try
            {

                OnNotifyEVChargingNeedsWSRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsWSRequest));
            }

            #endregion

            try
            {

                if (NotifyEVChargingNeedsRequest.TryParse(requestData,
                                                          requestId,
                                                          chargingStationId,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomNotifyEVChargingNeedsRequestParser) && request is not null) {

                    #region Send OnNotifyEVChargingNeedsRequest event

                    try
                    {

                        OnNotifyEVChargingNeedsRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyEVChargingNeedsResponse? response = null;

                    var responseTasks = OnNotifyEVChargingNeeds?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyEVChargingNeedsDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             this,
                                                                                                                             request,
                                                                                                                             CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyEVChargingNeedsResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyEVChargingNeedsResponse event

                    try
                    {

                        OnNotifyEVChargingNeedsResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsResponse));
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
                                            "The given 'NotifyEVChargingNeeds' request could not be parsed!",
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
                                        "Processing the given 'NotifyEVChargingNeeds' request led to an exception!",
                                        JSONObject.Create(
                                            new JProperty("request",    OCPPTextMessage),
                                            new JProperty("exception",  e.Message),
                                            new JProperty("stacktrace", e.StackTrace)
                                        )
                                    );

            }

            #region Send OnNotifyEVChargingNeedsWSResponse event

            try
            {

                OnNotifyEVChargingNeedsWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          json,
                                                          OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyEVChargingNeedsWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
