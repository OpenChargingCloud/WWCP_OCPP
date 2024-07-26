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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?  CustomNotifyCustomerInformationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?     CustomNotifyCustomerInformationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCustomerInformationRequestSentDelegate?     OnNotifyCustomerInformationRequestSent;

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                    OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event ClientResponseLogHandler?                                   OnNotifyCustomerInformationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCustomerInformationResponseReceivedDelegate?    OnNotifyCustomerInformationResponseReceived;

        #endregion


        #region NotifyCustomerInformation(Request)

        /// <summary>
        /// Notify about customer information.
        /// </summary>
        /// <param name="Request">A NotifyCustomerInformation request.</param>
        public async Task<NotifyCustomerInformationResponse>

            NotifyCustomerInformation(NotifyCustomerInformationRequest  Request)

        {

            #region Send OnNotifyCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationRequestSent?.Invoke(startTime,
                                                           parentNetworkingNode,
                                                           null,
                                                           Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyCustomerInformationRequestSent));
            }

            #endregion


            NotifyCustomerInformationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyCustomerInformationRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyCustomerInformationResponse.TryParse(Request,
                                                                   sendRequestState.JSONResponse.Payload,
                                                                   out var notifyCustomerInformationResponse,
                                                                   out var errorResponse,
                                                                   CustomNotifyCustomerInformationResponseParser) &&
                        notifyCustomerInformationResponse is not null)
                    {
                        response = notifyCustomerInformationResponse;
                    }

                    response ??= new NotifyCustomerInformationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new NotifyCustomerInformationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyCustomerInformationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationResponseReceived?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyCustomerInformationResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCustomerInformationResponseReceivedDelegate? OnNotifyCustomerInformationResponseReceived;

    }

}
