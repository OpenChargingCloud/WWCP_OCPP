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

    #region OnRemoveDefaultChargingTariff (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a RemoveDefaultChargingTariff request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnRemoveDefaultChargingTariffRequestDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      RemoveDefaultChargingTariffRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a RemoveDefaultChargingTariff request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnRemoveDefaultChargingTariffResponseDelegate(DateTime                              Timestamp,
                                                                       IEventSender                          Sender,
                                                                       RemoveDefaultChargingTariffRequest    Request,
                                                                       RemoveDefaultChargingTariffResponse   Response,
                                                                       TimeSpan                              Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?     CustomRemoveDefaultChargingTariffResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffRequestDelegate?     OnRemoveDefaultChargingTariffRequest;

        /// <summary>
        /// An event sent whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OnRemoveDefaultChargingTariffResponseDelegate?    OnRemoveDefaultChargingTariffResponse;

        #endregion


        #region RemoveDefaultChargingTariff(Request)

        public async Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)
        {

            #region Send OnRemoveDefaultChargingTariffRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffRequest?.Invoke(startTime,
                                                             this,
                                                             Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRemoveDefaultChargingTariffRequest));
            }

            #endregion


            RemoveDefaultChargingTariffResponse? response = null;

            var sendRequestState = await SendJSONAndWait(
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             Request.ChargingStationId,
                                             Request.Action,
                                             Request.ToJSON(
                                                 CustomRemoveDefaultChargingTariffRequestSerializer,
                                                 CustomSignatureSerializer,
                                                 CustomCustomDataSerializer
                                             ),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (RemoveDefaultChargingTariffResponse.TryParse(Request,
                                                                 sendRequestState.Response,
                                                                 out var setDisplayMessageResponse,
                                                                 out var errorResponse,
                                                                 CustomRemoveDefaultChargingTariffResponseParser) &&
                    setDisplayMessageResponse is not null)
                {
                    response = setDisplayMessageResponse;
                }

                response ??= new RemoveDefaultChargingTariffResponse(
                                 Request,
                                 Result.Format(errorResponse)
                             );

            }

            response ??= new RemoveDefaultChargingTariffResponse(
                             Request,
                             Result.FromSendRequestState(sendRequestState)
                         );


            #region Send OnRemoveDefaultChargingTariffResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoveDefaultChargingTariffResponse?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnRemoveDefaultChargingTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
