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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : AChargingStationWSClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<CancelReservationRequest>?       CustomCancelReservationRequestParser         { get; set; }
        public CustomJObjectSerializerDelegate<CancelReservationResponse>?  CustomCancelReservationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a CancelReservation websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnCancelReservationWSRequest;

        /// <summary>
        /// An event sent whenever a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?           OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a CancelReservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate?                  OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a CancelReservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?          OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a CancelReservation request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnCancelReservationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_CancelReservation(DateTime                   RequestTimestamp,
                                      WebSocketClientConnection  WebSocketConnection,
                                      NetworkingNode_Id          NetworkingNodeId,
                                      NetworkPath                NetworkPath,
                                      EventTracking_Id           EventTrackingId,
                                      Request_Id                 RequestId,
                                      JObject                    RequestJSON,
                                      CancellationToken          CancellationToken)

        {

            #region Send OnCancelReservationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationWSRequest?.Invoke(startTime,
                                                     WebSocketConnection,
                                                     NetworkingNodeId,
                                                     NetworkPath,
                                                     EventTrackingId,
                                                     RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (CancelReservationRequest.TryParse(RequestJSON,
                                                      RequestId,
                                                      NetworkingNodeId,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomCancelReservationRequestParser) && request is not null) {

                    #region Send OnCancelReservationRequest event

                    try
                    {

                        OnCancelReservationRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           WebSocketConnection,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    CancelReservationResponse? response = null;

                    var results = OnCancelReservation?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= CancelReservationResponse.Failed(request);

                    #endregion

                    #region Send OnCancelReservationResponse event

                    try
                    {

                        OnCancelReservationResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            WebSocketConnection,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomCancelReservationResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_CancelReservation)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_CancelReservation)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnCancelReservationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnCancelReservationWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnCancelReservationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
