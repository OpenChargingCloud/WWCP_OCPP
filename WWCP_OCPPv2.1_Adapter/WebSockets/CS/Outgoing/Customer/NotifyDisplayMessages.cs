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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

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
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseReceivedDelegate?    OnNotifyDisplayMessagesResponse;

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
                                                       this,
                                                       Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyDisplayMessagesRequestSent));
            }

            #endregion


            NotifyDisplayMessagesResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.DestinationNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomNotifyDisplayMessagesRequestSerializer,
                                             CustomMessageInfoSerializer,
                                             CustomMessageContentSerializer,
                                             CustomComponentSerializer,
                                             CustomEVSESerializer,
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

                response ??= new NotifyDisplayMessagesResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
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

                OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
