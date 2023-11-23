﻿/*
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

    #region OnSecurityEventNotification

    /// <summary>
    /// A security event notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSecurityEventNotificationRequestDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   SecurityEventNotificationRequest   Request);


    /// <summary>
    /// A security event notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecurityEventNotificationResponse>

        OnSecurityEventNotificationDelegate(DateTime                           Timestamp,
                                            IEventSender                       Sender,
                                            SecurityEventNotificationRequest   Request,
                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A security event notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSecurityEventNotificationResponseDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    SecurityEventNotificationRequest    Request,
                                                    SecurityEventNotificationResponse   Response,
                                                    TimeSpan                            Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SecurityEventNotificationRequest>?       CustomSecurityEventNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SecurityEventNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?     OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?            OnSecurityEventNotification;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?    OnSecurityEventNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a SecurityEventNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnSecurityEventNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SecurityEventNotification(JArray                     json,
                                              JObject                    requestData,
                                              Request_Id                 requestId,
                                              ChargingStation_Id         chargingStationId,
                                              WebSocketServerConnection  Connection,
                                              String                     OCPPTextMessage,
                                              CancellationToken          CancellationToken)

        {

            #region Send OnSecurityEventNotificationWSRequest event

            try
            {

                OnSecurityEventNotificationWSRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SecurityEventNotificationRequest.TryParse(requestData,
                                                              requestId,
                                                              chargingStationId,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomSecurityEventNotificationRequestParser) && request is not null) {

                    #region Send OnSecurityEventNotificationRequest event

                    try
                    {

                        OnSecurityEventNotificationRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SecurityEventNotificationResponse? response = null;

                    var responseTasks = OnSecurityEventNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnSecurityEventNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= SecurityEventNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnSecurityEventNotificationResponse event

                    try
                    {

                        OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomSecurityEventNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_SecurityEventNotification)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_SecurityEventNotification)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnSecurityEventNotificationWSResponse event

            try
            {

                OnSecurityEventNotificationWSResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              json,
                                                              OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecurityEventNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
