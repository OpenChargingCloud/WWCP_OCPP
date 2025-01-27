﻿/*
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
    /// A logging delegate called whenever an SetDefaultTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetDefaultTariffRequestReceivedDelegate(DateTime                  Timestamp,
                                                  IEventSender              Sender,
                                                  IWebSocketConnection      Connection,
                                                  SetDefaultTariffRequest   Request,
                                                  CancellationToken         CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultTariffResponseReceivedDelegate(DateTime                   Timestamp,
                                                                    IEventSender               Sender,
                                                                    IWebSocketConnection?      Connection,
                                                                    SetDefaultTariffRequest?   Request,
                                                                    SetDefaultTariffResponse   Response,
                                                                    TimeSpan?                  Runtime,
                                                                    CancellationToken          CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultTariffRequestErrorReceivedDelegate(DateTime                       Timestamp,
                                                                        IEventSender                   Sender,
                                                                        IWebSocketConnection           Connection,
                                                                        SetDefaultTariffRequest?       Request,
                                                                        OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                        TimeSpan?                      Runtime,
                                                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultTariffResponseErrorReceivedDelegate(DateTime                        Timestamp,
                                                                         IEventSender                    Sender,
                                                                         IWebSocketConnection            Connection,
                                                                         SetDefaultTariffRequest?        Request,
                                                                         SetDefaultTariffResponse?       Response,
                                                                         OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                         TimeSpan?                       Runtime,
                                                                         CancellationToken               CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an SetDefaultTariff response is expected
    /// for a received SetDefaultTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<SetDefaultTariffResponse>

        OnSetDefaultTariffDelegate(DateTime                  Timestamp,
                                   IEventSender              Sender,
                                   IWebSocketConnection      Connection,
                                   SetDefaultTariffRequest   Request,
                                   CancellationToken         CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive SetDefaultTariff request

        /// <summary>
        /// An event sent whenever an SetDefaultTariff request was received.
        /// </summary>
        public event OnSetDefaultTariffRequestReceivedDelegate?  OnSetDefaultTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an SetDefaultTariff request was received for processing.
        /// </summary>
        public event OnSetDefaultTariffDelegate?                 OnSetDefaultTariff;


        public async Task<OCPP_Response>

            Receive_SetDefaultTariff(DateTime              RequestTimestamp,
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

                if (SetDefaultTariffRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     Destination,
                                                     NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     RequestTimestamp,
                                                     parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                     EventTrackingId,
                                                     parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestParser)) {

                    SetDefaultTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSetDefaultTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomTariffSerializer,
                            parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                            parentNetworkingNode.OCPP.CustomPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            parentNetworkingNode.OCPP.CustomTariffConditionsSerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergySerializer,
                            parentNetworkingNode.OCPP.CustomTariffEnergyPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimeSerializer,
                            parentNetworkingNode.OCPP.CustomTariffTimePriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedSerializer,
                            parentNetworkingNode.OCPP.CustomTariffFixedPriceSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SetDefaultTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSetDefaultTariffRequestReceived event

                    await LogEvent(
                              OnSetDefaultTariffRequestReceived,
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
                                           OnSetDefaultTariff,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SetDefaultTariffResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSetDefaultTariffResponseSent(
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
                                       nameof(Receive_SetDefaultTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SetDefaultTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SetDefaultTariff response

        /// <summary>
        /// An event fired whenever an SetDefaultTariff response was received.
        /// </summary>
        public event OnSetDefaultTariffResponseReceivedDelegate? OnSetDefaultTariffResponseReceived;


        public async Task<SetDefaultTariffResponse>

            Receive_SetDefaultTariffResponse(SetDefaultTariffRequest  Request,
                                             JObject                  ResponseJSON,
                                             IWebSocketConnection     WebSocketConnection,
                                             SourceRouting            Destination,
                                             NetworkPath              NetworkPath,
                                             EventTracking_Id         EventTrackingId,
                                             Request_Id               RequestId,
                                             DateTime?                ResponseTimestamp   = null,
                                             CancellationToken        CancellationToken   = default)

        {

            SetDefaultTariffResponse? response = null;

            try
            {

                if (SetDefaultTariffResponse.TryParse(Request,
                                                      ResponseJSON,
                                                      Destination,
                                                      NetworkPath,
                                                      out response,
                                                      out var errorResponse,
                                                      ResponseTimestamp,
                                                      parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                true,
                                parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                parentNetworkingNode.OCPP.CustomCustomDataSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SetDefaultTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SetDefaultTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SetDefaultTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnSetDefaultTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultTariffResponseReceived,
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

        #region Receive SetDefaultTariff RequestError

        /// <summary>
        /// An event fired whenever an SetDefaultTariff request error was received.
        /// </summary>
        public event OnSetDefaultTariffRequestErrorReceivedDelegate? SetDefaultTariffRequestErrorReceived;


        public async Task<SetDefaultTariffResponse>

            Receive_SetDefaultTariffRequestError(SetDefaultTariffRequest       Request,
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
            //        parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
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

            #region Send SetDefaultTariffRequestErrorReceived event

            await LogEvent(
                      SetDefaultTariffRequestErrorReceived,
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


            var response = SetDefaultTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetDefaultTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultTariffResponseReceived,
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

        #region Receive SetDefaultTariff ResponseError

        /// <summary>
        /// An event fired whenever an SetDefaultTariff response error was received.
        /// </summary>
        public event OnSetDefaultTariffResponseErrorReceivedDelegate? SetDefaultTariffResponseErrorReceived;


        public async Task

            Receive_SetDefaultTariffResponseError(SetDefaultTariffRequest?       Request,
                                                  SetDefaultTariffResponse?      Response,
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
            //        parentNetworkingNode.OCPP.CustomSetDefaultTariffResponseSerializer,
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

            #region Send SetDefaultTariffResponseErrorReceived event

            await LogEvent(
                      SetDefaultTariffResponseErrorReceived,
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
