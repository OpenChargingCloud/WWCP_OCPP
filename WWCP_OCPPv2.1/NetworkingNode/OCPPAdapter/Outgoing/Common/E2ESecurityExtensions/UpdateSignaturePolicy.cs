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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestSentDelegate?         OnUpdateSignaturePolicyRequestSent;

        #endregion

        #region UpdateSignaturePolicy(Request)

        public async Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
        {

            #region Send OnUpdateSignaturePolicyRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyRequestSent?.Invoke(startTime,
                                                           parentNetworkingNode,
                                                           null,
                                                           Request,
                                                           SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateSignaturePolicyRequestSent));
            }

            #endregion


            UpdateSignaturePolicyResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         parentNetworkingNode.OCPP.CustomUpdateSignaturePolicyRequestSerializer
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (UpdateSignaturePolicyResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var setDisplayMessageResponse,
                                                               out var errorResponse,
                                                               parentNetworkingNode.OCPP.CustomUpdateSignaturePolicyResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new UpdateSignaturePolicyResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new UpdateSignaturePolicyResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new UpdateSignaturePolicyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnUpdateSignaturePolicyResponseReceived event

            //var endTime = Timestamp.Now;

            //try
            //{

            //    OnUpdateSignaturePolicyResponseReceived?.Invoke(endTime,
            //                                                    parentNetworkingNode,
            //                                                    Request,
            //                                                    response,
            //                                                    endTime - startTime);

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateSignaturePolicyResponseReceived));
            //}

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseReceivedDelegate? OnUpdateSignaturePolicyResponseReceived;

        #endregion

    }

}
