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

    #region OnPublishFirmwareStatusNotification (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a publish firmware status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnPublishFirmwareStatusNotificationRequestDelegate(DateTime                                   Timestamp,
                                                                            IEventSender                               Sender,
                                                                            PublishFirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a publish firmware status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnPublishFirmwareStatusNotificationResponseDelegate(DateTime                                    Timestamp,
                                                                             IEventSender                                Sender,
                                                                             PublishFirmwareStatusNotificationRequest    Request,
                                                                             PublishFirmwareStatusNotificationResponse   Response,
                                                                             TimeSpan                                    Runtime);

    #endregion


    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<PublishFirmwareStatusNotificationResponse>?     CustomPublishFirmwareStatusNotificationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?     OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                               OnPublishFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?    OnPublishFirmwareStatusNotificationResponse;

        #endregion


        #region SendPublishFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A PublishFirmwareStatusNotification request.</param>
        public async Task<PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request)

        {

            #region Send OnPublishFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                   this,
                                                                   Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            PublishFirmwareStatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (PublishFirmwareStatusNotificationResponse.TryParse(Request,
                                                                           sendRequestState.Response,
                                                                           out var publishFirmwareStatusNotificationResponse,
                                                                           out var errorResponse,
                                                                           CustomPublishFirmwareStatusNotificationResponseParser) &&
                        publishFirmwareStatusNotificationResponse is not null)
                    {
                        response = publishFirmwareStatusNotificationResponse;
                    }

                    response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                               Result.Format(errorResponse));

                }

                response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                           Result.FromSendRequestState(sendRequestState));

            }

            response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                       Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnPublishFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                    this,
                                                                    Request,
                                                                    response,
                                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
