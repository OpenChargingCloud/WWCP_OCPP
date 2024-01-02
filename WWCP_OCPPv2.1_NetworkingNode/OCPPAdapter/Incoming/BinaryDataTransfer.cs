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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomBinaryParserDelegate<BinaryDataTransferRequest>?       CustomBinaryDataTransferRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer websocket request was received.
        /// </summary>
        public event WebSocketBinaryRequestLogHandler?                  OnIncomingBinaryDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferRequestDelegate?       OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferDelegate?              OnIncomingBinaryDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnIncomingBinaryDataTransferResponseDelegate?      OnIncomingBinaryDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event WebSocketBinaryRequestBinaryResponseLogHandler?    OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_BinaryDataTransfer(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       NetworkingNode_Id     DestinationNodeId,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       Byte[]                RequestBinary,
                                       CancellationToken     CancellationToken)

        {

            #region Send OnIncomingBinaryDataTransferWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingBinaryDataTransferWSRequest?.Invoke(startTime,
                                                              parentNetworkingNode,
                                                              WebSocketConnection,
                                                              DestinationNodeId,
                                                              NetworkPath,
                                                              EventTrackingId,
                                                              RequestTimestamp,
                                                              RequestBinary);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
            }

            #endregion

            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                if (BinaryDataTransferRequest.TryParse(RequestBinary,
                                                       RequestId,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomBinaryDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingBinaryDataTransferRequest event

                    try
                    {

                        OnIncomingBinaryDataTransferRequest?.Invoke(Timestamp.Now,
                                                                    parentNetworkingNode,
                                                                    WebSocketConnection,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnIncomingBinaryDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    BinaryDataTransferResponse? response = null;

                    var results = OnIncomingBinaryDataTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnIncomingBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            parentNetworkingNode,
                                                                                                                            WebSocketConnection,
                                                                                                                            request,
                                                                                                                            CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= BinaryDataTransferResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingBinaryDataTransferResponse event

                    try
                    {

                        OnIncomingBinaryDataTransferResponse?.Invoke(Timestamp.Now,
                                                                     parentNetworkingNode,
                                                                     WebSocketConnection,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnIncomingBinaryDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse  = OCPP_BinaryResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath.Empty,
                                       RequestId,
                                       response.ToBinary(
                                           CustomBinaryDataTransferResponseSerializer,
                                           null, //CustomCustomDataSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_BinaryDataTransfer)[8..],
                                            RequestBinary,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_BinaryDataTransfer)[8..],
                                        RequestBinary,
                                        e
                                    );
            }

            #region Send OnIncomingBinaryDataTransferWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnIncomingBinaryDataTransferWSResponse?.Invoke(endTime,
                                                               parentNetworkingNode,
                                                               WebSocketConnection,
                                                               DestinationNodeId,
                                                               NetworkPath,
                                                               EventTrackingId,
                                                               RequestTimestamp,
                                                               RequestBinary,
                                                               OCPPResponse?.Payload,
                                                               OCPPErrorResponse?.ToJSON(),
                                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
