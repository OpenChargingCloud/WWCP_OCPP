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
    /// A logging delegate called whenever a ChangeAvailability request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeAvailabilityRequestReceivedDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        ChangeAvailabilityRequest           Request,
                                                        CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a ChangeAvailability response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeAvailabilityResponseReceivedDelegate(DateTime               Timestamp,
                                                         IEventSender           Sender,
                                                         IWebSocketConnection   Connection,
                                                         ChangeAvailabilityRequest?          Request,
                                                         ChangeAvailabilityResponse          Response,
                                                         TimeSpan?              Runtime,
                                                         CancellationToken      CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a ChangeAvailability request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeAvailabilityRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                             IEventSender                   Sender,
                                                             IWebSocketConnection           Connection,
                                                             ChangeAvailabilityRequest?                  Request,
                                                             OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                             TimeSpan?                      Runtime,
                                                             CancellationToken              CancellationToken = default);


    /// <summary>
    /// A logging delegate called whenever a ChangeAvailability response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnChangeAvailabilityResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                              IEventSender                    Sender,
                                                              IWebSocketConnection            Connection,
                                                              ChangeAvailabilityRequest?                   Request,
                                                              ChangeAvailabilityResponse?                  Response,
                                                              OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                              TimeSpan?                       Runtime,
                                                              CancellationToken               CancellationToken = default);

    #endregion


    /// <summary>
    /// A delegate called whenever a ChangeAvailability response is expected
    /// for a received ChangeAvailability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ChangeAvailabilityResponse>

        OnChangeAvailabilityDelegate(DateTime               Timestamp,
                        IEventSender           Sender,
                        IWebSocketConnection   Connection,
                        ChangeAvailabilityRequest           Request,
                        CancellationToken      CancellationToken = default);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive ChangeAvailability request

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestReceivedDelegate?  OnChangeAvailabilityRequestReceived;

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was received for processing.
        /// </summary>
        public event OnChangeAvailabilityDelegate?                 OnChangeAvailability;


        public async Task<OCPP_Response>

            Receive_ChangeAvailability(DateTime              RequestTimestamp,
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

                if (ChangeAvailabilityRequest.TryParse(JSONRequest,
                                          RequestId,
                                          DestinationId,
                                          NetworkPath,
                                          out var request,
                                          out var errorResponse,
                                          RequestTimestamp,
                                          parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                          EventTrackingId,
                                          parentNetworkingNode.OCPP.CustomChangeAvailabilityRequestParser)) {

                    ChangeAvailabilityResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomChangeAvailabilityRequestSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = ChangeAvailabilityResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnChangeAvailabilityRequestReceived event

                    await LogEvent(
                              OnChangeAvailabilityRequestReceived,
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

                            var responseTasks = OnChangeAvailability?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : ChangeAvailabilityResponse.Failed(request, $"Undefined {nameof(OnChangeAvailability)}!");

                        }
                        catch (Exception e)
                        {

                            response = ChangeAvailabilityResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnChangeAvailability),
                                      e
                                  );

                        }
                    }

                    response ??= ChangeAvailabilityResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2
                    );

                    #endregion


                    #region Send OnChangeAvailabilityResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnChangeAvailabilityResponseSent(
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
                                           parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
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
                                       nameof(Receive_ChangeAvailability)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_ChangeAvailability)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive ChangeAvailability response

        /// <summary>
        /// An event fired whenever a ChangeAvailability response was received.
        /// </summary>
        public event OnChangeAvailabilityResponseReceivedDelegate? OnChangeAvailabilityResponseReceived;


        public async Task<ChangeAvailabilityResponse>

            Receive_ChangeAvailabilityResponse(ChangeAvailabilityRequest          Request,
                                  JObject               ResponseJSON,
                                  IWebSocketConnection  WebSocketConnection,
                                  NetworkingNode_Id     DestinationId,
                                  NetworkPath           NetworkPath,
                                  EventTracking_Id      EventTrackingId,
                                  Request_Id            RequestId,
                                  DateTime?             ResponseTimestamp   = null,
                                  CancellationToken     CancellationToken   = default)

        {

            ChangeAvailabilityResponse? response = null;

            try
            {

                if (ChangeAvailabilityResponse.TryParse(Request,
                                           ResponseJSON,
                                           DestinationId,
                                           NetworkPath,
                                           out response,
                                           out var errorResponse,
                                           ResponseTimestamp,
                                           parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseParser,
                                           parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                           parentNetworkingNode.OCPP.CustomSignatureParser,
                                           parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = ChangeAvailabilityResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = ChangeAvailabilityResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = ChangeAvailabilityResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnChangeAvailabilityResponseReceived event

            await LogEvent(
                      OnChangeAvailabilityResponseReceived,
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

        #region Receive ChangeAvailability request error

        /// <summary>
        /// An event fired whenever a ChangeAvailability request error was received.
        /// </summary>
        public event OnChangeAvailabilityRequestErrorReceivedDelegate? ChangeAvailabilityRequestErrorReceived;


        public async Task<ChangeAvailabilityResponse>

            Receive_ChangeAvailabilityRequestError(ChangeAvailabilityRequest     Request,
                                                   OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                   IWebSocketConnection          Connection,
                                                   NetworkingNode_Id             DestinationId,
                                                   NetworkPath                   NetworkPath,
                                                   EventTracking_Id              EventTrackingId,
                                                   Request_Id                    RequestId,
                                                   DateTime?                     ResponseTimestamp   = null,
                                                   CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
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

            #region Send ChangeAvailabilityRequestErrorReceived event

            await LogEvent(
                      ChangeAvailabilityRequestErrorReceived,
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


            var response = ChangeAvailabilityResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnChangeAvailabilityResponseReceived event

            await LogEvent(
                      OnChangeAvailabilityResponseReceived,
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

        #region Receive ChangeAvailability response error

        /// <summary>
        /// An event fired whenever a ChangeAvailability response error was received.
        /// </summary>
        public event OnChangeAvailabilityResponseErrorReceivedDelegate? ChangeAvailabilityResponseErrorReceived;


        public async Task

            Receive_ChangeAvailabilityResponseError(ChangeAvailabilityRequest?     Request,
                                                    ChangeAvailabilityResponse?    Response,
                                                    OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                    IWebSocketConnection           Connection,
                                                    NetworkingNode_Id              DestinationId,
                                                    NetworkPath                    NetworkPath,
                                                    EventTracking_Id               EventTrackingId,
                                                    Request_Id                     RequestId,
                                                    DateTime?                      ResponseTimestamp   = null,
                                                    CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomChangeAvailabilityResponseSerializer,
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

            #region Send ChangeAvailabilityResponseErrorReceived event

            await LogEvent(
                      ChangeAvailabilityResponseErrorReceived,
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
