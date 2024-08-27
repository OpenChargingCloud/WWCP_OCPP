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
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a GetDisplayMessages request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDisplayMessagesRequestReceivedDelegate(DateTime                    Timestamp,
                                                                     IEventSender                Sender,
                                                                     IWebSocketConnection        Connection,
                                                                     GetDisplayMessagesRequest   Request,
                                                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetDisplayMessages response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDisplayMessagesResponseReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection?        Connection,
                                                                      GetDisplayMessagesRequest?   Request,
                                                                      GetDisplayMessagesResponse   Response,
                                                                      TimeSpan?                    Runtime,
                                                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetDisplayMessages request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDisplayMessagesRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                          IEventSender                   Sender,
                                                                          IWebSocketConnection           Connection,
                                                                          GetDisplayMessagesRequest?     Request,
                                                                          OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                          TimeSpan?                      Runtime,
                                                                          CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetDisplayMessages response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDisplayMessagesResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                           IEventSender                    Sender,
                                                                           IWebSocketConnection            Connection,
                                                                           GetDisplayMessagesRequest?      Request,
                                                                           GetDisplayMessagesResponse?     Response,
                                                                           OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                           TimeSpan?                       Runtime,
                                                                           CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetDisplayMessages response is expected
    /// for a received GetDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetDisplayMessagesResponse>

        OnGetDisplayMessagesDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     IWebSocketConnection        Connection,
                                     GetDisplayMessagesRequest   Request,
                                     CancellationToken           CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetDisplayMessages request

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesRequestReceivedDelegate?  OnGetDisplayMessagesRequestReceived;

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received for processing.
        /// </summary>
        public event OnGetDisplayMessagesDelegate?                 OnGetDisplayMessages;


        public async Task<OCPP_Response>

            Receive_GetDisplayMessages(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       SourceRouting         Destination,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       JObject               JSONRequest,
                                       CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (GetDisplayMessagesRequest.TryParse(JSONRequest,
                                                       RequestId,
                                                   Destination,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       RequestTimestamp,
                                                       parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                       EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomGetDisplayMessagesRequestParser)) {

                    GetDisplayMessagesResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetDisplayMessagesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetDisplayMessagesResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetDisplayMessagesRequestReceived event

                    await LogEvent(
                              OnGetDisplayMessagesRequestReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  request,
                                  CancellationToken
                              )
                          );

                    #endregion


                    response ??= await CallProcessor(
                                           OnGetDisplayMessages,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetDisplayMessagesResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetDisplayMessagesResponseSent(
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
                                       nameof(Receive_GetDisplayMessages)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetDisplayMessages)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetDisplayMessages response

        /// <summary>
        /// An event fired whenever a GetDisplayMessages response was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseReceivedDelegate? OnGetDisplayMessagesResponseReceived;


        public async Task<GetDisplayMessagesResponse>

            Receive_GetDisplayMessagesResponse(GetDisplayMessagesRequest  Request,
                                               JObject                    ResponseJSON,
                                               IWebSocketConnection       WebSocketConnection,
                                               SourceRouting          Destination,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               DateTime?                  ResponseTimestamp   = null,
                                               CancellationToken          CancellationToken   = default)

        {

            GetDisplayMessagesResponse? response = null;

            try
            {

                if (GetDisplayMessagesResponse.TryParse(Request,
                                                        ResponseJSON,
                                                    Destination,
                                                        NetworkPath,
                                                        out response,
                                                        out var errorResponse,
                                                        ResponseTimestamp,
                                                        parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseParser,
                                                        parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                        parentNetworkingNode.OCPP.CustomSignatureParser,
                                                        parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetDisplayMessagesResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetDisplayMessagesResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetDisplayMessagesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetDisplayMessagesResponseReceived event

            await LogEvent(
                      OnGetDisplayMessagesResponseReceived,
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

        #region Receive GetDisplayMessages request error

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request error was received.
        /// </summary>
        public event OnGetDisplayMessagesRequestErrorReceivedDelegate? GetDisplayMessagesRequestErrorReceived;


        public async Task<GetDisplayMessagesResponse>

            Receive_GetDisplayMessagesRequestError(GetDisplayMessagesRequest     Request,
                                                   OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                   IWebSocketConnection          Connection,
                                                   SourceRouting             Destination,
                                                   NetworkPath                   NetworkPath,
                                                   EventTracking_Id              EventTrackingId,
                                                   Request_Id                    RequestId,
                                                   DateTime?                     ResponseTimestamp   = null,
                                                   CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
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

            #region Send GetDisplayMessagesRequestErrorReceived event

            await LogEvent(
                      GetDisplayMessagesRequestErrorReceived,
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


            var response = GetDisplayMessagesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetDisplayMessagesResponseReceived event

            await LogEvent(
                      OnGetDisplayMessagesResponseReceived,
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

        #region Receive GetDisplayMessages response error

        /// <summary>
        /// An event fired whenever a GetDisplayMessages response error was received.
        /// </summary>
        public event OnGetDisplayMessagesResponseErrorReceivedDelegate? GetDisplayMessagesResponseErrorReceived;


        public async Task

            Receive_GetDisplayMessagesResponseError(GetDisplayMessagesRequest?     Request,
                                                    GetDisplayMessagesResponse?    Response,
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
            //        parentNetworkingNode.OCPP.CustomGetDisplayMessagesResponseSerializer,
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

            #region Send GetDisplayMessagesResponseErrorReceived event

            await LogEvent(
                      GetDisplayMessagesResponseErrorReceived,
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
