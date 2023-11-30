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
        public event WebSocketJSONRequestLogHandler?               OnTransactionEventWSRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventRequestDelegate?            OnTransactionEventRequest;

        /// <summary>
        /// An event sent whenever a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventDelegate?                   OnTransactionEvent;

        /// <summary>
        /// An event sent whenever a TransactionEvent response was sent.
        /// </summary>
        public event OnTransactionEventResponseDelegate?           OnTransactionEventResponse;

        /// <summary>
        /// An event sent whenever a TransactionEvent WebSocket response was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?   OnTransactionEventWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_TransactionEvent(DateTime                   RequestTimestamp,
                                     WebSocketServerConnection  Connection,
                                     ChargingStation_Id         ChargingStationId,
                                     EventTracking_Id           EventTrackingId,
                                     Request_Id                 RequestId,
                                     JObject                    JSONRequest,
                                     CancellationToken          CancellationToken)

        {

            #region Send OnTransactionEventWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTransactionEventWSRequest?.Invoke(startTime,
                                                    this,
                                                    Connection,
                                                    ChargingStationId,
                                                    EventTrackingId,
                                                    RequestTimestamp,
                                                    JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (TransactionEventRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     ChargingStationId,
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

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
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
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_TransactionEvent)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_TransactionEvent)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnTransactionEventWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnTransactionEventWSResponse?.Invoke(endTime,
                                                     this,
                                                     Connection,
                                                     ChargingStationId,
                                                     EventTrackingId,
                                                     RequestTimestamp,
                                                     JSONRequest,
                                                     endTime, //ToDo: Refactor me!
                                                     OCPPResponse?.Payload,
                                                     OCPPErrorResponse?.ToJSON(),
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnTransactionEventWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
