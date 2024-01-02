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
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>

        OnBootNotificationFilterDelegate(DateTime                  Timestamp,
                                         IEventSender              Sender,
                                         IWebSocketConnection      Connection,
                                         BootNotificationRequest   Request,
                                         CancellationToken         CancellationToken);


    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBootNotificationFilteredDelegate(DateTime                                                                Timestamp,
                                           IEventSender                                                            Sender,
                                           IWebSocketConnection                                                    Connection,
                                           BootNotificationRequest                                                 Request,
                                           ForwardingDecision<BootNotificationRequest, BootNotificationResponse>   ForwardingDecision);


    //public partial class INPUT : INetworkingNodeIN
    //{

    //    #region Events

    //    /// <summary>
    //    /// An event fired whenever a BootNotification request was received from the CSMS.
    //    /// </summary>
    //    public event OCPPv2_1.CSMS.OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

    //    /// <summary>
    //    /// An event sent whenever a reset request was received.
    //    /// </summary>
    //    public event OCPPv2_1.CSMS.OnBootNotificationDelegate?          OnBootNotification;

    //    /// <summary>
    //    /// An event fired whenever a response to a BootNotification request was sent.
    //    /// </summary>
    //    public event OCPPv2_1.CSMS.OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

    //    #endregion


    //    private async Task<BootNotificationResponse>

    //        ProcessIT(DateTime                 timestamp,
    //                  IEventSender             sender,
    //                  IWebSocketConnection     connection,
    //                  BootNotificationRequest  request,
    //                  CancellationToken        cancellationToken)

    //    {

    //        #region Send OnBootNotificationRequest event

    //        var startTime = Timestamp.Now;

    //        var requestLogger = OnBootNotificationRequest;
    //        if (requestLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(requestLogger.GetInvocationList().
    //                                                    OfType <OCPPv2_1.CSMS.OnBootNotificationRequestDelegate>().
    //                                                    Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                    sender,
    //                                                                                                    connection,
    //                                                                                                    request)).
    //                                                    ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                            nameof(TestNetworkingNode),
    //                            nameof(OnBootNotificationRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion


    //        #region Forwarding of the request...

    //        BootNotificationResponse? response = null;

    //        if (request.DestinationNodeId != parentNetworkingNode.Id)
    //        {

    //            #region Check request signature(s)

    //            if (!parentNetworkingNode.ForwardingSignaturePolicy.VerifyRequestMessage(
    //                    request,
    //                    request.ToJSON(
    //                        parentNetworkingNode.OCPPAdapter.CustomBootNotificationRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomChargingStationSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new BootNotificationResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                DebugX.Log($"Forwarding incoming BootNotification request to '{request.DestinationNodeId}'!");

    //                var filterResult  = await parentNetworkingNode.FORWARD.ProcessBootNotification(request,
    //                                                                                                connection,
    //                                                                                                cancellationToken);

    //                switch (filterResult.Result)
    //                {

    //                    case ForwardingResult.FORWARD:
    //                        response = await parentNetworkingNode.OUT.BootNotification(request);
    //                        break;

    //                    case ForwardingResult.DROP:
    //                        response = filterResult.DropResponse;
    //                        break;

    //                }

    //            }

    //        }

    //        #endregion

    //        else
    //        {

    //            #region Check request signature(s)

    //            if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
    //                    request,
    //                    request.ToJSON(
    //                        parentNetworkingNode.OCPPAdapter.CustomBootNotificationRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomChargingStationSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new BootNotificationResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                var requestHandler = OnBootNotification;
    //                if (requestHandler is not null)
    //                {
    //                    try
    //                    {

    //                        response = (await Task.WhenAll(
    //                                                requestHandler.GetInvocationList().
    //                                                                OfType <OnBootNotificationDelegate>().
    //                                                                Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                                    sender,
    //                                                                                                                    connection,
    //                                                                                                                    request,
    //                                                                                                                    cancellationToken)).
    //                                                                ToArray())).First();

    //                    }
    //                    catch (Exception e)
    //                    {
    //                        await HandleErrors(
    //                                    nameof(TestNetworkingNode),
    //                                    nameof(OnBootNotificationRequest),
    //                                    e
    //                                );
    //                    }

    //                }

    //            }

    //        }

    //        #region Default response

    //        response ??= new BootNotificationResponse(
    //                            Request:       request,
    //                            Status:        RegistrationStatus.Rejected,
    //                            CurrentTime:   Timestamp.Now,
    //                            Interval:      TimeSpan.FromMinutes(5),
    //                            StatusInfo:    null,
    //                            CustomData:    null
    //                        );

    //        #endregion

    //        #region Sign response message

    //        parentNetworkingNode.SignaturePolicy.SignResponseMessage(
    //            response,
    //            response.ToJSON(
    //                parentNetworkingNode.OCPPAdapter.CustomBootNotificationResponseSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomStatusInfoSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //            ),
    //            out var errorResponse2);

    //        #endregion


    //        #region Send OnBootNotificationResponse event

    //        var endTime = Timestamp.Now;

    //        var responseLogger = OnBootNotificationResponse;
    //        if (responseLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(responseLogger.GetInvocationList().
    //                                                    OfType <OCPPv2_1.CSMS.OnBootNotificationResponseDelegate>().
    //                                                    Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                        sender,
    //                                                                                                        connection,
    //                                                                                                        request,
    //                                                                                                        response,
    //                                                                                                        endTime - startTime)).
    //                                                    ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                            nameof(TestNetworkingNode),
    //                            nameof(OnBootNotificationRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion

    //        return response;

    //    }


    //    public void WireBootNotification(NetworkingNode.CS.  INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        //IncomingMessages.OnBootNotification += ProcessIT;
    //    }

    //    // MAIN!!!
    //    public void WireBootNotification(NetworkingNode.CSMS.INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        IncomingMessages.OnBootNotification += ProcessIT;
    //    }

    //}


    public partial class FORWARD
    {

        #region Events

        public event OnBootNotificationFilterDelegate?    OnBootNotification;

        public event OnBootNotificationFilteredDelegate?  OnBootNotificationLogging;

        #endregion

        public async Task<ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>

            ProcessBootNotification(BootNotificationRequest  Request,
                                    IWebSocketConnection     Connection,
                                    CancellationToken        CancellationToken   = default)

        {

            ForwardingDecision<BootNotificationRequest, BootNotificationResponse>? forwardingDecision = null;

            var requestFilter = OnBootNotification;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnBootNotificationFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new BootNotificationResponse(
                                                       Request,
                                                       Result.Filtered("Default handler")
                                                   ),
                                                   "Default handler"
                                               )
                                             : response;

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(TestNetworkingNode),
                              nameof(OnBootNotification),
                              e
                          );
                }

            }

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new BootNotificationResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );


            var resultLog = OnBootNotificationLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnBootNotificationFilteredDelegate>().
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
                              nameof(OnBootNotificationLogging),
                              e
                          );
                }

            }

            return forwardingDecision;

        }

    }




}
