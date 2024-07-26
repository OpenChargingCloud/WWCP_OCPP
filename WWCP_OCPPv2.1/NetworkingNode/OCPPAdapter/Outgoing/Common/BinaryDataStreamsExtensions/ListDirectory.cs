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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a ListDirectory request was sent.
        /// </summary>
        public event OnListDirectoryRequestSentDelegate?  OnListDirectoryRequestSent;

        #endregion

        #region ListDirectory(Request)

        public async Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        {

            #region Send OnListDirectoryRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnListDirectoryRequestSent?.Invoke(startTime,
                                                   parentNetworkingNode,
                                                   null,
                                                   Request,
                                                   SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnListDirectoryRequestSent));
            }

            #endregion


            ListDirectoryResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         parentNetworkingNode.OCPP.CustomListDirectoryRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!ListDirectoryResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out response,
                                                        out var errorResponse,
                                                        parentNetworkingNode.OCPP.CustomListDirectoryResponseParser))
                    {
                        response = new ListDirectoryResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new ListDirectoryResponse(
                                 Request,
                                 Request.DirectoryPath,
                                 ListDirectoryStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new ListDirectoryResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnListDirectoryResponseReceived event

            //var endTime = Timestamp.Now;

            //try
            //{

            //    OnListDirectoryResponseReceived?.Invoke(endTime,
            //                                            parentNetworkingNode,
            //                                            Request,
            //                                            response,
            //                                            endTime - startTime);

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnListDirectoryResponseReceived));
            //}

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was received.
        /// </summary>
        public event OnListDirectoryResponseReceivedDelegate?  OnListDirectoryResponseReceived;

        #endregion


        #region Receive ListDirectoryRequestError

        public async Task<ListDirectoryResponse>

            Receive_ListDirectoryRequestError(ListDirectoryRequest          Request,
                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection          WebSocketConnection)

        {

            var response = ListDirectoryResponse.RequestError(
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
            //        parentNetworkingNode.OCPP.CustomListDirectoryResponseSerializer,
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

            #region Send OnListDirectoryResponseReceived event

            var logger = OnListDirectoryResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnListDirectoryResponseReceivedDelegate>().
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnListDirectoryResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

}
