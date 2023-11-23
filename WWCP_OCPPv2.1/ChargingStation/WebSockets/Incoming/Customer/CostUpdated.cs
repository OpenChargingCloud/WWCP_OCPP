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

    #region OnCostUpdated

    /// <summary>
    /// A CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCostUpdatedRequestDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     CostUpdatedRequest   Request);


    /// <summary>
    /// A CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CostUpdatedResponse>

        OnCostUpdatedDelegate(DateTime                    Timestamp,
                              IEventSender                Sender,
                              WebSocketClientConnection   Connection,
                              CostUpdatedRequest          Request,
                              CancellationToken           CancellationToken);


    /// <summary>
    /// A CostUpdated response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCostUpdatedResponseDelegate(DateTime              Timestamp,
                                      IEventSender          Sender,
                                      CostUpdatedRequest    Request,
                                      CostUpdatedResponse   Response,
                                      TimeSpan              Runtime);

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

        public CustomJObjectParserDelegate<CostUpdatedRequest>?       CustomCostUpdatedRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a cost updated websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?        OnCostUpdatedWSRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?     OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedDelegate?            OnCostUpdated;

        /// <summary>
        /// An event sent whenever a response to a cost updated request was sent.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?    OnCostUpdatedResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cost updated request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?       OnCostUpdatedWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_CostUpdated(DateTime                   RequestTimestamp,
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

            #region Send OnCostUpdatedWSRequest event

            try
            {

                OnCostUpdatedWSRequest?.Invoke(Timestamp.Now,
                                               WebSocketConnection,
                                               chargingStationId,
                                               EventTrackingId,
                                               requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedWSRequest));
            }

            #endregion

            try
            {

                if (CostUpdatedRequest.TryParse(requestJSON,
                                                requestId,
                                                ChargingStationIdentity,
                                                out var request,
                                                out var errorResponse,
                                                CustomCostUpdatedRequestParser) && request is not null) {

                    #region Send OnCostUpdatedRequest event

                    try
                    {

                        OnCostUpdatedRequest?.Invoke(Timestamp.Now,
                                                     this,
                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    CostUpdatedResponse? response = null;

                    var results = OnCostUpdated?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnCostUpdatedDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= CostUpdatedResponse.Failed(request);

                    #endregion

                    #region Send OnCostUpdatedResponse event

                    try
                    {

                        OnCostUpdatedResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      request,
                                                      response,
                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomCostUpdatedResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "CostUpdated",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "CostUpdated",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnCostUpdatedWSResponse event

            try
            {

                OnCostUpdatedWSResponse?.Invoke(Timestamp.Now,
                                                WebSocketConnection,
                                                requestJSON,
                                                OCPPResponse?.Message,
                                                OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
