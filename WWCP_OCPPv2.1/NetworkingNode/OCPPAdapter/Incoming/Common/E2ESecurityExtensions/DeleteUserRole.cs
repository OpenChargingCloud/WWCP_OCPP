﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleRequestReceivedDelegate?  OnDeleteUserRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received for processing.
        /// </summary>
        public event OnDeleteUserRoleDelegate?                 OnDeleteUserRole;

        #endregion

        #region Receive DeleteUserRoleRequest (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_DeleteUserRole(DateTime              RequestTimestamp,
                                   IWebSocketConnection  WebSocketConnection,
                                   NetworkingNode_Id     DestinationId,
                                   NetworkPath           NetworkPath,
                                   EventTracking_Id      EventTrackingId,
                                   Request_Id            RequestId,
                                   JObject               JSONRequest,
                                   CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (DeleteUserRoleRequest.TryParse(JSONRequest,
                                                   RequestId,
                                                   DestinationId,
                                                   NetworkPath,
                                                   out var request,
                                                   out var errorResponse,
                                                   RequestTimestamp,
                                                   parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                   EventTrackingId,
                                                   parentNetworkingNode.OCPP.CustomDeleteUserRoleRequestParser)) {

                    DeleteUserRoleResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDeleteUserRoleRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = DeleteUserRoleResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnDeleteUserRoleRequestReceived event

                    var logger = OnDeleteUserRoleRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnDeleteUserRoleRequestReceivedDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request
                                                                             )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnDeleteUserRoleRequestReceived),
                                      e
                                  );
                        }
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnDeleteUserRole?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnDeleteUserRoleDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : DeleteUserRoleResponse.Failed(request, $"Undefined {nameof(OnDeleteUserRole)}!");

                        }
                        catch (Exception e)
                        {

                            response = DeleteUserRoleResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnDeleteUserRole),
                                      e
                                  );

                        }
                    }

                    response ??= DeleteUserRoleResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomDeleteUserRoleResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteUserRoleResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnDeleteUserRoleResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomDeleteUserRoleResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_DeleteUserRole)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_DeleteUserRole)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive DeleteUserRoleRequestError

        public async Task<DeleteUserRoleResponse>

            Receive_DeleteUserRoleRequestError(DeleteUserRoleRequest         Request,
                                               OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                               IWebSocketConnection          WebSocketConnection)

        {

            var response = DeleteUserRoleResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomDeleteUserRoleResponseSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
            //        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
            //        parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
            //        parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //        parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //    ),
            //    out errorResponse
            //);

            #region Send OnDeleteUserRoleResponseReceived event

            var logger = OnDeleteUserRoleResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnDeleteUserRoleResponseReceivedDelegate>().
                                                Select(loggingDelegate => loggingDelegate.Invoke(
                                                                               Timestamp.Now,
                                                                               parentNetworkingNode,
                                                                               //    WebSocketConnection,
                                                                               Request,
                                                                               response,
                                                                               response.Runtime
                                                                           )).
                                                ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteUserRoleResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a DeleteUserRole was sent.
        /// </summary>
        public event OnDeleteUserRoleResponseSentDelegate?  OnDeleteUserRoleResponseSent;

        #endregion

        #region Send OnDeleteUserRoleResponse event

        public async Task SendOnDeleteUserRoleResponseSent(DateTime                Timestamp,
                                                           IEventSender            Sender,
                                                           IWebSocketConnection    Connection,
                                                           DeleteUserRoleRequest   Request,
                                                           DeleteUserRoleResponse  Response,
                                                           TimeSpan                Runtime)
        {

            var logger = OnDeleteUserRoleResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnDeleteUserRoleResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnDeleteUserRoleResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
