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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel,
                                                 ICentralSystemChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?  CustomRemoteStartTransactionRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<RemoteStartTransactionResponse>?     CustomRemoteStartTransactionResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RemoteStartTransaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionRequestDelegate?     OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RemoteStartTransaction request was sent.
        /// </summary>
        public event OnRemoteStartTransactionResponseDelegate?    OnRemoteStartTransactionResponse;

        #endregion


        #region RemoteStartTransaction(Request)

        public async Task<RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request)
        {

            #region Send OnRemoteStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            RemoteStartTransactionResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomRemoteStartTransactionRequestSerializer,
                                                     CustomChargingProfileSerializer,
                                                     CustomChargingScheduleSerializer,
                                                     CustomChargingSchedulePeriodSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (RemoteStartTransactionResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var reserveNowResponse,
                                                                out var errorResponse,
                                                                CustomRemoteStartTransactionResponseParser) &&
                        reserveNowResponse is not null)
                    {
                        response = reserveNowResponse;
                    }

                    response ??= new RemoteStartTransactionResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new RemoteStartTransactionResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new RemoteStartTransactionResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnRemoteStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
