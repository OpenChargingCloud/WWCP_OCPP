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
    /// A logging delegate called whenever a ClearVariableMonitoring request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnClearVariableMonitoringRequestReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IWebSocketConnection             Connection,
                                                                          ClearVariableMonitoringRequest   Request,
                                                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ClearVariableMonitoring response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnClearVariableMonitoringResponseReceivedDelegate(DateTime                          Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IWebSocketConnection?             Connection,
                                                                           ClearVariableMonitoringRequest?   Request,
                                                                           ClearVariableMonitoringResponse   Response,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ClearVariableMonitoring request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnClearVariableMonitoringRequestErrorReceivedDelegate(DateTime                          Timestamp,
                                                                               IEventSender                      Sender,
                                                                               IWebSocketConnection              Connection,
                                                                               ClearVariableMonitoringRequest?   Request,
                                                                               OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                               TimeSpan?                         Runtime,
                                                                               CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ClearVariableMonitoring response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnClearVariableMonitoringResponseErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                ClearVariableMonitoringRequest?    Request,
                                                                                ClearVariableMonitoringResponse?   Response,
                                                                                OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a ClearVariableMonitoring response is expected
    /// for a received ClearVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ClearVariableMonitoringResponse>

        OnClearVariableMonitoringDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          IWebSocketConnection             Connection,
                                          ClearVariableMonitoringRequest   Request,
                                          CancellationToken                CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive ClearVariableMonitoring request

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestReceivedDelegate?  OnClearVariableMonitoringRequestReceived;

        /// <summary>
        /// An event sent whenever a ClearVariableMonitoring request was received for processing.
        /// </summary>
        public event OnClearVariableMonitoringDelegate?                 OnClearVariableMonitoring;


        public async Task<OCPP_Response>

            Receive_ClearVariableMonitoring(DateTime              RequestTimestamp,
                                            IWebSocketConnection  WebSocketConnection,
                                            SourceRouting     Destination,
                                            NetworkPath           NetworkPath,
                                            EventTracking_Id      EventTrackingId,
                                            Request_Id            RequestId,
                                            JObject               JSONRequest,
                                            CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (ClearVariableMonitoringRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                        Destination,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestParser)) {

                    ClearVariableMonitoringResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = ClearVariableMonitoringResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnClearVariableMonitoringRequestReceived event

                    await LogEvent(
                              OnClearVariableMonitoringRequestReceived,
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
                                           OnClearVariableMonitoring,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= ClearVariableMonitoringResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                            parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
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
                                           parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnClearVariableMonitoringResponseSent(
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
                                       nameof(Receive_ClearVariableMonitoring)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_ClearVariableMonitoring)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive ClearVariableMonitoring response

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring response was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseReceivedDelegate? OnClearVariableMonitoringResponseReceived;


        public async Task<ClearVariableMonitoringResponse>

            Receive_ClearVariableMonitoringResponse(ClearVariableMonitoringRequest  Request,
                                                    JObject                         ResponseJSON,
                                                    IWebSocketConnection            WebSocketConnection,
                                                    SourceRouting                   Destination,
                                                    NetworkPath                     NetworkPath,
                                                    EventTracking_Id                EventTrackingId,
                                                    Request_Id                      RequestId,
                                                    DateTime?                       ResponseTimestamp   = null,
                                                    CancellationToken               CancellationToken   = default)

        {

            ClearVariableMonitoringResponse? response = null;

            try
            {

                if (ClearVariableMonitoringResponse.TryParse(Request,
                                                             ResponseJSON,
                                                         Destination,
                                                             NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             ResponseTimestamp,
                                                             parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseParser,
                                                             parentNetworkingNode.OCPP.CustomClearMonitoringResultParser,
                                                             parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
                                parentNetworkingNode.OCPP.CustomClearMonitoringResultSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = ClearVariableMonitoringResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = ClearVariableMonitoringResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = ClearVariableMonitoringResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnClearVariableMonitoringResponseReceived event

            await LogEvent(
                      OnClearVariableMonitoringResponseReceived,
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

        #region Receive ClearVariableMonitoring request error

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request error was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestErrorReceivedDelegate? ClearVariableMonitoringRequestErrorReceived;


        public async Task<ClearVariableMonitoringResponse>

            Receive_ClearVariableMonitoringRequestError(ClearVariableMonitoringRequest  Request,
                                                        OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                        IWebSocketConnection            Connection,
                                                        SourceRouting               Destination,
                                                        NetworkPath                     NetworkPath,
                                                        EventTracking_Id                EventTrackingId,
                                                        Request_Id                      RequestId,
                                                        DateTime?                       ResponseTimestamp   = null,
                                                        CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
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

            #region Send ClearVariableMonitoringRequestErrorReceived event

            await LogEvent(
                      ClearVariableMonitoringRequestErrorReceived,
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


            var response = ClearVariableMonitoringResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnClearVariableMonitoringResponseReceived event

            await LogEvent(
                      OnClearVariableMonitoringResponseReceived,
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

        #region Receive ClearVariableMonitoring response error

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring response error was received.
        /// </summary>
        public event OnClearVariableMonitoringResponseErrorReceivedDelegate? ClearVariableMonitoringResponseErrorReceived;


        public async Task

            Receive_ClearVariableMonitoringResponseError(ClearVariableMonitoringRequest?   Request,
                                                         ClearVariableMonitoringResponse?  Response,
                                                         OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                         IWebSocketConnection              Connection,
                                                         SourceRouting                 Destination,
                                                         NetworkPath                       NetworkPath,
                                                         EventTracking_Id                  EventTrackingId,
                                                         Request_Id                        RequestId,
                                                         DateTime?                         ResponseTimestamp   = null,
                                                         CancellationToken                 CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomClearVariableMonitoringResponseSerializer,
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

            #region Send ClearVariableMonitoringResponseErrorReceived event

            await LogEvent(
                      ClearVariableMonitoringResponseErrorReceived,
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
