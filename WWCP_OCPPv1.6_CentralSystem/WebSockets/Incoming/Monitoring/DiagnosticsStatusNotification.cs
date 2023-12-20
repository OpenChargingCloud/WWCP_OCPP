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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;

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

        public CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?       CustomDiagnosticsStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationResponse>?  CustomDiagnosticsStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                       OnDiagnosticsStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?     OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationDelegate?            OnDiagnosticsStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a DiagnosticsStatusNotification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?    OnDiagnosticsStatusNotificationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a DiagnosticsStatusNotification request was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?           OnDiagnosticsStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_DiagnosticsStatusNotification(DateTime                   RequestTimestamp,
                                                  WebSocketServerConnection  Connection,
                                                  NetworkingNode_Id          DestinationNodeId,
                                                  NetworkPath                NetworkPath,
                                                  EventTracking_Id           EventTrackingId,
                                                  Request_Id                 RequestId,
                                                  JObject                    JSONRequest,
                                                  CancellationToken          CancellationToken)

        {

            #region Send OnDiagnosticsStatusNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDiagnosticsStatusNotificationWSRequest?.Invoke(startTime,
                                                                 this,
                                                                 Connection,
                                                                 DestinationNodeId,
                                                                 EventTrackingId,
                                                                 RequestTimestamp,
                                                                 JSONRequest,
                                                                 CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (DiagnosticsStatusNotificationRequest.TryParse(JSONRequest,
                                                                  RequestId,
                                                                  DestinationNodeId,
                                                                  NetworkPath,
                                                                  out var request,
                                                                  out var errorResponse,
                                                                  CustomDiagnosticsStatusNotificationRequestParser) && request is not null) {

                    #region Send OnDiagnosticsStatusNotificationRequest event

                    try
                    {

                        OnDiagnosticsStatusNotificationRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       Connection,
                                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    DiagnosticsStatusNotificationResponse? response = null;

                    var responseTasks = OnDiagnosticsStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnDiagnosticsStatusNotificationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= DiagnosticsStatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnDiagnosticsStatusNotificationResponse event

                    try
                    {

                        OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        Connection,
                                                                        request,
                                                                        response,
                                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomDiagnosticsStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_DiagnosticsStatusNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_DiagnosticsStatusNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnDiagnosticsStatusNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnDiagnosticsStatusNotificationWSResponse?.Invoke(endTime,
                                                                  this,
                                                                  Connection,
                                                                  DestinationNodeId,
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
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnDiagnosticsStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
