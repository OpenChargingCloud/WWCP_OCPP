﻿/*
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

using cloud.charging.open.protocols.OCPP;
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

        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer         { get; set; }

        public CustomJObjectParserDelegate<FirmwareStatusNotificationResponse>?     CustomFirmwareStatusNotificationResponseResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event CS.OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

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
        public event CS.OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            FirmwareStatusNotificationResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.NetworkingNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomFirmwareStatusNotificationRequestSerializer,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}