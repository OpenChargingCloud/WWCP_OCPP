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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a DataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnDataTransferRequestReceivedDelegate(DateTime               Timestamp,
                                              IEventSender           Sender,
                                              IWebSocketConnection   Connection,
                                              DataTransferRequest    Request,
                                              CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDataTransferResponseReceivedDelegate(DateTime                Timestamp,
                                                                IEventSender            Sender,
                                                                IWebSocketConnection?   Connection,
                                                                DataTransferRequest?    Request,
                                                                DataTransferResponse    Response,
                                                                TimeSpan?               Runtime,
                                                                CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request/request error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDataTransferRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                    IEventSender                   Sender,
                                                                    IWebSocketConnection           Connection,
                                                                    DataTransferRequest?           Request,
                                                                    OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                    TimeSpan?                      Runtime,
                                                                    CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a DataTransfer response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnDataTransferResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                     IEventSender                    Sender,
                                                                     IWebSocketConnection            Connection,
                                                                     DataTransferRequest?            Request,
                                                                     DataTransferResponse?           Response,
                                                                     OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                     TimeSpan?                       Runtime,
                                                                     CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a DataTransfer response is expected
    /// for a received DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The DataTransfer request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<DataTransferResponse>

        OnDataTransferDelegate(DateTime               Timestamp,
                               IEventSender           Sender,
                               IWebSocketConnection   Connection,
                               DataTransferRequest    Request,
                               CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive DataTransfer request

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferRequestReceivedDelegate?  OnDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received for processing.
        /// </summary>
        public event OnDataTransferDelegate?                 OnDataTransfer;


        public async Task<OCPP_Response>

            Receive_DataTransfer(DateTime              RequestTimestamp,
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

                if (DataTransferRequest.TryParse(JSONRequest,
                                                 RequestId,
                                             Destination,
                                                 NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 RequestTimestamp,
                                                 parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                 EventTrackingId,
                                                 parentNetworkingNode.OCPP.CustomDataTransferRequestParser)) {

                    DataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = DataTransferResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnDataTransferRequestReceived event

                    await LogEvent(
                              OnDataTransferRequestReceived,
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
                                           OnDataTransfer,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= DataTransferResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnDataTransferResponseSent(
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
                                       nameof(Receive_DataTransfer)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_DataTransfer)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive DataTransfer response

        /// <summary>
        /// An event fired whenever a DataTransfer response was received.
        /// </summary>
        public event OnDataTransferResponseReceivedDelegate? OnDataTransferResponseReceived;


        public async Task<DataTransferResponse>

            Receive_DataTransferResponse(DataTransferRequest   Request,
                                         JObject               ResponseJSON,
                                         IWebSocketConnection  WebSocketConnection,
                                         SourceRouting     Destination,
                                         NetworkPath           NetworkPath,
                                         EventTracking_Id      EventTrackingId,
                                         Request_Id            RequestId,
                                         DateTime?             ResponseTimestamp   = null,
                                         CancellationToken     CancellationToken   = default)

        {

            DataTransferResponse? response = null;

            try
            {

                if (DataTransferResponse.TryParse(Request,
                                                  ResponseJSON,
                                              Destination,
                                                  NetworkPath,
                                                  out response,
                                                  out var errorResponse,
                                                  ResponseTimestamp,
                                                  parentNetworkingNode.OCPP.CustomDataTransferResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = DataTransferResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = DataTransferResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = DataTransferResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnDataTransferResponseReceived event

            await LogEvent(
                      OnDataTransferResponseReceived,
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

        #region Receive DataTransfer RequestError

        /// <summary>
        /// An event fired whenever a DataTransfer request error was received.
        /// </summary>
        public event OnDataTransferRequestErrorReceivedDelegate? DataTransferRequestErrorReceived;


        public async Task<DataTransferResponse>

            Receive_DataTransferRequestError(DataTransferRequest           Request,
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
            //        parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
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

            #region Send DataTransferRequestErrorReceived event

            await LogEvent(
                      DataTransferRequestErrorReceived,
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


            var response = DataTransferResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnDataTransferResponseReceived event

            await LogEvent(
                      OnDataTransferResponseReceived,
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

        #region Receive DataTransfer ResponseError

        /// <summary>
        /// An event fired whenever a DataTransfer response error was received.
        /// </summary>
        public event OnDataTransferResponseErrorReceivedDelegate? DataTransferResponseErrorReceived;


        public async Task

            Receive_DataTransferResponseError(DataTransferRequest?           Request,
                                              DataTransferResponse?          Response,
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
            //        parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
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

            #region Send DataTransferResponseErrorReceived event

            await LogEvent(
                      DataTransferResponseErrorReceived,
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
