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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnDataTransfer (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a data transfer request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDataTransferRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       DataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDataTransferResponseDelegate(DateTime                    Timestamp,
                                                        IEventSender                Sender,
                                                        DataTransferRequest         Request,
                                                        CSMS.DataTransferResponse   Response,
                                                        TimeSpan                    Runtime);

    #endregion


    /// <summary>
    /// A CP client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestDelegate?     OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?    OnDataTransferResponse;

        #endregion


        #region TransferData                         (Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<CSMS.DataTransferResponse>

            TransferData(DataTransferRequest  Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CSMS.DataTransferResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomDataTransferRequestSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (CSMS.DataTransferResponse.TryParse(Request,
                                                           sendRequestState.Response,
                                                           out var dataTransferResponse,
                                                           out var errorResponse) &&
                        dataTransferResponse is not null)
                    {
                        response = dataTransferResponse;
                    }

                    response ??= new CSMS.DataTransferResponse(Request,
                                                               Result.Format(errorResponse));

                }

                response ??= new CSMS.DataTransferResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));

            }

            response ??= new CSMS.DataTransferResponse(Request,
                                                       Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
