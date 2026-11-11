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
    /// A logging delegate called whenever a InstallSSHKey request was received.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnInstallSSHKeyRequestReceivedDelegate(DateTimeOffset             Timestamp,
                                                                    IEventSender               Sender,
                                                                    IMessageConnection         Connection,
                                                                    InstallSSHKeyRequest   Request,
                                                                    CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a InstallSSHKey response was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response logging.</param>
    /// <param name="Sender">The sender of the request/response.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnInstallSSHKeyResponseReceivedDelegate(DateTimeOffset              Timestamp,
                                                                     IEventSender                Sender,
                                                                     IMessageConnection?         Connection,
                                                                     InstallSSHKeyRequest?   Request,
                                                                     InstallSSHKeyResponse   Response,
                                                                     TimeSpan?                   Runtime,
                                                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a InstallSSHKey request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnInstallSSHKeyRequestErrorReceivedDelegate(DateTimeOffset                 Timestamp,
                                                                         IEventSender                   Sender,
                                                                         IMessageConnection             Connection,
                                                                         InstallSSHKeyRequest?      Request,
                                                                         OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                         TimeSpan?                      Runtime,
                                                                         CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a InstallSSHKey response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnInstallSSHKeyResponseErrorReceivedDelegate(DateTimeOffset                  Timestamp,
                                                                          IEventSender                    Sender,
                                                                          IMessageConnection              Connection,
                                                                          InstallSSHKeyRequest?       Request,
                                                                          InstallSSHKeyResponse?      Response,
                                                                          OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                          TimeSpan?                       Runtime,
                                                                          CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever a InstallSSHKey response is expected
    /// for a received InstallSSHKey request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<InstallSSHKeyResponse>

        OnInstallSSHKeyDelegate(DateTimeOffset             Timestamp,
                                    IEventSender               Sender,
                                    IMessageConnection         Connection,
                                    InstallSSHKeyRequest   Request,
                                    CancellationToken          CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        // Wired via reflection!

        #region Receive InstallSSHKey request

        /// <summary>
        /// An event sent whenever a InstallSSHKey request was received.
        /// </summary>
        public event OnInstallSSHKeyRequestReceivedDelegate?  OnInstallSSHKeyRequestReceived;

        /// <summary>
        /// An event sent whenever a InstallSSHKey request was received for processing.
        /// </summary>
        public event OnInstallSSHKeyDelegate?                 OnInstallSSHKey;


        public async Task<OCPP_Response>

            Receive_InstallSSHKey(DateTimeOffset        RequestTimestamp,
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

                if (InstallSSHKeyRequest.TryParse(JSONRequest,
                                                      RequestId,
                                                      Destination,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      RequestTimestamp,
                                                      parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                      EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomInstallSSHKeyRequestParser)) {

                    InstallSSHKeyResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomInstallSSHKeyRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse,
                        ReceivedJSON: JSONRequest))
                    {

                        response = InstallSSHKeyResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnInstallSSHKeyRequestReceived event

                    await LogEvent(
                              OnInstallSSHKeyRequestReceived,
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
                                           OnInstallSSHKey,
                                           filter => filter.Invoke(
                                                         Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= InstallSSHKeyResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnInstallSSHKeyResponseSent(
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
                                       nameof(Receive_InstallSSHKey)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_InstallSSHKey)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive InstallSSHKey response

        /// <summary>
        /// An event fired whenever a InstallSSHKey response was received.
        /// </summary>
        public event OnInstallSSHKeyResponseReceivedDelegate? OnInstallSSHKeyResponseReceived;


        public async Task<InstallSSHKeyResponse>

            Receive_InstallSSHKeyResponse(InstallSSHKeyRequest  Request,
                                              JObject                   ResponseJSON,
                                              IMessageConnection        WebSocketConnection,
                                              SourceRouting         Destination,
                                              NetworkPath               NetworkPath,
                                              EventTracking_Id          EventTrackingId,
                                              Request_Id                RequestId,
                                              DateTimeOffset?           ResponseTimestamp   = null,
                                              CancellationToken         CancellationToken   = default)

        {

            InstallSSHKeyResponse? response = null;

            try
            {

                if (InstallSSHKeyResponse.TryParse(Request,
                                                       ResponseJSON,
                                                   Destination,
                                                       NetworkPath,
                                                       out response,
                                                       out var errorResponse,
                                                       ResponseTimestamp,
                                                       parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseParser,
                                                       parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                       parentNetworkingNode.OCPP.CustomSignatureParser,
                                                       parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse,
                            ReceivedJSON: ResponseJSON
                        ))
                    {

                        response = InstallSSHKeyResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                {

                    response = InstallSSHKeyResponse.FormationViolation(
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

                response = InstallSSHKeyResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnInstallSSHKeyResponseReceived event

            await LogEvent(
                      OnInstallSSHKeyResponseReceived,
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

        #region Receive InstallSSHKey request error

        /// <summary>
        /// An event fired whenever a InstallSSHKey request error was received.
        /// </summary>
        public event OnInstallSSHKeyRequestErrorReceivedDelegate? InstallSSHKeyRequestErrorReceived;


        public async Task<InstallSSHKeyResponse>

            Receive_InstallSSHKeyRequestError(InstallSSHKeyRequest      Request,
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
            //        parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseSerializer,
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

            #region Send InstallSSHKeyRequestErrorReceived event

            await LogEvent(
                      InstallSSHKeyRequestErrorReceived,
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


            var response = InstallSSHKeyResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnInstallSSHKeyResponseReceived event

            await LogEvent(
                      OnInstallSSHKeyResponseReceived,
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

        #region Receive InstallSSHKey response error

        /// <summary>
        /// An event fired whenever a InstallSSHKey response error was received.
        /// </summary>
        public event OnInstallSSHKeyResponseErrorReceivedDelegate? InstallSSHKeyResponseErrorReceived;


        public async Task

            Receive_InstallSSHKeyResponseError(InstallSSHKeyRequest?      Request,
                                                   InstallSSHKeyResponse?     Response,
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
            //        parentNetworkingNode.OCPP.CustomInstallSSHKeyResponseSerializer,
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

            #region Send InstallSSHKeyResponseErrorReceived event

            await LogEvent(
                      InstallSSHKeyResponseErrorReceived,
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
