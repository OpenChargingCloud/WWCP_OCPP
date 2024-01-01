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

        public CustomJObjectParserDelegate<AFRRSignalRequest>?       CustomAFRRSignalRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<AFRRSignalResponse>?  CustomAFRRSignalResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an AFRRSignal websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnAFRRSignalWSRequest;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalRequestReceivedDelegate?      OnAFRRSignalRequestReceived;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalDelegate?             OnAFRRSignal;

        /// <summary>
        /// An event sent whenever a response to an AFRRSignal request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalResponseSentDelegate?     OnAFRRSignalResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to an AFRRSignal request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnAFRRSignalWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_AFRRSignal(DateTime                   RequestTimestamp,
                               IWebSocketConnection  WebSocketConnection,
                               NetworkingNode_Id          DestinationNodeId,
                               NetworkPath                NetworkPath,
                               EventTracking_Id           EventTrackingId,
                               Request_Id                 RequestId,
                               JObject                    RequestJSON,
                               CancellationToken          CancellationToken)

        {

            #region Send OnAFRRSignalWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAFRRSignalWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAFRRSignalWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (AFRRSignalRequest.TryParse(RequestJSON,
                                               RequestId,
                                               DestinationNodeId,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               CustomAFRRSignalRequestParser) && request is not null) {

                    #region Send OnAFRRSignalRequest event

                    try
                    {

                        OnAFRRSignalRequestReceived?.Invoke(Timestamp.Now,
                                                    parentNetworkingNode,
                                                    WebSocketConnection,
                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAFRRSignalRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    AFRRSignalResponse? response = null;

                    var results = OnAFRRSignal?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnAFRRSignalDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= AFRRSignalResponse.Failed(request);

                    #endregion

                    #region Send OnAFRRSignalResponse event

                    try
                    {

                        OnAFRRSignalResponseSent?.Invoke(Timestamp.Now,
                                                     parentNetworkingNode,
                                                     WebSocketConnection,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAFRRSignalResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomAFRRSignalResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_AFRRSignal)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_AFRRSignal)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnAFRRSignalWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnAFRRSignalWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAFRRSignalWSResponse));
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
        /// An event sent whenever a response to an AFRRSignal request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalResponseSentDelegate? OnAFRRSignalResponseSent;

    }

}
