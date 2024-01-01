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

        public CustomBinaryParserDelegate<SendFileRequest>?        CustomSendFileRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<SendFileResponse>?  CustomSendFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile websocket request was received.
        /// </summary>
        public event WebSocketBinaryRequestLogHandler?                OnSendFileWSRequest;

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        public event OCPP.CS.OnSendFileRequestDelegate?               OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        public event OCPP.CS.OnSendFileDelegate?                      OnSendFile;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OCPP.CS.OnSendFileResponseDelegate?              OnSendFileResponseSent;

        /// <summary>
        /// An event sent whenever a websocket response to a SendFile request was sent.
        /// </summary>
        public event WebSocketBinaryRequestJSONResponseLogHandler?    OnSendFileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_SendFile(DateTime              RequestTimestamp,
                             IWebSocketConnection  WebSocketConnection,
                             NetworkingNode_Id     DestinationNodeId,
                             NetworkPath           NetworkPath,
                             EventTracking_Id      EventTrackingId,
                             Request_Id            RequestId,
                             Byte[]                RequestBinary,
                             CancellationToken     CancellationToken)

        {

            #region Send OnSendFileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileWSRequest?.Invoke(startTime,
                                            parentNetworkingNode,
                                            WebSocketConnection,
                                            DestinationNodeId,
                                            NetworkPath,
                                            EventTrackingId,
                                            RequestTimestamp,
                                            RequestBinary);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendFileWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (SendFileRequest.TryParse(RequestBinary,
                                             RequestId,
                                             DestinationNodeId,
                                             NetworkPath,
                                             out var request,
                                             out var errorResponse,
                                             CustomSendFileRequestParser) && request is not null) {

                    #region Send OnSendFileRequest event

                    try
                    {

                        OnSendFileRequest?.Invoke(Timestamp.Now,
                                                  parentNetworkingNode,
                                                  WebSocketConnection,
                                                  request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendFileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    SendFileResponse? response = null;

                    var results = OnSendFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnSendFileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= SendFileResponse.Failed(request);

                    #endregion

                    #region Send OnSendFileResponse event

                    try
                    {

                        OnSendFileResponseSent?.Invoke(Timestamp.Now,
                                                   parentNetworkingNode,
                                                   WebSocketConnection,
                                                   request,
                                                   response,
                                                   response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendFileResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomSendFileResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_SendFile)[8..],
                                            RequestBinary,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_SendFile)[8..],
                                        RequestBinary,
                                        e
                                    );
            }

            #region Send OnSendFileWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnSendFileWSResponse?.Invoke(endTime,
                                             parentNetworkingNode,
                                             WebSocketConnection,
                                             DestinationNodeId,
                                             NetworkPath,
                                             EventTrackingId,
                                             RequestTimestamp,
                                             RequestBinary,
                                             OCPPResponse?.Payload,
                                             OCPPErrorResponse?.ToJSON(),
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSendFileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OCPP.CS.OnSendFileResponseDelegate? OnSendFileResponseSent;

    }

}
