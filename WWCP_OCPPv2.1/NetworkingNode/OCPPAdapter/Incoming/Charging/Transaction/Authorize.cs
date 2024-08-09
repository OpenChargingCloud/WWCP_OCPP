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
    /// A logging delegate called whenever an Authorize request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The Authorize request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnAuthorizeRequestReceivedDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           AuthorizeRequest       Request,
                                           CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an Authorize response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAuthorizeResponseReceivedDelegate(DateTime               Timestamp,
                                                             IEventSender           Sender,
                                                             IWebSocketConnection   Connection,
                                                             AuthorizeRequest?      Request,
                                                             AuthorizeResponse      Response,
                                                             TimeSpan?              Runtime,
                                                             CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an Authorize request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAuthorizeRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                 IEventSender                   Sender,
                                                                 IWebSocketConnection           Connection,
                                                                 AuthorizeRequest?              Request,
                                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                 TimeSpan?                      Runtime,
                                                                 CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever an Authorize response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAuthorizeResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                  IEventSender                    Sender,
                                                                  IWebSocketConnection            Connection,
                                                                  AuthorizeRequest?               Request,
                                                                  AuthorizeResponse?              Response,
                                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                  TimeSpan?                       Runtime,
                                                                  CancellationToken               CancellationToken = default);

    #endregion


    /// <summary>
    /// A delegate called whenever an Authorize response is expected
    /// for a received Authorize request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The Authorize request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime               Timestamp,
                            IEventSender           Sender,
                            IWebSocketConnection   Connection,
                            AuthorizeRequest       Request,
                            CancellationToken      CancellationToken = default);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive Authorize request

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestReceivedDelegate?  OnAuthorizeRequestReceived;

        /// <summary>
        /// An event sent whenever an Authorize request was received for processing.
        /// </summary>
        public event OnAuthorizeDelegate?                 OnAuthorize;


        public async Task<OCPP_Response>

            Receive_Authorize(DateTime              RequestTimestamp,
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

                if (AuthorizeRequest.TryParse(JSONRequest,
                                              RequestId,
                                              DestinationId,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              RequestTimestamp,
                                              parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                              EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomAuthorizeRequestParser)) {

                    AuthorizeResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = AuthorizeResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnAuthorizeRequestReceived event

                    await LogEvent(
                              OnAuthorizeRequestReceived,
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

                            var responseTasks = OnAuthorize?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(
                                                                                                                               Timestamp.Now,
                                                                                                                               parentNetworkingNode,
                                                                                                                               WebSocketConnection,
                                                                                                                               request,
                                                                                                                               CancellationToken
                                                                                                                           )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : AuthorizeResponse.Failed(request, $"Undefined {nameof(OnAuthorize)}!");

                        }
                        catch (Exception e)
                        {

                            response = AuthorizeResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnAuthorize),
                                      e
                                  );

                        }
                    }

                    response ??= AuthorizeResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion


                    #region Send OnAuthorizeResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnAuthorizeResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime,
                              SentMessageResults.Unknown
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                           parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                           parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
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
                                       nameof(Receive_Authorize)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_Authorize)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive Authorize response

        /// <summary>
        /// An event fired whenever an Authorize response was received.
        /// </summary>
        public event OnAuthorizeResponseReceivedDelegate? OnAuthorizeResponseReceived;


        public async Task<AuthorizeResponse>

            Receive_AuthorizeResponse(AuthorizeRequest      Request,
                                      JObject               ResponseJSON,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id     DestinationId,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      DateTime?             ResponseTimestamp   = null,
                                      CancellationToken     CancellationToken   = default)

        {

            AuthorizeResponse? response = null;

            try
            {

                if (AuthorizeResponse.TryParse(Request,
                                               ResponseJSON,
                                               DestinationId,
                                               NetworkPath,
                                               out response,
                                               out var errorResponse,
                                               ResponseTimestamp,
                                               parentNetworkingNode.OCPP.CustomAuthorizeResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                                parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = AuthorizeResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = AuthorizeResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = AuthorizeResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnAuthorizeResponseReceived event

            await LogEvent(
                      OnAuthorizeResponseReceived,
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

        #region Receive Authorize RequestError

        /// <summary>
        /// An event fired whenever an Authorize request error was received.
        /// </summary>
        public event OnAuthorizeRequestErrorReceivedDelegate? AuthorizeRequestErrorReceived;


        public async Task<AuthorizeResponse>

            Receive_AuthorizeRequestError(AuthorizeRequest              Request,
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
            //        parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
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

            #region Send AuthorizeRequestErrorReceived event

            await LogEvent(
                      AuthorizeRequestErrorReceived,
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


            var response = AuthorizeResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnAuthorizeResponseReceived event

            await LogEvent(
                      OnAuthorizeResponseReceived,
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

        #region Receive Authorize ResponseError

        /// <summary>
        /// An event fired whenever an Authorize response error was received.
        /// </summary>
        public event OnAuthorizeResponseErrorReceivedDelegate? AuthorizeResponseErrorReceived;


        public async Task

            Receive_AuthorizeResponseError(AuthorizeRequest?              Request,
                                           AuthorizeResponse?             Response,
                                           OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                           IWebSocketConnection           Connection,
                                           NetworkingNode_Id              DestinationId,
                                           NetworkPath                    NetworkPath,
                                           EventTracking_Id               EventTrackingId,
                                           Request_Id                     RequestId,
                                           DateTime?                      ResponseTimestamp   = null,
                                           CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
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

            #region Send AuthorizeResponseErrorReceived event

            await LogEvent(
                      AuthorizeResponseErrorReceived,
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
