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

        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?  CustomListDirectoryRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ListDirectoryResponse>?     CustomListDirectoryResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ListDirectory request was sent.
        /// </summary>
        public event OCPP.CSMS.OnListDirectoryRequestDelegate?     OnListDirectoryRequest;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was sent.
        /// </summary>
        public event OCPP.CSMS.OnListDirectoryResponseDelegate?    OnListDirectoryResponse;

        #endregion


        #region ListDirectory(Request)

        public async Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        {

            #region Send OnListDirectoryRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnListDirectoryRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnListDirectoryRequest));
            }

            #endregion


            ListDirectoryResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomListDirectoryRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!ListDirectoryResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out response,
                                                        out var errorResponse,
                                                        CustomListDirectoryResponseParser))
                    {
                        response = new ListDirectoryResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new ListDirectoryResponse(
                                 Request,
                                 Request.DirectoryPath,
                                 ListDirectoryStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new ListDirectoryResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnListDirectoryResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnListDirectoryResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnListDirectoryResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
