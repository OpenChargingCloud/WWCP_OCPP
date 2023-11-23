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

    #region OnCostUpdated (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a cost updated request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnCostUpdatedRequestDelegate(DateTime             Timestamp,
                                                      IEventSender         Sender,
                                                      CostUpdatedRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a cost updated request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnCostUpdatedResponseDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       CostUpdatedRequest    Request,
                                                       CostUpdatedResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<CostUpdatedRequest>?  CustomCostUpdatedRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a CostUpdated request was sent.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?     OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a response to a CostUpdated request was sent.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?    OnCostUpdatedResponse;

        #endregion


        #region SendCostUpdated            (Request)

        public async Task<CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request)
        {

            #region Send OnCostUpdatedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCostUpdatedRequest?.Invoke(startTime,
                                             this,
                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCostUpdatedRequest));
            }

            #endregion


            CostUpdatedResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomCostUpdatedRequestSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CostUpdatedResponse.TryParse(Request,
                                                 sendRequestState.Response,
                                                 out var costUpdatedResponse,
                                                 out var errorResponse) &&
                    costUpdatedResponse is not null)
                {
                    response = costUpdatedResponse;
                }

                response ??= new CostUpdatedResponse(Request,
                                                     Result.Format(errorResponse));

            }

            response ??= new CostUpdatedResponse(Request,
                                                 Result.FromSendRequestState(sendRequestState));


            #region Send OnCostUpdatedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCostUpdatedResponse?.Invoke(endTime,
                                              this,
                                              Request,
                                              response,
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnCostUpdatedResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
