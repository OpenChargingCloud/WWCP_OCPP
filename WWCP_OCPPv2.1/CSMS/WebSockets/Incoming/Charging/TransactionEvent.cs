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

    #region OnTransactionEvent

    /// <summary>
    /// A transaction event request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    public delegate Task

        OnTransactionEventRequestDelegate(DateTime                 Timestamp,
                                          IEventSender             Sender,
                                          TransactionEventRequest  Request);


    /// <summary>
    /// A transaction event.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="CancellationToken">A token to cancel this transaction event request.</param>
    public delegate Task<TransactionEventResponse>

        OnTransactionEventDelegate(DateTime                  Timestamp,
                                   IEventSender              Sender,
                                   TransactionEventRequest   Request,
                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A transaction event response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event response.</param>
    /// <param name="Sender">The sender of the transaction event response.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="Response">The transaction event response.</param>
    /// <param name="Runtime">The runtime of the transaction event response.</param>
    public delegate Task

        OnTransactionEventResponseDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           TransactionEventRequest    Request,
                                           TransactionEventResponse   Response,
                                           TimeSpan                   Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<TransactionEventRequest>?       CustomTransactionEventRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<TransactionEventResponse>?  CustomTransactionEventResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a TransactionEvent WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?            OnTransactionEventWSRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventRequestDelegate?     OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventDelegate?            OnTransactionEvent;

        /// <summary>
        /// An event sent whenever a TransactionEvent response was sent.
        /// </summary>
        public event OnTransactionEventResponseDelegate?    OnTransactionEventResponse;

        /// <summary>
        /// An event sent whenever a TransactionEvent WebSocket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?           OnTransactionEventWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_TransactionEvent(JArray                     json,
                                     JObject                    requestData,
                                     Request_Id                 requestId,
                                     ChargingStation_Id         chargingStationId,
                                     WebSocketServerConnection  Connection,
                                     String                     OCPPTextMessage,
                                     CancellationToken          CancellationToken)

        {

            #region Send OnTransactionEventWSRequest event

            try
            {

                OnTransactionEventWSRequest?.Invoke(Timestamp.Now,
                                                    this,
                                                    json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (TransactionEventRequest.TryParse(requestData,
                                                     requestId,
                                                     chargingStationId,
                                                     out var request,
                                                     out var errorResponse,
                                                     CustomTransactionEventRequestParser) && request is not null) {

                    #region Send OnTransactionEventRequest event

                    try
                    {

                        OnTransactionEventRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    TransactionEventResponse? response = null;

                    var responseTasks = OnTransactionEvent?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnTransactionEventDelegate)?.Invoke(Timestamp.Now,
                                                                                                                        this,
                                                                                                                        request,
                                                                                                                        CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= TransactionEventResponse.Failed(request);

                    #endregion

                    #region Send OnTransactionEventResponse event

                    try
                    {

                        OnTransactionEventResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           request,
                                                           response,
                                                           response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomTransactionEventResponseSerializer,
                                           CustomIdTokenInfoSerializer,
                                           CustomIdTokenSerializer,
                                           CustomAdditionalInfoSerializer,
                                           CustomMessageContentSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_TransactionEvent)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_TransactionEvent)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnTransactionEventWSResponse event

            try
            {

                OnTransactionEventWSResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     json,
                                                     OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
