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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnBinaryDataTransferRequestReceivedDelegate(DateTime                    Timestamp,
                                                    IEventSender                Sender,
                                                    IWebSocketConnection        Connection,
                                                    BinaryDataTransferRequest   Request,
                                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnBinaryDataTransferResponseReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection         Connection,
                                                                      BinaryDataTransferRequest?   Request,
                                                                      BinaryDataTransferResponse   Response,
                                                                      TimeSpan?                    Runtime,
                                                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnBinaryDataTransferRequestErrorReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IWebSocketConnection             Connection,
                                                                          BinaryDataTransferRequest?       Request,
                                                                          OCPP_BinaryRequestErrorMessage   RequestErrorMessage,
                                                                          TimeSpan?                        Runtime,
                                                                          CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a BinaryDataTransfer response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnBinaryDataTransferResponseErrorReceivedDelegate(DateTime                          Timestamp,
                                                                           IEventSender                      Sender,
                                                                           IWebSocketConnection              Connection,
                                                                           BinaryDataTransferRequest?        Request,
                                                                           BinaryDataTransferResponse?       Response,
                                                                           OCPP_BinaryResponseErrorMessage   ResponseErrorMessage,
                                                                           TimeSpan?                         Runtime,
                                                                           CancellationToken                 CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a BinaryDataTransfer response is expected
    /// for a received BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<BinaryDataTransferResponse>

        OnBinaryDataTransferDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     IWebSocketConnection        Connection,
                                     BinaryDataTransferRequest   Request,
                                     CancellationToken           CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive BinaryDataTransfer request

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferRequestReceivedDelegate?  OnBinaryDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received for processing.
        /// </summary>
        public event OnBinaryDataTransferDelegate?                 OnBinaryDataTransfer;


        public async Task<OCPP_Response>

            Receive_BinaryDataTransfer(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       SourceRouting         SourceRouting,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       Byte[]                BinaryRequest,
                                       CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (BinaryDataTransferRequest.TryParse(BinaryRequest,
                                                       RequestId,
                                                       SourceRouting,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       RequestTimestamp,
                                                       parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                       EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestParser)) {

                    BinaryDataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out errorResponse))
                    {

                        response = BinaryDataTransferResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnBinaryDataTransferRequestReceived event

                    await LogEvent(
                              OnBinaryDataTransferRequestReceived,
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

                            var responseTasks = OnBinaryDataTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnBinaryDataTransferDelegate)?.Invoke(
                                                                                                                               Timestamp.Now,
                                                                                                                               parentNetworkingNode,
                                                                                                                               WebSocketConnection,
                                                                                                                               request,
                                                                                                                               CancellationToken
                                                                                                                           )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : BinaryDataTransferResponse.Failed(request, $"Undefined {nameof(OnBinaryDataTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = BinaryDataTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnBinaryDataTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= BinaryDataTransferResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: true
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
                                           parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnBinaryDataTransferResponseSent(
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
                                       nameof(Receive_BinaryDataTransfer)[8..],
                                       BinaryRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_BinaryDataTransfer)[8..],
                                   BinaryRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive BinaryDataTransfer response

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer response was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate? OnBinaryDataTransferResponseReceived;


        public async Task<BinaryDataTransferResponse>

            Receive_BinaryDataTransferResponse(BinaryDataTransferRequest  Request,
                                               Byte[]                     ResponseBytes,
                                               IWebSocketConnection       WebSocketConnection,
                                               SourceRouting              SourceRouting,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               DateTime?                  ResponseTimestamp   = null,
                                               CancellationToken          CancellationToken   = default)

        {

            BinaryDataTransferResponse? response = null;

            try
            {

                if (BinaryDataTransferResponse.TryParse(Request,
                                                        ResponseBytes,
                                                        SourceRouting,
                                                        NetworkPath,
                                                        out response,
                                                        out var errorResponse,
                                                        ResponseTimestamp,
                                                        parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToBinary(
                                parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                IncludeSignatures: true
                            ),
                            out errorResponse
                        ))
                    {

                        response = BinaryDataTransferResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = BinaryDataTransferResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = BinaryDataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnBinaryDataTransferResponseReceived event

            await LogEvent(
                      OnBinaryDataTransferResponseReceived,
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

        #region Receive BinaryDataTransfer RequestError

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request error was received.
        /// </summary>
        public event OnBinaryDataTransferRequestErrorReceivedDelegate? BinaryDataTransferRequestErrorReceived;


        public async Task<BinaryDataTransferResponse>

            Receive_BinaryDataTransferRequestError(BinaryDataTransferRequest       Request,
                                                   OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                                   IWebSocketConnection            Connection,
                                                   SourceRouting                   SourceRouting,
                                                   NetworkPath                     NetworkPath,
                                                   EventTracking_Id                EventTrackingId,
                                                   Request_Id                      RequestId,
                                                   DateTime?                       ResponseTimestamp   = null,
                                                   CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
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

            #region Send BinaryDataTransferRequestErrorReceived event

            await LogEvent(
                      BinaryDataTransferRequestErrorReceived,
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


            var response = BinaryDataTransferResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnBinaryDataTransferResponseReceived event

            await LogEvent(
                      OnBinaryDataTransferResponseReceived,
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

        #region Receive BinaryDataTransfer ResponseError

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer response error was received.
        /// </summary>
        public event OnBinaryDataTransferResponseErrorReceivedDelegate? BinaryDataTransferResponseErrorReceived;


        public async Task

            Receive_BinaryDataTransferResponseError(BinaryDataTransferRequest?       Request,
                                                    BinaryDataTransferResponse?      Response,
                                                    OCPP_BinaryResponseErrorMessage  ResponseErrorMessage,
                                                    IWebSocketConnection             Connection,
                                                    SourceRouting                    SourceRouting,
                                                    NetworkPath                      NetworkPath,
                                                    EventTracking_Id                 EventTrackingId,
                                                    Request_Id                       RequestId,
                                                    DateTime?                        ResponseTimestamp   = null,
                                                    CancellationToken                CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
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

            #region Send BinaryDataTransferResponseErrorReceived event

            await LogEvent(
                      BinaryDataTransferResponseErrorReceived,
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
