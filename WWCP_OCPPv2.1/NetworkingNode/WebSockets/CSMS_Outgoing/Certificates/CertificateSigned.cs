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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnCertificateSigned (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a certificate signed request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCertificateSignedRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            CertificateSignedRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a certificate signed request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCertificateSignedResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             CertificateSignedRequest    Request,
                                                             CertificateSignedResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : WebSocketServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?  CustomCertificateSignedRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<CertificateSignedResponse>?     CustomCertificateSignedResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a CertificateSigned request was sent.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?     OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?    OnCertificateSignedResponse;

        #endregion


        #region CertificateSigned(Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        public async Task<CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            CertificateSignedResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomCertificateSignedRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
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

                OnCertificateSignedResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
