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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetBaseReportResponse>?     CustomGetBaseReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetBaseReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetBaseReportRequestSentDelegate?     OnGetBaseReportRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetBaseReportResponseReceivedDelegate?    OnGetBaseReportResponseReceived;

        #endregion


        #region GetBaseReport(Request)

        public async Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request)
        {

            #region Send OnGetBaseReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetBaseReportRequestSent?.Invoke(startTime,
                                               parentNetworkingNode,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetBaseReportRequestSent));
            }

            #endregion


            GetBaseReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetBaseReportRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetBaseReportResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var getBaseReportResponse,
                                                       out var errorResponse,
                                                       CustomGetBaseReportResponseParser) &&
                        getBaseReportResponse is not null)
                    {
                        response = getBaseReportResponse;
                    }

                    response ??= new GetBaseReportResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetBaseReportResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetBaseReportResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetBaseReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetBaseReportResponseReceived?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetBaseReportResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetBaseReportResponseReceivedDelegate? OnGetBaseReportResponseReceived;

    }

}
