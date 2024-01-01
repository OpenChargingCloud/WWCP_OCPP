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
using cloud.charging.open.protocols.OCPP.WebSockets;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<BootNotificationResponse>?     CustomBootNotificationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnBootNotificationRequestSentDelegate?     OnBootNotificationRequestSent;

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                           OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                          OnBootNotificationWSResponse;

        // Should not be here!
        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnBootNotificationResponseReceivedDelegate?    OnBootNotificationResponseReceived;

        #endregion


        #region BootNotification(Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        public async Task<BootNotificationResponse>

            BootNotification(BootNotificationRequest Request)

        {

            #region Send OnBootNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBootNotificationRequestSent?.Invoke(startTime,
                                                  parentNetworkingNode,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBootNotificationRequestSent));
            }

            #endregion


            BootNotificationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomBootNotificationRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomChargingStationSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (BootNotificationResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var bootNotificationResponse,
                                                          out var errorResponse,
                                                          CustomBootNotificationResponseParser) &&
                        bootNotificationResponse is not null)
                    {
                        response = bootNotificationResponse;
                    }

                    response ??= new BootNotificationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new BootNotificationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new BootNotificationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                await parentNetworkingNode.OCPP.IN.RaiseOnBootNotificationResponseIN(endTime,
                                                                                    parentNetworkingNode,
                                                                                    Request,
                                                                                    response,
                                                                                    endTime - startTime);


                OnBootNotificationResponseReceived?.Invoke(endTime,
                                                   parentNetworkingNode,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBootNotificationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnBootNotificationResponseReceivedDelegate? OnBootNotificationResponseReceived;

        public async Task RaiseOnBootNotificationResponseIN(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            BootNotificationRequest    Request,
                                                            BootNotificationResponse   Response,
                                                            TimeSpan                   Runtime)
        {

            var requestLogger = OnBootNotificationResponseReceived;
            if (requestLogger is not null)
            {

                try
                {
                    await Task.WhenAll(
                              requestLogger.GetInvocationList().
                                            OfType <OCPPv2_1.CS.OnBootNotificationResponseReceivedDelegate>().
                                            Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                              Sender,
                                                                                              Request,
                                                                                              Response,
                                                                                              Runtime)).
                                            ToArray()
                          );
                }
                catch (Exception e)
                {
                    await parentNetworkingNode.HandleErrors(
                              nameof(OCPPWebSocketAdapterIN),
                              nameof(OnBootNotificationResponseReceived),
                              e
                          );
                }

            }

        }


    }

}
