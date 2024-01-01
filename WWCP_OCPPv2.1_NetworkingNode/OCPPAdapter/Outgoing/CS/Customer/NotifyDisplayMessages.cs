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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?  CustomNotifyDisplayMessagesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyDisplayMessagesResponse>?     CustomNotifyDisplayMessagesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesRequestSentDelegate?     OnNotifyDisplayMessagesRequestSent;

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event ClientResponseLogHandler?                               OnNotifyDisplayMessagesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseReceivedDelegate?    OnNotifyDisplayMessagesResponseReceived;

        #endregion


        #region NotifyDisplayMessages(Request)

        /// <summary>
        /// Notify about display messages.
        /// </summary>
        /// <param name="Request">A NotifyDisplayMessages request.</param>
        public async Task<NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(NotifyDisplayMessagesRequest  Request)

        {

            #region Send OnNotifyDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesRequestSent?.Invoke(startTime,
                                                       parentNetworkingNode,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyDisplayMessagesRequestSent));
            }

            #endregion


            NotifyDisplayMessagesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyDisplayMessagesRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomMessageInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomMessageContentSerializer,
                                                         parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyDisplayMessagesResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var notifyDisplayMessagesResponse,
                                                               out var errorResponse,
                                                               CustomNotifyDisplayMessagesResponseParser) &&
                        notifyDisplayMessagesResponse is not null)
                    {
                        response = notifyDisplayMessagesResponse;
                    }

                    response ??= new NotifyDisplayMessagesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new NotifyDisplayMessagesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyDisplayMessagesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesResponseReceived?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyDisplayMessagesResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseReceivedDelegate? OnNotifyDisplayMessagesResponseReceived;

    }

}
