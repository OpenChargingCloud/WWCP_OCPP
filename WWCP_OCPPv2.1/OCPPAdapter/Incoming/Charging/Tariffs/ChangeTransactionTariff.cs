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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The ChangeTransactionTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnChangeTransactionTariffRequestReceivedDelegate(DateTime                         Timestamp,
                                                         IEventSender                     Sender,
                                                         IWebSocketConnection             Connection,
                                                         ChangeTransactionTariffRequest   Request,
                                                         CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeTransactionTariffResponseReceivedDelegate(DateTime                          Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IWebSocketConnection?             Connection,
                                                                           ChangeTransactionTariffRequest?   Request,
                                                                           ChangeTransactionTariffResponse   Response,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeTransactionTariffRequestErrorReceivedDelegate(DateTime                          Timestamp,
                                                                               IEventSender                      Sender,
                                                                               IWebSocketConnection              Connection,
                                                                               ChangeTransactionTariffRequest?   Request,
                                                                               OCPP_JSONRequestErrorMessage      RequestErrorMessage,
                                                                               TimeSpan?                         Runtime,
                                                                               CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an ChangeTransactionTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeTransactionTariffResponseErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                ChangeTransactionTariffRequest?    Request,
                                                                                ChangeTransactionTariffResponse?   Response,
                                                                                OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an ChangeTransactionTariff response is expected
    /// for a received ChangeTransactionTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The ChangeTransactionTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<ChangeTransactionTariffResponse>

        OnChangeTransactionTariffDelegate(DateTime                         Timestamp,
                                          IEventSender                     Sender,
                                          IWebSocketConnection             Connection,
                                          ChangeTransactionTariffRequest   Request,
                                          CancellationToken                CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive ChangeTransactionTariff request

        /// <summary>
        /// An event sent whenever an ChangeTransactionTariff request was received.
        /// </summary>
        public event OnChangeTransactionTariffRequestReceivedDelegate?  OnChangeTransactionTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an ChangeTransactionTariff request was received for processing.
        /// </summary>
        public event OnChangeTransactionTariffDelegate?                 OnChangeTransactionTariff;


        public async Task<OCPP_Response>

            Receive_ChangeTransactionTariff(DateTime              RequestTimestamp,
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

                if (ChangeTransactionTariffRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                            Destination,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            RequestTimestamp,
                                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                            EventTrackingId,
                                                            parentNetworkingNode.OCPP.CustomChangeTransactionTariffRequestParser)) {

                    ChangeTransactionTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomChangeTransactionTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomTariffSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            parentNetworkingNode.OCPP.CustomTariffConditionsSerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergySerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergyPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimeSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimePriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedPriceSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = ChangeTransactionTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnChangeTransactionTariffRequestReceived event

                    await LogEvent(
                              OnChangeTransactionTariffRequestReceived,
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
                                           OnChangeTransactionTariff,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= ChangeTransactionTariffResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnChangeTransactionTariffResponseSent(
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
                                       nameof(Receive_ChangeTransactionTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_ChangeTransactionTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive ChangeTransactionTariff response

        /// <summary>
        /// An event fired whenever an ChangeTransactionTariff response was received.
        /// </summary>
        public event OnChangeTransactionTariffResponseReceivedDelegate? OnChangeTransactionTariffResponseReceived;


        public async Task<ChangeTransactionTariffResponse>

            Receive_ChangeTransactionTariffResponse(ChangeTransactionTariffRequest  Request,
                                                    JObject                         ResponseJSON,
                                                    IWebSocketConnection            WebSocketConnection,
                                                    SourceRouting                   Destination,
                                                    NetworkPath                     NetworkPath,
                                                    EventTracking_Id                EventTrackingId,
                                                    Request_Id                      RequestId,
                                                    DateTime?                       ResponseTimestamp   = null,
                                                    CancellationToken               CancellationToken   = default)

        {

            ChangeTransactionTariffResponse? response = null;

            try
            {

                if (ChangeTransactionTariffResponse.TryParse(Request,
                                                             ResponseJSON,
                                                             Destination,
                                                             NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             ResponseTimestamp,
                                                             parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseParser,
                                                             parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = ChangeTransactionTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = ChangeTransactionTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = ChangeTransactionTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnChangeTransactionTariffResponseReceived event

            await LogEvent(
                      OnChangeTransactionTariffResponseReceived,
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

        #region Receive ChangeTransactionTariff RequestError

        /// <summary>
        /// An event fired whenever an ChangeTransactionTariff request error was received.
        /// </summary>
        public event OnChangeTransactionTariffRequestErrorReceivedDelegate? ChangeTransactionTariffRequestErrorReceived;


        public async Task<ChangeTransactionTariffResponse>

            Receive_ChangeTransactionTariffRequestError(ChangeTransactionTariffRequest  Request,
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
            //        parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseSerializer,
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

            #region Send ChangeTransactionTariffRequestErrorReceived event

            await LogEvent(
                      ChangeTransactionTariffRequestErrorReceived,
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


            var response = ChangeTransactionTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnChangeTransactionTariffResponseReceived event

            await LogEvent(
                      OnChangeTransactionTariffResponseReceived,
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

        #region Receive ChangeTransactionTariff ResponseError

        /// <summary>
        /// An event fired whenever an ChangeTransactionTariff response error was received.
        /// </summary>
        public event OnChangeTransactionTariffResponseErrorReceivedDelegate? ChangeTransactionTariffResponseErrorReceived;


        public async Task

            Receive_ChangeTransactionTariffResponseError(ChangeTransactionTariffRequest?   Request,
                                                         ChangeTransactionTariffResponse?  Response,
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
            //        parentNetworkingNode.OCPP.CustomChangeTransactionTariffResponseSerializer,
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

            #region Send ChangeTransactionTariffResponseErrorReceived event

            await LogEvent(
                      ChangeTransactionTariffResponseErrorReceived,
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
