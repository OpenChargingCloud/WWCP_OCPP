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
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

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
    /// A logging delegate called whenever a GetSignerRoles request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetSignerRolesRequestReceivedDelegate(DateTimeOffset           Timestamp,
                                                                  IEventSender             Sender,
                                                                  IMessageConnection       Connection,
                                                                  GetSignerRolesRequest   Request,
                                                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetSignerRoles response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetSignerRolesResponseReceivedDelegate(DateTimeOffset            Timestamp,
                                                                   IEventSender              Sender,
                                                                   IMessageConnection?       Connection,
                                                                   GetSignerRolesRequest?   Request,
                                                                   GetSignerRolesResponse   Response,
                                                                   TimeSpan?                 Runtime,
                                                                   CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetSignerRoles request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetSignerRolesRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                       IEventSender                   Sender,
                                                                       IMessageConnection             Connection,
                                                                       GetSignerRolesRequest?        Request,
                                                                       OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                       TimeSpan?                      Runtime,
                                                                       CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetSignerRoles response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetSignerRolesResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                        IEventSender                    Sender,
                                                                        IMessageConnection              Connection,
                                                                        GetSignerRolesRequest?         Request,
                                                                        GetSignerRolesResponse?        Response,
                                                                        OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                        TimeSpan?                       Runtime,
                                                                        CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetSignerRoles response is expected
    /// for a received GetSignerRoles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetSignerRolesResponse>

        OnGetSignerRolesDelegate(DateTimeOffset           Timestamp,
                                  IEventSender             Sender,
                                  IMessageConnection       Connection,
                                  GetSignerRolesRequest   Request,
                                  CancellationToken        CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetSignerRoles request (JSON)

        /// <summary>
        /// An event sent whenever a GetSignerRoles request was received.
        /// </summary>
        public event OnGetSignerRolesRequestReceivedDelegate?  OnGetSignerRolesRequestReceived;

        /// <summary>
        /// An event sent whenever a GetSignerRoles request was received for processing.
        /// </summary>
        public event OnGetSignerRolesDelegate?                 OnGetSignerRoles;


        public async Task<OCPP_Response>

            Receive_GetSignerRoles(DateTimeOffset        RequestTimestamp,
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

                if (GetSignerRolesRequest.TryParse(JSONRequest,
                                                    RequestId,
                                                    Destination,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    RequestTimestamp,
                                                    parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                    EventTrackingId,
                                                    parentNetworkingNode.OCPP.CustomGetSignerRolesRequestParser)) {

                    GetSignerRolesResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetSignerRolesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse,
                        ReceivedJSON: JSONRequest,
                        Connection: ConnectionRightsOf(WebSocketConnection)))
                    {

                        response = GetSignerRolesResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetSignerRolesRequestReceived event

                    await LogEvent(
                              OnGetSignerRolesRequestReceived,
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
                                           OnGetSignerRoles,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetSignerRolesResponse.Failed(request);

                    // A request that is refused is answered with a CALLERROR SecurityError.
                    if (SecurityErrorFor(response.Result, EventTrackingId, NetworkPath, RequestId) is OCPP_Response securityError)
                        return securityError;


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetSignerRolesResponseSent(
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
                                       nameof(Receive_GetSignerRoles)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetSignerRoles)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetSignerRoles response (JSON)

        /// <summary>
        /// An event fired whenever a GetSignerRoles response was received.
        /// </summary>
        public event OnGetSignerRolesResponseReceivedDelegate? OnGetSignerRolesResponseReceived;


        public async Task<GetSignerRolesResponse>

            Receive_GetSignerRolesResponse(GetSignerRolesRequest  Request,
                                            JObject                 ResponseJSON,
                                            IMessageConnection      WebSocketConnection,
                                            SourceRouting           Destination,
                                            NetworkPath             NetworkPath,
                                            EventTracking_Id        EventTrackingId,
                                            Request_Id              RequestId,
                                            DateTimeOffset?         ResponseTimestamp   = null,
                                            CancellationToken       CancellationToken   = default)

        {

            GetSignerRolesResponse? response = null;

            try
            {

                if (GetSignerRolesResponse.TryParse(Request,
                                                     ResponseJSON,
                                                     Destination,
                                                     NetworkPath,
                                                     out response,
                                                     out var errorResponse,
                                                     ResponseTimestamp,
                                                     parentNetworkingNode.OCPP.CustomGetSignerRolesResponseParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse,
                            ReceivedJSON: ResponseJSON
                        ))
                    {

                        response = GetSignerRolesResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetSignerRolesResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetSignerRolesResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetSignerRolesResponseReceived event

            await LogEvent(
                      OnGetSignerRolesResponseReceived,
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


        #region Receive GetSignerRoles request error

        /// <summary>
        /// An event fired whenever a GetSignerRoles request error was received.
        /// </summary>
        public event OnGetSignerRolesRequestErrorReceivedDelegate? GetSignerRolesRequestErrorReceived;


        public async Task<GetSignerRolesResponse>

            Receive_GetSignerRolesRequestError(GetSignerRolesRequest        Request,
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
            //        parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
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

            #region Send GetSignerRolesRequestErrorReceived event

            await LogEvent(
                      GetSignerRolesRequestErrorReceived,
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


            var response = GetSignerRolesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetSignerRolesResponseReceived event

            await LogEvent(
                      OnGetSignerRolesResponseReceived,
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


        public async Task<GetSignerRolesResponse>

            Receive_GetSignerRolesRequestError(GetSignerRolesRequest          Request,
                                                OCPP_BinaryRequestErrorMessage  RequestErrorMessage,
                                                IMessageConnection              Connection,
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
            //        parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
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

            #region Send GetSignerRolesRequestErrorReceived event

            //await LogEvent(
            //          GetSignerRolesRequestErrorReceived,
            //          loggingDelegate => loggingDelegate.Invoke(
            //              Now,
            //              parentNetworkingNode,
            //              Connection,
            //              Request,
            //              RequestErrorMessage,
            //              RequestErrorMessage.ResponseTimestamp - Request.RequestTimestamp,
            //              CancellationToken
            //          )
            //      );

            #endregion


            var response = GetSignerRolesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetSignerRolesResponseReceived event

            await LogEvent(
                      OnGetSignerRolesResponseReceived,
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

        #region Receive GetSignerRoles response error

        /// <summary>
        /// An event fired whenever a GetSignerRoles response error was received.
        /// </summary>
        public event OnGetSignerRolesResponseErrorReceivedDelegate? GetSignerRolesResponseErrorReceived;


        public async Task

            Receive_GetSignerRolesResponseError(GetSignerRolesRequest?        Request,
                                                 GetSignerRolesResponse?       Response,
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
            //        parentNetworkingNode.OCPP.CustomGetSignerRolesResponseSerializer,
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

            #region Send GetSignerRolesResponseErrorReceived event

            await LogEvent(
                      GetSignerRolesResponseErrorReceived,
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
