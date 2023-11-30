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
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<PullDynamicScheduleUpdateRequest>?       CustomPullDynamicScheduleUpdateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnPullDynamicScheduleUpdateWSRequest;

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?    OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event sent whenever a PullDynamicScheduleUpdate was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateDelegate?           OnPullDynamicScheduleUpdate;

        /// <summary>
        /// An event sent whenever a response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?   OnPullDynamicScheduleUpdateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a PullDynamicScheduleUpdate was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnPullDynamicScheduleUpdateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_PullDynamicScheduleUpdate(DateTime                   RequestTimestamp,
                                              WebSocketServerConnection  Connection,
                                              ChargingStation_Id         ChargingStationId,
                                              EventTracking_Id           EventTrackingId,
                                              Request_Id                 RequestId,
                                              JObject                    JSONRequest,
                                              CancellationToken          CancellationToken)

        {

            #region Send OnPullDynamicScheduleUpdateWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateWSRequest?.Invoke(startTime,
                                                             this,
                                                             Connection,
                                                             ChargingStationId,
                                                             EventTrackingId,
                                                             RequestTimestamp,
                                                             JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (PullDynamicScheduleUpdateRequest.TryParse(JSONRequest,
                                                              RequestId,
                                                              ChargingStationId,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomPullDynamicScheduleUpdateRequestParser) && request is not null) {

                    #region Send OnPullDynamicScheduleUpdateRequest event

                    try
                    {

                        OnPullDynamicScheduleUpdateRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    PullDynamicScheduleUpdateResponse? response = null;

                    var responseTasks = OnPullDynamicScheduleUpdate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnPullDynamicScheduleUpdateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= PullDynamicScheduleUpdateResponse.Failed(request);

                    #endregion

                    #region Send OnPullDynamicScheduleUpdateResponse event

                    try
                    {

                        OnPullDynamicScheduleUpdateResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomPullDynamicScheduleUpdateResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_PullDynamicScheduleUpdate)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_PullDynamicScheduleUpdate)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnPullDynamicScheduleUpdateWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnPullDynamicScheduleUpdateWSResponse?.Invoke(endTime,
                                                              this,
                                                              Connection,
                                                              ChargingStationId,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPullDynamicScheduleUpdateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
