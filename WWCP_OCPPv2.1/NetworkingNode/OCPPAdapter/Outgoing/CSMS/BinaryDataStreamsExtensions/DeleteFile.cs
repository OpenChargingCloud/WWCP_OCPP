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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom binary serializer delegates

        public CustomJObjectSerializerDelegate<DeleteFileRequest>?  CustomDeleteFileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<DeleteFileResponse>?     CustomDeleteFileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DeleteFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnDeleteFileRequestDelegate?     OnDeleteFileRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        public event OCPP.CSMS.OnDeleteFileResponseDelegate?    OnDeleteFileResponse;

        #endregion


        #region DeleteFile(Request)

        public async Task<DeleteFileResponse> DeleteFile(DeleteFileRequest Request)
        {

            #region Send OnDeleteFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteFileRequest?.Invoke(startTime,
                                            parentNetworkingNode,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnDeleteFileRequest));
            }

            #endregion


            DeleteFileResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomDeleteFileRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!DeleteFileResponse.TryParse(Request,
                                                     sendRequestState.JSONResponse.Payload,
                                                     out response,
                                                     out var errorResponse,
                                                     CustomDeleteFileResponseParser))
                    {
                        response = new DeleteFileResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new DeleteFileResponse(
                                 Request,
                                 Request.FileName,
                                 DeleteFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new DeleteFileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnDeleteFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteFileResponse?.Invoke(endTime,
                                             parentNetworkingNode,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnDeleteFileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
