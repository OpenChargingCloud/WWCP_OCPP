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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.DataTransferRequest>?     CustomIncomingDataTransferRequestParser    { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.DataTransferResponse>?  CustomDataTransferResponseSerializer       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                 OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event CS.OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event CS.OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event CS.OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                OnIncomingDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_DataTransfer(DateTime                   RequestTimestamp,
                                 WebSocketClientConnection  WebSocketConnection,
                                 NetworkingNode_Id          NetworkingNodeId,
                                 NetworkPath                NetworkPath,
                                 EventTracking_Id           EventTrackingId,
                                 Request_Id                 RequestId,
                                 JObject                    RequestJSON,
                                 CancellationToken          CancellationToken)

        {

            #region Send OnIncomingDataTransferWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingDataTransferWSRequest?.Invoke(startTime,
                                                        WebSocketConnection,
                                                        NetworkingNodeId,
                                                        NetworkPath,
                                                        EventTrackingId,
                                                        RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (OCPPv2_1.CSMS.DataTransferRequest.TryParse(RequestJSON,
                                                               RequestId,
                                                               NetworkingNodeId,
                                                               NetworkPath,
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
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    OCPPv2_1.CS.DataTransferResponse? response = null;

                    var results = OnIncomingDataTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as CS.OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= OCPPv2_1.CS.DataTransferResponse.Failed(request);

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
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomDataTransferResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_DataTransfer)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_DataTransfer)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnIncomingDataTransferWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnIncomingDataTransferWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
