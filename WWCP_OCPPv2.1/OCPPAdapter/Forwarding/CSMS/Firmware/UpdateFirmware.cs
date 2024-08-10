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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates

    /// <summary>
    /// A filtered UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>>

        OnUpdateFirmwareRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              UpdateFirmwareRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUpdateFirmwareRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                UpdateFirmwareRequest                                               Request,
                                                ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>   ForwardingDecision);

    #endregion

    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnUpdateFirmwareRequestReceivedDelegate?    OnUpdateFirmwareRequestReceived;
        public event OnUpdateFirmwareRequestFilterDelegate?      OnUpdateFirmwareRequestFilter;
        public event OnUpdateFirmwareRequestFilteredDelegate?    OnUpdateFirmwareRequestFiltered;
        public event OnUpdateFirmwareRequestSentDelegate?        OnUpdateFirmwareRequestSent;

        public event OnUpdateFirmwareResponseReceivedDelegate?   OnUpdateFirmwareResponseReceived;
        public event OnUpdateFirmwareResponseSentDelegate?       OnUpdateFirmwareResponseSent;

        #endregion

        public async Task<ForwardingDecision>

            Forward_UpdateFirmware(OCPP_JSONRequestMessage  JSONRequestMessage,
                                   IWebSocketConnection     Connection,
                                   CancellationToken        CancellationToken   = default)

        {

            if (!UpdateFirmwareRequest.TryParse(JSONRequestMessage.Payload,
                                                JSONRequestMessage.RequestId,
                                                JSONRequestMessage.DestinationId,
                                                JSONRequestMessage.NetworkPath,
                                                out var request,
                                                out var errorResponse,
                                                JSONRequestMessage.RequestTimestamp,
                                                JSONRequestMessage.RequestTimeout - Timestamp.Now,
                                                JSONRequestMessage.EventTrackingId,
                                                parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestParser))
            {
                return ForwardingDecision.REJECT(errorResponse);
            }

            ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>? forwardingDecision = null;

            #region Send OnUpdateFirmwareRequestReceived event

            var receivedLogging = OnUpdateFirmwareRequestReceived;
            if (receivedLogging is not null)
            {
                try
                {

                    await Task.WhenAll(
                              receivedLogging.GetInvocationList().
                                  OfType<OnUpdateFirmwareRequestReceivedDelegate>().
                                  Select(filterDelegate => filterDelegate.Invoke(
                                                               Timestamp.Now,
                                                               parentNetworkingNode,
                                                               Connection,
                                                               request
                                                           )).
                                  ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnUpdateFirmwareRequestReceived),
                              e
                          );
                }

            }

            #endregion


            #region Send OnUpdateFirmwareRequestFilter event

            var requestFilter = OnUpdateFirmwareRequestFilter;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(
                                            requestFilter.GetInvocationList().
                                                OfType<OnUpdateFirmwareRequestFilterDelegate>().
                                                Select(filterDelegate => filterDelegate.Invoke(
                                                                             Timestamp.Now,
                                                                             parentNetworkingNode,
                                                                             Connection,
                                                                             request,
                                                                             CancellationToken
                                                                         )).
                                                ToArray()
                                        );

                    //ToDo: Find a good result!
                    forwardingDecision = results.First();

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnUpdateFirmwareRequestFilter),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            if (forwardingDecision is null && DefaultForwardingResult == ForwardingResults.FORWARD)
                forwardingDecision = new ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>(
                                         request,
                                         ForwardingResults.FORWARD
                                     );

            if (forwardingDecision is null ||
               (forwardingDecision.Result == ForwardingResults.REJECT && forwardingDecision.RejectResponse is null))
            {

                var response = forwardingDecision?.RejectResponse ??
                                   new UpdateFirmwareResponse(
                                       request,
                                       UpdateFirmwareStatus.Rejected,
                                       Result: Result.Filtered(ForwardingDecision.DefaultLogMessage)
                                   );

                forwardingDecision = new ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>(
                                         request,
                                         ForwardingResults.REJECT,
                                         response,
                                         response.ToJSON(
                                             parentNetworkingNode.OCPP.CustomUpdateFirmwareResponseSerializer,
                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                         )
                                     );

            }

            #endregion

            if (forwardingDecision.NewRequest is not null)
                forwardingDecision.NewJSONRequest = forwardingDecision.NewRequest.ToJSON(
                                                        parentNetworkingNode.OCPP.CustomUpdateFirmwareRequestSerializer,
                                                        parentNetworkingNode.OCPP.CustomFirmwareSerializer,
                                                        parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                        parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                    );

            #region Send OnUpdateFirmwareRequestFiltered event

            var logger = OnUpdateFirmwareRequestFiltered;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              logger.GetInvocationList().
                                  OfType<OnUpdateFirmwareRequestFilteredDelegate>().
                                  Select(loggingDelegate => loggingDelegate.Invoke(
                                                                Timestamp.Now,
                                                                parentNetworkingNode,
                                                                Connection,
                                                                request,
                                                                forwardingDecision
                                                            )).
                                  ToArray()
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(NetworkingNode),
                              nameof(OnUpdateFirmwareRequestFiltered),
                              e
                          );
                }

            }

            #endregion


            #region Attach OnUpdateFirmwareRequestSent event

            if (forwardingDecision.Result == ForwardingResults.FORWARD)
            {

                var sentLogging = OnUpdateFirmwareRequestSent;
                if (sentLogging is not null)
                    forwardingDecision.SentMessageLogger = async (sentMessageResult) => {

                        try
                        {

                            await Task.WhenAll(
                                      sentLogging.GetInvocationList().
                                          OfType<OnUpdateFirmwareRequestSentDelegate>().
                                          Select(filterDelegate => filterDelegate.Invoke(
                                                                       Timestamp.Now,
                                                                       parentNetworkingNode,
                                                                       sentMessageResult.Connection,
                                                                       request,
                                                                       sentMessageResult.Result
                                                                   )).
                                          ToArray()
                                  );

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(NetworkingNode),
                                      nameof(OnUpdateFirmwareRequestSent),
                                      e
                                  );
                        }

                    };

            }

            #endregion

            return forwardingDecision;


        }

    }

}
