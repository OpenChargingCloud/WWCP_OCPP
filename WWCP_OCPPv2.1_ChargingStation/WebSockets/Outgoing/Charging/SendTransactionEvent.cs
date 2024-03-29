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

        public CustomJObjectSerializerDelegate<TransactionEventRequest>?  CustomTransactionEventRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<TransactionEventResponse>?     CustomTransactionEventResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a TransactionEvent request will be sent to the CSMS.
        /// </summary>
        public event OnTransactionEventRequestSentDelegate?     OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever a TransactionEvent request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnTransactionEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnTransactionEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OnTransactionEventResponseReceivedDelegate?    OnTransactionEventResponse;

        #endregion


        #region TransactionEvent(Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A TransactionEvent request.</param>
        public async Task<TransactionEventResponse>

            TransactionEvent(TransactionEventRequest  Request)

        {

            #region Send OnTransactionEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTransactionEventRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            TransactionEventResponse? response = null;

            try
            {

                var requestMessage = await SendRequest(
                                               Request.DestinationNodeId,
                                               Request.Action,
                                               Request.RequestId,
                                               Request.ToJSON(
                                                   CustomTransactionEventRequestSerializer,
                                                   CustomTransactionSerializer,
                                                   CustomIdTokenSerializer,
                                                   CustomAdditionalInfoSerializer,
                                                   CustomEVSESerializer,
                                                   CustomMeterValueSerializer,
                                                   CustomSampledValueSerializer,
                                                   CustomSignedMeterValueSerializer,
                                                   CustomUnitsOfMeasureSerializer,
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

                        if (TransactionEventResponse.TryParse(Request,
                                                              sendRequestState.JSONResponse.Payload,
                                                              out var transactionEventResponse,
                                                              out var errorResponse,
                                                              CustomTransactionEventResponseParser) &&
                            transactionEventResponse is not null)
                        {
                            response = transactionEventResponse;
                        }

                        response ??= new TransactionEventResponse(
                                         Request,
                                         Result.Format(errorResponse)
                                     );

                    }

                    response ??= new TransactionEventResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

                response ??= new TransactionEventResponse(
                                 Request,
                                 Result.GenericError(requestMessage.ErrorMessage)
                             );

            }
            catch (Exception e)
            {

                response = new TransactionEventResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnTransactionEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTransactionEventResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
