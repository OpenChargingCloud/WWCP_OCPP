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

        public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyChargingLimitResponse>?     CustomNotifyChargingLimitResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitRequestSentDelegate?     OnNotifyChargingLimitRequestSent;

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                              OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                             OnNotifyChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitResponseReceivedDelegate?    OnNotifyChargingLimitResponseReceived;

        #endregion


        #region NotifyChargingLimit(Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A NotifyChargingLimit request.</param>
        public async Task<NotifyChargingLimitResponse>

            NotifyChargingLimit(NotifyChargingLimitRequest  Request)

        {

            #region Send OnNotifyChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitRequestSent?.Invoke(startTime,
                                                     parentNetworkingNode,
                                                     null,
                                                     Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyChargingLimitRequestSent));
            }

            #endregion


            NotifyChargingLimitResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyChargingLimitRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                                                         parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                                                         parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                                                         parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                                                         parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                                                         parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                                                         parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                                                         parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                                                         parentNetworkingNode.OCPP.CustomCostSerializer,

                                                         parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                                                         parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                                                         parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                                                         parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                                                         parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                                                         parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                                                         parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                                                         parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                                                         parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyChargingLimitResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var notifyChargingLimitResponse,
                                                             out var errorResponse,
                                                             CustomNotifyChargingLimitResponseParser) &&
                        notifyChargingLimitResponse is not null)
                    {
                        response = notifyChargingLimitResponse;
                    }

                    response ??= new NotifyChargingLimitResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new NotifyChargingLimitResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyChargingLimitResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitResponseReceived?.Invoke(endTime,
                                                      parentNetworkingNode,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyChargingLimitResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitResponseReceivedDelegate? OnNotifyChargingLimitResponseReceived;

    }

}
