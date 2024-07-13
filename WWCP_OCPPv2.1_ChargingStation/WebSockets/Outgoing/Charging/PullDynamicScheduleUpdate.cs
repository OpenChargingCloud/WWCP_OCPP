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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : AOCPPWebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   ICSIncomingMessages,
                                                   ICSOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?  CustomPullDynamicScheduleUpdateRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<PullDynamicScheduleUpdateResponse>?     CustomPullDynamicScheduleUpdateResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestSentDelegate?     OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnPullDynamicScheduleUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnPullDynamicScheduleUpdateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseReceivedDelegate?    OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region PullDynamicScheduleUpdate(Request)

        /// <summary>
        /// Pull a dynamic schedule update.
        /// </summary>
        /// <param name="Request">A PullDynamicScheduleUpdate request.</param>
        public async Task<PullDynamicScheduleUpdateResponse>

            PullDynamicScheduleUpdate(PullDynamicScheduleUpdateRequest  Request)

        {

            #region Send OnPullDynamicScheduleUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
            }

            #endregion


            PullDynamicScheduleUpdateResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomPullDynamicScheduleUpdateRequestSerializer,
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

                        if (PullDynamicScheduleUpdateResponse.TryParse(Request,
                                                                       sendRequestState.JSONResponse.Payload,
                                                                       out var reportChargingProfilesResponse,
                                                                       out var errorResponse,
                                                                       CustomPullDynamicScheduleUpdateResponseParser) &&
                            reportChargingProfilesResponse is not null)
                        {
                            response = reportChargingProfilesResponse;
                        }

                        response ??= new PullDynamicScheduleUpdateResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new PullDynamicScheduleUpdateResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new PullDynamicScheduleUpdateResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new PullDynamicScheduleUpdateResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnPullDynamicScheduleUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
