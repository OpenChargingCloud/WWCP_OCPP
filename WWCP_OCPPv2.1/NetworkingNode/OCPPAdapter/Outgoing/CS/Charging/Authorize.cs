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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestSentDelegate?     OnAuthorizeRequestSent;

        #endregion

        #region Authorize(Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An Authorize request.</param>
        public async Task<AuthorizeResponse> Authorize(AuthorizeRequest  Request)
        {

            AuthorizeResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {
                    response = new AuthorizeResponse(
                                   Request,
                                   Result.SignatureError(signingErrors),
                                   AuthorizationStatus.SignatureError
                               );
                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomAuthorizeRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                             parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                             parentNetworkingNode.OCPP.CustomOCSPRequestDataSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     async sendMessageResult => {

                                                         #region Send OnAuthorizeRequestSent event

                                                         var logger = OnAuthorizeRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                         OfType<OnAuthorizeRequestSentDelegate>().
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
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnAuthorizeRequestSent));
                                                             }
                                                         }

                                                         #endregion

                                                     }

                                                 );

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_AuthorizeResponse(
                                             Request,
                                             jsonResponse,
                                             null,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    else if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_AuthorizeRequestError(
                                             Request,
                                             jsonRequestError,
                                             null
                                         );

                    response ??= new AuthorizeResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState),
                                     AuthorizationStatus.Error
                                 );

                }

            }
            catch (Exception e)
            {

                response = new AuthorizeResponse(
                               Request,
                               Result.FromException(e),
                               AuthorizationStatus.Error
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
        /// An event fired whenever a Authorize response was received.
        /// </summary>
        public event OnAuthorizeResponseReceivedDelegate? OnAuthorizeResponseReceived;

        #endregion

        #region Receive AuthorizeResponse (wired via reflection!)

        public async Task<AuthorizeResponse>

            Receive_AuthorizeResponse(AuthorizeRequest      Request,
                                      JObject               ResponseJSON,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id     DestinationId,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      DateTime?             ResponseTimestamp   = null,
                                      CancellationToken     CancellationToken   = default)

        {

            var response = AuthorizeResponse.Failed(Request);

            try
            {

                if (AuthorizeResponse.TryParse(Request,
                                               ResponseJSON,
                                               DestinationId,
                                               NetworkPath,
                                               out response,
                                               out var errorResponse,
                                               ResponseTimestamp,
                                               parentNetworkingNode.OCPP.CustomAuthorizeResponseParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse
                    );

                    #region Send OnAuthorizeResponseReceived event

                    var logger = OnAuthorizeResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                      OfType <OnAuthorizeResponseReceivedDelegate>().
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
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeResponseReceived));
                        }
                    }

                    #endregion

                }

                else
                    response = new AuthorizeResponse(
                                   Request,
                                   Result.Format(errorResponse),
                                   AuthorizationStatus.ParsingError
                               );

            }
            catch (Exception e)
            {

                response = new AuthorizeResponse(
                               Request,
                               Result.FromException(e),
                               AuthorizationStatus.Error
                           );

            }

            return response;

        }

        #endregion

        #region Receive AuthorizeRequestError

        public async Task<AuthorizeResponse>

            Receive_AuthorizeRequestError(AuthorizeRequest              Request,
                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                          IWebSocketConnection          WebSocketConnection)

        {

            var response = AuthorizeResponse.RequestError(
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
            //        parentNetworkingNode.OCPP.CustomAuthorizeResponseSerializer,
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

            #region Send OnAuthorizeResponseReceived event

            var logger = OnAuthorizeResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType <OnAuthorizeResponseReceivedDelegate>().
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

}
