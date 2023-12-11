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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : ACSMSWSServer,
                                                 ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SecurityEventNotificationRequest>?       CustomSecurityEventNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SecurityEventNotification WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                   OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?     OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationDelegate?            OnSecurityEventNotification;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?    OnSecurityEventNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a SecurityEventNotification request was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?       OnSecurityEventNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SecurityEventNotification(DateTime                   RequestTimestamp,
                                              WebSocketServerConnection  Connection,
                                              NetworkingNode_Id          NetworkingNodeId,
                                              NetworkPath                NetworkPath,
                                              EventTracking_Id           EventTrackingId,
                                              Request_Id                 RequestId,
                                              JObject                    JSONRequest,
                                              CancellationToken          CancellationToken)

        {

            #region Send OnSecurityEventNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationWSRequest?.Invoke(startTime,
                                                             this,
                                                             Connection,
                                                             NetworkingNodeId,
                                                             EventTrackingId,
                                                             RequestTimestamp,
                                                             JSONRequest,
                                                             CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSecurityEventNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SecurityEventNotificationRequest.TryParse(JSONRequest,
                                                              RequestId,
                                                              NetworkingNodeId,
                                                              NetworkPath,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomSecurityEventNotificationRequestParser) && request is not null) {

                    #region Send OnSecurityEventNotificationRequest event

                    try
                    {

                        OnSecurityEventNotificationRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSecurityEventNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SecurityEventNotificationResponse? response = null;

                    var responseTasks = OnSecurityEventNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnSecurityEventNotificationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= SecurityEventNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnSecurityEventNotificationResponse event

                    try
                    {

                        OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSecurityEventNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomSecurityEventNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SecurityEventNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SecurityEventNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnSecurityEventNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSecurityEventNotificationWSResponse?.Invoke(endTime,
                                                              this,
                                                              Connection,
                                                              NetworkingNodeId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSecurityEventNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
