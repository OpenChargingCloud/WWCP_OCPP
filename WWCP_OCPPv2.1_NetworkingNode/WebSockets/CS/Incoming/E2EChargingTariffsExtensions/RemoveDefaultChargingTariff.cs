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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : AOCPPWebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeServer,
                                                  INetworkingNodeClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                                OnRemoveDefaultChargingTariffWSRequest;

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate?     OnRemoveDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffDelegate?            OnRemoveDefaultChargingTariff;

        /// <summary>
        /// An event sent whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate?    OnRemoveDefaultChargingTariffResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                    OnRemoveDefaultChargingTariffWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_RemoveDefaultChargingTariff(DateTime                   RequestTimestamp,
                                                WebSocketClientConnection  WebSocketConnection,
                                                NetworkingNode_Id          DestinationNodeId,
                                                NetworkPath                NetworkPath,
                                                EventTracking_Id           EventTrackingId,
                                                Request_Id                 RequestId,
                                                JObject                    RequestJSON,
                                                CancellationToken          CancellationToken)

        {

            #region Send OnRemoveDefaultChargingTariffWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffWSRequest?.Invoke(startTime,
                                                               WebSocketConnection,
                                                               DestinationNodeId,
                                                               NetworkPath,
                                                               EventTrackingId,
                                                               RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRemoveDefaultChargingTariffWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (RemoveDefaultChargingTariffRequest.TryParse(RequestJSON,
                                                                RequestId,
                                                                DestinationNodeId,
                                                                NetworkPath,
                                                                out var request,
                                                                out var errorResponse,
                                                                CustomRemoveDefaultChargingTariffRequestParser) && request is not null) {

                    #region Send OnRemoveDefaultChargingTariffRequest event

                    try
                    {

                        OnRemoveDefaultChargingTariffRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     WebSocketConnection,
                                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRemoveDefaultChargingTariffRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    RemoveDefaultChargingTariffResponse? response = null;

                    var results = OnRemoveDefaultChargingTariff?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnRemoveDefaultChargingTariffDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= RemoveDefaultChargingTariffResponse.Failed(request);

                    #endregion

                    #region Send OnRemoveDefaultChargingTariffResponse event

                    try
                    {

                        OnRemoveDefaultChargingTariffResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      WebSocketConnection,
                                                                      request,
                                                                      response,
                                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRemoveDefaultChargingTariffResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       NetworkPath.Source,
                                       RequestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_RemoveDefaultChargingTariff)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_RemoveDefaultChargingTariff)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnRemoveDefaultChargingTariffWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnRemoveDefaultChargingTariffWSResponse?.Invoke(endTime,
                                                                WebSocketConnection,
                                                                DestinationNodeId,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnRemoveDefaultChargingTariffWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
