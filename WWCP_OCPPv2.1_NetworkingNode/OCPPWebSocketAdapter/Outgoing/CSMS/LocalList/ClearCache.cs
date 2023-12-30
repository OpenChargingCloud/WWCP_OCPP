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

        public CustomJObjectSerializerDelegate<ClearCacheRequest>?   CustomClearCacheRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ClearCacheResponse>?      CustomClearCacheResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ClearCache request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearCacheResponseDelegate?    OnClearCacheResponse;

        #endregion


        #region ClearCache(Request)

        public async Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                            parentNetworkingNode,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            ClearCacheResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomClearCacheRequestSerializer,
                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ClearCacheResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var clearCacheResponse,
                                                    out var errorResponse,
                                                    CustomClearCacheResponseParser) &&
                        clearCacheResponse is not null)
                    {
                        response = clearCacheResponse;
                    }

                    response ??= new ClearCacheResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ClearCacheResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ClearCacheResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                             parentNetworkingNode,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
