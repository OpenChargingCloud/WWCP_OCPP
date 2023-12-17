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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeServer,
                                                  INetworkingNodeClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyChargingLimitResponse>?     CustomNotifyChargingLimitResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitRequestDelegate?     OnNotifyChargingLimitRequest;

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
        public event OCPPv2_1.CS.OnNotifyChargingLimitResponseDelegate?    OnNotifyChargingLimitResponse;

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

                OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            NotifyChargingLimitResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.NetworkingNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomNotifyChargingLimitRequestSerializer,
                                             CustomChargingScheduleSerializer,
                                             CustomLimitBeyondSoCSerializer,
                                             CustomChargingSchedulePeriodSerializer,
                                             CustomV2XFreqWattEntrySerializer,
                                             CustomV2XSignalWattEntrySerializer,
                                             CustomSalesTariffSerializer,
                                             CustomSalesTariffEntrySerializer,
                                             CustomRelativeTimeIntervalSerializer,
                                             CustomConsumptionCostSerializer,
                                             CustomCostSerializer,

                                             CustomAbsolutePriceScheduleSerializer,
                                             CustomPriceRuleStackSerializer,
                                             CustomPriceRuleSerializer,
                                             CustomTaxRuleSerializer,
                                             CustomOverstayRuleListSerializer,
                                             CustomOverstayRuleSerializer,
                                             CustomAdditionalServiceSerializer,

                                             CustomPriceLevelScheduleSerializer,
                                             CustomPriceLevelScheduleEntrySerializer,

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

                response ??= new NotifyChargingLimitResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
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

                OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
