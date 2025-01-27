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
    /// A logging delegate called whenever an AFRRSignal request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAFRRSignalRequestReceivedDelegate(DateTime               Timestamp,
                                                             IEventSender           Sender,
                                                             IWebSocketConnection   Connection,
                                                             AFRRSignalRequest      Request,
                                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AFRRSignal response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAFRRSignalResponseReceivedDelegate(DateTime                Timestamp,
                                                              IEventSender            Sender,
                                                              IWebSocketConnection?   Connection,
                                                              AFRRSignalRequest?      Request,
                                                              AFRRSignalResponse      Response,
                                                              TimeSpan?               Runtime,
                                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AFRRSignal request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAFRRSignalRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                  IEventSender                   Sender,
                                                                  IWebSocketConnection           Connection,
                                                                  AFRRSignalRequest?             Request,
                                                                  OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                  TimeSpan?                      Runtime,
                                                                  CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an AFRRSignal response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnAFRRSignalResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                   IEventSender                    Sender,
                                                                   IWebSocketConnection            Connection,
                                                                   AFRRSignalRequest?              Request,
                                                                   AFRRSignalResponse?             Response,
                                                                   OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                   TimeSpan?                       Runtime,
                                                                   CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an AFRRSignal response is expected
    /// for a received AFRRSignal request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AFRRSignalResponse>

        OnAFRRSignalDelegate(DateTime               Timestamp,
                             IEventSender           Sender,
                             IWebSocketConnection   Connection,
                             AFRRSignalRequest      Request,
                             CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive AFRRSignal request

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalRequestReceivedDelegate?  OnAFRRSignalRequestReceived;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received for processing.
        /// </summary>
        public event OnAFRRSignalDelegate?                 OnAFRRSignal;


        public async Task<OCPP_Response>

            Receive_AFRRSignal(DateTime              RequestTimestamp,
                               IWebSocketConnection  WebSocketConnection,
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

                if (AFRRSignalRequest.TryParse(JSONRequest,
                                               RequestId,
                                           Destination,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               RequestTimestamp,
                                               parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                               EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomAFRRSignalRequestParser)) {

                    AFRRSignalResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomAFRRSignalRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = AFRRSignalResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnAFRRSignalRequestReceived event

                    await LogEvent(
                              OnAFRRSignalRequestReceived,
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
                                           OnAFRRSignal,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= AFRRSignalResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnAFRRSignalResponseSent(
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
                                       nameof(Receive_AFRRSignal)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_AFRRSignal)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive AFRRSignal response

        /// <summary>
        /// An event fired whenever an AFRRSignal response was received.
        /// </summary>
        public event OnAFRRSignalResponseReceivedDelegate? OnAFRRSignalResponseReceived;


        public async Task<AFRRSignalResponse>

            Receive_AFRRSignalResponse(AFRRSignalRequest     Request,
                                       JObject               ResponseJSON,
                                       IWebSocketConnection  WebSocketConnection,
                                       SourceRouting     Destination,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       DateTime?             ResponseTimestamp   = null,
                                       CancellationToken     CancellationToken   = default)

        {

            AFRRSignalResponse? response = null;

            try
            {

                if (AFRRSignalResponse.TryParse(Request,
                                                ResponseJSON,
                                            Destination,
                                                NetworkPath,
                                                out response,
                                                out var errorResponse,
                                                ResponseTimestamp,
                                                parentNetworkingNode.OCPP.CustomAFRRSignalResponseParser,
                                                parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                parentNetworkingNode.OCPP.CustomSignatureParser,
                                                parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = AFRRSignalResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = AFRRSignalResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = AFRRSignalResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnAFRRSignalResponseReceived event

            await LogEvent(
                      OnAFRRSignalResponseReceived,
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

        #region Receive AFRRSignal request error

        /// <summary>
        /// An event fired whenever an AFRRSignal request error was received.
        /// </summary>
        public event OnAFRRSignalRequestErrorReceivedDelegate? AFRRSignalRequestErrorReceived;


        public async Task<AFRRSignalResponse>

            Receive_AFRRSignalRequestError(AFRRSignalRequest             Request,
                                           OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                           IWebSocketConnection          Connection,
                                           SourceRouting             Destination,
                                           NetworkPath                   NetworkPath,
                                           EventTracking_Id              EventTrackingId,
                                           Request_Id                    RequestId,
                                           DateTime?                     ResponseTimestamp   = null,
                                           CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
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

            #region Send AFRRSignalRequestErrorReceived event

            await LogEvent(
                      AFRRSignalRequestErrorReceived,
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


            var response = AFRRSignalResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnAFRRSignalResponseReceived event

            await LogEvent(
                      OnAFRRSignalResponseReceived,
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

        #region Receive AFRRSignal response error

        /// <summary>
        /// An event fired whenever an AFRRSignal response error was received.
        /// </summary>
        public event OnAFRRSignalResponseErrorReceivedDelegate? AFRRSignalResponseErrorReceived;


        public async Task

            Receive_AFRRSignalResponseError(AFRRSignalRequest?             Request,
                                            AFRRSignalResponse?            Response,
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
            //        parentNetworkingNode.OCPP.CustomAFRRSignalResponseSerializer,
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

            #region Send AFRRSignalResponseErrorReceived event

            await LogEvent(
                      AFRRSignalResponseErrorReceived,
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
