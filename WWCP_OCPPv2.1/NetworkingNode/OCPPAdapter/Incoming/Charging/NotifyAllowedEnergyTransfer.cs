﻿/*
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
    /// A logging delegate called whenever a NotifyAllowedEnergyTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyAllowedEnergyTransferRequestReceivedDelegate(DateTime                             Timestamp,
                                                                              IEventSender                         Sender,
                                                                              IWebSocketConnection                 Connection,
                                                                              NotifyAllowedEnergyTransferRequest   Request,
                                                                              CancellationToken                    CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a NotifyAllowedEnergyTransfer response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyAllowedEnergyTransferResponseReceivedDelegate(DateTime                              Timestamp,
                                                                               IEventSender                          Sender,
                                                                               IWebSocketConnection                  Connection,
                                                                               NotifyAllowedEnergyTransferRequest?   Request,
                                                                               NotifyAllowedEnergyTransferResponse   Response,
                                                                               TimeSpan?                             Runtime,
                                                                               CancellationToken                     CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a NotifyAllowedEnergyTransfer request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyAllowedEnergyTransferRequestErrorReceivedDelegate(DateTime                              Timestamp,
                                                                                   IEventSender                          Sender,
                                                                                   IWebSocketConnection                  Connection,
                                                                                   NotifyAllowedEnergyTransferRequest?   Request,
                                                                                   OCPP_JSONRequestErrorMessage          RequestErrorMessage,
                                                                                   TimeSpan?                             Runtime,
                                                                                   CancellationToken                     CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a NotifyAllowedEnergyTransfer response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyAllowedEnergyTransferResponseErrorReceivedDelegate(DateTime                               Timestamp,
                                                                                    IEventSender                           Sender,
                                                                                    IWebSocketConnection                   Connection,
                                                                                    NotifyAllowedEnergyTransferRequest?    Request,
                                                                                    NotifyAllowedEnergyTransferResponse?   Response,
                                                                                    OCPP_JSONResponseErrorMessage          ResponseErrorMessage,
                                                                                    TimeSpan?                              Runtime,
                                                                                    CancellationToken                      CancellationToken = default);

    #endregion


    /// <summary>
    /// A delegate called whenever a NotifyAllowedEnergyTransfer response is expected
    /// for a received NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyAllowedEnergyTransferResponse>

        OnNotifyAllowedEnergyTransferDelegate(DateTime                             Timestamp,
                                              IEventSender                         Sender,
                                              IWebSocketConnection                 Connection,
                                              NotifyAllowedEnergyTransferRequest   Request,
                                              CancellationToken                    CancellationToken = default);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive NotifyAllowedEnergyTransfer request

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestReceivedDelegate?  OnNotifyAllowedEnergyTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received for processing.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferDelegate?                 OnNotifyAllowedEnergyTransfer;


        public async Task<OCPP_Response>

            Receive_NotifyAllowedEnergyTransfer(DateTime              RequestTimestamp,
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

                if (NotifyAllowedEnergyTransferRequest.TryParse(JSONRequest,
                                                                RequestId,
                                                                DestinationId,
                                                                NetworkPath,
                                                                out var request,
                                                                out var errorResponse,
                                                                RequestTimestamp,
                                                                parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                                EventTrackingId,
                                                                parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestParser)) {

                    NotifyAllowedEnergyTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = NotifyAllowedEnergyTransferResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnNotifyAllowedEnergyTransferRequestReceived event

                    await LogEvent(
                              OnNotifyAllowedEnergyTransferRequestReceived,
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

                            var responseTasks = OnNotifyAllowedEnergyTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnNotifyAllowedEnergyTransferDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : NotifyAllowedEnergyTransferResponse.Failed(request, $"Undefined {nameof(OnNotifyAllowedEnergyTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = NotifyAllowedEnergyTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnNotifyAllowedEnergyTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= NotifyAllowedEnergyTransferResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion


                    #region Send OnNotifyAllowedEnergyTransferResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnNotifyAllowedEnergyTransferResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
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
                                       nameof(Receive_NotifyAllowedEnergyTransfer)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_NotifyAllowedEnergyTransfer)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive NotifyAllowedEnergyTransfer response

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer response was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseReceivedDelegate? OnNotifyAllowedEnergyTransferResponseReceived;


        public async Task<NotifyAllowedEnergyTransferResponse>

            Receive_NotifyAllowedEnergyTransferResponse(NotifyAllowedEnergyTransferRequest  Request,
                                                        JObject                             ResponseJSON,
                                                        IWebSocketConnection                WebSocketConnection,
                                                        NetworkingNode_Id                   DestinationId,
                                                        NetworkPath                         NetworkPath,
                                                        EventTracking_Id                    EventTrackingId,
                                                        Request_Id                          RequestId,
                                                        DateTime?                           ResponseTimestamp   = null,
                                                        CancellationToken                   CancellationToken   = default)

        {

            NotifyAllowedEnergyTransferResponse? response = null;

            try
            {

                if (NotifyAllowedEnergyTransferResponse.TryParse(Request,
                                                                 ResponseJSON,
                                                                 DestinationId,
                                                                 NetworkPath,
                                                                 out response,
                                                                 out var errorResponse,
                                                                 ResponseTimestamp,
                                                                 parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseParser,
                                                                 parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                                 parentNetworkingNode.OCPP.CustomSignatureParser,
                                                                 parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = NotifyAllowedEnergyTransferResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = NotifyAllowedEnergyTransferResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = NotifyAllowedEnergyTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnNotifyAllowedEnergyTransferResponseReceived event

            await LogEvent(
                      OnNotifyAllowedEnergyTransferResponseReceived,
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

        #region Receive NotifyAllowedEnergyTransfer request error

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request error was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestErrorReceivedDelegate? NotifyAllowedEnergyTransferRequestErrorReceived;


        public async Task<NotifyAllowedEnergyTransferResponse>

            Receive_NotifyAllowedEnergyTransferRequestError(NotifyAllowedEnergyTransferRequest  Request,
                                                            OCPP_JSONRequestErrorMessage        RequestErrorMessage,
                                                            IWebSocketConnection                Connection,
                                                            NetworkingNode_Id                   DestinationId,
                                                            NetworkPath                         NetworkPath,
                                                            EventTracking_Id                    EventTrackingId,
                                                            Request_Id                          RequestId,
                                                            DateTime?                           ResponseTimestamp   = null,
                                                            CancellationToken                   CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
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

            #region Send NotifyAllowedEnergyTransferRequestErrorReceived event

            await LogEvent(
                      NotifyAllowedEnergyTransferRequestErrorReceived,
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


            var response = NotifyAllowedEnergyTransferResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnNotifyAllowedEnergyTransferResponseReceived event

            await LogEvent(
                      OnNotifyAllowedEnergyTransferResponseReceived,
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

        #region Receive NotifyAllowedEnergyTransfer response error

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer response error was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseErrorReceivedDelegate? NotifyAllowedEnergyTransferResponseErrorReceived;


        public async Task

            Receive_NotifyAllowedEnergyTransferResponseError(NotifyAllowedEnergyTransferRequest?   Request,
                                                             NotifyAllowedEnergyTransferResponse?  Response,
                                                             OCPP_JSONResponseErrorMessage         ResponseErrorMessage,
                                                             IWebSocketConnection                  Connection,
                                                             NetworkingNode_Id                     DestinationId,
                                                             NetworkPath                           NetworkPath,
                                                             EventTracking_Id                      EventTrackingId,
                                                             Request_Id                            RequestId,
                                                             DateTime?                             ResponseTimestamp   = null,
                                                             CancellationToken                     CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomNotifyAllowedEnergyTransferResponseSerializer,
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

            #region Send NotifyAllowedEnergyTransferResponseErrorReceived event

            await LogEvent(
                      NotifyAllowedEnergyTransferResponseErrorReceived,
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
