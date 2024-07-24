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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<BinaryDataTransferResponse>?     CustomBinaryDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnBinaryDataTransferRequestSentDelegate?         OnBinaryDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                         OnBinaryDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?                        OnBinaryDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate?    OnBinaryDataTransferResponse;

        #endregion


        #region TransferBinaryData(Request)

        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        {

            BinaryDataTransferResponse? response = null;

            try
            {

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
                    response = new BinaryDataTransferResponse(
                                   Request,
                                   Result.SignatureError(signingErrors)
                               );
                }

                else
                {

                    var sendRequestState = await SendBinaryRequestAndWait(

                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToBinary(
                                                         CustomBinaryDataTransferRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                     )
                                                 ),

                                                 async sendMessageResult => {

                                                         #region Send OnBinaryDataTransferRequest event

                                                         var logger = OnBinaryDataTransferRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                         OfType<OnBinaryDataTransferRequestSentDelegate>().
                                                                                         Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                                                       Timestamp.Now,
                                                                                                                       parentNetworkingNode,
                                                                                                                       Request,
                                                                                                                       sendMessageResult
                                                                                                                   )).
                                                                                         ToArray());

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryDataTransferRequestSent));
                                                             }
                                                         }

                                                     #endregion

                                                 }

                                             );


                    if (sendRequestState.IsValidBinaryResponse(Request, out var binaryResponse))
                    {

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

                    }

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

        #region Events

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer response was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate? OnBinaryDataTransferResponseReceived;

        #endregion

        #region Receive BinaryDataTransfer response (wired via reflection!)

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

                    var logger = OnBinaryDataTransferResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                      OfType <OnBinaryDataTransferResponseReceivedDelegate>().
                                                      Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      Timestamp.Now,
                                                                                      parentNetworkingNode,
                                                                                      //    WebSocketConnection,
                                                                                      Request,
                                                                                      response,
                                                                                      response.Runtime
                                                                                  )).
                                                      ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBinaryDataTransferResponseReceived));
                        }
                    }

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

    }

}
