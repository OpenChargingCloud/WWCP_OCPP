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
    /// A logging delegate called whenever a RequestStartTransaction request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRequestStartTransactionRequestReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IWebSocketConnection             Connection,
                                                                          RequestStartTransactionRequest   Request,
                                                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a RequestStartTransaction response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRequestStartTransactionResponseReceivedDelegate(DateTime                          Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IWebSocketConnection?             Connection,
                                                                           RequestStartTransactionRequest?   Request,
                                                                           RequestStartTransactionResponse   Response,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a RequestStartTransaction request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRequestStartTransactionRequestErrorReceivedDelegate(DateTime                          Timestamp,
                                                                               IEventSender                      Sender,
                                                                               IWebSocketConnection              Connection,
                                                                               RequestStartTransactionRequest?   Request,
                                                                               OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                               TimeSpan?                         Runtime,
                                                                               CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a RequestStartTransaction response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRequestStartTransactionResponseErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                RequestStartTransactionRequest?    Request,
                                                                                RequestStartTransactionResponse?   Response,
                                                                                OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a RequestStartTransaction response is expected
    /// for a received RequestStartTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestStartTransactionResponse>

        OnRequestStartTransactionDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          IWebSocketConnection             Connection,
                                          RequestStartTransactionRequest   Request,
                                          CancellationToken                CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive RequestStartTransaction request

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was received.
        /// </summary>
        public event OnRequestStartTransactionRequestReceivedDelegate?  OnRequestStartTransactionRequestReceived;

        /// <summary>
        /// An event sent whenever a RequestStartTransaction request was received for processing.
        /// </summary>
        public event OnRequestStartTransactionDelegate?                 OnRequestStartTransaction;


        public async Task<OCPP_Response>

            Receive_RequestStartTransaction(DateTime              RequestTimestamp,
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

                if (RequestStartTransactionRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                        Destination,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomRequestStartTransactionRequestParser)) {

                    RequestStartTransactionResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(

                            parentNetworkingNode.OCPP.CustomRequestStartTransactionRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                            parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
                            parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                            parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                            parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                            parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                            parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                            parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                            parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                            parentNetworkingNode.OCPP.CustomCostSerializer,

                            parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                            parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                            parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                            parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                            parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                            parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,

                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer

                        ),
                        out errorResponse))
                    {

                        response = RequestStartTransactionResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnRequestStartTransactionRequestReceived event

                    await LogEvent(
                              OnRequestStartTransactionRequestReceived,
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
                                           OnRequestStartTransaction,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= RequestStartTransactionResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnRequestStartTransactionResponseSent(
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
                                       nameof(Receive_RequestStartTransaction)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_RequestStartTransaction)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive RequestStartTransaction response

        /// <summary>
        /// An event fired whenever a RequestStartTransaction response was received.
        /// </summary>
        public event OnRequestStartTransactionResponseReceivedDelegate? OnRequestStartTransactionResponseReceived;


        public async Task<RequestStartTransactionResponse>

            Receive_RequestStartTransactionResponse(RequestStartTransactionRequest  Request,
                                                    JObject                         ResponseJSON,
                                                    IWebSocketConnection            WebSocketConnection,
                                                    SourceRouting                   Destination,
                                                    NetworkPath                     NetworkPath,
                                                    EventTracking_Id                EventTrackingId,
                                                    Request_Id                      RequestId,
                                                    DateTime?                       ResponseTimestamp   = null,
                                                    CancellationToken               CancellationToken   = default)

        {

            RequestStartTransactionResponse? response = null;

            try
            {

                if (RequestStartTransactionResponse.TryParse(Request,
                                                             ResponseJSON,
                                                         Destination,
                                                             NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             ResponseTimestamp,
                                                             parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseParser,
                                                             parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = RequestStartTransactionResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = RequestStartTransactionResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = RequestStartTransactionResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnRequestStartTransactionResponseReceived event

            await LogEvent(
                      OnRequestStartTransactionResponseReceived,
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

        #region Receive RequestStartTransaction request error

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request error was received.
        /// </summary>
        public event OnRequestStartTransactionRequestErrorReceivedDelegate? RequestStartTransactionRequestErrorReceived;


        public async Task<RequestStartTransactionResponse>

            Receive_RequestStartTransactionRequestError(RequestStartTransactionRequest                  Request,
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
            //        parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
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

            #region Send RequestStartTransactionRequestErrorReceived event

            await LogEvent(
                      RequestStartTransactionRequestErrorReceived,
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


            var response = RequestStartTransactionResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnRequestStartTransactionResponseReceived event

            await LogEvent(
                      OnRequestStartTransactionResponseReceived,
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

        #region Receive RequestStartTransaction response error

        /// <summary>
        /// An event fired whenever a RequestStartTransaction response error was received.
        /// </summary>
        public event OnRequestStartTransactionResponseErrorReceivedDelegate? RequestStartTransactionResponseErrorReceived;


        public async Task

            Receive_RequestStartTransactionResponseError(RequestStartTransactionRequest?   Request,
                                                         RequestStartTransactionResponse?  Response,
                                                         OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                         IWebSocketConnection              Connection,
                                                         SourceRouting                     Destination,
                                                         NetworkPath                       NetworkPath,
                                                         EventTracking_Id                  EventTrackingId,
                                                         Request_Id                        RequestId,
                                                         DateTime?                         ResponseTimestamp   = null,
                                                         CancellationToken                 CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomRequestStartTransactionResponseSerializer,
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

            #region Send RequestStartTransactionResponseErrorReceived event

            await LogEvent(
                      RequestStartTransactionResponseErrorReceived,
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
