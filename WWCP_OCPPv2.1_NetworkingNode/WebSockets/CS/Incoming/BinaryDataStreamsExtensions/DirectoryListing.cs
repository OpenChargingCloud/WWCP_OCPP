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
using cloud.charging.open.protocols.OCPP.CSMS;
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
                                                  INetworkingNodeIncomingMessages,
                                                  INetworkingNodeOutgoingMessagesEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ListDirectoryRequest>?       CustomListDirectoryRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ListDirectoryResponse>?  CustomListDirectoryResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ListDirectory websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                OnListDirectoryWSRequest;

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        public event OCPP.CS.OnListDirectoryRequestDelegate?       OnListDirectoryRequest;

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        public event OCPP.CS.OnListDirectoryDelegate?              OnListDirectory;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was sent.
        /// </summary>
        public event OCPP.CS.OnListDirectoryResponseDelegate?      OnListDirectoryResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a ListDirectory request was sent.
        /// </summary>
        public event WSClientJSONRequestJSONResponseLogHandler?    OnListDirectoryWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ListDirectory(DateTime                   RequestTimestamp,
                                  WebSocketClientConnection  WebSocketConnection,
                                  NetworkingNode_Id          DestinationNodeId,
                                  NetworkPath                NetworkPath,
                                  EventTracking_Id           EventTrackingId,
                                  Request_Id                 RequestId,
                                  JObject                    RequestJSON,
                                  CancellationToken          CancellationToken)

        {

            #region Send OnListDirectoryWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnListDirectoryWSRequest?.Invoke(startTime,
                                                 WebSocketConnection,
                                                 DestinationNodeId,
                                                 NetworkPath,
                                                 EventTrackingId,
                                                 RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnListDirectoryWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ListDirectoryRequest.TryParse(RequestJSON,
                                                  RequestId,
                                                  DestinationNodeId,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomListDirectoryRequestParser) && request is not null) {

                    #region Send OnListDirectoryRequest event

                    try
                    {

                        OnListDirectoryRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       WebSocketConnection,
                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnListDirectoryRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ListDirectoryResponse? response = null;

                    var results = OnListDirectory?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnListDirectoryDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ListDirectoryResponse.Failed(request);

                    #endregion

                    #region Send OnListDirectoryResponse event

                    try
                    {

                        OnListDirectoryResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        WebSocketConnection,
                                                        request,
                                                        response,
                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnListDirectoryResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomListDirectoryResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ListDirectory)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ListDirectory)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnListDirectoryWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnListDirectoryWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnListDirectoryWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
