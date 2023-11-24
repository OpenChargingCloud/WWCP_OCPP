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

    #region OnGet15118EVCertificate

    /// <summary>
    /// A get 15118 EV certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnGet15118EVCertificateRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               Get15118EVCertificateRequest   Request);


    /// <summary>
    /// A get 15118 EV certificate at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<Get15118EVCertificateResponse>

        OnGet15118EVCertificateDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        Get15118EVCertificateRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A get 15118 EV certificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnGet15118EVCertificateResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                Get15118EVCertificateRequest    Request,
                                                Get15118EVCertificateResponse   Response,
                                                TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<Get15118EVCertificateRequest>?       CustomGet15118EVCertificateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                 OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?     OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event sent whenever a Get15118EVCertificate was received.
        /// </summary>
        public event OnGet15118EVCertificateDelegate?            OnGet15118EVCertificate;

        /// <summary>
        /// An event sent whenever a response to a Get15118EVCertificate was sent.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?    OnGet15118EVCertificateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a Get15118EVCertificate was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                OnGet15118EVCertificateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_Get15118EVCertificate(JArray                     json,
                                          JObject                    requestData,
                                          Request_Id                 requestId,
                                          ChargingStation_Id         chargingStationId,
                                          WebSocketServerConnection  Connection,
                                          String                     OCPPTextMessage,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnGet15118EVCertificateWSRequest event

            try
            {

                OnGet15118EVCertificateWSRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (Get15118EVCertificateRequest.TryParse(requestData,
                                                          requestId,
                                                          chargingStationId,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomGet15118EVCertificateRequestParser) && request is not null) {

                    #region Send OnGet15118EVCertificateRequest event

                    try
                    {

                        OnGet15118EVCertificateRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    Get15118EVCertificateResponse? response = null;

                    var responseTasks = OnGet15118EVCertificate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnGet15118EVCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             this,
                                                                                                                             request,
                                                                                                                             CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= Get15118EVCertificateResponse.Failed(request);

                    #endregion

                    #region Send OnGet15118EVCertificateResponse event

                    try
                    {

                        OnGet15118EVCertificateResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomGet15118EVCertificateResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_Get15118EVCertificate)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_Get15118EVCertificate)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnGet15118EVCertificateWSResponse event

            try
            {

                OnGet15118EVCertificateWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          json,
                                                          OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGet15118EVCertificateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
