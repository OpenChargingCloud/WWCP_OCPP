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

    #region OnReportChargingProfiles

    /// <summary>
    /// A report charging profiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnReportChargingProfilesRequestDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                ReportChargingProfilesRequest   Request);


    /// <summary>
    /// A report charging profiles at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReportChargingProfilesResponse>

        OnReportChargingProfilesDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         ReportChargingProfilesRequest   Request,
                                         CancellationToken               CancellationToken);


    /// <summary>
    /// A report charging profiles response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnReportChargingProfilesResponseDelegate(DateTime                         Timestamp,
                                                 IEventSender                     Sender,
                                                 ReportChargingProfilesRequest    Request,
                                                 ReportChargingProfilesResponse   Response,
                                                 TimeSpan                         Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
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
        public event WebSocketRequestLogHandler?                  OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?     OnReportChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a ReportChargingProfiles was received.
        /// </summary>
        public event OnReportChargingProfilesDelegate?            OnReportChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a ReportChargingProfiles was sent.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?    OnReportChargingProfilesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReportChargingProfiles was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                 OnReportChargingProfilesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_ReportChargingProfiles(JArray                     json,
                                           JObject                    requestData,
                                           Request_Id                 requestId,
                                           ChargingStation_Id         chargingStationId,
                                           WebSocketServerConnection  Connection,
                                           String                     OCPPTextMessage,
                                           CancellationToken          CancellationToken)

        {

            #region Send OnReportChargingProfilesWSRequest event

            try
            {

                OnReportChargingProfilesWSRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReportChargingProfilesRequest.TryParse(requestData,
                                                           requestId,
                                                           chargingStationId,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomReportChargingProfilesRequestParser) && request is not null) {

                    #region Send OnReportChargingProfilesRequest event

                    try
                    {

                        OnReportChargingProfilesRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ReportChargingProfilesResponse? response = null;

                    var responseTasks = OnReportChargingProfiles?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnReportChargingProfilesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              this,
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

                        OnReportChargingProfilesResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomReportChargingProfilesResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_ReportChargingProfiles)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_ReportChargingProfiles)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnReportChargingProfilesWSResponse event

            try
            {

                OnReportChargingProfilesWSResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           json,
                                                           OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnReportChargingProfilesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
