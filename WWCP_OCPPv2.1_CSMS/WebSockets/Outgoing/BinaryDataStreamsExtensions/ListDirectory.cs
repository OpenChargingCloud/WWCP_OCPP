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

        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?  CustomListDirectoryRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<CS.ListDirectoryResponse>?  CustomListDirectoryResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ListDirectory request was sent.
        /// </summary>
        public event OnListDirectoryRequestDelegate?     OnListDirectoryRequest;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was sent.
        /// </summary>
        public event OnListDirectoryResponseDelegate?    OnListDirectoryResponse;

        #endregion


        #region ListDirectory(Request)

        public async Task<CS.ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnListDirectoryRequest));
            }

            #endregion


            CS.ListDirectoryResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
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

                    if (CS.ListDirectoryResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var deleteFileResponse,
                                                          out var errorResponse,
                                                          CustomListDirectoryResponseParser) &&
                        deleteFileResponse is not null)
                    {
                        response = deleteFileResponse;
                    }

                    response ??= new CS.ListDirectoryResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                }

                response ??= new CS.ListDirectoryResponse(
                                 Request,
                                 Request.DirectoryPath,
                                 ListDirectoryStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new CS.ListDirectoryResponse(
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnListDirectoryResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
