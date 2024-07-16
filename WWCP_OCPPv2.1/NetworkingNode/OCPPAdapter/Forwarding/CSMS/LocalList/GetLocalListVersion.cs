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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>>

        OnGetLocalListVersionRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetLocalListVersionRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetLocalListVersionRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetLocalListVersionRequest                                                    Request,
                                                     ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>   ForwardingDecision);

    #endregion


    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetLocalListVersionRequestFilterDelegate?      OnGetLocalListVersionRequest;

        public event OnGetLocalListVersionRequestFilteredDelegate?    OnGetLocalListVersionRequestLogging;

        #endregion

        public async Task<ForwardingDecision>

            Forward_GetLocalListVersion(OCPP_JSONRequestMessage  JSONRequestMessage,
                                        IWebSocketConnection     Connection,
                                        CancellationToken        CancellationToken   = default)

        {

            if (!GetLocalListVersionRequest.TryParse(JSONRequestMessage.Payload,
                                                     JSONRequestMessage.RequestId,
                                                     JSONRequestMessage.DestinationId,
                                                     JSONRequestMessage.NetworkPath,
                                                     out var Request,
                                                     out var errorResponse,
                                                     parentNetworkingNode.OCPP.CustomGetLocalListVersionRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>? forwardingDecision = null;

            #region Send OnGetLocalListVersionRequest event

            var requestFilter = OnGetLocalListVersionRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnGetLocalListVersionRequestFilterDelegate>().
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
                              nameof(OnGetLocalListVersionRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>(
                                         Request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new GetLocalListVersionResponse(
                                       Request,
                                       Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>(
                                         Request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomGetLocalListVersionResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion


            #region Send OnGetLocalListVersionRequestLogging event

            var logger = OnGetLocalListVersionRequestLogging;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                       OfType <OnGetLocalListVersionRequestFilteredDelegate>().
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
                              nameof(OnGetLocalListVersionRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
