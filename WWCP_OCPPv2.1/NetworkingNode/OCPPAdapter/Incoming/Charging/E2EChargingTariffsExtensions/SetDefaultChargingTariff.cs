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

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OnSetDefaultChargingTariffRequestReceivedDelegate?  OnSetDefaultChargingTariffRequestReceived;

        /// <summary>
        /// An event sent whenever a SetDefaultChargingTariff request was received for processing.
        /// </summary>
        public event OnSetDefaultChargingTariffDelegate?                 OnSetDefaultChargingTariff;

        #endregion

        #region Receive SetDefaultChargingTariffRequest (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_SetDefaultChargingTariff(DateTime              RequestTimestamp,
                                             IWebSocketConnection  WebSocketConnection,
                                             NetworkingNode_Id     DestinationId,
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
                                                             DestinationId,
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
                            parentNetworkingNode.OCPP.CustomPriceSerializer,
                            parentNetworkingNode.OCPP.CustomTariffElementSerializer,
                            parentNetworkingNode.OCPP.CustomPriceComponentSerializer,
                            parentNetworkingNode.OCPP.CustomTaxRateSerializer,
                            parentNetworkingNode.OCPP.CustomTariffRestrictionsSerializer,
                            parentNetworkingNode.OCPP.CustomEnergyMixSerializer,
                            parentNetworkingNode.OCPP.CustomEnergySourceSerializer,
                            parentNetworkingNode.OCPP.CustomEnvironmentalImpactSerializer,
                            parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                            parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
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

                    var logger = OnSetDefaultChargingTariffRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnSetDefaultChargingTariffRequestReceivedDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request
                                                                             )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnSetDefaultChargingTariffRequestReceived),
                                      e
                                  );
                        }
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnSetDefaultChargingTariff?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnSetDefaultChargingTariffDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : SetDefaultChargingTariffResponse.Failed(request, $"Undefined {nameof(OnSetDefaultChargingTariff)}!");

                        }
                        catch (Exception e)
                        {

                            response = SetDefaultChargingTariffResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnSetDefaultChargingTariff),
                                      e
                                  );

                        }
                    }

                    response ??= SetDefaultChargingTariffResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetDefaultChargingTariffResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnSetDefaultChargingTariffResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
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

                ocppResponse = OCPP_Response.FormationViolation(
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

        #region Receive SetDefaultChargingTariffRequestError

        public async Task<SetDefaultChargingTariffResponse>

            Receive_SetDefaultChargingTariffRequestError(SetDefaultChargingTariffRequest  Request,
                                                         OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                         IWebSocketConnection             WebSocketConnection)

        {

            var response = SetDefaultChargingTariffResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

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

            #region Send OnSetDefaultChargingTariffResponseReceived event

            var logger = OnSetDefaultChargingTariffResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnSetDefaultChargingTariffResponseReceivedDelegate>().
                                                Select(loggingDelegate => loggingDelegate.Invoke(
                                                                               Timestamp.Now,
                                                                               parentNetworkingNode,
                                                                               //    WebSocketConnection,
                                                                               Request,
                                                                               response,
                                                                               response.Runtime
                                                                           )).
                                                ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetDefaultChargingTariffResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a SetDefaultChargingTariff was sent.
        /// </summary>
        public event OnSetDefaultChargingTariffResponseSentDelegate?  OnSetDefaultChargingTariffResponseSent;

        #endregion

        #region Send OnSetDefaultChargingTariffResponse event

        public async Task SendOnSetDefaultChargingTariffResponseSent(DateTime                          Timestamp,
                                                                     IEventSender                      Sender,
                                                                     IWebSocketConnection              Connection,
                                                                     SetDefaultChargingTariffRequest   Request,
                                                                     SetDefaultChargingTariffResponse  Response,
                                                                     TimeSpan                          Runtime)
        {

            var logger = OnSetDefaultChargingTariffResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnSetDefaultChargingTariffResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnSetDefaultChargingTariffResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
