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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<QRCodeScannedRequest>?  CustomQRCodeScannedRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<QRCodeScannedResponse>?     CustomQRCodeScannedResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a QRCodeScanned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnQRCodeScannedRequestSentDelegate?     OnQRCodeScannedRequestSent;

        /// <summary>
        /// An event sent whenever a response to a QRCodeScanned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnQRCodeScannedResponseReceivedDelegate?    OnQRCodeScannedResponseReceived;

        #endregion


        #region QRCodeScanned(Request)


        public async Task<QRCodeScannedResponse> QRCodeScanned(QRCodeScannedRequest Request)
        {

            #region Send OnQRCodeScannedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnQRCodeScannedRequestSent?.Invoke(startTime,
                                                   parentNetworkingNode,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnQRCodeScannedRequestSent));
            }

            #endregion


            QRCodeScannedResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomQRCodeScannedRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (QRCodeScannedResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var getCompositeScheduleResponse,
                                                       out var errorResponse,
                                                       CustomQRCodeScannedResponseParser) &&
                        getCompositeScheduleResponse is not null)
                    {
                        response = getCompositeScheduleResponse;
                    }

                    response ??= new QRCodeScannedResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new QRCodeScannedResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new QRCodeScannedResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnQRCodeScannedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnQRCodeScannedResponseReceived?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnQRCodeScannedResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a QRCodeScanned request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnQRCodeScannedResponseReceivedDelegate? OnQRCodeScannedResponseReceived;

    }

}
