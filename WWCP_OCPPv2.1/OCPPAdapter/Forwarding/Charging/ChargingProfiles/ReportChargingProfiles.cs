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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A ReportChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>>

        OnReportChargingProfilesRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      ReportChargingProfilesRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered ReportChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnReportChargingProfilesRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        ReportChargingProfilesRequest                                                       Request,
                                                        RequestForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>   ForwardingDecision,
                                                        CancellationToken                                                                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnReportChargingProfilesRequestReceivedDelegate?    OnReportChargingProfilesRequestReceived;
        public event OnReportChargingProfilesRequestFilterDelegate?      OnReportChargingProfilesRequestFilter;
        public event OnReportChargingProfilesRequestFilteredDelegate?    OnReportChargingProfilesRequestFiltered;
        public event OnReportChargingProfilesRequestSentDelegate?        OnReportChargingProfilesRequestSent;

        public event OnReportChargingProfilesResponseReceivedDelegate?   OnReportChargingProfilesResponseReceived;
        public event OnReportChargingProfilesResponseSentDelegate?       OnReportChargingProfilesResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_ReportChargingProfiles(OCPP_JSONRequestMessage    JSONRequestMessage,
                                           OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                           IWebSocketConnection       WebSocketConnection,
                                           CancellationToken          CancellationToken   = default)

        {

            #region Parse the Authorize request

            if (!ReportChargingProfilesRequest.TryParse(JSONRequestMessage.Payload,
                                                        JSONRequestMessage.RequestId,
                                                        JSONRequestMessage.Destination,
                                                        JSONRequestMessage.NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        JSONRequestMessage.RequestTimestamp,
                                                        JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                        JSONRequestMessage.EventTrackingId,
                                                        parentNetworkingNode.OCPP.CustomReportChargingProfilesRequestParser))
            {
                return RequestForwardingDecision.REJECT(errorResponse);
            }

            #endregion

            #region Send OnReportChargingProfilesRequestReceived event

            await LogEvent(
                      OnReportChargingProfilesRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnReportChargingProfilesRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnReportChargingProfilesRequestFilter,
                                               filter => filter.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             request,
                                                             CancellationToken
                                                         )
                                           );

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = new RequestForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new ReportChargingProfilesResponse(
                                       request,
                                       Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new RequestForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         response,
                                         response.ToJSON(
                                             false,
                                             parentNetworkingNode.OCPP.CustomReportChargingProfilesResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(

                                                        false,

                                                        parentNetworkingNode.OCPP.CustomReportChargingProfilesRequestSerializer,

                                                        parentNetworkingNode.OCPP.CustomChargingProfileSerializer,
                                                        parentNetworkingNode.OCPP.CustomLimitAtSoCSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                                                        parentNetworkingNode.OCPP.CustomV2XFreqWattEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomV2XSignalWattEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomSalesTariffSerializer,
                                                        parentNetworkingNode.OCPP.CustomSalesTariffEntrySerializer,
                                                        parentNetworkingNode.OCPP.CustomRelativeTimeIntervalSerializer,
                                                        parentNetworkingNode.OCPP.CustomConsumptionCostSerializer,
                                                        parentNetworkingNode.OCPP.CustomCostSerializer,

                                                        parentNetworkingNode.OCPP.CustomAbsolutePriceScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceRuleStackSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomTaxRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomOverstayRuleListSerializer,
                                                        parentNetworkingNode.OCPP.CustomOverstayRuleSerializer,
                                                        parentNetworkingNode.OCPP.CustomAdditionalServiceSerializer,

                                                        parentNetworkingNode.OCPP.CustomPriceLevelScheduleSerializer,
                                                        parentNetworkingNode.OCPP.CustomPriceLevelScheduleEntrySerializer,

                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer

                                                    );

            #region Send OnReportChargingProfilesRequestFiltered event

            await LogEvent(
                      OnReportChargingProfilesRequestFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          forwardingDecision,
                          CancellationToken
                      )
                  );

            #endregion


            #region Attach OnReportChargingProfilesRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnReportChargingProfilesRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnReportChargingProfilesRequestSent,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Timestamp.Now,
                                      parentNetworkingNode,
                                      sentMessageResult.Connection,
                                      request,
                                      sentMessageResult.Result,
                                      CancellationToken
                                  )
                              );

            }

            #endregion

            return forwardingDecision;

        }

    }

}
