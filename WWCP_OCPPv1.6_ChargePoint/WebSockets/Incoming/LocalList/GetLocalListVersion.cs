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
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetLocalListVersionRequest>?       CustomGetLocalListVersionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetLocalListVersion websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?         OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate?                OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?        OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetLocalListVersion request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnGetLocalListVersionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetLocalListVersion(DateTime                   RequestTimestamp,
                                        WebSocketClientConnection  WebSocketConnection,
                                        NetworkingNode_Id          DestinationNodeId,
                                        NetworkPath                NetworkPath,
                                        EventTracking_Id           EventTrackingId,
                                        Request_Id                 RequestId,
                                        JObject                    RequestJSON,
                                        CancellationToken          CancellationToken)

        {

            #region Send OnGetLocalListVersionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionWSRequest?.Invoke(startTime,
                                                       WebSocketConnection,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       EventTrackingId,
                                                       RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetLocalListVersionRequest.TryParse(RequestJSON,
                                                        RequestId,
                                                        DestinationNodeId,
                                                        NetworkPath,
                                                        out var request,
                                                        out var errorResponse,
                                                        CustomGetLocalListVersionRequestParser) && request is not null) {

                    #region Send OnGetLocalListVersionRequest event

                    try
                    {

                        OnGetLocalListVersionRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             WebSocketConnection,
                                                             request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetLocalListVersionResponse? response = null;

                    var results = OnGetLocalListVersion?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetLocalListVersionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetLocalListVersionResponse.Failed(request);

                    #endregion

                    #region Send OnGetLocalListVersionResponse event

                    try
                    {

                        OnGetLocalListVersionResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              WebSocketConnection,
                                                              request,
                                                              response,
                                                              response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetLocalListVersionResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetLocalListVersion)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetLocalListVersion)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetLocalListVersionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetLocalListVersionWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnGetLocalListVersionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
