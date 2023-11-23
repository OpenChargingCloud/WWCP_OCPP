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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnSignCertificate

    /// <summary>
    /// A sign certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSignCertificateRequestDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         SignCertificateRequest   Request);


    /// <summary>
    /// A sign certificate at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SignCertificateResponse>

        OnSignCertificateDelegate(DateTime                 Timestamp,
                                  IEventSender             Sender,
                                  SignCertificateRequest   Request,
                                  CancellationToken        CancellationToken);


    /// <summary>
    /// A sign certificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSignCertificateResponseDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          SignCertificateRequest    Request,
                                          SignCertificateResponse   Response,
                                          TimeSpan                  Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SignCertificateRequest>?       CustomSignCertificateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SignCertificateResponse>?  CustomSignCertificateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SignCertificate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?           OnSignCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateRequestDelegate?     OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateDelegate?            OnSignCertificate;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        public event OnSignCertificateResponseDelegate?    OnSignCertificateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a SignCertificate request was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?          OnSignCertificateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_SignCertificate(JArray                     json,
                                    JObject                    requestData,
                                    Request_Id                 requestId,
                                    ChargingStation_Id         chargingStationId,
                                    WebSocketServerConnection  Connection,
                                    String                     OCPPTextMessage,
                                    CancellationToken          CancellationToken)

        {

            #region Send OnSignCertificateWSRequest event

            try
            {

                OnSignCertificateWSRequest?.Invoke(Timestamp.Now,
                                                   this,
                                                   json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SignCertificateRequest.TryParse(requestData,
                                                    requestId,
                                                    chargingStationId,
                                                    out var request,
                                                    out var errorResponse,
                                                    CustomSignCertificateRequestParser) && request is not null) {

                    #region Send OnSignCertificateRequest event

                    try
                    {

                        OnSignCertificateRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SignCertificateResponse? response = null;

                    var responseTasks = OnSignCertificate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnSignCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                       this,
                                                                                                                       request,
                                                                                                                       CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= SignCertificateResponse.Failed(request);

                    #endregion

                    #region Send OnSignCertificateResponse event

                    try
                    {

                        OnSignCertificateResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          request,
                                                          response,
                                                          response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomSignCertificateResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_SignCertificate)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_SignCertificate)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnSignCertificateWSResponse event

            try
            {

                OnSignCertificateWSResponse?.Invoke(Timestamp.Now,
                                                    this,
                                                    json,
                                                    OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSignCertificateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
