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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                   Timestamp,
                                              IEventSender               Sender,
                                              CSMS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                    Timestamp,
                                       IEventSender                Sender,
                                       WebSocketClientConnection   Connection,
                                       CSMS.DataTransferRequest    Request,
                                       CancellationToken           CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                   Timestamp,
                                               IEventSender               Sender,
                                               CSMS.DataTransferRequest   Request,
                                               CS.DataTransferResponse    Response,
                                               TimeSpan                   Runtime);

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

        public CustomJObjectParserDelegate<CSMS.DataTransferRequest>?     CustomIncomingDataTransferRequestParser    { get; set; }

        public CustomJObjectSerializerDelegate<CS.DataTransferResponse>?  CustomDataTransferResponseSerializer       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnIncomingDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_DataTransfer(DateTime                   RequestTimestamp,
                                 WebSocketClientConnection  WebSocketConnection,
                                 ChargingStation_Id         chargingStationId,
                                 EventTracking_Id           EventTrackingId,
                                 String                     requestText,
                                 Request_Id                 requestId,
                                 JObject                    requestJSON,
                                 CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnIncomingDataTransferWSRequest event

            try
            {

                OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                        WebSocketConnection,
                                                        chargingStationId,
                                                        EventTrackingId,
                                                        requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
            }

            #endregion

            try
            {

                if (CSMS.DataTransferRequest.TryParse(requestJSON,
                                                      requestId,
                                                      ChargingStationIdentity,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomIncomingDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    DataTransferResponse? response = null;

                    var results = OnIncomingDataTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= DataTransferResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingDataTransferResponse event

                    try
                    {

                        OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomDataTransferResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "DataTransfer",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "DataTransfer",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnIncomingDataTransferWSResponse event

            try
            {

                OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                         WebSocketConnection,
                                                         requestJSON,
                                                         OCPPResponse?.Message,
                                                         OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
