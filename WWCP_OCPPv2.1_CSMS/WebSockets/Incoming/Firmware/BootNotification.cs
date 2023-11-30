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

        public CustomJObjectParserDelegate<BootNotificationRequest>?       CustomBootNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<BootNotificationResponse>?  CustomBootNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BootNotification WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?               OnBootNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?            OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a BootNotification was received.
        /// </summary>
        public event OnBootNotificationDelegate?                   OnBootNotification;

        /// <summary>
        /// An event sent whenever a response to a BootNotification was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?           OnBootNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a BootNotification was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?   OnBootNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_BootNotification(DateTime                   RequestTimestamp,
                                     WebSocketServerConnection  Connection,
                                     ChargingStation_Id         ChargingStationId,
                                     EventTracking_Id           EventTrackingId,
                                     Request_Id                 RequestId,
                                     JObject                    JSONRequest,
                                     CancellationToken          CancellationToken)

        {

            #region Send OnBootNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBootNotificationWSRequest?.Invoke(startTime,
                                                    this,
                                                    Connection,
                                                    ChargingStationId,
                                                    EventTrackingId,
                                                    RequestTimestamp,
                                                    JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (BootNotificationRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     ChargingStationId,
                                                     out var request,
                                                     out var errorResponse,
                                                     CustomBootNotificationRequestParser) && request is not null) {

                    #region Send OnBootNotificationRequest event

                    try
                    {

                        OnBootNotificationRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          Connection,
                                                          request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    BootNotificationResponse? response = null;

                    var responseTasks = OnBootNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnBootNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                        this,
                                                                                                                        Connection,
                                                                                                                        request,
                                                                                                                        CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= BootNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnBootNotificationResponse event

                    try
                    {

                        OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           Connection,
                                                           request,
                                                           response,
                                                           response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomBootNotificationResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_BootNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_BootNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnBootNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnBootNotificationWSResponse?.Invoke(endTime,
                                                     this,
                                                     Connection,
                                                     ChargingStationId,
                                                     EventTrackingId,
                                                     RequestTimestamp,
                                                     JSONRequest,
                                                     endTime, //ToDo: Refactor me!
                                                     OCPPResponse?.Payload,
                                                     OCPPErrorResponse?.ToJSON(),
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBootNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
