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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DataTransferRequest, DataTransferResponse>>

        OnDataTransferFilterDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     DataTransferRequest    Request,
                                     CancellationToken      CancellationToken);


    /// <summary>
    /// A DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDataTransferFilteredDelegate(DateTime                                                        Timestamp,
                                       IEventSender                                                    Sender,
                                       IWebSocketConnection                                            Connection,
                                       DataTransferRequest                                             Request,
                                       ForwardingDecision<DataTransferRequest, DataTransferResponse>   ForwardingDecision);


    //public partial class INPUT : INetworkingNodeIN
    //{

    //    #region Events

    //    /// <summary>
    //    /// An event fired whenever a DataTransfer request was received from the CSMS.
    //    /// </summary>
    //    public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

    //    /// <summary>
    //    /// An event sent whenever a reset request was received.
    //    /// </summary>
    //    public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

    //    /// <summary>
    //    /// An event fired whenever a response to a DataTransfer request was sent.
    //    /// </summary>
    //    public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

    //    #endregion


    //    private async Task<DataTransferResponse>

    //        ProcessIT(DateTime               timestamp,
    //                  IEventSender           sender,
    //                  IWebSocketConnection   connection,
    //                  DataTransferRequest    request,
    //                  CancellationToken      cancellationToken)

    //    {

    //        #region Send OnDataTransferRequest event

    //        var startTime = Timestamp.Now;

    //        var requestLogger = OnIncomingDataTransferRequest;
    //        if (requestLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(requestLogger.GetInvocationList().
    //                                                 OfType <OnIncomingDataTransferRequestDelegate>().
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
    //                            nameof(OnIncomingDataTransferRequest),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion


    //        #region Forwarding of the request...

    //        DataTransferResponse? response = null;

    //        if (request.DestinationNodeId != parentNetworkingNode.Id)
    //        {

    //            #region Check request signature(s)

    //            if (!parentNetworkingNode.ForwardingSignaturePolicy.VerifyRequestMessage(
    //                    request,
    //                    request.ToJSON(
    //                        parentNetworkingNode.CustomDataTransferRequestSerializer,
    //                        parentNetworkingNode.CustomSignatureSerializer,
    //                        parentNetworkingNode.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new DataTransferResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                DebugX.Log($"Forwarding incoming DataTransfer request to '{request.DestinationNodeId}'!");

    //                var filterResult  = await parentNetworkingNode.FORWARD.ProcessDataTransfer(request,
    //                                                                                                 connection,
    //                                                                                                 cancellationToken);

    //                switch (filterResult.Result)
    //                {

    //                    case ForwardingResult.FORWARD:
    //                        response = await parentNetworkingNode.OUT.DataTransfer(request);
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
    //                        parentNetworkingNode.CustomDataTransferRequestSerializer,
    //                        parentNetworkingNode.CustomSignatureSerializer,
    //                        parentNetworkingNode.CustomCustomDataSerializer
    //                    ),
    //                    out var errorResponse
    //                ))
    //            {

    //                response = new DataTransferResponse(
    //                                Request:  request,
    //                                Result:   Result.SignatureError(
    //                                                $"Invalid signature: {errorResponse}"
    //                                            )
    //                            );

    //            }

    //            #endregion

    //            else
    //            {

    //                var requestHandler = OnIncomingDataTransfer;
    //                if (requestHandler is not null)
    //                {
    //                    try
    //                    {

    //                        response = (await Task.WhenAll(
    //                                                requestHandler.GetInvocationList().
    //                                                               OfType <OnIncomingDataTransferDelegate>().
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
    //                                  nameof(OnIncomingDataTransfer),
    //                                  e
    //                              );
    //                    }

    //                }

    //            }

    //        }

    //        #region Default response

    //        response ??= new DataTransferResponse(
    //                         Request:      request,
    //                         Status:       DataTransferStatus.Rejected,
    //                         StatusInfo:   null,
    //                         Data:         null
    //                     );

    //        #endregion

    //        #region Sign response message

    //        parentNetworkingNode.SignaturePolicy.SignResponseMessage(
    //            response,
    //            response.ToJSON(
    //                parentNetworkingNode.CustomDataTransferResponseSerializer,
    //                parentNetworkingNode.CustomStatusInfoSerializer,
    //                parentNetworkingNode.CustomSignatureSerializer,
    //                parentNetworkingNode.CustomCustomDataSerializer
    //            ),
    //            out var errorResponse2);

    //        #endregion


    //        #region Send OnDataTransferResponse event

    //        var endTime = Timestamp.Now;

    //        var responseLogger = OnIncomingDataTransferResponse;
    //        if (responseLogger is not null)
    //        {
    //            try
    //            {

    //                await Task.WhenAll(responseLogger.GetInvocationList().
    //                                                  OfType <OnIncomingDataTransferResponseDelegate>().
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
    //                            nameof(OnIncomingDataTransferResponse),
    //                            e
    //                        );
    //            }

    //        }

    //        #endregion

    //        return response;

    //    }


    //    //public void WireDataTransfer(CS.  INetworkingNodeIncomingMessages IncomingMessages)
    //    //{
    //    //    IncomingMessages.OnIncomingDataTransfer += ProcessIT;
    //    //}

    //    // MAIN!!!
    //    public void WireDataTransfer(INetworkingNodeIncomingMessages IncomingMessages)
    //    {
    //        IncomingMessages.OnIncomingDataTransfer += ProcessIT;
    //    }

    //}


    public partial class FORWARD
    {

        #region Events

        public event OnDataTransferFilterDelegate?    OnDataTransfer;

        public event OnDataTransferFilteredDelegate?  OnDataTransferLogging;

        #endregion

        public async Task<ForwardingDecision<DataTransferRequest, DataTransferResponse>>

            ProcessDataTransfer(DataTransferRequest   Request,
                                IWebSocketConnection  Connection,
                                CancellationToken     CancellationToken   = default)

        {

            ForwardingDecision<DataTransferRequest, DataTransferResponse>? forwardingDecision = null;

            var requestFilter = OnDataTransfer;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnDataTransferFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new DataTransferResponse(
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
                              nameof(OnDataTransfer),
                              e
                          );
                }

            }

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new DataTransferResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );


            var resultLog = OnDataTransferLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnDataTransferFilteredDelegate>().
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
                              nameof(OnDataTransferLogging),
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
    //    /// An event fired whenever a DataTransfer request will be sent.
    //    /// </summary>
    //    public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

    //    /// <summary>
    //    /// An event fired whenever a response to a DataTransfer request was received.
    //    /// </summary>
    //    public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

    //    #endregion


    //    /// <summary>
    //    /// Send the given vendor-specific binary data.
    //    /// </summary>
    //    /// <param name="Request">A DataTransfer request.</param>
    //    public async Task<DataTransferResponse> DataTransfer(DataTransferRequest Request)
    //    {

    //        #region Send OnDataTransferRequest event

    //        var startTime = Timestamp.Now;

    //        try
    //        {

    //            OnDataTransferRequest?.Invoke(startTime,
    //                                          parentNetworkingNode,
    //                                          Request);

    //        }
    //        catch (Exception e)
    //        {
    //            DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferRequest));
    //        }

    //        #endregion


    //        var response = LookupNetworkingNode(Request.DestinationNodeId, out var channel) &&
    //                            channel is not null

    //                            ? parentNetworkingNode.SignaturePolicy.SignRequestMessage(
    //                                  Request,
    //                                  Request.ToJSON(
    //                                      parentNetworkingNode.CustomDataTransferRequestSerializer,
    //                                      parentNetworkingNode.CustomSignatureSerializer,
    //                                      parentNetworkingNode.CustomCustomDataSerializer
    //                                  ),
    //                                  out var errorResponse
    //                              )

    //                                  ? await channel.DataTransfer(Request)

    //                                  : new DataTransferResponse(
    //                                        Request,
    //                                        Result.SignatureError(errorResponse)
    //                                    )

    //                            : new DataTransferResponse(
    //                                  Request,
    //                                  Result.UnknownOrUnreachable(Request.DestinationNodeId)
    //                              );


    //        parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
    //            response,
    //            response.ToJSON(
    //                parentNetworkingNode.CustomDataTransferResponseSerializer,
    //                parentNetworkingNode.CustomStatusInfoSerializer,
    //                parentNetworkingNode.CustomSignatureSerializer,
    //                parentNetworkingNode.CustomCustomDataSerializer
    //            ),
    //            out errorResponse
    //        );


    //        #region Send OnDataTransferResponse event

    //        var endTime = Timestamp.Now;

    //        try
    //        {

    //            OnDataTransferResponse?.Invoke(endTime,
    //                                           parentNetworkingNode,
    //                                           Request,
    //                                           response,
    //                                           endTime - startTime);

    //        }
    //        catch (Exception e)
    //        {
    //            DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferResponse));
    //        }

    //        #endregion

    //        return response;

    //    }


    //}

}
