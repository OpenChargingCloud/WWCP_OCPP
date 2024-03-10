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

        public CustomJObjectSerializerDelegate<SendLocalListRequest>?  CustomSendLocalListRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SendLocalListResponse>?     CustomSendLocalListResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendLocalList request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSendLocalListRequestSentDelegate?     OnSendLocalListRequestSent;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSendLocalListResponseReceivedDelegate?    OnSendLocalListResponseReceived;

        #endregion


        #region SendLocalList(Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequestSent?.Invoke(startTime,
                                               parentNetworkingNode,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendLocalListRequestSent));
            }

            #endregion


            SendLocalListResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomSendLocalListRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                         parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SendLocalListResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var sendLocalListResponse,
                                                       out var errorResponse,
                                                       CustomSendLocalListResponseParser) &&
                        sendLocalListResponse is not null)
                    {
                        response = sendLocalListResponse;
                    }

                    response ??= new SendLocalListResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SendLocalListResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SendLocalListResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponseReceived?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendLocalListResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSendLocalListResponseReceivedDelegate? OnSendLocalListResponseReceived;

    }

}
