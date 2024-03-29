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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?       CustomPublishFirmwareStatusNotificationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationResponse>?  CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                           OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestReceivedDelegate?     OnPublishFirmwareStatusNotificationRequestReceived;

        /// <summary>
        /// An event sent whenever a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationDelegate?            OnPublishFirmwareStatusNotification;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseSentDelegate?    OnPublishFirmwareStatusNotificationResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a PublishFirmwareStatusNotification request was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?               OnPublishFirmwareStatusNotificationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_PublishFirmwareStatusNotification(DateTime                   RequestTimestamp,
                                                      WebSocketServerConnection  Connection,
                                                      NetworkingNode_Id          DestinationNodeId,
                                                      NetworkPath                NetworkPath,
                                                      EventTracking_Id           EventTrackingId,
                                                      Request_Id                 RequestId,
                                                      JObject                    JSONRequest,
                                                      CancellationToken          CancellationToken)

        {

            #region Send OnPublishFirmwareStatusNotificationWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (PublishFirmwareStatusNotificationRequest.TryParse(JSONRequest,
                                                                      RequestId,
                                                                      DestinationNodeId,
                                                                      NetworkPath,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomPublishFirmwareStatusNotificationRequestParser) && request is not null) {

                    #region Send OnPublishFirmwareStatusNotificationRequest event

                    try
                    {

                        OnPublishFirmwareStatusNotificationRequestReceived?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           Connection,
                                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    PublishFirmwareStatusNotificationResponse? response = null;

                    var responseTasks = OnPublishFirmwareStatusNotification?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnPublishFirmwareStatusNotificationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= PublishFirmwareStatusNotificationResponse.Failed(request);

                    #endregion

                    #region Send OnPublishFirmwareStatusNotificationResponse event

                    try
                    {

                        OnPublishFirmwareStatusNotificationResponseSent?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            Connection,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath.From(NetworkingNodeId),
                                       RequestId,
                                       response.ToJSON(
                                           CustomPublishFirmwareStatusNotificationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_PublishFirmwareStatusNotification)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_PublishFirmwareStatusNotification)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnPublishFirmwareStatusNotificationWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnPublishFirmwareStatusNotificationWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareStatusNotificationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
