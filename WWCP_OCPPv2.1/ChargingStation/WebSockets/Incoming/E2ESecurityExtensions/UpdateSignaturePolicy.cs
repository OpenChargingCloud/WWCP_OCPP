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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnUpdateSignaturePolicy

    /// <summary>
    /// An UpdateSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateSignaturePolicyRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               UpdateSignaturePolicyRequest   Request);


    /// <summary>
    /// An UpdateSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateSignaturePolicyResponse>

        OnUpdateSignaturePolicyDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        WebSocketClientConnection      Connection,
                                        UpdateSignaturePolicyRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// An UpdateSignaturePolicy response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateSignaturePolicyResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                UpdateSignaturePolicyRequest    Request,
                                                UpdateSignaturePolicyResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<UpdateSignaturePolicyRequest>?  CustomUpdateSignaturePolicyRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnUpdateSignaturePolicyWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?     OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyDelegate?            OnUpdateSignaturePolicy;

        /// <summary>
        /// An event sent whenever a response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?    OnUpdateSignaturePolicyResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnUpdateSignaturePolicyWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_UpdateSignaturePolicy(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          ChargingStation_Id         chargingStationId,
                                          EventTracking_Id           EventTrackingId,
                                          String                     requestText,
                                          Request_Id                 requestId,
                                          JObject                    requestJSON,
                                          CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnUpdateSignaturePolicyWSRequest event

            try
            {

                OnUpdateSignaturePolicyWSRequest?.Invoke(Timestamp.Now,
                                                         WebSocketConnection,
                                                         chargingStationId,
                                                         EventTrackingId,
                                                         requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyWSRequest));
            }

            #endregion

            try
            {

                if (UpdateSignaturePolicyRequest.TryParse(requestJSON,
                                                          requestId,
                                                          ChargingStationIdentity,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomUpdateSignaturePolicyRequestParser) && request is not null) {

                    #region Send OnUpdateSignaturePolicyRequest event

                    try
                    {

                        OnUpdateSignaturePolicyRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateSignaturePolicyResponse? response = null;

                    var results = OnUpdateSignaturePolicy?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateSignaturePolicyDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateSignaturePolicyResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateSignaturePolicyResponse event

                    try
                    {

                        OnUpdateSignaturePolicyResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "UpdateSignaturePolicy",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "UpdateSignaturePolicy",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateSignaturePolicyWSResponse event

            try
            {

                OnUpdateSignaturePolicyWSResponse?.Invoke(Timestamp.Now,
                                                          WebSocketConnection,
                                                          requestJSON,
                                                          OCPPResponse?.Message,
                                                          OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
