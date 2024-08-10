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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>>

        OnSetDefaultChargingTariffRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        SetDefaultChargingTariffRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          SetDefaultChargingTariffRequest                                                         Request,
                                                          ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSetDefaultChargingTariffRequestReceivedDelegate?    OnSetDefaultChargingTariffRequestReceived;
        public event OnSetDefaultChargingTariffRequestFilterDelegate?      OnSetDefaultChargingTariffRequestFilter;
        public event OnSetDefaultChargingTariffRequestFilteredDelegate?    OnSetDefaultChargingTariffRequestFiltered;
        public event OnSetDefaultChargingTariffRequestSentDelegate?        OnSetDefaultChargingTariffRequestSent;

        public event OnSetDefaultChargingTariffResponseReceivedDelegate?   OnSetDefaultChargingTariffResponseReceived;
        public event OnSetDefaultChargingTariffResponseSentDelegate?       OnSetDefaultChargingTariffResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SetDefaultChargingTariff(OCPP_JSONRequestMessage  JSONRequestMessage,
                                             IWebSocketConnection     Connection,
                                             CancellationToken        CancellationToken   = default)

        {

            if (!SetDefaultChargingTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                          JSONRequestMessage.RequestId,
                                                          JSONRequestMessage.DestinationId,
                                                          JSONRequestMessage.NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          JSONRequestMessage.RequestTimestamp,
                                                          JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                          JSONRequestMessage.EventTrackingId,
                                                          parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>? forwardingDecision = null;

            #region Send OnSetDefaultChargingTariffRequestReceived event

            var receivedLogging = OnSetDefaultChargingTariffRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnSetDefaultChargingTariffRequestReceivedDelegate>().
                                  Select(filterDelegate => filterDelegate.Invoke(
                                                               Timestamp.Now,
                                                               parentNetworkingNode,
                                                               Connection,
                                                               request
                                                           )).
                                  ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetDefaultChargingTariffRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnSetDefaultChargingTariffRequestFilter event

            var requestFilter = OnSetDefaultChargingTariffRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnSetDefaultChargingTariffRequestFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             Connection,
                                                                             request,
                                                                             CancellationToken
                                                                         )).
                                                ToArray()
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetDefaultChargingTariffRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SetDefaultChargingTariffResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSetDefaultChargingTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
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
                                                    );

            #region Send OnSetDefaultChargingTariffRequestFiltered event

            var logger = OnSetDefaultChargingTariffRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnSetDefaultChargingTariffRequestFilteredDelegate>().
                                  Select(loggingDelegate => loggingDelegate.Invoke(
                                                                Timestamp.Now,
                                                                parentNetworkingNode,
                                                                Connection,
                                                                request,
                                                                forwardingDecision
                                                            )).
                                  ToArray()
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnSetDefaultChargingTariffRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnSetDefaultChargingTariffRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnSetDefaultChargingTariffRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnSetDefaultChargingTariffRequestSentDelegate>().
                                          Select(filterDelegate => filterDelegate.Invoke(
                                                                       Timestamp.Now,
                                                                       parentNetworkingNode,
                                                                       sentMessageResult.Connection,
                                                                       request,
                                                                       sentMessageResult.Result
                                                                   )).
                                          ToArray()
                                  );

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(NetworkingNode),
                                      nameof(OnSetDefaultChargingTariffRequestSent),
                                      e
                                  );
                        }

                    };

            }

            #endregion

            return forwardingDecision;

        }

    }

}
