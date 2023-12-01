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

        public CustomJObjectParserDelegate<SetVariableMonitoringRequest>?       CustomSetVariableMonitoringRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?  CustomSetVariableMonitoringResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set variable monitoring websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                  OnSetVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?     OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringDelegate?            OnSetVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?    OnSetVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variable monitoring request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                 OnSetVariableMonitoringWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SetVariableMonitoring(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          NetworkingNode_Id          NetworkingNodeId,
                                          NetworkPath                NetworkPath,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    RequestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnSetVariableMonitoringWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariableMonitoringWSRequest?.Invoke(startTime,
                                                         WebSocketConnection,
                                                         NetworkingNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SetVariableMonitoringRequest.TryParse(RequestJSON,
                                                          RequestId,
                                                          NetworkingNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomSetVariableMonitoringRequestParser) && request is not null) {

                    #region Send OnSetVariableMonitoringRequest event

                    try
                    {

                        OnSetVariableMonitoringRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SetVariableMonitoringResponse? response = null;

                    var results = OnSetVariableMonitoring?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetVariableMonitoringDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetVariableMonitoringResponse.Failed(request);

                    #endregion

                    #region Send OnSetVariableMonitoringResponse event

                    try
                    {

                        OnSetVariableMonitoringResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomSetVariableMonitoringResponseSerializer,
                                           CustomSetMonitoringResultSerializer,
                                           CustomComponentSerializer,
                                           CustomEVSESerializer,
                                           CustomVariableSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SetVariableMonitoring)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SetVariableMonitoring)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnSetVariableMonitoringWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSetVariableMonitoringWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
