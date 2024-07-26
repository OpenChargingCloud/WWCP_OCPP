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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a UpdateUserRole request was sent.
        /// </summary>
        public event OnUpdateUserRoleRequestSentDelegate?         OnUpdateUserRoleRequestSent;

        #endregion

        #region UpdateUserRole(Request)

        public async Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
        {

            #region Send OnUpdateUserRoleRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleRequestSent?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    null,
                                                    Request,
                                                    SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateUserRoleRequestSent));
            }

            #endregion


            UpdateUserRoleResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         parentNetworkingNode.OCPP.CustomUpdateUserRoleRequestSerializer
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
                                                        parentNetworkingNode.OCPP.CustomUpdateUserRoleResponseParser) &&
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


            #region Send OnUpdateUserRoleResponseReceived event

            //var endTime = Timestamp.Now;

            //try
            //{

            //    OnUpdateUserRoleResponseReceived?.Invoke(endTime,
            //                                             parentNetworkingNode,
            //                                             Request,
            //                                             response,
            //                                             endTime - startTime);

            //}
            //catch (Exception e)
            //{
            //    DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnUpdateUserRoleResponseReceived));
            //}

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a UpdateUserRole request was sent.
        /// </summary>
        public event OnUpdateUserRoleResponseReceivedDelegate? OnUpdateUserRoleResponseReceived;

        #endregion

    }

}
