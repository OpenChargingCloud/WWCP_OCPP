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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : ACSMSWSServer,
                                        ICSMSChannel
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<OCPP.CSMS.BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<OCPP.CS.BinaryDataTransferResponse>?       CustomBinaryDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnBinaryDataTransferRequestDelegate?     OnBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was sent.
        /// </summary>
        public event OCPP.CSMS.OnBinaryDataTransferResponseDelegate?    OnBinaryDataTransferResponse;

        #endregion


        #region TransferBinaryData(Request)

        public async Task<OCPP.CS.BinaryDataTransferResponse> TransferBinaryData(OCPP.CSMS.BinaryDataTransferRequest Request)
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


            OCPP.CS.BinaryDataTransferResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryAndWait(
                                                 Request.EventTrackingId,
                                                 Request.NetworkingNodeId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToBinary(
                                                     CustomBinaryDataTransferRequestSerializer,
                                                     CustomBinarySignatureSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (OCPP.CS.BinaryDataTransferResponse.TryParse(Request,
                                                                    sendRequestState.BinaryResponse.Payload,
                                                                    out var binaryDataTransferResponse,
                                                                    out var errorResponse,
                                                                    CustomBinaryDataTransferResponseParser) &&
                        binaryDataTransferResponse is not null)
                    {
                        response = binaryDataTransferResponse;
                    }

                    response ??= new OCPP.CS.BinaryDataTransferResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                }

                response ??= new OCPP.CS.BinaryDataTransferResponse(
                                 Request,
                                 BinaryDataTransferStatus.Rejected
                             );// Result.FromSendRequestState(sendRequestState));

            }
            catch (Exception e)
            {

                response = new OCPP.CS.BinaryDataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


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
