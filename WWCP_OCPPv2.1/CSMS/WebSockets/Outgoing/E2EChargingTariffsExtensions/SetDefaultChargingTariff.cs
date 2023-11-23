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

    #region OnSetDefaultChargingTariff (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a SetDefaultChargingTariff request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnSetDefaultChargingTariffRequestDelegate(DateTime                          Timestamp,
                                                                   IEventSender                      Sender,
                                                                   SetDefaultChargingTariffRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a SetDefaultChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetDefaultChargingTariffResponseDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    SetDefaultChargingTariffRequest    Request,
                                                                    SetDefaultChargingTariffResponse   Response,
                                                                    TimeSpan                           Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?     CustomSetDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestDelegate?        OnSetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseDelegate?       OnSetDefaultChargingTariffResponse;

        #endregion


        #region SetDefaultChargingTariff(Request)

        public async Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)
        {

            #region Send OnSetDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDefaultChargingTariffRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDefaultChargingTariffRequest));
            }

            #endregion


            SetDefaultChargingTariffResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSetDefaultChargingTariffRequestSerializer,
                                                     CustomChargingTariffSerializer,
                                                     CustomPriceSerializer,
                                                     CustomTariffElementSerializer,
                                                     CustomPriceComponentSerializer,
                                                     CustomTaxRateSerializer,
                                                     CustomTariffRestrictionsSerializer,
                                                     CustomEnergyMixSerializer,
                                                     CustomEnergySourceSerializer,
                                                     CustomEnvironmentalImpactSerializer,
                                                     CustomIdTokenSerializer,
                                                     CustomAdditionalInfoSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (SetDefaultChargingTariffResponse.TryParse(Request,
                                                                  sendRequestState.Response,
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
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
