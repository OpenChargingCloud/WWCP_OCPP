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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnBinaryDataTransfer (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a BinaryDataTransfer request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnBinaryDataTransferRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             BinaryDataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a BinaryDataTransfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBinaryDataTransferResponseDelegate(DateTime                        Timestamp,
                                                              IEventSender                    Sender,
                                                              BinaryDataTransferRequest       Request,
                                                              CS.BinaryDataTransferResponse   Response,
                                                              TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<CS.BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnBinaryDataTransferRequestDelegate?     OnBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseDelegate?    OnBinaryDataTransferResponse;

        #endregion


        #region TransferBinaryData               (Request)

        public async Task<CS.BinaryDataTransferResponse> TransferBinaryData(BinaryDataTransferRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBinaryDataTransferRequest));
            }

            #endregion


            CS.BinaryDataTransferResponse? response = null;

            var sendRequestState = await SendBinaryAndWait(
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             Request.ChargingStationId,
                                             Request.Action,
                                             Request.ToBinary(
                                                 CustomBinaryDataTransferRequestSerializer
                                                 //CustomSignatureSerializer,
                                                 //CustomCustomBinaryDataSerializer
                                             ),
                                             Request.RequestTimeout
                                         );

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (CS.BinaryDataTransferResponse.TryParse(Request,
                                                           sendRequestState.Response,
                                                           out var dataTransferResponse,
                                                           out var errorResponse,
                                                           CustomBinaryDataTransferResponseParser) &&
                    dataTransferResponse is not null)
                {
                    response = dataTransferResponse;
                }

                response ??= new CS.BinaryDataTransferResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

            }

            response ??= new CS.BinaryDataTransferResponse(
                             Request,
                             BinaryDataTransferStatus.Rejected
                         );// Result.FromSendRequestState(sendRequestState));


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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnBinaryDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
