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
using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : AOCPPWebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   ICSIncomingMessages,
                                                   ICSOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<DataTransferRequest>?       CustomIncomingDataTransferRequestParser    { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?        OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?               OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?       OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnIncomingDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_DataTransfer(DateTime                   RequestTimestamp,
                                 WebSocketClientConnection  WebSocketConnection,
                                 NetworkingNode_Id          DestinationNodeId,
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
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        EventTrackingId,
                                                        RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (DataTransferRequest.TryParse(RequestJSON,
                                                 RequestId,
                                                 DestinationNodeId,
                                                 NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 CustomIncomingDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              WebSocketConnection,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferRequest));
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
                                                               WebSocketConnection,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
