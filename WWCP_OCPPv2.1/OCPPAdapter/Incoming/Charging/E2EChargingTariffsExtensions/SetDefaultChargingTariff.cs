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
    /// A logging delegate called whenever an SetDefaultE2EChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnSetDefaultE2EChargingTariffRequestReceivedDelegate(DateTime                             Timestamp,
                                                             IEventSender                         Sender,
                                                             IWebSocketConnection                 Connection,
                                                             SetDefaultE2EChargingTariffRequest   Request,
                                                             CancellationToken                    CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultE2EChargingTariff response was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The connection of the response.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The optional runtime of the request/response pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultE2EChargingTariffResponseReceivedDelegate(DateTime                              Timestamp,
                                                                               IEventSender                          Sender,
                                                                               IWebSocketConnection?                 Connection,
                                                                               SetDefaultE2EChargingTariffRequest?   Request,
                                                                               SetDefaultE2EChargingTariffResponse   Response,
                                                                               TimeSpan?                             Runtime,
                                                                               CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultE2EChargingTariff request error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The runtime of the request/request error pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultE2EChargingTariffRequestErrorReceivedDelegate(DateTime                              Timestamp,
                                                                                   IEventSender                          Sender,
                                                                                   IWebSocketConnection                  Connection,
                                                                                   SetDefaultE2EChargingTariffRequest?   Request,
                                                                                   OCPP_JSONRequestErrorMessage          RequestErrorMessage,
                                                                                   TimeSpan?                             Runtime,
                                                                                   CancellationToken                     CancellationToken);


    /// <summary>
    /// A logging delegate called whenever an SetDefaultE2EChargingTariff response error was received.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The request, when available.</param>
    /// <param name="Response">The response, when available.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response/response error message pair.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnSetDefaultE2EChargingTariffResponseErrorReceivedDelegate(DateTime                               Timestamp,
                                                                                    IEventSender                           Sender,
                                                                                    IWebSocketConnection                   Connection,
                                                                                    SetDefaultE2EChargingTariffRequest?    Request,
                                                                                    SetDefaultE2EChargingTariffResponse?   Response,
                                                                                    OCPP_JSONResponseErrorMessage          ResponseErrorMessage,
                                                                                    TimeSpan?                              Runtime,
                                                                                    CancellationToken                      CancellationToken);

    #endregion


    /// <summary>
    /// A delegate called whenever an SetDefaultE2EChargingTariff response is expected
    /// for a received SetDefaultE2EChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task<SetDefaultE2EChargingTariffResponse>

        OnSetDefaultE2EChargingTariffDelegate(DateTime                             Timestamp,
                                              IEventSender                         Sender,
                                              IWebSocketConnection                 Connection,
                                              SetDefaultE2EChargingTariffRequest   Request,
                                              CancellationToken                    CancellationToken);


    public partial class OCPPWebSocketAdapterIN
    {

        #region Receive SetDefaultE2EChargingTariff request

        /// <summary>
        /// An event sent whenever an SetDefaultE2EChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultE2EChargingTariffRequestReceivedDelegate?  OnSetDefaultE2EChargingTariffRequestReceived;

        /// <summary>
        /// An event sent whenever an SetDefaultE2EChargingTariff request was received for processing.
        /// </summary>
        public event OnSetDefaultE2EChargingTariffDelegate?                 OnSetDefaultE2EChargingTariff;


        public async Task<OCPP_Response>

            Receive_SetDefaultE2EChargingTariff(DateTime              RequestTimestamp,
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

                if (SetDefaultE2EChargingTariffRequest.TryParse(JSONRequest,
                                                                RequestId,
                                                                Destination,
                                                                NetworkPath,
                                                                out var request,
                                                                out var errorResponse,
                                                                RequestTimestamp,
                                                                parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                                EventTrackingId,
                                                                parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffRequestParser)) {

                    SetDefaultE2EChargingTariffResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            //parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffRequestSerializer,
                            //parentNetworkingNode.OCPP.CustomChargingTariffSerializer,
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
                            //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = SetDefaultE2EChargingTariffResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSetDefaultE2EChargingTariffRequestReceived event

                    await LogEvent(
                              OnSetDefaultE2EChargingTariffRequestReceived,
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
                                           OnSetDefaultE2EChargingTariff,
                                           filter => filter.Invoke(
                                                         Timestamp.Now,
                                                         parentNetworkingNode,
                                                         WebSocketConnection,
                                                         request,
                                                         CancellationToken
                                                     )
                                       );

                    response ??= SetDefaultE2EChargingTariffResponse.Failed(request);


                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
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
                                           parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer
                                       ),
                                       async sentMessageResult => await parentNetworkingNode.OCPP.OUT.SendOnSetDefaultE2EChargingTariffResponseSent(
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
                                       nameof(Receive_SetDefaultE2EChargingTariff)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.ExceptionOccurred(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SetDefaultE2EChargingTariff)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SetDefaultE2EChargingTariff response

        /// <summary>
        /// An event fired whenever an SetDefaultE2EChargingTariff response was received.
        /// </summary>
        public event OnSetDefaultE2EChargingTariffResponseReceivedDelegate? OnSetDefaultE2EChargingTariffResponseReceived;


        public async Task<SetDefaultE2EChargingTariffResponse>

            Receive_SetDefaultE2EChargingTariffResponse(SetDefaultE2EChargingTariffRequest  Request,
                                                     JObject                          ResponseJSON,
                                                     IWebSocketConnection             WebSocketConnection,
                                                     SourceRouting                Destination,
                                                     NetworkPath                      NetworkPath,
                                                     EventTracking_Id                 EventTrackingId,
                                                     Request_Id                       RequestId,
                                                     DateTime?                        ResponseTimestamp   = null,
                                                     CancellationToken                CancellationToken   = default)

        {

            SetDefaultE2EChargingTariffResponse? response = null;

            try
            {

                if (SetDefaultE2EChargingTariffResponse.TryParse(Request,
                                                              ResponseJSON,
                                                          Destination,
                                                              NetworkPath,
                                                              out response,
                                                              out var errorResponse,
                                                              ResponseTimestamp,
                                                              parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseParser)) {

                    #region Verify response signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                            response,
                            response.ToJSON(
                                parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
                                parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer
                            ),
                            out errorResponse
                        ))
                    {

                        response = SetDefaultE2EChargingTariffResponse.SignatureError(
                                       Request,
                                       errorResponse
                                   );

                    }

                    #endregion

                }

                else
                    response = SetDefaultE2EChargingTariffResponse.FormationViolation(
                                   Request,
                                   errorResponse
                               );

            }
            catch (Exception e)
            {

                response = SetDefaultE2EChargingTariffResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }


            #region Send OnSetDefaultE2EChargingTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultE2EChargingTariffResponseReceived,
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

        #region Receive SetDefaultE2EChargingTariff RequestError

        /// <summary>
        /// An event fired whenever an SetDefaultE2EChargingTariff request error was received.
        /// </summary>
        public event OnSetDefaultE2EChargingTariffRequestErrorReceivedDelegate? SetDefaultE2EChargingTariffRequestErrorReceived;


        public async Task<SetDefaultE2EChargingTariffResponse>

            Receive_SetDefaultE2EChargingTariffRequestError(SetDefaultE2EChargingTariffRequest  Request,
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
            //        parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
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

            #region Send SetDefaultE2EChargingTariffRequestErrorReceived event

            await LogEvent(
                      SetDefaultE2EChargingTariffRequestErrorReceived,
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


            var response = SetDefaultE2EChargingTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.Destination,
                               RequestErrorMessage.NetworkPath
                           );

            #region Send OnSetDefaultE2EChargingTariffResponseReceived event

            await LogEvent(
                      OnSetDefaultE2EChargingTariffResponseReceived,
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

        #region Receive SetDefaultE2EChargingTariff ResponseError

        /// <summary>
        /// An event fired whenever an SetDefaultE2EChargingTariff response error was received.
        /// </summary>
        public event OnSetDefaultE2EChargingTariffResponseErrorReceivedDelegate? SetDefaultE2EChargingTariffResponseErrorReceived;


        public async Task

            Receive_SetDefaultE2EChargingTariffResponseError(SetDefaultE2EChargingTariffRequest?   Request,
                                                             SetDefaultE2EChargingTariffResponse?  Response,
                                                             OCPP_JSONResponseErrorMessage         ResponseErrorMessage,
                                                             IWebSocketConnection                  Connection,
                                                             SourceRouting                         Destination,
                                                             NetworkPath                           NetworkPath,
                                                             EventTracking_Id                      EventTrackingId,
                                                             Request_Id                            RequestId,
                                                             DateTime?                             ResponseTimestamp   = null,
                                                             CancellationToken                     CancellationToken   = default)

        {

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomSetDefaultE2EChargingTariffResponseSerializer,
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

            #region Send SetDefaultE2EChargingTariffResponseErrorReceived event

            await LogEvent(
                      SetDefaultE2EChargingTariffResponseErrorReceived,
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
