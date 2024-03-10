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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationSerializer        { get; set; }

        public CustomJObjectParserDelegate<SecurityEventNotificationResponse>?     CustomSecurityEventNotificationResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSecurityEventNotificationRequestSentDelegate?     OnSecurityEventNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                    OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                   OnSecurityEventNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSecurityEventNotificationResponseReceivedDelegate?    OnSecurityEventNotificationResponseReceived;

        #endregion


        #region SendSecurityEventNotification(Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A SecurityEventNotification request.</param>
        public async Task<SecurityEventNotificationResponse>

            SecurityEventNotification(SecurityEventNotificationRequest  Request)

        {

            #region Send OnSecurityEventNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationRequestSent?.Invoke(startTime,
                                                           parentNetworkingNode,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSecurityEventNotificationRequestSent));
            }

            #endregion


            SecurityEventNotificationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomSecurityEventNotificationSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SecurityEventNotificationResponse.TryParse(Request,
                                                                   sendRequestState.JSONResponse.Payload,
                                                                   out var securityEventNotificationResponse,
                                                                   out var errorResponse,
                                                                   CustomSecurityEventNotificationResponseParser) &&
                        securityEventNotificationResponse is not null)
                    {
                        response = securityEventNotificationResponse;
                    }

                    response ??= new SecurityEventNotificationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SecurityEventNotificationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SecurityEventNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSecurityEventNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationResponseReceived?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSecurityEventNotificationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseReceivedDelegate?    OnSecurityEventNotificationResponseReceived;

    }

}
