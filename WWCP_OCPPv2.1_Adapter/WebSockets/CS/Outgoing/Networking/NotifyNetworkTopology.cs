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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class NetworkingNodeWSClient : AOCPPWebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeIncomingMessages,
                                                  INetworkingNodeOutgoingMessageEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<OCPP.NN.NotifyNetworkTopologyRequest>?  CustomNotifyNetworkTopologyRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<OCPP.NN.NotifyNetworkTopologyResponse>?     CustomNotifyNetworkTopologyResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OCPP.NN.OnNotifyNetworkTopologyRequestDelegate?     OnNotifyNetworkTopologyRequest;

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                            OnNotifyNetworkTopologyWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event ClientResponseLogHandler?                           OnNotifyNetworkTopologyWSResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OCPP.NN.OnNotifyNetworkTopologyResponseDelegate?    OnNotifyNetworkTopologyResponse;

        #endregion


        #region NotifyNetworkTopology(Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A NotifyNetworkTopology request.</param>
        public async Task<NotifyNetworkTopologyResponse>

            NotifyNetworkTopology(NotifyNetworkTopologyRequest Request)

        {

            #region Send OnNotifyNetworkTopologyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyNetworkTopologyRequest));
            }

            #endregion


            NotifyNetworkTopologyResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.DestinationNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomNotifyNetworkTopologyRequestSerializer,
                                             CustomNetworkTopologyInformationSerializer,
                                             CustomSignatureSerializer,
                                             CustomCustomDataSerializer
                                         )
                                     );

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (NotifyNetworkTopologyResponse.TryParse(Request,
                                                                   sendRequestState.JSONResponse.Payload,
                                                                   out var heartbeatResponse,
                                                                   out var errorResponse,
                                                                   CustomNotifyNetworkTopologyResponseParser) &&
                            heartbeatResponse is not null)
                        {
                            response = heartbeatResponse;
                        }

                        response ??= new NotifyNetworkTopologyResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new NotifyNetworkTopologyResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new NotifyNetworkTopologyResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyNetworkTopologyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyNetworkTopologyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyNetworkTopologyResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
