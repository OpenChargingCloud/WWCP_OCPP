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

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetChargingProfileResponse>?     CustomSetChargingProfileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetChargingProfile request was sent.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        #endregion


        #region SetChargingProfile(Request)

        public async Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            SetChargingProfileResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(

                                                     CustomSetChargingProfileRequestSerializer,
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

                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetChargingProfileResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var setChargingProfileResponse,
                                                            out var errorResponse,
                                                            CustomSetChargingProfileResponseParser) &&
                        setChargingProfileResponse is not null)
                    {
                        response = setChargingProfileResponse;
                    }

                    response ??= new SetChargingProfileResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetChargingProfileResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetChargingProfileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
