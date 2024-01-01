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

        public CustomJObjectParserDelegate<AuthorizeRequest>?       CustomAuthorizeRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an Authorize WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?               OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeRequestReceivedDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeDelegate?            OnAuthorize;

        /// <summary>
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeResponseSentDelegate?    OnAuthorizeResponseSent;

        /// <summary>
        /// An event sent whenever an Authorize WebSocket response was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?   OnAuthorizeWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_Authorize(DateTime                   RequestTimestamp,
                              IWebSocketConnection  WebSocketConnection,
                              NetworkingNode_Id          DestinationNodeId,
                              NetworkPath                NetworkPath,
                              EventTracking_Id           EventTrackingId,
                              Request_Id                 RequestId,
                              JObject                    JSONRequest,
                              CancellationToken          CancellationToken)

        {

            #region Send OnAuthorizeWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (AuthorizeRequest.TryParse(JSONRequest,
                                              RequestId,
                                              DestinationNodeId,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              CustomAuthorizeRequestParser) && request is not null) {

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                   parentNetworkingNode,
                                                   WebSocketConnection,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    AuthorizeResponse? response = null;

                    var responseTasks = OnAuthorize?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= AuthorizeResponse.Failed(request);

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        OnAuthorizeResponseSent?.Invoke(Timestamp.Now,
                                                    parentNetworkingNode,
                                                    WebSocketConnection,
                                                    request,
                                                    response,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomAuthorizeResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                           parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                           parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_Authorize)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_Authorize)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnAuthorizeWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnAuthorizeWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeWSResponse));
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
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeResponseSentDelegate? OnAuthorizeResponseSent;

    }

}
