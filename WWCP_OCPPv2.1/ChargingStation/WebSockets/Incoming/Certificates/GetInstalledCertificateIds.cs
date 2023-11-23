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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnGetInstalledCertificateIds

    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    GetInstalledCertificateIdsRequest   Request);


    /// <summary>
    /// A get installed certificate ids request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetInstalledCertificateIdsResponse>

        OnGetInstalledCertificateIdsDelegate(DateTime                            Timestamp,
                                             IEventSender                        Sender,
                                             WebSocketClientConnection           Connection,
                                             GetInstalledCertificateIdsRequest   Request,
                                             CancellationToken                   CancellationToken);


    /// <summary>
    /// A get installed certificate ids response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetInstalledCertificateIdsResponseDelegate(DateTime                             Timestamp,
                                                     IEventSender                         Sender,
                                                     GetInstalledCertificateIdsRequest    Request,
                                                     GetInstalledCertificateIdsResponse   Response,
                                                     TimeSpan                             Runtime);

    #endregion


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?       CustomGetInstalledCertificateIdsRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get installed certificate ids websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                       OnGetInstalledCertificateIdsWSRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsDelegate?            OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?    OnGetInstalledCertificateIdsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get installed certificate ids request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                      OnGetInstalledCertificateIdsWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_GetInstalledCertificateIds(DateTime                   RequestTimestamp,
                                               WebSocketClientConnection  WebSocketConnection,
                                               ChargingStation_Id         chargingStationId,
                                               EventTracking_Id           EventTrackingId,
                                               String                     requestText,
                                               Request_Id                 requestId,
                                               JObject                    requestJSON,
                                               CancellationToken          CancellationToken)

        {

            #region Send OnGetInstalledCertificateIdsWSRequest event

            try
            {

                OnGetInstalledCertificateIdsWSRequest?.Invoke(Timestamp.Now,
                                                              WebSocketConnection,
                                                              chargingStationId,
                                                              EventTrackingId,
                                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSRequest));
            }

            #endregion

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (GetInstalledCertificateIdsRequest.TryParse(requestJSON,
                                                               requestId,
                                                               ChargingStationIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomGetInstalledCertificateIdsRequestParser) && request is not null) {

                    #region Send OnGetInstalledCertificateIdsRequest event

                    try
                    {

                        OnGetInstalledCertificateIdsRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetInstalledCertificateIdsResponse? response = null;

                    var results = OnGetInstalledCertificateIds?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetInstalledCertificateIdsDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetInstalledCertificateIdsResponse.Failed(request);

                    #endregion

                    #region Send OnGetInstalledCertificateIdsResponse event

                    try
                    {

                        OnGetInstalledCertificateIdsResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomGetInstalledCertificateIdsResponseSerializer,
                                           CustomCertificateHashDataSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_GetInstalledCertificateIds)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_GetInstalledCertificateIds)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnGetInstalledCertificateIdsWSResponse event

            try
            {

                OnGetInstalledCertificateIdsWSResponse?.Invoke(Timestamp.Now,
                                                               WebSocketConnection,
                                                               requestJSON,
                                                               OCPPResponse?.Message,
                                                               OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
