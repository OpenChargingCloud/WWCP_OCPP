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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyDisplayMessagesRequest>?       CustomNotifyDisplayMessagesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                           OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesRequestReceivedDelegate?     OnNotifyDisplayMessagesRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesDelegate?            OnNotifyDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a NotifyDisplayMessages was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesResponseSentDelegate?    OnNotifyDisplayMessagesResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyDisplayMessages was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?               OnNotifyDisplayMessagesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_NotifyDisplayMessages(DateTime                   RequestTimestamp,
                                          IWebSocketConnection  WebSocketConnection,
                                          NetworkingNode_Id          DestinationNodeId,
                                          NetworkPath                NetworkPath,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    JSONRequest,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnNotifyDisplayMessagesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesWSRequest?.Invoke(startTime,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         DestinationNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestTimestamp,
                                                         JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyDisplayMessagesWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyDisplayMessagesRequest.TryParse(JSONRequest,
                                                          RequestId,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomNotifyDisplayMessagesRequestParser)) {

                    #region Send OnNotifyDisplayMessagesRequest event

                    try
                    {

                        OnNotifyDisplayMessagesRequestReceived?.Invoke(Timestamp.Now,
                                                               parentNetworkingNode,
                                                               WebSocketConnection,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyDisplayMessagesRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyDisplayMessagesResponse? response = null;

                    var responseTasks = OnNotifyDisplayMessages?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyDisplayMessagesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             parentNetworkingNode,
                                                                                                                             WebSocketConnection,
                                                                                                                             request,
                                                                                                                             CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyDisplayMessagesResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyDisplayMessagesResponse event

                    try
                    {

                        OnNotifyDisplayMessagesResponseSent?.Invoke(Timestamp.Now,
                                                                parentNetworkingNode,
                                                                WebSocketConnection,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyDisplayMessagesResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomNotifyDisplayMessagesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_NotifyDisplayMessages)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_NotifyDisplayMessages)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnNotifyDisplayMessagesWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnNotifyDisplayMessagesWSResponse?.Invoke(endTime,
                                                          parentNetworkingNode,
                                                          WebSocketConnection,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          JSONRequest,
                                                          OCPPResponse?.Payload,
                                                          OCPPErrorResponse?.ToJSON(),
                                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnNotifyDisplayMessagesWSResponse));
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
        /// An event sent whenever a response to a NotifyDisplayMessages was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesResponseSentDelegate? OnNotifyDisplayMessagesResponseSent;

    }

}
