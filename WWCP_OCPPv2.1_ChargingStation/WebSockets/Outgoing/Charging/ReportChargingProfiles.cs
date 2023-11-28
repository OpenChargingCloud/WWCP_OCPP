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

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ReportChargingProfilesResponse>?     CustomReportChargingProfilesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?     OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnReportChargingProfilesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?    OnReportChargingProfilesResponse;

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

                OnReportChargingProfilesRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            ReportChargingProfilesResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(Request.Action,
                                                       Request.RequestId,
                                                       Request.ToJSON(
                                                           CustomReportChargingProfilesRequestSerializer,
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

                                                           CustomSignatureSerializer,
                                                           CustomCustomDataSerializer
                                                       ));

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

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
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new ReportChargingProfilesResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new ReportChargingProfilesResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
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

                OnReportChargingProfilesResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
