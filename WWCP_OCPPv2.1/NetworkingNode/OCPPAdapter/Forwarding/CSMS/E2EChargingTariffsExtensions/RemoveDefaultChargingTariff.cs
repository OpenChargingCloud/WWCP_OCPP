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
    /// A RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>>

        OnRemoveDefaultChargingTariffRequestFilterDelegate(DateTime                             Timestamp,
                                                           IEventSender                         Sender,
                                                           IWebSocketConnection                 Connection,
                                                           RemoveDefaultChargingTariffRequest   Request,
                                                           CancellationToken                    CancellationToken);


    /// <summary>
    /// A filtered RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                      Timestamp,
                                                             IEventSender                                                                                  Sender,
                                                             IWebSocketConnection                                                                          Connection,
                                                             RemoveDefaultChargingTariffRequest                                                            Request,
                                                             ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnRemoveDefaultChargingTariffRequestFilterDelegate?      OnRemoveDefaultChargingTariffRequest;

        public event OnRemoveDefaultChargingTariffRequestFilteredDelegate?    OnRemoveDefaultChargingTariffRequestLogging;

        #endregion

        public async Task<ForwardingDecision>

            Forward_RemoveDefaultChargingTariff(OCPP_JSONRequestMessage  JSONRequestMessage,
                                                IWebSocketConnection     Connection,
                                                CancellationToken        CancellationToken   = default)

        {

            if (!RemoveDefaultChargingTariffRequest.TryParse(JSONRequestMessage.Payload,
                                                             JSONRequestMessage.RequestId,
                                                             JSONRequestMessage.DestinationId,
                                                             JSONRequestMessage.NetworkPath,
                                                             out var Request,
                                                             out var errorResponse,
                                                             JSONRequestMessage.RequestTimestamp,
                                                             JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                             JSONRequestMessage.EventTrackingId,
                                                             parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>? forwardingDecision = null;

            #region Send OnRemoveDefaultChargingTariffRequest event

            var requestFilter = OnRemoveDefaultChargingTariffRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnRemoveDefaultChargingTariffRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnRemoveDefaultChargingTariffRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                         Request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new RemoveDefaultChargingTariffResponse(
                                       Request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                         Request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomRemoveDefaultChargingTariffResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomEVSEStatusInfoSerializer2,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnRemoveDefaultChargingTariffRequestLogging event

            var logger = OnRemoveDefaultChargingTariffRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnRemoveDefaultChargingTariffRequestFilteredDelegate>().
                                       Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                         parentNetworkingNode,
                                                                                         Connection,
                                                                                         Request,
                                                                                         forwardingDecision)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnRemoveDefaultChargingTariffRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
