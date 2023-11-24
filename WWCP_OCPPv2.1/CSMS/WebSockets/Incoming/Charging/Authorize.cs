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

    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime          Timestamp,
                                   IEventSender      Sender,
                                   AuthorizeRequest  Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime            Timestamp,
                            IEventSender        Sender,
                            AuthorizeRequest    Request,
                            CancellationToken   CancellationToken);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize response.</param>
    /// <param name="Sender">The sender of the authorize response.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize response.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime            Timestamp,
                                    IEventSender        Sender,
                                    AuthorizeRequest    Request,
                                    AuthorizeResponse   Response,
                                    TimeSpan            Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<AuthorizeRequest>?       CustomAuthorizeRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an Authorize WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?     OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?            OnAuthorize;

        /// <summary>
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an Authorize WebSocket response was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?    OnAuthorizeWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_Authorize(JArray                     json,
                              JObject                    requestData,
                              Request_Id                 requestId,
                              ChargingStation_Id         chargingStationId,
                              WebSocketServerConnection  Connection,
                              String                     OCPPTextMessage,
                              CancellationToken          CancellationToken)

        {

            #region Send OnAuthorizeWSRequest event

            try
            {

                OnAuthorizeWSRequest?.Invoke(Timestamp.Now,
                                             this,
                                             json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (AuthorizeRequest.TryParse(requestData,
                                              requestId,
                                              chargingStationId,
                                              out var request,
                                              out var errorResponse,
                                              CustomAuthorizeRequestParser) && request is not null) {

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                   this,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    AuthorizeResponse? response = null;

                    var responseTasks = OnAuthorize?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
                                                                                                                 this,
                                                                                                                 request,
                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= AuthorizeResponse.Failed(request);

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                    this,
                                                    request,
                                                    response,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomAuthorizeResponseSerializer,
                                           CustomIdTokenInfoSerializer,
                                           CustomIdTokenSerializer,
                                           CustomAdditionalInfoSerializer,
                                           CustomMessageContentSerializer,
                                           CustomTransactionLimitsSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_Authorize)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_Authorize)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnAuthorizeWSResponse event

            try
            {

                OnAuthorizeWSResponse?.Invoke(Timestamp.Now,
                                              this,
                                              json,
                                              OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? new JArray());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAuthorizeWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
