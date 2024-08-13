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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyRequestReceivedDelegate(DateTime                       Timestamp,
                                                                        IEventSender                   Sender,
                                                                        IWebSocketConnection           Connection,
                                                                        NotifyNetworkTopologyRequest   Request,
                                                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyResponseReceivedDelegate(DateTime                        Timestamp,
                                                                         IEventSender                    Sender,
                                                                         IWebSocketConnection            Connection,
                                                                         NotifyNetworkTopologyRequest?   Request,
                                                                         NotifyNetworkTopologyResponse   Response,
                                                                         TimeSpan?                       Runtime,
                                                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyRequestErrorReceivedDelegate(DateTime                        Timestamp,
                                                                             IEventSender                    Sender,
                                                                             IWebSocketConnection            Connection,
                                                                             NotifyNetworkTopologyRequest?   Request,
                                                                             OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                                             TimeSpan?                       Runtime,
                                                                             CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTopology response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTopologyResponseErrorReceivedDelegate(DateTime                         Timestamp,
                                                                              IEventSender                     Sender,
                                                                              IWebSocketConnection             Connection,
                                                                              NotifyNetworkTopologyRequest?    Request,
                                                                              NotifyNetworkTopologyResponse?   Response,
                                                                              OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
                                                                              TimeSpan?                        Runtime,
                                                                              CancellationToken                CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a NotifyNetworkTopology response is expected
    /// for a received NotifyNetworkTopology request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyNetworkTopologyResponse>

        OnNotifyNetworkTopologyDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        IWebSocketConnection           Connection,
                                        NotifyNetworkTopologyRequest   Request,
                                        CancellationToken              CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyNetworkTopology request

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyRequestReceivedDelegate?  OnNotifyNetworkTopologyRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received for processing.
        /// </summary>
        public event OnNotifyNetworkTopologyDelegate?                 OnNotifyNetworkTopology;


        public async Task<OCPP_Response>

            Receive_NotifyNetworkTopology(DateTime              RequestTimestamp,
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

                if (NotifyNetworkTopologyRequest.TryParse(JSONRequest,
                                                          RequestId,
                                                          DestinationId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          RequestTimestamp,
                                                          parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                          EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyRequestParser)) {

                    NotifyNetworkTopologyResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyRequestSerializer,
                            parentNetworkingNode.OCPP.CustomNetworkTopologyInformationSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = NotifyNetworkTopologyResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyNetworkTopologyRequestReceived event

                    await LogEvent(
                              OnNotifyNetworkTopologyRequestReceived,
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

                            var responseTasks = OnNotifyNetworkTopology?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnNotifyNetworkTopologyDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : NotifyNetworkTopologyResponse.Failed(request, $"Undefined {nameof(OnNotifyNetworkTopology)}!");

                        }
                        catch (Exception e)
                        {

                            response = NotifyNetworkTopologyResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnNotifyNetworkTopology),
                                      e
                                  );

                        }
                    }

                    response ??= NotifyNetworkTopologyResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
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
                                           parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnNotifyNetworkTopologyResponseSent(
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
                                       nameof(Receive_NotifyNetworkTopology)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyNetworkTopology)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyNetworkTopology response

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology response was received.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseReceivedDelegate? OnNotifyNetworkTopologyResponseReceived;


        public async Task<NotifyNetworkTopologyResponse>

            Receive_NotifyNetworkTopologyResponse(NotifyNetworkTopologyRequest  Request,
                                                  JObject                       ResponseJSON,
                                                  IWebSocketConnection          WebSocketConnection,
                                                  NetworkingNode_Id             DestinationId,
                                                  NetworkPath                   NetworkPath,
                                                  EventTracking_Id              EventTrackingId,
                                                  Request_Id                    RequestId,
                                                  DateTime?                     ResponseTimestamp   = null,
                                                  CancellationToken             CancellationToken   = default)

        {

            NotifyNetworkTopologyResponse? response = null;

            try
            {

                if (NotifyNetworkTopologyResponse.TryParse(Request,
                                                           ResponseJSON,
                                                           DestinationId,
                                                           NetworkPath,
                                                           out response,
                                                           out var errorResponse,
                                                           ResponseTimestamp,
                                                           parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseParser,
                                                           parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                           parentNetworkingNode.OCPP.CustomSignatureParser,
                                                           parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = NotifyNetworkTopologyResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = NotifyNetworkTopologyResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = NotifyNetworkTopologyResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnNotifyNetworkTopologyResponseReceived event

            await LogEvent(
                      OnNotifyNetworkTopologyResponseReceived,
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

        #region Receive NotifyNetworkTopology request error

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology request error was received.
        /// </summary>
        public event OnNotifyNetworkTopologyRequestErrorReceivedDelegate? NotifyNetworkTopologyRequestErrorReceived;


        public async Task<NotifyNetworkTopologyResponse>

            Receive_NotifyNetworkTopologyRequestError(NotifyNetworkTopologyRequest  Request,
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
            //        parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
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

            #region Send NotifyNetworkTopologyRequestErrorReceived event

            await LogEvent(
                      NotifyNetworkTopologyRequestErrorReceived,
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


            var response = NotifyNetworkTopologyResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNotifyNetworkTopologyResponseReceived event

            await LogEvent(
                      OnNotifyNetworkTopologyResponseReceived,
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

        #region Receive NotifyNetworkTopology response error

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology response error was received.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseErrorReceivedDelegate? NotifyNetworkTopologyResponseErrorReceived;


        public async Task

            Receive_NotifyNetworkTopologyResponseError(NotifyNetworkTopologyRequest?   Request,
                                                       NotifyNetworkTopologyResponse?  Response,
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
            //        parentNetworkingNode.OCPP.CustomNotifyNetworkTopologyResponseSerializer,
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

            #region Send NotifyNetworkTopologyResponseErrorReceived event

            await LogEvent(
                      NotifyNetworkTopologyResponseErrorReceived,
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
