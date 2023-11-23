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

    #region OnDeleteCertificate

    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnDeleteCertificateRequestDelegate(DateTime                   Timestamp,
                                           IEventSender               Sender,
                                           DeleteCertificateRequest   Request);


    /// <summary>
    /// A delete certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DeleteCertificateResponse>

        OnDeleteCertificateDelegate(DateTime                    Timestamp,
                                    IEventSender                Sender,
                                    WebSocketClientConnection   Connection,
                                    DeleteCertificateRequest    Request,
                                    CancellationToken           CancellationToken);


    /// <summary>
    /// A delete certificate response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnDeleteCertificateResponseDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            DeleteCertificateRequest    Request,
                                            DeleteCertificateResponse   Response,
                                            TimeSpan                    Runtime);

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

        public CustomJObjectParserDelegate<DeleteCertificateRequest>?       CustomDeleteCertificateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a delete certificate websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnDeleteCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?     OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateDelegate?            OnDeleteCertificate;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?    OnDeleteCertificateResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a delete certificate request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnDeleteCertificateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_DeleteCertificate(DateTime                   RequestTimestamp,
                                      WebSocketClientConnection  WebSocketConnection,
                                      ChargingStation_Id         chargingStationId,
                                      EventTracking_Id           EventTrackingId,
                                      String                     requestText,
                                      Request_Id                 requestId,
                                      JObject                    requestJSON,
                                      CancellationToken          CancellationToken)

        {

            OCPP_WebSocket_ResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            #region Send OnDeleteCertificateWSRequest event

            try
            {

                OnDeleteCertificateWSRequest?.Invoke(Timestamp.Now,
                                                     WebSocketConnection,
                                                     chargingStationId,
                                                     EventTrackingId,
                                                     requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateWSRequest));
            }

            #endregion

            try
            {

                if (DeleteCertificateRequest.TryParse(requestJSON,
                                                      requestId,
                                                      ChargingStationIdentity,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomDeleteCertificateRequestParser) && request is not null) {

                    #region Send OnDeleteCertificateRequest event

                    try
                    {

                        OnDeleteCertificateRequest?.Invoke(Timestamp.Now,
                                                           this,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    DeleteCertificateResponse? response = null;

                    var results = OnDeleteCertificate?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDeleteCertificateDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= DeleteCertificateResponse.Failed(request);

                    #endregion

                    #region Send OnDeleteCertificateResponse event

                    try
                    {

                        OnDeleteCertificateResponse?.Invoke(Timestamp.Now,
                                                            this,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomDeleteCertificateResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            "DeleteCertificate",
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        "DeleteCertificate",
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnDeleteCertificateWSResponse event

            try
            {

                OnDeleteCertificateWSResponse?.Invoke(Timestamp.Now,
                                                      WebSocketConnection,
                                                      requestJSON,
                                                      OCPPResponse?.Message,
                                                      OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateWSResponse));
            }

            #endregion


            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
