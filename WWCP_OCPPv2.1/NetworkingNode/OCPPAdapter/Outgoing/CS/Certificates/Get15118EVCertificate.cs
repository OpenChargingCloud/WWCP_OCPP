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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?  CustomGet15118EVCertificateSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request will be sent.
        /// </summary>
        public event OnGet15118EVCertificateRequestSentDelegate?  OnGet15118EVCertificateRequestSent;

        #endregion


        #region Get15118EVCertificate(Request)

        /// <summary>
        /// Request a 15118 EV certificate.
        /// </summary>
        /// <param name="Request">A Get15118EVCertificate request.</param>
        public async Task<Get15118EVCertificateResponse>

            Get15118EVCertificate(Get15118EVCertificateRequest  Request)

        {

            #region Send OnGet15118EVCertificateRequestSent event

            var logger = OnGet15118EVCertificateRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                            OfType<OnGet15118EVCertificateRequestSentDelegate>().
                                            Select(loggingDelegate => loggingDelegate.Invoke(
                                                                          Timestamp.Now,
                                                                          parentNetworkingNode,
                                                                          Request
                                                                      )).
                                            ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateRequestSent));
                }
            }

            #endregion


            Get15118EVCertificateResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGet15118EVCertificateSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                {

                    response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_Get15118EVCertificateResponse(
                                                                            Request,
                                                                            jsonResponse,
                                                                            null,
                                                                            Request.EventTrackingId,
                                                                            Request.RequestId,
                                                                            Request.CancellationToken
                                                                        );

                }

                response ??= new Get15118EVCertificateResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new Get15118EVCertificateResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate response was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseReceivedDelegate?  OnGet15118EVCertificateResponseReceived;

        #endregion


        #region Receive Get15118EVCertificate response (wired via reflection!)

        public async Task<Get15118EVCertificateResponse>

            Receive_Get15118EVCertificateResponse(Get15118EVCertificateRequest  Request,
                                                  JObject                       ResponseJSON,
                                                  IWebSocketConnection          WebSocketConnection,
                                                  //NetworkingNode_Id             DestinationId,
                                                  //NetworkPath                   NetworkPath,
                                                  EventTracking_Id              EventTrackingId,
                                                  Request_Id                    RequestId,
                                                  CancellationToken             CancellationToken   = default)

        {

            Get15118EVCertificateResponse? response   = null;

            try
            {

                if (Get15118EVCertificateResponse.TryParse(Request,
                                                           ResponseJSON,
                                                           out response,
                                                           out var errorResponse,
                                                           CustomGet15118EVCertificateResponseParser)) {

                    #region Send OnGet15118EVCertificateResponseReceived event

                    var logger = OnGet15118EVCertificateResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                    OfType <OnGet15118EVCertificateResponseReceivedDelegate>().
                                                    Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                    Timestamp.Now,
                                                                                    parentNetworkingNode,
                                                                                    //    WebSocketConnection,
                                                                                    Request,
                                                                                    response,
                                                                                    response.Runtime
                                                                                )).
                                                    ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateResponseReceived));
                        }
                    }

                    #endregion

                }
                else
                    response = new Get15118EVCertificateResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );

            }
            catch (Exception e)
            {

                response = new Get15118EVCertificateResponse(
                                   Request,
                                   Result.FromException(e)
                               );

            }

            return response;

        }

        #endregion


    }

}
