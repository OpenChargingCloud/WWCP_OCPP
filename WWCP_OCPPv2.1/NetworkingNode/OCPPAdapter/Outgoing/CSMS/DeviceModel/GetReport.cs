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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetReportRequest>?  CustomGetReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetReportResponse>?     CustomGetReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetReportRequestSentDelegate?     OnGetReportRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetReportResponseReceivedDelegate?    OnGetReportResponseReceived;

        #endregion


        #region GetReport(Request)

        public async Task<GetReportResponse> GetReport(GetReportRequest Request)
        {

            #region Send OnGetReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetReportRequestSent?.Invoke(startTime,
                                           parentNetworkingNode,
                                           null,
                                           Request, SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetReportRequestSent));
            }

            #endregion


            GetReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetReportRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomComponentVariableSerializer,
                                                         parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                         parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetReportResponse.TryParse(Request,
                                                   sendRequestState.JSONResponse.Payload,
                                                   out var getReport,
                                                   out var errorResponse,
                                                   CustomGetReportResponseParser) &&
                        getReport is not null)
                    {
                        response = getReport;
                    }

                    response ??= new GetReportResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetReportResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetReportResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetReportResponseReceived?.Invoke(endTime,
                                            parentNetworkingNode,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetReportResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetReportResponseReceivedDelegate? OnGetReportResponseReceived;

    }

}
