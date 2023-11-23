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

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>

    public delegate Task

        OnFirmwareStatusNotificationRequestDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    FirmwareStatusNotificationRequest   Request);


    /// <summary>
    /// A firmware status notification from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                            Timestamp,
                                             IEventSender                        Sender,
                                             FirmwareStatusNotificationRequest   Request,
                                             CancellationToken                   CancellationToken);


    /// <summary>
    /// A firmware status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="Response">The firmware status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnFirmwareStatusNotificationResponseDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     FirmwareStatusNotificationRequest    Request,
                                                     FirmwareStatusNotificationResponse   Response,
                                                     TimeSpan                             Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?       CustomFirmwareStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationResponse>?  CustomFirmwareStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                      OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?            OnFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                     OnFirmwareStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_FirmwareStatusNotification(JArray                     json,
                                               JObject                    requestData,
                                               Request_Id                 requestId,
                                               ChargingStation_Id         chargingStationId,
                                               WebSocketServerConnection  Connection,
                                               String                     OCPPTextMessage,
                                               CancellationToken          CancellationToken)

        {

            #region Send OnFirmwareStatusNotificationWSRequest event

            try
            {

                OnFirmwareStatusNotificationWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (FirmwareStatusNotificationRequest.TryParse(requestData,
                                                               requestId,
                                                               chargingStationId,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomFirmwareStatusNotificationRequestParser) &&
                    request is not null) {

                    #region Send OnFirmwareStatusNotificationRequest event

                    try
                    {

                        OnFirmwareStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    FirmwareStatusNotificationResponse? response = null;

                    var responseTasks = OnFirmwareStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                  this,
                                                                                                                                  request,
                                                                                                                                  CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= FirmwareStatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnFirmwareStatusNotificationResponse event

                    try
                    {

                        OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomFirmwareStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_FirmwareStatusNotification)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_FirmwareStatusNotification)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnFirmwareStatusNotificationWSResponse event

            try
            {

                OnFirmwareStatusNotificationWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               json,
                                                               OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? []);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnFirmwareStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
