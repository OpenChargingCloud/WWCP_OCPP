/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a SwipeRFIDCard request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSwipeRFIDCardRequestReceivedDelegate(DateTimeOffset         Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                SwipeRFIDCardRequest   Request,
                                                                CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SwipeRFIDCard response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSwipeRFIDCardResponseReceivedDelegate(DateTimeOffset          Timestamp,
                                                                 IEventSender            Sender,
                                                                 IWebSocketConnection?   Connection,
                                                                 SwipeRFIDCardRequest?   Request,
                                                                 SwipeRFIDCardResponse   Response,
                                                                 TimeSpan?               Runtime,
                                                                 CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SwipeRFIDCard request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSwipeRFIDCardRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                     IEventSender                   Sender,
                                                                     IWebSocketConnection           Connection,
                                                                     SwipeRFIDCardRequest?          Request,
                                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                     TimeSpan?                      Runtime,
                                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SwipeRFIDCard response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSwipeRFIDCardResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                      IEventSender                    Sender,
                                                                      IWebSocketConnection            Connection,
                                                                      SwipeRFIDCardRequest?           Request,
                                                                      SwipeRFIDCardResponse?          Response,
                                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                      TimeSpan?                       Runtime,
                                                                      CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a SwipeRFIDCard response is expected
    /// for a received SwipeRFIDCard request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SwipeRFIDCardResponse>

        OnSwipeRFIDCardDelegate(DateTimeOffset         Timestamp,
                                IEventSender           Sender,
                                IWebSocketConnection   Connection,
                                SwipeRFIDCardRequest   Request,
                                CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive SwipeRFIDCard request (JSON)

        /// <summary>
        /// An event sent whenever a SwipeRFIDCard request was received.
        /// </summary>
        public event OnSwipeRFIDCardRequestReceivedDelegate?  OnSwipeRFIDCardRequestReceived;

        /// <summary>
        /// An event sent whenever a SwipeRFIDCard request was received for processing.
        /// </summary>
        public event OnSwipeRFIDCardDelegate?                 OnSwipeRFIDCard;


        public async Task<OCPP_Response>

            Receive_SwipeRFIDCard(DateTimeOffset        RequestTimestamp,
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

                if (SwipeRFIDCardRequest.TryParse(JSONRequest,
                                                  RequestId,
                                                  Destination,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  RequestTimestamp,
                                                  parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                  EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomSwipeRFIDCardRequestParser)) {

                    SwipeRFIDCardResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSwipeRFIDCardRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SwipeRFIDCardResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSwipeRFIDCardRequestReceived event

                    await LogEvent(
                              OnSwipeRFIDCardRequestReceived,
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
                                           OnSwipeRFIDCard,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SwipeRFIDCardResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSwipeRFIDCardResponseSent(
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
                                       nameof(Receive_SwipeRFIDCard)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SwipeRFIDCard)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SwipeRFIDCard response (JSON)

        /// <summary>
        /// An event fired whenever a SwipeRFIDCard response was received.
        /// </summary>
        public event OnSwipeRFIDCardResponseReceivedDelegate? OnSwipeRFIDCardResponseReceived;


        public async Task<SwipeRFIDCardResponse>

            Receive_SwipeRFIDCardResponse(SwipeRFIDCardRequest  Request,
                                          JObject               ResponseJSON,
                                          IWebSocketConnection  WebSocketConnection,
                                          SourceRouting         Destination,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          DateTimeOffset?       ResponseTimestamp   = null,
                                          CancellationToken     CancellationToken   = default)

        {

            SwipeRFIDCardResponse? response = null;

            try
            {

                if (SwipeRFIDCardResponse.TryParse(Request,
                                                   ResponseJSON,
                                                   Destination,
                                                   NetworkPath,
                                                   out response,
                                                   out var errorResponse,
                                                   ResponseTimestamp,
                                                   parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseParser,
                                                   parentNetworkingNode.OCPP.CustomSignatureParser,
                                                   parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SwipeRFIDCardResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SwipeRFIDCardResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SwipeRFIDCardResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnSwipeRFIDCardResponseReceived event

            await LogEvent(
                      OnSwipeRFIDCardResponseReceived,
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


        #region Receive SwipeRFIDCard request error

        /// <summary>
        /// An event fired whenever a SwipeRFIDCard request error was received.
        /// </summary>
        public event OnSwipeRFIDCardRequestErrorReceivedDelegate? SwipeRFIDCardRequestErrorReceived;


        public async Task<SwipeRFIDCardResponse>

            Receive_SwipeRFIDCardRequestError(SwipeRFIDCardRequest          Request,
                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection          Connection,
                                              SourceRouting                 Destination,
                                              NetworkPath                   NetworkPath,
                                              EventTracking_Id              EventTrackingId,
                                              Request_Id                    RequestId,
                                              DateTimeOffset?               ResponseTimestamp   = null,
                                              CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
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

            #region Send SwipeRFIDCardRequestErrorReceived event

            await LogEvent(
                      SwipeRFIDCardRequestErrorReceived,
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


            var response = SwipeRFIDCardResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSwipeRFIDCardResponseReceived event

            await LogEvent(
                      OnSwipeRFIDCardResponseReceived,
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


        public async Task<SwipeRFIDCardResponse>

            Receive_SwipeRFIDCardRequestError(SwipeRFIDCardRequest            Request,
                                              OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection            Connection,
                                              SourceRouting                   Destination,
                                              NetworkPath                     NetworkPath,
                                              EventTracking_Id                EventTrackingId,
                                              Request_Id                      RequestId,
                                              DateTimeOffset?                 ResponseTimestamp   = null,
                                              CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
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

            #region Send SwipeRFIDCardRequestErrorReceived event

            //await LogEvent(
            //          SwipeRFIDCardRequestErrorReceived,
            //          loggingDelegate => loggingDelegate.Invoke(
            //              Timestamp.Now,
            //              parentNetworkingNode,
            //              Connection,
            //              Request,
            //              RequestErrorMessage,
            //              RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
            //              CancellationToken
            //          )
            //      );

            #endregion


            var response = SwipeRFIDCardResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSwipeRFIDCardResponseReceived event

            await LogEvent(
                      OnSwipeRFIDCardResponseReceived,
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

        #region Receive SwipeRFIDCard response error

        /// <summary>
        /// An event fired whenever a SwipeRFIDCard response error was received.
        /// </summary>
        public event OnSwipeRFIDCardResponseErrorReceivedDelegate? SwipeRFIDCardResponseErrorReceived;


        public async Task

            Receive_SwipeRFIDCardResponseError(SwipeRFIDCardRequest?          Request,
                                               SwipeRFIDCardResponse?         Response,
                                               OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                               IWebSocketConnection           Connection,
                                               SourceRouting                  Destination,
                                               NetworkPath                    NetworkPath,
                                               EventTracking_Id               EventTrackingId,
                                               Request_Id                     RequestId,
                                               DateTimeOffset?                ResponseTimestamp   = null,
                                               CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSwipeRFIDCardResponseSerializer,
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

            #region Send SwipeRFIDCardResponseErrorReceived event

            await LogEvent(
                      SwipeRFIDCardResponseErrorReceived,
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
