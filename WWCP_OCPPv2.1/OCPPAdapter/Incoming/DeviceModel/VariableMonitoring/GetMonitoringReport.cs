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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a GetMonitoringReport request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetMonitoringReportRequestReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection         Connection,
                                                                      GetMonitoringReportRequest   Request,
                                                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetMonitoringReport response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetMonitoringReportResponseReceivedDelegate(DateTime                      Timestamp,
                                                                       IEventSender                  Sender,
                                                                       IWebSocketConnection?         Connection,
                                                                       GetMonitoringReportRequest?   Request,
                                                                       GetMonitoringReportResponse   Response,
                                                                       TimeSpan?                     Runtime,
                                                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetMonitoringReport request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetMonitoringReportRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                           IEventSender                   Sender,
                                                                           IWebSocketConnection           Connection,
                                                                           GetMonitoringReportRequest?    Request,
                                                                           OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                           TimeSpan?                      Runtime,
                                                                           CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetMonitoringReport response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetMonitoringReportResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                            IEventSender                    Sender,
                                                                            IWebSocketConnection            Connection,
                                                                            GetMonitoringReportRequest?     Request,
                                                                            GetMonitoringReportResponse?    Response,
                                                                            OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                            TimeSpan?                       Runtime,
                                                                            CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetMonitoringReport response is expected
    /// for a received GetMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetMonitoringReportResponse>

        OnGetMonitoringReportDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      IWebSocketConnection         Connection,
                                      GetMonitoringReportRequest   Request,
                                      CancellationToken            CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetMonitoringReport request

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was received.
        /// </summary>
        public event OnGetMonitoringReportRequestReceivedDelegate?  OnGetMonitoringReportRequestReceived;

        /// <summary>
        /// An event sent whenever a GetMonitoringReport request was received for processing.
        /// </summary>
        public event OnGetMonitoringReportDelegate?                 OnGetMonitoringReport;


        public async Task<OCPP_Response>

            Receive_GetMonitoringReport(DateTime              RequestTimestamp,
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

                if (GetMonitoringReportRequest.TryParse(JSONRequest,
                                                        RequestId,
                                                    Destination,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        RequestTimestamp,
                                                        parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                        EventTrackingId,
                                                        parentNetworkingNode.OCPP.CustomGetMonitoringReportRequestParser)) {

                    GetMonitoringReportResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetMonitoringReportRequestSerializer,
                            parentNetworkingNode.OCPP.CustomComponentVariableSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetMonitoringReportResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetMonitoringReportRequestReceived event

                    await LogEvent(
                              OnGetMonitoringReportRequestReceived,
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
                                           OnGetMonitoringReport,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetMonitoringReportResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetMonitoringReportResponseSent(
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
                                       nameof(Receive_GetMonitoringReport)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetMonitoringReport)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetMonitoringReport response

        /// <summary>
        /// An event fired whenever a GetMonitoringReport response was received.
        /// </summary>
        public event OnGetMonitoringReportResponseReceivedDelegate? OnGetMonitoringReportResponseReceived;


        public async Task<GetMonitoringReportResponse>

            Receive_GetMonitoringReportResponse(GetMonitoringReportRequest  Request,
                                                JObject                     ResponseJSON,
                                                IWebSocketConnection        WebSocketConnection,
                                                SourceRouting               Destination,
                                                NetworkPath                 NetworkPath,
                                                EventTracking_Id            EventTrackingId,
                                                Request_Id                  RequestId,
                                                DateTime?                   ResponseTimestamp   = null,
                                                CancellationToken           CancellationToken   = default)

        {

            GetMonitoringReportResponse? response = null;

            try
            {

                if (GetMonitoringReportResponse.TryParse(Request,
                                                         ResponseJSON,
                                                     Destination,
                                                         NetworkPath,
                                                         out response,
                                                         out var errorResponse,
                                                         ResponseTimestamp,
                                                         parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseParser,
                                                         parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                         parentNetworkingNode.OCPP.CustomSignatureParser,
                                                         parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetMonitoringReportResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetMonitoringReportResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetMonitoringReportResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetMonitoringReportResponseReceived event

            await LogEvent(
                      OnGetMonitoringReportResponseReceived,
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

        #region Receive GetMonitoringReport request error

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request error was received.
        /// </summary>
        public event OnGetMonitoringReportRequestErrorReceivedDelegate? GetMonitoringReportRequestErrorReceived;


        public async Task<GetMonitoringReportResponse>

            Receive_GetMonitoringReportRequestError(GetMonitoringReportRequest    Request,
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
            //        parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
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

            #region Send GetMonitoringReportRequestErrorReceived event

            await LogEvent(
                      GetMonitoringReportRequestErrorReceived,
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


            var response = GetMonitoringReportResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetMonitoringReportResponseReceived event

            await LogEvent(
                      OnGetMonitoringReportResponseReceived,
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

        #region Receive GetMonitoringReport response error

        /// <summary>
        /// An event fired whenever a GetMonitoringReport response error was received.
        /// </summary>
        public event OnGetMonitoringReportResponseErrorReceivedDelegate? GetMonitoringReportResponseErrorReceived;


        public async Task

            Receive_GetMonitoringReportResponseError(GetMonitoringReportRequest?    Request,
                                                     GetMonitoringReportResponse?   Response,
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
            //        parentNetworkingNode.OCPP.CustomGetMonitoringReportResponseSerializer,
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

            #region Send GetMonitoringReportResponseErrorReceived event

            await LogEvent(
                      GetMonitoringReportResponseErrorReceived,
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
