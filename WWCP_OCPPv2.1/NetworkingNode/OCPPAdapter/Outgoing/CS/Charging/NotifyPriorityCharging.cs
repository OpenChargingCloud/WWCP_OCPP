﻿/*
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
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyPriorityChargingResponse>?     CustomNotifyPriorityChargingResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyPriorityChargingRequestSentDelegate?     OnNotifyPriorityChargingRequestSent;

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                 OnNotifyPriorityChargingWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                OnNotifyPriorityChargingWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyPriorityChargingResponseReceivedDelegate?    OnNotifyPriorityChargingResponseReceived;

        #endregion


        #region NotifyPriorityCharging(Request)

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

                OnNotifyPriorityChargingRequestSent?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        null,
                                                        Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyPriorityChargingRequestSent));
            }

            #endregion


            NotifyPriorityChargingResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyPriorityChargingRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyPriorityChargingResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse,
                                                                CustomNotifyPriorityChargingResponseParser) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifyPriorityChargingResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new NotifyPriorityChargingResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyPriorityChargingResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyPriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingResponseReceived?.Invoke(endTime,
                                                         parentNetworkingNode,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyPriorityChargingResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyPriorityChargingResponseReceivedDelegate? OnNotifyPriorityChargingResponseReceived;

    }

}
