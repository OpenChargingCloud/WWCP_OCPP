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
    /// A logging delegate called whenever an GetTariffs request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The GetTariffs request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnGetTariffsRequestReceivedDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            GetTariffsRequest      Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetTariffsResponseReceivedDelegate(DateTime                Timestamp,
                                                              IEventSender            Sender,
                                                              IWebSocketConnection?   Connection,
                                                              GetTariffsRequest?      Request,
                                                              GetTariffsResponse      Response,
                                                              TimeSpan?               Runtime,
                                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetTariffsRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                  IEventSender                   Sender,
                                                                  IWebSocketConnection           Connection,
                                                                  GetTariffsRequest?             Request,
                                                                  OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                  TimeSpan?                      Runtime,
                                                                  CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetTariffs response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetTariffsResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                   IEventSender                    Sender,
                                                                   IWebSocketConnection            Connection,
                                                                   GetTariffsRequest?              Request,
                                                                   GetTariffsResponse?             Response,
                                                                   OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                   TimeSpan?                       Runtime,
                                                                   CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an GetTariffs response is expected
    /// for a received GetTariffs request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The GetTariffs request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<GetTariffsResponse>

        OnGetTariffsDelegate(DateTime               Timestamp,
                             IEventSender           Sender,
                             IWebSocketConnection   Connection,
                             GetTariffsRequest      Request,
                             CancellationToken      CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive GetTariffs request

        /// <summary>
        /// An event sent whenever an GetTariffs request was received.
        /// </summary>
        public event OnGetTariffsRequestReceivedDelegate?  OnGetTariffsRequestReceived;

        /// <summary>
        /// An event sent whenever an GetTariffs request was received for processing.
        /// </summary>
        public event OnGetTariffsDelegate?                 OnGetTariffs;


        public async Task<OCPP_Response>

            Receive_GetTariffs(DateTime              RequestTimestamp,
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

                if (GetTariffsRequest.TryParse(JSONRequest,
                                               RequestId,
                                               Destination,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               RequestTimestamp,
                                               parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                               EventTrackingId,
                                               parentNetworkingNode.OCPP.CustomGetTariffsRequestParser)) {

                    GetTariffsResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetTariffsRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetTariffsResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetTariffsRequestReceived event

                    await LogEvent(
                              OnGetTariffsRequestReceived,
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
                                           OnGetTariffs,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetTariffsResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomGetTariffsResponseSerializer,
                            parentNetworkingNode.OCPP.CustomTariffAssignmentSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
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
                                           parentNetworkingNode.OCPP.CustomGetTariffsResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomTariffAssignmentSerializer,
                                           parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                           parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetTariffsResponseSent(
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
                                       nameof(Receive_GetTariffs)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetTariffs)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetTariffs response

        /// <summary>
        /// An event fired whenever an GetTariffs response was received.
        /// </summary>
        public event OnGetTariffsResponseReceivedDelegate? OnGetTariffsResponseReceived;


        public async Task<GetTariffsResponse>

            Receive_GetTariffsResponse(GetTariffsRequest     Request,
                                       JObject               ResponseJSON,
                                       IWebSocketConnection  WebSocketConnection,
                                       SourceRouting         Destination,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       DateTime?             ResponseTimestamp   = null,
                                       CancellationToken     CancellationToken   = default)

        {

            GetTariffsResponse? response = null;

            try
            {

                if (GetTariffsResponse.TryParse(Request,
                                                ResponseJSON,
                                                Destination,
                                                NetworkPath,
                                                out response,
                                                out var errorResponse,
                                                ResponseTimestamp,
                                                parentNetworkingNode.OCPP.CustomGetTariffsResponseParser,
                                                parentNetworkingNode.OCPP.CustomTariffAssignmentParser,
                                                parentNetworkingNode.OCPP.CustomIdTokenParser,
                                                parentNetworkingNode.OCPP.CustomAdditionalInfoParser,
                                                parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                parentNetworkingNode.OCPP.CustomSignatureParser,
                                                parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomGetTariffsResponseSerializer,
                                parentNetworkingNode.OCPP.CustomTariffAssignmentSerializer,
                                parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = GetTariffsResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetTariffsResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetTariffsResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }


            #region Send OnGetTariffsResponseReceived event

            await LogEvent(
                      OnGetTariffsResponseReceived,
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

        #region Receive GetTariffs RequestError

        /// <summary>
        /// An event fired whenever an GetTariffs request error was received.
        /// </summary>
        public event OnGetTariffsRequestErrorReceivedDelegate? GetTariffsRequestErrorReceived;


        public async Task<GetTariffsResponse>

            Receive_GetTariffsRequestError(GetTariffsRequest             Request,
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
            //        parentNetworkingNode.OCPP.CustomGetTariffsResponseSerializer,
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

            #region Send GetTariffsRequestErrorReceived event

            await LogEvent(
                      GetTariffsRequestErrorReceived,
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


            var response = GetTariffsResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetTariffsResponseReceived event

            await LogEvent(
                      OnGetTariffsResponseReceived,
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

        #region Receive GetTariffs ResponseError

        /// <summary>
        /// An event fired whenever an GetTariffs response error was received.
        /// </summary>
        public event OnGetTariffsResponseErrorReceivedDelegate? GetTariffsResponseErrorReceived;


        public async Task

            Receive_GetTariffsResponseError(GetTariffsRequest?             Request,
                                            GetTariffsResponse?            Response,
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
            //        parentNetworkingNode.OCPP.CustomGetTariffsResponseSerializer,
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

            #region Send GetTariffsResponseErrorReceived event

            await LogEvent(
                      GetTariffsResponseErrorReceived,
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
