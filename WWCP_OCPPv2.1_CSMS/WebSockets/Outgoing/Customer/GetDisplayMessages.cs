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

        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetDisplayMessagesResponse>?     CustomGetDisplayMessagesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?     OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?    OnGetDisplayMessagesResponse;

        #endregion


        #region GetDisplayMessages(Request)

        public async Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
        {

            #region Send OnGetDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDisplayMessagesRequest));
            }

            #endregion


            GetDisplayMessagesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetDisplayMessagesRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetDisplayMessagesResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var getDisplayMessagesResponse,
                                                            out var errorResponse,
                                                            CustomGetDisplayMessagesResponseParser) &&
                        getDisplayMessagesResponse is not null)
                    {
                        response = getDisplayMessagesResponse;
                    }

                    response ??= new GetDisplayMessagesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetDisplayMessagesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetDisplayMessagesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
