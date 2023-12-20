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

        public CustomJObjectParserDelegate<GetDisplayMessagesRequest>?       CustomGetDisplayMessagesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDisplayMessages websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnGetDisplayMessagesWSRequest;

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?          OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was received.
        /// </summary>
        public event OnGetDisplayMessagesDelegate?                 OnGetDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?         OnGetDisplayMessagesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetDisplayMessages request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnGetDisplayMessagesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetDisplayMessages(DateTime                   RequestTimestamp,
                                       WebSocketClientConnection  WebSocketConnection,
                                       NetworkingNode_Id          DestinationNodeId,
                                       NetworkPath                NetworkPath,
                                       EventTracking_Id           EventTrackingId,
                                       Request_Id                 RequestId,
                                       JObject                    RequestJSON,
                                       CancellationToken          CancellationToken)

        {

            #region Send OnGetDisplayMessagesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesWSRequest?.Invoke(startTime,
                                                      WebSocketConnection,
                                                      DestinationNodeId,
                                                      NetworkPath,
                                                      EventTrackingId,
                                                      RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetDisplayMessagesRequest.TryParse(RequestJSON,
                                                       RequestId,
                                                       DestinationNodeId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       CustomGetDisplayMessagesRequestParser) && request is not null) {

                    #region Send OnGetDisplayMessagesRequest event

                    try
                    {

                        OnGetDisplayMessagesRequest?.Invoke(Timestamp.Now,
                                                            this,
                                                            WebSocketConnection,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetDisplayMessagesResponse? response = null;

                    var results = OnGetDisplayMessages?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetDisplayMessagesDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetDisplayMessagesResponse.Failed(request);

                    #endregion

                    #region Send OnGetDisplayMessagesResponse event

                    try
                    {

                        OnGetDisplayMessagesResponse?.Invoke(Timestamp.Now,
                                                             this,
                                                             WebSocketConnection,
                                                             request,
                                                             response,
                                                             response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetDisplayMessagesResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetDisplayMessages)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetDisplayMessages)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetDisplayMessagesWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetDisplayMessagesWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
