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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

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

        public CustomJObjectParserDelegate<ClearVariableMonitoringRequest>?       CustomClearVariableMonitoringRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a clear variable monitoring websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                    OnClearVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?     OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringDelegate?            OnClearVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a clear variable monitoring request was sent.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?    OnClearVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear variable monitoring request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                   OnClearVariableMonitoringWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ClearVariableMonitoring(DateTime                   RequestTimestamp,
                                            WebSocketClientConnection  WebSocketConnection,
                                            NetworkingNode_Id          NetworkingNodeId,
                                            NetworkPath                NetworkPath,
                                            EventTracking_Id           EventTrackingId,
                                            Request_Id                 RequestId,
                                            JObject                    RequestJSON,
                                            CancellationToken          CancellationToken)

        {

            #region Send OnClearVariableMonitoringWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearVariableMonitoringWSRequest?.Invoke(startTime,
                                                           WebSocketConnection,
                                                           NetworkingNodeId,
                                                           NetworkPath,
                                                           EventTrackingId,
                                                           RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ClearVariableMonitoringRequest.TryParse(RequestJSON,
                                                            RequestId,
                                                            NetworkingNodeId,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            CustomClearVariableMonitoringRequestParser) && request is not null) {

                    #region Send OnClearVariableMonitoringRequest event

                    try
                    {

                        OnClearVariableMonitoringRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ClearVariableMonitoringResponse? response = null;

                    var results = OnClearVariableMonitoring?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnClearVariableMonitoringDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ClearVariableMonitoringResponse.Failed(request);

                    #endregion

                    #region Send OnClearVariableMonitoringResponse event

                    try
                    {

                        OnClearVariableMonitoringResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  request,
                                                                  response,
                                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomClearVariableMonitoringResponseSerializer,
                                           CustomClearMonitoringResultSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ClearVariableMonitoring)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ClearVariableMonitoring)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnClearVariableMonitoringWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnClearVariableMonitoringWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
