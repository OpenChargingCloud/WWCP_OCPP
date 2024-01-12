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
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : AOCPPWebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   ICSIncomingMessages,
                                                   ICSOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?       CustomGetInstalledCertificateIdsRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                   OnGetInstalledCertificateIdsWSRequest;

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestReceivedDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsDelegate?            OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseSentDelegate?    OnGetInstalledCertificateIdsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?       OnGetInstalledCertificateIdsWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetInstalledCertificateIds(DateTime                   RequestTimestamp,
                                               WebSocketClientConnection  WebSocketConnection,
                                               NetworkingNode_Id          DestinationNodeId,
                                               NetworkPath                NetworkPath,
                                               EventTracking_Id           EventTrackingId,
                                               Request_Id                 RequestId,
                                               JObject                    RequestJSON,
                                               CancellationToken          CancellationToken)

        {

            #region Send OnGetInstalledCertificateIdsWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsWSRequest?.Invoke(startTime,
                                                              WebSocketConnection,
                                                              DestinationNodeId,
                                                              NetworkPath,
                                                              EventTrackingId,
                                                              RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetInstalledCertificateIdsRequest.TryParse(RequestJSON,
                                                               RequestId,
                                                               DestinationNodeId,
                                                               NetworkPath,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomGetInstalledCertificateIdsRequestParser) && request is not null) {

                    #region Send OnGetInstalledCertificateIdsRequest event

                    try
                    {

                        OnGetInstalledCertificateIdsRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    WebSocketConnection,
                                                                    request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetInstalledCertificateIdsResponse? response = null;

                    var results = OnGetInstalledCertificateIds?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetInstalledCertificateIdsDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetInstalledCertificateIdsResponse.Failed(request);

                    #endregion

                    #region Send OnGetInstalledCertificateIdsResponse event

                    try
                    {

                        OnGetInstalledCertificateIdsResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     WebSocketConnection,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetInstalledCertificateIdsResponseSerializer,
                                           CustomCertificateHashDataSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetInstalledCertificateIds)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetInstalledCertificateIds)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetInstalledCertificateIdsWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetInstalledCertificateIdsWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
