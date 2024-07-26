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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for sending messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestSentDelegate? OnDataTransferRequestSent;

        #endregion

        #region DataTransfer(Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<DataTransferResponse> DataTransfer(DataTransferRequest Request)
        {

            DataTransferResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {
                    response = new DataTransferResponse(
                                   Request,
                                   Result.SignatureError(signingErrors)
                               );
                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     async sendMessageResult => {

                                                         #region Send OnDataTransferRequestSent event

                                                         var logger = OnDataTransferRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                        OfType<OnDataTransferRequestSentDelegate>().
                                                                                        Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                                                      Timestamp.Now,
                                                                                                                      parentNetworkingNode,
                                                                                                                      sendMessageResult.Connection,
                                                                                                                      Request,
                                                                                                                      sendMessageResult.Result
                                                                                                                  )).
                                                                                        ToArray());

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnDataTransferRequestSent));
                                                             }
                                                         }

                                                         #endregion

                                                     }

                                                 );

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                    {

                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_DataTransferResponse(
                                                                                Request,
                                                                                jsonResponse,
                                                                                null,
                                                                                sendRequestState.DestinationIdReceived,
                                                                                sendRequestState.NetworkPathReceived,
                                                                                Request.         EventTrackingId,
                                                                                Request.         RequestId,
                                                                                sendRequestState.ResponseTimestamp,
                                                                                Request.         CancellationToken
                                                                            );

                    }

                    response ??= new DataTransferResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = new DataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP adapter for receiving messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event fired whenever a DataTransfer response was received.
        /// </summary>
        public event OnDataTransferResponseReceivedDelegate? OnDataTransferResponseReceived;

        #endregion

        #region Receive DataTransfer response (wired via reflection!)

        public async Task<DataTransferResponse>

            Receive_DataTransferResponse(DataTransferRequest   Request,
                                         JObject               ResponseJSON,
                                         IWebSocketConnection  WebSocketConnection,
                                         NetworkingNode_Id     DestinationId,
                                         NetworkPath           NetworkPath,
                                         EventTracking_Id      EventTrackingId,
                                         Request_Id            RequestId,
                                         DateTime?             ResponseTimestamp   = null,
                                         CancellationToken     CancellationToken   = default)

        {

            var response = DataTransferResponse.Failed(Request);

            try
            {

                if (DataTransferResponse.TryParse(Request,
                                                      ResponseJSON,
                                                      DestinationId,
                                                      NetworkPath,
                                                      out response,
                                                      out var errorResponse,
                                                      ResponseTimestamp,
                                                      parentNetworkingNode.OCPP.CustomDataTransferResponseParser,
                                                      parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                      parentNetworkingNode.OCPP.CustomSignatureParser,
                                                      parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse
                    );

                    #region Send OnDataTransferResponseReceived event

                    var logger = OnDataTransferResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                      OfType <OnDataTransferResponseReceivedDelegate>().
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
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDataTransferResponseReceived));
                        }
                    }

                    #endregion

                }

                else
                    response = new DataTransferResponse(
                                   Request,
                                   Result.Format(errorResponse)
                               );

            }
            catch (Exception e)
            {

                response = new DataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

    }

}
