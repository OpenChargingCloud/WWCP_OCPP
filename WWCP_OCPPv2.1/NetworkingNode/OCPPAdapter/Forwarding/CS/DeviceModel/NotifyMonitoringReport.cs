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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A NotifyMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>>

        OnNotifyMonitoringReportRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      NotifyMonitoringReportRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered NotifyMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyMonitoringReportRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        NotifyMonitoringReportRequest                                                       Request,
                                                        ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnNotifyMonitoringReportRequestReceivedDelegate?    OnNotifyMonitoringReportRequestReceived;
        public event OnNotifyMonitoringReportRequestFilterDelegate?      OnNotifyMonitoringReportRequestFilter;
        public event OnNotifyMonitoringReportRequestFilteredDelegate?    OnNotifyMonitoringReportRequestFiltered;
        public event OnNotifyMonitoringReportRequestSentDelegate?        OnNotifyMonitoringReportRequestSent;

        public event OnNotifyMonitoringReportResponseReceivedDelegate?   OnNotifyMonitoringReportResponseReceived;
        public event OnNotifyMonitoringReportResponseSentDelegate?       OnNotifyMonitoringReportResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_NotifyMonitoringReport(OCPP_JSONRequestMessage  JSONRequestMessage,
                                           IWebSocketConnection     Connection,
                                           CancellationToken        CancellationToken   = default)

        {

            if (!NotifyMonitoringReportRequest.TryParse(JSONRequestMessage.Payload,
                                                        JSONRequestMessage.RequestId,
                                                        JSONRequestMessage.DestinationId,
                                                        JSONRequestMessage.NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        JSONRequestMessage.RequestTimestamp,
                                                        JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                        JSONRequestMessage.EventTrackingId,
                                                        parentNetworkingNode.OCPP.CustomNotifyMonitoringReportRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>? forwardingDecision = null;


            #region Send OnNotifyMonitoringReportRequestReceived event

            var receivedLogging = OnNotifyMonitoringReportRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnNotifyMonitoringReportRequestReceivedDelegate>().
                                          Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                         parentNetworkingNode,
                                                                                         Connection,
                                                                                         request)).
                                          ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                "NetworkingNode",
                                nameof(OnNotifyMonitoringReportRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnNotifyMonitoringReportRequestFilter event

            var requestFilter = OnNotifyMonitoringReportRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType<OnNotifyMonitoringReportRequestFilterDelegate>().
                                                     Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                    parentNetworkingNode,
                                                                                                    Connection,
                                                                                                    request,
                                                                                                    CancellationToken)).
                                                     ToArray());

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnNotifyMonitoringReportRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new NotifyMonitoringReportResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomNotifyMonitoringReportResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnNotifyMonitoringReportRequestFiltered event

            var logger = OnNotifyMonitoringReportRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType<OnNotifyMonitoringReportRequestFilteredDelegate>().
                                       Select(loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                        parentNetworkingNode,
                                                                                        Connection,
                                                                                        request,
                                                                                        forwardingDecision)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              "NetworkingNode",
                              nameof(OnNotifyMonitoringReportRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnNotifyMonitoringReportRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnNotifyMonitoringReportRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnNotifyMonitoringReportRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                             parentNetworkingNode,
                                                                                             request,
                                                                                             SendMessageResult.Success)).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnNotifyMonitoringReportRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomNotifyMonitoringReportRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomMonitoringDataSerializer,
                                                        parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                        parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                        parentNetworkingNode.OCPP.CustomVariableMonitoringSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            return forwardingDecision;

        }

    }

}
