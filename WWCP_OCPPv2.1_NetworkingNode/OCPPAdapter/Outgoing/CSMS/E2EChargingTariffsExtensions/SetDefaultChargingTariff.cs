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

        public CustomJObjectSerializerDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?     CustomSetDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffRequestDelegate?        OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffResponseDelegate?       OnSetDefaultChargingTariffResponse;

        #endregion


        #region SetDefaultChargingTariff(Request)

        public async Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)
        {

            #region Send OnSetDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDefaultChargingTariffRequest?.Invoke(startTime,
                                                          parentNetworkingNode,
                                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSetDefaultChargingTariffRequest));
            }

            #endregion


            SetDefaultChargingTariffResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomSetDefaultChargingTariffRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
                                                         parentNetworkingNode.OCPP.CustomPriceSerializer,
                                                         parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                                                         parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                                                         parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                                                         parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                                                         parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                                                         parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                                                         parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                                                         parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                         parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetDefaultChargingTariffResponse.TryParse(Request,
                                                                  sendRequestState.JSONResponse.Payload,
                                                                  out var setDisplayMessageResponse,
                                                                  out var errorResponse,
                                                                  CustomSetDefaultChargingTariffResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new SetDefaultChargingTariffResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetDefaultChargingTariffResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetDefaultChargingTariffResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetDefaultChargingTariffResponse?.Invoke(endTime,
                                                           parentNetworkingNode,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
