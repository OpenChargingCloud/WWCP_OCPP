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

    #region OnTriggerMessage

    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnTriggerMessageRequestDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        TriggerMessageRequest   Request);


    /// <summary>
    /// A trigger message request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<TriggerMessageResponse>

        OnTriggerMessageDelegate(DateTime                    Timestamp,
                                 IEventSender                Sender,
                                 WebSocketClientConnection   Connection,
                                 TriggerMessageRequest       Request,
                                 CancellationToken           CancellationToken);


    /// <summary>
    /// A trigger message response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnTriggerMessageResponseDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         TriggerMessageRequest    Request,
                                         TriggerMessageResponse   Response,
                                         TimeSpan                 Runtime);

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

        public CustomJObjectParserDelegate<TriggerMessageRequest>?       CustomTriggerMessageRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a trigger message websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?           OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageDelegate?            OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a trigger message request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?          OnTriggerMessageWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_TriggerMessage(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   ChargingStation_Id         ChargingStationId,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnTriggerMessageWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  ChargingStationId,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (TriggerMessageRequest.TryParse(RequestJSON,
                                                   RequestId,
                                                   ChargingStationIdentity,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomTriggerMessageRequestParser) && request is not null) {

                    #region Send OnTriggerMessageRequest event

                    try
                    {

                        OnTriggerMessageRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    TriggerMessageResponse? response = null;

                    var results = OnTriggerMessage?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnTriggerMessageDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= TriggerMessageResponse.Failed(request);

                    #endregion

                    #region Send OnTriggerMessageResponse event

                    try
                    {

                        OnTriggerMessageResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomTriggerMessageResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_TriggerMessage)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_TriggerMessage)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnTriggerMessageWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnTriggerMessageWSResponse?.Invoke(endTime,
                                                   WebSocketConnection,
                                                   EventTrackingId,
                                                   RequestTimestamp,
                                                   RequestJSON,
                                                   OCPPResponse?.Payload,
                                                   OCPPErrorResponse?.ToJSON(),
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
