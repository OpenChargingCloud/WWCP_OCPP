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

        public CustomJObjectParserDelegate<GetCompositeScheduleRequest>?       CustomGetCompositeScheduleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?        OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?               OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?       OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnGetCompositeScheduleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetCompositeSchedule(DateTime                   RequestTimestamp,
                                         WebSocketClientConnection  WebSocketConnection,
                                         NetworkingNode_Id          DestinationNodeId,
                                         NetworkPath                NetworkPath,
                                         EventTracking_Id           EventTrackingId,
                                         Request_Id                 RequestId,
                                         JObject                    RequestJSON,
                                         CancellationToken          CancellationToken)

        {

            #region Send OnGetCompositeScheduleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleWSRequest?.Invoke(startTime,
                                                        WebSocketConnection,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        EventTrackingId,
                                                        RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetCompositeScheduleRequest.TryParse(RequestJSON,
                                                         RequestId,
                                                         DestinationNodeId,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         CustomGetCompositeScheduleRequestParser) && request is not null) {

                    #region Send OnGetCompositeScheduleRequest event

                    try
                    {

                        OnGetCompositeScheduleRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              WebSocketConnection,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetCompositeScheduleResponse? response = null;

                    var results = OnGetCompositeSchedule?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetCompositeScheduleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetCompositeScheduleResponse.Failed(request);

                    #endregion

                    #region Send OnGetCompositeScheduleResponse event

                    try
                    {

                        OnGetCompositeScheduleResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               WebSocketConnection,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetCompositeScheduleResponseSerializer,
                                           CustomChargingScheduleSerializer,
                                           CustomChargingSchedulePeriodSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetCompositeSchedule)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetCompositeSchedule)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetCompositeScheduleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetCompositeScheduleWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetCompositeScheduleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
