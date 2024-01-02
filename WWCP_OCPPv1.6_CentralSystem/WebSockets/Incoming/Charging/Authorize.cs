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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<AuthorizeRequest>?       CustomAuthorizeRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an Authorize WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                OnAuthorizeWSRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?                  OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeDelegate?                         OnAuthorize;

        /// <summary>
        /// An event sent whenever an Authorize response was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?                 OnAuthorizeResponse;

        /// <summary>
        /// An event sent whenever an Authorize WebSocket response was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?    OnAuthorizeWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_Authorize(DateTime                   RequestTimestamp,
                              WebSocketServerConnection  Connection,
                              NetworkingNode_Id          DestinationNodeId,
                              NetworkPath                NetworkPath,
                              EventTracking_Id           EventTrackingId,
                              Request_Id                 RequestId,
                              JObject                    JSONRequest,
                              CancellationToken          CancellationToken)

        {

            #region Send OnAuthorizeWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeWSRequest?.Invoke(startTime,
                                             this,
                                             Connection,
                                             DestinationNodeId,
                                             EventTrackingId,
                                             RequestTimestamp,
                                             JSONRequest,
                                             CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (AuthorizeRequest.TryParse(JSONRequest,
                                              RequestId,
                                              DestinationNodeId,
                                              NetworkPath,
                                              out var request,
                                              out var errorResponse,
                                              CustomAuthorizeRequestParser) && request is not null) {

                    #region Send OnAuthorizeRequest event

                    try
                    {

                        OnAuthorizeRequest?.Invoke(Timestamp.Now,
                                                   this,
                                                   Connection,
                                                   request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    AuthorizeResponse? response = null;

                    var responseTasks = OnAuthorize?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnAuthorizeDelegate)?.Invoke(Timestamp.Now,
                                                                                                                 this,
                                                                                                                 Connection,
                                                                                                                 request,
                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= AuthorizeResponse.Failed(request);

                    #endregion

                    #region Send OnAuthorizeResponse event

                    try
                    {

                        OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                    this,
                                                    Connection,
                                                    request,
                                                    response,
                                                    response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeResponse));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath,
                                       RequestId,
                                       response.ToJSON(
                                           CustomAuthorizeResponseSerializer,
                                           CustomIdTagInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_Authorize)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_Authorize)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnAuthorizeWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnAuthorizeWSResponse?.Invoke(endTime,
                                              this,
                                              Connection,
                                              DestinationNodeId,
                                              EventTrackingId,
                                              RequestTimestamp,
                                              JSONRequest,
                                              endTime, //ToDo: Refactor me!
                                              OCPPResponse?.Payload,
                                              OCPPErrorResponse?.ToJSON(),
                                              endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAuthorizeWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
