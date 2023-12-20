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

        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>?       CustomChangeAvailabilityRequestParser    { get; set; }

        public CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a change availability websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                       OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnChangeAvailabilityDelegate?            OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a change availability request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?           OnChangeAvailabilityWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ChangeAvailability(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       NetworkingNode_Id          DestinationNodeId,
                                       NetworkPath                NetworkPath,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       JObject                    RequestJSON,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnChangeAvailabilityWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityWSRequest?.Invoke(startTime,
                                                      WebSocketConnection,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      EventTrackingId,
                                                      RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnChangeAvailabilityWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ChangeAvailabilityRequest.TryParse(RequestJSON,
                                                       RequestId,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomChangeAvailabilityRequestParser) && request is not null) {

                    #region Send OnChangeAvailabilityRequest event

                    try
                    {

                        OnChangeAvailabilityRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            WebSocketConnection,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnChangeAvailabilityRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ChangeAvailabilityResponse? response = null;

                    var results = OnChangeAvailability?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ChangeAvailabilityResponse.Failed(request);

                    #endregion

                    #region Send OnChangeAvailabilityResponse event

                    try
                    {

                        OnChangeAvailabilityResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             WebSocketConnection,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnChangeAvailabilityResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomChangeAvailabilityResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ChangeAvailability)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ChangeAvailability)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnChangeAvailabilityWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnChangeAvailabilityWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
