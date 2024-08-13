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
    /// A logging delegate called whenever a SecureDataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSecureDataTransferRequestReceivedDelegate(DateTime                    Timestamp,
                                                                     IEventSender                Sender,
                                                                     IWebSocketConnection        Connection,
                                                                     SecureDataTransferRequest   Request,
                                                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SecureDataTransfer response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSecureDataTransferResponseReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection         Connection,
                                                                      SecureDataTransferRequest?   Request,
                                                                      SecureDataTransferResponse   Response,
                                                                      TimeSpan?                    Runtime,
                                                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SecureDataTransfer request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSecureDataTransferRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                          IEventSender                   Sender,
                                                                          IWebSocketConnection           Connection,
                                                                          SecureDataTransferRequest?     Request,
                                                                          OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                          TimeSpan?                      Runtime,
                                                                          CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SecureDataTransfer response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSecureDataTransferResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                           IEventSender                    Sender,
                                                                           IWebSocketConnection            Connection,
                                                                           SecureDataTransferRequest?      Request,
                                                                           SecureDataTransferResponse?     Response,
                                                                           OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                           TimeSpan?                       Runtime,
                                                                           CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a SecureDataTransfer response is expected
    /// for a received SecureDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecureDataTransferResponse>

        OnSecureDataTransferDelegate(DateTime                 Timestamp,
                                  IEventSender                Sender,
                                  IWebSocketConnection        Connection,
                                  SecureDataTransferRequest   Request,
                                  CancellationToken           CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive SecureDataTransfer request

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferRequestReceivedDelegate?  OnSecureDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received for processing.
        /// </summary>
        public event OnSecureDataTransferDelegate?                 OnSecureDataTransfer;


        public async Task<OCPP_Response>

            Receive_SecureDataTransfer(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       NetworkingNode_Id     DestinationId,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       Byte[]                BinaryRequest,
                                       CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (SecureDataTransferRequest.TryParse(BinaryRequest,
                                                       RequestId,
                                                       DestinationId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       RequestTimestamp,
                                                       parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                       EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomSecureDataTransferRequestParser,
                                                       parentNetworkingNode.OCPP.CustomBinarySignatureParser)) {

                    SecureDataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToBinary(
                            parentNetworkingNode.OCPP.CustomSecureDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out errorResponse))
                    {

                        response = SecureDataTransferResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSecureDataTransferRequestReceived event

                    await LogEvent(
                              OnSecureDataTransferRequestReceived,
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

                            var responseTasks = OnSecureDataTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnSecureDataTransferDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : SecureDataTransferResponse.Failed(request, $"Undefined {nameof(OnSecureDataTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = SecureDataTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OnSecureDataTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= SecureDataTransferResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2
                    );

                    #endregion

                    ocppResponse = OCPP_Response.BinaryResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToBinary(
                                           parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSecureDataTransferResponseSent(
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
                                       nameof(Receive_SecureDataTransfer)[8..],
                                       BinaryRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SecureDataTransfer)[8..],
                                   BinaryRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SecureDataTransfer response

        /// <summary>
        /// An event fired whenever a SecureDataTransfer response was received.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate? OnSecureDataTransferResponseReceived;


        public async Task<SecureDataTransferResponse>

            Receive_SecureDataTransferResponse(SecureDataTransferRequest  Request,
                                               Byte[]                     ResponseBytes,
                                               IWebSocketConnection       WebSocketConnection,
                                               NetworkingNode_Id          DestinationId,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               DateTime?                  ResponseTimestamp   = null,
                                               CancellationToken          CancellationToken   = default)

        {

            SecureDataTransferResponse? response = null;

            try
            {

                if (SecureDataTransferResponse.TryParse(Request,
                                                        ResponseBytes,
                                                        DestinationId,
                                                        NetworkPath,
                                                        out response,
                                                        out var errorResponse,
                                                        ResponseTimestamp,
                                                        parentNetworkingNode.OCPP.CustomSecureDataTransferResponseParser,
                                                        parentNetworkingNode.OCPP.CustomBinarySignatureParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToBinary(
                                parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                                parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                IncludeSignatures: false
                            ),
                            out errorResponse
                        ))
                    {

                        response = SecureDataTransferResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SecureDataTransferResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SecureDataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnSecureDataTransferResponseReceived event

            await LogEvent(
                      OnSecureDataTransferResponseReceived,
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

        #region Receive SecureDataTransfer request error

        /// <summary>
        /// An event fired whenever a SecureDataTransfer request error was received.
        /// </summary>
        public event OnSecureDataTransferRequestErrorReceivedDelegate? SecureDataTransferRequestErrorReceived;


        public async Task<SecureDataTransferResponse>

            Receive_SecureDataTransferRequestError(SecureDataTransferRequest     Request,
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
            //        parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
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

            #region Send SecureDataTransferRequestErrorReceived event

            await LogEvent(
                      SecureDataTransferRequestErrorReceived,
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


            var response = SecureDataTransferResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSecureDataTransferResponseReceived event

            await LogEvent(
                      OnSecureDataTransferResponseReceived,
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

        #region Receive SecureDataTransfer response error

        /// <summary>
        /// An event fired whenever a SecureDataTransfer response error was received.
        /// </summary>
        public event OnSecureDataTransferResponseErrorReceivedDelegate? SecureDataTransferResponseErrorReceived;


        public async Task

            Receive_SecureDataTransferResponseError(SecureDataTransferRequest?     Request,
                                                    SecureDataTransferResponse?    Response,
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
            //        parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
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

            #region Send SecureDataTransferResponseErrorReceived event

            await LogEvent(
                      SecureDataTransferResponseErrorReceived,
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
