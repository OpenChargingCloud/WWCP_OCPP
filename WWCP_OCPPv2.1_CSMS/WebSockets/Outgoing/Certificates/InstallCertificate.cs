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

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<InstallCertificateResponse>?     CustomInstallCertificateResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an InstallCertificate request was sent.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?     OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an InstallCertificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?    OnInstallCertificateResponse;

        #endregion


        #region InstallCertificate(Request)

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        public async Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {

            #region Send OnInstallCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnInstallCertificateRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            InstallCertificateResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomInstallCertificateRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (InstallCertificateResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var installCertificateResponse,
                                                            out var errorResponse,
                                                            CustomInstallCertificateResponseParser) &&
                        installCertificateResponse is not null)
                    {
                        response = installCertificateResponse;
                    }

                    response ??= new InstallCertificateResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new InstallCertificateResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new InstallCertificateResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
