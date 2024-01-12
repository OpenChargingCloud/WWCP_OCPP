/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<SetNetworkProfileRequest>?       CustomSetNetworkProfileRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a set network profile websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                      OnSetNetworkProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileRequestReceivedDelegate?     OnSetNetworkProfileRequestReceived;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileDelegate?            OnSetNetworkProfile;

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileResponseSentDelegate?    OnSetNetworkProfileResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a set network profile request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?          OnSetNetworkProfileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_SetNetworkProfile(DateTime                   RequestTimestamp,
                                      IWebSocketConnection  WebSocketConnection,
                                      NetworkingNode_Id          DestinationNodeId,
                                      NetworkPath                NetworkPath,
                                      EventTracking_Id           EventTrackingId,
                                      Request_Id                 RequestId,
                                      JObject                    RequestJSON,
                                      CancellationToken          CancellationToken)

        {

            #region Send OnSetNetworkProfileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetNetworkProfileWSRequest?.Invoke(startTime,
                                                     parentNetworkingNode,
                                                     WebSocketConnection,
                                                     DestinationNodeId,
                                                     NetworkPath,
                                                     EventTrackingId,
                                                     RequestTimestamp,
                                                     RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetNetworkProfileWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SetNetworkProfileRequest.TryParse(RequestJSON,
                                                      RequestId,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      out var request,
                                                      out var errorResponse,
                                                      CustomSetNetworkProfileRequestParser)) {

                    #region Send OnSetNetworkProfileRequest event

                    try
                    {

                        OnSetNetworkProfileRequestReceived?.Invoke(Timestamp.Now,
                                                           parentNetworkingNode,
                                                           WebSocketConnection,
                                                           request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetNetworkProfileRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    SetNetworkProfileResponse? response = null;

                    var results = OnSetNetworkProfile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSetNetworkProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                   parentNetworkingNode,
                                                                                                                   WebSocketConnection,
                                                                                                                   request,
                                                                                                                   CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= SetNetworkProfileResponse.Failed(request);

                    #endregion

                    #region Send OnSetNetworkProfileResponse event

                    try
                    {

                        OnSetNetworkProfileResponseSent?.Invoke(Timestamp.Now,
                                                            parentNetworkingNode,
                                                            WebSocketConnection,
                                                            request,
                                                            response,
                                                            response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetNetworkProfileResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomSetNetworkProfileResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SetNetworkProfile)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SetNetworkProfile)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnSetNetworkProfileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSetNetworkProfileWSResponse?.Invoke(endTime,
                                                      parentNetworkingNode,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSetNetworkProfileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileResponseSentDelegate? OnSetNetworkProfileResponseSent;

    }

}
