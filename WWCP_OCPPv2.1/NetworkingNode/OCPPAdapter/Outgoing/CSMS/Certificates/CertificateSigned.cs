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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a CertificateSigned request was sent.
        /// </summary>
        public event OnCertificateSignedRequestSentDelegate?  OnCertificateSignedRequestSent;

        #endregion

        #region CertificateSigned(Request)

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A CertificateSigned request.</param>
        public async Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {

            CertificateSignedResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(

                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         parentNetworkingNode.OCPP.CustomCertificateSignedRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 ),

                                                 async sendMessageResult => {

                                                     #region Send OnCertificateSignedRequestSent event

                                                     try
                                                     {

                                                         var logger = OnCertificateSignedRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                        OfType<OnCertificateSignedRequestSentDelegate>().
                                                                                        Select(filterDelegate => filterDelegate.Invoke(
                                                                                                                     Timestamp.Now,
                                                                                                                     parentNetworkingNode,
                                                                                                                     sendMessageResult.Connection,
                                                                                                                     Request,
                                                                                                                     sendMessageResult.Result
                                                                                                                 )).
                                                                                        ToArray());

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 await HandleErrors(
                                                                           nameof(OCPPWebSocketAdapterOUT),
                                                                           nameof(OnCertificateSignedRequestSent),
                                                                           e
                                                                       );
                                                             }

                                                         }

                                                     }
                                                     catch (Exception e)
                                                     {
                                                         DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnCertificateSignedRequestSent));
                                                     }

                                                     #endregion

                                                 }

                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (CertificateSignedResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var certificateSignedResponse,
                                                           out var errorResponse,
                                                           parentNetworkingNode.OCPP.CustomCertificateSignedResponseParser) &&
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

            //var endTime = Timestamp.Now;

            await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).SendOnCertificateSignedResponseReceived(
                       Timestamp.Now,
                       parentNetworkingNode,
                       null,
                       Request,
                       response,
                       response.Runtime
                   );

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
        public event OnCertificateSignedResponseReceivedDelegate? OnCertificateSignedResponseReceived;

        #region Send OnCertificateSignedResponseReceived event

        public async Task SendOnCertificateSignedResponseReceived(DateTime                   Timestamp,
                                                                  IEventSender               Sender,
                                                                  IWebSocketConnection       Connection,
                                                                  CertificateSignedRequest   Request,
                                                                  CertificateSignedResponse  Response,
                                                                  TimeSpan                   Runtime)
        {

            var logger = OnCertificateSignedResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnCertificateSignedResponseReceivedDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterIN),
                              nameof(OnCertificateSignedResponseReceived),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
