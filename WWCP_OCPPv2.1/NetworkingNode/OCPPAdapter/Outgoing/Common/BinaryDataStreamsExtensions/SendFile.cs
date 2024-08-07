﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile request was sent.
        /// </summary>
        public event OnSendFileRequestSentDelegate?         OnSendFileRequestSent;

        #endregion

        #region SendFile(Request)

        public async Task<SendFileResponse> SendFile(SendFileRequest Request)
        {

            #region Send OnSendFileRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileRequestSent?.Invoke(startTime,
                                              parentNetworkingNode,
                                              null,
                                              Request,
                                              SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendFileRequestSent));
            }

            #endregion


            SendFileResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryRequestAndWait(
                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToBinary(
                                                         parentNetworkingNode.OCPP.CustomSendFileRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                                         IncludeSignatures: true
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!SendFileResponse.TryParse(Request,
                                                   sendRequestState.JSONResponse.Payload,
                                                   out response,
                                                   out var errorResponse,
                                                   parentNetworkingNode.OCPP.CustomSendFileResponseParser))
                    {
                        response = new SendFileResponse(
                                       Request,
                                       Result.FormationViolation(errorResponse)
                                   );
                    }

                }

                response ??= new SendFileResponse(
                                 Request,
                                 Request.FileName,
                                 SendFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new SendFileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendFileResponseReceived event

            //var endTime = Timestamp.Now;

            //try
            //{

            //    OnSendFileResponseReceived?.Invoke(endTime,
            //                                       parentNetworkingNode,
            //                                       Request,
            //                                       response,
            //                                       endTime - startTime);

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendFileResponseReceived));
            //}

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a SendFile request was received.
        /// </summary>
        public event OnSendFileResponseReceivedDelegate?  OnSendFileResponseReceived;

        #endregion


        #region Receive SendFileRequestError

        public async Task<SendFileResponse>

            Receive_SendFileRequestError(SendFileRequest               Request,
                                         OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                         IWebSocketConnection          WebSocketConnection)

        {

            var response = SendFileResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSendFileResponseSerializer,
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

            #region Send OnSendFileResponseReceived event

            var logger = OnSendFileResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnSendFileResponseReceivedDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendFileResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

}
