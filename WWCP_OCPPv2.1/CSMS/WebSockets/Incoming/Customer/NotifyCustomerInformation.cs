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

    #region OnNotifyCustomerInformation

    /// <summary>
    /// A notify customer information request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnNotifyCustomerInformationRequestDelegate(DateTime                           Timestamp,
                                                   IEventSender                       Sender,
                                                   NotifyCustomerInformationRequest   Request);


    /// <summary>
    /// A notify customer information at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<NotifyCustomerInformationResponse>

        OnNotifyCustomerInformationDelegate(DateTime                           Timestamp,
                                            IEventSender                       Sender,
                                            NotifyCustomerInformationRequest   Request,
                                            CancellationToken                  CancellationToken);


    /// <summary>
    /// A notify customer information response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnNotifyCustomerInformationResponseDelegate(DateTime                            Timestamp,
                                                    IEventSender                        Sender,
                                                    NotifyCustomerInformationRequest    Request,
                                                    NotifyCustomerInformationResponse   Response,
                                                    TimeSpan                            Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyCustomerInformationRequest>?       CustomNotifyCustomerInformationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation WebSocket request was received.
        /// </summary>
        public event WebSocketRequestLogHandler?                     OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation request was received.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?     OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a NotifyCustomerInformation was received.
        /// </summary>
        public event OnNotifyCustomerInformationDelegate?            OnNotifyCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a NotifyCustomerInformation was sent.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?    OnNotifyCustomerInformationResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyCustomerInformation was sent.
        /// </summary>
        public event WebSocketResponseLogHandler?                    OnNotifyCustomerInformationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_WebSocket_ResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_NotifyCustomerInformation(JArray                     json,
                                              JObject                    requestData,
                                              Request_Id                 requestId,
                                              ChargingStation_Id         chargingStationId,
                                              WebSocketServerConnection  Connection,
                                              String                     OCPPTextMessage,
                                              CancellationToken          CancellationToken)

        {

            #region Send OnNotifyCustomerInformationWSRequest event

            try
            {

                OnNotifyCustomerInformationWSRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             json);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationWSRequest));
            }

            #endregion


            OCPP_WebSocket_ResponseMessage?  OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyCustomerInformationRequest.TryParse(requestData,
                                                              requestId,
                                                              chargingStationId,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomNotifyCustomerInformationRequestParser) && request is not null) {

                    #region Send OnNotifyCustomerInformationRequest event

                    try
                    {

                        OnNotifyCustomerInformationRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyCustomerInformationResponse? response = null;

                    var responseTasks = OnNotifyCustomerInformation?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnNotifyCustomerInformationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyCustomerInformationResponse.Failed(request);

                    #endregion

                    #region Send OnNotifyCustomerInformationResponse event

                    try
                    {

                        OnNotifyCustomerInformationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_WebSocket_ResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomNotifyCustomerInformationResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_NotifyCustomerInformation)[8..],
                                            requestData,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_NotifyCustomerInformation)[8..],
                                        requestData,
                                        e
                                    );

            }


            #region Send OnNotifyCustomerInformationWSResponse event

            try
            {

                OnNotifyCustomerInformationWSResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              json,
                                                              OCPPResponse?.ToJSON() ?? OCPPErrorResponse?.ToJSON() ?? []);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyCustomerInformationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_WebSocket_ResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
