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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargePointWSClient : AChargingStationWSClient,
                                               IChargePointWebSocketClient,
                                               IChargePointServer,
                                               IChargePointClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<StopTransactionRequest>?  CustomStopTransactionRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<StopTransactionResponse>?     CustomStopTransactionResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a StopTransaction request will be sent to the CSMS.
        /// </summary>
        public event OnStopTransactionRequestDelegate?     OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a StopTransaction request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?              OnStopTransactionWSRequest;

        /// <summary>
        /// An event fired whenever a response to a StopTransaction request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnStopTransactionWSResponse;

        /// <summary>
        /// An event fired whenever a response to a StopTransaction request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate?    OnStopTransactionResponse;

        #endregion


        #region StopTransaction(Request)

        /// <summary>
        /// Send a stop transaction information.
        /// </summary>
        /// <param name="Request">A StopTransaction request.</param>
        public async Task<StopTransactionResponse>

            StopTransaction(StopTransactionRequest Request)

        {

            #region Send OnStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStopTransactionRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            StopTransactionResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.NetworkingNodeId,
                                               Request.NetworkPath,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomStopTransactionRequestSerializer,
                                                   CustomMeterValueSerializer,
                                                   CustomSampledValueSerializer,
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

                        if (StopTransactionResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var meterValuesResponse,
                                                             out var errorResponse,
                                                             CustomStopTransactionResponseParser) &&
                            meterValuesResponse is not null)
                        {
                            response = meterValuesResponse;
                        }

                        response ??= new StopTransactionResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new StopTransactionResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new StopTransactionResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new StopTransactionResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnStopTransactionResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
