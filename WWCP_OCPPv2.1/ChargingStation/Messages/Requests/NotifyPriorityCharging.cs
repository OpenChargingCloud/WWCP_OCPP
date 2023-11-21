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

    #region OnNotifyPriorityCharging (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a NotifyPriorityCharging request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyPriorityChargingRequestDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyPriorityChargingRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a NotifyPriorityCharging request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyPriorityChargingResponseDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  NotifyPriorityChargingRequest    Request,
                                                                  NotifyPriorityChargingResponse   Response,
                                                                  TimeSpan                         Runtime);

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

        public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?     OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnNotifyPriorityChargingWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnNotifyPriorityChargingWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?    OnNotifyPriorityChargingResponse;

        #endregion


        #region NotifyPriorityCharging               (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="Request">A NotifyPriorityCharging request.</param>
        public async Task<NotifyPriorityChargingResponse>

            NotifyPriorityCharging(NotifyPriorityChargingRequest  Request)

        {

            #region Send OnNotifyPriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyPriorityChargingRequest));
            }

            #endregion


            NotifyPriorityChargingResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomNotifyPriorityChargingRequestSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyPriorityChargingResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifyPriorityChargingResponse(Request,
                                                                    Result.Format(errorResponse));

                }

                response ??= new NotifyPriorityChargingResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyPriorityChargingResponse(Request,
                                                            Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnNotifyPriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyPriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
