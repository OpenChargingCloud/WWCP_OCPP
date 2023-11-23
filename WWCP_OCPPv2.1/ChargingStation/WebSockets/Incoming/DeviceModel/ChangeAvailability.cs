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

    #region OnChangeAvailability

    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnChangeAvailabilityRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            ChangeAvailabilityRequest   Request);


    /// <summary>
    /// A change availability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeAvailabilityResponse>

        OnChangeAvailabilityDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     ChangeAvailabilityRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A change availability response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnChangeAvailabilityResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             ChangeAvailabilityRequest    Request,
                                             ChangeAvailabilityResponse   Response,
                                             TimeSpan                     Runtime);

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

        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>?       CustomChangeAvailabilityRequestParser    { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a change availability websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate?            OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a change availability request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnChangeAvailabilityWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_ChangeAvailability(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       ChargingStation_Id         chargingStationId,
                                       EventTracking_Id           EventTrackingId,
                                       String                     requestText,
                                       Request_Id                 requestId,
                                       JObject                    requestJSON,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnChangeAvailabilityWSRequest event

            try
            {

                OnChangeAvailabilityWSRequest?.Invoke(Timestamp.Now,
                                                      WebSocketConnection,
                                                      chargingStationId,
                                                      EventTrackingId,
                                                      requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (ChangeAvailabilityRequest.TryParse(requestJSON,
                                                       requestId,
                                                       ChargingStationIdentity,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomChangeAvailabilityRequestParser) && request is not null) {

                    #region Send OnChangeAvailabilityRequest event

                    try
                    {

                        OnChangeAvailabilityRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ChangeAvailabilityResponse? response = null;

                    var results = OnChangeAvailability?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ChangeAvailabilityResponse.Failed(request);

                    #endregion

                    #region Send OnChangeAvailabilityResponse event

                    try
                    {

                        OnChangeAvailabilityResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomChangeAvailabilityResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_ChangeAvailability)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_ChangeAvailability)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnChangeAvailabilityWSResponse event

            try
            {

                OnChangeAvailabilityWSResponse?.Invoke(Timestamp.Now,
                                                       WebSocketConnection,
                                                       requestJSON,
                                                       OCPPResponse?.Message,
                                                       OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
