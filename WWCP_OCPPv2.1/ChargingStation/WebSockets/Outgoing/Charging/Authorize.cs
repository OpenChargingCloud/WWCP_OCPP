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

    #region OnAuthorize (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever an authorize request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task OnAuthorizeRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    AuthorizeRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize request.</param>
    public delegate Task OnAuthorizeResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     AuthorizeRequest    Request,
                                                     AuthorizeResponse   Response,
                                                     TimeSpan            Runtime);

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

        public CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<AuthorizeResponse>?     CustomAuthorizeResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        #endregion


        #region Authorize(Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An Authorize request.</param>
        public async Task<AuthorizeResponse>

            Authorize(AuthorizeRequest  Request)

        {

            #region Send OnAuthorizeRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeRequest?.Invoke(startTime,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            AuthorizeResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(Request.Action,
                                                       Request.RequestId,
                                                       Request.ToJSON(
                                                           CustomAuthorizeRequestSerializer,
                                                           CustomIdTokenSerializer,
                                                           CustomAdditionalInfoSerializer,
                                                           CustomOCSPRequestDataSerializer,
                                                           CustomSignatureSerializer,
                                                           CustomCustomDataSerializer
                                                       ));

                if (requestMessage.NoErrors)
                {

                    var sendRequestState = await WaitForResponse(requestMessage);

                    if (sendRequestState.NoErrors &&
                        sendRequestState.JSONResponse is not null)
                    {

                        if (AuthorizeResponse.TryParse(Request,
                                                       sendRequestState.JSONResponse.Payload,
                                                       out var authorizeResponse,
                                                       out var errorResponse,
                                                       CustomAuthorizeResponseParser) &&
                            authorizeResponse is not null)
                        {
                            response = authorizeResponse;
                        }

                        response ??= new AuthorizeResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new AuthorizeResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new AuthorizeResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new AuthorizeResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            this,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
