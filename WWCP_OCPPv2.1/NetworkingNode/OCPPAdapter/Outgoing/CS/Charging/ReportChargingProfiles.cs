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

        public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ReportChargingProfilesResponse>?     CustomReportChargingProfilesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnReportChargingProfilesRequestSentDelegate?     OnReportChargingProfilesRequestSent;

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                 OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                OnReportChargingProfilesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnReportChargingProfilesResponseReceivedDelegate?    OnReportChargingProfilesResponseReceived;

        #endregion


        #region ReportChargingProfiles(Request)

        /// <summary>
        /// Report about charging profiles.
        /// </summary>
        /// <param name="Request">A ReportChargingProfiles request.</param>
        public async Task<ReportChargingProfilesResponse>

            ReportChargingProfiles(ReportChargingProfilesRequest  Request)

        {

            #region Send OnReportChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesRequestSent?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        null,
                                                        Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnReportChargingProfilesRequestSent));
            }

            #endregion


            ReportChargingProfilesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomReportChargingProfilesRequestSerializer,
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

                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ReportChargingProfilesResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse,
                                                                CustomReportChargingProfilesResponseParser) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new ReportChargingProfilesResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new ReportChargingProfilesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ReportChargingProfilesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnReportChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesResponseReceived?.Invoke(endTime,
                                                         parentNetworkingNode,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnReportChargingProfilesResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnReportChargingProfilesResponseReceivedDelegate? OnReportChargingProfilesResponseReceived;

    }

}
