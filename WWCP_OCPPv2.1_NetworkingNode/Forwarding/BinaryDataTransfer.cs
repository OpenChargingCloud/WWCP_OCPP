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
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

        OnBinaryDataTransferFilterDelegate(DateTime                    Timestamp,
                                           IEventSender                Sender,
                                           IWebSocketConnection        Connection,
                                           BinaryDataTransferRequest   Request,
                                           CancellationToken           CancellationToken);


    /// <summary>
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBinaryDataTransferFilteredDelegate(DateTime                                                                    Timestamp,
                                             IEventSender                                                                Sender,
                                             IWebSocketConnection                                                        Connection,
                                             BinaryDataTransferRequest                                                   Request,
                                             ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>   ForwardingDecision);


    //public partial class INPUT : INetworkingNodeIN
    //{

    //    #region Events

    //    /// <summary>
    //    /// An event fired whenever a BinaryDataTransfer request was received from the CSMS.
    //    /// </summary>
    //    public event OnIncomingBinaryDataTransferRequestDelegate?     OnIncomingBinaryDataTransferRequest;

    //    /// <summary>
    //    /// An event sent whenever a reset request was received.
    //    /// </summary>
    //    public event OnIncomingBinaryDataTransferDelegate?            OnIncomingBinaryDataTransfer;

    //    /// <summary>
    //    /// An event fired whenever a response to a BinaryDataTransfer request was sent.
    //    /// </summary>
    //    public event OnIncomingBinaryDataTransferResponseDelegate?    OnIncomingBinaryDataTransferResponse;

    //    #endregion


    //    private async Task<BinaryDataTransferResponse>

    //        ProcessIT(DateTime                   timestamp,
    //                  IEventSender               sender,
    //                  IWebSocketConnection       connection,
    //                  BinaryDataTransferRequest  request,
    //                  CancellationToken          cancellationToken)

    //    {

    //        #region Send OnBinaryDataTransferRequest event

    //        var startTime = Timestamp.Now;

    //        var requestLogger = OnIncomingBinaryDataTransferRequest;
    //        if (requestLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(requestLogger.GetInvocationList().
    //                                                 OfType <OnIncomingBinaryDataTransferRequestDelegate>().
    //                                                 Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                   sender,
    //                                                                                                   connection,
    //                                                                                                   request)).
    //                                                 ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                            nameof(TestNetworkingNode),
    //                            nameof(OnIncomingBinaryDataTransferRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion


    //        #region Forwarding of the request...

    //        BinaryDataTransferResponse? response = null;

    //        if (request.DestinationNodeId != parentNetworkingNode.Id)
    //        {

    //            #region Check request signature(s)

    //            if (!parentNetworkingNode.ForwardingSignaturePolicy.VerifyRequestMessage(
    //                    request,
    //                    request.ToBinary(
    //                        parentNetworkingNode.OCPPAdapter.CustomBinaryDataTransferRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomBinarySignatureSerializer,
    //                        IncludeSignatures: false
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new BinaryDataTransferResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                DebugX.Log($"Forwarding incoming BinaryDataTransfer request to '{request.DestinationNodeId}'!");

    //                var filterResult  = await parentNetworkingNode.FORWARD.ProcessBinaryDataTransfer(request,
    //                                                                                                 connection,
    //                                                                                                 cancellationToken);

    //                switch (filterResult.Result)
    //                {

    //                    case ForwardingResult.FORWARD:
    //                        response = await parentNetworkingNode.OUT.BinaryDataTransfer(request);
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
    //                    request.ToBinary(
    //                        parentNetworkingNode.OCPPAdapter.CustomBinaryDataTransferRequestSerializer,
    //                        parentNetworkingNode.OCPPAdapter.CustomBinarySignatureSerializer,
    //                        IncludeSignatures: false
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new BinaryDataTransferResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                var requestHandler = OnIncomingBinaryDataTransfer;
    //                if (requestHandler is not null)
    //                {
    //                    try
    //                    {

    //                        response = (await Task.WhenAll(
    //                                                requestHandler.GetInvocationList().
    //                                                               OfType <OnIncomingBinaryDataTransferDelegate>().
    //                                                               Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                                 sender,
    //                                                                                                                 connection,
    //                                                                                                                 request,
    //                                                                                                                 cancellationToken)).
    //                                                               ToArray())).First();

    //                    }
    //                    catch (Exception e)
    //                    {
    //                        await HandleErrors(
    //                                  nameof(TestNetworkingNode),
    //                                  nameof(OnIncomingBinaryDataTransfer),
    //                                  e
    //                              );
    //                    }

    //                }

    //            }

    //        }

    //        #region Default response

    //        response ??= new BinaryDataTransferResponse(
    //                         Request:                request,
    //                         Status:                 BinaryDataTransferStatus.Rejected,
    //                         AdditionalStatusInfo:   null,
    //                         Data:                   null,
    //                         Format:                 request.Format
    //                     );

    //        #endregion

    //        #region Sign response message

    //        parentNetworkingNode.SignaturePolicy.SignResponseMessage(
    //            response,
    //            response.ToBinary(
    //                parentNetworkingNode.OCPPAdapter.CustomBinaryDataTransferResponseSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomStatusInfoSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomBinarySignatureSerializer,
    //                IncludeSignatures: true
    //            ),
    //            out var errorResponse2);

    //        #endregion


    //        #region Send OnBinaryDataTransferResponse event

    //        var endTime = Timestamp.Now;

    //        var responseLogger = OnIncomingBinaryDataTransferResponse;
    //        if (responseLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(responseLogger.GetInvocationList().
    //                                                  OfType <OnIncomingBinaryDataTransferResponseDelegate>().
    //                                                  Select (loggingDelegate => loggingDelegate.Invoke(timestamp,
    //                                                                                                    sender,
    //                                                                                                    connection,
    //                                                                                                    request,
    //                                                                                                    response,
    //                                                                                                    endTime - startTime)).
    //                                                  ToArray());

    //            }
    //            catch (Exception e)
    //            {
    //                await HandleErrors(
    //                            nameof(TestNetworkingNode),
    //                            nameof(OnIncomingBinaryDataTransferResponse),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion

    //        return response;

    //    }


    //    public void WireBinaryDataTransfer(CS.  INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        IncomingMessages.OnIncomingBinaryDataTransfer += ProcessIT;
    //    }

    //    // MAIN!!!
    //    public void WireBinaryDataTransfer(CSMS.INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        IncomingMessages.OnIncomingBinaryDataTransfer += ProcessIT;
    //    }

    //}


    public partial class FORWARD
    {

        #region Events

        public event OnBinaryDataTransferFilterDelegate?    OnBinaryDataTransfer;

        public event OnBinaryDataTransferFilteredDelegate?  OnBinaryDataTransferLogging;

        #endregion

        public async Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

            ProcessBinaryDataTransfer(BinaryDataTransferRequest  Request,
                                      IWebSocketConnection       Connection,
                                      CancellationToken          CancellationToken   = default)

        {

            ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>? forwardingDecision = null;

            var requestFilter = OnBinaryDataTransfer;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnBinaryDataTransferFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new BinaryDataTransferResponse(
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
                              nameof(OnBinaryDataTransfer),
                              e
                          );
                }

            }

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new BinaryDataTransferResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );


            var resultLog = OnBinaryDataTransferLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnBinaryDataTransferFilteredDelegate>().
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
                              nameof(OnBinaryDataTransferLogging),
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
    //    /// An event fired whenever a BinaryDataTransfer request will be sent.
    //    /// </summary>
    //    public event OnBinaryDataTransferRequestDelegate?   OnBinaryDataTransferRequest;

    //    /// <summary>
    //    /// An event fired whenever a response to a BinaryDataTransfer request was received.
    //    /// </summary>
    //    public event OnBinaryDataTransferResponseDelegate?  OnBinaryDataTransferResponse;

    //    #endregion


    //    /// <summary>
    //    /// Send the given vendor-specific binary data.
    //    /// </summary>
    //    /// <param name="Request">A BinaryDataTransfer request.</param>
    //    public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
    //    {

    //        #region Send OnBinaryDataTransferRequest event

    //        var startTime = Timestamp.Now;

    //        try
    //        {

    //            OnBinaryDataTransferRequest?.Invoke(startTime,
    //                                                parentNetworkingNode,
    //                                                Request);

    //        }
    //        catch (Exception e)
    //        {
    //            DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBinaryDataTransferRequest));
    //        }

    //        #endregion


    //        var response = LookupNetworkingNode(Request.DestinationNodeId, out var channel) &&
    //                            channel is not null

    //                            ? parentNetworkingNode.SignaturePolicy.SignRequestMessage(
    //                                  Request,
    //                                  Request.ToBinary(
    //                                      parentNetworkingNode.OCPPAdapter.CustomBinaryDataTransferRequestSerializer,
    //                                      parentNetworkingNode.OCPPAdapter.CustomBinarySignatureSerializer,
    //                                      IncludeSignatures: false
    //                                  ),
    //                                  out var errorResponse
    //                              )

    //                                  ? await channel.BinaryDataTransfer(Request)

    //                                  : new BinaryDataTransferResponse(
    //                                        Request,
    //                                        Result.SignatureError(errorResponse)
    //                                    )

    //                            : new BinaryDataTransferResponse(
    //                                  Request,
    //                                  Result.UnknownOrUnreachable(Request.DestinationNodeId)
    //                              );


    //        parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
    //            response,
    //            response.ToBinary(
    //                parentNetworkingNode.OCPPAdapter.CustomBinaryDataTransferResponseSerializer,
    //                null, //parentNetworkingNode.OCPPAdapter.CustomStatusInfoSerializer,
    //                parentNetworkingNode.OCPPAdapter.CustomBinarySignatureSerializer,
    //                IncludeSignatures: false
    //            ),
    //            out errorResponse
    //        );


    //        #region Send OnBinaryDataTransferResponse event

    //        var endTime = Timestamp.Now;

    //        try
    //        {

    //            OnBinaryDataTransferResponse?.Invoke(endTime,
    //                                                    parentNetworkingNode,
    //                                                    Request,
    //                                                    response,
    //                                                    endTime - startTime);

    //        }
    //        catch (Exception e)
    //        {
    //            DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBinaryDataTransferResponse));
    //        }

    //        #endregion

    //        return response;

    //    }


    //}

}
