/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer         { get; set; }

        public CustomJObjectParserDelegate<FirmwareStatusNotificationResponse>?     CustomFirmwareStatusNotificationResponseResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnFirmwareStatusNotificationRequestSentDelegate?     OnFirmwareStatusNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                     OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                    OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnFirmwareStatusNotificationResponseReceivedDelegate?    OnFirmwareStatusNotificationResponseReceived;

        #endregion


        #region SendFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A FirmwareStatusNotification request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            FirmwareStatusNotification(FirmwareStatusNotificationRequest Request)

        {

            #region Send OnFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationRequestSent?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                SendMessageResult.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnFirmwareStatusNotificationRequestSent));
            }

            #endregion


            FirmwareStatusNotificationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomFirmwareStatusNotificationRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!FirmwareStatusNotificationResponse.TryParse(Request,
                                                                     sendRequestState.JSONResponse.Payload,
                                                                     out response,
                                                                     out var errorResponse,
                                                                     CustomFirmwareStatusNotificationResponseResponseParser))
                    {
                        response = new FirmwareStatusNotificationResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new FirmwareStatusNotificationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
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

                OnFirmwareStatusNotificationResponseReceived?.Invoke(endTime,
                                                             parentNetworkingNode,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnFirmwareStatusNotificationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnFirmwareStatusNotificationResponseReceivedDelegate? OnFirmwareStatusNotificationResponseReceived;

    }

}
