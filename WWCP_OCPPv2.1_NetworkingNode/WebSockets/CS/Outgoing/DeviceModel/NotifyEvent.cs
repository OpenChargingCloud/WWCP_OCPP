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
                                                  INetworkingNodeOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyEventRequest>?  CustomNotifyEventRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyEventResponse>?     CustomNotifyEventResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEventRequestDelegate?     OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                      OnNotifyEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event ClientResponseLogHandler?                     OnNotifyEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEventResponseDelegate?    OnNotifyEventResponse;

        #endregion


        #region NotifyEvent(Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="Request">A NotifyEvent request.</param>
        public async Task<NotifyEventResponse>

            NotifyEvent(NotifyEventRequest  Request)

        {

            #region Send OnNotifyEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEventRequest?.Invoke(startTime,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            NotifyEventResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.DestinationNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomNotifyEventRequestSerializer,
                                             CustomEventDataSerializer,
                                             CustomComponentSerializer,
                                             CustomEVSESerializer,
                                             CustomVariableSerializer,
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

                        if (NotifyEventResponse.TryParse(Request,
                                                         sendRequestState.JSONResponse.Payload,
                                                         out var notifyEventResponse,
                                                         out var errorResponse,
                                                         CustomNotifyEventResponseParser) &&
                            notifyEventResponse is not null)
                        {
                            response = notifyEventResponse;
                        }

                        response ??= new NotifyEventResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new NotifyEventResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new NotifyEventResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyEventResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEventResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
