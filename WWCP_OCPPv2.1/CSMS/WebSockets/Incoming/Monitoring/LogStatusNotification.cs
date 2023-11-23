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

    #region OnLogStatusNotification

    /// <summary>
    /// A log status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnLogStatusNotificationRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               LogStatusNotificationRequest   Request);


    /// <summary>
    /// A log status notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<LogStatusNotificationResponse>

        OnLogStatusNotificationDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        LogStatusNotificationRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A log status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnLogStatusNotificationResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                LogStatusNotificationRequest    Request,
                                                LogStatusNotificationResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<LogStatusNotificationRequest>?       CustomLogStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a LogStatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?     OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationDelegate?            OnLogStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?    OnLogStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a LogStatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnLogStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_LogStatusNotification(JArray                     json,
                                          JObject                    requestData,
                                          Request_Id                 requestId,
                                          ChargingStation_Id         chargingStationId,
                                          WebSocketServerConnection  Connection,
                                          String                     OCPPTextMessage,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnLogStatusNotificationWSRequest event

            try
            {

                OnLogStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (LogStatusNotificationRequest.TryParse(requestData,
                                                          requestId,
                                                          chargingStationId,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomLogStatusNotificationRequestParser) && request is not null) {

                    #region Send OnLogStatusNotificationRequest event

                    try
                    {

                        OnLogStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    LogStatusNotificationResponse? response = null;

                    var responseTasks = OnLogStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnLogStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             this,
                                                                                                                             request,
                                                                                                                             CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= LogStatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnLogStatusNotificationResponse event

                    try
                    {

                        OnLogStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomLogStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_LogStatusNotification)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_LogStatusNotification)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnLogStatusNotificationWSResponse event

            try
            {

                OnLogStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          json,
                                                          OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnLogStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
