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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?  CustomReservationStatusUpdateRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ReservationStatusUpdateResponse>?     CustomReservationStatusUpdateResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event CS.OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                      OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event ClientResponseLogHandler?                     OnReservationStatusUpdateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event CS.OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        #endregion


        #region SendReservationStatusUpdate(Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="Request">A ReservationStatusUpdate request.</param>
        public async Task<ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(ReservationStatusUpdateRequest  Request)

        {

            #region Send OnReservationStatusUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                         this,
                                                         Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            ReservationStatusUpdateResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.NetworkingNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomReservationStatusUpdateRequestSerializer,
                                             CustomSignatureSerializer,
                                             CustomCustomDataSerializer
                                         )
                                     );

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (ReservationStatusUpdateResponse.TryParse(Request,
                                                                     sendRequestState.JSONResponse.Payload,
                                                                     out var reservationStatusUpdateResponse,
                                                                     out var errorResponse,
                                                                     CustomReservationStatusUpdateResponseParser) &&
                            reservationStatusUpdateResponse is not null)
                        {
                            response = reservationStatusUpdateResponse;
                        }

                        response ??= new ReservationStatusUpdateResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new ReservationStatusUpdateResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new ReservationStatusUpdateResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new ReservationStatusUpdateResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnReservationStatusUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
