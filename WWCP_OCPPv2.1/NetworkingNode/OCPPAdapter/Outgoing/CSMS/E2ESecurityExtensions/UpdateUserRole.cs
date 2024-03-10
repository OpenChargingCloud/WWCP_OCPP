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
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?  CustomUpdateUserRoleRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<UpdateUserRoleResponse>?     CustomUpdateUserRoleResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a UpdateUserRole request was sent.
        /// </summary>
        public event OCPP.CSMS.OnUpdateUserRoleRequestDelegate?     OnUpdateUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to a UpdateUserRole request was sent.
        /// </summary>
        public event OCPP.CSMS.OnUpdateUserRoleResponseDelegate?    OnUpdateUserRoleResponse;

        #endregion


        #region UpdateUserRole(Request)

        public async Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
        {

            #region Send OnUpdateUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleRequest?.Invoke(startTime,
                                                parentNetworkingNode,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateUserRoleRequest));
            }

            #endregion


            UpdateUserRoleResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomUpdateUserRoleRequestSerializer
                                                         //CustomMessageInfoSerializer,
                                                         //CustomMessageContentSerializer,
                                                         //CustomComponentSerializer,
                                                         //CustomEVSESerializer,
                                                         //CustomSignatureSerializer,
                                                         //CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (UpdateUserRoleResponse.TryParse(Request,
                                                        sendRequestState.JSONResponse.Payload,
                                                        out var setDisplayMessageResponse,
                                                        out var errorResponse,
                                                        CustomUpdateUserRoleResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new UpdateUserRoleResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new UpdateUserRoleResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new UpdateUserRoleResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnUpdateUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleResponse?.Invoke(endTime,
                                                 parentNetworkingNode,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
