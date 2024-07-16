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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

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
        public event OCPPv2_1.CSMS.OnRequestStartTransactionRequestSentDelegate?     OnRequestStartTransactionRequestSent;

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionResponseReceivedDelegate?    OnRequestStartTransactionResponseReceived;

        #endregion


        #region RequestStartTransaction(Request)

        public async Task<RequestStartTransactionResponse> RequestStartTransaction(RequestStartTransactionRequest Request)
        {

            #region Send OnRequestStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRequestStartTransactionRequestSent?.Invoke(startTime,
                                                         parentNetworkingNode,
                                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRequestStartTransactionRequestSent));
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
                                                         parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                         parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                                                         parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
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

                                                         parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer

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

                OnRequestStartTransactionResponseReceived?.Invoke(endTime,
                                                          parentNetworkingNode,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnRequestStartTransactionResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionResponseReceivedDelegate? OnRequestStartTransactionResponseReceived;

    }

}
