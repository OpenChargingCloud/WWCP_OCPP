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

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A delegate called whenever a MessageTransfer message should be forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Message">The message.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MessageForwardingDecision<MessageTransferMessage>>

        OnMessageTransferMessageFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               MessageTransferMessage   Message,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A delegate called whenever a MessageTransfer message was forwarded or filtered.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Message">The message.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnMessageTransferMessageFilteredDelegate(DateTime                                            Timestamp,
                                                 IEventSender                                        Sender,
                                                 IWebSocketConnection                                Connection,
                                                 MessageTransferMessage                              Message,
                                                 MessageForwardingDecision<MessageTransferMessage>   ForwardingDecision,
                                                 CancellationToken                                   CancellationToken);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnMessageTransferMessageReceivedDelegate?   OnMessageTransferMessageReceived;
        public event OnMessageTransferMessageFilterDelegate?     OnMessageTransferMessageFilter;
        public event OnMessageTransferMessageFilteredDelegate?   OnMessageTransferMessageFiltered;
        public event OnMessageTransferMessageSentDelegate?       OnMessageTransferMessageSent;

        #endregion

        public async Task<MessageForwardingDecision>

            Forward_MessageTransfer(OCPP_JSONSendMessage    JSONSendMessage,
                                    OCPP_BinarySendMessage  BinarySendMessage,
                                    IWebSocketConnection    WebSocketConnection,
                                    CancellationToken       CancellationToken   = default)

        {

            #region Parse the BootNotification request

            MessageTransferMessage?  message;
            String?                  errorResponse;

            if (JSONSendMessage is not null)
            {
                if (!MessageTransferMessage.TryParse(JSONSendMessage.Payload,
                                                     JSONSendMessage.MessageId,
                                                     JSONSendMessage.Destination,
                                                     JSONSendMessage.NetworkPath,
                                                     out message,
                                                     out errorResponse,
                                                     JSONSendMessage.MessageTimestamp,
                                                     JSONSendMessage.EventTrackingId,
                                                     parentNetworkingNode.OCPP.CustomMessageTransferMessageParser,
                                                     parentNetworkingNode.OCPP.CustomSignatureParser,
                                                     parentNetworkingNode.OCPP.CustomCustomDataParser))
                {
                    return MessageForwardingDecision.REJECT(errorResponse);
                }
            }

            //else if (BinarySendMessage is not null)
            //{
            //    if (!MessageTransferMessage.TryParse(BinarySendMessage.Payload,
            //                                         BinarySendMessage.MessageId,
            //                                         BinarySendMessage.Destination,
            //                                         BinarySendMessage.NetworkPath,
            //                                         out message,
            //                                         out errorResponse,
            //                                         BinarySendMessage.MessageTimestamp,
            //                                         BinarySendMessage.EventTrackingId,
            //                                         parentNetworkingNode.OCPP.CustomBootNotificationSendParser,
            //                                         parentNetworkingNode.OCPP.CustomSignatureParser,
            //                                         parentNetworkingNode.OCPP.CustomCustomDataParser))
            //    {
            //        return ForwardingDecision.REJECT(errorResponse);
            //    }
            //}

            else
                return MessageForwardingDecision.REJECT("The given message could not be parsed!");

            #endregion

            #region Send OnMessageTransferMessageReceived event

            await LogEvent(
                      OnMessageTransferMessageReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          message,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnMessageTransferMessageFilter event

            var forwardingDecision = await CallFilter(
                                               OnMessageTransferMessageFilter,
                                               filter => filter.Invoke(
                                                             Timestamp.Now,
                                                             parentNetworkingNode,
                                                             WebSocketConnection,
                                                             message,
                                                             CancellationToken
                                                         )
                                           );

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingDecision == ForwardingDecisions.FORWARD)
                forwardingDecision = MessageForwardingDecision<MessageTransferMessage>.FORWARD(message);

            if (forwardingDecision is null) // ||
               //(forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                //var response = forwardingDecision?.RejectResponse ??
                //                   new MessageTransferResponse(
                //                       message,
                //                       MessageTransferStatus.Rejected,
                //                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                //                   );

                forwardingDecision = MessageForwardingDecision<MessageTransferMessage>.REJECT(
                                         message
                                         //response,
                                         //response.ToJSON(
                                         //    parentNetworkingNode.OCPP.CustomMessageTransferResponseSerializer,
                                         //    parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                         //    parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                         //    parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         //)
                                     );

            }

            #endregion

            if (forwardingDecision.NewMessage is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewMessage.ToJSON(
                                                        false,
                                                        parentNetworkingNode.OCPP.CustomMessageTransferMessageSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnMessageTransferRequestFiltered event

            await LogEvent(
                      OnMessageTransferMessageFiltered,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          message,
                          forwardingDecision,
                          CancellationToken
                      )
                  );

            #endregion


            #region Attach OnMessageTransferMessageSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnMessageTransferMessageSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnMessageTransferMessageSent,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Timestamp.Now,
                                      parentNetworkingNode,
                                      sentMessageResult.Connection,
                                      message,
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
