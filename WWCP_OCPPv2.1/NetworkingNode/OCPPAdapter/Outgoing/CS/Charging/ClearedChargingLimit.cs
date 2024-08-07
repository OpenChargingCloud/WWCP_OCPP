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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ClearedChargingLimitResponse>?     CustomClearedChargingLimitResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearedChargingLimitRequestSentDelegate?     OnClearedChargingLimitRequestSent;

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                               OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                              OnClearedChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnClearedChargingLimitResponseReceivedDelegate?    OnClearedChargingLimitResponseReceived;

        #endregion


        #region SendClearedChargingLimit(Request)

        /// <summary>
        /// Inform about a cleared charging limit.
        /// </summary>
        /// <param name="Request">A ClearedChargingLimit request.</param>
        public async Task<ClearedChargingLimitResponse>

            ClearedChargingLimit(ClearedChargingLimitRequest  Request)

        {

            #region Send OnClearedChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitRequestSent?.Invoke(startTime,
                                                      parentNetworkingNode,
                                                      null,
                                                      Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearedChargingLimitRequestSent));
            }

            #endregion


            ClearedChargingLimitResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomClearedChargingLimitRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

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
                                     Result.FormationViolation(errorResponse)
                                 );

                }

                response ??= new ClearedChargingLimitResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
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

                OnClearedChargingLimitResponseReceived?.Invoke(endTime,
                                                       parentNetworkingNode,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnClearedChargingLimitResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnClearedChargingLimitResponseReceivedDelegate? OnClearedChargingLimitResponseReceived;

    }

}
