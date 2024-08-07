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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListRequestSentDelegate?     OnSendLocalListRequestSent;

        #endregion

        #region SendLocalList(Request)

        public async Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            SendLocalListResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                            parentNetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = SendLocalListResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );

                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomSendLocalListRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomAuthorizationDataSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     async sendMessageResult =>
                                                     {

                                                         var logger = OnSendLocalListRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(
                                                                           logger.GetInvocationList().
                                                                               OfType<OnSendLocalListRequestSentDelegate>().
                                                                               Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                                             Timestamp.Now,
                                                                                                             parentNetworkingNode,
                                                                                                             sendMessageResult.Connection,
                                                                                                             Request,
                                                                                                             sendMessageResult.Result
                                                                                                         ))
                                                                       );

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnJSONRequestMessageSent));
                                                             }
                                                         }

                                                     });

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (SendLocalListResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var sendLocalListResponse,
                                                           out var errorResponse,
                                                           parentNetworkingNode.OCPP.CustomSendLocalListResponseParser))
                        {
                            response = sendLocalListResponse;
                        }

                        response ??= new SendLocalListResponse(
                                         Request,
                                         Result.FormationViolation(errorResponse)
                                     );

                    }

                    response ??= new SendLocalListResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = new SendLocalListResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendLocalListResponse event

            //await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterIN).SendOnCertificateSignedResponseReceived(
            //                  Timestamp.Now,
            //                  parentNetworkingNode,
            //                  sendRequestState.,
            //                  request,
            //                  response,
            //                  response.Runtime
            //              );

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListResponseReceivedDelegate? OnSendLocalListResponseReceived;

        #endregion

        #region Send OnSendLocalListResponse event

        public async Task SendOnSendLocalListResponseReceived(DateTime               Timestamp,
                                                              IEventSender           Sender,
                                                              IWebSocketConnection   Connection,
                                                              SendLocalListRequest   Request,
                                                              SendLocalListResponse  Response,
                                                              TimeSpan               Runtime)
        {

            var logger = OnSendLocalListResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnSendLocalListResponseReceivedDelegate>().
                                  Select(filterDelegate => filterDelegate.Invoke(
                                                               Timestamp,
                                                               Sender,
                                                               //Connection,
                                                               Request,
                                                               Response,
                                                               Runtime
                                                           ))
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterIN),
                              nameof(OnSendLocalListResponseReceived),
                              e
                          );
                }

            }

        }

        #endregion

        #region Receive SendLocalListRequestError

        public async Task<SendLocalListResponse>

            Receive_SendLocalListRequestError(SendLocalListRequest          Request,
                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection          WebSocketConnection)

        {

            var response = SendLocalListResponse.RequestError(
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
            //        parentNetworkingNode.OCPP.CustomSendLocalListResponseSerializer,
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

            #region Send OnSendLocalListResponseReceived event

            var logger = OnSendLocalListResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnSendLocalListResponseReceivedDelegate>().
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendLocalListResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

}
