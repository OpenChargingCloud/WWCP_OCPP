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
using cloud.charging.open.protocols.OCPP.WebSockets;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP HTTP Web Socket Adapter for incoming requests.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferRequestReceivedDelegate?  OnBinaryDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received for processing.
        /// </summary>
        public event OnBinaryDataTransferDelegate?                 OnBinaryDataTransfer;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_BinaryDataTransfer(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       NetworkingNode_Id     DestinationNodeId,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       Byte[]                BinaryRequest,
                                       CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (BinaryDataTransferRequest.TryParse(BinaryRequest,
                                                       RequestId,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestParser)) {

                    BinaryDataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out errorResponse))
                    {

                        response = new BinaryDataTransferResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature(s): {errorResponse}"
                                                 )
                                   );

                    }

                    #endregion

                    #region Send OnBinaryDataTransferRequest event

                    try
                    {

                        OnBinaryDataTransferRequestReceived?.Invoke(Timestamp.Now,
                                                                    parentNetworkingNode,
                                                                    WebSocketConnection,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(OCPPWebSocketAdapterIN),
                                  nameof(OnBinaryDataTransferRequestReceived),
                                  e
                              );
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnBinaryDataTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            parentNetworkingNode,
                                                                                                                            WebSocketConnection,
                                                                                                                            request,
                                                                                                                            CancellationToken)).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : BinaryDataTransferResponse.Failed(request, $"Undefined {nameof(OnBinaryDataTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = BinaryDataTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnBinaryDataTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= BinaryDataTransferResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: true
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnBinaryDataTransferResponse event

                    await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterOUT).SendOnBinaryDataTransferResponseSent(Timestamp.Now,
                                                                                                                          parentNetworkingNode,
                                                                                                                          WebSocketConnection,
                                                                                                                          request,
                                                                                                                          response,
                                                                                                                          response.Runtime);

                    #endregion

                    ocppResponse = OCPP_Response.BinaryResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToBinary(
                                           parentNetworkingNode.OCPP.CustomBinaryDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_BinaryDataTransfer)[8..],
                                       BinaryRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {
                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_BinaryDataTransfer)[8..],
                                   BinaryRequest,
                                   e
                               );
            }

            return ocppResponse;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP HTTP Web Socket Adapter for outgoing requests.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseSentDelegate?  OnBinaryDataTransferResponseSent;

        #endregion

        #region Send OnBinaryDataTransferResponse event

        public async Task SendOnBinaryDataTransferResponseSent(DateTime                    Timestamp,
                                                               IEventSender                Sender,
                                                               IWebSocketConnection        Connection,
                                                               BinaryDataTransferRequest   Request,
                                                               BinaryDataTransferResponse  Response,
                                                               TimeSpan                    Runtime)
        {

            var logger = OnBinaryDataTransferResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType <OnBinaryDataTransferResponseSentDelegate>().
                                              Select (filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                              Sender,
                                                                                              Connection,
                                                                                              Request,
                                                                                              Response,
                                                                                              Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnBinaryDataTransferResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
