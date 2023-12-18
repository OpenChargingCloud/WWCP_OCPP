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

        public CustomJObjectParserDelegate<GetFileRequest>?      CustomGetFileRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<GetFileResponse>?  CustomGetFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetFile websocket request was received.
        /// </summary>
        public event WSClientJSONRequestLogHandler?                  OnGetFileWSRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OCPP.CS.OnGetFileRequestDelegate?               OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OCPP.CS.OnGetFileDelegate?                      OnGetFile;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OCPP.CS.OnGetFileResponseDelegate?              OnGetFileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetFile request was sent.
        /// </summary>
        public event WSClientJSONRequestBinaryResponseLogHandler?    OnGetFileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetFile(DateTime                   RequestTimestamp,
                            WebSocketClientConnection  WebSocketConnection,
                            NetworkingNode_Id          DestinationNodeId,
                            NetworkPath                NetworkPath,
                            EventTracking_Id           EventTrackingId,
                            Request_Id                 RequestId,
                            JObject                    RequestJSON,
                            CancellationToken          CancellationToken)

        {

            #region Send OnGetFileWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetFileWSRequest?.Invoke(startTime,
                                           WebSocketConnection,
                                           DestinationNodeId,
                                           NetworkPath,
                                           EventTrackingId,
                                           RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileWSRequest));
            }

            #endregion

            OCPP_BinaryResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?       OCPPErrorResponse   = null;

            try
            {

                if (GetFileRequest.TryParse(RequestJSON,
                                            RequestId,
                                            DestinationNodeId,
                                            NetworkPath,
                                            out var request,
                                            out var errorResponse,
                                            CustomGetFileRequestParser) && request is not null) {

                    #region Send OnGetFileRequest event

                    try
                    {

                        OnGetFileRequest?.Invoke(Timestamp.Now,
                                                 this,
                                                 WebSocketConnection,
                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetFileResponse? response = null;

                    var results = OnGetFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetFileDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetFileResponse.Failed(request);

                    #endregion

                    #region Send OnGetFileResponse event

                    try
                    {

                        OnGetFileResponse?.Invoke(Timestamp.Now,
                                                  this,
                                                  WebSocketConnection,
                                                  request,
                                                  response,
                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_BinaryResponseMessage(
                                       NetworkPath.Source,
                                       RequestId,
                                       response.ToBinary(
                                           CustomGetFileResponseSerializer,
                                           null, //CustomCustomDataSerializer,
                                           CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetFile)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
