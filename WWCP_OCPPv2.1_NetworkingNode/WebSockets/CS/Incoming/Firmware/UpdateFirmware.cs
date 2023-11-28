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

        public CustomJObjectParserDelegate<UpdateFirmwareRequest>?       CustomUpdateFirmwareRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an update firmware websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?           OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event CS.OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event CS.OnUpdateFirmwareDelegate?            OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event CS.OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an update firmware request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?          OnUpdateFirmwareWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UpdateFirmware(DateTime                   RequestTimestamp,
                                   WebSocketClientConnection  WebSocketConnection,
                                   ChargingStation_Id         ChargingStationId,
                                   EventTracking_Id           EventTrackingId,
                                   Request_Id                 RequestId,
                                   JObject                    RequestJSON,
                                   CancellationToken          CancellationToken)

        {

            #region Send OnUpdateFirmwareWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareWSRequest?.Invoke(startTime,
                                                  WebSocketConnection,
                                                  ChargingStationId,
                                                  EventTrackingId,
                                                  RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (UpdateFirmwareRequest.TryParse(RequestJSON,
                                                   RequestId,
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
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateFirmwareRequest));
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
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomUpdateFirmwareResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UpdateFirmware)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UpdateFirmware)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateFirmwareWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUpdateFirmwareWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
