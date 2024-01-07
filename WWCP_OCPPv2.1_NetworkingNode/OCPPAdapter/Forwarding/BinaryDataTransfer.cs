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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnBinaryDataTransferRequestFilterDelegate?      OnBinaryDataTransferRequest;

        public event OnBinaryDataTransferRequestFilteredDelegate?    OnBinaryDataTransferRequestLogging;

        #endregion

        public async Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

            Forward_BinaryDataTransfer(BinaryDataTransferRequest  Request,
                                       IWebSocketConnection       Connection,
                                       CancellationToken          CancellationToken   = default)

        {

            #region Send OnBinaryDataTransferRequest event

            ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>? forwardingDecision = null;

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
                              nameof(TestNetworkingNode),
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
                                         ForwardingResult.FORWARD,
                                         LogMessage: "Default handler"
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResult.REJECT && forwardingDecision.BinaryRejectResponse is null))
            {

                var binaryDataTransferResponse = forwardingDecision?.RejectResponse ??
                                                     new BinaryDataTransferResponse(
                                                         Request,
                                                         Result.Filtered("Default handler")
                                                     );

                forwardingDecision = new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                         Request,
                                         ForwardingResult.REJECT,
                                         binaryDataTransferResponse,
                                         null,
                                         binaryDataTransferResponse.ToBinary(
                                             parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                             IncludeSignatures: true
                                         ),
                                         "Default handler"
                                     );

            }

            #endregion


            #region Send OnBinaryDataTransferRequestLogging event

            var resultLog = OnBinaryDataTransferRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
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
                              nameof(TestNetworkingNode),
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
