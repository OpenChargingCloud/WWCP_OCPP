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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetCRLRequest>?  CustomGetCRLSerializer        { get; set; }

        public CustomJObjectParserDelegate<GetCRLResponse>?     CustomGetCRLResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCRLRequestSentDelegate?     OnGetCRLRequestSent;

        /// <summary>
        /// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnGetCRLWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnGetCRLWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCRLResponseReceivedDelegate?    OnGetCRLResponseReceived;

        #endregion


        #region GetCRL(Request)

        /// <summary>
        /// Send a get certificate revocation list request.
        /// </summary>
        /// <param name="Request">A GetCRL request.</param>
        public async Task<GetCRLResponse>

            GetCRL(GetCRLRequest  Request)

        {

            #region Send OnGetCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCRLRequestSent?.Invoke(startTime,
                                        parentNetworkingNode,
                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetCRLRequestSent));
            }

            #endregion


            GetCRLResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetCRLSerializer,
                                                         parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetCRLResponse.TryParse(Request,
                                                sendRequestState.JSONResponse.Payload,
                                                out var getCertificateStatusResponse,
                                                out var errorResponse,
                                                CustomGetCRLResponseParser) &&
                        getCertificateStatusResponse is not null)
                    {
                        response = getCertificateStatusResponse;
                    }

                    response ??= new GetCRLResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetCRLResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetCRLResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCRLResponseReceived?.Invoke(endTime,
                                         parentNetworkingNode,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetCRLResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCRLResponseReceivedDelegate? OnGetCRLResponseReceived;

    }

}
