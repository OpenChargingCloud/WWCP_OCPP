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

        public CustomJObjectParserDelegate<StatusNotificationRequest>?       CustomStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a StatusNotification WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?               OnStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?          OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationDelegate?                 OnStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?         OnStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a StatusNotification request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?   OnStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_StatusNotification(DateTime                   RequestTimestamp,
                                       WebSocketServerConnection  Connection,
                                       ChargingStation_Id         ChargingStationId,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       JObject                    JSONRequest,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnStatusNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStatusNotificationWSRequest?.Invoke(startTime,
                                                      this,
                                                      Connection,
                                                      ChargingStationId,
                                                      EventTrackingId,
                                                      RequestTimestamp,
                                                      JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (StatusNotificationRequest.TryParse(JSONRequest,
                                                       RequestId,
                                                       ChargingStationId,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomStatusNotificationRequestParser) && request is not null) {

                    #region Send OnStatusNotificationRequest event

                    try
                    {

                        OnStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    StatusNotificationResponse? response = null;

                    var responseTasks = OnStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnStatusNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                          this,
                                                                                                                          request,
                                                                                                                          CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= StatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnStatusNotificationResponse event

                    try
                    {

                        OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_StatusNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_StatusNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnStatusNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnStatusNotificationWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
