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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Logging Delegates

    /// <summary>
    /// A delegate called whenever a Reset request was sent.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request logging.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The connection of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="SendMessageResult">The result of the send message process.</param>
    public delegate Task OnResetRequestSentDelegate(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    ResetRequest           Request,
                                                    SentMessageResults     SendMessageResult);


    /// <summary>
    /// A Reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnResetResponseSentDelegate(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    IWebSocketConnection   Connection,
                                    ResetRequest           Request,
                                    ResetResponse          Response,
                                    TimeSpan               Runtime);


    /// <summary>
    /// A logging delegate called whenever a Reset request error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the request error.</param>
    /// <param name="Connection">The connection of the request error.</param>
    /// <param name="Request">The optional request (when parsable).</param>
    /// <param name="RequestErrorMessage">The request error message.</param>
    /// <param name="Runtime">The optional runtime of the request error messag.</param>
    public delegate Task

        OnResetRequestErrorSentDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        IWebSocketConnection           Connection,
                                        ResetRequest?                  Request,
                                        OCPP_JSONRequestErrorMessage   RequestErrorMessage,
                                        TimeSpan?                      Runtime);


    /// <summary>
    /// A logging delegate called whenever a Reset response error was sent.
    /// </summary>
    /// <param name="Timestamp">The logging timestamp.</param>
    /// <param name="Sender">The sender of the response error.</param>
    /// <param name="Connection">The connection of the response error.</param>
    /// <param name="Request">The optional request.</param>
    /// <param name="Response">The optional response.</param>
    /// <param name="ResponseErrorMessage">The response error message.</param>
    /// <param name="Runtime">The optional runtime of the response error message.</param>
    public delegate Task

        OnResetResponseErrorSentDelegate(DateTime                        Timestamp,
                                         IEventSender                    Sender,
                                         IWebSocketConnection            Connection,
                                         ResetRequest?                   Request,
                                         ResetResponse?                  Response,
                                         OCPP_JSONResponseErrorMessage   ResponseErrorMessage,
                                         TimeSpan?                       Runtime);

    #endregion


    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Send Reset request

        /// <summary>
        /// An event fired whenever a Reset request will be sent.
        /// </summary>
        public event OnResetRequestSentDelegate?  OnResetRequestSent;


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
                            parentNetworkingNode.OCPP.CustomResetRequestSerializer,
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
                                                             parentNetworkingNode.OCPP.CustomResetRequestSerializer,
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

        #region Send OnResetResponseSent event

        /// <summary>
        /// An event sent whenever a response to a Reset was sent.
        /// </summary>
        public event OnResetResponseSentDelegate?  OnResetResponseSent;

        public async Task SendOnResetResponseSent(DateTime              Timestamp,
                                                  IEventSender          Sender,
                                                  IWebSocketConnection  Connection,
                                                  ResetRequest          Request,
                                                  ResetResponse         Response,
                                                  TimeSpan              Runtime)
        {

            var logger = OnResetResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnResetResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                             Sender,
                                                                                             Connection,
                                                                                             Request,
                                                                                             Response,
                                                                                             Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnResetResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

        #region Send OnResetRequestErrorSent event

        /// <summary>
        /// An event sent whenever a Reset request error was sent.
        /// </summary>
        public event OnResetRequestErrorSentDelegate? OnResetRequestErrorSent;


        public Task SendOnResetRequestErrorSent(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                ResetRequest?                 Request,
                                                OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                TimeSpan                      Runtime)
            => LogEvent(
                   OnResetRequestErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       RequestErrorMessage,
                       Runtime
                   )
               );

        #endregion

        #region Send OnResetResponseErrorSent event

        /// <summary>
        /// An event sent whenever a Reset response error was sent.
        /// </summary>
        public event OnResetResponseErrorSentDelegate? OnResetResponseErrorSent;


        public Task SendOnResetResponseErrorSent(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 ResetRequest?                  Request,
                                                 ResetResponse?                 Response,
                                                 OCPP_JSONResponseErrorMessage  ResponseErrorMessage,
                                                 TimeSpan                       Runtime)
            => LogEvent(
                   OnResetResponseErrorSent,
                   loggingDelegate => loggingDelegate.Invoke(
                       Timestamp,
                       Sender,
                       Connection,
                       Request,
                       Response,
                       ResponseErrorMessage,
                       Runtime
                   )
               );

        #endregion

    }

}
