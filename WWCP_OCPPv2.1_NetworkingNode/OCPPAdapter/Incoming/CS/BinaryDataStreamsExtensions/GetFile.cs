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
using cloud.charging.open.protocols.OCPP.CSMS;

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

        public CustomJObjectParserDelegate<GetFileRequest>?      CustomGetFileRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<GetFileResponse>?  CustomGetFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetFile websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                  OnGetFileWSRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OCPP.CS.OnGetFileRequestDelegate?                OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OCPP.CS.OnGetFileDelegate?                       OnGetFile;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OCPP.CS.OnGetFileResponseDelegate?               OnGetFileResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a GetFile request was sent.
        /// </summary>
        public event WebSocketJSONRequestBinaryResponseLogHandler?    OnGetFileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_GetFile(DateTime              RequestTimestamp,
                            IWebSocketConnection  WebSocketConnection,
                            NetworkingNode_Id     DestinationNodeId,
                            NetworkPath           NetworkPath,
                            EventTracking_Id      EventTrackingId,
                            Request_Id            RequestId,
                            JObject               RequestJSON,
                            CancellationToken     CancellationToken)

        {

            #region Send OnGetFileWSRequest event

            OCPP_Response? ocppResponse = null;

            var startTime = Timestamp.Now;

            try
            {

                OnGetFileWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetFileWSRequest));
            }

            #endregion

            try
            {

                if (GetFileRequest.TryParse(RequestJSON,
                                            RequestId,
                                            DestinationNodeId,
                                            NetworkPath,
                                            out var request,
                                            out var errorResponse,
                                            CustomGetFileRequestParser)) {

                    #region Send OnGetFileRequest event

                    try
                    {

                        OnGetFileRequest?.Invoke(Timestamp.Now,
                                                 parentNetworkingNode,
                                                 WebSocketConnection,
                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetFileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetFileResponse? response = null;

                    var results = OnGetFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetFileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetFileResponse.Failed(request);

                    #endregion

                    #region Send OnGetFileResponse event

                    try
                    {

                        OnGetFileResponseSent?.Invoke(Timestamp.Now,
                                                  parentNetworkingNode,
                                                  WebSocketConnection,
                                                  request,
                                                  response,
                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetFileResponseSent));
                    }

                    #endregion

                    ocppResponse = OCPP_Response.BinaryResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(DestinationNodeId),
                                       RequestId,
                                       response.ToBinary(
                                           CustomGetFileResponseSerializer,
                                           null, //CustomCustomDataSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_GetFile)[8..],
                                       RequestJSON,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {
                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetFile)[8..],
                                   RequestJSON,
                                   e
                               );
            }

            #region Send OnGetFileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetFileWSResponse?.Invoke(endTime,
                                            parentNetworkingNode,
                                            WebSocketConnection,
                                            DestinationNodeId,
                                            NetworkPath,
                                            EventTrackingId,
                                            RequestTimestamp,
                                            RequestJSON,
                                            ocppResponse.BinaryResponseMessage?.Payload,
                                            ocppResponse.JSONErrorMessage?.     ToJSON(),
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetFileWSResponse));
            }

            #endregion

            return ocppResponse;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OCPP.CS.OnGetFileResponseDelegate? OnGetFileResponseSent;

    }

}
