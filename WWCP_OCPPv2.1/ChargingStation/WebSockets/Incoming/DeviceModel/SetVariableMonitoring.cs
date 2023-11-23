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

    #region OnSetVariableMonitoring

    /// <summary>
    /// A set variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetVariableMonitoringRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               SetVariableMonitoringRequest   Request);


    /// <summary>
    /// A set variable monitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetVariableMonitoringResponse>

        OnSetVariableMonitoringDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        WebSocketClientConnection      Connection,
                                        SetVariableMonitoringRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A set variable monitoring response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetVariableMonitoringResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                SetVariableMonitoringRequest    Request,
                                                SetVariableMonitoringResponse   Response,
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

        public CustomJObjectParserDelegate<SetVariableMonitoringRequest>?       CustomSetVariableMonitoringRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?  CustomSetVariableMonitoringResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set variable monitoring websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnSetVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?     OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringDelegate?            OnSetVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?    OnSetVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variable monitoring request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnSetVariableMonitoringWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SetVariableMonitoring(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          ChargingStation_Id         chargingStationId,
                                          EventTracking_Id           EventTrackingId,
                                          String                     requestText,
                                          Request_Id                 requestId,
                                          JObject                    requestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnSetVariableMonitoringWSRequest event

            try
            {

                OnSetVariableMonitoringWSRequest?.Invoke(Timestamp.Now,
                                                         WebSocketConnection,
                                                         chargingStationId,
                                                         EventTrackingId,
                                                         requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (SetVariableMonitoringRequest.TryParse(requestJSON,
                                                          requestId,
                                                          ChargingStationIdentity,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomSetVariableMonitoringRequestParser) && request is not null) {

                    #region Send OnSetVariableMonitoringRequest event

                    try
                    {

                        OnSetVariableMonitoringRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SetVariableMonitoringResponse? response = null;

                    var results = OnSetVariableMonitoring?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetVariableMonitoringDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetVariableMonitoringResponse.Failed(request);

                    #endregion

                    #region Send OnSetVariableMonitoringResponse event

                    try
                    {

                        OnSetVariableMonitoringResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomSetVariableMonitoringResponseSerializer,
                                           CustomSetMonitoringResultSerializer,
                                           CustomComponentSerializer,
                                           CustomEVSESerializer,
                                           CustomVariableSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_SetVariableMonitoring)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_SetVariableMonitoring)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnSetVariableMonitoringWSResponse event

            try
            {

                OnSetVariableMonitoringWSResponse?.Invoke(Timestamp.Now,
                                                          WebSocketConnection,
                                                          requestJSON,
                                                          OCPPResponse?.Message,
                                                          OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
