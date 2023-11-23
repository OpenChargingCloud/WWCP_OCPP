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

    #region OnGetBaseReport

    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetBaseReportRequestDelegate(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       GetBaseReportRequest   Request);


    /// <summary>
    /// A get base report request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetBaseReportResponse>

        OnGetBaseReportDelegate(DateTime                    Timestamp,
                                IEventSender                Sender,
                                WebSocketClientConnection   Connection,
                                GetBaseReportRequest        Request,
                                CancellationToken           CancellationToken);


    /// <summary>
    /// A get base report response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetBaseReportResponseDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        GetBaseReportRequest    Request,
                                        GetBaseReportResponse   Response,
                                        TimeSpan                Runtime);

    #endregion


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

        public CustomJObjectParserDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get base report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnGetBaseReportWSRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportDelegate?            OnGetBaseReport;

        /// <summary>
        /// An event sent whenever a response to a get base report request was sent.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get base report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnGetBaseReportWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_GetBaseReport(DateTime                   RequestTimestamp,
                                  WebSocketClientConnection  WebSocketConnection,
                                  ChargingStation_Id         chargingStationId,
                                  EventTracking_Id           EventTrackingId,
                                  String                     requestText,
                                  Request_Id                 requestId,
                                  JObject                    requestJSON,
                                  CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnGetBaseReportWSRequest event

            try
            {

                OnGetBaseReportWSRequest?.Invoke(Timestamp.Now,
                                                 WebSocketConnection,
                                                 chargingStationId,
                                                 EventTrackingId,
                                                 requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportWSRequest));
            }

            #endregion

            try
            {

                if (GetBaseReportRequest.TryParse(requestJSON,
                                                  requestId,
                                                  ChargingStationIdentity,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomGetBaseReportRequestParser) && request is not null) {

                    #region Send OnGetBaseReportRequest event

                    try
                    {

                        OnGetBaseReportRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetBaseReportResponse? response = null;

                    var results = OnGetBaseReport?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetBaseReportDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetBaseReportResponse.Failed(request);

                    #endregion

                    #region Send OnGetBaseReportResponse event

                    try
                    {

                        OnGetBaseReportResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        request,
                                                        response,
                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "GetBaseReport",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "GetBaseReport",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnGetBaseReportWSResponse event

            try
            {

                OnGetBaseReportWSResponse?.Invoke(Timestamp.Now,
                                                  WebSocketConnection,
                                                  requestJSON,
                                                  OCPPResponse?.Message,
                                                  OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
