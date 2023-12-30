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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
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

        public CustomJObjectParserDelegate<GetBaseReportRequest>?       CustomGetBaseReportRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetBaseReportResponse>?  CustomGetBaseReportResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a get base report websocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                  OnGetBaseReportWSRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetBaseReportDelegate?            OnGetBaseReport;

        /// <summary>
        /// An event sent whenever a response to a get base report request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get base report request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?      OnGetBaseReportWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_GetBaseReport(DateTime                   RequestTimestamp,
                                  IWebSocketConnection  WebSocketConnection,
                                  NetworkingNode_Id          DestinationNodeId,
                                  NetworkPath                NetworkPath,
                                  EventTracking_Id           EventTrackingId,
                                  Request_Id                 RequestId,
                                  JObject                    RequestJSON,
                                  CancellationToken          CancellationToken)

        {

            #region Send OnGetBaseReportWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetBaseReportWSRequest?.Invoke(startTime,
                                           this,
                                           WebSocketConnection,
                                           DestinationNodeId,
                                           NetworkPath,
                                           EventTrackingId,
                                           RequestTimestamp,
                                                 RequestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetBaseReportWSRequest));
            }

            #endregion

            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (GetBaseReportRequest.TryParse(RequestJSON,
                                                  RequestId,
                                                  DestinationNodeId,
                                                  NetworkPath,
                                                  out var request,
                                                  out var errorResponse,
                                                  CustomGetBaseReportRequestParser) && request is not null) {

                    #region Send OnGetBaseReportRequest event

                    try
                    {

                        OnGetBaseReportRequest?.Invoke(Timestamp.Now,
                                                       this,
                                                       WebSocketConnection,
                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetBaseReportRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetBaseReportResponse? response = null;

                    var results = OnGetBaseReport?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetBaseReportDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= GetBaseReportResponse.Failed(request);

                    #endregion

                    #region Send OnGetBaseReportResponse event

                    try
                    {

                        OnGetBaseReportResponse?.Invoke(Timestamp.Now,
                                                        this,
                                                        WebSocketConnection,
                                                        request,
                                                        response,
                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetBaseReportResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetBaseReportResponseSerializer,
                                           parentNetworkingNode.CustomStatusInfoSerializer,
                                           parentNetworkingNode.CustomSignatureSerializer,
                                           parentNetworkingNode.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetBaseReport)[8..],
                                            RequestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetBaseReport)[8..],
                                        RequestJSON,
                                        e
                                    );
            }

            #region Send OnGetBaseReportWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetBaseReportWSResponse?.Invoke(endTime,
                                             this,
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
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetBaseReportWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
