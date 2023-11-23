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

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                 Timestamp,
                                              IEventSender             Sender,
                                              CS.DataTransferRequest   Request);


    /// <summary>
    /// An incoming data transfer from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                 Timestamp,
                                       IEventSender             Sender,
                                       CS.DataTransferRequest   Request,
                                       CancellationToken        CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                    Timestamp,
                                               IEventSender                Sender,
                                               CS.DataTransferRequest      Request,
                                               CSMS.DataTransferResponse   Response,
                                               TimeSpan                    Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<CS.DataTransferRequest>?    CustomDataTransferRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DataTransfer WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a DataTransfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnIncomingDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_DataTransfer(JArray                     json,
                                 JObject                    requestData,
                                 Request_Id                 requestId,
                                 ChargingStation_Id         chargingStationId,
                                 WebSocketServerConnection  Connection,
                                 String                     OCPPTextMessage,
                                 CancellationToken          CancellationToken)

        {

            #region Send OnIncomingDataTransferWSRequest event

            try
            {

                OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (CS.DataTransferRequest.TryParse(requestData,
                                                    requestId,
                                                    chargingStationId,
                                                    out var request,
                                                    out var errorResponse,
                                                    CustomDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    DataTransferResponse? response = null;

                    var responseTasks = OnIncomingDataTransfer?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
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
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferResponse));
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
                                            nameof(Receive_DataTransfer)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_DataTransfer)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnIncomingDataTransferWSResponse event

            try
            {

                OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         json,
                                                         OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
