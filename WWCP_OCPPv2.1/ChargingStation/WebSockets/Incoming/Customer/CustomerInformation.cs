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

    #region OnCustomerInformation

    /// <summary>
    /// A CustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnCustomerInformationRequestDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             CustomerInformationRequest   Request);


    /// <summary>
    /// A CustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CustomerInformationResponse>

        OnCustomerInformationDelegate(DateTime                     Timestamp,
                                      IEventSender                 Sender,
                                      WebSocketClientConnection    Connection,
                                      CustomerInformationRequest   Request,
                                      CancellationToken            CancellationToken);


    /// <summary>
    /// A CustomerInformation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCustomerInformationResponseDelegate(DateTime                      Timestamp,
                                              IEventSender                  Sender,
                                              CustomerInformationRequest    Request,
                                              CustomerInformationResponse   Response,
                                              TimeSpan                      Runtime);

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

        public CustomJObjectParserDelegate<CustomerInformationRequest>?       CustomCustomerInformationRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<CustomerInformationResponse>?  CustomCustomerInformationResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a customer information websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnCustomerInformationWSRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?     OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationDelegate?            OnCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a customer information request was sent.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?    OnCustomerInformationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a customer information request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnCustomerInformationWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_CustomerInformation(DateTime                   RequestTimestamp,
                                        WebSocketClientConnection  WebSocketConnection,
                                        ChargingStation_Id         chargingStationId,
                                        EventTracking_Id           EventTrackingId,
                                        String                     requestText,
                                        Request_Id                 requestId,
                                        JObject                    requestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnCustomerInformationWSRequest event

            try
            {

                OnCustomerInformationWSRequest?.Invoke(Timestamp.Now,
                                                       WebSocketConnection,
                                                       chargingStationId,
                                                       EventTrackingId,
                                                       requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage? OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?    OCPPErrorResponse   = null;

            try
            {

                if (CustomerInformationRequest.TryParse(requestJSON,
                                                        requestId,
                                                        ChargingStationIdentity,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomCustomerInformationRequestParser) && request is not null) {

                    #region Send OnCustomerInformationRequest event

                    try
                    {

                        OnCustomerInformationRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    CustomerInformationResponse? response = null;

                    var results = OnCustomerInformation?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnCustomerInformationDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= CustomerInformationResponse.Failed(request);

                    #endregion

                    #region Send OnCustomerInformationResponse event

                    try
                    {

                        OnCustomerInformationResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       requestId,
                                       response.ToJSON(
                                           CustomCustomerInformationResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_CustomerInformation)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_CustomerInformation)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnCustomerInformationWSResponse event

            try
            {

                OnCustomerInformationWSResponse?.Invoke(Timestamp.Now,
                                                        WebSocketConnection,
                                                        requestJSON,
                                                        OCPPResponse?.Payload,
                                                        OCPPErrorResponse?.ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
