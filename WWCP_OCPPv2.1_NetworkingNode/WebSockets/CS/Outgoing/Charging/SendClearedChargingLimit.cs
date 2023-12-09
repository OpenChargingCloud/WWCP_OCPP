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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ClearedChargingLimitResponse>?     CustomClearedChargingLimitResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event CS.OnClearedChargingLimitRequestDelegate?     OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnClearedChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event CS.OnClearedChargingLimitResponseDelegate?    OnClearedChargingLimitResponse;

        #endregion


        #region SendClearedChargingLimit(Request)

        /// <summary>
        /// Inform about a cleared charging limit.
        /// </summary>
        /// <param name="Request">A ClearedChargingLimit request.</param>
        public async Task<ClearedChargingLimitResponse>

            SendClearedChargingLimit(ClearedChargingLimitRequest  Request)

        {

            #region Send OnClearedChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            ClearedChargingLimitResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.NetworkingNodeId,
                                         Request.NetworkPath,
                                         Request.Action,
                                         Request.RequestId,
                                         Request.ToJSON(
                                             CustomClearedChargingLimitRequestSerializer,
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

                        if (ClearedChargingLimitResponse.TryParse(Request,
                                                                  sendRequestState.JSONResponse.Payload,
                                                                  out var clearedChargingLimitResponse,
                                                                  out var errorResponse,
                                                                  CustomClearedChargingLimitResponseParser) &&
                            clearedChargingLimitResponse is not null)
                        {
                            response = clearedChargingLimitResponse;
                        }

                        response ??= new ClearedChargingLimitResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new ClearedChargingLimitResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new ClearedChargingLimitResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new ClearedChargingLimitResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnClearedChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
