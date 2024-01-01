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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<BinaryDataTransferResponse>?     CustomBinaryDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnBinaryDataTransferRequestDelegate?     OnBinaryDataTransferRequest;

        /// <summary>
        /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnBinaryDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnBinaryDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseDelegate?    OnBinaryDataTransferResponse;

        #endregion


        #region TransferBinaryData(Request)

        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse>

            BinaryDataTransfer(BinaryDataTransferRequest Request)

        {

            #region Send OnBinaryDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferRequest?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryDataTransferRequest));
            }

            #endregion


            BinaryDataTransferResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryRequestAndWait(
                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToBinary(
                                                         CustomBinaryDataTransferRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (BinaryDataTransferResponse.TryParse(Request,
                                                            sendRequestState.BinaryResponse.Payload,
                                                            out var binaryDataTransferResponse,
                                                            out var errorResponse,
                                                            CustomBinaryDataTransferResponseParser) &&
                        binaryDataTransferResponse is not null)
                    {
                        response = binaryDataTransferResponse;
                    }

                    response ??= new BinaryDataTransferResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                }

                response ??= new BinaryDataTransferResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new BinaryDataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnBinaryDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferResponse?.Invoke(endTime,
                                                     parentNetworkingNode,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnBinaryDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
