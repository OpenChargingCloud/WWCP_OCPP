/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

        public CustomJObjectParserDelegate<ChangeConfigurationRequest>?       CustomChangeConfigurationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ChangeConfiguration websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnChangeConfigurationWSRequest;

        /// <summary>
        /// An event sent whenever a ChangeConfiguration request was received.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate?         OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a ChangeConfiguration request was received.
        /// </summary>
        public event OnChangeConfigurationDelegate?                OnChangeConfiguration;

        /// <summary>
        /// An event sent whenever a response to a ChangeConfiguration request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate?        OnChangeConfigurationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a ChangeConfiguration request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnChangeConfigurationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ChangeConfiguration(DateTime                   RequestTimestamp,
                                        WebSocketClientConnection  WebSocketConnection,
                                        NetworkingNode_Id          DestinationNodeId,
                                        NetworkPath                NetworkPath,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    RequestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnChangeConfigurationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationWSRequest?.Invoke(startTime,
                                                       WebSocketConnection,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       EventTrackingId,
                                                       RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ChangeConfigurationRequest.TryParse(RequestJSON,
                                                        RequestId,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomChangeConfigurationRequestParser) && request is not null) {

                    #region Send OnChangeConfigurationRequest event

                    try
                    {

                        OnChangeConfigurationRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             WebSocketConnection,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ChangeConfigurationResponse? response = null;

                    var results = OnChangeConfiguration?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnChangeConfigurationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ChangeConfigurationResponse.Failed(request);

                    #endregion

                    #region Send OnChangeConfigurationResponse event

                    try
                    {

                        OnChangeConfigurationResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              WebSocketConnection,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomChangeConfigurationResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ChangeConfiguration)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ChangeConfiguration)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnChangeConfigurationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnChangeConfigurationWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnChangeConfigurationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
