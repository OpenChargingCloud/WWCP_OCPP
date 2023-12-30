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

        public CustomJObjectParserDelegate<GetLocalListVersionRequest>?       CustomGetLocalListVersionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                        OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLocalListVersionRequestDelegate?     OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLocalListVersionDelegate?            OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLocalListVersionResponseDelegate?    OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?            OnGetLocalListVersionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetLocalListVersion(DateTime                   RequestTimestamp,
                                        IWebSocketConnection  WebSocketConnection,
                                        NetworkingNode_Id          DestinationNodeId,
                                        NetworkPath                NetworkPath,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    RequestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnGetLocalListVersionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetLocalListVersionWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetLocalListVersionRequest.TryParse(RequestJSON,
                                                        RequestId,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomGetLocalListVersionRequestParser) && request is not null) {

                    #region Send OnGetLocalListVersionRequest event

                    try
                    {

                        OnGetLocalListVersionRequest?.Invoke(Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetLocalListVersionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetLocalListVersionResponse? response = null;

                    var results = OnGetLocalListVersion?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetLocalListVersionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetLocalListVersionResponse.Failed(request);

                    #endregion

                    #region Send OnGetLocalListVersionResponse event

                    try
                    {

                        OnGetLocalListVersionResponse?.Invoke(Timestamp.Now,
                                                              parentNetworkingNode,
                                                              WebSocketConnection,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetLocalListVersionResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetLocalListVersionResponseSerializer,
                                           parentNetworkingNode.CustomSignatureSerializer,
                                           parentNetworkingNode.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetLocalListVersion)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetLocalListVersion)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetLocalListVersionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetLocalListVersionWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetLocalListVersionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
