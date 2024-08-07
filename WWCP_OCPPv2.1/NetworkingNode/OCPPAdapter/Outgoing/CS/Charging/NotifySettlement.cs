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
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifySettlementRequest>?  CustomNotifySettlementRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifySettlementResponse>?     CustomNotifySettlementResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a NotifySettlement request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifySettlementRequestSentDelegate?         OnNotifySettlementRequestSent;

        /// <summary>
        /// An event fired whenever a NotifySettlement request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                   OnNotifySettlementWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifySettlement request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                  OnNotifySettlementWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifySettlement request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifySettlementResponseReceivedDelegate?    OnNotifySettlementResponseReceived;

        #endregion


        #region NotifySettlement(Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="Request">A NotifySettlement request.</param>
        public async Task<NotifySettlementResponse>

            NotifySettlement(NotifySettlementRequest  Request)

        {

            #region Send OnNotifySettlementRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifySettlementRequestSent?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        null,
                                                        Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifySettlementRequestSent));
            }

            #endregion


            NotifySettlementResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifySettlementRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifySettlementResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse,
                                                                CustomNotifySettlementResponseParser) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifySettlementResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new NotifySettlementResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifySettlementResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifySettlementResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifySettlementResponseReceived?.Invoke(endTime,
                                                         parentNetworkingNode,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifySettlementResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a NotifySettlement request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifySettlementResponseReceivedDelegate? OnNotifySettlementResponseReceived;

    }

}
