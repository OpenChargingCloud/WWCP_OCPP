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

    #region OnNotifyAllowedEnergyTransfer (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a notify allowed energy transfer request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyAllowedEnergyTransferRequestDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      NotifyAllowedEnergyTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify allowed energy transfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyAllowedEnergyTransferResponseDelegate(DateTime                              Timestamp,
                                                                       IEventSender                          Sender,
                                                                       NotifyAllowedEnergyTransferRequest    Request,
                                                                       NotifyAllowedEnergyTransferResponse   Response,
                                                                       TimeSpan                              Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyAllowedEnergyTransferResponse>?     CustomNotifyAllowedEnergyTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?    OnNotifyAllowedEnergyTransferResponse;

        #endregion


        #region NotifyAllowedEnergyTransfer(Request)


        public async Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request)
        {

            #region Send OnNotifyAllowedEnergyTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyAllowedEnergyTransferRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyAllowedEnergyTransferRequest));
            }

            #endregion


            NotifyAllowedEnergyTransferResponse? response = null;

            var sendRequestState = await SendJSONAndWait(
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             Request.ChargingStationId,
                                             Request.Action,
                                             Request.ToJSON(
                                                 CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                 CustomSignatureSerializer,
                                                 CustomCustomDataSerializer
                                             ),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (NotifyAllowedEnergyTransferResponse.TryParse(Request,
                                                                 sendRequestState.Response,
                                                                 out var getCompositeScheduleResponse,
                                                                 out var errorResponse,
                                                                 CustomNotifyAllowedEnergyTransferResponseParser) &&
                    getCompositeScheduleResponse is not null)
                {
                    response = getCompositeScheduleResponse;
                }

                response ??= new NotifyAllowedEnergyTransferResponse(
                                 Request,
                                 Result.Format(errorResponse)
                             );

            }

            response ??= new NotifyAllowedEnergyTransferResponse(
                             Request,
                             Result.FromSendRequestState(sendRequestState)
                         );


            #region Send OnNotifyAllowedEnergyTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyAllowedEnergyTransferResponse?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnNotifyAllowedEnergyTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
