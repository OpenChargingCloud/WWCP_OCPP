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
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    //public partial class INPUT : INetworkingNodeIN
    //{

    //    #region Events

    //    /// <summary>
    //    /// An event fired whenever a Reset request was received from the CSMS.
    //    /// </summary>
    //    public event OCPPv2_1.CS.OnResetRequestDelegate?     OnResetRequest;

    //    /// <summary>
    //    /// An event sent whenever a reset request was received.
    //    /// </summary>
    //    public event OCPPv2_1.CS.OnResetDelegate?            OnReset;

    //    /// <summary>
    //    /// An event fired whenever a response to a Reset request was sent.
    //    /// </summary>
    //    public event OCPPv2_1.CS.OnResetResponseDelegate?    OnResetResponse;

    //    #endregion


    //    private async Task<ResetResponse>

    //        ProcessIT(DateTime              timestamp,
    //                  IEventSender          sender,
    //                  IWebSocketConnection  connection,
    //                  ResetRequest          request,
    //                  CancellationToken     cancellationToken)

    //    {

    //        #region Send OnResetRequest event

    //        var startTime = Timestamp.Now;

    //        var requestLogger = OnResetRequest;
    //        if (requestLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(requestLogger.GetInvocationList().
    //                                                    OfType <OCPPv2_1.CS.OnResetRequestDelegate>().
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
    //                            nameof(OnResetRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion


    //        #region Forwarding of the request...

    //        ResetResponse? response = null;

    //        if (request.DestinationNodeId != parentNetworkingNode.Id)
    //        {

    //            #region Check request signature(s)

    //            if (!parentNetworkingNode.ForwardingSignaturePolicy.VerifyRequestMessage(
    //                    request,
    //                    request.ToJSON(
    //                        parentNetworkingNode.OCPPAdapter.CustomResetRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new ResetResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                DebugX.Log($"Forwarding incoming '{request.ResetType}' reset request to '{request.DestinationNodeId}'!");

    //                var filterResult  = await parentNetworkingNode.FORWARD.ProcessReset(request,
    //                                                                                    connection,
    //                                                                                    cancellationToken);

    //                switch (filterResult.Result)
    //                {

    //                    case ForwardingResult.FORWARD:
    //                        response = await parentNetworkingNode.OUT.Reset(request);
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
    //                        parentNetworkingNode.OCPPAdapter.CustomResetRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new ResetResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                var requestHandler = OnReset;
    //                if (requestHandler is not null)
    //                {
    //                    try
    //                    {

    //                        response = (await Task.WhenAll(
    //                                                requestHandler.GetInvocationList().
    //                                                                OfType <OnResetDelegate>().
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
    //                                    nameof(OnResetRequest),
    //                                    e
    //                                );
    //                    }

    //                }

    //            }

    //        }

    //        #region Default response

    //        response ??= new ResetResponse(
    //                            Request:      request,
    //                            Status:       ResetStatus.Rejected,
    //                            StatusInfo:   null,
    //                            CustomData:   null
    //                        );

    //        #endregion

    //        #region Sign response message

    //        parentNetworkingNode.SignaturePolicy.SignResponseMessage(
    //            response,
    //            response.ToJSON(
    //                parentNetworkingNode.OCPPAdapter.CustomResetResponseSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomStatusInfoSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //            ),
    //            out var errorResponse2);

    //        #endregion


    //        #region Send OnResetResponse event

    //        var endTime = Timestamp.Now;

    //        var responseLogger = OnResetResponse;
    //        if (responseLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(responseLogger.GetInvocationList().
    //                                                    OfType <OCPPv2_1.CS.OnResetResponseDelegate>().
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
    //                            nameof(OnResetRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion

    //        return response;

    //    }


    //    // MAIN!!!
    //    public void WireReset(CS.  INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        IncomingMessages.OnReset += ProcessIT;
    //    }

    //    public void WireReset(CSMS.INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        //IncomingMessages.OnReset += ProcessIT;
    //    }


    //}


    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnResetRequestFilterDelegate?    OnResetRequest;

        public event OnResetRequestFilteredDelegate?  OnResetRequestLogging;

        #endregion

        public async Task<ForwardingDecision<ResetRequest, ResetResponse>>

            ProcessReset(ResetRequest          Request,
                         IWebSocketConnection  Connection,
                         CancellationToken     CancellationToken   = default)

        {

            ForwardingDecision<ResetRequest, ResetResponse>? forwardingDecision = null;

            var requestFilter = OnResetRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnResetRequestFilterDelegate>().
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
                              nameof(OnResetRequest),
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


            var resultLog = OnResetRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnResetRequestFilteredDelegate>().
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
                              nameof(OnResetRequestLogging),
                              e
                          );
                }

            }

            return forwardingDecision;

        }

    }


    //public partial class OUTPUT
    //{

    //    #region Events

    //    /// <summary>
    //    /// An event fired whenever a Reset request will be sent.
    //    /// </summary>
    //    public event OCPPv2_1.CSMS.OnResetRequestDelegate?   OnResetRequest;

    //    /// <summary>
    //    /// An event fired whenever a response to a Reset request was received.
    //    /// </summary>
    //    public event OCPPv2_1.CSMS.OnResetResponseDelegate?  OnResetResponse;

    //    #endregion


    //    /// <summary>
    //    /// Send a Reset request.
    //    /// </summary>
    //    /// <param name="Request">A Reset request.</param>
    //    public async Task<ResetResponse> Reset(ResetRequest Request)
    //    {

    //        #region Send OnResetRequest event

    //        var startTime = Timestamp.Now;

    //        var requestLogger = OnResetRequest;
    //        if (requestLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(requestLogger.GetInvocationList().
    //                                                 OfType <OCPPv2_1.CSMS.OnResetRequestDelegate>().
    //                                                 Select (loggingDelegate => loggingDelegate.Invoke(startTime,
    //                                                                                                   parentNetworkingNode,
    //                                                                                                   Request)).
    //                                                 ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                          nameof(TestNetworkingNode),
    //                          nameof(OnResetRequest),
    //                          e
    //                      );
    //            }

    //        }

    //        #endregion


    //        var response = LookupNetworkingNode(Request.DestinationNodeId, out var channel) &&
    //                            channel is not null

    //                            ? parentNetworkingNode.SignaturePolicy.SignRequestMessage(
    //                                  Request,
    //                                  Request.ToJSON(
    //                                      parentNetworkingNode.OCPPAdapter.CustomResetRequestSerializer,
    //                                      parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                                      parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //                                  ),
    //                                  out var errorResponse
    //                              )

    //                                  ? await channel.Reset(Request)

    //                                  : new ResetResponse(
    //                                        Request,
    //                                        Result.SignatureError(errorResponse)
    //                                    )

    //                            : new ResetResponse(
    //                                  Request,
    //                                  Result.UnknownOrUnreachable(Request.DestinationNodeId)
    //                              );


    //        parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
    //            response,
    //            response.ToJSON(
    //                parentNetworkingNode.OCPPAdapter.CustomResetResponseSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomStatusInfoSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomSignatureSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomCustomDataSerializer
    //            ),
    //            out errorResponse
    //        );


    //        #region Send OnResetResponse event

    //        var endTime = Timestamp.Now;

    //        var responseLogger = OnResetResponse;
    //        if (responseLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(responseLogger.GetInvocationList().
    //                                                  OfType <OCPPv2_1.CSMS.OnResetResponseDelegate>().
    //                                                  Select (loggingDelegate => loggingDelegate.Invoke(endTime,
    //                                                                                                    parentNetworkingNode,
    //                                                                                                    Request,
    //                                                                                                    response,
    //                                                                                                    endTime - startTime)).
    //                                                  ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                          nameof(TestNetworkingNode),
    //                          nameof(OnResetRequest),
    //                          e
    //                      );
    //            }

    //        }

    //        #endregion

    //        return response;

    //    }



    //}

}
