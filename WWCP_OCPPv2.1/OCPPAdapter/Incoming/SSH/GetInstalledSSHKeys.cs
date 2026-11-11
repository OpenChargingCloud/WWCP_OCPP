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
    /// A logging delegate called whenever a GetInstalledSSHKeys request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetInstalledSSHKeysRequestReceivedDelegate(DateTimeOffset             Timestamp,
                                                                    IEventSender               Sender,
                                                                    IMessageConnection         Connection,
                                                                    GetInstalledSSHKeysRequest   Request,
                                                                    CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetInstalledSSHKeys response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetInstalledSSHKeysResponseReceivedDelegate(DateTimeOffset              Timestamp,
                                                                     IEventSender                Sender,
                                                                     IMessageConnection?         Connection,
                                                                     GetInstalledSSHKeysRequest?   Request,
                                                                     GetInstalledSSHKeysResponse   Response,
                                                                     TimeSpan?                   Runtime,
                                                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetInstalledSSHKeys request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetInstalledSSHKeysRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                         IEventSender                   Sender,
                                                                         IMessageConnection             Connection,
                                                                         GetInstalledSSHKeysRequest?      Request,
                                                                         OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                         TimeSpan?                      Runtime,
                                                                         CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a GetInstalledSSHKeys response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetInstalledSSHKeysResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                          IEventSender                    Sender,
                                                                          IMessageConnection              Connection,
                                                                          GetInstalledSSHKeysRequest?       Request,
                                                                          GetInstalledSSHKeysResponse?      Response,
                                                                          OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                          TimeSpan?                       Runtime,
                                                                          CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a GetInstalledSSHKeys response is expected
    /// for a received GetInstalledSSHKeys request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetInstalledSSHKeysResponse>

        OnGetInstalledSSHKeysDelegate(DateTimeOffset             Timestamp,
                                    IEventSender               Sender,
                                    IMessageConnection         Connection,
                                    GetInstalledSSHKeysRequest   Request,
                                    CancellationToken          CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive GetInstalledSSHKeys request

        /// <summary>
        /// An event sent whenever a GetInstalledSSHKeys request was received.
        /// </summary>
        public event OnGetInstalledSSHKeysRequestReceivedDelegate?  OnGetInstalledSSHKeysRequestReceived;

        /// <summary>
        /// An event sent whenever a GetInstalledSSHKeys request was received for processing.
        /// </summary>
        public event OnGetInstalledSSHKeysDelegate?                 OnGetInstalledSSHKeys;


        public async Task<OCPP_Response>

            Receive_GetInstalledSSHKeys(DateTimeOffset        RequestTimestamp,
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

                if (GetInstalledSSHKeysRequest.TryParse(JSONRequest,
                                                      RequestId,
                                                      Destination,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      RequestTimestamp,
                                                      parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                      EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysRequestParser)) {

                    GetInstalledSSHKeysResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse,
                        ReceivedJSON: JSONRequest))
                    {

                        response = GetInstalledSSHKeysResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetInstalledSSHKeysRequestReceived event

                    await LogEvent(
                              OnGetInstalledSSHKeysRequestReceived,
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
                                           OnGetInstalledSSHKeys,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetInstalledSSHKeysResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetInstalledSSHKeysResponseSent(
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
                                       nameof(Receive_GetInstalledSSHKeys)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetInstalledSSHKeys)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetInstalledSSHKeys response

        /// <summary>
        /// An event fired whenever a GetInstalledSSHKeys response was received.
        /// </summary>
        public event OnGetInstalledSSHKeysResponseReceivedDelegate? OnGetInstalledSSHKeysResponseReceived;


        public async Task<GetInstalledSSHKeysResponse>

            Receive_GetInstalledSSHKeysResponse(GetInstalledSSHKeysRequest  Request,
                                              JObject                   ResponseJSON,
                                              IMessageConnection        WebSocketConnection,
                                              SourceRouting         Destination,
                                              NetworkPath               NetworkPath,
                                              EventTracking_Id          EventTrackingId,
                                              Request_Id                RequestId,
                                              DateTimeOffset?           ResponseTimestamp   = null,
                                              CancellationToken         CancellationToken   = default)

        {

            GetInstalledSSHKeysResponse? response = null;

            try
            {

                if (GetInstalledSSHKeysResponse.TryParse(Request,
                                                       ResponseJSON,
                                                   Destination,
                                                       NetworkPath,
                                                       out response,
                                                       out var errorResponse,
                                                       ResponseTimestamp,
                                                       parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseParser,
                                                       parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                       parentNetworkingNode.OCPP.CustomSignatureParser,
                                                       parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse,
                            ReceivedJSON: ResponseJSON
                        ))
                    {

                        response = GetInstalledSSHKeysResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                {

                    response = GetInstalledSSHKeysResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

                    await TellTheAnswererItCannotBeRead(
                              WebSocketConnection,
                              NetworkPath,
                              RequestId,
                              EventTrackingId,
                              Binary: false,
                              errorResponse,
                              CancellationToken
                          );

                }

            }
            catch (Exception e)
            {

                response = GetInstalledSSHKeysResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetInstalledSSHKeysResponseReceived event

            await LogEvent(
                      OnGetInstalledSSHKeysResponseReceived,
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

        #region Receive GetInstalledSSHKeys request error

        /// <summary>
        /// An event fired whenever a GetInstalledSSHKeys request error was received.
        /// </summary>
        public event OnGetInstalledSSHKeysRequestErrorReceivedDelegate? GetInstalledSSHKeysRequestErrorReceived;


        public async Task<GetInstalledSSHKeysResponse>

            Receive_GetInstalledSSHKeysRequestError(GetInstalledSSHKeysRequest      Request,
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
            //        parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
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

            #region Send GetInstalledSSHKeysRequestErrorReceived event

            await LogEvent(
                      GetInstalledSSHKeysRequestErrorReceived,
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


            var response = GetInstalledSSHKeysResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetInstalledSSHKeysResponseReceived event

            await LogEvent(
                      OnGetInstalledSSHKeysResponseReceived,
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

        #region Receive GetInstalledSSHKeys response error

        /// <summary>
        /// An event fired whenever a GetInstalledSSHKeys response error was received.
        /// </summary>
        public event OnGetInstalledSSHKeysResponseErrorReceivedDelegate? GetInstalledSSHKeysResponseErrorReceived;


        public async Task

            Receive_GetInstalledSSHKeysResponseError(GetInstalledSSHKeysRequest?      Request,
                                                   GetInstalledSSHKeysResponse?     Response,
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
            //        parentNetworkingNode.OCPP.CustomGetInstalledSSHKeysResponseSerializer,
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

            #region Send GetInstalledSSHKeysResponseErrorReceived event

            await LogEvent(
                      GetInstalledSSHKeysResponseErrorReceived,
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
