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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON parser delegates

        public CustomBinaryParserDelegate<OCPPv2_1.CSMS.BinaryDataTransferRequest>?     CustomBinaryDataTransferRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<OCPPv2_1.CS.BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer websocket request was received.
        /// </summary>
        public event WSClientBinaryRequestLogHandler?                 OnIncomingBinaryDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event CS.OnIncomingBinaryDataTransferRequestDelegate?     OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event CS.OnIncomingBinaryDataTransferDelegate?            OnIncomingBinaryDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event CS.OnIncomingBinaryDataTransferResponseDelegate?    OnIncomingBinaryDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event WSClientBinaryRequestBinaryResponseLogHandler?   OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_BinaryDataTransfer(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       NetworkingNode_Id          NetworkingNodeId,
                                       NetworkPath                NetworkPath,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       Byte[]                     RequestBinary,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnIncomingBinaryDataTransferWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingBinaryDataTransferWSRequest?.Invoke(startTime,
                                                              WebSocketConnection,
                                                              NetworkingNodeId,
                                                              NetworkPath,
                                                              EventTrackingId,
                                                              RequestBinary);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
            }

            #endregion

            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                if (OCPPv2_1.CSMS.BinaryDataTransferRequest.TryParse(RequestBinary,
                                                                     RequestId,
                                                                     NetworkingNodeId,
                                                                     NetworkPath,
                                                                     out var request,
                                                                     out var errorResponse,
                                                                     CustomBinaryDataTransferRequestParser) &&
                    request is not null) {

                    #region Send OnBinaryDataTransferRequest event

                    try
                    {

                        OnIncomingBinaryDataTransferRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnBinaryDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    OCPPv2_1.CS.BinaryDataTransferResponse? response = null;

                    var results = OnIncomingBinaryDataTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as CS.OnIncomingBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               WebSocketConnection,
                                                                                                                               request,
                                                                                                                               CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= OCPPv2_1.CS.BinaryDataTransferResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingBinaryDataTransferResponse event

                    try
                    {

                        OnIncomingBinaryDataTransferResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingBinaryDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_BinaryResponseMessage(
                                       RequestId,
                                       response.ToBinary(
                                           CustomBinaryDataTransferResponseSerializer,
                                           null, //CustomCustomDataSerializer,
                                           CustomBinarySignatureSerializer,
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
                                                               WebSocketConnection,
                                                               NetworkingNodeId,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
