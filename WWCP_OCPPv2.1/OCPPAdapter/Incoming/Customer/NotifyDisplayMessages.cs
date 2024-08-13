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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a NotifyDisplayMessages request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyDisplayMessagesRequestReceivedDelegate(DateTime                       Timestamp,
                                                                        IEventSender                   Sender,
                                                                        IWebSocketConnection           Connection,
                                                                        NotifyDisplayMessagesRequest   Request,
                                                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyDisplayMessages response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyDisplayMessagesResponseReceivedDelegate(DateTime                        Timestamp,
                                                                         IEventSender                    Sender,
                                                                         IWebSocketConnection            Connection,
                                                                         NotifyDisplayMessagesRequest?   Request,
                                                                         NotifyDisplayMessagesResponse   Response,
                                                                         TimeSpan?                       Runtime,
                                                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyDisplayMessages request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyDisplayMessagesRequestErrorReceivedDelegate(DateTime                        Timestamp,
                                                                             IEventSender                    Sender,
                                                                             IWebSocketConnection            Connection,
                                                                             NotifyDisplayMessagesRequest?   Request,
                                                                             OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                                             TimeSpan?                       Runtime,
                                                                             CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyDisplayMessages response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyDisplayMessagesResponseErrorReceivedDelegate(DateTime                         Timestamp,
                                                                              IEventSender                     Sender,
                                                                              IWebSocketConnection             Connection,
                                                                              NotifyDisplayMessagesRequest?    Request,
                                                                              NotifyDisplayMessagesResponse?   Response,
                                                                              OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
                                                                              TimeSpan?                        Runtime,
                                                                              CancellationToken                CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a NotifyDisplayMessages response is expected
    /// for a received NotifyDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyDisplayMessagesResponse>

        OnNotifyDisplayMessagesDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        IWebSocketConnection           Connection,
                                        NotifyDisplayMessagesRequest   Request,
                                        CancellationToken              CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyDisplayMessages request

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestReceivedDelegate?  OnNotifyDisplayMessagesRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyDisplayMessages request was received for processing.
        /// </summary>
        public event OnNotifyDisplayMessagesDelegate?                 OnNotifyDisplayMessages;


        public async Task<OCPP_Response>

            Receive_NotifyDisplayMessages(DateTime              RequestTimestamp,
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

                if (NotifyDisplayMessagesRequest.TryParse(JSONRequest,
                                                          RequestId,
                                                          DestinationId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          RequestTimestamp,
                                                          parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                          EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestParser)) {

                    NotifyDisplayMessagesResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomMessageInfoSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = NotifyDisplayMessagesResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyDisplayMessagesRequestReceived event

                    await LogEvent(
                              OnNotifyDisplayMessagesRequestReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  request,
                                  CancellationToken
                              )
                          );

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnNotifyDisplayMessages?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnNotifyDisplayMessagesDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : NotifyDisplayMessagesResponse.Failed(request, $"Undefined {nameof(OnNotifyDisplayMessages)}!");

                        }
                        catch (Exception e)
                        {

                            response = NotifyDisplayMessagesResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnNotifyDisplayMessages),
                                      e
                                  );

                        }
                    }

                    response ??= NotifyDisplayMessagesResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnNotifyDisplayMessagesResponseSent(
                                                                            Timestamp.Now,
                                                                            parentNetworkingNode,
                                                                            sentMessageResult.Connection,
                                                                            request,
                                                                            response,
                                                                            response.Runtime,
                                                                            sentMessageResult.Result,
                                                                            CancellationToken
                                                                        ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_NotifyDisplayMessages)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyDisplayMessages)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyDisplayMessages response

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages response was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseReceivedDelegate? OnNotifyDisplayMessagesResponseReceived;


        public async Task<NotifyDisplayMessagesResponse>

            Receive_NotifyDisplayMessagesResponse(NotifyDisplayMessagesRequest  Request,
                                                  JObject                       ResponseJSON,
                                                  IWebSocketConnection          WebSocketConnection,
                                                  NetworkingNode_Id             DestinationId,
                                                  NetworkPath                   NetworkPath,
                                                  EventTracking_Id              EventTrackingId,
                                                  Request_Id                    RequestId,
                                                  DateTime?                     ResponseTimestamp   = null,
                                                  CancellationToken             CancellationToken   = default)

        {

            NotifyDisplayMessagesResponse? response = null;

            try
            {

                if (NotifyDisplayMessagesResponse.TryParse(Request,
                                                           ResponseJSON,
                                                           DestinationId,
                                                           NetworkPath,
                                                           out response,
                                                           out var errorResponse,
                                                           ResponseTimestamp,
                                                           parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseParser,
                                                           parentNetworkingNode.OCPP.CustomSignatureParser,
                                                           parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = NotifyDisplayMessagesResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = NotifyDisplayMessagesResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = NotifyDisplayMessagesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnNotifyDisplayMessagesResponseReceived event

            await LogEvent(
                      OnNotifyDisplayMessagesResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          Request,
                          response,
                          response.Runtime,
                          CancellationToken
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region Receive NotifyDisplayMessages request error

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request error was received.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestErrorReceivedDelegate? NotifyDisplayMessagesRequestErrorReceived;


        public async Task<NotifyDisplayMessagesResponse>

            Receive_NotifyDisplayMessagesRequestError(NotifyDisplayMessagesRequest  Request,
                                                      OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                      IWebSocketConnection          Connection,
                                                      NetworkingNode_Id             DestinationId,
                                                      NetworkPath                   NetworkPath,
                                                      EventTracking_Id              EventTrackingId,
                                                      Request_Id                    RequestId,
                                                      DateTime?                     ResponseTimestamp   = null,
                                                      CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
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

            #region Send NotifyDisplayMessagesRequestErrorReceived event

            await LogEvent(
                      NotifyDisplayMessagesRequestErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          Connection,
                          Request,
                          RequestErrorMessage,
                          RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
                          CancellationToken
                      )
                  );

            #endregion


            var response = NotifyDisplayMessagesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNotifyDisplayMessagesResponseReceived event

            await LogEvent(
                      OnNotifyDisplayMessagesResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          Connection,
                          Request,
                          response,
                          response.Runtime,
                          CancellationToken
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region Receive NotifyDisplayMessages response error

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages response error was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseErrorReceivedDelegate? NotifyDisplayMessagesResponseErrorReceived;


        public async Task

            Receive_NotifyDisplayMessagesResponseError(NotifyDisplayMessagesRequest?   Request,
                                                       NotifyDisplayMessagesResponse?  Response,
                                                       OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                       IWebSocketConnection            Connection,
                                                       NetworkingNode_Id               DestinationId,
                                                       NetworkPath                     NetworkPath,
                                                       EventTracking_Id                EventTrackingId,
                                                       Request_Id                      RequestId,
                                                       DateTime?                       ResponseTimestamp   = null,
                                                       CancellationToken               CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomNotifyDisplayMessagesResponseSerializer,
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

            #region Send NotifyDisplayMessagesResponseErrorReceived event

            await LogEvent(
                      NotifyDisplayMessagesResponseErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          Connection,
                          Request,
                          Response,
                          ResponseErrorMessage,
                          Response is not null
                              ? ResponseErrorMessage.ResponseTimestamp - Response.ResponseTimestamp
                              : null,
                          CancellationToken
                      )
                  );

            #endregion


        }

        #endregion

    }

}
