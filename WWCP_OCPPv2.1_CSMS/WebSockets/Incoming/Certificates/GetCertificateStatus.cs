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

        public CustomJObjectParserDelegate<GetCertificateStatusRequest>?       CustomGetCertificateStatusRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetCertificateStatus WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                     OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusRequestReceivedDelegate?    OnGetCertificateStatusRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus was received.
        /// </summary>
        public event OnGetCertificateStatusDelegate?                   OnGetCertificateStatus;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus was sent.
        /// </summary>
        public event OnGetCertificateStatusResponseSentDelegate?       OnGetCertificateStatusResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a GetCertificateStatus was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?         OnGetCertificateStatusWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetCertificateStatus(DateTime                   RequestTimestamp,
                                         WebSocketServerConnection  Connection,
                                         NetworkingNode_Id          DestinationNodeId,
                                         NetworkPath                NetworkPath,
                                         EventTracking_Id           EventTrackingId,
                                         Request_Id                 RequestId,
                                         JObject                    JSONRequest,
                                         CancellationToken          CancellationToken)

        {

            #region Send OnGetCertificateStatusWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?      OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (GetCertificateStatusRequest.TryParse(JSONRequest,
                                                         RequestId,
                                                         DestinationNodeId,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         CustomGetCertificateStatusRequestParser) && request is not null) {

                    #region Send OnGetCertificateStatusRequestReceived event

                    try
                    {

                        OnGetCertificateStatusRequestReceived?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      Connection,
                                                                      request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    GetCertificateStatusResponse? response = null;

                    var responseTasks = OnGetCertificateStatus?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnGetCertificateStatusDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetCertificateStatusResponse.Failed(request);

                    #endregion

                    #region Send OnGetCertificateStatusResponseSent event

                    try
                    {

                        OnGetCertificateStatusResponseSent?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   Connection,
                                                                   request,
                                                                   response,
                                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath.From(NetworkingNodeId),
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetCertificateStatusResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetCertificateStatus)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetCertificateStatus)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnGetCertificateStatusWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetCertificateStatusWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                            OCPPErrorResponse);

        }

        #endregion


    }

}
