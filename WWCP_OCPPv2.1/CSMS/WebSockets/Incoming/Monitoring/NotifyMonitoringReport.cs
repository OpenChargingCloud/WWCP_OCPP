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

    #region OnNotifyMonitoringReport

    /// <summary>
    /// A notify monitoring report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyMonitoringReportRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                NotifyMonitoringReportRequest   Request);


    /// <summary>
    /// A notify monitoring report at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyMonitoringReportResponse>

        OnNotifyMonitoringReportDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         NotifyMonitoringReportRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A notify monitoring report response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyMonitoringReportResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 NotifyMonitoringReportRequest    Request,
                                                 NotifyMonitoringReportResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyMonitoringReportRequest>?       CustomNotifyMonitoringReportRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                  OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport request was received.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?     OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a NotifyMonitoringReport was received.
        /// </summary>
        public event OnNotifyMonitoringReportDelegate?            OnNotifyMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a NotifyMonitoringReport was sent.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?    OnNotifyMonitoringReportResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyMonitoringReport was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnNotifyMonitoringReportWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyMonitoringReport(JArray                     json,
                                           JObject                    requestData,
                                           Request_Id                 requestId,
                                           ChargingStation_Id         chargingStationId,
                                           WebSocketServerConnection  Connection,
                                           String                     OCPPTextMessage,
                                           CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            #region Send OnNotifyMonitoringReportWSRequest event

            try
            {

                OnNotifyMonitoringReportWSRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportWSRequest));
            }

            #endregion

            try
            {

                if (NotifyMonitoringReportRequest.TryParse(requestData,
                                                           requestId,
                                                           chargingStationId,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomNotifyMonitoringReportRequestParser) && request is not null) {

                    #region Send OnNotifyMonitoringReportRequest event

                    try
                    {

                        OnNotifyMonitoringReportRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyMonitoringReportResponse? response = null;

                    var responseTasks = OnNotifyMonitoringReport?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyMonitoringReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              this,
                                                                                                                              request,
                                                                                                                              CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyMonitoringReportResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyMonitoringReportResponse event

                    try
                    {

                        OnNotifyMonitoringReportResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomNotifyMonitoringReportResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                            requestId,
                                            ResultCodes.FormationViolation,
                                            "The given 'NotifyMonitoringReport' request could not be parsed!",
                                            new JObject(
                                                new JProperty("request",       OCPPTextMessage),
                                                new JProperty("errorResponse", errorResponse)
                                            )
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = new OCPP_WebSocket_ErrorMessage(
                                        requestId,
                                        ResultCodes.FormationViolation,
                                        "Processing the given 'NotifyMonitoringReport' request led to an exception!",
                                        JSONObject.Create(
                                            new JProperty("request",    OCPPTextMessage),
                                            new JProperty("exception",  e.Message),
                                            new JProperty("stacktrace", e.StackTrace)
                                        )
                                    );

            }

            #region Send OnNotifyMonitoringReportWSResponse event

            try
            {

                OnNotifyMonitoringReportWSResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           json,
                                                           OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyMonitoringReportWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
