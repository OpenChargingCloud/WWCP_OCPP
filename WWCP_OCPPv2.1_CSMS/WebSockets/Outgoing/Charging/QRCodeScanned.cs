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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<QRCodeScannedRequest>?  CustomQRCodeScannedRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<QRCodeScannedResponse>?     CustomQRCodeScannedResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an QRCodeScanned request was sent.
        /// </summary>
        public event OnQRCodeScannedRequestSentDelegate?         OnQRCodeScannedRequestSent;

        /// <summary>
        /// An event sent whenever a response to an QRCodeScanned request was sent.
        /// </summary>
        public event OnQRCodeScannedResponseReceivedDelegate?    OnQRCodeScannedResponseReceived;

        #endregion


        #region QRCodeScanned(Request)


        public async Task<QRCodeScannedResponse> QRCodeScanned(QRCodeScannedRequest Request)
        {

            #region Send OnQRCodeScannedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnQRCodeScannedRequestSent?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnQRCodeScannedRequestSent));
            }

            #endregion


            QRCodeScannedResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomQRCodeScannedRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (QRCodeScannedResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var qrCodeScannedResponse,
                                                       out var errorResponse,
                                                       CustomQRCodeScannedResponseParser) &&
                        qrCodeScannedResponse is not null)
                    {
                        response = qrCodeScannedResponse;
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
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnQRCodeScannedResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
