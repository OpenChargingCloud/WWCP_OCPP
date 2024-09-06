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

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RequestForwardingDecision<BootNotificationRequest, BootNotificationResponse>>

        OnBootNotificationRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                BootNotificationRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP WebSocket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task

        OnBootNotificationRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  BootNotificationRequest                                                 Request,
                                                  RequestForwardingDecision<BootNotificationRequest, BootNotificationResponse>   ForwardingDecision,
                                                  CancellationToken                                                       CancellationToken);

    #endregion


    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnBootNotificationRequestReceivedDelegate?    OnBootNotificationRequestReceived;
        public event OnBootNotificationRequestFilterDelegate?      OnBootNotificationRequestFilter;
        public event OnBootNotificationRequestFilteredDelegate?    OnBootNotificationRequestFiltered;
        public event OnBootNotificationRequestSentDelegate?        OnBootNotificationRequestSent;

        public event OnBootNotificationResponseReceivedDelegate?   OnBootNotificationResponseReceived;
        public event OnBootNotificationResponseSentDelegate?       OnBootNotificationResponseSent;

        #endregion

        public async Task<RequestForwardingDecision>

            Forward_BootNotification(OCPP_JSONRequestMessage    JSONRequestMessage,
                                     OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                     IWebSocketConnection       WebSocketConnection,
                                     CancellationToken          CancellationToken   = default)

        {

            #region Parse the BootNotification request

            BootNotificationRequest? request;
            String?                  errorResponse;

            if (JSONRequestMessage is not null)
            {
                if (!BootNotificationRequest.TryParse(JSONRequestMessage.Payload,
                                                      JSONRequestMessage.RequestId,
                                                      JSONRequestMessage.Destination,
                                                      JSONRequestMessage.NetworkPath,
                                                      out request,
                                                      out errorResponse,
                                                      JSONRequestMessage.RequestTimestamp,
                                                      JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                      JSONRequestMessage.EventTrackingId,
                                                      parentNetworkingNode.OCPP.CustomBootNotificationRequestParser,
                                                      parentNetworkingNode.OCPP.CustomSignatureParser,
                                                      parentNetworkingNode.OCPP.CustomCustomDataParser))
                {
                    return RequestForwardingDecision.REJECT(errorResponse);
                }
            }

            //else if (BinaryRequestMessage is not null)
            //{
            //    if (!BootNotificationRequest.TryParse(BinaryRequestMessage.Payload,
            //                                          BinaryRequestMessage.RequestId,
            //                                          BinaryRequestMessage.Destination,
            //                                          BinaryRequestMessage.NetworkPath,
            //                                          out request,
            //                                          out errorResponse,
            //                                          BinaryRequestMessage.RequestTimestamp,
            //                                          BinaryRequestMessage.RequestTimeout - Timestamp.Now,
            //                                          BinaryRequestMessage.EventTrackingId,
            //                                          parentNetworkingNode.OCPP.CustomBootNotificationRequestParser,
            //                                          parentNetworkingNode.OCPP.CustomSignatureParser,
            //                                          parentNetworkingNode.OCPP.CustomCustomDataParser))
            //    {
            //        return RequestForwardingDecision.REJECT(errorResponse);
            //    }
            //}

            else
                return RequestForwardingDecision.REJECT("The given message could not be parsed!");

            #endregion

            #region Send OnBootNotificationRequestReceived event

            await LogEvent(
                      OnBootNotificationRequestReceived,
                      loggingDelegate => loggingDelegate.Invoke(
                          Timestamp.Now,
                          parentNetworkingNode,
                          WebSocketConnection,
                          request,
                          CancellationToken
                      )
                  );

            #endregion


            #region Send OnBootNotificationRequestFilter event

            var forwardingDecision = await CallFilter(
                                               OnBootNotificationRequestFilter,
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
                forwardingDecision = new RequestForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
                                         request,
                                         ForwardingDecisions.FORWARD
                                        // NewRequest: request.Clone()//.SerializationFormat = SerializationFormats.JSON
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingDecisions.REJECT && forwardingDecision.RejectResponse is null))
            {

                var dataTransferResponse = forwardingDecision?.RejectResponse ??
                                               new BootNotificationResponse(
                                                   request,
                                                   RegistrationStatus.Rejected,
                                                   Timestamp.Now,
                                                   BootNotificationResponse.DefaultInterval,
                                                   Result: Result.Filtered(RequestForwardingDecision.DefaultLogMessage)
                                               );

                forwardingDecision = new RequestForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
                                         request,
                                         ForwardingDecisions.REJECT,
                                         dataTransferResponse,
                                         dataTransferResponse.ToJSON(
                                             parentNetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            //ToDo: Test how to translate a binary message into JSON and vice versa!
            if (forwardingDecision.NewRequest is not null)
            {
                if (forwardingDecision.NewRequest.SerializationFormat.Group() == SerializationFormatGroup.JSON)
                    forwardingDecision.NewJSONRequest   = forwardingDecision.NewRequest.ToJSON(
                                                              parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                                              parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                              parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                          );

                //else
                //    forwardingDecision.NewBinaryRequest = forwardingDecision.NewRequest.ToBinary(
                //                                              //parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                //                                              //parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                //                                              //parentNetworkingNode.OCPP.CustomSignatureSerializer,
                //                                              //parentNetworkingNode.OCPP.CustomCustomDataSerializer
                //                                          );

            }

            #region Send OnBootNotificationRequestFiltered event

            await LogEvent(
                      OnBootNotificationRequestFiltered,
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


            #region Attach OnBootNotificationRequestSent event

            if (forwardingDecision.Result == ForwardingDecisions.FORWARD)
            {

                var sentLogging = OnBootNotificationRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) =>
                        await LogEvent(
                                  OnBootNotificationRequestSent,
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
