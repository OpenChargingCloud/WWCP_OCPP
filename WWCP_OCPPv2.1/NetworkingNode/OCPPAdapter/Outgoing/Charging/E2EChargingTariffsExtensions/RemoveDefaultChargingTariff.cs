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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?     CustomRemoveDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestSentDelegate?         OnRemoveDefaultChargingTariffRequestSent;

        /// <summary>
        /// An event sent whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseReceivedDelegate?    OnRemoveDefaultChargingTariffResponseReceived;

        #endregion


        #region RemoveDefaultChargingTariff(Request)

        public async Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)
        {

            #region Send OnRemoveDefaultChargingTariffRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffRequestSent?.Invoke(startTime,
                                                                 parentNetworkingNode,
                                                                 null,
                                                                 Request,
                                                SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRemoveDefaultChargingTariffRequestSent));
            }

            #endregion


            RemoveDefaultChargingTariffResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomRemoveDefaultChargingTariffRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (RemoveDefaultChargingTariffResponse.TryParse(Request,
                                                                     sendRequestState.JSONResponse.Payload,
                                                                     out var setDisplayMessageResponse,
                                                                     out var errorResponse,
                                                                     CustomRemoveDefaultChargingTariffResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new RemoveDefaultChargingTariffResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new RemoveDefaultChargingTariffResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new RemoveDefaultChargingTariffResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnRemoveDefaultChargingTariffResponseReceived event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffResponseReceived?.Invoke(endTime,
                                                                      parentNetworkingNode,
                                                                      Request,
                                                                      response,
                                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRemoveDefaultChargingTariffResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffResponseReceivedDelegate? OnRemoveDefaultChargingTariffResponseReceived;

    }

}
