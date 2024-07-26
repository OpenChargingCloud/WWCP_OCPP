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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ResetRequest>?  CustomResetRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a Reset request will be sent to the CSMS.
        /// </summary>
        public event OnResetRequestSentDelegate?  OnResetRequestSent;

        #endregion


        #region Reset(Request)

        /// <summary>
        /// Send a Reset request.
        /// </summary>
        /// <param name="Request">A Reset request.</param>
        public async Task<ResetResponse>

            Reset(ResetRequest Request)

        {

            #region Send OnResetRequestSent event

            var logger = OnResetRequestSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                            OfType<OnResetRequestSentDelegate>().
                                            Select(loggingDelegate => loggingDelegate.Invoke(
                                                                          Timestamp.Now,
                                                                          parentNetworkingNode,
                                                                          null,
                                                                          Request,
                                                                          SentMessageResults.Success
                                                                      )).
                                            ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetRequestSent));
                }
            }

            #endregion


            ResetResponse? response = null;

            try
            {

                if (!parentNetworkingNode.OCPP.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            CustomResetRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var signingErrors
                    ))
                {
                    response = new ResetResponse(
                                   Request,
                                   Result.SignatureError(signingErrors)
                               );
                }

                else
                {

                    var sendRequestState = await SendJSONRequestAndWait(
                                                     OCPP_JSONRequestMessage.FromRequest(
                                                         Request,
                                                         Request.ToJSON(
                                                             CustomResetRequestSerializer,
                                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                         )
                                                     )
                                                 );

                    if (sendRequestState.IsValidJSONResponse(Request, out var jsonResponse))
                    {

                        response = await (parentNetworkingNode.OCPP.IN as OCPPWebSocketAdapterIN).Receive_ResetResponse(
                                                                                Request,
                                                                                jsonResponse,
                                                                                null,
                                                                                sendRequestState.DestinationIdReceived,
                                                                                sendRequestState.NetworkPathReceived,
                                                                                Request.         EventTrackingId,
                                                                                Request.         RequestId,
                                                                                sendRequestState.ResponseTimestamp,
                                                                                Request.         CancellationToken
                                                                            );

                    }

                    response ??= new ResetResponse(
                                     Request,
                                     Result.FromSendRequestState(sendRequestState)
                                 );

                }

            }
            catch (Exception e)
            {

                response = new ResetResponse(
                               Request,
                               Result.FromException(e)
                           );

            }

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ResetResponse>?  CustomResetResponseParser    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a Reset response was received.
        /// </summary>
        public event OnResetResponseReceivedDelegate? OnResetResponseReceived;

        #endregion


        #region Receive Reset response (wired via reflection!)

        public async Task<ResetResponse>

            Receive_ResetResponse(ResetRequest          Request,
                                  JObject               ResponseJSON,
                                  IWebSocketConnection  WebSocketConnection,
                                  NetworkingNode_Id     DestinationId,
                                  NetworkPath           NetworkPath,
                                  EventTracking_Id      EventTrackingId,
                                  Request_Id            RequestId,
                                  DateTime?             ResponseTimestamp   = null,
                                  CancellationToken     CancellationToken   = default)

        {

            var response = ResetResponse.Failed(Request);

            try
            {

                if (ResetResponse.TryParse(Request,
                                           ResponseJSON,
                                           DestinationId,
                                           NetworkPath,
                                           out response,
                                           out var errorResponse,
                                           ResponseTimestamp,
                                           CustomResetResponseParser,
                                           parentNetworkingNode.OCPP.CustomStatusInfoParser,
                                           parentNetworkingNode.OCPP.CustomSignatureParser,
                                           parentNetworkingNode.OCPP.CustomCustomDataParser)) {

                    parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomResetResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse
                    );

                    #region Send OnResetResponseReceived event

                    var logger = OnResetResponseReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                    OfType <OnResetResponseReceivedDelegate>().
                                                    Select (loggingDelegate => loggingDelegate.Invoke(
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
                            DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnResetResponseReceived));
                        }
                    }

                    #endregion

                }

                else
                    response = new ResetResponse(
                                   Request,
                                   Result.Format(errorResponse)
                               );

            }
            catch (Exception e)
            {

                response = new ResetResponse(
                                   Request,
                                   Result.FromException(e)
                               );

            }

            return response;

        }

        #endregion


    }

}
