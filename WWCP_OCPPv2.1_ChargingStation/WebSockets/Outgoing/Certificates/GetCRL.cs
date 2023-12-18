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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : AOCPPWebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   ICSIncomingMessages,
                                                   ICSOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetCRLRequest>?  CustomGetCRLSerializer        { get; set; }

        public CustomJObjectParserDelegate<GetCRLResponse>?     CustomGetCRLResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a GetCRL (certificate revocation list) request will be sent to the CSMS.
        /// </summary>
        public event OnGetCRLRequestDelegate?     OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a GetCRL (certificate revocation list) request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?     OnGetCRLWSRequest;

        /// <summary>
        /// An event fired whenever a response to a GetCRL (certificate revocation list) request was received.
        /// </summary>
        public event ClientResponseLogHandler?    OnGetCRLWSResponse;

        /// <summary>
        /// An event fired whenever a response to a GetCRL (certificate revocation list) request was received.
        /// </summary>
        public event OnGetCRLResponseDelegate?    OnGetCRLResponse;

        #endregion


        #region GetCRL(Request)

        /// <summary>
        /// Send a GetCRL (certificate revocation list) request.
        /// </summary>
        /// <param name="Request">A GetCRL request.</param>
        public async Task<GetCRLResponse>

            GetCRL(GetCRLRequest  Request)

        {

            #region Send OnGetCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCRLRequest?.Invoke(startTime,
                                        this,
                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCRLRequest));
            }

            #endregion


            GetCRLResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationNodeId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomGetCRLSerializer,
                                                   CustomCertificateHashDataSerializer,
                                                   CustomSignatureSerializer,
                                                   CustomCustomDataSerializer
                                               )
                                           );

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (GetCRLResponse.TryParse(Request,
                                                    sendRequestState.JSONResponse.Payload,
                                                    out var getCertificateStatusResponse,
                                                    out var errorResponse,
                                                    CustomGetCRLResponseParser) &&
                            getCertificateStatusResponse is not null)
                        {
                            response = getCertificateStatusResponse;
                        }

                        response ??= new GetCRLResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new GetCRLResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new GetCRLResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new GetCRLResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCRLResponse?.Invoke(endTime,
                                         this,
                                         Request,
                                         response,
                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
