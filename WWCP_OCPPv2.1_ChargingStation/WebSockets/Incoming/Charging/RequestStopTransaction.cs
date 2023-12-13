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
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : AChargingStationWSClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<RequestStopTransactionRequest>?       CustomRequestStopTransactionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<RequestStopTransactionResponse>?  CustomRequestStopTransactionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RequestStopTransaction websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnRequestStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?      OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a RequestStopTransaction request was received.
        /// </summary>
        public event OnRequestStopTransactionDelegate?             OnRequestStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a RequestStopTransaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?     OnRequestStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a RequestStopTransaction request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnRequestStopTransactionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_RequestStopTransaction(DateTime                   RequestTimestamp,
                                           WebSocketClientConnection  WebSocketConnection,
                                           NetworkingNode_Id          NetworkingNodeId,
                                           NetworkPath                NetworkPath,
                                           EventTracking_Id           EventTrackingId,
                                           Request_Id                 RequestId,
                                           JObject                    RequestJSON,
                                           CancellationToken          CancellationToken)

        {

            #region Send OnRequestStopTransactionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStopTransactionWSRequest?.Invoke(startTime,
                                                          WebSocketConnection,
                                                          NetworkingNodeId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (RequestStopTransactionRequest.TryParse(RequestJSON,
                                                           RequestId,
                                                           NetworkingNodeId,
                                                           NetworkPath,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomRequestStopTransactionRequestParser) && request is not null) {

                    #region Send OnRequestStopTransactionRequest event

                    try
                    {

                        OnRequestStopTransactionRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    RequestStopTransactionResponse? response = null;

                    var results = OnRequestStopTransaction?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnRequestStopTransactionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= RequestStopTransactionResponse.Failed(request);

                    #endregion

                    #region Send OnRequestStopTransactionResponse event

                    try
                    {

                        OnRequestStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomRequestStopTransactionResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_RequestStopTransaction)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_RequestStopTransaction)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnRequestStopTransactionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnRequestStopTransactionWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
