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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ResetRequest>?       CustomResetRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ResetResponse>?  CustomResetResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnResetRequestReceivedDelegate?            OnResetRequestReceived;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnResetDelegate?                   OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnResetResponseSentDelegate?           OnResetResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnResetWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_Reset(DateTime              RequestTimestamp,
                          IWebSocketConnection  WebSocketConnection,
                          NetworkingNode_Id     DestinationNodeId,
                          NetworkPath           NetworkPath,
                          EventTracking_Id      EventTrackingId,
                          Request_Id            RequestId,
                          JObject               RequestJSON,
                          CancellationToken     CancellationToken)

        {

            #region Send OnResetWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetWSRequest?.Invoke(startTime,
                                         parentNetworkingNode,
                                         WebSocketConnection,
                                         DestinationNodeId,
                                         NetworkPath,
                                         EventTrackingId,
                                         RequestTimestamp,
                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ResetRequest.TryParse(RequestJSON,
                                          RequestId,
                                          DestinationNodeId,
                                          NetworkPath,
                                          out var request,
                                          out var errorResponse,
                                          CustomResetRequestParser) && request is not null) {

                    #region Send OnResetRequest event

                    try
                    {

                        OnResetRequestReceived?.Invoke(Timestamp.Now,
                                               parentNetworkingNode,
                                               WebSocketConnection,
                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    ResetResponse? response = null;

                    var results = OnReset?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnResetDelegate)?.Invoke(Timestamp.Now,
                                                                                                       parentNetworkingNode,
                                                                                                       WebSocketConnection,
                                                                                                       request,
                                                                                                       CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= ResetResponse.Failed(request);

                    #endregion

                    #region Send OnResetResponse event

                    try
                    {

                        OnResetResponseSent?.Invoke(Timestamp.Now,
                                                parentNetworkingNode,
                                                WebSocketConnection,
                                                request,
                                                response,
                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomResetResponseSerializer,
                                           parentNetworkingNode.CustomStatusInfoSerializer,
                                           parentNetworkingNode.CustomSignatureSerializer,
                                           parentNetworkingNode.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_Reset)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_Reset)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnResetWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnResetWSResponse?.Invoke(endTime,
                                          parentNetworkingNode,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnResetResponseSentDelegate? OnResetResponseSent;

    }

}
