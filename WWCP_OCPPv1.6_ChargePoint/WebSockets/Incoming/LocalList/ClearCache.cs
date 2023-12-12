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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : WebSocketClient,
                                               IChargePointWebSocketClient,
                                               IChargePointServer,
                                               IChargePointClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ClearCacheRequest>?       CustomClearCacheRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ClearCacheResponse>?  CustomClearCacheResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ClearCache websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a ClearCache request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?                  OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a ClearCache request was received.
        /// </summary>
        public event OnClearCacheDelegate?                         OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?                 OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a ClearCache request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnClearCacheWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ClearCache(DateTime                   RequestTimestamp,
                               WebSocketClientConnection  WebSocketConnection,
                               NetworkingNode_Id          NetworkingNodeId,
                               NetworkPath                NetworkPath,
                               EventTracking_Id           EventTrackingId,
                               Request_Id                 RequestId,
                               JObject                    RequestJSON,
                               CancellationToken          CancellationToken)

        {

            #region Send OnClearCacheWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheWSRequest?.Invoke(startTime,
                                              WebSocketConnection,
                                              NetworkingNodeId,
                                              NetworkPath,
                                              EventTrackingId,
                                              RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ClearCacheRequest.TryParse(RequestJSON,
                                               RequestId,
                                               NetworkingNodeId,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               CustomClearCacheRequestParser) && request is not null) {

                    #region Send OnClearCacheRequest event

                    try
                    {

                        OnClearCacheRequest?.Invoke(Timestamp.Now,
                                                    this,
                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ClearCacheResponse? response = null;

                    var results = OnClearCache?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnClearCacheDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ClearCacheResponse.Failed(request);

                    #endregion

                    #region Send OnClearCacheResponse event

                    try
                    {

                        OnClearCacheResponse?.Invoke(Timestamp.Now,
                                                     this,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomClearCacheResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ClearCache)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ClearCache)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnClearCacheWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnClearCacheWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnClearCacheWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}