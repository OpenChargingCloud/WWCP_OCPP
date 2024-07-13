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
using cloud.charging.open.protocols.OCPP.CS;
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

        public CustomBinarySerializerDelegate<SendFileRequest>?  CustomSendFileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SendFileResponse>?    CustomSendFileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnSendFileRequestDelegate?     OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnSendFileResponseDelegate?    OnSendFileResponse;

        #endregion


        #region SendFile(Request)

        public async Task<SendFileResponse> SendFile(SendFileRequest Request)
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


            SendFileResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
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

                    if (!SendFileResponse.TryParse(Request,
                                                   sendRequestState.JSONResponse.Payload,
                                                   out response,
                                                   out var errorResponse,
                                                   CustomSendFileResponseParser))
                    {
                        response = new SendFileResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new SendFileResponse(
                                 Request,
                                 Request.FileName,
                                 SendFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new SendFileResponse(
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
