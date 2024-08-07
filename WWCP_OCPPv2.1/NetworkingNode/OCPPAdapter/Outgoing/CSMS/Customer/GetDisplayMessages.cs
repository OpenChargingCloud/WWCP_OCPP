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
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetDisplayMessagesResponse>?     CustomGetDisplayMessagesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDisplayMessagesRequestSentDelegate?     OnGetDisplayMessagesRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDisplayMessagesResponseReceivedDelegate?    OnGetDisplayMessagesResponseReceived;

        #endregion


        #region GetDisplayMessages(Request)

        public async Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
        {

            #region Send OnGetDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesRequestSent?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    null,
                                                    Request,
                                                SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetDisplayMessagesRequestSent));
            }

            #endregion


            GetDisplayMessagesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetDisplayMessagesRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetDisplayMessagesResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var getDisplayMessagesResponse,
                                                            out var errorResponse,
                                                            CustomGetDisplayMessagesResponseParser) &&
                        getDisplayMessagesResponse is not null)
                    {
                        response = getDisplayMessagesResponse;
                    }

                    response ??= new GetDisplayMessagesResponse(
                                     Request,
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new GetDisplayMessagesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetDisplayMessagesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesResponseReceived?.Invoke(endTime,
                                                     parentNetworkingNode,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetDisplayMessagesResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDisplayMessagesResponseReceivedDelegate? OnGetDisplayMessagesResponseReceived;

    }

}
