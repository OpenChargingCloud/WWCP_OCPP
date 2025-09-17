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
    /// A logging delegate called whenever an UnpublishFirmware request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUnpublishFirmwareRequestReceivedDelegate(DateTimeOffset             Timestamp,
                                                                    IEventSender               Sender,
                                                                    IWebSocketConnection       Connection,
                                                                    UnpublishFirmwareRequest   Request,
                                                                    CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an UnpublishFirmware response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUnpublishFirmwareResponseReceivedDelegate(DateTimeOffset              Timestamp,
                                                                     IEventSender                Sender,
                                                                     IWebSocketConnection?       Connection,
                                                                     UnpublishFirmwareRequest?   Request,
                                                                     UnpublishFirmwareResponse   Response,
                                                                     TimeSpan?                   Runtime,
                                                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an UnpublishFirmware request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUnpublishFirmwareRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                         IEventSender                   Sender,
                                                                         IWebSocketConnection           Connection,
                                                                         UnpublishFirmwareRequest?      Request,
                                                                         OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                         TimeSpan?                      Runtime,
                                                                         CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an UnpublishFirmware response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnUnpublishFirmwareResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                          IEventSender                    Sender,
                                                                          IWebSocketConnection            Connection,
                                                                          UnpublishFirmwareRequest?       Request,
                                                                          UnpublishFirmwareResponse?      Response,
                                                                          OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                          TimeSpan?                       Runtime,
                                                                          CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an UnpublishFirmware response is expected
    /// for a received UnpublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UnpublishFirmwareResponse>

        OnUnpublishFirmwareDelegate(DateTimeOffset             Timestamp,
                                    IEventSender               Sender,
                                    IWebSocketConnection       Connection,
                                    UnpublishFirmwareRequest   Request,
                                    CancellationToken          CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive UnpublishFirmware request

        /// <summary>
        /// An event sent whenever an UnpublishFirmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareRequestReceivedDelegate?  OnUnpublishFirmwareRequestReceived;

        /// <summary>
        /// An event sent whenever an UnpublishFirmware request was received for processing.
        /// </summary>
        public event OnUnpublishFirmwareDelegate?                 OnUnpublishFirmware;


        public async Task<OCPP_Response>

            Receive_UnpublishFirmware(DateTimeOffset        RequestTimestamp,
                                      IWebSocketConnection  WebSocketConnection,
                                      SourceRouting     Destination,
                                      NetworkPath           NetworkPath,
                                      EventTracking_Id      EventTrackingId,
                                      Request_Id            RequestId,
                                      JObject               JSONRequest,
                                      CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (UnpublishFirmwareRequest.TryParse(JSONRequest,
                                                      RequestId,
                                                  Destination,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      RequestTimestamp,
                                                      parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                      EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomUnpublishFirmwareRequestParser)) {

                    UnpublishFirmwareResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomUnpublishFirmwareRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = UnpublishFirmwareResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnUnpublishFirmwareRequestReceived event

                    await LogEvent(
                              OnUnpublishFirmwareRequestReceived,
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
                                           OnUnpublishFirmware,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= UnpublishFirmwareResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnUnpublishFirmwareResponseSent(
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
                                       nameof(Receive_UnpublishFirmware)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_UnpublishFirmware)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive UnpublishFirmware response

        /// <summary>
        /// An event fired whenever an UnpublishFirmware response was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseReceivedDelegate? OnUnpublishFirmwareResponseReceived;


        public async Task<UnpublishFirmwareResponse>

            Receive_UnpublishFirmwareResponse(UnpublishFirmwareRequest  Request,
                                              JObject                   ResponseJSON,
                                              IWebSocketConnection      WebSocketConnection,
                                              SourceRouting         Destination,
                                              NetworkPath               NetworkPath,
                                              EventTracking_Id          EventTrackingId,
                                              Request_Id                RequestId,
                                              DateTimeOffset?           ResponseTimestamp   = null,
                                              CancellationToken         CancellationToken   = default)

        {

            UnpublishFirmwareResponse? response = null;

            try
            {

                if (UnpublishFirmwareResponse.TryParse(Request,
                                                       ResponseJSON,
                                                   Destination,
                                                       NetworkPath,
                                                       out response,
                                                       out var errorResponse,
                                                       ResponseTimestamp,
                                                       parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseParser,
                                                       parentNetworkingNode.OCPP.CustomSignatureParser,
                                                       parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = UnpublishFirmwareResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = UnpublishFirmwareResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = UnpublishFirmwareResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnUnpublishFirmwareResponseReceived event

            await LogEvent(
                      OnUnpublishFirmwareResponseReceived,
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

        #region Receive UnpublishFirmware request error

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request error was received.
        /// </summary>
        public event OnUnpublishFirmwareRequestErrorReceivedDelegate? UnpublishFirmwareRequestErrorReceived;


        public async Task<UnpublishFirmwareResponse>

            Receive_UnpublishFirmwareRequestError(UnpublishFirmwareRequest      Request,
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
            //        parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
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

            #region Send UnpublishFirmwareRequestErrorReceived event

            await LogEvent(
                      UnpublishFirmwareRequestErrorReceived,
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


            var response = UnpublishFirmwareResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnUnpublishFirmwareResponseReceived event

            await LogEvent(
                      OnUnpublishFirmwareResponseReceived,
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

        #region Receive UnpublishFirmware response error

        /// <summary>
        /// An event fired whenever an UnpublishFirmware response error was received.
        /// </summary>
        public event OnUnpublishFirmwareResponseErrorReceivedDelegate? UnpublishFirmwareResponseErrorReceived;


        public async Task

            Receive_UnpublishFirmwareResponseError(UnpublishFirmwareRequest?      Request,
                                                   UnpublishFirmwareResponse?     Response,
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
            //        parentNetworkingNode.OCPP.CustomUnpublishFirmwareResponseSerializer,
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

            #region Send UnpublishFirmwareResponseErrorReceived event

            await LogEvent(
                      UnpublishFirmwareResponseErrorReceived,
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
