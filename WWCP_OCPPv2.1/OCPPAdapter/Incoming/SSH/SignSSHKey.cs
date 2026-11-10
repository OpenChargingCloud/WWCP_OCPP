/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a SignSSHKey request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSignSSHKeyRequestReceivedDelegate(DateTimeOffset           Timestamp,
                                                                  IEventSender             Sender,
                                                                  IMessageConnection       Connection,
                                                                  SignSSHKeyRequest   Request,
                                                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SignSSHKey response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSignSSHKeyResponseReceivedDelegate(DateTimeOffset            Timestamp,
                                                                   IEventSender              Sender,
                                                                   IMessageConnection?       Connection,
                                                                   SignSSHKeyRequest?   Request,
                                                                   SignSSHKeyResponse   Response,
                                                                   TimeSpan?                 Runtime,
                                                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SignSSHKey request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSignSSHKeyRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                       IEventSender                   Sender,
                                                                       IMessageConnection             Connection,
                                                                       SignSSHKeyRequest?        Request,
                                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                       TimeSpan?                      Runtime,
                                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SignSSHKey response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSignSSHKeyResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                        IEventSender                    Sender,
                                                                        IMessageConnection              Connection,
                                                                        SignSSHKeyRequest?         Request,
                                                                        SignSSHKeyResponse?        Response,
                                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                        TimeSpan?                       Runtime,
                                                                        CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a SignSSHKey response is expected
    /// for a received SignSSHKey request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SignSSHKeyResponse>

        OnSignSSHKeyDelegate(DateTimeOffset           Timestamp,
                                  IEventSender             Sender,
                                  IMessageConnection       Connection,
                                  SignSSHKeyRequest   Request,
                                  CancellationToken        CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive SignSSHKey request

        /// <summary>
        /// An event sent whenever a SignSSHKey request was received.
        /// </summary>
        public event OnSignSSHKeyRequestReceivedDelegate?  OnSignSSHKeyRequestReceived;

        /// <summary>
        /// An event sent whenever a SignSSHKey request was received for processing.
        /// </summary>
        public event OnSignSSHKeyDelegate?                 OnSignSSHKey;


        public async Task<OCPP_Response>

            Receive_SignSSHKey(DateTimeOffset        RequestTimestamp,
                                    IMessageConnection    WebSocketConnection,
                                    SourceRouting         Destination,
                                    NetworkPath           NetworkPath,
                                    EventTracking_Id      EventTrackingId,
                                    Request_Id            RequestId,
                                    JObject               JSONRequest,
                                    CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (SignSSHKeyRequest.TryParse(JSONRequest,
                                                    RequestId,
                                                Destination,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    RequestTimestamp,
                                                    parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                    EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomSignSSHKeyRequestParser)) {

                    SignSSHKeyResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSignSSHKeyRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SignSSHKeyResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSignSSHKeyRequestReceived event

                    await LogEvent(
                              OnSignSSHKeyRequestReceived,
                              loggingDelegate => loggingDelegate.Invoke(
                                  Now,
                                  parentNetworkingNode,
                                  WebSocketConnection,
                                  request,
                                  CancellationToken
                              )
                          );

                    #endregion


                    response ??= await CallProcessor(
                                           OnSignSSHKey,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SignSSHKeyResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSignSSHKeyResponseSerializer,
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
                                           false,
                                           parentNetworkingNode.OCPP.CustomSignSSHKeyResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSignSSHKeyResponseSent(
                                                                            Now,
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
                                       nameof(Receive_SignSSHKey)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SignSSHKey)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SignSSHKey response

        /// <summary>
        /// An event fired whenever a SignSSHKey response was received.
        /// </summary>
        public event OnSignSSHKeyResponseReceivedDelegate? OnSignSSHKeyResponseReceived;


        public async Task<SignSSHKeyResponse>

            Receive_SignSSHKeyResponse(SignSSHKeyRequest  Request,
                                            JObject                 ResponseJSON,
                                            IMessageConnection      WebSocketConnection,
                                            SourceRouting       Destination,
                                            NetworkPath             NetworkPath,
                                            EventTracking_Id        EventTrackingId,
                                            Request_Id              RequestId,
                                            DateTimeOffset?         ResponseTimestamp   = null,
                                            CancellationToken       CancellationToken   = default)

        {

            SignSSHKeyResponse? response = null;

            try
            {

                if (SignSSHKeyResponse.TryParse(Request,
                                                     ResponseJSON,
                                                 Destination,
                                                     NetworkPath,
                                                     out response,
                                                     out var errorResponse,
                                                     ResponseTimestamp,
                                                     parentNetworkingNode.OCPP.CustomSignSSHKeyResponseParser,
                                                     parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomSignSSHKeyResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SignSSHKeyResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SignSSHKeyResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SignSSHKeyResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnSignSSHKeyResponseReceived event

            await LogEvent(
                      OnSignSSHKeyResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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

        #region Receive SignSSHKey request error

        /// <summary>
        /// An event fired whenever a SignSSHKey request error was received.
        /// </summary>
        public event OnSignSSHKeyRequestErrorReceivedDelegate? SignSSHKeyRequestErrorReceived;


        public async Task<SignSSHKeyResponse>

            Receive_SignSSHKeyRequestError(SignSSHKeyRequest        Request,
                                                OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                IMessageConnection            Connection,
                                                SourceRouting                 Destination,
                                                NetworkPath                   NetworkPath,
                                                EventTracking_Id              EventTrackingId,
                                                Request_Id                    RequestId,
                                                DateTimeOffset?               ResponseTimestamp   = null,
                                                CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSignSSHKeyResponseSerializer,
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

            #region Send SignSSHKeyRequestErrorReceived event

            await LogEvent(
                      SignSSHKeyRequestErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
                          parentNetworkingNode,
                          Connection,
                          Request,
                          RequestErrorMessage,
                          RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
                          CancellationToken
                      )
                  );

            #endregion


            var response = SignSSHKeyResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSignSSHKeyResponseReceived event

            await LogEvent(
                      OnSignSSHKeyResponseReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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

        #region Receive SignSSHKey response error

        /// <summary>
        /// An event fired whenever a SignSSHKey response error was received.
        /// </summary>
        public event OnSignSSHKeyResponseErrorReceivedDelegate? SignSSHKeyResponseErrorReceived;


        public async Task

            Receive_SignSSHKeyResponseError(SignSSHKeyRequest?        Request,
                                                 SignSSHKeyResponse?       Response,
                                                 OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                 IMessageConnection             Connection,
                                                 SourceRouting                  Destination,
                                                 NetworkPath                    NetworkPath,
                                                 EventTracking_Id               EventTrackingId,
                                                 Request_Id                     RequestId,
                                                 DateTimeOffset?                ResponseTimestamp   = null,
                                                 CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSignSSHKeyResponseSerializer,
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

            #region Send SignSSHKeyResponseErrorReceived event

            await LogEvent(
                      SignSSHKeyResponseErrorReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Now,
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
