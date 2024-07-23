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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

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

        #region Events

        /// <summary>
        /// An event fired whenever a meter values request will be sent.
        /// </summary>
        public event OnMeterValuesRequestSentDelegate?  OnMeterValuesRequestSent;

        #endregion

        #region MeterValues(Request)

        /// <summary>
        /// Send meter values.
        /// </summary>
        /// <param name="Request">A MeterValues request.</param>
        public async Task<MeterValuesResponse> MeterValues(MeterValuesRequest Request)
        {

            MeterValuesResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                            parentNetworkingNode.OCPP.CustomMeterValueSerializer,
                            parentNetworkingNode.OCPP.CustomSampledValueSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {
                    response = MeterValuesResponse.SignatureError(
                                   Request,
                                   signingErrors
                               );
                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(

                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             parentNetworkingNode.OCPP.CustomMeterValuesRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomMeterValueSerializer,
                                                             parentNetworkingNode.OCPP.CustomSampledValueSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     ),

                                                     async sendMessageResult => {

                                                         #region Send OnMeterValuesRequestSent event

                                                         var logger = OnMeterValuesRequestSent;
                                                         if (logger is not null)
                                                         {
                                                             try
                                                             {

                                                                 await Task.WhenAll(logger.GetInvocationList().
                                                                                         OfType<OnMeterValuesRequestSentDelegate>().
                                                                                         Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                                                       Timestamp.Now,
                                                                                                                       parentNetworkingNode,
                                                                                                                       Request,
                                                                                                                       sendMessageResult
                                                                                                                   )).
                                                                                         ToArray());

                                                             }
                                                             catch (Exception e)
                                                             {
                                                                 DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnMeterValuesRequestSent));
                                                             }
                                                         }

                                                         #endregion

                                                     }

                                                 );

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_MeterValuesResponse(
                                             Request,
                                             jsonResponse,
                                             null,
                                             sendRequestState.DestinationIdReceived,
                                             sendRequestState.NetworkPathReceived,
                                             Request.EventTrackingId,
                                             Request.RequestId,
                                             sendRequestState.ResponseTimestamp,
                                             Request.CancellationToken
                                         );

                    else if (sendRequestState.IsValidJSONRequestError(Request, out var jsonRequestError))
                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_MeterValuesRequestError(
                                             Request,
                                             jsonRequestError,
                                             null
                                         );

                    response ??= new MeterValuesResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = MeterValuesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event fired whenever a MeterValues response was received.
        /// </summary>
        public event OnMeterValuesResponseReceivedDelegate?  OnMeterValuesResponseReceived;

        #endregion

        #region Receive MeterValues response (wired via reflection!)

        public async Task<MeterValuesResponse>

            Receive_MeterValuesResponse(MeterValuesRequest    Request,
                                        JObject               ResponseJSON,
                                        IWebSocketConnection  WebSocketConnection,
                                        NetworkingNode_Id     DestinationId,
                                        NetworkPath           NetworkPath,
                                        EventTracking_Id      EventTrackingId,
                                        Request_Id            RequestId,
                                        DateTime?             ResponseTimestamp   = null,
                                        CancellationToken     CancellationToken   = default)

        {

            var response = MeterValuesResponse.Failed(Request);

            try
            {

                if (MeterValuesResponse.TryParse(Request,
                                                 ResponseJSON,
                                                 DestinationId,
                                                 NetworkPath,
                                                 out response,
                                                 out var errorResponse,
                                                 ResponseTimestamp,
                                                 parentNetworkingNode.OCPP.CustomMeterValuesResponseParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomMeterValuesResponseSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse
                    );

                    #region Send OnMeterValuesResponseReceived event

                    var logger = OnMeterValuesResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                      OfType<OnMeterValuesResponseReceivedDelegate>().
                                                      Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                    Timestamp.Now,
                                                                                    parentNetworkingNode,
                                                                                    //    WebSocketConnection,
                                                                                    Request,
                                                                                    response,
                                                                                    response.Runtime
                                                                                )).
                                                      ToArray());

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnMeterValuesResponseReceived));
                        }
                    }

                    #endregion

                }

                else
                    response = new MeterValuesResponse(
                                   Request,
                                   Result.Format(errorResponse)
                               );

            }
            catch (Exception e)
            {

                response = MeterValuesResponse.ExceptionOccured(
                               Request,
                               e
                           );

            }

            return response;

        }

        #endregion

        #region Receive MeterValuesRequestError

        public async Task<MeterValuesResponse>

            Receive_MeterValuesRequestError(MeterValuesRequest            Request,
                                            OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                            IWebSocketConnection          WebSocketConnection)

        {

            var response = MeterValuesResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomMeterValuesResponseSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
            //        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
            //        parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
            //        parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //        parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //    ),
            //    out errorResponse
            //);

            #region Send OnMeterValuesResponseReceived event

            var logger = OnMeterValuesResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                           OfType<OnMeterValuesResponseReceivedDelegate>().
                                           Select(loggingDelegate => loggingDelegate.Invoke(
                                                                          Timestamp.Now,
                                                                          parentNetworkingNode,
                                                                          //    WebSocketConnection,
                                                                          Request,
                                                                          response,
                                                                          response.Runtime
                                                                      )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnMeterValuesResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

}
