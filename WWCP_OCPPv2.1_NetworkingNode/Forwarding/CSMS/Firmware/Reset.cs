/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ResetRequest, ResetResponse>>

        OnResetFilterDelegate(DateTime               Timestamp,
                              IEventSender           Sender,
                              IWebSocketConnection   Connection,
                              ResetRequest           Request,
                              CancellationToken      CancellationToken);


    /// <summary>
    /// A Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnResetFilteredDelegate(DateTime                                          Timestamp,
                                IEventSender                                      Sender,
                                IWebSocketConnection                              Connection,
                                ResetRequest                                      Request,
                                ForwardingDecision<ResetRequest, ResetResponse>   ForwardingDecision);


    public partial class INPUT : INetworkingNodeIN
    {

        #region Events

        /// <summary>
        /// An event fired whenever a Reset request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnResetRequestDelegate?     OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnResetDelegate?            OnReset;

        /// <summary>
        /// An event fired whenever a response to a Reset request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnResetResponseDelegate?    OnResetResponse;

        #endregion


        private async Task<ResetResponse>

            ProcessIT(DateTime              timestamp,
                      IEventSender          sender,
                      IWebSocketConnection  connection,
                      ResetRequest          request,
                      CancellationToken     cancellationToken)

        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            var requestLogger = OnResetRequest;
            if (requestLogger is not null)
            {
                try
                {

                    await Task.WhenAll(requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnResetRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                                        sender,
                                                                                                        connection,
                                                                                                        request)).
                                                        ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnResetRequest),
                                e
                            );
                }

            }

            #endregion


            #region Forwarding of the request...

            ResetResponse? response = null;

            if (request.DestinationNodeId != parentNetworkingNode.Id)
            {

                #region Check request signature(s)

                if (!parentNetworkingNode.ForwardingSignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.CustomResetRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response = new ResetResponse(
                                    Request:  request,
                                    Result:   Result.SignatureError(
                                                    $"Invalid signature: {errorResponse}"
                                                )
                                );

                }

                #endregion

                else
                {

                    DebugX.Log($"Forwarding incoming '{request.ResetType}' reset request to '{request.DestinationNodeId}'!");

                    var filterResult  = await parentNetworkingNode.FORWARD.ProcessReset(request,
                                                                                        connection,
                                                                                        cancellationToken);

                    switch (filterResult.Result)
                    {

                        case ForwardingResult.FORWARD:
                            response = await parentNetworkingNode.AsCSMS.Reset(request);
                            break;

                        case ForwardingResult.DROP:
                            response = filterResult.DropResponse;
                            break;

                    }

                }

            }

            #endregion

            else
            {

                #region Check request signature(s)

                if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.CustomResetRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response = new ResetResponse(
                                    Request:  request,
                                    Result:   Result.SignatureError(
                                                    $"Invalid signature: {errorResponse}"
                                                )
                                );

                }

                #endregion

                else
                {

                    var requestHandler = OnReset;
                    if (requestHandler is not null)
                    {
                        try
                        {

                            response = (await Task.WhenAll(
                                                    requestHandler.GetInvocationList().
                                                                    OfType <OnResetDelegate>().
                                                                    Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                                                        sender,
                                                                                                                        connection,
                                                                                                                        request,
                                                                                                                        cancellationToken)).
                                                                    ToArray())).First();

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                        nameof(TestNetworkingNode),
                                        nameof(OnResetRequest),
                                        e
                                    );
                        }

                    }

                }

            }

            #region Default response

            response ??= new ResetResponse(
                                Request:      request,
                                Status:       ResetStatus.Rejected,
                                StatusInfo:   null,
                                CustomData:   null
                            );

            #endregion

            #region Sign response message

            parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomResetResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out var errorResponse2);

            #endregion


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            var responseLogger = OnResetResponse;
            if (responseLogger is not null)
            {
                try
                {

                    await Task.WhenAll(responseLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnResetResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
                                                                                                            sender,
                                                                                                            connection,
                                                                                                            request,
                                                                                                            response,
                                                                                                            endTime - startTime)).
                                                        ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnResetRequest),
                                e
                            );
                }

            }

            #endregion

            return response;

        }


        // MAIN!!!
        public void WireReset(NetworkingNode.CS.  INetworkingNodeIncomingMessages IncomingMessages)
        {
            IncomingMessages.OnReset += ProcessIT;
        }

        public void WireReset(NetworkingNode.CSMS.INetworkingNodeIncomingMessages IncomingMessages)
        {
            //IncomingMessages.OnReset += ProcessIT;
        }


    }


    public partial class FORWARD
    {

        #region Events

        public event OnResetFilterDelegate?    OnReset;

        public event OnResetFilteredDelegate?  OnResetLogging;

        #endregion

        public async Task<ForwardingDecision<ResetRequest, ResetResponse>>

            ProcessReset(ResetRequest          Request,
                         IWebSocketConnection  Connection,
                         CancellationToken     CancellationToken   = default)

        {

            ForwardingDecision<ResetRequest, ResetResponse>? forwardingDecision = null;

            var requestFilter = OnReset;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnResetFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<ResetRequest, ResetResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new ResetResponse(
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
                              nameof(OnReset),
                              e
                          );
                }

            }

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<ResetRequest, ResetResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<ResetRequest, ResetResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new ResetResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );


            var resultLog = OnResetLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnResetFilteredDelegate>().
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
                              nameof(OnResetLogging),
                              e
                          );
                }

            }

            return forwardingDecision;

        }

    }

}
