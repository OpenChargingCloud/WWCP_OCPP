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

    #region Delegates

    /// <summary>
    /// A delegate called whenever a BinaryDataTransfer request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    public delegate Task OnBinaryDataTransferRequestSentDelegate(DateTime                    Timestamp,
                                                                 IEventSender                Sender,
                                                                 IWebSocketConnection        Connection,
                                                                 BinaryDataTransferRequest   Request,
                                                                 SentMessageResults          SendMessageResult);


    /// <summary>
    /// A delegate called whenever a response to a BinaryDataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBinaryDataTransferResponseReceivedDelegate(DateTime                     Timestamp,
                                                                      IEventSender                 Sender,
                                                                      IWebSocketConnection         Connection,
                                                                      BinaryDataTransferRequest    Request,
                                                                      BinaryDataTransferResponse   Response,
                                                                      TimeSpan                     Runtime);


    /// <summary>
    /// A delegate called whenever a RequestError to a BinaryDataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="RequestErrorMessage">The RequestErrorMessage.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBinaryDataTransferRequestErrorReceivedDelegate(DateTime                         Timestamp,
                                                                          IEventSender                     Sender,
                                                                          IWebSocketConnection             Connection,
                                                                          BinaryDataTransferRequest        Request,
                                                                          OCPP_BinaryRequestErrorMessage   RequestErrorMessage,
                                                                          TimeSpan                         Runtime);

    #endregion


    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnBinaryDataTransferRequestSentDelegate?         OnBinaryDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate?    OnBinaryDataTransferResponse;

        #endregion

        #region BinaryDataTransfer(Request)

        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        {

            BinaryDataTransferResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: true
                        ),
                        out var signingErrors
                    ))
                {
                    response = BinaryDataTransferResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );
                }

                #endregion

                else
                {

                    #region Send request message

                    var sendRequestState = await SendBinaryRequestAndWait(

                                               OCPP_BinaryRequestMessage.FromRequest(
                                                   Request,
                                                   Request.ToBinary(
                                                       parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                                                       parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                   )
                                               ),

                                               sendMessageResult => LogEvent(
                                                   OnBinaryDataTransferRequestSent,
                                                   loggingDelegate => loggingDelegate.Invoke(
                                                       Timestamp.Now,
                                                       parentNetworkingNode,
                                                       sendMessageResult.Connection,
                                                       Request,
                                                       sendMessageResult.Result
                                                   )
                                               )

                                           );

                    #endregion

                    if (sendRequestState.IsValidBinaryResponse(Request, out var binaryResponse))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_BinaryDataTransferResponse(
                                             Request,
                                             binaryResponse,
                                             null,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    if (sendRequestState.IsValidBinaryRequestError(Request, out var binaryRequestError))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_BinaryDataTransferRequestError(
                                             Request,
                                             binaryRequestError,
                                             null,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    response ??= new BinaryDataTransferResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = new BinaryDataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Receive BinaryDataTransferResponse (wired via reflection!)

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer response was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate? OnBinaryDataTransferResponseReceived;


        public async Task<BinaryDataTransferResponse>

            Receive_BinaryDataTransferResponse(BinaryDataTransferRequest  Request,
                                               Byte[]                     ResponseBytes,
                                               IWebSocketConnection       WebSocketConnection,
                                               NetworkingNode_Id          DestinationId,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               DateTime?                  ResponseTimestamp   = null,
                                               CancellationToken          CancellationToken   = default)

        {

            var response = BinaryDataTransferResponse.Failed(Request);

            try
            {

                if (BinaryDataTransferResponse.TryParse(Request,
                                                        ResponseBytes,
                                                        DestinationId,
                                                        NetworkPath,
                                                        out response,
                                                        out var errorResponse,
                                                        ResponseTimestamp,
                                                        parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: true
                        ),
                        out errorResponse
                    );

                    #region Send OnBinaryDataTransferResponseReceived event

                    await LogEvent(
                              OnBinaryDataTransferResponseReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Timestamp.Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  Request,
                                  response,
                                  response.Runtime
                              )
                          );

                    #endregion

                }

                else
                    response = new BinaryDataTransferResponse(
                                   Request,
                                   Result.Format(errorResponse)
                               );

            }
            catch (Exception e)
            {

                response = new BinaryDataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

        #region Receive BinaryDataTransferRequestError

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer RequestError was received.
        /// </summary>
        public event OnBinaryDataTransferRequestErrorReceivedDelegate? BinaryDataTransferRequestErrorReceived;


        public async Task<BinaryDataTransferResponse>

            Receive_BinaryDataTransferRequestError(BinaryDataTransferRequest       Request,
                                                   OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                                   IWebSocketConnection            WebSocketConnection,
                                                   NetworkingNode_Id               DestinationId,
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
                          WebSocketConnection,
                          Request,
                          RequestErrorMessage,
                          RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp
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
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnBinaryDataTransferResponseReceived event

            await LogEvent(
                      OnBinaryDataTransferResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          Request,
                          response,
                          response.Runtime
                      )
                  );

            #endregion

            return response;

        }

        #endregion

    }

}
