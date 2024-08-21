/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetDefaultChargingTariffRequestReceivedDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection              Connection,
                                                          SetDefaultChargingTariffRequest   Request,
                                                          CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultChargingTariffResponseReceivedDelegate(DateTime                           Timestamp,
                                                                            IEventSender                       Sender,
                                                                            IWebSocketConnection               Connection,
                                                                            SetDefaultChargingTariffRequest?   Request,
                                                                            SetDefaultChargingTariffResponse   Response,
                                                                            TimeSpan?                          Runtime,
                                                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultChargingTariffRequestErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                SetDefaultChargingTariffRequest?   Request,
                                                                                OCPP_JSONRequestErrorMessage       RequestErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultChargingTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultChargingTariffResponseErrorReceivedDelegate(DateTime                            Timestamp,
                                                                                 IEventSender                        Sender,
                                                                                 IWebSocketConnection                Connection,
                                                                                 SetDefaultChargingTariffRequest?    Request,
                                                                                 SetDefaultChargingTariffResponse?   Response,
                                                                                 OCPP_JSONResponseErrorMessage       ResponseErrorMessage,
                                                                                 TimeSpan?                           Runtime,
                                                                                 CancellationToken                   CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an SetDefaultChargingTariff response is expected
    /// for a received SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<SetDefaultChargingTariffResponse>

        OnSetDefaultChargingTariffDelegate(DateTime                          Timestamp,
                                           IEventSender                      Sender,
                                           IWebSocketConnection              Connection,
                                           SetDefaultChargingTariffRequest   Request,
                                           CancellationToken                 CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive SetDefaultChargingTariff request

        /// <summary>
        /// An event sent whenever an SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestReceivedDelegate?  OnSetDefaultChargingTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an SetDefaultChargingTariff request was received for processing.
        /// </summary>
        public event OnSetDefaultChargingTariffDelegate?                 OnSetDefaultChargingTariff;


        public async Task<OCPP_Response>

            Receive_SetDefaultChargingTariff(DateTime              RequestTimestamp,
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

                if (SetDefaultChargingTariffRequest.TryParse(JSONRequest,
                                                             RequestId,
                                                         Destination,
                                                             NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             RequestTimestamp,
                                                             parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                             EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffRequestParser)) {

                    SetDefaultChargingTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
                            //parentNetworkingNode.OCPP.CustomPriceSerializer,
                            //parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                            //parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                            //parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            //parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                            //parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                            //parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                            //parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                            //parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            //parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SetDefaultChargingTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSetDefaultChargingTariffRequestReceived event

                    await LogEvent(
                              OnSetDefaultChargingTariffRequestReceived,
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
                                           OnSetDefaultChargingTariff,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SetDefaultChargingTariffResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer
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
                                           parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSetDefaultChargingTariffResponseSent(
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
                                       nameof(Receive_SetDefaultChargingTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SetDefaultChargingTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SetDefaultChargingTariff response

        /// <summary>
        /// An event fired whenever an SetDefaultChargingTariff response was received.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseReceivedDelegate? OnSetDefaultChargingTariffResponseReceived;


        public async Task<SetDefaultChargingTariffResponse>

            Receive_SetDefaultChargingTariffResponse(SetDefaultChargingTariffRequest  Request,
                                                     JObject                          ResponseJSON,
                                                     IWebSocketConnection             WebSocketConnection,
                                                     SourceRouting                Destination,
                                                     NetworkPath                      NetworkPath,
                                                     EventTracking_Id                 EventTrackingId,
                                                     Request_Id                       RequestId,
                                                     DateTime?                        ResponseTimestamp   = null,
                                                     CancellationToken                CancellationToken   = default)

        {

            SetDefaultChargingTariffResponse? response = null;

            try
            {

                if (SetDefaultChargingTariffResponse.TryParse(Request,
                                                              ResponseJSON,
                                                          Destination,
                                                              NetworkPath,
                                                              out response,
                                                              out var errorResponse,
                                                              ResponseTimestamp,
                                                              parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SetDefaultChargingTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SetDefaultChargingTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SetDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnSetDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultChargingTariffResponseReceived,
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

        #region Receive SetDefaultChargingTariff RequestError

        /// <summary>
        /// An event fired whenever an SetDefaultChargingTariff request error was received.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestErrorReceivedDelegate? SetDefaultChargingTariffRequestErrorReceived;


        public async Task<SetDefaultChargingTariffResponse>

            Receive_SetDefaultChargingTariffRequestError(SetDefaultChargingTariffRequest  Request,
                                                         OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                         IWebSocketConnection             Connection,
                                                         SourceRouting                Destination,
                                                         NetworkPath                      NetworkPath,
                                                         EventTracking_Id                 EventTrackingId,
                                                         Request_Id                       RequestId,
                                                         DateTime?                        ResponseTimestamp   = null,
                                                         CancellationToken                CancellationToken   = default)
        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
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

            #region Send SetDefaultChargingTariffRequestErrorReceived event

            await LogEvent(
                      SetDefaultChargingTariffRequestErrorReceived,
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


            var response = SetDefaultChargingTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultChargingTariffResponseReceived,
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

        #region Receive SetDefaultChargingTariff ResponseError

        /// <summary>
        /// An event fired whenever an SetDefaultChargingTariff response error was received.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseErrorReceivedDelegate? SetDefaultChargingTariffResponseErrorReceived;


        public async Task

            Receive_SetDefaultChargingTariffResponseError(SetDefaultChargingTariffRequest?   Request,
                                                          SetDefaultChargingTariffResponse?  Response,
                                                          OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                          IWebSocketConnection               Connection,
                                                          SourceRouting                  Destination,
                                                          NetworkPath                        NetworkPath,
                                                          EventTracking_Id                   EventTrackingId,
                                                          Request_Id                         RequestId,
                                                          DateTime?                          ResponseTimestamp   = null,
                                                          CancellationToken                  CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
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

            #region Send SetDefaultChargingTariffResponseErrorReceived event

            await LogEvent(
                      SetDefaultChargingTariffResponseErrorReceived,
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
