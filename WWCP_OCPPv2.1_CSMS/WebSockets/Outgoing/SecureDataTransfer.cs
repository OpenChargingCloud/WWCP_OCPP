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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<SecureDataTransferRequest>?  CustomSecureDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<SecureDataTransferResponse>?     CustomSecureDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was sent.
        /// </summary>
        public event OnSecureDataTransferRequestSentDelegate?         OnSecureDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a SecureDataTransfer request was sent.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate?    OnSecureDataTransferResponse;

        #endregion


        #region SecureDataTransfer(Request)

        public async Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request)
        {

            #region Send OnSecureDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecureDataTransferRequest));
            }

            #endregion


            SecureDataTransferResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToBinary(
                                                     CustomSecureDataTransferRequestSerializer,
                                                     CustomBinarySignatureSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (!SecureDataTransferResponse.TryParse(Request,
                                                             sendRequestState.BinaryResponse.Payload,
                                                             sendRequestState.DestinationNodeId,
                                                             sendRequestState.NetworkPath,
                                                             out response,
                                                             out var errorResponse,
                                                             sendRequestState.ResponseTimestamp,
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
                                 SecureDataTransferStatus.Rejected
                             );// Result.FromSendRequestState(sendRequestState));

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
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSecureDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
