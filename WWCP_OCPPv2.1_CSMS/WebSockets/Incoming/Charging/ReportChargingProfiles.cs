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

        public CustomJObjectParserDelegate<ReportChargingProfilesRequest>?       CustomReportChargingProfilesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReportChargingProfilesResponse>?  CustomReportChargingProfilesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesRequestReceivedDelegate?     OnReportChargingProfilesRequestReceived;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        public event OnReportChargingProfilesDelegate?            OnReportChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OnReportChargingProfilesResponseSentDelegate?    OnReportChargingProfilesResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?    OnReportChargingProfilesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_ReportChargingProfiles(DateTime                   RequestTimestamp,
                                           WebSocketServerConnection  Connection,
                                           NetworkingNode_Id          DestinationNodeId,
                                           NetworkPath                NetworkPath,
                                           EventTracking_Id           EventTrackingId,
                                           Request_Id                 RequestId,
                                           JObject                    JSONRequest,
                                           CancellationToken          CancellationToken)

        {

            #region Send OnReportChargingProfilesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReportChargingProfilesRequest.TryParse(JSONRequest,
                                                           RequestId,
                                                           DestinationNodeId,
                                                           NetworkPath,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomReportChargingProfilesRequestParser) && request is not null) {

                    #region Send OnReportChargingProfilesRequest event

                    try
                    {

                        OnReportChargingProfilesRequestReceived?.Invoke(Timestamp.Now,
                                                                this,
                                                                Connection,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    ReportChargingProfilesResponse? response = null;

                    var responseTasks = OnReportChargingProfiles?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnReportChargingProfilesDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ReportChargingProfilesResponse.Failed(request);

                    #endregion

                    #region Send OnReportChargingProfilesResponse event

                    try
                    {

                        OnReportChargingProfilesResponseSent?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 Connection,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath.From(NetworkingNodeId),
                                       RequestId,
                                       response.ToJSON(
                                           CustomReportChargingProfilesResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ReportChargingProfiles)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ReportChargingProfiles)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnReportChargingProfilesWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnReportChargingProfilesWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
