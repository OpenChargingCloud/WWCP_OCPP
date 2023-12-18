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

        public CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?       CustomExtendedTriggerMessageRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an ExtendedTriggerMessage websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnExtendedTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever an ExtendedTriggerMessage request was received.
        /// </summary>
        public event OnExtendedTriggerMessageRequestDelegate?      OnExtendedTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever an ExtendedTriggerMessage request was received.
        /// </summary>
        public event OnExtendedTriggerMessageDelegate?             OnExtendedTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event OnExtendedTriggerMessageResponseDelegate?     OnExtendedTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnExtendedTriggerMessageWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ExtendedTriggerMessage(DateTime                   RequestTimestamp,
                                           WebSocketClientConnection  WebSocketConnection,
                                           NetworkingNode_Id          DestinationNodeId,
                                           NetworkPath                NetworkPath,
                                           EventTracking_Id           EventTrackingId,
                                           Request_Id                 RequestId,
                                           JObject                    RequestJSON,
                                           CancellationToken          CancellationToken)

        {

            #region Send OnExtendedTriggerMessageWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageWSRequest?.Invoke(startTime,
                                                          WebSocketConnection,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnExtendedTriggerMessageWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ExtendedTriggerMessageRequest.TryParse(RequestJSON,
                                                           RequestId,
                                                           DestinationNodeId,
                                                           NetworkPath,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomExtendedTriggerMessageRequestParser) && request is not null) {

                    #region Send OnExtendedTriggerMessageRequest event

                    try
                    {

                        OnExtendedTriggerMessageRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                WebSocketConnection,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnExtendedTriggerMessageRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ExtendedTriggerMessageResponse? response = null;

                    var results = OnExtendedTriggerMessage?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnExtendedTriggerMessageDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ExtendedTriggerMessageResponse.Failed(request);

                    #endregion

                    #region Send OnExtendedTriggerMessageResponse event

                    try
                    {

                        OnExtendedTriggerMessageResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 WebSocketConnection,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnExtendedTriggerMessageResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       NetworkPath.Source,
                                       RequestId,
                                       response.ToJSON(
                                           CustomExtendedTriggerMessageResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ExtendedTriggerMessage)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ExtendedTriggerMessage)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnExtendedTriggerMessageWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnExtendedTriggerMessageWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnExtendedTriggerMessageWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
