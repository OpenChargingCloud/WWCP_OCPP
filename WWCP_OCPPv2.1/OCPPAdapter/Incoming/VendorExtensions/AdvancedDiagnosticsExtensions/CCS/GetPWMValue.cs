/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a GetPWMValue request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetPWMValueRequestReceivedDelegate(DateTimeOffset         Timestamp,
                                                              IEventSender           Sender,
                                                              IWebSocketConnection   Connection,
                                                              GetPWMValueRequest     Request,
                                                              CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetPWMValue response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetPWMValueResponseReceivedDelegate(DateTimeOffset          Timestamp,
                                                               IEventSender            Sender,
                                                               IWebSocketConnection?   Connection,
                                                               GetPWMValueRequest?     Request,
                                                               GetPWMValueResponse     Response,
                                                               TimeSpan?               Runtime,
                                                               CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetPWMValue request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetPWMValueRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                   IEventSender                   Sender,
                                                                   IWebSocketConnection           Connection,
                                                                   GetPWMValueRequest?            Request,
                                                                   OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                   TimeSpan?                      Runtime,
                                                                   CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetPWMValue response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetPWMValueResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                    IEventSender                    Sender,
                                                                    IWebSocketConnection            Connection,
                                                                    GetPWMValueRequest?             Request,
                                                                    GetPWMValueResponse?            Response,
                                                                    OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                    TimeSpan?                       Runtime,
                                                                    CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetPWMValue response is expected
    /// for a received GetPWMValue request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetPWMValueResponse>

        OnGetPWMValueDelegate(DateTimeOffset         Timestamp,
                              IEventSender           Sender,
                              IWebSocketConnection   Connection,
                              GetPWMValueRequest     Request,
                              CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetPWMValue request (JSON)

        /// <summary>
        /// An event sent whenever a GetPWMValue request was received.
        /// </summary>
        public event OnGetPWMValueRequestReceivedDelegate?  OnGetPWMValueRequestReceived;

        /// <summary>
        /// An event sent whenever a GetPWMValue request was received for processing.
        /// </summary>
        public event OnGetPWMValueDelegate?                 OnGetPWMValue;


        public async Task<OCPP_Response>

            Receive_GetPWMValue(DateTimeOffset        RequestTimestamp,
                                IWebSocketConnection  WebSocketConnection,
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

                if (GetPWMValueRequest.TryParse(JSONRequest,
                                                RequestId,
                                                Destination,
                                                NetworkPath,
                                                out var request,
                                                out var errorResponse,
                                                RequestTimestamp,
                                                parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                EventTrackingId,
                                                parentNetworkingNode.OCPP.CustomGetPWMValueRequestParser)) {

                    GetPWMValueResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetPWMValueRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetPWMValueResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetPWMValueRequestReceived event

                    await LogEvent(
                              OnGetPWMValueRequestReceived,
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
                                           OnGetPWMValue,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetPWMValueResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetPWMValueResponseSent(
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
                                       nameof(Receive_GetPWMValue)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetPWMValue)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetPWMValue response (JSON)

        /// <summary>
        /// An event fired whenever a GetPWMValue response was received.
        /// </summary>
        public event OnGetPWMValueResponseReceivedDelegate? OnGetPWMValueResponseReceived;


        public async Task<GetPWMValueResponse>

            Receive_GetPWMValueResponse(GetPWMValueRequest    Request,
                                        JObject               ResponseJSON,
                                        IWebSocketConnection  WebSocketConnection,
                                        SourceRouting         Destination,
                                        NetworkPath           NetworkPath,
                                        EventTracking_Id      EventTrackingId,
                                        Request_Id            RequestId,
                                        DateTimeOffset?       ResponseTimestamp   = null,
                                        CancellationToken     CancellationToken   = default)

        {

            GetPWMValueResponse? response = null;

            try
            {

                if (GetPWMValueResponse.TryParse(Request,
                                                 ResponseJSON,
                                                 Destination,
                                                 NetworkPath,
                                                 out response,
                                                 out var errorResponse,
                                                 ResponseTimestamp,
                                                 parentNetworkingNode.OCPP.CustomGetPWMValueResponseParser,
                                                 parentNetworkingNode.OCPP.CustomSignatureParser,
                                                 parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetPWMValueResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetPWMValueResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetPWMValueResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetPWMValueResponseReceived event

            await LogEvent(
                      OnGetPWMValueResponseReceived,
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


        #region Receive GetPWMValue request error

        /// <summary>
        /// An event fired whenever a GetPWMValue request error was received.
        /// </summary>
        public event OnGetPWMValueRequestErrorReceivedDelegate? GetPWMValueRequestErrorReceived;


        public async Task<GetPWMValueResponse>

            Receive_GetPWMValueRequestError(GetPWMValueRequest            Request,
                                            OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                            IWebSocketConnection          Connection,
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
            //        parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
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

            #region Send GetPWMValueRequestErrorReceived event

            await LogEvent(
                      GetPWMValueRequestErrorReceived,
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


            var response = GetPWMValueResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetPWMValueResponseReceived event

            await LogEvent(
                      OnGetPWMValueResponseReceived,
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


        public async Task<GetPWMValueResponse>

            Receive_GetPWMValueRequestError(GetPWMValueRequest              Request,
                                            OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                            IWebSocketConnection            Connection,
                                            SourceRouting                   Destination,
                                            NetworkPath                     NetworkPath,
                                            EventTracking_Id                EventTrackingId,
                                            Request_Id                      RequestId,
                                            DateTimeOffset?                 ResponseTimestamp   = null,
                                            CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
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

            #region Send GetPWMValueRequestErrorReceived event

            //await LogEvent(
            //          GetPWMValueRequestErrorReceived,
            //          loggingDelegate => loggingDelegate.Invoke(
            //              Timestamp.Now,
            //              parentNetworkingNode,
            //              Connection,
            //              Request,
            //              RequestErrorMessage,
            //              RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
            //              CancellationToken
            //          )
            //      );

            #endregion


            var response = GetPWMValueResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetPWMValueResponseReceived event

            await LogEvent(
                      OnGetPWMValueResponseReceived,
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

        #region Receive GetPWMValue response error

        /// <summary>
        /// An event fired whenever a GetPWMValue response error was received.
        /// </summary>
        public event OnGetPWMValueResponseErrorReceivedDelegate? GetPWMValueResponseErrorReceived;


        public async Task

            Receive_GetPWMValueResponseError(GetPWMValueRequest?            Request,
                                             GetPWMValueResponse?           Response,
                                             OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                             IWebSocketConnection           Connection,
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
            //        parentNetworkingNode.OCPP.CustomGetPWMValueResponseSerializer,
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

            #region Send GetPWMValueResponseErrorReceived event

            await LogEvent(
                      GetPWMValueResponseErrorReceived,
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
