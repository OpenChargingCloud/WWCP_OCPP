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
    /// A logging delegate called whenever a Heartbeat request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnHeartbeatRequestReceivedDelegate(DateTime               Timestamp,
                                                            IEventSender           Sender,
                                                            IWebSocketConnection   Connection,
                                                            HeartbeatRequest       Request,
                                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a Heartbeat response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnHeartbeatResponseReceivedDelegate(DateTime               Timestamp,
                                                             IEventSender           Sender,
                                                             IWebSocketConnection   Connection,
                                                             HeartbeatRequest?      Request,
                                                             HeartbeatResponse      Response,
                                                             TimeSpan?              Runtime,
                                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a Heartbeat request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnHeartbeatRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                 IEventSender                   Sender,
                                                                 IWebSocketConnection           Connection,
                                                                 HeartbeatRequest?              Request,
                                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                 TimeSpan?                      Runtime,
                                                                 CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a Heartbeat response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnHeartbeatResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                  IEventSender                    Sender,
                                                                  IWebSocketConnection            Connection,
                                                                  HeartbeatRequest?               Request,
                                                                  HeartbeatResponse?              Response,
                                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                  TimeSpan?                       Runtime,
                                                                  CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a Heartbeat response is expected
    /// for a received Heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime               Timestamp,
                            IEventSender           Sender,
                            IWebSocketConnection   Connection,
                            HeartbeatRequest       Request,
                            CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive Heartbeat request

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestReceivedDelegate?  OnHeartbeatRequestReceived;

        /// <summary>
        /// An event sent whenever a Heartbeat request was received for processing.
        /// </summary>
        public event OnHeartbeatDelegate?                 OnHeartbeat;


        public async Task<OCPP_Response>

            Receive_Heartbeat(DateTime              RequestTimestamp,
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

                if (HeartbeatRequest.TryParse(JSONRequest,
                                              RequestId,
                                              SourceRouting,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              RequestTimestamp,
                                              parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                              EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomHeartbeatRequestParser)) {

                    HeartbeatResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomHeartbeatRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = HeartbeatResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnHeartbeatRequestReceived event

                    await LogEvent(
                              OnHeartbeatRequestReceived,
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
                                           OnHeartbeat,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= HeartbeatResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnHeartbeatResponseSent(
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
                                       nameof(Receive_Heartbeat)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_Heartbeat)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive Heartbeat response

        /// <summary>
        /// An event fired whenever a Heartbeat response was received.
        /// </summary>
        public event OnHeartbeatResponseReceivedDelegate? OnHeartbeatResponseReceived;


        public async Task<HeartbeatResponse>

            Receive_HeartbeatResponse(HeartbeatRequest      Request,
                                      JObject               ResponseJSON,
                                      IWebSocketConnection  WebSocketConnection,
                                      SourceRouting         SourceRouting,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      DateTime?             ResponseTimestamp   = null,
                                      CancellationToken     CancellationToken   = default)

        {

            HeartbeatResponse? response = null;

            try
            {

                if (HeartbeatResponse.TryParse(Request,
                                               ResponseJSON,
                                               SourceRouting,
                                               NetworkPath,
                                               out response,
                                               out var errorResponse,
                                               ResponseTimestamp,
                                               parentNetworkingNode.OCPP.CustomHeartbeatResponseParser,
                                               parentNetworkingNode.OCPP.CustomSignatureParser,
                                               parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = HeartbeatResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = HeartbeatResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = HeartbeatResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnHeartbeatResponseReceived event

            await LogEvent(
                      OnHeartbeatResponseReceived,
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

        #region Receive Heartbeat request error

        /// <summary>
        /// An event fired whenever a Heartbeat request error was received.
        /// </summary>
        public event OnHeartbeatRequestErrorReceivedDelegate? HeartbeatRequestErrorReceived;


        public async Task<HeartbeatResponse>

            Receive_HeartbeatRequestError(HeartbeatRequest              Request,
                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                          IWebSocketConnection          Connection,
                                          SourceRouting                 SourceRouting,
                                          NetworkPath                   NetworkPath,
                                          EventTracking_Id              EventTrackingId,
                                          Request_Id                    RequestId,
                                          DateTime?                     ResponseTimestamp   = null,
                                          CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
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

            #region Send HeartbeatRequestErrorReceived event

            await LogEvent(
                      HeartbeatRequestErrorReceived,
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


            var response = HeartbeatResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnHeartbeatResponseReceived event

            await LogEvent(
                      OnHeartbeatResponseReceived,
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

        #region Receive Heartbeat response error

        /// <summary>
        /// An event fired whenever a Heartbeat response error was received.
        /// </summary>
        public event OnHeartbeatResponseErrorReceivedDelegate? HeartbeatResponseErrorReceived;


        public async Task

            Receive_HeartbeatResponseError(HeartbeatRequest?              Request,
                                           HeartbeatResponse?             Response,
                                           OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                           IWebSocketConnection           Connection,
                                           SourceRouting                  SourceRouting,
                                           NetworkPath                    NetworkPath,
                                           EventTracking_Id               EventTrackingId,
                                           Request_Id                     RequestId,
                                           DateTime?                      ResponseTimestamp   = null,
                                           CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomHeartbeatResponseSerializer,
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

            #region Send HeartbeatResponseErrorReceived event

            await LogEvent(
                      HeartbeatResponseErrorReceived,
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
