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

        public CustomJObjectSerializerDelegate<GetDefaultChargingTariffRequest>?  CustomGetDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?     CustomGetDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestSentDelegate?         OnGetDefaultChargingTariffRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseReceivedDelegate?    OnGetDefaultChargingTariffResponseReceived;

        #endregion


        #region GetDefaultChargingTariff(Request)

        public async Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)
        {

            #region Send OnGetDefaultChargingTariffRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffRequestSent?.Invoke(startTime,
                                                              parentNetworkingNode,
                                                              null,
                                                              Request,
                                                SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetDefaultChargingTariffRequestSent));
            }

            #endregion


            GetDefaultChargingTariffResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetDefaultChargingTariffRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetDefaultChargingTariffResponse.TryParse(Request,
                                                                  sendRequestState.JSONResponse.Payload,
                                                                  out var setDisplayMessageResponse,
                                                                  out var errorResponse,
                                                                  CustomGetDefaultChargingTariffResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new GetDefaultChargingTariffResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new GetDefaultChargingTariffResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetDefaultChargingTariffResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetDefaultChargingTariffResponseReceived event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffResponseReceived?.Invoke(endTime,
                                                                   parentNetworkingNode,
                                                                   Request,
                                                                   response,
                                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetDefaultChargingTariffResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDefaultChargingTariffResponseReceivedDelegate? OnGetDefaultChargingTariffResponseReceived;

    }

}
