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

    #region OnSendLocalList (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a send local list request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSendLocalListRequestDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        SendLocalListRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a send local list request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSendLocalListResponseDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         SendLocalListRequest    Request,
                                                         SendLocalListResponse   Response,
                                                         TimeSpan                Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?  CustomSendLocalListRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SendLocalListResponse>?     CustomSendLocalListResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?    OnSendLocalListResponse;

        #endregion


        #region SendLocalList(Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            SendLocalListResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSendLocalListRequestSerializer,
                                                     CustomAuthorizationDataSerializer,
                                                     CustomIdTokenSerializer,
                                                     CustomAdditionalInfoSerializer,
                                                     CustomIdTokenInfoSerializer,
                                                     CustomMessageContentSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (SendLocalListResponse.TryParse(Request,
                                                       sendRequestState.Response,
                                                       out var sendLocalListResponse,
                                                       out var errorResponse,
                                                       CustomSendLocalListResponseParser) &&
                        sendLocalListResponse is not null)
                    {
                        response = sendLocalListResponse;
                    }

                    response ??= new SendLocalListResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SendLocalListResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SendLocalListResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
