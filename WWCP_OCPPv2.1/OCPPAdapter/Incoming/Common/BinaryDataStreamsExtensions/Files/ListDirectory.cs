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

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a ListDirectory request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnListDirectoryRequestReceivedDelegate(DateTime               Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                ListDirectoryRequest   Request,
                                                                CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ListDirectory response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnListDirectoryResponseReceivedDelegate(DateTime                Timestamp,
                                                                 IEventSender            Sender,
                                                                 IWebSocketConnection    Connection,
                                                                 ListDirectoryRequest?   Request,
                                                                 ListDirectoryResponse   Response,
                                                                 TimeSpan?               Runtime,
                                                                 CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ListDirectory request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnListDirectoryRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                     IEventSender                   Sender,
                                                                     IWebSocketConnection           Connection,
                                                                     ListDirectoryRequest?          Request,
                                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                     TimeSpan?                      Runtime,
                                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ListDirectory response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnListDirectoryResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                      IEventSender                    Sender,
                                                                      IWebSocketConnection            Connection,
                                                                      ListDirectoryRequest?           Request,
                                                                      ListDirectoryResponse?          Response,
                                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                      TimeSpan?                       Runtime,
                                                                      CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a ListDirectory response is expected
    /// for a received ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ListDirectoryResponse>

        OnListDirectoryDelegate(DateTime               Timestamp,
                                IEventSender           Sender,
                                IWebSocketConnection   Connection,
                                ListDirectoryRequest   Request,
                                CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive ListDirectory request

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        public event OnListDirectoryRequestReceivedDelegate?  OnListDirectoryRequestReceived;

        /// <summary>
        /// An event sent whenever a ListDirectory request was received for processing.
        /// </summary>
        public event OnListDirectoryDelegate?                 OnListDirectory;


        public async Task<OCPP_Response>

            Receive_ListDirectory(DateTime              RequestTimestamp,
                                  IWebSocketConnection  WebSocketConnection,
                                  SourceRouting         SourceRouting,
                                  NetworkPath           NetworkPath,
                                  EventTracking_Id      EventTrackingId,
                                  Request_Id            RequestId,
                                  JObject               JSONRequest,
                                  CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (ListDirectoryRequest.TryParse(JSONRequest,
                                                  RequestId,
                                                  SourceRouting,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  RequestTimestamp,
                                                  parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                  EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomListDirectoryRequestParser)) {

                    ListDirectoryResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomListDirectoryRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = ListDirectoryResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnListDirectoryRequestReceived event

                    await LogEvent(
                              OnListDirectoryRequestReceived,
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

                            var responseTasks = OnListDirectory?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnListDirectoryDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : ListDirectoryResponse.Failed(request, $"Undefined {nameof(OnListDirectory)}!");

                        }
                        catch (Exception e)
                        {

                            response = ListDirectoryResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnListDirectory),
                                      e
                                  );

                        }
                    }

                    response ??= ListDirectoryResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       SourceRouting.To(NetworkPath.Source),
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnListDirectoryResponseSent(
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
                                       nameof(Receive_ListDirectory)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_ListDirectory)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive ListDirectory response

        /// <summary>
        /// An event fired whenever a ListDirectory response was received.
        /// </summary>
        public event OnListDirectoryResponseReceivedDelegate? OnListDirectoryResponseReceived;


        public async Task<ListDirectoryResponse>

            Receive_ListDirectoryResponse(ListDirectoryRequest  Request,
                                          JObject               ResponseJSON,
                                          IWebSocketConnection  WebSocketConnection,
                                          SourceRouting         SourceRouting,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          DateTime?             ResponseTimestamp   = null,
                                          CancellationToken     CancellationToken   = default)

        {

            ListDirectoryResponse? response = null;

            try
            {

                if (ListDirectoryResponse.TryParse(Request,
                                                   ResponseJSON,
                                                   SourceRouting,
                                                   NetworkPath,
                                                   out response,
                                                   out var errorResponse,
                                                   ResponseTimestamp,
                                                   parentNetworkingNode.OCPP.CustomListDirectoryResponseParser,
                                                   parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                   parentNetworkingNode.OCPP.CustomSignatureParser,
                                                   parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = ListDirectoryResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = ListDirectoryResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = ListDirectoryResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnListDirectoryResponseReceived event

            await LogEvent(
                      OnListDirectoryResponseReceived,
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

        #region Receive ListDirectory request error

        /// <summary>
        /// An event fired whenever a ListDirectory request error was received.
        /// </summary>
        public event OnListDirectoryRequestErrorReceivedDelegate? ListDirectoryRequestErrorReceived;


        public async Task<ListDirectoryResponse>

            Receive_ListDirectoryRequestError(ListDirectoryRequest          Request,
                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection          Connection,
                                              SourceRouting                 Destination,
                                              NetworkPath                   NetworkPath,
                                              EventTracking_Id              EventTrackingId,
                                              Request_Id                    RequestId,
                                              DateTime?                     ResponseTimestamp   = null,
                                              CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
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

            #region Send ListDirectoryRequestErrorReceived event

            await LogEvent(
                      ListDirectoryRequestErrorReceived,
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


            var response = ListDirectoryResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnListDirectoryResponseReceived event

            await LogEvent(
                      OnListDirectoryResponseReceived,
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

        #region Receive ListDirectory response error

        /// <summary>
        /// An event fired whenever a ListDirectory response error was received.
        /// </summary>
        public event OnListDirectoryResponseErrorReceivedDelegate? ListDirectoryResponseErrorReceived;


        public async Task

            Receive_ListDirectoryResponseError(ListDirectoryRequest?          Request,
                                               ListDirectoryResponse?         Response,
                                               OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                               IWebSocketConnection           Connection,
                                               SourceRouting                  Destination,
                                               NetworkPath                    NetworkPath,
                                               EventTracking_Id               EventTrackingId,
                                               Request_Id                     RequestId,
                                               DateTime?                      ResponseTimestamp   = null,
                                               CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
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

            #region Send ListDirectoryResponseErrorReceived event

            await LogEvent(
                      ListDirectoryResponseErrorReceived,
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