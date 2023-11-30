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

        public CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?  CustomSetNetworkProfileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetNetworkProfileResponse>?     CustomSetNetworkProfileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetNetworkProfile request was sent.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?     OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetNetworkProfile request was sent.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?    OnSetNetworkProfileResponse;

        #endregion


        #region SetNetworkProfile(Request)

        public async Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request)
        {

            #region Send OnSetNetworkProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetNetworkProfileRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetNetworkProfileRequest));
            }

            #endregion


            SetNetworkProfileResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSetNetworkProfileRequestSerializer,
                                                     CustomNetworkConnectionProfileSerializer,
                                                     CustomVPNConfigurationSerializer,
                                                     CustomAPNConfigurationSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetNetworkProfileResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var setNetworkProfileResponse,
                                                           out var errorResponse,
                                                           CustomSetNetworkProfileResponseParser) &&
                        setNetworkProfileResponse is not null)
                    {
                        response = setNetworkProfileResponse;
                    }

                    response ??= new SetNetworkProfileResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetNetworkProfileResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetNetworkProfileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetNetworkProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetNetworkProfileResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetNetworkProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
