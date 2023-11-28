﻿/*
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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnSendFile

    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The SendFile request.</param>
    public delegate Task

        OnSendFileRequestDelegate(DateTime              Timestamp,
                                  IEventSender          Sender,
                                  CSMS.SendFileRequest   Request);


    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The SendFile request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SendFileResponse>

        OnSendFileDelegate(DateTime                    Timestamp,
                           IEventSender                Sender,
                           WebSocketClientConnection   Connection,
                           CSMS.SendFileRequest         Request,
                           CancellationToken           CancellationToken);


    /// <summary>
    /// A SendFile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The SendFile request.</param>
    /// <param name="Response">The SendFile response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSendFileResponseDelegate(DateTime              Timestamp,
                                   IEventSender          Sender,
                                   CSMS.SendFileRequest   Request,
                                   SendFileResponse       Response,
                                   TimeSpan              Runtime);

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

        public CustomBinaryParserDelegate<CSMS.SendFileRequest>?   CustomSendFileRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SendFileResponse>?  CustomSendFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile websocket request was received.
        /// </summary>
        public event WSClientBinaryRequestLogHandler?               OnSendFileWSRequest;

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        public event OnSendFileRequestDelegate?                     OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        public event OnSendFileDelegate?                            OnSendFile;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OnSendFileResponseDelegate?                    OnSendFileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a SendFile request was sent.
        /// </summary>
        public event WSClientBinaryRequestJSONResponseLogHandler?   OnSendFileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SendFile(DateTime                   RequestTimestamp,
                             WebSocketClientConnection  WebSocketConnection,
                             ChargingStation_Id         ChargingStationId,
                             EventTracking_Id           EventTrackingId,
                             Request_Id                 RequestId,
                             Byte[]                     RequestBinary,
                             CancellationToken          CancellationToken)

        {

            #region Send OnSendFileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileWSRequest?.Invoke(startTime,
                                            WebSocketConnection,
                                            ChargingStationId,
                                            EventTrackingId,
                                            RequestBinary);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendFileWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (CSMS.SendFileRequest.TryParse(RequestBinary,
                                                  RequestId,
                                                  ChargingStationIdentity,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomSendFileRequestParser) &&
                    request is not null) {

                    #region Send OnSendFileRequest event

                    try
                    {

                        OnSendFileRequest?.Invoke(Timestamp.Now,
                                                  this,
                                                  request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendFileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SendFileResponse? response = null;

                    var results = OnSendFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSendFileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SendFileResponse.Failed(request);

                    #endregion

                    #region Send OnSendFileResponse event

                    try
                    {

                        OnSendFileResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response,
                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendFileResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomSendFileResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SendFile)[8..],
                                            RequestBinary,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SendFile)[8..],
                                        RequestBinary,
                                        e
                                    );
            }

            #region Send OnSendFileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSendFileWSResponse?.Invoke(endTime,
                                             WebSocketConnection,
                                             EventTrackingId,
                                             RequestTimestamp,
                                             RequestBinary,
                                             OCPPResponse?.Payload,
                                             OCPPErrorResponse?.ToJSON(),
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendFileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
