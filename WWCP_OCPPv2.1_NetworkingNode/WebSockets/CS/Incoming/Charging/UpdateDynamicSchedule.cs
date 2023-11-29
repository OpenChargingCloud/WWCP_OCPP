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

        public CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?       CustomUpdateDynamicScheduleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?  CustomUpdateDynamicScheduleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                  OnUpdateDynamicScheduleWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event CS.OnUpdateDynamicScheduleRequestDelegate?     OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event CS.OnUpdateDynamicScheduleDelegate?            OnUpdateDynamicSchedule;

        /// <summary>
        /// An event sent whenever a response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event CS.OnUpdateDynamicScheduleResponseDelegate?    OnUpdateDynamicScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                 OnUpdateDynamicScheduleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UpdateDynamicSchedule(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          ChargingStation_Id         ChargingStationId,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    RequestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnUpdateDynamicScheduleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateDynamicScheduleWSRequest?.Invoke(startTime,
                                                         WebSocketConnection,
                                                         ChargingStationId,
                                                         EventTrackingId,
                                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateDynamicScheduleWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (UpdateDynamicScheduleRequest.TryParse(RequestJSON,
                                                          RequestId,
                                                          ChargingStation_Id.Parse(NetworkingNodeIdentity.ToString()),
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomUpdateDynamicScheduleRequestParser) && request is not null) {

                    #region Send OnUpdateDynamicScheduleRequest event

                    try
                    {

                        OnUpdateDynamicScheduleRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateDynamicScheduleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateDynamicScheduleResponse? response = null;

                    var results = OnUpdateDynamicSchedule?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateDynamicScheduleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateDynamicScheduleResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateDynamicScheduleResponse event

                    try
                    {

                        OnUpdateDynamicScheduleResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateDynamicScheduleResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomUpdateDynamicScheduleResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UpdateDynamicSchedule)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UpdateDynamicSchedule)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateDynamicScheduleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUpdateDynamicScheduleWSResponse?.Invoke(endTime,
                                                          WebSocketConnection,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          RequestJSON,
                                                          OCPPResponse?.Payload,
                                                          OCPPErrorResponse?.ToJSON(),
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateDynamicScheduleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
