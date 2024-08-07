﻿/*
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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel,
                                                 ICentralSystemChannel
    {

        #region Custom binary serializer delegates

        public CustomJObjectSerializerDelegate<GetFileRequest>?  CustomGetFileRequestSerializer    { get; set; }

        public CustomBinaryParserDelegate<GetFileResponse>?      CustomGetFileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnGetFileRequestDelegate?     OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnGetFileResponseDelegate?    OnGetFileResponse;

        #endregion


        #region GetFile(Request)

        public async Task<GetFileResponse> GetFile(GetFileRequest Request)
        {

            #region Send OnGetFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetFileRequest?.Invoke(startTime,
                                         this,
                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetFileRequest));
            }

            #endregion


            GetFileResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetFileRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.BinaryResponse is not null)
                {

                    if (!GetFileResponse.TryParse(Request,
                                                  sendRequestState.BinaryResponse.Payload,
                                                  out response,
                                                  out var errorResponse,
                                                  CustomGetFileResponseParser))
                    {
                        response = new GetFileResponse(
                                           Request,
                                           Result.Format(errorResponse)
                                       );
                    }

                }

                response ??= new GetFileResponse(
                                 Request,
                                 Request.FileName,
                                 GetFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new GetFileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetFileResponse?.Invoke(endTime,
                                          this,
                                          Request,
                                          response,
                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnGetFileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
