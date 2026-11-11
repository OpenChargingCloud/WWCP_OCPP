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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a QueryDNS request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnQueryDNSRequestReceivedDelegate(DateTimeOffset                   Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IMessageConnection               Connection,
                                                                          QueryDNSRequest   Request,
                                                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a QueryDNS response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnQueryDNSResponseReceivedDelegate(DateTimeOffset                    Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IMessageConnection?               Connection,
                                                                           QueryDNSRequest?   Request,
                                                                           QueryDNSResponse   Response,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a QueryDNS request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnQueryDNSRequestErrorReceivedDelegate(DateTimeOffset                    Timestamp,
                                                                               IEventSender                      Sender,
                                                                               IMessageConnection                Connection,
                                                                               QueryDNSRequest?   Request,
                                                                               OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                               TimeSpan?                         Runtime,
                                                                               CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a QueryDNS response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnQueryDNSResponseErrorReceivedDelegate(DateTimeOffset                     Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IMessageConnection                 Connection,
                                                                                QueryDNSRequest?    Request,
                                                                                QueryDNSResponse?   Response,
                                                                                OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a QueryDNS response is expected
    /// for a received QueryDNS request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<QueryDNSResponse>

        OnQueryDNSDelegate(DateTimeOffset                   Timestamp,
                                          IEventSender                     Sender,
                                          IMessageConnection               Connection,
                                          QueryDNSRequest   Request,
                                          CancellationToken                CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive QueryDNS request (JSON)

        /// <summary>
        /// An event sent whenever a QueryDNS request was received.
        /// </summary>
        public event OnQueryDNSRequestReceivedDelegate?  OnQueryDNSRequestReceived;

        /// <summary>
        /// An event sent whenever a QueryDNS request was received for processing.
        /// </summary>
        public event OnQueryDNSDelegate?                 OnQueryDNS;


        public async Task<OCPP_Response>

            Receive_QueryDNS(DateTimeOffset        RequestTimestamp,
                                            IMessageConnection    WebSocketConnection,
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

                if (QueryDNSRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                            Destination,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomQueryDNSRequestParser)) {

                    QueryDNSResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomQueryDNSRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse,
                        ReceivedJSON: JSONRequest))
                    {

                        response = QueryDNSResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnQueryDNSRequestReceived event

                    await LogEvent(
                              OnQueryDNSRequestReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  request,
                                  CancellationToken
                              )
                          );

                    #endregion


                    response ??= await CallProcessor(
                                           OnQueryDNS,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= QueryDNSResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnQueryDNSResponseSent(
                                                                            Now,
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
                                       nameof(Receive_QueryDNS)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_QueryDNS)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive QueryDNS response (JSON)

        /// <summary>
        /// An event fired whenever a QueryDNS response was received.
        /// </summary>
        public event OnQueryDNSResponseReceivedDelegate? OnQueryDNSResponseReceived;


        public async Task<QueryDNSResponse>

            Receive_QueryDNSResponse(QueryDNSRequest  Request,
                                                    JObject                         ResponseJSON,
                                                    IMessageConnection              WebSocketConnection,
                                                    SourceRouting                   Destination,
                                                    NetworkPath                     NetworkPath,
                                                    EventTracking_Id                EventTrackingId,
                                                    Request_Id                      RequestId,
                                                    DateTimeOffset?                 ResponseTimestamp   = null,
                                                    CancellationToken               CancellationToken   = default)

        {

            QueryDNSResponse? response = null;

            try
            {

                if (QueryDNSResponse.TryParse(Request,
                                                             ResponseJSON,
                                                             Destination,
                                                             NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             ResponseTimestamp,
                                                             parentNetworkingNode.OCPP.CustomQueryDNSResponseParser,
                                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse,
                            ReceivedJSON: ResponseJSON
                        ))
                    {

                        response = QueryDNSResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                {

                    response = QueryDNSResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

                    await TellTheAnswererItCannotBeRead(
                              WebSocketConnection,
                              NetworkPath,
                              RequestId,
                              EventTrackingId,
                              Binary: false,
                              errorResponse,
                              CancellationToken
                          );

                }

            }
            catch (Exception e)
            {

                response = QueryDNSResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnQueryDNSResponseReceived event

            await LogEvent(
                      OnQueryDNSResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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


        #region Receive QueryDNS request error

        /// <summary>
        /// An event fired whenever a QueryDNS request error was received.
        /// </summary>
        public event OnQueryDNSRequestErrorReceivedDelegate? QueryDNSRequestErrorReceived;


        public async Task<QueryDNSResponse>

            Receive_QueryDNSRequestError(QueryDNSRequest  Request,
                                                        OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                        IMessageConnection              Connection,
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
            //        parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
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

            #region Send QueryDNSRequestErrorReceived event

            await LogEvent(
                      QueryDNSRequestErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
                          parentNetworkingNode,
                          Connection,
                          Request,
                          RequestErrorMessage,
                          RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
                          CancellationToken
                      )
                  );

            #endregion


            var response = QueryDNSResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnQueryDNSResponseReceived event

            await LogEvent(
                      OnQueryDNSResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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


        public async Task<QueryDNSResponse>

            Receive_QueryDNSRequestError(QueryDNSRequest  Request,
                                                        OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                                        IMessageConnection              Connection,
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
            //        parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
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

            #region Send QueryDNSRequestErrorReceived event

            //await LogEvent(
            //          QueryDNSRequestErrorReceived,
            //          loggingDelegate => loggingDelegate.Invoke(
            //              Now,
            //              parentNetworkingNode,
            //              Connection,
            //              Request,
            //              RequestErrorMessage,
            //              RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
            //              CancellationToken
            //          )
            //      );

            #endregion


            var response = QueryDNSResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnQueryDNSResponseReceived event

            await LogEvent(
                      OnQueryDNSResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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

        #region Receive QueryDNS response error

        /// <summary>
        /// An event fired whenever a QueryDNS response error was received.
        /// </summary>
        public event OnQueryDNSResponseErrorReceivedDelegate? QueryDNSResponseErrorReceived;


        public async Task

            Receive_QueryDNSResponseError(QueryDNSRequest?   Request,
                                                         QueryDNSResponse?  Response,
                                                         OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                         IMessageConnection                Connection,
                                                         SourceRouting                     Destination,
                                                         NetworkPath                       NetworkPath,
                                                         EventTracking_Id                  EventTrackingId,
                                                         Request_Id                        RequestId,
                                                         DateTimeOffset?                   ResponseTimestamp   = null,
                                                         CancellationToken                 CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomQueryDNSResponseSerializer,
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

            #region Send QueryDNSResponseErrorReceived event

            await LogEvent(
                      QueryDNSResponseErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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
