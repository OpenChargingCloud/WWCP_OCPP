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
using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ResetRequest>?  CustomResetRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ResetResponse>?     CustomResetResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a Reset request was sent.
        /// </summary>
        public event OnResetRequestSentDelegate?    OnResetRequestSent;

        #endregion


        #region Reset(Request)

        public async Task<ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequestSent?.Invoke(startTime,
                                       parentNetworkingNode,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnResetRequestSent));
            }

            #endregion


            ResetResponse? response = null;

            try
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

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ResetResponse.TryParse(Request,
                                               sendRequestState.JSONResponse.Payload,
                                               out var resetResponse,
                                               out var errorResponse,
                                               CustomResetResponseParser) &&
                        resetResponse is not null)
                    {
                        response = resetResponse;
                    }

                    response ??= new ResetResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ResetResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ResetResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                await parentNetworkingNode.OCPP.IN.RaiseOnResetResponseReceived(endTime,
                                                                                parentNetworkingNode,
                                                                                Request,
                                                                                response,
                                                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(parentNetworkingNode.OCPP.IN.OnResetResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        public event OnResetResponseReceivedDelegate? OnResetResponseReceived;

        public async Task RaiseOnResetResponseReceived(DateTime        Timestamp,
                                                       IEventSender    Sender,
                                                       ResetRequest    Request,
                                                       ResetResponse   Response,
                                                       TimeSpan        Runtime)
        {

            var requestLogger = OnResetResponseReceived;
            if (requestLogger is not null)
            {

                try
                {
                    await Task.WhenAll(
                              requestLogger.GetInvocationList().
                                            OfType <OnResetResponseReceivedDelegate>().
                                            Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                              Sender,
                                                                                              Request,
                                                                                              Response,
                                                                                              Runtime)).
                                            ToArray()
                          );
                }
                catch (Exception e)
                {
                    await parentNetworkingNode.HandleErrors(
                              nameof(OCPPWebSocketAdapterIN),
                              nameof(OnResetResponseReceived),
                              e
                          );
                }

            }

        }

    }

}
