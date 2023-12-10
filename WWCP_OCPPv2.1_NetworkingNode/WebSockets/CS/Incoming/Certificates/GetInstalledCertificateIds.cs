﻿/*
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?       CustomGetInstalledCertificateIdsRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get installed certificate ids websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                       OnGetInstalledCertificateIdsWSRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsRequestDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsDelegate?            OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsResponseDelegate?    OnGetInstalledCertificateIdsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get installed certificate ids request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                      OnGetInstalledCertificateIdsWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetInstalledCertificateIds(DateTime                   RequestTimestamp,
                                               WebSocketClientConnection  WebSocketConnection,
                                               NetworkingNode_Id          NetworkingNodeId,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               JObject                    RequestJSON,
                                               CancellationToken          CancellationToken)

        {

            #region Send OnGetInstalledCertificateIdsWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsWSRequest?.Invoke(startTime,
                                                              WebSocketConnection,
                                                              NetworkingNodeId,
                                                              NetworkPath,
                                                              EventTrackingId,
                                                              RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetInstalledCertificateIdsRequest.TryParse(RequestJSON,
                                                               RequestId,
                                                               NetworkingNodeId,
                                                               NetworkPath,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomGetInstalledCertificateIdsRequestParser) && request is not null) {

                    #region Send OnGetInstalledCertificateIdsRequest event

                    try
                    {

                        OnGetInstalledCertificateIdsRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetInstalledCertificateIdsRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetInstalledCertificateIdsResponse? response = null;

                    var results = OnGetInstalledCertificateIds?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetInstalledCertificateIdsDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            WebSocketConnection,
                                                                                                                            request,
                                                                                                                            CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= GetInstalledCertificateIdsResponse.Failed(request);

                    #endregion

                    #region Send OnGetInstalledCertificateIdsResponse event

                    try
                    {

                        OnGetInstalledCertificateIdsResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetInstalledCertificateIdsResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetInstalledCertificateIdsResponseSerializer,
                                           CustomCertificateHashDataSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetInstalledCertificateIds)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetInstalledCertificateIds)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetInstalledCertificateIdsWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetInstalledCertificateIdsWSResponse?.Invoke(endTime,
                                                               WebSocketConnection,
                                                               NetworkingNodeId,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}