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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnClearCache

    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnClearCacheRequestDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    ClearCacheRequest   Request);


    /// <summary>
    /// A clear cache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearCacheResponse>

        OnClearCacheDelegate(DateTime                    Timestamp,
                             IEventSender                Sender,
                             WebSocketClientConnection   Connection,
                             ClearCacheRequest           Request,
                             CancellationToken           CancellationToken);


    /// <summary>
    /// A clear cache response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnClearCacheResponseDelegate(DateTime             Timestamp,
                                     IEventSender         Sender,
                                     ClearCacheRequest    Request,
                                     ClearCacheResponse   Response,
                                     TimeSpan             Runtime);

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

        public CustomJObjectParserDelegate<ClearCacheRequest>?       CustomClearCacheRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ClearCacheResponse>?  CustomClearCacheResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate?            OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?    OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnClearCacheWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_ClearCache(DateTime                   RequestTimestamp,
                               WebSocketClientConnection  WebSocketConnection,
                               ChargingStation_Id         chargingStationId,
                               EventTracking_Id           EventTrackingId,
                               String                     requestText,
                               Request_Id                 requestId,
                               JObject                    requestJSON,
                               CancellationToken          CancellationToken)

        {

            #region Send OnClearCacheWSRequest event

            try
            {

                OnClearCacheWSRequest?.Invoke(Timestamp.Now,
                                              WebSocketConnection,
                                              chargingStationId,
                                              EventTrackingId,
                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (ClearCacheRequest.TryParse(requestJSON,
                                               requestId,
                                               ChargingStationIdentity,
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
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheRequest));
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
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomClearCacheResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_ClearCache)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_ClearCache)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnClearCacheWSResponse event

            try
            {

                OnClearCacheWSResponse?.Invoke(Timestamp.Now,
                                               WebSocketConnection,
                                               requestJSON,
                                               OCPPResponse?.Message,
                                               OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
