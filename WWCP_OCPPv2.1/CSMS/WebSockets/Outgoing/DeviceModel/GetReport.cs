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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnGetReport (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a get report request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetReportRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    GetReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetReportResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     GetReportRequest    Request,
                                                     GetReportResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetReportRequest>?  CustomGetReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetReportResponse>?     CustomGetReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetReport request was sent.
        /// </summary>
        public event OnGetReportRequestDelegate?     OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetReport request was sent.
        /// </summary>
        public event OnGetReportResponseDelegate?    OnGetReportResponse;

        #endregion


        #region GetReport(Request)

        public async Task<GetReportResponse> GetReport(GetReportRequest Request)
        {

            #region Send OnGetReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetReportRequest?.Invoke(startTime,
                                           this,
                                           Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetReportRequest));
            }

            #endregion


            GetReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetReportRequestSerializer,
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

                OnGetReportResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetReportResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
