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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

        OnBinaryDataTransferRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  BinaryDataTransferRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBinaryDataTransferRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    BinaryDataTransferRequest                                                   Request,
                                                    ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD : IOCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnBinaryDataTransferRequestReceivedDelegate?    OnBinaryDataTransferRequestReceived;
        public event OnBinaryDataTransferRequestFilterDelegate?      OnBinaryDataTransferRequestFilter;
        public event OnBinaryDataTransferRequestFilteredDelegate?    OnBinaryDataTransferRequestFiltered;
        public event OnBinaryDataTransferRequestSentDelegate?        OnBinaryDataTransferRequestSent;

        public event OnBinaryDataTransferResponseReceivedDelegate?   OnBinaryDataTransferResponseReceived;
        public event OnBinaryDataTransferResponseSentDelegate?       OnBinaryDataTransferResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_BinaryDataTransfer(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                                       IWebSocketConnection       Connection,
                                       CancellationToken          CancellationToken   = default)

        {

            if (!BinaryDataTransferRequest.TryParse(BinaryRequestMessage.Payload,
                                                    BinaryRequestMessage.RequestId,
                                                    BinaryRequestMessage.DestinationId,
                                                    BinaryRequestMessage.NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }


            ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>? forwardingDecision = null;


            #region Send OnBinaryDataTransferRequestReceived event

            var receivedLogging = OnBinaryDataTransferRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(receivedLogging.GetInvocationList().
                                          OfType<OnBinaryDataTransferRequestReceivedDelegate>().
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
                                nameof(OnBinaryDataTransferRequestReceived),
                                e
                            );
                }

            }

            #endregion

            #region Send OnBinaryDataTransferRequestFilter event

            var requestFilter = OnBinaryDataTransferRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnBinaryDataTransferRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
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
                              nameof(OnBinaryDataTransferRequestFilter),
                              e
                          );
                }

            }

            #endregion


            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.BinaryRejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new BinaryDataTransferResponse(
                                       request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToBinary(
                                             parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         )
                                     );

            }

            #endregion


            #region Send OnBinaryDataTransferRequestFiltered event

            var logger = OnBinaryDataTransferRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnBinaryDataTransferRequestFilteredDelegate>().
                                       Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
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
                              nameof(OnBinaryDataTransferRequestFiltered),
                              e
                          );
                }

            }

            #endregion

            #region Send OnBinaryDataTransferRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnBinaryDataTransferRequestSent;
                if (sentLogging is not null)
                {
                    try
                    {

                        await Task.WhenAll(sentLogging.GetInvocationList().
                                              OfType<OnBinaryDataTransferRequestSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(
                                                                           Timestamp.Now,
                                                                           parentNetworkingNode,
                                                                           request,
                                                                           SendMessageResult.Success
                                                                       )).
                                              ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                    "NetworkingNode",
                                    nameof(OnBinaryDataTransferRequestSent),
                                    e
                                );
                    }

                }

            }

            #endregion


            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewBinaryRequest = forwardingDecision.NewRequest.ToBinary(
                                                          parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                                                          parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                      );

            return forwardingDecision;

        }

    }

}
