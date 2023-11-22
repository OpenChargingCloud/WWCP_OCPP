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

    #region OnIncomingBinaryDataTransfer

    /// <summary>
    /// An incoming binary data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The binary data transfer request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferRequestDelegate(DateTime                       Timestamp,
                                                    IEventSender                   Sender,
                                                    CS.BinaryDataTransferRequest   Request);


    /// <summary>
    /// An incoming binary data transfer from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The binary data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<BinaryDataTransferResponse>

        OnIncomingBinaryDataTransferDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             CS.BinaryDataTransferRequest   Request,
                                             CancellationToken              CancellationToken);


    /// <summary>
    /// An incoming binary data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The binary data transfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferResponseDelegate(DateTime                          Timestamp,
                                                     IEventSender                      Sender,
                                                     CS.BinaryDataTransferRequest      Request,
                                                     CSMS.BinaryDataTransferResponse   Response,
                                                     TimeSpan                          Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomBinaryParserDelegate<CS.BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                      OnIncomingBinaryDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferRequestDelegate?     OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferDelegate?            OnIncomingBinaryDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnIncomingBinaryDataTransferResponseDelegate?    OnIncomingBinaryDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                     OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_BinaryResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_BinaryDataTransfer(JArray                     json,
                                       Byte[]                     requestData,
                                       Request_Id                 requestId,
                                       ChargingStation_Id         chargingStationId,
                                       WebSocketServerConnection  Connection,
                                       Byte[]                     OCPPBinaryMessage,
                                       CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?           OCPPErrorResponse   = null;

            #region Send OnIncomingBinaryDataTransferWSRequest event

            //try
            //{

            //    OnIncomingBinaryDataTransferWSRequest?.Invoke(Timestamp.Now,
            //                                                  this,
            //                                                  OCPPBinaryMessage);

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
            //}

            #endregion

            try
            {

                if (CS.BinaryDataTransferRequest.TryParse(requestData,
                                                          requestId,
                                                          chargingStationId,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomBinaryDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingBinaryDataTransferRequest event

                    try
                    {

                        OnIncomingBinaryDataTransferRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    BinaryDataTransferResponse? response = null;

                    var responseTasks = OnIncomingBinaryDataTransfer?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnIncomingBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                  this,
                                                                                                                                  request,
                                                                                                                                  CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= BinaryDataTransferResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingBinaryDataTransferResponse event

                    try
                    {

                        OnIncomingBinaryDataTransferResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse  = new OCPP_WebSocket_BinaryResponseMessage(
                                        requestId,
                                        response.Data
                                    );

                }

                else
                    OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                            requestId,
                                            ResultCodes.FormationViolation,
                                            "The given 'IncomingBinaryDataTransfer' request could not be parsed!",
                                            new JObject(
                                                new JProperty("request",       OCPPBinaryMessage.ToBase64()),
                                                new JProperty("errorResponse", errorResponse)
                                            )
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                        requestId,
                                        ResultCodes.FormationViolation,
                                        "Processing the given 'IncomingBinaryDataTransfer' request led to an exception!",
                                        JSONObject.Create(
                                            new JProperty("request",    OCPPBinaryMessage.ToBase64()),
                                            new JProperty("exception",  e.Message),
                                            new JProperty("stacktrace", e.StackTrace)
                                        )
                                    );

            }

            #region Send OnIncomingBinaryDataTransferWSResponse event

            //try
            //{

            //    OnIncomingBinaryDataTransferWSResponse?.Invoke(Timestamp.Now,
            //                                                   this,
            //                                                   json,
            //                                                   OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
            //}

            #endregion


            return new Tuple<OCPP_WebSocket_BinaryResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
