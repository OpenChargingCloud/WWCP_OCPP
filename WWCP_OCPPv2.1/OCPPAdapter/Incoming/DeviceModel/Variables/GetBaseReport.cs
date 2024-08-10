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
    /// A logging delegate called whenever a GetBaseReport request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetBaseReportRequestReceivedDelegate(DateTime               Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                GetBaseReportRequest   Request,
                                                                CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetBaseReport response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetBaseReportResponseReceivedDelegate(DateTime                Timestamp,
                                                                 IEventSender            Sender,
                                                                 IWebSocketConnection    Connection,
                                                                 GetBaseReportRequest?   Request,
                                                                 GetBaseReportResponse   Response,
                                                                 TimeSpan?               Runtime,
                                                                 CancellationToken       CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetBaseReport request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetBaseReportRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                     IEventSender                   Sender,
                                                                     IWebSocketConnection           Connection,
                                                                     GetBaseReportRequest?          Request,
                                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                     TimeSpan?                      Runtime,
                                                                     CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetBaseReport response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetBaseReportResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                      IEventSender                    Sender,
                                                                      IWebSocketConnection            Connection,
                                                                      GetBaseReportRequest?           Request,
                                                                      GetBaseReportResponse?          Response,
                                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                      TimeSpan?                       Runtime,
                                                                      CancellationToken               CancellationToken = default);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetBaseReport response is expected
    /// for a received GetBaseReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetBaseReportResponse>

        OnGetBaseReportDelegate(DateTime               Timestamp,
                                IEventSender           Sender,
                                IWebSocketConnection   Connection,
                                GetBaseReportRequest   Request,
                                CancellationToken      CancellationToken = default);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetBaseReport request

        /// <summary>
        /// An event sent whenever a GetBaseReport request was received.
        /// </summary>
        public event OnGetBaseReportRequestReceivedDelegate?  OnGetBaseReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetBaseReport request was received for processing.
        /// </summary>
        public event OnGetBaseReportDelegate?                 OnGetBaseReport;


        public async Task<OCPP_Response>

            Receive_GetBaseReport(DateTime              RequestTimestamp,
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

                if (GetBaseReportRequest.TryParse(JSONRequest,
                                                  RequestId,
                                                  DestinationId,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  RequestTimestamp,
                                                  parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                  EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomGetBaseReportRequestParser)) {

                    GetBaseReportResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetBaseReportRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetBaseReportResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetBaseReportRequestReceived event

                    await LogEvent(
                              OnGetBaseReportRequestReceived,
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

                            var responseTasks = OnGetBaseReport?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnGetBaseReportDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : GetBaseReportResponse.Failed(request, $"Undefined {nameof(OnGetBaseReport)}!");

                        }
                        catch (Exception e)
                        {

                            response = GetBaseReportResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnGetBaseReport),
                                      e
                                  );

                        }
                    }

                    response ??= GetBaseReportResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion


                    #region Send OnGetBaseReportResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnGetBaseReportResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
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
                                       nameof(Receive_GetBaseReport)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetBaseReport)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetBaseReport response

        /// <summary>
        /// An event fired whenever a GetBaseReport response was received.
        /// </summary>
        public event OnGetBaseReportResponseReceivedDelegate? OnGetBaseReportResponseReceived;


        public async Task<GetBaseReportResponse>

            Receive_GetBaseReportResponse(GetBaseReportRequest  Request,
                                          JObject               ResponseJSON,
                                          IWebSocketConnection  WebSocketConnection,
                                          NetworkingNode_Id     DestinationId,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          DateTime?             ResponseTimestamp   = null,
                                          CancellationToken     CancellationToken   = default)

        {

            GetBaseReportResponse? response = null;

            try
            {

                if (GetBaseReportResponse.TryParse(Request,
                                                   ResponseJSON,
                                                   DestinationId,
                                                   NetworkPath,
                                                   out response,
                                                   out var errorResponse,
                                                   ResponseTimestamp,
                                                   parentNetworkingNode.OCPP.CustomGetBaseReportResponseParser,
                                                   parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                   parentNetworkingNode.OCPP.CustomSignatureParser,
                                                   parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetBaseReportResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetBaseReportResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetBaseReportResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetBaseReportResponseReceived event

            await LogEvent(
                      OnGetBaseReportResponseReceived,
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

        #region Receive GetBaseReport request error

        /// <summary>
        /// An event fired whenever a GetBaseReport request error was received.
        /// </summary>
        public event OnGetBaseReportRequestErrorReceivedDelegate? GetBaseReportRequestErrorReceived;


        public async Task<GetBaseReportResponse>

            Receive_GetBaseReportRequestError(GetBaseReportRequest          Request,
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
            //        parentNetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
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

            #region Send GetBaseReportRequestErrorReceived event

            await LogEvent(
                      GetBaseReportRequestErrorReceived,
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


            var response = GetBaseReportResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetBaseReportResponseReceived event

            await LogEvent(
                      OnGetBaseReportResponseReceived,
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

        #region Receive GetBaseReport response error

        /// <summary>
        /// An event fired whenever a GetBaseReport response error was received.
        /// </summary>
        public event OnGetBaseReportResponseErrorReceivedDelegate? GetBaseReportResponseErrorReceived;


        public async Task

            Receive_GetBaseReportResponseError(GetBaseReportRequest?          Request,
                                               GetBaseReportResponse?         Response,
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
            //        parentNetworkingNode.OCPP.CustomGetBaseReportResponseSerializer,
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

            #region Send GetBaseReportResponseErrorReceived event

            await LogEvent(
                      GetBaseReportResponseErrorReceived,
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
