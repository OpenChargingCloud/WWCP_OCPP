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

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<UpdateFirmwareResponse>?     CustomUpdateFirmwareResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateFirmwareRequestSentDelegate?     OnUpdateFirmwareRequestSent;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateFirmwareResponseReceivedDelegate?    OnUpdateFirmwareResponseReceived;

        #endregion


        #region UpdateFirmware(Request)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequestSent?.Invoke(startTime,
                                                parentNetworkingNode,
                                                null,
                                                Request,
                                                SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateFirmwareRequestSent));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomUpdateFirmwareRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomFirmwareSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (UpdateFirmwareResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var updateFirmwareResponse,
                                                        out var errorResponse,
                                                        CustomUpdateFirmwareResponseParser) &&
                        updateFirmwareResponse is not null)
                    {
                        response = updateFirmwareResponse;
                    }

                    response ??= new UpdateFirmwareResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new UpdateFirmwareResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new UpdateFirmwareResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponseReceived?.Invoke(endTime,
                                                 parentNetworkingNode,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateFirmwareResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateFirmwareResponseReceivedDelegate? OnUpdateFirmwareResponseReceived;

    }

}
