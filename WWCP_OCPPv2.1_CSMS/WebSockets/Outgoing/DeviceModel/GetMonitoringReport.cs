﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetMonitoringReportResponse>?     CustomGetMonitoringReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was sent.
        /// </summary>
        public event OnGetMonitoringReportRequestSentDelegate?     OnGetMonitoringReportRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetMonitoringReport request was sent.
        /// </summary>
        public event OnGetMonitoringReportResponseReceivedDelegate?    OnGetMonitoringReportResponseReceived;

        #endregion


        #region GetMonitoringReport(Request)

        public async Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request)
        {

            #region Send OnGetMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetMonitoringReportRequestSent?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetMonitoringReportRequestSent));
            }

            #endregion


            GetMonitoringReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetMonitoringReportRequestSerializer,
                                                     CustomComponentVariableSerializer,
                                                     CustomComponentSerializer,
                                                     CustomEVSESerializer,
                                                     CustomVariableSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetMonitoringReportResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var getMonitoringReportResponse,
                                                             out var errorResponse,
                                                             CustomGetMonitoringReportResponseParser) &&
                        getMonitoringReportResponse is not null)
                    {
                        response = getMonitoringReportResponse;
                    }

                    response ??= new GetMonitoringReportResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetMonitoringReportResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetMonitoringReportResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetMonitoringReportResponseReceived?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetMonitoringReportResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
