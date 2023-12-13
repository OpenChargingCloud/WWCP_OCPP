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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : AChargingStationWSClient,
                                               IChargePointWebSocketClient,
                                               IChargePointServer,
                                               IChargePointClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SendLocalListRequest>?       CustomSendLocalListRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SendLocalListResponse>?  CustomSendLocalListResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendLocalList websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?               OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a SendLocalList request was received.
        /// </summary>
        public event OnSendLocalListDelegate?                      OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?              OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a SendLocalList request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnSendLocalListWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SendLocalList(DateTime                   RequestTimestamp,
                                  WebSocketClientConnection  WebSocketConnection,
                                  NetworkingNode_Id          NetworkingNodeId,
                                  NetworkPath                NetworkPath,
                                  EventTracking_Id           EventTrackingId,
                                  Request_Id                 RequestId,
                                  JObject                    RequestJSON,
                                  CancellationToken          CancellationToken)

        {

            #region Send OnSendLocalListWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListWSRequest?.Invoke(startTime,
                                                 WebSocketConnection,
                                                 NetworkingNodeId,
                                                 NetworkPath,
                                                 EventTrackingId,
                                                 RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SendLocalListRequest.TryParse(RequestJSON,
                                                  RequestId,
                                                  NetworkingNodeId,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomSendLocalListRequestParser) && request is not null) {

                    #region Send OnSendLocalListRequest event

                    try
                    {

                        OnSendLocalListRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SendLocalListResponse? response = null;

                    var results = OnSendLocalList?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSendLocalListDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SendLocalListResponse.Failed(request);

                    #endregion

                    #region Send OnSendLocalListResponse event

                    try
                    {

                        OnSendLocalListResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        request,
                                                        response,
                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomSendLocalListResponseSerializer,
                                           //CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SendLocalList)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SendLocalList)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnSendLocalListWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSendLocalListWSResponse?.Invoke(endTime,
                                                  WebSocketConnection,
                                                  NetworkingNodeId,
                                                  NetworkPath,
                                                  EventTrackingId,
                                                  RequestTimestamp,
                                                  RequestJSON,
                                                  OCPPResponse?.Payload,
                                                  OCPPErrorResponse?.ToJSON(),
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnSendLocalListWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
