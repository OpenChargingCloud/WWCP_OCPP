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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event OnBootNotificationRequestSentDelegate? OnBootNotificationRequestSent;

        #endregion

        #region BootNotification(Request)

        /// <summary>
        /// Send a BootNotification request.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        public async Task<BootNotificationResponse> BootNotification(BootNotificationRequest Request)
        {

            BootNotificationResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                            parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {
                    response = new BootNotificationResponse(
                                   Request,
                                   Result.SignatureError(signingErrors)
                               );
                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomBootNotificationRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     async sendMessageResult => {

                                                         #region Send OnBootNotificationRequestSent event

                                                         var logger = OnBootNotificationRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                         OfType<OnBootNotificationRequestSentDelegate>().
                                                                                         Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                                                       Timestamp.Now,
                                                                                                                       parentNetworkingNode,
                                                                                                                       Request,
                                                                                                                       sendMessageResult
                                                                                                                   )).
                                                                                         ToArray());

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBootNotificationRequestSent));
                                                             }
                                                         }

                                                         #endregion

                                                     }

                                                 );

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                    {

                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_BootNotificationResponse(
                                             Request,
                                             jsonResponse,
                                             null,
                                             sendRequestState.DestinationId,
                                             sendRequestState.NetworkPath,
                                             Request.         EventTrackingId,
                                             Request.         RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.         CancellationToken
                                         );

                    }

                    response ??= new BootNotificationResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = new BootNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event fired whenever a BootNotification response was received.
        /// </summary>
        public event OnBootNotificationResponseReceivedDelegate? OnBootNotificationResponseReceived;

        #endregion

        #region Receive BootNotification response (wired via reflection!)

        public async Task<BootNotificationResponse>

            Receive_BootNotificationResponse(BootNotificationRequest  Request,
                                             JObject                  ResponseJSON,
                                             IWebSocketConnection     WebSocketConnection,
                                             NetworkingNode_Id        DestinationId,
                                             NetworkPath              NetworkPath,
                                             EventTracking_Id         EventTrackingId,
                                             Request_Id               RequestId,
                                             DateTime?                ResponseTimestamp   = null,
                                             CancellationToken        CancellationToken   = default)

        {

            var response = BootNotificationResponse.Failed(Request);

            try
            {

                if (BootNotificationResponse.TryParse(Request,
                                                      ResponseJSON,
                                                      DestinationId,
                                                      NetworkPath,
                                                      out response,
                                                      out var errorResponse,
                                                      ResponseTimestamp,
                                                      parentNetworkingNode.OCPP.CustomBootNotificationResponseParser,
                                                      parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                                      parentNetworkingNode.OCPP.CustomSignatureParser,
                                                      parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse
                    );

                    #region Send OnBootNotificationResponseReceived event

                    var logger = OnBootNotificationResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                      OfType <OnBootNotificationResponseReceivedDelegate>().
                                                      Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                      Timestamp.Now,
                                                                                      parentNetworkingNode,
                                                                                      //    WebSocketConnection,
                                                                                      Request,
                                                                                      response,
                                                                                      response.Runtime
                                                                                  )).
                                                      ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnBootNotificationResponseReceived));
                        }
                    }

                    #endregion

                }

                else
                    response = new BootNotificationResponse(
                                   Request,
                                   Result.Format(errorResponse)
                               );

            }
            catch (Exception e)
            {

                response = new BootNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion

    }

}
