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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : AOCPPWebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeIncomingMessages,
                                                  INetworkingNodeOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<RequestStartTransactionRequest>?       CustomRequestStartTransactionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a request start transaction websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                            OnRequestStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStartTransactionDelegate?            OnRequestStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a request start transaction request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request start transaction request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                OnRequestStartTransactionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_RequestStartTransaction(DateTime                   RequestTimestamp,
                                            WebSocketClientConnection  WebSocketConnection,
                                            NetworkingNode_Id          DestinationNodeId,
                                            NetworkPath                NetworkPath,
                                            EventTracking_Id           EventTrackingId,
                                            Request_Id                 RequestId,
                                            JObject                    RequestJSON,
                                            CancellationToken          CancellationToken)

        {

            #region Send OnRequestStartTransactionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionWSRequest?.Invoke(startTime,
                                                           WebSocketConnection,
                                                           DestinationNodeId,
                                                           NetworkPath,
                                                           EventTrackingId,
                                                           RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRequestStartTransactionWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (RequestStartTransactionRequest.TryParse(RequestJSON,
                                                            RequestId,
                                                            DestinationNodeId,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            CustomRequestStartTransactionRequestParser) && request is not null) {

                    #region Send OnRequestStartTransactionRequest event

                    try
                    {

                        OnRequestStartTransactionRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 WebSocketConnection,
                                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRequestStartTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    RequestStartTransactionResponse? response = null;

                    var results = OnRequestStartTransaction?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnRequestStartTransactionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= RequestStartTransactionResponse.Failed(request);

                    #endregion

                    #region Send OnRequestStartTransactionResponse event

                    try
                    {

                        OnRequestStartTransactionResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  WebSocketConnection,
                                                                  request,
                                                                  response,
                                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRequestStartTransactionResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomRequestStartTransactionResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_RequestStartTransaction)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_RequestStartTransaction)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnRequestStartTransactionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnRequestStartTransactionWSResponse?.Invoke(endTime,
                                                            WebSocketConnection,
                                                            DestinationNodeId,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRequestStartTransactionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
