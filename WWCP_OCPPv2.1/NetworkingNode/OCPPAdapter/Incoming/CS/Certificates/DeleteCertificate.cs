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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<DeleteCertificateRequest>?       CustomDeleteCertificateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a delete certificate websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                     OnDeleteCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateRequestReceivedDelegate?     OnDeleteCertificateRequestReceived;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateDelegate?            OnDeleteCertificate;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateResponseSentDelegate?    OnDeleteCertificateResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a delete certificate request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?         OnDeleteCertificateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_DeleteCertificate(DateTime              RequestTimestamp,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id     DestinationNodeId,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      JObject               RequestJSON,
                                      CancellationToken     CancellationToken)

        {

            #region Send OnDeleteCertificateWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateWSRequest?.Invoke(startTime,
                                                     parentNetworkingNode,
                                                     WebSocketConnection,
                                                     DestinationNodeId,
                                                     NetworkPath,
                                                     EventTrackingId,
                                                     RequestTimestamp,
                                                     RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteCertificateWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (DeleteCertificateRequest.TryParse(RequestJSON,
                                                      RequestId,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomDeleteCertificateRequestParser)) {

                    #region Send OnDeleteCertificateRequest event

                    try
                    {

                        OnDeleteCertificateRequestReceived?.Invoke(Timestamp.Now,
                                                           parentNetworkingNode,
                                                           WebSocketConnection,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteCertificateRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    DeleteCertificateResponse? response = null;

                    var results = OnDeleteCertificate?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDeleteCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                   parentNetworkingNode,
                                                                                                                   WebSocketConnection,
                                                                                                                   request,
                                                                                                                   CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= DeleteCertificateResponse.Failed(request);

                    #endregion

                    #region Send OnDeleteCertificateResponse event

                    try
                    {

                        OnDeleteCertificateResponseSent?.Invoke(Timestamp.Now,
                                                            parentNetworkingNode,
                                                            WebSocketConnection,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteCertificateResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomDeleteCertificateResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_DeleteCertificate)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_DeleteCertificate)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnDeleteCertificateWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnDeleteCertificateWSResponse?.Invoke(endTime,
                                                      parentNetworkingNode,
                                                      WebSocketConnection,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      EventTrackingId,
                                                      RequestTimestamp,
                                                      RequestJSON,
                                                      OCPPResponse?.Payload,
                                                      OCPPErrorResponse?.ToJSON(),
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteCertificateWSResponse));
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
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateResponseSentDelegate? OnDeleteCertificateResponseSent;

    }

}
