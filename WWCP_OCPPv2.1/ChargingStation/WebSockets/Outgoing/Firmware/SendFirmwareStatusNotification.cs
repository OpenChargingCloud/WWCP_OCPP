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

    #region OnFirmwareStatusNotification (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a firmware status notification request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnFirmwareStatusNotificationRequestDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     FirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a firmware status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnFirmwareStatusNotificationResponseDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      FirmwareStatusNotificationRequest    Request,
                                                                      FirmwareStatusNotificationResponse   Response,
                                                                      TimeSpan                             Runtime);

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

        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer         { get; set; }

        public CustomJObjectParserDelegate<FirmwareStatusNotificationResponse>?     CustomFirmwareStatusNotificationResponseResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                         OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                        OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

        #endregion


        #region SendFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A FirmwareStatusNotification request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request)

        {

            #region Send OnFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            FirmwareStatusNotificationResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(Request.Action,
                                                       Request.RequestId,
                                                       Request.ToJSON(
                                                           CustomFirmwareStatusNotificationRequestSerializer,
                                                           CustomSignatureSerializer,
                                                           CustomCustomDataSerializer
                                                       ));

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (FirmwareStatusNotificationResponse.TryParse(Request,
                                                                        sendRequestState.JSONResponse.Payload,
                                                                        out var firmwareStatusNotificationResponse,
                                                                        out var errorResponse,
                                                                        CustomFirmwareStatusNotificationResponseResponseParser) &&
                            firmwareStatusNotificationResponse is not null)
                        {
                            response = firmwareStatusNotificationResponse;
                        }

                        response ??= new FirmwareStatusNotificationResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new FirmwareStatusNotificationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new FirmwareStatusNotificationResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new FirmwareStatusNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
