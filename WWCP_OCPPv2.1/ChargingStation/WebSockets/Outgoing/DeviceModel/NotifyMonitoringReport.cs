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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnNotifyMonitoringReport (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a notify monitoring report request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnNotifyMonitoringReportRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyMonitoringReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify monitoring report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyMonitoringReportResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  NotifyMonitoringReportRequest    Request,
                                                                  NotifyMonitoringReportResponse   Response,
                                                                  TimeSpan                         Runtime);

    #endregion


    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?  CustomNotifyMonitoringReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?     CustomNotifyMonitoringReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?     OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnNotifyMonitoringReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?    OnNotifyMonitoringReportResponse;

        #endregion


        #region NotifyMonitoringReport(Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="Request">A NotifyMonitoringReport request.</param>
        public async Task<NotifyMonitoringReportResponse>

            NotifyMonitoringReport(NotifyMonitoringReportRequest  Request)

        {

            #region Send OnNotifyMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            NotifyMonitoringReportResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(Request.Action,
                                                       Request.RequestId,
                                                       Request.ToJSON(
                                                           CustomNotifyMonitoringReportRequestSerializer,
                                                           CustomMonitoringDataSerializer,
                                                           CustomComponentSerializer,
                                                           CustomEVSESerializer,
                                                           CustomVariableSerializer,
                                                           CustomVariableMonitoringSerializer,
                                                           CustomSignatureSerializer,
                                                           CustomCustomDataSerializer
                                                       ));

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.Response is not null)
                    {

                        if (NotifyMonitoringReportResponse.TryParse(Request,
                                                                    sendRequestState.Response,
                                                                    out var notifyMonitoringReportResponse,
                                                                    out var errorResponse,
                                                                    CustomNotifyMonitoringReportResponseParser) &&
                            notifyMonitoringReportResponse is not null)
                        {
                            response = notifyMonitoringReportResponse;
                        }

                        response ??= new NotifyMonitoringReportResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new NotifyMonitoringReportResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new NotifyMonitoringReportResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyMonitoringReportResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
