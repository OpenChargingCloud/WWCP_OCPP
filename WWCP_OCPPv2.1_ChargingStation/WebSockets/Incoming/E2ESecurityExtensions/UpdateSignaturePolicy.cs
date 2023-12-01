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

        public CustomJObjectParserDelegate<UpdateSignaturePolicyRequest>?  CustomUpdateSignaturePolicyRequestParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                  OnUpdateSignaturePolicyWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?     OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyDelegate?            OnUpdateSignaturePolicy;

        /// <summary>
        /// An event sent whenever a response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?    OnUpdateSignaturePolicyResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?                 OnUpdateSignaturePolicyWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_UpdateSignaturePolicy(DateTime                   RequestTimestamp,
                                          WebSocketClientConnection  WebSocketConnection,
                                          NetworkingNode_Id          NetworkingNodeId,
                                          NetworkPath                NetworkPath,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    RequestJSON,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnUpdateSignaturePolicyWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyWSRequest?.Invoke(startTime,
                                                         WebSocketConnection,
                                                         NetworkingNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (UpdateSignaturePolicyRequest.TryParse(RequestJSON,
                                                          RequestId,
                                                          NetworkingNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomUpdateSignaturePolicyRequestParser) && request is not null) {

                    #region Send OnUpdateSignaturePolicyRequest event

                    try
                    {

                        OnUpdateSignaturePolicyRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    UpdateSignaturePolicyResponse? response = null;

                    var results = OnUpdateSignaturePolicy?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnUpdateSignaturePolicyDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= UpdateSignaturePolicyResponse.Failed(request);

                    #endregion

                    #region Send OnUpdateSignaturePolicyResponse event

                    try
                    {

                        OnUpdateSignaturePolicyResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON()
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_UpdateSignaturePolicy)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_UpdateSignaturePolicy)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnUpdateSignaturePolicyWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnUpdateSignaturePolicyWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateSignaturePolicyWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
