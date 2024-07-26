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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<CustomerInformationResponse>?     CustomCustomerInformationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a CustomerInformation request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCustomerInformationRequestSentDelegate?     OnCustomerInformationRequestSent;

        /// <summary>
        /// An event sent whenever a response to a CustomerInformation request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCustomerInformationResponseReceivedDelegate?    OnCustomerInformationResponseReceived;

        #endregion


        #region RequestCustomerInformation(Request)

        public async Task<CustomerInformationResponse> CustomerInformation(CustomerInformationRequest Request)
        {

            #region Send OnCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCustomerInformationRequestSent?.Invoke(startTime,
                                                     parentNetworkingNode,
                                                     null,
                                                     Request,
                                                SentMessageResults.Success);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnCustomerInformationRequestSent));
            }

            #endregion


            CustomerInformationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomCustomerInformationRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomIdTokenSerializer,
                                                         parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
                                                         parentNetworkingNode.OCPP.CustomCertificateHashDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (CustomerInformationResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var customerInformationResponse,
                                                             out var errorResponse,
                                                             CustomCustomerInformationResponseParser) &&
                        customerInformationResponse is not null)
                    {
                        response = customerInformationResponse;
                    }

                    response ??= new CustomerInformationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new CustomerInformationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new CustomerInformationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCustomerInformationResponseReceived?.Invoke(endTime,
                                                      parentNetworkingNode,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnCustomerInformationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a CustomerInformation request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCustomerInformationResponseReceivedDelegate? OnCustomerInformationResponseReceived;

    }

}
