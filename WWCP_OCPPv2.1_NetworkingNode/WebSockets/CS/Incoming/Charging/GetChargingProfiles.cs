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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetChargingProfilesRequest>?       CustomGetChargingProfilesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetChargingProfilesResponse>?  CustomGetChargingProfilesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get charging profiles websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event CS.OnGetChargingProfilesRequestDelegate?     OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event CS.OnGetChargingProfilesDelegate?            OnGetChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a get charging profiles request was sent.
        /// </summary>
        public event CS.OnGetChargingProfilesResponseDelegate?    OnGetChargingProfilesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get charging profiles request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?               OnGetChargingProfilesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetChargingProfiles(DateTime                   RequestTimestamp,
                                        WebSocketClientConnection  WebSocketConnection,
                                        NetworkingNode_Id          NetworkingNodeId,
                                        NetworkPath                NetworkPath,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    RequestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnGetChargingProfilesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetChargingProfilesWSRequest?.Invoke(startTime,
                                                       WebSocketConnection,
                                                       NetworkingNodeId,
                                                       NetworkPath,
                                                       EventTrackingId,
                                                       RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetChargingProfilesWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetChargingProfilesRequest.TryParse(RequestJSON,
                                                        RequestId,
                                                        NetworkingNodeId,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomGetChargingProfilesRequestParser) && request is not null) {

                    #region Send OnGetChargingProfilesRequest event

                    try
                    {

                        OnGetChargingProfilesRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetChargingProfilesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetChargingProfilesResponse? response = null;

                    var results = OnGetChargingProfiles?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetChargingProfilesDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetChargingProfilesResponse.Failed(request);

                    #endregion

                    #region Send OnGetChargingProfilesResponse event

                    try
                    {

                        OnGetChargingProfilesResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetChargingProfilesResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetChargingProfilesResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetChargingProfiles)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetChargingProfiles)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetChargingProfilesWSResponse event

            try
            {
                var endTime = Timestamp.Now;

                OnGetChargingProfilesWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnGetChargingProfilesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
