/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A logging delegate called whenever a NotifyNetworkTimeResult request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTimeResultRequestReceivedDelegate(DateTimeOffset         Timestamp,
                                                            IEventSender           Sender,
                                                            IMessageConnection     Connection,
                                                            NotifyNetworkTimeResultRequest       Request,
                                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTimeResult response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTimeResultResponseReceivedDelegate(DateTimeOffset          Timestamp,
                                                             IEventSender            Sender,
                                                             IMessageConnection?     Connection,
                                                             NotifyNetworkTimeResultRequest?       Request,
                                                             NotifyNetworkTimeResultResponse       Response,
                                                             TimeSpan?               Runtime,
                                                             CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTimeResult request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTimeResultRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                 IEventSender                   Sender,
                                                                 IMessageConnection             Connection,
                                                                 NotifyNetworkTimeResultRequest?              Request,
                                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                 TimeSpan?                      Runtime,
                                                                 CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyNetworkTimeResult response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyNetworkTimeResultResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                  IEventSender                    Sender,
                                                                  IMessageConnection              Connection,
                                                                  NotifyNetworkTimeResultRequest?               Request,
                                                                  NotifyNetworkTimeResultResponse?              Response,
                                                                  OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                  TimeSpan?                       Runtime,
                                                                  CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a NotifyNetworkTimeResult response is expected
    /// for a received NotifyNetworkTimeResult request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyNetworkTimeResultResponse>

        OnNotifyNetworkTimeResultDelegate(DateTimeOffset         Timestamp,
                            IEventSender           Sender,
                            IMessageConnection     Connection,
                            NotifyNetworkTimeResultRequest       Request,
                            CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyNetworkTimeResult request

        /// <summary>
        /// An event sent whenever a NotifyNetworkTimeResult request was received.
        /// </summary>
        public event OnNotifyNetworkTimeResultRequestReceivedDelegate?  OnNotifyNetworkTimeResultRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTimeResult request was received for processing.
        /// </summary>
        public event OnNotifyNetworkTimeResultDelegate?                 OnNotifyNetworkTimeResult;


        public async Task<OCPP_Response>

            Receive_NotifyNetworkTimeResult(DateTimeOffset        RequestTimestamp,
                              IMessageConnection    WebSocketConnection,
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

                if (NotifyNetworkTimeResultRequest.TryParse(JSONRequest,
                                              RequestId,
                                          Destination,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              RequestTimestamp,
                                              parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                              EventTrackingId,
                                              parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultRequestParser)) {

                    NotifyNetworkTimeResultResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse,
                        ReceivedJSON: JSONRequest))
                    {

                        response = NotifyNetworkTimeResultResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyNetworkTimeResultRequestReceived event

                    await LogEvent(
                              OnNotifyNetworkTimeResultRequestReceived,
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
                                           OnNotifyNetworkTimeResult,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= NotifyNetworkTimeResultResponse.Failed(request);

                    // A request that is refused is answered with a CALLERROR SecurityError.
                    if (SecurityErrorFor(response.Result, EventTrackingId, NetworkPath, RequestId) is OCPP_Response securityError)
                        return securityError;


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseSerializer,
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
                                           false,
                                           parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnNotifyNetworkTimeResultResponseSent(
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
                                       nameof(Receive_NotifyNetworkTimeResult)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyNetworkTimeResult)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyNetworkTimeResult response

        /// <summary>
        /// An event fired whenever a NotifyNetworkTimeResult response was received.
        /// </summary>
        public event OnNotifyNetworkTimeResultResponseReceivedDelegate? OnNotifyNetworkTimeResultResponseReceived;


        public async Task<NotifyNetworkTimeResultResponse>

            Receive_NotifyNetworkTimeResultResponse(NotifyNetworkTimeResultRequest      Request,
                                      JObject               ResponseJSON,
                                      IMessageConnection    WebSocketConnection,
                                      SourceRouting         Destination,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      DateTimeOffset?       ResponseTimestamp   = null,
                                      CancellationToken     CancellationToken   = default)

        {

            NotifyNetworkTimeResultResponse? response = null;

            try
            {

                if (NotifyNetworkTimeResultResponse.TryParse(Request,
                                               ResponseJSON,
                                           Destination,
                                               NetworkPath,
                                               out response,
                                               out var errorResponse,
                                               ResponseTimestamp,
                                               parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseParser,
                                               parentNetworkingNode.OCPP.CustomSignatureParser,
                                               parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse,
                            ReceivedJSON: ResponseJSON
                        ))
                    {

                        response = NotifyNetworkTimeResultResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                {

                    response = NotifyNetworkTimeResultResponse.FormationViolation(
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

                response = NotifyNetworkTimeResultResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnNotifyNetworkTimeResultResponseReceived event

            await LogEvent(
                      OnNotifyNetworkTimeResultResponseReceived,
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

        #region Receive NotifyNetworkTimeResult request error

        /// <summary>
        /// An event fired whenever a NotifyNetworkTimeResult request error was received.
        /// </summary>
        public event OnNotifyNetworkTimeResultRequestErrorReceivedDelegate? NotifyNetworkTimeResultRequestErrorReceived;


        public async Task<NotifyNetworkTimeResultResponse>

            Receive_NotifyNetworkTimeResultRequestError(NotifyNetworkTimeResultRequest              Request,
                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                          IMessageConnection            Connection,
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
            //        parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseSerializer,
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

            #region Send NotifyNetworkTimeResultRequestErrorReceived event

            await LogEvent(
                      NotifyNetworkTimeResultRequestErrorReceived,
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


            var response = NotifyNetworkTimeResultResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNotifyNetworkTimeResultResponseReceived event

            await LogEvent(
                      OnNotifyNetworkTimeResultResponseReceived,
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

        #region Receive NotifyNetworkTimeResult response error

        /// <summary>
        /// An event fired whenever a NotifyNetworkTimeResult response error was received.
        /// </summary>
        public event OnNotifyNetworkTimeResultResponseErrorReceivedDelegate? NotifyNetworkTimeResultResponseErrorReceived;


        public async Task

            Receive_NotifyNetworkTimeResultResponseError(NotifyNetworkTimeResultRequest?              Request,
                                           NotifyNetworkTimeResultResponse?             Response,
                                           OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                           IMessageConnection             Connection,
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
            //        parentNetworkingNode.OCPP.CustomNotifyNetworkTimeResultResponseSerializer,
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

            #region Send NotifyNetworkTimeResultResponseErrorReceived event

            await LogEvent(
                      NotifyNetworkTimeResultResponseErrorReceived,
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
