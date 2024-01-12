/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                                   IChargePointWebSocketClient,
                                                   ICPIncomingMessages,
                                                   ICPOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ReserveNowRequest>?       CustomReserveNowRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReserveNowResponse>?  CustomReserveNowResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ReserveNow websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnReserveNowWSRequest;

        /// <summary>
        /// An event sent whenever a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?                  OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a ReserveNow request was received.
        /// </summary>
        public event OnReserveNowDelegate?                         OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a ReserveNow request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?                 OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a ReserveNow request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnReserveNowWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_ReserveNow(DateTime                   RequestTimestamp,
                               WebSocketClientConnection  WebSocketConnection,
                               NetworkingNode_Id          DestinationNodeId,
                               NetworkPath                NetworkPath,
                               EventTracking_Id           EventTrackingId,
                               Request_Id                 RequestId,
                               JObject                    RequestJSON,
                               CancellationToken          CancellationToken)

        {

            #region Send OnReserveNowWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowWSRequest?.Invoke(startTime,
                                              WebSocketConnection,
                                              DestinationNodeId,
                                              NetworkPath,
                                              EventTrackingId,
                                              RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReserveNowRequest.TryParse(RequestJSON,
                                               RequestId,
                                               DestinationNodeId,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               CustomReserveNowRequestParser) && request is not null) {

                    #region Send OnReserveNowRequest event

                    try
                    {

                        OnReserveNowRequest?.Invoke(Timestamp.Now,
                                                    this,
                                                    WebSocketConnection,
                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ReserveNowResponse? response = null;

                    var results = OnReserveNow?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ReserveNowResponse.Failed(request);

                    #endregion

                    #region Send OnReserveNowResponse event

                    try
                    {

                        OnReserveNowResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     WebSocketConnection,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomReserveNowResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ReserveNow)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ReserveNow)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnReserveNowWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnReserveNowWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnReserveNowWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
