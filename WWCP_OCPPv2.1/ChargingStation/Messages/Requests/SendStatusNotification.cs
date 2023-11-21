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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnStatusNotification (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStatusNotificationRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             StatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStatusNotificationResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              StatusNotificationRequest    Request,
                                                              StatusNotificationResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion


    /// <summary>
    /// A CP client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?     OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?    OnStatusNotificationResponse;

        #endregion


        #region SendStatusNotification               (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A StatusNotification request.</param>
        public async Task<StatusNotificationResponse>

            SendStatusNotification(StatusNotificationRequest  Request)

        {

            #region Send OnStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStatusNotificationRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            StatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomStatusNotificationRequestSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (StatusNotificationResponse.TryParse(Request,
                                                            sendRequestState.Response,
                                                            out var statusNotificationResponse,
                                                            out var errorResponse) &&
                        statusNotificationResponse is not null)
                    {
                        response = statusNotificationResponse;
                    }

                    response ??= new StatusNotificationResponse(Request,
                                                                Result.Format(errorResponse));

                }

                response ??= new StatusNotificationResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));

            }

            response ??= new StatusNotificationResponse(Request,
                                                        Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
