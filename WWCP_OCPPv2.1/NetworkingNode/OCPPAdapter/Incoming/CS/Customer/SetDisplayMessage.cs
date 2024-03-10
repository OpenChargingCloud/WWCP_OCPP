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

        public CustomJObjectParserDelegate<SetDisplayMessageRequest>?       CustomSetDisplayMessageRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?  CustomSetDisplayMessageResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set display message websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                      OnSetDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageRequestReceivedDelegate?     OnSetDisplayMessageRequestReceived;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageDelegate?            OnSetDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageResponseSentDelegate?    OnSetDisplayMessageResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a set display message request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?          OnSetDisplayMessageWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_SetDisplayMessage(DateTime                   RequestTimestamp,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id          DestinationNodeId,
                                      NetworkPath                NetworkPath,
                                      EventTracking_Id           EventTrackingId,
                                      Request_Id                 RequestId,
                                      JObject                    RequestJSON,
                                      CancellationToken          CancellationToken)

        {

            #region Send OnSetDisplayMessageWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDisplayMessageWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetDisplayMessageWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SetDisplayMessageRequest.TryParse(RequestJSON,
                                                      RequestId,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomSetDisplayMessageRequestParser)) {

                    #region Send OnSetDisplayMessageRequest event

                    try
                    {

                        OnSetDisplayMessageRequestReceived?.Invoke(Timestamp.Now,
                                                           parentNetworkingNode,
                                                           WebSocketConnection,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetDisplayMessageRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    SetDisplayMessageResponse? response = null;

                    var results = OnSetDisplayMessage?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetDisplayMessageDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetDisplayMessageResponse.Failed(request);

                    #endregion

                    #region Send OnSetDisplayMessageResponse event

                    try
                    {

                        OnSetDisplayMessageResponseSent?.Invoke(Timestamp.Now,
                                                            parentNetworkingNode,
                                                            WebSocketConnection,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetDisplayMessageResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomSetDisplayMessageResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SetDisplayMessage)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SetDisplayMessage)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnSetDisplayMessageWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSetDisplayMessageWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetDisplayMessageWSResponse));
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
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageResponseSentDelegate? OnSetDisplayMessageResponseSent;

    }

}
