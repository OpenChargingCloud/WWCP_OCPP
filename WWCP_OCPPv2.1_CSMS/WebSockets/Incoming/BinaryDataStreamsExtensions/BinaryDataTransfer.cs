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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomBinaryParserDelegate<CS.BinaryDataTransferRequest>?    CustomBinaryDataTransferRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer WebSocket request was received.
        /// </summary>
        public event WebSocketBinaryRequestLogHandler?                 OnIncomingBinaryDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferRequestDelegate?      OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        public event OnIncomingBinaryDataTransferDelegate?             OnIncomingBinaryDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnIncomingBinaryDataTransferResponseDelegate?     OnIncomingBinaryDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event WebSocketBinaryRequestBinaryResponseLogHandler?   OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_BinaryDataTransfer(DateTime                   RequestTimestamp,
                                       WebSocketServerConnection  Connection,
                                       ChargingStation_Id         ChargingStationId,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       Byte[]                     BinaryRequest,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnIncomingBinaryDataTransferWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingBinaryDataTransferWSRequest?.Invoke(startTime,
                                                              this,
                                                              Connection,
                                                              ChargingStationId,
                                                              EventTrackingId,
                                                              RequestTimestamp,
                                                              BinaryRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
            }

            #endregion


            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                if (CS.BinaryDataTransferRequest.TryParse(BinaryRequest,
                                                          RequestId,
                                                          ChargingStationId,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomBinaryDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingBinaryDataTransferRequest event

                    try
                    {

                        OnIncomingBinaryDataTransferRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    BinaryDataTransferResponse? response = null;

                    var responseTasks = OnIncomingBinaryDataTransfer?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnIncomingBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                  this,
                                                                                                                                  request,
                                                                                                                                  CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
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
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse  = new OCPP_BinaryResponseMessage(
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
                                            BinaryRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_BinaryDataTransfer)[8..],
                                        BinaryRequest,
                                        e
                                    );

            }


            #region Send OnIncomingBinaryDataTransferWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnIncomingBinaryDataTransferWSResponse?.Invoke(endTime,
                                                               this,
                                                               Connection,
                                                               ChargingStationId,
                                                               EventTrackingId,
                                                               RequestTimestamp,
                                                               BinaryRequest,
                                                               endTime, //ToDo: Refactor me!
                                                               OCPPResponse?.Payload,
                                                               OCPPErrorResponse?.ToJSON(),
                                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
