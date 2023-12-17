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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : AChargingStationWSClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<RemoteStopTransactionRequest>?       CustomRemoteStopTransactionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RemoteStopTransaction websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnRemoteStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a RemoteStopTransaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionRequestDelegate?       OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a RemoteStopTransaction request was received.
        /// </summary>
        public event OnRemoteStopTransactionDelegate?              OnRemoteStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a RemoteStopTransaction request was sent.
        /// </summary>
        public event OnRemoteStopTransactionResponseDelegate?      OnRemoteStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a RemoteStopTransaction request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnRemoteStopTransactionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_RemoteStopTransaction(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          NetworkingNode_Id          NetworkingNodeId,
                                          NetworkPath                NetworkPath,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    RequestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnRemoteStopTransactionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionWSRequest?.Invoke(startTime,
                                                         WebSocketConnection,
                                                         NetworkingNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (RemoteStopTransactionRequest.TryParse(RequestJSON,
                                                          RequestId,
                                                          NetworkingNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomRemoteStopTransactionRequestParser) && request is not null) {

                    #region Send OnRemoteStopTransactionRequest event

                    try
                    {

                        OnRemoteStopTransactionRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               WebSocketConnection,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    RemoteStopTransactionResponse? response = null;

                    var results = OnRemoteStopTransaction?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnRemoteStopTransactionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= RemoteStopTransactionResponse.Failed(request);

                    #endregion

                    #region Send OnRemoteStopTransactionResponse event

                    try
                    {

                        OnRemoteStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                WebSocketConnection,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomRemoteStopTransactionResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_RemoteStopTransaction)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_RemoteStopTransaction)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnRemoteStopTransactionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnRemoteStopTransactionWSResponse?.Invoke(endTime,
                                                          WebSocketConnection,
                                                          NetworkingNodeId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          RequestJSON,
                                                          OCPPResponse?.Payload,
                                                          OCPPErrorResponse?.ToJSON(),
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnRemoteStopTransactionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
