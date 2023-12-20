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
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<UpdateFirmwareRequest>?       CustomUpdateFirmwareRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateFirmware websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?              OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?                     OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?             OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateFirmware request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnUpdateFirmwareWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UpdateFirmware(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   NetworkingNode_Id          DestinationNodeId,
                                   NetworkPath                NetworkPath,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnUpdateFirmwareWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  DestinationNodeId,
                                                  NetworkPath,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (UpdateFirmwareRequest.TryParse(RequestJSON,
                                                   RequestId,
                                                   DestinationNodeId,
                                                   NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomUpdateFirmwareRequestParser) && request is not null) {

                    #region Send OnUpdateFirmwareRequest event

                    try
                    {

                        OnUpdateFirmwareRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        WebSocketConnection,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateFirmwareResponse? response = null;

                    var results = OnUpdateFirmware?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateFirmwareDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateFirmwareResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateFirmwareResponse event

                    try
                    {

                        OnUpdateFirmwareResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         WebSocketConnection,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomUpdateFirmwareResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UpdateFirmware)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UpdateFirmware)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateFirmwareWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUpdateFirmwareWSResponse?.Invoke(endTime,
                                                   WebSocketConnection,
                                                   DestinationNodeId,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
