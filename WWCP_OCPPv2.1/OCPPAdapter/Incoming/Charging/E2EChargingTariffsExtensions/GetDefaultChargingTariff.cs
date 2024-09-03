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
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The GetDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnGetDefaultChargingTariffRequestReceivedDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection              Connection,
                                                          GetDefaultChargingTariffRequest   Request,
                                                          CancellationToken                 CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDefaultChargingTariffResponseReceivedDelegate(DateTime                           Timestamp,
                                                                            IEventSender                       Sender,
                                                                            IWebSocketConnection?              Connection,
                                                                            GetDefaultChargingTariffRequest?   Request,
                                                                            GetDefaultChargingTariffResponse   Response,
                                                                            TimeSpan?                          Runtime,
                                                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDefaultChargingTariffRequestErrorReceivedDelegate(DateTime                           Timestamp,
                                                                                IEventSender                       Sender,
                                                                                IWebSocketConnection               Connection,
                                                                                GetDefaultChargingTariffRequest?   Request,
                                                                                OCPP_JSONRequestErrorMessage       RequestErrorMessage,
                                                                                TimeSpan?                          Runtime,
                                                                                CancellationToken                  CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an GetDefaultChargingTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnGetDefaultChargingTariffResponseErrorReceivedDelegate(DateTime                            Timestamp,
                                                                                 IEventSender                        Sender,
                                                                                 IWebSocketConnection                Connection,
                                                                                 GetDefaultChargingTariffRequest?    Request,
                                                                                 GetDefaultChargingTariffResponse?   Response,
                                                                                 OCPP_JSONResponseErrorMessage       ResponseErrorMessage,
                                                                                 TimeSpan?                           Runtime,
                                                                                 CancellationToken                   CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an GetDefaultChargingTariff response is expected
    /// for a received GetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The GetDefaultChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<GetDefaultChargingTariffResponse>

        OnGetDefaultChargingTariffDelegate(DateTime                          Timestamp,
                                           IEventSender                      Sender,
                                           IWebSocketConnection              Connection,
                                           GetDefaultChargingTariffRequest   Request,
                                           CancellationToken                 CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive GetDefaultChargingTariff request

        /// <summary>
        /// An event sent whenever an GetDefaultChargingTariff request was received.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestReceivedDelegate?  OnGetDefaultChargingTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an GetDefaultChargingTariff request was received for processing.
        /// </summary>
        public event OnGetDefaultChargingTariffDelegate?                 OnGetDefaultChargingTariff;


        public async Task<OCPP_Response>

            Receive_GetDefaultChargingTariff(DateTime              RequestTimestamp,
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

                if (GetDefaultChargingTariffRequest.TryParse(JSONRequest,
                                                             RequestId,
                                                         Destination,
                                                             NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             RequestTimestamp,
                                                             parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                             EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestParser)) {

                    GetDefaultChargingTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetDefaultChargingTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetDefaultChargingTariffRequestReceived event

                    await LogEvent(
                              OnGetDefaultChargingTariffRequestReceived,
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
                                           OnGetDefaultChargingTariff,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= GetDefaultChargingTariffResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
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
                        out var errorResponse2
                    );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       SourceRouting.To(NetworkPath.Source),
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
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
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnGetDefaultChargingTariffResponseSent(
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
                                       nameof(Receive_GetDefaultChargingTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetDefaultChargingTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetDefaultChargingTariff response

        /// <summary>
        /// An event fired whenever an GetDefaultChargingTariff response was received.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseReceivedDelegate? OnGetDefaultChargingTariffResponseReceived;


        public async Task<GetDefaultChargingTariffResponse>

            Receive_GetDefaultChargingTariffResponse(GetDefaultChargingTariffRequest  Request,
                                                     JObject                          ResponseJSON,
                                                     IWebSocketConnection             WebSocketConnection,
                                                     SourceRouting                Destination,
                                                     NetworkPath                      NetworkPath,
                                                     EventTracking_Id                 EventTrackingId,
                                                     Request_Id                       RequestId,
                                                     DateTime?                        ResponseTimestamp   = null,
                                                     CancellationToken                CancellationToken   = default)

        {

            GetDefaultChargingTariffResponse? response = null;

            try
            {

                if (GetDefaultChargingTariffResponse.TryParse(Request,
                                                              ResponseJSON,
                                                          Destination,
                                                              NetworkPath,
                                                              out response,
                                                              out var errorResponse,
                                                              ResponseTimestamp,
                                                              parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
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
                            out errorResponse
                        ))
                    {

                        response = GetDefaultChargingTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = GetDefaultChargingTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = GetDefaultChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnGetDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnGetDefaultChargingTariffResponseReceived,
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

        #region Receive GetDefaultChargingTariff RequestError

        /// <summary>
        /// An event fired whenever an GetDefaultChargingTariff request error was received.
        /// </summary>
        public event OnGetDefaultChargingTariffRequestErrorReceivedDelegate? GetDefaultChargingTariffRequestErrorReceived;


        public async Task<GetDefaultChargingTariffResponse>

            Receive_GetDefaultChargingTariffRequestError(GetDefaultChargingTariffRequest  Request,
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
            //        parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
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

            #region Send GetDefaultChargingTariffRequestErrorReceived event

            await LogEvent(
                      GetDefaultChargingTariffRequestErrorReceived,
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


            var response = GetDefaultChargingTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnGetDefaultChargingTariffResponseReceived event

            await LogEvent(
                      OnGetDefaultChargingTariffResponseReceived,
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

        #region Receive GetDefaultChargingTariff ResponseError

        /// <summary>
        /// An event fired whenever an GetDefaultChargingTariff response error was received.
        /// </summary>
        public event OnGetDefaultChargingTariffResponseErrorReceivedDelegate? GetDefaultChargingTariffResponseErrorReceived;


        public async Task

            Receive_GetDefaultChargingTariffResponseError(GetDefaultChargingTariffRequest?   Request,
                                                          GetDefaultChargingTariffResponse?  Response,
                                                          OCPP_JSONResponseErrorMessage      ResponseErrorMessage,
                                                          IWebSocketConnection               Connection,
                                                          SourceRouting                      Destination,
                                                          NetworkPath                        NetworkPath,
                                                          EventTracking_Id                   EventTrackingId,
                                                          Request_Id                         RequestId,
                                                          DateTime?                          ResponseTimestamp   = null,
                                                          CancellationToken                  CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomGetDefaultChargingTariffResponseSerializer,
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

            #region Send GetDefaultChargingTariffResponseErrorReceived event

            await LogEvent(
                      GetDefaultChargingTariffResponseErrorReceived,
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
