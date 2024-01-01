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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
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

        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetInstalledCertificateIdsResponse>?     CustomGetInstalledCertificateIdsResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsRequestSentDelegate?     OnGetInstalledCertificateIdsRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsResponseReceivedDelegate?    OnGetInstalledCertificateIdsResponseReceived;

        #endregion


        #region GetInstalledCertificateIds(Request)

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        public async Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {

            #region Send OnGetInstalledCertificateIdsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsRequestSent?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetInstalledCertificateIdsRequestSent));
            }

            #endregion


            GetInstalledCertificateIdsResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomGetInstalledCertificateIdsRequestSerializer,
                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetInstalledCertificateIdsResponse.TryParse(Request,
                                                                    sendRequestState.JSONResponse.Payload,
                                                                    out var getInstalledCertificateIdsResponse,
                                                                    out var errorResponse,
                                                                    CustomGetInstalledCertificateIdsResponseParser) &&
                        getInstalledCertificateIdsResponse is not null)
                    {
                        response = getInstalledCertificateIdsResponse;
                    }

                    response ??= new GetInstalledCertificateIdsResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetInstalledCertificateIdsResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetInstalledCertificateIdsResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponseReceived?.Invoke(endTime,
                                                             parentNetworkingNode,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnGetInstalledCertificateIdsResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsResponseReceivedDelegate? OnGetInstalledCertificateIdsResponseReceived;

    }

}
