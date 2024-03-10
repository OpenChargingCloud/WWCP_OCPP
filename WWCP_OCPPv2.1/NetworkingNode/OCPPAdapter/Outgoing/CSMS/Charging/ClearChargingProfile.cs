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

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ClearChargingProfileResponse>?     CustomClearChargingProfileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearChargingProfileRequestSentDelegate?     OnClearChargingProfileRequestSent;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearChargingProfileResponseReceivedDelegate?    OnClearChargingProfileResponseReceived;

        #endregion


        #region ClearChargingProfile(Request)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequestSent?.Invoke(startTime,
                                                      parentNetworkingNode,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearChargingProfileRequestSent));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomClearChargingProfileRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomClearChargingProfileSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ClearChargingProfileResponse.TryParse(Request,
                                                              sendRequestState.JSONResponse.Payload,
                                                              out var clearChargingProfileResponse,
                                                              out var errorResponse,
                                                              CustomClearChargingProfileResponseParser) &&
                        clearChargingProfileResponse is not null)
                    {
                        response = clearChargingProfileResponse;
                    }

                    response ??= new ClearChargingProfileResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ClearChargingProfileResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ClearChargingProfileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponseReceived?.Invoke(endTime,
                                                       parentNetworkingNode,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearChargingProfileResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearChargingProfileResponseReceivedDelegate? OnClearChargingProfileResponseReceived;

    }

}
