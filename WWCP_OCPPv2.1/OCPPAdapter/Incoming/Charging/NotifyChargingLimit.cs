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
    /// A logging delegate called whenever a NotifyChargingLimit request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyChargingLimitRequestReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection         Connection,
                                                                      NotifyChargingLimitRequest   Request,
                                                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyChargingLimit response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyChargingLimitResponseReceivedDelegate(DateTime                      Timestamp,
                                                                       IEventSender                  Sender,
                                                                       IWebSocketConnection?         Connection,
                                                                       NotifyChargingLimitRequest?   Request,
                                                                       NotifyChargingLimitResponse   Response,
                                                                       TimeSpan?                     Runtime,
                                                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyChargingLimit request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyChargingLimitRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                           IEventSender                   Sender,
                                                                           IWebSocketConnection           Connection,
                                                                           NotifyChargingLimitRequest?    Request,
                                                                           OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                           TimeSpan?                      Runtime,
                                                                           CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyChargingLimit response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyChargingLimitResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                            IEventSender                    Sender,
                                                                            IWebSocketConnection            Connection,
                                                                            NotifyChargingLimitRequest?     Request,
                                                                            NotifyChargingLimitResponse?    Response,
                                                                            OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                            TimeSpan?                       Runtime,
                                                                            CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a NotifyChargingLimit response is expected
    /// for a received NotifyChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyChargingLimitResponse>

        OnNotifyChargingLimitDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      IWebSocketConnection         Connection,
                                      NotifyChargingLimitRequest   Request,
                                      CancellationToken            CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyChargingLimit request

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received.
        /// </summary>
        public event OnNotifyChargingLimitRequestReceivedDelegate?  OnNotifyChargingLimitRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyChargingLimit request was received for processing.
        /// </summary>
        public event OnNotifyChargingLimitDelegate?                 OnNotifyChargingLimit;


        public async Task<OCPP_Response>

            Receive_NotifyChargingLimit(DateTime              RequestTimestamp,
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

                if (NotifyChargingLimitRequest.TryParse(JSONRequest,
                                                        RequestId,
                                                    Destination,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        RequestTimestamp,
                                                        parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                        EventTrackingId,
                                                        parentNetworkingNode.OCPP.CustomNotifyChargingLimitRequestParser)) {

                    NotifyChargingLimitResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(

                            parentNetworkingNode.OCPP.CustomNotifyChargingLimitRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomLimitBeyondSoCSerializer,
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

                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer

                        ),
                        out errorResponse))
                    {

                        response = NotifyChargingLimitResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyChargingLimitRequestReceived event

                    await LogEvent(
                              OnNotifyChargingLimitRequestReceived,
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
                                           OnNotifyChargingLimit,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= NotifyChargingLimitResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnNotifyChargingLimitResponseSent(
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
                                       nameof(Receive_NotifyChargingLimit)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyChargingLimit)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyChargingLimit response

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit response was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseReceivedDelegate? OnNotifyChargingLimitResponseReceived;


        public async Task<NotifyChargingLimitResponse>

            Receive_NotifyChargingLimitResponse(NotifyChargingLimitRequest  Request,
                                                JObject                     ResponseJSON,
                                                IWebSocketConnection        WebSocketConnection,
                                                SourceRouting           Destination,
                                                NetworkPath                 NetworkPath,
                                                EventTracking_Id            EventTrackingId,
                                                Request_Id                  RequestId,
                                                DateTime?                   ResponseTimestamp   = null,
                                                CancellationToken           CancellationToken   = default)

        {

            NotifyChargingLimitResponse? response = null;

            try
            {

                if (NotifyChargingLimitResponse.TryParse(Request,
                                                         ResponseJSON,
                                                     Destination,
                                                         NetworkPath,
                                                         out response,
                                                         out var errorResponse,
                                                         ResponseTimestamp,
                                                         parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseParser,
                                                         parentNetworkingNode.OCPP.CustomSignatureParser,
                                                         parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = NotifyChargingLimitResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = NotifyChargingLimitResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = NotifyChargingLimitResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnNotifyChargingLimitResponseReceived event

            await LogEvent(
                      OnNotifyChargingLimitResponseReceived,
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

        #region Receive NotifyChargingLimit request error

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request error was received.
        /// </summary>
        public event OnNotifyChargingLimitRequestErrorReceivedDelegate? NotifyChargingLimitRequestErrorReceived;


        public async Task<NotifyChargingLimitResponse>

            Receive_NotifyChargingLimitRequestError(NotifyChargingLimitRequest    Request,
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
            //        parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
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

            #region Send NotifyChargingLimitRequestErrorReceived event

            await LogEvent(
                      NotifyChargingLimitRequestErrorReceived,
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


            var response = NotifyChargingLimitResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNotifyChargingLimitResponseReceived event

            await LogEvent(
                      OnNotifyChargingLimitResponseReceived,
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

        #region Receive NotifyChargingLimit response error

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit response error was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseErrorReceivedDelegate? NotifyChargingLimitResponseErrorReceived;


        public async Task

            Receive_NotifyChargingLimitResponseError(NotifyChargingLimitRequest?    Request,
                                                     NotifyChargingLimitResponse?   Response,
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
            //        parentNetworkingNode.OCPP.CustomNotifyChargingLimitResponseSerializer,
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

            #region Send NotifyChargingLimitResponseErrorReceived event

            await LogEvent(
                      NotifyChargingLimitResponseErrorReceived,
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
