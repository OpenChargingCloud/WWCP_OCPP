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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<DataTransferResponse>?     CustomDataTransferResponseParser       { get; set; }

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


        #region DataTransfer(Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<DataTransferResponse>

            DataTransfer(DataTransferRequest  Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                              parentNetworkingNode,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            DataTransferResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomDataTransferRequestSerializer,
                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (DataTransferResponse.TryParse(Request,
                                                      sendRequestState.JSONResponse.Payload,
                                                      out var dataTransferResponse,
                                                      out var errorResponse,
                                                      CustomDataTransferResponseParser) &&
                        dataTransferResponse is not null)
                    {
                        response = dataTransferResponse;
                    }

                    response ??= new DataTransferResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                }

                response ??= new DataTransferResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new DataTransferResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                               parentNetworkingNode,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
