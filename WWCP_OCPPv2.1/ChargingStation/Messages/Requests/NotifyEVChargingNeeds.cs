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

    #region OnNotifyEVChargingNeeds (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a notify EV charging needs request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyEVChargingNeedsRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                NotifyEVChargingNeedsRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify EV charging needs request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyEVChargingNeedsResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 NotifyEVChargingNeedsRequest    Request,
                                                                 NotifyEVChargingNeedsResponse   Response,
                                                                 TimeSpan                        Runtime);

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

        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?     OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnNotifyEVChargingNeedsWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?    OnNotifyEVChargingNeedsResponse;

        #endregion


        #region NotifyEVChargingNeeds                (Request)

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

                OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            NotifyEVChargingNeedsResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomNotifyEVChargingNeedsRequestSerializer,
                                                       CustomChargingNeedsSerializer,
                                                       CustomACChargingParametersSerializer,
                                                       CustomDCChargingParametersSerializer,
                                                       CustomV2XChargingParametersSerializer,
                                                       CustomEVEnergyOfferSerializer,
                                                       CustomEVPowerScheduleSerializer,
                                                       CustomEVPowerScheduleEntrySerializer,
                                                       CustomEVAbsolutePriceScheduleSerializer,
                                                       CustomEVAbsolutePriceScheduleEntrySerializer,
                                                       CustomEVPriceRuleSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyEVChargingNeedsResponse.TryParse(Request,
                                                               sendRequestState.Response,
                                                               out var notifyEVChargingNeedsResponse,
                                                               out var errorResponse) &&
                        notifyEVChargingNeedsResponse is not null)
                    {
                        response = notifyEVChargingNeedsResponse;
                    }

                    response ??= new NotifyEVChargingNeedsResponse(Request,
                                                                   Result.Format(errorResponse));

                }

                response ??= new NotifyEVChargingNeedsResponse(Request,
                                                               Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyEVChargingNeedsResponse(Request,
                                                           Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
