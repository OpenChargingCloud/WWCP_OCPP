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
    /// A logging delegate called whenever a FirmwareStatusNotification request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnFirmwareStatusNotificationRequestReceivedDelegate(DateTime                            Timestamp,
                                                                             IEventSender                        Sender,
                                                                             IWebSocketConnection                Connection,
                                                                             FirmwareStatusNotificationRequest   Request,
                                                                             CancellationToken                   CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a FirmwareStatusNotification response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnFirmwareStatusNotificationResponseReceivedDelegate(DateTime                             Timestamp,
                                                                              IEventSender                         Sender,
                                                                              IWebSocketConnection                 Connection,
                                                                              FirmwareStatusNotificationRequest?   Request,
                                                                              FirmwareStatusNotificationResponse   Response,
                                                                              TimeSpan?                            Runtime,
                                                                              CancellationToken                    CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a FirmwareStatusNotification request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnFirmwareStatusNotificationRequestErrorReceivedDelegate(DateTime                             Timestamp,
                                                                                  IEventSender                         Sender,
                                                                                  IWebSocketConnection                 Connection,
                                                                                  FirmwareStatusNotificationRequest?   Request,
                                                                                  OCPP_JSONRequestErrorMessage         RequestErrorMessage,
                                                                                  TimeSpan?                            Runtime,
                                                                                  CancellationToken                    CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a FirmwareStatusNotification response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnFirmwareStatusNotificationResponseErrorReceivedDelegate(DateTime                              Timestamp,
                                                                                   IEventSender                          Sender,
                                                                                   IWebSocketConnection                  Connection,
                                                                                   FirmwareStatusNotificationRequest?    Request,
                                                                                   FirmwareStatusNotificationResponse?   Response,
                                                                                   OCPP_JSONResponseErrorMessage         ResponseErrorMessage,
                                                                                   TimeSpan?                             Runtime,
                                                                                   CancellationToken                     CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a FirmwareStatusNotification response is expected
    /// for a received FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                            Timestamp,
                                             IEventSender                        Sender,
                                             IWebSocketConnection                Connection,
                                             FirmwareStatusNotificationRequest   Request,
                                             CancellationToken                   CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive FirmwareStatusNotification request

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestReceivedDelegate?  OnFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received for processing.
        /// </summary>
        public event OnFirmwareStatusNotificationDelegate?                 OnFirmwareStatusNotification;


        public async Task<OCPP_Response>

            Receive_FirmwareStatusNotification(DateTime              RequestTimestamp,
                                               IWebSocketConnection  WebSocketConnection,
                                               SourceRouting         SourceRouting,
                                               NetworkPath           NetworkPath,
                                               EventTracking_Id      EventTrackingId,
                                               Request_Id            RequestId,
                                               JObject               JSONRequest,
                                               CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (FirmwareStatusNotificationRequest.TryParse(JSONRequest,
                                                               RequestId,
                                                               SourceRouting,
                                                               NetworkPath,
                                                               out var request,
                                                               out var errorResponse,
                                                               RequestTimestamp,
                                                               parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                               EventTrackingId,
                                                               parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestParser)) {

                    FirmwareStatusNotificationResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = FirmwareStatusNotificationResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnFirmwareStatusNotificationRequestReceived event

                    await LogEvent(
                              OnFirmwareStatusNotificationRequestReceived,
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

                            var responseTasks = OnFirmwareStatusNotification?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnFirmwareStatusNotificationDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : FirmwareStatusNotificationResponse.Failed(request, $"Undefined {nameof(OnFirmwareStatusNotification)}!");

                        }
                        catch (Exception e)
                        {

                            response = FirmwareStatusNotificationResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnFirmwareStatusNotification),
                                      e
                                  );

                        }
                    }

                    response ??= FirmwareStatusNotificationResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnFirmwareStatusNotificationResponseSent(
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
                                       nameof(Receive_FirmwareStatusNotification)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_FirmwareStatusNotification)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive FirmwareStatusNotification response

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification response was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseReceivedDelegate? OnFirmwareStatusNotificationResponseReceived;


        public async Task<FirmwareStatusNotificationResponse>

            Receive_FirmwareStatusNotificationResponse(FirmwareStatusNotificationRequest  Request,
                                                       JObject                            ResponseJSON,
                                                       IWebSocketConnection               WebSocketConnection,
                                                       SourceRouting                      SourceRouting,
                                                       NetworkPath                        NetworkPath,
                                                       EventTracking_Id                   EventTrackingId,
                                                       Request_Id                         RequestId,
                                                       DateTime?                          ResponseTimestamp   = null,
                                                       CancellationToken                  CancellationToken   = default)

        {

            FirmwareStatusNotificationResponse? response = null;

            try
            {

                if (FirmwareStatusNotificationResponse.TryParse(Request,
                                                                ResponseJSON,
                                                                SourceRouting,
                                                                NetworkPath,
                                                                out response,
                                                                out var errorResponse,
                                                                ResponseTimestamp,
                                                                parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseParser,
                                                                parentNetworkingNode.OCPP.CustomSignatureParser,
                                                                parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = FirmwareStatusNotificationResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = FirmwareStatusNotificationResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = FirmwareStatusNotificationResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnFirmwareStatusNotificationResponseReceived event

            await LogEvent(
                      OnFirmwareStatusNotificationResponseReceived,
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

        #region Receive FirmwareStatusNotification request error

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request error was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestErrorReceivedDelegate? FirmwareStatusNotificationRequestErrorReceived;


        public async Task<FirmwareStatusNotificationResponse>

            Receive_FirmwareStatusNotificationRequestError(FirmwareStatusNotificationRequest  Request,
                                                           OCPP_JSONRequestErrorMessage       RequestErrorMessage,
                                                           IWebSocketConnection               Connection,
                                                           SourceRouting                      SourceRouting,
                                                           NetworkPath                        NetworkPath,
                                                           EventTracking_Id                   EventTrackingId,
                                                           Request_Id                         RequestId,
                                                           DateTime?                          ResponseTimestamp   = null,
                                                           CancellationToken                  CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
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

            #region Send FirmwareStatusNotificationRequestErrorReceived event

            await LogEvent(
                      FirmwareStatusNotificationRequestErrorReceived,
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


            var response = FirmwareStatusNotificationResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnFirmwareStatusNotificationResponseReceived event

            await LogEvent(
                      OnFirmwareStatusNotificationResponseReceived,
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

        #region Receive FirmwareStatusNotification response error

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification response error was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseErrorReceivedDelegate? FirmwareStatusNotificationResponseErrorReceived;


        public async Task

            Receive_FirmwareStatusNotificationResponseError(FirmwareStatusNotificationRequest?   Request,
                                                            FirmwareStatusNotificationResponse?  Response,
                                                            OCPP_JSONResponseErrorMessage        ResponseErrorMessage,
                                                            IWebSocketConnection                 Connection,
                                                            SourceRouting                        SourceRouting,
                                                            NetworkPath                          NetworkPath,
                                                            EventTracking_Id                     EventTrackingId,
                                                            Request_Id                           RequestId,
                                                            DateTime?                            ResponseTimestamp   = null,
                                                            CancellationToken                    CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomFirmwareStatusNotificationResponseSerializer,
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

            #region Send FirmwareStatusNotificationResponseErrorReceived event

            await LogEvent(
                      FirmwareStatusNotificationResponseErrorReceived,
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