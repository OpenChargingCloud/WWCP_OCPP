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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
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
        public event OnRequestStartTransactionRequestSentDelegate?     OnRequestStartTransactionRequestSent;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseReceivedDelegate?    OnRequestStartTransactionResponseReceived;

        #endregion


        #region StartCharging(Request)

        public async Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request)
        {

            #region Send OnRequestStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionRequestSent?.Invoke(startTime,
                                                         this,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionRequestSent));
            }

            #endregion


            RequestStartTransactionResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
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
                    sendRequestState.JSONResponse is not null)
                {

                    if (RequestStartTransactionResponse.TryParse(Request,
                                                                 sendRequestState.JSONResponse.Payload,
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

            }
            catch (Exception e)
            {

                response = new RequestStartTransactionResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnRequestStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionResponseReceived?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRequestStartTransactionResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
