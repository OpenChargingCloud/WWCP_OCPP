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
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a StopTransaction request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnStopTransactionRequestReceivedDelegate(DateTimeOffset           Timestamp,
                                                                  IEventSender             Sender,
                                                                  IWebSocketConnection     Connection,
                                                                  StopTransactionRequest   Request,
                                                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a StopTransaction response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnStopTransactionResponseReceivedDelegate(DateTimeOffset            Timestamp,
                                                                   IEventSender              Sender,
                                                                   IWebSocketConnection?     Connection,
                                                                   StopTransactionRequest?   Request,
                                                                   StopTransactionResponse   Response,
                                                                   TimeSpan?                 Runtime,
                                                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a StopTransaction request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnStopTransactionRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                       IEventSender                   Sender,
                                                                       IWebSocketConnection           Connection,
                                                                       StopTransactionRequest?        Request,
                                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                       TimeSpan?                      Runtime,
                                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a StopTransaction response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnStopTransactionResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                        IEventSender                    Sender,
                                                                        IWebSocketConnection            Connection,
                                                                        StopTransactionRequest?         Request,
                                                                        StopTransactionResponse?        Response,
                                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                        TimeSpan?                       Runtime,
                                                                        CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a StopTransaction response is expected
    /// for a received StopTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StopTransactionResponse>

        OnStopTransactionDelegate(DateTimeOffset           Timestamp,
                                  IEventSender             Sender,
                                  IWebSocketConnection     Connection,
                                  StopTransactionRequest   Request,
                                  CancellationToken        CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive StopTransaction request (JSON)

        /// <summary>
        /// An event sent whenever a StopTransaction request was received.
        /// </summary>
        public event OnStopTransactionRequestReceivedDelegate?  OnStopTransactionRequestReceived;

        /// <summary>
        /// An event sent whenever a StopTransaction request was received for processing.
        /// </summary>
        public event OnStopTransactionDelegate?                 OnStopTransaction;


        public async Task<OCPP_Response>

            Receive_StopTransaction(DateTimeOffset        RequestTimestamp,
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

                if (StopTransactionRequest.TryParse(JSONRequest,
                                                    RequestId,
                                                    Destination,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    RequestTimestamp,
                                                    parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                    EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomStopTransactionRequestParser)) {

                    StopTransactionResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomStopTransactionRequestSerializer,
                            parentNetworkingNode.OCPP.CustomMeterValueSerializer,
                            parentNetworkingNode.OCPP.CustomSampledValueSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = StopTransactionResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnStopTransactionRequestReceived event

                    await LogEvent(
                              OnStopTransactionRequestReceived,
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
                                           OnStopTransaction,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= StopTransactionResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
                            parentNetworkingNode.OCPP.CustomIdTagInfoSerializer,
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
                                           parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTagInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnStopTransactionResponseSent(
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
                                       nameof(Receive_StopTransaction)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_StopTransaction)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive StopTransaction request (binary)

        public async Task<OCPP_Response>

            Receive_StopTransaction(DateTimeOffset        RequestTimestamp,
                                    IWebSocketConnection  WebSocketConnection,
                                    SourceRouting         Destination,
                                    NetworkPath           NetworkPath,
                                    EventTracking_Id      EventTrackingId,
                                    Request_Id            RequestId,
                                    Byte[]                BinaryRequest,
                                    CancellationToken     CancellationToken)

        {

            throw new NotImplementedException();

            //OCPP_Response? ocppResponse = null;

            //try
            //{

            //    if (StopTransactionRequest.TryParse(BinaryRequest,
            //                                         RequestId,
            //                                         Destination,
            //                                         NetworkPath,
            //                                         out var request,
            //                                         out var errorResponse,
            //                                         RequestTimestamp,
            //                                         parentNetworkingNode.OCPP.DefaultRequestTimeout,
            //                                         EventTrackingId,
            //                                         parentNetworkingNode.OCPP.CustomStopTransactionRequestParser)) {

            //        StopTransactionResponse? response = null;

            //        #region Verify request signature(s)

            //        if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
            //            request,
            //            request.ToBinary(
            //                //parentNetworkingNode.OCPP.CustomStopTransactionRequestSerializer,
            //                //parentNetworkingNode.OCPP.CustomChargingStationSerializer,
            //                //parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //                //parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //                IncludeSignatures: false
            //            ),
            //            out errorResponse))
            //        {

            //            response = StopTransactionResponse.SignatureError(
            //                           request,
            //                           errorResponse
            //                       );

            //        }

            //        #endregion

            //        #region Send OnStopTransactionRequestReceived event

            //        await LogEvent(
            //                  OnStopTransactionRequestReceived,
            //                  loggingDelegate => loggingDelegate.Invoke(
            //                      Timestamp.Now,
            //                      parentNetworkingNode,
            //                      WebSocketConnection,
            //                      request,
            //                      CancellationToken
            //                  )
            //              );

            //        #endregion


            //        response ??= await CallProcessor(
            //                               OnStopTransaction,
            //                               filter => filter.Invoke(
            //                                             Timestamp.Now,
            //                                             parentNetworkingNode,
            //                                             WebSocketConnection,
            //                                             request,
            //                                             CancellationToken
            //                                         )
            //                           );

            //        response ??= StopTransactionResponse.Failed(request);


            //        #region Sign response message

            //        parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
            //            response,
            //            response.ToBinary(
            //                //parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
            //                //parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
            //                //parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //                //parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //                IncludeSignatures: false
            //            ),
            //            out var errorResponse2
            //        );

            //        #endregion

            //        ocppResponse = OCPP_Response.BinaryResponse(
            //                           EventTrackingId,
            //                           SourceRouting.To(NetworkPath.Source),
            //                           NetworkPath.From(parentNetworkingNode.Id),
            //                           RequestId,
            //                           response.ToBinary(
            //                               //parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
            //                               //parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
            //                               //parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //                               //parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //                               IncludeSignatures: true
            //                           ),
            //                           async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnStopTransactionResponseSent(
            //                                                                Timestamp.Now,
            //                                                                parentNetworkingNode,
            //                                                                sentMessageResult.Connection,
            //                                                                request,
            //                                                                response,
            //                                                                response.Runtime,
            //                                                                sentMessageResult.Result,
            //                                                                CancellationToken
            //                                                            ),
            //                           CancellationToken
            //                       );

            //    }

            //    else
            //        ocppResponse = OCPP_Response.CouldNotParse(
            //                           EventTrackingId,
            //                           RequestId,
            //                           nameof(Receive_StopTransaction)[8..],
            //                           BinaryRequest,
            //                           errorResponse
            //                       );

            //}
            //catch (Exception e)
            //{

            //    ocppResponse = OCPP_Response.ExceptionOccurred(
            //                       EventTrackingId,
            //                       RequestId,
            //                       nameof(Receive_StopTransaction)[8..],
            //                       BinaryRequest,
            //                       e
            //                   );

            //}

            //return ocppResponse;

        }

        #endregion


        #region Receive StopTransaction response (JSON)

        /// <summary>
        /// An event fired whenever a StopTransaction response was received.
        /// </summary>
        public event OnStopTransactionResponseReceivedDelegate? OnStopTransactionResponseReceived;


        public async Task<StopTransactionResponse>

            Receive_StopTransactionResponse(StopTransactionRequest  Request,
                                            JObject                 ResponseJSON,
                                            IWebSocketConnection    WebSocketConnection,
                                            SourceRouting           Destination,
                                            NetworkPath             NetworkPath,
                                            EventTracking_Id        EventTrackingId,
                                            Request_Id              RequestId,
                                            DateTimeOffset?         ResponseTimestamp   = null,
                                            CancellationToken       CancellationToken   = default)

        {

            StopTransactionResponse? response = null;

            try
            {

                if (StopTransactionResponse.TryParse(Request,
                                                     ResponseJSON,
                                                     Destination,
                                                     NetworkPath,
                                                     out response,
                                                     out var errorResponse,
                                                     ResponseTimestamp,
                                                     parentNetworkingNode.OCPP.CustomStopTransactionResponseParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
                                parentNetworkingNode.OCPP.CustomIdTagInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = StopTransactionResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = StopTransactionResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = StopTransactionResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnStopTransactionResponseReceived event

            await LogEvent(
                      OnStopTransactionResponseReceived,
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

        #region Receive StopTransaction response (binary)

        public async Task<StopTransactionResponse>

            Receive_StopTransactionResponse(StopTransactionRequest  Request,
                                            Byte[]                  ResponseBinary,
                                            IWebSocketConnection    WebSocketConnection,
                                            SourceRouting           Destination,
                                            NetworkPath             NetworkPath,
                                            EventTracking_Id        EventTrackingId,
                                            Request_Id              RequestId,
                                            DateTimeOffset?         ResponseTimestamp   = null,
                                            CancellationToken       CancellationToken   = default)

        {

            throw new NotImplementedException();

            //StopTransactionResponse? response = null;

            //try
            //{

            //    var ResponseJSON = JObject.Parse(ResponseBinary.ToUTF8String());

            //    if (StopTransactionResponse.TryParse(Request,
            //                                          ResponseJSON,
            //                                          Destination,
            //                                          NetworkPath,
            //                                          out response,
            //                                          out var errorResponse,
            //                                          ResponseTimestamp,
            //                                          parentNetworkingNode.OCPP.CustomStopTransactionResponseParser,
            //                                          parentNetworkingNode.OCPP.CustomStatusInfoParser,
            //                                          parentNetworkingNode.OCPP.CustomSignatureParser,
            //                                          parentNetworkingNode.OCPP.CustomCustomDataParser)) {

            //        #region Verify response signature(s)

            //        if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //                response,
            //                response.ToJSON(
            //                    parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
            //                    parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
            //                    parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //                    parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //                ),
            //                out errorResponse
            //            ))
            //        {

            //            response = StopTransactionResponse.SignatureError(
            //                           Request,
            //                           errorResponse
            //                       );

            //        }

            //        #endregion

            //    }

            //    else
            //        response = StopTransactionResponse.FormationViolation(
            //                       Request,
            //                       errorResponse
            //                   );

            //}
            //catch (Exception e)
            //{

            //    response = StopTransactionResponse.ExceptionOccurred(
            //                   Request,
            //                   e
            //               );

            //}


            //#region Send OnStopTransactionResponseReceived event

            //await LogEvent(
            //          OnStopTransactionResponseReceived,
            //          loggingDelegate => loggingDelegate.Invoke(
            //              Timestamp.Now,
            //              parentNetworkingNode,
            //              WebSocketConnection,
            //              Request,
            //              response,
            //              response.Runtime,
            //              CancellationToken
            //          )
            //      );

            //#endregion

            //return response;

        }

        #endregion


        #region Receive StopTransaction request error

        /// <summary>
        /// An event fired whenever a StopTransaction request error was received.
        /// </summary>
        public event OnStopTransactionRequestErrorReceivedDelegate? StopTransactionRequestErrorReceived;


        public async Task<StopTransactionResponse>

            Receive_StopTransactionRequestError(StopTransactionRequest        Request,
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
            //        parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
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

            #region Send StopTransactionRequestErrorReceived event

            await LogEvent(
                      StopTransactionRequestErrorReceived,
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


            var response = StopTransactionResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnStopTransactionResponseReceived event

            await LogEvent(
                      OnStopTransactionResponseReceived,
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


        public async Task<StopTransactionResponse>

            Receive_StopTransactionRequestError(StopTransactionRequest          Request,
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
            //        parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
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

            #region Send StopTransactionRequestErrorReceived event

            //await LogEvent(
            //          StopTransactionRequestErrorReceived,
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


            var response = StopTransactionResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnStopTransactionResponseReceived event

            await LogEvent(
                      OnStopTransactionResponseReceived,
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

        #region Receive StopTransaction response error

        /// <summary>
        /// An event fired whenever a StopTransaction response error was received.
        /// </summary>
        public event OnStopTransactionResponseErrorReceivedDelegate? StopTransactionResponseErrorReceived;


        public async Task

            Receive_StopTransactionResponseError(StopTransactionRequest?        Request,
                                                 StopTransactionResponse?       Response,
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
            //        parentNetworkingNode.OCPP.CustomStopTransactionResponseSerializer,
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

            #region Send StopTransactionResponseErrorReceived event

            await LogEvent(
                      StopTransactionResponseErrorReceived,
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
