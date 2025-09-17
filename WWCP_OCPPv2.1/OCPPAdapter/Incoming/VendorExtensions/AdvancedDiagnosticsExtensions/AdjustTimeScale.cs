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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an AdjustTimeScale request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAdjustTimeScaleRequestReceivedDelegate(DateTimeOffset           Timestamp,
                                                                  IEventSender             Sender,
                                                                  IWebSocketConnection     Connection,
                                                                  AdjustTimeScaleRequest   Request,
                                                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AdjustTimeScale response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAdjustTimeScaleResponseReceivedDelegate(DateTimeOffset            Timestamp,
                                                                   IEventSender              Sender,
                                                                   IWebSocketConnection?     Connection,
                                                                   AdjustTimeScaleRequest?   Request,
                                                                   AdjustTimeScaleResponse   Response,
                                                                   TimeSpan?                 Runtime,
                                                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AdjustTimeScale request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAdjustTimeScaleRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                       IEventSender                   Sender,
                                                                       IWebSocketConnection           Connection,
                                                                       AdjustTimeScaleRequest?        Request,
                                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                       TimeSpan?                      Runtime,
                                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AdjustTimeScale response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAdjustTimeScaleResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                        IEventSender                    Sender,
                                                                        IWebSocketConnection            Connection,
                                                                        AdjustTimeScaleRequest?         Request,
                                                                        AdjustTimeScaleResponse?        Response,
                                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                        TimeSpan?                       Runtime,
                                                                        CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an AdjustTimeScale response is expected
    /// for a received AdjustTimeScale request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AdjustTimeScaleResponse>

        OnAdjustTimeScaleDelegate(DateTimeOffset           Timestamp,
                                  IEventSender             Sender,
                                  IWebSocketConnection     Connection,
                                  AdjustTimeScaleRequest   Request,
                                  CancellationToken        CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive AdjustTimeScale request (JSON)

        /// <summary>
        /// An event sent whenever an AdjustTimeScale request was received.
        /// </summary>
        public event OnAdjustTimeScaleRequestReceivedDelegate?  OnAdjustTimeScaleRequestReceived;

        /// <summary>
        /// An event sent whenever an AdjustTimeScale request was received for processing.
        /// </summary>
        public event OnAdjustTimeScaleDelegate?                 OnAdjustTimeScale;


        public async Task<OCPP_Response>

            Receive_AdjustTimeScale(DateTimeOffset        RequestTimestamp,
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

                if (AdjustTimeScaleRequest.TryParse(JSONRequest,
                                                    RequestId,
                                                    Destination,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    RequestTimestamp,
                                                    parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                    EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomAdjustTimeScaleRequestParser)) {

                    AdjustTimeScaleResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomAdjustTimeScaleRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = AdjustTimeScaleResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnAdjustTimeScaleRequestReceived event

                    await LogEvent(
                              OnAdjustTimeScaleRequestReceived,
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
                                           OnAdjustTimeScale,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= AdjustTimeScaleResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnAdjustTimeScaleResponseSent(
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
                                       nameof(Receive_AdjustTimeScale)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_AdjustTimeScale)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive AdjustTimeScale response (JSON)

        /// <summary>
        /// An event fired whenever an AdjustTimeScale response was received.
        /// </summary>
        public event OnAdjustTimeScaleResponseReceivedDelegate? OnAdjustTimeScaleResponseReceived;


        public async Task<AdjustTimeScaleResponse>

            Receive_AdjustTimeScaleResponse(AdjustTimeScaleRequest  Request,
                                            JObject                 ResponseJSON,
                                            IWebSocketConnection    WebSocketConnection,
                                            SourceRouting           Destination,
                                            NetworkPath             NetworkPath,
                                            EventTracking_Id        EventTrackingId,
                                            Request_Id              RequestId,
                                            DateTimeOffset?         ResponseTimestamp   = null,
                                            CancellationToken       CancellationToken   = default)

        {

            AdjustTimeScaleResponse? response = null;

            try
            {

                if (AdjustTimeScaleResponse.TryParse(Request,
                                                     ResponseJSON,
                                                     Destination,
                                                     NetworkPath,
                                                     out response,
                                                     out var errorResponse,
                                                     ResponseTimestamp,
                                                     parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = AdjustTimeScaleResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = AdjustTimeScaleResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = AdjustTimeScaleResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnAdjustTimeScaleResponseReceived event

            await LogEvent(
                      OnAdjustTimeScaleResponseReceived,
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


        #region Receive AdjustTimeScale request error

        /// <summary>
        /// An event fired whenever an AdjustTimeScale request error was received.
        /// </summary>
        public event OnAdjustTimeScaleRequestErrorReceivedDelegate? AdjustTimeScaleRequestErrorReceived;


        public async Task<AdjustTimeScaleResponse>

            Receive_AdjustTimeScaleRequestError(AdjustTimeScaleRequest        Request,
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
            //        parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
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

            #region Send AdjustTimeScaleRequestErrorReceived event

            await LogEvent(
                      AdjustTimeScaleRequestErrorReceived,
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


            var response = AdjustTimeScaleResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnAdjustTimeScaleResponseReceived event

            await LogEvent(
                      OnAdjustTimeScaleResponseReceived,
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


        public async Task<AdjustTimeScaleResponse>

            Receive_AdjustTimeScaleRequestError(AdjustTimeScaleRequest          Request,
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
            //        parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
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

            #region Send AdjustTimeScaleRequestErrorReceived event

            //await LogEvent(
            //          AdjustTimeScaleRequestErrorReceived,
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


            var response = AdjustTimeScaleResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnAdjustTimeScaleResponseReceived event

            await LogEvent(
                      OnAdjustTimeScaleResponseReceived,
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

        #region Receive AdjustTimeScale response error

        /// <summary>
        /// An event fired whenever an AdjustTimeScale response error was received.
        /// </summary>
        public event OnAdjustTimeScaleResponseErrorReceivedDelegate? AdjustTimeScaleResponseErrorReceived;


        public async Task

            Receive_AdjustTimeScaleResponseError(AdjustTimeScaleRequest?        Request,
                                                 AdjustTimeScaleResponse?       Response,
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
            //        parentNetworkingNode.OCPP.CustomAdjustTimeScaleResponseSerializer,
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

            #region Send AdjustTimeScaleResponseErrorReceived event

            await LogEvent(
                      AdjustTimeScaleResponseErrorReceived,
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
