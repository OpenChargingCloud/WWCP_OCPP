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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnUsePriorityCharging (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever an UsePriorityCharging request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUsePriorityChargingRequestDelegate(DateTime                      Timestamp,
                                                              IEventSender                  Sender,
                                                              UsePriorityChargingRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an UsePriorityCharging request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUsePriorityChargingResponseDelegate(DateTime                       Timestamp,
                                                               IEventSender                   Sender,
                                                               UsePriorityChargingRequest    Request,
                                                               UsePriorityChargingResponse   Response,
                                                               TimeSpan                       Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?  CustomUsePriorityChargingRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<UsePriorityChargingResponse>?     CustomUsePriorityChargingResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a UsePriorityCharging request was sent.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?     OnUsePriorityChargingRequest;

        /// <summary>
        /// An event sent whenever a response to a UsePriorityCharging request was sent.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?    OnUsePriorityChargingResponse;

        #endregion


        #region UsePriorityCharging(Request)


        public async Task<UsePriorityChargingResponse> UsePriorityCharging(UsePriorityChargingRequest Request)
        {

            #region Send OnUsePriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUsePriorityChargingRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUsePriorityChargingRequest));
            }

            #endregion


            UsePriorityChargingResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomUsePriorityChargingRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (UsePriorityChargingResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var getCompositeScheduleResponse,
                                                             out var errorResponse,
                                                             CustomUsePriorityChargingResponseParser) &&
                        getCompositeScheduleResponse is not null)
                    {
                        response = getCompositeScheduleResponse;
                    }

                    response ??= new UsePriorityChargingResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new UsePriorityChargingResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new UsePriorityChargingResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnUsePriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUsePriorityChargingResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUsePriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
