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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestSerializer         { get; set; }

        public CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?     CustomSignedFirmwareStatusNotificationResponseResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a SignedFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationRequestDelegate?     OnSignedFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a SignedFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                               OnSignedFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a SignedFirmwareStatusNotification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                              OnSignedFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a SignedFirmwareStatusNotification request was received.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationResponseDelegate?    OnSignedFirmwareStatusNotificationResponse;

        #endregion


        #region SignedFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a signed firmware status notification.
        /// </summary>
        /// <param name="Request">A SignedFirmwareStatusNotification request.</param>
        [SecurityExtensions]
        public async Task<SignedFirmwareStatusNotificationResponse>

            SignedFirmwareStatusNotification(SignedFirmwareStatusNotificationRequest Request)

        {

            #region Send OnSignedFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignedFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                  this,
                                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSignedFirmwareStatusNotificationRequest));
            }

            #endregion


            SignedFirmwareStatusNotificationResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationNodeId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomSignedFirmwareStatusNotificationRequestSerializer,
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

                        if (SignedFirmwareStatusNotificationResponse.TryParse(Request,
                                                                              sendRequestState.JSONResponse.Payload,
                                                                              out var firmwareStatusNotificationResponse,
                                                                              out var errorResponse,
                                                                              CustomSignedFirmwareStatusNotificationResponseResponseParser) &&
                            firmwareStatusNotificationResponse is not null)
                        {
                            response = firmwareStatusNotificationResponse;
                        }

                        response ??= new SignedFirmwareStatusNotificationResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new SignedFirmwareStatusNotificationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new SignedFirmwareStatusNotificationResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new SignedFirmwareStatusNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSignedFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignedFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                   this,
                                                                   Request,
                                                                   response,
                                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSignedFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
