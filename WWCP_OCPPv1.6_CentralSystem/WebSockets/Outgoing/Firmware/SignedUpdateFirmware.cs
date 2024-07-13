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
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel,
                                                 ICentralSystemChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?     CustomSignedUpdateFirmwareResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SignedUpdateFirmware request was sent.
        /// </summary>
        public event OnSignedUpdateFirmwareRequestDelegate?     OnSignedUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a SignedUpdateFirmware request was sent.
        /// </summary>
        public event OnSignedUpdateFirmwareResponseDelegate?    OnSignedUpdateFirmwareResponse;

        #endregion


        #region SignedUpdateFirmware(Request)

        public async Task<SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request)
        {

            #region Send OnSignedUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSignedUpdateFirmwareRequest));
            }

            #endregion


            SignedUpdateFirmwareResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSignedUpdateFirmwareRequestSerializer,
                                                     CustomFirmwareImageSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SignedUpdateFirmwareResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var updateFirmwareResponse,
                                                        out var errorResponse,
                                                        CustomSignedUpdateFirmwareResponseParser) &&
                        updateFirmwareResponse is not null)
                    {
                        response = updateFirmwareResponse;
                    }

                    response ??= new SignedUpdateFirmwareResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SignedUpdateFirmwareResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SignedUpdateFirmwareResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSignedUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnSignedUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
