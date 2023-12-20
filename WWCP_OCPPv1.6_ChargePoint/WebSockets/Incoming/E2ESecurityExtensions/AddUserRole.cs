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

        public CustomJObjectParserDelegate<AddUserRoleRequest>?       CustomAddUserRoleRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<AddUserRoleResponse>?  CustomAddUserRoleResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an AddUserRole websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnAddUserRoleWSRequest;

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        public event OCPP.CS.OnAddUserRoleRequestDelegate?         OnAddUserRoleRequest;

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        public event OCPP.CS.OnAddUserRoleDelegate?                OnAddUserRole;

        /// <summary>
        /// An event sent whenever a response to an AddUserRole request was sent.
        /// </summary>
        public event OCPP.CS.OnAddUserRoleResponseDelegate?        OnAddUserRoleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an AddUserRole request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnAddUserRoleWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_AddUserRole(DateTime                   RequestTimestamp,
                                WebSocketClientConnection  WebSocketConnection,
                                NetworkingNode_Id          DestinationNodeId,
                                NetworkPath                NetworkPath,
                                EventTracking_Id           EventTrackingId,
                                Request_Id                 RequestId,
                                JObject                    RequestJSON,
                                CancellationToken          CancellationToken)

        {

            #region Send OnAddUserRoleWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddUserRoleWSRequest?.Invoke(startTime,
                                               WebSocketConnection,
                                               DestinationNodeId,
                                               NetworkPath,
                                               EventTrackingId,
                                               RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAddUserRoleWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (AddUserRoleRequest.TryParse(RequestJSON,
                                                RequestId,
                                                DestinationNodeId,
                                                NetworkPath,
                                                out var request,
                                                out var errorResponse,
                                                CustomAddUserRoleRequestParser) && request is not null) {

                    #region Send OnAddUserRoleRequest event

                    try
                    {

                        OnAddUserRoleRequest?.Invoke(Timestamp.Now,
                                                     this,
                                                     WebSocketConnection,
                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAddUserRoleRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    AddUserRoleResponse? response = null;

                    var results = OnAddUserRole?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnAddUserRoleDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= AddUserRoleResponse.Failed(request);

                    #endregion

                    #region Send OnAddUserRoleResponse event

                    try
                    {

                        OnAddUserRoleResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      WebSocketConnection,
                                                      request,
                                                      response,
                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAddUserRoleResponse));
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
                                            nameof(Receive_AddUserRole)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_AddUserRole)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnAddUserRoleWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnAddUserRoleWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAddUserRoleWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
