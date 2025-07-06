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
    /// A logging delegate called whenever a GetExecutingEnvironment request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetExecutingEnvironmentRequestReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IWebSocketConnection             Connection,
                                                                          GetExecutingEnvironmentRequest   Request,
                                                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetExecutingEnvironment response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetExecutingEnvironmentResponseReceivedDelegate(DateTime                          Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IWebSocketConnection?             Connection,
                                                                           GetExecutingEnvironmentRequest?   Request,
                                                                           GetExecutingEnvironmentResponse   Response,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetExecutingEnvironment request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetExecutingEnvironmentRequestErrorReceivedDelegate(DateTime                          Timestamp,
                                                                               IEventSender                      Sender,
                                                                               IWebSocketConnection              Connection,
                                                                               GetExecutingEnvironmentRequest?   Request,
                                                                               OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                               TimeSpan?                         Runtime,
                                                                               CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetExecutingEnvironment response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetExecutingEnvironmentResponseErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                GetExecutingEnvironmentRequest?    Request,
                                                                                GetExecutingEnvironmentResponse?   Response,
                                                                                OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetExecutingEnvironment response is expected
    /// for a received GetExecutingEnvironment request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetExecutingEnvironmentResponse>

        OnGetExecutingEnvironmentDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          IWebSocketConnection             Connection,
                                          GetExecutingEnvironmentRequest   Request,
                                          CancellationToken                CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetExecutingEnvironment request (JSON)

        /// <summary>
        /// An event sent whenever a GetExecutingEnvironment request was received.
        /// </summary>
        public event OnGetExecutingEnvironmentRequestReceivedDelegate?  OnGetExecutingEnvironmentRequestReceived;

        /// <summary>
        /// An event sent whenever a GetExecutingEnvironment request was received for processing.
        /// </summary>
        public event OnGetExecutingEnvironmentDelegate?                 OnGetExecutingEnvironment;


        public async Task<OCPP_Response>

            Receive_GetExecutingEnvironment(DateTime              RequestTimestamp,
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

                if (GetExecutingEnvironmentRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                            Destination,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentRequestParser)) {

                    GetExecutingEnvironmentResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetExecutingEnvironmentResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetExecutingEnvironmentRequestReceived event

                    await LogEvent(
                              OnGetExecutingEnvironmentRequestReceived,
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
                                           OnGetExecutingEnvironment,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetExecutingEnvironmentResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetExecutingEnvironmentResponseSent(
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
                                       nameof(Receive_GetExecutingEnvironment)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetExecutingEnvironment)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetExecutingEnvironment response (JSON)

        /// <summary>
        /// An event fired whenever a GetExecutingEnvironment response was received.
        /// </summary>
        public event OnGetExecutingEnvironmentResponseReceivedDelegate? OnGetExecutingEnvironmentResponseReceived;


        public async Task<GetExecutingEnvironmentResponse>

            Receive_GetExecutingEnvironmentResponse(GetExecutingEnvironmentRequest  Request,
                                                    JObject                         ResponseJSON,
                                                    IWebSocketConnection            WebSocketConnection,
                                                    SourceRouting                   Destination,
                                                    NetworkPath                     NetworkPath,
                                                    EventTracking_Id                EventTrackingId,
                                                    Request_Id                      RequestId,
                                                    DateTime?                       ResponseTimestamp   = null,
                                                    CancellationToken               CancellationToken   = default)

        {

            GetExecutingEnvironmentResponse? response = null;

            try
            {

                if (GetExecutingEnvironmentResponse.TryParse(Request,
                                                             ResponseJSON,
                                                             Destination,
                                                             NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             ResponseTimestamp,
                                                             parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseParser,
                                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetExecutingEnvironmentResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetExecutingEnvironmentResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetExecutingEnvironmentResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetExecutingEnvironmentResponseReceived event

            await LogEvent(
                      OnGetExecutingEnvironmentResponseReceived,
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


        #region Receive GetExecutingEnvironment request error

        /// <summary>
        /// An event fired whenever a GetExecutingEnvironment request error was received.
        /// </summary>
        public event OnGetExecutingEnvironmentRequestErrorReceivedDelegate? GetExecutingEnvironmentRequestErrorReceived;


        public async Task<GetExecutingEnvironmentResponse>

            Receive_GetExecutingEnvironmentRequestError(GetExecutingEnvironmentRequest  Request,
                                                        OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                        IWebSocketConnection            Connection,
                                                        SourceRouting                   Destination,
                                                        NetworkPath                     NetworkPath,
                                                        EventTracking_Id                EventTrackingId,
                                                        Request_Id                      RequestId,
                                                        DateTime?                       ResponseTimestamp   = null,
                                                        CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
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

            #region Send GetExecutingEnvironmentRequestErrorReceived event

            await LogEvent(
                      GetExecutingEnvironmentRequestErrorReceived,
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


            var response = GetExecutingEnvironmentResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetExecutingEnvironmentResponseReceived event

            await LogEvent(
                      OnGetExecutingEnvironmentResponseReceived,
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


        public async Task<GetExecutingEnvironmentResponse>

            Receive_GetExecutingEnvironmentRequestError(GetExecutingEnvironmentRequest  Request,
                                                        OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                                        IWebSocketConnection            Connection,
                                                        SourceRouting                   Destination,
                                                        NetworkPath                     NetworkPath,
                                                        EventTracking_Id                EventTrackingId,
                                                        Request_Id                      RequestId,
                                                        DateTime?                       ResponseTimestamp   = null,
                                                        CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
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

            #region Send GetExecutingEnvironmentRequestErrorReceived event

            //await LogEvent(
            //          GetExecutingEnvironmentRequestErrorReceived,
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


            var response = GetExecutingEnvironmentResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetExecutingEnvironmentResponseReceived event

            await LogEvent(
                      OnGetExecutingEnvironmentResponseReceived,
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

        #region Receive GetExecutingEnvironment response error

        /// <summary>
        /// An event fired whenever a GetExecutingEnvironment response error was received.
        /// </summary>
        public event OnGetExecutingEnvironmentResponseErrorReceivedDelegate? GetExecutingEnvironmentResponseErrorReceived;


        public async Task

            Receive_GetExecutingEnvironmentResponseError(GetExecutingEnvironmentRequest?   Request,
                                                         GetExecutingEnvironmentResponse?  Response,
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
            //        parentNetworkingNode.OCPP.CustomGetExecutingEnvironmentResponseSerializer,
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

            #region Send GetExecutingEnvironmentResponseErrorReceived event

            await LogEvent(
                      GetExecutingEnvironmentResponseErrorReceived,
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
