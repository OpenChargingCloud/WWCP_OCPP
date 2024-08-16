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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffRequestReceivedDelegate(DateTime                             Timestamp,
                                                             IEventSender                         Sender,
                                                             IWebSocketConnection                 Connection,
                                                             RemoveDefaultChargingTariffRequest   Request,
                                                             CancellationToken                    CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRemoveDefaultChargingTariffResponseReceivedDelegate(DateTime                              Timestamp,
                                                                               IEventSender                          Sender,
                                                                               IWebSocketConnection                  Connection,
                                                                               RemoveDefaultChargingTariffRequest?   Request,
                                                                               RemoveDefaultChargingTariffResponse   Response,
                                                                               TimeSpan?                             Runtime,
                                                                               CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRemoveDefaultChargingTariffRequestErrorReceivedDelegate(DateTime                              Timestamp,
                                                                                   IEventSender                          Sender,
                                                                                   IWebSocketConnection                  Connection,
                                                                                   RemoveDefaultChargingTariffRequest?   Request,
                                                                                   OCPP_JSONRequestErrorMessage          RequestErrorMessage,
                                                                                   TimeSpan?                             Runtime,
                                                                                   CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an RemoveDefaultChargingTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnRemoveDefaultChargingTariffResponseErrorReceivedDelegate(DateTime                               Timestamp,
                                                                                    IEventSender                           Sender,
                                                                                    IWebSocketConnection                   Connection,
                                                                                    RemoveDefaultChargingTariffRequest?    Request,
                                                                                    RemoveDefaultChargingTariffResponse?   Response,
                                                                                    OCPP_JSONResponseErrorMessage          ResponseErrorMessage,
                                                                                    TimeSpan?                              Runtime,
                                                                                    CancellationToken                      CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an RemoveDefaultChargingTariff response is expected
    /// for a received RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<RemoveDefaultChargingTariffResponse>

        OnRemoveDefaultChargingTariffDelegate(DateTime                             Timestamp,
                                              IEventSender                         Sender,
                                              IWebSocketConnection                 Connection,
                                              RemoveDefaultChargingTariffRequest   Request,
                                              CancellationToken                    CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive RemoveDefaultChargingTariff request

        /// <summary>
        /// An event sent whenever an RemoveDefaultChargingTariff request was received.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestReceivedDelegate?  OnRemoveDefaultChargingTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an RemoveDefaultChargingTariff request was received for processing.
        /// </summary>
        public event OnRemoveDefaultChargingTariffDelegate?                 OnRemoveDefaultChargingTariff;


        public async Task<OCPP_Response>

            Receive_RemoveDefaultChargingTariff(DateTime              RequestTimestamp,
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

                if (RemoveDefaultChargingTariffRequest.TryParse(JSONRequest,
                                                                RequestId,
                                                                SourceRouting,
                                                                NetworkPath,
                                                                out var request,
                                                                out var errorResponse,
                                                                RequestTimestamp,
                                                                parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                                EventTrackingId,
                                                                parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestParser)) {

                    RemoveDefaultChargingTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = RemoveDefaultChargingTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnRemoveDefaultChargingTariffRequestReceived event

                    await LogEvent(
                              OnRemoveDefaultChargingTariffRequestReceived,
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

                            var responseTasks = OnRemoveDefaultChargingTariff?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnRemoveDefaultChargingTariffDelegate)?.Invoke(
                                                                                                                               Timestamp.Now,
                                                                                                                               parentNetworkingNode,
                                                                                                                               WebSocketConnection,
                                                                                                                               request,
                                                                                                                               CancellationToken
                                                                                                                           )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : RemoveDefaultChargingTariffResponse.Failed(request, $"Undefined {nameof(OnRemoveDefaultChargingTariff)}!");

                        }
                        catch (Exception e)
                        {

                            response = RemoveDefaultChargingTariffResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnRemoveDefaultChargingTariff),
                                      e
                                  );

                        }
                    }

                    response ??= RemoveDefaultChargingTariffResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer2,
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
                                           parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer2,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnRemoveDefaultChargingTariffResponseSent(
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
                                       nameof(Receive_RemoveDefaultChargingTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_RemoveDefaultChargingTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive RemoveDefaultChargingTariff response

        /// <summary>
        /// An event fired whenever an RemoveDefaultChargingTariff response was received.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseReceivedDelegate? OnRemoveDefaultChargingTariffResponseReceived;


        public async Task<RemoveDefaultChargingTariffResponse>

            Receive_RemoveDefaultChargingTariffResponse(RemoveDefaultChargingTariffRequest  Request,
                                                        JObject                             ResponseJSON,
                                                        IWebSocketConnection                WebSocketConnection,
                                                        SourceRouting                       SourceRouting,
                                                        NetworkPath                         NetworkPath,
                                                        EventTracking_Id                    EventTrackingId,
                                                        Request_Id                          RequestId,
                                                        DateTime?                           ResponseTimestamp   = null,
                                                        CancellationToken                   CancellationToken   = default)

        {

            RemoveDefaultChargingTariffResponse? response = null;

            try
            {

                if (RemoveDefaultChargingTariffResponse.TryParse(Request,
                                                                 ResponseJSON,
                                                                 SourceRouting,
                                                                 NetworkPath,
                                                                 out response,
                                                                 out var errorResponse,
                                                                 ResponseTimestamp,
                                                                 parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer2,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = RemoveDefaultChargingTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = RemoveDefaultChargingTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = RemoveDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnRemoveDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnRemoveDefaultChargingTariffResponseReceived,
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

        #region Receive RemoveDefaultChargingTariff RequestError

        /// <summary>
        /// An event fired whenever an RemoveDefaultChargingTariff request error was received.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestErrorReceivedDelegate? RemoveDefaultChargingTariffRequestErrorReceived;


        public async Task<RemoveDefaultChargingTariffResponse>

            Receive_RemoveDefaultChargingTariffRequestError(RemoveDefaultChargingTariffRequest  Request,
                                                            OCPP_JSONRequestErrorMessage        RequestErrorMessage,
                                                            IWebSocketConnection                Connection,
                                                            SourceRouting                       SourceRouting,
                                                            NetworkPath                         NetworkPath,
                                                            EventTracking_Id                    EventTrackingId,
                                                            Request_Id                          RequestId,
                                                            DateTime?                           ResponseTimestamp   = null,
                                                            CancellationToken                   CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
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

            #region Send RemoveDefaultChargingTariffRequestErrorReceived event

            await LogEvent(
                      RemoveDefaultChargingTariffRequestErrorReceived,
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


            var response = RemoveDefaultChargingTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnRemoveDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnRemoveDefaultChargingTariffResponseReceived,
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

        #region Receive RemoveDefaultChargingTariff ResponseError

        /// <summary>
        /// An event fired whenever an RemoveDefaultChargingTariff response error was received.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseErrorReceivedDelegate? RemoveDefaultChargingTariffResponseErrorReceived;


        public async Task

            Receive_RemoveDefaultChargingTariffResponseError(RemoveDefaultChargingTariffRequest?   Request,
                                                             RemoveDefaultChargingTariffResponse?  Response,
                                                             OCPP_JSONResponseErrorMessage         ResponseErrorMessage,
                                                             IWebSocketConnection                  Connection,
                                                             SourceRouting                         SourceRouting,
                                                             NetworkPath                           NetworkPath,
                                                             EventTracking_Id                      EventTrackingId,
                                                             Request_Id                            RequestId,
                                                             DateTime?                             ResponseTimestamp   = null,
                                                             CancellationToken                     CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
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

            #region Send RemoveDefaultChargingTariffResponseErrorReceived event

            await LogEvent(
                      RemoveDefaultChargingTariffResponseErrorReceived,
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
