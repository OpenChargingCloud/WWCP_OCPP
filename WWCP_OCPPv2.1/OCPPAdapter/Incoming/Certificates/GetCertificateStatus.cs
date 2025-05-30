﻿/*
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
    /// A logging delegate called whenever a GetCertificateStatus request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetCertificateStatusRequestReceivedDelegate(DateTime                      Timestamp,
                                                                       IEventSender                  Sender,
                                                                       IWebSocketConnection          Connection,
                                                                       GetCertificateStatusRequest   Request,
                                                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetCertificateStatus response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetCertificateStatusResponseReceivedDelegate(DateTime                       Timestamp,
                                                                        IEventSender                   Sender,
                                                                        IWebSocketConnection?          Connection,
                                                                        GetCertificateStatusRequest?   Request,
                                                                        GetCertificateStatusResponse   Response,
                                                                        TimeSpan?                      Runtime,
                                                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetCertificateStatus request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetCertificateStatusRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                            IEventSender                   Sender,
                                                                            IWebSocketConnection           Connection,
                                                                            GetCertificateStatusRequest?   Request,
                                                                            OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                            TimeSpan?                      Runtime,
                                                                            CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetCertificateStatus response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetCertificateStatusResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                             IEventSender                    Sender,
                                                                             IWebSocketConnection            Connection,
                                                                             GetCertificateStatusRequest?    Request,
                                                                             GetCertificateStatusResponse?   Response,
                                                                             OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                             TimeSpan?                       Runtime,
                                                                             CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetCertificateStatus response is expected
    /// for a received GetCertificateStatus request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCertificateStatusResponse>

        OnGetCertificateStatusDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       IWebSocketConnection          Connection,
                                       GetCertificateStatusRequest   Request,
                                       CancellationToken             CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetCertificateStatus request

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusRequestReceivedDelegate?  OnGetCertificateStatusRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received for processing.
        /// </summary>
        public event OnGetCertificateStatusDelegate?                 OnGetCertificateStatus;


        public async Task<OCPP_Response>

            Receive_GetCertificateStatus(DateTime              RequestTimestamp,
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

                if (GetCertificateStatusRequest.TryParse(JSONRequest,
                                                         RequestId,
                                                     Destination,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         RequestTimestamp,
                                                         parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                         EventTrackingId,
                                                         parentNetworkingNode.OCPP.CustomGetCertificateStatusRequestParser)) {

                    GetCertificateStatusResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetCertificateStatusRequestSerializer,
                            parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetCertificateStatusResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetCertificateStatusRequestReceived event

                    await LogEvent(
                              OnGetCertificateStatusRequestReceived,
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
                                           OnGetCertificateStatus,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetCertificateStatusResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
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
                                           false,
                                           parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetCertificateStatusResponseSent(
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
                                       nameof(Receive_GetCertificateStatus)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetCertificateStatus)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetCertificateStatus response

        /// <summary>
        /// An event fired whenever a GetCertificateStatus response was received.
        /// </summary>
        public event OnGetCertificateStatusResponseReceivedDelegate? OnGetCertificateStatusResponseReceived;


        public async Task<GetCertificateStatusResponse>

            Receive_GetCertificateStatusResponse(GetCertificateStatusRequest  Request,
                                                 JObject                      ResponseJSON,
                                                 IWebSocketConnection         WebSocketConnection,
                                                 SourceRouting            Destination,
                                                 NetworkPath                  NetworkPath,
                                                 EventTracking_Id             EventTrackingId,
                                                 Request_Id                   RequestId,
                                                 DateTime?                    ResponseTimestamp   = null,
                                                 CancellationToken            CancellationToken   = default)

        {

            GetCertificateStatusResponse? response = null;

            try
            {

                if (GetCertificateStatusResponse.TryParse(Request,
                                                          ResponseJSON,
                                                      Destination,
                                                          NetworkPath,
                                                          out response,
                                                          out var errorResponse,
                                                          ResponseTimestamp,
                                                          parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseParser,
                                                          parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                          parentNetworkingNode.OCPP.CustomSignatureParser,
                                                          parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetCertificateStatusResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetCertificateStatusResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetCertificateStatusResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetCertificateStatusResponseReceived event

            await LogEvent(
                      OnGetCertificateStatusResponseReceived,
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

        #region Receive GetCertificateStatus request error

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request error was received.
        /// </summary>
        public event OnGetCertificateStatusRequestErrorReceivedDelegate? GetCertificateStatusRequestErrorReceived;


        public async Task<GetCertificateStatusResponse>

            Receive_GetCertificateStatusRequestError(GetCertificateStatusRequest   Request,
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
            //        parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
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

            #region Send GetCertificateStatusRequestErrorReceived event

            await LogEvent(
                      GetCertificateStatusRequestErrorReceived,
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


            var response = GetCertificateStatusResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetCertificateStatusResponseReceived event

            await LogEvent(
                      OnGetCertificateStatusResponseReceived,
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

        #region Receive GetCertificateStatus response error

        /// <summary>
        /// An event fired whenever a GetCertificateStatus response error was received.
        /// </summary>
        public event OnGetCertificateStatusResponseErrorReceivedDelegate? GetCertificateStatusResponseErrorReceived;


        public async Task

            Receive_GetCertificateStatusResponseError(GetCertificateStatusRequest?   Request,
                                                      GetCertificateStatusResponse?  Response,
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
            //        parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
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

            #region Send GetCertificateStatusResponseErrorReceived event

            await LogEvent(
                      GetCertificateStatusResponseErrorReceived,
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
