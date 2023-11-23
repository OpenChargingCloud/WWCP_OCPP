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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnRequestStartTransaction (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a request start transaction request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnRequestStartTransactionRequestDelegate(DateTime                         Timestamp,
                                                                  IEventSender                     Sender,
                                                                  RequestStartTransactionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a request start transaction request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnRequestStartTransactionResponseDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   RequestStartTransactionRequest    Request,
                                                                   RequestStartTransactionResponse   Response,
                                                                   TimeSpan                          Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?  CustomRequestStartTransactionRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<RequestStartTransactionResponse>?     CustomRequestStartTransactionResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        #endregion


        #region StartCharging(Request)

        public async Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request)
        {

            #region Send OnRequestStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionRequest?.Invoke(startTime,
                                                         this,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionRequest));
            }

            #endregion


            RequestStartTransactionResponse? response = null;

            var sendRequestState = await SendJSONAndWait(
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             Request.ChargingStationId,
                                             Request.Action,
                                             Request.ToJSON(

                                                 CustomRequestStartTransactionRequestSerializer,
                                                 CustomIdTokenSerializer,
                                                 CustomAdditionalInfoSerializer,
                                                 CustomChargingProfileSerializer,
                                                 CustomLimitBeyondSoCSerializer,
                                                 CustomChargingScheduleSerializer,
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

                                                 CustomTransactionLimitsSerializer,

                                                 CustomSignatureSerializer,
                                                 CustomCustomDataSerializer

                                             ),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (RequestStartTransactionResponse.TryParse(Request,
                                                             sendRequestState.Response,
                                                             out var requestStartTransactionResponse,
                                                             out var errorResponse,
                                                             CustomRequestStartTransactionResponseParser) &&
                    requestStartTransactionResponse is not null)
                {
                    response = requestStartTransactionResponse;
                }

                response ??= new RequestStartTransactionResponse(
                                 Request,
                                 Result.Format(errorResponse)
                             );

            }

            response ??= new RequestStartTransactionResponse(
                             Request,
                             Result.FromSendRequestState(sendRequestState)
                         );


            #region Send OnRequestStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
