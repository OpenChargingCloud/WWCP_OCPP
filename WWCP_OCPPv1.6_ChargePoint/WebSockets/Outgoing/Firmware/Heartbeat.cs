﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<HeartbeatRequest>?  CustomHeartbeatRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<HeartbeatResponse>?     CustomHeartbeatResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnHeartbeatWSResponse;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        #endregion


        #region Heartbeat(Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A Heartbeat request.</param>
        public async Task<HeartbeatResponse>

            Heartbeat(HeartbeatRequest Request)

        {

            #region Send OnHeartbeatRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnHeartbeatRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            HeartbeatResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomHeartbeatRequestSerializer,
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

                        if (HeartbeatResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var heartbeatResponse,
                                                       out var errorResponse,
                                                       CustomHeartbeatResponseParser) &&
                            heartbeatResponse is not null)
                        {
                            response = heartbeatResponse;
                        }

                        response ??= new HeartbeatResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new HeartbeatResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new HeartbeatResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new HeartbeatResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
