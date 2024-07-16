/*
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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ReportChargingProfilesRequest>?       CustomReportChargingProfilesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReportChargingProfilesResponse>?  CustomReportChargingProfilesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                            OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesRequestReceivedDelegate?     OnReportChargingProfilesRequestReceived;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesDelegate?            OnReportChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesResponseSentDelegate?    OnReportChargingProfilesResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReportChargingProfiles was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?                OnReportChargingProfilesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_ReportChargingProfiles(DateTime                   RequestTimestamp,
                                           IWebSocketConnection  WebSocketConnection,
                                           NetworkingNode_Id          DestinationId,
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
                                                          parentNetworkingNode,
                                                          WebSocketConnection,
                                                          DestinationId,
                                                          NetworkPath,
                                                          EventTrackingId,
                                                          RequestTimestamp,
                                                          JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReportChargingProfilesWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReportChargingProfilesRequest.TryParse(JSONRequest,
                                                           RequestId,
                                                           DestinationId,
                                                           NetworkPath,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomReportChargingProfilesRequestParser)) {

                    #region Send OnReportChargingProfilesRequest event

                    try
                    {

                        OnReportChargingProfilesRequestReceived?.Invoke(Timestamp.Now,
                                                                parentNetworkingNode,
                                                                WebSocketConnection,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReportChargingProfilesRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    ReportChargingProfilesResponse? response = null;

                    var responseTasks = OnReportChargingProfiles?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnReportChargingProfilesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              parentNetworkingNode,
                                                                                                                              WebSocketConnection,
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
                                                                 parentNetworkingNode,
                                                                 WebSocketConnection,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReportChargingProfilesResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomReportChargingProfilesResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
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
                                                           parentNetworkingNode,
                                                           WebSocketConnection,
                                                           DestinationId,
                                                           NetworkPath,
                                                           EventTrackingId,
                                                           RequestTimestamp,
                                                           JSONRequest,
                                                           OCPPResponse?.Payload,
                                                           OCPPErrorResponse?.ToJSON(),
                                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReportChargingProfilesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesResponseSentDelegate? OnReportChargingProfilesResponseSent;

    }

}
