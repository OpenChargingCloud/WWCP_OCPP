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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<Get15118EVCertificateRequest>?       CustomGet15118EVCertificateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                           OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGet15118EVCertificateRequestReceivedDelegate?     OnGet15118EVCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGet15118EVCertificateDelegate?            OnGet15118EVCertificate;

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGet15118EVCertificateResponseSentDelegate?    OnGet15118EVCertificateResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a Get15118EVCertificate was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?               OnGet15118EVCertificateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_Get15118EVCertificate(DateTime              RequestTimestamp,
                                          IWebSocketConnection  WebSocketConnection,
                                          NetworkingNode_Id     DestinationNodeId,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          JObject               JSONRequest,
                                          CancellationToken     CancellationToken)

        {

            #region Send OnGet15118EVCertificateWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateWSRequest?.Invoke(startTime,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         DestinationNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestTimestamp,
                                                         JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (Get15118EVCertificateRequest.TryParse(JSONRequest,
                                                          RequestId,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomGet15118EVCertificateRequestParser)) {

                    #region Send OnGet15118EVCertificateRequest event

                    try
                    {

                        OnGet15118EVCertificateRequestReceived?.Invoke(Timestamp.Now,
                                                               parentNetworkingNode,
                                                               WebSocketConnection,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    Get15118EVCertificateResponse? response = null;

                    var responseTasks = OnGet15118EVCertificate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnGet15118EVCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             parentNetworkingNode,
                                                                                                                             WebSocketConnection,
                                                                                                                             request,
                                                                                                                             CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= Get15118EVCertificateResponse.Failed(request);

                    #endregion

                    #region Send OnGet15118EVCertificateResponse event

                    try
                    {

                        OnGet15118EVCertificateResponseSent?.Invoke(Timestamp.Now,
                                                                parentNetworkingNode,
                                                                WebSocketConnection,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGet15118EVCertificateResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_Get15118EVCertificate)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_Get15118EVCertificate)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnGet15118EVCertificateWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGet15118EVCertificateWSResponse?.Invoke(endTime,
                                                          parentNetworkingNode,
                                                          WebSocketConnection,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          JSONRequest,
                                                          OCPPResponse?.Payload,
                                                          OCPPErrorResponse?.ToJSON(),
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGet15118EVCertificateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate was sent.
        /// </summary>
        public event OnGet15118EVCertificateResponseSentDelegate? OnGet15118EVCertificateResponseSent;

    }

}
