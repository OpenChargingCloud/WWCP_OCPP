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

    #region Delegates

    /// <summary>
    /// A delegate called whenever a RequestError to a Authorize request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="RequestErrorMessage">The RequestErrorMessage.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnAuthorizeRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                 IEventSender                   Sender,
                                                                 IWebSocketConnection           Connection,
                                                                 AuthorizeRequest               Request,
                                                                 OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                 TimeSpan                       Runtime);

    #endregion


    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event fired whenever an Authorize request will be sent.
        /// </summary>
        public event OnAuthorizeRequestSentDelegate?  OnAuthorizeRequestSent;

        #endregion

        #region Authorize(Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An Authorize request.</param>
        public async Task<AuthorizeResponse> Authorize(AuthorizeRequest Request)
        {

            AuthorizeResponse? response = null;

            try
            {

                #region Sign request message

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
                    response = AuthorizeResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );
                }

                #endregion

                else
                {

                    #region Send request message

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

                                                     sendMessageResult => LogEvent(
                                                         OnAuthorizeRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sendMessageResult.Connection,
                                                             Request,
                                                             sendMessageResult.Result
                                                         )
                                                     )

                                                     //async sendMessageResult => {

                                                     //    #region Send OnAuthorizeRequestSent event

                                                     //    var logger = OnAuthorizeRequestSent;
                                                     //    if (logger is not null)
                                                     //    {
                                                     //        try
                                                     //        {

                                                     //            await Task.WhenAll(logger.GetInvocationList().
                                                     //                                   OfType<OnAuthorizeRequestSentDelegate>().
                                                     //                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                     //                                                                 Timestamp.Now,
                                                     //                                                                 parentNetworkingNode,
                                                     //                                                                 sendMessageResult.Connection,
                                                     //                                                                 Request,
                                                     //                                                                 sendMessageResult.Result
                                                     //                                                             )).
                                                     //                                   ToArray());

                                                     //        }
                                                     //        catch (Exception e)
                                                     //        {
                                                     //            DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnAuthorizeRequestSent));
                                                     //        }
                                                     //    }

                                                     //    #endregion

                                                     //}

                                                 );

                    #endregion

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
                                             null,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
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

                response = AuthorizeResponse.ExceptionOccured(
                               Request,
                               e
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
        /// An event fired whenever an Authorize response was received.
        /// </summary>
        public event OnAuthorizeResponseReceivedDelegate?  OnAuthorizeResponseReceived;

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

                    await LogEvent(
                        OnAuthorizeResponseReceived,
                        loggingDelegate => loggingDelegate.Invoke(
                            Timestamp.Now,
                            parentNetworkingNode,
                            WebSocketConnection,
                            Request,
                            response,
                            response.Runtime
                        )
                    );

                    //var logger = OnAuthorizeResponseReceived;
                    //if (logger is not null)
                    //{
                    //    try
                    //    {

                    //        await Task.WhenAll(logger.GetInvocationList().
                    //                                  OfType<OnAuthorizeResponseReceivedDelegate>().
                    //                                  Select(loggingDelegate => loggingDelegate.Invoke(
                    //                                                                 Timestamp.Now,
                    //                                                                 parentNetworkingNode,
                    //                                                                 //    WebSocketConnection,
                    //                                                                 Request,
                    //                                                                 response,
                    //                                                                 response.Runtime
                    //                                                             )).
                    //                                  ToArray());

                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnAuthorizeResponseReceived));
                    //    }
                    //}

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

                response = AuthorizeResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion

        #region Receive AuthorizeRequestError

        /// <summary>
        /// An event fired whenever a Authorize RequestError was received.
        /// </summary>
        public event OnAuthorizeRequestErrorReceivedDelegate? AuthorizeRequestErrorReceived;


        public async Task<AuthorizeResponse>

            Receive_AuthorizeRequestError(AuthorizeRequest              Request,
                                          OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                          IWebSocketConnection          WebSocketConnection,
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

            #region Send AuthorizeRequestErrorReceived event

            await LogEvent(
                      AuthorizeRequestErrorReceived,
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


            #region Send OnAuthorizeResponseReceived event

            await LogEvent(
                      OnAuthorizeResponseReceived,
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
