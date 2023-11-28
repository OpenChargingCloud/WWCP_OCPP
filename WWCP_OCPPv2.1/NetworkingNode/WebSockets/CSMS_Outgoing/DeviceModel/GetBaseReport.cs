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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnGetBaseReport (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a get base report request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetBaseReportRequestDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        GetBaseReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get base report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetBaseReportResponseDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         GetBaseReportRequest    Request,
                                                         GetBaseReportResponse   Response,
                                                         TimeSpan                Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : WebSocketServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetBaseReportResponse>?     CustomGetBaseReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetBaseReport request was sent.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a response to a GetBaseReport request was sent.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        #endregion


        #region GetBaseReport(Request)

        public async Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request)
        {

            #region Send OnGetBaseReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetBaseReportRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetBaseReportRequest));
            }

            #endregion


            GetBaseReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetBaseReportRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
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

                OnGetBaseReportResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetBaseReportResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
