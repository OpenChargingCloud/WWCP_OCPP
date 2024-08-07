﻿/*
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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel
    {

        #region Custom JSON parser delegates

        //public CustomBinaryParserDelegate<BinaryDataTransferRequest>?       CustomBinaryDataTransferRequestParser         { get; set; }

        //public CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        ///// <summary>
        ///// An event sent whenever a BinaryDataTransfer WebSocket request was received.
        ///// </summary>
        //public event OnOCPPBinaryRequestLogDelegate?                  OnIncomingBinaryDataTransferWSRequest;

        ///// <summary>
        ///// An event sent whenever a BinaryDataTransfer request was received.
        ///// </summary>
        //public event OnBinaryDataTransferRequestReceivedDelegate?     OnBinaryDataTransferRequestReceived;

        ///// <summary>
        ///// An event sent whenever a BinaryDataTransfer request was received.
        ///// </summary>
        //public event OnBinaryDataTransferDelegate?            OnIncomingBinaryDataTransfer;

        ///// <summary>
        ///// An event sent whenever a response to a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnBinaryDataTransferResponseSentDelegate?    OnBinaryDataTransferResponseSent;

        ///// <summary>
        ///// An event sent whenever a WebSocket response to a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnOCPPBinaryRequestBinaryResponseLogDelegate?    OnIncomingBinaryDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        //public async Task<Tuple<OCPP_BinaryResponseMessage?,
        //                        OCPP_JSONRequestErrorMessage?>>

        //    Receive_BinaryDataTransfer(DateTime                   RequestTimestamp,
        //                               WebSocketServerConnection  Connection,
        //                               NetworkingNode_Id          DestinationNodeId,
        //                               NetworkPath                NetworkPath,
        //                               EventTracking_Id           EventTrackingId,
        //                               Request_Id                 RequestId,
        //                               Byte[]                     BinaryRequest,
        //                               CancellationToken          CancellationToken)

        //{

        //    #region Send OnIncomingBinaryDataTransferWSRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnIncomingBinaryDataTransferWSRequest?.Invoke(startTime,
        //                                                      this,
        //                                                      Connection,
        //                                                      DestinationNodeId,
        //                                                      EventTrackingId,
        //                                                      RequestTimestamp,
        //                                                      BinaryRequest,
        //                                                      CancellationToken);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSRequest));
        //    }

        //    #endregion


        //    OCPP_BinaryResponseMessage?  OCPPResponse        = null;
        //    OCPP_JSONRequestErrorMessage?       OCPPErrorResponse   = null;

        //    try
        //    {

        //        if (BinaryDataTransferRequest.TryParse(BinaryRequest,
        //                                               RequestId,
        //                                               DestinationNodeId,
        //                                               NetworkPath,
        //                                               out var request,
        //                                               out var errorResponse,
        //                                               CustomBinaryDataTransferRequestParser)) {

        //            #region Send OnIncomingBinaryDataTransferRequest event

        //            try
        //            {

        //                OnBinaryDataTransferRequestReceived?.Invoke(Timestamp.Now,
        //                                                            this,
        //                                                            Connection,
        //                                                            request);

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBinaryDataTransferRequestReceived));
        //            }

        //            #endregion

        //            #region Call async subscribers

        //            BinaryDataTransferResponse? response = null;

        //            var responseTasks = OnIncomingBinaryDataTransfer?.
        //                                    GetInvocationList()?.
        //                                    SafeSelect(subscriber => (subscriber as OnBinaryDataTransferDelegate)?.Invoke(Timestamp.Now,
        //                                                                                                                          this,
        //                                                                                                                          Connection,
        //                                                                                                                          request,
        //                                                                                                                          CancellationToken)).
        //                                    ToArray();

        //            if (responseTasks?.Length > 0)
        //            {
        //                await Task.WhenAll(responseTasks!);
        //                response = responseTasks.FirstOrDefault()?.Result;
        //            }

        //            response ??= BinaryDataTransferResponse.Failed(request);

        //            #endregion

        //            #region Send OnIncomingBinaryDataTransferResponse event

        //            try
        //            {

        //                OnBinaryDataTransferResponseSent?.Invoke(Timestamp.Now,
        //                                                             this,
        //                                                             Connection,
        //                                                             request,
        //                                                             response,
        //                                                             response.Runtime);

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnBinaryDataTransferResponseSent));
        //            }

        //            #endregion

        //            OCPPResponse  = OCPP_BinaryResponseMessage.From(
        //                                NetworkingMode.Standard,
        //                                NetworkPath.Source,
        //                                NetworkPath.Empty,
        //                                RequestId,
        //                                response.ToBinary(
        //                                    CustomBinaryDataTransferResponseSerializer,
        //                                    null, //CustomCustomDataSerializer,
        //                                    CustomBinarySignatureSerializer,
        //                                    IncludeSignatures: true
        //                                )
        //                            );

        //        }

        //        else
        //            OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
        //                                    RequestId,
        //                                    nameof(Receive_BinaryDataTransfer)[8..],
        //                                    BinaryRequest,
        //                                    errorResponse
        //                                );

        //    }
        //    catch (Exception e)
        //    {

        //        OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
        //                                RequestId,
        //                                nameof(Receive_BinaryDataTransfer)[8..],
        //                                BinaryRequest,
        //                                e
        //                            );

        //    }


        //    #region Send OnIncomingBinaryDataTransferWSResponse event

        //    try
        //    {

        //        var endTime = Timestamp.Now;

        //        OnIncomingBinaryDataTransferWSResponse?.Invoke(endTime,
        //                                                       this,
        //                                                       Connection,
        //                                                       DestinationNodeId,
        //                                                       EventTrackingId,
        //                                                       RequestTimestamp,
        //                                                       BinaryRequest,
        //                                                       endTime, //ToDo: Refactor me!
        //                                                       OCPPResponse?.Payload,
        //                                                       OCPPErrorResponse?.ToJSON(),
        //                                                       endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingBinaryDataTransferWSResponse));
        //    }

        //    #endregion

        //    return new Tuple<OCPP_BinaryResponseMessage?,
        //                     OCPP_JSONRequestErrorMessage?>(OCPPResponse,
        //                                             OCPPErrorResponse);

        //}

        #endregion


    }

}
