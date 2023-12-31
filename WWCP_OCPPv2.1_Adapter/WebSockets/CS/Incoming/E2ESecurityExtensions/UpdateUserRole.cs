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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : AOCPPWebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeIncomingMessages,
                                                  INetworkingNodeOutgoingMessageEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateUserRole websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnUpdateUserRoleWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        public event OCPP.CS.OnUpdateUserRoleRequestDelegate?      OnUpdateUserRoleRequest;

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        public event OCPP.CS.OnUpdateUserRoleDelegate?             OnUpdateUserRole;

        /// <summary>
        /// An event sent whenever a response to an UpdateUserRole request was sent.
        /// </summary>
        public event OCPP.CS.OnUpdateUserRoleResponseDelegate?     OnUpdateUserRoleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateUserRole request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnUpdateUserRoleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UpdateUserRole(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   NetworkingNode_Id          DestinationNodeId,
                                   NetworkPath                NetworkPath,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnUpdateUserRoleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  DestinationNodeId,
                                                  NetworkPath,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateUserRoleWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (UpdateUserRoleRequest.TryParse(RequestJSON,
                                                   RequestId,
                                                   DestinationNodeId,
                                                   NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomUpdateUserRoleRequestParser) && request is not null) {

                    #region Send OnUpdateUserRoleRequest event

                    try
                    {

                        OnUpdateUserRoleRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        WebSocketConnection,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateUserRoleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateUserRoleResponse? response = null;

                    var results = OnUpdateUserRole?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateUserRoleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateUserRoleResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateUserRoleResponse event

                    try
                    {

                        OnUpdateUserRoleResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         WebSocketConnection,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateUserRoleResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UpdateUserRole)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UpdateUserRole)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateUserRoleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUpdateUserRoleWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateUserRoleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
