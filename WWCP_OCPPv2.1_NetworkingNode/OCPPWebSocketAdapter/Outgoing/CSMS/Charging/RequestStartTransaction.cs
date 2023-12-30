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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?  CustomRequestStartTransactionRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<RequestStartTransactionResponse>?     CustomRequestStartTransactionResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        #endregion


        #region RequestStartTransaction(Request)

        public async Task<RequestStartTransactionResponse> RequestStartTransaction(RequestStartTransactionRequest Request)
        {

            #region Send OnRequestStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionRequest?.Invoke(startTime,
                                                         parentNetworkingNode,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRequestStartTransactionRequest));
            }

            #endregion


            RequestStartTransactionResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(

                                                         CustomRequestStartTransactionRequestSerializer,
                                                         parentNetworkingNode.CustomIdTokenSerializer,
                                                         parentNetworkingNode.CustomAdditionalInfoSerializer,
                                                         parentNetworkingNode.CustomChargingProfileSerializer,
                                                         parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                                                         parentNetworkingNode.CustomChargingScheduleSerializer,
                                                         parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                                                         parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                                                         parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                                                         parentNetworkingNode.CustomSalesTariffSerializer,
                                                         parentNetworkingNode.CustomSalesTariffEntrySerializer,
                                                         parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                                                         parentNetworkingNode.CustomConsumptionCostSerializer,
                                                         parentNetworkingNode.CustomCostSerializer,

                                                         parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                                                         parentNetworkingNode.CustomPriceRuleStackSerializer,
                                                         parentNetworkingNode.CustomPriceRuleSerializer,
                                                         parentNetworkingNode.CustomTaxRuleSerializer,
                                                         parentNetworkingNode.CustomOverstayRuleListSerializer,
                                                         parentNetworkingNode.CustomOverstayRuleSerializer,
                                                         parentNetworkingNode.CustomAdditionalServiceSerializer,

                                                         parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                                                         parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                                                         parentNetworkingNode.CustomTransactionLimitsSerializer,

                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer

                                                     )
                                                 )
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

                OnRequestStartTransactionResponse?.Invoke(endTime,
                                                          parentNetworkingNode,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRequestStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
