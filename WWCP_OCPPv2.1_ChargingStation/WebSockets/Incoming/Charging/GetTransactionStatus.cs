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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

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

        public CustomJObjectParserDelegate<GetTransactionStatusRequest>?       CustomGetTransactionStatusRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetTransactionStatusResponse>?  CustomGetTransactionStatusResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetTransactionStatus websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetTransactionStatusWSRequest;

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?        OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was received.
        /// </summary>
        public event OnGetTransactionStatusDelegate?               OnGetTransactionStatus;

        /// <summary>
        /// An event sent whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?       OnGetTransactionStatusResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetTransactionStatus request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnGetTransactionStatusWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetTransactionStatus(DateTime                   RequestTimestamp,
                                         WebSocketClientConnection  WebSocketConnection,
                                         NetworkingNode_Id          NetworkingNodeId,
                                         NetworkPath                NetworkPath,
                                         EventTracking_Id           EventTrackingId,
                                         Request_Id                 RequestId,
                                         JObject                    RequestJSON,
                                         CancellationToken          CancellationToken)

        {

            #region Send OnGetTransactionStatusWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetTransactionStatusWSRequest?.Invoke(startTime,
                                                        WebSocketConnection,
                                                        NetworkingNodeId,
                                                        NetworkPath,
                                                        EventTrackingId,
                                                        RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetTransactionStatusRequest.TryParse(RequestJSON,
                                                         RequestId,
                                                         NetworkingNodeId,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         CustomGetTransactionStatusRequestParser) && request is not null) {

                    #region Send OnGetTransactionStatusRequest event

                    try
                    {

                        OnGetTransactionStatusRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetTransactionStatusResponse? response = null;

                    var results = OnGetTransactionStatus?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetTransactionStatusDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetTransactionStatusResponse.Failed(request);

                    #endregion

                    #region Send OnGetTransactionStatusResponse event

                    try
                    {

                        OnGetTransactionStatusResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetTransactionStatusResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetTransactionStatus)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetTransactionStatus)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetTransactionStatusWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetTransactionStatusWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
