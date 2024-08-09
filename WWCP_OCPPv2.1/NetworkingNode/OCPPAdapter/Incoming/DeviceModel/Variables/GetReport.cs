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
    /// A logging delegate called whenever a GetReport request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetReportRequestReceivedDelegate(DateTime               Timestamp,
                                                            IEventSender           Sender,
                                                            IWebSocketConnection   Connection,
                                                            GetReportRequest       Request,
                                                            CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetReport response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetReportResponseReceivedDelegate(DateTime               Timestamp,
                                                             IEventSender           Sender,
                                                             IWebSocketConnection   Connection,
                                                             GetReportRequest?      Request,
                                                             GetReportResponse      Response,
                                                             TimeSpan?              Runtime,
                                                             CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetReport request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetReportRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                 IEventSender                   Sender,
                                                                 IWebSocketConnection           Connection,
                                                                 GetReportRequest?              Request,
                                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                 TimeSpan?                      Runtime,
                                                                 CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a GetReport response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetReportResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                  IEventSender                    Sender,
                                                                  IWebSocketConnection            Connection,
                                                                  GetReportRequest?               Request,
                                                                  GetReportResponse?              Response,
                                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                  TimeSpan?                       Runtime,
                                                                  CancellationToken               CancellationToken = default);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetReport response is expected
    /// for a received GetReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetReportResponse>

        OnGetReportDelegate(DateTime               Timestamp,
                            IEventSender           Sender,
                            IWebSocketConnection   Connection,
                            GetReportRequest       Request,
                            CancellationToken      CancellationToken = default);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetReport request

        /// <summary>
        /// An event sent whenever a GetReport request was received.
        /// </summary>
        public event OnGetReportRequestReceivedDelegate?  OnGetReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetReport request was received for processing.
        /// </summary>
        public event OnGetReportDelegate?                 OnGetReport;


        public async Task<OCPP_Response>

            Receive_GetReport(DateTime              RequestTimestamp,
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

                if (GetReportRequest.TryParse(JSONRequest,
                                          RequestId,
                                          DestinationId,
                                          NetworkPath,
                                          out var request,
                                          out var errorResponse,
                                          RequestTimestamp,
                                          parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                          EventTrackingId,
                                          parentNetworkingNode.OCPP.CustomGetReportRequestParser)) {

                    GetReportResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetReportRequestSerializer,
                            parentNetworkingNode.OCPP.CustomComponentVariableSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetReportResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetReportRequestReceived event

                    await LogEvent(
                              OnGetReportRequestReceived,
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

                            var responseTasks = OnGetReport?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnGetReportDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : GetReportResponse.Failed(request, $"Undefined {nameof(OnGetReport)}!");

                        }
                        catch (Exception e)
                        {

                            response = GetReportResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnGetReport),
                                      e
                                  );

                        }
                    }

                    response ??= GetReportResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion


                    #region Send OnGetReportResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnGetReportResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
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
                                       nameof(Receive_GetReport)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetReport)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetReport response

        /// <summary>
        /// An event fired whenever a GetReport response was received.
        /// </summary>
        public event OnGetReportResponseReceivedDelegate? OnGetReportResponseReceived;


        public async Task<GetReportResponse>

            Receive_GetReportResponse(GetReportRequest      Request,
                                      JObject               ResponseJSON,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id     DestinationId,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      DateTime?             ResponseTimestamp   = null,
                                      CancellationToken     CancellationToken   = default)

        {

            GetReportResponse? response = null;

            try
            {

                if (GetReportResponse.TryParse(Request,
                                               ResponseJSON,
                                               DestinationId,
                                               NetworkPath,
                                               out response,
                                               out var errorResponse,
                                               ResponseTimestamp,
                                               parentNetworkingNode.OCPP.CustomGetReportResponseParser,
                                               parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                               parentNetworkingNode.OCPP.CustomSignatureParser,
                                               parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetReportResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetReportResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetReportResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetReportResponseReceived event

            await LogEvent(
                      OnGetReportResponseReceived,
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

        #region Receive GetReport request error

        /// <summary>
        /// An event fired whenever a GetReport request error was received.
        /// </summary>
        public event OnGetReportRequestErrorReceivedDelegate? GetReportRequestErrorReceived;


        public async Task<GetReportResponse>

            Receive_GetReportRequestError(GetReportRequest              Request,
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
            //        parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
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

            #region Send GetReportRequestErrorReceived event

            await LogEvent(
                      GetReportRequestErrorReceived,
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


            var response = GetReportResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetReportResponseReceived event

            await LogEvent(
                      OnGetReportResponseReceived,
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

        #region Receive GetReport response error

        /// <summary>
        /// An event fired whenever a GetReport response error was received.
        /// </summary>
        public event OnGetReportResponseErrorReceivedDelegate? GetReportResponseErrorReceived;


        public async Task

            Receive_GetReportResponseError(GetReportRequest?              Request,
                                           GetReportResponse?             Response,
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
            //        parentNetworkingNode.OCPP.CustomGetReportResponseSerializer,
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

            #region Send GetReportResponseErrorReceived event

            await LogEvent(
                      GetReportResponseErrorReceived,
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
