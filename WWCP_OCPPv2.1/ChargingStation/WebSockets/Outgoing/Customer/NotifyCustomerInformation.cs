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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnNotifyCustomerInformation            (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a notify customer information request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnNotifyCustomerInformationRequestDelegate(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    NotifyCustomerInformationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify customer information request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyCustomerInformationResponseDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     NotifyCustomerInformationRequest    Request,
                                                                     NotifyCustomerInformationResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion


    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?  CustomNotifyCustomerInformationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?     CustomNotifyCustomerInformationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?     OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnNotifyCustomerInformationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?    OnNotifyCustomerInformationResponse;

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

                OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            NotifyCustomerInformationResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(Request.Action,
                                                       Request.RequestId,
                                                       Request.ToJSON(
                                                           CustomNotifyCustomerInformationRequestSerializer,
                                                           CustomSignatureSerializer,
                                                           CustomCustomDataSerializer
                                                       ));

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.Response is not null)
                    {

                        if (NotifyCustomerInformationResponse.TryParse(Request,
                                                                       sendRequestState.Response,
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

                response ??= new NotifyCustomerInformationResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
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

                OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
