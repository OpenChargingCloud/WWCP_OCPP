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

        public CustomJObjectParserDelegate<PublishFirmwareRequest>?       CustomPublishFirmwareRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a PublishFirmware websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnPublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareRequestReceivedDelegate?             OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a PublishFirmware request was received.
        /// </summary>
        public event OnPublishFirmwareDelegate?                    OnPublishFirmware;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OnPublishFirmwareResponseSentDelegate?            OnPublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a PublishFirmware request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnPublishFirmwareWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_PublishFirmware(DateTime                   RequestTimestamp,
                                    WebSocketClientConnection  WebSocketConnection,
                                    NetworkingNode_Id          DestinationNodeId,
                                    NetworkPath                NetworkPath,
                                    EventTracking_Id           EventTrackingId,
                                    Request_Id                 RequestId,
                                    JObject                    RequestJSON,
                                    CancellationToken          CancellationToken)

        {

            #region Send OnPublishFirmwareWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareWSRequest?.Invoke(startTime,
                                                   WebSocketConnection,
                                                   DestinationNodeId,
                                                   NetworkPath,
                                                   EventTrackingId,
                                                   RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (PublishFirmwareRequest.TryParse(RequestJSON,
                                                    RequestId,
                                                    DestinationNodeId,
                                                    NetworkPath,
                                                    out var request,
                                                    out var errorResponse,
                                                    CustomPublishFirmwareRequestParser) && request is not null) {

                    #region Send OnPublishFirmwareRequest event

                    try
                    {

                        OnPublishFirmwareRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         WebSocketConnection,
                                                         request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    PublishFirmwareResponse? response = null;

                    var results = OnPublishFirmware?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnPublishFirmwareDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= PublishFirmwareResponse.Failed(request);

                    #endregion

                    #region Send OnPublishFirmwareResponse event

                    try
                    {

                        OnPublishFirmwareResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          WebSocketConnection,
                                                          request,
                                                          response,
                                                          response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomPublishFirmwareResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_PublishFirmware)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_PublishFirmware)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnPublishFirmwareWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnPublishFirmwareWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
