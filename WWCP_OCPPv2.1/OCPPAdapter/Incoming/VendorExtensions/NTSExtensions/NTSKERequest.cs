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
    /// A logging delegate called whenever a NTS-KE request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNTSKERequestReceivedDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        NTSKERequest           Request,
                                                        CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NTS-KE response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNTSKEResponseReceivedDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         IWebSocketConnection?   Connection,
                                                         NTSKERequest?           Request,
                                                         NTSKEResponse           Response,
                                                         TimeSpan?               Runtime,
                                                         CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NTS-KE request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNTSKERequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                             IEventSender                   Sender,
                                                             IWebSocketConnection           Connection,
                                                             NTSKERequest?                  Request,
                                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                             TimeSpan?                      Runtime,
                                                             CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NTS-KE response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNTSKEResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                              IEventSender                    Sender,
                                                              IWebSocketConnection            Connection,
                                                              NTSKERequest?                   Request,
                                                              NTSKEResponse?                  Response,
                                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                              TimeSpan?                       Runtime,
                                                              CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an NTS-KE response is expected
    /// for a received NTSKE request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NTSKEResponse>

        OnNTSKEDelegate(DateTime               Timestamp,
                        IEventSender           Sender,
                        IWebSocketConnection   Connection,
                        NTSKERequest           Request,
                        CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NTSKE request

        /// <summary>
        /// An event sent whenever a NTS-KE request was received.
        /// </summary>
        public event OnNTSKERequestReceivedDelegate?  OnNTSKERequestReceived;

        /// <summary>
        /// An event sent whenever a NTS-KE request was received for processing.
        /// </summary>
        public event OnNTSKEDelegate?                 OnNTSKE;


        public async Task<OCPP_Response>

            Receive_NTSKE(DateTime              RequestTimestamp,
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

                if (NTSKERequest.TryParse(JSONRequest,
                                          RequestId,
                                          Destination,
                                          NetworkPath,
                                          out var request,
                                          out var errorResponse,
                                          RequestTimestamp,
                                          parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                          EventTrackingId,
                                          parentNetworkingNode.OCPP.CustomNTSKERequestParser)) {

                    NTSKEResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNTSKERequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = NTSKEResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNTSKERequestReceived event

                    await LogEvent(
                              OnNTSKERequestReceived,
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
                                           OnNTSKE,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= NTSKEResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
                            parentNetworkingNode.OCPP.CustomNTSKEServerInfoSerializer,
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
                                           parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomNTSKEServerInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnNTSKEResponseSent(
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
                                       nameof(Receive_NTSKE)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NTSKE)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NTSKE response

        /// <summary>
        /// An event fired whenever a NTS-KE response was received.
        /// </summary>
        public event OnNTSKEResponseReceivedDelegate? OnNTSKEResponseReceived;


        public async Task<NTSKEResponse>

            Receive_NTSKEResponse(NTSKERequest          Request,
                                  JObject               ResponseJSON,
                                  IWebSocketConnection  WebSocketConnection,
                                  SourceRouting         Destination,
                                  NetworkPath           NetworkPath,
                                  EventTracking_Id      EventTrackingId,
                                  Request_Id            RequestId,
                                  DateTime?             ResponseTimestamp   = null,
                                  CancellationToken     CancellationToken   = default)

        {

            NTSKEResponse? response = null;

            try
            {

                if (NTSKEResponse.TryParse(Request,
                                           ResponseJSON,
                                           Destination,
                                           NetworkPath,
                                           out response,
                                           out var errorResponse,
                                           ResponseTimestamp,
                                           parentNetworkingNode.OCPP.CustomNTSKEResponseParser,
                                           parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                           parentNetworkingNode.OCPP.CustomSignatureParser,
                                           parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
                                parentNetworkingNode.OCPP.CustomNTSKEServerInfoSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = NTSKEResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = NTSKEResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = NTSKEResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnNTSKEResponseReceived event

            await LogEvent(
                      OnNTSKEResponseReceived,
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

        #region Receive NTSKE request error

        /// <summary>
        /// An event fired whenever a NTS-KE request error was received.
        /// </summary>
        public event OnNTSKERequestErrorReceivedDelegate? NTSKERequestErrorReceived;


        public async Task<NTSKEResponse>

            Receive_NTSKERequestError(NTSKERequest                  Request,
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
            //        parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
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

            #region Send NTSKERequestErrorReceived event

            await LogEvent(
                      NTSKERequestErrorReceived,
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


            var response = NTSKEResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNTSKEResponseReceived event

            await LogEvent(
                      OnNTSKEResponseReceived,
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

        #region Receive NTSKE response error

        /// <summary>
        /// An event fired whenever a NTS-KE response error was received.
        /// </summary>
        public event OnNTSKEResponseErrorReceivedDelegate? NTSKEResponseErrorReceived;


        public async Task

            Receive_NTSKEResponseError(NTSKERequest?                  Request,
                                       NTSKEResponse?                 Response,
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
            //        parentNetworkingNode.OCPP.CustomNTSKEResponseSerializer,
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

            #region Send NTSKEResponseErrorReceived event

            await LogEvent(
                      NTSKEResponseErrorReceived,
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
