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
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever a SetErrorState request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetErrorStateRequestReceivedDelegate(DateTime               Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                SetErrorStateRequest   Request,
                                                                CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetErrorState response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetErrorStateResponseReceivedDelegate(DateTime                Timestamp,
                                                                 IEventSender            Sender,
                                                                 IWebSocketConnection?   Connection,
                                                                 SetErrorStateRequest?   Request,
                                                                 SetErrorStateResponse   Response,
                                                                 TimeSpan?               Runtime,
                                                                 CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetErrorState request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetErrorStateRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                     IEventSender                   Sender,
                                                                     IWebSocketConnection           Connection,
                                                                     SetErrorStateRequest?          Request,
                                                                     OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                     TimeSpan?                      Runtime,
                                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetErrorState response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetErrorStateResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                      IEventSender                    Sender,
                                                                      IWebSocketConnection            Connection,
                                                                      SetErrorStateRequest?           Request,
                                                                      SetErrorStateResponse?          Response,
                                                                      OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                      TimeSpan?                       Runtime,
                                                                      CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a SetErrorState response is expected
    /// for a received SetErrorState request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetErrorStateResponse>

        OnSetErrorStateDelegate(DateTime               Timestamp,
                                IEventSender           Sender,
                                IWebSocketConnection   Connection,
                                SetErrorStateRequest   Request,
                                CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive SetErrorState request (JSON)

        /// <summary>
        /// An event sent whenever a SetErrorState request was received.
        /// </summary>
        public event OnSetErrorStateRequestReceivedDelegate?  OnSetErrorStateRequestReceived;

        /// <summary>
        /// An event sent whenever a SetErrorState request was received for processing.
        /// </summary>
        public event OnSetErrorStateDelegate?                 OnSetErrorState;


        public async Task<OCPP_Response>

            Receive_SetErrorState(DateTime              RequestTimestamp,
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

                if (SetErrorStateRequest.TryParse(JSONRequest,
                                                  RequestId,
                                                  Destination,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  RequestTimestamp,
                                                  parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                  EventTrackingId,
                                                  parentNetworkingNode.OCPP.CustomSetErrorStateRequestParser)) {

                    SetErrorStateResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetErrorStateRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SetErrorStateResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSetErrorStateRequestReceived event

                    await LogEvent(
                              OnSetErrorStateRequestReceived,
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
                                           OnSetErrorState,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SetErrorStateResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSetErrorStateResponseSent(
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
                                       nameof(Receive_SetErrorState)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SetErrorState)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SetErrorState response (JSON)

        /// <summary>
        /// An event fired whenever a SetErrorState response was received.
        /// </summary>
        public event OnSetErrorStateResponseReceivedDelegate? OnSetErrorStateResponseReceived;


        public async Task<SetErrorStateResponse>

            Receive_SetErrorStateResponse(SetErrorStateRequest  Request,
                                          JObject               ResponseJSON,
                                          IWebSocketConnection  WebSocketConnection,
                                          SourceRouting         Destination,
                                          NetworkPath           NetworkPath,
                                          EventTracking_Id      EventTrackingId,
                                          Request_Id            RequestId,
                                          DateTime?             ResponseTimestamp   = null,
                                          CancellationToken     CancellationToken   = default)

        {

            SetErrorStateResponse? response = null;

            try
            {

                if (SetErrorStateResponse.TryParse(Request,
                                                   ResponseJSON,
                                                   Destination,
                                                   NetworkPath,
                                                   out response,
                                                   out var errorResponse,
                                                   ResponseTimestamp,
                                                   parentNetworkingNode.OCPP.CustomSetErrorStateResponseParser,
                                                   parentNetworkingNode.OCPP.CustomSignatureParser,
                                                   parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SetErrorStateResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SetErrorStateResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SetErrorStateResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnSetErrorStateResponseReceived event

            await LogEvent(
                      OnSetErrorStateResponseReceived,
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


        #region Receive SetErrorState request error

        /// <summary>
        /// An event fired whenever a SetErrorState request error was received.
        /// </summary>
        public event OnSetErrorStateRequestErrorReceivedDelegate? SetErrorStateRequestErrorReceived;


        public async Task<SetErrorStateResponse>

            Receive_SetErrorStateRequestError(SetErrorStateRequest          Request,
                                              OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                              IWebSocketConnection          Connection,
                                              SourceRouting                 Destination,
                                              NetworkPath                   NetworkPath,
                                              EventTracking_Id              EventTrackingId,
                                              Request_Id                    RequestId,
                                              DateTime?                     ResponseTimestamp   = null,
                                              CancellationToken             CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
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

            #region Send SetErrorStateRequestErrorReceived event

            await LogEvent(
                      SetErrorStateRequestErrorReceived,
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


            var response = SetErrorStateResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetErrorStateResponseReceived event

            await LogEvent(
                      OnSetErrorStateResponseReceived,
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


        public async Task<SetErrorStateResponse>

            Receive_SetErrorStateRequestError(SetErrorStateRequest             Request,
                                             OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                             IWebSocketConnection            Connection,
                                             SourceRouting                   Destination,
                                             NetworkPath                     NetworkPath,
                                             EventTracking_Id                EventTrackingId,
                                             Request_Id                      RequestId,
                                             DateTime?                       ResponseTimestamp   = null,
                                             CancellationToken               CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
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

            #region Send SetErrorStateRequestErrorReceived event

            //await LogEvent(
            //          SetErrorStateRequestErrorReceived,
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


            var response = SetErrorStateResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetErrorStateResponseReceived event

            await LogEvent(
                      OnSetErrorStateResponseReceived,
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

        #region Receive SetErrorState response error

        /// <summary>
        /// An event fired whenever a SetErrorState response error was received.
        /// </summary>
        public event OnSetErrorStateResponseErrorReceivedDelegate? SetErrorStateResponseErrorReceived;


        public async Task

            Receive_SetErrorStateResponseError(SetErrorStateRequest?          Request,
                                               SetErrorStateResponse?         Response,
                                               OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                               IWebSocketConnection           Connection,
                                               SourceRouting                  Destination,
                                               NetworkPath                    NetworkPath,
                                               EventTracking_Id               EventTrackingId,
                                               Request_Id                     RequestId,
                                               DateTime?                      ResponseTimestamp   = null,
                                               CancellationToken              CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetErrorStateResponseSerializer,
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

            #region Send SetErrorStateResponseErrorReceived event

            await LogEvent(
                      SetErrorStateResponseErrorReceived,
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
