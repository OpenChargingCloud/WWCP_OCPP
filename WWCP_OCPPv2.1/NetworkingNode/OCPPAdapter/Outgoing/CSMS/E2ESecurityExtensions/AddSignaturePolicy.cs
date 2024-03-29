﻿/*
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
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
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

        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?  CustomAddSignaturePolicyRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<AddSignaturePolicyResponse>?     CustomAddSignaturePolicyResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a AddSignaturePolicy request was sent.
        /// </summary>
        public event OCPP.CSMS.OnAddSignaturePolicyRequestDelegate?     OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a AddSignaturePolicy request was sent.
        /// </summary>
        public event OCPP.CSMS.OnAddSignaturePolicyResponseDelegate?    OnAddSignaturePolicyResponse;

        #endregion


        #region AddSignaturePolicy(Request)

        public async Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
        {

            #region Send OnAddSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyRequest?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnAddSignaturePolicyRequest));
            }

            #endregion


            AddSignaturePolicyResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomAddSignaturePolicyRequestSerializer
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

                    if (AddSignaturePolicyResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var setDisplayMessageResponse,
                                                            out var errorResponse,
                                                            CustomAddSignaturePolicyResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new AddSignaturePolicyResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new AddSignaturePolicyResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new AddSignaturePolicyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnAddSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyResponse?.Invoke(endTime,
                                                     parentNetworkingNode,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnAddSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
