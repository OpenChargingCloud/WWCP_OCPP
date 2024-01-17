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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for sending messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<SecureDataTransferRequest>?  CustomSecureDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<SecureDataTransferResponse>?     CustomSecureDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a SecureDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event OnSecureDataTransferRequestSentDelegate?         OnSecureDataTransferRequestSent;

        /// <summary>
        /// An event fired whenever a SecureDataTransfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                         OnSecureDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?                        OnSecureDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate?    OnSecureDataTransferResponse;

        #endregion


        #region TransferSecureData(Request)

        /// <summary>
        /// Send vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        public async Task<SecureDataTransferResponse>

            SecureDataTransfer(SecureDataTransferRequest Request)

        {

            #region Send OnSecureDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferRequestSent?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSecureDataTransferRequestSent));
            }

            #endregion


            SecureDataTransferResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryRequestAndWait(
                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToBinary(
                                                         CustomSecureDataTransferRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomBinarySignatureSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (!SecureDataTransferResponse.TryParse(Request,
                                                             sendRequestState.BinaryResponse.Payload,
                                                             out response,
                                                             out var errorResponse,
                                                             CustomSecureDataTransferResponseParser))
                    {
                        response = new SecureDataTransferResponse(
                                           Request,
                                           Result.Format(errorResponse)
                                       );
                    }

                }

                response ??= new SecureDataTransferResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SecureDataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSecureDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferResponse?.Invoke(endTime,
                                                     parentNetworkingNode,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSecureDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }


    /// <summary>
    /// The OCPP adapter for receiving messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate? OnSecureDataTransferResponseReceived;

    }

}
