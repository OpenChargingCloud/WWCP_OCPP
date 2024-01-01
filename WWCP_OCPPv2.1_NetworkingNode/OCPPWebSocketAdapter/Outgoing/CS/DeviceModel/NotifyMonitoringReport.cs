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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?  CustomNotifyMonitoringReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?     CustomNotifyMonitoringReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyMonitoringReportRequestSentDelegate?     OnNotifyMonitoringReportRequestSent;

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                 OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                OnNotifyMonitoringReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyMonitoringReportResponseReceivedDelegate?    OnNotifyMonitoringReportResponseReceived;

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

                OnNotifyMonitoringReportRequestSent?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyMonitoringReportRequestSent));
            }

            #endregion


            NotifyMonitoringReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyMonitoringReportRequestSerializer,
                                                         parentNetworkingNode.CustomMonitoringDataSerializer,
                                                         parentNetworkingNode.CustomComponentSerializer,
                                                         parentNetworkingNode.CustomEVSESerializer,
                                                         parentNetworkingNode.CustomVariableSerializer,
                                                         parentNetworkingNode.CustomVariableMonitoringSerializer,
                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyMonitoringReportResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
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

                OnNotifyMonitoringReportResponseReceived?.Invoke(endTime,
                                                         parentNetworkingNode,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyMonitoringReportResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseReceivedDelegate? OnNotifyMonitoringReportResponseReceived;

    }

}
