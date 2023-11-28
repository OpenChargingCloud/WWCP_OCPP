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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnDeleteUserRole

    /// <summary>
    /// An DeleteUserRole request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnDeleteUserRoleRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           DeleteUserRoleRequest   Request);


    /// <summary>
    /// An DeleteUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DeleteUserRoleResponse>

        OnDeleteUserRoleDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    DeleteUserRoleRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// An DeleteUserRole response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnDeleteUserRoleResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            DeleteUserRoleRequest    Request,
                                            DeleteUserRoleResponse   Response,
                                            TimeSpan                    Runtime);

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

        public CustomJObjectParserDelegate<DeleteUserRoleRequest>?  CustomDeleteUserRoleRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DeleteUserRole websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?           OnDeleteUserRoleWSRequest;

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleRequestDelegate?     OnDeleteUserRoleRequest;

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleDelegate?            OnDeleteUserRole;

        /// <summary>
        /// An event sent whenever a response to a DeleteUserRole request was sent.
        /// </summary>
        public event OnDeleteUserRoleResponseDelegate?    OnDeleteUserRoleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a DeleteUserRole request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?          OnDeleteUserRoleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_DeleteUserRole(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   ChargingStation_Id         ChargingStationId,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnDeleteUserRoleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteUserRoleWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  ChargingStationId,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteUserRoleWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (DeleteUserRoleRequest.TryParse(RequestJSON,
                                                   RequestId,
                                                   ChargingStationIdentity,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomDeleteUserRoleRequestParser) && request is not null) {

                    #region Send OnDeleteUserRoleRequest event

                    try
                    {

                        OnDeleteUserRoleRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteUserRoleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    DeleteUserRoleResponse? response = null;

                    var results = OnDeleteUserRole?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDeleteUserRoleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= DeleteUserRoleResponse.Failed(request);

                    #endregion

                    #region Send OnDeleteUserRoleResponse event

                    try
                    {

                        OnDeleteUserRoleResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteUserRoleResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_DeleteUserRole)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_DeleteUserRole)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnDeleteUserRoleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnDeleteUserRoleWSResponse?.Invoke(endTime,
                                                   WebSocketConnection,
                                                   EventTrackingId,
                                                   RequestTimestamp,
                                                   RequestJSON,
                                                   OCPPResponse?.Payload,
                                                   OCPPErrorResponse?.ToJSON(),
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteUserRoleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
