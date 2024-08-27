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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a GetFile request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetFileRequestReceivedDelegate(DateTime               Timestamp,
                                                          IEventSender           Sender,
                                                          IWebSocketConnection   Connection,
                                                          GetFileRequest         Request,
                                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetFile response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetFileResponseReceivedDelegate(DateTime                Timestamp,
                                                           IEventSender            Sender,
                                                           IWebSocketConnection?   Connection,
                                                           GetFileRequest?         Request,
                                                           GetFileResponse         Response,
                                                           TimeSpan?               Runtime,
                                                           CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetFile request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetFileRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                               IEventSender                   Sender,
                                                               IWebSocketConnection           Connection,
                                                               GetFileRequest?                Request,
                                                               OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                               TimeSpan?                      Runtime,
                                                               CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetFile response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetFileResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                GetFileRequest?                 Request,
                                                                GetFileResponse?                Response,
                                                                OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                TimeSpan?                       Runtime,
                                                                CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetFile response is expected
    /// for a received GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetFileResponse>

        OnGetFileDelegate(DateTime               Timestamp,
                          IEventSender           Sender,
                          IWebSocketConnection   Connection,
                          GetFileRequest         Request,
                          CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetFile request

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OnGetFileRequestReceivedDelegate?  OnGetFileRequestReceived;

        /// <summary>
        /// An event sent whenever a GetFile request was received for processing.
        /// </summary>
        public event OnGetFileDelegate?                 OnGetFile;


        public async Task<OCPP_Response>

            Receive_GetFile(DateTime              RequestTimestamp,
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

                if (GetFileRequest.TryParse(JSONRequest,
                                            RequestId,
                                        Destination,
                                            NetworkPath,
                                            out var request,
                                            out var errorResponse,
                                            RequestTimestamp,
                                            parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                            EventTrackingId,
                                            parentNetworkingNode.OCPP.CustomGetFileRequestParser)) {

                    GetFileResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetFileRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetFileResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetFileRequestReceived event

                    await LogEvent(
                              OnGetFileRequestReceived,
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
                                           OnGetFile,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetFileResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2
                    );

                    #endregion

                    ocppResponse = OCPP_Response.BinaryResponse(
                                       EventTrackingId,
                                       SourceRouting.To(NetworkPath.Source),
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToBinary(
                                           parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetFileResponseSent(
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
                                       nameof(Receive_GetFile)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetFile)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetFile response

        /// <summary>
        /// An event fired whenever a GetFile response was received.
        /// </summary>
        public event OnGetFileResponseReceivedDelegate? OnGetFileResponseReceived;


        public async Task<GetFileResponse>

            Receive_GetFileResponse(GetFileRequest        Request,
                                    Byte[]                ResponseBytes,
                                    IWebSocketConnection  WebSocketConnection,
                                    SourceRouting     Destination,
                                    NetworkPath           NetworkPath,
                                    EventTracking_Id      EventTrackingId,
                                    Request_Id            RequestId,
                                    DateTime?             ResponseTimestamp   = null,
                                    CancellationToken     CancellationToken   = default)

        {

            GetFileResponse? response = null;

            try
            {

                if (GetFileResponse.TryParse(Request,
                                             ResponseBytes,
                                         Destination,
                                             NetworkPath,
                                             out response,
                                             out var errorResponse,
                                             ResponseTimestamp,
                                             parentNetworkingNode.OCPP.CustomGetFileResponseParser,
                                             parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                             parentNetworkingNode.OCPP.CustomSignatureParser,
                                             parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToBinary(
                                parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer,
                                IncludeSignatures: false
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetFileResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetFileResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetFileResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetFileResponseReceived event

            await LogEvent(
                      OnGetFileResponseReceived,
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

        #region Receive GetFile request error

        /// <summary>
        /// An event fired whenever a GetFile request error was received.
        /// </summary>
        public event OnGetFileRequestErrorReceivedDelegate? GetFileRequestErrorReceived;


        public async Task<GetFileResponse>

            Receive_GetFileRequestError(GetFileRequest                Request,
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
            //        parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
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

            #region Send GetFileRequestErrorReceived event

            await LogEvent(
                      GetFileRequestErrorReceived,
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


            var response = GetFileResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetFileResponseReceived event

            await LogEvent(
                      OnGetFileResponseReceived,
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

        #region Receive GetFile response error

        /// <summary>
        /// An event fired whenever a GetFile response error was received.
        /// </summary>
        public event OnGetFileResponseErrorReceivedDelegate? GetFileResponseErrorReceived;


        public async Task

            Receive_GetFileResponseError(GetFileRequest?                Request,
                                         GetFileResponse?               Response,
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
            //        parentNetworkingNode.OCPP.CustomGetFileResponseSerializer,
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

            #region Send GetFileResponseErrorReceived event

            await LogEvent(
                      GetFileResponseErrorReceived,
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
