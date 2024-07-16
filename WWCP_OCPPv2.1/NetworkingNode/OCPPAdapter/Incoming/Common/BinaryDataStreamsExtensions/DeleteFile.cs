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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

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

        public CustomJObjectParserDelegate<DeleteFileRequest>?       CustomDeleteFileRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<DeleteFileResponse>?  CustomDeleteFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DeleteFile websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                OnDeleteFileWSRequest;

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        public event OnDeleteFileRequestReceivedDelegate?           OnDeleteFileRequestReceived;

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        public event OnDeleteFileDelegate?                          OnDeleteFile;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        public event OnDeleteFileResponseSentDelegate?              OnDeleteFileResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a DeleteFile request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?    OnDeleteFileWSResponse;

        #endregion

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        public event OnDeleteFileResponseReceivedDelegate? OnDeleteFileResponseReceived;


        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_DeleteFile(DateTime              RequestTimestamp,
                               IWebSocketConnection  WebSocketConnection,
                               NetworkingNode_Id     DestinationId,
                               NetworkPath           NetworkPath,
                               EventTracking_Id      EventTrackingId,
                               Request_Id            RequestId,
                               JObject               RequestJSON,
                               CancellationToken     CancellationToken)

        {

            #region Send OnDeleteFileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteFileWSRequest?.Invoke(startTime,
                                              parentNetworkingNode,
                                              WebSocketConnection,
                                              DestinationId,
                                              NetworkPath,
                                              EventTrackingId,
                                              RequestTimestamp,
                                              RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteFileWSRequest));
            }

            OCPP_Response? ocppResponse   = null;

            #endregion

            try
            {

                if (DeleteFileRequest.TryParse(RequestJSON,
                                               RequestId,
                                               DestinationId,
                                               NetworkPath,
                                               out var request,
                                               out var errorResponse,
                                               CustomDeleteFileRequestParser)) {

                    #region Send OnDeleteFileRequestReceived event

                    try
                    {

                        OnDeleteFileRequestReceived?.Invoke(Timestamp.Now,
                                                            parentNetworkingNode,
                                                            WebSocketConnection,
                                                            request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteFileRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    DeleteFileResponse? response = null;

                    var results = OnDeleteFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnDeleteFileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= DeleteFileResponse.Failed(request);

                    #endregion

                    #region Send OnDeleteFileResponse event

                    try
                    {

                        OnDeleteFileResponseSent?.Invoke(Timestamp.Now,
                                                     parentNetworkingNode,
                                                     WebSocketConnection,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteFileResponseSent));
                    }

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomDeleteFileResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_DeleteFile)[8..],
                                       RequestJSON,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {
                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_DeleteFile)[8..],
                                   RequestJSON,
                                   e
                               );
            }

            #region Send OnDeleteFileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnDeleteFileWSResponse?.Invoke(endTime,
                                               parentNetworkingNode,
                                               WebSocketConnection,
                                               DestinationId,
                                               NetworkPath,
                                               EventTrackingId,
                                               RequestTimestamp,
                                               RequestJSON,
                                               ocppResponse.JSONResponseMessage?.Payload,
                                               ocppResponse.JSONRequestErrorMessage?.   ToJSON(),
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDeleteFileWSResponse));
            }

            #endregion

            return ocppResponse;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        public event OnDeleteFileResponseSentDelegate? OnDeleteFileResponseSent;

    }

}
