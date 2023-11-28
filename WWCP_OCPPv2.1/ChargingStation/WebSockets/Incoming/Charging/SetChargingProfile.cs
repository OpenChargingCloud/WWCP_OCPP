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

    #region OnSetChargingProfile

    /// <summary>
    /// A set charging profile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnSetChargingProfileRequestDelegate(DateTime                    Timestamp,
                                            IEventSender                Sender,
                                            SetChargingProfileRequest   Request);


    /// <summary>
    /// A set charging profile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SetChargingProfileResponse>

        OnSetChargingProfileDelegate(DateTime                    Timestamp,
                                     IEventSender                Sender,
                                     WebSocketClientConnection   Connection,
                                     SetChargingProfileRequest   Request,
                                     CancellationToken           CancellationToken);


    /// <summary>
    /// A set charging profile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnSetChargingProfileResponseDelegate(DateTime                     Timestamp,
                                             IEventSender                 Sender,
                                             SetChargingProfileRequest    Request,
                                             SetChargingProfileResponse   Response,
                                             TimeSpan                     Runtime);

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

        public CustomJObjectParserDelegate<SetChargingProfileRequest>?       CustomSetChargingProfileRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetChargingProfileResponse>?  CustomSetChargingProfileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set charging profile websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?               OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate?            OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set charging profile request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?              OnSetChargingProfileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SetChargingProfile(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       ChargingStation_Id         ChargingStationId,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       JObject                    RequestJSON,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnSetChargingProfileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileWSRequest?.Invoke(startTime,
                                                      WebSocketConnection,
                                                      ChargingStationId,
                                                      EventTrackingId,
                                                      RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (SetChargingProfileRequest.TryParse(RequestJSON,
                                                       RequestId,
                                                       ChargingStationIdentity,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomSetChargingProfileRequestParser) && request is not null) {

                    #region Send OnSetChargingProfileRequest event

                    try
                    {

                        OnSetChargingProfileRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SetChargingProfileResponse? response = null;

                    var results = OnSetChargingProfile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetChargingProfileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SetChargingProfileResponse.Failed(request);

                    #endregion

                    #region Send OnSetChargingProfileResponse event

                    try
                    {

                        OnSetChargingProfileResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomSetChargingProfileResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SetChargingProfile)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SetChargingProfile)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnSetChargingProfileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSetChargingProfileWSResponse?.Invoke(endTime,
                                                       WebSocketConnection,
                                                       EventTrackingId,
                                                       RequestTimestamp,
                                                       RequestJSON,
                                                       OCPPResponse?.Payload,
                                                       OCPPErrorResponse?.ToJSON(),
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
