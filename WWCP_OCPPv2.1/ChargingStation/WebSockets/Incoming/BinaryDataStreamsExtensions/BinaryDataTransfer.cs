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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnIncomingBinaryDataTransfer

    /// <summary>
    /// An incoming BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferRequestDelegate(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    CSMS.BinaryDataTransferRequest   Request);


    /// <summary>
    /// An incoming BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<BinaryDataTransferResponse>

        OnIncomingBinaryDataTransferDelegate(DateTime                         Timestamp,
                                             IEventSender                     Sender,
                                             WebSocketClientConnection        Connection,
                                             CSMS.BinaryDataTransferRequest   Request,
                                             CancellationToken                CancellationToken);


    /// <summary>
    /// An incoming BinaryDataTransfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The BinaryDataTransfer request.</param>
    /// <param name="Response">The BinaryDataTransfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingBinaryDataTransferResponseDelegate(DateTime                         Timestamp,
                                                     IEventSender                     Sender,
                                                     CSMS.BinaryDataTransferRequest   Request,
                                                     BinaryDataTransferResponse       Response,
                                                     TimeSpan                         Runtime);

    #endregion


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomBinaryParserDelegate<CSMS.BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                       OnIncomingBinaryDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferRequestDelegate?     OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferDelegate?            OnIncomingBinaryDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnIncomingBinaryDataTransferResponseDelegate?    OnIncomingBinaryDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                      OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_BinaryDataTransfer(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       ChargingStation_Id         chargingStationId,
                                       EventTracking_Id           EventTrackingId,
                                       Byte[]                     RequestText,
                                       Request_Id                 RequestId,
                                       Byte[]                     RequestBinary,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnIncomingBinaryDataTransferWSRequest event

            try
            {

                //OnIncomingBinaryDataTransferWSRequest?.Invoke(Timestamp.Now,
                //                                              this,
                //                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
            }

            #endregion

            OCPP_BinaryResponseMessage?   OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (CSMS.BinaryDataTransferRequest.TryParse(RequestBinary,
                                                            RequestId,
                                                            ChargingStationIdentity,
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
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBinaryDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    BinaryDataTransferResponse? response = null;

                    var results = OnIncomingBinaryDataTransfer?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnIncomingBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= BinaryDataTransferResponse.Failed(request);

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
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingBinaryDataTransferResponse));
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
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_BinaryDataTransfer)[8..],
                                            RequestBinary,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_BinaryDataTransfer)[8..],
                                        RequestBinary,
                                        e
                                    );
            }

            #region Send OnIncomingBinaryDataTransferWSResponse event

            try
            {

                //OnIncomingBinaryDataTransferWSResponse?.Invoke(Timestamp.Now,
                //                                               this,
                //                                               requestJSON,
                //                                               new OCPP_WebSocket_BinaryResponseMessage(requestMessage.RequestId,
                //                                                                                        OCPPResponseJSON ?? new JObject()).ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
