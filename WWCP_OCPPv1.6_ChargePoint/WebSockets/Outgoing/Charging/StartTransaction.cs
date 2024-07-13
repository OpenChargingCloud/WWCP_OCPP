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

        public CustomJObjectSerializerDelegate<StartTransactionRequest>?  CustomStartTransactionRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<StartTransactionResponse>?     CustomStartTransactionResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a StartTransaction request will be sent to the CSMS.
        /// </summary>
        public event OnStartTransactionRequestDelegate?     OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a StartTransaction request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnStartTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a StartTransaction request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnStartTransactionWSResponse;

        /// <summary>
        /// An event fired whenever a response to a StartTransaction request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate?    OnStartTransactionResponse;

        #endregion


        #region StartTransaction(Request)

        /// <summary>
        /// Send a start transaction information.
        /// </summary>
        /// <param name="Request">A StartTransaction request.</param>
        public async Task<StartTransactionResponse>

            StartTransaction(StartTransactionRequest Request)

        {

            #region Send OnStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStartTransactionRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            StartTransactionResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomStartTransactionRequestSerializer,
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

                        if (StartTransactionResponse.TryParse(Request,
                                                              sendRequestState.JSONResponse.Payload,
                                                              out var meterValuesResponse,
                                                              out var errorResponse,
                                                              CustomStartTransactionResponseParser) &&
                            meterValuesResponse is not null)
                        {
                            response = meterValuesResponse;
                        }

                        response ??= new StartTransactionResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new StartTransactionResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new StartTransactionResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new StartTransactionResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStartTransactionResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
