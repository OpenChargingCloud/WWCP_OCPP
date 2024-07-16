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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?  CustomNotifyNetworkTopologyRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?     CustomNotifyNetworkTopologyResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnNotifyNetworkTopologyRequestSentDelegate?         OnNotifyNetworkTopologyRequestSent;

        /// <summary>
        /// An event sent whenever a response to a NotifyNetworkTopology request was sent.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseReceivedDelegate?    OnNotifyNetworkTopologyResponseReceived;

        #endregion


        #region NotifyNetworkTopology(Request)

        public async Task<NotifyNetworkTopologyResponse> NotifyNetworkTopology(NotifyNetworkTopologyRequest Request)
        {

            #region Send OnNotifyNetworkTopologyRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyRequestSent?.Invoke(startTime,
                                                           parentNetworkingNode,
                                                           Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyNetworkTopologyRequestSent));
            }

            #endregion


            NotifyNetworkTopologyResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyNetworkTopologyRequestSerializer,
                                                         null, //CustomNotifyNetworkTopologySerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyNetworkTopologyResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var notifyNetworkTopologyResponse,
                                                               out var errorResponse,
                                                               CustomNotifyNetworkTopologyResponseParser) &&
                        notifyNetworkTopologyResponse is not null)
                    {
                        response = notifyNetworkTopologyResponse;
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
            catch (Exception e)
            {

                response = new NotifyNetworkTopologyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyNetworkTopologyResponseReceived event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyResponseReceived?.Invoke(endTime,
                                                                parentNetworkingNode,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyNetworkTopologyResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
