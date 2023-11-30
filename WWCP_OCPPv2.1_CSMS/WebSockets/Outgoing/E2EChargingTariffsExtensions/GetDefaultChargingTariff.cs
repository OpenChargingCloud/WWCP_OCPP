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

        public CustomJObjectSerializerDelegate<GetDefaultChargingTariffRequest>?  CustomGetDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?     CustomGetDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestDelegate?     OnGetDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseDelegate?    OnGetDefaultChargingTariffResponse;

        #endregion


        #region GetDefaultChargingTariff(Request)

        public async Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)
        {

            #region Send OnGetDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffRequest?.Invoke(startTime,
                                                          this,
                                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDefaultChargingTariffRequest));
            }

            #endregion


            GetDefaultChargingTariffResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetDefaultChargingTariffRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetDefaultChargingTariffResponse.TryParse(Request,
                                                                  sendRequestState.JSONResponse.Payload,
                                                                  out var setDisplayMessageResponse,
                                                                  out var errorResponse,
                                                                  CustomGetDefaultChargingTariffResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new GetDefaultChargingTariffResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetDefaultChargingTariffResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetDefaultChargingTariffResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDefaultChargingTariffResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
