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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD : IOCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnBinaryDataTransferRequestFilterDelegate?      OnBinaryDataTransferRequest;

        public event OnBinaryDataTransferRequestFilteredDelegate?    OnBinaryDataTransferRequestLogging;

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
                                                    out var Request,
                                                    out var errorResponse,
                                                    parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>? forwardingDecision = null;

            #region Send OnBinaryDataTransferRequest event

            var requestFilter = OnBinaryDataTransferRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnBinaryDataTransferRequestFilterDelegate>().
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
                              nameof(OnBinaryDataTransferRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultResult == ForwardingResult.FORWARD)
                forwardingDecision = new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         Request,
                                         ForwardingResult.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResult.REJECT && forwardingDecision.BinaryRejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new BinaryDataTransferResponse(
                                       Request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         Request,
                                         ForwardingResult.REJECT,
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


            #region Send OnBinaryDataTransferRequestLogging event

            var logger = OnBinaryDataTransferRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnBinaryDataTransferRequestFilteredDelegate>().
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
                              nameof(OnBinaryDataTransferRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
