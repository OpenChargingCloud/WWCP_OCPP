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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnNotifyEVChargingSchedule (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyEVChargingScheduleRequestDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   NotifyEVChargingScheduleRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a NotifyEVChargingSchedule request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyEVChargingScheduleResponseDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    NotifyEVChargingScheduleRequest    Request,
                                                                    NotifyEVChargingScheduleResponse   Response,
                                                                    TimeSpan                           Runtime);

    #endregion


    /// <summary>
    /// A CP client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?     OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                       OnNotifyEVChargingScheduleWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event ClientResponseLogHandler?                      OnNotifyEVChargingScheduleWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?    OnNotifyEVChargingScheduleResponse;

        #endregion


        #region NotifyEVChargingSchedule             (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingSchedule request.</param>
        public async Task<NotifyEVChargingScheduleResponse>

            NotifyEVChargingSchedule(NotifyEVChargingScheduleRequest  Request)

        {

            #region Send OnNotifyEVChargingScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleRequest?.Invoke(startTime,
                                                          this,
                                                          Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingScheduleRequest));
            }

            #endregion


            NotifyEVChargingScheduleResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomNotifyEVChargingScheduleRequestSerializer,
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
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyEVChargingScheduleResponse.TryParse(Request,
                                                                  sendRequestState.Response,
                                                                  out var reportChargingProfilesResponse,
                                                                  out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifyEVChargingScheduleResponse(Request,
                                                                      Result.Format(errorResponse));

                }

                response ??= new NotifyEVChargingScheduleResponse(Request,
                                                                  Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyEVChargingScheduleResponse(Request,
                                                              Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnNotifyEVChargingScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
