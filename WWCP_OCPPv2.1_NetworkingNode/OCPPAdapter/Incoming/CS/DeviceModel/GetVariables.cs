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

        public CustomJObjectParserDelegate<GetVariablesRequest>?       CustomGetVariablesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetVariablesResponse>?  CustomGetVariablesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get variables websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                 OnGetVariablesWSRequest;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesRequestReceivedDelegate?     OnGetVariablesRequestReceived;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesDelegate?            OnGetVariables;

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesResponseSentDelegate?    OnGetVariablesResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a get variables request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?     OnGetVariablesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetVariables(DateTime                   RequestTimestamp,
                                 IWebSocketConnection  WebSocketConnection,
                                 NetworkingNode_Id          DestinationNodeId,
                                 NetworkPath                NetworkPath,
                                 EventTracking_Id           EventTrackingId,
                                 Request_Id                 RequestId,
                                 JObject                    RequestJSON,
                                 CancellationToken          CancellationToken)

        {

            #region Send OnGetVariablesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetVariablesWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetVariablesWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetVariablesRequest.TryParse(RequestJSON,
                                                 RequestId,
                                                 DestinationNodeId,
                                                 NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 CustomGetVariablesRequestParser)) {

                    #region Send OnGetVariablesRequest event

                    try
                    {

                        OnGetVariablesRequestReceived?.Invoke(Timestamp.Now,
                                                      parentNetworkingNode,
                                                      WebSocketConnection,
                                                      request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetVariablesRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    GetVariablesResponse? response = null;

                    var results = OnGetVariables?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetVariablesDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetVariablesResponse.Failed(request);

                    #endregion

                    #region Send OnGetVariablesResponse event

                    try
                    {

                        OnGetVariablesResponseSent?.Invoke(Timestamp.Now,
                                                       parentNetworkingNode,
                                                       WebSocketConnection,
                                                       request,
                                                       response,
                                                       response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetVariablesResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetVariablesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomGetVariableResultSerializer,
                                           parentNetworkingNode.OCPP.CustomComponentSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSESerializer,
                                           parentNetworkingNode.OCPP.CustomVariableSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetVariables)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetVariables)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetVariablesWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetVariablesWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetVariablesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesResponseSentDelegate? OnGetVariablesResponseSent;

    }

}
