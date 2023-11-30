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

        public CustomJObjectSerializerDelegate<AFRRSignalRequest>?   CustomAFRRSignalRequestSerializer     { get; set; }

        public CustomJObjectParserDelegate<AFRRSignalResponse>?      CustomAFRRSignalResponseParser        { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an AFRR signal request was sent.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?     OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever a response to an AFRR signal request was sent.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?    OnAFRRSignalResponse;

        #endregion


        #region SendAFRRSignal(Request)

        public async Task<AFRRSignalResponse> SendAFRRSignal(AFRRSignalRequest Request)
        {

            #region Send OnAFRRSignalRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAFRRSignalRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAFRRSignalRequest));
            }

            #endregion


            AFRRSignalResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomAFRRSignalRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (AFRRSignalResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var unlockConnectorResponse,
                                                    out var errorResponse,
                                                    CustomAFRRSignalResponseParser) &&
                        unlockConnectorResponse is not null)
                    {
                        response = unlockConnectorResponse;
                    }

                    response ??= new AFRRSignalResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new AFRRSignalResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {
                response = new AFRRSignalResponse(
                               Request,
                               Result.FromException(e)
                           );
            }


            #region Send OnAFRRSignalResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAFRRSignalResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnAFRRSignalResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
