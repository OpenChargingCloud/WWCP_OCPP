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

    #region OnSetDisplayMessage

    /// <summary>
    /// A SetDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetDisplayMessageRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           SetDisplayMessageRequest   Request);


    /// <summary>
    /// A SetDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetDisplayMessageResponse>

        OnSetDisplayMessageDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    SetDisplayMessageRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A SetDisplayMessage response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetDisplayMessageResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetDisplayMessageRequest    Request,
                                            SetDisplayMessageResponse   Response,
                                            TimeSpan                    Runtime);

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

        public CustomJObjectParserDelegate<SetDisplayMessageRequest>?       CustomSetDisplayMessageRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?  CustomSetDisplayMessageResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set display message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?     OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageDelegate?            OnSetDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?    OnSetDisplayMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set display message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetDisplayMessageWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SetDisplayMessage(DateTime                   RequestTimestamp,
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

            #region Send OnSetDisplayMessageWSRequest event

            try
            {

                OnSetDisplayMessageWSRequest?.Invoke(Timestamp.Now,
                                                     WebSocketConnection,
                                                     chargingStationId,
                                                     EventTrackingId,
                                                     requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageWSRequest));
            }

            #endregion

            try
            {

                if (SetDisplayMessageRequest.TryParse(requestJSON,
                                                      requestId,
                                                      ChargingStationIdentity,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomSetDisplayMessageRequestParser) && request is not null) {

                    #region Send OnSetDisplayMessageRequest event

                    try
                    {

                        OnSetDisplayMessageRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SetDisplayMessageResponse? response = null;

                    var results = OnSetDisplayMessage?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetDisplayMessageDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetDisplayMessageResponse.Failed(request);

                    #endregion

                    #region Send OnSetDisplayMessageResponse event

                    try
                    {

                        OnSetDisplayMessageResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomSetDisplayMessageResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "SetDisplayMessage",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "SetDisplayMessage",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnSetDisplayMessageWSResponse event

            try
            {

                OnSetDisplayMessageWSResponse?.Invoke(Timestamp.Now,
                                                      WebSocketConnection,
                                                      requestJSON,
                                                      OCPPResponse?.Message,
                                                      OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
