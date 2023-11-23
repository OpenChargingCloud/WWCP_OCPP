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

    #region OnGetCertificateStatus

    /// <summary>
    /// A get certificate status request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnGetCertificateStatusRequestDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              GetCertificateStatusRequest   Request);


    /// <summary>
    /// A get certificate status at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetCertificateStatusResponse>

        OnGetCertificateStatusDelegate(DateTime                      Timestamp,
                                       IEventSender                  Sender,
                                       GetCertificateStatusRequest   Request,
                                       CancellationToken             CancellationToken);


    /// <summary>
    /// A get certificate status response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnGetCertificateStatusResponseDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               GetCertificateStatusRequest    Request,
                                               GetCertificateStatusResponse   Response,
                                               TimeSpan                       Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
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
        public event WebSocketRequestLogHandler?                OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus request was received.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?     OnGetCertificateStatusRequest;

        /// <summary>
        /// An event sent whenever a GetCertificateStatus was received.
        /// </summary>
        public event OnGetCertificateStatusDelegate?            OnGetCertificateStatus;

        /// <summary>
        /// An event sent whenever a response to a GetCertificateStatus was sent.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?    OnGetCertificateStatusResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a GetCertificateStatus was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?               OnGetCertificateStatusWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_GetCertificateStatus(JArray                     json,
                                         JObject                    requestData,
                                         Request_Id                 requestId,
                                         ChargingStation_Id         chargingStationId,
                                         WebSocketServerConnection  Connection,
                                         String                     OCPPTextMessage,
                                         CancellationToken          CancellationToken)

        {

            #region Send OnGetCertificateStatusWSRequest event

            try
            {

                OnGetCertificateStatusWSRequest?.Invoke(Timestamp.Now,
                                                        this,
                                                        json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetCertificateStatusRequest.TryParse(requestData,
                                                         requestId,
                                                         chargingStationId,
                                                         out var request,
                                                         out var errorResponse,
                                                         CustomGetCertificateStatusRequestParser) && request is not null) {

                    #region Send OnGetCertificateStatusRequest event

                    try
                    {

                        OnGetCertificateStatusRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetCertificateStatusResponse? response = null;

                    var responseTasks = OnGetCertificateStatus?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnGetCertificateStatusDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
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

                    #region Send OnGetCertificateStatusResponse event

                    try
                    {

                        OnGetCertificateStatusResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomGetCertificateStatusResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_GetCertificateStatus)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_GetCertificateStatus)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnGetCertificateStatusWSResponse event

            try
            {

                OnGetCertificateStatusWSResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         json,
                                                         OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCertificateStatusWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
