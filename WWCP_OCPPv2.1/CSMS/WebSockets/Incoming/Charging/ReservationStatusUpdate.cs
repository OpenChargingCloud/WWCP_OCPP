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

    #region OnReservationStatusUpdate

    /// <summary>
    /// A reservation status update request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnReservationStatusUpdateRequestDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 ReservationStatusUpdateRequest   Request);


    /// <summary>
    /// A reservation status update at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReservationStatusUpdateResponse>

        OnReservationStatusUpdateDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          ReservationStatusUpdateRequest   Request,
                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A reservation status update response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnReservationStatusUpdateResponseDelegate(DateTime                          Timestamp,
                                                  IEventSender                      Sender,
                                                  ReservationStatusUpdateRequest    Request,
                                                  ReservationStatusUpdateResponse   Response,
                                                  TimeSpan                          Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ReservationStatusUpdateRequest>?       CustomReservationStatusUpdateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateResponse>?  CustomReservationStatusUpdateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                   OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate was received.
        /// </summary>
        public event OnReservationStatusUpdateDelegate?            OnReservationStatusUpdate;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                  OnReservationStatusUpdateWSResponse;

        #endregion


        
#region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_ReservationStatusUpdate(JArray                     json,
                                            JObject                    requestData,
                                            Request_Id                 requestId,
                                            ChargingStation_Id         chargingStationId,
                                            WebSocketServerConnection  Connection,
                                            String                     OCPPTextMessage,
                                            CancellationToken          CancellationToken)

        {

            #region Send OnReservationStatusUpdateWSRequest event

            try
            {

                OnReservationStatusUpdateWSRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReservationStatusUpdateRequest.TryParse(requestData,
                                                            requestId,
                                                            chargingStationId,
                                                            out var request,
                                                            out var errorResponse,
                                                            CustomReservationStatusUpdateRequestParser) && request is not null) {

                    #region Send OnReservationStatusUpdateRequest event

                    try
                    {

                        OnReservationStatusUpdateRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ReservationStatusUpdateResponse? response = null;

                    var responseTasks = OnReservationStatusUpdate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnReservationStatusUpdateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               request,
                                                                                                                               CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= ReservationStatusUpdateResponse.Failed(request);

                    #endregion

                    #region Send OnReservationStatusUpdateResponse event

                    try
                    {

                        OnReservationStatusUpdateResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  request,
                                                                  response,
                                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomReservationStatusUpdateResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_ReservationStatusUpdate)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_ReservationStatusUpdate)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnReservationStatusUpdateWSResponse event

            try
            {

                OnReservationStatusUpdateWSResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            json,
                                                            OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReservationStatusUpdateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion

    }

}
