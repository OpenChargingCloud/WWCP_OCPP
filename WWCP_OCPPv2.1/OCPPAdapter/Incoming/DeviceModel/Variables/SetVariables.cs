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
    /// A logging delegate called whenever a SetVariables request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetVariablesRequestReceivedDelegate(DateTimeOffset         Timestamp,
                                                               IEventSender           Sender,
                                                               IWebSocketConnection   Connection,
                                                               SetVariablesRequest    Request,
                                                               CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetVariables response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetVariablesResponseReceivedDelegate(DateTimeOffset          Timestamp,
                                                                IEventSender            Sender,
                                                                IWebSocketConnection?   Connection,
                                                                SetVariablesRequest?    Request,
                                                                SetVariablesResponse    Response,
                                                                TimeSpan?               Runtime,
                                                                CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetVariables request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetVariablesRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                    IEventSender                   Sender,
                                                                    IWebSocketConnection           Connection,
                                                                    SetVariablesRequest?           Request,
                                                                    OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                    TimeSpan?                      Runtime,
                                                                    CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a SetVariables response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetVariablesResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                     IEventSender                    Sender,
                                                                     IWebSocketConnection            Connection,
                                                                     SetVariablesRequest?            Request,
                                                                     SetVariablesResponse?           Response,
                                                                     OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                     TimeSpan?                       Runtime,
                                                                     CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a SetVariables response is expected
    /// for a received SetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetVariablesResponse>

        OnSetVariablesDelegate(DateTimeOffset         Timestamp,
                               IEventSender           Sender,
                               IWebSocketConnection   Connection,
                               SetVariablesRequest    Request,
                               CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive SetVariables request

        /// <summary>
        /// An event sent whenever a SetVariables request was received.
        /// </summary>
        public event OnSetVariablesRequestReceivedDelegate?  OnSetVariablesRequestReceived;

        /// <summary>
        /// An event sent whenever a SetVariables request was received for processing.
        /// </summary>
        public event OnSetVariablesDelegate?                 OnSetVariables;


        public async Task<OCPP_Response>

            Receive_SetVariables(DateTimeOffset        RequestTimestamp,
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

                if (SetVariablesRequest.TryParse(JSONRequest,
                                                 RequestId,
                                                 Destination,
                                                 NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 RequestTimestamp,
                                                 parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                 EventTrackingId,
                                                 parentNetworkingNode.OCPP.CustomSetVariablesRequestParser)) {

                    SetVariablesResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSetVariablesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSetVariableDataSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SetVariablesResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSetVariablesRequestReceived event

                    await LogEvent(
                              OnSetVariablesRequestReceived,
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
                                           OnSetVariables,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SetVariablesResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSetVariablesResponseSerializer,
                            parentNetworkingNode.OCPP.CustomSetVariableResultSerializer,
                            parentNetworkingNode.OCPP.CustomComponentSerializer,
                            parentNetworkingNode.OCPP.CustomEVSESerializer,
                            parentNetworkingNode.OCPP.CustomVariableSerializer,
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
                                           parentNetworkingNode.OCPP.CustomSetVariablesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSetVariableResultSerializer,
                                           parentNetworkingNode.OCPP.CustomComponentSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSESerializer,
                                           parentNetworkingNode.OCPP.CustomVariableSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSetVariablesResponseSent(
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
                                       nameof(Receive_SetVariables)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SetVariables)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SetVariables response

        /// <summary>
        /// An event fired whenever a SetVariables response was received.
        /// </summary>
        public event OnSetVariablesResponseReceivedDelegate? OnSetVariablesResponseReceived;


        public async Task<SetVariablesResponse>

            Receive_SetVariablesResponse(SetVariablesRequest   Request,
                                         JObject               ResponseJSON,
                                         IWebSocketConnection  WebSocketConnection,
                                         SourceRouting     Destination,
                                         NetworkPath           NetworkPath,
                                         EventTracking_Id      EventTrackingId,
                                         Request_Id            RequestId,
                                         DateTimeOffset?       ResponseTimestamp   = null,
                                         CancellationToken     CancellationToken   = default)

        {

            SetVariablesResponse? response = null;

            try
            {

                if (SetVariablesResponse.TryParse(Request,
                                                  ResponseJSON,
                                              Destination,
                                                  NetworkPath,
                                                  out response,
                                                  out var errorResponse,
                                                  ResponseTimestamp,
                                                  parentNetworkingNode.OCPP.CustomSetVariablesResponseParser,
                                                  parentNetworkingNode.OCPP.CustomSetVariableResultSerializer,
                                                  parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                  parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                  parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                  parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                  parentNetworkingNode.OCPP.CustomSignatureParser,
                                                  parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomSetVariablesResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSetVariableResultSerializer,
                                parentNetworkingNode.OCPP.CustomComponentSerializer,
                                parentNetworkingNode.OCPP.CustomEVSESerializer,
                                parentNetworkingNode.OCPP.CustomVariableSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SetVariablesResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SetVariablesResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SetVariablesResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnSetVariablesResponseReceived event

            await LogEvent(
                      OnSetVariablesResponseReceived,
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

        #region Receive SetVariables request error

        /// <summary>
        /// An event fired whenever a SetVariables request error was received.
        /// </summary>
        public event OnSetVariablesRequestErrorReceivedDelegate? SetVariablesRequestErrorReceived;


        public async Task<SetVariablesResponse>

            Receive_SetVariablesRequestError(SetVariablesRequest           Request,
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
            //        parentNetworkingNode.OCPP.CustomSetVariablesResponseSetVariables,
            //        parentNetworkingNode.OCPP.CustomIdTokenInfoSetVariables,
            //        parentNetworkingNode.OCPP.CustomIdTokenSetVariables,
            //        parentNetworkingNode.OCPP.CustomAdditionalInfoSetVariables,
            //        parentNetworkingNode.OCPP.CustomMessageContentSetVariables,
            //        parentNetworkingNode.OCPP.CustomTransactionLimitsSetVariables,
            //        parentNetworkingNode.OCPP.CustomSignatureSetVariables,
            //        parentNetworkingNode.OCPP.CustomCustomDataSetVariables
            //    ),
            //    out errorResponse
            //);

            #region Send SetVariablesRequestErrorReceived event

            await LogEvent(
                      SetVariablesRequestErrorReceived,
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


            var response = SetVariablesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetVariablesResponseReceived event

            await LogEvent(
                      OnSetVariablesResponseReceived,
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

        #region Receive SetVariables response error

        /// <summary>
        /// An event fired whenever a SetVariables response error was received.
        /// </summary>
        public event OnSetVariablesResponseErrorReceivedDelegate? SetVariablesResponseErrorReceived;


        public async Task

            Receive_SetVariablesResponseError(SetVariablesRequest?           Request,
                                              SetVariablesResponse?          Response,
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
            //        parentNetworkingNode.OCPP.CustomSetVariablesResponseSetVariables,
            //        parentNetworkingNode.OCPP.CustomIdTokenInfoSetVariables,
            //        parentNetworkingNode.OCPP.CustomIdTokenSetVariables,
            //        parentNetworkingNode.OCPP.CustomAdditionalInfoSetVariables,
            //        parentNetworkingNode.OCPP.CustomMessageContentSetVariables,
            //        parentNetworkingNode.OCPP.CustomTransactionLimitsSetVariables,
            //        parentNetworkingNode.OCPP.CustomSignatureSetVariables,
            //        parentNetworkingNode.OCPP.CustomCustomDataSetVariables
            //    ),
            //    out errorResponse
            //);

            #region Send SetVariablesResponseErrorReceived event

            await LogEvent(
                      SetVariablesResponseErrorReceived,
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
