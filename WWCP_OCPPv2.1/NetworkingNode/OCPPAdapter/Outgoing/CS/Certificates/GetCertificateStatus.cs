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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?  CustomGetCertificateStatusSerializer        { get; set; }

        public CustomJObjectParserDelegate<GetCertificateStatusResponse>?     CustomGetCertificateStatusResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCertificateStatusRequestSentDelegate?     OnGetCertificateStatusRequestSent;

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                               OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event ClientResponseLogHandler?                              OnGetCertificateStatusWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCertificateStatusResponseReceivedDelegate?    OnGetCertificateStatusResponseReceived;

        #endregion


        #region GetCertificateStatus(Request)

        /// <summary>
        /// Send a get certificate status request.
        /// </summary>
        /// <param name="Request">A GetCertificateStatus request.</param>
        public async Task<GetCertificateStatusResponse>

            GetCertificateStatus(GetCertificateStatusRequest Request)

        {

            #region Send OnGetCertificateStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusRequestSent?.Invoke(startTime,
                                                      parentNetworkingNode,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetCertificateStatusRequestSent));
            }

            #endregion


            GetCertificateStatusResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetCertificateStatusSerializer,
                                                         parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetCertificateStatusResponse.TryParse(Request,
                                                              sendRequestState.JSONResponse.Payload,
                                                              out var getCertificateStatusResponse,
                                                              out var errorResponse,
                                                              CustomGetCertificateStatusResponseParser) &&
                        getCertificateStatusResponse is not null)
                    {
                        response = getCertificateStatusResponse;
                    }

                    response ??= new GetCertificateStatusResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetCertificateStatusResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetCertificateStatusResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetCertificateStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusResponseReceived?.Invoke(endTime,
                                                       parentNetworkingNode,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetCertificateStatusResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCertificateStatusResponseReceivedDelegate? OnGetCertificateStatusResponseReceived;

    }

}
