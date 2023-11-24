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

    #region OnNotifyChargingLimit

    /// <summary>
    /// A notify charging limit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyChargingLimitRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             NotifyChargingLimitRequest   Request);


    /// <summary>
    /// A notify charging limit at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyChargingLimitResponse>

        OnNotifyChargingLimitDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      NotifyChargingLimitRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A notify charging limit response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyChargingLimitResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              NotifyChargingLimitRequest    Request,
                                              NotifyChargingLimitResponse   Response,
                                              TimeSpan                      Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyChargingLimitRequest>?       CustomNotifyChargingLimitRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?               OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?     OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit was received.
        /// </summary>
        public event OnNotifyChargingLimitDelegate?            OnNotifyChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a NotifyChargingLimit was sent.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?    OnNotifyChargingLimitResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyChargingLimit was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?              OnNotifyChargingLimitWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyChargingLimit(JArray                     json,
                                        JObject                    requestData,
                                        Request_Id                 requestId,
                                        ChargingStation_Id         chargingStationId,
                                        WebSocketServerConnection  Connection,
                                        String                     OCPPTextMessage,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnNotifyChargingLimitWSRequest event

            try
            {

                OnNotifyChargingLimitWSRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyChargingLimitRequest.TryParse(requestData,
                                                        requestId,
                                                        chargingStationId,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomNotifyChargingLimitRequestParser) && request is not null) {

                    #region Send OnNotifyChargingLimitRequest event

                    try
                    {

                        OnNotifyChargingLimitRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyChargingLimitResponse? response = null;

                    var responseTasks = OnNotifyChargingLimit?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyChargingLimitDelegate)?.Invoke(Timestamp.Now,
                                                                                                                           this,
                                                                                                                           request,
                                                                                                                           CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyChargingLimitResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyChargingLimitResponse event

                    try
                    {

                        OnNotifyChargingLimitResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomNotifyChargingLimitResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_NotifyChargingLimit)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_NotifyChargingLimit)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnNotifyChargingLimitWSResponse event

            try
            {

                OnNotifyChargingLimitWSResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        json,
                                                        OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyChargingLimitWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
