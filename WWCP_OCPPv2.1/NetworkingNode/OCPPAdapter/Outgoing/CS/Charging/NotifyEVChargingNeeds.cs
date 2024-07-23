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

        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyEVChargingNeedsResponse>?     CustomNotifyEVChargingNeedsResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingNeedsRequestSentDelegate?     OnNotifyEVChargingNeedsRequestSent;

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event ClientResponseLogHandler?                               OnNotifyEVChargingNeedsWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseReceivedDelegate?    OnNotifyEVChargingNeedsResponseReceived;

        #endregion


        #region NotifyEVChargingNeeds(Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingNeeds request.</param>
        public async Task<NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest  Request)

        {

            #region Send OnNotifyEVChargingNeedsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsRequestSent?.Invoke(startTime,
                                                       parentNetworkingNode,
                                                       Request,
                                                SendMessageResult.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyEVChargingNeedsRequestSent));
            }

            #endregion


            NotifyEVChargingNeedsResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyEVChargingNeedsRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingNeedsSerializer,
                                                         parentNetworkingNode.OCPP.CustomACChargingParametersSerializer,
                                                         parentNetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                                                         parentNetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                                                         parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                                                         parentNetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyEVChargingNeedsResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var notifyEVChargingNeedsResponse,
                                                               out var errorResponse,
                                                               CustomNotifyEVChargingNeedsResponseParser) &&
                        notifyEVChargingNeedsResponse is not null)
                    {
                        response = notifyEVChargingNeedsResponse;
                    }

                    response ??= new NotifyEVChargingNeedsResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new NotifyEVChargingNeedsResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyEVChargingNeedsResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponseReceived?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyEVChargingNeedsResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseReceivedDelegate? OnNotifyEVChargingNeedsResponseReceived;

    }

}
