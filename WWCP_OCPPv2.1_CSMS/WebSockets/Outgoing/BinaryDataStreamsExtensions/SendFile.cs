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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<SendFileRequest>?   CustomSendFileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<CS.SendFileResponse>?  CustomSendFileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile request was sent.
        /// </summary>
        public event OnSendFileRequestDelegate?     OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OnSendFileResponseDelegate?    OnSendFileResponse;

        #endregion


        #region SendFile(Request)

        public async Task<CS.SendFileResponse> SendFile(SendFileRequest Request)
        {

            #region Send OnSendFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileRequest?.Invoke(startTime,
                                          this,
                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendFileRequest));
            }

            #endregion


            CS.SendFileResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToBinary(
                                                     CustomSendFileRequestSerializer,
                                                     CustomBinarySignatureSerializer,
                                                     IncludeSignatures: true
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (CS.SendFileResponse.TryParse(Request,
                                                     sendRequestState.JSONResponse.Payload,
                                                     out var getFileResponse,
                                                     out var errorResponse,
                                                     CustomSendFileResponseParser) &&
                        getFileResponse is not null)
                    {
                        response = getFileResponse;
                    }

                    response ??= new CS.SendFileResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                }

                response ??= new CS.SendFileResponse(
                                 Request,
                                 Request.FileName,
                                 SendFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new CS.SendFileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendFileResponse?.Invoke(endTime,
                                           this,
                                           Request,
                                           response,
                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSendFileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
