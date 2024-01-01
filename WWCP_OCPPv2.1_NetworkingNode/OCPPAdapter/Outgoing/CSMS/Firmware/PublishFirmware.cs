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

        public CustomJObjectSerializerDelegate<PublishFirmwareRequest>?  CustomPublishFirmwareRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<PublishFirmwareResponse>?     CustomPublishFirmwareResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a PublishFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareRequestSentDelegate?     OnPublishFirmwareRequestSent;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareResponseReceivedDelegate?    OnPublishFirmwareResponseReceived;

        #endregion


        #region PublishFirmware(Request)

        public async Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request)
        {

            #region Send OnPublishFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareRequestSent?.Invoke(startTime,
                                                 parentNetworkingNode,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnPublishFirmwareRequestSent));
            }

            #endregion


            PublishFirmwareResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomPublishFirmwareRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (PublishFirmwareResponse.TryParse(Request,
                                                         sendRequestState.JSONResponse.Payload,
                                                         out var publishFirmwareResponse,
                                                         out var errorResponse,
                                                         CustomPublishFirmwareResponseParser) &&
                        publishFirmwareResponse is not null)
                    {
                        response = publishFirmwareResponse;
                    }

                    response ??= new PublishFirmwareResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new PublishFirmwareResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new PublishFirmwareResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnPublishFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareResponseReceived?.Invoke(endTime,
                                                  parentNetworkingNode,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnPublishFirmwareResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareResponseReceivedDelegate? OnPublishFirmwareResponseReceived;

    }

}
