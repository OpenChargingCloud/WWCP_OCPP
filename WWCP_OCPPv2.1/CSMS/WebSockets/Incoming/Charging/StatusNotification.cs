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

    #region OnStatusNotification

    /// <summary>
    /// A status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            StatusNotificationRequest   Request);


    /// <summary>
    /// Send a status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     StatusNotificationRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="Response">The status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             StatusNotificationRequest    Request,
                                             StatusNotificationResponse   Response,
                                             TimeSpan                     Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<StatusNotificationRequest>?       CustomStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a StatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?              OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?     OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?            OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?    OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a StatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?             OnStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_StatusNotification(JArray                     json,
                                       JObject                    requestData,
                                       Request_Id                 requestId,
                                       ChargingStation_Id         chargingStationId,
                                       WebSocketServerConnection  Connection,
                                       String                     OCPPTextMessage,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnStatusNotificationWSRequest event

            try
            {

                OnStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                      this,
                                                      json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (StatusNotificationRequest.TryParse(requestData,
                                                       requestId,
                                                       chargingStationId,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomStatusNotificationRequestParser) && request is not null) {

                    #region Send OnStatusNotificationRequest event

                    try
                    {

                        OnStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    StatusNotificationResponse? response = null;

                    var responseTasks = OnStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                          this,
                                                                                                                          request,
                                                                                                                          CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= StatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnStatusNotificationResponse event

                    try
                    {

                        OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_StatusNotification)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_StatusNotification)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnStatusNotificationWSResponse event

            try
            {

                OnStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       json,
                                                       OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
