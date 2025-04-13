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
    /// A delegate called whenever a NotifyEVChargingNeeds request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SentMessageResult">The result of the send message process.</param>
    /// <param name="CancellationToken">An optional cancellation token.</param>
    public delegate Task OnNotifyEVChargingNeedsRequestSentDelegate(DateTime                       Timestamp,
                                                                    IEventSender                   Sender,
                                                                    IWebSocketConnection?          Connection,
                                                                    NotifyEVChargingNeedsRequest   Request,
                                                                    SentMessageResults             SentMessageResult,
                                                                    CancellationToken              CancellationToken);


    /// <summary>
    /// A NotifyEVChargingNeeds response.
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

        OnNotifyEVChargingNeedsResponseSentDelegate(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    IWebSocketConnection?           Connection,
                                                    NotifyEVChargingNeedsRequest    Request,
                                                    NotifyEVChargingNeedsResponse   Response,
                                                    TimeSpan                        Runtime,
                                                    SentMessageResults              SentMessageResult,
                                                    CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyEVChargingNeeds request error was sent.
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

        OnNotifyEVChargingNeedsRequestErrorSentDelegate(DateTime                        Timestamp,
                                                        IEventSender                    Sender,
                                                        IWebSocketConnection?           Connection,
                                                        NotifyEVChargingNeedsRequest?   Request,
                                                        OCPP_JSONRequestErrorMessage    RequestErrorMessage,
                                                        TimeSpan?                       Runtime,
                                                        SentMessageResults              SentMessageResult,
                                                        CancellationToken               CancellationToken);


    /// <summary>
    /// A logging delegate called whenever a NotifyEVChargingNeeds response error was sent.
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

        OnNotifyEVChargingNeedsResponseErrorSentDelegate(DateTime                         Timestamp,
                                                         IEventSender                     Sender,
                                                         IWebSocketConnection?            Connection,
                                                         NotifyEVChargingNeedsRequest?    Request,
                                                         NotifyEVChargingNeedsResponse?   Response,
                                                         OCPP_JSONResponseErrorMessage    ResponseErrorMessage,
                                                         TimeSpan?                        Runtime,
                                                         SentMessageResults               SentMessageResult,
                                                         CancellationToken                CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterOUT
    {

        #region Send NotifyEVChargingNeeds request

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestSentDelegate?  OnNotifyEVChargingNeedsRequestSent;


        /// <summary>
        /// Send a NotifyEVChargingNeeds request.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingNeeds request.</param>
        public async Task<NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest Request)

        {

            NotifyEVChargingNeedsResponse? response = null;

            try
            {

                #region Sign request message

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            true,
                            parentNetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingNeedsSerializer,
                            parentNetworkingNode.OCPP.CustomACChargingParametersSerializer,
                            parentNetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                            parentNetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                            parentNetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                            parentNetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                            parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                            parentNetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {

                    response = NotifyEVChargingNeedsResponse.SignatureError(
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
                                                             parentNetworkingNode.OCPP.CustomNotifyEVChargingNeedsRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingNeedsSerializer,
                                                             parentNetworkingNode.OCPP.CustomACChargingParametersSerializer,
                                                             parentNetworkingNode.OCPP.CustomDCChargingParametersSerializer,
                                                             parentNetworkingNode.OCPP.CustomV2XChargingParametersSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVEnergyOfferSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVPowerScheduleSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVPowerScheduleEntrySerializer,
                                                             parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleSerializer,
                                                             parentNetworkingNode.OCPP.CustomEVAbsolutePriceScheduleEntrySerializer,
                                                             parentNetworkingNode.OCPP.CustomEVPriceRuleSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     sentMessageResult => LogEvent(
                                                         OnNotifyEVChargingNeedsRequestSent,
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyEVChargingNeedsResponse(
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
                        response = await parentNetworkingNode.OCPP.IN.Receive_NotifyEVChargingNeedsRequestError(
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

                    response ??= new NotifyEVChargingNeedsResponse(
                                     Request,
                                     NotifyEVChargingNeedsStatus.Rejected,
                                     Result: Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = NotifyEVChargingNeedsResponse.ExceptionOccurred(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion


        #region Send OnNotifyEVChargingNeedsResponseSent event

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds response was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseSentDelegate?  OnNotifyEVChargingNeedsResponseSent;

        public Task SendOnNotifyEVChargingNeedsResponseSent(DateTime                       Timestamp,
                                                            IEventSender                   Sender,
                                                            IWebSocketConnection?          Connection,
                                                            NotifyEVChargingNeedsRequest   Request,
                                                            NotifyEVChargingNeedsResponse  Response,
                                                            TimeSpan                       Runtime,
                                                            SentMessageResults             SentMessageResult,
                                                            CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnNotifyEVChargingNeedsResponseSent,
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

        #region Send OnNotifyEVChargingNeedsRequestErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds request error was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestErrorSentDelegate? OnNotifyEVChargingNeedsRequestErrorSent;


        public Task SendOnNotifyEVChargingNeedsRequestErrorSent(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection?          Connection,
                                                                NotifyEVChargingNeedsRequest?  Request,
                                                                OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                                                TimeSpan                       Runtime,
                                                                SentMessageResults             SentMessageResult,
                                                                CancellationToken              CancellationToken = default)

            => LogEvent(
                   OnNotifyEVChargingNeedsRequestErrorSent,
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

        #region Send OnNotifyEVChargingNeedsResponseErrorSent event

        /// <summary>
        /// An event sent whenever a NotifyEVChargingNeeds response error was sent.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseErrorSentDelegate? OnNotifyEVChargingNeedsResponseErrorSent;


        public Task SendOnNotifyEVChargingNeedsResponseErrorSent(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 IWebSocketConnection?           Connection,
                                                                 NotifyEVChargingNeedsRequest?   Request,
                                                                 NotifyEVChargingNeedsResponse?  Response,
                                                                 OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                                                 TimeSpan                        Runtime,
                                                                 SentMessageResults              SentMessageResult,
                                                                 CancellationToken               CancellationToken = default)

            => LogEvent(
                   OnNotifyEVChargingNeedsResponseErrorSent,
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
