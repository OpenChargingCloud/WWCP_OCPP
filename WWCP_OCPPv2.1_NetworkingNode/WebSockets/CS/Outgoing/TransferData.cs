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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class NetworkingNodeWSClient : WebSocketClient,
                                                   INetworkingNodeWebSocketClient,
                                                   INetworkingNodeServer,
                                                   INetworkingNodeClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.DataTransferRequest>?  CustomDataTransferRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<OCPPv2_1.CSMS.DataTransferResponse>?   CustomDataTransferResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event CS.OnDataTransferRequestDelegate?     OnDataTransferRequest;

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
        public event CS.OnDataTransferResponseDelegate?    OnDataTransferResponse;

        #endregion


        #region TransferData(Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<OCPPv2_1.CSMS.DataTransferResponse>

            TransferData(OCPPv2_1.CS.DataTransferRequest  Request)

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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            OCPPv2_1.CSMS.DataTransferResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                         Request.NetworkingNodeId,
                                         Request.NetworkPath,
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

                        if (OCPPv2_1.CSMS.DataTransferResponse.TryParse(Request,
                                                                        sendRequestState.JSONResponse.Payload,
                                                                        out var dataTransferResponse,
                                                                        out var errorResponse,
                                                                        CustomDataTransferResponseParser) &&
                            dataTransferResponse is not null)
                        {
                            response = dataTransferResponse;
                        }

                        response ??= new OCPPv2_1.CSMS.DataTransferResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new OCPPv2_1.CSMS.DataTransferResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new OCPPv2_1.CSMS.DataTransferResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new OCPPv2_1.CSMS.DataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


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
                DebugX.Log(e, nameof(NetworkingNodeWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
