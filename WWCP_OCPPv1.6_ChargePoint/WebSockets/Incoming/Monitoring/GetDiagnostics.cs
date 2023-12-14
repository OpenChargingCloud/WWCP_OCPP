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

        public CustomJObjectParserDelegate<GetDiagnosticsRequest>?       CustomGetDiagnosticsRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDiagnostics websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetDiagnosticsWSRequest;

        /// <summary>
        /// An event sent whenever a GetDiagnostics request was received.
        /// </summary>
        public event OnGetDiagnosticsRequestDelegate?              OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a GetDiagnostics request was received.
        /// </summary>
        public event OnGetDiagnosticsDelegate?                     OnGetDiagnostics;

        /// <summary>
        /// An event sent whenever a response to a GetDiagnostics request was sent.
        /// </summary>
        public event OnGetDiagnosticsResponseDelegate?             OnGetDiagnosticsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetDiagnostics request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnGetDiagnosticsWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetDiagnostics(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   NetworkingNode_Id          NetworkingNodeId,
                                   NetworkPath                NetworkPath,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnGetDiagnosticsWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  NetworkingNodeId,
                                                  NetworkPath,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetDiagnosticsRequest.TryParse(RequestJSON,
                                                   RequestId,
                                                   NetworkingNodeId,
                                                   NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomGetDiagnosticsRequestParser) && request is not null) {

                    #region Send OnGetDiagnosticsRequest event

                    try
                    {

                        OnGetDiagnosticsRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetDiagnosticsResponse? response = null;

                    var results = OnGetDiagnostics?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetDiagnosticsDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetDiagnosticsResponse.Failed(request);

                    #endregion

                    #region Send OnGetDiagnosticsResponse event

                    try
                    {

                        OnGetDiagnosticsResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetDiagnosticsResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetDiagnostics)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetDiagnostics)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetDiagnosticsWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetDiagnosticsWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetDiagnosticsWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
