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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnUnpublishFirmware

    /// <summary>
    /// An unpublish firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUnpublishFirmwareRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           UnpublishFirmwareRequest   Request);


    /// <summary>
    /// An unpublish firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UnpublishFirmwareResponse>

        OnUnpublishFirmwareDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    UnpublishFirmwareRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// An unpublish firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUnpublishFirmwareResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            UnpublishFirmwareRequest    Request,
                                            UnpublishFirmwareResponse   Response,
                                            TimeSpan                    Runtime);

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

        public CustomJObjectParserDelegate<UnpublishFirmwareRequest>?       CustomUnpublishFirmwareRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an unpublish firmware websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?              OnUnpublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?     OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareDelegate?            OnUnpublishFirmware;

        /// <summary>
        /// An event sent whenever a response to an unpublish firmware request was sent.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?    OnUnpublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an unpublish firmware request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?             OnUnpublishFirmwareWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UnpublishFirmware(DateTime                   RequestTimestamp,
                                      WebSocketClientConnection  WebSocketConnection,
                                      ChargingStation_Id         ChargingStationId,
                                      EventTracking_Id           EventTrackingId,
                                      Request_Id                 RequestId,
                                      JObject                    RequestJSON,
                                      CancellationToken          CancellationToken)

        {

            #region Send OnUnpublishFirmwareWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnpublishFirmwareWSRequest?.Invoke(startTime,
                                                     WebSocketConnection,
                                                     ChargingStationId,
                                                     EventTrackingId,
                                                     RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (UnpublishFirmwareRequest.TryParse(RequestJSON,
                                                      RequestId,
                                                      ChargingStationIdentity,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomUnpublishFirmwareRequestParser) && request is not null) {

                    #region Send OnUnpublishFirmwareRequest event

                    try
                    {

                        OnUnpublishFirmwareRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UnpublishFirmwareResponse? response = null;

                    var results = OnUnpublishFirmware?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUnpublishFirmwareDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UnpublishFirmwareResponse.Failed(request);

                    #endregion

                    #region Send OnUnpublishFirmwareResponse event

                    try
                    {

                        OnUnpublishFirmwareResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomUnpublishFirmwareResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UnpublishFirmware)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UnpublishFirmware)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUnpublishFirmwareWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUnpublishFirmwareWSResponse?.Invoke(endTime,
                                                      WebSocketConnection,
                                                      EventTrackingId,
                                                      RequestTimestamp,
                                                      RequestJSON,
                                                      OCPPResponse?.Payload,
                                                      OCPPErrorResponse?.ToJSON(),
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}