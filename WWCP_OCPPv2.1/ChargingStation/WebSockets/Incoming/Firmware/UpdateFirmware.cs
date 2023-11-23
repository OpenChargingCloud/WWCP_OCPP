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

    #region OnUpdateFirmware

    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnUpdateFirmwareRequestDelegate(DateTime                Timestamp,
                                        IEventSender            Sender,
                                        UpdateFirmwareRequest   Request);


    /// <summary>
    /// An update firmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<UpdateFirmwareResponse>

        OnUpdateFirmwareDelegate(DateTime                    Timestamp,
                                 IEventSender                Sender,
                                 WebSocketClientConnection   Connection,
                                 UpdateFirmwareRequest       Request,
                                 CancellationToken           CancellationToken);


    /// <summary>
    /// An update firmware response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnUpdateFirmwareResponseDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         UpdateFirmwareRequest    Request,
                                         UpdateFirmwareResponse   Response,
                                         TimeSpan                 Runtime);

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

        public CustomJObjectParserDelegate<UpdateFirmwareRequest>?       CustomUpdateFirmwareRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an update firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?            OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an update firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnUpdateFirmwareWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_UpdateFirmware(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   ChargingStation_Id         chargingStationId,
                                   EventTracking_Id           EventTrackingId,
                                   String                     requestText,
                                   Request_Id                 requestId,
                                   JObject                    requestJSON,
                                   CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnUpdateFirmwareWSRequest event

            try
            {

                OnUpdateFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                  WebSocketConnection,
                                                  chargingStationId,
                                                  EventTrackingId,
                                                  requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
            }

            #endregion

            try
            {

                if (UpdateFirmwareRequest.TryParse(requestJSON,
                                                   requestId,
                                                   ChargingStationIdentity,
                                                   out var request,
                                                   out var errorResponse,
                                                   CustomUpdateFirmwareRequestParser) && request is not null) {

                    #region Send OnUpdateFirmwareRequest event

                    try
                    {

                        OnUpdateFirmwareRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateFirmwareResponse? response = null;

                    var results = OnUpdateFirmware?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateFirmwareDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateFirmwareResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateFirmwareResponse event

                    try
                    {

                        OnUpdateFirmwareResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         request,
                                                         response,
                                                         response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomUpdateFirmwareResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "UpdateFirmware",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "UpdateFirmware",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateFirmwareWSResponse event

            try
            {

                OnUpdateFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                   WebSocketConnection,
                                                   requestJSON,
                                                   OCPPResponse?.Message,
                                                   OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
