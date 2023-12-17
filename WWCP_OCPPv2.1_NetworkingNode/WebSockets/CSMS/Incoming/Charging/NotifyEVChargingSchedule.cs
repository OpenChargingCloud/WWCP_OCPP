﻿/*
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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : ACSMSWSServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?       CustomNotifyEVChargingScheduleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                              OnNotifyEVChargingScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleRequestDelegate?     OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event sent whenever a NotifyEVChargingSchedule was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleDelegate?            OnNotifyEVChargingSchedule;

        /// <summary>
        /// An event sent whenever a response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleResponseDelegate?    OnNotifyEVChargingScheduleResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyEVChargingSchedule was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?                  OnNotifyEVChargingScheduleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_NotifyEVChargingSchedule(DateTime                   RequestTimestamp,
                                             WebSocketServerConnection  Connection,
                                             NetworkingNode_Id          NetworkingNodeId,
                                             NetworkPath                NetworkPath,
                                             EventTracking_Id           EventTrackingId,
                                             Request_Id                 RequestId,
                                             JObject                    JSONRequest,
                                             CancellationToken          CancellationToken)

        {

            #region Send OnNotifyEVChargingScheduleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleWSRequest?.Invoke(startTime,
                                                            this,
                                                            Connection,
                                                            NetworkingNodeId,
                                                            EventTrackingId,
                                                            RequestTimestamp,
                                                            JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyEVChargingScheduleRequest.TryParse(JSONRequest,
                                                             RequestId,
                                                             NetworkingNodeId,
                                                             NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             CustomNotifyEVChargingScheduleRequestParser) && request is not null) {

                    #region Send OnNotifyEVChargingScheduleRequest event

                    try
                    {

                        OnNotifyEVChargingScheduleRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  Connection,
                                                                  request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnNotifyEVChargingScheduleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyEVChargingScheduleResponse? response = null;

                    var responseTasks = OnNotifyEVChargingSchedule?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyEVChargingScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                Connection,
                                                                                                                                request,
                                                                                                                                CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyEVChargingScheduleResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyEVChargingScheduleResponse event

                    try
                    {

                        OnNotifyEVChargingScheduleResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   Connection,
                                                                   request,
                                                                   response,
                                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnNotifyEVChargingScheduleResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomNotifyEVChargingScheduleResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_NotifyEVChargingSchedule)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_NotifyEVChargingSchedule)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnNotifyEVChargingScheduleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnNotifyEVChargingScheduleWSResponse?.Invoke(endTime,
                                                             this,
                                                             Connection,
                                                             NetworkingNodeId,
                                                             EventTrackingId,
                                                             RequestTimestamp,
                                                             JSONRequest,
                                                             endTime, //ToDo: Refactor me!
                                                             OCPPResponse?.Payload,
                                                             OCPPErrorResponse?.ToJSON(),
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnNotifyEVChargingScheduleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
