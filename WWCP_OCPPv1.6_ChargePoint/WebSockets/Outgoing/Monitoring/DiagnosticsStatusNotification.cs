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
    public partial class ChargePointWSClient : AChargingStationWSClient,
                                               IChargePointWebSocketClient,
                                               IChargePointServer,
                                               IChargePointClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?     CustomDiagnosticsStatusNotificationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a DiagnosticsStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?     OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a DiagnosticsStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                            OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                           OnDiagnosticsStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?    OnDiagnosticsStatusNotificationResponse;

        #endregion


        #region DiagnosticsStatusNotification(Request)

        /// <summary>
        /// Send a diagnostics status notification.
        /// </summary>
        /// <param name="Request">A DiagnosticsStatusNotification request.</param>
        public async Task<DiagnosticsStatusNotificationResponse>

            DiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest Request)

        {

            #region Send OnDiagnosticsStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
                                                               this,
                                                               Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            DiagnosticsStatusNotificationResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.NetworkingNodeId,
                                               Request.NetworkPath,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomDiagnosticsStatusNotificationRequestSerializer,
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

                        if (DiagnosticsStatusNotificationResponse.TryParse(Request,
                                                                           sendRequestState.JSONResponse.Payload,
                                                                           out var heartbeatResponse,
                                                                           out var errorResponse,
                                                                           CustomDiagnosticsStatusNotificationResponseParser) &&
                            heartbeatResponse is not null)
                        {
                            response = heartbeatResponse;
                        }

                        response ??= new DiagnosticsStatusNotificationResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new DiagnosticsStatusNotificationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new DiagnosticsStatusNotificationResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new DiagnosticsStatusNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnDiagnosticsStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(endTime,
                                                                this,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
