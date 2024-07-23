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

        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?  CustomCertificateSignedRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<CertificateSignedResponse>?     CustomCertificateSignedResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a CertificateSigned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCertificateSignedRequestSentDelegate?     OnCertificateSignedRequestSent;

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCertificateSignedResponseReceivedDelegate?    OnCertificateSignedResponseReceived;

        #endregion


        #region CertificateSigned(Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        public async Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequestSent?.Invoke(startTime,
                                                   parentNetworkingNode,
                                                   Request,
                                                SendMessageResult.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnCertificateSignedRequestSent));
            }

            #endregion


            CertificateSignedResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomCertificateSignedRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (CertificateSignedResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var certificateSignedResponse,
                                                           out var errorResponse,
                                                           CustomCertificateSignedResponseParser) &&
                        certificateSignedResponse is not null)
                    {
                        response = certificateSignedResponse;
                    }

                    response ??= new CertificateSignedResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new CertificateSignedResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new CertificateSignedResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponseReceived?.Invoke(endTime,
                                                    parentNetworkingNode,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnCertificateSignedResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCertificateSignedResponseReceivedDelegate? OnCertificateSignedResponseReceived;

    }

}
