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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ReservationStatusUpdateRequest>?       CustomReservationStatusUpdateRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateResponse>?  CustomReservationStatusUpdateResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?                             OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event sent whenever a ReservationStatusUpdate was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReservationStatusUpdateDelegate?            OnReservationStatusUpdate;

        /// <summary>
        /// An event sent whenever a response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ReservationStatusUpdate was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?                 OnReservationStatusUpdateWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ReservationStatusUpdate(DateTime                   RequestTimestamp,
                                            IWebSocketConnection  WebSocketConnection,
                                            NetworkingNode_Id          DestinationNodeId,
                                            NetworkPath                NetworkPath,
                                            EventTracking_Id           EventTrackingId,
                                            Request_Id                 RequestId,
                                            JObject                    JSONRequest,
                                            CancellationToken          CancellationToken)

        {

            #region Send OnReservationStatusUpdateWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateWSRequest?.Invoke(startTime,
                                           this,
                                           WebSocketConnection,
                                           DestinationNodeId,
                                           NetworkPath,
                                           EventTrackingId,
                                           RequestTimestamp,
                                                           JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReservationStatusUpdateWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ReservationStatusUpdateRequest.TryParse(JSONRequest,
                                                            RequestId,
                                                            DestinationNodeId,
                                                            NetworkPath,
                                                            out var request,
                                                            out var errorResponse,
                                                            CustomReservationStatusUpdateRequestParser) && request is not null) {

                    #region Send OnReservationStatusUpdateRequest event

                    try
                    {

                        OnReservationStatusUpdateRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 WebSocketConnection,
                                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReservationStatusUpdateRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ReservationStatusUpdateResponse? response = null;

                    var responseTasks = OnReservationStatusUpdate?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnReservationStatusUpdateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               WebSocketConnection,
                                                                                                                               request,
                                                                                                                               CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= ReservationStatusUpdateResponse.Failed(request);

                    #endregion

                    #region Send OnReservationStatusUpdateResponse event

                    try
                    {

                        OnReservationStatusUpdateResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  WebSocketConnection,
                                                                  request,
                                                                  response,
                                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReservationStatusUpdateResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomReservationStatusUpdateResponseSerializer,
                                           parentNetworkingNode.CustomSignatureSerializer,
                                           parentNetworkingNode.CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ReservationStatusUpdate)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ReservationStatusUpdate)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnReservationStatusUpdateWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnReservationStatusUpdateWSResponse?.Invoke(endTime,
                                                         this,
                                                         WebSocketConnection,
                                                         DestinationNodeId,
                                                         NetworkPath,
                                                         EventTrackingId,
                                                         RequestTimestamp,
                                                         JSONRequest,
                                                         OCPPResponse?.Payload,
                                                            OCPPErrorResponse?.ToJSON(),
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnReservationStatusUpdateWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
