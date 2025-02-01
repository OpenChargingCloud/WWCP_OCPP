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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a ReportChargingProfiles request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnReportChargingProfilesRequestSentDelegate(DateTime                        Timestamp,
                                                                     IEventSender                    Sender,
                                                                     IWebSocketConnection?           Connection,
                                                                     ReportChargingProfilesRequest   Request,
                                                                     SentMessageResults              SentMessageResult,
                                                                     CancellationToken               CancellationToken);


    /// <summary>
    /// A ReportChargingProfiles response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP WebSocket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnReportChargingProfilesResponseSentDelegate(DateTime                         Timestamp,
                                                     IEventSender                     Sender,
                                                     IWebSocketConnection?            Connection,
                                                     ReportChargingProfilesRequest    Request,
                                                     ReportChargingProfilesResponse   Response,
                                                     TimeSpan                         Runtime,
                                                     SentMessageResults               SentMessageResult,
                                                     CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReportChargingProfiles request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error message.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnReportChargingProfilesRequestErrorSentDelegate(DateTime                         Timestamp,
                                                         IEventSender                     Sender,
                                                         IWebSocketConnection?            Connection,
                                                         ReportChargingProfilesRequest?   Request,
                                                         OCPP_JSONRequestErrorMessage     RequestErrorMessage,
                                                         TimeSpan?                        Runtime,
                                                         SentMessageResults               SentMessageResult,
                                                         CancellationToken                CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a ReportChargingProfiles response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task

        OnReportChargingProfilesResponseErrorSentDelegate(DateTime                          Timestamp,
                                                          IEventSender                      Sender,
                                                          IWebSocketConnection?             Connection,
                                                          ReportChargingProfilesRequest?    Request,
                                                          ReportChargingProfilesResponse?   Response,
                                                          OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                          TimeSpan?                         Runtime,
                                                          SentMessageResults                SentMessageResult,
                                                          CancellationToken                 CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send ReportChargingProfiles request

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request was sent.
        /// </summary>
        public event OnReportChargingProfilesRequestSentDelegate?  OnReportChargingProfilesRequestSent;


        /// <summary>
        /// Send a ReportChargingProfiles request.
        /// </summary>
        /// <param name="Request">A ReportChargingProfiles request.</param>
        public async Task<ReportChargingProfilesResponse>

            ReportChargingProfiles(ReportChargingProfilesRequest Request)

        {

            ReportChargingProfilesResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(

                            true,

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

                        ),
                        out var signingErrors
                    ))
                {

                    response = ReportChargingProfilesResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );

                }

                #endregion

                else
                {

                    #region Send request message

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(

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

                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnReportChargingProfilesRequestSent,
                                                         loggingDelegate => loggingDelegate.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             sentMessageResult.Connection,
                                                             Request,
                                                             sentMessageResult.Result,
                                                             Request.CancellationToken
                                                         )
                                                     )

                                                 );

                    #endregion

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReportChargingProfilesResponse(
                                             Request,
                                             jsonResponse,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await parentNetworkingNode.OCPP.IN.Receive_ReportChargingProfilesRequestError(
                                             Request,
                                             jsonRequestError,
                                             sendRequestState.WebSocketConnectionReceived,
                                             sendRequestState.DestinationReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    response ??= new ReportChargingProfilesResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = ReportChargingProfilesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnReportChargingProfilesResponseSent event

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles response was sent.
        /// </summary>
        public event OnReportChargingProfilesResponseSentDelegate?  OnReportChargingProfilesResponseSent;

        public Task SendOnReportChargingProfilesResponseSent(DateTime                        Timestamp,
                                                             IEventSender                    Sender,
                                                             IWebSocketConnection?           Connection,
                                                             ReportChargingProfilesRequest   Request,
                                                             ReportChargingProfilesResponse  Response,
                                                             TimeSpan                        Runtime,
                                                             SentMessageResults              SentMessageResult,
                                                             CancellationToken               CancellationToken = default)

            => LogEvent(
                   OnReportChargingProfilesResponseSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnReportChargingProfilesRequestErrorSent event

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request error was sent.
        /// </summary>
        public event OnReportChargingProfilesRequestErrorSentDelegate? OnReportChargingProfilesRequestErrorSent;


        public Task SendOnReportChargingProfilesRequestErrorSent(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 IWebSocketConnection?           Connection,
                                                                 ReportChargingProfilesRequest?  Request,
                                                                 OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                                 TimeSpan                        Runtime,
                                                                 SentMessageResults              SentMessageResult,
                                                                 CancellationToken               CancellationToken = default)

            => LogEvent(
                   OnReportChargingProfilesRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

        #region Send OnReportChargingProfilesResponseErrorSent event

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles response error was sent.
        /// </summary>
        public event OnReportChargingProfilesResponseErrorSentDelegate? OnReportChargingProfilesResponseErrorSent;


        public Task SendOnReportChargingProfilesResponseErrorSent(DateTime                          Timestamp,
                                                                  IEventSender                      Sender,
                                                                  IWebSocketConnection?             Connection,
                                                                  ReportChargingProfilesRequest?    Request,
                                                                  ReportChargingProfilesResponse?   Response,
                                                                  OCPP_JSONResponseErrorMessage     ResponseErrorMessage,
                                                                  TimeSpan                          Runtime,
                                                                  SentMessageResults                SentMessageResult,
                                                                  CancellationToken                 CancellationToken = default)

            => LogEvent(
                   OnReportChargingProfilesResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime,
                       SentMessageResult,
                       CancellationToken
                   )
               );

        #endregion

    }

}
