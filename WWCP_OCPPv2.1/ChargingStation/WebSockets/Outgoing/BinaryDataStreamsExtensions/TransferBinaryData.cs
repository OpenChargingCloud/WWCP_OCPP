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

    #region OnBinaryDataTransfer (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a binary data transfer request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnBinaryDataTransferRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             BinaryDataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a binary data transfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBinaryDataTransferResponseDelegate(DateTime                          Timestamp,
                                                              IEventSender                      Sender,
                                                              BinaryDataTransferRequest         Request,
                                                              CSMS.BinaryDataTransferResponse   Response,
                                                              TimeSpan                          Runtime);

    #endregion


    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<BinaryDataTransferRequest>?    CustomBinaryDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<CSMS.BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a binary data transfer request will be sent to the CSMS.
        /// </summary>
        public event OnBinaryDataTransferRequestDelegate?     OnBinaryDataTransferRequest;

        /// <summary>
        /// An event fired whenever a binary data transfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnBinaryDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a binary data transfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnBinaryDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a binary data transfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseDelegate?    OnBinaryDataTransferResponse;

        #endregion


        #region TransferBinaryData(Request)

        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<CSMS.BinaryDataTransferResponse>

            TransferBinaryData(BinaryDataTransferRequest  Request)

        {

            #region Send OnBinaryDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBinaryDataTransferRequest));
            }

            #endregion


            CSMS.BinaryDataTransferResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToBinary(
                                                       CustomBinaryDataTransferRequestSerializer
                                                       //CustomSignatureSerializer,
                                                       //CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (CSMS.BinaryDataTransferResponse.TryParse(Request,
                                                                 sendRequestState.BinaryResponse,
                                                                 out var binaryDataTransferResponse,
                                                                 out var errorResponse,
                                                                 CustomBinaryDataTransferResponseParser) &&
                        binaryDataTransferResponse is not null)
                    {
                        response = binaryDataTransferResponse;
                    }

                    response ??= new CSMS.BinaryDataTransferResponse(Request,
                                                                     Result.Format(errorResponse));

                }

                response ??= new CSMS.BinaryDataTransferResponse(Request,
                                                                 Result.FromSendRequestState(sendRequestState));

            }

            response ??= new CSMS.BinaryDataTransferResponse(Request,
                                                             Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnBinaryDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBinaryDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
