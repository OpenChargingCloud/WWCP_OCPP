﻿/*
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
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargePointWSClient : AOCPPWebSocketClient,
                                               IChargePointWebSocketClient,
                                               ICPIncomingMessages,
                                               ICPOutgoingMessagesEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<AuthorizeResponse>?     CustomAuthorizeResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            AuthorizeResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomAuthorizeRequestSerializer,
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
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
