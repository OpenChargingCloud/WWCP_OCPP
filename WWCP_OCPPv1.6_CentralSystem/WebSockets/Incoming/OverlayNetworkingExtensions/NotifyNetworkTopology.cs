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
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<NotifyNetworkTopologyRequest>?       CustomNotifyNetworkTopologyRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                       OnIncomingNotifyNetworkTopologyWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnIncomingNotifyNetworkTopologyRequestDelegate?     OnIncomingNotifyNetworkTopologyRequest;

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnIncomingNotifyNetworkTopologyDelegate?            OnIncomingNotifyNetworkTopology;

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnIncomingNotifyNetworkTopologyResponseDelegate?    OnIncomingNotifyNetworkTopologyResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?           OnIncomingNotifyNetworkTopologyWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_NotifyNetworkTopology(DateTime                   RequestTimestamp,
                                          WebSocketServerConnection  Connection,
                                          NetworkingNode_Id          DestinationNodeId,
                                          NetworkPath                NetworkPath,
                                          EventTracking_Id           EventTrackingId,
                                          Request_Id                 RequestId,
                                          JObject                    JSONRequest,
                                          CancellationToken          CancellationToken)

        {

            #region Send OnIncomingNotifyNetworkTopologyWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingNotifyNetworkTopologyWSRequest?.Invoke(startTime,
                                                                 this,
                                                                 Connection,
                                                                 DestinationNodeId,
                                                                 EventTrackingId,
                                                                 RequestTimestamp,
                                                                 JSONRequest,
                                                                 CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingNotifyNetworkTopologyWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (NotifyNetworkTopologyRequest.TryParse(JSONRequest,
                                                          RequestId,
                                                          DestinationNodeId,
                                                          NetworkPath,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomNotifyNetworkTopologyRequestParser) && request is not null) {

                    #region Send OnIncomingNotifyNetworkTopologyRequest event

                    try
                    {

                        OnIncomingNotifyNetworkTopologyRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       Connection,
                                                                       request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingNotifyNetworkTopologyRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    NotifyNetworkTopologyResponse? response = null;

                    var responseTasks = OnIncomingNotifyNetworkTopology?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnIncomingNotifyNetworkTopologyDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     Connection,
                                                                                                                                     request,
                                                                                                                                     CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= NotifyNetworkTopologyResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingNotifyNetworkTopologyResponse event

                    try
                    {

                        OnIncomingNotifyNetworkTopologyResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        Connection,
                                                                        request,
                                                                        response,
                                                                        response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingNotifyNetworkTopologyResponse));
                    }

                    #endregion

                    OCPPResponse  = OCPP_JSONResponseMessage.From(
                                        NetworkPath.Source,
                                        NetworkPath.Empty,
                                        RequestId,
                                        response.ToJSON(
                                            CustomNotifyNetworkTopologyResponseSerializer,
                                            CustomSignatureSerializer,
                                            CustomCustomDataSerializer
                                        )
                                    );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_NotifyNetworkTopology)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_NotifyNetworkTopology)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnIncomingNotifyNetworkTopologyWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnIncomingNotifyNetworkTopologyWSResponse?.Invoke(endTime,
                                                                  this,
                                                                  Connection,
                                                                  DestinationNodeId,
                                                                  EventTrackingId,
                                                                  RequestTimestamp,
                                                                  JSONRequest,
                                                                  endTime, //ToDo: Refactor me!
                                                                  OCPPResponse?.Payload,
                                                                  OCPPErrorResponse?.ToJSON(),
                                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnIncomingNotifyNetworkTopologyWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
