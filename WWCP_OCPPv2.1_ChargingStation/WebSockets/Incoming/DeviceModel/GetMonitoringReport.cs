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

        public CustomJObjectParserDelegate<GetMonitoringReportRequest>?       CustomGetMonitoringReportRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetMonitoringReportResponse>?  CustomGetMonitoringReportResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get monitoring report websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetMonitoringReportWSRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?     OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportDelegate?            OnGetMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a get monitoring report request was sent.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?    OnGetMonitoringReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get monitoring report request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?               OnGetMonitoringReportWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetMonitoringReport(DateTime                   RequestTimestamp,
                                        WebSocketClientConnection  WebSocketConnection,
                                        ChargingStation_Id         ChargingStationId,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    RequestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnGetMonitoringReportWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetMonitoringReportWSRequest?.Invoke(startTime,
                                                       WebSocketConnection,
                                                       ChargingStationId,
                                                       EventTrackingId,
                                                       RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (GetMonitoringReportRequest.TryParse(RequestJSON,
                                                        RequestId,
                                                        ChargingStationIdentity,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomGetMonitoringReportRequestParser) && request is not null) {

                    #region Send OnGetMonitoringReportRequest event

                    try
                    {

                        OnGetMonitoringReportRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetMonitoringReportResponse? response = null;

                    var results = OnGetMonitoringReport?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetMonitoringReportDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetMonitoringReportResponse.Failed(request);

                    #endregion

                    #region Send OnGetMonitoringReportResponse event

                    try
                    {

                        OnGetMonitoringReportResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetMonitoringReportResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetMonitoringReport)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetMonitoringReport)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetMonitoringReportWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetMonitoringReportWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
