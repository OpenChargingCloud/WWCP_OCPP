﻿/*
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : ACSMSWSServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?  CustomGetChargingProfilesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetChargingProfilesResponse>?     CustomGetChargingProfilesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetChargingProfiles request was sent.
        /// </summary>
        public event CSMS.OnGetChargingProfilesRequestDelegate?     OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetChargingProfiles request was sent.
        /// </summary>
        public event CSMS.OnGetChargingProfilesResponseDelegate?    OnGetChargingProfilesResponse;

        #endregion


        #region GetChargingProfiles(Request)

        public async Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request)
        {

            #region Send OnGetChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetChargingProfilesRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetChargingProfilesRequest));
            }

            #endregion


            GetChargingProfilesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.NetworkingNodeId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetChargingProfilesRequestSerializer,
                                                     CustomChargingProfileCriterionSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetChargingProfilesResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var getChargingProfilesResponse,
                                                             out var errorResponse,
                                                             CustomGetChargingProfilesResponseParser) &&
                        getChargingProfilesResponse is not null)
                    {
                        response = getChargingProfilesResponse;
                    }

                    response ??= new GetChargingProfilesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );
                        
                }

                response ??= new GetChargingProfilesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetChargingProfilesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetChargingProfilesResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}