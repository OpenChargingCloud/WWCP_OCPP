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

        public CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<DataTransferResponse>?     CustomDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestSentDelegate?         OnDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a DataTransfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferResponseReceivedDelegate?    OnDataTransferResponseReceived;

        #endregion


        #region DataTransfer(Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<DataTransferResponse>

            DataTransfer(DataTransferRequest  Request)

        {

            #region Send OnDataTransferRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequestSent?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequestSent));
            }

            #endregion


            DataTransferResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationNodeId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomDataTransferRequestSerializer,
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

                        if (!DataTransferResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out response,
                                                           out var errorResponse,
                                                           CustomDataTransferResponseParser))
                        {
                            response = new DataTransferResponse(
                                           Request,
                                           Result.Format(errorResponse)
                                       );
                        }

                    }

                    response ??= new DataTransferResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new DataTransferResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new DataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnDataTransferResponseReceived event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponseReceived?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
