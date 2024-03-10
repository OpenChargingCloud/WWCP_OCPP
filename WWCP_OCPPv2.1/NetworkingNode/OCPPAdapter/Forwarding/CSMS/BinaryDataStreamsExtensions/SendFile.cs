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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnSendFileRequestFilterDelegate?      OnSendFileRequest;

        public event OnSendFileRequestFilteredDelegate?    OnSendFileRequestLogging;

        #endregion

        public async Task<ForwardingDecision>

            Forward_SendFile(OCPP_BinaryRequestMessage  BinaryRequestMessage,
                             IWebSocketConnection       Connection,
                             CancellationToken          CancellationToken   = default)

        {

            if (!SendFileRequest.TryParse(BinaryRequestMessage.Payload,
                                          BinaryRequestMessage.RequestId,
                                          BinaryRequestMessage.DestinationId,
                                          BinaryRequestMessage.NetworkPath,
                                          out var Request,
                                          out var errorResponse,
                                          parentNetworkingNode.OCPP.CustomSendFileRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<SendFileRequest, SendFileResponse>? forwardingDecision = null;

            #region Send OnSendFileRequest event

            var requestFilter = OnSendFileRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnSendFileRequestFilterDelegate>().
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
                              nameof(OnSendFileRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultResult == ForwardingResult.FORWARD)
                forwardingDecision = new ForwardingDecision<SendFileRequest, SendFileResponse>(
                                         Request,
                                         ForwardingResult.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResult.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new SendFileResponse(
                                       Request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<SendFileRequest, SendFileResponse>(
                                         Request,
                                         ForwardingResult.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomSendFileResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnSendFileRequestLogging event

            var logger = OnSendFileRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnSendFileRequestFilteredDelegate>().
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
                              nameof(OnSendFileRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
