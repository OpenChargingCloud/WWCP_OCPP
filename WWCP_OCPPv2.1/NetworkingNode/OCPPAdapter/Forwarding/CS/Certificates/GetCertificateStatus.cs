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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetCertificateStatusRequestFilterDelegate?      OnGetCertificateStatusRequest;

        public event OnGetCertificateStatusRequestFilteredDelegate?    OnGetCertificateStatusRequestLogging;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetCertificateStatus(OCPP_JSONRequestMessage  JSONRequestMessage,
                                         IWebSocketConnection     Connection,
                                         CancellationToken        CancellationToken   = default)

        {

            if (!GetCertificateStatusRequest.TryParse(JSONRequestMessage.Payload,
                                                      JSONRequestMessage.RequestId,
                                                      JSONRequestMessage.DestinationId,
                                                      JSONRequestMessage.NetworkPath,
                                                      out var Request,
                                                      out var errorResponse,
                                                      parentNetworkingNode.OCPP.CustomGetCertificateStatusRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<GetCertificateStatusRequest, GetCertificateStatusResponse>? forwardingDecision = null;

            #region Send OnGetCertificateStatusRequest event

            var requestFilter = OnGetCertificateStatusRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnGetCertificateStatusRequestFilterDelegate>().
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
                              nameof(OnGetCertificateStatusRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultResult == ForwardingResult.FORWARD)
                forwardingDecision = new ForwardingDecision<GetCertificateStatusRequest, GetCertificateStatusResponse>(
                                         Request,
                                         ForwardingResult.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResult.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetCertificateStatusResponse(
                                       Request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetCertificateStatusRequest, GetCertificateStatusResponse>(
                                         Request,
                                         ForwardingResult.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetCertificateStatusResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnGetCertificateStatusRequestLogging event

            var logger = OnGetCertificateStatusRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnGetCertificateStatusRequestFilteredDelegate>().
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
                              nameof(OnGetCertificateStatusRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
